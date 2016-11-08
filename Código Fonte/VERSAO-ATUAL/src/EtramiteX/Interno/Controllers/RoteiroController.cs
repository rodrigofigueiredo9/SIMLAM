using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRoteiro.Pdf.RoteiroPdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro;
using NameRoteiroRelatorio = Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class RoteiroController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		RoteiroBus _bus = new RoteiroBus(new RoteiroValidar());
		RoteiroValidar _validar = new RoteiroValidar();
		ItemValidar _validarItem = new ItemValidar();
		FuncionarioBus _busFuncionario = new FuncionarioBus();

		#endregion

		#region Listar/Associar

		[Permite(RoleArray = new Object[] { ePermissao.RoteiroListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.Setores, _busLista.QuantPaginacao, _busLista.AtividadesSolicitada);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View("Index", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RoteiroListar })]
		public ActionResult Associar()
		{
			ListarVM vm = new ListarVM(_busLista.Setores, _busLista.QuantPaginacao, _busLista.AtividadesSolicitada);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return PartialView("ListarFiltros", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RoteiroCriar, ePermissao.RoteiroEditar})]
		public ActionResult ValidarAssociarAtividade(int roteiroSetor, int atividadeId)
		{
			_bus.ValidarAssociarAtividade(roteiroSetor, atividadeId);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, JsonRequestBehavior.AllowGet });
		}

		[Permite(RoleArray = new Object[] { ePermissao.RoteiroListar })]
		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<Roteiro> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = User.IsInRole(ePermissao.RoteiroEditar.ToString());
				vm.PodeDesativar = User.IsInRole(ePermissao.RoteiroDesativar.ToString());
				vm.MostrarRelatorio = true;
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.RoteiroVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.RoteiroCriar })]
		public ActionResult Criar()
		{
			SalvarVM vm = new SalvarVM(_busFuncionario.ObterSetoresFuncionario(), _busLista.Finalidades);
			if (!string.IsNullOrEmpty(Request.QueryString["idPdf"]))
			{
				vm.IdPdf = Convert.ToInt32(Request.QueryString["idPdf"]);
			}

			return View("Criar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RoteiroCriar })]
		public ActionResult Criar(SalvarVM vm)
		{
			if (vm.Roteiro.Anexos != null && vm.Roteiro.Anexos.Count > 0)
			{
				foreach (Anexo anexo in vm.Roteiro.Anexos)
				{
					anexo.Arquivo = ViewModelHelper.JsSerializer.Deserialize<Arquivo>(anexo.ArquivoJson);
				}
			}

			_bus.Salvar(vm.Roteiro);
			string urlRetorno = Url.Action("Criar", "Roteiro");
			urlRetorno += "?Msg=" + Validacao.QueryParam() + "&acaoId=" + vm.Roteiro.Id.ToString();

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @urlRetorno = urlRetorno }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RoteiroCriar })]
		public ActionResult CopiarRoteiro(int id)
		{
			SalvarVM vm = new SalvarVM(_busLista.Setores, _busLista.Finalidades);
			vm.Roteiro = _bus.Obter(id) ?? new Roteiro();
			vm.Roteiro.Padrao = false;

			preencheSalvarVM(vm);
			return PartialView("RoteiroContentPartial", vm);
		}	

		public ActionResult ObterModelosAtividades(List<AtividadeSolicitada> atividades)
		{
			List<TituloModeloLst> lista = _bus.ObterModelosAtividades(atividades);
			_validar.PossuiModelosAtividades(lista);

			return Json(new { @Lista = lista, @Msg = Validacao.Erros });
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.RoteiroEditar })]
		public ActionResult Editar(int id)
		{
			SalvarVM vm = new SalvarVM(_busLista.Setores, _busLista.Finalidades);
			
			vm.Roteiro = _bus.Obter(id);

			_validar.PossuiModelosAtividades(vm.Roteiro.ModelosAtuais, id);
			
			if (!_validar.AbrirEditar(vm.Roteiro))
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			if (vm.Roteiro.Id > 0)
			{
				preencheSalvarVM(vm);
				return View("Editar", vm);
			}
			else
			{
				Validacao.Add(Mensagem.Roteiro.NaoEncontrouRegistros);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RoteiroEditar })]
		public ActionResult Editar(SalvarVM vm)
		{
			preencheSalvarVM(vm);
			_bus.Salvar(vm.Roteiro);

			string urlRetorno = Url.Action("Index", "Roteiro");
			urlRetorno += "?Msg=" + Validacao.QueryParam() + "&acaoId=" + vm.Roteiro.Id.ToString();

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @urlRetorno = urlRetorno }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RoteiroEditar })]
		public ActionResult ConfirmarEditar(int id)
		{
			Roteiro roteiro = _bus.ObterSimplificado(id);
			return View(roteiro);
		}
		
		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.RoteiroVisualizar })]
		public ActionResult Visualizar(int id, string tid = null)
		{
			SalvarVM vm = new SalvarVM(_busLista.Setores, _bus.Obter(id, tid), _busLista.Finalidades);

			if (vm.Roteiro == null || vm.Roteiro.Id <= 0)
			{
				Validacao.Add(Mensagem.Roteiro.NaoEncontrado);
			}
			else
			{
				vm.Roteiro.Padrao = _busLista.RoteiroPadrao.Count>0;
				vm.IsVisualizar = true;
			}

			_validar.PossuiModelosAtividades(vm.Roteiro.ModelosAtuais, id);

			if (Request.IsAjaxRequest())
			{
				return PartialView("VisualizarPartial", vm);
			}
			return View("Visualizar", vm);
		}

		#endregion

		#region Desativar

		[Permite(RoleArray = new Object[] { ePermissao.RoteiroDesativar })]
		public ActionResult DesativarConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();
			Roteiro roteiro = _bus.ObterSimplificado(id);
			vm.Id = id;
			vm.Mensagem = Mensagem.Roteiro.DesativarConfirm(roteiro.Nome);
			vm.Titulo = "Desativar Roteiro";
			return PartialView("Confirmar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RoteiroDesativar })]
		public ActionResult Desativar(int id)
		{
			_bus.AlterarSituacao(id, 2);// Desativar
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Relatório de Roteiro

		public ActionResult RelatorioRoteiro(int id, string tid= "")
		{
			try
			{
				PdfRoteiroOrientativo pdfRoteiro = new PdfRoteiroOrientativo();
				return ViewModelHelper.GerarArquivoPdf(pdfRoteiro.Gerar(id, tid), "Relatorio de Roteiro Orientativo");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		#endregion

		#region Baixar Arquivo

		[Permite(RoleArray = new Object[] { ePermissao.RoteiroCriar, ePermissao.RoteiroEditar, ePermissao.RoteiroVisualizar })]
		public FileResult Baixar(int id)
		{
			return ViewModelHelper.BaixarArquivo(id);
		}

		#endregion

		#region Auxiliares

		private void preencheSalvarVM(SalvarVM vm)
		{
			vm.Setores = ViewModelHelper.CriarSelectList(_busLista.Setores, true);

			if (vm.Roteiro != null)
			{
				if (vm.Roteiro.Anexos != null && vm.Roteiro.Anexos.Count > 0)
				{
					foreach (Anexo anexo in vm.Roteiro.Anexos)
					{
						if (anexo.Arquivo.Id > 0)
						{
							anexo.ArquivoJson = ViewModelHelper.JsSerializer.Serialize(anexo.Arquivo);
						}
						else
						{
							if (!String.IsNullOrEmpty(anexo.ArquivoJson) && anexo.Arquivo.ContentType == null)
							{
								anexo.Arquivo = ViewModelHelper.JsSerializer.Deserialize<Arquivo>(anexo.ArquivoJson);
							}
							else if (anexo.Arquivo.ContentType != null && String.IsNullOrEmpty(anexo.ArquivoJson))
							{
								anexo.ArquivoJson = ViewModelHelper.JsSerializer.Serialize(anexo.Arquivo);
							}
						}
					}
				}
				else
				{
					vm.Roteiro.Anexos = new List<Anexo>();
				}
			}

			if (vm.Roteiro.Id > 0)
			{
				vm.Item.Tipo = 1;
			}
		}

		public ActionResult ValidarDesativarRoteiro(int id)
		{
			_validar.ValidarDesativarRoteiro(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		public ActionResult ValidarRoteiroDesativado(int id)
		{
			_validar.ValidarRoteiroDesativado(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		public ActionResult ValidarItemAbrirEditar(int id)
		{
			_validarItem.Editar(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		#endregion

		#region Ações de Item

		#region Filtrar/Associar

		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroListar })]
		public ActionResult IndexItem()
		{
			ListarItemVM vm = new ListarItemVM(_busLista.ItemTipos, _busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroListar })]
		public ActionResult ListarItem()
		{
			ListarItemVM vm = new ListarItemVM(_busLista.ItemTipos, _busLista.QuantPaginacao);
			vm.PodeCadastrar = User.IsInRole(ePermissao.ItemRoteiroCriar.ToString());
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return PartialView("ListarFiltroItem", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroListar })]
		public ActionResult FiltrarItem(ListarItemVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarItemVM>(vm.UltimaBusca).Filtros;
			}
			
			vm.Filtros.TiposPermitidos.Add((int)eRoteiroItemTipo.Tecnico);
			vm.Filtros.TiposPermitidos.Add((int)eRoteiroItemTipo.Administrativo);

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<Item> resultados = _bus.FiltrarItem(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeExcluir = User.IsInRole(ePermissao.ItemRoteiroExcluir.ToString());
			vm.PodeEditar = User.IsInRole(ePermissao.ItemRoteiroEditar.ToString());
			vm.PodeVisualizar = !vm.PodeAssociar && User.IsInRole(ePermissao.ItemRoteiroVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultadosItem", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar/Editar

		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroCriar })]
		public ActionResult CriarItem()
		{
			ItemRoteiroVM vm = new ItemRoteiroVM();
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroEditar })]
		public ActionResult EditarItem(int id)
		{
			if (!_validarItem.Editar(id))
			{
				return RedirectToAction("IndexItem", "Roteiro", Validacao.QueryParamSerializer());
			}

			ItemRoteiroVM vm = new ItemRoteiroVM(_bus.ObterItem(id));
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroCriar, ePermissao.ItemRoteiroEditar })]
		public ActionResult SalvarItem(ItemRoteiroVM vm)
		{
			string urlRetorno = string.Empty;
			if (vm.ItemRoteiro.Id > 0)
			{
				urlRetorno = Url.Action("IndexItem", "Roteiro");
			}
			else
			{
				urlRetorno = Url.Action("CriarItem", "Roteiro");
			}

			_bus.SalvarItem(vm.ItemRoteiro);
			urlRetorno += "?Msg=" + Validacao.QueryParam();

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @urlRetorno = urlRetorno }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroCriar, ePermissao.ItemRoteiroEditar })]
		public ActionResult ItemModal(int? id)
		{
			ItemRoteiroVM vm = new ItemRoteiroVM();

			if (id != null)
			{
				vm = new ItemRoteiroVM(_bus.ObterItem((int)id));
			}

			vm.IsModal = true;
			return PartialView("ItemAdicionar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroCriar, ePermissao.ItemRoteiroEditar })]
		public ActionResult SalvarItemModal(ItemRoteiroVM vm)
		{
			_bus.SalvarItem(vm.ItemRoteiro);
			vm.ItemRoteiro.TipoTexto = vm.ItemRoteiro.Tipo == (int)eRoteiroItemTipo.Tecnico ? "Técnico" : "Administrativo";
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @item = vm.ItemRoteiro }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroVisualizar })]
		public ActionResult ItemVisualizar(int id)
		{
			if (!_validarItem.Visualizar(id))
			{
				return RedirectToAction("IndexItem", "Roteiro", Validacao.QueryParamSerializer());
			}
			ItemRoteiroVM vm = new ItemRoteiroVM(_bus.ObterItem(id));

			return View(vm);
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroExcluir })]
		public ActionResult ItemComfirmExcluir(int id)
		{
			ExcluirVM vm = new ExcluirVM();

			Item item = _bus.ObterItem(id);
			vm.Id = id;
			vm.Mensagem = Mensagem.Item.MensagemExcluir(item.Nome);
			vm.Titulo = "Excluir Item de Roteiro";
			return PartialView("Excluir", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroExcluir })]
		public ActionResult ExcluirItem(int id)
		{
			_bus.ExcluirItem(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroEditar })]
		public ActionResult ItemConfirmar(int id)
		{
			ItemConfirmarVM vmC = new ItemConfirmarVM();

			vmC.CarregarMensagem(_bus.ObterRoteiros(new Item { Id = id }));

			return PartialView("ItemConfirmar", vmC);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ItemRoteiroEditar })]
		public ActionResult AtualizarRoteiro(int id)
		{
			int atualiza = _bus.ObterRoteiros(new Item { Id = id }).Count;
			return Json(new { @Msg = Validacao.Erros, @atualiza = atualiza }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}
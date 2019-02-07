using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloRelatorioTecnico.Pdf;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMRequerimento;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class ProjetoDigitalController : DefaultController
	{
		#region Propriedade

		ProjetoDigitalCredenciadoBus _bus = new ProjetoDigitalCredenciadoBus();
		ProjetoDigitalCredenciadoValidar _validar = new ProjetoDigitalCredenciadoValidar();

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalListar })]
		public ActionResult Index()
		{
			ProjetoDigitalListarVM vm = new ProjetoDigitalListarVM(ListaCredenciadoBus.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalListar })]
		public ActionResult Filtrar(ProjetoDigitalListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ProjetoDigitalListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			vm.Filtros.Credenciado = _bus.User.FuncionarioId;
			Resultados<ProjetoDigital> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeEditar = User.IsInRole(ePermissao.ProjetoDigitalEditar.ToString());
			vm.PodeVisualizar = User.IsInRole(ePermissao.ProjetoDigitalVisualizar.ToString());
			vm.PodeExcluir = User.IsInRole(ePermissao.ProjetoDigitalExcluir.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { Msg = Validacao.Erros, Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalListar })]
		public ActionResult Associar()
		{
			ProjetoDigitalListarVM vm = new ProjetoDigitalListarVM(ListaCredenciadoBus.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.PodeAssociar = true;

			return PartialView("ListarFiltros", vm);
		}

		#endregion

		#region Criar/Editar - Operar

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalCriar, ePermissao.ProjetoDigitalEditar })]
		public ActionResult Operar(int id = 0, bool modoVisualizar = false, int acaoId = 0)
		{
			ProjetoDigitalVM vm = new ProjetoDigitalVM();
			vm.ProjetoDigital.Situacao = (int)eProjetoDigitalSituacao.EmElaboracao;
			vm.ModoVisualizar = modoVisualizar;
			vm.UrlRequerimento = Url.Action("Criar", "Requerimento");

			if (acaoId > 0)
			{
				vm.PossuiAtividadeCAR = _bus.PossuiAtividadeCAR(acaoId);
			}

			if (id > 0)
			{
				vm.ProjetoDigital = _bus.Obter(id);

				RequerimentoCredenciadoBus requerimentoBus = new RequerimentoCredenciadoBus();
				vm.DesativarPasso4 = requerimentoBus.RequerimentoDeclaratorio(vm.ProjetoDigital.RequerimentoId);

				if (!_validar.EmPosseCredenciado(vm.ProjetoDigital))
				{
					Validacao.Add(Mensagem.ProjetoDigital.PosseCredenciado);
					return RedirectToAction("Index", Validacao.QueryParamSerializer());
				}

				vm.UrlRequerimento = Url.Action("Editar", "Requerimento", new { id = vm.ProjetoDigital.RequerimentoId, projetoDigitalId = vm.ProjetoDigital.Id });
				vm.UrlRequerimentoVisualizar = Url.Action("Visualizar", "Requerimento", new { id = vm.ProjetoDigital.RequerimentoId, projetoDigitalId = vm.ProjetoDigital.Id, isVisualizar = true });

				vm.UrlCaracterizacao = Url.Action("Index", "Caracterizacao", new { area = "Caracterizacoes", id = vm.ProjetoDigital.EmpreendimentoId.GetValueOrDefault(), projetoDigitalId = vm.ProjetoDigital.Id });
				vm.UrlCaracterizacaoVisualizar = Url.Action("Index", "Caracterizacao", new { area = "Caracterizacoes", id = vm.ProjetoDigital.EmpreendimentoId.GetValueOrDefault(), projetoDigitalId = vm.ProjetoDigital.Id, visualizar = true });

				vm.UrlEnviar = Url.Action("Enviar", "ProjetoDigital", new { id = vm.ProjetoDigital.Id });
				vm.UrlEnviarVisualizar = Url.Action("Enviar", "ProjetoDigital", new { id = vm.ProjetoDigital.Id, modoVisualizar = true });

				vm.UrlImprimirDocumentos = Url.Action("ImprimirDocumentos", "ProjetoDigital", new { id = vm.ProjetoDigital.Id });
			}

			return View(vm);
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ProjetoDigital projetoDigital = _bus.Obter(id);
			ConfirmarVM vm = new ConfirmarVM();

			vm.Id = id;
			vm.Mensagem = Mensagem.ProjetoDigital.MensagemExcluir(projetoDigital.RequerimentoId.ToString());
			vm.Titulo = "Excluir Projeto Digital";
			return PartialView("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Enviar

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalEditar })]
		public ActionResult Enviar(int id, bool modoVisualizar = false)
		{
			ProjetoDigital projetoDigital = _bus.Obter(id);

			if (!modoVisualizar && !_validar.EnviarBasicas(projetoDigital, true))
			{
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = id }));
			}

			RequerimentoCredenciadoBus requerimentoCredenciadoBus = new RequerimentoCredenciadoBus();
			Requerimento requerimento = requerimentoCredenciadoBus.ObterFinalizar(projetoDigital.RequerimentoId);

			ProjetoDigitalEnviarVM vm = new ProjetoDigitalEnviarVM()
			{
				ModoVisualizar = modoVisualizar,
				ProjetoDigital = projetoDigital,
				RequerimentoVM = new RequerimentoVM(requerimento, true)
			};

			ProjetoGeograficoBus projetoGeoBus = new ProjetoGeograficoBus();
			foreach (var item in projetoDigital.Dependencias)
			{
				if (item.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico)
				{
					List<ArquivoProjeto> arquivos = projetoGeoBus.ObterArquivosHistorico(item.DependenciaId, item.DependenciaTid);
					item.Id = (arquivos.SingleOrDefault(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui) ?? new ArquivoProjeto()).Id.GetValueOrDefault();
				}
				else
				{
					item.Id = 0;
				}
			}

			vm.RequerimentoVM.IsAbaFinalizar = true;
			vm.RequerimentoVM.CarregarListas(ListaCredenciadoBus.ResponsavelFuncoes, new List<AgendamentoVistoria>());
			vm.DocumentosGeradosVM.CarregarDocumentos(requerimento, projetoDigital);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalEditar })]
		public ActionResult Enviar(int id)
		{
			string urlRedirecionar = string.Empty;
			ProjetoDigital projeto = _bus.Obter(id);
			_bus.Enviar(projeto);

			if (Validacao.EhValido)
			{
				if (projeto.Dependencias.Exists(x => x.DependenciaCaracterizacao == (int)eCaracterizacao.InformacaoCorte))
					urlRedirecionar = Url.Action("Criar", "TituloDeclaratorio");
				else
				{
					RequerimentoCredenciadoBus requerimentoBus = new RequerimentoCredenciadoBus();
					if (requerimentoBus.RequerimentoDeclaratorio(projeto.RequerimentoId))
						urlRedirecionar = Url.Action("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { Id = id }));
					else
						urlRedirecionar = Url.Action("ImprimirDocumentos", "ProjetoDigital", Validacao.QueryParamSerializer(new { Id = id, MostrarFechar = true }));
				}
			}

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, UrlRedirecionar = urlRedirecionar }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Imprimir Documentos

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalEditar })]
		public ActionResult ImprimirDocumentos(int id)
		{
			ProjetoDigital projetoDigital = _bus.Obter(id);

			RequerimentoCredenciadoBus requerimentoBus = new RequerimentoCredenciadoBus();
			if(requerimentoBus.RequerimentoDeclaratorio(projetoDigital.RequerimentoId))
			{
				Validacao.Add(Mensagem.ProjetoDigital.ImprimirDocumentosDesativado);
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = id }));
			}

			if (projetoDigital.Situacao == (int)eProjetoDigitalSituacao.AguardandoCorrecao)
			{
				Validacao.Add(Mensagem.ProjetoDigital.PossuiPendenciasCorrecao);
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = id }));
			}

			RequerimentoCredenciadoBus requerimentoCredenciadoBus = new RequerimentoCredenciadoBus();
			Requerimento requerimento = requerimentoCredenciadoBus.ObterFinalizar(projetoDigital.RequerimentoId);

			ProjetoDigitalImprimirDocumentosVM vm = new ProjetoDigitalImprimirDocumentosVM();
			vm.ProjetoDigital = projetoDigital;

			ProjetoGeograficoBus projetoGeoBus = new ProjetoGeograficoBus();
			foreach (var item in projetoDigital.Dependencias)
			{
				if (item.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico)
				{
					List<ArquivoProjeto> arquivos = projetoGeoBus.ObterArquivosHistorico(item.DependenciaId, item.DependenciaTid);
					item.Id = (arquivos.SingleOrDefault(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui) ?? new ArquivoProjeto()).Id.GetValueOrDefault();
				}
				else
				{
					item.Id = 0;
				}
			}

			vm.DocumentosGeradosVM.CarregarDocumentos(requerimento, projetoDigital);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalEditar })]
		public ActionResult ImprimirDocumentos(ProjetoDigital projeto)
		{
			_bus.ImprimirDocumentos(projeto);

			string urlRedirecionar = Url.Action("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { acaoId = projeto.Id }));
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, UrlRedirecionar = urlRedirecionar }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Cancelar Envio

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalEditar })]
		public ActionResult CancelarEnvioConfirm(int id)
		{
			ProjetoDigital projetoDigital = _bus.Obter(id);
			ConfirmarVM vm = new ConfirmarVM();

			if (_bus.PossuiAtividadeCAR(projetoDigital.Id) && _bus.PossuiSolicitacaoCAR(projetoDigital.Id))
			{
				vm.Id = id;
				vm.Mensagem = Mensagem.ProjetoDigital.MensagemCancelarEnvioCAR();
				vm.Titulo = "Confirmação do cancelamento";
				return PartialView("Confirmar", vm);
			}
			else
			{
				vm.Id = id;
				vm.Mensagem = Mensagem.ProjetoDigital.MensagemCancelarEnvio(projetoDigital.RequerimentoId.ToString());
				vm.Titulo = "Confirmação do cancelamento";
				return PartialView("Confirmar", vm);
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalEditar })]
		public ActionResult CancelarEnvio(int id)
		{
			_bus.CancelarEnvio(id);
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Documentos Gerados

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalListar })]
		public ActionResult DocumentosGerados(int id)
		{
			ProjetoDigital projetoDigital = _bus.Obter(id);
			RequerimentoCredenciadoBus requerimentoCredenciadoBus = new RequerimentoCredenciadoBus();
			Requerimento requerimento = requerimentoCredenciadoBus.Obter(projetoDigital.RequerimentoId);

			ProjetoGeograficoBus projetoGeoBus = new ProjetoGeograficoBus();
			foreach (var item in projetoDigital.Dependencias)
			{
				if (item.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico)
				{
					List<ArquivoProjeto> arquivos = projetoGeoBus.ObterArquivosHistorico(item.DependenciaId, item.DependenciaTid);
					item.Id = (arquivos.SingleOrDefault(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui) ?? new ArquivoProjeto()).Id.GetValueOrDefault();
				}
				else
				{
					item.Id = 0;
				}
			}

			DocumentosGeradosVM vm = new DocumentosGeradosVM();
			vm.MostrarTitulo = true;
			vm.CarregarDocumentos(requerimento, projetoDigital);

			return PartialView("DocumentosGerados", vm);
		}

		#endregion

		#region Notificação para correção

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalListar })]
		public ActionResult NotificacaoCorrecao(int id)
		{
			ProjetoDigital projetoDigital = _bus.Obter(id);
			ProjetoDigitalVM vm = new ProjetoDigitalVM();

			vm.ProjetoDigital = projetoDigital;

			return PartialView("NotificacaoCorrecao", vm);
		}

		#endregion

		#region Metodos Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalEditar })]
		public ActionResult EditarRequerimentoValidarConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();

			if (!_validar.PodeEditarRequerimento(id))
			{
				vm.Mensagem = Mensagem.ProjetoDigital.EditarRequerimentoValidar;
				vm.Titulo = "Editar Requerimento Digital";
				return PartialView("Confirmar", vm);
			}

			return null;
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalEditar })]
		public ActionResult EditarCaracterizacaoValidarConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();

			if (!_validar.PodeEditarCaracterizacao(id))
			{
				vm.Mensagem = Mensagem.ProjetoDigital.EditarCaracterizacaoValidar;
				vm.Titulo = "Editar Caracterização";
				return PartialView("Confirmar", vm);
			}

			return null;
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalEditar })]
		public ActionResult AlterarDados(int id)
		{
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoDigitalListar, ePermissao.ProjetoDigitalEditar })]
		public ActionResult GerarPdfRelatorioTecnico(int id, int caracterizacaoTipo)
		{
			try
			{
				PdfRelatorioTecnicoCredenciado pdf = new PdfRelatorioTecnicoCredenciado();
				return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, caracterizacaoTipo), "Relatorio Tecnico");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "ProjetoDigital", Validacao.QueryParamSerializer());
			}
		}

		#endregion
	}
}
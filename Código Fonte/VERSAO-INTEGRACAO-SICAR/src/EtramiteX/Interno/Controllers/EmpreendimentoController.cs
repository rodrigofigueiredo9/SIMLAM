using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class EmpreendimentoController : DefaultController
	{
		#region Propriedades

		EmpreendimentoValidar _validar = new EmpreendimentoValidar();
		ListaBus _busLista = new ListaBus();
		EmpreendimentoBus _bus = new EmpreendimentoBus(new EmpreendimentoValidar());
		CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();

		#endregion

		#region Ações

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_bus.Atividades, _busLista.Segmentos, _busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return PartialView(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoListar })]
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

			Resultados<Empreendimento> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = User.IsInRole(ePermissao.EmpreendimentoEditar.ToString());
				vm.PodeExcluir = User.IsInRole(ePermissao.EmpreendimentoExcluir.ToString());

				vm.PodeCaracterizar = caracterizacaoBus.ValidarAcessarTela(mostrarMensagem: false);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.EmpreendimentoVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoListar })]
		public ActionResult Associar()
		{
			ListarVM vm = new ListarVM();
			vm.SelListSegmentos = ViewModelHelper.CriarSelectList(_busLista.Segmentos);
			return PartialView("ListarFiltros", vm);
		}

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar })]
		public ActionResult Criar()
		{
			EmpreendimentoVM vm = new EmpreendimentoVM(_busLista.Estados, _busLista.Municipios(_busLista.EstadoDefault),
				_busLista.Segmentos, _busLista.TiposCoordenada, _busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busLista.TiposResponsavel);

			return View("Criar", vm);
		}

		//Usado em outras controllers
		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar, ePermissao.EmpreendimentoVisualizar, ePermissao.EmpreendimentoEditar })]
		public ActionResult EmpreendimentoInline(int id, string tid = null)
		{
			EmpreendimentoVM vm = new EmpreendimentoVM();

			if (id > 0)
			{
				Empreendimento emp = null;
				Boolean IsVisualizarHistorico = false;

				if (String.IsNullOrWhiteSpace(tid))
				{
					emp = _bus.Obter(id);
				}
				else
				{
					emp = _bus.ObterHistorico(id, tid);
					IsVisualizarHistorico = true;
				}

				if (emp.Enderecos.Count == 0)
				{
					emp.Enderecos.Add(new Endereco());
					emp.Enderecos.Add(new Endereco());
				}
				else if (emp.Enderecos.Count == 1)
				{
					emp.Enderecos.Add(new Endereco());
				}

				SalvarVM salvarVM = new SalvarVM(_busLista.Estados, _busLista.Municipios(emp.Enderecos[0].EstadoId), _busLista.Municipios(emp.Enderecos[1].EstadoId), _busLista.Segmentos, _busLista.TiposCoordenada,
					_busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busLista.TiposResponsavel, _busLista.LocalColetaPonto, _busLista.FormaColetaPonto,
					emp.Enderecos[0].EstadoId, emp.Enderecos[0].MunicipioId, emp.Enderecos[1].EstadoId, emp.Enderecos[1].MunicipioId, emp.Coordenada.LocalColeta.GetValueOrDefault(), emp.Coordenada.FormaColeta.GetValueOrDefault());

				vm.SalvarVM = salvarVM;
				vm.SalvarVM.Empreendimento = emp;
				vm.SalvarVM.MostrarTituloTela = false;
				vm.SalvarVM.IsVisualizar = true;
				vm.SalvarVM.IsVisualizarHistorico = IsVisualizarHistorico;
				PreencherSalvar(vm.SalvarVM);
			}
			else
			{
				vm = new EmpreendimentoVM(_busLista.Estados, _busLista.Municipios(_busLista.EstadoDefault),
				_busLista.Segmentos, _busLista.TiposCoordenada, _busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busLista.TiposResponsavel);
			}

			return PartialView("EmpreendimentoInline", vm);
		}

		#region Localizar

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar })]
		public ActionResult LocalizarFiscalizacao(LocalizarVM vm)
		{
			_bus.ValidarLocalizarFiscalizacao(vm.Filtros);

			if (!Validacao.EhValido)
			{
				return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				vm.Paginacao = new Paginacao();
				vm.Paginacao.QuantPaginacao = Convert.ToInt32(Int32.MaxValue);
				vm.Paginacao.OrdenarPor = 1;

				Resultados<Empreendimento> retorno = _bus.Filtrar(vm.Filtros, vm.Paginacao);
				if (retorno == null)
				{
					return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
				}

				vm.PodeEditar = User.IsInRole(ePermissao.EmpreendimentoEditar.ToString());
				vm.PodeVisualizar = User.IsInRole(ePermissao.EmpreendimentoVisualizar.ToString());

				vm.Paginacao.EfetuarPaginacao();
				vm.SetResultados(retorno.Itens);

				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultadosLocalizar", vm) }, JsonRequestBehavior.AllowGet);
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar })]
		public ActionResult Localizar(LocalizarVM vm)
		{
			_bus.ValidarLocalizar(vm.Filtros);

			if (!Validacao.EhValido)
			{
				return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				vm.Paginacao = new Paginacao();
				vm.Paginacao.QuantPaginacao = Convert.ToInt32(Int32.MaxValue);
				vm.Paginacao.OrdenarPor = 1;

				Resultados<Empreendimento> retorno = _bus.Filtrar(vm.Filtros, vm.Paginacao, false);
				if (retorno == null)
				{
					return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
				}

				_validar.VerificarCodigo(vm.Filtros, retorno);

				vm.PodeEditar = User.IsInRole(ePermissao.EmpreendimentoEditar.ToString());
				vm.PodeVisualizar = User.IsInRole(ePermissao.EmpreendimentoVisualizar.ToString());

				vm.Paginacao.EfetuarPaginacao();
				vm.SetResultados(retorno.Itens);

				return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultadosLocalizar", vm) }, JsonRequestBehavior.AllowGet);
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar })]
		public ActionResult LocalizarMontar(LocalizarVM localizarVm)
		{
			LocalizarVM vm = new LocalizarVM(_busLista.Estados, _busLista.Municipios(localizarVm.Filtros.EstadoId.GetValueOrDefault()),
				_busLista.Segmentos, _busLista.TiposCoordenada, _busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios);

			vm.Filtros = localizarVm.Filtros;

			if (Request.IsAjaxRequest())
			{
				return PartialView("Localizar", vm);
			}
			else
			{
				return View("Localizar", vm);
			}
		}

		#endregion

		#region Cadastrar

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar })]
		public ActionResult Salvar(LocalizarVM localizarVm)
		{
			#region Configurar Estado e Municipio

			Municipio municipio = null;
			List<Estado> lstEstados = new List<Estado>();
			List<Municipio> lstMunicipios = new List<Municipio>();
			ResponseJsonData<dynamic> resposta = new ResponseJsonData<dynamic>();
			resposta = _bus.ObterEstadosMunicipiosPorCoordenada(localizarVm.Filtros.Coordenada.EastingUtmTexto, localizarVm.Filtros.Coordenada.NorthingUtmTexto);

			var objJson = resposta.Data;
			int codigoIbge = 0;
			if (objJson["Municipio"] != null)
			{
				codigoIbge = Convert.ToInt32(objJson["Municipio"]["IBGE"] ?? 0);
			}

			ListaValoresDa _da = new ListaValoresDa();
			municipio = _da.ObterMunicipio(codigoIbge);

			if (municipio.Estado.Sigla != ViewModelHelper.EstadoDefaultSigla())
			{
				lstEstados = _busLista.Estados.Where(x => x.Texto != ViewModelHelper.EstadoDefaultSigla()).ToList();
				lstMunicipios = new List<Municipio>();
			}
			else
			{
				lstEstados = _busLista.Estados;
				lstMunicipios = _busLista.Municipios(municipio.Estado.Id);

				localizarVm.Filtros.EstadoId = municipio.Estado.Id;
				localizarVm.Filtros.MunicipioId = municipio.Id;
			}

			#endregion

			SalvarVM vm = new SalvarVM(lstEstados, lstMunicipios, lstMunicipios, _busLista.Segmentos, _busLista.TiposCoordenada,
				_busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busLista.TiposResponsavel, _busLista.LocalColetaPonto, _busLista.FormaColetaPonto,
				localizarVm.Filtros.EstadoId, localizarVm.Filtros.MunicipioId, 0, 0);

			vm.SetLocalizarVm(localizarVm.Filtros);
			vm.SetCoordenada();
			PreencherSalvar(vm);

			if (vm.Empreendimento.Responsaveis != null && vm.Empreendimento.Responsaveis.Count > 0)
			{
				vm.Responsaveis = ViewModelHelper.CriarSelectList(ObterResponsaveisList(vm.Empreendimento.Responsaveis), true);
			}

			if (vm.Empreendimento.Enderecos.Count == 0)
			{
				vm.Empreendimento.Enderecos.Add(new Endereco());
				vm.Empreendimento.Enderecos.Add(new Endereco());
			}
			else if (vm.Empreendimento.Enderecos.Count == 1)
			{
				vm.Empreendimento.Enderecos.Add(new Endereco());
			}

			if (Request.IsAjaxRequest())
			{
				vm.IsAjaxRequest = true;
				return PartialView("Salvar", vm);
			}
			else
			{
				return View("Salvar", vm);
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar })]
		public ActionResult SalvarCadastrar(SalvarVM vm)
		{
			vm.SetCoordenada();
			PreencherSalvar(vm);
			if (_bus.Salvar(vm.Empreendimento))
			{
				Validacao.Add(Mensagem.Empreendimento.Salvar);
				string urlRedireciona = Url.Action("Criar", "Empreendimento");
				urlRedireciona += "?Msg=" + Validacao.QueryParam();

				return Json(new { IsEmpreendimentoSalvo = Validacao.EhValido, UrlRedireciona = urlRedireciona, @Empreendimento = vm.Empreendimento, Msg = Validacao.Erros });
			}
			else
			{
				return Json(new { IsEmpreendimentoSalvo = false, Msg = Validacao.Erros });
			}
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoEditar })]
		public ActionResult Editar(int? id)
		{
			int empreendimentoId = id ?? 0;
			Empreendimento emp = _bus.Obter(empreendimentoId);
			if (emp.Id > 0)
			{
				if (_validar.EmpreendimentoEmPosse(emp.Id))
				{
					if (emp.Enderecos.Count == 0)
					{
						emp.Enderecos.Add(new Endereco());
						emp.Enderecos.Add(new Endereco());
					}
					else if (emp.Enderecos.Count == 1)
					{
						emp.Enderecos.Add(new Endereco());
					}

					SalvarVM vm = new SalvarVM(_busLista.Estados,
						_busLista.Municipios(emp.Enderecos[0].EstadoId),
						_busLista.Municipios(emp.Enderecos[1].EstadoId),
						_busLista.Segmentos, _busLista.TiposCoordenada,
						_busLista.Datuns,
						_busLista.Fusos,
						_busLista.Hemisferios,
						_busLista.TiposResponsavel,
						_busLista.LocalColetaPonto,
						_busLista.FormaColetaPonto,
						emp.Enderecos[0].EstadoId,
						emp.Enderecos[0].MunicipioId,
						emp.Enderecos[1].EstadoId,
						emp.Enderecos[1].MunicipioId,
						emp.Coordenada.LocalColeta.GetValueOrDefault(),
						emp.Coordenada.FormaColeta.GetValueOrDefault());

					vm.Empreendimento = emp;

					PreencherSalvar(vm);


					if (vm.Empreendimento.Responsaveis != null && vm.Empreendimento.Responsaveis.Count > 0)
					{
						vm.Responsaveis = ViewModelHelper.CriarSelectList(ObterResponsaveisList(vm.Empreendimento.Responsaveis), true, true);
					}

					if (Request.IsAjaxRequest())
					{
						vm.IsAjaxRequest = true;
						return PartialView("Salvar", vm);
					}
					else
					{
						EmpreendimentoVM empVm = new EmpreendimentoVM();
						vm.IsEditar = true;
						empVm.SalvarVM = vm;
						return View("Editar", empVm);
					}
				}
				else
				{
					Validacao.Add(Mensagem.Empreendimento.Posse);
					if (Request.IsAjaxRequest())
					{
						return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
					}
					return RedirectToAction("Index", Validacao.QueryParamSerializer());
				}
			}
			else
			{
				Validacao.Add(Mensagem.Empreendimento.NaoEncontrouRegistros);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoEditar })]
		public ActionResult Editar(SalvarVM vm)
		{
			vm.SetCoordenada();
			PreencherSalvar(vm);

			if (_bus.Salvar(vm.Empreendimento))
			{
				Validacao.Add(Mensagem.Empreendimento.Editar);
				string urlRedireciona = Url.Action("Index", "Empreendimento");
				urlRedireciona += "?Msg=" + Validacao.QueryParam();

				return Json(new { IsEmpreendimentoSalvo = Validacao.EhValido, Msg = Validacao.Erros, UrlRedireciona = urlRedireciona, Empreendimento = vm.Empreendimento });
			}
			else
			{
				return Json(new { IsEmpreendimentoSalvo = false, Msg = Validacao.Erros });
			}
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoVisualizar })]
		public ActionResult Visualizar(int? id, bool mostrarTituloTela = true)
		{
			int empreendimentoId = id ?? 0;
			Empreendimento emp = _bus.Obter(empreendimentoId);
			if (emp.Id > 0)
			{
				if (emp.Enderecos.Count == 0)
				{
					emp.Enderecos.Add(new Endereco());
					emp.Enderecos.Add(new Endereco());
				}
				else if (emp.Enderecos.Count == 1)
				{
					emp.Enderecos.Add(new Endereco());
				}

				SalvarVM vm = new SalvarVM(_busLista.Estados,
					_busLista.Municipios(emp.Enderecos[0].EstadoId),
					_busLista.Municipios(emp.Enderecos[1].EstadoId),
					_busLista.Segmentos, _busLista.TiposCoordenada,
					_busLista.Datuns,
					_busLista.Fusos,
					_busLista.Hemisferios,
					_busLista.TiposResponsavel,
					_busLista.LocalColetaPonto,
					_busLista.FormaColetaPonto,
					emp.Enderecos[0].EstadoId,
					emp.Enderecos[0].MunicipioId,
					emp.Enderecos[1].EstadoId,
					emp.Enderecos[1].MunicipioId,
					emp.Coordenada.LocalColeta.GetValueOrDefault(),
					emp.Coordenada.FormaColeta.GetValueOrDefault());

				vm.Empreendimento = emp;
				vm.MostrarTituloTela = mostrarTituloTela;
				PreencherSalvar(vm);

				if (Request.IsAjaxRequest())
				{
					return PartialView("VisualizarPartial", vm);
				}
				else
				{
					return View("Visualizar", vm);
				}
			}
			else
			{
				Validacao.Add(Mensagem.Empreendimento.NaoEncontrouRegistros);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			Empreendimento empreendimento = _bus.Obter(id);

			vm.Id = id;
			vm.Mensagem = Mensagem.Empreendimento.MensagemExcluirEmpreendimento(empreendimento.Denominador);
			vm.Titulo = "Excluir Empreendimento";
			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoExcluir })]
		public ActionResult Excluir(int id)
		{
			return Json(new { Msg = Validacao.Erros, @EhValido = _bus.Excluir(id) });
		}

		#endregion

		#region Caracterização

		public ActionResult Caracterizacao(int id)
		{
			return Redirect("~/Caracterizacoes/Caracterizacao/Index/" + id);
		}

		#endregion

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoVisualizar })]
		public ActionResult EmpreendimentoModalVisualizar(int id)
		{
			Empreendimento emp = _bus.Obter(id);

			if (emp.Id > 0)
			{
				if (emp.Enderecos.Count == 0)
				{
					emp.Enderecos.Add(new Endereco());
					emp.Enderecos.Add(new Endereco());
				}
				else if (emp.Enderecos.Count == 1)
				{
					emp.Enderecos.Add(new Endereco());
				}

				EmpreendimentoVM vm = new EmpreendimentoVM();
				SalvarVM vmSalvar = new SalvarVM(_busLista.Estados, _busLista.Municipios(emp.Enderecos[0].EstadoId), _busLista.Municipios(emp.Enderecos[1].EstadoId), _busLista.Segmentos, _busLista.TiposCoordenada,
					_busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busLista.TiposResponsavel, _busLista.LocalColetaPonto, _busLista.FormaColetaPonto,
					emp.Enderecos[0].EstadoId, emp.Enderecos[0].MunicipioId, emp.Enderecos[1].EstadoId, emp.Enderecos[1].MunicipioId, emp.Coordenada.LocalColeta.GetValueOrDefault(), emp.Coordenada.FormaColeta.GetValueOrDefault());

				vm.SalvarVM = vmSalvar;
				vm.SalvarVM.IsVisualizar = true;
				vm.SalvarVM.Empreendimento = emp;

				PreencherSalvar(vm.SalvarVM);

				return PartialView("EmpreendimentoModal", vm);
			}
			else
			{
				Validacao.Add(Mensagem.Empreendimento.NaoEncontrouRegistros);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		#region Validações

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar, ePermissao.EmpreendimentoEditar })]
		public ActionResult VerificarCnpj(string cnpj, int id)
		{
			return Json(new { @Msg = _bus.ExisteCnpj(cnpj, id), EhValido = Validacao.EhValido }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult VerificarLocalizacaoEmpreendimento(string easting, string northing, int estadoID, int municipioID)
		{
			return Json(new { @Msg = _bus.VerificarLocalizacaoEmpreendimento(easting, northing, estadoID, municipioID), EhValido = Validacao.EhValido }, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult ValidarPosseEmpreendimento(int id)
		{
			if (!_validar.EmpreendimentoEmPosse(id))
			{
				Validacao.Add(Mensagem.Empreendimento.Posse);
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		#endregion

		#region Auxiliares

		private void PreencherSalvar(SalvarVM vm)
		{
			if (vm.Empreendimento != null)
			{
				#region meios de contato

				if (vm.Empreendimento.MeiosContatos == null || vm.Empreendimento.MeiosContatos.Count == 0)
				{
					vm.Empreendimento.MeiosContatos = PreencheContato(vm.Contato);
				}

				if (vm.Empreendimento.MeiosContatos != null && vm.Empreendimento.MeiosContatos.Count > 0)
				{
					vm.Contato = CarregaMeiosContatos(vm.Empreendimento.MeiosContatos);
				}

				#endregion

				if (vm.Segmentos == null)
				{
					vm.Segmentos = new List<SelectListItem>();
				}

				#region responsáveis

				if (vm.Empreendimento.Responsaveis != null && vm.Empreendimento.Responsaveis.Count > 0)
				{
					foreach (Responsavel resp in vm.Empreendimento.Responsaveis)
					{
						if (resp.Tipo == 3)
						{
							if (resp.DataVencimento != null)
							{
								resp.DataVencimentoTexto = resp.DataVencimento.Value.ToShortDateString();
							}
							else
							{
								resp.DataVencimento = ValidacoesGenericasBus.ParseData(resp.DataVencimentoTexto);
							}
						}
						else
						{
							resp.DataVencimento = null;
						}
					}
				}

				#endregion
			}
		}

		private List<Contato> PreencheContato(ContatoVME contato)
		{
			List<Contato> meiosContatos = new List<Contato>();

			try
			{
				if (!string.IsNullOrEmpty(contato.Telefone))
				{
					meiosContatos.Add(new Contato { Valor = contato.Telefone, Id = contato.TelefoneId, TipoContato = eTipoContato.TelefoneResidencial });
				}

				if (!string.IsNullOrEmpty(contato.TelefoneFax))
				{
					meiosContatos.Add(new Contato { Valor = contato.TelefoneFax, Id = contato.TelefoneFaxId, TipoContato = eTipoContato.TelefoneFax });
				}

				if (!string.IsNullOrEmpty(contato.Email))
				{
					meiosContatos.Add(new Contato { Valor = contato.Email, Id = contato.EmailId, TipoContato = eTipoContato.Email });
				}

				if (!string.IsNullOrEmpty(contato.NomeContato))
				{
					meiosContatos.Add(new Contato { Valor = contato.NomeContato, Id = contato.NomeContatoId, TipoContato = eTipoContato.NomeContato });
				}

				return meiosContatos;
			}
			catch (Exception exc)
			{
				Validacao.AddAdvertencia(exc.Message);
			}
			return null;
		}

		private ContatoVME CarregaMeiosContatos(List<Contato> meiosContatos)
		{
			ContatoVME contatoVME = new ContatoVME();

			if (meiosContatos != null && meiosContatos.Count > 0)
			{
				foreach (Contato cont in meiosContatos)
				{
					switch (cont.TipoContato)
					{
						case eTipoContato.TelefoneResidencial:
							contatoVME.Telefone = cont.Valor;
							contatoVME.TelefoneId = cont.Id;
							break;

						case eTipoContato.TelefoneFax:
							contatoVME.TelefoneFax = cont.Valor;
							contatoVME.TelefoneFaxId = cont.Id;
							break;

						case eTipoContato.Email:
							contatoVME.Email = cont.Valor;
							contatoVME.EmailId = cont.Id;
							break;

						case eTipoContato.NomeContato:
							contatoVME.NomeContato = cont.Valor;
							contatoVME.NomeContatoId = cont.Id;
							break;
					}
				}
			}

			return contatoVME;
		}

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoListar })]
		public ActionResult AssociarCompensacao(int empreendimentoCompensacao)
		{
			ListarVM vm = new ListarVM();
			vm.SelListSegmentos = ViewModelHelper.CriarSelectList(_busLista.Segmentos);
			vm.Filtros.EmpreendimentoCompensacao = empreendimentoCompensacao;
			return PartialView("ListarFiltros", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar, ePermissao.EmpreendimentoEditar })]
		public ActionResult ObterEnderecoResponsavel(int responsavelId)
		{
			Endereco endereco = new PessoaBus().Obter(responsavelId).Endereco;

			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				@Endereco = endereco
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar, ePermissao.EmpreendimentoEditar })]
		public ActionResult ObterListaResponsaveis(List<Responsavel> responsaveis)
		{

			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				@Responsaveis = ObterResponsaveisList(responsaveis)
			}, JsonRequestBehavior.AllowGet);
		}

		private List<Lista> ObterResponsaveisList(List<Responsavel> responsaveis)
		{
			List<Lista> lista = new List<Lista>();

			if (responsaveis != null && responsaveis.Count > 0)
			{
				responsaveis.ForEach(responsavel => { if (responsavel.Id.HasValue && responsavel.Id > 0) { lista.Add(new Lista() { Id = responsavel.Id.ToString(), Texto = responsavel.NomeRazao, IsAtivo = true }); } });
			}

			return lista;
		}

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar, ePermissao.EmpreendimentoEditar })]
		public ActionResult ObterListaPessoasAssociada(int tipo, int empreendimentoId = 0, int requerimentoId = 0)
		{
			List<PessoaLst> lista = new List<PessoaLst>();

			switch ((eEnderecoTipo)tipo)
			{
				case eEnderecoTipo.Interessado:
					lista = _bus.ObterListaInteressadoEmpreendimento(empreendimentoId, requerimentoId);
					break;
				case eEnderecoTipo.ResponsavelTecnico:
					lista = _bus.ObterListaResponsavelTecnicoEmpreendimento(empreendimentoId, requerimentoId);
					break;
			}

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@Lista = lista
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}
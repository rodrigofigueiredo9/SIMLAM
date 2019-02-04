using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMEmpreendimento;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class EmpreendimentoController : DefaultController
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		EmpreendimentoInternoBus _busInterno;
		EmpreendimentoCredenciadoValidar _validar;
		EmpreendimentoCredenciadoBus _bus;
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		RequerimentoCredenciadoBus _busRequerimento;
		public String EstadoDefault
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyEstadoDefault); }
		}

		#endregion

		public EmpreendimentoController()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_busInterno = new EmpreendimentoInternoBus();
			_validar = new EmpreendimentoCredenciadoValidar();
			_bus = new EmpreendimentoCredenciadoBus();
			_busRequerimento = new RequerimentoCredenciadoBus();
		} 

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar })]
		public ActionResult Criar()
		{
			EmpreendimentoVM vm = new EmpreendimentoVM(ListaCredenciadoBus.Estados, ListaCredenciadoBus.Municipios(ListaCredenciadoBus.EstadoDefault),
				ListaCredenciadoBus.Segmentos, ListaCredenciadoBus.TiposCoordenada, ListaCredenciadoBus.Datuns, ListaCredenciadoBus.Fusos, ListaCredenciadoBus.Hemisferios, ListaCredenciadoBus.TiposResponsavel);

			return View("Criar", vm);
		}

		#region Localizar

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

				Resultados<Empreendimento> aux = _busInterno.Filtrar(vm.Filtros, vm.Paginacao, false);
				if (aux != null && aux.Itens != null && aux.Itens.Count > 0)
				{
					foreach (Empreendimento empreendimento in aux.Itens)
					{
						empreendimento.InternoId = empreendimento.Id;
						empreendimento.Id = 0;
					}
				}

				retorno.Itens.AddRange(aux.Itens.Where(x => !retorno.Itens.Exists(y => y.InternoId == x.InternoId)).ToList());
				retorno.Quantidade += aux.Quantidade;

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
			LocalizarVM vm = new LocalizarVM(ListaCredenciadoBus.Estados, ListaCredenciadoBus.Municipios(localizarVm.Filtros.EstadoId.GetValueOrDefault()),
				ListaCredenciadoBus.Segmentos, ListaCredenciadoBus.TiposCoordenada, ListaCredenciadoBus.Datuns, ListaCredenciadoBus.Fusos, ListaCredenciadoBus.Hemisferios);

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

			if (resposta.Erros.Count > 0)
			{
				return Json(new { IsEmpreendimentoSalvo = false, Msg = Validacao.Erros });
			}

			var objJson = resposta.Data;
			int codigoIbge = 0;

			codigoIbge = Convert.ToInt32(objJson["Municipio"]["IBGE"] ?? 0);

			ListaValoresDa _da = new ListaValoresDa();
			municipio = _da.ObterMunicipio(codigoIbge);

			if (municipio.Estado.Sigla != ViewModelHelper.EstadoDefaultSigla())
			{
				lstEstados = ListaCredenciadoBus.Estados.Where(x => x.Texto != ViewModelHelper.EstadoDefaultSigla()).ToList();
				lstMunicipios = new List<Municipio>();
			}
			else
			{
				lstEstados = ListaCredenciadoBus.Estados;
				lstMunicipios = ListaCredenciadoBus.Municipios(municipio.Estado.Id);

				localizarVm.Filtros.EstadoId = municipio.Estado.Id;
				localizarVm.Filtros.MunicipioId = municipio.Id;
			}

			#endregion

			SalvarVM vm = new SalvarVM(
				lstEstados,
				lstMunicipios,
				lstMunicipios,
				ListaCredenciadoBus.Segmentos,
				ListaCredenciadoBus.TiposCoordenada,
				ListaCredenciadoBus.Datuns,
				ListaCredenciadoBus.Fusos,
				ListaCredenciadoBus.Hemisferios,
				ListaCredenciadoBus.TiposResponsavel,
				ListaCredenciadoBus.LocalColetaPonto,
				ListaCredenciadoBus.FormaColetaPonto,
				localizarVm.Filtros.EstadoId,
				localizarVm.Filtros.MunicipioId, 0, 0);

			vm.SetLocalizarVm(localizarVm.Filtros);
			vm.SetCoordenada();
			PreencherSalvar(vm);

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

			bool isInfCorte = _busRequerimento.IsRequerimentoAtividadeCorte(vm.requerimentoId);
			if (_bus.Salvar(vm.Empreendimento, isInfCorte))
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
		public ActionResult Editar(int? id, int internoId = 0)
		{
			if (id > 0 || internoId > 0)
			{
				if (_validar.EmpreendimentoEmPosse(id.Value))
				{
					Empreendimento empreendimento = _bus.ObterEmpreendimento(id.Value, internoId);

					if (empreendimento.Enderecos.Count == 0)
					{
						empreendimento.Enderecos.Add(new Endereco());
						empreendimento.Enderecos.Add(new Endereco());
					}
					else if (empreendimento.Enderecos.Count == 1)
					{
						empreendimento.Enderecos.Add(new Endereco());
					}

					SalvarVM vm = new SalvarVM(
						ListaCredenciadoBus.Estados,
						ListaCredenciadoBus.Municipios(empreendimento.Enderecos[0].EstadoId),
						ListaCredenciadoBus.Municipios(empreendimento.Enderecos[1].EstadoId),
						ListaCredenciadoBus.Segmentos, 
						ListaCredenciadoBus.TiposCoordenada,
						ListaCredenciadoBus.Datuns,
						ListaCredenciadoBus.Fusos,
						ListaCredenciadoBus.Hemisferios,
						ListaCredenciadoBus.TiposResponsavel,
						ListaCredenciadoBus.LocalColetaPonto,
						ListaCredenciadoBus.FormaColetaPonto,
						empreendimento.Enderecos[0].EstadoId,
						empreendimento.Enderecos[0].MunicipioId,
						empreendimento.Enderecos[1].EstadoId,
						empreendimento.Enderecos[1].MunicipioId,
						empreendimento.Coordenada.LocalColeta.GetValueOrDefault(),
						empreendimento.Coordenada.FormaColeta.GetValueOrDefault());

					vm.Empreendimento = empreendimento;

					PreencherSalvar(vm);
					
					if (Request.IsAjaxRequest())
					{
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

		public ActionResult CopiarDadosIdaf(int id = 0, int internoId = 0)
		{
			if (id > 0 || internoId > 0)
			{
				if (_validar.EmpreendimentoEmPosse(id))
				{
					Empreendimento empreendimento = _bus.ObterEmpreendimento(0, internoId);
					empreendimento.Id = id;

					if (empreendimento.Enderecos.Count == 0)
					{
						empreendimento.Enderecos.Add(new Endereco());
						empreendimento.Enderecos.Add(new Endereco());
					}
					else if (empreendimento.Enderecos.Count == 1)
					{
						empreendimento.Enderecos.Add(new Endereco());
					}

					SalvarVM vm = new SalvarVM(
						ListaCredenciadoBus.Estados,
						ListaCredenciadoBus.Municipios(empreendimento.Enderecos[0].EstadoId),
						ListaCredenciadoBus.Municipios(empreendimento.Enderecos[1].EstadoId),
						ListaCredenciadoBus.Segmentos,
						ListaCredenciadoBus.TiposCoordenada,
						ListaCredenciadoBus.Datuns,
						ListaCredenciadoBus.Fusos,
						ListaCredenciadoBus.Hemisferios,
						ListaCredenciadoBus.TiposResponsavel,
						ListaCredenciadoBus.LocalColetaPonto,
						ListaCredenciadoBus.FormaColetaPonto,
						empreendimento.Enderecos[0].EstadoId,
						empreendimento.Enderecos[0].MunicipioId,
						empreendimento.Enderecos[1].EstadoId,
						empreendimento.Enderecos[1].MunicipioId,
						empreendimento.Coordenada.LocalColeta.GetValueOrDefault(),
						empreendimento.Coordenada.FormaColeta.GetValueOrDefault());

					vm.Empreendimento = empreendimento;
					vm.IsCopiado = true;
					PreencherSalvar(vm);
					vm.Empreendimento.Responsaveis.ForEach(x => {
						x.Id = 0;
						x.IdRelacionamento = 0;
						x.IsCopiado = true;					
					});

					if (Request.IsAjaxRequest())
					{
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

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoVisualizar })]
		public ActionResult Visualizar(int? id, int internoId = 0, bool mostrarTituloTela = true)
		{
			Empreendimento empreendimento = _bus.ObterEmpreendimento(id.GetValueOrDefault(), internoId);

			if (empreendimento.Id > 0 || empreendimento.InternoId > 0)
			{
				if (empreendimento.Enderecos.Count == 0)
				{
					empreendimento.Enderecos.Add(new Endereco());
					empreendimento.Enderecos.Add(new Endereco());
				}
				else if (empreendimento.Enderecos.Count <= 1)
				{
					empreendimento.Enderecos.Add(new Endereco());
				}

				SalvarVM vm = new SalvarVM(
					ListaCredenciadoBus.Estados,
					ListaCredenciadoBus.Municipios(empreendimento.Enderecos[0].EstadoId),
					ListaCredenciadoBus.Municipios(empreendimento.Enderecos[1].EstadoId),
					ListaCredenciadoBus.Segmentos, 
					ListaCredenciadoBus.TiposCoordenada,
					ListaCredenciadoBus.Datuns,
					ListaCredenciadoBus.Fusos,
					ListaCredenciadoBus.Hemisferios,
					ListaCredenciadoBus.TiposResponsavel,
					ListaCredenciadoBus.LocalColetaPonto,
					ListaCredenciadoBus.FormaColetaPonto,
					empreendimento.Enderecos[0].EstadoId,
					empreendimento.Enderecos[0].MunicipioId,
					empreendimento.Enderecos[1].EstadoId,
					empreendimento.Enderecos[1].MunicipioId,
					empreendimento.Coordenada.LocalColeta.GetValueOrDefault(),
					empreendimento.Coordenada.FormaColeta.GetValueOrDefault());

				vm.Empreendimento = empreendimento;
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

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoVisualizar })]
		public ActionResult EmpreendimentoModalVisualizar(int id, int internoId = 0)
		{
			Empreendimento empreendimento = _bus.ObterEmpreendimento(id, internoId);

			if (empreendimento.Id > 0 || empreendimento.InternoId > 0)
			{
				if (empreendimento.Enderecos.Count == 0)
				{
					empreendimento.Enderecos.Add(new Endereco());
					empreendimento.Enderecos.Add(new Endereco());
				}
				else if (empreendimento.Enderecos.Count == 1)
				{
					empreendimento.Enderecos.Add(new Endereco());
				}

				EmpreendimentoVM vm = new EmpreendimentoVM();
				SalvarVM vmSalvar = new SalvarVM(
					ListaCredenciadoBus.Estados,
					ListaCredenciadoBus.Municipios(empreendimento.Enderecos[0].EstadoId),
					ListaCredenciadoBus.Municipios(empreendimento.Enderecos[1].EstadoId),
					ListaCredenciadoBus.Segmentos,
					ListaCredenciadoBus.TiposCoordenada,
					ListaCredenciadoBus.Datuns,
					ListaCredenciadoBus.Fusos,
					ListaCredenciadoBus.Hemisferios,
					ListaCredenciadoBus.TiposResponsavel,
					ListaCredenciadoBus.LocalColetaPonto,
					ListaCredenciadoBus.FormaColetaPonto,
					empreendimento.Enderecos[0].EstadoId,
					empreendimento.Enderecos[0].MunicipioId,
					empreendimento.Enderecos[1].EstadoId,
					empreendimento.Enderecos[1].MunicipioId,
					empreendimento.Coordenada.LocalColeta.GetValueOrDefault(),
					empreendimento.Coordenada.FormaColeta.GetValueOrDefault());

				vm.SalvarVM = vmSalvar;
				vm.SalvarVM.IsVisualizar = true;
				vm.SalvarVM.Empreendimento = empreendimento;

				PreencherSalvar(vm.SalvarVM);

				return PartialView("EmpreendimentoModal", vm);
			}
			else
			{
				Validacao.Add(Mensagem.Empreendimento.NaoEncontrouRegistros);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoVisualizar })]
		public ActionResult VisualizarInterno(int id, bool mostrarTituloTela = true)
		{
			Empreendimento empreendimento = _bus.ObterEmpreendimento(0, id);

			if (empreendimento.Id > 0 || empreendimento.InternoId > 0)
			{
				if (empreendimento.Enderecos.Count == 0)
				{
					empreendimento.Enderecos.Add(new Endereco());
					empreendimento.Enderecos.Add(new Endereco());
				}
				else if (empreendimento.Enderecos.Count <= 1)
				{
					empreendimento.Enderecos.Add(new Endereco());
				}

				EmpreendimentoVM vm = new EmpreendimentoVM();
				SalvarVM vmSalvar = new SalvarVM(
					ListaCredenciadoBus.Estados,
					ListaCredenciadoBus.Municipios(empreendimento.Enderecos[0].EstadoId),
					ListaCredenciadoBus.Municipios(empreendimento.Enderecos[1].EstadoId),
					ListaCredenciadoBus.Segmentos,
					ListaCredenciadoBus.TiposCoordenada,
					ListaCredenciadoBus.Datuns,
					ListaCredenciadoBus.Fusos,
					ListaCredenciadoBus.Hemisferios,
					ListaCredenciadoBus.TiposResponsavel,
					ListaCredenciadoBus.LocalColetaPonto,
					ListaCredenciadoBus.FormaColetaPonto,
					empreendimento.Enderecos[0].EstadoId,
					empreendimento.Enderecos[0].MunicipioId,
					empreendimento.Enderecos[1].EstadoId,
					empreendimento.Enderecos[1].MunicipioId,
					empreendimento.Coordenada.LocalColeta.GetValueOrDefault(),
					empreendimento.Coordenada.FormaColeta.GetValueOrDefault());

				vm.SalvarVM = vmSalvar;
				vm.SalvarVM.IsVisualizar = true;
				vm.SalvarVM.Empreendimento = empreendimento;
				vm.SalvarVM.MostrarTituloTela = mostrarTituloTela;

				PreencherSalvar(vm.SalvarVM);

				if (Request.IsAjaxRequest())
				{
					return PartialView("VisualizarPartial", vm.SalvarVM);
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
			ConfirmarVM vm = new ConfirmarVM();
			Empreendimento empreendimento = _bus.Obter(id);

			vm.Id = id;
			vm.Mensagem = Mensagem.Empreendimento.MensagemExcluirEmpreendimento(empreendimento.Denominador);
			vm.Titulo = "Excluir Empreendimento";
			return PartialView("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoExcluir })]
		public ActionResult Excluir(int id)
		{
			return Json(new { Msg = Validacao.Erros, @EhValido = _bus.Excluir(id) });
		}

		#endregion

		#region Inline

		//Usado em outras controllers
		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar })]
		public ActionResult EmpreendimentoInline(int id)
		{
			EmpreendimentoVM vm = new EmpreendimentoVM();

			if (id > 0)
			{
				Empreendimento emp = _bus.Obter(id);

				if (emp.Enderecos.Count == 0)
				{
					emp.Enderecos.Add(new Endereco());
					emp.Enderecos.Add(new Endereco());
				}
				else if (emp.Enderecos.Count == 1)
				{
					emp.Enderecos.Add(new Endereco());
				}

				SalvarVM salvarVM = new SalvarVM(
					ListaCredenciadoBus.Estados,
					ListaCredenciadoBus.Municipios(emp.Enderecos[0].EstadoId),
					ListaCredenciadoBus.Municipios(emp.Enderecos[1].EstadoId),
					ListaCredenciadoBus.Segmentos,
					ListaCredenciadoBus.TiposCoordenada,
					ListaCredenciadoBus.Datuns,
					ListaCredenciadoBus.Fusos,
					ListaCredenciadoBus.Hemisferios,
					ListaCredenciadoBus.TiposResponsavel,
					ListaCredenciadoBus.LocalColetaPonto,
					ListaCredenciadoBus.FormaColetaPonto,
					emp.Enderecos[0].EstadoId,
					emp.Enderecos[0].MunicipioId,
					emp.Enderecos[1].EstadoId,
					emp.Enderecos[1].MunicipioId,
					emp.Coordenada.LocalColeta.GetValueOrDefault(),
					emp.Coordenada.FormaColeta.GetValueOrDefault());

				vm.SalvarVM = salvarVM;
				vm.SalvarVM.Empreendimento = emp;
				vm.SalvarVM.MostrarTituloTela = false;
				vm.SalvarVM.IsVisualizar = true;
				PreencherSalvar(vm.SalvarVM);
			}
			else
			{
				vm = new EmpreendimentoVM(
					ListaCredenciadoBus.Estados,
					ListaCredenciadoBus.Municipios(ListaCredenciadoBus.EstadoDefault),
					ListaCredenciadoBus.Segmentos,
					ListaCredenciadoBus.TiposCoordenada,
					ListaCredenciadoBus.Datuns,
					ListaCredenciadoBus.Fusos,
					ListaCredenciadoBus.Hemisferios,
					ListaCredenciadoBus.TiposResponsavel);
			}

			return PartialView("EmpreendimentoInline", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar })]
		public ActionResult EmpreendimentoInlineInteressado(int id, int requerimentoId)
		{
			EmpreendimentoVM vm = new EmpreendimentoVM();

			if(requerimentoId > 0 && id <= 0)
			{
				var interessado = _busRequerimento.ObterSimplificado(requerimentoId).Interessado.Id;
				vm.ListarVM.Resultados = _bus.ObterEmpreendimentoResponsavel(interessado);
			}else
			{
				if (id > 0)
				{
					Empreendimento emp = _bus.Obter(id);

					if (emp.Enderecos.Count == 0)
					{
						emp.Enderecos.Add(new Endereco());
						emp.Enderecos.Add(new Endereco());
					}
					else if (emp.Enderecos.Count == 1)
					{
						emp.Enderecos.Add(new Endereco());
					}

					SalvarVM salvarVM = new SalvarVM(
						ListaCredenciadoBus.Estados,
						ListaCredenciadoBus.Municipios(emp.Enderecos[0].EstadoId),
						ListaCredenciadoBus.Municipios(emp.Enderecos[1].EstadoId),
						ListaCredenciadoBus.Segmentos,
						ListaCredenciadoBus.TiposCoordenada,
						ListaCredenciadoBus.Datuns,
						ListaCredenciadoBus.Fusos,
						ListaCredenciadoBus.Hemisferios,
						ListaCredenciadoBus.TiposResponsavel,
						ListaCredenciadoBus.LocalColetaPonto,
						ListaCredenciadoBus.FormaColetaPonto,
						emp.Enderecos[0].EstadoId,
						emp.Enderecos[0].MunicipioId,
						emp.Enderecos[1].EstadoId,
						emp.Enderecos[1].MunicipioId,
						emp.Coordenada.LocalColeta.GetValueOrDefault(),
						emp.Coordenada.FormaColeta.GetValueOrDefault());

					vm.SalvarVM = salvarVM;
					vm.SalvarVM.Empreendimento = emp;
					vm.SalvarVM.MostrarTituloTela = false;
					vm.SalvarVM.IsVisualizar = true;
					PreencherSalvar(vm.SalvarVM);
				}
				else
				{
					_bus.Obter(id);
					vm = new EmpreendimentoVM(
						ListaCredenciadoBus.Estados,
						ListaCredenciadoBus.Municipios(ListaCredenciadoBus.EstadoDefault),
						ListaCredenciadoBus.Segmentos,
						ListaCredenciadoBus.TiposCoordenada,
						ListaCredenciadoBus.Datuns,
						ListaCredenciadoBus.Fusos,
						ListaCredenciadoBus.Hemisferios,
						ListaCredenciadoBus.TiposResponsavel);
				}
			}

			return PartialView("EmpreendimentoInlineCorte", vm);
		}

		#endregion

		#region Validações

	   [Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar, ePermissao.EmpreendimentoEditar })]
		public ActionResult VerificarCnpj(string cnpj, int id)
		{
			return Json(new { @Msg = _bus.ExisteCnpj(cnpj, id), EhValido = Validacao.EhValido }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoCriar, ePermissao.EmpreendimentoEditar })]
		public ActionResult VerificarCnpjEmpreendimento(string cnpj)
		{
			bool existe = _bus.ValidarEmpreendimentoCnpjLocalizar(cnpj);

			if (!existe)
			{
				Validacao.Erros.Clear();
				existe = _busInterno.ValidarEmpreendimentoCnpjLocalizar(cnpj);
			}

			return Json(new { @ExisteEmpreendimento = existe, EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

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

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoListar})]
		public ActionResult Associar()
		{
			ListarVM vm = new ListarVM();
			vm.SelListSegmentos = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.Segmentos);
			vm.PodeAssociar = true;
			return PartialView("ListarFiltros", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.EmpreendimentoListar })]
		public ActionResult AssociarCompensacao(int empreendimentoCompensacao)
		{
			ListarVM vm = new ListarVM();
			vm.SelListSegmentos = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.Segmentos);
			vm.Filtros.EmpreendimentoCompensacao = empreendimentoCompensacao;
			vm.PodeAssociar = true;

			return PartialView("ListarFiltros", vm);
		}
		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<Empreendimento> resultados = _busInterno.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = User.IsInRole(ePermissao.EmpreendimentoEditar.ToString());
				vm.PodeExcluir = User.IsInRole(ePermissao.EmpreendimentoExcluir.ToString());

				vm.PodeCaracterizar = _caracterizacaoBus.ValidarAcessarTela(mostrarMensagem: false);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.EmpreendimentoVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}
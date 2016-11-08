using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloOrgaoParceiroConveniado.Bussiness;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class OrgaosParceirosConveniadosController : DefaultController
	{
		#region Propriedades

		OrgaoParceiroConveniadoBus _bus = new OrgaoParceiroConveniadoBus();
		OrgaoParceiroConveniadoValidar _validar = new OrgaoParceiroConveniadoValidar();
		ListaBus _busLista = new ListaBus();

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.OrgaoParceirosConveniadosSituacoes);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoListar })]
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

			Resultados<OrgaoParceiroConveniadoListarResultados> resultados = _bus.Filtrar(vm.Filtros, paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.OrgaoParceiroConveniadoVisualizar.ToString());
			vm.PodeAlterarSituacao = User.IsInRole(ePermissao.OrgaoParceiroConveniadoAlterarSituacao.ToString());
			vm.PodeEditar = User.IsInRole(ePermissao.OrgaoParceiroConveniadoEditar.ToString());
			vm.PodeGerenciar = true;

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoCriar })]
		public ActionResult Criar()
		{
			OrgaoParceiroConveniadoVM viewModel = new OrgaoParceiroConveniadoVM()
			{
				IsVisualizar = false,
				OrgaoParceiroConveniado = new OrgaoParceiroConveniado()
				{
					Nome = "",
					Sigla = "",
					TermoNumeroAno = "",
					Unidades = new List<Unidade>()
				}
			};

			return View(viewModel);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoCriar })]
		public ActionResult Criar(OrgaoParceiroConveniado orgaoParceiro)
		{
			_bus.Salvar(orgaoParceiro);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Url = Url.Action("Criar", "OrgaosParceirosConveniados", new { acaoId = orgaoParceiro.Id, Msg = Validacao.QueryParam() }) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoCriar, ePermissao.OrgaoParceiroConveniadoEditar })]
		public ActionResult Editar(int id)
		{
			OrgaoParceiroConveniado orgaoParceiro = _bus.Obter(id);

			if (_validar.EstaBloqueado(orgaoParceiro))
			{
				return RedirectToAction("Index", "OrgaosParceirosConveniados", new { @Msg = Validacao.QueryParam() });
			}

			OrgaoParceiroConveniadoVM vm = new OrgaoParceiroConveniadoVM()
			{
				OrgaoParceiroConveniado = orgaoParceiro
			};

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoEditar })]
		public ActionResult Editar(OrgaoParceiroConveniado orgaoParceiro)
		{
			_bus.Salvar(orgaoParceiro);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Url = Url.Action("Index", "OrgaosParceirosConveniados", new { acaoId = orgaoParceiro.Id, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoVisualizar })]
		public ActionResult Visualizar(int id)
		{
			OrgaoParceiroConveniadoVM viewModel = new OrgaoParceiroConveniadoVM()
			{
				IsVisualizar = true,
				OrgaoParceiroConveniado = _bus.Obter(id)

			};

			return View(viewModel);
		}

		#endregion

		#region Alterar Situação

		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoAlterarSituacao })]
		public ActionResult AlterarSituacao(int id)
		{
			AlterarSituacaoVM vm = new AlterarSituacaoVM();
			vm.OrgaoParceiroConveniado = _bus.Obter(id);
			vm.Situacoes = ViewModelHelper.CriarSelectList(_busLista.OrgaoParceirosConveniadosSituacoes
				.Where(item => vm.OrgaoParceiroConveniado.SituacaoId != int.Parse(item.Id)).ToList());

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoAlterarSituacao })]
		public ActionResult AlterarSituacao(OrgaoParceiroConveniado orgao)
		{
			_bus.AlterarSituacao(orgao);
			string url = string.Empty;

			if (orgao.SituacaoId == (int)eOrgaoParceiroConveniadoSituacao.Bloqueado)
			{
				url = Url.Action("Index", "OrgaosParceirosConveniados", new { acaoId = orgao.Id.ToString(), ocutarEditar = true, Msg = Validacao.QueryParam() });
			}
			else
			{
				url = Url.Action("Index", "OrgaosParceirosConveniados", new { acaoId = orgao.Id.ToString(), Msg = Validacao.QueryParam() });
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Url = url }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Gerenciar

		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoGerenciar })]
		public ActionResult Gerenciar(int id)
		{
			OrgaoParceiroConveniado orgao = _bus.Obter(id);

			if (_validar.EstaBloqueado(orgao))
			{
				return RedirectToAction("Index", "OrgaosParceirosConveniados", new { @Msg = Validacao.QueryParam() });
			}

			GerenciarVM vm = new GerenciarVM()
			{
				NomeOrgao = orgao.Nome,
				Sigla = orgao.Sigla,
				IdOrgao = orgao.Id,
				CredenciadosAguardandoAtivacao = new ListarCredenciadoParceiroVM() { Credenciados = new List<CredenciadoPessoa>() },
				CredenciadosAtivos = new ListarCredenciadoParceiroVM() { Credenciados = new List<CredenciadoPessoa>() },
				CredenciadosBloqueados = new ListarCredenciadoParceiroVM() { Credenciados = new List<CredenciadoPessoa>() },
				Unidades = new List<SelectListItem>()
			};

			vm.Unidades = ViewModelHelper.CriarSelectList(_bus.ObterUnidadesLst(0, orgao.Unidades), true);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoGerenciar })]
		public ActionResult GerenciarFiltrar(int idOrgaoParceiro, int idUnidade)
		{
			ListarCredenciadoParceiroVM vmAguardandoAtivacao = new ListarCredenciadoParceiroVM();
			ListarCredenciadoParceiroVM vmAtivos = new ListarCredenciadoParceiroVM();
			ListarCredenciadoParceiroVM vmBloqueados = new ListarCredenciadoParceiroVM();

			List<CredenciadoPessoa> credenciados = _bus.ObterCredenciados(idOrgaoParceiro, idUnidade);

			if (credenciados == null)
			{
				return Json(new
				{
					@EhValido = Validacao.EhValido,
					@Msg = Validacao.Erros,
					@Url = Url.Action("Index", "OrgaosParceirosConveniados", new { acaoId = idOrgaoParceiro, Msg = Validacao.QueryParam() })
				}, JsonRequestBehavior.AllowGet);
			}

			vmAguardandoAtivacao.Credenciados = credenciados.Where(credenciado => credenciado.Situacao == (int)eCredenciadoSituacao.Cadastrado).ToList();
			vmAtivos.Credenciados = credenciados.Where(credenciado => credenciado.Situacao == (int)eCredenciadoSituacao.Ativo).ToList();
			vmBloqueados.Credenciados = credenciados.Where(credenciado => credenciado.Situacao == (int)eCredenciadoSituacao.Bloqueado).ToList();

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@HtmlAguardandoAtivacao = vmAguardandoAtivacao.Credenciados.Count < 1 ? "" : ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarCredenciadosParceiros", vmAguardandoAtivacao),
				@HtmlAtivos = vmAtivos.Credenciados.Count < 1 ? "" : ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarCredenciadosParceiros", vmAtivos),
				@HtmlBloqueados = vmBloqueados.Credenciados.Count < 1 ? "" : ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarCredenciadosParceiros", vmBloqueados)

			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoGerenciar })]
		public ActionResult GerarChave(List<CredenciadoPessoa> credenciados, int idOrgaoParceiro, int idUnidade)
		{
			_bus.EnviarEmail(credenciados, idOrgaoParceiro);

			List<CredenciadoPessoa> lstCredenciados = _bus.ObterCredenciados(idOrgaoParceiro, idUnidade);

			if (lstCredenciados == null)
			{
				return Json(new
				{
					@EhValido = Validacao.EhValido,
					@Msg = Validacao.Erros,
					@Url = Url.Action("Index", "OrgaosParceirosConveniados", new { acaoId = idOrgaoParceiro, Msg = Validacao.QueryParam() })
				}, JsonRequestBehavior.AllowGet);
			}

			ListarCredenciadoParceiroVM vm = new ListarCredenciadoParceiroVM()
			{
				Credenciados = lstCredenciados.Where(credenciado => credenciado.Situacao == (int)eCredenciadoSituacao.Cadastrado).ToList()
			};
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@HtmlRetorno = vm.Credenciados.Count < 1 ? "" : ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarCredenciadosParceiros", vm)
			});
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoGerenciar })]
		public ActionResult Bloquear(List<CredenciadoPessoa> credenciados, int idOrgaoParceiro, int idUnidade)
		{
			if (!_bus.BloquearCredenciados(credenciados))
			{
				return Json(new
				{
					@Msg = Validacao.Erros,
					@Url = Url.Action("Index", "OrgaosParceirosConveniados", new { acaoId = idOrgaoParceiro, Msg = Validacao.QueryParam() })
				}, JsonRequestBehavior.AllowGet);
			}

			List<CredenciadoPessoa> lstCredenciados = _bus.ObterCredenciados(idOrgaoParceiro, idUnidade);

			ListarCredenciadoParceiroVM vmBloqueados = new ListarCredenciadoParceiroVM()
			{
				Credenciados = lstCredenciados.Where(credenciado => credenciado.Situacao == (int)eCredenciadoSituacao.Bloqueado).ToList()
			};

			ListarCredenciadoParceiroVM vmAtivos = new ListarCredenciadoParceiroVM()
			{
				Credenciados = lstCredenciados.Where(credenciado => credenciado.Situacao == (int)eCredenciadoSituacao.Ativo).ToList()
			};

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@HtmlRetornoBloqueados = vmBloqueados.Credenciados.Count < 1 ? "" : ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarCredenciadosParceiros", vmBloqueados),
				@HtmlRetornoAtivos = vmAtivos.Credenciados.Count < 1 ? "" : ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarCredenciadosParceiros", vmAtivos)
			});
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoGerenciar })]
		public ActionResult Desbloquear(List<CredenciadoPessoa> credenciados, int idOrgaoParceiro, int idUnidade)
		{
			if (!_bus.DesbloquearCredenciados(credenciados))
			{
				return Json(new
				{
					@Msg = Validacao.Erros,
					@Url = Url.Action("Index", "OrgaosParceirosConveniados", new { acaoId = idOrgaoParceiro, Msg = Validacao.QueryParam() })
				}, JsonRequestBehavior.AllowGet);
			}

			List<CredenciadoPessoa> lstCredenciados = _bus.ObterCredenciados(idOrgaoParceiro, idUnidade);


			ListarCredenciadoParceiroVM vmBloqueados = new ListarCredenciadoParceiroVM()
			{
				Credenciados = lstCredenciados.Where(credenciado => credenciado.Situacao == (int)eCredenciadoSituacao.Bloqueado).ToList()
			};


			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@HtmlRetornoBloqueados = vmBloqueados.Credenciados.Count < 1 ? "" : ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarCredenciadosParceiros", vmBloqueados),
			});
		}

		#endregion

		#region Auxiliares

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.OrgaoParceiroConveniadoListar })]
		public ActionResult VerificarCredenciadoAssociado(Unidade unidade)
		{
			_bus.VerificarCredenciadoAssociado(unidade);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}
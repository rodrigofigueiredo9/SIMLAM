using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAdministrador;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAdministrador.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class AdministradorController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		AdministradorBus _bus = new AdministradorBus(new AdministradorValidar());

		private string QuantidadePorPagina
		{
			get { return (Request.Cookies.Get("QuantidadePorPagina") != null) ? Request.Cookies.Get("QuantidadePorPagina").Value : "5"; }
		}

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.AdministradorListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_bus.Situacoes, _busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AdministradorListar })]
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

			Resultados<Administrador> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeEditar = User.IsInRole(ePermissao.AdministradorEditar.ToString());
			vm.PodeVisualizar = User.IsInRole(ePermissao.AdministradorVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.AdministradorCriar })]
		public ActionResult Criar()
		{
			CriarVM viewModel = new CriarVM();

			return View(viewModel);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AdministradorCriar })]
		public ActionResult Criar(AdministradorVM vm)
		{
			Administrador funcionario = _bus.Obter(vm.Cpf) ?? new Administrador();
			funcionario.Usuario.Login = vm.Login;
			funcionario.Cpf = vm.Cpf;
			funcionario.Nome = vm.Nome;
			funcionario.Email = vm.Email;

			vm.papeis = vm.papeis ?? new List<PapeisVME>();

			foreach (var item in vm.papeis)
			{
				if (item.IsAtivo)
				{
					if (!funcionario.Papeis.Exists(x => x.Id == item.Papel.Id) &&
						_bus.PapeisAdministrador.Exists(y => y.Id == item.Papel.Id))
					{
						funcionario.Papeis.Add(_bus.PapeisAdministrador.Single(y => y.Id == item.Papel.Id));
					}
				}
				else
				{
					funcionario.Papeis.RemoveAll(x => x.Id == item.Papel.Id);
				}
			}

			_bus.Salvar(funcionario, vm.Senha, vm.ConfirmarSenha, false);

			CriarVM viewModel = new CriarVM();
			viewModel.Administrador = funcionario;
			viewModel.CpfValido = true;

			viewModel.Papeis = _bus.PapeisAdministrador.
				Select(x => new PapeisVME() { Papel = x, IsAtivo = viewModel.Administrador.Papeis.Any(y => y.Id == x.Id) }).ToList();

			viewModel.TextoPermissoes = TextoPermissoes(viewModel.Papeis);

			if (!Validacao.EhValido)
			{
				return View(viewModel);
			}

			return RedirectToAction("Criar", Validacao.QueryParamSerializer());
		}

		[Permite(RoleArray = new Object[] { ePermissao.AdministradorCriar })]
		public ActionResult VerificarCpf(String Cpf)
		{
			CriarVM viewModel = new CriarVM();
			viewModel.CpfValido = _bus.VerificarCpf(Cpf);

			if (viewModel.CpfValido)
			{
				viewModel.Administrador = _bus.Obter(Cpf) ?? new Administrador();

				if (viewModel.Administrador != null && viewModel.Administrador.Id > 0)
				{
					Validacao.Add(Mensagem.Administrador.CpfEncontrado);
					return RedirectToAction("Editar/" + viewModel.Administrador.Id, Validacao.QueryParamSerializer());
				}
			}

			viewModel.Papeis = _bus.PapeisAdministrador.
				Select(x => new PapeisVME() { Papel = x, IsAtivo = viewModel.Administrador.Papeis.Any(y => y.Id == x.Id) }).ToList();

			viewModel.TextoPermissoes = TextoPermissoes(viewModel.Papeis);

			return View("Criar", viewModel);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.AdministradorEditar })]
		public ActionResult Editar(int id)
		{
			Administrador item = _bus.Obter(id);
			EditarVM viewModel = new EditarVM();

			viewModel.Administrador = item;

			if (item != null)
			{
				List<Papel> lp = _bus.PapeisAdministrador;

				viewModel.Papeis = lp.
					Select(x => new PapeisVME() { Papel = x, IsAtivo = viewModel.Administrador.Papeis.Any(y => y.Id == x.Id) }).ToList();

				viewModel.TextoPermissoes = TextoPermissoes(viewModel.Papeis);
			}
			return View(viewModel);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AdministradorEditar })]
		public ActionResult Editar(AdministradorVM vm)
		{
			Administrador funcionario = _bus.Obter(vm.Cpf) ?? new Administrador();
			funcionario.Usuario.Login = vm.Login;
			funcionario.Nome = vm.Nome;
			funcionario.Email = vm.Email;
			vm.papeis = vm.papeis ?? new List<PapeisVME>();
			if (!vm.AlterarSenha)
			{
				vm.Senha = "";
				vm.ConfirmarSenha= "";
			}

			foreach (var item in vm.papeis)
			{
				if (item.IsAtivo)
				{
					if (!funcionario.Papeis.Exists(x => x.Id == item.Papel.Id) &&
						_bus.PapeisAdministrador.Exists(y => y.Id == item.Papel.Id))
					{
						funcionario.Papeis.Add(item.Papel);
					}
				}
				else
				{
					funcionario.Papeis.RemoveAll(x => x.Id == item.Papel.Id);
				}
			}

			_bus.Salvar(funcionario, vm.Senha, vm.ConfirmarSenha, vm.AlterarSenha);

			EditarVM viewModel = new EditarVM();
			viewModel.Administrador = funcionario;

			viewModel.Papeis = _bus.PapeisAdministrador.
				Select(x => new PapeisVME() { Papel = x, IsAtivo = viewModel.Administrador.Papeis.Any(y => y.Id == x.Id) }).ToList();

			viewModel.TextoPermissoes = TextoPermissoes(viewModel.Papeis);

			if (!Validacao.EhValido)
			{
				return View(viewModel);
			}

			return RedirectToAction("Index", Validacao.QueryParamSerializer());
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.AdministradorVisualizar })]
		public ActionResult Visualizar(int id)
		{
			Administrador func = _bus.Obter(id);

			VisualizarVM viewModel = new VisualizarVM();

			viewModel.Administrador = func;

			if (func != null)
			{
				viewModel.Papeis = _bus.PapeisAdministrador.
					Select(x => new PapeisVME() { Papel = x, IsAtivo = viewModel.Administrador.Papeis.Any(y => y.Id == x.Id) }).ToList();

				viewModel.TextoPermissoes = TextoPermissoes(viewModel.Papeis);
			}

			return View("Visualizar", viewModel);
		}

		#endregion

		#region Transferir Sistema

		[Permite(RoleArray = new Object[] { ePermissao.AdministradorEditar })]
		public ActionResult TransferirSistema(int id)
		{
			_bus.ValidarTransferirSistema(id);
			Administrador adm = _bus.Obter(id);

			TransferirVM vm = new TransferirVM();
			vm.Id = adm.Id;
			vm.Nome = adm.Nome;
			vm.UrlLogout = Validacao.QueryParamSerializer(
				Url.Action("Logon", "Autenticacao"),
				new List<Mensagem>() { Mensagem.Administrador.Transferido }
			);

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AdministradorEditar })]
		public ActionResult TransferirSistema(int id, string motivo)
		{
			TransferirVM vm = new TransferirVM();
			if (_bus.TransferirSistema(id, motivo))
			{
				GerenciarAutenticacao.Deslogar();
				FormsAuthentication.SignOut();
			}
			else
			{
				Administrador adm = _bus.Obter(id);
				vm.Id = adm.Id;
				vm.Nome = adm.Nome;
			}
			return Json(new { Msg = Validacao.Erros, @Vm = vm });
		}

		#endregion

		#region Alterar Situação

		[Permite(RoleArray = new Object[] { ePermissao.AdministradorAlterarSituacao })]
		public ActionResult AlterarSituacao(int id)
		{
			AlterarSituacaoVM viewModel = new AlterarSituacaoVM(_bus.Situacoes.Where(x => x.Id != 3 && x.Id != 5 && x.Id != 6).ToList());

			Administrador funcionario = _bus.Obter(id);
			if (funcionario != null)
			{
				viewModel.Id = id;
				viewModel.Nome = funcionario.Nome;
				viewModel.Cpf = funcionario.Cpf;
				viewModel.Motivo = funcionario.SituacaoMotivo;
				viewModel.Situacao = _bus.Situacoes.Single(x => x.Id == funcionario.Situacao).Nome;
				viewModel.SituacaoId = funcionario.Situacao;
				viewModel.Situacoes.RemoveAll(x => x.Value == funcionario.Situacao.ToString());

				if (viewModel.SituacaoId == 3)
				{
					viewModel.Situacoes.RemoveAll(x => x.Value != "0");
				}
			}

			return View(viewModel);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AdministradorAlterarSituacao })]
		public ActionResult AlterarSituacao(int id, int novaSituacao, string motivo)
		{
			if (_bus.AlterarSituacao(id, novaSituacao, motivo))
			{
				if (novaSituacao == 1)
				{
					var param = Validacao.QueryParamSerializer();
					param.Add("id", id);
					return RedirectToAction("Editar", param);
				}

				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			AlterarSituacaoVM viewModel = new AlterarSituacaoVM(_bus.Situacoes.Where(x => x.Id != 3).ToList());
			viewModel.NovaSituacaoId = novaSituacao;
			viewModel.Id = id;

			Administrador funcionario = _bus.Obter(id);
			if (funcionario != null)
			{
				viewModel.Nome = funcionario.Nome;
				viewModel.Cpf = funcionario.Cpf;
				viewModel.SituacaoId = funcionario.Situacao;
				viewModel.Situacao = _bus.Situacoes.Single(x => x.Id == funcionario.Situacao).Nome;
				viewModel.Motivo = funcionario.SituacaoMotivo;

				if (viewModel.SituacaoId == 3)
				{
					viewModel.Situacoes.RemoveAll(x => x.Value != "0");
				}

				viewModel.Situacoes.RemoveAll(x => x.Value == funcionario.Situacao.ToString());
			}

			return View(viewModel);
		}

		#endregion

		#region Alterar Administrador

		public ActionResult AlterarAdministrador(int id)
		{
			EditarVM viewModel = new EditarVM();
			if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
			{
				EtramiteIdentity func = User.Identity as EtramiteIdentity;
				if (func == null || (func.FuncionarioTipo != 1 && func.FuncionarioTipo != 2)) // se não for "admin" ou "sistema" mostra mensagem
				{
					Validacao.Add(Mensagem.Administrador.SemPermissao);
				}
				else
				{
					Administrador adm = null;
					if (_bus.VerificarAlterarAdministrador(id))
					{
						adm = _bus.Obter(id);
					}
					viewModel.Administrador = adm;
				}
				return View("AlterarAdministrador", viewModel);
			}
			else
			{
				return Redirect(FormsAuthentication.LoginUrl);
			}
		}

		[HttpPost]
		public ActionResult AlterarAdministrador(int id, AdministradorVM vm)
		{
			if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
			{
				EditarVM viewModel = new EditarVM();
				EtramiteIdentity func = User.Identity as EtramiteIdentity;
				if (func == null || (func.FuncionarioTipo != 1 && func.FuncionarioTipo != 2)) // se não for "admin" ou "sistema" mostra mensagem
				{
					Validacao.Add(Mensagem.Administrador.SemPermissao);
				}
				else
				{
					viewModel.Administrador = _bus.Obter((User.Identity as EtramiteIdentity).FuncionarioId);
					_bus.AlterarSenhaAdministrador(viewModel.Administrador, vm.Senha, vm.ConfirmarSenha);
				}

				return View("AlterarAdministrador", viewModel);
			}
			else
			{
				return Redirect(FormsAuthentication.LoginUrl);
			}
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.AdministradorCriar, ePermissao.AdministradorEditar, ePermissao.AdministradorListar })]
		public string TextoPermissoes(List<PapeisVME> papeis)
		{
			List<String> texto = new List<string>();
			String permissoes = string.Empty;

			List<Papel> lstPapeis = _bus.PapeisAdministrador;

			foreach (PapeisVME p in papeis)
			{
				if (p.IsAtivo)
				{
					p.Papel = lstPapeis.Single(x => x.Id == p.Papel.Id);
					permissoes = string.Empty;
					p.Papel.Permissoes.ForEach(x => permissoes += "\n    " + x.Descricao);
					texto.Add(String.Format("-{0}{1}", p.Papel.Nome, permissoes));
				}
			}

			return String.Join("\n\n", texto.ToArray());
		}

		#endregion
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMFuncionario;
using Tecnomapas.Blocos.Arquivo;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class FuncionarioController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		FuncionarioBus _bus = new FuncionarioBus(new FuncionarioValidar());

		private string QuantidadePorPagina
		{
			get { return ViewModelHelper.CookieQuantidadePorPagina; }
		}

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_bus.Situacoes, _busLista.Cargos, _busLista.SetoresComSigla, _busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioListar, ePermissao.RelatorioPersonalizadoAtribuirExecutor })]
		public ActionResult Associar()
		{
			ListarVM vm = new ListarVM(_bus.Situacoes, _busLista.Cargos, _busLista.SetoresComSigla, _busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			return PartialView("ListarFiltros", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioListar, ePermissao.RelatorioPersonalizadoAtribuirExecutor })]
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

			Resultados<Funcionario> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = User.IsInRole(ePermissao.FuncionarioEditar.ToString());
				vm.PodeAlterarSituacao = User.IsInRole(ePermissao.FuncionarioAlterarSituacao.ToString());
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.FuncionarioVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioCriar })]
		public ActionResult Criar()
		{
			CriarVM viewModel = new CriarVM(_busLista.Cargos, _busLista.SetoresComSigla);

			return View(viewModel);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioCriar })]
		public ActionResult Criar(FuncionarioVM funcVM)
		{
			Funcionario funcionario = _bus.Obter(funcVM.Cpf) ?? new Funcionario();
			funcionario.Usuario.Login = (funcVM.Login ?? string.Empty).Trim();
			funcionario.Cpf = (funcVM.Cpf ?? string.Empty).Trim();
			funcionario.Nome = (funcVM.Nome ?? string.Empty).Trim();
			funcionario.Email = (funcVM.Email ?? string.Empty).Trim();

			funcionario.Arquivo.ContentType = funcVM.ArquivoContentType;
			funcionario.Arquivo.Extensao = funcVM.ArquivoExtensao;
			funcionario.Arquivo.Id = funcVM.ArquivoId;
			funcionario.Arquivo.Nome = funcVM.ArquivoNome;
			funcionario.Arquivo.TemporarioNome = funcVM.ArquivoTemporarioNome;

			funcVM.ListaCargos = funcVM.ListaCargos ?? new List<String>();
			funcVM.ListaSetores = funcVM.ListaSetores ?? new List<Setor>();
			funcVM.papeis = funcVM.papeis ?? new List<PapeisVME>();

			funcionario.Cargos.RemoveAll(x => !funcVM.ListaCargos.Exists(y => y == x.Id.ToString()));
			funcionario.Cargos.AddRange(
				_busLista.Cargos
				.Where(x =>
					funcVM.ListaCargos.Contains(x.Id.ToString()) &&
					!funcionario.Cargos.Exists(y => y.Id == x.Id)));

			funcVM.ListaSetores.ForEach(x => x.Nome = _busLista.SetoresComSigla.Single(y => y.Id == x.Id).Nome);
			funcionario.Setores.RemoveAll(x => !funcVM.ListaSetores.Exists(y => y.Id == x.Id));
			funcionario.Setores.AddRange(
				funcVM.ListaSetores
				.Where(x => !funcionario.Setores.Exists(y => y.Id == x.Id)));

			foreach (var item in funcVM.papeis)
			{
				if (item.IsAtivo)
				{
					if (!funcionario.Papeis.Exists(x => x.Id == item.Papel.Id) &&
						_bus.PapeisFuncionario.Exists(y => y.Id == item.Papel.Id))
					{
						funcionario.Papeis.Add(_bus.PapeisFuncionario.Single(y => y.Id == item.Papel.Id));
					}
				}
				else
				{
					funcionario.Papeis.RemoveAll(x => x.Id == item.Papel.Id);
				}
			}

			if (_bus.Salvar(funcionario, funcVM.Senha, funcVM.ConfirmarSenha))
			{
				return RedirectToAction("Criar", Validacao.QueryParamSerializer());
			}

			CriarVM viewModel = new CriarVM(_busLista.Cargos, _busLista.SetoresComSigla);
			viewModel.Funcionario = funcionario;
			viewModel.CpfValido = true;

			viewModel.Papeis = _bus.PapeisFuncionario.
				Select(x => new PapeisVME() { Papel = x, IsAtivo = viewModel.Funcionario.Papeis.Any(y => y.Id == x.Id) }).ToList();

			viewModel.TextoPermissoes = TextoPermissoes(viewModel.Papeis);

			return View(viewModel);
		}

		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioCriar })]
		public ActionResult VerificarCpf(String Cpf)
		{
			CriarVM viewModel = new CriarVM(_busLista.Cargos, _busLista.SetoresComSigla);
			viewModel.CpfValido = _bus.VerificarCpf(Cpf);

			if (viewModel.CpfValido)
			{
				viewModel.Funcionario = _bus.Obter(Cpf) ?? new Funcionario();

				if (viewModel.Funcionario != null && viewModel.Funcionario.Id > 0)
				{
					Validacao.Add(Mensagem.Funcionario.CpfEncontrado);
					return RedirectToAction("Editar/" + viewModel.Funcionario.Id, Validacao.QueryParamSerializer());
				}
			}

			List<Papel> lp = _bus.PapeisFuncionario;

			viewModel.Papeis = lp.
				Select(x => new PapeisVME() { Papel = x, IsAtivo = viewModel.Funcionario.Papeis.Any(y => y.Id == x.Id) }).ToList();

			viewModel.TextoPermissoes = TextoPermissoes(viewModel.Papeis);

			return View("Criar", viewModel);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioEditar })]
		public ActionResult Editar(int id)
		{
			Funcionario func = _bus.Obter(id);
			EditarVM viewModel = new EditarVM(_busLista.Cargos, _busLista.SetoresComSigla);

			viewModel.Funcionario = func;

			if (func != null)
			{
				List<Papel> lp = _bus.PapeisFuncionario;
				viewModel.Papeis = lp.
					Select(x => new PapeisVME() { Papel = x, IsAtivo = viewModel.Funcionario.Papeis.Any(y => y.Id == x.Id) }).ToList();

				viewModel.TextoPermissoes = TextoPermissoes(viewModel.Papeis);
			}

			return View(viewModel);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioEditar })]
		public ActionResult Editar(FuncionarioVM funcVM)
		{
			Funcionario funcionario = _bus.Obter(funcVM.Cpf) ?? new Funcionario();

			if (funcionario.Arquivo.Id.HasValue && !funcVM.ArquivoId.HasValue)
			{
				Validacao.Add(Mensagem.Funcionario.AssinaturaObrigatoria);
			}
			
			funcionario.AlterarSenha = funcVM.AlterarSenha;
			funcionario.Usuario.Login = funcVM.Login;
			funcionario.Nome = funcVM.Nome;
			funcionario.Email = funcVM.Email;

			funcionario.Arquivo.ContentType = funcVM.ArquivoContentType;
			funcionario.Arquivo.Extensao = funcVM.ArquivoExtensao;
			funcionario.Arquivo.Id = funcVM.ArquivoId;
			funcionario.Arquivo.Nome = funcVM.ArquivoNome;
			funcionario.Arquivo.TemporarioNome = funcVM.ArquivoTemporarioNome;
			
			//funcionario.ArquivoJSon = ViewModelHelper.JsSerializer.Serialize(funcionario.Arquivo);

			//vm.Modelo.Arquivo = vm.Modelo.Arquivo ?? new Arquivo();
			//vm.ArquivoJSon = ViewModelHelper.JsSerializer.Serialize(vm.Modelo.Arquivo);
			//ViewModelHelper.JsSerializer.Deserialize<Arquivo>(anexo.ArquivoJson);



			funcVM.ListaCargos = funcVM.ListaCargos ?? new List<String>();
			funcVM.ListaSetores = funcVM.ListaSetores ?? new List<Setor>();
			funcVM.papeis = funcVM.papeis ?? new List<PapeisVME>();

			funcionario.Cargos.RemoveAll(x => !funcVM.ListaCargos.Exists(y => y == x.Id.ToString()));
			funcionario.Cargos.AddRange(
				_busLista.Cargos
				.Where(x =>
					funcVM.ListaCargos.Contains(x.Id.ToString()) &&
					!funcionario.Cargos.Exists(y => y.Id == x.Id)));

			funcionario.Setores.RemoveAll(x => !funcVM.ListaSetores.Exists(y => y.Id == x.Id));
			funcionario.Setores.AddRange(
				funcVM.ListaSetores
				.Where(x => !funcionario.Setores.Exists(y => y.Id == x.Id)));
			List<Setor> lstSetorAtual = _busLista.SetoresComSiglaAtuais;

			foreach (var item in funcionario.Setores)
			{
				item.EhResponsavel = funcVM.ListaSetores.Single(x => x.Id == item.Id).EhResponsavel;

				item.Nome = lstSetorAtual.Single(x => x.Id == item.Id).Nome;
				if (item.EhResponsavel)
				{
					item.Responsavel = funcionario.Id;
				}
				else
				{
					item.Responsavel = lstSetorAtual.Single(x => x.Id == item.Id).Responsavel;
				}
			}

			if (funcVM.ListaSetores.Count == 0)
				funcionario.Setores.Clear();

			foreach (var item in funcVM.papeis)
			{
				if (item.IsAtivo)
				{
					if (!funcionario.Papeis.Exists(x => x.Id == item.Papel.Id) &&
						_bus.PapeisFuncionario.Exists(y => y.Id == item.Papel.Id))
					{
						funcionario.Papeis.Add(item.Papel);
					}
				}
				else
				{
					funcionario.Papeis.RemoveAll(x => x.Id == item.Papel.Id);
				}
			}

			if (_bus.Salvar(funcionario, funcVM.Senha, funcVM.ConfirmarSenha))
			{
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}

			EditarVM viewModel = new EditarVM(_busLista.Cargos, _busLista.SetoresComSigla);
			viewModel.Funcionario = funcionario;

			viewModel.Papeis = _bus.PapeisFuncionario.
				Select(x => new PapeisVME() { Papel = x, IsAtivo = viewModel.Funcionario.Papeis.Any(y => y.Id == x.Id) }).ToList();

			viewModel.TextoPermissoes = TextoPermissoes(viewModel.Papeis);

			return View(viewModel);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioVisualizar })]
		public ActionResult Visualizar(int id)
		{
			Funcionario func = _bus.Obter(id);
			VisualizarVM viewModel = new VisualizarVM();
			viewModel.Funcionario = func;

			if (func != null)
			{
				viewModel.Papeis = _bus.PapeisFuncionario.Select(x => new PapeisVME() { Papel = x, IsAtivo = viewModel.Funcionario.Papeis.Any(y => y.Id == x.Id) }).ToList();
				viewModel.TextoPermissoes = TextoPermissoes(viewModel.Papeis);
			}

			if (Request.IsAjaxRequest())
			{
				return PartialView("VisualizarPartial", viewModel);
			}

			return View(viewModel);
		}

		#endregion

		#region Alterar Situação

		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioAlterarSituacao })]
		public ActionResult AlterarSituacao(int id)
		{
			AlterarSituacaoVM viewModel = new AlterarSituacaoVM(_bus.Situacoes.Where(x => x.Id != 3 && x.Id != 5 && x.Id != 6).ToList());

			Funcionario funcionario = _bus.Obter(id);
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
		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioAlterarSituacao })]
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

			AlterarSituacaoVM viewModel = new AlterarSituacaoVM(_bus.Situacoes.Where(x => x.Id != 3 && x.Id != 5 && x.Id != 6).ToList());
			viewModel.NovaSituacaoId = novaSituacao;
			viewModel.Id = id;

			Funcionario funcionario = _bus.Obter(id);
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

		#region Alterar Funcionário

		public ActionResult AlterarFuncionario(int id)
		{
			if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
			{
				EditarVM viewModel = new EditarVM();
				EtramiteIdentity func = User.Identity as EtramiteIdentity;
				if (func == null || func.FuncionarioTipo != 3) // se não for "funcionário" mostra mensagem
				{
					Validacao.Add(Mensagem.Funcionario.SemPermissao);
				}
				else
				{
					if (_bus.VerificarAlterarFuncionario(func.FuncionarioId))
					{
						viewModel.Funcionario = _bus.Obter(func.FuncionarioId);
					}
				}

				return View("AlterarFuncionario", viewModel);
			}

			return Redirect(FormsAuthentication.LoginUrl);
		}

		[HttpPost]
		public ActionResult AlterarFuncionario(int id, FuncionarioVM funcVM)
		{
			if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
			{
				EditarVM viewModel = new EditarVM();
				EtramiteIdentity func = User.Identity as EtramiteIdentity;
				if (func == null || func.FuncionarioTipo != 3) // se não for "funcionário" mostra mensagem
				{
					Validacao.Add(Mensagem.Funcionario.SemPermissao);
				}
				else
				{
					if (_bus.VerificarAlterarFuncionario(id))
					{
						viewModel.Funcionario = _bus.Obter((User.Identity as EtramiteIdentity).FuncionarioId);
						_bus.AlterarSenhaFuncionario(viewModel.Funcionario, funcVM.Senha, funcVM.ConfirmarSenha);
					}
				}
				return View("AlterarFuncionario", viewModel);
			}
			else
			{
				return Redirect(FormsAuthentication.LoginUrl);
			}
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioCriar, ePermissao.FuncionarioEditar })]
		public ActionResult VerificarResponsavelSetor(int idSetor)
		{
			bool temResponsavel = _bus.VerificarResponsavelSetor(idSetor);
			return Json(new { TemResponsavel = temResponsavel, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.FuncionarioCriar, ePermissao.FuncionarioEditar, ePermissao.FuncionarioListar })]
		public string TextoPermissoes(List<PapeisVME> papeis)
		{
			List<String> texto = new List<string>();
			String permissoes = string.Empty;

			List<Papel> lstPapeis = _bus.PapeisFuncionario;

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
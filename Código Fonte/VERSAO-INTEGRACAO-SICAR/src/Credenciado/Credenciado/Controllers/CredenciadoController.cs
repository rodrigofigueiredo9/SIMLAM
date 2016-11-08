using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCredenciado;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPessoa;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class CredenciadoController : DefaultController
	{
		#region Propriedades

		CredenciadoBus _bus = new CredenciadoBus(new CredenciadoValidar());

		CredenciadoValidar _validar = new CredenciadoValidar();

		#endregion

		#region Criar e Reativar Acesso

		public ActionResult Index(string chave)
		{
			CredenciadoVM vm = new CredenciadoVM() { Chave = chave };
			vm.Credenciado = _bus.ObterCredenciado(vm.Chave) ?? new CredenciadoPessoa();

			return View("Ativar", vm);
		}

		public ActionResult Verificar(CredenciadoVM vm)
		{
			if (_validar.VerificarChave(vm.Chave))
			{
				vm.Credenciado = _bus.ObterCredenciado(vm.Chave) ?? new CredenciadoPessoa();
				vm.Login = vm.Credenciado.Usuario.Login;
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "AtivarPartial", vm) }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult GerenciarAcesso(CredenciadoVM vm)
		{
			if (_bus.GerenciarAcesso(vm.Chave, vm.Login, vm.Senha, vm.ConfirmarSenha))
			{
				Validacao.Add(Mensagem.Credenciado.CredenciadoAtivado);
				return Json(new { @EhValido = Validacao.EhValido, @UrlRedirecionar = Url.Action("Index", "Home", Validacao.QueryParamSerializer()) }, JsonRequestBehavior.AllowGet);
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Alterar Dados

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AlterarDados()
		{
			if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
			{
				CredenciadoVM vm = new CredenciadoVM();
				vm.Credenciado = _bus.Obter((HttpContext.User.Identity as EtramiteIdentity).FuncionarioId);
				vm.OrgaosParceiros = ViewModelHelper.CriarSelectList(_bus.ObterOrgaosParceirosLst());
				vm.OrgaosParceirosUnidades = ViewModelHelper.CriarSelectList(_bus.ObterUnidadesLst(vm.Credenciado.OrgaoParceiroId));
				vm.Credenciado.Pessoa = _bus.ObterPessoaCredenciado(vm.Credenciado.Pessoa.Id);

				vm.PessoaVM.Pessoa = vm.Credenciado.Pessoa;
				vm.PessoaVM.TipoCadastro = vm.Credenciado.Pessoa.IsFisica ? 1 : 0;
				vm.PessoaVM.CpfCnpjValido = true;
				vm.PessoaVM.IsCredenciado = true;
				vm.PessoaVM.ExibirMensagensPartial = true;
				vm.PessoaVM.OcultarLimparPessoa = true;
				vm.PessoaVM.OcultarIsCopiado = true;
				vm.PessoaVM.UrlAcao = Url.Action("AlterarDados", "Credenciado");

				CarregaCampos(vm);
				PreencheSalvarVM(vm.PessoaVM);

				return View(vm);
			}

			return Redirect(FormsAuthentication.LoginUrl);
		}

		[HttpPost]
		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AlterarDados(CredenciadoVM vm)
		{
			if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
			{
				CarregaCampos(vm);

				if (vm.Credenciado.Pessoa.IsJuridica)
				{
					for (int i = 0; i < vm.Credenciado.Pessoa.Juridica.Representantes.Count; i++)
					{
						vm.Credenciado.Pessoa.Juridica.Representantes[i] = _bus.ObterPessoaCredenciado(vm.Credenciado.Pessoa.Juridica.Representantes[i].Id);
						vm.Credenciado.Pessoa.Juridica.Representantes[i].IsCopiado = false;
					}
				}

				if (_bus.AlterarDados(vm.Credenciado, vm.Senha, vm.ConfirmarSenha))
				{
					Validacao.Add(Mensagem.Credenciado.Salvar);
					string urlRedireciona = Url.Action("Index", "Home");
					urlRedireciona += "?Msg=" + Validacao.QueryParam();
					return Json(new { IsPessoaSalva = true, @Pessoa = vm.Credenciado.Pessoa, Msg = Validacao.Erros, UrlRedireciona = urlRedireciona });
				}

				return Json(new { IsPessoaSalva = false, Msg = Validacao.Erros });
			}

			return Redirect(FormsAuthentication.LoginUrl);
		}

		#endregion

		#region Auxiliares

		[Permite(Tipo = ePermiteTipo.Logado)]
		public List<Contato> PreencheContato(ContatoVME contato)
		{
			List<Contato> meiosContatos = new List<Contato>();

			try
			{
				if (!string.IsNullOrEmpty(contato.TelefoneResidencial))
				{
					meiosContatos.Add(new Contato { Valor = contato.TelefoneResidencial, TipoContato = eTipoContato.TelefoneResidencial });
				}

				if (!string.IsNullOrEmpty(contato.TelefoneComercial))
				{
					meiosContatos.Add(new Contato { Valor = contato.TelefoneComercial, TipoContato = eTipoContato.TelefoneComercial });
				}

				if (!string.IsNullOrEmpty(contato.TelefoneCelular))
				{
					meiosContatos.Add(new Contato { Valor = contato.TelefoneCelular, TipoContato = eTipoContato.TelefoneCelular });
				}

				if (!string.IsNullOrEmpty(contato.TelefoneFax))
				{
					meiosContatos.Add(new Contato { Valor = contato.TelefoneFax, TipoContato = eTipoContato.TelefoneFax });
				}

				if (!string.IsNullOrEmpty(contato.Email))
				{
					meiosContatos.Add(new Contato { Valor = contato.Email, TipoContato = eTipoContato.Email });
				}

				if (!string.IsNullOrEmpty(contato.Nome))
				{
					meiosContatos.Add(new Contato { Valor = contato.Nome, TipoContato = eTipoContato.NomeContato });
				}

				return meiosContatos;
			}
			catch (Exception exc)
			{
				Validacao.AddAdvertencia(exc.Message);
			}

			return null;
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
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
							contatoVME.TelefoneResidencial = cont.Valor;
							break;

						case eTipoContato.TelefoneCelular:
							contatoVME.TelefoneCelular = cont.Valor;
							break;

						case eTipoContato.TelefoneFax:
							contatoVME.TelefoneFax = cont.Valor;
							break;

						case eTipoContato.TelefoneComercial:
							contatoVME.TelefoneComercial = cont.Valor;
							break;

						case eTipoContato.Email:
							contatoVME.Email = cont.Valor;
							break;

						case eTipoContato.NomeContato:
							contatoVME.Nome = cont.Valor;
							break;
					}
				}
			}

			return contatoVME;
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public CredenciadoVM CarregaCampos(CredenciadoVM vm)
		{
			if (vm.Pessoa.MeiosContatos == null || vm.Pessoa.MeiosContatos.Count == 0)
			{
				vm.Pessoa.MeiosContatos = PreencheContato(vm.Contato);
			}

			if (!vm.Pessoa.IsJuridica)
			{
				if (vm.Pessoa.Fisica.DataNascimento != null)
				{
					vm.PessoaVM.DataNascimento = vm.Pessoa.Fisica.DataNascimento.Value.ToShortDateString();
				}
				else
				{
					vm.Pessoa.Fisica.DataNascimento = ValidacoesGenericasBus.ParseData(vm.DataNascimento);
				}
			}

			if (vm.Pessoa.MeiosContatos != null && vm.Pessoa.MeiosContatos.Count > 0)
			{
				vm.Contato = CarregaMeiosContatos(vm.Pessoa.MeiosContatos);
			}

			return vm;
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public Pessoa PreencheSalvarVM(PessoaVM vm)
		{
			vm.EstadosCivis = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.EstadosCivil, true);
			vm.Sexos = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.Sexos, true);
			vm.Profissoes = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.Profissoes, true);
			vm.OrgaoClasses = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.OrgaosClasse, true);
			vm.Estados = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.Estados, true);

			if (vm.Pessoa != null || vm.Pessoa.Endereco.EstadoId > 0)
			{
				vm.Municipios = (List<SelectListItem>)(from m in ViewModelHelper.CriarSelectList(ListaCredenciadoBus.Municipios(vm.Pessoa.Endereco.EstadoId), true) orderby m.Text ascending select m).ToList();
			}
			else
			{
				vm.Municipios = new List<SelectListItem>() { ViewModelHelper.SelecionePadrao };
			}

			if (vm.Pessoa.Fisica != null && vm.Pessoa.Fisica.Profissao != null)
			{
				if (vm.Pessoa.Fisica.Profissao.Id > 0)
				{
					vm.Pessoa.Fisica.Profissao.ProfissaoTexto = _bus.ObterProfissao(vm.Pessoa.Fisica.Profissao.Id);
				}
				else
				{
					vm.Pessoa.Fisica.Profissao.Id = 0;
					vm.Pessoa.Fisica.Profissao.IdRelacionamento = 0;
					vm.Pessoa.Fisica.Profissao.ProfissaoTexto = "*** Associar uma profissão ***";
				}
			}

			return vm.Pessoa;
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult CredenciadoAssociar()
		{
			ListarVM vm = new ListarVM(ListaCredenciadoBus.QuantPaginacao, ListaCredenciadoBus.CredenciadoTipos, ListaCredenciadoBus.CredenciadoSituacoes);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.IsAssociar = true;
			return PartialView("ListarFiltros", vm);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, ListaCredenciadoBus.CredenciadoTipos, ListaCredenciadoBus.CredenciadoSituacoes, vm.Paginacao.QuantPaginacao);

			var resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;


			return Json(new { Msg = Validacao.Erros, Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}
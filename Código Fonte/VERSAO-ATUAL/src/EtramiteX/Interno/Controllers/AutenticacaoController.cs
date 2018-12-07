using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.Interfaces;
using Tecnomapas.EtramiteX.Interno.Servicos;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMAutenticacao;
using Tecnomapas.EtramiteX.Perfil.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Business;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class AutenticacaoController : DefaultController
	{
		public IFormsAuthenticationService formsAuthenticationService;

		public AutenticacaoController() : this(new FormsAuthenticationService()) { }

		public AutenticacaoController(IFormsAuthenticationService formsAuthenticationService)
		{
			this.formsAuthenticationService = formsAuthenticationService;
		}

		public bool InsereCookieEPTV()
		{
			bool inseriu = false;

			HttpCookie cookieEPTV = new HttpCookie("eptv");
			cookieEPTV.Value = DateTime.Now.ToString();
			cookieEPTV.Expires = DateTime.Now.AddDays(1);
			Response.Cookies.Add(cookieEPTV);

			if (Request.Cookies["eptv"] != null)
			{
				inseriu = true;
			}

			return inseriu;
		}

		public String getAlterarSenhaMsg(String login)
		{
			FuncionarioBus credenciadoBus = new FuncionarioBus(new FuncionarioValidar());

			return credenciadoBus.AlterarSenhaMensagem(login);
		}

		public bool HasToAlterarSenha(String login)
		{
			return this.getAlterarSenhaMsg(login) != String.Empty;
		}

		public ActionResult LogOn(string msg)
		{
			if (Request.IsAjaxRequest())
			{
				return PartialView("LogOnPartial", new LogonVM() { IsAjaxRequest = Request.IsAjaxRequest() });
			}
			else
			{
				return View(new LogonVM() { });
			}
		}

		[HttpPost]
		public ActionResult LogOn(string login, string senha, bool? alterarSenha, string novaSenha, string confirmarNovaSenha, string returnUrl)
		{
			LogonVM viewModel = new LogonVM() { AlterarSenha = alterarSenha ?? false };
			viewModel.IsAjaxRequest = Request.IsAjaxRequest();

			try
			{
				string strSessionId = null;
				if (!GerenciarAutenticacao.ValidarLogOn(login, senha, out strSessionId))
				{
					if (Request.IsAjaxRequest())
					{
						return PartialView("LogOnPartial", new LogonVM() { IsAjaxRequest = Request.IsAjaxRequest() });
					}
					else
					{
						return View(viewModel);
					}
				}

				FuncionarioBus busFuncionario = new FuncionarioBus(new FuncionarioValidar());
				string alterarSenhaMsg = busFuncionario.AlterarSenhaMensagem(login);

				if (!String.IsNullOrEmpty(alterarSenhaMsg))
				{
					Validacao.Erros.Clear();

					if (!viewModel.AlterarSenha || !busFuncionario.AlterarSenha(login, senha, novaSenha, confirmarNovaSenha))
					{
						viewModel = new LogonVM() { AlterarSenha = true, AlterarSenhaMsg = alterarSenhaMsg };
						if (Request.IsAjaxRequest())
						{
							return PartialView("LogOnPartial", new LogonVM() { IsAjaxRequest = Request.IsAjaxRequest() });
						}
						else
						{
							return View(viewModel);
						}
					}
				}

				this.formsAuthenticationService.SetAuthCookie(login, true);

				FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, login, DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), true, strSessionId);

				HttpCookie cookie = null;

				String cookieName = this.formsAuthenticationService.FormsCookieName;

				if (cookieName != null)
				{
					cookie = Request.Cookies[cookieName];
				}

				if (cookie != null) {
					cookie.Value = this.formsAuthenticationService.Encrypt(ticket);
				}

				//Cria o cookie que é usado para a verificação periódica de E-PTVs aguardando análise.
				#region Insere cookie EPTV

				InsereCookieEPTV();

				#endregion Insere cookie EPTV

				GerenciarAutenticacao.CarregarUser(login);

				BusMenu.Menu = null;

				//Gera a mensagem de alerta de E-PTV (se houver)
				#region Alerta de E-PTV

				PTVBus _busPTV = new PTVBus();
				_busPTV.VerificaAlertaEPTV();

				#endregion Alerta de E-PTV

				if (Request.IsAjaxRequest())
				{
					return PartialView("LogOnPartial", new LogonVM() { IsAjaxRequest = Request.IsAjaxRequest() });
				}
				else if (!String.IsNullOrEmpty(returnUrl))
				{
					return Redirect(Validacao.QueryParamSerializer(HttpUtility.UrlDecode(returnUrl)));
				}
				else
				{
					return RedirectToAction("Index", "Home", Validacao.QueryParamSerializer());
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			if (Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Home", Validacao.QueryParamSerializer());
			}
			else
			{
				return View(viewModel);
			}
		}

		public ActionResult LogOff(string msg)
		{
			GerenciarAutenticacao.Deslogar();
			FormsAuthentication.SignOut();

			return RedirectToAction("Logon", "Autenticacao");
		}
	}
}
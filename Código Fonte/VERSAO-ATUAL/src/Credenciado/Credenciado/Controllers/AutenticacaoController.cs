using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.Model.Security.Interfaces;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAutenticacao;
using Tecnomapas.EtramiteX.Perfil.Business;
using Tecnomapas.EtramiteX.Credenciado.Services;
using Tecnomapas.EtramiteX.Credenciado.Interfaces;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class AutenticacaoController : DefaultController
	{
		public IFormsAuthenticationService formsAuthenticationService;

		public AutenticacaoController() : this(new FormsAuthenticationService()) { }

		public AutenticacaoController(IFormsAuthenticationService formsAuthenticationService)
		{
			this.formsAuthenticationService = formsAuthenticationService;
		}

		public ActionResult LogOn(string msg)
		{
			if (Request.IsAjaxRequest())
				return PartialView("LogOnPartial", new LogonVM() { IsAjaxRequest = Request.IsAjaxRequest() });

			return View(new LogonVM());
		}

		public String getAlterarSenhaMsg(String login)
		{
			CredenciadoBus credenciadoBus = new CredenciadoBus(new CredenciadoValidar());

			return credenciadoBus.AlterarSenhaMensagem(login);
		}

		public bool HasToAlterarSenha(String login)
		{
			return this.getAlterarSenhaMsg(login) != String.Empty;
		}

		private PartialViewResult getAjaxLogOnPartial()
		{
			return PartialView("LogOnPartial", new LogonVM() {
				IsAjaxRequest = Request.IsAjaxRequest()
			});
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
						return this.getAjaxLogOnPartial();

					return View(viewModel);
				}

				CredenciadoBus credenciadoBus = new CredenciadoBus(new CredenciadoValidar());

				string alterarSenhaMsg = credenciadoBus.AlterarSenhaMensagem(login);

				if (!String.IsNullOrEmpty(alterarSenhaMsg))
				{
					Validacao.Erros.Clear();

					if (!viewModel.AlterarSenha || !credenciadoBus.AlterarSenha(login, senha, novaSenha, confirmarNovaSenha))
					{
						viewModel = new LogonVM() { AlterarSenha = true, AlterarSenhaMsg = alterarSenhaMsg };

						if (Request.IsAjaxRequest())
							return this.getAjaxLogOnPartial();

						return View(viewModel);
					}
				}

				this.formsAuthenticationService.SetAuthCookie(login, true);

				FormsAuthenticationTicket ticket =
					new FormsAuthenticationTicket(1, login, DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), true, strSessionId);

				HttpCookie cookie = null;
				String cookieName = this.formsAuthenticationService.FormsCookieName;

				if (cookieName != null) {
					cookie = Request.Cookies[cookieName];
				}

				if (cookie != null) {
					cookie.Value = this.formsAuthenticationService.Encrypt(ticket);
				}

				GerenciarAutenticacao.CarregarUser(login);

				BusMenu.Menu = null;

				if (Request.IsAjaxRequest())
					return PartialView("LogOnPartial", new LogonVM() { IsAjaxRequest = Request.IsAjaxRequest() });

				if (!String.IsNullOrEmpty(returnUrl))
					return Redirect(Validacao.QueryParamSerializer(HttpUtility.UrlDecode(returnUrl)));

				return RedirectToAction("Index", "Home", Validacao.QueryParamSerializer()) as RedirectToRouteResult;;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			if (Request.IsAjaxRequest())
				return RedirectToAction("Index", "Home", Validacao.QueryParamSerializer());

			return View(viewModel);
		}

		public ActionResult LogOff(string msg)
		{
			GerenciarAutenticacao.Deslogar();
			FormsAuthentication.SignOut();

			return RedirectToAction("Logon", "Autenticacao");
		}
	}
}
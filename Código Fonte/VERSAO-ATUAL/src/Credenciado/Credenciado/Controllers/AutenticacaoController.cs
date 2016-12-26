using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAutenticacao;
using Tecnomapas.EtramiteX.Perfil.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class AutenticacaoController : DefaultController
	{
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

				CredenciadoBus credenciadoBus = new CredenciadoBus(new CredenciadoValidar());

				string alterarSenhaMsg = credenciadoBus.AlterarSenhaMensagem(login);
				if (!String.IsNullOrEmpty(alterarSenhaMsg))
				{
					Validacao.Erros.Clear();

					if (!viewModel.AlterarSenha || !credenciadoBus.AlterarSenha(login, senha, novaSenha, confirmarNovaSenha))
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

				FormsAuthentication.SetAuthCookie(login, true);

				FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, login, DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), true, strSessionId);
				HttpCookie cookie = null;

				if (FormsAuthentication.FormsCookieName != null) {
					cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
				}

				if (cookie != null) {
					cookie.Value = FormsAuthentication.Encrypt(ticket);
				}

				GerenciarAutenticacao.CarregarUser(login);

				BusMenu.Menu = null;

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
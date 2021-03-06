﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Gerencial.Model.Security;

namespace Gerencial
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("areas/navegadores/{*restOfUrl}");

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RegisterRoutes(RouteTable.Routes);
			GeradorAspose.Autorizacao();

			ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
			ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
			HttpCookie c = Request.Cookies[FormsAuthentication.FormsCookieName];
			if (c == null) return;

			FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(c.Value);
			string login = ticket.Name;
			string sessionId = ticket.UserData;

			GerenciarAutenticacao.CarregarUser(login, sessionId);
		}
	}
}
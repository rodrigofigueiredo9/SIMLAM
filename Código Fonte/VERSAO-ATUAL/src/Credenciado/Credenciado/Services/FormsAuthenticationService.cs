using System;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.EtramiteX.Credenciado.Interfaces;

namespace Tecnomapas.EtramiteX.Credenciado.Services
{
	public class FormsAuthenticationService : IFormsAuthenticationService
	{
		public void SetAuthCookie(string value, bool isPersistent)
		{
			FormsAuthentication.SetAuthCookie(value, isPersistent);
		}

		public string Encrypt(FormsAuthenticationTicket ticket)
		{
			return FormsAuthentication.Encrypt(ticket);
		}

		public string FormsCookieName
		{
			get { return FormsAuthentication.FormsCookieName; }
		}
	}
}
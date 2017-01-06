using System;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnomapas.EtramiteX.Credenciado.Interfaces;

namespace Tests.Fakes
{
	public class FakeBuggyFormsAuthenticationService : IFormsAuthenticationService
	{
		public void SetAuthCookie(string value, bool isPersistent)
		{
			Console.Write("Set AuthCookie Called with value: " + value);
			throw new Exception("Erro proposital.");
		}

		public string Encrypt(FormsAuthenticationTicket ticket)
		{
			return "any-text";
		}

		public string FormsCookieName
		{
			get { return null; }
		}
	}
}

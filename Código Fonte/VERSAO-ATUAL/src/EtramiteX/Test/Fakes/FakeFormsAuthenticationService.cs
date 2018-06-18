using System;
using System.Web.Security;
using Tecnomapas.EtramiteX.Interno.Interfaces;

namespace Tests.Fakes
{
	public class FakeFormsAuthenticationService : IFormsAuthenticationService
	{
		public void SetAuthCookie(string value, bool isPersistent)
		{
			Console.Write("Set AuthCookie Called with value: " + value);
		}

		public string Encrypt(FormsAuthenticationTicket ticket)
		{
			return "would-be-a-encrypted-value";
		}

		public string FormsCookieName
		{
			get { return null; }
		}
	}
}

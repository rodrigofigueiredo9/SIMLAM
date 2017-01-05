using System;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnomapas.EtramiteX.Credenciado.Interfaces;

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
    }
}

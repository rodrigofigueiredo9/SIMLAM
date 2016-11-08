using System;
using System.Web;
using System.Web.Security;
using Tecnomapas.Geobases.WebServices.Models.Data;

namespace Tecnomapas.Geobases.WebServices.Models.Business
{
    public static class AutenticacaoBus
    {
        public static bool Logar(string chaveAutenticacao)
        {   bool autenticado = false;
            AutenticacaoDa da = new AutenticacaoDa();
          
            autenticado = da.Autenticar(chaveAutenticacao);
            if (autenticado)
            {
                FormsAuthentication.SetAuthCookie(chaveAutenticacao, true);

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, chaveAutenticacao, DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), true, string.Empty);
                HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                cookie.Value = FormsAuthentication.Encrypt(ticket);

            }
            return autenticado;
        }
    }
}
﻿using System;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.EtramiteX.Credenciado.Interfaces
{
    public interface IFormsAuthenticationService
    {
        void SetAuthCookie(string value, bool isPersistent);
        string Encrypt(FormsAuthenticationTicket ticket);
    }
}

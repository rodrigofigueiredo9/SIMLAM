using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Security.Interfaces
{
    public interface IGerenciarAutenticacao
    {
        bool ValidarLogin(string login, string senha, out string sessionId);
    }
}

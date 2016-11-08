using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.DesenhadorWS.Models.DataAcess;
using Tecnomapas.DesenhadorWS.Models.Entities;

namespace Tecnomapas.DesenhadorWS.Models.Business
{
    public class NavegadorBusiness
    {
        NavegadorDa da;

        public NavegadorBusiness()
        {
            da = new NavegadorDa();
        }

        #region Buscar
        public Navegador Buscar(int idNavegador, int idProjeto)
        {
            return da.Buscar(idNavegador, idProjeto); 
        }
        #endregion
    }
}
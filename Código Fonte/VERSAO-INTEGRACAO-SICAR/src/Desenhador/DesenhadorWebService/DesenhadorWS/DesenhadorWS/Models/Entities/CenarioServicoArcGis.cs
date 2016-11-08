using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class CenarioServicoArcGis
    {
        public int Id;
        public int Indice;
        public string Nome;
        public string[] Servicos;
        public bool IsPrincipal;
        public bool ExibirLogotipo;

        public CenarioServicoArcGis()
        {
            Id = 0;
            Nome = string.Empty;           
            IsPrincipal = false;
            ExibirLogotipo = true;
        }
    }
}
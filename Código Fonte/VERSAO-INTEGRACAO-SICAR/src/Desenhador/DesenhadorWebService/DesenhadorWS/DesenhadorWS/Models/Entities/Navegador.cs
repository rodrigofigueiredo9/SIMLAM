using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class Navegador
    {
        public int Id;
        public string Nome;
        public ServicoArcGis[] Servicos;
        public CenarioServicoArcGis[] Cenarios;
        public string[] Filtros;
        public string[] ProjetosAssociados;

        public Navegador()
        {
            Id = 0;
            Nome = string.Empty;
        }
    }
}
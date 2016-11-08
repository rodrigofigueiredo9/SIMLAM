using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class ServicoArcGis
    {
        public int Id { get; set; }
        public int Indice { get; set; }
        public string Nome { get; set; }
        public string Url { get; set; }
        public bool IsCacheado { get; set; }
        public bool IsPrincipal { get; set; }
        public bool Identificar { get; set; }
        public bool GeraLegenda { get; set; }
        public int UltimoIdLayer { get; set; }
     
        public ServicoArcGis()
        {
            Id = 0;
            UltimoIdLayer = 0;
            Indice = 1;
            Nome = string.Empty;
            Url = string.Empty;
            IsCacheado = false;
            IsPrincipal = false;
            GeraLegenda = false;
            Identificar = false;
        }
    }
}
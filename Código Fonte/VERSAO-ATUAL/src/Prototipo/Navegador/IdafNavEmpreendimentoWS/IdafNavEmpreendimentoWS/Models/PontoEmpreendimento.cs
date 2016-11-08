using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdafNavEmpreendimentoWS.Models
{
    public class PontoEmpreendimento
    {
        public int id { get; set; }
        public string denominador { get; set; }
        public string segmento { get; set; }
        public string municipio { get; set; }
        public string atividade { get; set; }
        public string processos { get; set; }
        public Decimal x { get; set; }
        public Decimal y { get; set; }
    }
}
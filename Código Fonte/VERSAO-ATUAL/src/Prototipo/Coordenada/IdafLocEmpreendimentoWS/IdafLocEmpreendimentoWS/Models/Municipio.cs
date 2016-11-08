using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdafLocEmpreendimentoWS.Models
{
    public class Municipio
    {
        public int id { get; set; }
        public string nome { get; set; }
        public Decimal x { get; set; }
        public Decimal y { get; set; }
    }
}
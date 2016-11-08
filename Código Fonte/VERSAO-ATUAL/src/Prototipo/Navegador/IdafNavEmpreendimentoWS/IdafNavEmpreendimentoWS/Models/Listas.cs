using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdafNavEmpreendimentoWS.Models
{
    public class Listas
    {
        public List<Atividade> atividades { get; set; }
        public List<Segmento> segmentos { get; set; }
        public List<Municipio> municipios { get; set; }
    }
}
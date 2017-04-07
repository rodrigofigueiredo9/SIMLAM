using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria
{
    public class BloqueioLocalVistoria
    {
        public int Id { get; set; }
        public DateTime DiaInicio { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public DateTime DiaFim { get; set; }
        public string Tid { get; set; }

        public BloqueioLocalVistoria() { }
    }
}

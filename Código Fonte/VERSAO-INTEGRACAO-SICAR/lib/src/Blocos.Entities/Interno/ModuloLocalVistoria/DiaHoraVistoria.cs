using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria
{
    public class DiaHoraVistoria
    {
        public int Id { get; set; }
        public int DiaSemanaId { get; set; }
        public string DiaSemanaTexto { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public int Situacao { get; set; }
        public string Tid { get; set; }

        public DiaHoraVistoria() { }
    }
}

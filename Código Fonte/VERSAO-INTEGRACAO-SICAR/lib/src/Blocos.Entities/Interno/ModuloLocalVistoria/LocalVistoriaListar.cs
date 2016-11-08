using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria
{
    public class LocalVistoriaListar
    {
        public int SetorID { get; set; }
        public string SetorTexto { get; set; }
        public int DiaSemanaId { get; set; }
        public string DiaSemanaTexto { get; set; }

        public LocalVistoriaListar()
        {
        }
    }
}

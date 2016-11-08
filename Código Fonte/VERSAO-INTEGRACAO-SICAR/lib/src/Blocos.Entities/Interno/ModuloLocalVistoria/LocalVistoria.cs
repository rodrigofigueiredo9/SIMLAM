using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria
{
    public class LocalVistoria
    {
        public int SetorID { get; set; }
        public string SetorTexto { get; set; }


        private List<DiaHoraVistoria> _diasHorasVistoria = new List<DiaHoraVistoria>();

        public List<DiaHoraVistoria> DiasHorasVistoria
        {
            get { return _diasHorasVistoria; }
            set { _diasHorasVistoria = value; }
        }
        
        public LocalVistoria()		{
            DiasHorasVistoria = new List<DiaHoraVistoria>();
        }
    }
}

﻿using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria
{
	public class LocalVistoria
    {
        public int SetorID { get; set; }
        public string SetorTexto { get; set; }

        private List<BloqueioLocalVistoria> _lstBloqueios = new List<BloqueioLocalVistoria>();

        private List<DiaHoraVistoria> _diasHorasVistoria = new List<DiaHoraVistoria>();

        public List<DiaHoraVistoria> DiasHorasVistoria
        {
            get { return _diasHorasVistoria; }
            set { _diasHorasVistoria = value; }
        }

        public List<BloqueioLocalVistoria> Bloqueios
        {
            get { return _lstBloqueios; }
            set { _lstBloqueios = value; }
        }
        
        public LocalVistoria()		{
            DiasHorasVistoria = new List<DiaHoraVistoria>();
            _lstBloqueios = new List<BloqueioLocalVistoria>();
        }
    }
}

using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF
{
    public class SilviculturaPPFF
	{
		public Int32 Id { get; set; }
		public Int32 EmpreendimentoId { get; set; }
        public Int32 Atividade { get; set; }
        public String AreaTotal { get; set; }
        public String Tid { get; set; }
        public eFomentoTipo FomentoTipo { get; set; }

        private List<SilviculturaPPFFItem> _itens = new List<SilviculturaPPFFItem>();
        public List<SilviculturaPPFFItem> Itens
		{
            get { return _itens; }
            set { _itens = value; }
		}
	}
}

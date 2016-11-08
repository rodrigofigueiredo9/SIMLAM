using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado
{
    public class OrgaoParceiroConveniado
    {
        public Int32 Id { get; set; }
		public String Tid { get; set; }
        public String Nome { get; set; }
        public String Sigla{ get; set; }
        public String TermoNumeroAno { get; set; }
		public Int32 SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }
		public String SituacaoMotivo { get; set; }

		public String SiglaNome
		{
			get
			{
				return Sigla + " - " + Nome;
			}
		}

		private DateTecno _situacaoData = new DateTecno();
		public DateTecno SituacaoData
		{
			get { return _situacaoData; }
			set { _situacaoData = value; }
		}

        private DateTecno _diarioOficialData = new DateTecno();
        public DateTecno DiarioOficialData
        {
            get { return _diarioOficialData; }
            set { _diarioOficialData = value; }
        }

        private List<Unidade> _unidades = new List<Unidade>();
        public List<Unidade> Unidades
        {
            get { return _unidades; }
            set { _unidades = value; }
        }

    }
}

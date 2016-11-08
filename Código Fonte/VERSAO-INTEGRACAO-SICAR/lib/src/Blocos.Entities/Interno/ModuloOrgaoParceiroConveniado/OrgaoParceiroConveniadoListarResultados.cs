using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado
{
    public class OrgaoParceiroConveniadoListarResultados
    {
        public Int32 Id { get; set; }
        public String Nome { get; set; }
        public String Sigla { get; set; }
        public String SituacaoTexto { get; set; }
		public Int32 SituacaoId { get; set; }
	}
}

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class FiscalizacaoPDF
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }

		public String NumeroFiscalizacao { get; set; }
		public String DataFiscalizacao { get; set; }
		public String NumeroProcesso { get; set; }
		public Boolean InfracaoAutuada { get; set; }

		public FiscalizacaoPDF() { }
	}
}
using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao
{
	public class Fiscalizacao
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }
		public Int32? ProtocoloId { get; set; }
		public String ProtocoloTid { get; set; }
		public String NumeroFiscalizacao { get; set; }
		public String DataFiscalizacao { get; set; }//Data de Vistoria
		public String NumeroProcesso { get; set; }
		public Boolean InfracaoAutuada { get; set; }
	}
}
using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca
{
	public class Motosserra
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 ProprietarioId { get; set; }
		public String NotaFiscal { set; get; }
		public String NumeroRegistro { set; get; }
		public String NumeroFabricacao { set; get; }
		public String Marca { set; get; }
		public Int32 SituacaoId { get; set; }
	}
}

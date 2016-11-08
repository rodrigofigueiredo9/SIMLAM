

using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao
{
	public class ConverterDocumento
	{
		public int DocumentoId { get; set; }
		public string NumeroDocumento { get; set; }
		public string NumeroAutuacao { get; set; }
		public string DataAutuacao { get; set; }
		public bool PossuiSEP { get; set; }
		public Processo Processo { get; set; }

		public ConverterDocumento()
		{
			this.NumeroDocumento = 
			this.NumeroAutuacao =
			this.DataAutuacao = string.Empty;
			this.Processo = new Processo();
		}
	}
}

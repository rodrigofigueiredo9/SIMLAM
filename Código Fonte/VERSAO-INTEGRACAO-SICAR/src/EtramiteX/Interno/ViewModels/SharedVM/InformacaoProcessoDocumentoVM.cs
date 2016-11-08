namespace Tecnomapas.EtramiteX.Interno.ViewModels
{
	public enum TipoInformacacao {
		Visualizar,
		PDF
	}

	public class InformacaoProtocoloVM
	{
		public string Texto { get; set; }
		public TipoInformacacao Tipo { get; set; }
		public string Chave { get; set; }
		public object Valor { get; set; }
		public bool Mostrar { get; set; }
	}
}
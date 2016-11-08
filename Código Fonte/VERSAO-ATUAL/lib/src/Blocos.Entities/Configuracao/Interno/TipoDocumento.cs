namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class TipoDocumento : IListaValor
	{
		public int Id { get; set; }
		public string Texto { get; set; }
		public string NumeroDocumentoAnterior { get; set; }
		public bool Marcado { get; set; }
		public bool IsAtivo { get; set; }
		public int Codigo { get; set; }
		public int IdRelacionamento { get; set; }

		public TipoDocumento()
		{
			NumeroDocumentoAnterior = string.Empty;
		}
	}
}
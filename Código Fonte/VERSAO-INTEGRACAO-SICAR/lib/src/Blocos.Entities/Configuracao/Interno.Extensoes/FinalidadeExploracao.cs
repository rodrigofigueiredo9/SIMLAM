namespace Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes
{
	public class FinalidadeExploracao : IListaValor
	{
		public int Id { get; set; }
		public string Texto { get; set; }
		public bool IsAtivo { get; set; }
		public int Codigo { get; set; }
	}
}
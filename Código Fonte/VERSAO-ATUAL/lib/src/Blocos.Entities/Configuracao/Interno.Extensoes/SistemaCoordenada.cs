namespace Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes
{
	public class SistemaCoordenada : IListaValorString
	{
		public string Id { get; set; }
		public bool IsAtivo { get; set; }
		public string Texto { get; set; }
	}
}
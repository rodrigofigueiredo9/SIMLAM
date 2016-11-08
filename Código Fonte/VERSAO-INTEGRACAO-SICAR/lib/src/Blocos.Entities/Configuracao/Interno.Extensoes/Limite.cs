namespace Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes
{
	public class Limite : IListaValor
	{
		public int Id { set; get; }
		public string Texto { set; get; }
		public bool IsAtivo { set; get; }
	}
}
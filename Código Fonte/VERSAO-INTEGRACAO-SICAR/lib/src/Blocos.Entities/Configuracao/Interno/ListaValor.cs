namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class ListaValor : IListaValor
	{
		public int Id { get; set; }
		public string Texto { get; set; }
		public bool IsAtivo { get; set; }
	}
}
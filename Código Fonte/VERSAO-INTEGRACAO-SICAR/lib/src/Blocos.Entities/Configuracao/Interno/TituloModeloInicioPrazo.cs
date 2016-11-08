namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class TituloModeloInicioPrazo : IListaValor
	{
		public int Id { set; get; }
		public string Texto { set; get; }
		public bool IsAtivo { set; get; }
	}
}
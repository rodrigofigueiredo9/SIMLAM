namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class CoordenadaTipo : IListaValor
	{
		public int Id { get; set; }
		public bool IsAtivo { get; set; }
		public string Texto { get; set; }

		public CoordenadaTipo() { }
	}
}
namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class Lista : IListaValorString
	{
		public string Id { get; set; }
		public string Tid { get; set; }
		public string Texto { get; set; }
		public bool IsAtivo { get; set; }
		public string Codigo { get; set; }
		public int Tipo { get; set; }
	}
}
namespace Tecnomapas.Blocos.Entities.Configuracao
{
	public class ContatoLst : IListaValor
	{
		public int Id { get; set; }
		public bool IsAtivo { get; set; }
		public string Mascara { get; set; }
		public string Texto { get; set; }
		public string Tid { get; set; }
	}
}
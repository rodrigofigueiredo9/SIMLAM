namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class Modelo : IListaValor
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public string Texto { get; set; }
		public bool IsAtivo { get; set; }
	}
}
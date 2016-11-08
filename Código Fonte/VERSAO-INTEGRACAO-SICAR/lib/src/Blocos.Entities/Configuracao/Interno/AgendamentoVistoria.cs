namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class AgendamentoVistoria : IListaValor
	{
		public int Id { get; set; }
		public string Texto { get; set; }
		public bool IsAtivo { get; set; }
	}
}
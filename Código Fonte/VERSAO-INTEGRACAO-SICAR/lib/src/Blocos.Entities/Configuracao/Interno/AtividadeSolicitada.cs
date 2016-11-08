namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class AtividadeSolicitada : IListaValor
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public string Texto { get; set; }
		public bool IsAtivo { get; set; }
		public bool Situacao { get; set; }
		public int SetorId { get; set; }
		public string SetorTexto { get; set; }
		public string Categoria { get; set; }
	}
}
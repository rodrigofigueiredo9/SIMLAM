namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade
{
	public class TituloAssociadoEsp
	{
		public int Id { get; set; }
		public int? IdRelacionamento { get; set; }
		public int ModeloId { get; set; }
		public int ModeloCodigo { get; set; }
		public string ModeloTexto { get; set; }
		public string ModeloSigla { get; set; }
		public string TituloNumero { get; set; }
		public int Situacao { get; set; }
		public string SituacaoTexto { get; set; }
		public int EmpreendimentoId { get; set; }
		public bool Associado { get; set; }
	}
}
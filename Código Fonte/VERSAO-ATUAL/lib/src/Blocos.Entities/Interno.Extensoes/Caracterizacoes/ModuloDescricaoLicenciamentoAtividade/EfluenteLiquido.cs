namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade
{
	public class EfluenteLiquido
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }		
		public int TipoId { get; set; }
		public string TipoTexto { get; set; }
		public decimal Vazao { get; set; }
		public int UnidadeId { get; set; }
		public string UnidadeTexto { get; set; }
		public string SistemaTratamento { get; set; }
		public string Descricao { get; set; }
		public string Tid { get; set; }

		public EfluenteLiquido()
		{
			this.TipoTexto = 
			this.UnidadeTexto = 
			this.SistemaTratamento =
			this.Descricao =
			this.Tid = "";
		}
	}
}

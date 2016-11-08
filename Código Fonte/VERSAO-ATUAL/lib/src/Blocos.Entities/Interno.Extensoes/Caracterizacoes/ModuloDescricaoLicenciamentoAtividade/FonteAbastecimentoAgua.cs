namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade
{
	public class FonteAbastecimentoAgua
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }		
		public int TipoId { get; set; }
		public string TipoTexto { get; set; }
		public string Descricao { get; set; }
		public string Tid { get; set; }

		public FonteAbastecimentoAgua()
		{
			this.TipoTexto = 
			this.Descricao =
			this.Tid = "";
		}
	}
}

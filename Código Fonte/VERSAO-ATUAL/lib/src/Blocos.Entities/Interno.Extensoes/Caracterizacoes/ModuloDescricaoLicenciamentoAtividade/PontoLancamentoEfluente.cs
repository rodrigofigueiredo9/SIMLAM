namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade
{
	public class PontoLancamentoEfluente
	{		
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public int TipoId { get; set; }
		public string TipoTexto { get; set; }
		public string Descricao { get; set; }
		public string Tid { get; set; }

		public PontoLancamentoEfluente()
		{
			this.TipoTexto = 
			this.Descricao =
			this.Tid = "";
		}
	}
}

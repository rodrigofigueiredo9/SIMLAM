namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class GrupoDados
	{
		public GrupoDados(DadosRelatorio relatorio)
		{
			_relatorio = relatorio;
			Dados = new ColecaoDados(_relatorio.Colunas);
			Sumarizacoes = new ColecaoDados(_relatorio.Colunas);
		}

		public string Campo { get; set; }
		public string Valor { get; set; }
		public int Total { get { return Dados.Linhas.Count; } }
		public ColecaoDados Dados { get; set; }
		public ColecaoDados Sumarizacoes { get; set; }
		public DadosRelatorio _relatorio { get; set; }
	}
}
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class DadosRelatorio
	{
		public DadosRelatorio()
		{
			Colunas = new Dictionary<int, string>();
			Dados = new ColecaoDados(Colunas);
			Sumarizacoes = new ColecaoDados(Colunas);
			Grupos = new List<GrupoDados>();
			Campos = new List<ValoresBanco>();
			Filtros = new List<Termo>();
			Operadores = new List<Lista>();
		}

		public DadosRelatorio(Dictionary<int, string> colunas)
		{
			Colunas = colunas;
			Dados = new ColecaoDados(Colunas);
			Sumarizacoes = new ColecaoDados(Colunas);
			Grupos = new List<GrupoDados>();
			Campos = new List<ValoresBanco>();
			Filtros = new List<Termo>();
			Operadores = new List<Lista>();
		}

		public string Nome { get; set; }
		public string Data { get; set; }
		public bool ComAgrupamento { get; set; }
		public bool Totalizar { get; set; }
		public List<ValoresBanco> Campos { get; set; }
		public List<Termo> Filtros { get; set; }
		public List<Lista> Operadores { get; set; }
		public ColecaoDados Dados { get; private set; }
		public ColecaoDados Sumarizacoes { get; private set; }
		public List<GrupoDados> Grupos { get; private set; }
		public Dictionary<int, string> Colunas { get; private set; }
		public ConfiguracaoDocumentoPDF ConfiguracaoDocumentoPDF { get; set; }

		public int Total { get { return Dados.Linhas.Count + Grupos.SelectMany(x => x.Dados.Linhas).Count(); } }

		public GrupoDados CriarGrupo()
		{
			GrupoDados grupo = new GrupoDados(this);
			Grupos.Add(grupo);
			return grupo;
		}
	}
}
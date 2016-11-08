using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class ConfiguracaoRelatorio
	{
		public Fato FonteDados { get; set; }
		public List<ConfiguracaoCampo> CamposSelecionados { get; set; }
		public int FonteDadosId { get; set; }
		public string FonteDadosTid { get; set; }
		public List<ConfiguracaoOrdenacao> Ordenacoes { get; set; }
		public List<Termo> Termos { get; set; }
		public List<Sumario> Sumarios { get; set; }
		public bool ContarRegistros { get; set; }
		public List<ConfiguracaoAgrupamento> Agrupamentos { get; set; }
		public string Nome { get; set; }
		public string Descricao { get; set; }
		public DateTecno DataCriacao { get; set; }
		public bool OrientacaoRetrato { get; set; }
		public ConfiguracaoRelatorio()
		{
			OrientacaoRetrato = true;
			FonteDados = new Fato();
			CamposSelecionados = new List<ConfiguracaoCampo>();
			Ordenacoes = new List<ConfiguracaoOrdenacao>();
			Termos = new List<Termo>();
			Sumarios = new List<Sumario>();
			Agrupamentos = new List<ConfiguracaoAgrupamento>();
			DataCriacao = new DateTecno();
		}
	}
}
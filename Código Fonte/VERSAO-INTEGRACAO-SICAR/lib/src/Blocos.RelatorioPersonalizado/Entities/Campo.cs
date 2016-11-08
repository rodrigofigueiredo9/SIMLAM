using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class Campo
	{
		public int Id { get; set; }
		public int Codigo { get; set; }
		public string Nome { get; set; }
		public string Alias { get; set; }
		public string Texto { get; set; }
		public int Largura { get; set; }
		public bool CampoExibicao { get; set; }
		public bool CampoFiltro { get; set; }
		public bool CampoOrdenacao { get; set; }
		public string CampoConsulta { get; set; }
		public int SistemaConsulta { get; set; }
		public eSistemaConsulta SistemaConsultaEnum { get { return (eSistemaConsulta)SistemaConsulta; } }
		public int TipoDados { get; set; }
		public eTipoDados TipoDadosEnum { get { return (eTipoDados)TipoDados; } }
		public string Consulta { get; set; }
		public string DimensaoNome { get; set; }
		public string Tabela { get; set; }
		public List<Lista> Lista { get; set; }
		public bool PossuiListaDeValores { get { return !string.IsNullOrEmpty(Consulta); } }
	}
}
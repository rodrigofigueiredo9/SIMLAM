using System;

namespace Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Entities
{
	public class RelatorioCampo
	{
		public String TabelaFato { get; set; }
		public String TabelaDimensao { get; set; }
		public String Alias { get; set; }
		public String Nome { get; set; }
		public eCampoTipoDado TipoDado { get; set; }

		public Boolean Exibicao { get; set; }
		public Boolean Filtro { get; set; }
		public Boolean Ordenacao { get; set; }

		public eSistemaConsulta ConsultaSistema { get; set; }
		public String ConsultaSql { get; set; }
		public String ConsultaCampo { get; set; }
	}
}
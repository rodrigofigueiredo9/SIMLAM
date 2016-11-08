using System;

namespace Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Entities
{
	public class ConfiguracaoRelatorio
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String Fato { get; set; }
		public String Processo { get; set; }
		public TimeSpan Intervalo { get; set; }
		public DateTime DadosAte { get; set; }
		public DateTime ExecucaoInicio { get; set; }
		public DateTime? ExecucaoFim { get; set; }
		public Boolean EmExecucao { get; set; }
		public Boolean Erro { get; set; }
	}
}
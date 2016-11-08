using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.EtramiteX.WindowsService.SVCVencimentoCFOCFOCPTV.Entities
{
	class ConfiguracaoServico
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String Servico { get; set; }
		public TimeSpan Intervalo { get; set; }
		public DateTime? DataInicio { get; set; }
		public DateTime? DataInicioExecucao { get; set; }
		public DateTime? DataUltimaExecucao { get; set; }
		public Boolean EmExecucao { get; set; }
	}
}
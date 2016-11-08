using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Tecnomapas.EtramiteX.WindowsService.SVCAtividade.Business;
using Tecnomapas.EtramiteX.WindowsService.SVCAtividade.Properties;

namespace Tecnomapas.EtramiteX.WindowsService.SVCAtividade
{
	partial class ServicoAtividade : ServiceBase
	{
		private AtividadeIrregularBus _atividadeIrregularBus = new AtividadeIrregularBus();

		public ServicoAtividade()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			_atividadeIrregularBus.Inicializar(Settings.Default.CheckIntervalMinuto * 1000 * 60);
		}

		protected override void OnStop()
		{
			_atividadeIrregularBus.Finalizar();
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Tecnomapas.EtramiteX.WindowsService.SVCEmail;
using Tecnomapas.EtramiteX.WindowsService.SVCEmail.Properties;

namespace Tecnomapas.EtramiteX.WindowsService.SVCEmail
{
	public partial class ServicoEmail : ServiceBase
	{
		private EmailBus _servicoEmail = new EmailBus();
		
		public ServicoEmail()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			_servicoEmail.Inicializar( Settings.Default.CheckIntervalMinuto * 1000 * 60 );
		}

		protected override void OnStop()
		{
			_servicoEmail.Finalizar();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Tecnomapas.EtramiteX.WindowsService.SVCEmail
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			//new EmailBus().Teste();
			//return;

			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new ServicoEmail() 
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
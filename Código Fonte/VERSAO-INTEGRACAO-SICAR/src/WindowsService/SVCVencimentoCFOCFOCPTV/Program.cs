using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Tecnomapas.EtramiteX.WindowsService.SVCVencimentoCFOCFOCPTV.Business;

namespace Tecnomapas.EtramiteX.WindowsService.SVCVencimentoCFOCFOCPTV
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			//new VencimentoCFOCFOCPTVBus().Teste();
			//return;

			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{
				new ServicoVencimentoCFOCFOCPTV() 
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
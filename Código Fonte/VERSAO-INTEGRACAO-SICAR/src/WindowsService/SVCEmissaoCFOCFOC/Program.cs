using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Tecnomapas.EtramiteX.WindowsService.SVCEmissaoCFOCFOC.Business;

namespace Tecnomapas.EtramiteX.WindowsService.SVCEmissaoCFOCFOC
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			//new EmissaoCFOCFOCBus().Teste();
			//return;

			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{
				new ServicoEmissaoCFOCFOC() 
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
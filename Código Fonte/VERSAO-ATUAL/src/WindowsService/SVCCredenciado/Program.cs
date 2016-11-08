using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Tecnomapas.EtramiteX.WindowsService.SVCCredenciado.Business;

namespace Tecnomapas.EtramiteX.WindowsService.SVCCredenciado
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			//new CredenciadoBus().Teste();
			//return;

			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new ServicoCredenciado() 
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
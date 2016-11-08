using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Tecnomapas.EtramiteX.WindowsService.SVCCARSolicitacao.Business;

namespace Tecnomapas.EtramiteX.WindowsService.SVCCARSolicitacao
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			//new CARSolicitacaoBus().Teste();
			//return;

			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{
				new ServicoCARSolicitacao() 
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
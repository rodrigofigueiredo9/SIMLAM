using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			//new ImportarTituloDeclaratorioBus().Teste();
			//return;

			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{
				new ServicoImportarTituloDeclaratorio() 
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
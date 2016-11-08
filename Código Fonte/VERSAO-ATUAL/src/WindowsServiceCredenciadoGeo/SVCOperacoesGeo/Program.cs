using System.ServiceProcess;

namespace Tecnomapas.EtramiteX.WindowsService.SVCOperacoesGeo
{
	static class Program
	{
		static void Main()
		{
			//Debugger.Launch();
			ServiceBase[] ServicesToRun = new ServiceBase[] { new ServicoOperacoesGeo() };
			ServiceBase.Run(ServicesToRun);
		}
	}
}
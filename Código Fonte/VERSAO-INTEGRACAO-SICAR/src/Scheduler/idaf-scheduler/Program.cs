using System.ServiceProcess;

namespace Tecnomapas.EtramiteX.Scheduler
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
#if DEBUG
			var service = new SchedulerService();
			service.OnDebug();
			System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else

			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new SchedulerService() 
			};
			ServiceBase.Run(ServicesToRun);
#endif
		}
	}
}
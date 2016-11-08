using System;
using Tecnomapas.EtramiteX.WindowsService.CaractUPUCETL.Business;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;

namespace Tecnomapas.EtramiteX.WindowsService.CaractUPUCETL
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				CaractUPUCBus etl = new CaractUPUCBus();
				etl.AtualizarETL();
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}
	}
}
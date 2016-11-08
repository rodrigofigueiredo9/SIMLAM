using System;
using Tecnomapas.EtramiteX.WindowsService.TituloETL.Business;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;

namespace Tecnomapas.EtramiteX.WindowsService.TituloETL
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				TituloBus etl = new TituloBus();
				etl.AtualizarETL();
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}
	}
}
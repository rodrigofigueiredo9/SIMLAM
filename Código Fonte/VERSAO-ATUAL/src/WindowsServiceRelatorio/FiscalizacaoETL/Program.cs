using System;
using Tecnomapas.EtramiteX.WindowsService.FiscalizacaoETL.Business;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;

namespace Tecnomapas.EtramiteX.WindowsService.FiscalizacaoETL
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				FiscalizacaoBus etl = new FiscalizacaoBus();
				etl.AtualizarETL();
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}
	}
}
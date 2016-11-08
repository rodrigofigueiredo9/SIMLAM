using System.ServiceProcess;
using Tecnomapas.EtramiteX.WindowsService.SVCEmissaoCFOCFOC.Business;
using Tecnomapas.EtramiteX.WindowsService.SVCEmissaoCFOCFOC.Properties;

namespace Tecnomapas.EtramiteX.WindowsService.SVCEmissaoCFOCFOC
{
	partial class ServicoEmissaoCFOCFOC : ServiceBase
	{
		private EmissaoCFOCFOCBus _servico = new EmissaoCFOCFOCBus();

		public ServicoEmissaoCFOCFOC()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			_servico.Inicializar(Settings.Default.CheckIntervalMinuto * 1000 * 60);
		}

		protected override void OnStop()
		{
			_servico.Finalizar();
		}
	}
}
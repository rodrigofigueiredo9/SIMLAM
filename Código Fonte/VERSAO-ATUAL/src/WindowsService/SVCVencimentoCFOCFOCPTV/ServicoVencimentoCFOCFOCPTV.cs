using System.ServiceProcess;
using Tecnomapas.EtramiteX.WindowsService.SVCVencimentoCFOCFOCPTV.Business;
using Tecnomapas.EtramiteX.WindowsService.SVCVencimentoCFOCFOCPTV.Properties;

namespace Tecnomapas.EtramiteX.WindowsService.SVCVencimentoCFOCFOCPTV
{
	partial class ServicoVencimentoCFOCFOCPTV : ServiceBase
	{
		private VencimentoCFOCFOCPTVBus _servico = new VencimentoCFOCFOCPTVBus();

		public ServicoVencimentoCFOCFOCPTV()
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
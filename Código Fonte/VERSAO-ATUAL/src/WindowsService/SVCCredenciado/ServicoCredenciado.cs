using System.ServiceProcess;
using Tecnomapas.EtramiteX.WindowsService.SVCCredenciado.Business;
using Tecnomapas.EtramiteX.WindowsService.SVCCredenciado.Properties;

namespace Tecnomapas.EtramiteX.WindowsService.SVCCredenciado
{
	partial class ServicoCredenciado : ServiceBase
	{
		private CredenciadoBus _servico = new CredenciadoBus();

		public ServicoCredenciado()
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
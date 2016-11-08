using System.ServiceProcess;
using Tecnomapas.EtramiteX.WindowsService.SVCCARSolicitacao.Business;
using Tecnomapas.EtramiteX.WindowsService.SVCCARSolicitacao.Properties;

namespace Tecnomapas.EtramiteX.WindowsService.SVCCARSolicitacao
{
	partial class ServicoCARSolicitacao : ServiceBase
	{
		private CARSolicitacaoBus _servico = new CARSolicitacaoBus();

		public ServicoCARSolicitacao()
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
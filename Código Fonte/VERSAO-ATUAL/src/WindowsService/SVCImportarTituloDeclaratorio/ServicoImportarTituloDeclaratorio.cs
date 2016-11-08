using System.ServiceProcess;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Properties;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio
{
	partial class ServicoImportarTituloDeclaratorio : ServiceBase
	{
		private ImportarTituloDeclaratorioBus _servico = new ImportarTituloDeclaratorioBus();

		public ServicoImportarTituloDeclaratorio()
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
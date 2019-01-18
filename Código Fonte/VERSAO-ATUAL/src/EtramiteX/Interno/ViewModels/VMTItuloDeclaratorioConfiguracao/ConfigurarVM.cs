using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTItuloDeclaratorioConfiguracao
{
	public class ConfigurarVM
	{
		public ConfigurarVM() {}

		public bool IsVisualizar { get; set; }

		public string BarragemSemAPPJSon { get; set; }
		public string BarragemComAPPJSon { get; set; }
		public TituloDeclaratorioConfiguracao Configuracao { get; set; } = new TituloDeclaratorioConfiguracao();
	}
}
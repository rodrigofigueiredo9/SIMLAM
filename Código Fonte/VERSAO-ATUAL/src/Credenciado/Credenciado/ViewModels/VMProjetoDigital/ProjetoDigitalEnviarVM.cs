using System;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMRequerimento;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital
{
	public class ProjetoDigitalEnviarVM
	{
		public Boolean ModoVisualizar { get; set; }
		public ProjetoDigital ProjetoDigital { get; set; }
		public RequerimentoVM RequerimentoVM { get; set; }
		public DocumentosGeradosVM DocumentosGeradosVM { get; set; }

		public ProjetoDigitalEnviarVM()
		{
			ProjetoDigital = new ProjetoDigital();
			RequerimentoVM = new RequerimentoVM();
			DocumentosGeradosVM = new DocumentosGeradosVM();
		}
	}
}
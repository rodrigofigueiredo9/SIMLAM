using System;
using Tecnomapas.Blocos.Arquivo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMProjetoGeografico
{
	public class ProjetoImportadoVM
	{
		public String ArquivoJson { set; get; }

		public Arquivo Arquivo { get; set; }
	}
}

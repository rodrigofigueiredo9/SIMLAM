using System;
using Tecnomapas.Blocos.Arquivo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico
{
	public class ProjetoImportadoVM
	{
		public String ArquivoJson { set; get; }

		public Arquivo Arquivo { get; set; }
	}
}

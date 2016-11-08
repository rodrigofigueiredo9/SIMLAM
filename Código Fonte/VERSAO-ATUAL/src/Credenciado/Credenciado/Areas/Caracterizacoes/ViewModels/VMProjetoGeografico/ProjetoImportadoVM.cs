using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico
{
	public class ProjetoImportadoVM
	{
		public String ArquivoJson { set; get; }

		public Arquivo Arquivo { get; set; }
	}
}
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMLiberacaoCFOCFOC
{
	public class MotivoCancelamentoVM
	{
		public string Motivo { get; set; }
		public int Id { get; set; }
		public string TipoDocumento { get; set; }
		public string Numero { get; set; }

		public bool IsVisualizar { get; set; }
	}
}
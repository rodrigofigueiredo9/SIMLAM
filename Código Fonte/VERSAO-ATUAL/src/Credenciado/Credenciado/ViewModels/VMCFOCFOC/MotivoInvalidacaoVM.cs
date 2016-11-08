using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC
{
	public class MotivoInvalidacaoVM
	{
		public string Motivo { get; set; }
		public int Id { get; set; }
		public string TipoDocumento { get; set; }
		public string Numero { get; set; }
		public bool IsVisualizar { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC
{
	public class ConsultarNumeroCFOCFOCLiberadoVM
	{
		public List<SelectListItem> LstTipoDocumento { get; set; }

		public ConsultarNumeroCFOCFOCLiberadoVM()
		{
			List<Lista> lista = new List<Lista>();
			lista.Add(new Lista() { Id = "1", Texto = "CFO" });
			lista.Add(new Lista() { Id = "2", Texto = "CFOC" });

			LstTipoDocumento = ViewModelHelper.CriarSelectList(lista);
		}
	}
}
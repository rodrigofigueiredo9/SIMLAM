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
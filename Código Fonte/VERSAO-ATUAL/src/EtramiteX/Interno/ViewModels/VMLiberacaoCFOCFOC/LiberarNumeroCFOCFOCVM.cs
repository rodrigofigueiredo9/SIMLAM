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
	public class LiberarNumeroCFOCFOCVM
	{
		public bool isVisualizar { get; set; }

		public LiberaracaoNumeroCFOCFOC Liberacao { get; set; }

		public List<SelectListItem> LstQuantidadeNumeroDigitalCFO { get; set; }
		public List<SelectListItem> LstQuantidadeNumeroDigitalCFOC { get; set; }

		public LiberarNumeroCFOCFOCVM()
		{
			Liberacao = new LiberaracaoNumeroCFOCFOC();
						
			List<Lista> valores = new List<Lista>();
			valores.Add(new Lista() { Id = "0", Texto = "0" });
			valores.Add(new Lista() { Id = "25", Texto = "25" });
			valores.Add(new Lista() { Id = "50", Texto = "50" });

			LstQuantidadeNumeroDigitalCFO = ViewModelHelper.CriarSelectList(valores);
			LstQuantidadeNumeroDigitalCFOC = ViewModelHelper.CriarSelectList(valores);		
		}
	}
}
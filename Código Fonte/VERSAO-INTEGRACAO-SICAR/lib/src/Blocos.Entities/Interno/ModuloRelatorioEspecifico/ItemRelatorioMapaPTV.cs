using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloRelatorioEspecifico
{
	public class ItemRelatorioMapaPTV
	{
		public string DataEmissao { get; set; }
		public string NumeroPTV { get; set; }
		public string CulturaCultivar { get; set; }
		public decimal Quantidade { get; set; }
		public string UnidadeMedida { get; set; }
		public string DestinatarioNome { get; set; }
		public string DestinatarioMunicipio { get; set; }
		public string DestinatarioEstado { get; set; }

		public ItemRelatorioMapaPTV()
		{
			
		}
	}
}

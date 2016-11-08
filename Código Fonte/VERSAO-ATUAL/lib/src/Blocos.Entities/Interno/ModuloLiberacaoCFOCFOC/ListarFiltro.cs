using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC
{
	public class ListarFiltro
	{
		public string ResponsavelNome { get; set; }
		public int TipoDocumento { get; set; }
		public int TipoNumero { get; set; }
		public long? Numero{ get; set; }
	}
}
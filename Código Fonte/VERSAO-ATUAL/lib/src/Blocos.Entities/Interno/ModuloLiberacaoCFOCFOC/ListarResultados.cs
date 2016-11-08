using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC
{
	public class ListarResultados
	{
		public string ResponsavelNome { get; set; }
		public int LiberacaoId { get; set; }

		public ListarResultados()
		{

		}
	}
}
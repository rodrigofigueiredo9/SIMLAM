using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloRelatorioEspecifico
{
	public class RelatorioMapaFiltroeResultado
	{
		public int tipoRelatorio { get; set; }
		public string DataInicial { get; set; }

		public string DataFinal { get; set; }

		public string LocalRelatorio { get; set; }

		public int IdSetor { get; set; }

		public string DataRelatorio { get; set; }

		public string NomeFuncionario { get; set; }

		public List<ItemRelatorioMapaPTV> ItensRelatorioMapaPTV { get; set; }
		public List<ItemRelatorioMapaCFOCFOC> ItensRelatorioMapaCFOCFOC { get; set; }

		public RelatorioMapaFiltroeResultado()
		{
			DataRelatorio = DateTime.Today.ToString("dd/MM/yyyy");
			ItensRelatorioMapaPTV = new List<ItemRelatorioMapaPTV>();
			ItensRelatorioMapaCFOCFOC = new List<ItemRelatorioMapaCFOCFOC>();
		}
	}
}

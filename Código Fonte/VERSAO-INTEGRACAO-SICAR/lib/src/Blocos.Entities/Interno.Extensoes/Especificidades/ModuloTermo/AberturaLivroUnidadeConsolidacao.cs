using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo
{
	public class AberturaLivroUnidadeConsolidacao : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }
		public String TotalPaginasLivro { get; set; }
		public String PaginaInicial { get; set; }
		public String PaginaFinal { get; set; }

		public List<Cultura> Culturas { set; get; }

		public AberturaLivroUnidadeConsolidacao()
		{
			Culturas = new List<Cultura>();
		}
	}
}
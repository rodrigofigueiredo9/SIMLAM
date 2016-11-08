using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo
{
	public class AberturaLivroUnidadeProducao : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }

		public String TotalPaginasLivro { get; set; }
		public String PaginaInicial { get; set; }
		public String PaginaFinal { get; set; }

		//public Int32 UnidadeProducaoUnidadeId { get; set; }
		//public String UnidadeProducaoUnidadeCodigo { get; set; }
		//public String UnidadeProducaoUnidadeTid { get; set; }

		public List<UnidadeProducaoItem> Unidades { get; set; }

		public AberturaLivroUnidadeProducao()
		{
			Unidades = new List<UnidadeProducaoItem>();
		}
	}
}
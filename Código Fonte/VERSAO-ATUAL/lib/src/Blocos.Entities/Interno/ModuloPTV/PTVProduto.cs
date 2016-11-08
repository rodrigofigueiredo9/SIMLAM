using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
	public class PTVProduto
	{
		public int Id { get; set; }

		public string Tid { get; set; }

		public int PTV { get; set; }
        public int ProducaoTipo { get; set; }

        public string ProducaoTipoTexto { get; set; }

		public int OrigemTipo { get; set; }

		public string OrigemTipoTexto { get; set; }

		public string OrigemTexto { get; set; }

		public int Origem { get; set; }

		public string OrigemNumero { get; set; }

		public bool IsNumeroOrigem { get; set; }

		public int Cultura { get; set; }

		public string CulturaTexto { get; set; }

		public int Cultivar { get; set; }

		public string CultivarTexto { get; set; }

		public string CulturaCultivar { get; set; }

		public decimal Quantidade { get; set; }

		public int UnidadeMedida { get; set; }

		public string UnidadeMedidaTexto { get; set; }

        public int EmpreendimentoId { get; set; }

        public string EmpreendimentoDeclaratorio { get; set; }
	}
}
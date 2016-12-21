using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV
{
	public class PTVProdutoRelatorio
	{
		public int Id { get; set; }

        public string CulturaTexto { get; set; }

        public int CulturaId { get; set; }

		public string CultivarTexto { get; set; }

        public int CultivarId { get; set; }
		
		public string NumeroCFOC { get; set; }
		
		public string NumeroCFO { get; set; }

		public string NumeroPTV { get; set; }

		public string NumeroCFCFR { get; set; }

		public string NumeroTF { get; set; }

		public string UnidadeMedida { get; set; }

		public decimal Quantidade { get; set; }

		public string DeclaracaoAdicional { get; set; }

        public string DeclaracaoAdicionalHtml { get; set; }

        public int Origem { get; set; }

        public string OrigemTid { get; set; }

        public int OrigemTipo { get; set; }

		public string CulturaCultivar
		{
			get
			{
				string retorno = CultivarTexto;

				if (CulturaTexto.ToLower() != "citros")
				{
					retorno = CulturaTexto + " " + CultivarTexto;
				}
				else if (String.IsNullOrWhiteSpace(retorno))
				{
					retorno = CulturaTexto;
				}

				return retorno;
			}
		}
	}
}

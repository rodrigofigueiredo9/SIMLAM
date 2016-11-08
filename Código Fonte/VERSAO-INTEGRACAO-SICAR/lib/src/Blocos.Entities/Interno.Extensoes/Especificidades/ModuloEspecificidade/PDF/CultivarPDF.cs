using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class CultivarPDF
	{
		public String Nome { get; set; }
		public String Capacidade { get; set; }
		public String Unidade { get; set; }
		public String Cultura { get; set; }

		public String CultivarCulturaNome
		{
			get
			{
				string retorno = Nome;

				if (Cultura.ToLower() != "citros")
				{
					retorno = Cultura + " " + Nome;
				}
				else if (String.IsNullOrWhiteSpace(retorno))
				{
					retorno = Cultura;
				}

				return retorno;
			}
		}

		public String CapacidadeUnidade
		{
			get { return Capacidade + "/" + Unidade; }
		}
	}
}

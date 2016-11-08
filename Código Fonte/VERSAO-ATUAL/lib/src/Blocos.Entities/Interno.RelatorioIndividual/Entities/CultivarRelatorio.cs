using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities
{
	public class CultivarRelatorio
	{
		string _culturaNome = string.Empty;
		public string CulturaNome
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(CultivarNome) && !_culturaNome.ToLower().Equals("citros"))
				{
					return string.Concat(_culturaNome, " ", CultivarNome);
				}
				else if (_culturaNome.ToLower().Equals("citros"))
				{
					return CultivarNome;
				}
				return _culturaNome;
			}
			set
			{
				_culturaNome = value;
			}
		}

		public string CultivarNome { get; set; }
		public string CapacidadeMes { get; set; }
		public int UnidadeMedida { get; set; }
		public string UnidadeMedidaTexto { get; set; }
	}
}
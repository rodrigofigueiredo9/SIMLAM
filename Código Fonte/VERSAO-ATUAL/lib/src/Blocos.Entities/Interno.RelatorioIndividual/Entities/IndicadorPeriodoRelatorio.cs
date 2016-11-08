

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities
{
	public class IndicadorPeriodoRelatorio
	{
		public int? Hoje { get; set; }
		public int? EssaSemana { get; set; }
		public int? EsseMes { get; set; }
		public int? ProximoMes { get; set; }

		public decimal Total { get; set; }

		public void CalcularTotal()
		{

			if (ProximoMes > EsseMes)
			{
				Total = Convert.ToDecimal(((ProximoMes > EssaSemana) ? ProximoMes : EssaSemana));
			}
			else
			{
				Total = Convert.ToDecimal(((EsseMes > EssaSemana) ? EsseMes : EssaSemana));
			}

			Total = (Total > 0) ? Total : 1;


			if (ProximoMes > EsseMes)
			{
				Total = Convert.ToDecimal(((ProximoMes > EssaSemana) ? ProximoMes : EssaSemana));
			}
			else
			{
				Total = Convert.ToDecimal(((EsseMes > EssaSemana) ? EsseMes : EssaSemana));
			}

			Total = (Total > 0) ? Total : 1;
		}
	}
}

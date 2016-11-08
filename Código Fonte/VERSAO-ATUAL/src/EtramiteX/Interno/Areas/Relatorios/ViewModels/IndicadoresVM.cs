

using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;

namespace Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels
{
	public class IndicadoresVM
	{
		public bool Exibir { get; set; }

		public IndicadorPeriodoRelatorio Titulos { get; set; }
		public IndicadorPeriodoRelatorio Condicionantes { get; set; }

		public IndicadoresVM() { }

		public void CalcularRelatorio()
		{
			Titulos.CalcularTotal();
			Condicionantes.CalcularTotal();
		}
	}
}
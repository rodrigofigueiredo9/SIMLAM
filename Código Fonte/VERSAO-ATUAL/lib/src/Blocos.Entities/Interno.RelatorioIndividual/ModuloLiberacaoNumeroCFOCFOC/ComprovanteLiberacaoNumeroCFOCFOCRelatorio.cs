using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloLiberacaoNumeroCFOCFOC
{
	public class ComprovanteLiberacaoNumeroCFOCFOCRelatorio
	{
		public int Id { get; set; }
		public String Tid { get; set; }

		public string Nome { get; set; }
		public string CPF { get; set; }
		public string NumeroBlocoInicialCFO { get; set; }
		public string NumeroBlocoFinalCFO { get; set; }
		public string NumeroBlocoInicialCFOC { get; set; }
		public string NumeroBlocoFinalCFOC { get; set; }
		public string QtdNumeroDigitalCFO { get; set; }
		public string QtdNumeroDigitalCFOC { get; set; }
		public string DiaEmissao { get; set; }
		public string MesEmissao { get; set; }
		public string AnoEmissao { get; set; }
		public ComprovanteLiberacaoNumeroCFOCFOCRelatorio() 
		{
			DiaEmissao = DateTime.Today.Day.ToString();
			MesEmissao = DateTime.Today.ToString("MMMM");
			AnoEmissao = DateTime.Today.Year.ToString();

		}

	}
}
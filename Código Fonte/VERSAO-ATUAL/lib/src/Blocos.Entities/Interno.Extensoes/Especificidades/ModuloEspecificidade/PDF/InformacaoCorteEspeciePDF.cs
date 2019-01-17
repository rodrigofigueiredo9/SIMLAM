using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class InformacaoCorteEspeciePDF
	{
		public String Especie { get; set; }
		public String ArvoresIsoladas { get; set; }
		public String AreaCorte { get; set; }

		public InformacaoCorteEspeciePDF(InformacaoCorteTipo informacaoCorteEspecie)
		{
			this.Especie = informacaoCorteEspecie.EspecieInformadaTexto;
			this.ArvoresIsoladas = informacaoCorteEspecie.AreaCorte.ToStringTrunc();
			this.AreaCorte = informacaoCorteEspecie.AreaCorte.ToStringTrunc();
		}
	}
}
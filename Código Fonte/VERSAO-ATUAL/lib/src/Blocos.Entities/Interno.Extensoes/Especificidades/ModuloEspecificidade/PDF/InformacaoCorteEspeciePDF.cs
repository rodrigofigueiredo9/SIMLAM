using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class InformacaoCorteEspeciePDF
	{
		public String Especie { get; set; }
		public String ArvoresIsoladas { get; set; }
		public String AreaCorte { get; set; }

		//public InformacaoCorteEspeciePDF(Especie informacaoCorteEspecie)
		//{
		//	this.Especie = informacaoCorteEspecie.EspecieTipoTexto;
		//	this.ArvoresIsoladas = informacaoCorteEspecie.ArvoresIsoladas;
		//	this.AreaCorte = informacaoCorteEspecie.AreaCorte;
		//}
	}
}
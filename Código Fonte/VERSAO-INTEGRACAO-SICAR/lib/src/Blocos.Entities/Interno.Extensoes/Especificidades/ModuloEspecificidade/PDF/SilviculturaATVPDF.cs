using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaATV;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class SilviculturaATVPDF
	{
		public String AreaPlantioProprio { get; set; }
		public String AreaOutroFomento { get; set; }
		public String DeclividadePredominantePropriedade { get; set; }

		public String DeclividadePredominante { get { return DeclividadePredominanteDecimal.ToStringTrunc(); } }
		public Decimal DeclividadePredominanteDecimal { get; set; }

		public String TotalAreaRequerida { get { return TotalAreaRequeridaDecimal.ToStringTrunc(); } }
		public Decimal TotalAreaRequeridaDecimal { get; set; }

		public String TotalAreaCroqui { get { return TotalAreaCroquiDecimal.ToStringTrunc(); } }
		public Decimal TotalAreaCroquiDecimal { get; set; }

		public String AreaPlantadaEucalipito { get { return AreaPlantadaEucalipitoDecimal.ToStringTrunc(); } }
		public Decimal AreaPlantadaEucalipitoDecimal { get; set; }

		private List<CulturaFlorestalATV> _culturas = new List<CulturaFlorestalATV>();
		public List<CulturaFlorestalATV> Culturas
		{
			get { return _culturas; }
			set { _culturas = value; }
		}

		public SilviculturaATVPDF() { }

		public SilviculturaATVPDF(SilviculturaATV caracterizacao)
		{
			if (caracterizacao.Areas.Count > 0)
			{
				AreaPlantioProprio = caracterizacao.Areas.FirstOrDefault(x => x.Tipo == (int)eSilviculturaAreaATV.AA_PLANTIO).ValorTexto;
				AreaOutroFomento = caracterizacao.Areas.FirstOrDefault(x => x.Tipo == (int)eSilviculturaAreaATV.AA_FOMENTO).ValorTexto;
				DeclividadePredominantePropriedade = caracterizacao.Areas.FirstOrDefault(x => x.Tipo == (int)eSilviculturaAreaATV.DECLIVIDADE).ValorTexto;
			}

			DeclividadePredominanteDecimal = 0;
			TotalAreaRequeridaDecimal = 0;
			TotalAreaCroquiDecimal = 0;
			AreaPlantadaEucalipitoDecimal = 0;

			foreach (SilviculturaCaracteristicaATV silvicultura in caracterizacao.Caracteristicas)
			{
				DeclividadePredominanteDecimal = (silvicultura.DeclividadeToDecimal.GetValueOrDefault(0) > DeclividadePredominanteDecimal) ? silvicultura.DeclividadeToDecimal.GetValueOrDefault(0) : DeclividadePredominanteDecimal;
				TotalAreaRequeridaDecimal += silvicultura.TotalRequeridaToDecimal.GetValueOrDefault(0);
				TotalAreaCroquiDecimal += silvicultura.TotalCroqui;
				AreaPlantadaEucalipitoDecimal += silvicultura.TotalPlantadaComEucaliptoToDecimal.GetValueOrDefault(0);
			}

			//Agrupando Culturas Florestais ATV
			List<CulturaFlorestalATV> culturas = new List<CulturaFlorestalATV>();
			caracterizacao.Caracteristicas.ForEach(x => { x.Culturas.ForEach(y => { culturas.Add(y); }); });
			culturas.ForEach(cultura =>
			{
				if (!Culturas.Exists(y => y.CulturaTipoTexto.ToLower() == cultura.CulturaTipoTexto.ToLower()))
				{
					cultura.AreaCultura = culturas
						.Where(x => x.CulturaTipoTexto.ToLower() == cultura.CulturaTipoTexto.ToLower())
						.Sum(x => Convert.ToDecimal(x.AreaCultura)).ToString();
					Culturas.Add(cultura);
				}
			});
		}
	}
}
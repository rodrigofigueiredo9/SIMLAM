using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilvicultura;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class SilviculturaPDF
	{
		public String AreaTotalHa { get; set; }

		private List<CulturaFlorestal> _culturas = new List<CulturaFlorestal>();
		public List<CulturaFlorestal> Culturas
		{
			get { return _culturas; }
			set { _culturas = value; }
		}

		public SilviculturaPDF() { }

		public SilviculturaPDF(Silvicultura silvicultura)
		{
			Decimal totalHa = silvicultura.Silviculturas.Sum(x => x.AreaCroquiHa);
			AreaTotalHa = totalHa.ToStringTrunc(4);

			//Agrupando Culturas Florestais
			List<CulturaFlorestal> culturas = silvicultura.Silviculturas.SelectMany(x => x.Culturas).ToList();

			culturas.ForEach(cultura =>
			{
				if (!Culturas.Exists(y => y.CulturaTipo == cultura.CulturaTipo))
				{
					cultura.AreaCulturaHa = culturas
						.Where(x => x.CulturaTipo == cultura.CulturaTipo)
						.Sum(x => x.AreaCulturaHa);
					Culturas.Add(cultura);
				}
			});
		}
	}
}
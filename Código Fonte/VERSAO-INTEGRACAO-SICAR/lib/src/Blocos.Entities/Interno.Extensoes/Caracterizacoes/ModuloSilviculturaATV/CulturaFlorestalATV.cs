

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaATV
{
	public class CulturaFlorestalATV
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 CulturaTipo { get; set; }
		public String CulturaTipoTexto { get; set; }
		public String EspecificarTexto { get; set; }

		public String AreaCultura { get; set; }
		public Decimal? AreaCulturaToDecimal { get { return this.ToDecimal(this.AreaCultura); } }

		public CulturaFlorestalATV()
		{
			this.Tid =
			this.CulturaTipoTexto =
			this.EspecificarTexto =
			this.AreaCultura = String.Empty;
		}

		internal Decimal? ToDecimal(string strValor)
		{
			Decimal decimalValor = 0;

			if (string.IsNullOrEmpty(strValor))
			{
				return null;
			}

			if (!Decimal.TryParse(strValor, out decimalValor))
			{
				return null;
			}

			return decimalValor;
		}
	}
}

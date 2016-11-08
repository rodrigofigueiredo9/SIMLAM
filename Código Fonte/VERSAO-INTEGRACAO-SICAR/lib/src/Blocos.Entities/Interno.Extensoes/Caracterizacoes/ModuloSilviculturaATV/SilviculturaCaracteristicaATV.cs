using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaATV
{
	public class SilviculturaCaracteristicaATV
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String Identificacao { get; set; }
		public Int32 GeometriaTipo { get; set; }
		public String GeometriaTipoTexto { get; set; }
		public Int32 FomentoId { get; set; }
		public eFomentoTipoATV Fomento { get { return (eFomentoTipoATV)this.FomentoId; } }

		public String Declividade { get; set; }
		public String TotalRequerida { get; set; }
		public Decimal TotalCroqui { get; set; }
		public String TotalPlantadaComEucalipto { get; set; }

		public Decimal? DeclividadeToDecimal { get { return this.ToDecimal(this.Declividade); } }
		public Decimal? TotalRequeridaToDecimal { get { return this.ToDecimal(this.TotalRequerida); } }
		public Decimal? TotalPlantadaComEucaliptoToDecimal { get { return this.ToDecimal(this.TotalPlantadaComEucalipto); } }

		public List<CulturaFlorestalATV> Culturas { get; set; }		

		public SilviculturaCaracteristicaATV()
		{
			this.Tid =
			this.Identificacao =
			this.GeometriaTipoTexto =
			this.Declividade =
			this.TotalRequerida =
			this.TotalPlantadaComEucalipto = String.Empty;
			this.Culturas = new List<CulturaFlorestalATV>();
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

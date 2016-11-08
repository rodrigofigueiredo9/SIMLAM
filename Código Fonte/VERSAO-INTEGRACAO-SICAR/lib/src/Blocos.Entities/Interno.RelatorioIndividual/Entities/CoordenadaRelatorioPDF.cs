using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities
{
	public class CoordenadaRelatorioPDF
	{
		public int Id { get; set; }
		public string Tid { get; set; }

		public int? FormaColeta { get; set; }
		public int? LocalColeta { get; set; }

		public int? Abrangencia { get; set; }
		public int? FusoUtm { get; set; }
		public int? HemisferioUtm { get; set; }

		public String HemisferioUtmTexto { get; set; }
		public String LocalColetaTexto { get; set; }
		public String FormaColetaTexto { get; set; }
		public String DatumTexto { get; set; }

		public String LatitudeGms { get; set; }
		public String LongitudeGms { get; set; }

		public Double? EastingUtm { get; set; }
		public Double? NorthingUtm { get; set; }

		private String _eastingUtmTexto;
		public String EastingUtmTexto
		{
			get { return _eastingUtmTexto; }
			set
			{
				_eastingUtmTexto = (!String.IsNullOrWhiteSpace(value))? value.Trim().Replace(".", ",").Replace(" ",""):value;
				_eastingUtmTexto = value;
				if (ValidarDouble(_eastingUtmTexto))
				{
					EastingUtm = Convert.ToDouble(_eastingUtmTexto);
				}
			}
		}

		private String _northingUtmTexto;
		public String NorthingUtmTexto
		{
			get { return _northingUtmTexto; }
			set
			{
				_northingUtmTexto = (!String.IsNullOrWhiteSpace(value)) ? value.Trim().Replace(".", ",").Replace(" ","") : value;
				if (ValidarDouble(_northingUtmTexto))
				{
					NorthingUtm = Convert.ToDouble(_northingUtmTexto);
				}
			}
		}

		public Double? LatitudeGdec{ get; set; }
		public Double? LongitudeGdec{ get; set; }
		
		private String _latitudeGdecTexto;
		public String LatitudeGdecTexto
		{
			get { return _latitudeGdecTexto; }
			set
			{
				_latitudeGdecTexto = (!String.IsNullOrWhiteSpace(value)) ? value.Trim().Replace(".", ",").Replace(" ","") : value;
				if (ValidarDouble(_latitudeGdecTexto))
				{
					LatitudeGdec = Convert.ToDouble(_latitudeGdecTexto);
				}
			}
		}

		private String _longitudeGdecTexto;
		public String LongitudeGdecTexto
		{
			get { return _longitudeGdecTexto; }
			set
			{
				_longitudeGdecTexto = (!String.IsNullOrWhiteSpace(value)) ? value.Trim().Replace(".", ",").Replace(" ","") : value;
				if (ValidarDouble(_longitudeGdecTexto))
				{
					LongitudeGdec = Convert.ToDouble(_longitudeGdecTexto);
				}
			}
		}

		private CoordenadaTipo _tipo = new CoordenadaTipo();
		public CoordenadaTipo Tipo
		{
			get { return _tipo; }
			set { _tipo = value; }
		}

		private CoordenadaDatum _datum = new CoordenadaDatum();
		public CoordenadaDatum Datum
		{
			get { return _datum; }
			set { _datum = value; }
		}

		public static bool ValidarDouble(String texto)
		{
			try
			{
				if (String.IsNullOrEmpty(texto))
				{
					return false;
				}
				Convert.ToDouble(texto);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
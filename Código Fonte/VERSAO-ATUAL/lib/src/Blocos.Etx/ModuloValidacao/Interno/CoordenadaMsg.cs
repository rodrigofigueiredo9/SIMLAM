

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao.Interno
{
	public class CoordenadaMsg
	{
		public String Prefixo { get; set; }

		public CoordenadaMsg(string prefixo = null)
		{
			Prefixo = prefixo;
		}

		public Mensagem CoordenadaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Coordenada é obrigatória." }; } }
		public Mensagem CoordenadaTipoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_Tipo_Id", Prefixo), Texto = "Sistema de coordenada é inválido." }; } }
		public Mensagem DatumInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_Datum_Id", Prefixo), Texto = "Datum é inválido." }; } }
		public Mensagem DatumObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_Datum_Id", Prefixo), Texto = "Datum é obrigatório." }; } }

		public Mensagem LongitudeGmsFormato { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LongitudeGms", Prefixo), Texto = "Longitude (GMS) fora do formato: xx:xx:xx,xx." }; } }
		public Mensagem LatitudeGmsFormato { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LatitudeGms", Prefixo), Texto = "Latitude (GMS) fora do formato: xx:xx:xx,xx." }; } }

		public Mensagem LongitudeGmsObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LongitudeGms", Prefixo), Texto = "Longitude (GMS) é obrigatório." }; } }
		public Mensagem LongitudeGmsInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LongitudeGms", Prefixo), Texto = "Longitude (GMS) é inválido." }; } }
		public Mensagem LatitudeGmsObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LatitudeGms", Prefixo), Texto = "Latitude (GMS) é obrigatório." }; } }
		public Mensagem LatitudeGmsInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LatitudeGms", Prefixo), Texto = "Latitude (GMS) é inválido." }; } }
		public Mensagem LongitudeGdecObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LongitudeGdecTexto", Prefixo), Texto = "Longitude (GDEC) é obrigatório." }; } }
		public Mensagem LongitudeGdecInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LongitudeGdecTexto", Prefixo), Texto = "Longitude (GDEC) é inválido." }; } }
		public Mensagem LatitudeGdecObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LatitudeGdecTexto", Prefixo), Texto = "Latitude (GDEC) é obrigatório." }; } }
		public Mensagem LatitudeGdecInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LatitudeGdecTexto", Prefixo), Texto = "Latitude (GDEC) é inválido." }; } }
		public Mensagem EastingUtmObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_EastingUtmTexto", Prefixo), Texto = "Easting é obrigatório." }; } }
		public Mensagem EastingUtmInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_EastingUtmTexto", Prefixo), Texto = "Easting (UTM) é inválido." }; } }
		public Mensagem NorthingUtmObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_NorthingUtmTexto", Prefixo), Texto = "Northing é obrigatório." }; } }
		public Mensagem NorthingUtmInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_NorthingUtmTexto", Prefixo), Texto = "Northing (UTM) é inválido." }; } }
		public Mensagem FusoUtmObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_FusoUtm", Prefixo), Texto = "Fuso é obrigatório." }; } }
		public Mensagem HemisferioUtmObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_HemisferioUtm", Prefixo), Texto = "Hemisfério é obrigatório." }; } }

		public Mensagem LongitudeGdecForaDoEscopo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LongitudeGdecTexto", Prefixo), Texto = "Longitude (GDEC) -180 e 180." }; } }
		public Mensagem LatitudeGdecForaDoEscopo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LatitudeGdecTexto", Prefixo), Texto = "Latitude (GDEC) -90 e 90." }; } }
		public Mensagem EastingUtmForaDoEscopo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_EastingUtmTexto", Prefixo), Texto = "Easting (UTM) deve estar entre 0,0001 e 9999999,99999." }; } }
		public Mensagem NorthingUtmForaDoEscopo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_NorthingUtmTexto", Prefixo), Texto = "Northing (UTM) deve estar entre 0,0001 e 9999999,99999." }; } }
		public Mensagem CoordenadaForaAbrangencia { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada", Prefixo), Texto = "Coordenada fora da área de abrangência." }; } }

		public Mensagem LocalColetaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_LocalColeta", Prefixo), Texto = "Local de coleta do ponto é obrigatório." }; } }
		public Mensagem FormaColetaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_Coordenada_FormaColeta", Prefixo), Texto = "Forma de coleta do ponto é obrigatória." }; } }
	}
}
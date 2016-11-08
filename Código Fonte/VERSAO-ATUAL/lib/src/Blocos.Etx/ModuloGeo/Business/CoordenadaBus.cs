using System;
using System.Data;
using System.Text.RegularExpressions;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao.Interno;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.Blocos.Etx.ModuloGeo.Business
{
	public class CoordenadaBus
	{
		#region Propriedades

		private static GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		
		public static string UsuarioGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		public const string MascaraGMS = @"^((-|\d)|(-?\d)|(-?\d\d)|(-?\d\d:|-?\d:)|(-?\d\d:\d|-?\d:\d)|(-?\d\d:[0-5]\d|-?\d:[0-5]\d)|(-?\d\d:[0-5]\d\:|-?\d:[0-5]\d\:)|(-?\d\d:\d\:|-?\d:\d\:)|(-?\d\d:[0-5]\d\:[0-5]|-?\d:[0-5]\d\:[0-5])|(-?\d\d:\d\:[0-5]|-?\d:\d\:[0-5])|(-?\d\d:[0-5]\d\:\d|-?\d:[0-5]\d\:\d)|(-?\d\d:\d\:\d|-?\d:\d\:\d)|(-?\d\d:[0-5]\d\:[0-5]\d|-?\d:[0-5]\d\:[0-5]\d)|(-?\d\d:\d\:[0-5]\d|-?\d:\d\:[0-5]\d)|(-?\d\d:[0-5]\d\:[0-5]\d\,|-?\d:[0-5]\d\:[0-5]\d\,)|(-?\d\d:\d\:[0-5]\d\,|-?\d:\d\:[0-5]\d\,)|(-?\d\d:[0-5]\d\:\d\,|-?\d:[0-5]\d\:\d\,)|(-?\d\d:\d\:\d\,|-?\d:\d\:\d\,)|(-?\d\d:[0-5]\d\:[0-5]\d\,\d{0,2}|-?\d:[0-5]\d\:[0-5]\d\,\d{0,2})|(-?\d\d:\d\:[0-5]\d\,\d{0,2}|-?\d:\d\:[0-5]\d\,\d{0,2})|(-?\d\d:[0-5]\d\:\d\,\d{0,2}|-?\d:[0-5]\d\:\d\,\d{0,2})|(-?\d\d:\d\:\d\,\d{0,2}|-?\d:\d\:\d\,\d{0,2}))$";

		#endregion

		public static Coordenada TransformarCoordenada(Coordenada coord)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando;
				#region Transformar a coordenada entre os tipos

				switch (coord.Tipo.Id)
				{
					case 1:
						#region Transformando GMS para UTM
						
						if (!coord.NorthingUtm.HasValue)
						{
							comando = bancoDeDados.CriarComando(@"begin {0}coordenada.gms2utm(:datum, {0}coordenada.formataGMS(:longitudeGMS), {0}coordenada.formataGMS(:latitudeGMS), :easting, :northing, :fuso, :hemisferio); end;", UsuarioGeo);

							comando.AdicionarParametroEntrada("datum", DbType.String, 20, coord.Datum.Sigla);
							comando.AdicionarParametroEntrada("longitudeGMS", DbType.String, 20, coord.LongitudeGms);
							comando.AdicionarParametroEntrada("latitudeGMS", DbType.String, 20, coord.LatitudeGms);
							comando.AdicionarParametroSaida("easting", DbType.Double);
							comando.AdicionarParametroSaida("northing", DbType.Double);
							comando.AdicionarParametroSaida("fuso", DbType.Int32);
							comando.AdicionarParametroSaida("hemisferio", DbType.Int32);

							bancoDeDados.ExecutarNonQuery(comando);

							coord.NorthingUtm = Convert.ToDouble(comando.ObterValorParametro("northing"));
							coord.EastingUtm = Convert.ToDouble(comando.ObterValorParametro("easting"));
							coord.FusoUtm = Convert.ToInt32(comando.ObterValorParametro("fuso"));
							coord.HemisferioUtm = Convert.ToInt32(comando.ObterValorParametro("hemisferio"));
						}

						#endregion

						#region Transformando GMS para GDEC

						if (!coord.LongitudeGdec.HasValue)
						{
							comando = bancoDeDados.CriarComando(@"begin {0}coordenada.gms2gdec({0}coordenada.formatagms(:longitudeGMS), {0}coordenada.formatagms(:latitudeGMS), :longitude, :latitude); end;", UsuarioGeo);

							comando.AdicionarParametroEntrada("longitudeGMS", DbType.String, 20, coord.LongitudeGms);
							comando.AdicionarParametroEntrada("latitudeGMS", DbType.String, 20, coord.LatitudeGms);
							comando.AdicionarParametroSaida("longitude", DbType.Double);
							comando.AdicionarParametroSaida("latitude", DbType.Double);

							bancoDeDados.ExecutarNonQuery(comando);

							coord.LongitudeGdec = Convert.ToDouble(comando.ObterValorParametro("longitude"));
							coord.LatitudeGdec = Convert.ToDouble(comando.ObterValorParametro("latitude"));
						}

						#endregion
						break;
						
					case 2:
						#region Transformando GDEC para UTM
						if (!coord.NorthingUtm.HasValue)
						{
							comando = bancoDeDados.CriarComando(@"begin {0}coordenada.gdec2utm(:datum, :longitude, :latitude, :easting, :northing, :fuso, :hemisferio); end;", UsuarioGeo);

							comando.AdicionarParametroEntrada("datum", coord.Datum.Sigla, DbType.String);
							comando.AdicionarParametroEntrada("longitude", coord.LongitudeGdec, DbType.Double);
							comando.AdicionarParametroEntrada("latitude", coord.LatitudeGdec, DbType.Double);
							comando.AdicionarParametroSaida("easting", DbType.Double);
							comando.AdicionarParametroSaida("northing", DbType.Double);
							comando.AdicionarParametroSaida("fuso", DbType.Int32);
							comando.AdicionarParametroSaida("hemisferio", DbType.Int32);

							bancoDeDados.ExecutarNonQuery(comando);

							coord.NorthingUtm = Convert.ToDouble(comando.ObterValorParametro("northing"));
							coord.EastingUtm = Convert.ToDouble(comando.ObterValorParametro("easting"));
							coord.FusoUtm = Convert.ToInt32(comando.ObterValorParametro("fuso"));
							coord.HemisferioUtm = Convert.ToInt32(comando.ObterValorParametro("hemisferio"));
						}

						#endregion

						#region Transformando GDEC para GMS

						if (string.IsNullOrEmpty(coord.LongitudeGms))
						{
							comando = bancoDeDados.CriarComando(@"begin {0}coordenada.gdec2gms(:longitudeGDEC, :latitudeGDEC, :longitude, :latitude); end;", UsuarioGeo);

							comando.AdicionarParametroEntrada("longitudeGDEC", coord.LongitudeGdec, DbType.Double);
							comando.AdicionarParametroEntrada("latitudeGDEC", coord.LatitudeGdec, DbType.Double);
							comando.AdicionarParametroSaida("longitude", DbType.String, 15);
							comando.AdicionarParametroSaida("latitude", DbType.String, 15);

							bancoDeDados.ExecutarNonQuery(comando);

							//Obtendo os valores
							coord.LongitudeGms = comando.ObterValorParametro("longitude").ToString();
							coord.LatitudeGms = comando.ObterValorParametro("latitude").ToString();
						}

						#endregion
						break;

					case 3:
						#region Transformando UTM para GMS
						if (string.IsNullOrEmpty(coord.LongitudeGms))
						{
							comando = bancoDeDados.CriarComando(@"begin {0}coordenada.utm2gms(:datum, :easting, :northing, :fuso, :hemisferio, :longitude, :latitude); end;", UsuarioGeo);

							comando.AdicionarParametroEntrada("datum", coord.Datum.Sigla, DbType.String);
							comando.AdicionarParametroEntrada("easting", coord.EastingUtm, DbType.Double);
							comando.AdicionarParametroEntrada("northing", coord.NorthingUtm, DbType.Double);
							comando.AdicionarParametroEntrada("fuso", coord.FusoUtm, DbType.Int32);
							comando.AdicionarParametroEntrada("hemisferio", 1, DbType.Int32); // 1 - Sul
							comando.AdicionarParametroSaida("longitude", DbType.String, 15);
							comando.AdicionarParametroSaida("latitude", DbType.String, 15);
							bancoDeDados.ExecutarNonQuery(comando);

							//Obtendo os valores
							coord.LongitudeGms = comando.ObterValorParametro("longitude").ToString();
							coord.LatitudeGms = comando.ObterValorParametro("latitude").ToString();
						}
						#endregion

						#region Transformando UTM para GDEC
						if (!coord.LongitudeGdec.HasValue)
						{
							comando = bancoDeDados.CriarComando(@"begin {0}coordenada.utm2gdec(:datum, :easting, :northing, :fuso, :hemisferio, :longitude, :latitude); end;", UsuarioGeo);

							comando.AdicionarParametroEntrada("datum", coord.Datum.Sigla, DbType.String);
							comando.AdicionarParametroEntrada("easting", coord.EastingUtm, DbType.Double);
							comando.AdicionarParametroEntrada("northing", coord.NorthingUtm, DbType.Double);
							comando.AdicionarParametroEntrada("fuso", coord.FusoUtm, DbType.Int32);
							comando.AdicionarParametroEntrada("hemisferio", 1, DbType.Int32); // 1 - Sul
							comando.AdicionarParametroSaida("longitude", DbType.Double);
							comando.AdicionarParametroSaida("latitude", DbType.Double);
							bancoDeDados.ExecutarNonQuery(comando);

							//Obtendo os valores
							coord.LongitudeGdec = Convert.ToDouble(comando.ObterValorParametro("longitude"));
							coord.LatitudeGdec = Convert.ToDouble(comando.ObterValorParametro("latitude"));
						}
						
						#endregion
						break;
				}
				#endregion
			}

			return coord;
		}

		public static bool EstaNaAbrangencia(Coordenada coord, string prefixo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				CoordenadaMsg msg = new CoordenadaMsg(prefixo);

				#region validar se está na abrangência

				Comando comando;

				#region Geometria

				comando = bancoDeDados.CriarComando(@"declare"
					+ " v_x number;"
					+ " v_y number;"
					+ " v_fuso number;"
					+ " v_hemi number;"
					+ " v_count number :=0;"
					+ " geo mdsys.sdo_geometry;"
				+ " begin"
					+ " case :tipo"
						+ " when 1 then /*Grau, minuto e Segundo : Geografico*/"
							+ " {0}coordenada.gms2utm(:datum_sigla, {0}coordenada.formatagms(:x), {0}coordenada.formatagms(:y), v_x, v_y, v_fuso, v_hemi);"
						+ " when 2 then /*Grau decimal: Geografico*/"
							+ " {0}coordenada.gdec2utm(:datum_sigla, :x, :y, v_x, v_y, v_fuso, v_hemi);"
						+ " when 3 then /*UTM: Projetado*/"
							+ " v_x := :x;"
							+ " v_y := :y;"
					+ " end case;"
					+ " v_count := {0}coordenada.EstaNaAbrangencia(v_x, v_y, :abrangencia);"
					+ " :resultado := v_count;"
				+ " end;", UsuarioGeo);

				comando.AdicionarParametroEntrada("tipo", coord.Tipo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("abrangencia", coord.Abrangencia??1, DbType.Int32);
				comando.AdicionarParametroEntrada("datum_sigla", coord.Datum.Sigla, DbType.String);
				comando.AdicionarParametroSaida("resultado", DbType.Int32);

				if (coord.Tipo.Id == 1)
				{
					comando.AdicionarParametroEntrada("x", coord.LongitudeGms, DbType.String);
					comando.AdicionarParametroEntrada("y", coord.LatitudeGms, DbType.String);
				}
				else if (coord.Tipo.Id == 2)
				{
					comando.AdicionarParametroEntrada("x", coord.LongitudeGdec, DbType.Double);
					comando.AdicionarParametroEntrada("y", coord.LatitudeGdec, DbType.Double);
				}
				else
				{
					comando.AdicionarParametroEntrada("x", coord.EastingUtm, DbType.Double);
					comando.AdicionarParametroEntrada("y", coord.NorthingUtm, DbType.Double);
				}

				#endregion

				bancoDeDados.ExecutarNonQuery(comando);

				if (comando.ObterValorParametro("resultado").ToString() == "0")
				{
					Validacao.Add(msg.CoordenadaForaAbrangencia);
				}

				#endregion
			}

			return Validacao.EhValido;
		}

		public static bool ValidarFormatoGMS(string coordenada)
		{
			if (String.IsNullOrEmpty(coordenada))
			{
				return false;
			}

			return Regex.IsMatch(coordenada.Replace("+", ""), MascaraGMS);
		}

		public static decimal? Gms2DGdec(string gms)
		{
			if (string.IsNullOrWhiteSpace(gms))
			{
				return null;
			}
			decimal? result = 0;
			decimal divisor = 1;
			decimal parsedNum = 0;
			string[] partes = gms.Split(':');
			if (partes.Length > 3)
			{
				return null;
			}
			for (int i = 0; i < partes.Length && i < 2; i++)
			{
				if (!String.IsNullOrWhiteSpace(partes[i]))
				{
					parsedNum = Convert.ToDecimal(partes[i]);
					result += parsedNum / divisor;
				}
				divisor *= 60;
			}
			return result;
		}

		public static bool Validar(Coordenada coordenada, string prefixo, bool isObrigatorio = false)
		{
			CoordenadaMsg msg = new CoordenadaMsg(prefixo);

			if (isObrigatorio && (coordenada == null || coordenada.Tipo.Id <= 0))
			{
				Validacao.Add(msg.CoordenadaObrigatoria);
				return false;
			}

			if (coordenada.Tipo.Id > 0)
			{
				if (coordenada.Datum.Id <= 0)
				{
					Validacao.Add(msg.DatumObrigatorio);
				}

				if (coordenada.Tipo.Id == 1) // GMS
				{
					decimal? gdecLongitude = Gms2DGdec(coordenada.LongitudeGms);
					decimal? gdecLatitude = Gms2DGdec(coordenada.LatitudeGms);

					if (gdecLongitude == null || String.IsNullOrWhiteSpace(coordenada.LongitudeGms))
					{
						Validacao.Add(msg.LongitudeGmsObrigatorio);
					}

					if (gdecLatitude == null || String.IsNullOrWhiteSpace(coordenada.LatitudeGms))
					{
						Validacao.Add(msg.LatitudeGmsObrigatorio);
					}

					if (!Validacao.EhValido)
					{
						return false;
					}

					if (!ValidarFormatoGMS(coordenada.LongitudeGms))
					{
						Validacao.Add(msg.LongitudeGmsFormato);
					}
					else if (gdecLongitude == null || gdecLongitude < -180 || gdecLongitude > 180)
					{
						Validacao.Add(msg.LongitudeGmsInvalida);
					}

					if (!ValidarFormatoGMS(coordenada.LatitudeGms))
					{
						Validacao.Add(msg.LatitudeGmsFormato);
					}
					else if (gdecLatitude == null || gdecLatitude < -90 || gdecLatitude > 90)
					{
						Validacao.Add(msg.LatitudeGmsInvalida);
					}
				}
				else if (coordenada.Tipo.Id == 2) // GDEC
				{
					if (coordenada.LongitudeGdec == null && String.IsNullOrEmpty(coordenada.LongitudeGdecTexto))
					{
						Validacao.Add(msg.LongitudeGdecObrigatorio);
					}

					if (coordenada.LatitudeGdec == null && String.IsNullOrEmpty(coordenada.LatitudeGdecTexto))
					{
						Validacao.Add(msg.LatitudeGdecObrigatorio);
					}

					if (!Validacao.EhValido)
					{
						return false;
					}

					if (!ValidarDouble(coordenada.LongitudeGdecTexto))
					{
						Validacao.Add(msg.LongitudeGdecInvalida);
					}

					if (!ValidarDouble(coordenada.LatitudeGdecTexto))
					{
						Validacao.Add(msg.LatitudeGdecInvalida);
					}

					if (coordenada.LongitudeGdec < -180 || coordenada.LongitudeGdec > 180)
					{
						Validacao.Add(msg.LongitudeGdecForaDoEscopo);
					}
					
					if (coordenada.LatitudeGdec < -90 || coordenada.LatitudeGdec > 90)
					{
						Validacao.Add(msg.LatitudeGdecForaDoEscopo);
					}
				}
				else if (coordenada.Tipo.Id == 3) // UTM
				{
					if (coordenada.EastingUtm.GetValueOrDefault() <= 0 && String.IsNullOrEmpty(coordenada.EastingUtmTexto))
					{
						Validacao.Add(msg.EastingUtmObrigatorio);
					}

					if (coordenada.NorthingUtm.GetValueOrDefault() <= 0 && String.IsNullOrEmpty(coordenada.NorthingUtmTexto))
					{
						Validacao.Add(msg.NorthingUtmObrigatorio);
					}

					if (coordenada.FusoUtm == 0)
					{
						Validacao.Add(msg.FusoUtmObrigatorio);
					}

					if (coordenada.HemisferioUtm == 0)
					{
						Validacao.Add(msg.HemisferioUtmObrigatorio);
					}

					if (!Validacao.EhValido)
					{
						return false;
					}

					if (!ValidarDouble(coordenada.EastingUtmTexto))
					{
						Validacao.Add(msg.EastingUtmInvalido);
					}

					if (!ValidarDouble(coordenada.NorthingUtmTexto))
					{
						Validacao.Add(msg.NorthingUtmInvalido);
					}

					if (coordenada.EastingUtm < 0.0001 || coordenada.EastingUtm > 9999999.99999)
					{
						Validacao.Add(msg.EastingUtmForaDoEscopo);
					}

					if (coordenada.NorthingUtm < 0.0001 || coordenada.NorthingUtm > 9999999.99999)
					{
						Validacao.Add(msg.NorthingUtmForaDoEscopo);
					}
				}
			}

			return Validacao.EhValido;
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
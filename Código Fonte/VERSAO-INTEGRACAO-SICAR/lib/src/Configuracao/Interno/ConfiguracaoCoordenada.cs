using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoCoordenada : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyTiposCoordenada = "TiposCoordenada";
		public List<CoordenadaTipo> TiposCoordenada { get { return _daLista.ObterTiposCoordenada(); } }

		public const string KeyDatuns = "Datuns";
		public List<Datum> Datuns { get { return _daLista.ObterDatuns(); } }

		public const string KeyFusos = "Fusos";
		public List<Fuso> Fusos { get {
			List<Fuso> lst = new List<Fuso>();
			lst.Add(new Fuso() { Id = 24, Texto = "24k", IsAtivo = true });
			return lst;
		} }

		public static string KeyHemisferios = "Hemisferios";
		public List<CoordenadaHemisferio> Hemisferios { get { return _daLista.ObterHemisferios(); } }

		public static string KeyLocalColetaPonto = "LocalColetaPonto";
		public List<Lista> LocalColetaPonto { get { return _daLista.ObterLocalColetaPonto(); } }

		public static string KeyFormaColetaPonto = "FormaColetaPonto";
		public List<Lista> FormaColetaPonto { get { return _daLista.ObterFormaColetaPonto(); } }

		public const String KeyUrlObterMunicipioCoordenada = "UrlObterMunicipioCoordenada";
		public String UrlObterMunicipioCoordenada { get { return _daLista.BuscarConfiguracaoSistema("geobaseswebservices") + "/municipio/ObterMunicipio"; } }
	}
}
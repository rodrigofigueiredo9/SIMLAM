using System;
using System.Collections.Generic;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoSimuladorGeo : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const String KeyDiretoriosArquivoTemp = "DiretoriosArquivoTemp";
		public Dictionary<Int32,String> DiretoriosArquivoTemp { get { return _daLista.ObterDiretorioArquivoTempPublicoGeo(); } }

		public const String KeyDiretoriosArquivo = "DiretoriosArquivo";
		public Dictionary<Int32, String> DiretoriosArquivo { get { return _daLista.ObterDiretorioArquivoPublicoGeo(); } }
	}
}
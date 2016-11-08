using System;
using System.Collections.Generic;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoArquivo : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const String KeyDiretoriosArquivoTemp = "DiretoriosArquivoTemp";
		public Dictionary<Int32,String> DiretoriosArquivoTemp { get { return _daLista.ObterDiretorioArquivoTemp(); } }

		public const String KeyDiretoriosArquivo = "DiretoriosArquivo";
		public Dictionary<Int32, String> DiretoriosArquivo { get { return _daLista.ObterDiretorioArquivo(); } }

		public const String KeyCredenciadoDiretoriosArquivoTemp = "CredenciadoDiretoriosArquivoTemp";
		public Dictionary<Int32, String> CredenciadoDiretoriosArquivoTemp { get { return _daLista.ObterCredenciadoDiretorioArquivoTemp(); } }

		public const String KeyCredenciadoDiretoriosArquivo = "CredenciadoDiretoriosArquivo";
		public Dictionary<Int32, String> CredenciadoDiretoriosArquivo { get { return _daLista.ObterCredenciadoDiretorioArquivo(); } }
	}
}
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoEndereco : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyEstados = "Estados";
		public List<Estado> Estados { get { return _daLista.ObterEstados(); } }

		public const string KeyMunicipios = "Municipios";
		public Dictionary<int, List<Municipio>> Municipios { get { return _daLista.ObterMunicipios(Estados); } }

		public const string KeyModulosFiscais = "ModulosFiscais";
		public Dictionary<int, ListaValor> ModulosFiscais { get { return _daLista.ObterModulosFiscais(); } }
	}
}
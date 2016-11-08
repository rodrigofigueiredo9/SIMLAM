using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoEmpreendimento: ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeySegmentos = "Segmentos";
		public List<Segmento> Segmentos { get { return _daLista.ObterSegmentos(); } }

		public const string KeyTiposResponsavel = "TiposResponsavel";
		public List<TipoResponsavel> TiposResponsavel { get { return _daLista.ObterEmpTipoResponsavel(); } }
	}
}
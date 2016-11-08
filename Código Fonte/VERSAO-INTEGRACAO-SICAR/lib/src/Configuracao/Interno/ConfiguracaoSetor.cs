using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoSetor : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyAgrupadoresSetor = "AgrupadoresSetor";
		public List<SetorAgrupador> AgrupadoresSetor { get { return _daLista.ObterAgrupadoresSetor(); } }
	}
}
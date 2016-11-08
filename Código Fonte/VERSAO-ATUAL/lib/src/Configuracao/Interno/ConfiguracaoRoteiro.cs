using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoRoteiro : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeySetores = "Setores";
		public List<Setor> Setores { get { return _daLista.ObterSetores(); } }

		public const string KeyItemTipos = "ItemTipos";
		public List<TipoItem> ItemTipos { get { return _daLista.ObterItemTipos(); } }
	}
}

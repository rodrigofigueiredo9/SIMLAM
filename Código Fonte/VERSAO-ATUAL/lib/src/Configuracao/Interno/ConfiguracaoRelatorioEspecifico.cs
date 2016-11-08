using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;


namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoRelatorioEspecifico : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyTipoRelatorioMapa = "TipoRelatorioMapa";
		public List<ListaValor> TipoRelatorioMapa { get { return _daLista.ObterTipoRelatorioMapa(); } }

	}
}

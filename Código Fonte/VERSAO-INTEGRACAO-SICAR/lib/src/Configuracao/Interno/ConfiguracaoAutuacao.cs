using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoAutuacao
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyObterAutuacaoNiveisPrecisao = "ObterAutuacaoNiveisPrecisao";
		public List<NivelPrecisao> ObterAutuacaoNiveisPrecisao { get { return _daLista.ObterAutuacaoNiveisPrecisao(); } }
	}
}
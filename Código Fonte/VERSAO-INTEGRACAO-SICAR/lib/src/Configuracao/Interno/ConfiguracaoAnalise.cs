using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoAnalise : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeySituacoesItemAnalise = "SituacoesItemAnalise";
		public List<Situacao> SituacoesItemAnalise { get { return _daLista.ObterSituacoesItemAnalise(); } }
	}
}
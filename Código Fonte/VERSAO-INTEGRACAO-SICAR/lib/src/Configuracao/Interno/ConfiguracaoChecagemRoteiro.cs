using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoChecagemRoteiro : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public static string KeySituacoesChecagemRoteiro = "SituacoesChecagemRoteiro";
		public List<Situacao> SituacoesChecagemRoteiro { get { return _daLista.ObterSituacaoChecagemRoteiro(); } }
	}
}
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoChecagemPendencia : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public static string KeySituacoesChecagemPendencia = "SituacoesChecagemPendencia";
		public static string KeySituacoesChecagemPendenciaItem = "SituacoesChecagemPendenciaItem";

		public List<Situacao> SituacoesChecagemPendencia { get { return _daLista.ObterSituacaoChecagemPendencia(); } }
		public List<Situacao> SituacoesChecagemPendenciaItem { get { return _daLista.ObterSituacaoChecagemPendenciaItem(); } }
	}
}
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoTramitacao : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyTramitacaoArquivoTipo = "TramitacaoArquivoTipo";
		public List<TramitacaoArquivoTipo> TramitacaoArquivoTipo { get { return _daLista.ObterTramitacaoArquivoTipos(); } }

		public const string KeyTramitacaoArquivoModo = "TramitacaoArquivoModo";
		public List<TramitacaoArquivoModo> TramitacaoArquivoModo { get { return _daLista.ObterTramitacaoArquivoModo(); } }

		public const string KeyAtividadeSolicitadaSituacoes = "AtividadeSolicitadaSituacoes";
		public List<Situacao> AtividadeSolicitadaSituacoes { get { return _daLista.ObterAtividadeSolicitadaSituacoes(); } }
	}
}
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoDocumento: ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyDocumentoTipos = "DocumentoTipos";
		public List<ProtocoloTipo> DocumentoTipos { get { return _daLista.ObterTiposDocumento(); } }

		public const string KeyAtividadesProcesso = "AtividadesProcesso";
		public List<ProcessoAtividadeItem> AtividadesProcesso { get { return _daLista.ObterAtividadesProcesso(); } }

		public const string KeySituacoesProcessoAtividade = "SituacoesProcessoAtividade";
		public List<Situacao> SituacoesProcessoAtividade { get { return _daLista.ObterSituacoesProcessoAtividade(); } }
	}
}

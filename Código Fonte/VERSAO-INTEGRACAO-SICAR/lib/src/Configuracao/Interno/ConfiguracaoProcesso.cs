using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoProcesso : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyProcessoTipos = "ProcessoTipos";
		public List<ProtocoloTipo> ProcessoTipos { get { return _daLista.ObterTiposProcesso(); } }

		public const string KeyAtividadesProcesso = "AtividadesProcesso";
		public List<ProcessoAtividadeItem> AtividadesProcesso { get { return _daLista.ObterAtividadesProcesso(); } }

		public const string KeyAtividadesSolicitadaTodas = "AtividadesSolicitadaTodas";
		public List<ProcessoAtividadeItem> AtividadesSolicitadaTodas { get { return _daLista.ObterAtividadesSolicitadaTodas(); } }

		public const string KeyAtividadesProcessoCredenciado = "AtividadesProcessoCredenciado";
		public List<ProcessoAtividadeItem> AtividadesProcessoCredenciado { get { return _daLista.ObtertividadesProcessoCredenciado(); } }

		public const string KeySituacoesProcessoAtividade = "SituacoesProcessoAtividade";
		public List<Situacao> SituacoesProcessoAtividade { get { return _daLista.ObterSituacoesProcessoAtividade(); } }
	}
}
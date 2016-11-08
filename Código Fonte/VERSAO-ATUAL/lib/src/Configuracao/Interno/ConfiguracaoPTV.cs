using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoPTV : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyPTVSituacao = "PTVSituacao";
		public List<Lista> PTVSituacao { get { return _daLista.ObterPTVSituacao(); } }

		public const string KeyPTVUnidadeMedida = "PTVUnidadeMedida";
		public List<Lista> PTVUnidadeMedida { get { return _daLista.ObterPTVUnidadeMedida(); } }

		public const string KeyTipoTransporte = "TipoTransporte";
		public List<Lista> TipoTransporte { get { return _daLista.ObterTipoTransporte(); } }

		public const string KeyPTVSolicitacaoSituacao = "PTVSolicitacaoSituacao";
		public List<Lista> PTVSolicitacaoSituacao { get { return _daLista.ObterPTVSolicitacaoSituacao(); } }
	}
}
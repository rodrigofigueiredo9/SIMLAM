using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoCadastroAmbientalRural : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		#region Recibo

		public const string KeyCadastroAmbientalRuralSolicitacaoSituacao = "CadastroAmbientalRuralSolicitacaoSituacao";
		public List<Lista> CadastroAmbientalRuralSolicitacaoSituacao { get { return _daLista.ObterCadastroAmbientalRuralSolicitacaoSituacao(); } }

        public const string KeySicarSituacao = "SicarSituacao";
		public List<Lista> SicarSituacao { get { return _daLista.ObterSicarSituacao(); } }

		public const string KeyCadastroAmbientalRuralSolicitacaoOrigem = "CadastroAmbientalRuralSolicitacaoOrigem";
		public List<Lista> CadastroAmbientalRuralSolicitacaoOrigem { get { return _daLista.ObterCadastroAmbientalRuralSolicitacaoOrigem(); } }

		#endregion
	}
}

using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoRequerimento : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyFinalidades = "Finalidades";
		public List<Finalidade> Finalidades { get { return _daLista.ObterFinalidades(); } }

		public const string KeyAgendamentoVistoria = "AgendamentoVistoria";
		public List<AgendamentoVistoria> AgendamentoVistoria { get { return _daLista.AgendamentoVistoria(); } }

		public const string KeySituacoesRequerimento = "SituacoesRequerimento";
		public List<Situacao> SituacoesRequerimento { get { return _daLista.ObterSituacoesRequerimento(); } }
	}
}
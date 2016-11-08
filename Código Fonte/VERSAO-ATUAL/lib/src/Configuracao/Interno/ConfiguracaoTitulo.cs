using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoTitulo : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeySituacoes = "Situacoes";
		public List<Situacao> Situacoes { get { return _daLista.ObterTituloSituacoes(); } }

		public const string KeyTituloDeclaratorioSituacoes = "TituloDeclaratorioSituacoes";
		public List<Situacao> TituloDeclaratorioSituacoes { get { return _daLista.ObterTituloDeclaratorioSituacoes(); } }

		public const string KeyTituloCondicionantePeriodicidades = "TituloCondicionantePeriodicidades";
		public List<TituloCondicionantePeriodTipo> TituloCondicionantePeriodicidades { get { return _daLista.ObterTituloCondicionantePeriodicidades(); } }

		public const string KeyTituloCondicionanteSituacoes = "TituloCondicionanteSituacoes";
		public List<TituloCondicionanteSituacao> TituloCondicionanteSituacoes { get { return _daLista.ObterTituloCondicionanteSituacoes(); } }

		public const string KeyTituloModeloTipos = "TituloModeloTipos";
		public List<TituloModeloTipo> TituloModeloTipos { get { return _daLista.ObterTituloModeloTiposLst(); } }

		public const string KeyTituloModeloProtocoloTipos = "TituloModeloProtocoloTipos";
		public List<TituloModeloProtocoloTipo> TituloModeloProtocoloTipos { get { return _daLista.ObterTituloModeloProtocoloTipos(); } }

		public const string KeyTituloModeloPeriodosRenovacoes = "TituloModeloPeriodosRenovacoes";
		public List<TituloModeloPeriodoRenovacao> TituloModeloPeriodosRenovacoes { get { return _daLista.ObterTituloModeloPeriodoRenovacao(); } }

		public const string KeyTituloModeloIniciosPrazos = "TituloModeloIniciosPrazos";
		public List<TituloModeloInicioPrazo> TituloModeloIniciosPrazos { get { return _daLista.ObterTituloModeloInicioPrazo(); } }

		public const string KeyTituloModeloTiposPrazos = "TituloModeloTiposPrazos";
		public List<TituloModeloTipoPrazo> TituloModeloTiposPrazos { get { return _daLista.ObterTituloModeloTipoPrazo(); } }

		public const string KeyTituloModeloAssinantes = "TituloModeloAssinantes";
		public List<TituloModeloAssinante> TituloModeloAssinantes { get { return _daLista.ObterTituloModeloAssinantes(); } }

		public const string KeyAlterarSituacaoAcoes = "AlterarSituacaoAcoes";
		public List<Acao> AlterarSituacaoAcoes { get { return _daLista.ObterAlterarSituacaoAcoes(); } }

		public const string KeyMotivosEncerramento = "MotivosEncerramento";
		public List<Motivo> MotivosEncerramento { get { return _daLista.ObterMotivosEncerramento(); } }

		public const string KeyDeclaratorioMotivosEncerramento = "DeclaratorioMotivosEncerramento";
		public List<Motivo> DeclaratorioMotivosEncerramento { get { return _daLista.ObterDeclaratorioMotivosEncerramento(); } }

		public const string KeyTituloModeloTipoDocumento = "TituloModeloTipoDocumento";
		public List<TituloModeloTipoDocumento> TituloModeloTipoDocumento { get { return _daLista.ObterTituloModeloTipoDocumento(); } }
	}
}

using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoFiscalizacao : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyComplementoDadosRespostas = "ComplementoDadosRespostas";
		public List<Lista> ComplementoDadosRespostas { get { return _daLista.ObterComplementoDadosRespostas(); } }

		public const string KeyComplementoDadosRendaMensal = "ComplementoDadosRendaMensal";
		public List<Lista> ComplementoDadosRendaMensal { get { return _daLista.ObterComplementoDadosRendaMensal(); } }

		public const string KeyComplementoDadosNivelEscolaridade = "ComplementoDadosNivelEscolaridade";
		public List<Lista> ComplementoDadosNivelEscolaridade { get { return _daLista.ObterComplementoDadosNivelEscolaridade(); } }

		public const string KeyComplementoDadosReservaLegalTipo = "ComplementoDadosReservaLegalTipo";
		public List<ReservaLegalLst> ComplementoDadosReservaLegalTipo { get { return _daLista.ObterComplementoDadosReservaLegalTipo(); } }

		public const string KeyMaterialApreendidoTipo = "MaterialApreendidoTipo";
		public List<Lista> MaterialApreendidoTipo { get { return _daLista.ObterMaterialApreendidoTipo(); } }		

		public const string KeyFiscalizacaoObjetoInfracaoCaracteristicaSolo = "FiscalizacaoObjetoInfracaoCaracteristicaSolo";
		public List<CaracteristicaSoloAreaDanificada> FiscalizacaoObjetoInfracaoCaracteristicaSolo { get { return _daLista.ObterFiscalizacaoObjetoInfracaoCaracteristicaSolo(); } }

		public const string KeyFiscalizacaoObjetoInfracaoSerie = "FiscalizacaoObjetoInfracaoSerie";
		public List<Lista> FiscalizacaoObjetoInfracaoSerie { get { return _daLista.ObterFiscalizacaoObjetoInfracaoSerie(); } }

		public const string KeyInfracaoClassificacao = "InfracaoClassificacao";
		public List<Lista> InfracaoClassificacao { get { return _daLista.ObterInfracaoClassificacao(); } }

		public const string KeyObterInfracaoCodigoReceita = "ObterInfracaoCodigoReceita";
		public List<Lista> ObterInfracaoCodigoReceita { get { return _daLista.ObterInfracaoCodigoReceita(); } }

		public const string KeyFiscalizacaoSituacao = "FiscalizacaoSituacao";
		public List<Lista> FiscalizacaoSituacao { get { return _daLista.ObterFiscalizacaoSituacao(); } }

		public const string KeyFiscalizacaoSerie = "FiscalizacaoSerie";
		public List<Lista> FiscalizacaoSerie { get { return _daLista.ObterFiscalizacaoSerie(); } }

		#region Configuracao 

		public const string KeyConfiguracaoFiscalizacaoCamposTipo = "ConfiguracaoFiscalizacaoCamposTipo";
		public List<Lista> ConfiguracaoFiscalizacaoCamposTipo { get { return _daLista.ObterConfiguracaoFiscalizacaoCamposTipo(); } }

		public const string KeyConfiguracaoFiscalizacaoCamposUnidade = "ConfiguracaoFiscalizacaoCamposUnidade";
		public List<Lista> ConfiguracaoFiscalizacaoCamposUnidade { get { return _daLista.ObterConfiguracaoFiscalizacaoCamposUnidade(); } }

		#endregion

		#region Acompanhamento

		public const string KeyAcompanhamentoFiscalizacaoSituacao = "AcompanhamentoFiscalizacaoSituacao";
		public List<Lista> AcompanhamentoFiscalizacaoSituacao { get { return _daLista.ObterAcompanhamentoFiscalizacaoSituacao(); } }

		public const string KeyAcompanhamentoFiscalizacaoCaracteristicaSolo = "AcompanhamentoFiscalizacaoCaracteristicaSolo";
		public List<CaracteristicaSoloAreaDanificada> AcompanhamentoFiscalizacaoCaracteristicaSolo { get { return _daLista.ObterAcompanhamentoFiscalizacaoCaracteristicaSolo(); } }

		public const string KeyAcompanhamentoFiscalizacaoReservaLegalTipo = "AcompanhamentoFiscalizacaoReservaLegalTipo";
		public List<ReservaLegalLst> AcompanhamentoFiscalizacaoReservaLegalTipo { get { return _daLista.ObterAcompanhamentoFiscalizacaoReservaLegalTipo(); } }

		#endregion Acompanhamento
	}
}
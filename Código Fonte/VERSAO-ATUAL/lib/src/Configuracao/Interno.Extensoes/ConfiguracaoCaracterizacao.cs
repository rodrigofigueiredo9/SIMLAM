using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes
{
	public class ConfiguracaoCaracterizacao : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		#region Caracterizações

		public const string KeyCaracterizacoesDependencias = "CaracterizacoesDependencias";
		public List<DependenciaLst> CaracterizacoesDependencias { get { return _daLista.ObterCaracterizacoesDependencias(); } }

		public const string KeyCaracterizacaoGeometriaTipo = "CaracterizacaoGeometriaTipo";
		public List<Lista> CaracterizacaoGeometriaTipo { get { return _daLista.ObterCaracterizacaoGeometriaTipo(); } }

		public const string KeyCaracterizacaoUnidadeMedida = "CaracterizacaoUnidadeMedida";
		public List<Lista> CaracterizacaoUnidadeMedida { get { return _daLista.ObterCaracterizacaoUnidadeMedida(); } }

		public const string KeyCaracterizacoes = "Caracterizacoes";
		public List<CaracterizacaoLst> Caracterizacoes { get { return _daLista.ObterCaracterizacoes(); } }

		#endregion

		#region Dominialidade

		public const string KeyDominialidadeDominioTipos = "DominialidadeDominioTipos";
		public List<Lista> DominialidadeDominioTipos { get { return _daLista.ObterDominialidadeDominioTipos(); } }

		public const string KeyDominialidadeComprovacoes = "DominialidadeComprovacoes";
		public List<Lista> DominialidadeComprovacoes { get { return _daLista.ObterDominialidadeComprovacoes(); } }

		public const string KeyDominialidadeReservaSituacoes = "DominialidadeReservaSituacoes";
		public List<Lista> DominialidadeReservaSituacoes { get { return _daLista.DominialidadeReservaSituacoes(); } }

		public const string KeyDominialidadeReservaLocalizacoes = "DominialidadeReservaLocalizacoes";
		public List<Lista> DominialidadeReservaLocalizacoes { get { return _daLista.DominialidadeReservaLocalizacoes(); } }

		public const string KeyDominialidadeReservaCartorios = "DominialidadeReservaCartorios";
		public List<Lista> DominialidadeReservaCartorios { get { return _daLista.DominialidadeReservaCartorios(); } }

		public const string KeyDominialidadeReservaSituacaoVegetacao = "DominialidadeReservaSituacaoVegetacao";
		public List<Lista> DominialidadeReservaSituacaoVegetacao { get { return _daLista.DominialidadeReservaSituacaoVegetacao(); } }


		#endregion

		#region RegularizacaoFundiaria

		public const string KeyRegularizacaoFundiariaHomologacao = "RegularizacaoFundiariaHomologacao";
		public List<Lista> RegularizacaoFundiariaHomologacao { get { return _daLista.ObterRegularizacaoFundiariaHomologacao(); } }

		public const string KeyRegularizacaoFundiariaRelacaoTrabalho = "RegularizacaoFundiariaRelacaoTrabalho";
		public List<RelacaoTrabalho> RegularizacaoFundiariaRelacaoTrabalho { get { return _daLista.ObterRegularizacaoFundiariaRelacaoTrabalho(); } }

		public const string KeyRegularizacaoFundiariaTipoLimite = "RegularizacaoFundiariaTipoLimite";
		public List<Lista> RegularizacaoFundiariaTipoLimite { get { return _daLista.ObterRegularizacaoFundiariaTipoLimite(); } }

		public const string KeyRegularizacaoFundiariaTipoRegularizacao = "RegularizacaoFundiariaTipoRegularizacao";
		public List<Lista> RegularizacaoFundiariaTipoRegularizacao { get { return _daLista.ObterRegularizacaoFundiariaTipoRegularizacao(); } }

		public const string KeyRegularizacaoFundiariaTipoUso = "RegularizacaoFundiariaTipoUso";
		public List<UsoAtualSoloLst> RegularizacaoFundiariaTipoUso { get { return _daLista.ObterRegularizacaoFundiariaTipoUso(); } }

		#endregion

		#region ExploracaoFlorestal

		public const string KeyExploracaoFlorestalClassificacaoVegetal = "ExploracaoFlorestalClassificacaoVegetal";
		public List<Lista> ExploracaoFlorestalClassificacaoVegetal { get { return _daLista.ObterExploracaoFlorestalClassificacaoVegetal(); } }

		public const string KeyExploracaoFlorestalExploracao = "ExploracaoFlorestalExploracao";
		public List<Lista> ExploracaoFlorestalExploracao { get { return _daLista.ObterExploracaoFlorestalExploracao(); } }

		public const string KeyExploracaoFlorestalFinalidadeExploracao = "ExploracaoFlorestalFinalidadeExploracao";
		public List<FinalidadeExploracao> ExploracaoFlorestalFinalidadeExploracao { get { return _daLista.ObterExploracaoFlorestalFinalidadeExploracao(); } }

		public const string KeyCaracterizacaoProdutosExploracao = "CaracterizacaoProdutosExploracao";
		public List<Lista> CaracterizacaoProdutosExploracao { get { return _daLista.ObterCaracterizacaoProdutosExploracao(); } }

		public const string KeyCaracterizacaoDestinacaoMaterialLenhoso = "CaracterizacaoDestinacaoMaterialLenhoso";
		public List<Lista> CaracterizacaoDestinacaoMaterialLenhoso { get { return _daLista.ObterCaracterizacaoDestinacaoMaterialLenhoso(); } }

		public const string KeyTipoExploracaoFlorestal = "TipoExploracaoFlorestal";
		public List<Lista> TipoExploracaoFlorestal { get { return _daLista.ObterTipoExploracaoFlorestal(); } }
		#endregion

		#region Queima Controlada

		public const string KeyQueimaControladaCultivoTipo = "QueimaControladaCultivoTipo";
		public List<Lista> QueimaControladaCultivoTipo { get { return _daLista.ObterCultivoTipo(); } }

		#endregion

		#region Secagem Mecânica de Grãos

		public const string KeySecagemMecanicaGraosMateriaPrimaConsumida = "SecagemMecanicaGraosMateriaPrimaConsumida";
		public List<Lista> SecagemMecanicaGraosMateriaPrimaConsumida { get { return _daLista.ObterSecagemMateriaPrimaConsumida(); } }

		#endregion

		#region Descrição de Licenciamento de Atividade

		public const string KeyDscLicAtvFontesAbastecimentoAguaTipo = "DscLicAtvFontesAbastecimentoAguaTipo";
		public List<Lista> DscLicAtvFontesAbastecimentoAguaTipo { get { return _daLista.ObterDscLicAtvFontesAbastecimentoAguaTipo(); } }

		public const string KeyDscLicAtvPontosLancamentoEfluenteTipo = "DscLicAtvPontosLancamentoEfluenteTipo";
		public List<Lista> DscLicAtvPontosLancamentoEfluenteTipo { get { return _daLista.ObterDscLicAtvPontosLancamentoEfluenteTipo(); } }

		public const string KeyDscLicAtvOutorgaAguaTipo = "DscLicAtvOutorgaAguaTipo";
		public List<Lista> DscLicAtvOutorgaAguaTipo { get { return _daLista.ObterDscLicAtvOutorgaAguaTipo(); } }

		public const string KeyDscLicAtvFontesGeracaoTipo = "DscLicAtvFontesGeracaoTipo";
		public List<Lista> DscLicAtvFontesGeracaoTipo { get { return _daLista.ObterDscLicAtvFontesGeracaoTipo(); } }

		public const string KeyDscLicAtvUnidadeTipo = "DscLicAtvUnidadeTipo";
		public List<Lista> DscLicAtvUnidadeTipo { get { return _daLista.ObterDscLicAtvUnidadeTipo(); } }

		public const string KeyDscLicAtvCombustivelTipo = "DscLicAtvCombustivelTipo";
		public List<Lista> DscLicAtvCombustivelTipo { get { return _daLista.ObterDscLicAtvCombustivelTipo(); } }

		public const string KeyDscLicAtvVegetacaoAreaUtil = "DscLicAtvVegetacaoAreaUtil";
		public List<Lista> DscLicAtvVegetacaoAreaUtil { get { return _daLista.ObterDscLicAtvVegetacaoAreaUtil(); } }

		public const string KeyDscLicAtvAcondicionamento = "DscLicAtvAcondicionamento";
		public List<Lista> DscLicAtvAcondicionamento { get { return _daLista.ObterDscLicAtvAcondicionamento(); } }

		public const string KeyDscLicAtvEstocagem = "DscLicAtvEstocagem";
		public List<Lista> DscLicAtvEstocagem { get { return _daLista.ObterDscLicAtvEstocagem(); } }

		public const string KeyDscLicAtvTratamento = "DscLicAtvTratamento";
		public List<Lista> DscLicAtvTratamento { get { return _daLista.ObterDscLicAtvTratamento(); } }

		public const string KeyDscLicAtvDestinoFinal = "DscLicAtvDestinoFinal";
		public List<Lista> DscLicAtvDestinoFinal { get { return _daLista.ObterDscLicAtvDestinoFinal(); } }

		#endregion

		#region Producao Carvao Vegetal

		public const string KeyProducaoCarvaoVegetalMateriaPrimaConsumida = "ProducaoCarvaoVegetalMateriaPrimaConsumida";
		public List<Lista> ProducaoCarvaoVegetalMateriaPrimaConsumida { get { return _daLista.ObterProducaoCarvaoVegetalMateriaPrimaConsumida(); } }

		#endregion

		#region Avicultura

		public const string KeyAviculturaConfinamentoFinalidades = "AviculturaConfinamentoFinalidades";
		public List<Lista> AviculturaConfinamentoFinalidades { get { return _daLista.ObterAviculturaConfinamentoFinalidades(); } }

		#endregion

		#region Suinocultura

		public const string KeySuinoculturaFases = "SuinoculturaFases";
		public List<Lista> SuinoculturaFases { get { return _daLista.ObterSuinoculturaFases(); } }

		#endregion

		#region Beneficiamento e tratamento de madeira

		public const string KeyBeneficiamentoMadeiraMateriaPrimaConsumida = "BeneficiamentoMadeiraMateriaPrimaConsumida";
		public List<Lista> BeneficiamentoMadeiraMateriaPrimaConsumida { get { return _daLista.ObterBeneficiamentoMadeiraMateriaPrimaConsumida(); } }

		#endregion

		#region Registro de atividade florestal

		public const string KeyRegistroAtividadeFlorestalFonte = "RegistroAtividadeFlorestalFonte";
		public List<ListaValor> RegistroAtividadeFlorestalFonte { get { return _daLista.ObterRegistroAtividadeFlorestalFonte(); } }

		public const string KeyRegistroAtividadeFlorestalUnidade = "RegistroAtividadeFlorestalUnidade";
		public List<ListaValor> RegistroAtividadeFlorestalUnidade { get { return _daLista.ObterRegistroAtividadeFlorestalUnidade(); } }

		public const string KeyRegAtvFlorestalNumeroMaximo = "RegAtvFlorestalNumeroMaximo";
		public int RegAtvFlorestalNumeroMaximo { get { return Convert.ToInt32(_daLista.BuscarConfiguracaoSistema("crt_reg_atv_flor_numeroregistromaximo")); } }

		#endregion

		#region Barragem

		public const string KeyBarragemFinalidades = "BarragemFinalidades";
		public List<Lista> BarragemFinalidades { get { return _daLista.ObterBarragemFinalidades(); } }

		public const string KeyBarragemOutorgas = "BarragemOutorgas";
		public List<Lista> BarragemOutorgas { get { return _daLista.ObterBarragemOutorgas(); } }

		#endregion

		#region Silvicultura

		public const string KeySilviculturaCulturasFlorestais = "SilviculturaCulturasFlorestais";
		public List<Lista> SilviculturaCulturasFlorestais { get { return _daLista.ObterSilviculturaCulturasFlorestais(); } }

		#endregion

		#region Silvicultura - Implantação da Atividade de Silvicultura (Fomento)

		public const string KeySilviculturaAtvCaracteristicaFomento = "SilviculturaAtvCaracteristicaFomento";
		public List<Lista> SilviculturaAtvCaracteristicaFomento { get { return _daLista.ObterSilviculturaAtvCaracteristicaFomento(); } }

		public const string KeySilviculturaAtvCoberturaExitente = "SilviculturaAtvCoberturaExitente";
		public List<Lista> SilviculturaAtvCoberturaExitente { get { return _daLista.ObterSilviculturaAtvCoberturaExitente(); } }

		#endregion

		#region Informacao de Corte

		public const string KeyDestinacaoMaterial = "DestinacaoMaterial";
		public List<Lista> DestinacaoMaterial { get { return _daLista.ObterDestinacaoMaterial(); } }

		#endregion

		#region Pulverização Aérea de Produtos Agrotóxicos

		public const string KeyPulverizacaoProdutoCulturas = "PulverizacaoProdutoCulturas";
		public List<Lista> PulverizacaoProdutoCulturas { get { return _daLista.ObterPulverizacaoProdutoCulturas(); } }

		#endregion

		#region Barragem para Dispensa de Licença Ambiental

		public const string KeyBarragemDispensaLicencaBarragemTipo = "BarragemDispensaLicencaBarragemTipo";
		public List<Lista> BarragemDispensaLicencaBarragemTipo { get { return _daLista.ObterBarragemDispensaLicencaBarragemTipo(); } }

		public const string KeyBarragemDispensaLicencaFinalidadeAtividade = "BarragemDispensaLicencaFinalidadeAtividade";
		public List<Lista> BarragemDispensaLicencaFinalidadeAtividade { get { return _daLista.ObterBarragemDispensaLicencaFinalidadeAtividade(); } }

		public const string KeyBarragemDispensaLicencaFase = "BarragemDispensaLicencaFase";
		public List<Lista> BarragemDispensaLicencaFase { get { return _daLista.ObterBarragemDispensaLicencaFase(); } }

		public const string KeyBarragemDispensaLicencaMongeTipo = "BarragemDispensaLicencaMongeTipo";
		public List<Lista> BarragemDispensaLicencaMongeTipo { get { return _daLista.ObterBarragemDispensaLicencaMongeTipo(); } }

		public const string KeyBarragemDispensaLicencaVertedouroTipo = "BarragemDispensaLicencaVertedouroTipo";
		public List<Lista> BarragemDispensaLicencaVertedouroTipo { get { return _daLista.ObterBarragemDispensaLicencaVertedouroTipo(); } }

		public const string KeyBarragemDispensaLicencaFormacaoRT = "BarragemDispensaLicencaFormacaoRT";
		public List<Lista> BarragemDispensaLicencaFormacaoRT { get { return _daLista.ObterBarragemDispensaLicencaFormacaoRT(); } }

		#endregion Barragem para Dispensa de Licença Ambiental
	}
}
using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloAutorizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCadastro.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertidao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEscritura;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEscritura.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloNotificacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOficio.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business
{
	public static class EspecificiadadeBusFactory
	{
		public static bool Possui(int modeloCodigo)
		{
			return Enum.IsDefined(typeof(eEspecificidade), modeloCodigo);
		}

		public static IEspecificidadeBus Criar(int modeloCodigo)
		{
			if (!Possui(modeloCodigo))
			{
				return new EspecificidadeBusDefault();
			}

			eEspecificidade modeloEsp = (eEspecificidade)modeloCodigo;

			switch (modeloEsp)
			{
				case eEspecificidade.OficioNotificacao:
					return new OficioNotificacaoBus();

				case eEspecificidade.CertificadoRegistro:
					return new CertificadoRegistroBus();

				case eEspecificidade.LaudoConstatacao:
					return new LaudoConstatacaoBus();

				case eEspecificidade.LaudoVistoriaFlorestal:
				case eEspecificidade.LaudoVistoriaQueimaControlada:
					return new LaudoVistoriaFlorestalBus();

				case eEspecificidade.LaudoVistoriaFundiaria:
					return new LaudoVistoriaFundiariaBus();

				case eEspecificidade.AutorizacaoExploracaoFlorestal:
					return new AutorizacaoExploracaoFlorestalBus();

				case eEspecificidade.EscrituraPublicaDoacao:
					return new EscrituraPublicaDoacaoBus();

				case eEspecificidade.EscrituraPublicaCompraVenda:
					return new EscrituraPublicaCompraVendaBus();

				case eEspecificidade.CadComercProdutosAgrotoxicos:
					return new CadComercProdutosAgrotoxicosBus();

				case eEspecificidade.CadAplicProdutosAgrotoxicos:
					return new CadAplicProdutosAgrotoxicosBus();

				case eEspecificidade.CertidaoAnuencia:
					return new CertidaoAnuenciaBus();

				case eEspecificidade.LicencaPorteUsoMotosserra:
					return new LicencaPorteUsoMotosserraBus();

				case eEspecificidade.TermoAprovacaoMedicao:
					return new TermoAprovacaoMedicaoBus();

				case eEspecificidade.AutorizacaoQueimaControlada:
					return new AutorizacaoQueimaControladaBus();

				case eEspecificidade.CertificadoCadastroProdutoVegetal:
					return new CertificadoCadastroProdutoVegetalBus();

				case eEspecificidade.LicencaOperacao:
					return new LicencaOperacaoBus();

				case eEspecificidade.LicencaInstalacao:
					return new LicencaInstalacaoBus();

				case eEspecificidade.LicencaPrevia:
					return new LicencaPreviaBus();

				case eEspecificidade.LicencaAmbientalRegularizacao:
					return new LicencaAmbientalRegularizacaoBus();

				case eEspecificidade.LicencaAmbientalUnica:
					return new LicencaAmbientalUnicaBus();

				case eEspecificidade.LicencaSimplificada:
					return new LicencaSimplificadaBus();

				case eEspecificidade.LaudoVistoriaLicenciamento:
					return new LaudoVistoriaLicenciamentoBus();

				case eEspecificidade.LaudoFundiarioSimplificado:
					return new LaudoFundiarioSimplificadoBus();

				case eEspecificidade.CertificadoRegistroAtividadeFlorestal:
					return new CertificadoRegistroAtividadeFlorestalBus();

				case eEspecificidade.LicencaOperacaoFomento:
					return new LicencaOperacaoFomentoBus();

				case eEspecificidade.OutrosReciboEntregaCopia:
					return new OutrosReciboEntregaCopiaBus();

				case eEspecificidade.OutrosConclusaoTransferenciaDominio:
					return new OutrosConclusaoTransferenciaDominioBus();

				case eEspecificidade.NotificacaoEmbargo:
					return new NotificacaoEmbargoBus();

				case eEspecificidade.CertidaoCartaAnuencia:
					return new CertidaoCartaAnuenciaBus();

				case eEspecificidade.TermoCPFARL:
					return new TermoCPFARLBus();

				case eEspecificidade.LaudoAuditoriaFomentoFlorestal:
					return new LaudoAuditoriaFomentoFlorestalBus();

				case eEspecificidade.LaudoVistoriaFomentoFlorestal:
					return new LaudoVistoriaFomentoFlorestalBus();

				case eEspecificidade.TermoCPFARLR:
					return new TermoCPFARLRBus();

				case eEspecificidade.OutrosLegitimacaoTerraDevoluta:
					return new OutrosLegitimacaoTerraDevolutaBus();

				case eEspecificidade.OutrosInformacaoCorte:
					return new OutrosInformacaoCorteBus();

				case eEspecificidade.OutrosInformacaoCorteDeclaratorio:
					return new OutrosInformacaoCorteDeclaratorioBus();

				case eEspecificidade.CertidaoDebito:
					return new CertidaoDebitoBus();

				case eEspecificidade.OficioUsucapiao:
					return new OficioUsucapiaoBus();

				case eEspecificidade.CadastroAmbientalRuralTitulo:
					return new CadastroAmbientalRuralTituloBus();


				case eEspecificidade.TermoCompromissoAmbiental:
					return new TermoCompromissoAmbientalBus();

				case eEspecificidade.AberturaLivroUnidadeProducao:
					return new AberturaLivroUnidadeProducaoBus();

				case eEspecificidade.AberturaLivroUnidadeConsolidacao:
					return new AberturaLivroUnidadeConsolidacaoBus();

				case eEspecificidade.CertificadoCadastroProdutoAgrotoxico:
					return new CertificadoCadastroProdutoAgrotoxicoBus();

				case eEspecificidade.TermoCPFARLC:
					return new TermoCPFARLCBus();

				case eEspecificidade.TermoCPFARLCR:
					return new TermoCPFARLCRBus();

				//Declaração de Dispensa de Licenciamento Ambiental de Barragem
				case eEspecificidade.CertidaoDispensaLicenciamentoAmbiental:
					return new CertidaoDispensaLicenciamentoAmbientalBus();

				default:
					return new EspecificidadeBusDefault();
			}
		}
	}
}
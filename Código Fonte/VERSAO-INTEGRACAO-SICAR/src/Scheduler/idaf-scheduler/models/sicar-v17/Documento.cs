using System.Collections.Generic;
using System.ComponentModel;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class Documento
	{
		//Domínio do campo TIPO
		public const string TipoPropriedade = "PROPRIEDADE";
		public const string TipoPosse = "POSSE";
		public const string TipoConcessao = "CONCESSAO";

		//Domínio do campo TIPODOCUMENTOPROPRIEDADE
		public const string TipoDocPropCertidaoRegistro = "CREG";
		public const string TipoDocPropContratoCompraVenda = "CCVE";
		public const string TipoDocPropEmRegularizacao = "EREG";
		public const string TipoDocPropEscritura = "ESCR";
		public const string TipoDocPropImissaoPosse = "IMPO";

		//Domínio do campo TIPODOCUMENTOPOSSE
		public const string TipoDocPosseAutorizOcupacao = "AOCP";
		public const string TipoDocPosseCartaAnuencia = "CANU";
		public const string TipoDocPosseConcesRealDireitoUso = "CRDU";
		public const string TipoDocPosseContrAlienaTerrasPublicas = "CATP";
		public const string TipoDocPosseContrConcesDomTerrasPublicas = "CCDP";
		public const string TipoDocPosseContrConcesTerrasPublicas = "CCTP";
		public const string TipoDocPosseContrTransAforramento = "CTAF";
		public const string TipoDocPosseLicencaOcupacao = "LOCP";
		public const string TipoDocPosseTermoDoacao = "TDOA";
		public const string TipoDocPosseTituloProfCondResolutiva = "TPCR";
		public const string TipoDocPosseTituloDefReservaFlorestal = "TDRF";
		public const string TipoDocPosseTituloDefSufReRatificacao = "TDRR";
		public const string TipoDocPosseTituloDefTransferido = "TDOF";
		public const string TipoDocPosseTituloDominio = "TDOM";
		public const string TipoDocPosseTituloReconDominio = "TRDO";
		public const string TipoDocPosseTituloRatificao = "TRAT";
		public const string TipoDocPosseContAssentOrgaoFundiario = "CAOF";
		public const string TipoDocPosseContPromessaCompraVenda = "CPCV";
		public const string TipoDocPosseDeclarSindicatoRural = "DSRU";
		public const string TipoDocPosseDeclarAssentMunicipal = "DAMU";
		public const string TipoDocPosseDeclarConfrontantes = "DCON";
		public const string TipoDocPosseTermoAutodeclaracao = "ADEC";

		//Domínio do campo TIPODOCUMENTOCONCESSAO
		public const string TipoDocConcesContrConcesDireRealUso = "CDRU";
		public const string TipoDocConcesContConcessUso = "CCU";
		public const string TipoDocConcesEmRegularizacao = "EREG";
		public const string TipoDocConcesTituloDominio = "TIDO";

		[DefaultValue("")]
		public string tipo { get; set; }

		[DefaultValue("")]
		public string denominacao { get; set; }

		[DefaultValue(0.0)]
		public double area { get; set; }

		public List<string> proprietariosPosseirosConcessionarios { get; set; }

		[DefaultValue("")]
		public string tipoDocumentoPropriedade { get; set; }

		[DefaultValue("")]
		public string tipoDocumentoPosse { get; set; }

		public List<string> tipoDocumentoConcessao { get; set; }
		public DetalheDocumentoPropriedade detalheDocumentoPropriedade { get; set; }
		public DetalheDocumentoPosse detalheDocumentoPosse { get; set; }
		public ReservaLegal reservaLegal { get; set; }

		[DefaultValue("")]
		public string ccir { get; set; }

		[DefaultValue("")]
		public string certificacaoIncra { get; set; }
	}
}
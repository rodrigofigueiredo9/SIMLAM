using System.ComponentModel;
using GeoJSON.Net;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class Geo
	{
		//Domínio do campo TIPO
		public const string TipoAreaImovel = "AREA_IMOVEL";
		public const string TipoAreaInfraestrutuaPublica = "AREA_INFRAESTRUTURA_PUBLICA";
		public const string TipoAreaUtilidadePublica = "AREA_UTILIDADE_PUBLICA";
		public const string TipoAreaReservatorioEnergia = "RESERVATORIO_ENERGIA";
		public const string TipoAreaEntornoReservatorioEnergia = "AREA_ENTORNO_RESERVATORIO_ENERGIA";
		public const string TipoAreaServidaoAdminTotal = "AREA_SERVIDAO_ADMINISTRATIVA_TOTAL";
		public const string TipoAreaImovelLiquida = "AREA_IMOVEL_LIQUIDA";
		public const string TipoRioAte10 = "RIO_ATE_10";
		public const string TipoRio10A50 = "RIO_10_A_50";
		public const string TipoRio50A200 = "RIO_50_A_200";
		public const string TipoRio200A600 = "RIO_200_A_600";
		public const string TipoRioAcima600 = "RIO_ACIMA_600";
		public const string TipoNascenteOlhoDagua = "NASCENTE_OLHO_DAGUA";
		public const string TipoLagoNatural = "LAGO_NATURAL";
		public const string TipoReservArtificialDecorBarramento = "RESERVATORIO_ARTIFICIAL_DECORRENTE_BARRAMENTO";
		public const string TipoVereda = "VEREDA";
		public const string TipoAreaTopoMorro = "AREA_TOPO_MORRO";
		public const string TipoAreaDeclivMaior45 = "AREA_DECLIVIDADE_MAIOR_45";
		public const string TipoBordaChapada = "BORDA_CHAPADA";
		public const string TipoRestinga = "RESTINGA";
		public const string TipoManguezal = "MANGUEZAL";
		public const string TipoAreaAltitudeSuperior1800 = "AREA_ALTITUDE_SUPERIOR_1800";
		public const string TipoAppTotal = "APP_TOTAL";
		public const string TipoAreaConsolidada = "AREA_CONSOLIDADA";
		public const string TipoAppAreaVn = "APP_AREA_VN";
		public const string TipoVegetacaoNativa = "VEGETACAO_NATIVA";
		public const string TipoAreaPousio = "AREA_POUSIO";
		public const string TipoArlProposta = "ARL_PROPOSTA";
		public const string TipoArlAverbada = "ARL_AVERBADA";
		public const string TipoArlAprovadaNaoAverbada = "ARL_APROVADA_NAO_AVERBADA";
		public const string TipoArlTotal = "ARL_TOTAL";
		public const string TipoArlARecuperar = "ARL_A_RECUPERAR";
		public const string TipoAppARecuperar = "APP_A_RECUPERAR";
        public const string TipoAppCalculada = "APP_CALCULADA";
        public const string TipoEscadinhaCalculada = "ESCADINHA_CALCULADA";

		[DefaultValue("")]
		public string tipo { get; set; }

		public GeoJSONObject geoJson { get; set; }
		public double area { get; set; }

		//public double? largura { get; set; }

		/*[DefaultValue("EPSG:31983")]
		public string proj { get; set; }*/
	}
}
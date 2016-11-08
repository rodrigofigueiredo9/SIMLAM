

using System;

namespace Tecnomapas.EtramiteX.Interno.Areas.GeoProcessamento.ViewModels.VMMapa
{
	public class CoordenadaVM
	{
		public String CoordenadaGeoUrl { get; set; }
        public String DesenhadorWebserviceURL { get; set; } /* http://devap2/projetos/Etramite2010/IDAF/Desenvolvimento/DesenhadorWebServices */
        public String MunicipioWebserviceURL { get; set; }/* http://devap2/Projetos/Etramite2010/IDAF/GeobasesWebServices/Municipio */
        public String GeoprocessamentoWebserviceURL { get; set; }/* http://devap2/Projetos/Etramite2010/IDAF/Desenvolvimento/Institucional/geoprocessamento/mapa */
        public String MapaTematicoURL { get; set; }/* http://atlas.suportetm.com.br/ArcGIS/rest/services/GEOBASES/Vetor_Aerolevantamento/MapServer */
        public String AeroLevantamentoMapaImagemURL { get; set; }/* http://atlas.suportetm.com.br/ArcGIS/rest/services/GEOBASES/Imagem_Aerolevantamento/MapServer */
        public String DevEmpreendimentoMapaLoteURL { get; set; }/*  http://devap3/ArcGIS/rest/services/IDAF/DEV_EMPREENDIMENTO/MapServer */
        public String FiscalMapaLoteURL { get; set; }/* http://devap3/ArcGIS/rest/services/IDAF/DEV_D_FISCAL/MapServer */
		public int Modo { get; set; }
	}
}
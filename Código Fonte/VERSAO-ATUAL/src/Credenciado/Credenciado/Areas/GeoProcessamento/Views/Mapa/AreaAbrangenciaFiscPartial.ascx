<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.GeoProcessamento.ViewModels.VMMapa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CoordenadaVM>" %>

<div class="divFlash"></div>

<script language="JavaScript" type="text/javascript">
	$('.divFlash').flashembed({ src: '<%= Model.CoordenadaGeoUrl %>', height: '550px', w3c: true, version: [11, 0, 0] });
</script>

<script language="JavaScript" type="text/javascript">
	function saveAndClose(result) {
		Modal.fechar(Coordenada.container);
		if (Coordenada.settings.callBackSalvarCoordenada) {
			Coordenada.settings.callBackSalvarCoordenada(result);
		}
	}

	function justClose() {
		//self.parent.tb_remove();
	}

	function loadMapCoordinates() 
	{
		var mapa = $('.divFlash object').get(0);  //DOM elements

		Coordenada.settings.easting = Coordenada.settings.easting||"0";
		Coordenada.settings.northing = Coordenada.settings.northing||"0";
		Coordenada.settings.easting2 = Coordenada.settings.easting2||"0";
		Coordenada.settings.northing2 = Coordenada.settings.northing2||"0";

		mapa.externalLoadMapCoordinates(Coordenada.settings.easting,
			Coordenada.settings.northing,
			Coordenada.settings.easting2,
			Coordenada.settings.northing2,
			Coordenada.settings.empreendimentoEasting, 
			Coordenada.settings.empreendimentoNorthing);
	}
</script>
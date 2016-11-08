<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.GeoProcessamento.ViewModels.VMMapa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CoordenadaVM>" %>

<div class="divFlash"></div>

<script language="JavaScript" type="text/javascript">
	$('.divFlash').flashembed({ src: '<%= Model.CoordenadaGeoUrl %>loc_emp.swf', height: '475px', w3c: true, version: [11, 0, 0] });
</script>

<script language="JavaScript" type="text/javascript">
	function saveAndClose(result) {
		if (Coordenada.settings.callBackSalvarCoordenada) {
			Coordenada.settings.callBackSalvarCoordenada(result);
		}
		Modal.fechar(Coordenada.container);
	}

	function justClose() {
		//self.parent.tb_remove();
	}

	function loadMapCoordinates() {
		var mapa = $('.divFlash object').get(0);  //DOM elements
		mapa.externalLoadMapCoordinates(Coordenada.settings.easting, Coordenada.settings.northing, Coordenada.settings.pagemode);
	}
</script>
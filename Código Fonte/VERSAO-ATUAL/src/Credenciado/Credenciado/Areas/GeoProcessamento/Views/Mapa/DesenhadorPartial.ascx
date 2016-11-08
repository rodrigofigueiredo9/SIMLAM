<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.Areas.GeoProcessamento.ViewModels.VMMapa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CoordenadaVM>" %>

<div class="divFlash"></div>

<script language="JavaScript" type="text/javascript">
	Navegador.settings.urls.atualizarSessao = '<%= Url.Action("AtualizarSessao", "Mapa", new {area="GeoProcessamento"}) %>';
	$('.divFlash').flashembed({ src: '<%= Model.CoordenadaGeoUrl %>', height: '800px', w3c: true, version: [11, 0, 0] });    
</script>

<script language="JavaScript" type="text/javascript">

	var desenhador = $('.divFlash object').get(0);

	function saveAndClose(result) {
		Modal.fechar(Coordenada.container);
	}

	/*function obterConfiguracoes() {
	    return JSON.stringify({ webserviceURL: 'http://devap2/projetos/Etramite2010/IDAF/Desenvolvimento/DesenhadorWebServices' });
	}*/

	function fecharDesenhador() {
		Modal.fechar(Coordenada.container);
	}

	function onProcessar() {
		Navegador.settings.onProcessar();
	}

	function onCancelar() {
		Navegador.settings.onCancelar();
	}

	function setSituacaoProcessamento(objeto) {
		if (!desenhador) {
			desenhador = $('.divFlash object').get(0);
		}
		if (!desenhador || !desenhador.setSituacaoProcessamento) {
			return;
		}

		desenhador.setSituacaoProcessamento(objeto);
	}

	function onBaixarArquivo(objeto) {
		Navegador.settings.onBaixarArquivo(objeto);
	}

	function obterSituacaoInicial() {
		setSituacaoProcessamento(Navegador.settings.obterSituacaoInicial());
	}

	function obterAreaAbrangencia() {
		var areaAbrangencia = Navegador.settings.obterAreaAbrangencia();
		desenhador.obterAreaAbrangencia(areaAbrangencia);
	}

	function atualizarSessao() {
		return Navegador.atualizarSessao();
	}

	function initDesenhador() {
		desenhador.width = Navegador.settings.width;
		desenhador.height = Navegador.settings.height;
		Navegador.settings.setSituacaoProcessamento = setSituacaoProcessamento;
		Navegador.navegadorElemnto = desenhador;
		desenhador.loadDesenhador(Navegador.settings.id, Navegador.settings.tipo, Navegador.settings.modo);

		$(window).resize(function () {

			var width = $(window).width();
			var height = $(window).height();
			var minWidth = 1065;
			var minHeight = 490;

			if (width < minWidth && height < minHeight) {
				return;
			}

			if (width < minWidth && height > minHeight) {
				desenhador.height = height - 100;
				desenhador.ajustarTamanho(minWidth, height - 100);
				return;
			}

			if (height < minHeight) {
				desenhador.width = width - 100;
				desenhador.ajustarTamanho(width - 100, minHeight);
				return;
			}

			desenhador.width = width - 100;
			desenhador.height = height - 100;
			desenhador.ajustarTamanho(width - 100, height - 100);
		});
	}
</script>
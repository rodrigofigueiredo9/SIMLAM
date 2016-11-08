/// <reference path="../masterpage.js" />
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />

TituloModeloVisualizar = {
	urlObterAssinantes: null,
	urlObterUltimoNumero: null,
	urlEnviarArquivo: null,
	container: null,
	Mensagens: null,

	load: function (container) {
		TituloModeloVisualizar.container = container;
		TituloModeloVisualizar.onConfigurarTela();
		container.delegate('.titFiltros', 'click', TituloModeloVisualizar.expandirFiltro);
	},

	onConfigurarTela: function () {
		TituloModeloVisualizar.onMostrarDivPossuiPrazo();
		TituloModeloVisualizar.onMostrarDivRenovacao();
		TituloModeloVisualizar.onMostrarDivEmail();
		TituloModeloVisualizar.onMostrarDivDias();
		TituloModeloVisualizar.onMostrarDivFaseAnterior();
	},

	expandirFiltro: function () {

		var container = $(this).closest('fieldset');

		$('.titFiltros', container).toggleClass('fAberto');

		if ($('.titFiltro', container).parent().find('.fixado').length == 0) {
			if ($('.filtroCorpo', container).is(':animated')) {
				$('.filtroCorpo', container).stop(true, true);
				$('.titFiltros', container).toggleClass('fAberto');
			} else {
				$('.filtroCorpo', container).slideToggle('normal');
			}
		} else {
			if ($('.filtroCorpo > div', container).children().not('.fixado').is(':animated')) {
				$('.filtroCorpo > div', container).children().not('.fixado').stop(true, true);
				$('.titFiltros', container).toggleClass('fAberto');
			} else {
				$('.titFiltros > div', container).children().not('.fixado').slideToggle('normal');
			}
		}
	},

	onMostrarDivFaseAnterior: function () {
		if ($('.radFaseAnteriorS').attr('checked')) {
			$('.divFaseAnteriror').show();
		} else {
			$('.divFaseAnteriror').hide();
		}
	},

	onMostrarDivPossuiPrazo: function () {

		if ($('.radPossuiPrazoS').attr('checked')) {
			$('.divPossuiPrazo').show();
		} else {
			$('.divPossuiPrazo').hide();
		}
	},

	onMostrarDivRenovacao: function () {

		if ($('.radPassivelRenovacaoS').attr('checked')) {
			$('.divPeriodoRenovacao').show();
		} else {
			$('.divPeriodoRenovacao').hide();
		}
	},

	onMostrarDivEmail: function () {

		if ($('.radEnviarEmailS').attr('checked')) {
			$('.divEnviarEmail').show();
		} else {
			$('.divEnviarEmail').hide();
		}
	},

	onMostrarDivDias: function () {
		if ($('.ddlPeriodoRenovacao').val() == '2') {
			$('.divDiasPeriodoRenovacao').show();
		} else {
			$('.divDiasPeriodoRenovacao').hide();
		}
	}
}
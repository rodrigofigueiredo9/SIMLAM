/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
Indicadores = {

	urlModalIndicadores: "",

	load: function () {
	},

	onAbrirModalTitulo: function () {
		Indicadores.abrirModal(this, 1);
	},

	abrirModal: function (ctr, tipo) {
		var getData = { Tipo: tipo };
		getData["periodo"] = $(ctr).parent().find("span").text();

		if (parseInt($(ctr).find("p").text()) > 0) {
			Modal.abrir(Indicadores.urlModalIndicadores, getData, "IndicadoresModal.load", Modal.tamanhoModalPequena);
		}
	}
};

$(document).ready(Indicadores.load);
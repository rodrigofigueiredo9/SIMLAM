/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

HabilitarEmissaoCFOCFOC = {
	container: null,
	settings: {
	},

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(HabilitarEmissaoCFOCFOC.settings, options); }

		container.listarAjax();
		container.listarAjax('ultimaBusca');
		HabilitarEmissaoCFOCFOC.container = container;
	},
	
	onFiltrar: function () {
	}
}
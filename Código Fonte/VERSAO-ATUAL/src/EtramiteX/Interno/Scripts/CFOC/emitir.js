/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.ddl.js" />

CFOCEmitir = {
	settings: {
		urls: {
			loteVisualizar: null
		}
	},
	container: null,

	load: function (container, options) {

		if (options) {
			$.extend(CFOCEmitir.settings, options);
		}

		CFOCEmitir.container = MasterPage.getContent(container);
		container.delegate('.btnVisualizarLote', 'click', CFOCEmitir.visualizarLote);

		Mascara.load(CFOCEmitir.container);
	},

	visualizarLote: function () {
		var item = JSON.parse($(this).closest('tr').find('.hdnItemJSon').val());
		Modal.abrir(CFOCEmitir.settings.urls.loteVisualizar + '/' + item.LoteId, null, function (container) {
			Lote.load(container);
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);
	}
}
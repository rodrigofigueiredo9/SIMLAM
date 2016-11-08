/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../jquery.listar-ajax.js" />
/// <reference path="../masterpage.js" />

CFOCListar = {
	urlVisualizar: null,
	urlPDF: null,
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(CFOCListar.settings, options); }

		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnVisualizar', 'click', CFOCListar.visualizar);
		container.delegate('.btnPDF', 'click', CFOCListar.gerarPDF);

		Aux.setarFoco(container);
		CFOCListar.container = container;
	},

	obterItemJson: function (container) {
		return $.parseJSON($(container).closest('tr').find('.itemJson').val());
	},

	visualizar: function () {
		var item = CFOCListar.obterItemJson(this);
		MasterPage.redireciona(CFOCListar.urlVisualizar + '/' + item.Id);
	},

	gerarPDF: function () {
		var item = CFOCListar.obterItemJson(this);
		MasterPage.redireciona(CFOCListar.urlPDF + '/' + item.Id);
	}
}
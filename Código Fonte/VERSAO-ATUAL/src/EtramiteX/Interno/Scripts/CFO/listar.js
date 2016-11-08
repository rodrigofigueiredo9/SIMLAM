/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../jquery.listar-ajax.js" />
/// <reference path="../masterpage.js" />

CFOListar = {
	urlVisualizar: null,
	urlPDF: null,
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(CFOListar.settings, options); }

		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnVisualizar', 'click', CFOListar.visualizar);
		container.delegate('.btnPDF', 'click', CFOListar.gerarPDF);

		Aux.setarFoco(container);
		CFOListar.container = container;
	},

	obterItemJson: function (container) {
		return $.parseJSON($(container).closest('tr').find('.itemJson').val());
	},

	visualizar: function () {
		var item = CFOListar.obterItemJson(this);
		MasterPage.redireciona(CFOListar.urlVisualizar + '/' + item.Id);
	},

	gerarPDF: function () {
		var item = CFOListar.obterItemJson(this);
		MasterPage.redireciona(CFOListar.urlPDF + '/' + item.Id);
	}
}
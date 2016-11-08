/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

AgrotoxicoListar = {
	urlPDF: null,
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(AgrotoxicoListar.settings, options); }

		container.listarAjax();
		container.delegate('.btnPDF', 'click', AgrotoxicoListar.gerarPDF);
		AgrotoxicoListar.container = container;
		
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},
	
	gerarPDF: function () {
		var objeto = AgrotoxicoListar.obter(this);
		MasterPage.redireciona(AgrotoxicoListar.urlPDF + '/' + objeto.ArquivoId);		
	}
}
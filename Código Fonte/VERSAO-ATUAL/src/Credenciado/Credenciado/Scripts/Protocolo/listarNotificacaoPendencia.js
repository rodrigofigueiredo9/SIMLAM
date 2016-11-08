/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

NotificacaoPendenciaListar = {
	container: null,

	load: function (container) {
		NotificacaoPendenciaListar.container = MasterPage.getContent(container);

		NotificacaoPendenciaListar.container.delegate('.btnPDF', 'click', NotificacaoPendenciaListar.pdf);
	},

	pdf: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		MasterPage.redireciona($('.urlPdf', NotificacaoPendenciaListar.container).val() + '/' + itemId);
	}
}
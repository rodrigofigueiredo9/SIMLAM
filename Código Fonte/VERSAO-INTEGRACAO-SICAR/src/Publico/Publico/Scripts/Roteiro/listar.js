/// <reference path="Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

RoteiroListar = {
	urlPdfRoteiro: '',
	container: null,

	load: function (container, fnAssociar) {
		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnPdf', 'click', RoteiroListar.PdfGerar);

		Aux.setarFoco(container);
		RoteiroListar.container = container;
		MasterPage.load();
	},

	PdfGerar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(RoteiroListar.urlPdfRoteiro + '/' + itemId);
	}
}
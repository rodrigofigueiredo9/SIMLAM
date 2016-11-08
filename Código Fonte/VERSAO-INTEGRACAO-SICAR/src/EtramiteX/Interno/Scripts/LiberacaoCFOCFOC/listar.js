LiberacaoCFOCFOCListar = {
	urlVisualizar: null,
	urlGerarPDF: null,

	container: null,

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(LiberacaoCFOCFOCListar.settings, options); }

		container.listarAjax();
		container.delegate('.btnVisualizar', 'click', LiberacaoCFOCFOCListar.visualizar);
		container.delegate('.btnGerarPDF', 'click', LiberacaoCFOCFOCListar.gerarPDF);

		Aux.setarFoco(container);
		LiberacaoCFOCFOCListar.container = container;
	},

	obter: function (container) {
		return parseInt($(container).closest('tr').find('.itemId').val());
	},
		
	visualizar: function () {
		var liberacaoId = LiberacaoCFOCFOCListar.obter(this);
		MasterPage.redireciona(LiberacaoCFOCFOCListar.urlVisualizar + '/' + liberacaoId);
	},

	gerarPDF: function () {
		var liberacaoId = LiberacaoCFOCFOCListar.obter(this);
		MasterPage.redireciona(LiberacaoCFOCFOCListar.urlGerarPDF + '/' + liberacaoId);
	}
}
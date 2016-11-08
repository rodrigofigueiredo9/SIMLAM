/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

FichaFundiariaListar = {
	settings: {
		urls: {
			visualizar: '',
			editar: '',
			excluir: '',
			excluirConfirm: null
		}
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(FichaFundiariaListar.settings, options); }

		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnVisualizar', 'click', FichaFundiariaListar.visualizar);
		container.delegate('.btnEditar', 'click', FichaFundiariaListar.editar);
		container.delegate('.btnExcluir', 'click', FichaFundiariaListar.excluir);
		container.delegate('.btnGerarPdf', 'click', FichaFundiariaListar.gerarPdf);

		Aux.setarFoco(container);
		FichaFundiariaListar.container = container;
	},

	gerarPdf: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona($('.urlGerarPdf', FichaFundiariaListar.container).val() + '/' + itemId);
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(FichaFundiariaListar.settings.urls.visualizar + '/' + itemId);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(FichaFundiariaListar.settings.urls.editar + '/' + itemId);
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': FichaFundiariaListar.settings.urls.excluirConfirm,
			'urlAcao': FichaFundiariaListar.settings.urls.excluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});
	}
}
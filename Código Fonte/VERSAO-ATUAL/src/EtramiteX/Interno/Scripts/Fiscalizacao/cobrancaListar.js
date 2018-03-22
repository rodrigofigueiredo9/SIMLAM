/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

CobrancaListar = {
	container: null,
	settings: {
		urls: {
			urlEditar: '',
			urlVisualizar: ''
		}
	},

	load: function (container, options) {
		if (options) { $.extend(CobrancaListar.settings, options); }
		CobrancaListar.container = MasterPage.getContent(container);
		CobrancaListar.container.listarAjax({ onAfterFiltrar: CobrancaListar.gerenciarSituacao });

		//CobrancaListar.container.delegate('.btnExcluir', 'click', ConfigurarListarParametrizacao.excluir);
		//CobrancaListar.container.delegate('.btnVisualizar', 'click', ConfigurarListarParametrizacao.visualizar);
		//CobrancaListar.container.delegate('.btnEditar', 'click', ConfigurarListarParametrizacao.editar);

		Aux.setarFoco(container);
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': CobrancaListar.settings.urls.urlExcluirConfirm,
			'urlAcao': CobrancaListar.settings.urls.urlExcluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(CobrancaListar.settings.urls.urlVisualizar + "/" + itemId);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(CobrancaListar.settings.urls.urlEditar + '/' + itemId);
	}
}
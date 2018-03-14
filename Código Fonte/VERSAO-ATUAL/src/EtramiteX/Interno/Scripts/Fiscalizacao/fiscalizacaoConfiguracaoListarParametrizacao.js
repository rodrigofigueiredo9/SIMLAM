/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ConfigurarListarParametrizacao = {
	container: null,
	settings: {
		urls: {
			urlExcluirConfirm: '',
			urlEditar: '',
			urlVisualizar: '',
			urlExcluir: ''
		}
	},

	load: function (container, options) {
		if (options) { $.extend(ConfigurarListarParametrizacao.settings, options); }
		ConfigurarListarParametrizacao.container = MasterPage.getContent(container);
		ConfigurarListarParametrizacao.container.listarAjax({ onAfterFiltrar: ConfigurarListarParametrizacao.gerenciarSituacao });

		ConfigurarListarParametrizacao.container.delegate('.btnExcluir', 'click', ConfigurarListarParametrizacao.excluir);
		ConfigurarListarParametrizacao.container.delegate('.btnVisualizar', 'click', ConfigurarListarParametrizacao.visualizar);
		ConfigurarListarParametrizacao.container.delegate('.btnEditar', 'click', ConfigurarListarParametrizacao.editar);

		Aux.setarFoco(container);
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': ConfigurarListarParametrizacao.settings.urls.urlExcluirConfirm,
			'urlAcao': ConfigurarListarParametrizacao.settings.urls.urlExcluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(ConfigurarListarParametrizacao.settings.urls.urlVisualizar + "/" + itemId);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(ConfigurarListarParametrizacao.settings.urls.urlEditar + '/' + itemId);
	}
}
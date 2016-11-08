/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

FiscalizacaoConfiguracaoListar = {
	settings: {
		urls: {
			urlExcluirConfirm: '',
			urlEditar: '',
			urlVisualizar: '',
			urlExcluir: ''
		}
	},

	load: function (container, options) {
		if (options) { $.extend(FiscalizacaoConfiguracaoListar.settings, options); }
		FiscalizacaoConfiguracaoListar.container = MasterPage.getContent(container);
		FiscalizacaoConfiguracaoListar.container.listarAjax();

		FiscalizacaoConfiguracaoListar.container.delegate('.btnExcluir', 'click', FiscalizacaoConfiguracaoListar.excluir);
		FiscalizacaoConfiguracaoListar.container.delegate('.btnVisualizar', 'click', FiscalizacaoConfiguracaoListar.visualizar);
		FiscalizacaoConfiguracaoListar.container.delegate('.btnEditar', 'click', FiscalizacaoConfiguracaoListar.editar);
		Aux.setarFoco(container);
	},
	excluir: function () {
		Modal.excluir({
			'urlConfirm': FiscalizacaoConfiguracaoListar.settings.urls.urlExcluirConfirm,
			'urlAcao': FiscalizacaoConfiguracaoListar.settings.urls.urlExcluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});	
	},
	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(FiscalizacaoConfiguracaoListar.settings.urls.urlVisualizar + "/" + itemId);	
	},
	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(FiscalizacaoConfiguracaoListar.settings.urls.urlEditar + '/' + itemId);
	}
}
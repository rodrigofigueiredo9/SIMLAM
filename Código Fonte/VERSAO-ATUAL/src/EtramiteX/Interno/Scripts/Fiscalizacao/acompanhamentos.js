/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

Acompanhamentos = {
	container: null,
	settings: {
		urls: {
			novo: null,
			visualizar: null,
			editar: null,
			excluir: null,
			excluirConfirm: null,
			alterarSituacao: null,
			pdf: null
		}
	},

	load: function (container, options) {
		if (options) { $.extend(Acompanhamentos.settings, options); }
		Acompanhamentos.container = MasterPage.getContent(container);

		Acompanhamentos.container.delegate('.btnNovoAcompanhamento', 'click', Acompanhamentos.novo);
		Acompanhamentos.container.delegate('.btnVisualizar', 'click', Acompanhamentos.visualizar);
		Acompanhamentos.container.delegate('.btnEditar', 'click', Acompanhamentos.editar);
		Acompanhamentos.container.delegate('.btnExcluir', 'click', Acompanhamentos.excluir);
		Acompanhamentos.container.delegate('.btnAlterarSituacao', 'click', Acompanhamentos.alterarSituacao);
		Acompanhamentos.container.delegate('.btnPDFAcompanhamento', 'click', Acompanhamentos.pdf);
	},

	novo: function () {
		MasterPage.redireciona(Acompanhamentos.settings.urls.novo);
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(Acompanhamentos.settings.urls.visualizar + '/' + itemId);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(Acompanhamentos.settings.urls.editar + '/' + itemId);
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': Acompanhamentos.settings.urls.excluirConfirm,
			'urlAcao': Acompanhamentos.settings.urls.excluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this,
			'naoExecutarUltimaBusca': true,
			'callBack': function (response, btnExcluir) { $(btnExcluir).closest('tr').remove(); }
		});
	},

	alterarSituacao: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(Acompanhamentos.settings.urls.alterarSituacao + '/' + itemId);
	},

	pdf: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(Acompanhamentos.settings.urls.pdf + '/' + itemId);
		MasterPage.carregando(false);
	}
}
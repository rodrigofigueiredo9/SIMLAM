/// <reference path="../../masterpage.js" />

ExploracaoFlorestalListar = {
	container: null,
	settings: {
	},

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(ExploracaoFlorestalListar.settings, options); }

		container.listarAjax();
		container.delegate('.btnVisualizar', 'click', ExploracaoFlorestalListar.visualizar);
		container.delegate('.btnEditar', 'click', ExploracaoFlorestalListar.editar);
		container.delegate('.btnExcluir', 'click', ExploracaoFlorestalListar.excluir);

		Aux.setarFoco(container);
		ExploracaoFlorestalListar.container = container;
		$('.btnBuscar', container).click();
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona($('.urlVisualizar', ExploracaoFlorestalListar.container).val() + "/" + itemId);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona($('.urlEditar', ExploracaoFlorestalListar.container).val() + "/" + itemId);
	},

	excluir: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		Modal.excluir({
			'urlConfirm': $('.urlExcluirConfirm', ExploracaoFlorestalListar.container).val() + "/" + itemId,
			'urlAcao': $('.urlExcluir', ExploracaoFlorestalListar.container).val() + "/" + itemId,
			'id': $('.hdnEmpreendimento', ExploracaoFlorestalListar.container).val(),
			'callBack': ExploracaoFlorestalListar.callBackExcluirCaracterizacao,
			'naoExecutarUltimaBusca': true
		});
	},

	callBackExcluirCaracterizacao: function (data) {
		MasterPage.redireciona(data.urlRedireciona);
	}
}
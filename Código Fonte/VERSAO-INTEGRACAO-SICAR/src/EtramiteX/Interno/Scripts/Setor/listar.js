/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

SetorListar = {
	urlEditar: '',
	container: null,

	load: function (container) {
		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnVisualizar', 'click', SetorListar.visualizar);
		container.delegate('.btnEditar', 'click', SetorListar.editar);

		Aux.setarFoco(container);
		SetorListar.container = container;
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(SetorListar.urlEditar + '/' + itemId);
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		var content = MasterPage.getContent($(this, SetorListar.container));

		if (SetorListar.associarFuncao) {
			Modal.abrir($('.urlVisualizar', content).val() + "/" + itemId, null, function (context) {
				Modal.defaultButtons(context);
			});
		} else {
			MasterPage.redireciona($('.urlVisualizar', content).val() + "/" + itemId);
		}
	}
}
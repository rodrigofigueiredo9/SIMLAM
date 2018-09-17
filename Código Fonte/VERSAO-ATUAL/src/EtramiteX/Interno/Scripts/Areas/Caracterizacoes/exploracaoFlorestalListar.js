/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ExploracaoFlorestalListar = {
	container: null,
	settings: {
	},

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(ExploracaoFlorestalListar.settings, options); }

		container.delegate('.btnVisualizar', 'click', ExploracaoFlorestalListar.visualizar);

		Aux.setarFoco(container);
		ExploracaoFlorestalListar.container = container;
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		if (ExploracaoFlorestalListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', ExploracaoFlorestalListar.container).val() + "/" + itemId, null, function (container) {
				Modal.defaultButtons(container);
			});
		} else {
			MasterPage.redireciona($('.urlVisualizar', ExploracaoFlorestalListar.container).val() + "/" + itemId);
		}
	}
}
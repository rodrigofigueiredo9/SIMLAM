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
		CobrancaListar.container.listarAjax();

		CobrancaListar.container.delegate('.btnVisualizar', 'click', CobrancaListar.visualizar);
		CobrancaListar.container.delegate('.btnEditar', 'click', CobrancaListar.editar);

		CobrancaListar.container.delegate('.radioAutuadoCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioAutuadoCpfCnpj', CobrancaListar.container));
		Aux.setarFoco(container);
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
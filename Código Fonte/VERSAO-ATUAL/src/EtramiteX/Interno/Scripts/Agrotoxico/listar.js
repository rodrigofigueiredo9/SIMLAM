AgrotoxicoListar = {
	urlExcluir: null,
	urlExcluirConfirm: null,
	urlVisualizar: null,
	urlEditar: null,

	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(AgrotoxicoListar.settings, options); }

		container.listarAjax();
		container.delegate('.btnEditar', 'click', AgrotoxicoListar.editar);
		container.delegate('.btnVisualizar', 'click', AgrotoxicoListar.visualizar);
		container.delegate('.btnExcluir', 'click', AgrotoxicoListar.excluir);

		Aux.setarFoco(container);
		AgrotoxicoListar.container = container;
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	excluir: function () {
		var objeto = AgrotoxicoListar.obter(this);
		Modal.excluir({
			'urlConfirm': AgrotoxicoListar.urlExcluirConfirm,
			'urlAcao': AgrotoxicoListar.urlExcluir,
			'id': objeto.Id,
			'btnExcluir': this
		});
	},

	visualizar: function () {
		var objeto = AgrotoxicoListar.obter(this);
		MasterPage.redireciona(AgrotoxicoListar.urlVisualizar + '/' + objeto.Id);
	},

	editar: function () {
		var objeto = AgrotoxicoListar.obter(this);
		MasterPage.redireciona(AgrotoxicoListar.urlEditar + '/' + objeto.Id);
	}
}
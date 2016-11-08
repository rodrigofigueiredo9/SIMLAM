/// <reference path="../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />

PersonalizadoListar = {
	urlExecutar: '',
	urlEditar: '',
	urlExcluirConfirm: '',
	urlExcluir: '',
	urlExportar: '',
	container: null,

	load: function (container) {
		container = MasterPage.getContent(container);
		PersonalizadoListar.container = container;
		container.listarAjax();

		container.delegate('.btnExecutar', 'click', PersonalizadoListar.executar);
		container.delegate('.btnEditar', 'click', PersonalizadoListar.editar);
		container.delegate('.btnExcluir', 'click', PersonalizadoListar.excluir);
		container.delegate('.btnExportar', 'click', PersonalizadoListar.exportar);
		container.delegate('.btnAtribuirExecutor', 'click', PersonalizadoListar.atribuirExecutor);

		Aux.setarFoco(container);
	},

	obter: function (elemento) {
		return parseInt($(elemento).closest('div').find('.itemId:first').val());
	},

	executar: function () {
		MasterPage.redireciona(PersonalizadoListar.urlExecutar + '/' + PersonalizadoListar.obter(this));
	},

	atribuirExecutor: function () {
		MasterPage.redireciona(PersonalizadoListar.urlAtribuirExecutor + '/' + PersonalizadoListar.obter(this));
	},

	editar: function () {
		MasterPage.redireciona(PersonalizadoListar.urlEditar + '/' + PersonalizadoListar.obter(this));
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': PersonalizadoListar.urlExcluirConfirm,
			'urlAcao': PersonalizadoListar.urlExcluir,
			'id': PersonalizadoListar.obter(this),
			'btnExcluir': this
		});
	},
	
	exportar: function () {
		MasterPage.redireciona(PersonalizadoListar.urlExportar + '/' + PersonalizadoListar.obter(this));
	}
}
/// <reference path="../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />

PersonalizadoListar = {
	urlExecutar: '',
	container: null,

	load: function (container) {
		container = MasterPage.getContent(container);
		PersonalizadoListar.container = container;
		container.listarAjax();

		container.delegate('.btnExecutar', 'click', PersonalizadoListar.executar);
		Aux.setarFoco(container);
	},

	obter: function (elemento) {
		return parseInt($(elemento).closest('div').find('.itemId:first').val());
	},

	executar: function () {
		MasterPage.redireciona(PersonalizadoListar.urlExecutar + '/' + PersonalizadoListar.obter(this));
	}
}
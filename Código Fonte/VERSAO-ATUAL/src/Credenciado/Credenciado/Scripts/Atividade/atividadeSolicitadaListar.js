/// <reference path="Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../masterpage.js" />

AtividadeSolicitadaListar = {
	defaults: {
		'onAssociarCallback': null
	},
	settings: {},

	load: function (container, options) {
		container = MasterPage.getContent(container);
		container.listarAjax();

		AtividadeSolicitadaListar.settings = $.extend({}, AtividadeSolicitadaListar.defaults, options);
		container.delegate('.btnAssociarAtividade', 'click', AtividadeSolicitadaListar.associar);

		Modal.defaultButtons(container);
		Aux.setarFoco(container);
	},

	associar: function () {
		var modal = Modal.getModalContent(this);
		var linha = $(this).closest('tr');

		var itemJson = $.parseJSON($('.itemJson', linha).val());

		var msg = AtividadeSolicitadaListar.settings.onAssociarCallback(itemJson);

		if (msg !== undefined && msg.length > 0) {
			Mensagem.gerar(modal, msg);
		} else {
			Modal.fechar(modal);
		}
	}
}
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

CulturaListar = {
	settings: {
		onAssociarCallback: null
	},
	urlEditar: null,

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(CulturaListar.settings, options); }

		container.listarAjax();
		container.delegate('.btnAssociar', 'click', CulturaListar.associar);

		Aux.setarFoco(container);
		CulturaListar.container = container;
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	associar: function () {
		var objeto = CulturaListar.obter(this);
		var sucesso = CulturaListar.settings.onAssociarCallback(objeto);
		if (sucesso) {
			Modal.fechar(CulturaListar.container);
		} else {
			Mensagem.gerar(CulturaListar.container, msgErro);
		}
	}
}
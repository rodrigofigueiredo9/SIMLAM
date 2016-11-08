/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />

EPTVListar = {
	settings: {
		urls: {
			urlComunicador: null,
			urlAnalisar: null,
			urlValidarAcessoComunicador: null,
			urlComunicadorPTV: null
		}
	},

	container: null,

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(EPTVListar.settings, options); }

		EPTVListar.container = container;
		container.listarAjax();
		container.delegate('.btnAnalisar', 'click', EPTVListar.analisar);
		container.delegate('.btnComunicador', 'click', EPTVListar.comunicador);

		container.delegate('.radioCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioCpfCnpj', container));
		Aux.setarFoco(container);
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	analisar: function () {
		var objeto = EPTVListar.obter(this);
		MasterPage.redireciona(EPTVListar.settings.urls.urlAnalisar + '/' + objeto.Id);
	},

	comunicador: function () {
		var item = EPTVListar.obter(this);

		if (!MasterPage.validarAjax(EPTVListar.settings.urls.urlValidarAcessoComunicador + '/' + item.Id, null, EPTVListar.container, false).EhValido) {
			return;
		}

		Modal.abrir(
			EPTVListar.settings.urls.urlComunicadorPTV,
			{ id: item.Id },
			function (container) {
				ComunicadorPTV.load(container, {
					callBackSalvar: EPTVListar.comunicador
				});
			},
			Modal.tamanhoModalMedia);
	}
}
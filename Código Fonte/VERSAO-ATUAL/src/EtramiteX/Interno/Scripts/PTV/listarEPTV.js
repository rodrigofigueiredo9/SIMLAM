/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />

EPTVListar = {
	settings: {
		urls: {
			urlComunicador: null,
			urlAnalisar: null,
			urlValidarAcessoComunicador: null,
			urlComunicadorPTV: null,
			urlAnalisarDesbloqueio: null,
			urlVisualizar: null,
			urlPDFEPTV: null,
			urlValidarAcessoAnalisarDesbloqueio: null,
		}
	},

	container: null,

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(EPTVListar.settings, options); }

		EPTVListar.container = container;
		container.listarAjax();
		container.delegate('.btnAnalisar', 'click', EPTVListar.analisar);
		container.delegate('.btnVisualizar', 'click', EPTVListar.visualizar);
		container.delegate('.btnPDF', 'click', EPTVListar.gerarPDF);
		container.delegate('.btnAnalisarDesbloqueio', 'click', EPTVListar.analisarDesbloqueio);
		container.delegate('.btnComunicador', 'click', EPTVListar.comunicador);
		container.delegate('.ddlTipoDocumento', 'change', EPTVListar.onChangeTipoDocumento);		

		container.delegate('.radioCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioCpfCnpj', container));
		Aux.setarFoco(container);
		if ($('.hdnAlerta').val() == "True")
			$('#Filtros_Situacao', container).val(2);
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	analisar: function () {
		var objeto = EPTVListar.obter(this);
		MasterPage.redireciona(EPTVListar.settings.urls.urlAnalisar + '/' + objeto.Id);
	},

	onChangeTipoDocumento: function () {
		if ($(this).val() > 0)
			$('.txtNumeroDocumento', EPTVListar.container).toggleClass('hide', false);
		else
			$('.txtNumeroDocumento', EPTVListar.container).toggleClass('hide', true);
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
	},

	analisarDesbloqueio: function () {
		var item = EPTVListar.obter(this);

		if (!MasterPage.validarAjax(EPTVListar.settings.urls.urlValidarAcessoAnalisarDesbloqueio + '/' + item.Id, null, EPTVListar.container, false).EhValido) {
			return;
		}

		Modal.abrir(
			EPTVListar.settings.urls.urlAnalisarDesbloqueio,
			{ id: item.Id },
			function (container) {
				ComunicadorPTV.load(container, {
					callBackSalvar: EPTVListar.analisarDesbloqueio
				});
			},
			Modal.tamanhoModalMedia);
	},

	visualizar: function () {
		var objeto = EPTVListar.obter(this);
		MasterPage.redireciona(EPTVListar.settings.urls.urlVisualizar + '/' + objeto.Id);
	},
	
	gerarPDF: function () {
		var item = EPTVListar.obter(this);
		MasterPage.redireciona(EPTVListar.settings.urls.urlPDFEPTV + '/' + item.Id);
	}
}
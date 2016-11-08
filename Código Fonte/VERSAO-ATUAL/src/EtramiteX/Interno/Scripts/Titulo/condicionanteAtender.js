/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

CondicionanteAtender = {
	settings: {
		urls: {
			atender: ''
		},
		onSalvar: null
	},
	container: null,

	load: function (container, options) {
		CondicionanteAtender.container = container;
		if (options) {
			$.extend(CondicionanteAtender.settings, options);
		}
		Modal.defaultButtons(CondicionanteAtender.container, CondicionanteAtender.oSalvarClick, 'Atender');
	},

	oSalvarClick: function () {
		var TituloCondicionante = { 
			condicionanteId: parseInt($('.hdnCondicionanteId', CondicionanteAtender.container).val()),
			periodicidadeId: parseInt($('.hdnCondicionantePeriodicidadeId', CondicionanteAtender.container).val())
		};
		if (isNaN(TituloCondicionante.condicionanteId)) condicionante.condicionanteId = 0;

		Mensagem.limpar(MasterPage.getContent(CondicionanteAtender.container));
		MasterPage.carregando(true);

		$.ajax({
			url: CondicionanteAtender.settings.urls.atender,
			data: JSON.stringify(TituloCondicionante),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CondicionanteAtender.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(CondicionanteAtender.container), response.Msg);
				}

				if (response.EhValido) {
					var retorno = CondicionanteAtender.settings.onSalvar(response.Condicionante, response.Periodicidade);
					if (retorno !== true) {
						Mensagem.gerar(MasterPage.getContent(CondicionanteAtender.container), response.Msg);
					} else {
						Modal.fechar(CondicionanteAtender.container);
					}
				}
			}
		});
		MasterPage.carregando(false);
	}
}
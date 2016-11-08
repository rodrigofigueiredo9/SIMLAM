/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

CondicionanteProrrogar = {
	settings: {
		urls: {
			prorrogar: ''
		},
		onSalvar: null
	},
	container: null,

	load: function (container, options) {
		CondicionanteProrrogar.container = container;
		if (options) {
			$.extend(CondicionanteProrrogar.settings, options);
		}
		Modal.defaultButtons(CondicionanteProrrogar.container, CondicionanteProrrogar.oSalvarClick, 'Salvar');
	},

	oSalvarClick: function () {
		var CondicionanteSituacaoProrrogarVM = {
			Dias: parseInt($('.txtDias', CondicionanteProrrogar.container).val()),
			CondicionanteId: parseInt($('.hdnCondicionanteId', CondicionanteProrrogar.container).val()),
			PeriodicidadeId: parseInt($('.hdnPeriodicidadeId', CondicionanteProrrogar.container).val())
		};

		Mensagem.limpar(MasterPage.getContent(CondicionanteProrrogar.container));
		MasterPage.carregando(true);

		$.ajax({
			url: CondicionanteProrrogar.settings.urls.prorrogar,
			data: JSON.stringify(CondicionanteSituacaoProrrogarVM),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CondicionanteProrrogar.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(CondicionanteProrrogar.container), response.Msg);
				}

				if (response.EhValido) {
					var retorno = CondicionanteProrrogar.settings.onSalvar(response.Condicionante, response.Periodicidade);
					if (retorno !== true) {
						Mensagem.gerar(MasterPage.getContent(CondicionanteProrrogar.container), response.Msg);
					} else {
						Modal.fechar(CondicionanteProrrogar.container);
					}
				}
			}
		});
		MasterPage.carregando(false);
	}
}
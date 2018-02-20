/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

Notificacao = {
	settings: {
		urls: {
			salvar: '',
			editar: ''
		},
		salvarCallBack: null,
		mensagens: {},
		idsTela: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Notificacao.settings, options); }
		Notificacao.container = MasterPage.getContent(container);
		Notificacao.container.delegate('.btnSalvar', 'click', Notificacao.salvar);
		Mascara.load();
		Notificacao.alterarVisibilidadeData();
	},

	obter: function () {
		var container = Notificacao.container;

		var obj = {
			Id: $('.hdnNotificacaoId', container).val(),
			FiscalizacaoId: $('.hdnFiscalizacaoId', container).val(),
			FormaIUF: $('.rdbFormaIUF:checked', container).val(),
			FormaJIAPI: $('.rdbFormaJIAPI:checked', container).val(),
			FormaCORE: $('.rdbFormaCORE:checked', container).val(),
			DataIUF: { DataTexto: $('.txtDataIUF', container).val() },
			DataJIAPI: { DataTexto: $('.txtDataJIAPI', container).val() },
			DataCORE: { DataTexto: $('.txtDataCORE', container).val() },
		};

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: Notificacao.settings.urls.salvar,
			data: JSON.stringify(Notificacao.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Notificacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Notificacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	alterarVisibilidadeData: function () {
		var obj = Notificacao.obter();
		if (obj.FormaJIAPI > 0)
			$('.jiapi').css('visibility', '');
		if (obj.FormaCORE > 0)
			$('.core').css('visibility', '');
	}
}
/// <reference path="Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../mensagem.js" />

ChaveReenviar = {
	settings: {
		urls: {
			reenviar: '',
			atualizarDados:''
		}
	},
	container: null,

	load: function (content, options) {
		ChaveReenviar.container = MasterPage.getContent(content);

		if (options) {
			$.extend(ChaveReenviar.settings, options);
		}

		content.delegate('.btnReenviar', 'click', ChaveReenviar.reenviar);
		content.delegate('.btnAtualizar', 'click', ChaveReenviar.atualizarDados);
	},

	atualizarDados: function () {
		Aux.carregando(ChaveReenviar.content, true);
		var params = { cpfCnpj: $('.txtCpfCnpj:visible', ChaveReenviar.container).val(), email: $('.txtEmail', ChaveReenviar.container).val() };

		$.ajax({url: ChaveReenviar.settings.urls.atualizarDados, data: JSON.stringify(params), type: 'POST', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				Aux.error(XMLHttpRequest, textStatus, errorThrown, ChaveReenviar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedireciona);
				}
				else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ChaveReenviar.container, response.Msg);
				}
			}
		});
		Aux.carregando(ChaveReenviar.content, false);
	},

	reenviar: function () {
		Aux.carregando(ChaveReenviar.content, true);
		var params = { cpfCnpj: $('.txtCpfCnpj:visible', ChaveReenviar.container).val(), email: $('.txtEmail', ChaveReenviar.container).val() };

		$.ajax({ url: ChaveReenviar.settings.urls.reenviar, data: JSON.stringify(params), type: 'POST', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				Aux.error(XMLHttpRequest, textStatus, errorThrown, ChaveReenviar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedireciona);
				}
				else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ChaveReenviar.container, response.Msg);
				}
			}
		});
		Aux.carregando(ChaveReenviar.content, false);
	}
}
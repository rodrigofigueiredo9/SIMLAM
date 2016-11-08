/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

NotificacaoEmbargo = {
	container: null,
	settings: {
		urls: { obterDadosNotificacao: '' }
	},

	load: function (especificidadeRef) {

		NotificacaoEmbargo.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
		DestinatarioEspecificidade.load(especificidadeRef);
	},

	obterNotificacaoEmbargo: function (protocolo) {

		if (protocolo == null) {
			return;
		}

		$.ajax({ url: NotificacaoEmbargo.settings.urls.obterDadosNotificacao,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(NotificacaoEmbargo.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				$('.ddlAtividadesEmbargo', NotificacaoEmbargo.container).ddlLoad(response.Atividades);
			}
		});
	},
	obterObjeto: function () {
		return {
			Destinatarios: DestinatarioEspecificidade.obter(),
			AtividadeEmbargo: NotificacaoEmbargo.container.find('.ddlAtividadesEmbargo').val()
		};
	}
};

Titulo.settings.especificidadeLoadCallback = NotificacaoEmbargo.load;
Titulo.settings.obterEspecificidadeObjetoFunc = NotificacaoEmbargo.obterObjeto;
Titulo.addCallbackProtocolo(NotificacaoEmbargo.obterNotificacaoEmbargo);
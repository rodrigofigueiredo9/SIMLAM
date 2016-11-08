/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />

NotificacaoIndeferimento = {
	container: null,
	urlObterProtocolo: '',

	load: function (especificidadeRef) {
		NotificacaoIndeferimento.container = especificidadeRef;
		AtividadeEspecificidade.load(NotificacaoIndeferimento.container);
	},

	obterDestinatario: function (protocolo) {
		if (protocolo == null) {
			$('.txtDestinatario', NotificacaoIndeferimento.container).val('');
			return;
		}

		//Obter Destinatario
		$.ajax({ url: NotificacaoIndeferimento.urlObterProtocolo,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(NotificacaoIndeferimento.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				$('.txtDestinatario', NotificacaoIndeferimento.container).val(response.Objeto.Interessado.NomeRazaoSocial);
			}
		});
	},

	obterObjeto: function () {
		return null;
	}
};

Titulo.settings.especificidadeLoadCallback = NotificacaoIndeferimento.load;
Titulo.addCallbackProtocolo(NotificacaoIndeferimento.obterDestinatario);
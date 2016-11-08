/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />

OficioNotificacao = {
	container: null,
	urlObterDadosOficioNotificacao: '',

	load: function (especificidadeRef) {
		OficioNotificacao.container = especificidadeRef;
		AtividadeEspecificidade.load(OficioNotificacao.container);
	},

	obterDadosOficioNotificacao: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', OficioNotificacao.container).ddlClear();
			return;
		}

		$.ajax({ url: OficioNotificacao.urlObterDadosOficioNotificacao,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(OficioNotificacao.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios.length > 0) {
					$('.ddlDestinatarios', OficioNotificacao.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	obterObjeto: function () {
		return {
			Destinatario: $('.ddlDestinatarios', OficioNotificacao.container).val()
		};
	}
};

Titulo.settings.especificidadeLoadCallback = OficioNotificacao.load;
Titulo.settings.obterEspecificidadeObjetoFunc = OficioNotificacao.obterObjeto;
Titulo.addCallbackProtocolo(OficioNotificacao.obterDadosOficioNotificacao);
/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

LicencaPrevia = {
	container: null,
	settings: {
		urls: {
			obterDadosLicencaPrevia: ''
		},
		modelos: {}
	},

	load: function (especificidadeRef) {
		LicencaPrevia.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
		TituloCondicionante.load($('.condicionantesContainer', LicencaPrevia.container));
	},

	obterLicencaPrevia: function (protocolo) {

		if (protocolo == null) {
			$('.ddlDestinatarios', LicencaPrevia.container).ddlClear();
			return;
		}

		$.ajax({ url: LicencaPrevia.settings.urls.obterDadosLicencaPrevia,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LicencaPrevia.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios.length > 0) {
					$('.ddlDestinatarios', LicencaPrevia.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},
	obterObjeto: function () {
		return {
			Destinatario: LicencaPrevia.container.find('.ddlDestinatarios').val(),
			BarragemId: AtividadeEspecificidade.Barragem.gerarObjeto().barragemId
		};
	}
};

Titulo.settings.especificidadeLoadCallback = LicencaPrevia.load;
Titulo.settings.obterEspecificidadeObjetoFunc = LicencaPrevia.obterObjeto;
Titulo.addCallbackProtocolo(LicencaPrevia.obterLicencaPrevia);
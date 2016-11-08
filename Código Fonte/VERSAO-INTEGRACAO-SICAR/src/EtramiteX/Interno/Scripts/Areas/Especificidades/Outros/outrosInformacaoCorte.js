/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

OutrosInformacaoCorte = {
	container: null,
	settings: {
		urls: {
			obterDadosOutrosInformacaoCorte: ''
		},
	},

	load: function (especificidadeRef) {
		OutrosInformacaoCorte.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
		TituloCondicionante.load($('.condicionantesContainer', OutrosInformacaoCorte.container));
	},

	obterOutrosInformacaoCorte: function (protocolo) {

		if (protocolo == null) {
			$('.ddlDestinatarios', OutrosInformacaoCorte.container).ddlClear();
			$('.ddlInformacaoCortes', OutrosInformacaoCorte.container).ddlClear();
			return;
		}

		$.ajax({ url: OutrosInformacaoCorte.settings.urls.obterDadosOutrosInformacaoCorte,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
			
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(OutrosInformacaoCorte.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
			
				if (response.Destinatarios.length > 0) {
					$('.ddlDestinatarios', OutrosInformacaoCorte.container).ddlLoad(response.Destinatarios);
				}

				if (response.InformacaoCortes.length > 0) {
					$('.ddlInformacaoCortes', OutrosInformacaoCorte.container).ddlLoad(response.InformacaoCortes);
				}
			}
		});
	},
	obterObjeto: function () {
		return {
			Destinatario: OutrosInformacaoCorte.container.find('.ddlDestinatarios').val(),			
			InformacaoCorte: OutrosInformacaoCorte.container.find('.ddlInformacaoCortes').val()
		};
	}
};

Titulo.settings.especificidadeLoadCallback = OutrosInformacaoCorte.load;
Titulo.settings.obterEspecificidadeObjetoFunc = OutrosInformacaoCorte.obterObjeto;
Titulo.addCallbackProtocolo(OutrosInformacaoCorte.obterOutrosInformacaoCorte);
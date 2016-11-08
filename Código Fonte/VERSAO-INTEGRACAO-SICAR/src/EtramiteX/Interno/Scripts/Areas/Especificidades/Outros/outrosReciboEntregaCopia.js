/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

OutrosReciboEntregaCopia = {
	container: null,
	settings: {
		urls: { obterDadosOutros: '' }
	},

	load: function (especificidadeRef) {

		OutrosReciboEntregaCopia.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
	},

	obterOutrosReciboEntregaCopia: function (protocolo) {

		if (protocolo == null) {
			$('.ddlDestinatarios', OutrosReciboEntregaCopia.container).ddlClear();
			return;
		}
		
		$.ajax({ url: OutrosReciboEntregaCopia.settings.urls.obterDadosOutros,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(OutrosReciboEntregaCopia.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios.length > 0) {
					$('.ddlDestinatarios', OutrosReciboEntregaCopia.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},
	obterObjeto: function () {
		return {
			Destinatario: OutrosReciboEntregaCopia.container.find('.ddlDestinatarios').val()
		};
	}
};

Titulo.settings.especificidadeLoadCallback = OutrosReciboEntregaCopia.load;
Titulo.settings.obterEspecificidadeObjetoFunc = OutrosReciboEntregaCopia.obterObjeto;
Titulo.addCallbackProtocolo(OutrosReciboEntregaCopia.obterOutrosReciboEntregaCopia);
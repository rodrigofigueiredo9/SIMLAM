/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

CertificadoRegistro = {
	container: null,
	urlObterDadosCertificadoRegistro: '',

	load: function (especificidadeRef) {
		CertificadoRegistro.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
	},

	obterDadosCertificadoRegistro: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', CertificadoRegistro.container).ddlClear();
			return;
		}

		$.ajax({ url: CertificadoRegistro.urlObterDadosCertificadoRegistro,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CertificadoRegistro.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios.length > 0) {
					$('.ddlDestinatarios', CertificadoRegistro.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	obterObjeto: function () {
		return {
			Destinatario: $('.ddlDestinatarios', CertificadoRegistro.container).val(),
			Classificacao: $('.txtClassificacao', CertificadoRegistro.container).val(),
			Registro: $('.txtRegistro', CertificadoRegistro.container).val() 
		};
	}
};

Titulo.settings.especificidadeLoadCallback = CertificadoRegistro.load;
Titulo.settings.obterEspecificidadeObjetoFunc = CertificadoRegistro.obterObjeto;
Titulo.addCallbackProtocolo(CertificadoRegistro.obterDadosCertificadoRegistro);
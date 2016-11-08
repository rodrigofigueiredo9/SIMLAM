/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

CertificadoRegistroAtividadeFlorestal = {
	container: null,
	settings: {
		urls: {
			obterDadosCertificadoRegistroAtividadeFlorestal: ''
		},
		modelos: {}
	},

	load: function (especificidadeRef) {
		CertificadoRegistroAtividadeFlorestal.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
		CertificadoRegistroAtividadeFlorestal.container.delegate('.ddlVias', 'change', CertificadoRegistroAtividadeFlorestal.changeDdlVias);
	},

	obterDadosCertificadoRegistroAtividadeFlorestal: function (protocolo) {

		if (protocolo == null) {
			$('.ddlDestinatarios', CertificadoRegistroAtividadeFlorestal.container).ddlClear();
			return;
		}

		$.ajax({ url: CertificadoRegistroAtividadeFlorestal.settings.urls.obterDadosCertificadoRegistroAtividadeFlorestal,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CertificadoRegistroAtividadeFlorestal.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios.length > 0) {
					$('.ddlDestinatarios', CertificadoRegistroAtividadeFlorestal.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	changeDdlVias: function () {
		var id = $('.ddlVias', CertificadoRegistroAtividadeFlorestal.container).val();
		$('.divViasOutra', CertificadoRegistroAtividadeFlorestal.container).toggleClass('hide', id != 6);
	},

	obterObjeto: function () {

		var vias = $('.ddlVias', CertificadoRegistroAtividadeFlorestal.container).val();

		if (vias == 6) {
			vias = CertificadoRegistroAtividadeFlorestal.container.find('.txtViasOutra').val();
		}

		return {

			Destinatario: CertificadoRegistroAtividadeFlorestal.container.find('.ddlDestinatarios').val(),
			Vias: vias,
			AnoExercicio: CertificadoRegistroAtividadeFlorestal.container.find('.txtAnoExercicio').val(),
			Objetivo: CertificadoRegistroAtividadeFlorestal.container.find('.txtObjetivo').val(),
			Constatacao: CertificadoRegistroAtividadeFlorestal.container.find('.txtConstatacao').val(),
			DataVistoria: { DataTexto: CertificadoRegistroAtividadeFlorestal.container.find('.txtDataVistoria').val() }
		};
	}
};



Titulo.settings.especificidadeLoadCallback = CertificadoRegistroAtividadeFlorestal.load;
Titulo.settings.obterEspecificidadeObjetoFunc = CertificadoRegistroAtividadeFlorestal.obterObjeto;
Titulo.addCallbackProtocolo(CertificadoRegistroAtividadeFlorestal.obterDadosCertificadoRegistroAtividadeFlorestal);

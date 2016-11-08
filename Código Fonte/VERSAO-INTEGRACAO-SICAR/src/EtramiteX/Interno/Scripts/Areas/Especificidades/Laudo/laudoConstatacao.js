/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

LaudoConstatacao = {
	container: null,
	urlObterDadosLaudoConstatacao: '',

	load: function (especificidadeRef) {
		LaudoConstatacao.container = especificidadeRef;
		AtividadeEspecificidade.load(LaudoConstatacao.container);
		LaudoConstatacao.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });
	},

	obterDadosLaudoConstatacao: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', LaudoConstatacao.container).ddlClear();
			return;
		}

		$.ajax({
			url: LaudoConstatacao.urlObterDadosLaudoConstatacao,
			data: JSON.stringify({ id: protocolo.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LaudoConstatacao.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', LaudoConstatacao.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	obterObjeto: function () {
		return {
			Objetivo: LaudoConstatacao.container.find('.txtObjetivo').val(),
			Constatacao: LaudoConstatacao.container.find('.txtConstatacao').val(),
			Anexos: LaudoConstatacao.container.find('.fsArquivos').arquivo('obterObjeto'),
			Destinatario: LaudoConstatacao.container.find('.ddlDestinatarios').val(),
			DataVistoria: {
				DataTexto: LaudoConstatacao.container.find('.txtDataVistoria').val()
			}
		};
	},

	obterAnexosObjeto: function () {
		var anexos = new Array();
		anexos = LaudoConstatacao.container.find('.fsArquivos').arquivo('obterObjeto');
		return anexos;
	}
};

Titulo.settings.especificidadeLoadCallback = LaudoConstatacao.load;
Titulo.addCallbackProtocolo(LaudoConstatacao.obterDadosLaudoConstatacao);
Titulo.settings.obterEspecificidadeObjetoFunc = LaudoConstatacao.obterObjeto;
Titulo.settings.obterAnexosCallback = LaudoConstatacao.obterAnexosObjeto;
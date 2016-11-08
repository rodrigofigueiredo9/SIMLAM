/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

EscrituraPublicaDoacao = {
	container: null,
	urlObterDadosEscrituraPublicaDoacao: '',

	load: function (especificidadeRef) {
		EscrituraPublicaDoacao.container = especificidadeRef;
		AtividadeEspecificidade.load(EscrituraPublicaDoacao.container);
		EscrituraPublicaDoacao.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });
	},

	obterDadosEscrituraPublicaDoacao: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', EscrituraPublicaDoacao.container).ddlClear();
			return;
		}

		$.ajax({
			url: EscrituraPublicaDoacao.urlObterDadosEscrituraPublicaDoacao,
			data: JSON.stringify({ id: protocolo.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(EscrituraPublicaDoacao.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', EscrituraPublicaDoacao.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	obterObjeto: function () {
		return {
			Livro: EscrituraPublicaDoacao.container.find('.txtLivro').val(),
			Folhas: EscrituraPublicaDoacao.container.find('.txtFolhas').val(),
			Destinatario: EscrituraPublicaDoacao.container.find('.ddlDestinatarios').val(),
			Anexos: EscrituraPublicaDoacao.container.find('.fsArquivos').arquivo('obterObjeto')
		};
	},

	obterAnexosObjeto: function () {
		var anexos = new Array();
		anexos = EscrituraPublicaDoacao.container.find('.fsArquivos').arquivo('obterObjeto');
		return anexos;
	}
};

Titulo.settings.especificidadeLoadCallback = EscrituraPublicaDoacao.load;
Titulo.addCallbackProtocolo(EscrituraPublicaDoacao.obterDadosEscrituraPublicaDoacao);
Titulo.settings.obterEspecificidadeObjetoFunc = EscrituraPublicaDoacao.obterObjeto;
Titulo.settings.obterAnexosCallback = EscrituraPublicaDoacao.obterAnexosObjeto;
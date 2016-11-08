/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

EscrituraPublicaCompraVenda = {
	container: null,
	urlObterDadosEscrituraPublicaCompraVenda: '',

	load: function (especificidadeRef) {
		EscrituraPublicaCompraVenda.container = especificidadeRef;
		AtividadeEspecificidade.load(EscrituraPublicaCompraVenda.container);
		EscrituraPublicaCompraVenda.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });
	},

	obterDadosEscrituraPublicaCompraVenda: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', EscrituraPublicaCompraVenda.container).ddlClear();
			return;
		}

		$.ajax({
			url: EscrituraPublicaCompraVenda.urlObterDadosEscrituraPublicaCompraVenda,
			data: JSON.stringify({ id: protocolo.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(EscrituraPublicaCompraVenda.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', EscrituraPublicaCompraVenda.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	obterObjeto: function () {
		return {
			Livro: EscrituraPublicaCompraVenda.container.find('.txtLivro').val(),
			Folhas: EscrituraPublicaCompraVenda.container.find('.txtFolhas').val(),
			Destinatario: EscrituraPublicaCompraVenda.container.find('.ddlDestinatarios').val(),
			Anexos: EscrituraPublicaCompraVenda.container.find('.fsArquivos').arquivo('obterObjeto')
		};
	},

	obterAnexosObjeto: function () {
		var anexos = new Array();
		anexos = EscrituraPublicaCompraVenda.container.find('.fsArquivos').arquivo('obterObjeto');
		return anexos;
	}
};

Titulo.settings.especificidadeLoadCallback = EscrituraPublicaCompraVenda.load;
Titulo.addCallbackProtocolo(EscrituraPublicaCompraVenda.obterDadosEscrituraPublicaCompraVenda);
Titulo.settings.obterEspecificidadeObjetoFunc = EscrituraPublicaCompraVenda.obterObjeto;
Titulo.settings.obterAnexosCallback = EscrituraPublicaCompraVenda.obterAnexosObjeto;
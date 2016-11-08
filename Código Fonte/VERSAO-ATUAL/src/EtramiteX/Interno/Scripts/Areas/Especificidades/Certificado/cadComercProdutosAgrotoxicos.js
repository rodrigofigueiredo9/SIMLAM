/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

CadComercProdutosAgrotoxicos = {
	container: null,
	urlObterDadosCadComercProdutosAgrotoxicos: '',

	load: function (especificidadeRef) {
		CadComercProdutosAgrotoxicos.container = especificidadeRef;
		AtividadeEspecificidade.load(CadComercProdutosAgrotoxicos.container);
		CadComercProdutosAgrotoxicos.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });
	},

	obterDadosCadComercProdutosAgrotoxicos: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', CadComercProdutosAgrotoxicos.container).ddlClear();
			return;
		}

		$.ajax({
			url: CadComercProdutosAgrotoxicos.urlObterDadosCadComercProdutosAgrotoxicos,
			data: JSON.stringify({ id: protocolo.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CadComercProdutosAgrotoxicos.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', CadComercProdutosAgrotoxicos.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	obterObjeto: function () {
		return {
			Destinatario: CadComercProdutosAgrotoxicos.container.find('.ddlDestinatarios').val(),
			Anexos: CadComercProdutosAgrotoxicos.container.find('.fsArquivos').arquivo('obterObjeto')
		};
	}
};

Titulo.settings.especificidadeLoadCallback = CadComercProdutosAgrotoxicos.load;
Titulo.addCallbackProtocolo(CadComercProdutosAgrotoxicos.obterDadosCadComercProdutosAgrotoxicos);
Titulo.settings.obterEspecificidadeObjetoFunc = CadComercProdutosAgrotoxicos.obterObjeto;
Titulo.settings.obterAnexosCallback = CadComercProdutosAgrotoxicos.obterAnexosObjeto;
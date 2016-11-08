/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

CadAplicProdutosAgrotoxicos = {
	container: null,
	urlObterDadosCadAplicProdutosAgrotoxicos: '',

	load: function (especificidadeRef) {
		CadAplicProdutosAgrotoxicos.container = especificidadeRef;
		AtividadeEspecificidade.load(CadAplicProdutosAgrotoxicos.container);
		CadAplicProdutosAgrotoxicos.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });
	},

	obterDadosCadAplicProdutosAgrotoxicos: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', CadAplicProdutosAgrotoxicos.container).ddlClear();
			return;
		}

		$.ajax({
			url: CadAplicProdutosAgrotoxicos.urlObterDadosCadAplicProdutosAgrotoxicos,
			data: JSON.stringify({ id: protocolo.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CadAplicProdutosAgrotoxicos.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', CadAplicProdutosAgrotoxicos.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	obterObjeto: function () {
		return {
			Destinatario: CadAplicProdutosAgrotoxicos.container.find('.ddlDestinatarios').val(),
			Anexos: CadAplicProdutosAgrotoxicos.container.find('.fsArquivos').arquivo('obterObjeto')
		};
	}
};

Titulo.settings.especificidadeLoadCallback = CadAplicProdutosAgrotoxicos.load;
Titulo.addCallbackProtocolo(CadAplicProdutosAgrotoxicos.obterDadosCadAplicProdutosAgrotoxicos);
Titulo.settings.obterEspecificidadeObjetoFunc = CadAplicProdutosAgrotoxicos.obterObjeto;
Titulo.settings.obterAnexosCallback = CadAplicProdutosAgrotoxicos.obterAnexosObjeto;
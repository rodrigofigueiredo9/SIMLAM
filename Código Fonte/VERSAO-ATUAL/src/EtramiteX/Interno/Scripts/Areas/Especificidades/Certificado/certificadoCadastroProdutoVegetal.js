/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />

CertificadoCadastroProdutoVegetal = {
	container: null,

	settings: {
		urls: {
			urlObterDadosCertificado: ''
		},
		Mensagens: null
	},

	load: function (especificidadeRef) {
		CertificadoCadastroProdutoVegetal.container = especificidadeRef;
		AtividadeEspecificidade.load(CertificadoCadastroProdutoVegetal.container);
	},

	obterDadosCertificadoCadastroProdutoVegetal: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', CertificadoCadastroProdutoVegetal.container).ddlClear();
			return;
		}

		$.ajax({ url: CertificadoCadastroProdutoVegetal.settings.urls.urlObterDadosCertificado,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CertificadoCadastroProdutoVegetal.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios.length > 0) {
					$('.ddlDestinatarios', CertificadoCadastroProdutoVegetal.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	obterObjeto: function () {
		return {
			Destinatario: $('.ddlDestinatarios', CertificadoCadastroProdutoVegetal.container).val(),
			Nome: $('.txtNome', CertificadoCadastroProdutoVegetal.container).val().trim(),
			Fabricante: $('.txtFabricante', CertificadoCadastroProdutoVegetal.container).val().trim(),
			ClasseToxicologica: $('.txtClasseToxicologica', CertificadoCadastroProdutoVegetal.container).val().trim(),
			Classe: $('.txtClasse', CertificadoCadastroProdutoVegetal.container).val().trim(),
			Ingrediente: $('.txtIngrediente', CertificadoCadastroProdutoVegetal.container).val().trim(),
			Cultura: $('.txtCultura', CertificadoCadastroProdutoVegetal.container).val().trim(),
			Classificacao: $('.txtClassificacao', CertificadoCadastroProdutoVegetal.container).val().trim()
		};
	}
};

Titulo.settings.especificidadeLoadCallback = CertificadoCadastroProdutoVegetal.load;
Titulo.settings.obterEspecificidadeObjetoFunc = CertificadoCadastroProdutoVegetal.obterObjeto;
Titulo.addCallbackProtocolo(CertificadoCadastroProdutoVegetal.obterDadosCertificadoCadastroProdutoVegetal);
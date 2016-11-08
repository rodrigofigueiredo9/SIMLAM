/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

CertificadoCadastroProdutoAgrotoxico = {
	settings: {
		urls: {
			obterDadosCertificadoCadastroProdutoAgrotoxico: null,
			obterDadosAgrotoxico: null
		}
	},

	container: null,

	load: function (especificidadeRef) {
		CertificadoCadastroProdutoAgrotoxico.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
		CertificadoCadastroProdutoAgrotoxico.container.delegate('.btnAtualizarAgrotoxico', 'click', CertificadoCadastroProdutoAgrotoxico.obterDadosAgrotoxico);
	},

	obterDadosCertificadoCadastroProdutoAgrotoxico: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', CertificadoCadastroProdutoAgrotoxico.container).ddlClear();
			return;
		}

		$.ajax({
			url: CertificadoCadastroProdutoAgrotoxico.settings.urls.obterDadosCertificadoCadastroProdutoAgrotoxico,
			data: JSON.stringify({ id: protocolo.Id }),
			cache: false,
			async: true,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CertificadoCadastroProdutoAgrotoxico.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', CertificadoCadastroProdutoAgrotoxico.container).ddlLoad(response.Destinatarios);
				}

				if (response.Agrotoxico) {
					$('.hdnAgrotoxicoId', CertificadoCadastroProdutoAgrotoxico.container).val(response.Agrotoxico.Id);
					$('.hdnAgrotoxicoTid', CertificadoCadastroProdutoAgrotoxico.container).val(response.Agrotoxico.Tid);
					$('.txtAgrotoxicoNome', CertificadoCadastroProdutoAgrotoxico.container).val(response.Agrotoxico.NomeComercial);
				}
			}
		});
	},

	mostrarAtualizarAgrotoxico: function (erros) {
		var mostrar = $.grep(erros, function (item, i) {
			return item.Campo == 'Certificado_AgrotoxicoNome';
		});
		$('.btnAtualizarAgrotoxico', CertificadoCadastroProdutoAgrotoxico.container).toggleClass('hide', (mostrar.length < 1));
	},

	obterDadosAgrotoxico: function (protocolo) {
		var protocolo = AtividadeEspecificidade.obterProtocoloDdlSelecionado();

		$.ajax({
			url: CertificadoCadastroProdutoAgrotoxico.settings.urls.obterDadosAgrotoxico,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CertificadoCadastroProdutoAgrotoxico.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Agrotoxico) {
					$('.hdnAgrotoxicoId', CertificadoCadastroProdutoAgrotoxico.container).val(response.Agrotoxico.Id);
					$('.hdnAgrotoxicoTid', CertificadoCadastroProdutoAgrotoxico.container).val(response.Agrotoxico.Tid);
					$('.txtAgrotoxicoNome', CertificadoCadastroProdutoAgrotoxico.container).val(response.Agrotoxico.NomeComercial);

					if (response.Agrotoxico.Id > 0) {
						$('.btnAtualizarAgrotoxico', CertificadoCadastroProdutoAgrotoxico.container).addClass('hide');
					}
				}
			}
		});
	},

	obter: function () {
		var container = CertificadoCadastroProdutoAgrotoxico.container;

		return {
			DestinatarioId: $('.ddlDestinatarios :selected', container).val(),
			AgrotoxicoId: $('.hdnAgrotoxicoId', container).val(),
			AgrotoxicoTid: $('.hdnAgrotoxicoTid', container).val()
		};
	}
};

Titulo.settings.especificidadeLoadCallback = CertificadoCadastroProdutoAgrotoxico.load;
Titulo.addCallbackProtocolo(CertificadoCadastroProdutoAgrotoxico.obterDadosCertificadoCadastroProdutoAgrotoxico);
Titulo.settings.obterEspecificidadeObjetoFunc = CertificadoCadastroProdutoAgrotoxico.obter;
Titulo.settings.especificidadeErroSalvarCallback = CertificadoCadastroProdutoAgrotoxico.mostrarAtualizarAgrotoxico;
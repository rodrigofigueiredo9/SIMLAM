/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../mensagem.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ConverterDocumento = {
	settings: {
		urls: {
			converter: '',
			verificar: '',
			avancar: '',
			redireciona: ''
		}
	},
	containerDocumentoCarregado: false,
	pessoaModalInte: null,
	callBackEditarInteressado: null,
	container: null,
	load: function (container, options) {
		if (options) { $.extend(ConverterDocumento.settings, options); }
		ConverterDocumento.container = MasterPage.getContent(container);

		ConverterDocumento.container.delegate('.rdbProcessoPossuiSEP', 'click', ConverterDocumento.onClickSEP);
		ConverterDocumento.container.delegate('.btnLimpar', 'click', ConverterDocumento.onClickLimpar);
		ConverterDocumento.container.delegate('.btnVerificar', 'click', ConverterDocumento.onClickVerificar);

		ConverterDocumento.container.delegate('.btnConverter', 'click', ConverterDocumento.onClickConverter);
	},
	gerarObjeto: function () {
		return {
			DocumentoId: parseInt($('.hdnDocumentoId', ConverterDocumento.container).val()),
			NumeroDocumento: $('.txtNumero', ConverterDocumento.container).val().trim(),
			NumeroAutuacao: $('.txtNumeroAutuacao', ConverterDocumento.container).val().trim(),
			DataAutuacao: $('.txtDataAutuacao', ConverterDocumento.container).val(),
			PossuiSEP: parseInt($('.rdbProcessoPossuiSEP:checked', ConverterDocumento.container).val()),
			Processo: Processo.montarObjetoProcesso()
		};
	},
	onClickSEP: function () {
		if (parseInt($(this).val()) == 1) {
			$('.divAutuacao', ConverterDocumento.container).removeClass('hide');
		} else {
			$('.divAutuacao', ConverterDocumento.container).addClass('hide');
			$('.txtNumeroAutuacao', ConverterDocumento.container).val('');
			$('.txtDataAutuacao', ConverterDocumento.container).val('');
		}
	},
	onClickLimpar: function () {
		$('.hdnDocumentoId', ConverterDocumento.container).val('');
		$('.divDocumentoCovert', ConverterDocumento.container).empty();
		$('.spanConverter', ConverterDocumento.container).addClass('hide');
		$('.spanVerificar', ConverterDocumento.container).removeClass('hide');
		$('.spanLimpar', ConverterDocumento.container).addClass('hide');
		$('.txtNumero', ConverterDocumento.container).val('').removeAttr('disabled');
		Mensagem.limpar(ConverterDocumento.container);
	},
	onClickVerificar: function () {

		MasterPage.carregando(true);

		$.ajax({ url: ConverterDocumento.settings.urls.verificar,
			data: JSON.stringify({ strDocumentoNumero: $('.txtNumero', ConverterDocumento.container).val().trim() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ConverterDocumento.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					$('.hdnDocumentoId', ConverterDocumento.container).val(response.Id);
					$('.divDocumentoCovert', ConverterDocumento.container).empty().html(response.Html);
					Mascara.load('.divDocumentoCovert');
					$('.spanConverter', ConverterDocumento.container).removeClass('hide');
					$('.spanVerificar', ConverterDocumento.container).addClass('hide');
					$('.spanLimpar', ConverterDocumento.container).removeClass('hide');
					$('.txtNumero', ConverterDocumento.container).attr('disabled', 'disabled');
					Mensagem.limpar(ConverterDocumento.container);

					if (!ConverterDocumento.containerDocumentoCarregado) {
						ConverterDocumento.containerDocumentoCarregado = true;
						AtividadeSolicitadaAssociar.load($('.divDocumentoCovert', ConverterDocumento.container));
						Processo.load($('.divDocumentoCovert', ConverterDocumento.container));
					} else {
						Processo.configurarAssociarMultiplo();
					}
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ConverterDocumento.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},
	onClickConverter: function () {

		var arrayMsg = [];
		var convertDoc = ConverterDocumento.gerarObjeto();

		if (isNaN(convertDoc.PossuiSEP)) {
			arrayMsg.push(Processo.settings.Mensagens.PossuiNumeroSEPObrigatorio);
		}
		else if (convertDoc.PossuiSEP == 1) {

			if (convertDoc.NumeroAutuacao == '') {
				arrayMsg.push(Processo.settings.Mensagens.NumeroAutuacaoObrigatorio);
			}

			if (!new RegExp(/^\d{2}\/\d{2}\/\d{4}$/).test(convertDoc.DataAutuacao)) {
				arrayMsg.push(Processo.settings.Mensagens.DataAutuacaoObrigatoria);
			}
		}

		if (arrayMsg.length > 0) {
			Mensagem.gerar(ConverterDocumento.container, arrayMsg);
			return;
		}

		MasterPage.carregando(true);

		$.ajax({ url: ConverterDocumento.settings.urls.converter,
			data: JSON.stringify({ convertDoc: convertDoc }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ConverterDocumento.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					ConverterDocumento.settings.urls.redireciona = ConverterDocumento.settings.urls.avancar + '/' + $('.hdnDocumentoId', ConverterDocumento.container).val();
					ConverterDocumento.onClickLimpar();
					Mensagem.limpar(ConverterDocumento.container);
					Mensagem.gerar(ConverterDocumento.container, response.Msg);
					ContainerAcoes.load(ConverterDocumento.container, { limparContainer: false, botoes: new Array({ label: 'Editar Processo', callBack: ConverterDocumento.onClickAvancar }) });

					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ConverterDocumento.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},
	onClickAvancar: function () {
		MasterPage.redireciona(ConverterDocumento.settings.urls.redireciona);
	}
}
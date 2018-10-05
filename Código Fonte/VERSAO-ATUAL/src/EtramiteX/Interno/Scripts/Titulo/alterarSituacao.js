/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

TituloAlterarSituacao = {
	settings: {
		urls: {
			pdfTitulo: '',
			validarObterSituacao: '',
			salvar: '',
			redirecionar: '',
			integracaoSinaflor: null
		},
		gerouPdf: false
	},
	container: null,

	load: function (container, options) {
		TituloAlterarSituacao.container = container;
		if (options) {
			$.extend(TituloAlterarSituacao.settings, options);
		}

		container.delegate('.btnPdfTitulo', 'click', TituloAlterarSituacao.onAbrirPdfTitulo);
		container.delegate('.rdbOpcaoSituacao', 'change', TituloAlterarSituacao.onSituacaoChange);
	},

	onAbrirPdfTitulo: function () {
		TituloAlterarSituacao.settings.gerouPdf = true;
		MasterPage.redireciona(TituloAlterarSituacao.settings.urls.pdfTitulo + "?id=" + $('.hdnTituloId', TituloAlterarSituacao.container).val());
		MasterPage.carregando(false);
	},

	limparCampos: function (container) {
		container.find(':text:enabled').unmask().val('');
		Mascara.load(container);

		container.find('select').each(function () {
			$(this).find('option:first').attr('selected', 'selected');
		});
	},

	onSituacaoChange: function () {
		$('.btnSalvar', TituloAlterarSituacao.container).button({ disabled: false });
		$('.btnSalvar', TituloAlterarSituacao.container).unbind('click');
		$(".btnSalvar", TituloAlterarSituacao.container).click(TituloAlterarSituacao.onSalvar);

		$('.divCamposSituacao', TituloAlterarSituacao.container).addClass('hide');
		TituloAlterarSituacao.limparCampos($('.divCamposSituacao', TituloAlterarSituacao.container));
		var container = null;

		switch (parseInt($('.rdbOpcaoSituacao:checked', TituloAlterarSituacao.container).val())) {
			case 1:
				container = $('.divEmitirParaAssinatura', TituloAlterarSituacao.container)
				container.removeClass('hide');
				break;

			case 2:
				container = $('.divCancelarEmissao', TituloAlterarSituacao.container)
				container.removeClass('hide');
				break;

			case 3:
				container = $('.divAssinar', TituloAlterarSituacao.container)
				container.removeClass('hide');
				break;

			case 4:
				container = $('.divProrrogar', TituloAlterarSituacao.container)
				container.removeClass('hide');
				break;

			case 5:
				container = $('.divEncerrar', TituloAlterarSituacao.container)
				container.removeClass('hide');
				break;

			case 6:
				container = $('.divConcluir', TituloAlterarSituacao.container)
				container.removeClass('hide');
				break;
		}

		$.ajax({
			url: TituloAlterarSituacao.settings.urls.validarObterSituacao,
			data: { id: $('.hdnTituloId', TituloAlterarSituacao.container).val(), acao: $('.rdbOpcaoSituacao:checked', TituloAlterarSituacao.container).val() },
			cache: false, async: true,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, TituloAlterarSituacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(TituloAlterarSituacao.container), response.Msg);
				} else {
					$('.txtSituacaoNova', TituloAlterarSituacao.container).val(response.Situacao.Nome);
				}
			}
		});
	},

	onSalvar: function () {
		var objeto = {
			Id: $('.hdnTituloId', TituloAlterarSituacao.container).val(),
			Prazo: $('.txtPrazo', TituloAlterarSituacao.container).val(),
			DiasProrrogados: $('.txtDiasProrrogados', TituloAlterarSituacao.container).val(),
			MotivoEncerramentoId: $('.ddlMotivo', TituloAlterarSituacao.container).val(),
			DataEmissao: { DataTexto: $('.txtDataEmissao', TituloAlterarSituacao.container).val() },
			DataAssinatura: { DataTexto: $('.txtDataAssinatura', TituloAlterarSituacao.container).val() },
			DataEncerramento: { DataTexto: $('.txtDataEncerramento', TituloAlterarSituacao.container).val() }
		};
		var acao = $('.rdbOpcaoSituacao:checked', TituloAlterarSituacao.container).val();

		MasterPage.carregando(true);

		$.get(TituloAlterarSituacao.integracaoSinaflor, { tituloId: objeto.Id, codigoSicar: objeto.CodigoSicar },
			function (data, textStatus, XMLHttpRequest) {
				debugger;
				if (textStatus == "200") {
					$.ajax({
						url: TituloAlterarSituacao.settings.urls.salvar,
						data: JSON.stringify({ titulo: objeto, acao: acao, gerouPdf: TituloAlterarSituacao.settings.gerouPdf }),
						cache: false,
						async: false,
						type: 'POST',
						dataType: 'json',
						contentType: 'application/json; charset=utf-8',
						error: function (XMLHttpRequest, textStatus, erroThrown) {
							Aux.error(XMLHttpRequest, textStatus, erroThrown, TituloAlterarSituacao.container);
							MasterPage.carregando(false);
						},
						success: function (response, textStatus, XMLHttpRequest) {
							if (response.EhValido) {
								MasterPage.redireciona(TituloAlterarSituacao.settings.urls.redirecionar + '?Msg=' + response.Msg + '&acaoId=' + response.AcaoId);
							} else {
								if (response.Msg && response.Msg.length > 0) {
									Mensagem.gerar(MasterPage.getContent(TituloAlterarSituacao.container), response.Msg);
								}
							}
							MasterPage.carregando(false);
						}
					});
				} else {
					MasterPage.carregando(false);
				}
			}, "json");
	}
}
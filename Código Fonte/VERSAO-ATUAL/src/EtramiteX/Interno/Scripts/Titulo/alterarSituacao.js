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
				container = $('.divEmitirParaAssinatura', TituloAlterarSituacao.container);
				container.removeClass('hide');
				break;

			case 2:
				container = $('.divCancelarEmissao', TituloAlterarSituacao.container);
				container.removeClass('hide');
				break;

			case 3:
				container = $('.divAssinar', TituloAlterarSituacao.container);
				container.removeClass('hide');
				break;

			case 4:
				container = $('.divProrrogar', TituloAlterarSituacao.container);
				container.removeClass('hide');
				break;

			case 5:
				container = $('.divEncerrar', TituloAlterarSituacao.container);
				container.removeClass('hide');
				break;

			case 6:
				container = $('.divConcluir', TituloAlterarSituacao.container);
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
		var modelo = $('.hdnModeloId', TituloAlterarSituacao.container).val();
		var codigoSicar = $('.hdnCodigoSicar', TituloAlterarSituacao.container).val();
		
		MasterPage.carregando(true);

		if (modelo == 13) {
			var data = $('.txtDataEmissao', TituloAlterarSituacao.container).val();
			var dataEmissao = data.substring(6, data.length) + '-' + data.substring(3, data.length - 5) + '-' + data.substring(0, data.length - 8);

				$.ajax({
					type: "POST",
					url: TituloAlterarSituacao.settings.urls.integracaoSinaflor + '/titulo/' + objeto.Id + '/dataEmissao/' + dataEmissao +
						'/prazo/' + objeto.Prazo + (codigoSicar != '' ? '/Sicar/' + codigoSicar : ''),
					success: function (msg) {
						debugger;
						console.info(msg);
						TituloAlterarSituacao.alterarSituacao(objeto);
					},
					error: function (XMLHttpRequest, textStatus, errorThrown) {
						var data = JSON.parse(XMLHttpRequest.response);
						MasterPage.carregando(false);
						var msg = "";
						if (data.message.lengh > 1)
							msg = data.message[0].description[0];
						else
							msg = data.message;
						ExibirMensagemValidacao(msg);
					}
				});
		} else {
			TituloAlterarSituacao.alterarSituacao(objeto);
		}
	},

	alterarSituacao: function (objeto) {
		var acao = $('.rdbOpcaoSituacao:checked', TituloAlterarSituacao.container).val();
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
	}
}

function ExibirMensagemValidacao(erro) {
	var mensagem = '\
			<div class=\"mensagemSistema alerta ui-draggable\" style=\"position: relative;\">\
				<div class=\"textoMensagem \">\
					<a class=\"fecharMensagem\" title=\"Fechar Mensagem\">Fechar Mensagem</a>\
					<p> Mensagem do Sistema</p>\
					<ul>\
						<li>' + erro + '</li>\
					</ul>\
				</div>\
				<div class=\"redirecinamento block containerAcoes hide\">\
					<h5> O que deseja fazer agora ?</h5>\
					<p class=\"hide\">#DESCRICAO</p>\
					<div class=\"coluna100 margem0 divAcoesContainer\">\
						<p class=\"floatLeft margem0 append1\"><button title=\"[title]\" class=\"btnTemplateAcao hide ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only\" role=\"button\" aria-disabled=\"false\"><span class=\"ui-button-text\">[ACAO]</span></button></p>\
						<div class=\"containerBotoes\"></div>\
					</div>\
				</div>\
			</div>';
	$('.mensagemSistemaHolder')[0].innerHTML = mensagem;
}
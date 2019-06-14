/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

TituloAlterarSituacao = {
	settings: {
		urls: {
			salvar: '',
			redirecionar: ''
		}
	},
	container: null,

	load: function (container, options) {
		TituloAlterarSituacao.container = container;
		if (options) {
			$.extend(TituloAlterarSituacao.settings, options);
		}

		container.delegate('.ddlNovaSituacao', 'change', TituloAlterarSituacao.changeSituacao);
		container.delegate('.btnSalvar', 'click', TituloAlterarSituacao.salvar);
	},

	limparCampos: function (container) {
		container.find(':text:enabled').unmask().val('');
		Mascara.load(container);

		container.find('select').each(function () {
			$(this).find('option:first').attr('selected', 'selected');
		});
	},

	changeSituacao: function () {
		$('.divEncerramento, .divSuspenso', TituloAlterarSituacao.container).addClass('hide');
		$('.divProrrogar', TituloAlterarSituacao.container).addClass('hide');
		$('.divSuspenso', TituloAlterarSituacao.container).addClass('hide');
		$('.ddlMotivoEncerramento', TituloAlterarSituacao.container).ddlFirst();
		$('.txtMotivo', TituloAlterarSituacao.container).val('');

		if ($('.ddlNovaSituacao', TituloAlterarSituacao.container).val() == 9) {
			$('.divSuspenso', TituloAlterarSituacao.container).removeClass('hide');
		}
		else if ($('.ddlNovaSituacao', TituloAlterarSituacao.container).val() == 10) {
			$('.divEncerramento', TituloAlterarSituacao.container).removeClass('hide');
		}
		else if ($('.ddlNovaSituacao', TituloAlterarSituacao.container).val() == 13 && $('#SituacaoAtual', TituloAlterarSituacao.container).val() != "Suspenso") {
			$('.divProrrogar', TituloAlterarSituacao.container).removeClass('hide');
		}
	},

	salvar: function () {
		var objeto = {
			Id: $('.hdnTituloId', TituloAlterarSituacao.container).val(),
			Situacao: { Id: $('.ddlNovaSituacao', TituloAlterarSituacao.container).val() },
			DiasProrrogados: $('.txtDiasProrrogados', TituloAlterarSituacao.container).val(),
			MotivoEncerramentoId: $('.ddlMotivoEncerramento', TituloAlterarSituacao.container).val(),
			MotivoSuspensao: $('.txtMotivo', TituloAlterarSituacao.container).val()
		};

		MasterPage.carregando(true);
		$.ajax({
			url: TituloAlterarSituacao.settings.urls.salvar,
			data: JSON.stringify({ titulo: objeto }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(TituloAlterarSituacao.settings.urls.redirecionar + '?Msg=' + response.Msg + '&acaoId=' + response.AcaoId + '&modelo=' + response.modelo);
				} else {
					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(MasterPage.getContent(TituloAlterarSituacao.container), response.Msg);
					}
				}
			}
		});
		MasterPage.carregando(false);
	}
}
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />

AcompanhamentoAlterarSituacao = {
	container: null,
	settings: {
		situacaoCancelado: 0,
		urls: {
			alterarSituacao: null
		}
	},

	load: function (container, options) {
		if (options) { $.extend(AcompanhamentoAlterarSituacao.settings, options); }
		AcompanhamentoAlterarSituacao.container = MasterPage.getContent(container);

		AcompanhamentoAlterarSituacao.container.delegate('.btnAlterarSituacao', 'click', AcompanhamentoAlterarSituacao.alterarSituacao);
		AcompanhamentoAlterarSituacao.container.delegate('.ddlSituacaoNova', 'change', AcompanhamentoAlterarSituacao.ddlSituacaoChange);

		ddl = $('.ddlSituacaoNova', AcompanhamentoAlterarSituacao.container);
		ddl.val(ddl.find('option').eq(1).val());

		AcompanhamentoAlterarSituacao.ddlSituacaoChange();
	},

	ddlSituacaoChange: function () {
		var situacao = $('.ddlSituacaoNova', AcompanhamentoAlterarSituacao.container).val();
		if (situacao == AcompanhamentoAlterarSituacao.settings.situacaoCancelado) {
			$('.divMotivo', AcompanhamentoAlterarSituacao.container).show();
			$('.txtMotivo', AcompanhamentoAlterarSituacao.container).val('');
		} else {
			$('.divMotivo', AcompanhamentoAlterarSituacao.container).hide();
		}
	},

	alterarSituacao: function () {
		MasterPage.carregando(true);
		var objeto = {
			Id: $('.hdnAcompanhamentoId', AcompanhamentoAlterarSituacao.container).val(),
			FiscalizacaoId: $('.hdnFiscalizacaoId', AcompanhamentoAlterarSituacao.container).val(),
			SituacaoId: $('.ddlSituacaoNova', AcompanhamentoAlterarSituacao.container).val(),
			DataSituacao: { DataTexto: $('.txtDataSituacaoNova', AcompanhamentoAlterarSituacao.container).val() },
			Motivo: $('.txtMotivo', AcompanhamentoAlterarSituacao.container).val()
		};

		var params = { acompanhamento: objeto };

		$.ajax({
			url: AcompanhamentoAlterarSituacao.settings.urls.alterarSituacao, data: JSON.stringify(params),
			type: 'POST', typeData: 'json', contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, AcompanhamentoAlterarSituacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(AcompanhamentoAlterarSituacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
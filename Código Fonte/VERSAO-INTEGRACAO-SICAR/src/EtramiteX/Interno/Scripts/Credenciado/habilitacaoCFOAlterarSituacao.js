/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />

HabilitacaoCFOAlterarSituacao = {
	settings: {
		urls: {
			alterarSituacao: null
		},
		situacaoMotivo: null
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(HabilitacaoCFOAlterarSituacao.settings, options);
		}
		HabilitacaoCFOAlterarSituacao.container = MasterPage.getContent(container);
		HabilitacaoCFOAlterarSituacao.container.delegate('.btnSalvar', 'click', HabilitacaoCFOAlterarSituacao.alterarSituacao);
		$(".ddlSituacao", HabilitacaoCFOAlterarSituacao.container).change(HabilitacaoCFOAlterarSituacao.situacaoChange);
	},

	situacaoChange: function () {
		if ($(".ddlSituacao", HabilitacaoCFOAlterarSituacao.container).val() == HabilitacaoCFOAlterarSituacao.settings.situacaoMotivo) {
			$(".divMotivo", HabilitacaoCFOAlterarSituacao.container).removeClass("hide");
		}
		else {
			$(".divMotivo", HabilitacaoCFOAlterarSituacao.container).addClass("hide");
			$('.ddlMotivo :selected', container).ddlFirst();
		}
	},

	obter: function () {
		var container = HabilitacaoCFOAlterarSituacao.container;
		var obj = {
			Id: $('.hdnHabilitacaoId', container).val(),
			Situacao: $('.ddlSituacao :selected', container).val(),
			SituacaoData: $('.txtSituacaoData', container).val(),
			Motivo: $('.ddlMotivo :selected', container).val(),
			Observacao: $('.txtObservacao', container).val()
		}

		return obj;
	},

	alterarSituacao: function () {
		Mensagem.limpar(HabilitacaoCFOAlterarSituacao.container);

		MasterPage.carregando(true);
		$.ajax({
			url: HabilitacaoCFOAlterarSituacao.settings.urls.alterarSituacao,
			data: JSON.stringify(HabilitacaoCFOAlterarSituacao.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(HabilitacaoCFOAlterarSituacao.container, response.Msg);
				}

				if (response.EhValido) {
					HabilitarEmissaoCFOCFOCListar.container.listarAjax('ultimaBusca');
					Mensagem.gerar(HabilitarEmissaoCFOCFOCListar.container, response.Msg);
					Modal.fechar(HabilitacaoCFOAlterarSituacao.container);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
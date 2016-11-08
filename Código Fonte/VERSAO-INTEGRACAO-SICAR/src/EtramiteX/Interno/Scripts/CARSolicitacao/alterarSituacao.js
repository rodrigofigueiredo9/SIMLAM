/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />


CARSolicitacaoAlterarSituacao = {
	settings: {
		urls: {
			salvar: ''
		}
	},
	container: null,
	mensagem: null,

	load: function (container, options) {
		if (options) {
			$.extend(CARSolicitacaoAlterarSituacao.settings, options);
		}

		CARSolicitacaoAlterarSituacao.container = MasterPage.getContent(container);
		CARSolicitacaoAlterarSituacao.container.delegate('.btnSalvar', 'click', CARSolicitacaoAlterarSituacao.salvar);

		$(".ddlSituacaoNova", CARSolicitacaoAlterarSituacao.container).change(CARSolicitacaoAlterarSituacao.onSituacaoChange);
	},

	onSituacaoChange: function () {

		if ($(".ddlSituacaoNova", CARSolicitacaoAlterarSituacao.container).val() == "4") {
			$(".divMotivo", CARSolicitacaoAlterarSituacao.container).removeClass("hide");
			$(".divMotivo txtSituacaoMotivo", CARSolicitacaoAlterarSituacao.container).focus();
		}
		else {
			if (!$(".divMotivo", CARSolicitacaoAlterarSituacao.container).hasClass("hide")) {
				$(".divMotivo", CARSolicitacaoAlterarSituacao.container).addClass("hide");
			}

		}
		MasterPage.redimensionar();

	},

	obter: function () {
		var container = CARSolicitacaoAlterarSituacao.container;
		var obj = {
			Id: $('.hdnSolicitacaoId', container).val(),
			Numero: $('.txtSituacaoNumeroControle', container).val(),
			DataEmissao: { DataTexto: $('.txtDataEmissao', container).val() },
			SituacaoAnteriorTexto: $('.txtSituacaoAtual', container).val(),
			DataSituacaoAnterior: { DataTexto: $('.txtSituacaoDataAnterior', container).val() },
			SituacaoId: $('.ddlSituacaoNova :selected', container).val(),
			DataSituacao: { DataTexto: $('.txtDataSituacaoNova', container).val() },
			Motivo:  $('.ddlSituacaoNova :selected', container).val() != "4"? "" : $('.txtSituacaoMotivo', container).val()
		}

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: CARSolicitacaoAlterarSituacao.settings.urls.salvar,
			data: JSON.stringify(CARSolicitacaoAlterarSituacao.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, SecagemMecanicaGraos.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(CARSolicitacaoAlterarSituacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
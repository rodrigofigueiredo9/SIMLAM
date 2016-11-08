/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />

FiscalizacaoAlterarSituacao = {
	settings: {
		urls: {
			salvar: ''
		}
	},
	container: null,
	mensagens: null,

	load: function (container, options) {
		if (options) { $.extend(FiscalizacaoAlterarSituacao.settings, options); }
		FiscalizacaoAlterarSituacao.container = MasterPage.getContent(container);

		FiscalizacaoAlterarSituacao.container.delegate('.btnSalvar', 'click', FiscalizacaoAlterarSituacao.salvar);

	},

	obter: function () {
		var container = FiscalizacaoAlterarSituacao.container;
		var obj = {
			Id: $('.hdnFiscalizacaoId', container).val(),
			SituacaoAtualTipo: $('.ddlSituacaoAtual :selected', container).val(),
			SituacaoAtualTipoTexto: $('.ddlSituacaoAtual :selected', container).text(),
			SituacaoAtualData: { Data: $('.txtSituacaoAtualData', container).val() },
			SituacaoNovaTipo: $('.ddlSituacaoNovaTipo :selected', container).val(),
			SituacaoNovaTipoTexto: $('.ddlSituacaoNovaTipo :selected', container).text(),
			SituacaoNovaMotivoTexto: $('.txtSituacaoNovaMotivoTexto', container).val(),
			SituacaoNovaData: { DataTexto: $('.txtSituacaoNovaData', container).val() }
		}

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: FiscalizacaoAlterarSituacao.settings.urls.salvar,
			data: JSON.stringify(FiscalizacaoAlterarSituacao.obter()),
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
					Mensagem.gerar(FiscalizacaoAlterarSituacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
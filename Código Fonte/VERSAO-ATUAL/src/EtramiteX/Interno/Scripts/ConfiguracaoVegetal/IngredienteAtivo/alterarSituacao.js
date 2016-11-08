/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../mensagem.js" />

IngredienteAtivoAlterarSituacao = {
	settings: {
		urls: {
			alterarSituacao: null
		}
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(IngredienteAtivoAlterarSituacao.settings, options);
		}

		IngredienteAtivoAlterarSituacao.container = MasterPage.getContent(container);
		container.delegate('.btnSalvar', 'click', IngredienteAtivoAlterarSituacao.alterarSituacao);
	},

	alterarSituacao: function () {
		Mensagem.limpar(IngredienteAtivoAlterarSituacao.container);

		var objeto = {
			Id: $('.hdnIngredienteAtivoId', IngredienteAtivoAlterarSituacao.container).val(),
			SituacaoId: $('.ddlSituacaoNova', IngredienteAtivoAlterarSituacao.container).val(),
			Motivo: $('.txtMotivo', IngredienteAtivoAlterarSituacao.container).val()
		};

		MasterPage.carregando(true);
		$.ajax({
			url: IngredienteAtivoAlterarSituacao.settings.urls.alterarSituacao,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(IngredienteAtivoAlterarSituacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
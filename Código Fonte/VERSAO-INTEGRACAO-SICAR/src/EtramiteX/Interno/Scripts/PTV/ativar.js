/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
PTVAtivar = {
	settings: {
		urls: {
			urlSalvar: null
		}
	},

	container: null,

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(PTVAtivar.settings, options); }

		container.listarAjax();
		container.delegate('.btnSalvar', 'click', PTVAtivar.ativar);
		Aux.setarFoco(container);
		PTVAtivar.container = container;
	},

	ativar: function () {
		var objeto = {
			Id: $('.hdnEmissaoId', PTVAtivar.container).val(),
			NumeroTipo: $('.hdnNumeroTipo', PTVAtivar.container).val(),
			DataAtivacao: DataAtivacao = { DataTexto: $('.txtDataAtivacao', PTVAtivar.container).val() }
		};

		MasterPage.carregando(true);
		$.ajax({
			url: PTVAtivar.settings.urls.urlSalvar,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.Url);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(PTVAtivar.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
PTVCancelar = {
	settings: {
		urls: {
			urlSalvar: null
		}
	},

	container: null,

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(PTVCancelar.settings, options); }

		container.listarAjax();
		container.delegate('.btnSalvar', 'click', PTVCancelar.cancelar);
		Aux.setarFoco(container);
		PTVCancelar.container = container;
	},

	cancelar: function () {
		var objeto = {
			Id: $('.hdnEmissaoId', PTVCancelar.container).val(),
			DataCancelamento: DataCancelamento = { DataTexto: $('.txtDataCancelamento', PTVCancelar.container).val() }
		};

		MasterPage.carregando(true);
		$.ajax({
			url: PTVCancelar.settings.urls.urlSalvar,
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
					Mensagem.gerar(PTVCancelar.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
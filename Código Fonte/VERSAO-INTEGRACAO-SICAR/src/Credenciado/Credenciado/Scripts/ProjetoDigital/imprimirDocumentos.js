/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="projetoDigital.js" />
/// <reference path="../mensagem.js" />

ImprimirDocumentos = {
    settings: {
        urls: {
            fechar: null
        }
    },
    
	container: null,
	load: function (container, options) {

		if (options) {$.extend(ImprimirDocumentos.settings, options);}

		ImprimirDocumentos.container = MasterPage.getContent(container);
		ImprimirDocumentos.container.delegate('.btnFechar', 'click', ImprimirDocumentos.onFechar);

	},

	onFechar: function () {

		var container = ImprimirDocumentos.container;

		$.ajax({
			url: ImprimirDocumentos.settings.urls.fechar,
			data: JSON.stringify({ Id: $('.hdnProjetoDigitalId', container).val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ImprimirDocumentos.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido && response.Msg) {
					Mensagem.gerar(ImprimirDocumentos.container, response.Msg);
					return;
				}

				if (response.UrlRedirecionar) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}

			}
		});
	}
}
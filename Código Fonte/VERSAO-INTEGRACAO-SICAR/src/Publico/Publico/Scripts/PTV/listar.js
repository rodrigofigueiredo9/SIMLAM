/// <reference path="Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

PTVListar = {
    settings: {
        urls: {
        	urlPDFInterno: null,
        	urlPDFCredenciado: null,
            urlEnviarConfirm: null,
            urlEnviar: null
        }
    },

    container: null,

    load: function (container, options) {
        container = MasterPage.getContent(container);
        if (options) { $.extend(PTVListar.settings, options); }

        container.listarAjax();
        container.delegate('.btnPDF', 'click', PTVListar.gerarPDF);

        Aux.setarFoco(container);

        PTVListar.container = container;
    },

    obter: function (container) {
        return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
    },

    gerarPDF: function () {
    	var item = PTVListar.obter(this);
    	if (item.Tipo == "PTV") {
    		MasterPage.redireciona(PTVListar.settings.urls.urlPDFInterno + '/' + item.Id);
    	}
    	else {
    		MasterPage.redireciona(PTVListar.settings.urls.urlPDFCredenciado + '/' + item.Id);
    	}
    }
}
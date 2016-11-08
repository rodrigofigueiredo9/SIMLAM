/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

PragaListar = {
	settings: {
		onAssociarCallback: null,
	},

    urlEditar: null,
    urlAssociarCultura: null,
    container: null,

    load: function (container, options) {
        container = MasterPage.getContent(container);
        if (options) { $.extend(PragaListar.settings, options); }

        container.listarAjax();

        container.delegate('.btnEditar', 'click', PragaListar.editar);
        container.delegate('.btnAssociar', 'click', PragaListar.associarCultura);
        container.delegate('.btnAssociarPraga', 'click', PragaListar.associarPraga);

        Aux.setarFoco(container);
        PragaListar.container = container;
    },

    obter: function (container) {
        return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
    },

    editar: function () {
        var objeto = PragaListar.obter(this);
        MasterPage.redireciona(PragaListar.urlEditar + '/' + objeto.Id);
    },

    associarCultura: function () {
        var objeto = PragaListar.obter(this);
        MasterPage.redireciona(PragaListar.urlAssociarCultura + '/?pragaId=' + objeto.Id);
    },

    associarPraga: function () {
    	var objeto = PragaListar.obter(this);
    	var sucesso = PragaListar.settings.onAssociarCallback(objeto);
    	if (sucesso) {
    		Modal.fechar(PragaListar.container);
    	} else {
    		Mensagem.gerar(PragaListar.container, msgErro);
    	}
    }
}
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

DeclaracaoAdicionalListar = {
	settings: {
		onAssociarCallback: null,
	},

	urlEditar: null,
	urlExcluir: null,
	urlExcluirConfirm: '',
    container: null,

    load: function (container, options) {
        container = MasterPage.getContent(container);
        if (options) { $.extend(DeclaracaoAdicionalListar.settings, options); }

        container.listarAjax();

        container.delegate('.btnEditar', 'click', DeclaracaoAdicionalListar.editar);
        container.delegate('.btnExcluir', 'click', DeclaracaoAdicionalListar.excluir);

        DeclaracaoAdicionalListar.container = container;
    },

    obter: function (container) {
        return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
    },

    editar: function () {
        var objeto = DeclaracaoAdicionalListar.obter(this);
        MasterPage.redireciona(DeclaracaoAdicionalListar.urlEditar + '/' + objeto.Id);
    },

    excluir: function () {
        var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;

        Modal.excluir({
            'urlConfirm': DeclaracaoAdicionalListar.urlExcluirConfirm,
            'urlAcao': DeclaracaoAdicionalListar.urlExcluir,
            'id': itemId,
            'btnExcluir': this
        });
    }

}
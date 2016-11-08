/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

AtividadeConfiguracaoListar = {
    urlExcluirConfirm: '',
    urlExcluir: '',
    urlEditar: '',
    container: null,

    load: function (container) {
        container = MasterPage.getContent(container);
        container.listarAjax();

        container.delegate('.btnVisualizar', 'click', AtividadeConfiguracaoListar.visualizar);
        container.delegate('.btnEditar', 'click', AtividadeConfiguracaoListar.editar);
        container.delegate('.btnExcluir', 'click', AtividadeConfiguracaoListar.excluir);

        AtividadeConfiguracaoListar.container = container;
        Aux.setarFoco(container);
    },

    editar: function () {
        var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
        MasterPage.redireciona(AtividadeConfiguracaoListar.urlEditar + '/' + itemId);
    },

    visualizar: function () {
        var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
        MasterPage.redireciona($('.urlVisualizar', AtividadeConfiguracaoListar.container).val() + "/" + itemId);
    },

    excluir: function () {
        Modal.excluir({
            'urlConfirm': AtividadeConfiguracaoListar.urlExcluirConfirm,
            'urlAcao': AtividadeConfiguracaoListar.urlExcluir,
            'id': $(this).closest('tr').find('.itemId:first').val(),
            'btnExcluir': this
        });
    }
}
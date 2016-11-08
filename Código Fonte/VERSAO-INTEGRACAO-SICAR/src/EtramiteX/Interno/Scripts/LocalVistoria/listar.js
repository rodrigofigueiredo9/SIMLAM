/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

LocalVistoriaListar = {
	urlEditar: null,
	urlAlterarSituacao: null,
	PodeEditar: null,
    PodeVisualizar: null,
    container: null,
    Mensagens: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
	    if (options) { $.extend(LocalVistoriaListar.settings, options); }
	    LocalVistoriaListar.container = MasterPage.getContent(container);
	    LocalVistoriaListar.container.listarAjax();

	    LocalVistoriaListar.container.delegate('.btnVisualizar', 'click', LocalVistoriaListar.visualizar);
	    LocalVistoriaListar.container.delegate('.btnEditar', 'click', LocalVistoriaListar.editar);

		Aux.setarFoco(container);

		if (LocalVistoriaListar.settings.associarFuncao) {
		    $('.hdnIsAssociar', LocalVistoriaListar.container).val(true);
		}
	},

	editar: function () {
	    if (LocalVistoriaListar.PodeEditar) {
	        var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).SetorID;
	        MasterPage.carregando(true);
	        MasterPage.redireciona(LocalVistoriaListar.urlEditar + '?idSetor=' + itemId);
	    }
	    else {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoriaListar.container), [LocalVistoriaListar.Mensagens.SemPermissaoEditar]);
	    }
	},

	visualizar: function () {
	    if (LocalVistoriaListar.PodeVisualizar) {
	        var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).SetorID;
	        MasterPage.carregando(true);
	        MasterPage.redireciona(LocalVistoriaListar.urlVisualizar + '?idSetor=' + itemId);
	    }
	    else {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoriaListar.container), [LocalVistoriaListar.Mensagens.SemPermissaoVisualizar]);
	    }

	}


}
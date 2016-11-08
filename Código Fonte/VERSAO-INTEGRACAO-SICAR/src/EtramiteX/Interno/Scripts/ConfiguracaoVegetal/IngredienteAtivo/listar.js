/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

IngredienteAtivoListar = {
	urlEditar: null,
	urlAlterarSituacao: null,
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(IngredienteAtivoListar.settings, options); }
		IngredienteAtivoListar.container = MasterPage.getContent(container);
		IngredienteAtivoListar.container.listarAjax();

		IngredienteAtivoListar.container.delegate('.btnAssociar', 'click', IngredienteAtivoListar.associar);
		IngredienteAtivoListar.container.delegate('.btnEditar', 'click', IngredienteAtivoListar.editar);
		IngredienteAtivoListar.container.delegate('.btnAlterarSituacao', 'click', IngredienteAtivoListar.alterarSituacao);

		Aux.setarFoco(container);

		if (IngredienteAtivoListar.settings.associarFuncao) {
			$('.hdnIsAssociar', IngredienteAtivoListar.container).val(true);
		}
	},

	editar: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		MasterPage.redireciona(IngredienteAtivoListar.urlEditar + '?id=' + itemId);
	},

	alterarSituacao: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		MasterPage.redireciona(IngredienteAtivoListar.urlAlterarSituacao + '?id=' + itemId);
	},

	associar: function () {
		var objeto = $.parseJSON($(this).closest('tr').find('.itemJson').val());
		var retorno = IngredienteAtivoListar.settings.associarFuncao(objeto);

		if (retorno === true) {
			Modal.fechar(IngredienteAtivoListar.container);
		} else {
			Mensagem.gerar(IngredienteAtivoListar.container, retorno);
		}
	}
}
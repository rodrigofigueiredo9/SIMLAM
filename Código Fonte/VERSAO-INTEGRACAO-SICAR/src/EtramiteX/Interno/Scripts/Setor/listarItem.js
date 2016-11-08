/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ItemListar = {
	urlEditar: '',
	urlValidarAbrirEditar: '',
	mensagens: {},
	container: null,
	settings: {
		editarFuncao: null
	},

	load: function (container, options) {
		container = MasterPage.getContent(container);
		container.listarAjax();
		if (options) { $.extend(ItemListar.settings, options); }

		Aux.setarFoco(container);
		container.delegate('.btnVisualizar', 'click', ItemListar.visualizar);
		container.delegate('.btnEditarItem', 'click', ItemListar.editar);

		if ($('.hdnCriarItem').val() == "True") {
			Modal.defaultButtons(container, ItemListar.criar, "Novo item");
		} else {
			Modal.defaultButtons(container);
		}

		ItemListar.container = container;

		if (ItemListar.settings.associarFuncao) {
			$('.hdnIsAssociar', container).val(true);
		}
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.hdnItemId:first').val());
		var content = MasterPage.getContent($(this, ItemListar.container));
		MasterPage.redireciona($('.urlVisualizar', content).val() + "/" + itemId);
	},

	editar: function () {
		var linha = $(this).closest('tr');

		if ($('.hdnIsAssociar', ItemListar.container).val().toLowerCase() === "true") {
			var retorno = MasterPage.validarAjax(ItemListar.urlValidarAbrirEditar, { id: $('.hdnItemId', linha).val() }, ItemListar.container, false);

			if (retorno.EhValido) {
				Modal.abrir(
					ItemListar.urlCriarItem,
					{ id: $('.hdnItemId', linha).val() },
					function (container) { ItemSalvar.load(container, { associarFuncao: ItemListar.settings.editarFuncao, callBackSalvar: ItemListar.atualizarLista }); },
					Modal.tamanhoModalGrande);
			}
		} else {
			MasterPage.redireciona(ItemListar.urlEditar + '/' + $('.hdnItemId', linha).val());
		}
	},

	atualizarLista: function (retorno) {
		ItemListar.container.listarAjax('ultimaBusca');
		Mensagem.gerar(ItemListar.container, retorno.mensagem);
	}
}
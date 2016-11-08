/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ItemListar = {
	urlEditar: '',
	urlValidarAbrirEditar: '',
	urlCriarItem: '',
	urlExcluir: '',
	urlComfirmExcluir: '',
	mensagens: {},
	container: null,
	settings: {
		excluirFuncao: null,
		editarFuncao: null,
		associarFuncao: null
	},

	load: function (container, options) {
		container = MasterPage.getContent(container);
		container.listarAjax();
		if (options) { $.extend(ItemListar.settings, options); }

		Aux.setarFoco(container);
		container.delegate('.btnAssociar', 'click', ItemListar.associar);
		container.delegate('.btnVisualizar', 'click', ItemListar.visualizar);
		container.delegate('.btnExcluirItem', 'click', ItemListar.excluir);
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

	criar: function () {
		Modal.abrir(
			ItemListar.urlCriarItem, null,
			function (container) { ItemSalvar.load(container, { associarFuncao: ItemListar.settings.associarFuncao, callBackSalvar: ItemListar.atualizarLista }); },
			Modal.tamanhoModalGrande);
	},

	atualizarLista: function (retorno) {
		ItemListar.container.listarAjax('ultimaBusca');
		Mensagem.gerar(ItemListar.container, retorno.mensagem);
	},

	associar: function () {
		var container = $(this).closest('tr');
		var Item = {};

		Item.Id = +$('.hdnItemId', container).val();
		Item.Nome = $('.hdnItemNome', container).val();
		Item.Condicionante = $('.hdnItemCondicionante', container).val();
		Item.TipoTexto = $('.hdnItemtipoTexto', container).val();
		Item.Tipo = +$('.hdnItemtipo', container).val();
		Item.ProcedimentoAnalise = $('.hdnItemProcedimento', container).val();
		Item.Tid = $('.hdnItemTid', container).val();

		var retorno = ItemListar.settings.associarFuncao(Item);
		if (retorno.length == 0) {
			Mensagem.gerar(ItemListar.container, new Array(Mensagem.replace(ItemListar.mensagens.ItemAdicionado, '#ITEM#', Item.Nome)));
		} else {
			Mensagem.gerar(ItemListar.container, retorno);
		}
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': ItemListar.urlComfirmExcluir,
			'urlAcao': ItemListar.urlExcluir,
			'id': parseInt($(this).closest('tr').find('.hdnItemId:first').val()),
			'callBack': ItemListar.excluirCallBack,
			'btnExcluir': this
		});
	},

	excluirCallBack: function (response, btnExcluir) {
		var itemId = parseInt($(btnExcluir).closest('tr').find('.hdnItemId').val());

		if (ItemListar.settings.excluirFuncao && ItemListar.settings.excluirFuncao != null) {
			ItemListar.settings.excluirFuncao({ Id: itemId }, response.Msg);
		}
	}
}
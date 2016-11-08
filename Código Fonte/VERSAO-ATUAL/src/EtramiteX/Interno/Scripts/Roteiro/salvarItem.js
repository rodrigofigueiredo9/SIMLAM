/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ItemSalvar = {
	urlModal: null,
	urlSalvarItem: null,
	urlModalConfirmar: null,
	urlAtualizaRoteiro: null,
	Mensagens: {},
	container: null,
	settings: {
		associarFuncao: null,
		callBackSalvar: null
	},

	load: function (container, options) {
		if (options) { $.extend(ItemSalvar.settings, options); }
		ItemSalvar.container = MasterPage.getContent(container);

		if (ItemSalvar.container.hasClass('modalContent')) {
			Modal.defaultButtons(ItemSalvar.container, ItemSalvar.onCadastrarItem, "Salvar");
		} else {
			ItemSalvar.container.delegate('.bntSalvarItemRoteiro', 'click', ItemSalvar.onCadastrarItem);
		}

		$('.Nome', ItemSalvar.container).focus();
	},

	onModalItemConfirmar: function (objeto) {

		ItemSalvar.urlModal = ItemSalvar.urlSalvarItem;

		$.ajax({
			url: ItemSalvar.urlAtualizaRoteiro,
			type: "POST",
			data: JSON.stringify({ id: objeto.ItemRoteiro.Id }),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ItemSalvar.container, response.Msg);
					MasterPage.carregando(false);
					return;
				}

				if (response.atualiza < 1) {
					ItemSalvar.salvarItem();
					return;
				}

				Modal.confirma({
					btnOkLabel: 'Alterar',
					btnOkCallback: ItemSalvar.salvarItem,
					url: ItemSalvar.urlModalConfirmar,
					urlData: { id: objeto.ItemRoteiro.Id },
					tamanhoModal: Modal.tamanhoModalMedia
				});
			}
		});
		MasterPage.carregando(false);
	},

	obter: function () {

		var item = {
			Id: $('.hdnItemNumero', ItemSalvar.container).val(),
			Nome: $('.Nome', ItemSalvar.container).val(),
			Condicionante: $('.txtCondicionante', ItemSalvar.container).val(),
			ProcedimentoAnalise: $('.Procedimento', ItemSalvar.container).val(),
			Tid: $('.hdnTid', ItemSalvar.container).val(),
			Tipo: $('.rdbItemTipo:checked', ItemSalvar.container).val() || 0
		};

		return { ItemRoteiro: item };
	},

	onCadastrarItem: function () {
		var editar = ($('.hdnEditar', ItemSalvar.container).val().toLowerCase() === "true");

		var objeto = ItemSalvar.obter();

		if (editar) {
			ItemSalvar.onModalItemConfirmar(objeto);
		} else {
			ItemSalvar.urlModal = ItemSalvar.urlSalvarItem;
			ItemSalvar.salvarItem();
		}
	},

	salvarItem: function (container) {

		var containerModal = container ? MasterPage.getContent(container) : ItemSalvar.container;

		Modal.carregando(containerModal, true);

		var objParamItem = ItemSalvar.obter();

		$.ajax({
			url: ItemSalvar.urlModal,
			type: "POST",
			data: JSON.stringify(objParamItem),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					if (!ItemSalvar.settings.associarFuncao) {
						MasterPage.redireciona(response.urlRetorno);
						Modal.carregando(containerModal, false);
						return;
					}

					var mensagem = null;

					if ($('.hdnEditar', ItemSalvar.container).val().toLowerCase() === "true") {
						mensagem = new Array(ItemSalvar.Mensagens.ItemEditadoSucesso);
						ItemSalvar.settings.associarFuncao(response.item);
					} else {
						mensagem = new Array(Mensagem.replace(ItemSalvar.Mensagens.ItemAdicionado, '#ITEM#', objParamItem.ItemRoteiro.Nome));
						ItemSalvar.settings.associarFuncao(response.item);
					}

					if (ItemSalvar.settings.callBackSalvar != null) {
						ItemSalvar.settings.callBackSalvar({ mensagem: mensagem });
					}

					Modal.fechar(containerModal);
					Modal.fechar(ItemSalvar.container);
				} else {
					Mensagem.gerar(containerModal, response.Msg);
				}
				Modal.carregando(containerModal, false);
			}
		});
	}
}
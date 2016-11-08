/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../jquery.listar-ajax.js" />
/// <reference path="../masterpage.js" />

LoteListar = {
	urlVisualizar: null,
	urlEditar: null,
	urlConfirmarExcluir: null,
	urlExcluir: null,
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(LoteListar.settings, options); }

		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnBuscar', 'click', LoteListar.buscar);
		container.delegate('.btnVisualizar', 'click', LoteListar.visualizar);
		container.delegate('.btnEditar', 'click', LoteListar.editar);
		container.delegate('.btnExcluir', 'click', LoteListar.excluir);
		container.delegate('.btnAssociar', 'click', LoteListar.associar);


		Aux.setarFoco(container);
		LoteListar.container = container;

		if (LoteListar.settings.associarFuncao) {
			$('.hdnIsAssociar', LoteListar.container).val(true);
		}
	},

	obterItemJson: function (container) {
		return $.parseJSON($(container).closest('tr').find('.itemJson').val());
	},

	visualizar: function () {
		var item = LoteListar.obterItemJson(this);

		if (LoteListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', LoteListar.container).val() + '/' + item.Id, null,
			function (container) {
				Modal.defaultButtons(container);
			}, Modal.tamanhoModalGrande);
		} else {
			MasterPage.redireciona($('.urlVisualizar', LoteListar.container).val() + '/' + item.Id);
		}
	},

	editar: function () {
		var item = LoteListar.obterItemJson(this);
		MasterPage.redireciona(LoteListar.urlEditar + '/' + item.Id);
	},

	excluir: function () {
		Mensagem.limpar(LoteListar.container);
		Modal.excluir({
			'urlConfirm': LoteListar.urlConfirmarExcluir,
			'urlAcao': LoteListar.urlExcluir,
			'id': LoteListar.obterItemJson(this).Id,
			'btnExcluir': this
		});
	},

	associar: function () {
		var item = LoteListar.obterItemJson(this);
		var retorno = LoteListar.settings.associarFuncao(item);

		if (retorno !== undefined && retorno.length > 0) {
			Mensagem.gerar(MasterPage.getContent(LoteListar.container), retorno);
		} else {
			Modal.fechar(LoteListar.container);
		}
	}
}
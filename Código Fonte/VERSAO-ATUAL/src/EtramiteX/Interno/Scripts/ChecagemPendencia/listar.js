/// <reference path="../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />

ChecagemPendenciaListar = {
	urlExcluir: '',
	urlExcluirConfirm: '',
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(ChecagemPendenciaListar.settings, options); }
		ChecagemPendenciaListar.container = MasterPage.getContent(container);
		ChecagemPendenciaListar.container.listarAjax();

		ChecagemPendenciaListar.container.delegate('.btnExcluir', 'click', ChecagemPendenciaListar.excluir);
		ChecagemPendenciaListar.container.delegate('.btnVisualizar', 'click', ChecagemPendenciaListar.visualizar);
		ChecagemPendenciaListar.container.delegate('.btnAssociar', 'click', ChecagemPendenciaListar.associar);

		Aux.setarFoco(container);

		if (ChecagemPendenciaListar.settings.associarFuncao) {
			$('.hdnIsAssociar', ChecagemPendenciaListar.container).val(true);
		}
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		if (ChecagemPendenciaListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', ChecagemPendenciaListar.container).val() + "/" + itemId, null, function (context) {
				ChecagemPendencia.load(MasterPage.getContent(context));
				Modal.defaultButtons(context);
			});
		} else {
			MasterPage.redireciona($('.urlVisualizar', ChecagemPendenciaListar.container).val() + "/" + itemId);
		}
	},

	associar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		var itemSituacao = parseInt($(this).closest('tr').find('.itemSituacaoId:first').val());

		var retorno = ChecagemPendenciaListar.settings.associarFuncao({ Id: itemId, SituacaoId: itemSituacao });

		if (retorno === true) {
			Modal.fechar(ChecagemPendenciaListar.container);
		} else {
			Mensagem.gerar(ChecagemPendenciaListar.container, retorno);
		}
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': ChecagemPendenciaListar.urlExcluirConfirm,
			'urlAcao': ChecagemPendenciaListar.urlExcluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});
	}
};
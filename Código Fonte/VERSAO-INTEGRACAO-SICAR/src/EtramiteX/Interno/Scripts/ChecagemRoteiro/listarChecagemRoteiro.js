/// <reference path="../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />

ChecagemRoteirotListar = {
	urlExcluirConfirm: '',
	urlExcluir: '',
	urlGerarPdf: '', 
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(ChecagemRoteirotListar.settings, options); }
		ChecagemRoteirotListar.container = MasterPage.getContent(container);
		container.listarAjax();

		ChecagemRoteirotListar.container.delegate('.btnExcluir', 'click', ChecagemRoteirotListar.excluir);
		ChecagemRoteirotListar.container.delegate('.btnVisualizar', 'click', ChecagemRoteirotListar.visualizar);
		ChecagemRoteirotListar.container.delegate('.btnAssociar', 'click', ChecagemRoteirotListar.associar);
		ChecagemRoteirotListar.container.delegate('.btnGerarPdf', 'click', ChecagemRoteirotListar.gerarPdf);

		Aux.setarFoco(container);

		if (ChecagemRoteirotListar.settings.associarFuncao) {
			$('.hdnIsAssociar', ChecagemRoteirotListar.container).val(true);
		}
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		if (ChecagemRoteirotListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', ChecagemRoteirotListar.container).val() + "/" + itemId, null, function (context) {
				Modal.defaultButtons(context);
			});
		} else {
			MasterPage.redireciona($('.urlVisualizar', ChecagemRoteirotListar.container).val() + "/" + itemId);
		}
	},

	gerarPdf: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona($('.urlVisualizarPdf', ChecagemRoteirotListar.container).val() + "/" + itemId);
	},

	associar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		var itemSituacao = parseInt($(this).closest('tr').find('.itemSituacaoId:first').val());

		var retorno = ChecagemRoteirotListar.settings.associarFuncao({ Id: itemId, SituacaoId: itemSituacao });

		if (retorno === true) {
			Modal.fechar(ChecagemRoteirotListar.container);
		} else {
			Mensagem.gerar(ChecagemRoteirotListar.container, retorno);
		}
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': ChecagemRoteirotListar.urlExcluirConfirm,
			'urlAcao': ChecagemRoteirotListar.urlExcluir,
			'id': parseInt($(this).closest('tr').find('.itemId:first').val()),
			'btnExcluir': this
		});
	}
};
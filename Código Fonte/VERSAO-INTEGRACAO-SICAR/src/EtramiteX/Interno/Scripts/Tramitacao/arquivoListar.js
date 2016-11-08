/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

TramitacaoArquivoListar = {
	urlEditar: '',
	urlVisualizar: '',
	urlConfirmarExcluir: '',
	urlExcluir: '',
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(TramitacaoArquivoListar.settings, options); }
		TramitacaoArquivoListar.container = MasterPage.getContent(container);
		TramitacaoArquivoListar.container.listarAjax();

		TramitacaoArquivoListar.container.delegate('.btnEditar', 'click', TramitacaoArquivoListar.editar);
		TramitacaoArquivoListar.container.delegate('.btnExcluir', 'click', TramitacaoArquivoListar.excluir);
		TramitacaoArquivoListar.container.delegate('.btnVisualizar', 'click', TramitacaoArquivoListar.visualizar);

		Aux.setarFoco(container);
		if (TramitacaoArquivoListar.settings.associarFuncao) {
			$('.hdnIsAssociar', TramitacaoArquivoListar.container).val(true);
		}
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(TramitacaoArquivoListar.urlEditar + "/" + itemId);
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		if (TramitacaoArquivoListar.associarFuncao) {
			Modal.abrir($('.urlVisualizar', TramitacaoArquivoListar.container).val() + "/" + itemId, null, function (context) {
				Modal.defaultButtons(context);
			}, Modal.tamanhoModalGrande);
		} else {
			MasterPage.redireciona($('.urlVisualizar', TramitacaoArquivoListar.container).val() + "/" + itemId);
		}
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': TramitacaoArquivoListar.urlConfirmarExcluir,
			'urlAcao': TramitacaoArquivoListar.urlExcluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});
	}
}
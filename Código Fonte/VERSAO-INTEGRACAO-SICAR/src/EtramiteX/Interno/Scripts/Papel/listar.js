/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

PapelListar = {
	urlExcluirConfirm: '',
	urlExcluir: '',
	urlEditar: '',
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(PapelListar.settings, options); }
		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnExcluir', 'click', PapelListar.excluir);
		container.delegate('.btnVisualizar', 'click', PapelListar.visualizar);
		container.delegate('.btnEditar', 'click', PapelListar.editar);

		PapelListar.container = container;
		Aux.setarFoco(container);

		if (PapelListar.settings.associarFuncao) {
			$('.hdnIsAssociar', container).val(true);
		}
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(PapelListar.urlEditar + '/' + itemId);
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		if (PapelListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', PapelListar.container).val() + "/" + itemId, null, function (context) {
				Modal.defaultButtons(context);				
			});
		} else {
			MasterPage.redireciona($('.urlVisualizar', PapelListar.container).val() + "/" + itemId);
		}
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': PapelListar.urlExcluirConfirm,
			'urlAcao': PapelListar.urlExcluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});
	}
}
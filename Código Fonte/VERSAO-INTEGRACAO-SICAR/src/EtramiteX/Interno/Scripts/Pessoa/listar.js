/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />

PessoaListar = {
	urlExcluirConfirm: '',
	urlExcluir: '',
	urlVisualizar: '',
	urlEditar: '',
	container: null,

	load: function (container) {
		container = MasterPage.getContent(container);
		PessoaListar.container = container;
		container.listarAjax();

		container.delegate('.btnExcluir', 'click', PessoaListar.excluir);
		container.delegate('.btnVisualizar', 'click', PessoaListar.visualizar);
		container.delegate('.btnEditar', 'click', PessoaListar.editar);

		container.delegate('.radioPessoaCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioPessoaCpfCnpj', container));
		Aux.setarFoco(container);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(PessoaListar.urlEditar + '/' + itemId);
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(PessoaListar.urlVisualizar + '/' + itemId);
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': PessoaListar.urlExcluirConfirm,
			'urlAcao': PessoaListar.urlExcluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});
	}
}
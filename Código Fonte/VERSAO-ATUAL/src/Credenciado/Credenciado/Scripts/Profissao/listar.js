/// <reference path="../masterpage.js" />
/// <reference path="../mensagem.js" />
/// <reference path="../Lib/JQuery/jquery-1.10.1.js" />
ProfissaoListar = {
	settings: {
		associarFuncao:null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(ProfissaoListar.settings, options); }

		container = MasterPage.getContent(container);
		container.listarAjax();
		container.delegate('.btnAssociar', 'click', ProfissaoListar.associar);

		ProfissaoListar.container = container;

		if (ProfissaoListar.settings.associarFuncao) {
			$('.hdnIsAssociar', ProfissaoListar.container).val(true);
		}

		Aux.setarFoco(container);
	},
	
	associar: function () {
		var linha = $(this).closest('tr');
		var id = linha.find('.itemId').val();
		var texto = linha.find('td:first').text();
		texto = $.trim(texto).replace("\n", "");

		ProfissaoListar.settings.associarFunc(id, texto);
		Modal.fechar($(this));
	}
}
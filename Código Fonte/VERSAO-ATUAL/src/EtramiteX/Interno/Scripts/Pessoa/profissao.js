/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

ProfissaoAssociar = {
	container: null,
	settings: {
		associarFunc: null
	},

	load: function (container, options) {
		if (options) {
			$.extend(ProfissaoAssociar.settings, options);
		}

		container.listarAjax();
		container.delegate('.btnAssociar', 'click', ProfissaoAssociar.associar);

		ProfissaoAssociar.container = container;
		Aux.setarFoco(container);
	},

	associar: function () {
		var linha = $(this).closest('tr');
		var id = linha.find('.itemId').val();
		var texto = linha.find('td:first').text();
		texto = $.trim(texto).replace("\n", "");

		ProfissaoAssociar.settings.associarFunc(id, texto);
		Modal.fechar($(this));
	}
}
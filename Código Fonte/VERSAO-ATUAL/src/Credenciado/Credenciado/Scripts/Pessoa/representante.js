/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

RepresentanteAssociar = {
	container: null,
	settings: {
		associarFunc: null
	},

	load: function (container, options) {
		if (options) {
			$.extend(RepresentanteAssociar.settings, options);
		}

		container.listarAjax();
		container.delegate('.btnAssociar', 'click', RepresentanteAssociar.associar);

		RepresentanteAssociar.container = container;
		Aux.setarFoco(container);
	},

	associar: function () {
		var contr = Modal.getModalContent(RepresentanteAssociar.container);
		var linha = $(this).closest('tr');
		var id = linha.find('.itemId').val();
		var nome = $.trim(linha.find('td:first').text()).replace("\n", "");
		var cpf = $.trim(linha.find('td:nth-child(2)').text()).replace("\n", "");

		var retorno = RepresentanteAssociar.settings.associarFunc(id, nome, cpf, contr);

		if (retorno === true) {
			Modal.fechar($(this));
		} else {
			Mensagem.gerar(contr, retorno);
		}
	}
}
ProfissaoListar = {
	settings: {
		urls: {
			editar: '',
			criar: '',
			salvar: '',
			visualizar: ''
		},
		associarFunc: null
	},
	container: null,
	
	load: function (container, options) {
		if (options) { $.extend(ProfissaoListar.settings, options); }

		container = MasterPage.getContent(container);
		container.listarAjax();		
		container.delegate('.btnEditar', 'click', ProfissaoListar.editar);
		container.delegate('.btnAssociar', 'click', ProfissaoListar.associar);
		
		ProfissaoListar.container = container;

		if (ProfissaoListar.settings.associarFuncao) {
			$('.hdnIsAssociar', ProfissaoListar.container).val(true);
		}

		Aux.setarFoco(container);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(ProfissaoListar.settings.urls.editar  + '/' + itemId);
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(ProfissaoListar.settings.urls.visualizar + "/" + itemId);
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
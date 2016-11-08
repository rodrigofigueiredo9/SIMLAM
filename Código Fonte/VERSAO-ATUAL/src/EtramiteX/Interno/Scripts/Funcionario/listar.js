/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

FuncionarioListar = {
	urlAlterarSituacao: '',
	urlEditar: '',
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(FuncionarioListar.settings, options); }
		container.listarAjax();

		container.delegate('.btnAssociar', 'click', FuncionarioListar.associar);
		container.delegate('.btnVisualizar', 'click', FuncionarioListar.visualizar);
		container.delegate('.btnEditar', 'click', function () { FuncionarioListar.redirecionar(this, FuncionarioListar.urlEditar); });
		container.delegate('.btnAltStatus', 'click', function () { FuncionarioListar.redirecionar(this, FuncionarioListar.urlAlterarSituacao); });

		Aux.setarFoco(container);
		FuncionarioListar.container = container;

		if (FuncionarioListar.settings.associarFuncao) {
			$('.hdnIsAssociar', container).val(true);
		}
	},

	redirecionar: function (btn, url) {
		MasterPage.redireciona(url + "/" + $(btn).closest('tr').find('.itemId:first').val());
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		if (FuncionarioListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', FuncionarioListar.container).val() + "/" + itemId, null, function (context) {
				Modal.defaultButtons(context);
			});
		} else {
			MasterPage.redireciona($('.urlVisualizar', FuncionarioListar.container).val() + "/" + itemId);
		}
	},

	associar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		var itemNome = $(this).closest('tr').find('.itemNome:first').val();
		var itemSituacao = $(this).closest('tr').find('.itemSituacao').val();

		var retorno = FuncionarioListar.settings.associarFuncao({ Id: itemId, Nome: itemNome, Situacao: itemSituacao }, FuncionarioListar.container);

		if (retorno === true) {
			Modal.fechar(FuncionarioListar.container);
		} else {
			var modalContainer = MasterPage.getContent(FuncionarioListar.container);
			Mensagem.gerar(modalContainer, retorno);
		}
	}
}
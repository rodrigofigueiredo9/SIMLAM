/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

TituloModeloListar = {
	urlEditar: null,
	urlDesativarComfirm: null,
	urlDesativar: null,
	urlAtivar: null,
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		container.listarAjax();
		if (options) { $.extend(TituloModeloListar.settings, options); }

		container.delegate('.btnVisualizar', 'click', TituloModeloListar.visualizar);
		container.delegate('.btnEditar', 'click', TituloModeloListar.editar);

		container.delegate('.btnAtivar', 'click', TituloModeloListar.ativar);
		container.delegate('.btnDesativar', 'click', TituloModeloListar.desativar);

		Aux.setarFoco(container);
		TituloModeloListar.container = MasterPage.getContent(container);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(TituloModeloListar.urlEditar + '/' + itemId);
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona($('.urlVisualizar', TituloModeloListar.container).val() + "/" + itemId);
	},

	desativar: function () {
		Modal.executar({
			'urlConfirm': TituloModeloListar.urlDesativarComfirm,
			'urlAcao': TituloModeloListar.urlDesativar,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnAcao': this,
			'btnTexto': 'Desativar'
		});
	},

	ativar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		$.ajax({ url: TituloModeloListar.urlAtivar + "/" + itemId,
			type: "POST",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				Aux.error(XMLHttpRequest, textStatus, errorThrown, TituloModeloListar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					TituloModeloListar.container.listarAjax('ultimaBusca');
				}
				Mensagem.gerar(TituloModeloListar.container, response.Msg);
			}
		});
	}
}
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

CondicionanteDescricaoListar = {
	settings: {
		urls: {
			criar: '',
			editar: '',
			excluir: '',
			excluirSalvar: ''
		},
		onAssociar: null
	},

	container: null,

	load: function (container, options) {
		container = MasterPage.getContent(container);
		CondicionanteDescricaoListar.container = container;

		if (options) {
			$.extend(CondicionanteDescricaoListar.settings, options);
		}
		container.delegate('.btnAssociar', 'click', CondicionanteDescricaoListar.onBtnAssociarClick);
		container.delegate('.btnEditar', 'click', CondicionanteDescricaoListar.onBtnEditarClick);
		container.delegate('.btnExcluir', 'click', CondicionanteDescricaoListar.onBtnExcluirClick);

		container.listarAjax();
		Aux.setarFoco(container);
		Modal.defaultButtons(CondicionanteDescricaoListar.container, CondicionanteDescricaoListar.onBntAdicionarDescricaoClick, 'Cadastrar');
	},

	onBtnEditarClick: function () {
		CondicionanteDescricaoSalvar.settings.onSalvar = CondicionanteDescricaoListar.onSalvar;
		var tr = $(this).closest('tr');
		var itemId = parseInt(tr.find('.hdnItemId').val());
		Modal.abrir(CondicionanteDescricaoListar.settings.urls.editar + '/' + itemId, null, function (container) {
			CondicionanteDescricaoSalvar.load(container);
		});
	},

	onSalvar: function (descricao) {
		$('.btnBuscar', CondicionanteDescricaoListar.container).click();
		return true;
	},

	onBtnExcluirClick: function () {
		Modal.excluir({
			'urlConfirm': CondicionanteDescricaoListar.settings.urls.excluir,
			'urlAcao': CondicionanteDescricaoListar.settings.urls.excluirSalvar,
			'id': parseInt($(this).closest('tr').find('.hdnItemId').val()),
			'btnExcluir': this
		});
	},

	onBtnAssociarClick: function () {
		var descricao = $(this).closest('tr').find('.hdnDescricao').val();
		CondicionanteDescricaoListar.onAssociar(descricao, true);
	},

	onAssociar: function (descricao, mostraErros) {
		var retorno = CondicionanteDescricaoListar.settings.onAssociar(descricao);
		if (retorno !== true) {
			if (mostraErros) {
				Mensagem.gerar(MasterPage.getContent(CondicionanteDescricaoListar.container), retorno);
			}
		} else {
			Modal.fechar(CondicionanteDescricaoListar.container);
		}
		return retorno;
	},

	onBntAdicionarDescricaoClick: function () {
		CondicionanteDescricaoSalvar.settings.onSalvar = CondicionanteDescricaoListar.onAssociar;
		Modal.abrir(CondicionanteDescricaoListar.settings.urls.criar, null, function (container) {
			CondicionanteDescricaoSalvar.load(container);
		});
	}
}
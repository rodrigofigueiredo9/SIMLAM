/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

Dua = {
	urlEditar: null,
	urlVisualizar: null,
	urlExcluir: null,
	urlExcluirConfirm: null,
	urlAlterarSituacao: null,
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(TituloListar.settings, options); }
		TituloListar.container = MasterPage.getContent(container);
		TituloListar.container.listarAjax({ onBeforeFiltrar: TituloListar.onBeforeFiltrar });

		container.delegate('.btnEditar', 'click', TituloListar.editar);
		container.delegate('.btnAlterarSituacao', 'click', TituloListar.onBtnAlterarSituacaoClick);
		container.delegate('.btnExcluir', 'click', TituloListar.excluir);
		container.delegate('.btnVisualizar', 'click', TituloListar.visualizar);
		container.delegate('.btnAssociar', 'click', TituloListar.onAssociarTitulo);
		container.delegate('.btnPDF', 'click', TituloListar.gerarPdf);

		container.delegate('.radioCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioCpfCnpj', container));
		Aux.setarFoco(container);

		if (TituloListar.settings.associarFuncao) {
			$('.hdnIsAssociar', TituloListar.container).val(true);
		}
	},

	onBeforeFiltrar: function (container, serializedData) {
		serializedData.Filtros.EmpreendimentoCodigo = Mascara.getIntMask($(".txtEmpreendimentoCodigo", TituloListar.container).val()).toString();
	},

	onBtnAlterarSituacaoClick: function () {
		var tituloId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		MasterPage.redireciona(TituloListar.urlAlterarSituacao + "/" + tituloId);
	},

	editar: function () {
		var id = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		MasterPage.redireciona(TituloListar.urlEditar + "/" + id);
	},

	visualizar: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;

		if (TituloListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', TituloListar.container).val() + "/" + itemId, null, function (context) {
				Modal.defaultButtons(context);
			}, Modal.tamanhoModalGrande);
		} else {
			MasterPage.redireciona($('.urlVisualizar', TituloListar.container).val() + "/" + itemId);
		}
	},

	excluir: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		Modal.excluir({
			'urlConfirm': TituloListar.urlExcluirConfirm,
			'urlAcao': TituloListar.urlExcluir,
			'id': itemId,
			'btnExcluir': this
		});
	},

	gerarPdf: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		MasterPage.redireciona($('.urlPdf', TituloListar.container).val() + "/" + itemId);
	},

	onAssociarTitulo: function () {
		var objeto = $.parseJSON($(this).closest('tr').find('.itemJson').val());
		var retorno = TituloListar.settings.associarFuncao(objeto);

		Mensagem.gerar(TituloListar.container, retorno.Msg);

		if (retorno.FecharModal) {
			Modal.fechar(TituloListar.container);
		}
	}
};
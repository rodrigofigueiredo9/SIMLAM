/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

FiscalizacaoListar = {
	urlEditar: '',
	urlEditarValidar: '',
	urlExcluir: '',
	ExcluirConfirm: null,
	urlVisualizarPdf: '',
	urlAlterarSituacao: '',
	urlAcompanhamentos: '',
	urlNotificacao: '',
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(FiscalizacaoListar.settings, options); }
		FiscalizacaoListar.container = MasterPage.getContent(container);
		FiscalizacaoListar.container.listarAjax();

		FiscalizacaoListar.container.delegate('.btnExcluir', 'click', FiscalizacaoListar.excluir);
		FiscalizacaoListar.container.delegate('.btnVisualizar', 'click', FiscalizacaoListar.visualizar);
		FiscalizacaoListar.container.delegate('.btnEditar', 'click', FiscalizacaoListar.editar);
		FiscalizacaoListar.container.delegate('.btnAlterarSituacao', 'click', FiscalizacaoListar.alterarSituacao);
		FiscalizacaoListar.container.delegate('.btnAssociar', 'click', FiscalizacaoListar.associar);
		FiscalizacaoListar.container.delegate('.btnDocumentos', 'click', FiscalizacaoListar.documentosGerados);
		FiscalizacaoListar.container.delegate('.btnAcompanhamentos', 'click', FiscalizacaoListar.acompanhamentos);
		FiscalizacaoListar.container.delegate('.btNotificacao', 'click', FiscalizacaoListar.notificacao);

		FiscalizacaoListar.container.delegate('.radioAutuadoCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioAutuadoCpfCnpj', FiscalizacaoListar.container));
		Aux.setarFoco(container);

		if (FiscalizacaoListar.settings.associarFuncao) {
			$('.hdnIsAssociar', container).val(true);
		}
	},

	pdfAbrir: function () {
		MasterPage.redireciona($('.urlVisualizarPdf', FiscalizacaoListar.container).val() + "?id=" + $(this).closest('tr').find('.itemId:first').val());
		MasterPage.carregando(false);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(FiscalizacaoListar.urlEditar + '/' + itemId);
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona($('.urlVisualizar', FiscalizacaoListar.container).val() + "/" + itemId);
	},

	alterarSituacao: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(FiscalizacaoListar.urlAlterarSituacao + '/' + itemId);
	},

	associar: function () {
		var tr = $(this).closest('tr');
		var itemId = parseInt($('.itemId:first', tr).val());
		var itemSituacao = parseInt($('.itemSituacao:first', tr).val());
		var itemDataCriacao = $('.itemDataCriacao:first', tr).val();

		var retorno = FiscalizacaoListar.settings.associarFuncao({ Id: itemId, SituacaoId: itemSituacao, DataCriacao: itemDataCriacao });

		if (retorno === true) {
			Modal.fechar(FiscalizacaoListar.container);
		} else {
			Mensagem.gerar(FiscalizacaoListar.container, retorno);
		}
	},

	documentosGerados: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		Modal.abrir($('.urlDocumentosGerados', FiscalizacaoListar.container).val() + "/" + itemId, null, null, Modal.tamanhoModalMedia);
	},

	acompanhamentos: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(FiscalizacaoListar.urlAcompanhamentos + '/' + itemId);
	},

	notificacao: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(FiscalizacaoListar.urlNotificacao + '/' + itemId);
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': FiscalizacaoListar.ExcluirConfirm,
			'urlAcao': FiscalizacaoListar.urlExcluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});
	}
}
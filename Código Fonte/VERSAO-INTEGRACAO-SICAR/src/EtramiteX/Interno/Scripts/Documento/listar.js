/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

DocumentoListar = {
	urlEditar: '',
	urlExcluir: '',
	urlExcluirConfirm: '',
	urlVisualizar: '',
	urlAtividadesSolicitadas: '',
	urlValidarPossuiRequerimentoAtividades: '',
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(DocumentoListar.settings, options); }
		DocumentoListar.container = MasterPage.getContent(container);
		DocumentoListar.container.listarAjax({ onBeforeFiltrar: DocumentoListar.onBeforeFiltrar });

		DocumentoListar.container.delegate('.btnExcluir', 'click', DocumentoListar.excluir);
		DocumentoListar.container.delegate('.btnVisualizar', 'click', DocumentoListar.visualizar);
		DocumentoListar.container.delegate('.btnAssociar', 'click', DocumentoListar.associar);
		DocumentoListar.container.delegate('.btnAtividadesSolicitadas', 'click', DocumentoListar.atividadesSolicitadas);
		DocumentoListar.container.delegate('.btnConsultar', 'click', DocumentoListar.consultarInformacoes);
		DocumentoListar.container.delegate('.btnEditar', 'click', DocumentoListar.editar);

		DocumentoListar.container.delegate('.radioInteressadoCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioInteressadoCpfCnpj', DocumentoListar.container));
		Aux.setarFoco(container);

		if (DocumentoListar.settings.associarFuncao) {
			$('.hdnIsAssociar', DocumentoListar.container).val(true);
		}
	},

	onBeforeFiltrar: function (container, serializedData) {
		serializedData.Filtros.EmpreendimentoCodigo = Mascara.getIntMask($(".txtEmpreendimentoCodigo", DocumentoListar.container).val()).toString();
	},

	editar: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		MasterPage.redireciona(DocumentoListar.urlEditar + '?id=' + itemId);
	},

	visualizar: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;

		if (DocumentoListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', DocumentoListar.container).val() + "/" + itemId, null, function (context) {
				Modal.defaultButtons(context);
			}, Modal.tamanhoModalGrande);
		} else {
			MasterPage.redireciona($('.urlVisualizar', DocumentoListar.container).val() + "/" + itemId);
		}
	},

	associar: function () {

		var objeto = $.parseJSON($(this).closest('tr').find('.itemJson').val());

		var retorno = DocumentoListar.settings.associarFuncao(objeto);

		if (retorno === true) {
			Modal.fechar(DocumentoListar.container);
		} else {
			Mensagem.gerar(DocumentoListar.container, retorno);
		}
	},

	excluir: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;

		Modal.excluir({
			'urlConfirm': DocumentoListar.urlExcluirConfirm,
			'urlAcao': DocumentoListar.urlExcluir,
			'id': itemId,
			'btnExcluir': this
		});
	},

	atividadesSolicitadas: function () {
		var id = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		var retorno = MasterPage.validarAjax(DocumentoListar.urlValidarPossuiRequerimentoAtividades, { id: id }, DocumentoListar.container, false);
		if (!retorno.EhValido) {
			return;
		}

		MasterPage.redireciona(DocumentoListar.urlAtividadesSolicitadas + '?id=' + id + '&isProcesso=false');
	},

	consultarInformacoes: function () {
		var id = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		MasterPage.redireciona(DocumentoListar.urlConsultarInformacoes + '/' + id);
	}
}
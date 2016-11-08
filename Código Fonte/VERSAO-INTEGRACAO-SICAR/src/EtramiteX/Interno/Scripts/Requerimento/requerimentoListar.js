/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

RequerimentoListar = {
	urlEditar: '',
	urlEditarValidar: '',
	urlExcluir: '',
	urlImportar: '',
	ExcluirConfirm: null,
	urlVisualizarPdf: '',
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(RequerimentoListar.settings, options); }
		RequerimentoListar.container = MasterPage.getContent(container);
		RequerimentoListar.container.listarAjax();

		RequerimentoListar.container.delegate('.btnExcluir', 'click', RequerimentoListar.excluir);
		RequerimentoListar.container.delegate('.btnVisualizar', 'click', RequerimentoListar.visualizar);
		RequerimentoListar.container.delegate('.btnAssociar', 'click', RequerimentoListar.associar);
		RequerimentoListar.container.delegate('.btnPdf', 'click', RequerimentoListar.pdfAbrir);
		RequerimentoListar.container.delegate('.btnEditar', 'click', RequerimentoListar.editar);
		RequerimentoListar.container.delegate('.btnImportar', 'click', RequerimentoListar.importar);

		RequerimentoListar.container.delegate('.radioInteressadoCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioInteressadoCpfCnpj', RequerimentoListar.container));
		Aux.setarFoco(container);

		if (RequerimentoListar.settings.associarFuncao) {
			$('.hdnIsAssociar', container).val(true);
		}
	},

	pdfAbrir: function () {
		var url = $('.urlVisualizarPdf', RequerimentoListar.container).val();
		MasterPage.redireciona(url + "?id=" + $(this).closest('tr').find('.itemId:first').val());
		MasterPage.carregando(false);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		var retorno = MasterPage.validarAjax(RequerimentoListar.urlEditarValidar, { id: itemId }, RequerimentoListar.container, false);

		if (retorno.EhValido) {
			MasterPage.redireciona(RequerimentoListar.urlEditar + '/' + itemId);
		}
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona($('.urlVisualizar', RequerimentoListar.container).val() + "/" + itemId);
	},

	associar: function () {
		var tr = $(this).closest('tr');
		var itemId = parseInt($('.itemId:first', tr).val());
		var itemSituacao = parseInt($('.itemSituacao:first', tr).val());
		var itemDataCriacao = $('.itemDataCriacao:first', tr).val();
		var itemEmpreendimento = $('.itemEmpreendimento:first', tr).val();
		var itemEmpreendimentoId = $('.itemEmpreendimentoId:first', tr).val();

		var retorno = RequerimentoListar.settings.associarFuncao({ Id: itemId, SituacaoId: itemSituacao, DataCriacao: itemDataCriacao, EmpreendimentoId: itemEmpreendimentoId, EmpreendimentoDenominador: itemEmpreendimento });

		if (retorno === true) {
			Modal.fechar(RequerimentoListar.container);
		} else {
			Mensagem.gerar(RequerimentoListar.container, retorno);
		}
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': RequerimentoListar.ExcluirConfirm,
			'urlAcao': RequerimentoListar.urlExcluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	importar: function () {
		var objeto = RequerimentoListar.obter(this);
		MasterPage.redireciona(RequerimentoListar.urlImportar + '/' + objeto.Id);
	}
}
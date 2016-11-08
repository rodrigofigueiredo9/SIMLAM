/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ProcessoListar = {
	urlEditar: '',
	urlValidarEditar: '',
	urlEditarApensadosJuntados: '',
	urlExcluirConfirm: '',
	urlExcluir: '',
	urlExisteProcessoAtividade: null,
	urlAtividadesSolicitadas: '',
	urlValidarEditarApensadosJuntados: '',
	urlConsultarInformacoes: '',
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(ProcessoListar.settings, options); }
		ProcessoListar.container = MasterPage.getContent(container);
		ProcessoListar.container.listarAjax({ onBeforeFiltrar: ProcessoListar.onBeforeFiltrar });

		ProcessoListar.container.delegate('.btnExcluir', 'click', ProcessoListar.excluir);
		ProcessoListar.container.delegate('.btnEditar', 'click', ProcessoListar.editar);
		ProcessoListar.container.delegate('.btnVisualizar', 'click', ProcessoListar.visualizar);
		ProcessoListar.container.delegate('.btnAssociar', 'click', ProcessoListar.associar);
		ProcessoListar.container.delegate('.btnAtividadesSolicitadas', 'click', ProcessoListar.atividadesSolicitadas);
		ProcessoListar.container.delegate('.btnEditarApensadosJuntados', 'click', ProcessoListar.editarApensadosJuntados);
		ProcessoListar.container.delegate('.btnConsultar', 'click', ProcessoListar.consultarInformacoes);

		ProcessoListar.container.delegate('.radioInteressadoCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioInteressadoCpfCnpj', ProcessoListar.container));
		Aux.setarFoco(container);

		if (ProcessoListar.settings.associarFuncao) {
			$('.hdnIsAssociar', ProcessoListar.container).val(true);
		}
	},

	onBeforeFiltrar: function (container, serializedData) {
		serializedData.Filtros.EmpreendimentoCodigo = Mascara.getIntMask($(".txtEmpreendimentoCodigo", ProcessoListar.container).val()).toString();
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		var retorno = MasterPage.validarAjax(ProcessoListar.urlValidarEditar, { processoId: itemId }, ProcessoListar.container, false);

		if (!retorno.EhValido) {
			return;
		}

		MasterPage.redireciona(ProcessoListar.urlEditar + '/' + itemId);
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		if (ProcessoListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', ProcessoListar.container).val() + '/' + itemId, null, function (context) {
				Modal.defaultButtons(context);
			}, Modal.tamanhoModalGrande);
		} else {
			MasterPage.redireciona($('.urlVisualizar', ProcessoListar.container).val() + '/' + itemId);
		}
	},

	associar: function () {
		var modal = ProcessoListar.container;
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		var itemNumero = $(this).closest('tr').find('.itemNumeroId:first').val();
		var itemSituacaoId = parseInt($(this).closest('tr').find('.itemSituacaoId:first').val());

		var retorno = ProcessoListar.settings.associarFuncao({ Id: itemId, Numero: itemNumero, SituacaoId: itemSituacaoId });

		if (retorno !== undefined && retorno.length > 0) {
			Mensagem.gerar(MasterPage.getContent(modal), retorno);
		} else {
			Modal.fechar(modal);
		}
	},

	editarApensadosJuntados: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		var retorno = MasterPage.validarAjax(ProcessoListar.urlValidarEditarApensadosJuntados, { processo: itemId }, ProcessoListar.container, false);

		if (!retorno.EhValido) {
			return;
		}
		MasterPage.redireciona(ProcessoListar.urlEditarApensadosJuntados + '/' + itemId);
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': ProcessoListar.urlExcluirConfirm,
			'urlAcao': ProcessoListar.urlExcluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});
	},

	atividadesSolicitadas: function () {
		var id = parseInt($(this).closest('tr').find('.itemId:first').val());

		if (!MasterPage.validarAjax(ProcessoListar.urlExisteProcessoAtividade, { id: id }, ProcessoListar.container, false).EhValido) {
			return;
		}

		MasterPage.redireciona(ProcessoListar.urlAtividadesSolicitadas + '?id=' + id + '&isProcesso=true');
	},

	consultarInformacoes: function () {
		var id = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(ProcessoListar.urlConsultarInformacoes + '/' + id);
	}
}
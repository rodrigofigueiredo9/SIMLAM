/// <reference path="Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ProtocoloListar = {
	urlEditar: '',
	urlValidarEditar: '',
	urlEditarApensadosJuntados: '',
	urlExcluirConfirm: '',
	urlExcluir: '',
	urlExisteProcessoAtividade: null,
	urlAtividadesSolicitadas: '',
	urlHistoricoTramitacao: '',
	urlValidarEditarApensadosJuntados: '',
	urlConsultarInformacoes: '',
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(ProtocoloListar.settings, options); }
		ProtocoloListar.container = MasterPage.getContent(container);
		ProtocoloListar.container.listarAjax();
		
		ProtocoloListar.container.delegate('.btnVisualizar', 'click', ProtocoloListar.visualizar);
		ProtocoloListar.container.delegate('.btnAtividadesSolicitadas', 'click', ProtocoloListar.atividadesSolicitadas);
		ProtocoloListar.container.delegate('.btnHistoricoTramitacao', 'click', ProtocoloListar.historicoTramitacao);
		ProtocoloListar.container.delegate('.btnSep', 'click', ProtocoloListar.consultarSep);
		

		ProtocoloListar.container.delegate('.radioInteressadoCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioInteressadoCpfCnpj', ProtocoloListar.container));
		Aux.setarFoco(container);

		if (ProtocoloListar.settings.associarFuncao) {
			$('.hdnIsAssociar', ProtocoloListar.container).val(true);
		}
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		if (ProtocoloListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', ProtocoloListar.container).val() + '/' + itemId, null, function (context) {
				Modal.defaultButtons(context);
			}, Modal.tamanhoModalGrande);
		} else {
			MasterPage.redireciona($('.urlVisualizar', ProtocoloListar.container).val() + '/' + itemId);
		}
	},

	atividadesSolicitadas: function () {
		var id = parseInt($(this).closest('tr').find('.itemId:first').val());
		var isProcesso = $(this).closest('tr').find('.itemIsProcesso:first').val();

		if (!MasterPage.validarAjax(ProtocoloListar.urlExisteProtocoloAtividade, { id: id }, ProtocoloListar.container, false).EhValido) {
			return;
		}

		MasterPage.redireciona(ProtocoloListar.urlAtividadesSolicitadas + '?id=' + id + '&isProcesso=' + isProcesso);
	},

	historicoTramitacao: function () {
		var id = parseInt($(this).closest('tr').find('.itemId:first').val());

		Modal.abrir(ProtocoloListar.urlHistoricoTramitacao, { id: id, tipo: 1 }, function (context) {
			Modal.defaultButtons(context);
		});
	},

	consultarSep: function () {
		var numero_autuacao = parseInt($(this).closest('tr').find('.itemNumeroAutuacao:first').val());
		window.open('https://sistemas.es.gov.br/prodest/consultaprocesso/ConsultaProcesso/Consultar?id=' + numero_autuacao);
	}
}
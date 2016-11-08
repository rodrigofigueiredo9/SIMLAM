/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../jquery.listar-ajax.js" />
/// <reference path="../masterpage.js" />

ProjetoDigitalListar = {
	urlOperar: null,
	urlConfirmarExcluir: null,
	urlExcluir: null,
	urlConfirmarCancelarEnvio: null,
	urlCancelarEnvio: null,
	urlDocumentosGerados: null,
	urlNotificacaoCorrecao: null,
	container: null,
	settings: {
		associarFuncao: null,
		urlVisualizarRequerimento: null
	},

	load: function (container, options) {
		if (options) { $.extend(ProjetoDigitalListar.settings, options); }

		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnOperar', 'click', ProjetoDigitalListar.operar);
		container.delegate('.btnExcluir', 'click', ProjetoDigitalListar.excluir);
		container.delegate('.btnCancelarEnvio', 'click', ProjetoDigitalListar.cancelarEnvio);
		container.delegate('.btnDocumentosGerados', 'click', ProjetoDigitalListar.documentosGerados);
		container.delegate('.btnNotificacaoCorrecao', 'click', ProjetoDigitalListar.notificacaoCorrecao);
		container.delegate('.btnAssociar', 'click', ProjetoDigitalListar.associar);
		container.delegate('.btnVisualizarRequerimento', 'click', ProjetoDigitalListar.onVisualizarRequerimento);

		container.delegate('.radioInteressadoCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioInteressadoCpfCnpj', container));

		Aux.setarFoco(container);
		ProjetoDigitalListar.container = container;

		if (ProjetoDigitalListar.settings.associarFuncao) {
			$('.hdnIsAssociar', ProjetoDigitalListar.container).val(true);
		}
	},

	onVisualizarRequerimento: function () {
		var item = JSON.parse($(this).closest('tr').find('.itemJson').val());

		Modal.abrir(ProjetoDigitalListar.settings.urlVisualizarRequerimento, { id: item.RequerimentoId, projetoDigitalId: item.Id, isVisualizar: 'True' }, function (context) {
			Modal.defaultButtons(context);
			RequerimentoVis.mostrarBtnEditar = false;
		});
	},

	associar: function () {
		var modal = ProjetoDigitalListar.container;
		var objeto = $(this).closest('tr').find('.itemJson').val();

		var retorno = ProjetoDigitalListar.settings.associarFuncao(objeto);

		if (retorno !== undefined && retorno.length > 0) {
			Mensagem.gerar(MasterPage.getContent(modal), retorno);
		} else {
			Modal.fechar(modal);
		}
	},

	obterItemJson: function (container) {
		return $.parseJSON($(container).closest('tr').find('.itemJson').val());
	},

	operar: function () {
		MasterPage.redireciona(ProjetoDigitalListar.urlOperar + '/' + ProjetoDigitalListar.obterItemJson(this).Id);
	},

	excluir: function () {
		Mensagem.limpar(ProjetoDigitalListar.container);
		Modal.excluir({
			'urlConfirm': ProjetoDigitalListar.urlConfirmarExcluir,
			'urlAcao': ProjetoDigitalListar.urlExcluir,
			'id': ProjetoDigitalListar.obterItemJson(this).Id,
			'btnExcluir': this
		});
	},

	cancelarEnvio: function () {
		Mensagem.limpar(ProjetoDigitalListar.container);
		Modal.excluir({
			'urlConfirm': ProjetoDigitalListar.urlConfirmarCancelarEnvio,
			'urlAcao': ProjetoDigitalListar.urlCancelarEnvio,
			'id': ProjetoDigitalListar.obterItemJson(this).Id,
			'btnExcluir': this,
			'btnTexto': 'Confirmar'
		});
	},

	documentosGerados: function () {
		var id = ProjetoDigitalListar.obterItemJson(this).Id;

		Modal.abrir(
			ProjetoDigitalListar.urlDocumentosGerados + "/" + id,
			null, function (modalContent) { Modal.defaultButtons(modalContent); }, Modal.tamanhoModalMedia);
	},

	notificacaoCorrecao: function () {
		Modal.abrir(
			ProjetoDigitalListar.urlNotificacaoCorrecao + "/" + ProjetoDigitalListar.obterItemJson(this).Id,
			null, function (container) { Modal.defaultButtons(container); }, Modal.tamanhoModalMedia);
	}
}
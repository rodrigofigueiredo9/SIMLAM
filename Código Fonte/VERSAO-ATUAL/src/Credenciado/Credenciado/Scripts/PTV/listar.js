/// <reference path="../masterpage.js" />
/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />

EPTVListar = {
	settings: {
		urls: {
			urlHistorico: null,
			urlVisualizar: null,
			urlEditar: null,
			urlExcluirConfirm: null,
			urlExcluir: null,
			urlPDF: null,
			urlEnviarConfirm: null,
			urlEnviar: null,
			urlValidarAcessoComunicador: null,
			urlComunicadorPTV: null
		}
	},

	container: null,

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(EPTVListar.settings, options); }

		container.listarAjax();
		container.delegate('.btnVisualizar', 'click', EPTVListar.visualizar)
		container.delegate('.btnEditar', 'click', EPTVListar.editar);
		container.delegate('.btnExcluir', 'click', EPTVListar.excluir);
		container.delegate('.btnPDF', 'click', EPTVListar.gerarPDF);
		container.delegate('.btnEnviar', 'click', EPTVListar.enviar);
		container.delegate('.btnHistorico', 'click', EPTVListar.historico);
		container.delegate('.btnSolicitarDesbloqueio', 'click', EPTVListar.comunicador);
		container.delegate('.ddlTipoDocumento', 'change', EPTVListar.onChangeTipoDocumento);		

		container.delegate('.radioCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioCpfCnpj', container));
		Aux.setarFoco(container);

		EPTVListar.container = container;
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	historico: function (item, listarSucesso) {
		Mensagem.limpar(EPTVListar.container);

		var item = EPTVListar.obter(this);
		Modal.abrir(EPTVListar.settings.urls.urlHistorico + '/' + item.Id, null, function (container) {
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);
	},

	editar: function () {
		var objeto = EPTVListar.obter(this);
		MasterPage.redireciona(EPTVListar.settings.urls.urlEditar + '/' + objeto.Id);
	},

	excluir: function () {
		var objeto = EPTVListar.obter(this);
		Modal.excluir({
			'urlConfirm': EPTVListar.settings.urls.urlExcluirConfirm,
			'urlAcao': EPTVListar.settings.urls.urlExcluir,
			'id': objeto.Id,
			'btnExcluir': this
		});
	},

	visualizar: function () {
		var objeto = EPTVListar.obter(this);
		MasterPage.redireciona(EPTVListar.settings.urls.urlVisualizar + '/' + objeto.Id);
	},

	gerarPDF: function () {
		var item = EPTVListar.obter(this);
		MasterPage.redireciona(EPTVListar.settings.urls.urlPDF + '/' + item.Id);
	},

	enviar: function () {
		EPTVListar.enviarItem(EPTVListar.obter(this), true);
	},

	enviarItem: function (item, listarSucesso) {
		Mensagem.limpar(EPTVListar.container);

		Modal.abrir(EPTVListar.settings.urls.urlEnviarConfirm + '/' + item.Id, null, function (container) {
			Modal.defaultButtons(container, function (container) {

				MasterPage.carregando(true);
				$.ajax({
					url: EPTVListar.settings.urls.urlEnviar,
					data: JSON.stringify(item),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: Aux.error,
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.EhValido) {
							if (listarSucesso) {
								EPTVListar.container.listarAjax('ultimaBusca');
							}
							Modal.fechar(container);
						}

						if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(container, response.Msg);
						}
					}
				});
				MasterPage.carregando(false);
			}, 'Enviar');
		});
	},

	comunicador: function () {
		var item = EPTVListar.obter(this);

		if (!MasterPage.validarAjax(EPTVListar.settings.urls.urlValidarAcessoComunicador + '/' + item.Id, null, EPTVListar.container, false).EhValido) {
			return;
		}

		Modal.abrir(
			EPTVListar.settings.urls.urlComunicadorPTV,
			{ id: item.Id },
			function (container) {
				ComunicadorPTV.load(container, {
					callBackSalvar: EPTVListar.comunicador
				});
			},
			Modal.tamanhoModalMedia);
	},
			
	onChangeTipoDocumento: function () {
		debugger;
		if ($(this).val() > 0)
			$('.txtNumeroDocumento', EPTVListar.container).toggleClass('hide', false);
		else
			$('.txtNumeroDocumento', EPTVListar.container).toggleClass('hide', true);
	}
}
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

TituloListar = {
	urlEditar: null,
	urlVisualizar: null,
	urlExcluir: null,
	urlExcluirConfirm: null,
	urlAlterarSituacao: null,
	urlCondicionanteSituacaoAlterar: null,
	urlValidarPossuiCondicionantes: null,
	urlValidarAlterarAutorSetor: null,
	urlValidarAlterarSituacao: null,
	urlAlterarAutorSetor: null,
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(TituloListar.settings, options); }
		TituloListar.container = MasterPage.getContent(container);
		TituloListar.container.listarAjax({ onBeforeFiltrar: TituloListar.onBeforeFiltrar });

		TituloListar.container.delegate('.btnEditar', 'click', TituloListar.editar);
		TituloListar.container.delegate('.btnAlterarSituacao', 'click', TituloListar.onBtnAlterarSituacaoClick);
		TituloListar.container.delegate('.btnExcluir', 'click', TituloListar.excluir);
		TituloListar.container.delegate('.btnVisualizar', 'click', TituloListar.visualizar);
		TituloListar.container.delegate('.btnAssociar', 'click', TituloListar.onAssociarTitulo);
		TituloListar.container.delegate('.btnAlterarSituacaoCondicionante', 'click', TituloListar.onBtnAlterarSituacaoCondicionanteClick);
		TituloListar.container.delegate('.btnPDF', 'click', TituloListar.gerarPdf);

		Aux.setarFoco(TituloListar.container);

		if (TituloListar.settings.associarFuncao) {
			$('.hdnIsAssociar', TituloListar.container).val(true);
		}
	},

	onBeforeFiltrar: function (container, serializedData) {
		serializedData.Filtros.EmpreendimentoCodigo = Mascara.getIntMask($(".txtEmpreendimentoCodigo", TituloListar.container).val()).toString();
	},

	onBtnAlterarSituacaoClick: function () {
		var tituloId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		var retorno = MasterPage.validarAjax(TituloListar.urlValidarAlterarSituacao, { id: tituloId }, TituloListar.container, false);

		if (retorno.EhValido) {
			MasterPage.redireciona(TituloListar.urlAlterarSituacao + "/" + tituloId);
		}
	},

	editar: function () {
		var id = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		var redirecionar = false;

		$.ajax({ url: TituloListar.urlValidarAlterarAutorSetor, data: { id: id }, cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, TituloListar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length) {
					Mensagem.gerar(TituloListar.container, response.Msg);
				} else {
					redirecionar = !response.AbrirTela;

					if (response.AbrirTela) {
						Modal.confirma({
							url: TituloListar.urlAlterarAutorSetor,
							urlData: { id: id, trocarAutor: response.TrocarAutor, trocarSetor: response.TrocarSetor },
							btnOkLabel: "Sim",
							btCancelLabel: "Não",
							btnOkCallback: function (container) {
								var trocarAutor = $(container).find('.fsAutor').length > 0;
								var setorTrocado = $(container).find('.fsSetor').length > 0 ? $(container).find('.ddlSetores  :selected').val() : 0;
								MasterPage.redireciona(TituloListar.urlEditar + "/" + id + "?trocarAutor=" + trocarAutor + "&setorTrocado=" + setorTrocado);
							},
							tamanhoModal: Modal.tamanhoModalGrande
						});
					}
				}
			}
		});

		if (redirecionar) {
			MasterPage.redireciona(TituloListar.urlEditar + "/" + id);
		}
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

	onBtnAlterarSituacaoCondicionanteClick: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		var retorno = MasterPage.validarAjax(TituloListar.urlValidarPossuiCondicionantes, { id: itemId }, TituloListar.container, false);

		if (!retorno.EhValido) {
			return;
		}

		Modal.abrir(TituloListar.urlCondicionanteSituacaoAlterar + '/' + itemId, null, function (container) {
			CondicionanteSituacaoAlterar.load(container, {
				onSalvar: TituloListar.onCondicionanteSituacaoAlterar
			});
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
}
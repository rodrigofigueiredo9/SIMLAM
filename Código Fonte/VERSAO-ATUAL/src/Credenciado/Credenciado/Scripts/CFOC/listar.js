/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../jquery.listar-ajax.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../mensagem.js" />

CFOCListar = {
	urlVisualizar: null,
	urlEditar: null,
	urlConfirmarExcluir: null,
	urlExcluir: null,
	urlConfirmarAtivar: null,
	urlAtivar: null,
	urlPDF: null,
	idsTela: null,
	mensagens: null,
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(CFOCListar.settings, options); }

		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnVisualizar', 'click', CFOCListar.visualizar);
		container.delegate('.btnEditar', 'click', CFOCListar.editar);
		container.delegate('.btnPDF', 'click', CFOCListar.gerarPDF);
		container.delegate('.btnExcluir', 'click', CFOCListar.excluir);
		container.delegate('.btnAtivar', 'click', CFOCListar.ativar);

		Aux.setarFoco(container);
		CFOCListar.container = container;
	},

	obterItemJson: function (container) {
		return $.parseJSON($(container).closest('tr').find('.itemJson').val());
	},

	visualizar: function () {
		var item = CFOCListar.obterItemJson(this);
		MasterPage.redireciona(CFOCListar.urlVisualizar + '/' + item.Id);
	},

	editar: function () {
		var item = CFOCListar.obterItemJson(this);
		MasterPage.redireciona(CFOCListar.urlEditar + '/' + item.Id);
	},

	gerarPDFLoad: function (id) {
		setTimeout(function () {
			MasterPage.redireciona(CFOCListar.urlPDF + '/' + id);
			MasterPage.carregando(false);
		}, 100);
	},

	gerarPDF: function () {
		var item = CFOCListar.obterItemJson(this);
		MasterPage.redireciona(CFOCListar.urlPDF + '/' + item.Id);
	},

	excluir: function () {
		Mensagem.limpar(CFOCListar.container);
		Modal.excluir({
			'urlConfirm': CFOCListar.urlConfirmarExcluir,
			'urlAcao': CFOCListar.urlExcluir,
			'id': CFOCListar.obterItemJson(this).Id,
			'btnExcluir': this
		});
	},

	ativar: function () {
		CFOCListar.ativarItem(CFOCListar.obterItemJson(this), true);
	},

	ativarItem: function (item, listarSucesso) {

		Mensagem.limpar(CFOCListar.container);
		if (item.SituacaoId != CFOCListar.idsTela.EmElaboracao) {
			Mensagem.gerar(CFOCListar.container, [CFOCListar.mensagens.AtivarSituacaoInvalida]);
			return;
		}

		Modal.abrir(CFOCListar.urlConfirmarAtivar + '/' + item.Id, null, function (container) {
			Modal.defaultButtons(container, function (container) {
				var objeto = item;

				objeto['Numero'] = $('.hdnEmissaoNumero', container).val();
				objeto['TipoNumero'] = $('.hdnEmissaoTipoNumero', container).val();
				objeto.DataAtivacao = DataAtivacao = { DataTexto: $('.txtDataAtivacao', container).val() };

				MasterPage.carregando(true);
				$.ajax({
					url: CFOCListar.urlAtivar,
					data: JSON.stringify(objeto),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: Aux.error,
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.EhValido) {
							if (listarSucesso) {
								CFOCListar.container.listarAjax('ultimaBusca');
							}

							Modal.fechar(container);
							MasterPage.redireciona(CFOCListar.urlPDF + '/' + response.Id);
						}

						if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(container, response.Msg);
						}
					}
				});
				MasterPage.carregando(false);
			}, 'Ativar');
		});
	}
}
/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../jquery.listar-ajax.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../mensagem.js" />

CFOListar = {
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
		if (options) { $.extend(CFOListar.settings, options); }

		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnVisualizar', 'click', CFOListar.visualizar);
		container.delegate('.btnEditar', 'click', CFOListar.editar);
		container.delegate('.btnPDF', 'click', CFOListar.gerarPDF);
		container.delegate('.btnExcluir', 'click', CFOListar.excluir);
		container.delegate('.btnAtivar', 'click', CFOListar.ativar);

		Aux.setarFoco(container);
		CFOListar.container = container;
	},

	obterItemJson: function (container) {
		return $.parseJSON($(container).closest('tr').find('.itemJson').val());
	},

	visualizar: function () {
		var item = CFOListar.obterItemJson(this);
		MasterPage.redireciona(CFOListar.urlVisualizar + '/' + item.Id);
	},

	editar: function () {
		var item = CFOListar.obterItemJson(this);
		MasterPage.redireciona(CFOListar.urlEditar + '/' + item.Id);
	},

	gerarPDFLoad: function (id) {
		setTimeout(function () {
			MasterPage.redireciona(CFOListar.urlPDF + '/' + id);
			MasterPage.carregando(false);
		}, 100);
	},

	gerarPDF: function () {
		var item = CFOListar.obterItemJson(this);
		MasterPage.redireciona(CFOListar.urlPDF + '/' + item.Id);
	},

	excluir: function () {
		Mensagem.limpar(CFOListar.container);
		Modal.excluir({
			'urlConfirm': CFOListar.urlConfirmarExcluir,
			'urlAcao': CFOListar.urlExcluir,
			'id': CFOListar.obterItemJson(this).Id,
			'btnExcluir': this
		});
	},

	ativar: function () {
		CFOListar.ativarItem(CFOListar.obterItemJson(this), true);
	},

	ativarItem: function (item, listarSucesso) {
		Mensagem.limpar(CFOListar.container);

		if (item.SituacaoId != CFOListar.idsTela.EmElaboracao) {
			Mensagem.gerar(CFOListar.container, [CFOListar.mensagens.AtivarSituacaoInvalida]);
			return;
		}

		Modal.abrir(CFOListar.urlConfirmarAtivar + '/' + item.Id, null, function (container) {
			Modal.defaultButtons(container, function (container) {
				var objeto = item;
				
				objeto['Numero'] = $('.hdnEmissaoNumero', container).val();
				objeto['TipoNumero'] = $('.hdnEmissaoTipoNumero', container).val();
				objeto.DataAtivacao =  DataAtivacao = { DataTexto: $('.txtDataAtivacao', container).val() };

				MasterPage.carregando(true);
				$.ajax({
					url: CFOListar.urlAtivar,
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
								CFOListar.container.listarAjax('ultimaBusca');
							}

							Modal.fechar(container);
							MasterPage.redireciona(CFOListar.urlPDF + '/' + response.Id);
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
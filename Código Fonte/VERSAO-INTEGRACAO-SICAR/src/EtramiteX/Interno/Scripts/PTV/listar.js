/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
PTVListar = {
	settings: {
		urls:{
			urlEditar: null,
			urlExcluir: null,
			urlExcluirConfirm: null,
			urlVisualizar: null,
			urlConfirmarAtivar: null,
			urlAtivar: null,
			urlPDF: null,
			urlCancelar:null
		}
	},
	
	container: null,

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(PTVListar.settings, options); }

		container.listarAjax();
		container.delegate('.btnEditar', 'click', PTVListar.editar);
		container.delegate('.btnVisualizar', 'click', PTVListar.visualizar)
		container.delegate('.btnPDF', 'click', PTVListar.gerarPDF);
		container.delegate('.btnExcluir', 'click', PTVListar.excluir);
		container.delegate('.btnAtivar', 'click', PTVListar.ativar);
		container.delegate('.btnCancelar', 'click', PTVListar.cancelar);
		
		Aux.setarFoco(container);
		PTVListar.container = container;
	},

	obter: function (container) {		
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	editar: function () {
		var objeto = PTVListar.obter(this);
		MasterPage.redireciona(PTVListar.settings.urls.urlEditar + '/' + objeto.Id);
	},

	excluir: function () {
		var objeto = PTVListar.obter(this);
		Modal.excluir({
			'urlConfirm': PTVListar.settings.urls.urlExcluirConfirm,
			'urlAcao': PTVListar.settings.urls.urlExcluir,
			'id': objeto.Id,
			'btnExcluir': this
		});
	},

	visualizar: function () {		
		var objeto = PTVListar.obter(this);
		MasterPage.redireciona(PTVListar.settings.urls.urlVisualizar + '/' + objeto.Id);
	},
	
	gerarPDFLoad: function (id) {
		setTimeout(function () {
			MasterPage.redireciona(PTVListar.settings.urls.urlPDF + '/' + id);
			MasterPage.carregando(false);
		}, 100);
	},

	gerarPDF: function () {
		var item = PTVListar.obter(this);
		MasterPage.redireciona(PTVListar.settings.urls.urlPDF + '/' + item.Id);
	},

	ativar: function () {
		PTVListar.ativarItem(PTVListar.obter(this), true);
	},

	ativarItem: function (item, listarSucesso) {
		Mensagem.limpar(PTVListar.container);

		Modal.abrir(PTVListar.settings.urls.urlConfirmarAtivar + '/' + item.Id, null, function (container) {
			Modal.defaultButtons(container, function (container) {
				var objeto = item;

				objeto['Numero'] = $('.hdnNumero', container).val();
				objeto['TipoNumero'] = $('.hdnNumeroTipo', container).val();
				objeto.DataAtivacao =  DataAtivacao = { DataTexto: $('.txtDataAtivacao', container).val() };

				MasterPage.carregando(true);
				$.ajax({
					url: PTVListar.settings.urls.urlAtivar,
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
								Modal.fechar(container);
								PTVListar.container.listarAjax('ultimaBusca');
								MasterPage.redireciona(PTVListar.settings.urls.urlPDF + '/' + response.Id);
							} else {
								MasterPage.redireciona(response.Url);
							}
						}

						if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(container, response.Msg);
						}
					}
				});
				MasterPage.carregando(false);
			}, 'Ativar');
		});
	},

	cancelar: function () {
		var item = PTVListar.obter(this);

		Modal.abrir(PTVListar.settings.urls.urlConfirmCancel + '/' + item.Id, null, function (container) {
			Modal.defaultButtons(container, function (container) {
				var objeto = item;
				objeto.DataCancelamento = DataCancelamento = { DataTexto: $('.txtDataCancelamento', container).val() };

				MasterPage.carregando(true);
				$.ajax({
					url: PTVListar.settings.urls.urlCancelar ,
					data: JSON.stringify(objeto),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: Aux.error,
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.EhValido) {
							PTVListar.container.listarAjax('ultimaBusca');
							Modal.fechar(container);
						}

						if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(container, response.Msg);
						}
					}
				});
				MasterPage.carregando(false);
			}, 'Confirmar');
		});
	}
}
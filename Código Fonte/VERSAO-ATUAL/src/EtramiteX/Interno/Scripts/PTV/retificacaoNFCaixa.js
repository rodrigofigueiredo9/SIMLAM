/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
NFCaixa = {
	settings: {
		urls:{
			urlEditar: null,
			urlExcluir: null,
			urlExcluirConfirm: null,
			urlVisualizar: null,
			urlConfirmarAtivar: null,
			urlAtivar: null,
			urlPDF: null,
			urlCancelar: null,
			urlPTVNFCaixaPag: null
		}
	},
	
	container: null,

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(NFCaixa.settings, options); }

		container.listarAjax();
		container.delegate('.btnEditar', 'click', NFCaixa.editar);
		//container.delegate('.btnVisualizar', 'click', NFCaixa.visualizar)
		//container.delegate('.btnPDF', 'click', NFCaixa.gerarPDF);
		container.delegate('.btnExcluir', 'click', NFCaixa.excluir);
		//container.delegate('.btnAtivar', 'click', NFCaixa.ativar);
		//container.delegate('.btnCancelar', 'click', NFCaixa.cancelar);
		//container.delegate('.ddlTipoDocumento', 'change', NFCaixa.onChangeTipoDocumento);
		
		Aux.setarFoco(container);
		NFCaixa.container = container;
	},

	obter: function (container) {		
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	//editar: function () {
	//	var objeto = NFCaixa.obter(this);
	//	MasterPage.redireciona(NFCaixa.settings.urls.urlEditar + '/' + objeto.Id);
	//},

	excluir: function () {
		var objeto = NFCaixa.obter(this);
		Modal.excluir({
			'urlConfirm': NFCaixa.settings.urls.urlExcluirConfirm,
			'urlAcao': NFCaixa.settings.urls.urlExcluir,
			'id': objeto.id,
			'tamanho': Modal.tamanhoModalPequena,
			'btnExcluir': this
		});
	},

	editar: function () {
		Mensagem.limpar(NFCaixa.container);

		var item = NFCaixa.obter(this);
		Modal.abrir(NFCaixa.settings.urls.urlEditar + '/' + item.id, null, function (container) {
			Modal.defaultButtons(container, function (container) {
				
				var objeto = {
					id: $('.hdnNFCaixaId').val(),
					novoSaldo: $('.novoSaldo').val()
				};

				MasterPage.carregando(true);
				$.ajax({
					url: NFCaixa.settings.urls.urlSalvar,
					data: JSON.stringify(objeto),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: Aux.error,
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.EhValido) {
							Modal.fechar(container);
							NFCaixa.container.listarAjax('ultimaBusca');
						}

						if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(container, response.Msg);
						}
					}
				});
				MasterPage.carregando(false);
			}, 'Salvar');
		});
	},

	//visualizar: function () {		
	//	var objeto = NFCaixa.obter(this);
	//	MasterPage.redireciona(NFCaixa.settings.urls.urlVisualizar + '/' + objeto.Id);
	//},
	
	//gerarPDFLoad: function (id) {
	//	setTimeout(function () {
	//		MasterPage.redireciona(NFCaixa.settings.urls.urlPDF + '/' + id);
	//		MasterPage.carregando(false);
	//	}, 100);
	//},

	//gerarPDF: function () {
	//	var item = NFCaixa.obter(this);
	//	MasterPage.redireciona(NFCaixa.settings.urls.urlPDF + '/' + item.Id);
	//},

	//ativar: function () {
	//	NFCaixa.ativarItem(NFCaixa.obter(this), true);
	//},

	//ativarItem: function (item, listarSucesso) {
	//	Mensagem.limpar(NFCaixa.container);

	//	Modal.abrir(NFCaixa.settings.urls.urlConfirmarAtivar + '/' + item.Id, null, function (container) {
	//		Modal.defaultButtons(container, function (container) {
	//			var objeto = item;

	//			objeto['Numero'] = $('.hdnNumero', container).val();
	//			objeto['TipoNumero'] = $('.hdnNumeroTipo', container).val();
	//			objeto.DataAtivacao =  DataAtivacao = { DataTexto: $('.txtDataAtivacao', container).val() };

	//			MasterPage.carregando(true);
	//			$.ajax({
	//				url: NFCaixa.settings.urls.urlAtivar,
	//				data: JSON.stringify(objeto),
	//				cache: false,
	//				async: false,
	//				type: 'POST',
	//				dataType: 'json',
	//				contentType: 'application/json; charset=utf-8',
	//				error: Aux.error,
	//				success: function (response, textStatus, XMLHttpRequest) {
	//					if (response.EhValido) {
	//						if (listarSucesso) {
	//							Modal.fechar(container);
	//							NFCaixa.container.listarAjax('ultimaBusca');
	//							MasterPage.redireciona(NFCaixa.settings.urls.urlPDF + '/' + response.Id);
	//						} else {
	//							MasterPage.redireciona(response.Url);
	//						}
	//					}

	//					if (response.Msg && response.Msg.length > 0) {
	//						Mensagem.gerar(container, response.Msg);
	//					}
	//				}
	//			});
	//			MasterPage.carregando(false);
	//		}, 'Ativar');
	//	});
	//},

	//cancelar: function () {
	//	var item = NFCaixa.obter(this);

	//	Modal.abrir(NFCaixa.settings.urls.urlConfirmCancel + '/' + item.Id, null, function (container) {
	//		Modal.defaultButtons(container, function (container) {
	//			var objeto = item;
	//			objeto.DataCancelamento = DataCancelamento = { DataTexto: $('.txtDataCancelamento', container).val() };

	//			MasterPage.carregando(true);
	//			$.ajax({
	//				url: NFCaixa.settings.urls.urlCancelar ,
	//				data: JSON.stringify(objeto),
	//				cache: false,
	//				async: false,
	//				type: 'POST',
	//				dataType: 'json',
	//				contentType: 'application/json; charset=utf-8',
	//				error: Aux.error,
	//				success: function (response, textStatus, XMLHttpRequest) {
	//					if (response.EhValido) {
	//						NFCaixa.container.listarAjax('ultimaBusca');
	//						Modal.fechar(container);
	//					}

	//					if (response.Msg && response.Msg.length > 0) {
	//						Mensagem.gerar(container, response.Msg);
	//					}
	//				}
	//			});
	//			MasterPage.carregando(false);
	//		}, 'Confirmar');
	//	});
	//},

	//onChangeTipoDocumento: function () {
	//	if ($(this).val() > 0 && $(this).val() != 7)
	//		$('.txtNumeroDocumento', NFCaixa.container).toggleClass('hide', false);
	//	else
	//		$('.txtNumeroDocumento', NFCaixa.container).toggleClass('hide', true);
	//}
}
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

CARSolicitacaoListar = {
	urlEditar: null,
	urlExcluir: null,
	urlExcluirConfirm: null,
	urlAlterarSituacao: null,
	urlPDFPendencia: null,
	urlEnviarReenviarArquivoSICAR: null,
	urlMensagemErroEnviarArquivoSICAR: null,
	urlGerarPdfComprovanteSICAR: null,
	urlBaixarDemonstrativoCAR: null,
	idsTela: null,
	mensagens: null,
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(CARSolicitacaoListar.settings, options); }
        
		container.listarAjax({
			onBeforeFiltrar: CARSolicitacaoListar.onBeforeFiltrar
		});

		container.delegate('.btnAssociar', 'click', CARSolicitacaoListar.associar);
		container.delegate('.btnVisualizar', 'click', CARSolicitacaoListar.visualizar);
		container.delegate('.btnNotificacao', 'click', CARSolicitacaoListar.visualizarMotivoNotificacao);
		container.delegate('.btnPDF', 'click', CARSolicitacaoListar.gerarPDF);
		container.delegate('.btnPDFTitulo', 'click', CARSolicitacaoListar.gerarPDFTitulo);
		container.delegate('.btnEditar', 'click', CARSolicitacaoListar.editar);
		container.delegate('.btnExcluir', 'click', CARSolicitacaoListar.excluir);
		container.delegate('.btnAlterarSituacao', 'click', CARSolicitacaoListar.alterarSituacao);

		container.delegate('.btnEnviar', 'click', CARSolicitacaoListar.reenviarArquivoSICAR);
		container.delegate('.btnPdfPendencia', 'click', CARSolicitacaoListar.gerarPDFPendencia);
		container.delegate('.btnPdfSicar', 'click', CARSolicitacaoListar.redirecionarLinkPdfSICAR);
		container.delegate('.btnBaixarArquivoSicar', 'click', CARSolicitacaoListar.baixarArquivoSicar);
		container.delegate('.btnDemonstrativoCar', 'click', CARSolicitacaoListar.baixarDemonstrativoCar);

		container.delegate('.radioDeclaranteCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioDeclaranteCpfCnpj', container));

		Aux.setarFoco(container);
		CARSolicitacaoListar.container = container;

		if (CARSolicitacaoListar.settings.associarFuncao) {
			$('.hdnIsAssociar', container).val(true);
		}
	},

	onBeforeFiltrar: function (container, serializedData)
	{
		serializedData.Filtros.EmpreendimentoCodigo = Mascara.getIntMask($(".txtCodigo", CARSolicitacaoListar.container).val()).toString();
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},
	
	associar: function () {
		var objeto = CARSolicitacaoListar.obter(this);
		var retorno = CARSolicitacaoListar.settings.associarFuncao(objeto);

		Mensagem.gerar(CARSolicitacaoListar.container, retorno.Msg);

		if (retorno.FecharModal) {
			Modal.fechar(CARSolicitacaoListar.container);
		}
	},

	visualizar: function () {
		var objeto = CARSolicitacaoListar.obter(this);

		if (CARSolicitacaoListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', CARSolicitacaoListar.container).val() + "/" + objeto.Id, null, function (context) {
				Modal.defaultButtons(context);
			}, Modal.tamanhoModalGrande);
		} else {
			MasterPage.redireciona($('.urlVisualizar', CARSolicitacaoListar.container).val() + "/" + objeto.Id);
		}
	},

	gerarPDF: function () {
		var objeto = CARSolicitacaoListar.obter(this);
		MasterPage.redireciona($('.urlPdf', CARSolicitacaoListar.container).val() + "/" + objeto.Id);
	},

	gerarPDFTitulo: function () {
		var objeto = CARSolicitacaoListar.obter(this);
		MasterPage.redireciona($('.urlPdfTitulo', CARSolicitacaoListar.container).val() + "/" + objeto.Id);
	},

	editar: function () {
		var objeto = CARSolicitacaoListar.obter(this);
		MasterPage.redireciona(CARSolicitacaoListar.urlEditar + '/' + objeto.Id);
	},

	excluir: function () {
		var objeto = CARSolicitacaoListar.obter(this);
		
		Modal.excluir({
			'urlConfirm': CARSolicitacaoListar.urlExcluirConfirm,
			'urlAcao': CARSolicitacaoListar.urlExcluir,
			'id': objeto.Id,
			'btnExcluir': this
		});
	},

	alterarSituacao: function () {
		var objeto = CARSolicitacaoListar.obter(this);
		MasterPage.redireciona(CARSolicitacaoListar.urlAlterarSituacao + '/' + objeto.Id);
	},

	visualizarMotivoNotificacao: function () {
		var objeto = CARSolicitacaoListar.obter(this);

		Modal.abrir($('.urlVisualizarMotivo', CARSolicitacaoListar.container).val() + '/' + objeto.Id, null, function (context) {
			Modal.defaultButtons(context);
		}, Modal.tamanhoModalMedia);
	},

	baixarArquivoSicar: function () {
		var objeto = CARSolicitacaoListar.obter(this);

		MasterPage.redireciona(CARSolicitacaoListar.urlBaixarAquivoSICAR + '/' + objeto.ArquivoSICAR);
	},

	baixarDemonstrativoCar: function () {
		var objeto = CARSolicitacaoListar.obter(this);
		var isTitulo = JSON.parse($(this).closest('tr').find('.itemJson').val()).isTitulo;
		
	    MasterPage.carregando(true);
	    $.ajax({
			url: CARSolicitacaoListar.urlBaixarDemonstrativoCAR,
			data: JSON.stringify({ id: objeto.Id, isTitulo: isTitulo }),
	        cache: false,
	        async: false,
	        type: 'POST',
	        dataType: 'json',
	        contentType: 'application/json; charset=utf-8',
	        error: function (XMLHttpRequest, textStatus, erroThrown) {
	            Aux.error(XMLHttpRequest, textStatus, erroThrown, CARSolicitacaoListar.container);
	            MasterPage.carregando(false);
	        },
	        success: function (response, textStatus, XMLHttpRequest) {
	            MasterPage.carregando(false);
				if (!response.UrlPdfDemonstrativo.isNullOrWhitespace()) {
	                window.open(response.UrlPdfDemonstrativo);
	            }
	            else {
					Mensagem.limpar(CARSolicitacaoListar.container);
					Mensagem.gerar(CARSolicitacaoListar.container, [CARSolicitacaoListar.mensagens.ErroPdfDemonstrativo]);
	            }


	            CARSolicitacaoListar.callBackPost(response, CARSolicitacaoListar.container);
	        }
	    });
	    MasterPage.carregando(false);
	},

	redirecionarLinkPdfSICAR: function () {
		var objeto = CARSolicitacaoListar.obter(this);
		var data = { solicitacaoId: objeto.Id };

		MasterPage.carregando(true);
		$.ajax({
			url: CARSolicitacaoListar.urlGerarPdfComprovanteSICAR+'/'+objeto.Id,
			cache: false,
			//data: data,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, CARSolicitacaoListar.container);
				MasterPage.carregando(false);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				MasterPage.carregando(false);
				if (response.UrlPdfReciboSICAR) {
					window.open(response.UrlPdfReciboSICAR);
				}
				else {
					Mensagem.limpar(CARSolicitacaoListar.container);
					Mensagem.gerar(CARSolicitacaoListar.container, [CARSolicitacaoListar.mensagens.GerarPdfSICARUrlNaoEncontrada]);
				}


				CARSolicitacaoListar.callBackPost(response, container);
			}
		});
		MasterPage.carregando(false);
	},

	gerarPDFPendencia: function () {
	    var objeto = CARSolicitacaoListar.obter(this);

	    MasterPage.redireciona(CARSolicitacaoListar.urlPDFPendencia + '/' + objeto.Id + '?isCredenciado=' + (objeto.Origem == 2));
	},

	reenviarArquivoSICAR: function () {
	    Mensagem.limpar(CARSolicitacaoListar.container);
	    var objeto = CARSolicitacaoListar.obter(this);

	    Modal.confirma({
	    	btnOkCallback: function (container) { CARSolicitacaoListar.callBackReenviar(CARSolicitacaoListar.container, objeto.Id, objeto.Origem, false) },
	    	titulo: "Reenviar arquivo do CAR ao SICAR",
	    	conteudo: CARSolicitacaoListar.mensagens.ReenviarMsgConfirmacao.Texto,
	    	tamanhoModal: Modal.tamanhoModalMedia
	    });


	    //if (objeto.SituacaoSolicitacaoId == CARSolicitacaoListar.idsTela.SolicitacaoEmCadastro) {
	    //    if (!objeto.SituacaoArquivoCarTexto) {
	    //        CARSolicitacaoListar.callBackReenviar(CARSolicitacaoListar.container, objeto.Id, objeto.Origem, true);
	    //        return;
	    //    }
	    //    CARSolicitacaoListar.mensagemErroEnviarArquivoSICAR(CARSolicitacaoListar.container, true, objeto.SituacaoSolicitacaoTexto, objeto.SituacaoArquivoCarTexto);
	    //}
		//
	    //if (objeto.SituacaoSolicitacaoId == CARSolicitacaoListar.idsTela.SolicitacaoPendente) {
		//
	    //    if (objeto.SituacaoArquivoCarId == CARSolicitacaoListar.idsTela.ArquivoArquivoReprovado) {
	    //        Modal.confirma({
	    //            btnOkCallback: function (container) { CARSolicitacaoListar.callBackReenviar(CARSolicitacaoListar.container, objeto.Id, objeto.Origem, false) },
	    //            titulo: "Reenviar arquivo do CAR ao SICAR",
	    //            conteudo: CARSolicitacaoListar.mensagens.ReenviarMsgConfirmacao.Texto,
	    //            tamanhoModal: Modal.tamanhoModalMedia
	    //        });
	    //        return;
	    //    }
	    //    CARSolicitacaoListar.mensagemErroEnviarArquivoSICAR(CARSolicitacaoListar.container, false, objeto.SituacaoSolicitacaoTexto, objeto.SituacaoArquivoCarTexto);
	    //}
	    //CARSolicitacaoListar.mensagemErroEnviarArquivoSICAR(CARSolicitacaoListar.container, true, objeto.SituacaoSolicitacaoTexto, objeto.SituacaoArquivoCarTexto);
	},

	callBackReenviar: function (container, solicitacaoId, origem, isEnviar) {

	    var dados = { solicitacaoId: solicitacaoId, origem: origem, isEnviar: isEnviar };

	    MasterPage.carregando(true);

	    $.ajax({
	        url: CARSolicitacaoListar.urlEnviarReenviarArquivoSICAR,
	        data: JSON.stringify(dados),
	        cache: false,
	        async: false,
	        type: 'POST',
	        dataType: 'json',
	        contentType: 'application/json; charset=utf-8',
	        error: function (XMLHttpRequest, textStatus, erroThrown) {
	            Aux.error(XMLHttpRequest, textStatus, erroThrown, CARSolicitacaoListar.container);
	            MasterPage.carregando(false);
	        },
	        success: function (response, textStatus, XMLHttpRequest) {
	        	if (response.EhValido) {
	        		CARSolicitacaoListar.callBackPost(response, container);
	        		//Modal.fechar(container);
	        	}

	        	if (response.Msg && response.Msg.length > 0) {
	        		Mensagem.gerar(container, response.Msg);
	        	}
	        }
	    });
	    MasterPage.carregando(false);
	},

	mensagemErroEnviarArquivoSICAR: function (container, isEnviar, situacaoSolicitacaoTexto, situacaoArquivoCarTexto) {
	    var dados = { isEnviar: isEnviar, solicitacaoSituacao: situacaoSolicitacaoTexto, arquivoSituacao: situacaoArquivoCarTexto };

	    MasterPage.carregando(true);
	    $.ajax({
	        url: CARSolicitacaoListar.urlMensagemErroEnviarArquivoSICAR,
	        data: JSON.stringify(dados),
	        cache: false,
	        async: false,
	        type: 'POST',
	        dataType: 'json',
	        contentType: 'application/json; charset=utf-8',
	        error: function (XMLHttpRequest, textStatus, erroThrown) {
	            Aux.error(XMLHttpRequest, textStatus, erroThrown, container);
	            MasterPage.carregando(false);
	        },
	        success: function (response, textStatus, XMLHttpRequest) {
	            CARSolicitacaoListar.callBackPost(response, container);
	        }
	    });
	    MasterPage.carregando(false);
	},

	callBackPost: function (resultado, container) {
	    MasterPage.carregando(false);

	    if (!resultado.EhValido) {
	        Mensagem.gerar(container, resultado.Msg);
	        return;
	    }
	    if (resultado.urlRetorno != null && typeof resultado.urlRetorno != "undefined") {
	        MasterPage.redireciona(resultado.urlRetorno);
	    }
	}
}
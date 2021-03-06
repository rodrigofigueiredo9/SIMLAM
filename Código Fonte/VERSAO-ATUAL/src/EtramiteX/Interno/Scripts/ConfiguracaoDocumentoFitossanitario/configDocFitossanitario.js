﻿/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" 
/// <reference path="../masterpage.js" />

ConfigDocFitossanitario = {
	settings: {
		urls: {
			salvar: '',
			validarIntervalo: '',
			editar: '',
			salvarEdicao: '',
			excluir: '',
            validarEdicao: '',
		},
		Mensagens: null
	},

	container: null,

    modalOrigem: null,

	load: function (container, options) {
        
		if (options) { $.extend(ConfigDocFitossanitario.settings, options); }
		ConfigDocFitossanitario.container = MasterPage.getContent(container);

		container.delegate('.btnAdicionarNumero', 'click', ConfigDocFitossanitario.adicionarIntervalo);
		container.delegate('.btnSalvar', 'click', ConfigDocFitossanitario.salvar);
		container.delegate('.btnEditar', 'click', ConfigDocFitossanitario.editarIntervalo);
		container.delegate('.btnExcluir', 'click', ConfigDocFitossanitario.abrirModalConfirmarExcluir);
		container.delegate('.ddlTipoDocumento', 'change', ConfigDocFitossanitario.toggleMask);

		Aux.setarFoco(container);
	},

	editarIntervalo: function () {
	    var id = ConfigDocFitossanitario.obterId(this);

	    Mensagem.limpar(ConfigDocFitossanitario.container);
	    
	    var retorno = MasterPage.validarAjax(ConfigDocFitossanitario.settings.urls.validarEdicao, { idStr: id }, ConfigDocFitossanitario.container, false);
	    if (!retorno.EhValido) {
	        return;
	    }

	    var tipo = ConfigDocFitossanitario.obterTipo(this);
	    
	    var settings = function (content) {
	        Modal.defaultButtons(content, function () {
	            ConfigDocFitossanitario.modalOrigem = content;
	            ConfigDocFitossanitario.salvarEdicao(content, id);
	        }, 'Salvar');
	        ConfigDocFitossanitario.toggleMaskModal(tipo);
	    };
	    
	    Modal.abrir(ConfigDocFitossanitario.settings.urls.editar + '/' + id, null, settings, Modal.tamanhoModalMedia, "Editar Numeração");
	},

	salvarEdicao: function(modal, iditem){
	    //Modal.fechar(modal);
	    Mensagem.limpar(ConfigDocFitossanitario.container);
	    MasterPage.carregando(true);

	    var numInicial = ConfigDocFitossanitario.modalOrigem.find('.txtNumeroInicial').val();
	    var numFinal = ConfigDocFitossanitario.modalOrigem.find('.txtNumeroFinal').val();

	    $.ajax({
	        url: ConfigDocFitossanitario.settings.urls.salvarEdicao,
	        data: JSON.stringify({
	            configuracao: ConfigDocFitossanitario.obter(),
	            idstring: iditem,
	            novoNumInicial: numInicial,
                novoNumFinal: numFinal,
	        }),
	        cache: false,
	        async: false,
	        type: 'POST',
	        dataType: 'json',
	        contentType: 'application/json; charset=utf-8',
	        error: Aux.error,
	        success: function (response, textStatus, XMLHttpRequest) {
	            if (response.EhValido) {
	                MasterPage.redireciona(response.Url);
	            }

	            if (response.Msg && response.Msg.length > 0) {
	                Mensagem.gerar(ConfigDocFitossanitario.container, response.Msg);
	            }
	        }
	    });

	    MasterPage.carregando(false);
	},

	adicionarIntervalo: function () {
		Mensagem.limpar(ConfigDocFitossanitario.container);
		var mensagens = new Array();

		var container = $(this).closest('fieldset');
		var ehBloco = container.find('.dgNumerosBloco').length > 0;
		var ddl = container.find('.ddlTipoDocumento');

		var ddlSelecionado = ddl.ddlSelecionado();

		var ddlSerieSelecionado = container.find('.ddlDigitalSerie').ddlSelecionado();

		var valSerie = "";

		if (ddlSerieSelecionado.Texto != "") {
		    valSerie = " / " + ddlSerieSelecionado.Texto;
		}

		var item = {
			TipoDocumentoID: ddlSelecionado.Id,
			TipoDocumentoTexto: ddlSelecionado.Texto,
			Tipo: (ehBloco ? 1 : 2),
			NumeroInicial: $('.txtNumeroInicial', container).val(),
			NumeroFinal: $('.txtNumeroFinal', container).val(),
			Serie: ddlSerieSelecionado.Texto
		};

		var itens = [];
		$($('.dgNumeros tbody tr:not(.trTemplateRow) .hdnItemJSon', ConfigDocFitossanitario.container)).each(function () {
			itens.push(JSON.parse($(this).val()));
		});

		var retorno = MasterPage.validarAjax(ConfigDocFitossanitario.settings.urls.validarIntervalo, { intervalo: item, intervalos: itens }, ConfigDocFitossanitario.container, false);
		if (!retorno.EhValido) {
			return;
		}

		ConfigDocFitossanitario.atualizarDataGrid(container, item);

		//Limpa os controles, mas mantém o tipo de documento selecionado
		$('.txtNumeroInicial', container).val('');
		$('.txtNumeroFinal', container).val('');
	},

	atualizarDataGrid: function (container, item) {
	    var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide').addClass('Linha');
	    var btnEdit = $('<button type="button" title="Editar" class="icone editar btnEditar"></button><button type="button" title="Excluir" class="icone excluir btnExcluir"></button>');

	    var valSerieInicial = item.NumeroInicial;
	    var valSerieFinal = item.NumeroFinal;
	    if (item.Serie != "") {
	        valSerieInicial += " / " + item.Serie;
	        valSerieFinal += " / " + item.Serie;
	    }

	

		linha.find('.hdnItemJSon').val(JSON.stringify(item));
		linha.find('.TipoDocumentoTexto').html(item.TipoDocumentoTexto).attr('title', item.TipoDocumentoTexto);
		linha.find('.NumeroInicial').html(valSerieInicial).attr('title', valSerieInicial);
		linha.find('.NumeroFinal').html(valSerieFinal).attr('title', valSerieFinal);
		linha.find('.Acoes').html(btnEdit);

		$('tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container);
	},

	salvar: function (modal) {
		Modal.fechar(modal);
		Mensagem.limpar(ConfigDocFitossanitario.container);
		MasterPage.carregando(true);

		$.ajax({
			url: ConfigDocFitossanitario.settings.urls.salvar,
			data: JSON.stringify({ configuracao: ConfigDocFitossanitario.obter() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.Url);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ConfigDocFitossanitario.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},

	obter: function () {
		var objeto = {
			ID: $('.configuracaoID', ConfigDocFitossanitario.container).val(),
			DocumentoFitossanitarioIntervalos: []
		}

		$($('.dgNumeros tbody tr:not(.trTemplateRow) .hdnItemJSon', ConfigDocFitossanitario.container)).each(function () {
			objeto.DocumentoFitossanitarioIntervalos.push(JSON.parse($(this).val()));
		});

		return objeto;
	},

	obterId: function (container) {
	    var id = $(container).closest('tr').find('.ItemID').val();

	    return id;
	},

	obterTipo: function (container) {
	    var tipo = $(container).closest('tr').find('.TipoDocumentoTexto').text();

	    return tipo;
	},

	obterInicioIntervalo: function (container){
	    var numero = $(container).closest('tr').find('.NumeroInicial').text();

	    return numero;
	},

	obterFinalIntervalo: function (container) {
	    var numero = $(container).closest('tr').find('.NumeroFinal').text();

	    return numero;
	},

	abrirModalConfirmarExcluir: function () {
	    var tipo = ConfigDocFitossanitario.obterTipo(this);
	    var inicio = ConfigDocFitossanitario.obterInicioIntervalo(this);
	    var fim = ConfigDocFitossanitario.obterFinalIntervalo(this);

	    var html = '<p>Tem certeza de que deseja excluir o intervalo de ' + inicio + ' a ' + fim + ' do tipo ' + tipo + '?</p>';

	    var id = ConfigDocFitossanitario.obterId(this);
	    
	    var settings = {
	        titulo: 'Excluir Intervalo ' + tipo,
	        onLoadCallbackName: function (content) {
	            Modal.defaultButtons(content, function () {
	                ConfigDocFitossanitario.excluirIntervalo(content, id);
	            }, 'Excluir');
	        }
	    };
	    Modal.abrirHtml(html, settings);
	},

	excluirIntervalo: function (modal, idItem) {
	    Modal.fechar(modal);
	    Mensagem.limpar(ConfigDocFitossanitario.container);
	    MasterPage.carregando(true);
	    
	    $.ajax({
	        url: ConfigDocFitossanitario.settings.urls.excluir,
	        data: JSON.stringify({
	            configuracao: ConfigDocFitossanitario.obter(),
	            idString: idItem,
	        }),
	        cache: false,
	        async: false,
	        type: 'POST',
	        dataType: 'json',
	        contentType: 'application/json; charset=utf-8',
	        error: Aux.error,
	        success: function (response, textStatus, XMLHttpRequest) {
	            if (response.EhValido) {
	                MasterPage.redireciona(response.Url);
	            }

	            if (response.Msg && response.Msg.length > 0) {
	                Mensagem.gerar(ConfigDocFitossanitario.container, response.Msg);
	            }
	        }
	    });
	    
	    MasterPage.carregando(false);
	},

	toggleMask: function (evt) {
        function toggleClass(element, txt) {
	        switch (txt) {
	            case "CFO":
	            case "CFOC":
	                element.classList.remove("maskNum8");
	                element.classList.add("maskNum10");
	                break;

	            default:
	                element.classList.remove("maskNum8");
	                element.classList.add("maskNum10");
	        }
        }

        var target = evt.target
        var txt = target.selectedOptions[0].text

	    var isBloco = target.classList.contains("ddlBloco")
	    var isDigital = target.classList.contains("ddlDigital")
	    var complemento = isBloco ? ".txtBloco" : ".txtDigital"

	    var campoInicial = document.querySelector(".txtNumeroInicial" + complemento)
	    var campoFinal = document.querySelector(".txtNumeroFinal" + complemento)

	    toggleClass(campoInicial, txt)
	    toggleClass(campoFinal, txt)

	    //Oculta as linhas que não são do mesmo tipo de documento selecionado

	    $(this).closest('fieldset').find('.Linha').each(function () {
	        if (txt == "CFO" || txt == "CFOC" || txt == "PTV") {
	            var linha = $(this);
	            if (linha.find('.TipoDocumentoTexto').text() != txt) {
	                linha.hide();
	            } else {
	                linha.show();
	            }

	        } else {
	            var linha = $(this);
	            linha.show();
	        }
	    });

	    $(".maskNum8" + complemento)
            .unmask()
            .mask("99999999")
            .val("");

	    $(".maskNum10" + complemento)
            .unmask()
            .mask("9999999999")
            .val("");
	},

	toggleMaskModal: function (tipo) {
	    function toggleClass(element, tipo) {
	        switch (tipo) {
	            case "CFO":
	            case "CFOC":
	                element.classList.remove("maskNum8");
	                element.classList.add("maskNum10");
	                break;

	            default:
	                element.classList.remove("maskNum8");
	                element.classList.add("maskNum10");
	        }
	    }

	    var campoInicial = document.querySelector(".txtNumeroInicial")
	    var campoFinal = document.querySelector(".txtNumeroFinal")

	    toggleClass(campoInicial, tipo)
	    toggleClass(campoFinal, tipo)

	    $(".maskNum8")
            .unmask()
            .mask("99999999");

	    $(".maskNum10")
            .unmask()
            .mask("9999999999");
	}
}

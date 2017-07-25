/// <reference path="../jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json - 2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

ConfigurarProdutosDestinos = {
    settings: {
        urls: {
            salvar: '',
            //validarIntervalo: '',
            //editar: '',
            //salvarEdicao: '',
            //excluir: '',
            //validarEdicao: '',
        },
        Mensagens: null
    },

    container: null,

    load: function (container, options) {

        if (options) { $.extend(ConfigurarProdutosDestinos.settings, options); }
        ConfigurarProdutosDestinos.container = MasterPage.getContent(container);

        container.delegate('.btnAdicionarProduto', 'click', ConfigurarProdutosDestinos.adicionarProduto);
        //container.delegate('.btnSalvar', 'click', ConfigDocFitossanitario.salvar);
        //container.delegate('.btnEditar', 'click', ConfigDocFitossanitario.editarIntervalo);
        //container.delegate('.btnExcluir', 'click', ConfigDocFitossanitario.abrirModalConfirmarExcluir);
        //container.delegate('.ddlTipoDocumento', 'change', ConfigDocFitossanitario.toggleMask);

        Listar.atualizarEstiloTable('.tabProdutos', ConfigurarProdutosDestinos.container)
        Aux.setarFoco(container);
    },

    adicionarProduto: function () {
        Mensagem.limpar(ConfigurarProdutosDestinos.container);
        var mensagens = new Array();

        //Pega os campos para adicionar na tabela
        var container = $(this).closest('fieldset');
        var item = container.find('.txtProdutoItem').val();
        var unidade = container.find('.txtProdutoUnidade').val();
        var id = container.find('.hdnItemId').val();
        var ehAtivo = container.find('.hdnItemIsAtivo').val();

        //Verifica se os campos foram preenchidos
        if (item.length == 0) {
            $('.txtProdutoItem', container).addClass('erroItem');
            mensagens.push(ConfigurarProdutosDestinos.settings.Mensagens.ItemProdutoObrigatorio);
        }
        if (unidade.length == 0) {
            $('.txtProdutoUnidade', container).addClass('erroUnidade');
            mensagens.push(ConfigurarProdutosDestinos.settings.Mensagens.UnidadeProdutoObrigatoria);
        }
        if (ConfigurarProdutosDestinos.publicarMensagem(mensagens)) {
            return false;
        }

        //Verifica se o produto (item+unidade) já existe na tabela
        var tabelaProdutos = $('.tabProdutos tbody tr', container);
        $(tabelaProdutos).each(function (i, prod) {
            if ($('.nomeItem', prod).text() == item && $('.unidadeMedida', prod).text() == unidade) {
                mensagens.push(ConfigurarProdutosDestinos.settings.Mensagens.ProdutoDuplicado);
            }
        });
        if (ConfigurarProdutosDestinos.publicarMensagem(mensagens)) {
            return false;
        }

        //Monta a nova linha e insere na tabela
        var linha = $('.trTemplateRow', container).clone();
        linha.find('.nomeItem').text(item);
        linha.find('.nomeItem').attr('title', item);
        linha.find('.unidadeMedida').text(unidade);
        linha.find('.unidadeMedida').attr('title', unidade);
        linha.find('.itemId').val(id);
        linha.removeClass('trTemplateRow hide');
        $('.tabProdutos > tbody:last', container).append(linha);

        Listar.atualizarEstiloTable($('.tabProdutos', container));

        //limpa os campos de texto
        $('.txtProdutoItem', container).val('');
        $('.txtProdutoUnidade', container).val('');
        $('.hdnItemId', container).val('0');
        $('.hdnItemIsAtivo', container).val('1');
    },

    publicarMensagem: function (mensagens) {
        if (mensagens.length > 0) {
            Mensagem.gerar(ConfigurarProdutosDestinos.container, mensagens)
            return true;
        }
        return false;
    }






    //editarIntervalo: function () {
    //    var id = ConfigDocFitossanitario.obterId(this);

    //    Mensagem.limpar(ConfigDocFitossanitario.container);

    //    var retorno = MasterPage.validarAjax(ConfigDocFitossanitario.settings.urls.validarEdicao, { idStr: id }, ConfigDocFitossanitario.container, false);
    //    if (!retorno.EhValido) {
    //        return;
    //    }

    //    var tipo = ConfigDocFitossanitario.obterTipo(this);

    //    var settings = function (content) {
    //        Modal.defaultButtons(content, function () {
    //            ConfigDocFitossanitario.modalOrigem = content;
    //            ConfigDocFitossanitario.salvarEdicao(content, id);
    //        }, 'Salvar');
    //        ConfigDocFitossanitario.toggleMaskModal(tipo);
    //    };

    //    Modal.abrir(ConfigDocFitossanitario.settings.urls.editar + '/' + id, null, settings, Modal.tamanhoModalMedia, "Editar Numeração");
    //},

    //salvarEdicao: function (modal, iditem) {
    //    //Modal.fechar(modal);
    //    Mensagem.limpar(ConfigDocFitossanitario.container);
    //    MasterPage.carregando(true);

    //    var numInicial = ConfigDocFitossanitario.modalOrigem.find('.txtNumeroInicial').val();
    //    var numFinal = ConfigDocFitossanitario.modalOrigem.find('.txtNumeroFinal').val();

    //    $.ajax({
    //        url: ConfigDocFitossanitario.settings.urls.salvarEdicao,
    //        data: JSON.stringify({
    //            configuracao: ConfigDocFitossanitario.obter(),
    //            idstring: iditem,
    //            novoNumInicial: numInicial,
    //            novoNumFinal: numFinal,
    //        }),
    //        cache: false,
    //        async: false,
    //        type: 'POST',
    //        dataType: 'json',
    //        contentType: 'application/json; charset=utf-8',
    //        error: Aux.error,
    //        success: function (response, textStatus, XMLHttpRequest) {
    //            if (response.EhValido) {
    //                MasterPage.redireciona(response.Url);
    //            }

    //            if (response.Msg && response.Msg.length > 0) {
    //                Mensagem.gerar(ConfigDocFitossanitario.container, response.Msg);
    //            }
    //        }
    //    });

    //    MasterPage.carregando(false);
    //},

   

    //atualizarDataGrid: function (container, item) {
    //    var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide').addClass('Linha');
    //    var btnEdit = $('<button type="button" title="Editar" class="icone editar btnEditar"></button><button type="button" title="Excluir" class="icone excluir btnExcluir"></button>');

    //    linha.find('.hdnItemJSon').val(JSON.stringify(item));
    //    linha.find('.TipoDocumentoTexto').html(item.TipoDocumentoTexto).attr('title', item.TipoDocumentoTexto);
    //    linha.find('.NumeroInicial').html(item.NumeroInicial).attr('title', item.NumeroInicial);
    //    linha.find('.NumeroFinal').html(item.NumeroFinal).attr('title', item.NumeroFinal);
    //    linha.find('.Acoes').html(btnEdit);

    //    $('tbody:last', container).append(linha);
    //    Listar.atualizarEstiloTable(container);
    //},

    //salvar: function (modal) {
    //    Modal.fechar(modal);
    //    Mensagem.limpar(ConfigDocFitossanitario.container);
    //    MasterPage.carregando(true);

    //    $.ajax({
    //        url: ConfigDocFitossanitario.settings.urls.salvar,
    //        data: JSON.stringify({ configuracao: ConfigDocFitossanitario.obter() }),
    //        cache: false,
    //        async: false,
    //        type: 'POST',
    //        dataType: 'json',
    //        contentType: 'application/json; charset=utf-8',
    //        error: Aux.error,
    //        success: function (response, textStatus, XMLHttpRequest) {
    //            if (response.EhValido) {
    //                MasterPage.redireciona(response.Url);
    //            }

    //            if (response.Msg && response.Msg.length > 0) {
    //                Mensagem.gerar(ConfigDocFitossanitario.container, response.Msg);
    //            }
    //        }
    //    });

    //    MasterPage.carregando(false);
    //},

    //obter: function () {
    //    var objeto = {
    //        ID: $('.configuracaoID', ConfigDocFitossanitario.container).val(),
    //        DocumentoFitossanitarioIntervalos: []
    //    }

    //    $($('.dgNumeros tbody tr:not(.trTemplateRow) .hdnItemJSon', ConfigDocFitossanitario.container)).each(function () {
    //        objeto.DocumentoFitossanitarioIntervalos.push(JSON.parse($(this).val()));
    //    });

    //    return objeto;
    //},

    //obterId: function (container) {
    //    var id = $(container).closest('tr').find('.ItemID').val();

    //    return id;
    //},

    //obterTipo: function (container) {
    //    var tipo = $(container).closest('tr').find('.TipoDocumentoTexto').text();

    //    return tipo;
    //},

    //obterInicioIntervalo: function (container) {
    //    var numero = $(container).closest('tr').find('.NumeroInicial').text();

    //    return numero;
    //},

    //obterFinalIntervalo: function (container) {
    //    var numero = $(container).closest('tr').find('.NumeroFinal').text();

    //    return numero;
    //},

    //abrirModalConfirmarExcluir: function () {
    //    var tipo = ConfigDocFitossanitario.obterTipo(this);
    //    var inicio = ConfigDocFitossanitario.obterInicioIntervalo(this);
    //    var fim = ConfigDocFitossanitario.obterFinalIntervalo(this);

    //    var html = '<p>Tem certeza de que deseja excluir o intervalo de ' + inicio + ' a ' + fim + ' do tipo ' + tipo + '?</p>';

    //    var id = ConfigDocFitossanitario.obterId(this);

    //    var settings = {
    //        titulo: 'Excluir Intervalo ' + tipo,
    //        onLoadCallbackName: function (content) {
    //            Modal.defaultButtons(content, function () {
    //                ConfigDocFitossanitario.excluirIntervalo(content, id);
    //            }, 'Excluir');
    //        }
    //    };
    //    Modal.abrirHtml(html, settings);
    //},

    //excluirIntervalo: function (modal, idItem) {
    //    Modal.fechar(modal);
    //    Mensagem.limpar(ConfigDocFitossanitario.container);
    //    MasterPage.carregando(true);

    //    $.ajax({
    //        url: ConfigDocFitossanitario.settings.urls.excluir,
    //        data: JSON.stringify({
    //            configuracao: ConfigDocFitossanitario.obter(),
    //            idString: idItem,
    //        }),
    //        cache: false,
    //        async: false,
    //        type: 'POST',
    //        dataType: 'json',
    //        contentType: 'application/json; charset=utf-8',
    //        error: Aux.error,
    //        success: function (response, textStatus, XMLHttpRequest) {
    //            if (response.EhValido) {
    //                MasterPage.redireciona(response.Url);
    //            }

    //            if (response.Msg && response.Msg.length > 0) {
    //                Mensagem.gerar(ConfigDocFitossanitario.container, response.Msg);
    //            }
    //        }
    //    });

    //    MasterPage.carregando(false);
    //},

    //toggleMask: function (evt) {
    //    function toggleClass(element, txt) {
    //        switch (txt) {
    //            case "CFO":
    //            case "CFOC":
    //                element.classList.remove("maskNum10");
    //                element.classList.add("maskNum8");
    //                break;

    //            default:
    //                element.classList.remove("maskNum8");
    //                element.classList.add("maskNum10");
    //        }
    //    }

    //    var target = evt.target
    //    var txt = target.selectedOptions[0].text

    //    var isBloco = target.classList.contains("ddlBloco")
    //    var isDigital = target.classList.contains("ddlDigital")
    //    var complemento = isBloco ? ".txtBloco" : ".txtDigital"

    //    var campoInicial = document.querySelector(".txtNumeroInicial" + complemento)
    //    var campoFinal = document.querySelector(".txtNumeroFinal" + complemento)

    //    toggleClass(campoInicial, txt)
    //    toggleClass(campoFinal, txt)

    //    //Oculta as linhas que não são do mesmo tipo de documento selecionado

    //    $(this).closest('fieldset').find('.Linha').each(function () {
    //        if (txt == "CFO" || txt == "CFOC" || txt == "PTV") {
    //            var linha = $(this);
    //            if (linha.find('.TipoDocumentoTexto').text() != txt) {
    //                linha.hide();
    //            } else {
    //                linha.show();
    //            }

    //        } else {
    //            var linha = $(this);
    //            linha.show();
    //        }
    //    });

    //    $(".maskNum8" + complemento)
    //        .unmask()
    //        .mask("99999999")
    //        .val("");

    //    $(".maskNum10" + complemento)
    //        .unmask()
    //        .mask("9999999999")
    //        .val("");
    //},

    //toggleMaskModal: function (tipo) {
    //    function toggleClass(element, tipo) {
    //        switch (tipo) {
    //            case "CFO":
    //            case "CFOC":
    //                element.classList.remove("maskNum10");
    //                element.classList.add("maskNum8");
    //                break;

    //            default:
    //                element.classList.remove("maskNum8");
    //                element.classList.add("maskNum10");
    //        }
    //    }

    //    var campoInicial = document.querySelector(".txtNumeroInicial")
    //    var campoFinal = document.querySelector(".txtNumeroFinal")

    //    toggleClass(campoInicial, tipo)
    //    toggleClass(campoFinal, tipo)

    //    $(".maskNum8")
    //        .unmask()
    //        .mask("99999999");

    //    $(".maskNum10")
    //        .unmask()
    //        .mask("9999999999");
    //}
}

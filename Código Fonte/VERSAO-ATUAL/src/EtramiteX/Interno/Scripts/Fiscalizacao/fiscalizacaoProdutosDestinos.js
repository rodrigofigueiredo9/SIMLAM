/// <reference path="../jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json - 2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

ConfigurarProdutosDestinos = {
    settings: {
        urls: {
            salvar: ''
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
        container.delegate('.btnSalvar', 'click', ConfigurarProdutosDestinos.salvar);
        container.delegate('.btnEditarProduto', 'click', ConfigurarProdutosDestinos.editarProduto);
        container.delegate('.btnDesativarProduto', 'click', ConfigurarProdutosDestinos.desativarProduto);
        container.delegate('.btnAtivarProduto', 'click', ConfigurarProdutosDestinos.ativarProduto);
        container.delegate('.btnExcluirProduto', 'click', ConfigurarProdutosDestinos.excluirProduto);

        Listar.atualizarEstiloTable('.tabProdutos', ConfigurarProdutosDestinos.container)
        Aux.setarFoco(container);
    },

    adicionarProduto: function () {
        Mensagem.limpar(ConfigurarProdutosDestinos.container);
        var mensagens = new Array();

        //Pega os campos para adicionar na tabela
        var container = $(this).closest('fieldset');
        var item = container.find('.txtProdutoItem').val().toString().trim();
        var unidade = container.find('.txtProdutoUnidade').val().toString().trim();
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

            /*Aqui, além de comparar item e unidade, ele verifica se o id do produto na linha
            é igual ao que está sendo adicionado, porque pode se tratar de um produto editado*/
            if ($('.nomeItem', prod).text() == item
                  && $('.unidadeMedida', prod).text() == unidade
                  && $('.produtoId', prod).val() != id) {
                mensagens.push(ConfigurarProdutosDestinos.settings.Mensagens.ProdutoDuplicado);
            }
        });
        if (ConfigurarProdutosDestinos.publicarMensagem(mensagens)) {
            return false;
        }

        //monta o objeto
        var objeto = {
            Id: id,
            Tid: '',
            Item: item,
            Unidade: unidade,
            Ativo: ehAtivo,
            Excluir: false
        };

        var linha = '';
        if (objeto.Id == 0) {   //Produto novo
            linha = $('.trTemplateRow', container).clone();

            //Monta a nova linha e insere na tabela
            linha.find('.hdnItemJSon').val(JSON.stringify(objeto));
            linha.find('.nomeItem').text(item);
            linha.find('.nomeItem').attr('title', item);
            linha.find('.unidadeMedida').text(unidade);
            linha.find('.unidadeMedida').attr('title', unidade);
            linha.find('.produtoId').val(id);
            linha.find('.produtoAtivo').val(ehAtivo);

            linha.removeClass('trTemplateRow hide');
            $('.tabProdutos > tbody:last', container).append(linha);

        } else {    //Produto editado
            $(tabelaProdutos).each(function (i, prod) {

                //Procura a linha que tem o mesmo id do produto
                if ($('.produtoId', prod).val() == id) {
                    
                    //Edita a linha
                    $('.hdnItemJSon', prod).val(JSON.stringify(objeto));
                    $('.nomeItem', prod).text(item);
                    $('.nomeItem', prod).attr('title', item);
                    $('.unidadeMedida', prod).text(unidade);
                    $('.unidadeMedida', prod).attr('title', unidade);
                    $('.produtoId', prod).val(id);
                    $('.produtoAtivo', prod).val(ehAtivo);
                }
            });
        }

        Listar.atualizarEstiloTable($('.tabProdutos', container));

        //limpa os campos de texto
        $('.txtProdutoItem', container).val('');
        $('.txtProdutoUnidade', container).val('');
        $('.hdnItemId', container).val('0');
        $('.hdnItemIsAtivo', container).val('true');
    },

    publicarMensagem: function (mensagens) {
        if (mensagens.length > 0) {
            Mensagem.gerar(ConfigurarProdutosDestinos.container, mensagens)
            return true;
        }
        return false;
    },

    salvar: function () {
        Mensagem.limpar(ConfigurarProdutosDestinos.container);
        MasterPage.carregando(true);
        
        $.ajax({
            url: ConfigurarProdutosDestinos.settings.urls.salvar,
            data: JSON.stringify({
                listaProdutos: ConfigurarProdutosDestinos.obterListaProdutos(),
                listaDestinos: ConfigurarProdutosDestinos.obterListaDestinos()
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
                }else if (response.Msg && response.Msg.length > 0) {
                    Mensagem.gerar(ConfigurarProdutosDestinos.container, response.Msg);
                }
            }
        });

        MasterPage.carregando(false);
    },

    obterListaProdutos: function () {
        var lista = [];

        $($('.tabProdutos tbody tr:not(.trTemplateRow) .hdnItemJSon', ConfigurarProdutosDestinos.container)).each(function () {
            lista.push(JSON.parse($(this).val()));
        });

        return lista;
    },

    obterListaDestinos: function () {
        var lista = [];

        //var objeto = {
        //    ID: $('.configuracaoID', ConfigDocFitossanitario.container).val(),
        //    DocumentoFitossanitarioIntervalos: []
        //}

        //$($('.dgNumeros tbody tr:not(.trTemplateRow) .hdnItemJSon', ConfigDocFitossanitario.container)).each(function () {
        //    objeto.DocumentoFitossanitarioIntervalos.push(JSON.parse($(this).val()));
        //});

        return lista;
    },

    editarProduto: function () {

        //Pega os campos que serão editados, e o id
        var container = $(this).closest('tr');
        var item = container.find('.nomeItem').text();
        var unidade = container.find('.unidadeMedida').text();
        var id = container.find('.produtoId').val();
        var ehAtivo = container.find('.produtoAtivo').val();

        //preenche os textbox
        container = $(this).closest('fieldset');
        container.find('.txtProdutoItem').val(item);
        container.find('.txtProdutoUnidade').val(unidade);
        container.find('.hdnItemId').val(id);
        container.find('.hdnItemIsAtivo').val(ehAtivo);
    },

    ativarProduto: function () {
        //recria o objeto
        var objeto = {
            Id: $(this).closest('tr').find('.produtoId').val(),
            Tid: '',
            Item: $(this).closest('tr').find('.nomeItem').text(),
            Unidade: $(this).closest('tr').find('.unidadeMedida').text(),
            Ativo: true,
            Excluir: false
        };

        //desabilita o botão de ativar
        $(this).closest('tr').find('.btnAtivarProduto').attr('disabled', 'disabled');
        $(this).closest('tr').find('.btnAtivarProduto').attr('aria-disabled', true);
        $(this).closest('tr').find('.btnAtivarProduto').addClass('disabled').addClass('ui-button-disabled').addClass('ui-state-disabled');

        //habilita o botão de desativar
        $(this).closest('tr').find('.btnDesativarProduto').removeAttr('disabled');
        $(this).closest('tr').find('.btnDesativarProduto').removeAttr('aria-disabled');
        $(this).closest('tr').find('.btnDesativarProduto').removeClass('disabled').removeClass('ui-button-disabled').removeClass('ui-state-disabled');

        //altera o valor da propriedade no objeto
        $(this).closest('tr').find('.hdnItemJSon').val(JSON.stringify(objeto));
    },

    desativarProduto: function () {
        //recria o objeto
        var objeto = {
            Id: $(this).closest('tr').find('.produtoId').val(),
            Tid: '',
            Item: $(this).closest('tr').find('.nomeItem').text(),
            Unidade: $(this).closest('tr').find('.unidadeMedida').text(),
            Ativo: false,
            Excluir: false
        };

        //desabilita o botão de desativar
        $(this).closest('tr').find('.btnDesativarProduto').attr('disabled', 'disabled');
        $(this).closest('tr').find('.btnDesativarProduto').attr('aria-disabled', true);
        $(this).closest('tr').find('.btnDesativarProduto').addClass('disabled').addClass('ui-button-disabled').addClass('ui-state-disabled');

        //habilita o botão de ativar
        $(this).closest('tr').find('.btnAtivarProduto').removeAttr('disabled');
        $(this).closest('tr').find('.btnAtivarProduto').removeAttr('aria-disabled');
        $(this).closest('tr').find('.btnAtivarProduto').removeClass('disabled').removeClass('ui-button-disabled').removeClass('ui-state-disabled');

        //altera o valor da propriedade no objeto
        $(this).closest('tr').find('.hdnItemJSon').val(JSON.stringify(objeto));
        $(this).closest('tr').find('.produtoAtivo').val(objeto.Ativo);
    },

    excluirProduto: function () {
        //recria o objeto
        var objeto = {
            Id: $(this).closest('tr').find('.produtoId').val(),
            Tid: '',
            Item: '',
            Unidade: '',
            Ativo: false,
            Excluir: true
        };
        if (objeto.Id != 0) {
            $(this).closest('tr').find('.hdnItemJSon').val(JSON.stringify(objeto));
            $(this).closest('tr').hide();
        } else {
            $(this).closest('tr').remove();
        }
    },






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

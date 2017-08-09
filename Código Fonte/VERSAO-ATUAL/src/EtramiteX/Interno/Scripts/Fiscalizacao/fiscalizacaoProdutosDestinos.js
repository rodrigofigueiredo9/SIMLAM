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

        container.delegate('.btnAdicionarDestino', 'click', ConfigurarProdutosDestinos.adicionarDestino);

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
                  && ((id != 0 && $('.produtoId', prod).val() != id)
                      || id == 0)) {
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
            Excluir: false,
            Editado: false
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
            objeto.Editado = true;
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

    adicionarDestino: function () {
        Mensagem.limpar(ConfigurarProdutosDestinos.container);
        var mensagens = new Array();

        //Pega os campos para adicionar na tabela
        var container = $(this).closest('fieldset');
        var destino = container.find('.txtDestino').val().toString().trim();
        var id = container.find('.hdnDestinoId').val();
        var ehAtivo = container.find('.hdnDestinoIsAtivo').val();

        //Verifica se os campos foram preenchidos
        if (destino.length == 0) {
            $('.txtDestino', container).addClass('erroItem');
            mensagens.push(ConfigurarProdutosDestinos.settings.Mensagens.DestinoObrigatorio);
        }
        if (ConfigurarProdutosDestinos.publicarMensagem(mensagens)) {
            return false;
        }

        //Verifica se o destino já existe na tabela
        var tabelaDestinos = $('.tabDestinos tbody tr', container);
        $(tabelaDestinos).each(function (i, prod) {

            /*Aqui, além de comparar o destino, verifica se o id na linha é igual ao que está
            sendo adicionado, porque pode se tratar de um destino editado*/
            if ($('.nomeDestino', prod).text() == destino
                  && ((id != 0 && $('.destinoId', prod).val() != id)
                      || id == 0)) {
                mensagens.push(ConfigurarProdutosDestinos.settings.Mensagens.DestinoDuplicado);
            }
        });
        if (ConfigurarProdutosDestinos.publicarMensagem(mensagens)) {
            return false;
        }

        //monta o objeto
        var objeto = {
            Id: id,
            Tid: '',
            Destino: destino,
            Ativo: ehAtivo,
            Excluir: false,
            Editado: false
        };

        var linha = '';
        if (objeto.Id == 0) {   //Destino novo
            linha = $('.trTemplateRow', container).clone();

            //Monta a nova linha e insere na tabela
            linha.find('.hdnItemJSon').val(JSON.stringify(objeto));
            linha.find('.nomeDestino').text(destino);
            linha.find('.nomeDestino').attr('title', destino);
            linha.find('.destinoId').val(id);
            linha.find('.destinoAtivo').val(ehAtivo);

            linha.removeClass('trTemplateRow hide');
            $('.tabDestinos > tbody:last', container).append(linha);

        } else {    //Destino editado
            objeto.Editado = true;
            $(tabelaDestinos).each(function (i, prod) {

                //Procura a linha que tem o mesmo id da destinação
                if ($('.destinoId', prod).val() == id) {

                    //Edita a linha
                    $('.hdnItemJSon', prod).val(JSON.stringify(objeto));
                    $('.nomeDestino', prod).text(destino);
                    $('.nomeDestino', prod).attr('title', destino);
                    $('.destinoId', prod).val(id);
                    $('.destinoAtivo', prod).val(ehAtivo);
                }
            });
        }

        Listar.atualizarEstiloTable($('.tabDestinos', container));

        //limpa os campos de texto
        $('.txtDestino', container).val('');
        $('.hdnDestinoId', container).val('0');
        $('.hdnDestinoIsAtivo', container).val('true');
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
                } else if (response.Msg && response.Msg.length > 0) {
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

        $($('.tabDestinos tbody tr:not(.trTemplateRow) .hdnItemJSon', ConfigurarProdutosDestinos.container)).each(function () {
            lista.push(JSON.parse($(this).val()));
        });

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
            Excluir: false,
            Editado: true
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
            Excluir: false,
            Editado: true
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

}
/// <reference path="../jquery-1.4.3-vsdoc.js" /> 
/// <reference path="../jquery.json - 2.2.min.js" /> 
/// <reference path="../masterpage.js" /> 
/// <reference path="../jquery.ddl.js" /> 
 
ConfigurarCodigosReceita = { 
    settings: { 
        urls: { 
            salvar: ''
        }, 
        Mensagens: null 
    }, 
 
    container: null, 
 
    load: function (container, options) { 
 
        if (options) { $.extend(ConfigurarCodigosReceita.settings, options); } 
        ConfigurarCodigosReceita.container = MasterPage.getContent(container); 
 
        container.delegate('.btnAdicionarCodigoReceita', 'click', ConfigurarCodigosReceita.adicionarCodigoReceita);
        container.delegate('.btnSalvar', 'click', ConfigurarCodigosReceita.salvar); 
        container.delegate('.btnEditarCodigoReceita', 'click', ConfigurarCodigosReceita.editarCodigoReceita); 
        container.delegate('.btnDesativarCodigoReceita', 'click', ConfigurarCodigosReceita.desativarCodigoReceita); 
        container.delegate('.btnAtivarCodigoReceita', 'click', ConfigurarCodigosReceita.ativarCodigoReceita); 
        container.delegate('.btnExcluirCodigoReceita', 'click', ConfigurarCodigosReceita.excluirCodigoReceita);
 
        Listar.atualizarEstiloTable('.tabCodigosReceita', ConfigurarCodigosReceita.container)

        //Coloca a máscara no texbox Código da Receita
        $(".txtCodigo").mask("999-9");

        Aux.setarFoco(container); 
    },
 
    adicionarCodigoReceita: function () { 
        Mensagem.limpar(ConfigurarCodigosReceita.container); 
        var mensagens = new Array(); 
 
        //Pega os campos para adicionar na tabela 
        var container = $(this).closest('fieldset'); 
        var codigo = container.find('.txtCodigo').val().toString().trim(); 
        var descricao = container.find('.txtDescricao').val().toString().trim(); 
        var id = container.find('.hdnItemId').val(); 
        var ehAtivo = container.find('.hdnItemIsAtivo').val(); 
 
        //Verifica se os campos foram preenchidos 
        if (codigo.length == 0) { 
            $('.txtCodigo', container).addClass('erroCodigo'); 
            mensagens.push(ConfigurarCodigosReceita.settings.Mensagens.CodigoReceitaObrigatorio);
        } 
        if (descricao.length == 0) { 
            $('.txtDescricao', container).addClass('erroDescricao'); 
            mensagens.push(ConfigurarCodigosReceita.settings.Mensagens.DescricaoCodigoObrigatoria); 
        } 
        if (ConfigurarCodigosReceita.publicarMensagem(mensagens)) { 
            return false; 
        } 
 
        //Verifica se o CodigoReceita já existe na tabela 
        var tabelaCodigosReceita = $('.tabCodigosReceita tbody tr', container);
        $(tabelaCodigosReceita).each(function (i, cod) { 
 
            /*Aqui, além de comparar codigo e descriçãoo, ele verifica se o id do CodigoReceita na linha 
            é igual ao que está sendo adicionado, porque pode se tratar de um CodigoReceita editado*/ 
            if ($('.codigo', cod).text().toLowerCase() == codigo.toLowerCase()
                  && $('.descricao', cod).text().toLowerCase() == descricao.toLowerCase()
                  && ((id != 0 && $('.codigoReceitaId', cod).val() != id) 
                      || id == 0)) { 
                mensagens.push(ConfigurarCodigosReceita.settings.Mensagens.CodigoReceitaDuplicado); 
            } 
        }); 
        if (ConfigurarCodigosReceita.publicarMensagem(mensagens)) { 
            return false; 
        } 
 
        //monta o objeto 
        var objeto = { 
            Id: id, 
            Tid: '', 
            Codigo: codigo, 
            Descricao: descricao, 
            Ativo: ehAtivo, 
            Excluir: false, 
            Editado: false 
        }; 
 
        var linha = ''; 
        if (objeto.Id == 0) {   //CodigoReceita novo 
            linha = $('.trTemplateRow', container).clone(); 
 
            //Monta a nova linha e insere na tabela 
            linha.find('.hdnItemJSon').val(JSON.stringify(objeto)); 
            linha.find('.codigo').text(codigo); 
            linha.find('.codigo').attr('title', codigo); 
            linha.find('.descricao').text(descricao); 
            linha.find('.descricao').attr('title', descricao); 
            linha.find('.codigoReceitaId').val(id); 
            linha.find('.codigoReceitaAtivo').val(ehAtivo); 
 
            linha.removeClass('trTemplateRow hide'); 
            $('.tabCodigosReceita > tbody:last', container).append(linha); 
 
        } else {    //CodigoReceita editado 
            objeto.Editado = true;
            $(tabelaCodigosReceita).each(function (i, cod) {
 
                //Procura a linha que tem o mesmo id do CodigoReceita 
                if ($('.codigoReceitaId', cod).val() == id) { 
 
                    //Edita a linha 
                    $('.hdnItemJSon', cod).val(JSON.stringify(objeto)); 
                    $('.codigo', cod).text(codigo); 
                    $('.codigo', cod).attr('title', codigo); 
                    $('.descricao', cod).text(descricao); 
                    $('.descricao', cod).attr('title', descricao); 
                    $('.codigoReceitaId', cod).val(id); 
                    $('.codigoReceitaAtivo', cod).val(ehAtivo); 
                } 
            }); 
        } 
 
        Listar.atualizarEstiloTable($('.tabCodigosReceita', container)); 
 
        //limpa os campos de texto 
        $('.txtCodigo', container).val(''); 
        $('.txtDescricao', container).val(''); 
        $('.hdnItemId', container).val('0'); 
        $('.hdnItemIsAtivo', container).val('true'); 
    },
 
    publicarMensagem: function (mensagens) { 
        if (mensagens.length > 0) { 
            Mensagem.gerar(ConfigurarCodigosReceita.container, mensagens) 
            return true; 
        } 
        return false; 
    }, 
 
    salvar: function () { 
        Mensagem.limpar(ConfigurarCodigosReceita.container); 
        MasterPage.carregando(true); 
 
        $.ajax({ 
            url: ConfigurarCodigosReceita.settings.urls.salvar, 
            data: JSON.stringify({ 
                listaCodigosReceita: ConfigurarCodigosReceita.obterListaCodigoReceitas()
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
                    Mensagem.gerar(ConfigurarCodigosReceita.container, response.Msg); 
                } 
            } 
        }); 
 
        MasterPage.carregando(false); 
    }, 
 
    obterListaCodigoReceitas: function () { 
        var lista = []; 
 
        $($('.tabCodigosReceita tbody tr:not(.trTemplateRow) .hdnItemJSon', ConfigurarCodigosReceita.container)).each(function () { 
            lista.push(JSON.parse($(this).val())); 
        }); 
 
        return lista; 
    },
 
    editarCodigoReceita: function () { 
 
        //Pega os campos que serão editados, e o id 
        var container = $(this).closest('tr'); 
        var codigo = container.find('.codigo').text(); 
        var descricao = container.find('.descricao').text(); 
        var id = container.find('.codigoReceitaId').val(); 
        var ehAtivo = container.find('.codigoReceitaAtivo').val(); 
 
        //preenche os textbox 
        container = $(this).closest('fieldset'); 
        container.find('.txtCodigo').val(codigo); 
        container.find('.txtDescricao').val(descricao); 
        container.find('.hdnItemId').val(id); 
        container.find('.hdnItemIsAtivo').val(ehAtivo); 
    },
 
    ativarCodigoReceita: function () { 
        //recria o objeto 
        var objeto = { 
            Id: $(this).closest('tr').find('.codigoReceitaId').val(), 
            Tid: '', 
            Codigo: $(this).closest('tr').find('.codigo').text(), 
            Descricao: $(this).closest('tr').find('.descricao').text(), 
            Ativo: true, 
            Excluir: false, 
            Editado: true 
        }; 
 
        //desabilita o botão de ativar 
        $(this).closest('tr').find('.btnAtivarCodigoReceita').attr('disabled', 'disabled'); 
        $(this).closest('tr').find('.btnAtivarCodigoReceita').attr('aria-disabled', true); 
        $(this).closest('tr').find('.btnAtivarCodigoReceita').addClass('disabled').addClass('ui-button-disabled').addClass('ui-state-disabled'); 
 
        //habilita o botão de desativar 
        $(this).closest('tr').find('.btnDesativarCodigoReceita').removeAttr('disabled'); 
        $(this).closest('tr').find('.btnDesativarCodigoReceita').removeAttr('aria-disabled'); 
        $(this).closest('tr').find('.btnDesativarCodigoReceita').removeClass('disabled').removeClass('ui-button-disabled').removeClass('ui-state-disabled'); 
 
        //altera o valor da propriedade no objeto 
        $(this).closest('tr').find('.hdnItemJSon').val(JSON.stringify(objeto)); 
        $(this).closest('tr').find('.codigoReceitaAtivo').val(objeto.Ativo); 
    },
 
    desativarCodigoReceita: function () { 
        //recria o objeto 
        var objeto = { 
            Id: $(this).closest('tr').find('.codigoReceitaId').val(), 
            Tid: '', 
            Codigo: $(this).closest('tr').find('.codigo').text(), 
            Descricao: $(this).closest('tr').find('.descricao').text(), 
            Ativo: false, 
            Excluir: false, 
            Editado: true 
        }; 
 
        //desabilita o botão de desativar 
        $(this).closest('tr').find('.btnDesativarCodigoReceita').attr('disabled', 'disabled'); 
        $(this).closest('tr').find('.btnDesativarCodigoReceita').attr('aria-disabled', true); 
        $(this).closest('tr').find('.btnDesativarCodigoReceita').addClass('disabled').addClass('ui-button-disabled').addClass('ui-state-disabled'); 
 
        //habilita o botão de ativar 
        $(this).closest('tr').find('.btnAtivarCodigoReceita').removeAttr('disabled'); 
        $(this).closest('tr').find('.btnAtivarCodigoReceita').removeAttr('aria-disabled'); 
        $(this).closest('tr').find('.btnAtivarCodigoReceita').removeClass('disabled').removeClass('ui-button-disabled').removeClass('ui-state-disabled'); 
 
        //altera o valor da propriedade no objeto 
        $(this).closest('tr').find('.hdnItemJSon').val(JSON.stringify(objeto)); 
        $(this).closest('tr').find('.codigoReceitaAtivo').val(objeto.Ativo); 
    },
 
    excluirCodigoReceita: function () { 
        //recria o objeto 
        var objeto = { 
            Id: $(this).closest('tr').find('.codigoReceitaId').val(), 
            Tid: '', 
            Codigo: '', 
            Descricao: '', 
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
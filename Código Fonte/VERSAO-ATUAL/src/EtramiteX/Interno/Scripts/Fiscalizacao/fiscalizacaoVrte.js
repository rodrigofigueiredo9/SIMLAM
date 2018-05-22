/// <reference path="../jquery-1.4.3-vsdoc.js" /> 
/// <reference path="../jquery.json - 2.2.min.js" /> 
/// <reference path="../masterpage.js" /> 
/// <reference path="../jquery.ddl.js" /> 
 
ConfigurarVrte = { 
    settings: { 
        urls: { 
            salvar: '',
            excluir: '',
        }, 
        Mensagens: null 
    }, 
 
    container: null, 
 
    load: function (container, options) { 
 
        if (options) { $.extend(ConfigurarVrte.settings, options); } 
        ConfigurarVrte.container = MasterPage.getContent(container); 
 
        container.delegate('.btnAdicionarVrte', 'click', ConfigurarVrte.adicionarVrte);
        container.delegate('.btnSalvar', 'click', ConfigurarVrte.salvar); 
        container.delegate('.btnEditarVrte', 'click', ConfigurarVrte.editarVrte); 
        container.delegate('.btnExcluirVrte', 'click', ConfigurarVrte.excluirVrte);
 
        Listar.atualizarEstiloTable('.tabVrte', ConfigurarVrte.container)

        //Coloca a máscara no texbox Ano
        $(".txtAno").mask("9999");

        Aux.setarFoco(container); 
    },
 
    adicionarVrte: function () { 
        Mensagem.limpar(ConfigurarVrte.container); 
        var mensagens = new Array(); 
 
        //Pega os campos para adicionar na tabela 
        var container = $(this).closest('fieldset'); 
        var ano = container.find('.txtAno').val().toString().trim(); 
        var vrte = container.find('.txtVrte').val().toString().trim(); 
        var id = container.find('.hdnItemId').val(); 
 
        //Verifica se os campos foram preenchidos 
        if (ano.length == 0) { 
            $('.txtAno', container).addClass('erroAno'); 
            mensagens.push(ConfigurarVrte.settings.Mensagens.VrteObrigatorio);
        } 
        if (vrte.length == 0) { 
            $('.txtVrte', container).addClass('erroVrte'); 
			mensagens.push(ConfigurarVrte.settings.Mensagens.AnoObrigatorio); 
        } 
        if (ConfigurarVrte.publicarMensagem(mensagens)) { 
            return false; 
        } 
 
        //Verifica se o Vrte já existe na tabela 
        var tabelaVrte = $('.tabVrte tbody tr', container);
        $(tabelaVrte).each(function (i, cod) { 
 
            /*Aqui, além de comparar ano e vrte, ele verifica se o id do Vrte na linha 
            é igual ao que está sendo adicionado, porque pode se tratar de um Vrte editado*/ 
            if ($('.ano', cod).text().toLowerCase() == ano.toLowerCase()
                  && $('.vrte', cod).text().toLowerCase() == vrte.toLowerCase()
                  && ((id != 0 && $('.VrteId', cod).val() != id) 
                      || id == 0)) { 
                mensagens.push(ConfigurarVrte.settings.Mensagens.VrteDuplicado); 
            } 
        }); 
        if (ConfigurarVrte.publicarMensagem(mensagens)) { 
            return false; 
        } 
 
        //monta o objeto 
        var objeto = { 
            Id: id, 
            Tid: '', 
            Ano: ano, 
            VrteEmReais: vrte, 
            Excluir: false, 
            Editado: false 
        }; 
 
        var linha = ''; 
        if (objeto.Id == 0) {   //Vrte novo 
            linha = $('.trTemplateRow', container).clone(); 
 
            //Monta a nova linha e insere na tabela 
            linha.find('.hdnItemJSon').val(JSON.stringify(objeto)); 
            linha.find('.ano').text(ano); 
            linha.find('.ano').attr('title', ano); 
            linha.find('.vrte').text(vrte); 
            linha.find('.vrte').attr('title', vrte); 
            linha.find('.VrteId').val(id); 
 
            linha.removeClass('trTemplateRow hide'); 
            $('.tabVrte > tbody:last', container).append(linha); 
 
        } else {    //Vrte editado 
            objeto.Editado = true;
            $(tabelaVrte).each(function (i, cod) {
 
                //Procura a linha que tem o mesmo id do Vrte 
                if ($('.VrteId', cod).val() == id) { 
 
                    //Edita a linha 
                    $('.hdnItemJSon', cod).val(JSON.stringify(objeto)); 
                    $('.ano', cod).text(ano); 
                    $('.ano', cod).attr('title', ano); 
                    $('.vrte', cod).text(vrte); 
                    $('.vrte', cod).attr('title', vrte); 
                    $('.VrteId', cod).val(id); 
                } 
            }); 
        } 
 
        Listar.atualizarEstiloTable($('.tabVrte', container)); 
 
        //limpa os campos de texto 
        $('.txtAno', container).val(''); 
        $('.txtVrte', container).val(''); 
        $('.hdnItemId', container).val('0'); 
    },
 
    publicarMensagem: function (mensagens) { 
        if (mensagens.length > 0) { 
            Mensagem.gerar(ConfigurarVrte.container, mensagens) 
            return true; 
        } 
        return false; 
    }, 
 
    salvar: function () { 
        Mensagem.limpar(ConfigurarVrte.container); 
        MasterPage.carregando(true); 
 
        $.ajax({ 
            url: ConfigurarVrte.settings.urls.salvar, 
            data: JSON.stringify({ 
                listaVrte: ConfigurarVrte.obterListaVrtes()
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
                    Mensagem.gerar(ConfigurarVrte.container, response.Msg); 
                } 
            } 
        }); 
 
        MasterPage.carregando(false); 
    }, 
 
    obterListaVrtes: function () { 
        var lista = []; 
 
        $($('.tabVrte tbody tr:not(.trTemplateRow) .hdnItemJSon', ConfigurarVrte.container)).each(function () { 
            lista.push(JSON.parse($(this).val())); 
        }); 
 
        return lista; 
    },
 
    editarVrte: function () { 
        //Pega os campos que serão editados, e o id 
        var container = $(this).closest('tr'); 
        var ano = container.find('.ano').text(); 
        var vrte = container.find('.vrte').text(); 
        var id = container.find('.VrteId').val(); 
 
        //preenche os textbox 
        container = $(this).closest('fieldset'); 
        container.find('.txtAno').val(ano); 
        container.find('.txtVrte').val(vrte); 
        container.find('.hdnItemId').val(id); 
    },
 
    excluirVrte: function () { 
        //recria o objeto 
        var objeto = { 
            Id: $(this).closest('tr').find('.VrteId').val(), 
            Tid: '', 
            Ano: '', 
			VrteEmReais: '', 
            Excluir: true 
        };

        if (objeto.Id != 0) {
            $(this).closest('tr').addClass('excluirLinha');

            Mensagem.limpar(ConfigurarVrte.container);
            MasterPage.carregando(true);

            $.ajax({
                url: ConfigurarVrte.settings.urls.excluir,
                data: JSON.stringify({
                    codigoExcluido: objeto
                }),
                cache: false,
                async: false,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                error: Aux.error,
                success: function (response, textStatus, XMLHttpRequest) {
                    if (response.EhValido) {
                        $('.excluirLinha').find('.hdnItemJSon').val(JSON.stringify(objeto));
                        $('.excluirLinha').hide();
                    } else if (response.Msg && response.Msg.length > 0) {
                        Mensagem.gerar(ConfigurarVrte.container, response.Msg);
                    }
                }
            });

            $(this).closest('tr').removeClass('excluirLinha');

            MasterPage.carregando(false);
        } else { 
            $(this).closest('tr').remove(); 
        } 
    },
 
}
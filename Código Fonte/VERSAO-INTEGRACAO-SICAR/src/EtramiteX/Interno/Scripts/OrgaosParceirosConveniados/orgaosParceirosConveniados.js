///<reference path="../masterpage.js" />
///<reference path="../jquery.json-2.2.min.js" />
OrgaosParceirosConveniados = {
    settings: {
        urls: {
            salvar: null,
            verificarCredenciadoAssociado: null
        },
        Mensagens: null
    },
    container: null,
    
    load: function (container, options) {
        
        if (options) {
            $.extend(OrgaosParceirosConveniados.settings, options);
            
            OrgaosParceirosConveniados.container = MasterPage.getContent(container);
            OrgaosParceirosConveniados.container.delegate('.btnAdicionarUnidade', 'click', OrgaosParceirosConveniados.onAdicionarUnidade);
            OrgaosParceirosConveniados.container.delegate('.btnExcluirUnidade', 'click', OrgaosParceirosConveniados.onExcluirUnidade);
            OrgaosParceirosConveniados.container.delegate('.btnSalvar', 'click', OrgaosParceirosConveniados.onSalvar);
        }
    },

    onAdicionarUnidade: function () {
        var mensagens = new Array();
        Mensagem.limpar(OrgaosParceirosConveniados.container);
        var container = $('.fsUnidade', OrgaosParceirosConveniados.container);

        var item = {
            Id: 0,
            Nome: $('.txtUnidadeNomeLocal', container).val(),
            Sigla: $('.txtUnidadeSigla', container).val()
        };
        
        if (jQuery.trim(item.Sigla) == '' && jQuery.trim(item.Nome) == '') {
            mensagens.push(jQuery.extend(true, {}, OrgaosParceirosConveniados.settings.Mensagens.UnidadeSiglaNomeLocalObrigatorio));
        }

        
        $('.hdnItemJSon', container).each(function () {
            var obj = String($(this).val());
            if (obj != '') {
                var itemAdd = (JSON.parse(obj));
                
                if (item.Sigla == itemAdd.Sigla && item.Nome == itemAdd.Nome) {
                    mensagens.push(jQuery.extend(true, {}, OrgaosParceirosConveniados.settings.Mensagens.UnidadeJaAdicionada));
                }
            }
        });

        if (mensagens.length > 0) {
            Mensagem.gerar(OrgaosParceirosConveniados.container, mensagens);
            return;
        }

        var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
        linha.find('.hdnItemJSon').val(JSON.stringify(item));
        linha.find('.NomeLocal').html(item.Nome).attr('title', item.Nome);
        linha.find('.Sigla').html(item.Sigla).attr('title', item.Sigla);

        $('.dataGridTable tbody:last', container).append(linha);
        Listar.atualizarEstiloTable(container.find('.dataGridTable'));

        $('.txtUnidadeNomeLocal', container).val('');
        $('.txtUnidadeSigla', container).val('');
    },

    onExcluirUnidade: function () {
        var container = $('.fsUnidade');
        var linha = $(this).closest('tr');
        var unidade = JSON.parse($('.hdnItemJSon', linha).val());
        
        $.ajax({
            url: OrgaosParceirosConveniados.settings.urls.verificarCredenciadoAssociado,
            data: JSON.stringify(unidade),
            async: false,
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            error: function (XMLHttpRequest, textStatus, erroThrown) {
                Aux.error(XMLHttpRequest, textStatus, erroThrown, OrgaosParceirosConveniados.container);
            },
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    Modal.confirma({
                        btnOkCallback: function (modalContent) {
                        	linha.remove();
                            Listar.atualizarEstiloTable(container.find('.dataGridTable'));
                            Modal.fechar(modalContent);
                            return;
                        },
                       conteudo: OrgaosParceirosConveniados.settings.Mensagens.ConfirmExcluirUnidade.Texto.toString().replace('#siglaUnidade#', unidade.Sigla +' - '+ unidade.Nome),
                       titulo :'Deseja excluir?'
                    });
                }
                if (response.Msg && response.Msg.length > 0) {
                    Mensagem.gerar(OrgaosParceirosConveniados.container, response.Msg);
                }
            }
        });
    },
    
    obterObjeto: function () {
        var Orgao = {};
        Orgao.Unidades = [];
        
        Orgao.Id = $('.hdnOrgaoParceiroId', OrgaosParceirosConveniados.container).val();
        Orgao.Sigla = $('.txtSigla', OrgaosParceirosConveniados.container).val();
        Orgao.Nome = $('.txtNomeOrgao', OrgaosParceirosConveniados.container).val();
        Orgao.TermoNumeroAno = $('.txtTermoCooperacaoNumeroAno', OrgaosParceirosConveniados.container).val();
        Orgao.DiarioOficialData = { DataTexto: $('.txtTermoCooperacaoData', OrgaosParceirosConveniados.container).val()};

        var unidadesContainer = $('.fsUnidade', OrgaosParceirosConveniados.container);
        $('.hdnItemJSon', unidadesContainer).each(function () {
            var objUnidade = String($(this).val());
            if (objUnidade != '') {
                Orgao.Unidades.push(JSON.parse(objUnidade));
            }
        });

        return Orgao;
    },

    onSalvar: function () {
        $.ajax({
            url: OrgaosParceirosConveniados.settings.urls.salvar,
            data: JSON.stringify(OrgaosParceirosConveniados.obterObjeto()),
            async: false,
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            error: function (XMLHttpRequest, textStatus, erroThrown) {
                Aux.error(XMLHttpRequest, textStatus, erroThrown, OrgaosParceirosConveniados.container);
            },
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido){
                    MasterPage.redireciona(response.Url)
                    return;
                }

                if (response.Msg && response.Msg.length > 0) {
                    Mensagem.gerar(OrgaosParceirosConveniados.container, response.Msg);
                }
            }
        });
    },
       
}
/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

SilviculturaPPFF = {
    settings: {
        urls: {
            salvar: '',
            editar: '',
            visualizar: ''
        },
        salvarCallBack: null,
        mensagens: {}
    },
    container: null,

    load: function (container, options) {
        if (options) { $.extend(SilviculturaPPFF.settings, options); }
        SilviculturaPPFF.container = MasterPage.getContent(container);

        SilviculturaPPFF.container.delegate('.btnSalvar', 'click', SilviculturaPPFF.salvar);
        SilviculturaPPFF.container.delegate('.btnAdicionarMunicipio', 'click', SilviculturaPPFF.adicionarMunicipio);
        SilviculturaPPFF.container.delegate('.btnExcluirMunicipio', 'click', SilviculturaPPFF.excluirMunicipio);

    },

    adicionarMunicipio: function () {
        var mensagens = new Array();
        Mensagem.limpar(SilviculturaPPFF.container);
        var container = $('.divMunicipios');

        var item = { Municipio: { Id: $('.ddlMunicipio :selected', container).val(), Tid: '', Texto: $('.ddlMunicipio :selected', container).text()} };

        if (item.Municipio.Id == 0) {
            mensagens.push(jQuery.extend(true, {}, SilviculturaPPFF.settings.mensagens.MunicipioObrigatorio));
        }

        $('.hdnItemJSon', container).each(function () {
            var obj = String($(this).val());
            if (obj != '') {
                var itemAdd = (JSON.parse(obj));
                if (item.Municipio.Texto == itemAdd.Municipio.Texto) {
                    mensagens.push(jQuery.extend(true, {}, SilviculturaPPFF.settings.mensagens.MunicipioDuplicado));
                }
            }
        });

        if (mensagens.length > 0) {
            Mensagem.gerar(SilviculturaPPFF.container, mensagens);
            return;
        }

        var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
        linha.find('.hdnItemJSon').val(JSON.stringify(item));
        linha.find('.municipio').html(item.Municipio.Texto).attr('title', item.Municipio.Texto);

        $('.dataGridTable tbody:last', container).append(linha);
        Listar.atualizarEstiloTable(container.find('.dataGridTable'));

        $('.ddlMunicipio', container).ddlFirst();
    },

    excluirMunicipio: function () {
        var container = $('.divMunicipios');
        var linha = $(this).closest('tr');
        linha.remove();
        Listar.atualizarEstiloTable(container.find('.dataGridTable'));
    },

    obter: function () {
        var container = SilviculturaPPFF.container;
        var obj = {
            Id: $('.hdnCaracterizacaoId', container).val(),
            EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
            Atividade: $('.ddlAtividade :selected', container).val(),
            FomentoTipo: $('.rboFomentoTipo:checked', container).val(),
            AreaTotal: $('.txtAreaTotal', container).val(),
            Itens: []
        };

        //Municipios
        container = SilviculturaPPFF.container.find('.divMunicipios');
        $('.hdnItemJSon', container).each(function () {
            var objMunicipio = String($(this).val());
            if (objMunicipio != '') {
                obj.Itens.push(JSON.parse(objMunicipio));
            }
        });

        return obj;
    },

    salvar: function () {
        MasterPage.carregando(true);
        $.ajax({ url: SilviculturaPPFF.settings.urls.salvar,
            data: JSON.stringify(SilviculturaPPFF.obter()),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: function (XMLHttpRequest, textStatus, erroThrown) {
                Aux.error(XMLHttpRequest, textStatus, erroThrown, SilviculturaPPFF.container);
            },
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    MasterPage.redireciona(response.UrlRedirecionar);
                }
                if (response.Msg && response.Msg.length > 0) {
                    Mensagem.gerar(SilviculturaPPFF.container, response.Msg);
                }
            }
        });
        MasterPage.carregando(false);
    }
}
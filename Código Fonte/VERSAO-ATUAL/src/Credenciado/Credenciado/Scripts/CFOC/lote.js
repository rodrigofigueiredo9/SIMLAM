/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.ddl.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />

Lote = {
    settings: {
        urls: {
            urlEmpreendimento: null,
            urlVefiricar: null,
            associarCultura: null,
            urlAdicionarLoteItem: null,
            urlObterCultivar: null,
            urlUnidadeMedida: null,
            urlSalvar: null
        },
        idsTela: null,
        Mensagens: null
    },
    container: null,
    UCId: null,
    UCCodigo: null,

    load: function (container, options) {
        if (options) {
            $.extend(Lote.settings, options);
        }

        Lote.container = MasterPage.getContent(container);
        container.delegate('.ddlEmpreemdimento', 'change', Lote.changeEmpreendimento);
        container.delegate('.ddlOrigem', 'change', Lote.changeDocumentoOrigem);
        $('.divNumeroEnter', Lote.container).keyup(Lote.verificarNumeroEnter);

        container.delegate('.btnVerificar', 'click', Lote.verificar);
        container.delegate('.btnAssociarCultura', 'click', Lote.associarCultura);
        container.delegate('.ddlCultura', 'change', Lote.changeCultura);
        container.delegate('.ddlCultivar', 'change', Lote.changeCultivar);
        container.delegate('.btnAdicionar', 'click', Lote.adicionar);
        container.delegate('.btnExcluir', 'click', Lote.excluir);
        container.delegate('.btnSalvar', 'click', Lote.salvar);

        if ($('.ddlEmpreemdimento', Lote.container).val() != 0 && $('.ddlOrigem', Lote.container).length != 0) {
            $('.ddlEmpreemdimento', Lote.container).change();
        }
    },

    verificarNumeroEnter: function (e) {
        if (e.keyCode == MasterPage.keyENTER) {
            $('.btnVerificar', Lote.container).click();
        }
        return false;
    },

    changeEmpreendimento: function () {
        Mensagem.limpar(Lote.container);
        if ($('.rbNumeroBloco', Lote.container).is(':checked')) {
            return;
        }

        var empreendimetoId = $('.ddlEmpreemdimento', Lote.container).val();
        MasterPage.carregando(true);
        $.ajax({
            url: Lote.settings.urls.urlEmpreendimento,
            data: JSON.stringify({ empreendimentoID: empreendimetoId }),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    Lote.UCId = response.Caracterizacao.Id;
                    Lote.UCCodigo = response.Caracterizacao.Texto;
                    $('.hdnUCId', Lote.container).val(response.Caracterizacao.Id);
                    $('.txtUCCodigo', Lote.container).val(response.Caracterizacao.Texto);
                }

                if (response.Msg && response.Msg.length > 0) {
                    Mensagem.gerar(Lote.container, response.Msg);
                }
            }
        });
        MasterPage.carregando(false);
    },

    changeDocumentoOrigem: function () {
        $('.txtNumeroOrigem', Lote.container).unmask().val('');
        $('.divAdicionar', Lote.container).find('input[type=text], input[type=hidden]').val('');
        $('.divAdicionar', Lote.container).find('select').ddlClear();
        var origem = $('.ddlOrigem', Lote.container).ddlSelecionado();

        if (origem.Id == Lote.settings.idsTela.TipoCFCFR || origem.Id == Lote.settings.idsTela.TipoTF) {
            $('.divVerificarNumero', Lote.container).addClass('hide');
            $('.divObterCultura', Lote.container).removeClass('hide');
            $('.txtNumeroOrigem', Lote.container).attr("maxlength", 20);

            $('.txtQuantidade', Lote.container).removeClass('disabled');
            $('.txtQuantidade', Lote.container).removeAttr('disabled');
            //$('label[for="Quantidade"]').show();
        }
        else {
            $('.divVerificarNumero', Lote.container).removeClass('hide');
            $('.divObterCultura', Lote.container).addClass('hide');
            $('.txtNumeroOrigem', Lote.container).attr("maxlength", 10);

            $('.txtQuantidade', Lote.container).addClass('disabled');
            $('.txtQuantidade', Lote.container).attr('disabled', 'disabled');
            //$('label[for="Quantidade"]').hide();


        }

        if (origem.Id == Lote.settings.idsTela.TipoCFO) {

            $('.txtQuantidade', Lote.container).removeClass('disabled');
            $('.txtQuantidade', Lote.container).removeAttr('disabled');
        }

        Mascara.load($('.txtNumeroOrigem', Lote.container).closest('div'));
    },

    verificar: function () {
        Mensagem.limpar(Lote.container);
        var origemTipo = $('.ddlOrigem', Lote.container).val();
        var numero = $('.txtNumeroOrigem', Lote.container).val();

        MasterPage.carregando(true);
        $.ajax({
            url: Lote.settings.urls.urlVefiricar,
            data: JSON.stringify({ origemTipo: origemTipo, origemNumero: numero }),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    $('.hdnOrigemID', Lote.constructor).val(response.OrigemID);
                    $('.ddlCultura', Lote.container).ddlLoad(response.Culturas);
                    $('.ddlCultura', Lote.container).removeAttr('disabled').removeClass('disabled');
                    if ($('.ddlCultura', Lote.container).val() != '0') {
                        $('.ddlCultura', Lote.container).change();
                    }
                }



                if (response.Msg && response.Msg.length > 0) {
                    Mensagem.gerar(Lote.container, response.Msg);
                }
            }
        });
        MasterPage.carregando(false);
    },

    associarCultura: function () {
        Modal.abrir(Lote.settings.urls.associarCultura, null, function (container) {
            CulturaListar.load(container, { onAssociarCallback: Lote.callBackAssociarCultura });
            Modal.defaultButtons(container);
        });
    },

    callBackAssociarCultura: function (response) {
        $('.txtCultura', Lote.container).val(response.Nome);
        $('.hdnCulturaId', Lote.container).val(response.Id);
        $('.ddlCultura', Lote.container).append(new Option(response.Nome, response.Id, true, true));

        if (response.Id != '0') {
            $('.ddlCultura', Lote.container).change();
        }
        return true;
    },

    changeCultura: function () {
        Mensagem.limpar(Lote.container);
        var origemTipo = $('.ddlOrigem', Lote.container).val();
        var origemID = $('.hdnOrigemID', Lote.container).val() || 0;
        var culturaID = $('.ddlCultura', Lote.container).val();

        MasterPage.carregando(true);
        $.ajax({
            url: Lote.settings.urls.urlObterCultivar,
            data: JSON.stringify({ origemTipo: origemTipo, origemID: origemID, culturaID: culturaID }),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    $('.ddlCultivar', Lote.container).ddlLoad(response.Cultivar);
                    $('.ddlCultivar', Lote.container).removeAttr('disabled').removeClass('disabled');
                    if ($('.ddlCultivar', Lote.container).val() != '0') {
                        $('.ddlCultivar', Lote.container).change();
                    }
                }

                if (response.Erros && response.Erros.length > 0) {
                    Mensagem.gerar(Lote.container, response.Erros);
                }
            }
        });
        MasterPage.carregando(false);
    },

    changeCultivar: function () {
        Mensagem.limpar(Lote.container);
        var origemTipo = +$('.ddlOrigem', Lote.container).val();
        var origemID = +$('.hdnOrigemID', Lote.container).val() || 0;
        var culturaID = +$('.ddlCultura', Lote.container).val();

        MasterPage.carregando(true);
        $.ajax({
            url: Lote.settings.urls.urlUnidadeMedida,
            data: JSON.stringify({ origemTipo: origemTipo, origemID: origemID, culturaID: culturaID, cultivarID: +$(this).val() }),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    $('.ddlUnidadeMedida', Lote.container).ddlLoad(response.Lista);

                    var quantidade = JSON.stringify(response.QtdDocOrigem).replace('.', ',');


                    if (origemTipo != Lote.settings.idsTela.TipoCFO) {

                        $('.txtQuantidade', Lote.container).val(quantidade);
                        $('.txtQuantidade', Lote.container).change();
                    }

                    if (origemTipo == Lote.settings.idsTela.TipoCFCFR || origemTipo == Lote.settings.idsTela.TipoTF)
                        $('.ddlUnidadeMedida', Lote.container).removeAttr('disabled').removeClass('disabled');
                    // $('.ddlUnidadeMedida', Lote.container).attr('disabled', 'disabled').addClass('disabled');
                }

                if (response.Msg && response.Msg.length > 0) {
                    Mensagem.gerar(Lote.container, response.Msg);
                }
            }
        });
        MasterPage.carregando(false);
    },

    adicionar: function () {
        Mensagem.limpar(Lote.container);

        var container = $(this).closest('fieldset');
        var tabela = $('.gridLote', container);
        var DataCriacao = { DataTexto: $('.txtDataCriacao', Lote.container).val() };
        var IdEmpreendimeto = $('.ddlEmpreemdimento', Lote.container).val();
        var unidadeMedida = $('.ddlUnidadeMedida', container).ddlSelecionado();

        var txtUnidMedida = $('.ddlUnidadeMedida :selected', container).text();

        var bExibeKg = txtUnidMedida.indexOf("KG") >= 0;

        var item = {
            Numero: $('.txtNumeroOrigem', container).val(),
            Origem: $('.hdnOrigemID', container).val() || 0,
            OrigemNumero: $('.txtNumeroOrigem', container).val(),
            OrigemTipo: $('.ddlOrigem', container).val(),
            OrigemTipoTexto: $('.ddlOrigem option:selected', container).text(),
            Cultura: $('.ddlCultura', container).val(),
            CulturaTexto: $('.ddlCultura option:selected', container).text(),
            Cultivar: $('.ddlCultivar', container).val(),
            CultivarTexto: $('.ddlCultivar option:selected', container).text(),
            UnidadeMedida: unidadeMedida.Id,
            Quantidade: $('.txtQuantidade', container).val(),
            ExibeKg: bExibeKg
        };

        //Valida Item já adicionado na Grid		
        var _objeto = { Lotes: [] }
        $($('.gridLote tbody tr:not(.trTemplate) .hdnItemJson', container)).each(function () {
            _objeto.Lotes.push(JSON.parse($(this).val()));
        }); 1

        if (_objeto.Lotes.length <= 0) {
            _objeto.Lotes = null;
        }

        var Ret = MasterPage.validarAjax(
			Lote.settings.urls.urlAdicionarLoteItem,
			{ item: item, dataCriacao: DataCriacao, empreendimentoID: IdEmpreendimeto, lista: _objeto.Lotes, loteID: $('.hdLoteId', Lote.container).val() },
			Lote.container, false);

        if (!Ret.EhValido) {
            return;
        }


        if (item.OrigemTipo < 5 && item.OrigemTipo != 1)
            item.Quantidade = Ret.ObjResponse.QtdDocOrigem;

        var linha = $('.trTemplate', tabela).clone();
        $(linha).removeClass('hide trTemplate');

        $('.hdnItemJson', linha).val(JSON.stringify(item));
        $('.lblOrigem', linha).html(item.OrigemTipoTexto + '-' + item.OrigemNumero).attr('title', item.OrigemTipoTexto + '-' + item.OrigemNumero);
        $('.lblCultivar', linha).html(item.CulturaTexto + ' ' + item.CultivarTexto).attr('title', item.CulturaTexto + ' ' + item.CultivarTexto);
        $('.lblQuantidade', linha).html(item.Quantidade).attr('title', item.Quantidade);
        $('.lblUnidadeMedida', linha).html(unidadeMedida.Texto).attr('title', unidadeMedida.Texto);

        $('tbody', tabela).append(linha);

        $('select', container).ddlFirst();
        $('.divAdicionar', container).find('select').ddlClear();
        container.find('.text').unmask().val('');
        Mascara.load(container);

        Listar.atualizarEstiloTable(tabela);
        //$('.ddlOrigem', container).focus();
    },

    excluir: function () {
        Mensagem.limpar(Lote.container);
        var container = $(this).closest('.dataGridTable');
        $(this).closest('tr').toggle(
			function () {
			    $(this).remove();
			});
        Listar.atualizarEstiloTable(container);
    },

    obter: function () {
        var objeto = {
            Id: $('.hdLoteId', Lote.container).val(),
            EmpreendimentoId: $('.ddlEmpreemdimento', Lote.container).val(),
            DataCriacao: { DataTexto: $('.txtDataCriacao', Lote.container).val() },
            CodigoUC: $('.txtUCCodigo', Lote.container).val(),
            Ano: $('.txtAno', Lote.container).val(),
            Numero: $('.txtNumero', Lote.container).val(),
            Lotes: []
        };

        $('.gridLote tbody tr:not(.trTemplate)', Lote.container).each(function (i, linha) {
            objeto.Lotes.push(JSON.parse($('.hdnItemJson', linha).val()));
        });

        for (var i = 0; i < objeto.Lotes.length; i++) {

            if (objeto.Lotes[i].ExibeKg) {
                objeto.Lotes[i].Quantidade = parseInt(objeto.Lotes[i].Quantidade) / 1000;
            }
        }

        return objeto;
    },

    salvar: function () {
        Mensagem.limpar(Lote.container);
        MasterPage.carregando(true);

        $.ajax({
            url: Lote.settings.urls.urlSalvar,
            data: JSON.stringify(Lote.obter()),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    if (response.Url) {
                        MasterPage.redireciona(response.Url);
                    }
                }

                if (response.Msg && response.Msg.length > 0) {
                    Mensagem.gerar(Lote.container, response.Msg);
                }
            }
        });

        MasterPage.carregando(false);
    }
}
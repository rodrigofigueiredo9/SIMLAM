﻿/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />

PTVOutroEmitir = {
    settings: {
        urls: {
            urlVerificarNumeroPTV: null,
            urlAssociarCultura: null,
            urlObterCultivar: null,
            urlObterMunicipio: null,
            urlAdicionarProdutos: null,
            urlObterDestinarioCodigoUc: null,
            urlAssociarDestinatario: null,
            urlObterDestinatario: null,
            urlSalvar: null,
            urlObterPragas: null,
            urlObterDeclaracaoAdicional: null,
            urlValidarPraga: null,
            urlObterDeclaracaoCombo: null
        }
    },
    container: null,

    ajax: function(request) {
        MasterPage.carregando(true);
        Mensagem.limpar(PTVOutroEmitir.container);
        $.ajax({
            url: request.url,
            data: JSON.stringify(request.data),
            cache: false,
            async: true,
            type: request.type ? request.type : 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function(response, textStatus, XMLHttpRequest) {
                try {
                    request.success(response, textStatus, XMLHttpRequest);
                } catch (e) {
                    throw e;
                } finally {
                    if (response.Erros && response.Erros.length > 0) {
                        Mensagem.gerar(PTVOutroEmitir.container, response.Erros);
                    } else {
                        Mensagem.gerar(PTVOutroEmitir.container, response.Msg);
                    }
                    MasterPage.carregando(false);
                }
            }
        });
    },

    load: function(container, options) {
        if (options) {
            $.extend(PTVOutroEmitir.settings, options);
        }

        PTVOutroEmitir.container = MasterPage.getContent(container);
        PTVOutroEmitir.container.delegate('.btnVerificarPTV', 'click', PTVOutroEmitir.onVerificarPTV);
        PTVOutroEmitir.container.delegate('.btnLimparPTV', 'click', PTVOutroEmitir.onLimparNumeroPTV);
        PTVOutroEmitir.container.delegate('.ddlOrigemTipo', 'change', PTVOutroEmitir.onChangeOrigemTipo);
        PTVOutroEmitir.container.delegate('.btnIdentificacaoProduto', 'click', PTVOutroEmitir.onAdicionarIdentificacaoProduto);
        PTVOutroEmitir.container.delegate('.btnExcluir', 'click', PTVOutroEmitir.onExcluirIdentificacaoProduto);
        PTVOutroEmitir.container.delegate('.btnAssociarCultura', 'click', PTVOutroEmitir.associarCultura);
        PTVOutroEmitir.container.delegate('.btnVerificarDestinatario', 'click', PTVOutroEmitir.onValidarDocumento);
        PTVOutroEmitir.container.delegate('.btnLimparDestinatario', 'click', PTVOutroEmitir.onLimparDestinatario);



        PTVOutroEmitir.container.delegate('.btnAddPraga', 'click', PTVOutroEmitir.addPraga);

        PTVOutroEmitir.container.delegate('.btnExcluirAnexo', 'click', PTVOutroEmitir.onExcluirLinha);
        PTVOutroEmitir.container.delegate('.btnDescerLinha', 'click', function (container) { PTVOutroEmitir.onBtnDescerClick(container, 'tabAnexos'); });
        PTVOutroEmitir.container.delegate('.btnSubirLinha', 'click', function (container) { PTVOutroEmitir.onBtnSubirClick(container, 'tabAnexos'); });
        PTVOutroEmitir.atualizaEstiloGrid('tabAnexos');

        PTVOutroEmitir.container.delegate('.rbTipoProducao ', 'change', function(item) {

            switch ($('.rbTipoProducao:checked', PTVOutroEmitir.container).val()) {
            case "1":
                $('.ddlProdutoUnidadeMedida', PTVOutroEmitir.container).val('2');
                break;

            case "2":
                $('.ddlProdutoUnidadeMedida', PTVOutroEmitir.container).val('1');
                break;

            case "3":
                $('.ddlProdutoUnidadeMedida', PTVOutroEmitir.container).val('3');
                break;
            };
        });

        PTVOutroEmitir.container.delegate('.ddlEstados', 'change', PTVOutroEmitir.onObterMunicipio);
        PTVOutroEmitir.container.delegate('.btnSalvar', 'click', PTVOutroEmitir.onSalvar);

        PTVOutroEmitir.container.delegate('input[name="CnpjCpf"]', 'click', PTVOutroEmitir.onClickRadioCnpjCpf);
        PTVOutroEmitir.container.delegate('.ddlEstadosInteressado', 'change', PTVOutroEmitir.onObterMunicipioInteressado);

        PTVOutroEmitir.container.delegate('.ddlPragas', 'change', PTVOutroEmitir.onSelPragra);

        if (parseInt($('.hdnID', PTVOutroEmitir.container).val()) > 0) {
            PTVOutroEmitir.habilitarCampos(false);
            $('.btnLimparPTV', PTVOutroEmitir.container).hide();
        } else {
            $('.ddlOrigemTipo', PTVOutroEmitir.container).val(2);
            $('.labelOrigem', PTVOutroEmitir.container).text('CFOC');
        }
        Aux.setarFoco(PTVOutroEmitir.container);
    },

    onLoadMunicipio: function(element, estadoId) {
        PTVOutroEmitir.ajax({
            url: PTVOutroEmitir.settings.urls.urlObterMunicipio,
            data: { estado: estadoId },
            success: function(response, textStatus, XMLHttpRequest) {
                element.ddlLoad(response.Municipios);
            }
        });
    },

    onClickRadioCnpjCpf: function() {
        var txtInteressadoCpfCnpj = $('.txtInteressadoCpfCnpj', PTVOutroEmitir.container);
        txtInteressadoCpfCnpj.val('');
        txtInteressadoCpfCnpj.removeAttr('disabled').removeClass('disabled');
        if ($(this).hasClass('rbTipoPessoaFisicaPF')) { /*1-CPF rbTipoPessoaFisica*/
            txtInteressadoCpfCnpj.unmask().mask("999.999.999-99");
        } else { /*2-CNPJ rbTipoPessoaJuridica*/
            txtInteressadoCpfCnpj.unmask().mask("99.999.999/9999-99");
        }
    },

   

    onObterMunicipioInteressado: function() {
        var ddl = PTVOutroEmitir.container.find('.ddlEstadosInteressado');
        var ddlSelecionado = ddl.ddlSelecionado();
        PTVOutroEmitir.onLoadMunicipio($('.ddlMunicipiosInteressado', PTVOutroEmitir.container), ddlSelecionado.Id);
    },

    onVerificarPTV: function() {
        var txtNumero = $('.txtNumero', PTVOutroEmitir.container);
        PTVOutroEmitir.ajax({
            url: PTVOutroEmitir.settings.urls.urlVerificarNumeroPTV,
            data: { numero: txtNumero.val() },
            success: function(response, textStatus, XMLHttpRequest) {
                if (response.Valido) {
                    var interessado = $('.interessado', PTVOutroEmitir.container);
                    $('input, select', interessado).removeClass('disabled').removeAttr('disabled');
                    $('.btnVerificarPTV', PTVOutroEmitir.container).addClass('hide');
                    $('.campoTela, .btnLimparPTV', PTVOutroEmitir.container).removeClass('hide');
                    txtNumero.addClass('disabled').attr('disabled', 'disabled');
                }
            }
        });
    },

    onLimparNumeroPTV: function() {
        $('.campoTela, .btnLimparPTV', PTVOutroEmitir.container).addClass('hide');
        $('.btnVerificarPTV', PTVOutroEmitir.container).removeClass('hide');
        $('.hdnEmpreendimentoID,.txtEmpreendimento', PTVOutroEmitir.container).val("");
        $('.ddlResponsaveis', PTVOutroEmitir.container).val("");
        $('.rbTipoNumero, .txtNumero', PTVOutroEmitir.container).removeClass('disabled').removeAttr('disabled');
        $(".rbTipoNumero", PTVOutroEmitir.container).attr("checked", false);
        $('.btnModalOu', PTVOutroEmitir.container).hide();
        $('.txtNumero', PTVOutroEmitir.container).val('');
        $('.ddlOrigemTipo', PTVOutroEmitir.container).ddlFirst();
        $('.ddlNumeroOrigem, .ddlProdutoCultura, .ddlProdutoCultivar, .ddlEstados, .ddlMunicipios', PTVOutroEmitir.container).ddlClear();
        $('.txtProdutoQuantidade', PTVOutroEmitir.container).val('');

        $('.txtValidade', PTVOutroEmitir.container).val('');
        $('.txtTecnico', PTVOutroEmitir.container).val('');
        $('.txtNumHab', PTVOutroEmitir.container).val('');

        $(".gridProdutos > tbody").html("");
        $(".gridPragas > tbody").html("");
        
        var interessado = $('.interessado', PTVOutroEmitir.container);
        $('input, select', interessado).addClass('disabled').attr('disabled', 'disabled').val('');
        $('input[type=radio]', interessado).each(function() { this.checked = false; });
    },

    onChangeOrigemTipo: function () {
        Mensagem.limpar(PTVOutroEmitir.container);
        var labelOrigem = $('.labelOrigem', PTVOutroEmitir.container);
        labelOrigem.text('');
        var option = $('option:selected', this);
        if (option.val() != '' && option.val() != '0') {
            labelOrigem.text(option.text());
        }
    },

    associarCultura: function() {
        Modal.abrir(PTVOutroEmitir.settings.urls.urlAssociarCultura, null, function(container) {
            CulturaListar.load(container, { onAssociarCallback: PTVOutroEmitir.callBackAssociarCultura });
            Modal.defaultButtons(container);
        });
    },

    callBackAssociarCultura: function(response) {

        $('.txtCultura', PTVOutroEmitir.container).val(response.Nome);
        $('.hdnCulturaId', PTVOutroEmitir.container).val(response.Id);
        $('.ddlProdutoCultura', PTVOutroEmitir.container).append(new Option(response.Nome, response.Id, true, true));

        if (response.Id != '0') {

            var origemTipo = $('.ddlOrigemTipo', PTVOutroEmitir.container).val();
            var culturaID = $('.ddlProdutoCultura', PTVOutroEmitir.container).val();

            PTVOutroEmitir.ajax({
                url: PTVOutroEmitir.settings.urls.urlObterCultivar,
                data: { origemTipo: origemTipo, culturaID: culturaID },
                success: function(response, textStatus, XMLHttpRequest) {
                    $('.ddlProdutoCultivar', PTVOutroEmitir.container).ddlLoad(response.Cultivar);
                }
            });
        }
        return true;
    },

    onAdicionarIdentificacaoProduto: function() {
        Mensagem.limpar(PTVOutroEmitir.container);
        var validacao = true;

        var container = $(this).closest('.identificacao_produto');
        var tabela = $('.gridProdutos', container);

        var NumeroOrigemTexto = $('.txtNumeroOrigem', container).val();

        var item = {
            ProducaoTipo: $('.rbTipoProducao:checked', container).val(),
            ProducaoTipoTexto: $('.rbTipoProducao' + $('.rbTipoProducao:checked', container).val(), container).text(),
            OrigemTipo: $('.ddlOrigemTipo', container).val(),
            OrigemTipoTexto: $('.ddlOrigemTipo option:selected', container).text(),
            OrigemNumero: $('.txtNumeroOrigem', container).val(),
            Cultura: $('.hdnCulturaId', container).val(),
            CulturaTexto: $('.txtCultura', container).val(),
            Cultivar: $('.ddlProdutoCultivar', container).val(),
            CultivarTexto: $('.ddlProdutoCultivar option:selected', container).text(),
            UnidadeMedida: $('.ddlProdutoUnidadeMedida option:selected', container).val(),
            UnidadeMedidaTexto: $('.ddlProdutoUnidadeMedida option:selected', container).text(),
            Quantidade: $('.txtProdutoQuantidade', container).val()
        };

        //Valida Item já adicionado na Grid	
        var _objeto = { Produtos: [] }
        $($('.gridProdutos tbody tr:not(.trTemplate) .hdnItemJson', container)).each(function() {
            _objeto.Produtos.push(JSON.parse($(this).val()));
        });

        if (_objeto.Produtos.length <= 0) {
            _objeto.Produtos = null;
        }

        var ehValido = MasterPage.validarAjax(PTVOutroEmitir.settings.urls.urlAdicionarProdutos, { item: item, lista: _objeto.Produtos }, PTVOutroEmitir.container, false).EhValido;
        if (!ehValido) {
            return;
        }

        var linha = $('.trTemplate', tabela).clone();
        $(linha).removeClass('hide trTemplate');

        //adicionar na grid
        $('.hdnItemJson', linha).val(JSON.stringify(item));

        $('.lblTipoProducao', linha).html(item.ProducaoTipoTexto).attr('title', item.ProducaoTipoTexto);
        $('.lblOrigemTipo', linha).html(item.OrigemTipoTexto + '-' + NumeroOrigemTexto).attr('title', item.OrigemTipoTexto + '-' + NumeroOrigemTexto);
        $('.lblCulturaCultivar', linha).html(item.CulturaTexto + '/' + item.CultivarTexto).attr('title', item.CulturaTexto + '/' + item.CultivarTexto);
        $('.lblQuantidade', linha).html(item.Quantidade).attr('title', item.Quantidade);
        $('.lblUnidadeMedida', linha).html(item.UnidadeMedidaTexto).attr('title', item.UnidadeMedidaTexto);

        $('tbody', tabela).append(linha);

        $('.ddlOrigemTipo', container).ddlFirst();
        $('.ddlProdutoCultivar', container).ddlClear({ disabled: false });
        $('input[type=text]', container).val("");

        Listar.atualizarEstiloTable(tabela);
        PTVOutroEmitir.carregarPragas();

        $('.ddlPragas').change();
    },

    onObterMunicipio: function() {
        var ddl = PTVOutroEmitir.container.find('.ddlEstados');
        var ddlSelecionado = ddl.ddlSelecionado();
        PTVOutroEmitir.onLoadMunicipio($('.ddlMunicipios', PTVOutroEmitir.container), ddlSelecionado.Id);
    },

    habilitarCampos: function(habilita) {
        if (habilitado) {
            $('.btnValidar', PTVOutroEmitir.container).show();
            $('.btnLimpar', PTVOutroEmitir.container).hide();
            $('.rbTipoDocumento', PTVOutroEmitir.container).removeAttr('disabled');
        } else {
            $('.btnValidar', PTVOutroEmitir.container).hide();
            $('.btnLimpar', PTVOutroEmitir.container).show();
            $('.block', PTVOutroEmitir.container).removeClass('hide');
            $('.rbTipoDocumento', PTVOutroEmitir.container).attr('disabled', 'disabled');
        }
    },

    obterIdentificacoes: function () {
        var retorno = [];
        $('.gridProdutos tbody tr:not(.trTemplate)', PTVOutroEmitir.container).each(function () {
            retorno.push(JSON.parse($('.hdnItemJson', this).val()));
        });

        return retorno;
    },

    carregarPragas: function () {
        $('.ddlPragas', PTVOutroEmitir.container).ddlClear();
        var produtos = PTVOutroEmitir.obterIdentificacoes();
        

        $.ajax({
            url: PTVOutroEmitir.settings.urls.urlObterPragas,
            data: JSON.stringify({ produtos: produtos }),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    $('.ddlPragas', PTVOutroEmitir.container).ddlLoad(response.Lista);
                } else {
                    Mensagem.gerar(PTVOutroEmitir.container, response.Msg);
                }
            }
        });

        /*$('.gridPragas tbody tr:not(.trTemplate)', PTVOutroEmitir.container).each(function () {
            var item = JSON.parse($('.hdnItemJson', this).val());

            var possui = false;
            $('.ddlPragas option', PTVOutroEmitir.container).each(function () {
                if ($(this).val() == item.Id) {
                    possui = true;
                }
            });

            if (!possui) {
                $(this).remove();
            }
        });*/
    },

    addPraga: function () {
        Mensagem.limpar(PTVOutroEmitir.container);
     
        var praga = $('.ddlPragas', PTVOutroEmitir.container).ddlSelecionado();
        var lista = PTVOutroEmitir.obterPragas();

        var declaracao = $('.ddlDeclaracaoAdicional', PTVOutroEmitir.container).ddlSelecionado();

        var produto;
        $('.gridProdutos tbody tr:last .hdnItemJson', PTVOutroEmitir.container).each(function () {
            produto = (JSON.parse($(this).val()));
        });

        

        var item = {
            IdPraga: praga.Id,
            NomeCientifico: praga.Texto.split('-')[0],
            NomeComum: praga.Texto.split('-')[1],
            DeclaracaoAdicional: declaracao.Texto,
            IdDeclaracao: declaracao.Id,
            IdCultivar: produto.Cultivar
        };

        //var retorno = MasterPage.validarAjax(PTVOutroEmitir.settings.urls.urlValidarPraga, { item: item, lista: lista }, null, false);

        //if (!retorno.EhValido && retorno.Msg) {
        //    Mensagem.gerar(PTVOutroEmitir.container, retorno.Msg);
        //    return;

        //}

   

        var tabela = $('.gridPragas', PTVOutroEmitir.container);
        var linha = $('.trTemplate', tabela).clone();

        $('.nome_cientifico', linha).text(item.NomeCientifico);
        $('.nome_comum', linha).text(item.NomeComum);
        $('.declaracao_adicional', linha).text(item.DeclaracaoAdicional);
        $('.hdnItemJson', linha).val(JSON.stringify(item));

        $('tbody', tabela).append(linha);
        $(linha).removeClass('hide').removeClass('trTemplate');

        Listar.atualizarEstiloTable(tabela);
        $('.ddlPragas', PTVOutroEmitir.container).ddlFirst();
        $('.ddlDeclaracaoAdicional', PTVOutroEmitir.container).ddlFirst();


       // PTVOutroEmitir.obterDeclaracaoAdicional();
    },

    obterPragas: function () {
        var retorno = [];
        $('.gridPragas tbody tr:not(.trTemplate)', PTVOutroEmitir.container).each(function () {
            retorno.push(JSON.parse($('.hdnItemJson', this).val()));
        });

        return retorno;
    },

    obterDeclaracoes: function () {
        var retorno = [];
        $('.gridPragas tbody tr:not(.trTemplate)', PTVOutroEmitir.container).each(function () {
            retorno.push(JSON.parse($('.hdnItemJson', this).val()));
        });

        return retorno;
    },

    onSelPragra: function () {

        var praga = $('.ddlPragas', PTVOutroEmitir.container).ddlSelecionado();
        
      

        var produto = [];
        $('.gridProdutos tbody tr:not(.trTemplate)', PTVOutroEmitir.container).each(function () {
            produto.push(JSON.parse($('.hdnItemJson', this).val()));
        });


        var produtoIds = [];

        for(var i = 0; i < produto.length; i++)
        {
         
            produtoIds.push(produto[i].Cultivar);
        }

       
            
        $.ajax({
            url: PTVOutroEmitir.settings.urls.urlObterDeclaracaoCombo,
            data: JSON.stringify({ pragaId: praga.Id, cultivarId : produtoIds }),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: function (XMLHttpRequest, textStatus, erroThrown) {
                Aux.error(XMLHttpRequest, textStatus, erroThrown, Cultura.container);
            },
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.Declaracoes) {
                    $('.ddlDeclaracaoAdicional', DeclaracaoAdicional.container).ddlLoad(response.Declaracoes, { disabled: false });
                }
            }
        });

    },

    obterDeclaracaoAdicional: function () {
        var produtos = PTVOutroEmitir.obterIdentificacoes();
        var pragas = PTVOutroEmitir.obterPragas();

        MasterPage.carregando(true);
        $.ajax({
            url: PTVOutroEmitir.settings.urls.urlObterDeclaracaoAdicional,
            data: JSON.stringify({ produtos: produtos, pragas: pragas }),
            cache: false,
            async: true,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    $('.txtDeclaracaoAdicional', PTVOutroEmitir.container).html(response.DeclaracoesAdicionais);
                }

                if (response.Msg && response.Msg.length > 0) {
                    Mensagem.gerar(PTVOutroEmitir.container, response.Msg);
                }
            }
        });
        MasterPage.carregando(false);
    },

    onExcluirIdentificacaoProduto: function () {

        var produto = JSON.parse($(this).closest('tr').find('.hdnItemJson').val());
      

        Mensagem.limpar(PTVOutroEmitir.container);
        var container = $(this).closest('.gridIdentificacaoProdutos');

      
       

        $(this).closest('tr').toggle(
            function () {
                
                $(this).remove();
            });

     

        Listar.atualizarEstiloTable(container);
    },

    onValidarDocumento: function() {
        Mensagem.limpar(PTVOutroEmitir.container);
        var container = $(this).closest('.destinatario');
        var codigoUc = parseInt($('.txtCodigoUC', container).val());

        MasterPage.carregando(true);
        $.ajax({
            url: PTVOutroEmitir.settings.urls.urlObterDestinarioCodigoUc,
            data: JSON.stringify({ codigoUc: codigoUc }),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function(response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    PTVOutroEmitir.callBackAssociarDestinatario(response.Destinatario);
                } else {
                    Mensagem.gerar(PTVOutroEmitir.container, response.Msg);
                }
            }
        });
        MasterPage.carregando(false);
    },

    obterDestinatario: function () {

        var Destinatario = {};

        Destinatario.ID = $('.hdnDestinatarioID', PTVOutroEmitir.container).val();
        Destinatario.CodigoUC = $('.txtCodigoUC', PTVOutroEmitir.container).val();
        Destinatario.CPFCNPJ = $('.txtDocumentoCpfCnpj', PTVOutroEmitir.container).val();
        Destinatario.NomeRazaoSocial = $('.txtNomeDestinatario', PTVOutroEmitir.container).val();
        Destinatario.Endereco = $('.txtEndereco', PTVOutroEmitir.container).val();
        Destinatario.EstadoTexto = $('.txtUF', PTVOutroEmitir.container).val();
        Destinatario.MunicipioTexto = $('.txtMunicipio', PTVOutroEmitir.container).val();
        Destinatario.EmpreendimentoId = $('.hdnEmpreendimentoID', PTVOutroEmitir.container).val();
        Destinatario.EstadoID = $('.hdnUfID', PTVOutroEmitir.container).val();
        Destinatario.MunicipioID = $('.hdnMunicipioID', PTVOutroEmitir.container).val();
        return Destinatario;
    },

	onLimparDestinatario: function () {
		var container = $(this).closest('.destinatario');
		$('.txtDocumentoCpfCnpj', container).val("");
		$('.txtNomeDestinatario', container).val("");
		$('.txtEndereco', container).val("");
		$('.txtUF', container).val("");
		$('.txtMunicipio', container).val("");
	    $('.txtCodigoUC', container).val("");
	    $('.btnLimparDestinatario, .destinatarioDados', container).addClass('hide');
	    $('.btnVerificarDestinatario', PTVOutroEmitir.container).removeClass('hide');
		$('.btnVerificarDestinatario', container).removeClass('hide');
		$('.txtCodigoUC', container).removeClass('disabled').removeAttr('disabled');
		$('.txtCodigoUC', container).focus();
	},

	onAssociarDestinatario: function () {
		Modal.abrir(PTVOutroEmitir.settings.urls.urlAssociarDestinatario, null, function (container) {
			DestinatarioPTV.load(container, {
				associarFuncao: PTVOutroEmitir.callBackAssociarDestinatario
			});
			Modal.defaultButtons(container, DestinatarioPTV.salvar, "Salvar");
		}, Modal.tamanhoModalMedia);
	},

	callBackAssociarDestinatario: function (destinatario) {
	    $('.hdnDestinatarioID', PTVOutroEmitir.container).val(destinatario.ID);
	    $('.hdnEmpreendimentoID', PTVOutroEmitir.container).val(destinatario.EmpreendimentoId);
		$('.txtNomeDestinatario', PTVOutroEmitir.container).val(destinatario.NomeRazaoSocial);
		$('.txtEndereco', PTVOutroEmitir.container).val(destinatario.Endereco);
		$('.txtUF', PTVOutroEmitir.container).val(destinatario.EstadoSigla);
		$('.txtMunicipio', PTVOutroEmitir.container).val(destinatario.MunicipioTexto);
		$('.txtDocumentoCpfCnpj', PTVOutroEmitir.container).val(destinatario.CPFCNPJ);
		$('.hdnUfID', PTVOutroEmitir.container).val(destinatario.EstadoID);
		$('.hdnMunicipioID', PTVOutroEmitir.container).val(destinatario.MunicipioID);
		$('.destinatarioDados', PTVOutroEmitir.container).removeClass('hide');
		$('.btnVerificarDestinatario', PTVOutroEmitir.container).addClass('hide');
		$('.btnLimparDestinatario', PTVOutroEmitir.container).removeClass('hide');
		$('.rbTipoDocumento, .txtDocumentoCpfCnpj, .txtCodigoUC', PTVOutroEmitir.container).addClass('disabled').attr('disabled', 'disabled');
	},

	onSalvar: function () {
		PTVOutroEmitir.ajax({
			url: PTVOutroEmitir.settings.urls.urlSalvar,
			data: PTVOutroEmitir.obter(),
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.Url);
				}
			}
		});
	},

	obter: function () {

	    var declaracoes = "";
	    /*$('.gridPragas tbody tr:not(.trTemplate)', PTVOutroEmitir.container).each(function () {
	        $('.declaracao_adicional', PTVOutroEmitir.container).each(function () {
	            declaracoes += $('.declaracao_adicional', PTVOutroEmitir.container).text().trim() + "|";
	        });
	    });*/

	    $($('.gridPragas tbody tr:not(.trTemplate) .declaracao_adicional', PTVOutroEmitir.container)).each(function () {
	        declaracoes += $(this).text().trim() + "|";
	    });


	   


		var objeto = {

			Id: $('.hdnEmissaoId', PTVOutroEmitir.container).val(),
			Numero: $('.txtNumero', PTVOutroEmitir.container).val(),
			DataEmissao: { DataTexto: $('.txtDataEmissao', PTVOutroEmitir.container).val() },
			Situacao: $('.ddlSituacoes', PTVOutroEmitir.container).val(),
			SituacaoTexto: $('.ddlSituacoes option:selected', PTVOutroEmitir.container).text(),
			DestinatarioID: $('.hdnDestinatarioID', PTVOutroEmitir.container).val(),
		    Destinatario: {},
			EmpreendimentoID: $('.hdnEmpreendimentoID', PTVOutroEmitir.container).val(),
			ValidoAte: { DataTexto: $('.txtDataValidade', PTVOutroEmitir.container).val() },
			LocalEmissaoId: $('.ddlLocalEmissao', PTVOutroEmitir.container).val(),
			Interessado: $('.txtInteressado', PTVOutroEmitir.container).val(),
			InteressadoCnpjCpf: $('.txtInteressadoCpfCnpj', PTVOutroEmitir.container).val(),
			InteressadoEndereco: $('.txtInteressadoEndereco', PTVOutroEmitir.container).val(),
			InteressadoEstadoId: $('.ddlEstadosInteressado', PTVOutroEmitir.container).val(),
			InteressadoEstadoTexto: $('.ddlEstadosInteressado option:selected', PTVOutroEmitir.container).text(),
			InteressadoMunicipioId: $('.ddlMunicipiosInteressado', PTVOutroEmitir.container).val(),
			InteressadoMunicipioTexto: $('.ddlMunicipiosInteressado option:selected', PTVOutroEmitir.container).text(),
			RespTecnico: $('.txtTecnico', PTVOutroEmitir.container).val(),
			RespTecnicoNumHab: $('.txtNumHab', PTVOutroEmitir.container).val(), 
			Estado: $('.ddlEstados', PTVOutroEmitir.container).val(),
			Municipio: $('.ddlMunicipios', PTVOutroEmitir.container).val(),
			DeclaracaoAdicional: declaracoes,
			Produtos: [],
			Declaracoes: [],
			Anexos: []
		}

		

		objeto.Destinatario = PTVOutroEmitir.obterDestinatario();

		objeto.Declaracoes = PTVOutroEmitir.obterDeclaracoes();

		var retorno = [];

		$('.gridProdutos tbody tr:not(.trTemplate)', PTVOutroEmitir.container).each(function () {
			retorno.push(JSON.parse($('.hdnItemJson', this).val()));
		});

		objeto.Produtos = retorno;

		$('.tabAnexos', PTVOutroEmitir.container).find('tbody tr').each(function (index, linha) {
		    objeto.Anexos.push({
		        Ordem: (index + 1),
		        Descricao: $(linha).find('.hdnAnexoDescricao').val(),
		        Arquivo: JSON.parse($(linha).find(".hdnAnexoArquivoJson").val())
		    });
		});


		return objeto;
	},

    //----------ANEXOS - ENVIAR ARQUIVO---------------

	onEnviarAnexoArquivoClick: function (url) {
	    if ($('.tabAnexos tbody tr', PTVOutroEmitir.container).length == 5) {
	        Mensagem.gerar(PTVOutroEmitir.container, [PTVOutroEmitir.settings.Mensagens.AnexoLimiteMaximo]);
	        return;
	    }

	    var nome = "enviando ...";
	    var nomeArquivo = $('.inputFile', PTVOutroEmitir.container).val();
	    var descricao = $('.txtAnexoDescricao', PTVOutroEmitir.container).val();
	    var tabAnexos = $('.tabAnexos', PTVOutroEmitir.container);
	    var trElem = $('tr', tabAnexos);

	    erroMsg = new Array();

	    if (nomeArquivo === '') {
	        erroMsg.push(PTVOutroEmitir.settings.Mensagens.ArquivoAnexoObrigatorio);
	    }

	    if (descricao === '') {
	        erroMsg.push(PTVOutroEmitir.settings.Mensagens.DescricaoAnexoObrigatorio);
	    }

	    if (nomeArquivo !== '' && descricao !== '') {
	        if (PTVOutroEmitir.existeAssociado(nomeArquivo, tabAnexos, "hdnArquivoNome")) {
	            erroMsg.push(PTVOutroEmitir.settings.Mensagens.ArquivoExistente);
	        }

	        var extensao = nomeArquivo.toLowerCase().substr(nomeArquivo.length - 4);
	        if (extensao !== ".pdf" && extensao !== ".jpg" && extensao !== ".png") {
	            erromsg.push(PTVOutroEmitir.settings.Mensagens.AnexoFormatoErrado);
	        }
	    }

	    if (erroMsg.length > 0) {
	        $('.txtAnexoDescricao', PTVOutroEmitir.container).addClass('erroCampo');
	        Mensagem.gerar(PTVOutroEmitir.container, erroMsg);
	        return;
	    }

	    var lastIndex = PTVOutroEmitir.buscarUltimoIndice(tabAnexos);
	    var linha = $('.trAnexoTemplate', PTVOutroEmitir.container).clone().removeClass('trAnexoTemplate');
	    var id = "ArquivoId_" + lastIndex;

	    linha.find('.hdnAnexoIndex').val(lastIndex).attr('name', 'PTV.Anexos.Index');
	    linha.find('.hdnArquivoNome').val(nomeArquivo).attr('name', 'PTV.Anexos[' + lastIndex + '].Arquivo.Nome');
	    linha.find('.hdnArquivoExtensao').val('').attr('name', 'PTV.Anexos[' + lastIndex + '].Extensao');
	    linha.find('.hdnAnexoOrdem').val(lastIndex).attr('name', 'PTV.Anexos[' + lastIndex + '].Ordem');
	    linha.find('.hdnAnexoArquivoJson').val(JSON.stringify({ Id: id })).attr('name', 'PTV.Anexos[' + lastIndex + '].Arquivo');
	    linha.find('.hdnAnexoDescricao').val(descricao).attr('name', 'PTV.Anexos[' + lastIndex + '].Descricao');

	    linha.find('.ArquivoNome').html(nome).attr('title', nome);
	    linha.find('.AnexoDescricao').html(descricao).attr('title', descricao);

	    $('tbody:last', tabAnexos).append(linha);
	    tabAnexos.removeClass('hide');
	    $('.lblGridVazio', tabAnexos.closest('fieldset')).addClass('hide');
	    $('.txtArquivoNome, .txtAnexoDescricao', PTVOutroEmitir.container).val('');

	    var inputFile = $('.inputFileDiv input:file', PTVOutroEmitir.container);
	    PTVOutroEmitir.atualizaEstiloGrid('tabAnexos');

	    FileUpload.upload(url, inputFile, PTVOutroEmitir.msgArqEnviado);
	    $('.inputFile', PTVOutroEmitir.container).val('');
	},

	msgArqEnviado: function (controle, retorno, isHtml) {
	    var tr = $('.tabAnexos tbody tr', PTVOutroEmitir.container).last();
	    var ret = eval('(' + retorno + ')');
	    if (ret.Arquivo != null) {
	        $('.ArquivoNome', tr).html(ret.Arquivo.Nome).attr('title', ret.Arquivo.Nome);
	        $('.hdnArquivoNome', tr).val(ret.Arquivo.Nome);
	        $('.hdnArquivoExtensao', tr).val(ret.Arquivo.Extensao);
	        $('.hdnAnexoArquivoJson', tr).val(JSON.stringify(ret.Arquivo));
	    } else {
	        PTVOutroEmitir.onLimparArquivoClick();
	        tr.remove();
	    }

	    PTVOutroEmitir.reorganizarEstiloTab($('.tabAnexos tbody', PTVOutroEmitir.container));
	    $(".btnAddAnexoArquivo", PTVOutroEmitir.container).show();
	    Mensagem.gerar(PTVOutroEmitir.container, ret.Msg);
	},

	onBtnDescerClick: function (container, tab) {
	    var tr = $(container.currentTarget).closest('tr');
	    tr.next().after(tr);
	    PTVOutroEmitir.atualizaEstiloGrid(tab);
	},

	onBtnSubirClick: function (container, tab) {
	    var tr = $(container.currentTarget).closest('tr');
	    tr.prev().before(tr);
	    PTVOutroEmitir.atualizaEstiloGrid(tab);
	},

	atualizaEstiloGrid: function (tab) {
	    var table = PTVOutroEmitir.container.find('.' + tab);
	    Listar.atualizarEstiloTable(table);

	    var rows = $('tbody tr:visible', table).removeClass('selecionado');
	    rows.each(function (index, elem) {
	        var btnDescer = $(elem).find('.btnDescerLinhaTab,.btnDescerLinha');
	        var btnSubir = $(elem).find('.btnSubirLinhaTab,.btnSubirLinha');

	        if (index == 0) {
	            btnSubir.addClass('desativado');
	        } else {
	            btnSubir.removeClass('desativado');
	        }

	        if (index >= rows.length - 1) {
	            btnDescer.addClass('desativado');
	        } else {
	            btnDescer.removeClass('desativado');
	        }
	    });
	},

	reorganizarEstiloTab: function (tab) {
	    $(tab).find('tr').each(function (i, linha) {
	        $(linha).removeClass();
	        $(linha).addClass((i % 2) === 0 ? 'par' : 'impar');
	    });
	},

	reorganizarIndicesTab: function (tab) {
	    $(tab).find('tr').each(function (i, linha) {
	        $(linha).find('input[name$=Index]').val(i + 1);
	    });
	},

	buscarUltimoIndice: function (tab) {
	    var ultimoIndex = $(tab).find('tbody tr').length + 1;
	    return ultimoIndex;
	},

	onLimparArquivoClick: function () {
	    //implementar Limpar

	    $('.txtArquivoNome').data('arquivo', null);
	    $('.txtArquivoNome').val("");
	    $('.inputFileArquivo').val("");
	    $('.hdnFileArquivo').val("");
	},

	existeAssociado: function (item, tab, itemClass) {
	    var existe = false;

	    var trs = $(tab).find('tbody tr');
	    $.each(trs, function (key, trElem) {
	        if ($(trElem).find('.' + itemClass) !== '') {
	            var trItem = $(trElem).find('.' + itemClass).val();
	            existe = (item.toLowerCase().trim() === trItem.toLowerCase().trim());
	            if (existe) {
	                return false;
	            }
	        }
	    });
	    return existe;
	},

	onExcluirLinha: function () {
	    var tab = $(this).closest('table.dataGridTable tbody');

	    if (tab.closest('table').hasClass('tabAnexos')) {
	        PTVOutroEmitir.atualizaEstiloGrid('tabAnexos');
	        FileUpload.cancelar(JSON.parse($(this).closest('tr').find('.hdnAnexoArquivoJson').val()).Id);
	    }

	    $(this).closest('tr').remove();
	    PTVOutroEmitir.reorganizarEstiloTab(tab);
	}

    //----------ANEXOS - ENVIAR ARQUIVO---------------
}
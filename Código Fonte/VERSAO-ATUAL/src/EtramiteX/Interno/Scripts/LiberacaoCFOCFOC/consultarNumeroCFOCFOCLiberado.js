/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../masterpage.js" />

ConsultarNumeroCFOCFOCLiberado = {
    settings: {
        urls: {
            buscar: null,
            verificarCPF: null,
            visualizarPessoa: null,
            cancelarModal: null,
            cancelar: null
        },
        Mensagens: null
    },
    container: null,
    Blocos: [],
    Digitais: [],

    load: function (container, options) {
        if (options) {
            $.extend(ConsultarNumeroCFOCFOCLiberado.settings, options);
        }

        ConsultarNumeroCFOCFOCLiberado.container = MasterPage.getContent(container);

        ConsultarNumeroCFOCFOCLiberado.container.delegate('.btnVerificarCpf', 'click', ConsultarNumeroCFOCFOCLiberado.verificarCPF);
        ConsultarNumeroCFOCFOCLiberado.container.delegate('.btnVisualizarPessoa', 'click', ConsultarNumeroCFOCFOCLiberado.abrirModalVisualizarPessoa);
        ConsultarNumeroCFOCFOCLiberado.container.delegate('.btnBuscar', 'click', ConsultarNumeroCFOCFOCLiberado.buscar);
        ConsultarNumeroCFOCFOCLiberado.container.delegate('.btnCancelar', 'click', ConsultarNumeroCFOCFOCLiberado.abrirModalMotivo);
        ConsultarNumeroCFOCFOCLiberado.container.delegate('.btnVisualizarMotivo', 'click', ConsultarNumeroCFOCFOCLiberado.abrirModalMotivo);

        //Eventos Paginação
        ConsultarNumeroCFOCFOCLiberado.container.delegate('.paginar.comeco', 'click', ConsultarNumeroCFOCFOCLiberado.paginarComeco);
        ConsultarNumeroCFOCFOCLiberado.container.delegate('.paginar.anterior', 'click', ConsultarNumeroCFOCFOCLiberado.paginarAnterior);
        ConsultarNumeroCFOCFOCLiberado.container.delegate('.paginar.proxima', 'click', ConsultarNumeroCFOCFOCLiberado.paginarProximo);
        ConsultarNumeroCFOCFOCLiberado.container.delegate('.paginar.final', 'click', ConsultarNumeroCFOCFOCLiberado.paginarUltima);
        ConsultarNumeroCFOCFOCLiberado.container.delegate('.paginar.pag', 'click', ConsultarNumeroCFOCFOCLiberado.paginar);

        ConsultarNumeroCFOCFOCLiberado.container.delegate('.txtCpf', 'keyup', function (e) {
            if (e.keyCode == 13) $('.btnVerificarCpf', ConsultarNumeroCFOCFOCLiberado.container).click();
        });
        Aux.setarFoco(ConsultarNumeroCFOCFOCLiberado.container);
    },

    verificarCPF: function () {
        Mensagem.limpar(ConsultarNumeroCFOCFOCLiberado.container);
        MasterPage.carregando(true);

        $.ajax({
            url: ConsultarNumeroCFOCFOCLiberado.settings.urls.verificarCPF,
            data: JSON.stringify({ cpf: $('.txtCpf', ConsultarNumeroCFOCFOCLiberado.container).val() }),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    $('.hdnCredenciadoId', ConsultarNumeroCFOCFOCLiberado.container).val(response.Credenciado.Id);
                    $('.hdnPessoaId', ConsultarNumeroCFOCFOCLiberado.container).val(response.Credenciado.Pessoa.InternoId);
                    $('.txtNome', ConsultarNumeroCFOCFOCLiberado.container).val(response.Credenciado.Nome);
                    $('.mostrar', ConsultarNumeroCFOCFOCLiberado.container).removeClass('hide');
                }
                else {
                    Mensagem.gerar(ConsultarNumeroCFOCFOCLiberado.container, response.Msg);
                }
            }
        });
        MasterPage.carregando(false);
    },

    abrirModalVisualizarPessoa: function () {
        var url = ConsultarNumeroCFOCFOCLiberado.settings.urls.visualizarPessoa + '/' + $('.hdnCredenciadoId', ConsultarNumeroCFOCFOCLiberado.container).val();
        Modal.abrir(url, null,
			function (context) { Modal.defaultButtons(context); },
			Modal.tamanhoModalGrande,
			'Visualizar Responsável Técnico');
    },

    obterFiltros: function (container) {
        var retorno = {
            TipoDocumento: $('.ddlTipoDocumento', container).val(),
            Numero: $('.txtNumero', container).val(),
            DataInicialEmissao: $('.txtDataInicialEmissao', container).val(),
            DataFinalEmissao: $('.txtDataFinalEmissao', container).val(),
            TipoNumero: 0,
            CredenciadoId: $('.hdnCredenciadoId', ConsultarNumeroCFOCFOCLiberado.container).val()
        };

        return retorno;
    },

    buscar: function () {
        MasterPage.carregando(true);
        var container = $(this).closest('fieldset');
        var objeto = ConsultarNumeroCFOCFOCLiberado.obterFiltros(container);

        if ($(this).hasClass('bloco')) {
            objeto.TipoNumero = 1;
        } else {
            objeto.TipoNumero = 2;
        }

        $.ajax({
            url: ConsultarNumeroCFOCFOCLiberado.settings.urls.buscar,
            data: JSON.stringify(objeto),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    if (response.lstRetorno && response.lstRetorno.length > 0) {
                        if (objeto.TipoNumero == 1) {
                            ConsultarNumeroCFOCFOCLiberado.Blocos = response.lstRetorno;
                        } else {
                            ConsultarNumeroCFOCFOCLiberado.Digitais = response.lstRetorno;
                            ConsultarNumeroCFOCFOCLiberado.quantidadeDigitais();
                        }
                        ConsultarNumeroCFOCFOCLiberado.carregarTabela(container, 1, response.lstRetorno);
                    } else {
                        Mensagem.gerar(ConsultarNumeroCFOCFOCLiberado.container, response.Msg);
                        $('tbody tr:not(.templateRow)', container).remove();
                    }
                }
                else {
                    Mensagem.gerar(ConsultarNumeroCFOCFOCLiberado.container, response.Msg);
                }
            }
        });

        MasterPage.carregando(false);
    },

    carregarTabela: function (container, numeroPagina, lista) {
        Mensagem.limpar(ConsultarNumeroCFOCFOCLiberado.container);
        var tabela = $(container).find('.dataGridTable');
        var qtdPaginas = Math.ceil(lista.length / 5);

        $(container).find('.numerosPag').empty();

        var qtdPaginas = Math.ceil(lista.length / 5);

        if (qtdPaginas <= 10) {
            for (var i = 0; i < qtdPaginas; i++) {
                $(container).find('.numerosPag').append("<a class='" + (i + 1) + " paginar pag'>" + (i + 1) + "</a>");
            }
        } else {
            if (numeroPagina <= 6) {
                for (var i = 0; i < 10; i++) {
                    $(container).find('.numerosPag').append("<a class='" + (i + 1) + " paginar pag'>" + (i + 1) + "</a>");
                }
            } else if (numeroPagina + 4 >= qtdPaginas) {
                for (i = qtdPaginas - 9; i < qtdPaginas; i++) {
                    $(container).find('.numerosPag').append("<a class='" + (i + 1) + " paginar pag'>" + (i + 1) + "</a>");
                }
            } else {
                for (i = numeroPagina - 5; i < numeroPagina + 4; i++) {
                    $(container).find('.numerosPag').append("<a class='" + (i + 1) + " paginar pag'>" + (i + 1) + "</a>");
                }
            }
        }

        $(container).find('.paginacaoCaixa').find('.hdnPaginaProxima').val((numeroPagina == qtdPaginas ? qtdPaginas : (numeroPagina + 1)));
        $(container).find('.paginacaoCaixa').find('.hdnPaginaUltima').val((qtdPaginas));
        $(container).find('.paginacaoCaixa').find('.hdnPaginaAnterior').val((numeroPagina == 1 ? 1 : (numeroPagina - 1)));

        $('tbody tr:not(.templateRow)', tabela).remove();

        var linha = null;
        var item = null;
        var index = (numeroPagina * 5) - 5;

        for (var i = index; i < (index + 5) ; i++) {
            linha = $('.templateRow', tabela).clone();
            item = lista[i];

            $('.tipoDocumento', linha).append(item.TipoDocumentoTexto);
            $('.Numero', linha).append(item.Numero);
            $('.Utilizado', linha).append(item.UtilizadoTexto);
            $('.Situacao', linha).append(item.SituacaoTexto);
            $('.hdnObjetoJson', linha).val(JSON.stringify(item));

            if (item.Situacao) {
                $('.btnCancelar', linha).removeClass('hide');
                $('.btnVisualizarMotivo', linha).addClass('hide');
            } else {
                $('.btnCancelar', linha).addClass('hide');
                $('.btnVisualizarMotivo', linha).removeClass('hide');
            }

            $(linha).removeClass('hide').removeClass('templateRow');
            $('tbody', tabela).append(linha);

            if ((i + 1) == lista.length) {
                break;
            }
        }

        $(tabela).removeClass('hide');
        $(tabela).closest('fieldset').find('.paginacaoCaixa').removeClass('hide');
        $(tabela).closest('fieldset').find('.paginacaoCaixa').find('.numerosPag').find('.' + numeroPagina).addClass('ativo');
        Listar.atualizarEstiloTable(container);
    },

    quantidadeDigitais: function () {
        var qtdCFO = 0;
        var qtdCFOC = 0;
        var qtdUtilizadoCFO = 0;
        var qtdUtilizadoCFOC = 0;

        $(ConsultarNumeroCFOCFOCLiberado.Digitais).each(function () {
            var item = this;
            if (item.TipoDocumentoTexto == 'CFO') {
                qtdCFO++;
                if (item.UtilizadoTexto == 'Sim') {
                    qtdUtilizadoCFO++;
                }
            }

            if (item.TipoDocumentoTexto == 'CFOC') {
                qtdCFOC++;
                if (item.UtilizadoTexto == 'Sim') {
                    qtdUtilizadoCFOC++;
                }
            }
        });

        $('.lblQtdCFO', ConsultarNumeroCFOCFOCLiberado.container).text(qtdCFO);
        $('.lblQtdCFOC', ConsultarNumeroCFOCFOCLiberado.container).text(qtdCFOC);
        $('.lblCFOUtilizado', ConsultarNumeroCFOCFOCLiberado.container).text(qtdUtilizadoCFO);
        $('.lblCFOCUtilizado', ConsultarNumeroCFOCFOCLiberado.container).text(qtdUtilizadoCFOC);
    },

    abrirModalMotivo: function () {

        var objetoJson = JSON.parse($(this).closest('tr').find('.hdnObjetoJson').val());
        var visualizar = $(this).hasClass('btnVisualizarMotivo');
        var obj = {
            Id: objetoJson.Id,
            Motivo: objetoJson.Motivo,
            TipoDocumento: objetoJson.TipoDocumentoTexto,
            Numero: objetoJson.Numero,
            IsVisualizar: visualizar
        };

        $(this).closest('tr').addClass('editando');
        Modal.abrir(ConsultarNumeroCFOCFOCLiberado.settings.urls.cancelarModal, { vm: obj }, function (content) {
            $('.txtMotivo', content).focus();
            if (!visualizar) {
                Modal.defaultButtons(content, ConsultarNumeroCFOCFOCLiberado.cancelar, 'Salvar', null, null, 'Cancelar', null, false);

            } else {
                Modal.defaultButtons(content, null, null, null, null, 'Cancelar', null, false);
            }
        }, Modal.tamanhoModalPequena);
    },

    cancelar: function (content) {
        var objeto = JSON.parse($('.editando', ConsultarNumeroCFOCFOCLiberado.container).find('.hdnObjetoJson').val());
        objeto.Motivo = $('.txtMotivo', content).val();
        objeto.Situacao = 0;
        objeto.SituacaoTexto = 'Cancelado';

        $.ajax({
            url: ConsultarNumeroCFOCFOCLiberado.settings.urls.cancelar,
            data: JSON.stringify(objeto),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    $('.editando', ConsultarNumeroCFOCFOCLiberado.container).find('.Situacao').html(objeto.SituacaoTexto);
                    $('.editando', ConsultarNumeroCFOCFOCLiberado.container).find('.btnCancelar').addClass('hide');
                    $('.editando', ConsultarNumeroCFOCFOCLiberado.container).find('.btnVisualizarMotivo').removeClass('hide');
                    $('.editando', ConsultarNumeroCFOCFOCLiberado.container).find('.hdnObjetoJson').val(JSON.stringify(objeto));
                    $('.editando', ConsultarNumeroCFOCFOCLiberado.container).removeClass('editando');

                    if (objeto.TipoNumero == 1) {
                        for (var i = 0; i < ConsultarNumeroCFOCFOCLiberado.Blocos.length; i++) {
                            if (ConsultarNumeroCFOCFOCLiberado.Blocos[i].Id == objeto.Id) {
                                ConsultarNumeroCFOCFOCLiberado.Blocos[i] = objeto;
                                break;
                            }
                        }
                    } else {
                        for (var i = 0; i < ConsultarNumeroCFOCFOCLiberado.Digitais.length; i++) {
                            if (ConsultarNumeroCFOCFOCLiberado.Digitais[i].Id == objeto.Id) {
                                ConsultarNumeroCFOCFOCLiberado.Digitais[i] = objeto;
                                break;
                            }
                        }
                    }

                    Modal.fechar(content);
                }
                Mensagem.gerar(ConsultarNumeroCFOCFOCLiberado.container, response.Msg);
            }
        });
    },

    //Paginação
    paginarComeco: function () {
        var container = $(this).closest('fieldset');
        var lista = $('.btnBuscar', container).hasClass('bloco') ? ConsultarNumeroCFOCFOCLiberado.Blocos : ConsultarNumeroCFOCFOCLiberado.Digitais;

        ConsultarNumeroCFOCFOCLiberado.carregarTabela(container, 1, lista);
    },

    paginarAnterior: function () {
        var container = $(this).closest('fieldset');
        var lista = $('.btnBuscar', container).hasClass('bloco') ? ConsultarNumeroCFOCFOCLiberado.Blocos : ConsultarNumeroCFOCFOCLiberado.Digitais;

        ConsultarNumeroCFOCFOCLiberado.carregarTabela(container, +$('.hdnPaginaAnterior', container).val(), lista);
    },

    paginarProximo: function () {
        var container = $(this).closest('fieldset');
        var lista = $('.btnBuscar', container).hasClass('bloco') ? ConsultarNumeroCFOCFOCLiberado.Blocos : ConsultarNumeroCFOCFOCLiberado.Digitais;

        ConsultarNumeroCFOCFOCLiberado.carregarTabela(container, +$('.hdnPaginaProxima', container).val(), lista);
    },

    paginarUltima: function () {
        var container = $(this).closest('fieldset');
        var lista = $('.btnBuscar', container).hasClass('bloco') ? ConsultarNumeroCFOCFOCLiberado.Blocos : ConsultarNumeroCFOCFOCLiberado.Digitais;

        ConsultarNumeroCFOCFOCLiberado.carregarTabela(container, +$('.hdnPaginaUltima', container).val(), lista);
    },

    paginar: function () {
        var container = $(this).closest('fieldset');
        var lista = $('.btnBuscar', container).hasClass('bloco') ? ConsultarNumeroCFOCFOCLiberado.Blocos : ConsultarNumeroCFOCFOCLiberado.Digitais;

        ConsultarNumeroCFOCFOCLiberado.carregarTabela(container, +$(this).text(), lista);
    }
}
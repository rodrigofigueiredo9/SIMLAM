/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../mensagem.js" />
/// <reference path="../../jquery.ddl.js" />

BarragemDispensaLicenca = {
    settings: {
        urls: {
            coordenadaGeo: null
        },
        mensagens: null,
        idsTela: null
    },
    container: null,

    load: function (container, options) {
        if (options) { $.extend(BarragemDispensaLicenca.settings, options); }
        BarragemDispensaLicenca.container = MasterPage.getContent(container);

        container.delegate('.rbBarragemTipo', 'change', BarragemDispensaLicenca.changeBarragemTipo);
        container.delegate('.rbFase', 'change', BarragemDispensaLicenca.changeFase);
        container.delegate('.ddlMongeTipo', 'change', BarragemDispensaLicenca.changeMongeTipo);
        container.delegate('.ddlVertedouroTipo', 'change', BarragemDispensaLicenca.changeVertedouroTipo);

        container.delegate('.rbPossuiMonge', 'change', BarragemDispensaLicenca.changeEstrutura);
        container.delegate('.rbPossuiVertedouro', 'change', BarragemDispensaLicenca.changeEstrutura);

  //      container.delegate('.cbFormacaoRT', 'change', function () { BarragemDispensaLicenca.changeFormacaoRT(); });
  //      container.delegate('.btnBuscarCoordenada', 'click', BarragemDispensaLicenca.buscarCoordenada);
  //      container.delegate(".btnArqLimpar", 'click', BarragemDispensaLicenca.onLimparArquivoClick);
  //      container.delegate('.btnSalvar', 'click', BarragemDispensaLicenca.salvar);

		container.delegate('.rbPerguntaFaixaDemarcada', 'change', BarragemDispensaLicenca.onChangeFaixaDemarcada);
		container.delegate('.rbPerguntaBarramentoDimensionado', 'change', BarragemDispensaLicenca.onChangeBarramento);
		container.delegate('.rbPerguntaVazaoMinInstalado', 'change', BarragemDispensaLicenca.onChangeVazaoMinInstalado);
		container.delegate('.rbPerguntaVazaoMinNormas', 'change', BarragemDispensaLicenca.onChangeVazaoMinNormas);
		container.delegate('.rbPerguntaVazaoMaxInstalado', 'change', BarragemDispensaLicenca.onChangeVazaoMaxInstalado);
		container.delegate('.rbPerguntaVazaoMaxNormas', 'change', BarragemDispensaLicenca.onChangeVazaoMaxNormas);

        //BarragemDispensaLicenca.changeBarragemTipo();
        //BarragemDispensaLicenca.changeFase();
        //BarragemDispensaLicenca.changeMongeVertedouroTipo();
        //BarragemDispensaLicenca.changeFormacaoRT($('.cbFormacaoRT', BarragemDispensaLicenca.container));

        BarragemDispensaLicenca.configurarTela();
    },

    configurarTela: function () {


        var containerMonge = $('.rbPossuiMonge:checked', BarragemDispensaLicenca.container).closest('.divRadio');
        containerMonge.find('.divRadioEsconder').toggleClass('hide', ($('.rbPossuiMonge:checked', BarragemDispensaLicenca.container).val() == 0 || $('.rbPossuiMonge:checked', BarragemDispensaLicenca.container).val() == ''));

        if ($('.ddlMongeTipo', BarragemDispensaLicenca.container).val() == BarragemDispensaLicenca.settings.idsTela.MongeTipoOutros) {
            $('.txtEspecificacaoMonge', BarragemDispensaLicenca.container).closest('div').removeClass('hide');
        } else {
            $('.txtEspecificacaoMonge', BarragemDispensaLicenca.container).closest('div').addClass('hide');
        }

        var containerVertedouro = $('.rbPossuiVertedouro:checked', BarragemDispensaLicenca.container).closest('.divRadio');
        containerVertedouro.find('.divRadioEsconder').toggleClass('hide', ($('.rbPossuiVertedouro:checked', BarragemDispensaLicenca.container).val() == 0 || $('.rbPossuiVertedouro:checked', BarragemDispensaLicenca.container).val() == ''));

        if ($('.ddlVertedouroTipo', BarragemDispensaLicenca.container).val() == BarragemDispensaLicenca.settings.idsTela.MongeTipoOutros) {
            $('.txtEspecificacaoVertedouro', BarragemDispensaLicenca.container).closest('div').removeClass('hide');
        } else {
            $('.txtEspecificacaoVertedouro', BarragemDispensaLicenca.container).closest('div').addClass('hide');
        }

        if ($('.rbFase:checked', BarragemDispensaLicenca.container).val() == BarragemDispensaLicenca.settings.idsTela.FaseConstruida) {
            $('.faseAConstruir', BarragemDispensaLicenca.container).addClass('hide');
            $('.faseConstruida', BarragemDispensaLicenca.container).removeClass('hide');
        } else {
            $('.faseConstruida', BarragemDispensaLicenca.container).addClass('hide');
            $('.faseAConstruir', BarragemDispensaLicenca.container).removeClass('hide');
        }

        if ($('.rbPossuiMonge:checked', BarragemDispensaLicenca.container).val() == 0 || $('.rbPossuiVertedouro:checked', BarragemDispensaLicenca.container).val() == 0 ||
            ($('.rbPossuiMonge:checked', BarragemDispensaLicenca.container).val() == '' || $('.rbPossuiVertedouro:checked', BarragemDispensaLicenca.container).val() == '')) {
            $('.rbPossuiEstruturaHidraulica', BarragemDispensaLicenca.container).removeClass('disabled').removeAttr('disabled');
        } else {
            $('.rbPossuiEstruturaHidraulica', BarragemDispensaLicenca.container).addClass('disabled').attr('disabled', 'disabled');
        }

        if ($('.rbBarragemTipo:checked', BarragemDispensaLicenca.container).val() == BarragemDispensaLicenca.settings.idsTela.BarragemTipoTerra) {
            $('.divFormacaoRT', BarragemDispensaLicenca.container).each(function () {
                $(this).removeClass('hide');
            });
        } else {
            $('.divFormacaoRT', BarragemDispensaLicenca.container).each(function () {
                var aux = $(this).find('.hdnFormacaoRT').val();
                if (aux != BarragemDispensaLicenca.settings.idsTela.FormacaoRTEngenheiroCivil && aux != BarragemDispensaLicenca.settings.idsTela.FormacaoRTOutros) {
                    $(this).addClass('hide');
                }
            });
        }

        $('.txtEspecificacaoRT', BarragemDispensaLicenca.container).closest('div').addClass('hide');
        var elemento = $('.cbFormacaoRT:checked', BarragemDispensaLicenca.container);
        elemento.closest('div').find('.hdnFormacaoRT').each(function (index, item) {
            if ($(item).val() == BarragemDispensaLicenca.settings.idsTela.FormacaoRTOutros) {
                if (!$('.txtEspecificacaoRT', BarragemDispensaLicenca.container).closest('div').is(':visible')) {
                    $('.txtEspecificacaoRT', BarragemDispensaLicenca.container).closest('div').removeClass('hide');
                }
            }
        });
    },

    changeBarragemTipo: function () {
        $('.txtEspecificacaoRT', BarragemDispensaLicenca.container).closest('div').addClass('hide');

        if ($('.rbBarragemTipo:checked', BarragemDispensaLicenca.container).val() == BarragemDispensaLicenca.settings.idsTela.BarragemTipoTerra) {
            $('.divFormacaoRT', BarragemDispensaLicenca.container).each(function () {
                $(this).find('.cbFormacaoRT').removeAttr('checked');
                $(this).removeClass('hide');
            });
        } else {
            $('.divFormacaoRT', BarragemDispensaLicenca.container).each(function () {
                $(this).find('.cbFormacaoRT').removeAttr('checked');
                var aux = $(this).find('.hdnFormacaoRT').val();

                if (aux != BarragemDispensaLicenca.settings.idsTela.FormacaoRTEngenheiroCivil && aux != BarragemDispensaLicenca.settings.idsTela.FormacaoRTOutros) {
                    $(this).addClass('hide');
                }
            });
        }
    },

    changeFase: function () {
        if ($('.rbFase:checked', BarragemDispensaLicenca.container).val() == BarragemDispensaLicenca.settings.idsTela.FaseConstruida) {
			$('.divBarragemAContruir', BarragemDispensaLicenca.container).addClass('hide');
			$('.divBarragemContruida', BarragemDispensaLicenca.container).removeClass('hide');
        } else {
			$('.divBarragemContruida', BarragemDispensaLicenca.container).addClass('hide');
			$('.divBarragemAContruir', BarragemDispensaLicenca.container).removeClass('hide');
        }

        //$('.rbPossuiMonge, .rbPossuiVertedouro, .rbPossuiEstruturaHidraulica', BarragemDispensaLicenca.container).removeAttr('checked');
        //$('.divRadioEsconder, .divRadioEsconderOutro', BarragemDispensaLicenca.container).addClass('hide');
        //$('.txtEspecificacaoVertedouro, .txtEspecificacaoMonge, .txtAdequacoesRealizada, .txtDataInicioObra, .txtDataPrevisaoTerminoObra', BarragemDispensaLicenca.container).val('');
        //$('.rbPossuiEstruturaHidraulica', BarragemDispensaLicenca.container).addClass('disabled').attr('disabled', 'disabled');

        //$('.ddlMongeTipo, .ddlVertedouroTipo', BarragemDispensaLicenca.container).ddlFirst();
        //var containerTxt = $('.ddlMongeTipo, .ddlVertedouroTipo', BarragemDispensaLicenca.container).closest('fieldset');
        //$('.txtEspecificacaoVertedouro, .txtEspecificacaoMonge', containerTxt).closest('div').addClass('hide');

    },

    changeMongeTipo: function () {
        var container = $(this).closest('fieldset');
        if ($('.ddlMongeTipo', container).val() == BarragemDispensaLicenca.settings.idsTela.MongeTipoOutros) {
            $('.txtEspecificacaoMonge', container).closest('div').removeClass('hide');
        } else {
            $('.txtEspecificacaoMonge', container).closest('div').addClass('hide');
        }
    },

    changeVertedouroTipo: function () {
        var container = $(this).closest('fieldset');
        if ($('.ddlVertedouroTipo', container).val() == BarragemDispensaLicenca.settings.idsTela.VertedouroTipoOutros) {
            $('.txtEspecificacaoVertedouro', container).closest('div').removeClass('hide');
        } else {
            $('.txtEspecificacaoVertedouro', container).closest('div').addClass('hide');
        }
    },

    changeMongeVertedouroTipo: function () {
        if ($('.rbPossuiMonge:checked', BarragemDispensaLicenca.container).val() == 0 || $('.rbPossuiVertedouro:checked', BarragemDispensaLicenca.container).val() == 0 ||
            ($('.rbPossuiMonge:checked', BarragemDispensaLicenca.container).val() == '' || $('.rbPossuiVertedouro:checked', BarragemDispensaLicenca.container).val() == '')) {
            $('.rbPossuiEstruturaHidraulica', BarragemDispensaLicenca.container).removeClass('disabled').removeAttr('disabled');
        } else {
            $('.rbPossuiEstruturaHidraulica', BarragemDispensaLicenca.container).removeAttr('checked');
            $('.rbPossuiEstruturaHidraulica', BarragemDispensaLicenca.container).addClass('disabled').attr('disabled', 'disabled');
        }
    },

    changeEstrutura: function () {
        var container = $(this).closest('.divRadio');
        container.find('.divRadioEsconder').toggleClass('hide', $(this).val() == 0);
        container.find('.divRadioEsconderOutro').addClass('hide');

        $('select', container).ddlFirst();
        $('input[type=text]', container).val('');

        BarragemDispensaLicenca.changeMongeVertedouroTipo();
    },

    changeFormacaoRT: function () {
        var elemento = $('.cbFormacaoRT:checked', BarragemDispensaLicenca.container);

        var v_outraFormacao = false;
        elemento.closest('div').find('.hdnFormacaoRT').each(function (index, item) {
            if ($(item).val() == BarragemDispensaLicenca.settings.idsTela.FormacaoRTOutros) {
                v_outraFormacao = true;
                if (!$('.txtEspecificacaoRT', BarragemDispensaLicenca.container).closest('div').is(':visible')) {
                    Mensagem.gerar(BarragemDispensaLicenca.container, [BarragemDispensaLicenca.settings.mensagens.FormacaoRTOutros]);
                    $('.txtEspecificacaoRT', BarragemDispensaLicenca.container).closest('div').removeClass('hide');
                }
            }
        });
        if (!v_outraFormacao) {
            $('.txtEspecificacaoRT', BarragemDispensaLicenca.container).closest('div').addClass('hide');
            $('.txtEspecificacaoRT', BarragemDispensaLicenca.container).val('');
        }
    },

    //---------------------------COORDENADA---------------------------//
    buscarCoordenada: function () {
        Modal.abrir(BarragemDispensaLicenca.settings.urls.coordenadaGeo, null, function (container) {
            Coordenada.load(container, {
                northing: $('.txtNorthing', BarragemDispensaLicenca.settings.container).val(),
                easting: $('.txtEasting', BarragemDispensaLicenca.settings.container).val(),
                pagemode: 'editMode',
                callBackSalvarCoordenada: BarragemDispensaLicenca.setarCoordenada
            });
            Modal.defaultButtons(container);
        },
		Modal.tamanhoModalGrande);
    },

    setarCoordenada: function (retorno) {
        retorno = JSON.parse(retorno);

        $('.txtNorthing', BarragemDispensaLicenca.settings.container).val(retorno.northing);
        $('.txtEasting', BarragemDispensaLicenca.settings.container).val(retorno.easting);
        $('.btnBuscarCoordenada', BarragemDispensaLicenca.settings.container).focus();
    },
    //---------------------------COORDENADA---------------------------//

    //-------------------------ENVIAR ARQUIVO-------------------------//
    enviarArquivo: function (url) {
        var nome = "enviando ...";
        var nomeArquivo = $('.inputFile').val();

        if (nomeArquivo === '') {
            Mensagem.gerar(BarragemDispensaLicenca.container, [BarragemDispensaLicenca.settings.mensagens.ArquivoObrigatorio]);
            return;
        }

        var inputFile = $('.inputFileDiv input[type="file"]');
        inputFile.attr("id", "ArquivoId");

        FileUpload.upload(url, inputFile, BarragemDispensaLicenca.callBackEnviarArquivo);
        $('.inputFile').val('');
    },

    callBackEnviarArquivo: function (controle, retorno, isHtml) {
        var ret = eval('(' + retorno + ')');
        if (ret.Arquivo != null) {
            $('.txtArquivoNome', BarragemDispensaLicenca.container).val(ret.Arquivo.Nome);
            $('.hdnArquivo', BarragemDispensaLicenca.container).val(JSON.stringify(ret.Arquivo));

            $('.spanInputFile', BarragemDispensaLicenca.container).addClass('hide');
            $('.txtArquivoNome', BarragemDispensaLicenca.container).removeClass('hide');

            $('.btnArq', BarragemDispensaLicenca.container).addClass('hide');
            $('.btnArqLimpar', BarragemDispensaLicenca.container).removeClass('hide');

        } else {
            BarragemDispensaLicenca.limparArquivo();
        }

        Mensagem.gerar(MasterPage.getContent(BarragemDispensaLicenca.container), ret.Msg);
    },

    limparArquivo: function () {
        $('.txtArquivoNome', BarragemDispensaLicenca.container).data('arquivo', null);
        $('.txtArquivoNome', BarragemDispensaLicenca.container).val('');
        $('.hdnArquivo', BarragemDispensaLicenca.container).val('');

        $('.spanInputFile', BarragemDispensaLicenca.container).removeClass('hide');
        $('.txtArquivoNome', BarragemDispensaLicenca.container).addClass('hide');

        $('.btnArq', BarragemDispensaLicenca.container).removeClass('hide');
        $('.btnArqLimpar', BarragemDispensaLicenca.container).addClass('hide');

        $('.lnkArquivo', BarragemDispensaLicenca.container).remove();
    },

    onLimparArquivoClick: function () {
        $('.hdnArquivo', BarragemDispensaLicenca.container).val('');
        $('.inputFile', BarragemDispensaLicenca.container).val('');

        $('.spanInputFile', BarragemDispensaLicenca.container).removeClass('hide');
        $('.txtArquivoNome', BarragemDispensaLicenca.container).addClass('hide');

        $('.btnArq', BarragemDispensaLicenca.container).removeClass('hide');
        $('.btnArqLimpar', BarragemDispensaLicenca.container).addClass('hide');

        $('.lnkArquivo', BarragemDispensaLicenca.container).closest('div').addClass('hide');

        Mensagem.limpar(BarragemDispensaLicenca.container);
    },
    //-------------------------ENVIAR ARQUIVO-------------------------//

    obter: function () {
        var objeto = {
            Id: $('.hdnCaracterizacaoId', BarragemDispensaLicenca.container).val(),
            EmpreendimentoID: $('.hdnEmpreendimentoId', BarragemDispensaLicenca.container).val(),
            AtividadeID: $('.ddlAtividade', BarragemDispensaLicenca.container).val(),
            BarragemTipo: $('.rbBarragemTipo:checked', BarragemDispensaLicenca.container).val(),
            FinalidadeAtividade: 0,
            CursoHidrico: $('.txtCursoHidrico', BarragemDispensaLicenca.container).val(),
            VazaoEnchente: $('.txtVazaoEnchente', BarragemDispensaLicenca.container).val(),
            AreaBaciaContribuicao: $('.txtAreaBaciaContribuicao', BarragemDispensaLicenca.container).val(),
            Precipitacao: $('.txtPrecipitacao', BarragemDispensaLicenca.container).val(),
            PeriodoRetorno: $('.txtPeriodoRetorno', BarragemDispensaLicenca.container).val(),
            CoeficienteEscoamento: $('.txtCoeficienteEscoamento', BarragemDispensaLicenca.container).val(),
            TempoConcentracao: $('.txtTempoConcentracao', BarragemDispensaLicenca.container).val(),
            EquacaoCalculo: $('.txtEquacaoCalculo', BarragemDispensaLicenca.container).val(),
            AreaAlagada: $('.txtAreaAlagada', BarragemDispensaLicenca.container).val(),
            VolumeArmazanado: $('.txtVolumeArmazenado', BarragemDispensaLicenca.container).val(),
            Fase: $('.rbFase:checked', BarragemDispensaLicenca.container).val(),
            PossuiMonge: $('.rbPossuiMonge:visible:checked', BarragemDispensaLicenca.container).val(),
            MongeTipo: $('.ddlMongeTipo:visible', BarragemDispensaLicenca.container).val(),
            EspecificacaoMonge: $('.txtEspecificacaoMonge:visible', BarragemDispensaLicenca.container).val(),
            PossuiVertedouro: $('.rbPossuiVertedouro:visible:checked', BarragemDispensaLicenca.container).val(),
            VertedouroTipo: $('.ddlVertedouroTipo:visible', BarragemDispensaLicenca.container).val(),
            EspecificacaoVertedouro: $('.txtEspecificacaoVertedouro:visible', BarragemDispensaLicenca.container).val(),
            PossuiEstruturaHidraulica: $('.rbPossuiEstruturaHidraulica:visible:checked', BarragemDispensaLicenca.container).val(),
            AdequacoesRealizada: $('.txtAdequacoesRealizada:visible', BarragemDispensaLicenca.container).val(),
            DataInicioObra: $('.txtDataInicioObra:visible', BarragemDispensaLicenca.container).val(),
            DataPrevisaoTerminoObra: $('.txtDataPrevisaoTerminoObra:visible', BarragemDispensaLicenca.container).val(),
            Coordenada: {
                EastingUtmTexto: $('.txtEasting', BarragemDispensaLicenca.container).val(),
                NorthingUtmTexto: $('.txtNorthing', BarragemDispensaLicenca.container).val()
            },
            FormacaoRT: 0,
            EspecificacaoRT: $('.txtEspecificacaoRT:visible', BarragemDispensaLicenca.container).val(),
            NumeroARTElaboracao: $('.txtNumeroARTElaboracao', BarragemDispensaLicenca.container).val(),
            NumeroARTExecucao: $('.txtNumeroARTExecucao', BarragemDispensaLicenca.container).val(),
            Autorizacao: $.parseJSON($('.hdnArquivo', BarragemDispensaLicenca.container).val())
        };

        $('.cbFinalidadeAtividade').each(function (index, item) {
            if ($(item).is(':checked')) {
                objeto.FinalidadeAtividade += +$(item).val();
            }
        });

        $('.cbFormacaoRT').each(function (index, item) {
            if ($(item).is(':checked')) {
                objeto.FormacaoRT += +$(item).val();
            }
        });

        return objeto;
    },

    salvar: function () {
        Mensagem.limpar(BarragemDispensaLicenca.container);
        MasterPage.carregando(true);

        $.ajax({
            url: BarragemDispensaLicenca.settings.urls.salvar,
            data: JSON.stringify({ caracterizacao: BarragemDispensaLicenca.obter(), projetoDigitalId: $('.hdnProjetoDigitalId', BarragemDispensaLicenca.container).val() }),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: Aux.error,
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.EhValido) {
                    MasterPage.redireciona(response.UrlRedirecionar);
                }

                if (response.Msg && response.Msg.length > 0) {
                    Mensagem.gerar(BarragemDispensaLicenca.container, response.Msg);
                }
            }
        });

        MasterPage.carregando(false);
	},

	//-------------------------BARRGGEM CONSTRUIDA--------------------//

	onChangeFaixaDemarcada: function () {
		if ($('.rbPerguntaFaixaDemarcada:checked').val() == 1) {
			console.log('remove')
			$('.boxApp').removeClass('hide');
		} else {
			$('.boxApp').addClass('hide');
		}

		BarragemDispensaLicenca.onLimparFaixaDemarcada();
	},

	onLimparFaixaDemarcada: function () {
		$('.txtAreaAlagada').val('');
		$('.rbPerguntaSupressao:checked').prop('checked', false);
		$('.rbPerguntaCercada:checked').prop('checked', false);
		$('.txtDescricaoDesenvolvimento').val('');
	},

	onChangeBarramento: function () {
		if ($('.rbPerguntaBarramentoDimensionado:checked').val() == 1) {
			$('.AdequacoesDimensionamentoBarramento').removeClass('hide');

		} else {
			$('.AdequacoesDimensionamentoBarramento').addClass('hide');
		}
		$('.txtAdequacoesDimensionamentoBarramento').val('');
	},

	onChangeVazaoMinInstalado: function () {
		if ($('.rbPerguntaVazaoMinInstalado:checked').val() == 1) {
			$('.vazaoMinNormas').removeClass('hide');
		} else {
			$('.vazaoMinNormas').addClass('hide');
			$('.AdequacoesDimensionamentoVazaoMin').addClass('hide');
		}
		$('.rbPerguntaVazaoMinNormas:checked').prop('checked', false);
	},

	onChangeVazaoMinNormas: function () {
		if ($('.rbPerguntaVazaoMinNormas:checked').val() == 1) {
			$('.vazaoMinNormas').removeClass('hide');

		} else {
			$('.vazaoMinNormas').addClass('hide');
		}
		$('.txtAdequacoesDimensionamentoVazaoMin').val('');
	},

	onChangeVazaoMaxInstalado: function () {
		if ($('.rbPerguntaVazaoMaxInstalado:checked').val() == 1) {
			$('.vazaoMaxNormas').removeClass('hide');
		} else {
			$('.vazaoMaxNormas').addClass('hide');
			$('.AdequacoesDimensionamentoVazaoMax').addClass('hide');
		}
		$('.rbPerguntaVazaoMaxNormas:checked').prop('checked', false);
	},

	onChangeVazaoMaxNormas: function () {
		if ($('.rbPerguntaVazaoMaxNormas:checked').val() == 1) {
			$('.AdequacoesDimensionamentoVazaoMax').removeClass('hide');

		} else {
			$('.AdequacoesDimensionamentoVazaoMax').addClass('hide');
		}
		$('.txtAdequacoesDimensionamentoVazaoMax').val('');
	}
}
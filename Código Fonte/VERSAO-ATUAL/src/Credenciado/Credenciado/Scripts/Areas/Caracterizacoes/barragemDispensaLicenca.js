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
		containerCoordenada: null,
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

        container.delegate('.cbFormacaoRT', 'change', function () { BarragemDispensaLicenca.changeFormacaoRT(); });
        container.delegate('.btnBuscarCoordenada', 'click', BarragemDispensaLicenca.buscarCoordenada);
        container.delegate(".btnArqLimpar", 'click', BarragemDispensaLicenca.onLimparArquivoClick);
		container.delegate('.btnSalvar', 'click', BarragemDispensaLicenca.salvarConfirm);

		container.delegate('.rbDemarcaoAPP', 'change', BarragemDispensaLicenca.onChangeFaixaDemarcada);
		container.delegate('.rbBarramentoNormas', 'change', BarragemDispensaLicenca.onChangeBarramento);
		container.delegate('.rbVazaoMinInstalado', 'change', BarragemDispensaLicenca.onChangeVazaoMinInstalado);
		container.delegate('.rbVazaoMinNormas', 'change', BarragemDispensaLicenca.onChangeVazaoMinNormas);
		container.delegate('.rbVazaoMaxInstalado', 'change', BarragemDispensaLicenca.onChangeVazaoMaxInstalado);
		container.delegate('.rbVazaoMaxNormas', 'change', BarragemDispensaLicenca.onChangeVazaoMaxNormas);
		container.delegate('.cbCopiaDeclarante', 'change', BarragemDispensaLicenca.onCheckCopiaDeclarante);

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
		BarragemDispensaLicenca.settings.containerCoordenada = this.parentElement.parentElement;
		
        Modal.abrir(BarragemDispensaLicenca.settings.urls.coordenadaGeo, null, function (container) {
            Coordenada.load(container, {
				northing: $('.txtNorthing', BarragemDispensaLicenca.settings.containerCoordenada).val(),
				easting: $('.txtEasting', BarragemDispensaLicenca.settings.containerCoordenada).val(),
                pagemode: 'editMode',
                callBackSalvarCoordenada: BarragemDispensaLicenca.setarCoordenada
            });
            Modal.defaultButtons(container);
        },
		Modal.tamanhoModalGrande);
    },

	setarCoordenada: function (retorno) {
        retorno = JSON.parse(retorno);

		$('.txtNorthing', BarragemDispensaLicenca.settings.containerCoordenada).val(retorno.northing);
		$('.txtEasting', BarragemDispensaLicenca.settings.containerCoordenada).val(retorno.easting);
		$('.btnBuscarCoordenada', BarragemDispensaLicenca.settings.containerCoordenada).focus();
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
			fase: $('.rbFase:checked', BarragemDispensaLicenca.container).val(),
            //FinalidadeAtividade: 0,
			barragemContiguaMesmoNivel: $('.rbBarragemTipo:checked').val(), 
			areaAlagada: $('.txtAreaAlagada').val(),
			volumeArmazanado: $('.txtVolumeArmazenamento').val(),
			alturaBarramento: $('.txtAlturaBarramento').val(),
			comprimentoBarramento: $('.txtComprimentoBarramento').val(),
			larguraBaseBarramento: $('.txtLarguraBaseBarramento').val(),
			larguraCristaBarramento: $('.txtLarguraCristaBarramento').val(),
			coordenadas: [],
			finalidade: [], 
			responsaveisTecnicos: [],
			cursoHidrico: $('.txtCursoHidrico').val(),
			areaBaciaContribuicao: $('.txtAreaBacia').val(),
			intensidadeMaxPrecipitacao: $('.txtIntensidadeMaxPrecipitacao').val(),
			fonteDadosPrecipitacao: $('.txtFonteDadosIntensidade').val(),
			periodoRetorno: $('.txtPeriodoRetorno').val(),
			coeficienteEscoamento: $('.txtCoeficienteEscoamento').val(),
			fonteDadosCoeficienteEscoamento: $('.txtFonteDadosCoeficiente').val(),
			tempoConcentracao: $('.txtTempoConcentracao').val(),
			tempoConcentracaoEquacaoUtilizada: $('.txtEquacaoTempoConcentracao').val(),
			vazaoEnchente: $('.txtVazaoEnchente').val(),
			fonteDadosVazaoEnchente: $('.txtFonteDadosVazao').val(),
			construidaConstruir: {
				id: 0,
				isSupressaoAPP: $('.rbSupressaoAPP:checked').val(), 
				isDemarcacaoAPP: $('.txtFonteDadosVazao').val(),
				larguraDemarcada: $('.txtLarguraDemarcada').val(),
				larguraDemarcadaLegislacao: $('.rbLarguraDemarcadaLegislacao:checked').val(),
				faixaCercada: $('.rbFaixaCercada:checked').val(),
				descricacaoDesenvolvimentoAPP: $('.txtDescricaoDesenvolvimento').val(),
				barramentoNormas: $('.rbBarramentoNormas:checked').val(),
				barramentoAdequacoes: $('.txtAdequacoesDimensionamentoBarramento').val(),
				vazaoMinTipo: ($('.rbFase:checked')).val() == 1 ?
					$('.ddlTipoDispositivoVazaoMin:visible').val() : $('.ddlTipoDispositivoVazaoMinAConstruir:visible').val(),
				vazaoMinDiametro: ($('.rbFase:checked')).val() == 1 ?
					$('.txtDiametroTubulacaoVazaoMin').val() : $('.txtDiametroTubulacaoVazaoMinAConstruir').val(),
				vazaoMinInstalado: $('.rbVazaoMinInstalado:checked').val(),
				vazaoMinNormas: $('.rbVazaoMinNormas:checked').val(),
				vazaoMinAdequacoes: $('.txtAdequacoesDimensionamentoVazaoMin').val(),
				vazaoMaxTipo: ($('.rbFase:checked')).val() == 1 ?
					$('.ddlTipoDispositivoVazaoMax:visible').val() : $('.ddlTipoDispositivoVazaoMaxAConstruir:visible').val(),
				vazaoMaxDiametro: ($('.rbFase:checked')).val() == 1 ?
					$('.txtDiametroTubulacaoVazaoMax').val() : $('.txtDiametroTubulacaoVazaoMaxAConstruir').val(),
				vazaoMaxInstalado: $('.rbVazaoMaxInstalado:checked').val(),
				vazaoMaxNormas: $('.rbVazaoMaxNormas:checked').val(),
				vazaoMaxAdequacoes: $('.txtAdequacoesDimensionamentoVazaoMax').val(),
				mesInicioObra: $('.txtAdequacoesDimensionamentoVazaoMax').val(),
				anoInicioObra: $('.txtAdequacoesDimensionamentoVazaoMax').val()
				
			},

			//CursoHidrico: $('.txtCursoHidrico', BarragemDispensaLicenca.container).val(),
   //         VazaoEnchente: $('.txtVazaoEnchente', BarragemDispensaLicenca.container).val(),
   //         AreaBaciaContribuicao: $('.txtAreaBaciaContribuicao', BarragemDispensaLicenca.container).val(),
   //         Precipitacao: $('.txtPrecipitacao', BarragemDispensaLicenca.container).val(),
   //         PeriodoRetorno: $('.txtPeriodoRetorno', BarragemDispensaLicenca.container).val(),
   //         CoeficienteEscoamento: $('.txtCoeficienteEscoamento', BarragemDispensaLicenca.container).val(),
   //         TempoConcentracao: $('.txtTempoConcentracao', BarragemDispensaLicenca.container).val(),
   //         EquacaoCalculo: $('.txtEquacaoCalculo', BarragemDispensaLicenca.container).val(),
   //         AreaAlagada: $('.txtAreaAlagada', BarragemDispensaLicenca.container).val(),
   //         VolumeArmazanado: $('.txtVolumeArmazenado', BarragemDispensaLicenca.container).val(),
   //         Fase: $('.rbFase:checked', BarragemDispensaLicenca.container).val(),
   //         PossuiMonge: $('.rbPossuiMonge:visible:checked', BarragemDispensaLicenca.container).val(),
   //         MongeTipo: $('.ddlMongeTipo:visible', BarragemDispensaLicenca.container).val(),
   //         EspecificacaoMonge: $('.txtEspecificacaoMonge:visible', BarragemDispensaLicenca.container).val(),
   //         PossuiVertedouro: $('.rbPossuiVertedouro:visible:checked', BarragemDispensaLicenca.container).val(),
   //         VertedouroTipo: $('.ddlVertedouroTipo:visible', BarragemDispensaLicenca.container).val(),
   //         EspecificacaoVertedouro: $('.txtEspecificacaoVertedouro:visible', BarragemDispensaLicenca.container).val(),
   //         PossuiEstruturaHidraulica: $('.rbPossuiEstruturaHidraulica:visible:checked', BarragemDispensaLicenca.container).val(),
   //         AdequacoesRealizada: $('.txtAdequacoesRealizada:visible', BarragemDispensaLicenca.container).val(),
   //         DataInicioObra: $('.txtDataInicioObra:visible', BarragemDispensaLicenca.container).val(),
   //         DataPrevisaoTerminoObra: $('.txtDataPrevisaoTerminoObra:visible', BarragemDispensaLicenca.container).val(),
            //Coordenada: {
            //    EastingUtmTexto: $('.txtEasting', BarragemDispensaLicenca.container).val(),
            //    NorthingUtmTexto: $('.txtNorthing', BarragemDispensaLicenca.container).val()
            //},
            //FormacaoRT: 0,
            //EspecificacaoRT: $('.txtEspecificacaoRT:visible', BarragemDispensaLicenca.container).val(),
            //NumeroARTElaboracao: $('.txtNumeroARTElaboracao', BarragemDispensaLicenca.container).val(),
            //NumeroARTExecucao: $('.txtNumeroARTExecucao', BarragemDispensaLicenca.container).val(),
            //Autorizacao: $.parseJSON($('.hdnArquivo', BarragemDispensaLicenca.container).val())
        };

        $('.cbFinalidadeAtividade').each(function (index, item) {
			if ($(item).is(':checked')) {
				objeto.finalidade.push($(item).val());
            }
        });

        $('.cbFormacaoRT').each(function (index, item) {
            if ($(item).is(':checked')) {
                objeto.FormacaoRT += +$(item).val();
            }
		});

		objeto.responsaveisTecnicos = BarragemDispensaLicenca.obterRT();
		objeto.coordenadas = BarragemDispensaLicenca.obterCoordenadas();

        return objeto;
	},

	obterRT: function () {

		var retorno = [];
		var tipo = [
			{ codigo: '1', nome: 'ElaboracaoDeclaracao' },
			{ codigo: '2', nome: 'ElaboracaoProjeto' },
			{ codigo: '3', nome: 'ExecucaoBarragem' },
			{ codigo: '4', nome: 'ElaboracaoEstudoAmbiental' },
			{ codigo: '5', nome: 'ElaboracaoPlanoRecuperacao' },
			{ codigo: '6', nome: 'ExecucaoPlanoRecuperacao' }
		];

		for (var i = 0; i < 6; i++) {
			var rt = {
				id: 0,
				nome: $('.txtRT' + tipo[i].nome + 'Nome').val(),
				tipo: tipo[i].codigo,
				profissao: { Id: $('.ddlRT' + tipo[i].nome + 'Profissao').val() },
				registroCREA: $('.txtRT' + tipo[i].nome + 'CREA').val(),
				numeroART: $('.txtRT' + tipo[i].nome + 'Numero').val(),
				autorizacaoCREA: tipo[i].codigo == 2 ? $.parseJSON($('.hdnArquivo').val()) : null,
			};
			retorno.push(rt);
		}

		return retorno;
	},

	obterCoordenadas: function () {
		return [
			{
				id: 8,
				tipo: '1',
				easting: $('.txtEastingBarramento').val(),
				northing: $('.txtNorthingBarramento').val()
			},
			{
				id: 0,
				tipo: '2',
				easting: $('.txtEastingBotaFora').val(),
				northing: $('.txtNorthingBotaFora').val()
			},
			{
				id: 0,
				tipo: '3',
				easting: $('.txtEastingEmprestimo').val(),
				northing: $('.txtNorthingEmprestimo').val()
			}
		];
	},

	salvarConfirm: function () {
		Mensagem.limpar(BarragemDispensaLicenca.container);
		MasterPage.carregando(true);

		$.ajax({
			url: BarragemDispensaLicenca.settings.urls.salvarConfirm,
			data: JSON.stringify({ caracterizacao: BarragemDispensaLicenca.obter(), projetoDigitalId: $('.hdnProjetoDigitalId', BarragemDispensaLicenca.container).val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(BarragemDispensaLicenca.container, response.Msg);
					return;
				}

				Modal.confirma({
					btnOkLabel: 'Sim',
					btCancelLabel: "Voltar para a caracterização",
					titulo: 'Confirmação da veracidade das informações',
					conteudo: '<b>Declaro que as informações prestadas são expressões da verdade sob as penas legais\
								por omissão ou prestação de informação falsa ou imprecisa.</b>',
					btnOkCallback: function (conteudoModal) {
						Modal.fechar(conteudoModal);
						BarragemDispensaLicenca.salvar();
					}
				});
			}
		});

		MasterPage.carregando(false);
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
		if ($('.rbDemarcaoAPP:checked').val() == 1) {
			console.log('remove')
			$('.boxApp').removeClass('hide');
		} else {
			$('.boxApp').addClass('hide');
		}

		BarragemDispensaLicenca.onLimparFaixaDemarcada();
	},

	onLimparFaixaDemarcada: function () {
		$('.txtAreaAlagada').val('');
		$('.rbLarguraDemarcadaLegislacao:checked').prop('checked', false);
		$('.rbFaixaCercada:checked').prop('checked', false);
		$('.txtDescricaoDesenvolvimento').val('');
	},

	onChangeBarramento: function () {
		if ($('.rbBarramentoNormas:checked').val() == 1) {
			$('.AdequacoesDimensionamentoBarramento').removeClass('hide');

		} else {
			$('.AdequacoesDimensionamentoBarramento').addClass('hide');
		}
		$('.txtAdequacoesDimensionamentoBarramento').val('');
	},

	onChangeVazaoMinInstalado: function () {
		if ($('.rbVazaoMinInstalado:checked').val() == 1) {
			$('.vazaoMinNormas').removeClass('hide');
		} else {
			$('.vazaoMinNormas').addClass('hide');
			$('.AdequacoesDimensionamentoVazaoMin').addClass('hide');
		}
		$('.rbVazaoMinNormas:checked').prop('checked', false);
	},

	onChangeVazaoMinNormas: function () {
		if ($('.rbVazaoMinNormas:checked').val() == 1) {
			$('.AdequacoesDimensionamentoVazaoMin').removeClass('hide');

		} else {
			$('.AdequacoesDimensionamentoVazaoMin').addClass('hide');
		}
		$('.txtAdequacoesDimensionamentoVazaoMin').val('');
	},

	onChangeVazaoMaxInstalado: function () {
		if ($('.rbVazaoMaxInstalado:checked').val() == 1) {
			$('.vazaoMaxNormas').removeClass('hide');
		} else {
			$('.vazaoMaxNormas').addClass('hide');
			$('.AdequacoesDimensionamentoVazaoMax').addClass('hide');
		}
		$('.rbVazaoMaxNormas:checked').prop('checked', false);
	},

	onChangeVazaoMaxNormas: function () {
		if ($('.rbVazaoMaxNormas:checked').val() == 1) {
			$('.AdequacoesDimensionamentoVazaoMax').removeClass('hide');

		} else {
			$('.AdequacoesDimensionamentoVazaoMax').addClass('hide');
		}
		$('.txtAdequacoesDimensionamentoVazaoMax').val('');
	},

	onCheckCopiaDeclarante: function () {
		var container = this.parentElement.parentElement.parentElement;
		if ($('.cbCopiaDeclarante:checked').val()) {
			nomeDeclarante = $('.txtRTElaboracaoDeclaracaoNome').val();
			profissaoDeclarante = $('.ddlRTElaboracaoDeclaracaoProfissao:visible').val();
			creaDeclarante = $('.txtRTElaboracaoDeclaracaoCREA').val();
			numeroDeclarante = $('.txtRTElaboracaoDeclaracaoNumero').val();

			$('.rtNome', container).val(nomeDeclarante).prop('disabled', true).addClass('disabled');
			$('.rtProfissao', container).val(profissaoDeclarante).prop('disabled', true).addClass('disabled');
			$('.rtCREA', container).val(creaDeclarante).prop('disabled', true).addClass('disabled');
			$('.rtNumero', container).val(numeroDeclarante).prop('disabled', true).addClass('disabled');
		} else {
			$('.rtNome', container).val('').prop('disabled', false).removeClass('disabled');
			$('.rtProfissao', container).val(' ').prop('disabled', false).removeClass('disabled');
			$('.rtCREA', container).val(' ').prop('disabled', false).removeClass('disabled');
			$('.rtNumero', container).val(' ').prop('disabled', false).removeClass('disabled');
		}
	}
}
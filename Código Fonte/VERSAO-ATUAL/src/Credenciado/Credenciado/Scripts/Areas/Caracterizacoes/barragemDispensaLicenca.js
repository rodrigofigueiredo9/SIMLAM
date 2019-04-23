/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
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
		idsTela: null,
		projetoDigitalId: null,
		empreendimentoId: null,
		profissoesSemAutorizacao : [15, 37, 38], /*Eng. Civil, Eng. Agricola, Eng. Agronomo*/
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
		container.delegate('.btnAdicionar', 'click', BarragemDispensaLicenca.criar);

		container.delegate('.btnAssociar', 'click', BarragemDispensaLicenca.associar);
		container.delegate('.btnDesassociar', 'click', BarragemDispensaLicenca.desassociar);
		container.delegate('.btnVisualizar', 'click', BarragemDispensaLicenca.visualizar);
		container.delegate('.btnEditar', 'click', BarragemDispensaLicenca.editar);

		container.delegate('.rbDemarcaoAPP', 'change', BarragemDispensaLicenca.onChangeFaixaDemarcada);
		container.delegate('.rbBarramentoNormas', 'change', BarragemDispensaLicenca.onChangeBarramento);
		container.delegate('.rbVazaoMinInstalado', 'change', BarragemDispensaLicenca.onChangeVazaoMinInstalado);
		container.delegate('.rbVazaoMinNormas', 'change', BarragemDispensaLicenca.onChangeVazaoMinNormas);
		container.delegate('.rbVazaoMaxInstalado', 'change', BarragemDispensaLicenca.onChangeVazaoMaxInstalado);
		container.delegate('.rbVazaoMaxNormas', 'change', BarragemDispensaLicenca.onChangeVazaoMaxNormas);
		container.delegate('.cbCopiaDeclarante', 'change', BarragemDispensaLicenca.onCheckCopiaDeclarante);
		container.delegate('.ddlRTElaboracaoProjetoProfissao', 'change', BarragemDispensaLicenca.onChangeProfissaoRT);
		container.delegate('.txtAreaAlagada', 'blur', BarragemDispensaLicenca.onChangeAreaAlagada);

		BarragemDispensaLicenca.bloquearCriar();
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

		BarragemDispensaLicenca.limparConstruidaAConstruir()
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
		MasterPage.carregando(true);
        if (nomeArquivo === '') {
            Mensagem.gerar(BarragemDispensaLicenca.container, [BarragemDispensaLicenca.settings.mensagens.ArquivoObrigatorio]);
            return;
        }

        var inputFile = $('.inputFileDiv input[type="file"]');
        inputFile.attr("id", "ArquivoId");

        FileUpload.upload(url, inputFile, BarragemDispensaLicenca.callBackEnviarArquivo);
        $('.inputFile').val('');
		MasterPage.carregando(false);
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
		Mensagem.limpar(BarragemDispensaLicenca.container);
        var objeto = {
            Id: $('.hdnCaracterizacaoId', BarragemDispensaLicenca.container).val(),
			EmpreendimentoID: $('.hdnEmpreendimentoId', BarragemDispensaLicenca.container).val(),
			ProjetoDigitalId: $('.hdnProjetoDigitalId', BarragemDispensaLicenca.container).val(),
            AtividadeID: $('.ddlAtividade', BarragemDispensaLicenca.container).val(),
            BarragemTipo: $('.rbBarragemTipo:checked', BarragemDispensaLicenca.container).val(),
			fase: $('.rbFase:checked', BarragemDispensaLicenca.container).val(),
            //FinalidadeAtividade: 0,
			barragemContiguaMesmoNivel: ($('.rbBarragemContiguaMesmoNivel:checked').val() == 1) , 
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
			precipitacao: $('.txtIntensidadeMaxPrecipitacao').val(),
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
				isDemarcacaoAPP: $('.rbDemarcaoAPP:checked').val(),
				larguraDemarcada: $('.txtLarguraDemarcada').val(),
				larguraDemarcadaLegislacao: $('.rbLarguraDemarcadaLegislacao:checked').val() == 1 ? true :
					($('.rbLarguraDemarcadaLegislacao:checked').val() == 0 ? false : null),
				faixaCercada: $('.rbFaixaCercada:checked').val(),
				descricaoDesenvolvimentoAPP: $('.txtDescricaoDesenvolvimento').val(),
				barramentoNormas: $('.rbBarramentoNormas:checked').val() == 1 ? true : false,
				barramentoAdequacoes: $('.txtAdequacoesDimensionamentoBarramento').val(),
				vazaoMinTipo: ($('.rbFase:checked')).val() == 1 ?
					$('.ddlTipoDispositivoVazaoMin:visible').val() : $('.ddlTipoDispositivoVazaoMinAConstruir:visible').val(),
				vazaoMinDiametro: ($('.rbFase:checked')).val() == 1 ?
					$('.txtDiametroTubulacaoVazaoMin').val() : $('.txtDiametroTubulacaoVazaoMinAConstruir').val(),
				vazaoMinInstalado: $('.rbVazaoMinInstalado:checked').val() == 1 ? true : false,
				vazaoMinNormas: $('.rbVazaoMinNormas:checked').val() == 1 ? true : false,
				vazaoMinAdequacoes: $('.txtAdequacoesDimensionamentoVazaoMin').val(),
				vazaoMaxTipo: ($('.rbFase:checked')).val() == 1 ?
					$('.ddlTipoDispositivoVazaoMax:visible').val() : $('.ddlTipoDispositivoVazaoMaxAConstruir:visible').val(),
				vazaoMaxDiametro: ($('.rbFase:checked')).val() == 1 ?
					$('.txtDiametroTubulacaoVazaoMax').val() : $('.txtDiametroTubulacaoVazaoMaxAConstruir').val(),
				vazaoMaxInstalado: $('.rbVazaoMaxInstalado:checked').val() == 1 ? true : false,
				vazaoMaxNormas: $('.rbVazaoMaxNormas:checked').val() == 1 ? true : false,
				vazaoMaxAdequacoes: $('.txtAdequacoesDimensionamentoVazaoMax').val(),
				periodoInicioObra: $('.txtperiodoInicioObra').val(),
				periodoTerminoObra: $('.txtperiodoTerminoObra').val()
				
			},
		};

		if (objeto.fase == 2) {
			objeto.construidaConstruir.isSupressaoAPP = $('.rbPerguntaSupressaoAConstruir:checked').val();

			if (objeto.construidaConstruir.periodoInicioObra.length < 7) {
				Mensagem.gerar(BarragemDispensaLicenca.container, [BarragemDispensaLicenca.settings.mensagens.PeriodoInicioRequired]);
				return false;
			}
			if (objeto.construidaConstruir.periodoTerminoObra.length < 7) {
				Mensagem.gerar(BarragemDispensaLicenca.container, [BarragemDispensaLicenca.settings.mensagens.PeriodoTerminoRequired]);
				return false;
			}
		}

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

		objeto.coordenadas = BarragemDispensaLicenca.obterCoordenadas();
		objeto.responsaveisTecnicos = BarragemDispensaLicenca.obterRT();
		if (!objeto.responsaveisTecnicos)
			return false;

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
			var arquivo = null;

			if (tipo[i].codigo == 2 && !BarragemDispensaLicenca.settings.profissoesSemAutorizacao.contains($('.ddlRT' + tipo[i].nome + 'Profissao').val())) {
				arquivo = $('.hdnArquivo').val();
				if (arquivo.isNullOrWhitespace()) {
					Mensagem.gerar(BarragemDispensaLicenca.container, [BarragemDispensaLicenca.settings.mensagens.ArquivoObrigatorio]);
					return;
				} else
					arquivo = JSON.parse(arquivo);
			}

			var rt = {
				id: $('.hdnRT' + tipo[i].nome + 'Id').val(),
				nome: $('.txtRT' + tipo[i].nome + 'Nome').val(),
				tipo: tipo[i].codigo,
				profissao: { Id: $('.ddlRT' + tipo[i].nome + 'Profissao').val() },
				registroCREA: $('.txtRT' + tipo[i].nome + 'CREA').val(),
				numeroART: $('.txtRT' + tipo[i].nome + 'Numero').val(),
				autorizacaoCREA: arquivo,
				proprioDeclarante: $('.cb' + tipo[i].nome + 'CopiaDeclarante:checked').val()
			};
			//Se tiver algum campo preenchido, todos são requeridos
			if ((rt.nome.isNullOrWhitespace() || rt.profissao.Id == 0 || rt.registroCREA.isNullOrWhitespace() || rt.numeroART.isNullOrWhitespace()) &&
				(!rt.nome.isNullOrWhitespace() || rt.profissao.Id > 0 || !rt.registroCREA.isNullOrWhitespace() || !rt.numeroART.isNullOrWhitespace() )) {
					Mensagem.gerar(BarragemDispensaLicenca.container, [BarragemDispensaLicenca.settings.mensagens.RtRequired]);
				return false;
			}
			if (rt.nome.isNullOrWhitespace()) rt.id = 0;
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
		var carac = BarragemDispensaLicenca.obter();
		if (!carac) return;

		Mensagem.limpar(BarragemDispensaLicenca.container);
		MasterPage.carregando(true);

		$.ajax({
			url: BarragemDispensaLicenca.settings.urls.salvarConfirm,
			data: JSON.stringify({ caracterizacao: carac, projetoDigitalId: $('.hdnProjetoDigitalId', BarragemDispensaLicenca.container).val() }),
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
		var carac = BarragemDispensaLicenca.obter();
		if (!carac) return;

        Mensagem.limpar(BarragemDispensaLicenca.container);
		MasterPage.carregando(true);

        $.ajax({
            url: BarragemDispensaLicenca.settings.urls.salvar,
			data: JSON.stringify({ caracterizacao: carac, projetoDigitalId: $('.hdnProjetoDigitalId', BarragemDispensaLicenca.container).val() }),
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

	bloquearCriar: function () {
		var count = $('.associadoAoProjeto', BarragemDispensaLicenca.container).val()
		if (count > 0)
		{
			$('.btnAdicionar').prop('disabled', true);
			$('.btnAdicionar').addClass('disabled');
		}
		if (count > 0)
		{
			$('.btnAssociar').prop('disabled', true);
			$('.btnAssociar').addClass('desativado');
		}

	},

	criar: function () {
		MasterPage.redireciona(BarragemDispensaLicenca.settings.urls.salvar + '/' + BarragemDispensaLicenca.settings.empreendimentoId +
			'?projetoDigitalId=' + BarragemDispensaLicenca.settings.projetoDigitalId);
	},

	associar: function () {
		var caracterizacao = $(this).closest('tr').find('.hdnId').val();		
		var tid = $(this).closest('tr').find('.hdnTid').val();		
		$.get(BarragemDispensaLicenca.settings.urls.associar, { projetoDigitalId: BarragemDispensaLicenca.settings.projetoDigitalId, caracterizacao: caracterizacao, tid: tid },
			function (response) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(BarragemDispensaLicenca.container, response.Msg);
				}
			}
		);
	},
	
	desassociar: function () {
		var caracterizacao = $(this).closest('tr').find('.hdnId').val();		
		var possuiAssociacaoExterna = $(this).closest('tr').find('.dependencias').val() == 'True';

		if (possuiAssociacaoExterna)
		{
			Modal.confirma({
				btnOkLabel: 'Confirmar',
				titulo: 'Cancelar associação da barragem',
				conteudo: 'Essa ação irá desfazer a associação com o projeto digital. Deseja continuar?',
				btnOkCallback: function (conteudoModal) {
					Modal.fechar(conteudoModal);

					$.get(BarragemDispensaLicenca.settings.urls.desassociar, { projetoDigitalId: BarragemDispensaLicenca.settings.projetoDigitalId, caracterizacao: caracterizacao },
						function (response) {
							if (response.EhValido) {
								MasterPage.redireciona(response.UrlRedirecionar);
								return;
							}

							if (response.Msg && response.Msg.length > 0) {
								Mensagem.gerar(BarragemDispensaLicenca.container, response.Msg);
							}
						}
					);
				}
			});
		}
		else
		{
			Modal.confirma({
				btnOkLabel: 'Confirmar',
				titulo: 'Cancelar associação da barragem',
				conteudo: 'Essa ação deletará a caracterização de barragem. Deseja continuar?',
				btnOkCallback: function (conteudoModal) {
					Modal.fechar(conteudoModal);

					$.get(BarragemDispensaLicenca.settings.urls.desassociar, { projetoDigitalId: BarragemDispensaLicenca.settings.projetoDigitalId, caracterizacao: caracterizacao },
						function (response) {
							if (response.EhValido) {
								MasterPage.redireciona(response.UrlRedirecionar);
								return;
							}

							if (response.Msg && response.Msg.length > 0) {
								Mensagem.gerar(BarragemDispensaLicenca.container, response.Msg);
							}
						}
					);
				}
			});
		}
	},

	editar: function () {
		//MasterPage.redireciona(BarragemDispensaLicenca.settings.urls.editar + '/' + BarragemDispensaLicenca.settings.empreendimentoId + '?projetoDigitalId=' + BarragemDispensaLicenca.settings.projetoDigitalId);
		var id = $(this).closest('tr').find('.hdnId').val();

		MasterPage.redireciona(BarragemDispensaLicenca.settings.urls.editar + '/' + id + '?empreendimentoId=' +
			BarragemDispensaLicenca.settings.empreendimentoId +
			'&projetoDigitalId=' + BarragemDispensaLicenca.settings.projetoDigitalId +
			'&retornarVisualizar=' + ($('.btnFinalizarPasso', BarragemDispensaLicenca.container).length <= 0));
	},

	visualizar: function () {
		var id = $(this).closest('tr').find('.hdnId').val();

		MasterPage.redireciona(BarragemDispensaLicenca.settings.urls.visualizar + '/' + id + '?empreendimentoId='+
			BarragemDispensaLicenca.settings.empreendimentoId +
			'&projetoDigitalId=' + BarragemDispensaLicenca.settings.projetoDigitalId +
			'&retornarVisualizar=' + ($('.btnFinalizarPasso', BarragemDispensaLicenca.container).length <= 0));
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
		$('.txtLarguraDemarcada').val('');
		$('.rbLarguraDemarcadaLegislacao:checked').prop('checked', false);
		$('.rbFaixaCercada:checked').prop('checked', false);
		$('.txtDescricaoDesenvolvimento').val('');
	},

	onChangeBarramento: function () {
		if ($('.rbBarramentoNormas:checked').val() == 0) {
			$('.AdequacoesDimensionamentoBarramento').removeClass('hide');

		} else {
			$('.AdequacoesDimensionamentoBarramento').addClass('hide');
		}
		$('.txtAdequacoesDimensionamentoBarramento').val('');
	},

	onChangeVazaoMinInstalado: function () {
		if ($('.rbVazaoMinInstalado:checked').val() == 1) {
			$('.vazaoMinNormas').removeClass('hide');
			$('.AdequacoesDimensionamentoVazaoMin').addClass('hide');
		} else {
			$('.vazaoMinNormas').addClass('hide');
			$('.AdequacoesDimensionamentoVazaoMin').addClass('hide');
			$('.AdequacoesDimensionamentoVazaoMin').removeClass('hide');
		}
		$('.rbVazaoMinNormas:checked').prop('checked', false);
		$('.txtAdequacoesDimensionamentoVazaoMin').val('');
	},

	onChangeVazaoMinNormas: function () {
		if ($('.rbVazaoMinNormas:checked').val() == 0) {
			$('.AdequacoesDimensionamentoVazaoMin').removeClass('hide');

		} else {
			$('.AdequacoesDimensionamentoVazaoMin').addClass('hide');
		}
		$('.txtAdequacoesDimensionamentoVazaoMin').val('');
	},

	onChangeVazaoMaxInstalado: function () {
		if ($('.rbVazaoMaxInstalado:checked').val() == 1) {
			$('.vazaoMaxNormas').removeClass('hide');
			$('.AdequacoesDimensionamentoVazaoMax').addClass('hide');
		} else {
			$('.vazaoMaxNormas').addClass('hide');
			$('.AdequacoesDimensionamentoVazaoMax').removeClass('hide');
		}
		$('.rbVazaoMaxNormas:checked').prop('checked', false);
		$('.txtAdequacoesDimensionamentoVazaoMax').val('');
	},

	onChangeVazaoMaxNormas: function () {
		if ($('.rbVazaoMaxNormas:checked').val() == 0) {
			$('.AdequacoesDimensionamentoVazaoMax').removeClass('hide');

		} else {
			$('.AdequacoesDimensionamentoVazaoMax').addClass('hide');
		}
		$('.txtAdequacoesDimensionamentoVazaoMax').val('');
	},

	limparConstruidaAConstruir: function () {
		$('.rbSupressaoAPP:checked').prop('checked', false);
		$('.rbDemarcaoAPP:checked').prop('checked', false);
		$('.txtLarguraDemarcada').val('');
		$('.rbLarguraDemarcadaLegislacao:checked').prop('checked', false);
		$('.rbFaixaCercada:checked').prop('checked', false);
		$('.txtDescricaoDesenvolvimento').val('');
		$('.rbBarramentoNormas:checked').prop('checked', false);
		$('.txtAdequacoesDimensionamentoBarramento').val('');
		$('.AdequacoesDimensionamentoBarramento').addClass('hide');

		$('.ddlTipoDispositivoVazaoMin').val(0);
		$('.txtDiametroTubulacaoVazaoMin').val('');
		$('.rbVazaoMinInstalado:checked').prop('checked', false);
		$('.rbVazaoMinNormas:checked').prop('checked', false);
		$('.txtAdequacoesDimensionamentoVazaoMin').val('');

		$('.ddlTipoDispositivoVazaoMax').val(0);
		$('.txtDiametroTubulacaoVazaoMax').val('');
		$('.rbVazaoMaxInstalado:checked').prop('checked', false);
		$('.rbVazaoMaxNormas:checked').prop('checked', false);
		$('.txtAdequacoesDimensionamentoVazaoMax').val('');

		/*A Construir*/
		$('.rbPerguntaSupressaoAContruir:checked').prop('checked', false);
		$('.ddlTipoDispositivoVazaoMinAConstruir').val(0);
		$('.txtDiametroTubulacaoVazaoMinAConstruir').val('');
		$('.ddlTipoDispositivoVazaoMaxAConstruir').val(0);
		$('.txtDiametroTubulacaoVazaoMaxAConstruir').val('');
		$('.txtperiodoInicioObra').val('');
		$('.txtperiodoTerminoObra').val('');

		$('.boxApp').addClass('hide');
		$('.vazaoMinNormas').addClass('hide');
		$('.AdequacoesDimensionamentoVazaoMin').addClass('hide');
		$('.vazaoMaxNormas').addClass('hide');
		$('.AdequacoesDimensionamentoVazaoMax').addClass('hide');
		
		//$('.rbPossuiMonge, .rbPossuiVertedouro, .rbPossuiEstruturaHidraulica', BarragemDispensaLicenca.container).removeAttr('checked');
	},
	//-------------------------  --------------------//

	
	onCheckCopiaDeclarante: function () {
		var container = this.parentElement.parentElement.parentElement;
		if ($('.cbCopiaDeclarante:checked', container).val()) {
			nomeDeclarante = $('.txtRTElaboracaoDeclaracaoNome').val();
			profissaoDeclarante = $('.ddlRTElaboracaoDeclaracaoProfissao:visible').val();
			creaDeclarante = $('.txtRTElaboracaoDeclaracaoCREA').val();

			$('.rtNome', container).val(nomeDeclarante).prop('disabled', true).addClass('disabled');
			$('.rtProfissao', container).val(profissaoDeclarante).prop('disabled', true).addClass('disabled');
			$('.rtCREA', container).val(creaDeclarante).prop('disabled', true).addClass('disabled');
		} else {
			$('.rtNome', container).val('').prop('disabled', false).removeClass('disabled');
			$('.rtProfissao', container).val(' ').prop('disabled', false).removeClass('disabled');
			$('.rtCREA', container).val(' ').prop('disabled', false).removeClass('disabled');
			$('.rtNumero', container).val(' ');
		}
	},

	onChangeProfissaoRT: function () {
		if (!BarragemDispensaLicenca.settings.profissoesSemAutorizacao.contains($('.ddlRTElaboracaoProjetoProfissao:visible').val())) 
			$('.arquivoRT').removeClass('hide');
		else 
			$('.arquivoRT').addClass('hide');
	},

	onChangeAreaAlagada: function () {
		if (parseFloat($('.txtAreaAlagada').val()) >= 1)
			$('.isDemarcacaoAPPNaoSeAplica').addClass('hide');
		else
			$('.isDemarcacaoAPPNaoSeAplica').removeClass('hide');
	}
}
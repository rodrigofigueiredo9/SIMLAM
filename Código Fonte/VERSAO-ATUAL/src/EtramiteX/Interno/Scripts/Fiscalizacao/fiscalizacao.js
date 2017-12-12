
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Pessoa/inline.js" />
/// <reference path="../Empreendimento/inline.js" />

Fiscalizacao = {
	salvarEdicao: false,
	salvarTelaAtual: null,
	stepAtual: 1,
	Mensagens: null,
	containerMensagem: null,
	container: null,
	containerAba: null,
	modo: 1,
	objetoFiscalizacao: {},
	urls: {
		localInfracaoVisualizar: '',
		autuado: '',
		projetoGeografico: '',
		objetoInfracao: '',
		diagnostico: '',
        outrasPenalidadesVisualizar: '',
		consideracaoFinalVisualizar: '',
		finalizar: '',
		infracao: '',
		materialApreendido: '',
		documentosGerados: '',
		multa: '',
	},

	load: function (container) {
		Fiscalizacao.container = container;
		Fiscalizacao.containerMensagem = MasterPage.getContent(container);

		Fiscalizacao.configurarStepWizard();

		FiscalizacaoLocalInfracao.callBackObterLocalInfracao();

		container.delegate('.btnSalvar', 'click', Fiscalizacao.gerenciarWizard);

		if (parseInt($('#hdnFiscalizacaoId', Fiscalizacao.container).val()) > 0) {
			FiscalizacaoLocalInfracao.configurarBtnEditar();
		} else {
			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarCadastro: true });
		}
		Fiscalizacao.containerAba = $('.conteudoFiscalizacao', Fiscalizacao.container);

        //esconde as abas das infrações. Só deverão ser exibidas se as infrações correspondentes forem selecionadas.
		$('.step4', Fiscalizacao.container).hide();
		$('.step5', Fiscalizacao.container).hide();
		$('.step6', Fiscalizacao.container).hide();
		$('.step7', Fiscalizacao.container).hide();

		var abas = [];

		var infracao = JSON.parse($('.hdnInfracoes', Fiscalizacao.container).val());

		if (infracao.PossuiAdvertencia == true) {
		    abas.push('advertencia');
		}
		if (infracao.PossuiMulta == true) {
		    abas.push('multa');
		}
		if (infracao.PossuiApreensao == true) {
		    abas.push('apreensao');
		}
		if (infracao.PossuiInterdicaoEmbargo == true) {
		    abas.push('interdicaoembargo');
		}

		Fiscalizacao.ocultarAbas(abas);
	},

	gerenciarWizardAbas: function () {

		if (Fiscalizacao.stepAtual === +$(this).data('step')) {
			return;
		}

		if (Fiscalizacao.modo == 1) {
			if (Fiscalizacao.salvarEdicao) {
				if (!Fiscalizacao.salvarTelaAtual()) {
					return;
				}
			} else {
				Mensagem.limpar(Fiscalizacao.containerMensagem);
			}
		}

		MasterPage.carregando(true);

		$('.divFinalizar, .divPdf', Fiscalizacao.container).addClass('hide');
		var objeto = Fiscalizacao.gerarObjetoWizard();
		objeto.step = Fiscalizacao.obterStep(this);

		Fiscalizacao.switchGerenciarWizard(objeto);
	},

	gerarObjetoWizard: function () {
	    var param = { id: $('#hdnFiscalizacaoId', Fiscalizacao.container).val() };
		return { step: 0, params: param };
	},

	switchGerenciarWizard: function (objeto) {
		switch (objeto.step) {

			case 1:
				Fiscalizacao.onObterStep(Fiscalizacao.urls.localInfracaoVisualizar, objeto.params, function () {
					FiscalizacaoLocalInfracao.callBackObterLocalInfracao();
					Fiscalizacao.gerenciarVisualizacao('.hdnLocalInfracaoId');
				});
				break;

			case 2:
				Fiscalizacao.onObterStep(Fiscalizacao.urls.projetoGeografico, objeto.params, function () {
					FiscalizacaoProjetoGeografico.callBackObterProjetoGeograficoVisualizar();
					Fiscalizacao.gerenciarVisualizacao('.hdnProjetoId');
				});
				break;

			case 3:
				Fiscalizacao.onObterStep(Fiscalizacao.urls.infracao, objeto.params, function () {
					Infracao.callBackObterInfracaoVisualizar();
					Fiscalizacao.gerenciarVisualizacao('.hdnInfracaoId');
				});
				break;

		    case 4:
		        Fiscalizacao.onObterStep(Fiscalizacao.urls.multa, objeto.params, function () {
		            FiscalizacaoMulta.callBackObterFiscalizacaoMultaVisualizar();
		            Fiscalizacao.gerenciarVisualizacao('.hdnMultaId');
		        });
		        break;

			case 5:
				Fiscalizacao.onObterStep(FiscalizacaoObjetoInfracao.settings.urls.visualizar, objeto.params, function () {
					FiscalizacaoObjetoInfracao.callBackObterObjetoInfracaoVisualizar();
					Fiscalizacao.gerenciarVisualizacao('.hdnObjetoInfracaoId');
				});
				break;

			case 6:
				Fiscalizacao.onObterStep(Fiscalizacao.urls.materialApreendido, objeto.params, function () {
					FiscalizacaoMaterialApreendido.callBackObterFiscalizacaoMaterialApreendidoVisualizar();
					Fiscalizacao.gerenciarVisualizacao('.hdnMaterialApreendidoId');
				});
				break;

		    case 7:
		        Fiscalizacao.onObterStep(Fiscalizacao.urls.outrasPenalidadesVisualizar, objeto.params, function () {
		            FiscalizacaoOutrasPenalidades.callBackObterFiscalizacaoOutrasPenalidadesVisualizar();
		            Fiscalizacao.gerenciarVisualizacao('.hdnOutrasPenalidadesId');
		        });
		        break;

            case 8:
				Fiscalizacao.onObterStep(Fiscalizacao.urls.consideracaoFinalVisualizar, objeto.params, function () {
					FiscalizacaoConsideracaoFinal.callBackObterConsideracaoFinalVisualizar();
					Fiscalizacao.gerenciarVisualizacao('.hdnConsideracaoFinalId');
				});
				break;

			case 9:
				Fiscalizacao.onObterStep(Fiscalizacao.urls.finalizar, objeto.params, function () {
					FiscalizacaoFinalizar.callBackObterFiscalizacaoFinalizar();
					Fiscalizacao.gerenciarVisualizacao();
				});
				break;
		}
	},

	gerenciarWizard: function () {

		if (Fiscalizacao.stepAtual === +$(this).data('step')) {
			return;
		}

		if (!Fiscalizacao.salvarTelaAtual()) {
			return;
		}

		MasterPage.carregando(true);
		$('.divFinalizar, .divPdf', Fiscalizacao.container).addClass('hide');

		var objeto = Fiscalizacao.gerarObjetoWizard();
		objeto.step = Fiscalizacao.obterStep(this);
		Fiscalizacao.switchGerenciarWizard(objeto);
	},

	obterStep: function (container) {

	    if ($(container).data('step') !== null && $(container).data('step') !== NaN && $(container).data('step') !== undefined) {
	        var step = +$(container).data('step');

	        return step;
		}

		if ($(container).hasClass('btnVoltar')) {
		    var step = Fiscalizacao.stepAtual - 1;

		    while ($('.step' + step, Fiscalizacao.container).hasClass('isOculta')) {
		        step--;
		    }
		}

		if ($(container).hasClass('btnAvancar') || $(container).hasClass('btnSalvar')) {
		    var step = Fiscalizacao.stepAtual + 1;

		    while ($('.step' + step, Fiscalizacao.container).hasClass('isOculta')) {
		        step++;
		    }
		}

		return step;
	},

	configurarStepWizard: function () {

		$('.AbasFiscalizacao ul li').each(function (i) {
			$(this).data("step", (i + 1));
			$(this).click(Fiscalizacao.gerenciarWizardAbas);
		});
	},

	alternarAbas: function () {
		$('.AbasFiscalizacao .ativo').removeClass('ativo');
		$('.AbasFiscalizacao ul li').each(function () {
			if (parseInt($(this).data('step')) === Fiscalizacao.stepAtual) {
				$(this).addClass('ativo');
				return;
			}
		});
	},

	onObterStep: function (urlStep, params, callBack) {
		$.ajax({ url: urlStep,
			type: "GET",
			data: params,
			cache: false,
			async: true,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
				MasterPage.carregando(false);
			},
			success: function (response, textStatus, XMLHttpRequest) {
			    Fiscalizacao.containerAba.empty().hide();
			    Fiscalizacao.containerAba.append(response);
			    callBack();
			}
		});
	},

	onSalvarStep: function (url, objetoStep, msg, abas) {

		var isSalvo = false;

		$.ajax({
			url: url,
			type: "POST",
			data: JSON.stringify(objetoStep),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Fiscalizacao.containerMensagem, response.Msg);
					return;
				}

				Mensagem.gerar(Fiscalizacao.containerMensagem, msg);
				isSalvo = true;
			}
		});
		
		if (abas != null) {
		    Fiscalizacao.ocultarAbas(abas);
		}

		return isSalvo;
	},

	ocultarAbas: function(abas){
	    var multa = false;
	    var interdicao = false;
	    var apreensao = false;
	    var outras = false;

	    abas.forEach(function (aba) {
	        if (aba == 'multa') {
	            $('.step4', Fiscalizacao.container).show();
	            $('.step4', Fiscalizacao.container).removeClass('isOculta');
	            multa = true;
	        } else if (aba == 'interdicaoembargo') {
	            $('.step5', Fiscalizacao.container).show();
	            $('.step5', Fiscalizacao.container).removeClass('isOculta');
	            interdicao = true;
	        } else if (aba == 'apreensao') {
	            $('.step6', Fiscalizacao.container).show();
	            $('.step6', Fiscalizacao.container).removeClass('isOculta');
	            apreensao = true;
	        } else if (aba == 'outras') {
	            $('.step7', Fiscalizacao.container).show();
	            $('.step7', Fiscalizacao.container).removeClass('isOculta');
	            outras = true;
	        } else if (aba == 'advertencia') {
	            $('.step7', Fiscalizacao.container).show();
	            $('.step7', Fiscalizacao.container).removeClass('isOculta');
	            outras = true;
	        }
	    });

	    if (multa == false) {
	        $('.step4', Fiscalizacao.container).hide();
	        $('.step4', Fiscalizacao.container).addClass('isOculta');
	    }

	    if (interdicao == false) {
	        $('.step5', Fiscalizacao.container).hide();
	        $('.step5', Fiscalizacao.container).addClass('isOculta');
	    }

	    if (apreensao == false) {
	        $('.step6', Fiscalizacao.container).hide();
	        $('.step6', Fiscalizacao.container).addClass('isOculta');
	    }

	    if (outras == false) {
	        $('.step7', Fiscalizacao.container).hide();
	        $('.step7', Fiscalizacao.container).addClass('isOculta');
	    }
	},

	obterReqInterEmp: function (urlBuscar, params) {
		if (Fiscalizacao.ReqInterEmp && Fiscalizacao.ReqInterEmp.empreendimentoId != 0) {
			return;
		}

		$.ajax({ url: urlBuscar,
			type: "GET",
			data: params,
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Fiscalizacao.containerMensagem, response.Msg);
				}
				else {
					Fiscalizacao.ReqInterEmp = response.Fiscalizacao;
				}
			}
		});
	},

	botoes: function (botoes) {
		$('.btnSalvar', Fiscalizacao.container).val('Salvar');

		$(".divSalvar", Fiscalizacao.container).toggleClass('hide', typeof botoes.btnSalvar == 'undefined');
		$(".divEditar", Fiscalizacao.container).toggleClass('hide', typeof botoes.btnEditar == 'undefined');
		$(".divIntNovo", Fiscalizacao.container).toggleClass('hide', typeof botoes.btnIntAssNovo == 'undefined');
		$(".divEmpAvancar", Fiscalizacao.container).toggleClass('hide', typeof botoes.btnEmpAvancar == 'undefined');
		$(".divEmpNovo", Fiscalizacao.container).toggleClass('hide', typeof botoes.btnEmpAssNovo == 'undefined');
		$(".divFinalizar", Fiscalizacao.container).toggleClass('hide', typeof botoes.btnFinalizar == 'undefined');
		$(".divVoltar", Fiscalizacao.container).toggleClass('hide', typeof botoes.btnVoltar == 'undefined');
		$(".divPdf", Fiscalizacao.container).toggleClass('hide', typeof botoes.btnPdf == 'undefined');
		$(".spnCancelarEdicao", Fiscalizacao.container).toggleClass('hide', typeof botoes.spnCancelarEdicao == 'undefined');
		$(".spnCancelarCadastro", Fiscalizacao.container).toggleClass('hide', typeof botoes.spnCancelarCadastro == 'undefined');
	},

	bloquearCampos: function (container) {
		$('.modoVisualizar', container).remove();
		$('.bloquear', container).attr('disabled', 'disabled').addClass('disabled');
	},

	configurarBtnCancelarStep: function (step) {
		Aux.scrollTop(Fiscalizacao.container);
		$('.linkCancelarEdicao', Fiscalizacao.container).unbind('click');
		$(".linkCancelarEdicao", Fiscalizacao.container).click(function () {
			var objeto = Fiscalizacao.gerarObjetoWizard();
			objeto.step = step;
			Fiscalizacao.switchGerenciarWizard(objeto);
		});
	},

	gerenciarVisualizacao: function (strSeletorHiddenAbaId) {
		if (strSeletorHiddenAbaId && Fiscalizacao.modo == 3/*Visualizar*/) {
			if (parseInt($(strSeletorHiddenAbaId, Fiscalizacao.containerAba).val()) > 0) {
				Fiscalizacao.containerAba.fadeIn();
				return;
			}
			var divMensagemTemplate = $('.divMensagemTemplate', Fiscalizacao.container).clone();
			$('.lblMensagem', divMensagemTemplate).text('Dados não cadastrados');
			Fiscalizacao.containerAba.empty().append(divMensagemTemplate.removeClass('hide')).fadeIn();
			return;
		}
		Fiscalizacao.containerAba.fadeIn();
		Listar.atualizarEstiloTable();
	}
}

// 1ª Aba - Local da Infração
FiscalizacaoLocalInfracao = {
	settings: {
		urls: {
			obter: '',
			salvar: '',
			coordenadaGeo: '',
			obterEstadosMunicipiosPorCoordenada: '',
			associarAutuadoPessoa: '',
			editarAutuadoPessoa: '',
			localizarEmpreendimento: '',
			localizarEmpreendimentoPessoa: '',
			visualizarEmpreendimento: '',
			editarEmpreendimento: '',
			novoEmpreendimento: '',
			salvarCadastrar: '',
			obterResponsaveis: ''
		},
		modo: 1
	},
	isLoad: true,
	container: null,

	load: function (container, options) {
		if (options) { $.extend(FiscalizacaoLocalInfracao.settings, options); }
		FiscalizacaoLocalInfracao.container = MasterPage.getContent(container);

		Fiscalizacao.stepAtual = 1;
		Fiscalizacao.salvarTelaAtual = FiscalizacaoLocalInfracao.onClickSalvar;
		Fiscalizacao.alternarAbas();

		if (FiscalizacaoLocalInfracao.isLoad) {
			FiscalizacaoLocalInfracao.isLoad = false;
			FiscalizacaoLocalInfracao.container.delegate('.btnBuscarCoorLocal', 'click', FiscalizacaoLocalInfracao.onBuscarCoordenada);
			FiscalizacaoLocalInfracao.container.delegate('.ddlEstado', 'change', Aux.onEnderecoEstadoChange);
			FiscalizacaoLocalInfracao.container.delegate('.rblAutuado', 'click', FiscalizacaoLocalInfracao.onClickRadioAutuado);
			FiscalizacaoLocalInfracao.container.delegate('.btnBuscarPessoa', 'click', FiscalizacaoLocalInfracao.onClickBuscarPessoa);
			FiscalizacaoLocalInfracao.container.delegate('.btnEditarVisualizarPessoa', 'click', FiscalizacaoLocalInfracao.onClickEditarVisualizarPessoa);
			FiscalizacaoLocalInfracao.container.delegate('.btnVerificarEmp, .btnEmpAssNovo', 'click', FiscalizacaoLocalInfracao.onClickVerificarEmp);
			FiscalizacaoLocalInfracao.container.delegate('.btnEditarEmpreendimento', 'click', FiscalizacaoLocalInfracao.onVisualizarEmp);
			FiscalizacaoLocalInfracao.container.delegate('.btnEmpAssociar', 'click', FiscalizacaoLocalInfracao.onClickAssociarEmp);
			FiscalizacaoLocalInfracao.container.delegate('.btnEmpSalvar', 'click', FiscalizacaoLocalInfracao.onEditarEmp);
			FiscalizacaoLocalInfracao.container.delegate('.btnEmpNovo', 'click', FiscalizacaoLocalInfracao.onNovoEmp);
			FiscalizacaoLocalInfracao.container.delegate('.btnEmpSalvarCadastrar', 'click', FiscalizacaoLocalInfracao.onClickSalvarEmp);
			FiscalizacaoLocalInfracao.container.delegate('.btnEmpSalvarEditar', 'click', FiscalizacaoLocalInfracao.onEditarCadastroEmp);

			FiscalizacaoLocalInfracao.container.delegate('.btnVerificarEmpPessoa, .btnEmpAssNovoPessoa', 'click', FiscalizacaoLocalInfracao.onClickVerificarEmpPessoa);
			FiscalizacaoLocalInfracao.container.delegate('.btnEmpBuscaLocal', 'click', FiscalizacaoLocalInfracao.onClickAtivarBuscaLocalizacao);
		}

		if (parseInt($('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val()) > 0) {
			if ($('.hdnFiscalizacaoSituacaoId', Fiscalizacao.container).val() >= 2) {
				FiscalizacaoLocalInfracao.onCarregarVisualizarEmp(parseInt($('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val()), $('.hdnAutuadoEmpreendimentoTid', FiscalizacaoLocalInfracao.container).val());
			} else {
				FiscalizacaoLocalInfracao.onCarregarVisualizarEmp(parseInt($('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val()));
			}
			FiscalizacaoLocalInfracao.toggleBotoes('.spanEmpSalvar, .spanEmpAssNovo, .fdsEmpreendimento, .divDdlResponsavel');
		}

		if ($('.rblAutuado:checked', FiscalizacaoLocalInfracao.container).val() == 0) {  //"não"
		    $('.divAreaAbrangencia', FiscalizacaoLocalInfracao.container).hide();
		} else if ($('.rblAutuado:checked', FiscalizacaoLocalInfracao.container).val() == 1) {    //"sim"
		    $('.divAreaAbrangencia', FiscalizacaoLocalInfracao.container).show();
		}
	},

	configurarBtnEditar: function () {
		$(".btnEditar", Fiscalizacao.container).unbind('click');
		$(".btnEditar", Fiscalizacao.container).click(FiscalizacaoLocalInfracao.onBtnEditar);
	},

	callBackObterLocalInfracao: function () {
		var context = $('.divLocalInfracao', Fiscalizacao.container);
		FiscalizacaoLocalInfracao.load(context);
		Mascara.load(context);
		Fiscalizacao.salvarEdicao = false;
		Fiscalizacao.botoes({ btnEditar: true, spnCancelarCadastro: true });
		FiscalizacaoLocalInfracao.configurarBtnEditar();
		MasterPage.carregando(false);
	},

	onBtnEditar: function () {

		Fiscalizacao.onObterStep(FiscalizacaoLocalInfracao.settings.urls.obter, Fiscalizacao.gerarObjetoWizard().params, function () {
			var context = $('.divLocalInfracao', Fiscalizacao.container);
			FiscalizacaoLocalInfracao.load(context);
			Mascara.load(context);
			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarEdicao: true });
			Fiscalizacao.configurarBtnCancelarStep(1);
			MasterPage.carregando(false);
			MasterPage.botoes(context);
			Fiscalizacao.gerenciarVisualizacao();
		});
	},

	onBuscarCoordenada: function () {

		Modal.abrir(FiscalizacaoLocalInfracao.settings.urls.coordenadaGeo, null, function (container) {
			Coordenada.load(container, {
				northing: $('.txtNorthing', FiscalizacaoLocalInfracao.container).val(),
				easting: $('.txtEasting', FiscalizacaoLocalInfracao.container).val(),
				pagemode: 'editMode',
				callBackSalvarCoordenada: FiscalizacaoLocalInfracao.setarCoordenada
			});
			Modal.defaultButtons(container);
		},
		Modal.tamanhoModalGrande);
	},

	setarCoordenada: function (retorno) {

		retorno = JSON.parse(retorno);

		$('.txtNorthing', FiscalizacaoLocalInfracao.container).val(retorno.northing);
		$('.txtEasting', FiscalizacaoLocalInfracao.container).val(retorno.easting);

		$.ajax({ url: FiscalizacaoLocalInfracao.settings.urls.obterEstadosMunicipiosPorCoordenada + '?easting=' + retorno.easting + '&northing=' + retorno.northing,
			cache: false, async: false, type: 'GET',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(FiscalizacaoLocalInfracao.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Fiscalizacao.container, response.Msg);
					return;
				}

				if (response.Municipio.Estado.Id > 0) {
					$('.ddlEstado', FiscalizacaoLocalInfracao.container).ddlLoad(response.Estados, { selecionado: response.Municipio.Estado.Id });
					$('.ddlMunicipio', FiscalizacaoLocalInfracao.container).ddlLoad(response.Municipios, { selecionado: response.Municipio.Id });
					$('.ddlEstado,.ddlMunicipio', FiscalizacaoLocalInfracao.container).addClass('disabled').attr('disabled', 'disabled');
				} else {
					$('.ddlEstado', FiscalizacaoLocalInfracao.container).ddlLoad(response.Estados);
					$('.ddlMunicipio', FiscalizacaoLocalInfracao.container).ddlClear();
				}

				$('.btnBuscarCoorLocal', FiscalizacaoLocalInfracao.container).focus();
			}
		});

		if ($('.rblAutuado:checked', FiscalizacaoLocalInfracao.container).val().toString() == "1") {
		    $('.divEmpreendimento', FiscalizacaoLocalInfracao.container).show();
		    $('.fsEmpreendimentoBuscar', FiscalizacaoLocalInfracao.container).show();

		    $('.divBtnVerificarEmpreendimento', FiscalizacaoLocalInfracao.container).show();
		    $('.btnVerificarEmp', FiscalizacaoLocalInfracao.container).show();
		    $('.btnVerificarEmpPessoa', FiscalizacaoLocalInfracao.container).hide();

		    $('.fdsEmpreendimento', FiscalizacaoLocalInfracao.container).hide();

		    $('.divDdlResponsavel', FiscalizacaoLocalInfracao.container).hide();
		}
	},

	gerarObjetoFiltroLocalizar: function () {

		return JSON.stringify({
			Filtros: {
				EstadoId: $('.ddlEstado', FiscalizacaoLocalInfracao.container).val(),
				MunicipioId: $('.ddlMunicipio', FiscalizacaoLocalInfracao.container).val(),
				AreaAbrangencia: $('.txtAreaAbran', FiscalizacaoLocalInfracao.container).val(),
				Coordenada: {
					Abrangencia: $('.txtAreaAbran', FiscalizacaoLocalInfracao.container).val(),
					Datum: {
						Id: $('.ddlDatum', FiscalizacaoLocalInfracao.container).val()
					},
					EastingUtmTexto: $('.txtEasting', FiscalizacaoLocalInfracao.container).val(),
					FusoUtm: $('.ddlFuso', FiscalizacaoLocalInfracao.container).val(),
					HemisferioUtm: $('.ddlHemisferio', FiscalizacaoLocalInfracao.container).val(),
					NorthingUtmTexto: $('.txtNorthing', FiscalizacaoLocalInfracao.container).val(),
					Tipo: {
						Id: $('.ddlCoordenadaTipo', FiscalizacaoLocalInfracao.container).val()
					}
				}
			},
			CpfCnpj: $('.txtCpfCnpj', FiscalizacaoLocalInfracao.container).val()
		});
	},

	gerarObjetoLocalInfracao: function () {

		var localInfracao = {
			Id: $('.hdnFiscalizacaoId', Fiscalizacao.container).val(),
			LocalInfracao: {
				Id: $('.hdnLocalInfracaoId', FiscalizacaoLocalInfracao.container).val(),
				FiscalizacaoId: $('.hdnFiscalizacaoId', Fiscalizacao.container).val(),
				SetorId: $('.ddlSetores', FiscalizacaoLocalInfracao.container).val(),
			    //Data: { DataTexto: $('.txtData', FiscalizacaoLocalInfracao.container).val() },
				SistemaCoordId: $('.ddlCoordenadaTipo', FiscalizacaoLocalInfracao.container).val(),
				Datum: $('.ddlDatum', FiscalizacaoLocalInfracao.container).val(),
				AreaAbrangencia: $('.txtAreaAbran', FiscalizacaoLocalInfracao.container).val(),
				LatNorthing: $('.txtNorthing', FiscalizacaoLocalInfracao.container).val(),
				LonEasting: $('.txtEasting', FiscalizacaoLocalInfracao.container).val(),
				Hemisferio: $('.ddlHemisferio', FiscalizacaoLocalInfracao.container).val(),
				MunicipioId: $('.ddlMunicipio', FiscalizacaoLocalInfracao.container).val(),
				Local: $('.txtLocal', FiscalizacaoLocalInfracao.container).val(),
				PessoaId: $('.hdnAutuadoPessoaId', FiscalizacaoLocalInfracao.container).val(),
				EmpreendimentoId: $('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val(),
				ResponsavelId: $('.ddlResponsaveis', FiscalizacaoLocalInfracao.container).val(),
				ResponsavelPropriedadeId: $('.ddlResponsaveisPropriedade', FiscalizacaoLocalInfracao.container).val()
			},
			SituacaoId: $('.hdnFiscalizacaoSituacaoId', Fiscalizacao.container).val()
		};

		$('.rblAreaFiscalizacao', FiscalizacaoLocalInfracao.container).each(function () {
		    if ($(this).attr('checked')) {
		        localInfracao.LocalInfracao.AreaFiscalizacao = $(this).val();
		    }
		});

		$('.rblAutuado', FiscalizacaoLocalInfracao.container).each(function () {
		    if ($(this).attr('checked')) {
		        localInfracao.LocalInfracao.DentroEmpreendimento = $(this).val();
		    }
		});

		return localInfracao;
	},

	onClickRadioAutuado: function () {
	    
		$('.divPessoa, .divEmpreendimento, .fdsEmpreendimento, .divDdlResponsavel', FiscalizacaoLocalInfracao.container).addClass("hide");
		$('.empreendimentoPartial', FiscalizacaoLocalInfracao.container).empty();
		$('.hdnAutuadoEmpreendimentoId, .hdnAutuadoPessoaId', FiscalizacaoLocalInfracao.container).val('');
		$('.spanVisualizarAutuado', FiscalizacaoLocalInfracao.container).addClass('hide');
		$('.txtNomeRazao', FiscalizacaoLocalInfracao.container).val('');
		$('.txtCpfCnpj', FiscalizacaoLocalInfracao.container).val('');
		$('.ddlResponsaveis option', FiscalizacaoLocalInfracao.container).remove();
		$('.ddlResponsaveisPropriedade option', FiscalizacaoLocalInfracao.container).remove();

		$('.divPessoa', FiscalizacaoLocalInfracao.container).removeClass("hide");

		$('.fsLocalInfracao', FiscalizacaoLocalInfracao.container).hide();
		$('.txtAreaAbran', FiscalizacaoLocalInfracao.container).val('');
		$('.txtEasting', FiscalizacaoLocalInfracao.container).val('');
		$('.txtNorthing', FiscalizacaoLocalInfracao.container).val('');
		$('.ddlEstado', FiscalizacaoLocalInfracao.container).ddlClear();
		$('.ddlMunicipio', FiscalizacaoLocalInfracao.container).ddlClear();
		$('.txtLocal', FiscalizacaoLocalInfracao.container).val('');
		$('.txtLocal', FiscalizacaoLocalInfracao.container).text('');

		$('.fsEmpreendimentoBuscar', FiscalizacaoLocalInfracao.container).hide();


		if ($(this).val().toString() == "0") {  //"não"
		    $('.divAreaAbrangencia', FiscalizacaoLocalInfracao.container).hide();
		} else {    //"sim"
		    $('.divAreaAbrangencia', FiscalizacaoLocalInfracao.container).show();
		}
	},

	onClickBuscarPessoa: function () {

		var pessoaModalInte = new PessoaAssociar();

		Modal.abrir(FiscalizacaoLocalInfracao.settings.urls.associarAutuadoPessoa, null, function (container) {
			pessoaModalInte.load(container, {
				tituloCriar: 'Cadastrar Autuado',
				tituloEditar: 'Editar Autuado',
				tituloVisualizar: 'Visualizar Autuado',
				onAssociarCallback: FiscalizacaoLocalInfracao.callBackAutuadoPessoa
			});
		});
	},

	onClickEditarVisualizarPessoa: function () {

		var id = $('.hdnAutuadoPessoaId', FiscalizacaoLocalInfracao.container).val();
		var tid = $('.hdnAutuadoPessoaTid', FiscalizacaoLocalInfracao.container).val() || '';
		var pessoaModalInte = new PessoaAssociar();

		var url = FiscalizacaoLocalInfracao.settings.urls.editarAutuadoPessoa + "/?id=" + id;
		if ($('.hdnFiscalizacaoSituacaoId', Fiscalizacao.container).val() >= 2) url += '&tid=' + tid;

		Modal.abrir(url, null, function (container) {
			pessoaModalInte.load(container, {
				tituloCriar: 'Cadastrar Autuado',
				tituloEditar: 'Editar Autuado',
				tituloVisualizar: 'Visualizar Autuado',
				onAssociarCallback: FiscalizacaoLocalInfracao.callBackAutuadoPessoa,
				editarVisualizar: Fiscalizacao.salvarEdicao
			});
		});
	},

	callBackAutuadoPessoa: function (Pessoa) {
		$('.spanVisualizarAutuado', FiscalizacaoLocalInfracao.container).removeClass('hide');
		$('.hdnAutuadoPessoaId', FiscalizacaoLocalInfracao.container).val(Pessoa.Id);
		$('.txtNomeRazao', FiscalizacaoLocalInfracao.container).val(Pessoa.NomeRazaoSocial);
		$('.txtCpfCnpj', FiscalizacaoLocalInfracao.container).val(Pessoa.CPFCNPJ);

        //Dentro de empreedimento == sim
		if ($('.rblAutuado:checked', FiscalizacaoLocalInfracao.container).val().toString() == "1") {
		    $('.divEmpreendimento', FiscalizacaoLocalInfracao.container).removeClass("hide");
		    $('.divEmpreendimento', FiscalizacaoLocalInfracao.container).show();
		    $('.fsEmpreendimentoBuscar', FiscalizacaoLocalInfracao.container).removeClass("hide");
		    $('.fsEmpreendimentoBuscar', FiscalizacaoLocalInfracao.container).show();

		    $('.divBtnVerificarEmpreendimento', FiscalizacaoLocalInfracao.container).show();
		    $('.fdsEmpreendimento', FiscalizacaoLocalInfracao.container).hide();
		    $('.btnVerificarEmpPessoa', FiscalizacaoLocalInfracao.container).show();
		    $('.btnVerificarEmp', FiscalizacaoLocalInfracao.container).hide();

		    $('.fsLocalInfracao', FiscalizacaoLocalInfracao.container).hide();
		}
        //Dentro de empreendimento == não
		else if ($('.rblAutuado:checked', FiscalizacaoLocalInfracao.container).val().toString() == "0") {
		    $('.fsLocalInfracao', FiscalizacaoLocalInfracao.container).removeClass("hide");
		    $('.fsLocalInfracao', FiscalizacaoLocalInfracao.container).show();
		}

		return true;
	},

	toggleBotoes: function (seletor, fnCancelar) {
		$('.spanEmpAssociar, .spanEmpSalvar, .spanEmpNovo, .spanEmpSalvarCadastrar, .spanEmpSalvarEditar, .spanEmpAssNovo, .spanCancelarEmp, .divDdlResponsavel', FiscalizacaoLocalInfracao.container).addClass('hide');
		$(seletor, FiscalizacaoLocalInfracao.container).removeClass('hide');
		if (fnCancelar) {
			$('.linkCancelarEmp', Fiscalizacao.container).unbind('click');
			$('.linkCancelarEmp', Fiscalizacao.container).click(fnCancelar);
		}
	},

	onClickVerificarEmp: function () {
		$('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val('');
		$('.hdnResponsavelId', FiscalizacaoLocalInfracao.container).val(0);

		MasterPage.carregando(true);

		$.ajax({ url: FiscalizacaoLocalInfracao.settings.urls.localizarEmpreendimento,
			data: FiscalizacaoLocalInfracao.gerarObjetoFiltroLocalizar(),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, FiscalizacaoLocalInfracao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
			    if (response.EhValido) {
			        $('.fsEmpreendimentoBuscar', FiscalizacaoLocalInfracao.container).show();
			        $('.divBtnVerificarEmpreendimento', FiscalizacaoLocalInfracao.container).hide();

			        $('.fdsEmpreendimento', FiscalizacaoLocalInfracao.container).show();

					$('.divResultados', FiscalizacaoLocalInfracao.container).html(response.Html);
					$('.divResultados', FiscalizacaoLocalInfracao.container).removeClass('hide');
					$('.empreendimentoPartial', FiscalizacaoLocalInfracao.container).empty();
					FiscalizacaoLocalInfracao.toggleBotoes('.spanEmpNovo, .fdsEmpreendimento');

					$('.spanEmpBuscaLocal', FiscalizacaoLocalInfracao.container).hide();
					$('.spanEmpAssNovoPessoa', FiscalizacaoLocalInfracao.container).hide();

					$('.spanEmpNovo', FiscalizacaoLocalInfracao.container).show();
					$('.spanEmpAssNovo', FiscalizacaoLocalInfracao.container).show();

					$('.spanEmpNovoPessoa', FiscalizacaoLocalInfracao.container).hide();
					$('.spanEmpAssNovoPessoa', FiscalizacaoLocalInfracao.container).hide();

					Mensagem.limpar(Fiscalizacao.container);
				} else {

					$(response.Msg).each(function (i, item) {
						if (item.Campo.indexOf('Municipio') > -1) {
							item.Campo = 'LocalInfracao_MunicipioId';
							return;
						}
						if (item.Campo.indexOf('AreaAbrangencia') > -1) {
							item.Campo = 'LocalInfracao_AreaAbrangencia';
							return;
						}
						if (item.Campo.indexOf('Easting') > -1) {
							item.Campo = 'LocalInfracao_Setor_Easting';
							return;
						}
						if (item.Campo.indexOf('Northing') > -1) {
							item.Campo = 'LocalInfracao_Setor_Northing';
						}
					});
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Fiscalizacao.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},

	onClickVerificarEmpPessoa: function () {
	    $('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val('');
	    $('.hdnResponsavelId', FiscalizacaoLocalInfracao.container).val(0);

	    MasterPage.carregando(true);

	    $.ajax({
	        url: FiscalizacaoLocalInfracao.settings.urls.localizarEmpreendimentoPessoa,
	        data: FiscalizacaoLocalInfracao.gerarObjetoFiltroLocalizar(),
	        cache: false,
	        async: false,
	        type: 'POST',
	        dataType: 'json',
	        contentType: 'application/json; charset=utf-8',
	        error: function (XMLHttpRequest, textStatus, erroThrown) {
	            Aux.error(XMLHttpRequest, textStatus, erroThrown, FiscalizacaoLocalInfracao.container);
	        },
	        success: function (response, textStatus, XMLHttpRequest) {

	            if (response.EhValido) {
	                $('.fdsEmpreendimento', FiscalizacaoLocalInfracao.container).show();

	                $('.divResultados', FiscalizacaoLocalInfracao.container).html(response.Html);
	                $('.divResultados', FiscalizacaoLocalInfracao.container).removeClass('hide');
	                $('.empreendimentoPartial', FiscalizacaoLocalInfracao.container).empty();
	                FiscalizacaoLocalInfracao.toggleBotoes('.spanEmpBuscaLocal, .fdsEmpreendimento');

	                $('.spanEmpNovo', FiscalizacaoLocalInfracao.container).hide();
	                $('.spanEmpAssNovo', FiscalizacaoLocalInfracao.container).hide();

	                $('.spanEmpNovoPessoa', FiscalizacaoLocalInfracao.container).show();
	                $('.spanEmpAssNovoPessoa', FiscalizacaoLocalInfracao.container).show();

	                $('.btnEmpBuscaLocal', FiscalizacaoLocalInfracao.container).show();
	                $('.spanEmpBuscaLocal', FiscalizacaoLocalInfracao.container).show();

	                Mensagem.limpar(Fiscalizacao.container);
	            } else {

	                $(response.Msg).each(function (i, item) {
	                    if (item.Campo.indexOf('Municipio') > -1) {
	                        item.Campo = 'LocalInfracao_MunicipioId';
	                        return;
	                    }
	                    if (item.Campo.indexOf('AreaAbrangencia') > -1) {
	                        item.Campo = 'LocalInfracao_AreaAbrangencia';
	                        return;
	                    }
	                    if (item.Campo.indexOf('Easting') > -1) {
	                        item.Campo = 'LocalInfracao_Setor_Easting';
	                        return;
	                    }
	                    if (item.Campo.indexOf('Northing') > -1) {
	                        item.Campo = 'LocalInfracao_Setor_Northing';
	                    }
	                });
	            }
	            if (response.Msg && response.Msg.length > 0) {
	                Mensagem.gerar(Fiscalizacao.container, response.Msg);
	            }
	        }
	    });

	    MasterPage.carregando(false);
	},

	onVisualizarEmp: function () {

		var id = parseInt($(this).closest("tr").find('.hdnEmpreendimentoId').val());
		var tid = $('.hdnAutuadoEmpreendimentoTid', FiscalizacaoLocalInfracao.container).val();

		if (isNaN(id)) {
			id = parseInt($('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val());
		}

		FiscalizacaoLocalInfracao.onCarregarVisualizarEmp(id, tid);
	},

	onCarregarVisualizarEmp: function (id, tid) {
		MasterPage.carregando(true);

		tid = (tid == null) ? '' : tid;

		$.ajax({ url: FiscalizacaoLocalInfracao.settings.urls.visualizarEmpreendimento,
			data: { id: id, tid: tid, mostrarTituloTela: false }, cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				$('.empreendimentoPartial', FiscalizacaoLocalInfracao.container).html(response);
				$('.divResultados', FiscalizacaoLocalInfracao.container).addClass('hide');
				FiscalizacaoLocalInfracao.toggleBotoes('.spanEmpAssociar, .spanEmpAssNovo');
			}
		});

		MasterPage.carregando(false);
	},

	onClickAssociarEmp: function () {

		MasterPage.carregando(true);

		$.ajax({ url: FiscalizacaoLocalInfracao.settings.urls.obterResponsaveis,
			data: $.toJSON({ empreendimentoId: $('.hdnEmpId', FiscalizacaoLocalInfracao.container).val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					Mensagem.limpar(Fiscalizacao.container);
					var itens = [];
					$.each(response.Responsaveis, function (i, item) { itens.push(item); });

					$('.ddlResponsaveis', FiscalizacaoLocalInfracao.container).ddlLoad(response.Responsaveis);
					$('.ddlResponsaveisPropriedade', FiscalizacaoLocalInfracao.container).ddlLoad(itens);

					$('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val($('.hdnEmpId', FiscalizacaoLocalInfracao.container).val());
					FiscalizacaoLocalInfracao.toggleBotoes('.spanEmpSalvar, .spanEmpAssNovo, .divDdlResponsavel');

					$('.divDdlResponsavel', FiscalizacaoLocalInfracao.container).show();
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Fiscalizacao.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);

	},

	onEditarEmp: function () {
		MasterPage.carregando(true);

		$.ajax({ url: FiscalizacaoLocalInfracao.settings.urls.editarEmpreendimento,
			data: { id: $('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val() }, cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(FiscalizacaoLocalInfracao.container, response.Msg);
					return;
				}

				var content = $(response);
				$('.titTela, br:first', content).remove();
				$('.btnAsmAssociar:first', content).show();

				//hack para IE 7,8 - o metodo .empty() ele não retorna content ao lipar o conteudo do div;
				$('.empreendimentoPartial', FiscalizacaoLocalInfracao.container).empty();
				$('.empreendimentoPartial', FiscalizacaoLocalInfracao.container).append(content);

				Empreendimento.settings.container = $('.divDadosEmpreendimento', FiscalizacaoLocalInfracao.container);

				EmpreendimentoAssociar.load(Empreendimento.settings.container, {
					onAssociarCallback: function () { },
					editarVisualizar: true
				});
				Mascara.load(Empreendimento.settings.container);

				$('.divResultados', FiscalizacaoLocalInfracao.container).addClass('hide');

				FiscalizacaoLocalInfracao.toggleBotoes('.spanEmpSalvarEditar, .spanEmpAssNovo, .spanCancelarEmp', function () {

					var empreendimentoId = parseInt($('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val());
					var empreendimentoTid = $('.hdnAutuadoEmpreendimentoTid', FiscalizacaoLocalInfracao.container).val();

					FiscalizacaoLocalInfracao.onCarregarVisualizarEmp(empreendimentoId, empreendimentoTid);
					FiscalizacaoLocalInfracao.toggleBotoes('.spanEmpSalvar, .spanEmpAssNovo, .fdsEmpreendimento, .divDdlResponsavel');
					FiscalizacaoLocalInfracao.onClickAssociarEmp();

					if ($('.ddlResponsaveis option', FiscalizacaoLocalInfracao.container).length > 2) {
						$('.ddlResponsaveis', FiscalizacaoLocalInfracao.container).val($('.hdnResponsavelId', FiscalizacaoLocalInfracao.container).val());
					}
					if ($('.ddlResponsaveisPropriedade option', FiscalizacaoLocalInfracao.container).length > 2) {
						$('.ddlResponsaveisPropriedade', FiscalizacaoLocalInfracao.container).val($('.hdnResponsavelPropriedadeId', FiscalizacaoLocalInfracao.container).val());
					}
				});
			}
		});

		MasterPage.carregando(false);
	},

	onNovoEmp: function () {

		MasterPage.carregando(true);

		$.ajax({
		    url: FiscalizacaoLocalInfracao.settings.urls.novoEmpreendimento,
		    data: FiscalizacaoLocalInfracao.gerarObjetoFiltroLocalizar(),
		    type: 'POST',
		    typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				var content = $('.empreendimentoPartial', FiscalizacaoLocalInfracao.container);
				Empreendimento.settings.container = $('.divDadosEmpreendimento', FiscalizacaoLocalInfracao.container);

				content.empty();
				content.append(response);

				EmpreendimentoAssociar.load($('.divDadosEmpreendimento', FiscalizacaoLocalInfracao.container), {
					onAssociarCallback: function () { },
					editarVisualizar: true
				});

				MasterPage.botoes(content);
				Mascara.load(content);

				$('.titTela, br:first', content).remove();
				$('.btnAsmAssociar:first', content).show();
				$('.btnAsmEditar:first', content).hide();

				$('.divResultados', FiscalizacaoLocalInfracao.container).addClass('hide');
				FiscalizacaoLocalInfracao.toggleBotoes('.spanEmpSalvarCadastrar, .spanEmpAssNovo');
			}
		});
		MasterPage.carregando(false);

	},

	onClickSalvarEmp: function () {

		MasterPage.carregando(true);

		$.ajax({ url: FiscalizacaoLocalInfracao.settings.urls.salvarCadastrar, data: $.toJSON(EmpreendimentoSalvar.criarObjetoEmpreendimento(Empreendimento.settings.container)), type: 'POST', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.IsEmpreendimentoSalvo) {

					$('.hdnEmpId', FiscalizacaoLocalInfracao.container).val(response.Empreendimento.Id);

					FiscalizacaoLocalInfracao.onClickAssociarEmp();
					FiscalizacaoLocalInfracao.onCarregarVisualizarEmp(response.Empreendimento.Id, response.Empreendimento.Tid);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Fiscalizacao.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},

	onEditarCadastroEmp: function () {

		MasterPage.carregando(true);

		$.ajax({ url: FiscalizacaoLocalInfracao.settings.urls.editarEmpreendimento, data: $.toJSON(EmpreendimentoSalvar.criarObjetoEmpreendimento(Empreendimento.settings.container)), type: 'POST', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.IsEmpreendimentoSalvo) {

					$('.hdnEmpId', FiscalizacaoLocalInfracao.container).val(response.Empreendimento.Id);

					FiscalizacaoLocalInfracao.onClickAssociarEmp();
					FiscalizacaoLocalInfracao.onCarregarVisualizarEmp(response.Empreendimento.Id, response.Empreendimento.Tid);

					FiscalizacaoLocalInfracao.toggleBotoes('.spanEmpSalvar, .spanEmpAssNovo, .spanCancelarEmp');

					FiscalizacaoLocalInfracao.onClickAssociarEmp();

					if ($('.ddlResponsaveis option', FiscalizacaoLocalInfracao.container).length > 2) {
						$('.ddlResponsaveis', FiscalizacaoLocalInfracao.container).val($('.hdnResponsavelId', FiscalizacaoLocalInfracao.container).val());
					}
					if ($('.ddlResponsaveisPropriedade option', FiscalizacaoLocalInfracao.container).length > 2) {
						$('.ddlResponsaveisPropriedade', FiscalizacaoLocalInfracao.container).val($('.hdnResponsavelPropriedadeId', FiscalizacaoLocalInfracao.container).val());
					}
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Fiscalizacao.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},

	onClickAtivarBuscaLocalizacao: function(){
	    MasterPage.carregando(true);

	    $('.fsEmpreendimentoBuscar', FiscalizacaoLocalInfracao.container).hide();
	    $('.fsLocalInfracao', FiscalizacaoLocalInfracao.container).show();

        MasterPage.carregando(false);
	},

	onClickSalvar: function () {

		var flag = false;

		MasterPage.carregando(true);

		$.ajax({ url: FiscalizacaoLocalInfracao.settings.urls.salvar,
			data: $.toJSON({ fiscalizacao: FiscalizacaoLocalInfracao.gerarObjetoLocalInfracao() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				flag = response.EhValido;

				if (response.EhValido) {
					Mensagem.limpar(Fiscalizacao.container);
					$('.hdnFiscalizacaoId', Fiscalizacao.container).val(response.id);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Fiscalizacao.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);

		return flag;
	}
}

// 2ª Aba - Progeto Geográfico
FiscalizacaoProjetoGeografico = {

	urlObter: '',

	callBackObterProjetoGeograficoVisualizar: function () {

		Fiscalizacao.stepAtual = 2;
		Fiscalizacao.alternarAbas();

		$('.rblProjGeo', Fiscalizacao.container).click(FiscalizacaoProjetoGeografico.onClickRadioProjGeo);

		var projetoContainer = $('.projetoGeograficoContainer');

		ProjetoGeografico.load(projetoContainer,
		{
			projetoId: $('.hdnProjetoId', $('.conteudoFiscalizacao')).val(),
			containerMsg: Fiscalizacao.containerMensagem
		});

		MasterPage.botoes(projetoContainer);

		if (parseInt($('.hdnProjetoId', Fiscalizacao.container).val()) > 0) {		
			Fiscalizacao.salvarEdicao = false;				
			Fiscalizacao.botoes({ btnEditar: true, spnCancelarCadastro: true });
			FiscalizacaoProjetoGeografico.configurarBtnEditar();
		} else {
			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarCadastro: true });
		}

		$('.projetoGeograficoContainer', Fiscalizacao.container).addClass('hide');
		Fiscalizacao.salvarTelaAtual = FiscalizacaoProjetoGeografico.onSalvarProjetoEmBranco;
		if ($('.hdnProjetoNivelPrecisao', Fiscalizacao.container).val().toString() != "0") {
		    $('.projetoGeograficoContainer', Fiscalizacao.container).removeClass('hide');
		    Fiscalizacao.salvarTelaAtual = ProjetoGeografico.onSalvar;
		}

		MasterPage.carregando(false);
	},

	configurarBtnEditar: function () {
		$(".btnEditar", Fiscalizacao.container).unbind('click');
		$(".btnEditar", Fiscalizacao.container).click(FiscalizacaoProjetoGeografico.onBtnEditar);
	},

	callBackObterProjetoGeografico: function () {

		Fiscalizacao.stepAtual = 2;
		FiscalizacaoResponsavel.mostrarBtnEditar = true;
		FiscalizacaoResponsavel.configurarAssociarMultiploResponsavel();
		Fiscalizacao.salvarTelaAtual = FiscalizacaoResponsavel.onSalvarResponsavelTecnico;
		Fiscalizacao.alternarAbas();

		Fiscalizacao.botoes({ btnSalvar: true, spnCancelarEdicao: true });

		Fiscalizacao.configurarBtnCancelarStep(2);
		MasterPage.carregando(false);
	},

	onBtnEditar: function () {		
		
		Fiscalizacao.onObterStep(FiscalizacaoProjetoGeografico.urlObter, Fiscalizacao.gerarObjetoWizard().params, function () {
			
			FiscalizacaoProjetoGeografico.callBackObterProjetoGeograficoVisualizar();

			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.configurarBtnCancelarStep(2);					
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarEdicao: true });
			Fiscalizacao.gerenciarVisualizacao();

		});
	},

	onSalvar: function () {

		ProjetoGeografico.onSalvar();

		var arrayMensagem = new Array();

		arrayMensagem.push(Fiscalizacao.Mensagens.ResponsavelSalvar);

		//var params = { id: $('#hdnFiscalizacaoId').val(), responsaveis: objetoResponsaveis };

		return true;

		//return Fiscalizacao.onSalvarStep(FiscalizacaoResponsavel.urlCriarResponsavel, params, arrayMensagem);
	},

	onSalvarProjetoEmBranco: function(){
	    return true;
	},

	onClickRadioProjGeo: function () {
	    $('.projetoGeograficoContainer', Fiscalizacao.container).addClass('hide');
	    Fiscalizacao.salvarTelaAtual = FiscalizacaoProjetoGeografico.onSalvarProjetoEmBranco;
	    if ($(this).val().toString() != "0") {
	        $('.projetoGeograficoContainer', Fiscalizacao.container).removeClass('hide');
	        Fiscalizacao.salvarTelaAtual = ProjetoGeografico.onSalvar;
	    }
	}
}

// 3ª Aba - Infração/Fiscalização
Infracao = {
	settings: {
		urls: {
			salvar: '',
			obterTipo: '',
			obterItem: '',
			obterConfiguracao: '',
			obterSerie: '',
			enviarArquivo: '',
			obter: ''
		},
		mensagens: ''
	},
	TiposArquivo: [],
	container: null,

	callBackObterInfracaoVisualizar: function () {

		Fiscalizacao.stepAtual = 3;
		Fiscalizacao.salvarTelaAtual = Infracao.onSalvarInfracao;
		Fiscalizacao.alternarAbas();

		$('.fsInfracao', Infracao.container).hide();
		$('.ddlTiposPenalidade', Infracao.container).attr('disabled', 'disable');
		$('.ddlTiposPenalidade', Infracao.container).addClass('disabled');

	    Infracao.container.delegate('.rdoComInfracao', 'change', Infracao.onSelecionarComInfracao);
	    Infracao.container.delegate('.rdoSemInfracao', 'change', Infracao.onSelecionarSemInfracao);
	    Infracao.container.delegate('.ddlClassificacoes', 'change', Infracao.onSelecionarClassificacao);
	    Infracao.container.delegate('.ddlTipos', 'change', Infracao.onSelecionarTipo);
	    Infracao.container.delegate('.ddlItens', 'change', Infracao.onSelecionarItem);
	    Infracao.container.delegate('.cbPenalidade', 'change', Infracao.onMarcarPenalidade);
	    Infracao.container.delegate('.ddlTiposPenalidade', 'change', Infracao.onSelecionarPenalidade);

		Mascara.load(Infracao.container);

		if (parseInt($('.hdnInfracaoId', Infracao.container).val()) > 0) {
			Fiscalizacao.salvarEdicao = false;
			Fiscalizacao.botoes({ btnEditar: true, spnCancelarCadastro: true });
			Infracao.configurarBtnEditar();
		} else {
			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarCadastro: true });
		}

		if ($('.hdnConfigAlterou', Infracao.container).val() === "true") {

			Modal.confirma({
				removerFechar: true,
				btnOkLabel: 'Confirmar',
				btnOkCallback: function (conteudoModal) {
					Modal.fechar(conteudoModal);
				},
				btCancelLabel: 'Cancelar',
				btnCancelCallback: function (conteudoModal) {
					Modal.fechar(conteudoModal);
					$('.linkCancelarEdicao', Fiscalizacao.container).click();
				},
				conteudo: Infracao.settings.mensagens.ConfigAlteradaConfirme.Texto,
				titulo: 'Configuração Alterada',
				tamanhoModal: Modal.tamanhoModalMedia
			});
		}

		if ($('.rdoComInfracao', Infracao.container).attr('checked') == true) {
		    Infracao.onSelecionarComInfracao();
		} else if ($('.rdoSemInfracao', Infracao.container).attr('checked') == true) {
		    Infracao.onSelecionarSemInfracao();
		}

		$('.cbPenalidadeOutras', Infracao.container).each(function () {
		    if ($(this).attr('checked')) {
		        if ($(this).attr('disabled') == false) {
		            $(this).closest('.block').find('.ddlTiposPenalidade').removeAttr('disabled');
		            $(this).closest('.block').find('.ddlTiposPenalidade').removeClass('disabled');
		        }

		        var container = $(this).closest('.block');

		        var penalidade = $('.ddlTiposPenalidade :selected', container).val();
		        var descricao = $('.hdnPenalidade' + penalidade, Infracao.container).val();

		        $('.txtDescricaoPenalidade', container).val(descricao);
		    }
		});

		MasterPage.botoes();
		MasterPage.carregando(false);
	},

	onSalvarInfracao: function () {

	    var container = Infracao.container;

	    var obj = {
	        Id: Number($('.hdnInfracaoId', container).val()),
	        FiscalizacaoId: Number($('.hdnFiscalizacaoId', Fiscalizacao.container).val()),
	        ClassificacaoId: $('.ddlClassificacoes :selected', container).val(),
	        TipoId: $('.ddlTipos :selected', container).val(),
	        ItemId: $('.ddlItens :selected', container).val(),
	        ConfiguracaoId: $('.hdnConfiguracaoId', container).val(),
	        ConfiguracaoTid: $('.hdnConfiguracaoTid', container).val(),
	        Campos: [],
	        Perguntas: [],
	    }
        
	    var abas = [];

	    if ($('.rdoComInfracao', container).attr('checked')) {
	        obj.ComInfracao = true;

	        //Quadro de Enquadramento
	        var EnquadramentoInfracao = {
	            Id: $('.enquadramentoId', container).val(),
	            Artigos: []
	        };

	        $('.divQuadroEnquadramento .dataGridTable tbody tr', container).each(function () {
	            var item = {
	                ArtigoTexto: $(this).find('.txtArtigoEnquadramento').val().trim(),
	                ArtigoParagrafo: $(this).find('.txtItemEnquadramento').val().trim(),
	                DaDo: $(this).find('.txtLeiEnquadramento').val().trim(),
	            };

	            
	            if (item.ArtigoTexto != '' && item.DaDo != '') {  //  && item.ArtigoParagrafo != '' // NAO PRECISA MAIS O 'item.ArtigoParagrafo' = item/	coluna: “Item/Parágrafo/Alínea”.
	                EnquadramentoInfracao.Artigos.push(item);
	            }
	        });

	        obj.EnquadramentoInfracao = EnquadramentoInfracao;

            //Informações da fiscalização
	        obj.DescricaoInfracao = $('.txtDescricaoInfracao', container).val(); 
	        obj.DataConstatacao = { DataTexto: $('.txtDataConstatacao', container).val() };
	        obj.HoraConstatacao = $('.txtHoraConstatacao', container).val();
            
	        if ($('.rdoClassificacaoLeve', container).attr('checked')) {
	            obj.ClassificacaoInfracao = 0;
	        } else if ($('.rdoClassificacaoMedia', container).attr('checked')) {
	            obj.ClassificacaoInfracao = 1;
	        } else if ($('.rdoClassificacaoGrave', container).attr('checked')) {
	            obj.ClassificacaoInfracao = 2;
	        } else if ($('.rdoClassificacaoGravissima', container).attr('checked')) {
	            obj.ClassificacaoInfracao = 3;
	        }

	        //Penalidades - Itens fixos
	        obj.PossuiAdvertencia = $('.cbPenalidadeAdvertencia', container).attr('checked');
	        obj.PossuiMulta = $('.cbPenalidadeMulta', container).attr('checked');
	        obj.PossuiApreensao = $('.cbPenalidadeApreensao', container).attr('checked');
	        obj.PossuiInterdicaoEmbargo = $('.cbPenalidadeInterdicaoEmbargo', container).attr('checked');

	        if (obj.PossuiAdvertencia) {
	            abas.push('advertencia');
	        }
	        if (obj.PossuiMulta) {
	            abas.push('multa');
	        }
	        if (obj.PossuiApreensao) {
	            abas.push('apreensao');
	        }
	        if (obj.PossuiInterdicaoEmbargo) {
	            abas.push('interdicaoembargo');
	        }

	        //Penalidades - Outros
	        obj.IdsOutrasPenalidades = [];
	        var possuiOutras = false;
	        $('.ddlTiposPenalidadeOutras', container).each(function () {
	            if ($(this).val() > 0) {
	                possuiOutras = true;
	                obj.IdsOutrasPenalidades.push($(this).val());
	            }
	        });
	        if (possuiOutras) {
	            abas.push('outras');
	        }

	    } else if ($('.rdoSemInfracao', container).attr('checked')) { 
	        obj.ComInfracao = false;

	        //Informações da fiscalização
	        obj.DataConstatacao = { DataTexto: $('.txtDataConstatacao', container).val() };
	        obj.HoraConstatacao = $('.txtHoraConstatacao', container).val();
	    } 

	    if ($('.ddlSubitens :selected', container).val() != '0') {
	        obj.SubitemId = $('.ddlSubitens :selected', container).val();
	    }

	    $('.divCampos .divCampo', container).each(function (index, item) {
	        obj.Campos.push({
	            Id: $('.hdnCampoInfracaoId', item).val(),
	            CampoId: $('.hdnCampoId', item).val(),
	            Texto: $('.txtTexto', item).val()
	        });
	    });

	    $('.divQuestionarios .divQuestionario', container).each(function (index, item) {

	        var respostaId = $('.rdoResposta:checked', item).val();
	        var perguntaId = $('.hdnPerguntaId', item).val();
	        var especificar = $('.hdnRespostaEspecificar' + perguntaId, item).val() == "1";

	        obj.Perguntas.push({
	            Id: $('.hdnQuestionarioId', item).val(),
	            PerguntaId: perguntaId,
	            PerguntaTid: $('.hdnPerguntaTid', item).val(),
	            RespostaId: respostaId,
	            RespostaTid: $('.hdnRespostaTid' + respostaId, item).val(),
	            Especificacao: (especificar) ? $('.txtEspecificar', item).val() : '',
	            IsEspecificar: especificar
	        });
	    });

	    var arrayMensagem = [];

	    arrayMensagem.push(Infracao.settings.mensagens.Salvar);

	    return Fiscalizacao.onSalvarStep(Infracao.settings.urls.salvar, obj, arrayMensagem, abas);
	},

	configurarBtnEditar: function () {
		$(".btnEditar", Fiscalizacao.container).unbind('click');
		$(".btnEditar", Fiscalizacao.container).click(Infracao.onBtnEditar);
	},

	onBtnEditar: function () {

		Fiscalizacao.onObterStep(Infracao.settings.urls.obter, Fiscalizacao.gerarObjetoWizard().params, function () {

			Infracao.callBackObterInfracaoVisualizar();
			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarEdicao: true });
			Fiscalizacao.configurarBtnCancelarStep(3);
			Fiscalizacao.gerenciarVisualizacao();
		});
	},

	onSelecionarClassificacao: function () {
	    var classificacao = $('.ddlClassificacoes', Infracao.container).val();

	    if (classificacao) {

	        $('.ddlTipos', Infracao.container).ddlClear();
	        $('.ddlItens', Infracao.container).ddlClear();
	        $('.ddlSubitens', Infracao.container).ddlClear();
	        $('.divCamposPerguntas', Infracao.container).html('');

	        $.ajax({
	            url: Infracao.settings.urls.obterTipo,
	            data: JSON.stringify({ classificacaoId: classificacao }),
	            cache: false,
	            async: true,
	            type: 'POST',
	            dataType: 'json',
	            contentType: 'application/json; charset=utf-8',
	            error: function (XMLHttpRequest, textStatus, erroThrown) {
	                Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Infracao.container));
	            },
	            success: function (response, textStatus, XMLHttpRequest) {

	                if (response.EhValido) {
	                    $('.ddlTipos', Infracao.container).ddlLoad(response.Tipos);

	                    if (response.Tipos.length == 2) {
	                        Infracao.onSelecionarTipo();
	                    }
	                }
	            }
	        });
	    }

	},

	onSelecionarTipo: function () {

	    var tipo = $('.ddlTipos', Infracao.container).val();
	    var classificacao = $('.ddlClassificacoes', Infracao.container).val();

	    if (tipo && classificacao) {

	        $('.ddlItens', Infracao.container).ddlClear();

	        $.ajax({
	            url: Infracao.settings.urls.obterItem,
	            data: JSON.stringify({ tipoId: tipo, classificacaoId: classificacao }),
	            cache: false,
	            async: true,
	            type: 'POST',
	            dataType: 'json',
	            contentType: 'application/json; charset=utf-8',
	            error: function (XMLHttpRequest, textStatus, erroThrown) {
	                Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Infracao.container));
	            },
	            success: function (response, textStatus, XMLHttpRequest) {

	                if (response.EhValido) {
	                    $('.ddlItens', Infracao.container).ddlLoad(response.Itens, { disabled: false });
	                    $('.ddlSubitens', Infracao.container).ddlClear();
	                    $('.divCamposPerguntas', Infracao.container).html('');

	                    if (response.Itens.length == 2) {
	                        Infracao.onSelecionarItem();
	                    }
	                }
	            }
	        });
	    }

	},

	onSelecionarItem: function () {

	    var item = $('.ddlItens', Infracao.container).val();
	    var tipo = $('.ddlTipos', Infracao.container).val();
	    var classificacao = $('.ddlClassificacoes', Infracao.container).val();

	    if (item && tipo && classificacao) {

	        $('.ddlSubitens', Infracao.container).ddlClear();

	        $.ajax({
	            url: Infracao.settings.urls.obterConfiguracao,
	            data: JSON.stringify({ tipoId: tipo, itemId: item, classificacaoId: classificacao }),
	            cache: false,
	            async: true,
	            type: 'POST',
	            dataType: 'json',
	            contentType: 'application/json; charset=utf-8',
	            error: function (XMLHttpRequest, textStatus, erroThrown) {
	                Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Infracao.container));
	            },
	            success: function (response, textStatus, XMLHttpRequest) {

	                if (response.EhValido) {
	                    $('.ddlSubitens', Infracao.container).ddlLoad(response.Subitens);
	                    $('.divCamposPerguntas', Infracao.container).html(response.Html);
	                    Mascara.load($('.divCamposPerguntas', Infracao.container));
	                }
	            }
	        });
	    }
	},

	onSelecionarComInfracao: function () {
	    $('.fsInfracao', Infracao.container).show();

	    $('.divDescricaoInfracao', Infracao.container).show();
	    $('.divClassificacao', Infracao.container).show();

	    Listar.atualizarEstiloTable(Infracao.container.find('.dataGridTable'));
	},

	onSelecionarSemInfracao: function () {
	    $('.fsInfracao', Infracao.container).hide();

	    $('.fsCaracterizacao', Infracao.container).show();
	    $('.fsDadosInfracao', Infracao.container).show();

	    $('.divDescricaoInfracao', Infracao.container).hide();
	    $('.divClassificacao', Infracao.container).hide();
	},

	onMarcarPenalidade: function(){
	    var marcado = $(this).closest('.cbPenalidade').attr('checked');
	    var container = $(this).closest('.block');
	    
	    if (marcado == true) {
	        $('.ddlTiposPenalidade', container).removeAttr('disabled');
	        $('.ddlTiposPenalidade', container).removeClass('disabled');
	    } else {
	        $('.ddlTiposPenalidade option:eq(0)', container).attr('selected', 'selected');
	        $('.ddlTiposPenalidade', container).attr('disabled', 'disable');
	        $('.ddlTiposPenalidade', container).addClass('disabled');

	        $('.txtDescricaoPenalidade', container).val('');
	    }
	},

	onSelecionarPenalidade: function () {
	    var container = $(this).closest('.block');

	    var penalidade = $('.ddlTiposPenalidade :selected', container).val();
	    var descricao = $('.hdnPenalidade' + penalidade, Infracao.container).val();

	    $('.txtDescricaoPenalidade', container).val(descricao);
	},
}

// 4ª aba - Multa
FiscalizacaoMulta = {
    settings: {
        urls: {
            salvar: '',
            obterSerie: '',
            enviarArquivo: '',
            obter: '',
        },
    },
    container: null,
    mensagens: null,
    TiposArquivo: [],

    callBackObterFiscalizacaoMultaVisualizar: function () {
        FiscalizacaoMulta.callBackObterFiscalizacaoMultaDefault();
    },

    callBackObterFiscalizacaoMulta: function () {
        FiscalizacaoMulta.callBackObterFiscalizacaoMultaDefault();
    },

    callBackObterFiscalizacaoMultaDefault: function () {
        Fiscalizacao.stepAtual = 4;
        Fiscalizacao.salvarTelaAtual = FiscalizacaoMulta.onSalvarFiscalizacaoMulta;
        Fiscalizacao.alternarAbas();

        $('.fsCamposMulta', FiscalizacaoMulta.container).hide();
        
        FiscalizacaoMulta.container.delegate('.rdoIsDigital', 'change', FiscalizacaoMulta.onSelecionarIsDigital);
        FiscalizacaoMulta.container.delegate('.rdoIsBloco', 'change', FiscalizacaoMulta.onSelecionarIsBloco);
        FiscalizacaoMulta.container.delegate('.btnAddArq', 'click', FiscalizacaoMulta.onEnviarArquivoClick);
        FiscalizacaoMulta.container.delegate('.btnLimparArq', 'click', FiscalizacaoMulta.onLimparArquivoClick);

        Mascara.load(FiscalizacaoMulta.container);

        if (parseInt($('.hdnMultaId', FiscalizacaoMulta.container).val()) > 0) {
            Fiscalizacao.salvarEdicao = false;
            Fiscalizacao.botoes({ btnEditar: true, spnCancelarCadastro: true });
            FiscalizacaoMulta.configurarBtnEditar();
        } else {
            Fiscalizacao.salvarEdicao = true;
            Fiscalizacao.botoes({ btnSalvar: true, spnCancelarCadastro: true });
        }

        if ($('.rdoIsDigital', FiscalizacaoMulta.container).attr('checked') == true
            || $('.rdoIsBloco', FiscalizacaoMulta.container).attr('checked') == true) {
            $('.fsCamposMulta', FiscalizacaoMulta.container).show();
        }

        MasterPage.botoes();
        MasterPage.carregando(false);
    },

    onSalvarFiscalizacaoMulta: function () {
        var container = FiscalizacaoMulta.container;

        //Criação do objeto (da classe Multa)
        var obj = {
            Id: Number($('.hdnMultaId', container).val()),
            FiscalizacaoId: Number($('.hdnFiscalizacaoId', Fiscalizacao.container).val())
        };

        //Preenchendo o objeto
        if ($('.rdoIsBloco', container).attr('checked')) {
            obj.IsDigital = false;
            obj.NumeroIUF = $('.txtNumeroIUF', container).val();
            obj.Arquivo = $.parseJSON($('.hdnArquivoJson', container).val());
            obj.DataLavratura = { DataTexto: $('.txtDataLavratura', container).val() };
        } else if ($('.rdoIsDigital', container).attr('checked')) {
            obj.IsDigital = true;
        }
        obj.SerieId = $('.ddlSeries :selected', container).val();
        obj.CodigoReceitaId = $('.ddlCodigosReceita :selected', container).val();
        obj.ValorMulta = $('.txtValorMulta', container).val();
        obj.Justificativa = $('.txtJustificativa', container).val();

        var arrayMensagem = [];

        arrayMensagem.push(FiscalizacaoMulta.settings.mensagens.Salvar);

        return Fiscalizacao.onSalvarStep(FiscalizacaoMulta.settings.urls.salvar, obj, arrayMensagem);
    },

    configurarBtnEditar: function () {
        $(".btnEditar", Fiscalizacao.container).unbind('click');
        $(".btnEditar", Fiscalizacao.container).click(FiscalizacaoMulta.onBtnEditar);
    },

    onBtnEditar: function () {
        Fiscalizacao.onObterStep(FiscalizacaoMulta.settings.urls.obter, Fiscalizacao.gerarObjetoWizard().params, function () {
            FiscalizacaoMulta.callBackObterFiscalizacaoMulta();
            Fiscalizacao.salvarEdicao = true;
            Fiscalizacao.botoes({ btnSalvar: true, spnCancelarEdicao: true });
            Fiscalizacao.configurarBtnCancelarStep(4);
            Fiscalizacao.gerenciarVisualizacao();
        })
    },

    onSelecionarIsDigital: function () {
        $('.fsCamposMulta', FiscalizacaoMulta.container).show();

        $('.txtNumeroIUF', FiscalizacaoMulta.container).attr('disabled', 'disabled');
        $('.txtNumeroIUF', FiscalizacaoMulta.container).addClass('disabled');
        $('.txtNumeroIUF', FiscalizacaoMulta.container).val('Gerado automaticamente');

        $('.ddlSeries option:eq(5)', FiscalizacaoMulta.container).attr('selected', 'selected');
        $('.ddlSeries option:eq(5)', FiscalizacaoMulta.container).show();
        $('.ddlSeries', FiscalizacaoMulta.container).attr('disabled', 'disabled');
        $('.ddlSeries', FiscalizacaoMulta.container).addClass('disabled');

        $('.txtDataLavratura', FiscalizacaoMulta.container).attr('disabled', 'disabled');
        $('.txtDataLavratura', FiscalizacaoMulta.container).addClass('disabled');
        $('.txtDataLavratura', FiscalizacaoMulta.container).val('Gerado automaticamente');
    },

    onSelecionarIsBloco: function () {
        $('.fsCamposMulta', FiscalizacaoMulta.container).show();

        $('.txtNumeroIUF', FiscalizacaoMulta.container).removeAttr('disabled');
        $('.txtNumeroIUF', FiscalizacaoMulta.container).removeClass('disabled');
        $('.txtNumeroIUF', FiscalizacaoMulta.container).val('');

        $('.ddlSeries option:eq(0)', FiscalizacaoMulta.container).attr('selected', 'selected');
        $('.ddlSeries option:eq(5)', FiscalizacaoMulta.container).hide();
        $('.ddlSeries', FiscalizacaoMulta.container).removeAttr('disabled', 'disabled');
        $('.ddlSeries', FiscalizacaoMulta.container).removeClass('disabled');

        $('.txtDataLavratura', FiscalizacaoMulta.container).removeAttr('disabled', 'disabled');
        $('.txtDataLavratura', FiscalizacaoMulta.container).removeClass('disabled');
        $('.txtDataLavratura', FiscalizacaoMulta.container).val('');
    },

    onEnviarArquivoClick: function () {
        var nomeArquivo = $('.inputFile', FiscalizacaoMulta.container).val();

        erroMsg = new Array();

        if (nomeArquivo == '') {
            erroMsg.push(FiscalizacaoMulta.settings.mensagens.ArquivoObrigatorio);
        } else {
            var tam = nomeArquivo.length - 4;
            if (!FiscalizacaoMulta.validarTipoArquivo(nomeArquivo.toLowerCase().substr(tam))) {
                erroMsg.push(FiscalizacaoMulta.settings.mensagens.ArquivoNaoEhPdf);
            }
        }

        if (erroMsg.length > 0) {
            Mensagem.gerar(Fiscalizacao.container, erroMsg);
            return;
        }

        MasterPage.carregando(true);
        var inputFile = $('.inputFile', FiscalizacaoMulta.container);
        FileUpload.upload(FiscalizacaoMulta.settings.urls.enviarArquivo, inputFile, FiscalizacaoMulta.callBackArqEnviado);
    },

    onLimparArquivoClick: function () {
        $('.hdnArquivoJson', FiscalizacaoMulta.container).val('');
        $('.inputFile', FiscalizacaoMulta.container).val('');

        $('.spanInputFile', FiscalizacaoMulta.container).removeClass('hide');
        $('.txtArquivoNome', FiscalizacaoMulta.container).addClass('hide');

        $('.btnAddArq', FiscalizacaoMulta.container).removeClass('hide');
        $('.btnLimparArq', FiscalizacaoMulta.container).addClass('hide');
    },

    validarTipoArquivo: function (tipo) {

        var tipoValido = false;
        $(FiscalizacaoMulta.TiposArquivo).each(function (i, tipoItem) {
            if (tipoItem == tipo) {
                tipoValido = true;
            }
        });

        return tipoValido;
    },

    callBackArqEnviado: function (controle, retorno, isHtml) {
        var ret = eval('(' + retorno + ')');
        if (ret.Arquivo != null) {
            $('.txtArquivoNome', FiscalizacaoMulta.container).text(ret.Arquivo.Nome);
            $('.hdnArquivoJson', FiscalizacaoMulta.container).val(JSON.stringify(ret.Arquivo));
            $('.txtArquivoNome', FiscalizacaoMulta.container).attr('href', '/Arquivo/BaixarTemporario?nomeTemporario=' + ret.Arquivo.TemporarioNome + '&contentType=' + ret.Arquivo.ContentType);

            $('.spanInputFile', FiscalizacaoMulta.container).addClass('hide');
            $('.txtArquivoNome', FiscalizacaoMulta.container).removeClass('hide');

            $('.btnAddArq', FiscalizacaoMulta.container).addClass('hide');
            $('.btnLimparArq', FiscalizacaoMulta.container).removeClass('hide');
        } else {
            FiscalizacaoMulta.onLimparArquivoClick();
            Mensagem.gerar(FiscalizacaoMulta.container, ret.Msg);
        }
        MasterPage.carregando(false);

        Mensagem.limpar(Fiscalizacao.container);
    },
}

// 5ª Aba - Interdição/Embargo
FiscalizacaoObjetoInfracao = {
	settings: {
		urls: {
			salvar: '',
			visualizar: '',
			enviarArquivo: ''
		},
		idsTela: null,
		mensagens: null

	},
	TiposArquivo: [],
	container: null,

	configurarBtnEditar: function () {
		$(".btnEditar", Fiscalizacao.container).unbind('click');
		$(".btnEditar", Fiscalizacao.container).click(FiscalizacaoObjetoInfracao.onBtnEditar);
	},

	onBtnEditar: function () {
		MasterPage.carregando(true);
		var param = { id: $('#hdnFiscalizacaoId', Fiscalizacao.container).val() };
		Fiscalizacao.onObterStep(Fiscalizacao.urls.objetoInfracao, param, FiscalizacaoObjetoInfracao.callBackObterObjetoInfracao);
	},

	callBackObterObjetoInfracaoVisualizar: function () {
		if (parseInt($('.hdnObjetoInfracaoId', Fiscalizacao.container).val()) > 0) {
			Fiscalizacao.salvarEdicao = false;
			Fiscalizacao.botoes({ btnEditar: true, spnCancelarCadastro: true });
			FiscalizacaoObjetoInfracao.configurarBtnEditar();
		} else {
			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarCadastro: true });
		}

		FiscalizacaoObjetoInfracao.callBackObterObjetoInfracaoDefault();
	},

	callBackObterObjetoInfracao: function () {
		Fiscalizacao.salvarEdicao = true;
		FiscalizacaoObjetoInfracao.callBackObterObjetoInfracaoDefault();
		Fiscalizacao.botoes({ btnSalvar: true, spnCancelarEdicao: true });
	},

	callBackObterObjetoInfracaoDefault: function () {
		FiscalizacaoObjetoInfracao.container = $('.FiscalizacaoObjetoInfracaoContainer', Fiscalizacao.container);
		Fiscalizacao.stepAtual = 5;
		Fiscalizacao.salvarTelaAtual = FiscalizacaoObjetoInfracao.onSalvarFiscalizacaoObjetoInfracao;
		Fiscalizacao.alternarAbas();

		$('.fsCamposInterdicaoEmbargo', FiscalizacaoObjetoInfracao.container).hide();

		FiscalizacaoObjetoInfracao.container.delegate('.rdoIsDigital', 'change', FiscalizacaoObjetoInfracao.onSelecionarIsDigital);
		FiscalizacaoObjetoInfracao.container.delegate('.rdoIsBloco', 'change', FiscalizacaoObjetoInfracao.onSelecionarIsBloco);
		FiscalizacaoObjetoInfracao.container.delegate('.rdbExisteAtvAreaDegrad', 'click', FiscalizacaoObjetoInfracao.gerenciarExisteAtvAreaDegradEspecificarTexto);
	    FiscalizacaoObjetoInfracao.container.delegate('.btnAddArq', 'click', FiscalizacaoObjetoInfracao.onEnviarArquivoClick);
	    FiscalizacaoObjetoInfracao.container.delegate('.btnLimparArq', 'click', FiscalizacaoObjetoInfracao.onLimparArquivoClick);

		FiscalizacaoObjetoInfracao.gerenciarExisteAtvAreaDegradEspecificarTexto();

		Mascara.load(FiscalizacaoObjetoInfracao.container);
		Fiscalizacao.configurarBtnCancelarStep(5);

		if ($('.rdoIsDigital', FiscalizacaoObjetoInfracao.container).attr('checked') == true
            || $('.rdoIsBloco', FiscalizacaoObjetoInfracao.container).attr('checked') == true) {
		    $('.fsCamposInterdicaoEmbargo', FiscalizacaoObjetoInfracao.container).show();
		}

		MasterPage.carregando(false);
		MasterPage.botoes();
		Fiscalizacao.gerenciarVisualizacao();
	},

	onSalvarFiscalizacaoObjetoInfracao: function () {
	    var container = FiscalizacaoObjetoInfracao.container;

	    var obj = {
	        Id: Number($('.hdnObjetoInfracaoId', container).val()) || 0,
	        FiscalizacaoId: Number($('.hdnFiscalizacaoId', Fiscalizacao.container).val()) || 0,
	        OpniaoAreaDanificada: $('.txtOpniaoAreaDanificada', container).val(),
	        DescricaoTermoEmbargo: $('.txtDescricaoTermoEmbargo', container).val(),
	        NumeroLacre: $('.txtNumeroLacre', container).val(),
	        ExisteAtvAreaDegrad: $('.rdbExisteAtvAreaDegrad:checked', container).val(),
	        ExisteAtvAreaDegradEspecificarTexto: $('.txtExisteAtvAreaDegradEspecificarTexto', container).val(),
	    }

	    //Preenchendo o objeto
	    if ($('.rdoIsBloco', container).attr('checked')) {
	        obj.IsDigital = false;
	        obj.NumeroIUF = $('.txtNumeroIUF', container).val();
	        obj.DataLavraturaTermo = { DataTexto: $('.txtDataLavratura', container).val() };
	        obj.Arquivo = $.parseJSON($('.hdnArquivoJson', container).val());
	    } else if ($('.rdoIsDigital', container).attr('checked')) {
	        obj.IsDigital = true;
	    }
	    obj.SerieId = $('.ddlSeries :selected', container).val();

	    if ($('.rdbInterditado', container).attr('checked')) {
	        obj.Interditado = true;
	    } else if ($('.rdbEmbargado', container).attr('checked')) {
	        obj.Interditado = false;
	    }

	    if (obj.ExisteAtvAreaDegrad == 0) {
	        obj.ExisteAtvAreaDegradEspecificarTexto = '';
	    }
	    
	    var arrayMensagem = new Array();

	    arrayMensagem.push(FiscalizacaoObjetoInfracao.settings.mensagens.Salvar)

	    return Fiscalizacao.onSalvarStep(FiscalizacaoObjetoInfracao.settings.urls.salvar, obj, arrayMensagem);
	},

	onSelecionarIsDigital: function () {
	    $('.fsCamposInterdicaoEmbargo', FiscalizacaoObjetoInfracao.container).show();

	    $('.txtNumeroIUF', FiscalizacaoObjetoInfracao.container).attr('disabled', 'disabled');
	    $('.txtNumeroIUF', FiscalizacaoObjetoInfracao.container).addClass('disabled');
	    $('.txtNumeroIUF', FiscalizacaoObjetoInfracao.container).val('Gerado automaticamente');

	    $('.ddlSeries option:eq(5)', FiscalizacaoObjetoInfracao.container).attr('selected', 'selected');
	    $('.ddlSeries option:eq(5)', FiscalizacaoObjetoInfracao.container).show();
	    $('.ddlSeries', FiscalizacaoObjetoInfracao.container).attr('disabled', 'disabled');
	    $('.ddlSeries', FiscalizacaoObjetoInfracao.container).addClass('disabled');

	    $('.txtDataLavratura', FiscalizacaoObjetoInfracao.container).attr('disabled', 'disabled');
	    $('.txtDataLavratura', FiscalizacaoObjetoInfracao.container).addClass('disabled');
	    $('.txtDataLavratura', FiscalizacaoObjetoInfracao.container).val('Gerado automaticamente');
	},

	onSelecionarIsBloco: function () {
	    $('.fsCamposInterdicaoEmbargo', FiscalizacaoObjetoInfracao.container).show();

	    $('.txtNumeroIUF', FiscalizacaoObjetoInfracao.container).removeAttr('disabled');
	    $('.txtNumeroIUF', FiscalizacaoObjetoInfracao.container).removeClass('disabled');
	    $('.txtNumeroIUF', FiscalizacaoObjetoInfracao.container).val('');

	    $('.ddlSeries option:eq(0)', FiscalizacaoObjetoInfracao.container).attr('selected', 'selected');
	    $('.ddlSeries option:eq(5)', FiscalizacaoObjetoInfracao.container).hide();
	    $('.ddlSeries', FiscalizacaoObjetoInfracao.container).removeAttr('disabled', 'disabled');
	    $('.ddlSeries', FiscalizacaoObjetoInfracao.container).removeClass('disabled');

	    $('.txtDataLavratura', FiscalizacaoObjetoInfracao.container).removeAttr('disabled', 'disabled');
	    $('.txtDataLavratura', FiscalizacaoObjetoInfracao.container).removeClass('disabled');
	    $('.txtDataLavratura', FiscalizacaoObjetoInfracao.container).val('');
	},

	gerenciarExisteAtvAreaDegradEspecificarTexto: function () {
	    var container = FiscalizacaoObjetoInfracao.container;
	    var rdb = $('.rdbExisteAtvAreaDegrad:checked', container).val();

	    $('.divExisteAtvAreaDegradEspecificarTexto', container).addClass('hide');
	    if (rdb == 1) {
	        $('.divExisteAtvAreaDegradEspecificarTexto', container).removeClass('hide');
	    }
	},

	mascaraLacre: function (evt) {
	    //Permite apenas números, vírgulas e espaços.
	    if ((evt.originalEvent.key >= 0
             && evt.originalEvent.key <= 9)
            || evt.originalEvent.key == ','
            || evt.originalEvent.key == ' ') {
	        return true;
	    } else {
	        return false;
	    }
	},

	onEnviarArquivoClick: function () {
	    var nomeArquivo = $('.inputFile', FiscalizacaoObjetoInfracao.container).val();

	    erroMsg = new Array();

	    if (nomeArquivo == '') {
	        erroMsg.push(FiscalizacaoObjetoInfracao.settings.mensagens.ArquivoObrigatorio);
	    } else {
	        var tam = nomeArquivo.length - 4;
	        if (!FiscalizacaoObjetoInfracao.validarTipoArquivo(nomeArquivo.toLowerCase().substr(tam))) {
	            erroMsg.push(FiscalizacaoObjetoInfracao.settings.mensagens.ArquivoNaoEhPdf);
	        }
	    }

	    if (erroMsg.length > 0) {
	        Mensagem.gerar(Fiscalizacao.container, erroMsg);
	        return;
	    }

	    MasterPage.carregando(true);
	    var inputFile = $('.inputFile', FiscalizacaoObjetoInfracao.container);
	    FileUpload.upload(FiscalizacaoObjetoInfracao.settings.urls.enviarArquivo, inputFile, FiscalizacaoObjetoInfracao.callBackArqEnviado);
	},

	onLimparArquivoClick: function () {
	    $('.hdnArquivoJson', FiscalizacaoObjetoInfracao.container).val('');
	    $('.inputFile', FiscalizacaoObjetoInfracao.container).val('');

	    $('.spanInputFile', FiscalizacaoObjetoInfracao.container).removeClass('hide');
	    $('.txtArquivoNome', FiscalizacaoObjetoInfracao.container).addClass('hide');

	    $('.btnAddArq', FiscalizacaoObjetoInfracao.container).removeClass('hide');
	    $('.btnLimparArq', FiscalizacaoObjetoInfracao.container).addClass('hide');
	},

	validarTipoArquivo: function (tipo) {

	    var tipoValido = false;
	    $(FiscalizacaoObjetoInfracao.TiposArquivo).each(function (i, tipoItem) {
	        if (tipoItem == tipo) {
	            tipoValido = true;
	        }
	    });

	    return tipoValido;
	},

	callBackArqEnviado: function (controle, retorno, isHtml) {
	    var ret = eval('(' + retorno + ')');
	    if (ret.Arquivo != null) {
	        $('.txtArquivoNome', FiscalizacaoObjetoInfracao.container).text(ret.Arquivo.Nome);
	        $('.hdnArquivoJson', FiscalizacaoObjetoInfracao.container).val(JSON.stringify(ret.Arquivo));
	        $('.txtArquivoNome', FiscalizacaoObjetoInfracao.container).attr('href', '/Arquivo/BaixarTemporario?nomeTemporario=' + ret.Arquivo.TemporarioNome + '&contentType=' + ret.Arquivo.ContentType);

	        $('.spanInputFile', FiscalizacaoObjetoInfracao.container).addClass('hide');
	        $('.txtArquivoNome', FiscalizacaoObjetoInfracao.container).removeClass('hide');

	        $('.btnAddArq', FiscalizacaoObjetoInfracao.container).addClass('hide');
	        $('.btnLimparArq', FiscalizacaoObjetoInfracao.container).removeClass('hide');
	    } else {
	        FiscalizacaoObjetoInfracao.onLimparArquivoClick();
	        Mensagem.gerar(Fiscalizacao.container, ret.Msg);
	    }
	    MasterPage.carregando(false);
	}
}

// 6ª Aba - Apreensão
FiscalizacaoMaterialApreendido = {
	settings: {
		urls: {
			salvar: '',
			obterSerie: '',
			associarDepositario: '',
			editarDepositario: '',
			enviarArquivo: '',
			obter: ''
		},
		mensagens: {
			TipoObrigatorio: '',
			EspecificacaoObrigatorio: '',
			MaterialJaAdicionado: ''
		}
	},
	TiposArquivo: [],
	container: null,

	pessoaModalInte: null,

	callBackObterFiscalizacaoMaterialApreendidoVisualizar: function () {
		FiscalizacaoMaterialApreendido.callBackObterFiscalizacaoMaterialApreendidoDefault();
	},

	callBackObterFiscalizacaoMaterialApreendido: function () {
		FiscalizacaoMaterialApreendido.callBackObterFiscalizacaoMaterialApreendidoDefault();
	},

	callBackObterFiscalizacaoMaterialApreendidoDefault: function () {
		Fiscalizacao.stepAtual = 6;
		Fiscalizacao.salvarTelaAtual = FiscalizacaoMaterialApreendido.onSalvarFiscalizacaoMaterialApreendido;
		Fiscalizacao.alternarAbas();
		
		FiscalizacaoMaterialApreendido.container.delegate('.rdoIsDigital', 'change', FiscalizacaoMaterialApreendido.onSelecionarIsDigital);
		FiscalizacaoMaterialApreendido.container.delegate('.rdoIsBloco', 'change', FiscalizacaoMaterialApreendido.onSelecionarIsBloco);
		FiscalizacaoMaterialApreendido.container.delegate('.btnAssociarDepositario', 'click', FiscalizacaoMaterialApreendido.onAssociarDepositario);
		FiscalizacaoMaterialApreendido.container.delegate('.ddlProdutosApreendidos', 'change', FiscalizacaoMaterialApreendido.onSelecionarProdutoApreendido);
		FiscalizacaoMaterialApreendido.container.delegate('.btnAdicionarProdutoApreendido', 'click', FiscalizacaoMaterialApreendido.adicionarProdutoApreendido);
		FiscalizacaoMaterialApreendido.container.delegate('.btnExcluirProdutoApreendido', 'click', FiscalizacaoMaterialApreendido.excluirProdutoApreendido);
		FiscalizacaoMaterialApreendido.container.delegate('.btnEditarDepositario', 'click', FiscalizacaoMaterialApreendido.onEditarDepositario);
	    FiscalizacaoMaterialApreendido.container.delegate('.btnAddArq', 'click', FiscalizacaoMaterialApreendido.onEnviarArquivoClick);
	    FiscalizacaoMaterialApreendido.container.delegate('.btnLimparArq', 'click', FiscalizacaoMaterialApreendido.onLimparArquivoClick);

	    Mascara.load(FiscalizacaoMaterialApreendido.container);

	    $('.fsCorpo', FiscalizacaoMaterialApreendido.container).hide();

		if (parseInt($('.hdnMaterialApreendidoId', FiscalizacaoMaterialApreendido.container).val()) > 0) {
			Fiscalizacao.salvarEdicao = false;
			Fiscalizacao.botoes({ btnEditar: true, spnCancelarCadastro: true });
			FiscalizacaoMaterialApreendido.configurarBtnEditar();
		} else {
			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarCadastro: true });
		}
		
		if ($('.rdoIsDigital', FiscalizacaoMaterialApreendido.container).attr('checked') == true
            || $('.rdoIsBloco', FiscalizacaoMaterialApreendido.container).attr('checked') == true) {
		    $('.fsCorpo', FiscalizacaoMaterialApreendido.container).show();
		}

		MasterPage.botoes();
		MasterPage.carregando(false);
	},

	configurarBtnEditar: function () {
	    $(".btnEditar", Fiscalizacao.container).unbind('click');
	    $(".btnEditar", Fiscalizacao.container).click(FiscalizacaoMaterialApreendido.onBtnEditar);
	},

	onBtnEditar: function () {
	    Fiscalizacao.onObterStep(FiscalizacaoMaterialApreendido.settings.urls.obter, Fiscalizacao.gerarObjetoWizard().params, function () {
	        FiscalizacaoMaterialApreendido.callBackObterFiscalizacaoMaterialApreendido();
	        Fiscalizacao.salvarEdicao = true;
	        Fiscalizacao.botoes({ btnSalvar: true, spnCancelarEdicao: true });
	        Fiscalizacao.configurarBtnCancelarStep(6);
	        Fiscalizacao.gerenciarVisualizacao();
	    });
	},

	onSelecionarIsDigital: function () {
	    $('.fsCorpo', FiscalizacaoMaterialApreendido.container).show();

	    $('.txtNumeroIUF', FiscalizacaoMaterialApreendido.container).attr('disabled', 'disabled');
	    $('.txtNumeroIUF', FiscalizacaoMaterialApreendido.container).addClass('disabled');
	    $('.txtNumeroIUF', FiscalizacaoMaterialApreendido.container).val('Gerado automaticamente');
	    
	    $('.ddlSeries option:eq(5)', FiscalizacaoMaterialApreendido.container).attr('selected', 'selected');
	    $('.ddlSeries option:eq(5)', FiscalizacaoMaterialApreendido.container).show();
	    $('.ddlSeries', FiscalizacaoMaterialApreendido.container).attr('disabled', 'disabled');
	    $('.ddlSeries', FiscalizacaoMaterialApreendido.container).addClass('disabled');

	    $('.txtDataLavratura', FiscalizacaoMaterialApreendido.container).attr('disabled', 'disabled');
	    $('.txtDataLavratura', FiscalizacaoMaterialApreendido.container).addClass('disabled');
	    $('.txtDataLavratura', FiscalizacaoMaterialApreendido.container).val('Gerado automaticamente');

	    $('.divPDF', FiscalizacaoMaterialApreendido.container).hide();
	},

	onSelecionarIsBloco: function () {
	    $('.fsCorpo', FiscalizacaoMaterialApreendido.container).show();

	    $('.txtNumeroIUF', FiscalizacaoMaterialApreendido.container).removeAttr('disabled');
	    $('.txtNumeroIUF', FiscalizacaoMaterialApreendido.container).removeClass('disabled');
	    $('.txtNumeroIUF', FiscalizacaoMaterialApreendido.container).val('');
	    
	    $('.ddlSeries option:eq(0)', FiscalizacaoMaterialApreendido.container).attr('selected', 'selected');
	    $('.ddlSeries option:eq(5)', FiscalizacaoMaterialApreendido.container).hide();
	    $('.ddlSeries', FiscalizacaoMaterialApreendido.container).removeAttr('disabled', 'disabled');
	    $('.ddlSeries', FiscalizacaoMaterialApreendido.container).removeClass('disabled');

	    $('.txtDataLavratura', FiscalizacaoMaterialApreendido.container).removeAttr('disabled', 'disabled');
	    $('.txtDataLavratura', FiscalizacaoMaterialApreendido.container).removeClass('disabled');
	    $('.txtDataLavratura', FiscalizacaoMaterialApreendido.container).val('');

	    $('.divPDF', FiscalizacaoMaterialApreendido.container).show();
	},

	onAssociarDepositario: function () {
	    FiscalizacaoMaterialApreendido.pessoaModalInte = new PessoaAssociar();

	    //Quando tipoCadastro = 1, o modal Pessoa exibirá apenas a busca por pessoa física.
        //Se o objeto não for passado para o modal (null), ele exibe a busca normal (CPF/CNPJ).
	    var dataPessoa = {
	        cpfCnpj: null,
            tipoPessoa: null,
            tipoCadastro: '1'
	    };

	    Modal.abrir(FiscalizacaoMaterialApreendido.settings.urls.associarDepositario, dataPessoa, function (container) {
	        FiscalizacaoMaterialApreendido.pessoaModalInte.load(container, {
	            tituloCriar: 'Cadastrar Depositario',
	            tituloEditar: 'Editar Depositario',
	            tituloVisualizar: 'Visualizar Depositario',
	            onAssociarCallback: FiscalizacaoMaterialApreendido.callBackEditarDepositario
	        });
	    });
	},

	callBackEditarDepositario: function (Pessoa) {
	    $('.spanVisualizarDepositario', FiscalizacaoMaterialApreendido.container).removeClass('hide');
	    $('.hdnDepositarioId', FiscalizacaoMaterialApreendido.container).val(Pessoa.Id);
	    $('.txtNome', FiscalizacaoMaterialApreendido.container).val(Pessoa.NomeRazaoSocial);
	    $('.txtCnpj', FiscalizacaoMaterialApreendido.container).val(Pessoa.CPFCNPJ);
	    return true;
	},

	onEditarDepositario: function () {
	    var id = $('.hdnDepositarioId', FiscalizacaoMaterialApreendido.container).val();
	    FiscalizacaoMaterialApreendido.pessoaModalInte = new PessoaAssociar();

	    Modal.abrir(FiscalizacaoMaterialApreendido.settings.urls.editarDepositario + "/" + id, null, function (container) {
	        FiscalizacaoMaterialApreendido.pessoaModalInte.load(container, {
	            tituloCriar: 'Cadastrar Depositario',
	            tituloEditar: 'Editar Depositario',
	            tituloVisualizar: 'Visualizar Depositario',
	            onAssociarCallback: FiscalizacaoMaterialApreendido.callBackEditarDepositario,
	            editarVisualizar: Fiscalizacao.salvarEdicao
	        });
	    });
	},

	onSelecionarProdutoApreendido: function () {
	    var produto = $('.ddlProdutosApreendidos :selected', FiscalizacaoMaterialApreendido.container).val();
	    var unidade = $('.hdnUnidade' + produto, FiscalizacaoMaterialApreendido.container).val();

	    $('.txtUnidade', FiscalizacaoMaterialApreendido.container).val(unidade);
	},

	adicionarProdutoApreendido: function () {
	    var mensagens = new Array();
	    Mensagem.limpar(FiscalizacaoMaterialApreendido.container);
	    var container = $('.fsProdutosApreendidos');

        //Monta o objeto
	    var item = {
	        Id: 0,
	        ProdutoId: $('.ddlProdutosApreendidos :selected', container).val(),
	        UnidadeTexto: $('.txtUnidade', container).val(),
	        Quantidade: $('.txtQuantidade', container).val(),
	        DestinoId: $('.ddlDestinos :selected', container).val(),
	        DestinoTexto: $('.ddlDestinos :selected', container).text()
	    };

	    var temp = $('.ddlProdutosApreendidos :selected', container).text().split('-');
	    item.ProdutoTexto = temp[0];

        //Verifica se todos os campos foram preenchidos, e se  objeto não está repetido
	    if (item.ProdutoId == 0) {
	        mensagens.push(jQuery.extend(true, {}, FiscalizacaoMaterialApreendido.settings.mensagens.ProdutoObrigatorio));
	    }
	    if (item.Quantidade.trim() == '') {
	        mensagens.push(jQuery.extend(true, {}, FiscalizacaoMaterialApreendido.settings.mensagens.QuantidadeObrigatoria));
	    }
	    if (item.DestinoId == 0) {
	        mensagens.push(jQuery.extend(true, {}, FiscalizacaoMaterialApreendido.settings.mensagens.DestinoObrigatorio));
	    }
	    $('.hdnItemJSon', container).each(function () {
	        var obj = String($(this).val());
	        if (obj != '') {
	            var itemAdd = (JSON.parse(obj));
	            if (item.ProdutoId == itemAdd.ProdutoId
                    && item.DestinoId == itemAdd.DestinoId) {
	                mensagens.push(jQuery.extend(true, {}, FiscalizacaoMaterialApreendido.settings.mensagens.ProdutoJaAdicionado));
	            }
	        }
	    });

	    //Se alguma obrigatoriedade não for atendida, retorna uma mensagem de erro
	    if (mensagens.length > 0) {
	        Mensagem.gerar(FiscalizacaoMaterialApreendido.container, mensagens);
	        return;
	    }

        //Monta a nova linha
	    var numItem = 0;
	    $('.hdnItemJSon', container).each(function(){
	        numItem++;
	    });
	    var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
	    linha.find('.hdnItemJSon').val(JSON.stringify(item));
	    linha.find('.item').text(numItem).attr('title', numItem);
	    linha.find('.produto').text(item.ProdutoTexto).attr('title', item.ProdutoTexto);
	    linha.find('.unidade').text(item.UnidadeTexto).attr('title', item.UnidadeTexto);
	    linha.find('.quantidade').text(item.Quantidade).attr('title', item.Quantidade);
	    linha.find('.destino').text(item.DestinoTexto).attr('title', item.DestinoTexto);

        //Adiciona a nova linha na tabela
	    $('.dataGridTable tbody:last', container).append(linha);
	    Listar.atualizarEstiloTable(container.find('.dataGridTable'));

        //Limpa os controles
	    $('.ddlProdutosApreendidos', container).ddlFirst();
	    $('.txtUnidade', container).val('');
	    $('.txtQuantidade', container).val('');
	    $('.ddlDestinos', container).ddlFirst();
	},

	excluirProdutoApreendido: function () {
        //remove a linha
	    $(this).closest('tr').remove();

	    var container = $('.fsProdutosApreendidos');

	    //atualiza o número na coluna item das outras linhas
	    var numItem = 0;
	    container.find('tr').each(function () {
	        if ($(this).find('.item').text() != '') {
	            $(this).find('.item').text(++numItem);
	        }
	    });

	    Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	},

	onSalvarFiscalizacaoMaterialApreendido: function () {
	    var container = FiscalizacaoMaterialApreendido.container;

        //Criação do objeto (da classe MaterialApreendido)
	    var obj = {
	        Id: Number($('.hdnMaterialApreendidoId', container).val()),
	        FiscalizacaoId: Number($('.hdnFiscalizacaoId', Fiscalizacao.container).val()),
	        ProdutosApreendidos: []
	    };

        //Preenchendo o objeto com os itens da sessão Apreensão
	    if ($('.rdoIsBloco', container).attr('checked')) {
	        obj.IsDigital = false;
	        obj.NumeroIUF = $('.txtNumeroIUF', container).val();
	        obj.Arquivo = $.parseJSON($('.hdnArquivoJson', container).val());
	        obj.DataLavratura = { DataTexto: $('.txtDataLavratura', container).val() };
	    } else if ($('.rdoIsDigital', container).attr('checked')) {
	        obj.IsDigital = true;
	    }
	    obj.SerieId = $('.ddlSeries :selected', container).val();
	    obj.Descricao = $('.txtDescricao', container).val();
	    obj.ValorProdutosExtenso = $('.txtValorBensApreendidosExtenso', container).val();
	    obj.ValorProdutosReais = $('.txtValorBensApreendidosReais', container).val();
	    obj.NumeroLacre = $('.txtNumeroLacre', container).val();

	    //Preenchendo o objeto com os itens da sessão Depositário
	    obj.Depositario = {
	        Id: $('.hdnDepositarioId', container).val(),
	        Logradouro: $('.txtLogradouro', container).val(),
	        Bairro: $('.txtBairro', container).val(),
	        Distrito: $('.txtDistrito', container).val(),
	        Estado: $('.ddlEstado :selected', container).val(),
	        Municipio: $('.ddlMunicipio :selected', container).val()
	    };

	    //Preenchendo o objeto com os itens da sessão Produtos Apreendidos / Destinação
	    $('.hdnItemJSon', container.find('.divProdutosApreendidos')).each(function () {
	        var objProdutoApreendido = String($(this).val());
	        if (objProdutoApreendido != '') {
	            obj.ProdutosApreendidos.push(JSON.parse(objProdutoApreendido));
	        }
	    });
	    obj.Opiniao = $('.txtOpiniao', container).val();

	    var arrayMensagem = [];

	    arrayMensagem.push(FiscalizacaoMaterialApreendido.settings.mensagens.Salvar);

	    return Fiscalizacao.onSalvarStep(FiscalizacaoMaterialApreendido.settings.urls.salvar, obj, arrayMensagem);
	},

	onEnviarArquivoClick: function () {
	    var nomeArquivo = $('.inputFile', FiscalizacaoMaterialApreendido.container).val();

	    erroMsg = new Array();

	    if (nomeArquivo == '') {
	        erroMsg.push(FiscalizacaoMaterialApreendido.settings.mensagens.ArquivoObrigatorio);
	    } else {
	        var tam = nomeArquivo.length - 4;
	        if (!FiscalizacaoMaterialApreendido.validarTipoArquivo(nomeArquivo.toLowerCase().substr(tam))) {
	            erroMsg.push(FiscalizacaoMaterialApreendido.settings.mensagens.ArquivoNaoEhPdf);
	        }
	    }

	    if (erroMsg.length > 0) {
	        Mensagem.gerar(Fiscalizacao.container, erroMsg);
	        return;
	    }

	    MasterPage.carregando(true);
	    var inputFile = $('.inputFile', FiscalizacaoMaterialApreendido.container);
	    FileUpload.upload(FiscalizacaoMaterialApreendido.settings.urls.enviarArquivo, inputFile, FiscalizacaoMaterialApreendido.callBackArqEnviado);
	},

	onLimparArquivoClick: function () {
	    $('.hdnArquivoJson', FiscalizacaoMaterialApreendido.container).val('');
	    $('.inputFile', FiscalizacaoMaterialApreendido.container).val('');

	    $('.spanInputFile', FiscalizacaoMaterialApreendido.container).removeClass('hide');
	    $('.txtArquivoNome', FiscalizacaoMaterialApreendido.container).addClass('hide');

	    $('.btnAddArq', FiscalizacaoMaterialApreendido.container).removeClass('hide');
	    $('.btnLimparArq', FiscalizacaoMaterialApreendido.container).addClass('hide');
	},

	validarTipoArquivo: function (tipo) {

	    var tipoValido = false;
	    $(FiscalizacaoMaterialApreendido.TiposArquivo).each(function (i, tipoItem) {
	        if (tipoItem == tipo) {
	            tipoValido = true;
	        }
	    });

	    return tipoValido;
	},

	callBackArqEnviado: function (controle, retorno, isHtml) {
	    var ret = eval('(' + retorno + ')');
	    if (ret.Arquivo != null) {
	        $('.txtArquivoNome', FiscalizacaoMaterialApreendido.container).text(ret.Arquivo.Nome);
	        $('.hdnArquivoJson', FiscalizacaoMaterialApreendido.container).val(JSON.stringify(ret.Arquivo));
	        $('.txtArquivoNome', FiscalizacaoMaterialApreendido.container).attr('href', '/Arquivo/BaixarTemporario?nomeTemporario=' + ret.Arquivo.TemporarioNome + '&contentType=' + ret.Arquivo.ContentType);

	        $('.spanInputFile', FiscalizacaoMaterialApreendido.container).addClass('hide');
	        $('.txtArquivoNome', FiscalizacaoMaterialApreendido.container).removeClass('hide');

	        $('.btnAddArq', FiscalizacaoMaterialApreendido.container).addClass('hide');
	        $('.btnLimparArq', FiscalizacaoMaterialApreendido.container).removeClass('hide');
	    } else {
	        FiscalizacaoMaterialApreendido.onLimparArquivoClick();
	        Mensagem.gerar(FiscalizacaoMaterialApreendido.container, ret.Msg);
	    }
	    MasterPage.carregando(false);

	    Mensagem.limpar(Fiscalizacao.container);
	},
}

// 7ª aba - Outras Penalidades
FiscalizacaoOutrasPenalidades = {
    settings: {
        urls: {
            salvar: '',
            obterSerie: '',
            enviarArquivo: '',
            obter: '',
        },
    },
    container: null,
    mensagens: null,
    TiposArquivo: [],

    callBackObterFiscalizacaoOutrasPenalidadesVisualizar: function () {
        FiscalizacaoOutrasPenalidades.callBackObterFiscalizacaoOutrasPenalidadesDefault();
    },

    callBackObterFiscalizacaoOutrasPenalidades: function () {
        FiscalizacaoOutrasPenalidades.callBackObterFiscalizacaoOutrasPenalidadesDefault();
    },

    callBackObterFiscalizacaoOutrasPenalidadesDefault: function () {
        Fiscalizacao.stepAtual = 7;
        Fiscalizacao.salvarTelaAtual = FiscalizacaoOutrasPenalidades.onSalvarFiscalizacaoOutrasPenalidades;
        Fiscalizacao.alternarAbas();

        $('.fsCamposOutrasPenalidades', FiscalizacaoOutrasPenalidades.container).hide();

        FiscalizacaoOutrasPenalidades.container.delegate('.rdoIsDigital', 'change', FiscalizacaoOutrasPenalidades.onSelecionarIsDigital);
        FiscalizacaoOutrasPenalidades.container.delegate('.rdoIsBloco', 'change', FiscalizacaoOutrasPenalidades.onSelecionarIsBloco);
        FiscalizacaoOutrasPenalidades.container.delegate('.btnAddArq', 'click', FiscalizacaoOutrasPenalidades.onEnviarArquivoClick);
        FiscalizacaoOutrasPenalidades.container.delegate('.btnLimparArq', 'click', FiscalizacaoOutrasPenalidades.onLimparArquivoClick);
        
        Mascara.load(FiscalizacaoOutrasPenalidades.container);

        if (parseInt($('.hdnOutrasPenalidadesId', FiscalizacaoOutrasPenalidades.container).val()) > 0) {
            Fiscalizacao.salvarEdicao = false;
            Fiscalizacao.botoes({ btnEditar: true, spnCancelarCadastro: true });
            FiscalizacaoOutrasPenalidades.configurarBtnEditar();
        } else {
            Fiscalizacao.salvarEdicao = true;
            Fiscalizacao.botoes({ btnSalvar: true, spnCancelarCadastro: true });
        }
        
        if ($('.rdoIsDigital', FiscalizacaoOutrasPenalidades.container).attr('checked') == true
            || $('.rdoIsBloco', FiscalizacaoOutrasPenalidades.container).attr('checked') == true) {
            $('.fsCamposOutrasPenalidades', FiscalizacaoOutrasPenalidades.container).show();
        }
        
        MasterPage.botoes();
        MasterPage.carregando(false);
    },

    onSalvarFiscalizacaoOutrasPenalidades: function () {
        var container = FiscalizacaoOutrasPenalidades.container;

        //Criação do objeto (da classe OutrasPenalidades)
        var obj = {
            Id: Number($('.hdnOutrasPenalidadesId', container).val()),
            FiscalizacaoId: Number($('.hdnFiscalizacaoId', Fiscalizacao.container).val())
        };

        //Preenchendo o objeto
        if ($('.rdoIsBloco', container).attr('checked')) {
            obj.IsDigital = false;
            obj.NumeroIUF = $('.txtNumeroIUF', container).val();
            obj.Arquivo = $.parseJSON($('.hdnArquivoJson', container).val());
            obj.DataLavratura = { DataTexto: $('.txtDataLavratura', container).val() };
        } else if ($('.rdoIsDigital', container).attr('checked')) {
            obj.IsDigital = true;
        }
        obj.SerieId = $('.ddlSeries :selected', container).val();
        obj.Descricao = $('.txtDescricao', container).val();

        var arrayMensagem = [];

        arrayMensagem.push(FiscalizacaoOutrasPenalidades.settings.mensagens.Salvar);

        return Fiscalizacao.onSalvarStep(FiscalizacaoOutrasPenalidades.settings.urls.salvar, obj, arrayMensagem);
    },

    configurarBtnEditar: function () {
        $(".btnEditar", Fiscalizacao.container).unbind('click');
        $(".btnEditar", Fiscalizacao.container).click(FiscalizacaoOutrasPenalidades.onBtnEditar);
    },

    onBtnEditar: function () {
        Fiscalizacao.onObterStep(FiscalizacaoOutrasPenalidades.settings.urls.obter, Fiscalizacao.gerarObjetoWizard().params, function () {
            FiscalizacaoOutrasPenalidades.callBackObterFiscalizacaoOutrasPenalidades();
            Fiscalizacao.salvarEdicao = true;
            Fiscalizacao.botoes({ btnSalvar: true, spnCancelarEdicao: true });
            Fiscalizacao.configurarBtnCancelarStep(7);
            Fiscalizacao.gerenciarVisualizacao();
        });
    },

    onSelecionarIsDigital: function () {
        $('.fsCamposOutrasPenalidades', FiscalizacaoOutrasPenalidades.container).show();

        $('.txtNumeroIUF', FiscalizacaoOutrasPenalidades.container).attr('disabled', 'disabled');
        $('.txtNumeroIUF', FiscalizacaoOutrasPenalidades.container).addClass('disabled');
        $('.txtNumeroIUF', FiscalizacaoOutrasPenalidades.container).val('Gerado automaticamente');

        $('.ddlSeries option:eq(5)', FiscalizacaoOutrasPenalidades.container).attr('selected', 'selected');
        $('.ddlSeries option:eq(5)', FiscalizacaoOutrasPenalidades.container).show();
        $('.ddlSeries', FiscalizacaoOutrasPenalidades.container).attr('disabled', 'disabled');
        $('.ddlSeries', FiscalizacaoOutrasPenalidades.container).addClass('disabled');

        $('.txtDataLavratura', FiscalizacaoOutrasPenalidades.container).attr('disabled', 'disabled');
        $('.txtDataLavratura', FiscalizacaoOutrasPenalidades.container).addClass('disabled');
        $('.txtDataLavratura', FiscalizacaoOutrasPenalidades.container).val('Gerado automaticamente');

        $('.divPDF', FiscalizacaoOutrasPenalidades.container).hide();
    },

    onSelecionarIsBloco: function () {
        $('.fsCamposOutrasPenalidades', FiscalizacaoOutrasPenalidades.container).show();

        $('.txtNumeroIUF', FiscalizacaoOutrasPenalidades.container).removeAttr('disabled');
        $('.txtNumeroIUF', FiscalizacaoOutrasPenalidades.container).removeClass('disabled');
        $('.txtNumeroIUF', FiscalizacaoOutrasPenalidades.container).val('');

        $('.ddlSeries option:eq(0)', FiscalizacaoOutrasPenalidades.container).attr('selected', 'selected');
        $('.ddlSeries option:eq(5)', FiscalizacaoOutrasPenalidades.container).hide();
        $('.ddlSeries', FiscalizacaoOutrasPenalidades.container).removeAttr('disabled', 'disabled');
        $('.ddlSeries', FiscalizacaoOutrasPenalidades.container).removeClass('disabled');

        $('.txtDataLavratura', FiscalizacaoOutrasPenalidades.container).removeAttr('disabled', 'disabled');
        $('.txtDataLavratura', FiscalizacaoOutrasPenalidades.container).removeClass('disabled');
        $('.txtDataLavratura', FiscalizacaoOutrasPenalidades.container).val('');

        $('.divPDF', FiscalizacaoOutrasPenalidades.container).show();
    },

    onEnviarArquivoClick: function () {
        var nomeArquivo = $('.inputFile', FiscalizacaoOutrasPenalidades.container).val();

        erroMsg = new Array();

        if (nomeArquivo == '') {
            erroMsg.push(FiscalizacaoOutrasPenalidades.settings.mensagens.ArquivoObrigatorio);
        } else {
            var tam = nomeArquivo.length - 4;
            if (!FiscalizacaoOutrasPenalidades.validarTipoArquivo(nomeArquivo.toLowerCase().substr(tam))) {
                erroMsg.push(FiscalizacaoOutrasPenalidades.settings.mensagens.ArquivoNaoEhPdf);
            }
        }

        if (erroMsg.length > 0) {
            Mensagem.gerar(Fiscalizacao.container, erroMsg);
            return;
        }

        MasterPage.carregando(true);
        var inputFile = $('.inputFile', FiscalizacaoOutrasPenalidades.container);
        FileUpload.upload(FiscalizacaoOutrasPenalidades.settings.urls.enviarArquivo, inputFile, FiscalizacaoOutrasPenalidades.callBackArqEnviado);
    },

    onLimparArquivoClick: function () {
        $('.hdnArquivoJson', FiscalizacaoOutrasPenalidades.container).val('');
        $('.inputFile', FiscalizacaoOutrasPenalidades.container).val('');

        $('.spanInputFile', FiscalizacaoOutrasPenalidades.container).removeClass('hide');
        $('.txtArquivoNome', FiscalizacaoOutrasPenalidades.container).addClass('hide');

        $('.btnAddArq', FiscalizacaoOutrasPenalidades.container).removeClass('hide');
        $('.btnLimparArq', FiscalizacaoOutrasPenalidades.container).addClass('hide');
    },

    validarTipoArquivo: function (tipo) {

        var tipoValido = false;
        $(FiscalizacaoOutrasPenalidades.TiposArquivo).each(function (i, tipoItem) {
            if (tipoItem == tipo) {
                tipoValido = true;
            }
        });

        return tipoValido;
    },

    callBackArqEnviado: function (controle, retorno, isHtml) {
        var ret = eval('(' + retorno + ')');
        if (ret.Arquivo != null) {
            $('.txtArquivoNome', FiscalizacaoOutrasPenalidades.container).text(ret.Arquivo.Nome);
            $('.hdnArquivoJson', FiscalizacaoOutrasPenalidades.container).val(JSON.stringify(ret.Arquivo));
            $('.txtArquivoNome', FiscalizacaoOutrasPenalidades.container).attr('href', '/Arquivo/BaixarTemporario?nomeTemporario=' + ret.Arquivo.TemporarioNome + '&contentType=' + ret.Arquivo.ContentType);

            $('.spanInputFile', FiscalizacaoOutrasPenalidades.container).addClass('hide');
            $('.txtArquivoNome', FiscalizacaoOutrasPenalidades.container).removeClass('hide');

            $('.btnAddArq', FiscalizacaoOutrasPenalidades.container).addClass('hide');
            $('.btnLimparArq', FiscalizacaoOutrasPenalidades.container).removeClass('hide');
        } else {
            FiscalizacaoOutrasPenalidades.onLimparArquivoClick();
            Mensagem.gerar(FiscalizacaoOutrasPenalidades.container, ret.Msg);
        }
        MasterPage.carregando(false);

        Mensagem.limpar(Fiscalizacao.container);
    },
}

// 8ª Aba - Considerações Finais
FiscalizacaoConsideracaoFinal = {
	settings: {
		urls: {
			salvar: '',
			obter: '',
			obterSetores: '',
			obterEnderecoSetor: '',
            obterCPF: '',
			enviarArquivo: '',
			obterAssinanteCargos: '',
			obterAssinanteFuncionarios: '',
            associarTestemunha: ''
		},
		modo: 1,
		mensagens: {}
	},
	TiposArquivo: [],
	isLoad: true,
	container: null,
	pessoaModalInte: null,

	load: function (container, options) {
		if (options) { $.extend(FiscalizacaoLocalInfracao.settings, options); }
		FiscalizacaoConsideracaoFinal.container = MasterPage.getContent(container);

		Fiscalizacao.stepAtual = 8;
		Fiscalizacao.salvarTelaAtual = FiscalizacaoConsideracaoFinal.onSalvar;
		Fiscalizacao.alternarAbas();

		Aux.setarFoco(FiscalizacaoConsideracaoFinal.container);

		FiscalizacaoConsideracaoFinal.isLoad = false;

		$('.rblOpinar', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onClickRadioOpinar);
		$('.rblFuncIDAF', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onClickRblFuncIDAF);
		$('.rblTermo', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onClickRadioTermos);
		$('.fsArquivos', FiscalizacaoConsideracaoFinal.container).arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });
		$('.fsArquivosIUF', FiscalizacaoConsideracaoFinal.container).arquivo({ extPermitidas: ['pdf'] });

		$('.ddlFuncIDAF', FiscalizacaoConsideracaoFinal.container).change(FiscalizacaoConsideracaoFinal.onChangeFuncIDAF);
		$('.ddlTestemunha', FiscalizacaoConsideracaoFinal.container).change(FiscalizacaoConsideracaoFinal.onChangeFunc);
		$('.ddlSetor', FiscalizacaoConsideracaoFinal.container).change(FiscalizacaoConsideracaoFinal.onChangeSetor);

		$('.btnAddArq', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onEnviarArquivoClick);
		$('.btnLimparArq', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onLimparArquivoClick);

		$('.ddlAssinanteSetores', FiscalizacaoConsideracaoFinal.container).change(FiscalizacaoConsideracaoFinal.onSelecionarSetor);
		$('.ddlAssinanteCargos', FiscalizacaoConsideracaoFinal.container).change(FiscalizacaoConsideracaoFinal.onSelecionarCargo);

		$('.btnAdicionarAssinante', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onAdicionarAssinante);
		$('.btnExcluirAssinante', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onExcluirAssinante);

		$('.btnAssociarTestemunha', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onAssociarTestemunha);
		$('.btnEditarTestemunha', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onEditarTestemunha);

		MasterPage.botoes(FiscalizacaoConsideracaoFinal.container);
		FiscalizacaoConsideracaoFinal.onClickRadioOpinar();
	},

	onAssociarTestemunha: function () {
	    FiscalizacaoConsideracaoFinal.pessoaModalInte = new PessoaAssociar();

	    //Quando tipoCadastro = 1, o modal Pessoa exibirá apenas a busca por pessoa física.
	    //Se o objeto não for passado para o modal (null), ele exibe a busca normal (CPF/CNPJ).
	    var dataPessoa = {
	        cpfCnpj: null,
	        tipoPessoa: null,
	        tipoCadastro: '1'
	    };

	    Modal.abrir(FiscalizacaoConsideracaoFinal.settings.urls.associarTestemunha, dataPessoa, function (container) {
	        FiscalizacaoConsideracaoFinal.pessoaModalInte.load(container, {
	            tituloCriar: 'Cadastrar Testemunha',
	            tituloEditar: 'Editar Testemunha',
	            tituloVisualizar: 'Visualizar Testemunha',
	            onAssociarCallback: FiscalizacaoConsideracaoFinal.callBackEditarTestemunha,
                isFiscalizacao: true
	        });
	    });
	},

	callBackEditarTestemunha: function (Pessoa) {
	    $('.spanVisualizarTestemunha', FiscalizacaoMaterialApreendido.container).removeClass('hide');
	    $('.hdnTestemunhaId', FiscalizacaoMaterialApreendido.container).val(Pessoa.Id);
	    $('.txtTestemunhaNome', FiscalizacaoMaterialApreendido.container).val(Pessoa.NomeRazaoSocial);
	    $('.txtTestemunhaCPF', FiscalizacaoMaterialApreendido.container).val(Pessoa.CPFCNPJ);
	    return true;
	},

	onEditarTestemunha: function () {
	    var id = $('.hdnTestemunhaId', FiscalizacaoConsideracaoFinal.container).val();
	    FiscalizacaoConsideracaoFinal.pessoaModalInte = new PessoaAssociar();

	    Modal.abrir(FiscalizacaoConsideracaoFinal.settings.urls.editarTestemunha + "/" + id, null, function (container) {
	        FiscalizacaoConsideracaoFinal.pessoaModalInte.load(container, {
	            tituloCriar: 'Cadastrar Testemunha',
	            tituloEditar: 'Editar Testemunha',
	            tituloVisualizar: 'Visualizar Testemunha',
	            onAssociarCallback: FiscalizacaoConsideracaoFinal.callBackEditarTestemunha,
	            editarVisualizar: Fiscalizacao.salvarEdicao
	        });
	    });
	},

	configurarBtnEditar: function () {

		$(".btnEditar", Fiscalizacao.container).unbind('click');
		$(".btnEditar", Fiscalizacao.container).click(FiscalizacaoConsideracaoFinal.onBtnEditar);
	},

	gerarObjetoConsideracaoFinal: function () {

		var consideracaoFinal = {
			Id: $('.hdnConsideracaoFinalId', FiscalizacaoConsideracaoFinal.container).val(),
			FiscalizacaoId: $('.hdnFiscalizacaoId', Fiscalizacao.container).val(),
			Descrever: $('.txtDescrever', FiscalizacaoConsideracaoFinal.container).val().trim(),
			HaReparacao: parseInt($('.rblOpinar:checked', FiscalizacaoConsideracaoFinal.container).val()),
			Reparacao: '',
			HaTermoCompromisso: parseInt($('.rblTermo:checked', FiscalizacaoConsideracaoFinal.container).val()),
			TermoCompromissoJustificar: $('.txtTermoJustificar', FiscalizacaoConsideracaoFinal.container).val().trim(),
			Arquivo: null,
			Assinantes: [],
			Testemunhas: [],
			Anexos: $('.fsArquivos', FiscalizacaoConsideracaoFinal.container).arquivo('obterObjeto'),
			AnexosIUF: $('.fsArquivosIUF', FiscalizacaoConsideracaoFinal.container).arquivo('obterObjeto')
		};

		if (consideracaoFinal.HaReparacao == 1) {
			consideracaoFinal.Reparacao = $('.txtOpinarReparacao', FiscalizacaoConsideracaoFinal.container).val().trim();
		} else if (consideracaoFinal.HaReparacao == 0) {
			consideracaoFinal.Reparacao = $('.txtOpinarReparacao', FiscalizacaoConsideracaoFinal.container).val().trim();
			consideracaoFinal.HaTermoCompromisso = null;
			consideracaoFinal.TermoCompromissoJustificar = '';
		}

		if (consideracaoFinal.HaTermoCompromisso == 1) {
			consideracaoFinal.HaTermoCompromisso = true;
			consideracaoFinal.TermoCompromissoJustificar = '';
			consideracaoFinal.Arquivo = $.parseJSON($('.hdnArquivoJson', FiscalizacaoConsideracaoFinal.container).val());
		} else if (consideracaoFinal.HaTermoCompromisso == 0) {
			consideracaoFinal.HaTermoCompromisso = false;
			consideracaoFinal.TermoCompromissoJustificar = $('.txtTermoJustificar', FiscalizacaoConsideracaoFinal.container).val().trim();
			consideracaoFinal.Arquivo = null;
		}

		$('.fdsTestemunhas', FiscalizacaoConsideracaoFinal.container).each(function (i, item) {
			var testemunha = {
				TestemunhaIDAF: parseInt($('.ddlFuncIDAF', item).val()),
				TestemunhaId: parseInt($('.ddlTestemunha', item).val()),
				TestemunhaNome: $('.txtTestemunhaNome', item).val().trim(),
				TestemunhaCPF: $('.txtTestemunhaCPF', item).val().trim(),
				Colocacao: $('.hdnColocacao', item).val()
			};

			if (testemunha.TestemunhaIDAF == 1) {
				testemunha.TestemunhaIDAF = true;
				testemunha.TestemunhaNome = '';
			} else if (testemunha.TestemunhaIDAF == 2) {
				testemunha.TestemunhaIDAF = false;
				testemunha.TestemunhaId = null;
				testemunha.TestemunhaSetorId = null;
			} else {
				testemunha.TestemunhaId = null;
				testemunha.TestemunhaSetorId = null;
				testemunha.TestemunhaIDAF = null;
				testemunha.TestemunhaId = null;
				testemunha.TestemunhaNome = null;
				testemunha.TestemunhaEndereco = null;
			}

			testemunha.TestemunhaId = testemunha.TestemunhaId == 0 ? null : testemunha.TestemunhaId;

			consideracaoFinal.Testemunhas.push(testemunha);
		});

		var assinantesContainer = FiscalizacaoConsideracaoFinal.container.find('.fdsAssinante');
		$('.hdnItemJSon', assinantesContainer).each(function () {
			var objAssinante = String($(this).val());
			if (objAssinante != '') {
				consideracaoFinal.Assinantes.push(JSON.parse(objAssinante));
			}
		});

		return consideracaoFinal;
	},

	callBackObterConsideracaoFinalVisualizar: function () {
		var context = $('.divConsideracaoFinal', Fiscalizacao.container);
		FiscalizacaoConsideracaoFinal.load(context);
		Mascara.load(context);
		if (parseInt($('.hdnConsideracaoFinalId', FiscalizacaoConsideracaoFinal.container).val()) > 0) {
			Fiscalizacao.salvarEdicao = false;
			Fiscalizacao.botoes({ btnEditar: true, spnCancelarCadastro: true });
			FiscalizacaoConsideracaoFinal.configurarBtnEditar();
		} else {
			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarCadastro: true });
		}
		MasterPage.carregando(false);
	},

	onBtnEditar: function () {

		Fiscalizacao.onObterStep(FiscalizacaoConsideracaoFinal.settings.urls.obter, Fiscalizacao.gerarObjetoWizard().params, function () {
			var context = $('.divConsideracaoFinal', Fiscalizacao.container);
			FiscalizacaoConsideracaoFinal.load(context);
			Mascara.load(context);
			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarEdicao: true });
			Fiscalizacao.configurarBtnCancelarStep(8);
			MasterPage.carregando(false);
			Fiscalizacao.gerenciarVisualizacao();
		});
	},

	onClickRadioOpinar: function () {
	    var haReparacao = parseInt($('.rblOpinar:checked', FiscalizacaoConsideracaoFinal.container).val());

		$('.divTermo', FiscalizacaoConsideracaoFinal.container).addClass('hide');
		if (haReparacao == 1) {
			$('.divTermo', FiscalizacaoConsideracaoFinal.container).removeClass('hide');
		}

		$('.divReparacaoSim', FiscalizacaoConsideracaoFinal.container).addClass('hide');
		if (haReparacao == 1 || haReparacao == 0) {
		    $('.divReparacaoSim', FiscalizacaoConsideracaoFinal.container).removeClass('hide');
		}
	},

	onClickRadioTermos: function () {
		$('.divTermoJustificar, .divArquivo', FiscalizacaoConsideracaoFinal.container).addClass('hide');

		if ($(this).val().toString() == "0") {
			$('.divTermoJustificar', FiscalizacaoConsideracaoFinal.container).removeClass('hide');
			$('.inputFile', FiscalizacaoConsideracaoFinal.container).val();
		} else {
			$('.txtTermoJustificar', FiscalizacaoConsideracaoFinal.container).val('');
			$('.divArquivo', FiscalizacaoConsideracaoFinal.container).removeClass('hide');
		}
	},

	onChangeFuncIDAF: function () {
		var ddlFuncIDAF = $(this);
		var context = ddlFuncIDAF.closest('fieldset');

		$('.divFuncionario, .divDadosTestemunha', context).addClass('hide');
		$('.ddlTestemunha', context).val(0);
		$('.txtTestemunhaNome, .txtTestemunhaCPF', context).val('');

		if (ddlFuncIDAF.val().toString() == "1") {
		    $('input[type="text"]', context).val('').removeAttr('disabled').removeClass('disabled');
			$('.divFuncionario', context).removeClass('hide');
			$('.txtTestemunhaCPF', FiscalizacaoLocalInfracao.container).attr('disabled', 'disabled').addClass('disabled');
		} else if (ddlFuncIDAF.val().toString() == "2") {
		    $('.divDadosTestemunha', context).removeClass('hide');
		    $('.txtTestemunhaCPF', FiscalizacaoLocalInfracao.container).attr('disabled', 'disabled').addClass('disabled');
		    $('.txtTestemunhaNome', FiscalizacaoLocalInfracao.container).attr('disabled', 'disabled').addClass('disabled');
		}
	},

	onChangeFunc: function () {
		var value = parseInt($(this).val());
		var context = $(this).closest('fieldset');
		var txtTestemunhaCPF = $('.txtTestemunhaCPF', context);

		txtTestemunhaCPF.val('');

		if (!value) {
			return;
		}

		MasterPage.carregando(true);

		$.ajax({
		    url: FiscalizacaoConsideracaoFinal.settings.urls.obterCPF,
			data: $.toJSON({ funcionarioId: value }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					Mensagem.limpar(Fiscalizacao.container);
					if (response.CPF) {
					    txtTestemunhaCPF.val(response.CPF).attr('disabled', 'disabled');
					    txtTestemunhaCPF.val(response.CPF).addClass('disabled');
					}
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Fiscalizacao.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},

	onChangeSetor: function () {
		var value = parseInt($(this).val());
		var context = $(this).closest('fieldset');
		var txtTestemunhaEndereco = $('.txtTestemunhaEndereco', context);

		txtTestemunhaEndereco.val('');

		if (!value) {
			return;
		}

		MasterPage.carregando(true);

		$.ajax({ url: FiscalizacaoConsideracaoFinal.settings.urls.obterEnderecoSetor,
			data: $.toJSON({ setorId: value }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					Mensagem.limpar(Fiscalizacao.container);
					txtTestemunhaEndereco.val(response.Endereco).attr('disabled', 'disabled').addClass('disabled');
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Fiscalizacao.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},

	onClickRblFuncIDAF: function () {
		var radio = $(this);
		var context = radio.closest('fieldset');

		$('.divFuncionario, .divDadosTestemunha', context).addClass('hide');
		$('input[type="text"]', context).val('');
		$('select', context).val(0);

		if (radio.val().toString() == "1") {
			$('.divFuncionario', context).removeClass('hide');
		} else {
			$('.divDadosTestemunha', context).removeClass('hide');
		}
		$('.divDadosEndereco', context).removeClass('hide');
	},

	onSalvar: function () {
		var flag = false;

		MasterPage.carregando(true);

		$.ajax({ url: FiscalizacaoConsideracaoFinal.settings.urls.salvar,
			data: $.toJSON({ consideracaoFinal: FiscalizacaoConsideracaoFinal.gerarObjetoConsideracaoFinal() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				flag = response.EhValido;

				if (response.EhValido) {
					Mensagem.limpar(Fiscalizacao.container);
					$('.hdnConsideracaoFinalId', FiscalizacaoLocalInfracao.container).val(response.id);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Fiscalizacao.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);

		return flag;
	},

	onEnviarArquivoClick: function () {
		var nomeArquivo = $('#fileTermo', FiscalizacaoConsideracaoFinal.container).val();

		erroMsg = new Array();

		if (nomeArquivo == '') {
			erroMsg.push(FiscalizacaoConsideracaoFinal.settings.mensagens.ArquivoObrigatorio);
		} else {
			var tam = nomeArquivo.length - 4;
			if (!FiscalizacaoConsideracaoFinal.validarTipoArquivo(nomeArquivo.toLowerCase().substr(tam))) {
				erroMsg.push(FiscalizacaoConsideracaoFinal.settings.mensagens.ArquivoNaoEhPdf);
			}
		}

		if (erroMsg.length > 0) {
			Mensagem.gerar(Fiscalizacao.container, erroMsg);
			return;
		}

		MasterPage.carregando(true);
		var inputFile = $('#fileTermo', FiscalizacaoConsideracaoFinal.container);
		FileUpload.upload(FiscalizacaoConsideracaoFinal.settings.urls.enviarArquivo, inputFile, FiscalizacaoConsideracaoFinal.callBackArqEnviado);
	},

	onLimparArquivoClick: function () {
		$('.hdnArquivoJson', FiscalizacaoConsideracaoFinal.container).val('');
		$('.inputFile', FiscalizacaoConsideracaoFinal.container).val('');

		$('.spanInputFile', FiscalizacaoConsideracaoFinal.container).removeClass('hide');
		$('.txtArquivoNome', FiscalizacaoConsideracaoFinal.container).addClass('hide');

		$('.btnAddArq', FiscalizacaoConsideracaoFinal.container).removeClass('hide');
		$('.btnLimparArq', FiscalizacaoConsideracaoFinal.container).addClass('hide');
	},

	validarTipoArquivo: function (tipo) {

		var tipoValido = false;
		$(FiscalizacaoConsideracaoFinal.TiposArquivo).each(function (i, tipoItem) {
			if (tipoItem == tipo) {
				tipoValido = true;
			}
		});

		return tipoValido;
	},

	callBackArqEnviado: function (controle, retorno, isHtml) {
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoNome', FiscalizacaoConsideracaoFinal.container).text(ret.Arquivo.Nome);
			$('.hdnArquivoJson', FiscalizacaoConsideracaoFinal.container).val(JSON.stringify(ret.Arquivo));
			$('.txtArquivoNome', FiscalizacaoConsideracaoFinal.container).attr('href', '/Arquivo/BaixarTemporario?nomeTemporario=' + ret.Arquivo.TemporarioNome + '&contentType=' + ret.Arquivo.ContentType);

			$('.spanInputFile', FiscalizacaoConsideracaoFinal.container).addClass('hide');
			$('.txtArquivoNome', FiscalizacaoConsideracaoFinal.container).removeClass('hide');

			$('.btnAddArq', FiscalizacaoConsideracaoFinal.container).addClass('hide');
			$('.btnLimparArq', FiscalizacaoConsideracaoFinal.container).removeClass('hide');
		} else {
			FiscalizacaoConsideracaoFinal.onLimparArquivoClick();
			Mensagem.gerar(Fiscalizacao.container, ret.Msg);
		}

		Listar.atualizarEstiloTable(FiscalizacaoConsideracaoFinal.container.find('.dataGrid'));

		MasterPage.carregando(false);
	},

	//Selecionar Assinante 
	onSelecionarSetor: function () {

		var ddlA = $(".ddlAssinanteSetores", FiscalizacaoConsideracaoFinal.container);
		var ddlB = $('.ddlAssinanteCargos', FiscalizacaoConsideracaoFinal.container);
		var ddlC = $('.ddlAssinanteFuncionarios', FiscalizacaoConsideracaoFinal.container);

		var setorId = $('.ddlAssinanteSetores', FiscalizacaoConsideracaoFinal.container).val();

		ddlA.ddlCascate(ddlB, { url: FiscalizacaoConsideracaoFinal.settings.urls.obterAssinanteCargos, data: { setorId: setorId }, callBack: function () {			
			var cargoId = $('.ddlAssinanteCargos', FiscalizacaoConsideracaoFinal.container).val();
			ddlB.ddlCascate(ddlC, { url: FiscalizacaoConsideracaoFinal.settings.urls.obterAssinanteFuncionarios, data: { setorId: setorId, cargoId: cargoId} });
		} });
	},

	onSelecionarCargo: function () {

		var ddlA = $(".ddlAssinanteCargos", FiscalizacaoConsideracaoFinal.container);
		var ddlB = $('.ddlAssinanteFuncionarios', FiscalizacaoConsideracaoFinal.container);

		var setorId = $('.ddlAssinanteSetores', FiscalizacaoConsideracaoFinal.container).val();
		var cargoId = $('.ddlAssinanteCargos', FiscalizacaoConsideracaoFinal.container).val();

		ddlA.ddlCascate(ddlB, { url: FiscalizacaoConsideracaoFinal.settings.urls.obterAssinanteFuncionarios, data: { setorId: setorId, cargoId: cargoId} });
	},

	onAdicionarAssinante: function () {

		var mensagens = new Array();
		Mensagem.limpar(FiscalizacaoConsideracaoFinal.container);
		var container = $('.fdsAssinante', FiscalizacaoConsideracaoFinal.container);

		var item = {
			SetorId: $('.ddlAssinanteSetores :selected', container).val(),
			FuncionarioNome: $('.ddlAssinanteFuncionarios :selected', container).text(),
			FuncionarioId: $('.ddlAssinanteFuncionarios :selected', container).val(),
			FuncionarioCargoNome: $('.ddlAssinanteCargos :selected', container).text(),
			FuncionarioCargoId: $('.ddlAssinanteCargos :selected', container).val()
		};

		if (jQuery.trim(item.SetorId) == '0') {
			mensagens.push(jQuery.extend(true, {}, FiscalizacaoConsideracaoFinal.settings.mensagens.AssinanteSetorObrigatorio));
		}

		if (jQuery.trim(item.FuncionarioCargoId) == '0') {
			mensagens.push(jQuery.extend(true, {}, FiscalizacaoConsideracaoFinal.settings.mensagens.AssinanteCargoObrigatorio));
		}

		if (jQuery.trim(item.FuncionarioId) == '0') {
			mensagens.push(jQuery.extend(true, {}, FiscalizacaoConsideracaoFinal.settings.mensagens.AssinanteFuncionarioObrigatorio));
		}

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var itemAdd = (JSON.parse(obj));
				if (item.FuncionarioId == itemAdd.FuncionarioId && item.FuncionarioCargoId == itemAdd.FuncionarioCargoId) {
					mensagens.push(jQuery.extend(true, {}, FiscalizacaoConsideracaoFinal.settings.mensagens.AssinanteJaAdicionado));
				}
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(FiscalizacaoConsideracaoFinal.container, mensagens);
			return;
		}

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(item));
		linha.find('.Funcionario').html(item.FuncionarioNome).attr('title', item.FuncionarioNome);
		linha.find('.Cargo').html(item.FuncionarioCargoNome).attr('title', item.FuncionarioCargoNome);

		$('.btnExcluirAssinante', linha).click(FiscalizacaoConsideracaoFinal.onExcluirAssinante);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.ddlAssinanteSetores', container).ddlFirst();
		FiscalizacaoConsideracaoFinal.onSelecionarSetor();

	},

	onExcluirAssinante: function () {
		var container = $('.fdsAssinante');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	}
}

// 9ª Aba - Concluir Cadastro
FiscalizacaoFinalizar = {
	settings: {
		urls: {
			finalizar: '',
			download: '',
			pdfAuto: '',
			pdfIUF: '',
			pdfLaudo: ''
		}
	},
	container: null,
	isLoad: true,
	callBackObterFiscalizacaoFinalizar: function () {
		FiscalizacaoFinalizar.container = $('.divFinalizar', Fiscalizacao.container); 
		Fiscalizacao.stepAtual = 9;
		Fiscalizacao.alternarAbas();
		Fiscalizacao.salvarTelaAtual = FiscalizacaoFinalizar.onFinalizar;

		Fiscalizacao.botoes({ btnFinalizar: true, spnCancelarCadastro: true });

		Fiscalizacao.configurarBtnCancelarStep(9);

		if (FiscalizacaoFinalizar.isLoad) {
			FiscalizacaoFinalizar.isLoad = false;
			$('.btnFinalizar', Fiscalizacao.container).click(FiscalizacaoFinalizar.onFinalizar);
		}

		$('input', $('.fdsInfracao', Fiscalizacao.container)).attr('disabled','disabled').addClass('disabled');
		$('.btnAnexo', FiscalizacaoFinalizar.container).click(FiscalizacaoFinalizar.onClickDownload);
		$('.btnAnexoCroqui', FiscalizacaoFinalizar.container).click(FiscalizacaoFinalizar.onClickDownload);
		$('.btnPdfAuto', FiscalizacaoFinalizar.container).click(FiscalizacaoFinalizar.onGerarPdfAuto);
		$('.btnPdfLaudo', FiscalizacaoFinalizar.container).click(FiscalizacaoFinalizar.onGerarPdfLaudo);
		$('.btnPdfIUF', FiscalizacaoFinalizar.container).click(FiscalizacaoFinalizar.onGerarPdfIUF);

		MasterPage.carregando(false);
	},
	onClickDownload: function () {
		MasterPage.redireciona(FiscalizacaoFinalizar.settings.urls.download + "/" + $('.hdnArquivoId', $(this).closest('td')).val());
	},
	onFinalizar: function () {

		MasterPage.carregando(true);

		$.ajax({
			url: FiscalizacaoFinalizar.settings.urls.finalizar,
			type: "POST",
			data: $.toJSON({ id: $('#hdnFiscalizacaoId', Fiscalizacao.container).val() }),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Fiscalizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(Fiscalizacao.container, response.Msg);
					return;
				}
				if (response.Redirect) {
					MasterPage.redireciona(response.Redirect);
				}
			}
		});

		MasterPage.carregando(false);
	},
	onGerarPdfAuto: function () {
		MasterPage.redireciona(FiscalizacaoFinalizar.settings.urls.pdfAuto + "/" + $('.hdnFiscalizacaoId', Fiscalizacao.container).val());
	},
	onGerarPdfIUF: function () {
	    MasterPage.redireciona(FiscalizacaoFinalizar.settings.urls.pdfIUF + "/" + $('.hdnFiscalizacaoId', Fiscalizacao.container).val());
	},
	onGerarPdfLaudo: function () {
		MasterPage.redireciona(FiscalizacaoFinalizar.settings.urls.pdfLaudo + "/" + $('.hdnFiscalizacaoId', Fiscalizacao.container).val());
	}
}
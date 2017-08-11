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
		complementacaoDados: '',
		enquadramento: '',
		objetoInfracao: '',
		diagnostico: '',
		consideracaoFinalVisualizar: '',
		finalizar: '',
		infracao: '',
		materialApreendido: '',
		documentosGerados: ''
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

	},

	gerenciarWizardAbas: function () {

		if (Fiscalizacao.stepAtual === +$(this).data('step')) {
			return;
		}

		if (Fiscalizacao.modo == 1) {
			if (Fiscalizacao.stepAtual != 9 && Fiscalizacao.salvarEdicao) {
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
				Fiscalizacao.onObterStep(FiscalizacaoComplementacaoDados.settings.urls.visualizar, objeto.params, function () {
					FiscalizacaoComplementacaoDados.callBackObterComplementacaoDadosVisualizar();
					Fiscalizacao.gerenciarVisualizacao('.hdnComplementacaoDadosId');
				});
				break;

			case 4:
				Fiscalizacao.onObterStep(FiscalizacaoEnquadramento.settings.urls.visualizar, objeto.params, function () {
					FiscalizacaoEnquadramento.callBackObterEnquadramentoVisualizar();
					Fiscalizacao.gerenciarVisualizacao('.hdnEnquadramentoId');
				});
				break;

			case 5:
				Fiscalizacao.onObterStep(Fiscalizacao.urls.infracao, objeto.params, function () {
					Infracao.callBackObterInfracaoVisualizar();
					Fiscalizacao.gerenciarVisualizacao('.hdnInfracaoId');
				});
				break;

			case 6:
				Fiscalizacao.onObterStep(FiscalizacaoObjetoInfracao.settings.urls.visualizar, objeto.params, function () {
					FiscalizacaoObjetoInfracao.callBackObterObjetoInfracaoVisualizar();
					Fiscalizacao.gerenciarVisualizacao('.hdnObjetoInfracaoId');
				});
				break;

			case 7:
				Fiscalizacao.onObterStep(Fiscalizacao.urls.materialApreendido, objeto.params, function () {
					FiscalizacaoMaterialApreendido.callBackObterFiscalizacaoMaterialApreendidoVisualizar();
					Fiscalizacao.gerenciarVisualizacao('.hdnMaterialApreendidoId');
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

		if (Fiscalizacao.stepAtual != 9) {
			if (!Fiscalizacao.salvarTelaAtual()) {
				return;
			}
		} else {
			Mensagem.limpar(Fiscalizacao.containerMensagem);
		}

		MasterPage.carregando(true);
		$('.divFinalizar, .divPdf', Fiscalizacao.container).addClass('hide');

		var objeto = Fiscalizacao.gerarObjetoWizard();
		objeto.step = Fiscalizacao.obterStep(this);
		Fiscalizacao.switchGerenciarWizard(objeto);
	},

	obterStep: function (container) {

		if ($(container).data('step') !== null && $(container).data('step') !== NaN && $(container).data('step') !== undefined) {
			return +$(container).data('step');
		}

		if ($(container).hasClass('btnVoltar')) {
			return Fiscalizacao.stepAtual - 1;
		}

		if ($(container).hasClass('btnAvancar') || $(container).hasClass('btnSalvar')) {
			return Fiscalizacao.stepAtual + 1;
		}
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

	onSalvarStep: function (url, objetoStep, msg) {

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
		return isSalvo;
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

// 1ª Aba
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
		}

		if (parseInt($('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val()) > 0) {
			if ($('.hdnFiscalizacaoSituacaoId', Fiscalizacao.container).val() >= 2) {
				FiscalizacaoLocalInfracao.onCarregarVisualizarEmp(parseInt($('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val()), $('.hdnAutuadoEmpreendimentoTid', FiscalizacaoLocalInfracao.container).val());
			} else {
				FiscalizacaoLocalInfracao.onCarregarVisualizarEmp(parseInt($('.hdnAutuadoEmpreendimentoId', FiscalizacaoLocalInfracao.container).val()));
			}
			FiscalizacaoLocalInfracao.toggleBotoes('.spanEmpSalvar, .spanEmpAssNovo, .fdsEmpreendimento, .divDdlResponsavel');
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
			}
		});
	},
	gerarObjetoLocalInfracao: function () {
		var localInfracao = {
			Id: $('.hdnFiscalizacaoId', Fiscalizacao.container).val(),
			LocalInfracao: {
				Id: $('.hdnLocalInfracaoId', FiscalizacaoLocalInfracao.container).val(),
				FiscalizacaoId: $('.hdnFiscalizacaoId', Fiscalizacao.container).val(),
				SetorId: $('.ddlSetores', FiscalizacaoLocalInfracao.container).val(),
				Data: { DataTexto: $('.txtData', FiscalizacaoLocalInfracao.container).val() },
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


		if ($(this).val().toString() == "0") {
			$('.divPessoa', FiscalizacaoLocalInfracao.container).removeClass("hide");
		} else {
			$('.divEmpreendimento', FiscalizacaoLocalInfracao.container).removeClass("hide");
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
					$('.divResultados', FiscalizacaoLocalInfracao.container).html(response.Html);
					$('.divResultados', FiscalizacaoLocalInfracao.container).removeClass('hide');
					$('.empreendimentoPartial', FiscalizacaoLocalInfracao.container).empty();
					FiscalizacaoLocalInfracao.toggleBotoes('.spanEmpNovo, .fdsEmpreendimento');
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

		$.ajax({ url: FiscalizacaoLocalInfracao.settings.urls.novoEmpreendimento, data: FiscalizacaoLocalInfracao.gerarObjetoFiltroLocalizar(), type: 'POST', typeData: 'json',
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

// 2ª Aba
FiscalizacaoProjetoGeografico = {

	urlObter: '',

	callBackObterProjetoGeograficoVisualizar: function () {

		Fiscalizacao.stepAtual = 2;
		Fiscalizacao.salvarTelaAtual = ProjetoGeografico.onSalvar;
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
		if ($('.hdnProjetoNivelPrecisao', Fiscalizacao.container).val().toString() != "0") {
		    $('.projetoGeograficoContainer', Fiscalizacao.container).removeClass('hide');
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

	onClickRadioProjGeo: function () {
	    $('.projetoGeograficoContainer', Fiscalizacao.container).addClass('hide');
	    if ($(this).val().toString() != "0") {
	        $('.projetoGeograficoContainer', Fiscalizacao.container).removeClass('hide');
	    }
	}
}

// 3ª Aba - Foi removida!
FiscalizacaoComplementacaoDados = {
	settings: {
		urls: {
			salvar: '',
			visualizar: ''
		},
		idsTela: null
	},

	container: null,

	configurarBtnEditar: function () {
		$(".btnEditar", Fiscalizacao.container).unbind('click');
		$(".btnEditar", Fiscalizacao.container).click(FiscalizacaoComplementacaoDados.onBtnEditar);
	},

	onBtnEditar: function () {
		MasterPage.carregando(true);
		var param = { id: $('#hdnFiscalizacaoId', Fiscalizacao.container).val() };
		Fiscalizacao.onObterStep(Fiscalizacao.urls.complementacaoDados, param, FiscalizacaoComplementacaoDados.callBackObterComplementacaoDados);
	},

	callBackObterComplementacaoDadosVisualizar: function () {
		var isVisualizar = false;
		if (parseInt($('.hdnComplementacaoDadosId', Fiscalizacao.container).val()) > 0) {
			Fiscalizacao.salvarEdicao = false;
			Fiscalizacao.botoes({ btnEditar: true, spnCancelarCadastro: true });
			FiscalizacaoComplementacaoDados.configurarBtnEditar();
			isVisualizar = true;
		} else {
			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarCadastro: true });
		}

		FiscalizacaoComplementacaoDados.callBackObterComplementacaoDadosDefault(isVisualizar);
		FiscalizacaoComplementacaoDados.configurarBtnEditar();
	},

	callBackObterComplementacaoDados: function () {
		Fiscalizacao.salvarEdicao = true;
		Fiscalizacao.botoes({ btnSalvar: true, spnCancelarEdicao: true });
		FiscalizacaoComplementacaoDados.callBackObterComplementacaoDadosDefault();
	},

	callBackObterComplementacaoDadosDefault: function (isVisualizar) {
		Fiscalizacao.stepAtual = 3;
		Fiscalizacao.configurarBtnCancelarStep(3);
		Fiscalizacao.salvarTelaAtual = FiscalizacaoComplementacaoDados.onSalvarFiscalizacaoComplementacaoDados;

		if (!isVisualizar) {
			FiscalizacaoComplementacaoDados.container = $('.FiscalizacaoComplementacaoDadosContainer', Fiscalizacao.container);
			FiscalizacaoComplementacaoDados.container.delegate('.ddlVinculoComPropriedadeTipo', 'change', FiscalizacaoComplementacaoDados.gerenciarVinculoPropriedade);
			FiscalizacaoComplementacaoDados.container.delegate('.ddlConhecimentoLegislacaoTipo', 'change', FiscalizacaoComplementacaoDados.gerenciarConhecimentoLegislacao);
			FiscalizacaoComplementacaoDados.container.delegate('.checkboxReservasLegais', 'change', FiscalizacaoComplementacaoDados.gerenciarReservasLegais);

			FiscalizacaoComplementacaoDados.gerenciarVinculoPropriedade();
			FiscalizacaoComplementacaoDados.gerenciarConhecimentoLegislacao();
			FiscalizacaoComplementacaoDados.gerenciarDadosPropriedade();

			if (FiscalizacaoComplementacaoDados.obterAutuadoTipo() == FiscalizacaoComplementacaoDados.settings.idsTela.TipoAutuadoEmpreendimento) {
				FiscalizacaoComplementacaoDados.obterResponsavelVinculoPropriedade();
			}
		}

		Fiscalizacao.alternarAbas();
		Mascara.load();
		MasterPage.carregando(false);
		Fiscalizacao.gerenciarVisualizacao();
	},


	obterResponsavelVinculoPropriedade: function () {
		var container = FiscalizacaoComplementacaoDados.container;
		var empreendimentoId = $('.hdnEmpreendimentoId', container).val();
		var autuadoId = $('.hdnAutuadoId', container).val();

		$.ajax({
			url: FiscalizacaoComplementacaoDados.settings.urls.urlObterVinculoPropriedade,
			data: JSON.stringify({ autuadoId: autuadoId, empreendimentoId: empreendimentoId }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.vinculo) {
					$('.ddlVinculoComPropriedadeTipo option', FiscalizacaoComplementacaoDados.container).each(function () {
						if ($(this).val() == response.vinculo) {
							$(this).attr('selected', 'selected');

							if (response.vinculo == FiscalizacaoComplementacaoDados.settings.idsTela.VinculoPropriedadeOutro) {
								$('.txtVinculoComPropriedadeEspecificarTexto', FiscalizacaoComplementacaoDados.container).val(response.especificar);
							}

							FiscalizacaoComplementacaoDados.gerenciarVinculoPropriedade();
						}
					});
				} else {
					$('.ddlVinculoComPropriedadeTipo option:eq(0)', container).attr('selected', 'selected');
				}
			}
		});
	},

	gerenciarConhecimentoLegislacao: function () {
		var conhecimento = $('.ddlConhecimentoLegislacaoTipo :selected', FiscalizacaoComplementacaoDados.container).val();
		$('.divJustificativa', FiscalizacaoComplementacaoDados.container).addClass('hide');

		if (conhecimento == FiscalizacaoComplementacaoDados.settings.idsTela.RespostasDefaultSim ||
			conhecimento == FiscalizacaoComplementacaoDados.settings.idsTela.RespostasDefaultNao) {
			$('.divJustificativa', FiscalizacaoComplementacaoDados.container).removeClass('hide');
		}
	},

	gerenciarVinculoPropriedade: function () {
		var vinculo = $('.ddlVinculoComPropriedadeTipo :selected', FiscalizacaoComplementacaoDados.container).val();
		$('.divEspecificarVinculoPropriedade', FiscalizacaoComplementacaoDados.container).addClass('hide');
		if (vinculo == FiscalizacaoComplementacaoDados.settings.idsTela.VinculoPropriedadeOutro) {
			$('.divEspecificarVinculoPropriedade', FiscalizacaoComplementacaoDados.container).removeClass('hide');
		}
	},

	gerenciarDadosPropriedade: function () {
		var tipoAutuado = FiscalizacaoComplementacaoDados.obterAutuadoTipo();
		$('.fsDadosPropriedade', FiscalizacaoComplementacaoDados.container).addClass('hide');
		if (tipoAutuado == 2) {
			$('.fsDadosPropriedade', FiscalizacaoComplementacaoDados.container).removeClass('hide');
		}

	},

	gerenciarReservasLegais: function () {
		var container = FiscalizacaoComplementacaoDados.container;
		var current = $(this).val();

		var averbada = $('.checkbox1', container).is(':checked');
		var proposta = $('.checkbox2', container).is(':checked');
		var naoInformado = $('.checkbox3', container).is(':checked');
		var naoPossui = $('.checkbox4', container).is(':checked');

		if (averbada || proposta) {
			$('.checkbox3', container).removeAttr('checked');
			$('.checkbox4', container).removeAttr('checked');
		}

		if (naoInformado && current == 4) {
			$('.checkbox1', container).removeAttr('checked');
			$('.checkbox2', container).removeAttr('checked');
			$('.checkbox3', container).attr('checked', 'checked');
			$('.checkbox4', container).removeAttr('checked');
		}

		if (naoPossui && current == 8) {
			$('.checkbox1', container).removeAttr('checked');
			$('.checkbox2', container).removeAttr('checked');
			$('.checkbox3', container).removeAttr('checked');
			$('.checkbox4', container).attr('checked', 'checked');
		}
	},

	obterAutuadoTipo: function () {
		return Number($('.hdnTipoAutuado', FiscalizacaoComplementacaoDados.container).val()) || 0;
	},

	onSalvarFiscalizacaoComplementacaoDados: function () {
		var container = FiscalizacaoComplementacaoDados.container;

		var obj = {
			Id: Number($('.hdnComplementacaoDadosId', container).val()) || 0,
			FiscalizacaoId: Number($('.hdnFiscalizacaoId', Fiscalizacao.container).val()) || 0,
			EmpreendimentoId: Number($('.hdnEmpreendimentoId', container).val()) || 0,
			AutuadoTipo: FiscalizacaoComplementacaoDados.obterAutuadoTipo(),
			AutuadoId: $('.hdnAutuadoId', container).val(),
			ResidePropriedadeTipo: $('.ddlResidePropriedadeTipo :selected', container).val(),
			ResidePropriedadeTipoTexto: $('.ddlResidePropriedadeTipo :selected', container).text(),
			RendaMensalFamiliarTipo: $('.ddlRendaMensalFamiliarTipo :selected', container).val(),
			RendaMensalFamiliarTipoTexto: $('.ddlRendaMensalFamiliarTipo :selected', container).text(),
			NivelEscolaridadeTipo: $('.ddlNivelEscolaridadeTipo :selected', container).val(),
			NivelEscolaridadeTipoTexto: $('.ddlNivelEscolaridadeTipo :selected', container).text(),
			VinculoComPropriedadeTipo: $('.ddlVinculoComPropriedadeTipo :selected', container).val(),
			VinculoComPropriedadeTipoTexto: $('.ddlVinculoComPropriedadeTipo :selected', container).text(),
			VinculoComPropriedadeEspecificarTexto: $('.txtVinculoComPropriedadeEspecificarTexto', container).val(),
			ConhecimentoLegislacaoTipo: $('.ddlConhecimentoLegislacaoTipo :selected', container).val(),
			ConhecimentoLegislacaoTipoTexto: $('.ddlConhecimentoLegislacaoTipo :selected', container).text(),
			Justificativa: $('.txtJustificativa', container).val(),
			AreaTotalInformada: $('.txtAreaTotalInformada', container).val(),
			AreaCoberturaFlorestalNativa: $('.txtAreaCoberturaFlorestalNativa', container).val(),
			ReservalegalTipo: 0
		}

		$('.checkboxReservasLegais:checked', container).each(function () {
			obj.ReservalegalTipo += parseInt($(this).val());
		});

		if (obj.AutuadoTipo == FiscalizacaoComplementacaoDados.settings.idsTela.TipoAutuadoPessoa) {
			//setando id do autuado, pois o campo '.ddlResponsaveis' esta hide
			obj.ResponsavelId = $('.hdnAutuadoId', container).val();

			//limpando campos ocultos
			obj.AreaTotalInformada = '';
			obj.AreaCoberturaFlorestalNativa = '';
			obj.ReservalegalTipo = 0;
			obj.EmpreendimentoId = 0;
		}

		if (obj.VinculoComPropriedadeTipo != FiscalizacaoComplementacaoDados.settings.idsTela.VinculoPropriedadeOutro) {
			obj.VinculoComPropriedadeEspecificarTexto = '';
		}

		if (obj.ConhecimentoLegislacaoTipo == 3) {
			obj.Justificativa = '';
		}

		var arrayMensagem = new Array();

		arrayMensagem.push(FiscalizacaoComplementacaoDados.settings.mensagens.Salvar)

		return Fiscalizacao.onSalvarStep(FiscalizacaoComplementacaoDados.settings.urls.salvar, obj, arrayMensagem);

	}

},

// 4ª Aba
FiscalizacaoEnquadramento = {
	settings: {
		urls: {
			salvar: '',
			visualizar: ''
		},
		idsTela: null,
		mensagens: null
	},
	container: null,

	configurarBtnEditar: function () {
		$(".btnEditar", Fiscalizacao.container).unbind('click');
		$(".btnEditar", Fiscalizacao.container).click(FiscalizacaoEnquadramento.onBtnEditar);
	},

	onBtnEditar: function () {
		MasterPage.carregando(true);
		var param = { id: $('#hdnFiscalizacaoId', Fiscalizacao.container).val() };
		Fiscalizacao.onObterStep(Fiscalizacao.urls.enquadramento, param, FiscalizacaoEnquadramento.callBackObterEnquadramento);
	},

	callBackObterEnquadramentoVisualizar: function () {
		if (parseInt($('.hdnEnquadramentoId', Fiscalizacao.container).val()) > 0) {
			Fiscalizacao.salvarEdicao = false;
			Fiscalizacao.botoes({ btnEditar: true, spnCancelarCadastro: true });
			FiscalizacaoEnquadramento.configurarBtnEditar();
		} else {
			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarCadastro: true });
		}

		FiscalizacaoEnquadramento.callBackObterEnquadramentoDefault();
		FiscalizacaoEnquadramento.configurarBtnEditar();
	},

	callBackObterEnquadramento: function () {
		Fiscalizacao.salvarEdicao = true;
		Fiscalizacao.botoes({ btnSalvar: true, spnCancelarEdicao: true });
		FiscalizacaoEnquadramento.callBackObterEnquadramentoDefault();
	},

	callBackObterEnquadramentoDefault: function () {
		FiscalizacaoEnquadramento.container = $('.FiscalizacaoEnquadramentoContainer', Fiscalizacao.container);
		Fiscalizacao.configurarBtnCancelarStep(4);
		Fiscalizacao.stepAtual = 4;
		Fiscalizacao.salvarTelaAtual = FiscalizacaoEnquadramento.onSalvarFiscalizacaoEnquadramento;
		Fiscalizacao.alternarAbas();
		MasterPage.botoes(Fiscalizacao.container);

		MasterPage.carregando(false);
		Fiscalizacao.gerenciarVisualizacao();
	},

	onSalvarFiscalizacaoEnquadramento: function () {
		var container = FiscalizacaoEnquadramento.container;

		var obj = {
			Id: Number($('.hdnEnquadramentoId', container).val()) || 0,
			FiscalizacaoId: Number($('.hdnFiscalizacaoId', Fiscalizacao.container).val()) || 0,
			Artigos: []
		}

		$('.fsArtigo', container).each(function () {
			var artigo = {
				Id: Number($('.hdnArtigoId', this).val()) || 0,
				Identificador: $('.hdnIdentificador', this).val(),
				EnquadramentoId: $('.hdnEnquadramentoId', FiscalizacaoEnquadramento.container).val(),
				ArtigoTexto: $('.txtArtigoTexto', this).val(),
				ArtigoParagrafo: $('.txtArtigoParagrafo', this).val(),
				CombinadoArtigo: $('.txtCombinadoArtigo', this).val(),
				CombinadoArtigoParagrafo: $('.txtCombinadoArtigoParagrafo', this).val(),
				DaDo: $('.txtDaDo', this).val()
			}

			obj.Artigos.push(artigo);

		});

		var arrayMensagem = new Array();

		arrayMensagem.push(FiscalizacaoEnquadramento.settings.mensagens.Salvar)

		return Fiscalizacao.onSalvarStep(FiscalizacaoEnquadramento.settings.urls.salvar, obj, arrayMensagem);

	}

},

// 5ª Aba
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

		Fiscalizacao.stepAtual = 5;
		Fiscalizacao.salvarTelaAtual = Infracao.onSalvarInfracao;
		Fiscalizacao.alternarAbas();

		Infracao.container.delegate('.ddlClassificacoes', 'change', Infracao.onSelecionarClassificacao);
		Infracao.container.delegate('.ddlTipos', 'change', Infracao.onSelecionarTipo);
		Infracao.container.delegate('.ddlItens', 'change', Infracao.onSelecionarItem);
		Infracao.container.delegate('.rdoIsGeradaSistemaSim', 'change', Infracao.onSelecionarIsGeradaSistemaSim);
		Infracao.container.delegate('.rdoIsGeradaSistemaNao', 'change', Infracao.onSelecionarIsGeradaSistemaNao);
		Infracao.container.delegate('.rdoIsAutuadaSim', 'change', Infracao.onSelecionarIsAutuadaSim);
		Infracao.container.delegate('.rdoIsAutuadaNao', 'change', Infracao.onSelecionarrdoIsAutuadaNao);
		Infracao.container.delegate('.rdoIsEspecificar', 'change', Infracao.onSelecionarIsEspecificar);
		Infracao.container.delegate('.rdoIsNotEspecificar', 'change', Infracao.onSelecionarIsNotEspecificar);

		Infracao.container.delegate('.rdbIsGeradaSistema', 'click', Infracao.gerenciarIsGeradaSistema);
		Infracao.container.delegate('.ddlSeries', 'change', Infracao.gerenciarSerie);

		Infracao.gerenciarSerie();

		Infracao.container.delegate('.btnAddArq', 'click', Infracao.onEnviarArquivoClick);
		Infracao.container.delegate('.btnLimparArq', 'click', Infracao.onLimparArquivoClick);

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
		MasterPage.botoes();
		MasterPage.carregando(false);
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
			Fiscalizacao.configurarBtnCancelarStep(5);
			Fiscalizacao.gerenciarVisualizacao();
		});
	},

	gerenciarIsGeradaSistema: function () {
		var rdb = $('.rdbIsGeradaSistema:checked', Infracao.container).val();
		if (rdb == 0) {
			Infracao.onSelecionarIsGeradaSistemaNao();
		} else {
			if (rdb == 1) {
				Infracao.onSelecionarIsGeradaSistemaSim();
			}
		}
	},

	callBackObterInfracao: function () {

		Fiscalizacao.stepAtual = 5;
		Fiscalizacao.salvarTelaAtual = Infracao.onSalvarInfracao;
		Fiscalizacao.alternarAbas();

		Fiscalizacao.botoes({ btnSalvar: true, spnCancelarEdicao: true });

		Fiscalizacao.configurarBtnCancelarStep(5);
		MasterPage.carregando(false);
	},

	onSelecionarIsGeradaSistemaSim: function () {
		$('.divIsGeradoSistema', Infracao.container).hide();
		$('.ddlSeries option:eq(3)', Infracao.container).attr('selected', 'selected');
		$('.ddlSeries', Infracao.container).attr('disabled', 'disabled');

		Infracao.onLimparArquivoClick();
	},

	onSelecionarIsGeradaSistemaNao: function () {
		$('.divIsGeradoSistema', Infracao.container).show();
		$('.ddlSeries', Infracao.container).removeAttr('disabled', 'disabled').val(0);
		Infracao.gerenciarSerie();
	},

	gerenciarSerie: function () {
		if ($('.rdbIsGeradaSistema:checked', Infracao.container).val() == 0) {
			var serie = $('.ddlSeries :selected', Infracao.container).val();
			if (serie == 3) {
				$('.lblNumAutoInfracao', Infracao.container).text('Nº do auto de infração *');
			} else {
				$('.lblNumAutoInfracao', Infracao.container).text('Nº do auto de infração - bloco *');
			}
		} else {
			Infracao.onSelecionarIsGeradaSistemaSim();
		}

	},

	onSelecionarIsAutuadaSim: function () {
		$('.divInfracaoAutuada', Infracao.container).show();
	},

	onSelecionarrdoIsAutuadaNao: function () {
		$('.divInfracaoAutuada', Infracao.container).hide();
	},

	onSelecionarIsEspecificar: function () {
		var perguntaId = $(this, Infracao.container).attr('perguntaId');
		$('.divEspecificacao' + perguntaId, Infracao.container).show();
		$('.hdnRespostaEspecificar' + perguntaId, Infracao.container).val('1');
	},

	onSelecionarIsNotEspecificar: function () {
		var perguntaId = $(this, Infracao.container).attr('perguntaId');
		$('.divEspecificacao' + perguntaId, Infracao.container).hide();
		$('.hdnRespostaEspecificar' + perguntaId, Infracao.container).val('0');
	},

	onSelecionarClassificacao: function () {

		var classificacao = $('.ddlClassificacoes', Infracao.container).val();

		if (classificacao) {

			$('.ddlTipos', Infracao.container).ddlClear();
			$('.ddlItens', Infracao.container).ddlClear();
			$('.ddlSubitens', Infracao.container).ddlClear();
			$('.divCamposPerguntas', Infracao.container).html('');

			$.ajax({ url: Infracao.settings.urls.obterTipo,
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

			$.ajax({ url: Infracao.settings.urls.obterItem,
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

			$.ajax({ url: Infracao.settings.urls.obterConfiguracao,
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

	onSalvarInfracao: function () {

		var container = Infracao.container;

		var obj = {
			Id: Number($('.hdnInfracaoId', container).val()),
			FiscalizacaoId: Number($('.hdnFiscalizacaoId', Fiscalizacao.container).val()),
			ClassificacaoId: $('.ddlClassificacoes :selected', container).val(),
			TipoId: $('.ddlTipos :selected', container).val(),
			ItemId: $('.ddlItens :selected', container).val(),
			IsAutuada: '',
			ConfiguracaoId: $('.hdnConfiguracaoId', container).val(),
			ConfiguracaoTid: $('.hdnConfiguracaoTid', container).val(),
			Campos: [],
			Perguntas: []
		}

		if ($('.rdoIsAutuadaSim', container).attr('checked')) {
			obj.IsAutuada = true;
			obj.SerieId = $('.ddlSeries :selected', container).val();
			obj.CodigoReceitaId = $('.ddlCodigoReceitas :selected', container).val();
			obj.IsGeradaSistema = $('.rdoIsGeradaSistemaSim', container).attr('checked');
			obj.ValorMulta = $('.txtValorMulta', container).val();
			obj.CodigoReceita = $('.txtCodigoReceita', container).val();
			obj.NumeroAutoInfracaoBloco = $('.txtNumeroAutoInfracaoBloco', container).val();
			obj.DescricaoInfracao = $('.txtDescricaoInfracao', container).val();
			obj.DataLavraturaAuto = { DataTexto: $('.txtDataLavraturaAuto', container).val() };
			obj.ValorExtenso = $('.txtValorExtenso', container).val();
			obj.Arquivo = $.parseJSON($('.hdnArquivoJson', container).val());
		}

		if ($('.rdoIsAutuadaNao', container).attr('checked')) {
			obj.IsAutuada = false;
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

		//Limpando dados de campos
		if (obj.IsGeradaSistema) {
			obj.DataLavraturaAuto.DataTexto = '';
			obj.NumeroAutoInfracaoBloco = '';
		}

		var arrayMensagem = [];

		arrayMensagem.push(Infracao.settings.mensagens.Salvar);

		return Fiscalizacao.onSalvarStep(Infracao.settings.urls.salvar, obj, arrayMensagem);

	},

	onEnviarArquivoClick: function () {
		var nomeArquivo = $('.inputFile', Infracao.container).val();

		erroMsg = new Array();

		if (nomeArquivo == '') {
			erroMsg.push(Infracao.settings.mensagens.ArquivoObrigatorio);
		} else {
			var tam = nomeArquivo.length - 4;
			if (!Infracao.validarTipoArquivo(nomeArquivo.toLowerCase().substr(tam))) {
				erroMsg.push(Infracao.settings.mensagens.ArquivoNaoEhDoc);
			}
		}

		if (erroMsg.length > 0) {
			Mensagem.gerar(Fiscalizacao.container, erroMsg);
			return;
		}

		MasterPage.carregando(true);
		var inputFile = $('.inputFile', Infracao.container);
		FileUpload.upload(Infracao.settings.urls.enviarArquivo, inputFile, Infracao.callBackArqEnviado);
	},

	onLimparArquivoClick: function () {
		$('.hdnArquivoJson', Infracao.container).val('');
		$('.inputFile', Infracao.container).val('');

		$('.spanInputFile', Infracao.container).removeClass('hide');
		$('.txtArquivoNome', Infracao.container).addClass('hide');

		$('.btnAddArq', Infracao.container).removeClass('hide');
		$('.btnLimparArq', Infracao.container).addClass('hide');
	},

	validarTipoArquivo: function (tipo) {

		var tipoValido = false;
		$(Infracao.TiposArquivo).each(function (i, tipoItem) {
			if (tipoItem == tipo) {
				tipoValido = true;
			}
		});

		return tipoValido;
	},

	callBackArqEnviado: function (controle, retorno, isHtml) {
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoNome', Infracao.container).text(ret.Arquivo.Nome);
			$('.hdnArquivoJson', Infracao.container).val(JSON.stringify(ret.Arquivo));
			$('.txtArquivoNome', Infracao.container).attr('href', '/Arquivo/BaixarTemporario?nomeTemporario=' + ret.Arquivo.TemporarioNome + '&contentType=' + ret.Arquivo.ContentType);

			$('.spanInputFile', Infracao.container).addClass('hide');
			$('.txtArquivoNome', Infracao.container).removeClass('hide');

			$('.btnAddArq', Infracao.container).addClass('hide');
			$('.btnLimparArq', Infracao.container).removeClass('hide');
		} else {
			Infracao.onLimparArquivoClick();
			Mensagem.gerar(Infracao.container, ret.Msg);
		}
		MasterPage.carregando(false);
	}
}

// 6ª Aba
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
		Fiscalizacao.stepAtual = 6;
		Fiscalizacao.salvarTelaAtual = FiscalizacaoObjetoInfracao.onSalvarFiscalizacaoObjetoInfracao;
		Fiscalizacao.alternarAbas();

		FiscalizacaoObjetoInfracao.container.delegate('.rdbAreaEmbargadaAtvIntermed', 'click', FiscalizacaoObjetoInfracao.gerenciarAreaEmbarcadaAtvIntermed);
		FiscalizacaoObjetoInfracao.container.delegate('.rdbTeiGeradoPeloSistema', 'click', FiscalizacaoObjetoInfracao.gerenciarTeiGeradoPeloSistema);
		FiscalizacaoObjetoInfracao.container.delegate('.rdbExisteAtvAreaDegrad', 'click', FiscalizacaoObjetoInfracao.gerenciarExisteAtvAreaDegradEspecificarTexto);
		FiscalizacaoObjetoInfracao.container.delegate('.btnAddArq', 'click', FiscalizacaoObjetoInfracao.onEnviarArquivoClick);
		FiscalizacaoObjetoInfracao.container.delegate('.btnLimparArq', 'click', FiscalizacaoObjetoInfracao.onLimparArquivoClick);
		FiscalizacaoObjetoInfracao.container.delegate('.ddlInfracaoResultouErosaoTipo', 'change', FiscalizacaoObjetoInfracao.onChangeErosao);
		FiscalizacaoObjetoInfracao.container.delegate('.ddlTeiGeradoPeloSistemaSerieTipo', 'change', FiscalizacaoObjetoInfracao.gerenciarSerie);

		FiscalizacaoObjetoInfracao.gerenciarAreaEmbarcadaAtvIntermed();
		FiscalizacaoObjetoInfracao.gerenciarTeiGeradoPeloSistema();
		FiscalizacaoObjetoInfracao.gerenciarExisteAtvAreaDegradEspecificarTexto();
		FiscalizacaoObjetoInfracao.gerenciarSerie();

		Mascara.load(FiscalizacaoObjetoInfracao.container);
		Fiscalizacao.configurarBtnCancelarStep(6);
		MasterPage.carregando(false);
		MasterPage.botoes();
		Fiscalizacao.gerenciarVisualizacao();
	},

	gerenciarAreaEmbarcadaAtvIntermed: function () {
		var container = FiscalizacaoObjetoInfracao.container;
		var rdb = $('.rdbAreaEmbargadaAtvIntermed:checked', container).val();

		$('.divAreaEmbarcada', container).addClass('hide');
		if (rdb == 1) {
			$('.divAreaEmbarcada', container).removeClass('hide');
		}

		if (rdb == 0) {
			$('.divTeiGeradoPeloSistema', container).addClass('hide');

			//Limpa campos
			$('.rdbTeiGeradoPeloSistema:checked', container).removeAttr('checked');
			$('.ddlTeiGeradoPeloSistemaSerieTipo option:eq(0)', container).attr('selected', 'selected');
			$('.txtDataLavraturaTermo', container).val('');
			$('.txtDescricaoTermoEmbargo', container).val('');
			$('.txtNumTeiBloco', container).val('');

		}
	},

	gerenciarTeiGeradoPeloSistema: function () {
		var container = FiscalizacaoObjetoInfracao.container;
		var rdb = $('.rdbTeiGeradoPeloSistema:checked', container).val();

		$('.divTeiGeradoPeloSistema', container).addClass('hide');
		$('.ddlTeiGeradoPeloSistemaSerieTipo', container).attr('disabled', 'disabled');

		if (rdb == 0) {
			$('.divTeiGeradoPeloSistema', container).removeClass('hide');

			if ($('.hdnIsVisualizar', container).val() != 'true')
				$('.ddlTeiGeradoPeloSistemaSerieTipo', container).removeAttr('disabled', 'disabled');
			FiscalizacaoObjetoInfracao.gerenciarSerie();
		} else {

			if (rdb == 1) {
				$('.ddlTeiGeradoPeloSistemaSerieTipo option:eq(3)', container).attr('selected', 'selected');
				$('.ddlTeiGeradoPeloSistemaSerieTipo', container).attr('disabled', 'disabled');
			} else {
				$('.ddlTeiGeradoPeloSistemaSerieTipo option:eq(0)', container).attr('selected', 'selected');
			}
		}

	},

	gerenciarExisteAtvAreaDegradEspecificarTexto: function () {
		var container = FiscalizacaoObjetoInfracao.container;
		var rdb = $('.rdbExisteAtvAreaDegrad:checked', container).val();

		$('.divExisteAtvAreaDegradEspecificarTexto', container).addClass('hide');
		if (rdb == 1) {
			$('.divExisteAtvAreaDegradEspecificarTexto', container).removeClass('hide');
		}
	},

	gerenciarSerie: function () {
		var container = FiscalizacaoObjetoInfracao.container;
		var rdb = $('.rdbTeiGeradoPeloSistema:checked', container).val();
		if (rdb == 0) {
			var serie = $('.ddlTeiGeradoPeloSistemaSerieTipo :selected').val();
			if (serie == 3) {
				$('.lblNumTEI', container).text('Nº do TEI *');
			} else {
				$('.lblNumTEI', container).text('Nº do TEI - bloco *');
			}
		}
	},

	onSalvarFiscalizacaoObjetoInfracao: function () {
		var container = FiscalizacaoObjetoInfracao.container;

		var obj = {
			Id: Number($('.hdnObjetoInfracaoId', container).val()) || 0,
			FiscalizacaoId: Number($('.hdnFiscalizacaoId', Fiscalizacao.container).val()) || 0,
			AreaEmbargadaAtvIntermed: $('.rdbAreaEmbargadaAtvIntermed:checked', container).val(),
			TeiGeradoPeloSistema: $('.rdbTeiGeradoPeloSistema:checked', container).val(),
			TeiGeradoPeloSistemaSerieTipo: $('.ddlTeiGeradoPeloSistemaSerieTipo :selected', container).val(),
			TeiGeradoPeloSistemaSerieTipoTexto: $('.ddlTeiGeradoPeloSistemaSerieTipo :selected', container).text(),
			NumTeiBloco: $('.txtNumTeiBloco', container).val(),
			DataLavraturaTermo: { DataTexto: $('.txtDataLavraturaTermo', container).val() },
			OpniaoAreaDanificada: $('.txtOpniaoAreaDanificada', container).val(),
			DescricaoTermoEmbargo: $('.txtDescricaoTermoEmbargo', container).val(),
			ExisteAtvAreaDegrad: $('.rdbExisteAtvAreaDegrad:checked', container).val(),
			ExisteAtvAreaDegradEspecificarTexto: $('.txtExisteAtvAreaDegradEspecificarTexto', container).val(),
			FundamentoInfracao: $('.txtFundamentoInfracao', container).val(),
			UsoSoloAreaDanificada: $('.txtUsoSoloAreaDanificada', container).val(),
			CaracteristicaSoloAreaDanificada: 0,
			AreaDeclividadeMedia: $('.txtAreaDeclividadeMedia', container).val(),
			InfracaoResultouErosaoTipo: $('.ddlInfracaoResultouErosaoTipo', container).val(),
			Arquivo: $.parseJSON($('.hdnArquivoJson', container).val()),
			InfracaoResultouErosaoTipoTexto: $('.txtErosao', container).val().trim()
		}

		$('.checkboxCaracteristicasSolo:checked', Fiscalizacao.container).each(function () {
			obj.CaracteristicaSoloAreaDanificada += parseInt($(this).val());
		});


		//limpando dados {
		if (obj.AreaEmbargadaAtvIntermed == 1) {
			if (obj.TeiGeradoPeloSistema == 1) {
				obj.NumTeiBloco = '';
				obj.Arquivo = null;
				obj.DataLavraturaTermo.DataTexto = '';
			}
		}

		if (obj.AreaEmbargadaAtvIntermed == 0) {
			obj.OpniaoAreaDanificada = '';
		}

		if (obj.ExisteAtvAreaDegrad == 0) {
			obj.ExisteAtvAreaDegradEspecificarTexto = '';
		}

		//}

		var arrayMensagem = new Array();

		arrayMensagem.push(FiscalizacaoObjetoInfracao.settings.mensagens.Salvar)

		return Fiscalizacao.onSalvarStep(FiscalizacaoObjetoInfracao.settings.urls.salvar, obj, arrayMensagem);
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
	},

	onChangeErosao: function () {
		$('.divTxtErosao', FiscalizacaoObjetoInfracao.container).addClass('hide');
		$('.txtErosao', FiscalizacaoObjetoInfracao.container).val('');
		if ($(this).val() === "1") {
			$('.divTxtErosao', FiscalizacaoObjetoInfracao.container).removeClass('hide');
		}
	}
},

// 7ª Aba
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
		FiscalizacaoMaterialApreendido.gerenciarIsGeradaSistema();
	},

	callBackObterFiscalizacaoMaterialApreendidoDefault: function () {
		Fiscalizacao.stepAtual = 7;
		Fiscalizacao.salvarTelaAtual = FiscalizacaoMaterialApreendido.onSalvarFiscalizacaoMaterialApreendido;
		Fiscalizacao.alternarAbas();

		FiscalizacaoMaterialApreendido.container.delegate('.ddlTipos', 'change', FiscalizacaoMaterialApreendido.onSelecionarTipo);
		FiscalizacaoMaterialApreendido.container.delegate('.ddlEstado', 'change', Aux.onEnderecoEstadoChange);
		FiscalizacaoMaterialApreendido.container.delegate('.rdoIsApreendidoSim', 'change', FiscalizacaoMaterialApreendido.onSelecionarIsApreendidoSim);
		FiscalizacaoMaterialApreendido.container.delegate('.rdoIsApreendidoNao', 'change', FiscalizacaoMaterialApreendido.onSelecionarIsApreendidoNao);
		FiscalizacaoMaterialApreendido.container.delegate('.rdoIsGeradoSistemaSim', 'change', FiscalizacaoMaterialApreendido.onSelecionarIsGeradaSistemaSim);
		FiscalizacaoMaterialApreendido.container.delegate('.rdoIsGeradoSistemaNao', 'change', FiscalizacaoMaterialApreendido.onSelecionarIsGeradaSistemaNao);
		FiscalizacaoMaterialApreendido.container.delegate('.btnAssociarDepositario', 'click', FiscalizacaoMaterialApreendido.onAssociarDepositario);
		FiscalizacaoMaterialApreendido.container.delegate('.btnEditarDepositario', 'click', FiscalizacaoMaterialApreendido.onEditarDepositario);
		FiscalizacaoMaterialApreendido.container.delegate('.btnAdicionarMaterial', 'click', FiscalizacaoMaterialApreendido.adicionarMaterial);
		FiscalizacaoMaterialApreendido.container.delegate('.btnExcluirMaterial', 'click', FiscalizacaoMaterialApreendido.excluirMaterial);

		FiscalizacaoMaterialApreendido.container.delegate('.btnAddArq', 'click', FiscalizacaoMaterialApreendido.onEnviarArquivoClick);
		FiscalizacaoMaterialApreendido.container.delegate('.btnLimparArq', 'click', FiscalizacaoMaterialApreendido.onLimparArquivoClick);

		FiscalizacaoMaterialApreendido.container.delegate('.rbdIsGeradoSistema', 'change', FiscalizacaoMaterialApreendido.gerenciarIsGeradaSistema);
		FiscalizacaoMaterialApreendido.container.delegate('.ddlSeries', 'change', FiscalizacaoMaterialApreendido.gerenciarSerie);

		Mascara.load(FiscalizacaoMaterialApreendido.container);

		if (parseInt($('.hdnMaterialApreendidoId', FiscalizacaoMaterialApreendido.container).val()) > 0) {
			Fiscalizacao.salvarEdicao = false;
			Fiscalizacao.botoes({ btnEditar: true, spnCancelarCadastro: true });
			FiscalizacaoMaterialApreendido.configurarBtnEditar();
		} else {
			Fiscalizacao.salvarEdicao = true;
			Fiscalizacao.botoes({ btnSalvar: true, spnCancelarCadastro: true });
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
			Fiscalizacao.configurarBtnCancelarStep(7);
			Fiscalizacao.gerenciarVisualizacao();
		});
	},

	onSelecionarIsApreendidoSim: function () {
		$('.divApreensao', FiscalizacaoMaterialApreendido.container).show();
	},

	onSelecionarIsApreendidoNao: function () {
		$('.divApreensao', FiscalizacaoMaterialApreendido.container).hide();

		$('.divIsTad', FiscalizacaoMaterialApreendido.container).hide();
		$('.rdoIsGeradoSistemaNao', FiscalizacaoMaterialApreendido.container).removeAttr('checked');
		$('.rdoIsGeradoSistemaSim', FiscalizacaoMaterialApreendido.container).removeAttr('checked');
		$('.txtNumeroTad', FiscalizacaoMaterialApreendido.container).val('');
		$('.txtDataLavratura', FiscalizacaoMaterialApreendido.container).val('');
	},

	gerenciarSerie: function () {
		var container = FiscalizacaoMaterialApreendido.container;
		var rdb = $('.rbdIsGeradoSistema:checked', container).val();
		if (rdb == 0) {
			var serie = $('.ddlSeries :selected').val();
			if (serie == 3) {
				$('.lblNumTAD', container).text('Nº do TAD *');
			} else {
				$('.lblNumTAD', container).text('Nº do TAD - bloco *');
			}
		}
	},

	gerenciarIsGeradaSistema: function () {
		var rdb = $('.rbdIsGeradoSistema:checked', FiscalizacaoMaterialApreendido.container).val();
		if (rdb == 0) {
			FiscalizacaoMaterialApreendido.onSelecionarIsGeradaSistemaNao();
		} else {
			if (rdb == 1) {
				FiscalizacaoMaterialApreendido.onSelecionarIsGeradaSistemaSim();
			}
		}
		FiscalizacaoMaterialApreendido.gerenciarSerie();
	},

	onSelecionarIsGeradaSistemaSim: function () {
		$('.divIsTad', FiscalizacaoMaterialApreendido.container).hide();

		$('.ddlSeries option:eq(3)', FiscalizacaoMaterialApreendido.container).attr('selected', 'selected');
		$('.ddlSeries', FiscalizacaoMaterialApreendido.container).attr('disabled', 'disabled');
	},

	onSelecionarIsGeradaSistemaNao: function () {
		$('.divIsTad', FiscalizacaoMaterialApreendido.container).show();
		$('.ddlSeries', FiscalizacaoMaterialApreendido.container).removeAttr('disabled', 'disabled');
		FiscalizacaoMaterialApreendido.gerenciarSerie();
	},

	onSalvarFiscalizacaoMaterialApreendido: function () {
		var container = FiscalizacaoMaterialApreendido.container;

		var obj = {
			Id: Number($('.hdnMaterialApreendidoId', container).val()),
			FiscalizacaoId: Number($('.hdnFiscalizacaoId', Fiscalizacao.container).val()),
			IsApreendido: '',
			Materiais: []
		};

		if ($('.rdoIsApreendidoSim', container).attr('checked')) {
			obj.IsApreendido = true;

			obj.SerieId = $('.ddlSeries :selected', container).val();

			if ($('.rdoIsGeradoSistemaNao', container).attr('checked')) {
				obj.IsTadGeradoSistema = false;
				obj.NumeroTad = $('.txtNumeroTad', container).val();
				obj.Arquivo = $.parseJSON($('.hdnArquivoJson', container).val());
				obj.DataLavratura = { DataTexto: $('.txtDataLavratura', container).val() };
			}

			if ($('.rdoIsGeradoSistemaSim', container).attr('checked')) {
				obj.IsTadGeradoSistema = true;
			}


			obj.Depositario = {
				Id: $('.hdnDepositarioId', container).val(),
				Estado: $('.ddlEstado :selected', container).val(),
				Municipio: $('.ddlMunicipio :selected', container).val(),
				Logradouro: $('.txtLogradouro', container).val(),
				Bairro: $('.txtBairro', container).val(),
				Distrito: $('.txtDistrito', container).val()
			};
			obj.Descricao = $('.txtDescricao', container).val();
			obj.ValorProdutos = $('.txtValorProdutos', container).val();
			obj.Opiniao = $('.txtOpiniao', container).val();

			$('.hdnItemJSon', container.find('.divMateriais')).each(function () {
				var objMaterial = String($(this).val());
				if (objMaterial != '') {
					obj.Materiais.push(JSON.parse(objMaterial));
				}
			});
		}

		if ($('.rdoIsApreendidoNao', container).attr('checked')) {
			obj.IsApreendido = false;
		}

		var arrayMensagem = [];

		arrayMensagem.push(FiscalizacaoMaterialApreendido.settings.mensagens.Salvar);

		return Fiscalizacao.onSalvarStep(FiscalizacaoMaterialApreendido.settings.urls.salvar, obj, arrayMensagem);
	},

	onAssociarDepositario: function () {
		FiscalizacaoMaterialApreendido.pessoaModalInte = new PessoaAssociar();

		Modal.abrir(FiscalizacaoMaterialApreendido.settings.urls.associarDepositario, null, function (container) {
			FiscalizacaoMaterialApreendido.pessoaModalInte.load(container, {
				tituloCriar: 'Cadastrar Depositario',
				tituloEditar: 'Editar Depositario',
				tituloVisualizar: 'Visualizar Depositario',
				onAssociarCallback: FiscalizacaoMaterialApreendido.callBackEditarDepositario
			});
		});
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

	callBackEditarDepositario: function (Pessoa) {
		$('.spanVisualizarDepositario', FiscalizacaoMaterialApreendido.container).removeClass('hide');
		$('.hdnDepositarioId', FiscalizacaoMaterialApreendido.container).val(Pessoa.Id);
		$('.txtNome', FiscalizacaoMaterialApreendido.container).val(Pessoa.NomeRazaoSocial);
		$('.txtCnpj', FiscalizacaoMaterialApreendido.container).val(Pessoa.CPFCNPJ);
		return true;
	},

	onSelecionarTipo: function () {
		var tipo = $('.ddlTipos :selected', FiscalizacaoMaterialApreendido.container).val();
		$('.labEspecificacao', FiscalizacaoMaterialApreendido.container).text('');

		switch (tipo) {

			case '1':
			case '7':
				$('.labEspecificacao', FiscalizacaoMaterialApreendido.container).text('Especificar a quantidade');
				break;

			case '2':
			case '3':
			case '4':
			case '6':
				$('.labEspecificacao', FiscalizacaoMaterialApreendido.container).text('Especificar o volume em (m³) e como foi medido');
				break;

			case '5':
			case '8':
				$('.labEspecificacao', FiscalizacaoMaterialApreendido.container).text('Especificar');
				break;
		}
	},

	adicionarMaterial: function () {
		var mensagens = new Array();
		Mensagem.limpar(FiscalizacaoMaterialApreendido.container);
		var container = $('.fsMateriais');

		var item = { Id: '', TipoId: $('.ddlTipos :selected', container).val(), TipoTexto: $('.ddlTipos :selected', container).text(), Especificacao: $('.txtEspecificacao', container).val() };

		if (item.TipoId == 0) {
			mensagens.push(jQuery.extend(true, {}, FiscalizacaoMaterialApreendido.settings.mensagens.TipoObrigatorio));
		}

		if (jQuery.trim(item.Especificacao) == '') {
			mensagens.push(jQuery.extend(true, {}, FiscalizacaoMaterialApreendido.settings.mensagens.EspecificacaoObrigatorio));
		}

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var itemAdd = (JSON.parse(obj));
				if (item.TipoId == itemAdd.TipoId) {
					mensagens.push(jQuery.extend(true, {}, FiscalizacaoMaterialApreendido.settings.mensagens.MaterialJaAdicionado));
				}
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(FiscalizacaoMaterialApreendido.container, mensagens);
			return;
		}

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(item));
		linha.find('.tipo').html(item.TipoTexto).attr('title', item.TipoTexto);
		linha.find('.especificacao').html(item.Especificacao).attr('title', item.Especificacao);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.ddlTipos', container).ddlFirst();
		$('.txtEspecificacao', container).val('');
	},

	excluirMaterial: function () {
		var container = $('.fsMateriais');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
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
	}
}

// 8ª Aba
FiscalizacaoConsideracaoFinal = {
	settings: {
		urls: {
			salvar: '',
			obter: '',
			obterSetores: '',
			obterEnderecoSetor: '',
			enviarArquivo: '',
			obterAssinanteCargos: '',
			obterAssinanteFuncionarios: ''
		},
		modo: 1,
		mensagens: {}
	},
	TiposArquivo: [],
	isLoad: true,
	container: null,
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

		$('.ddlFuncIDAF', FiscalizacaoConsideracaoFinal.container).change(FiscalizacaoConsideracaoFinal.onChangeFuncIDAF);
		$('.ddlTestemunha', FiscalizacaoConsideracaoFinal.container).change(FiscalizacaoConsideracaoFinal.onChangeFunc);
		$('.ddlSetor', FiscalizacaoConsideracaoFinal.container).change(FiscalizacaoConsideracaoFinal.onChangeSetor);

		$('.btnAddArq', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onEnviarArquivoClick);
		$('.btnLimparArq', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onLimparArquivoClick);

		$('.ddlAssinanteSetores', FiscalizacaoConsideracaoFinal.container).change(FiscalizacaoConsideracaoFinal.onSelecionarSetor);
		$('.ddlAssinanteCargos', FiscalizacaoConsideracaoFinal.container).change(FiscalizacaoConsideracaoFinal.onSelecionarCargo);

		$('.btnAdicionarAssinante', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onAdicionarAssinante);
		$('.btnExcluirAssinante', FiscalizacaoConsideracaoFinal.container).click(FiscalizacaoConsideracaoFinal.onExcluirAssinante);

		MasterPage.botoes(FiscalizacaoConsideracaoFinal.container);
	},
	configurarBtnEditar: function () {

		$(".btnEditar", Fiscalizacao.container).unbind('click');
		$(".btnEditar", Fiscalizacao.container).click(FiscalizacaoConsideracaoFinal.onBtnEditar);
	},
	gerarObjetoConsideracaoFinal: function () {

		var consideracaoFinal = {
			Id: $('.hdnConsideracaoFinalId', FiscalizacaoConsideracaoFinal.container).val(),
			FiscalizacaoId: $('.hdnFiscalizacaoId', Fiscalizacao.container).val(),
			Justificar: $('.txtJustificar', FiscalizacaoConsideracaoFinal.container).val().trim(),
			Descrever: $('.txtDescrever', FiscalizacaoConsideracaoFinal.container).val().trim(),
			HaReparacao: parseInt($('.rblOpinar:checked', FiscalizacaoConsideracaoFinal.container).val()),
			Reparacao: '',
			HaTermoCompromisso: parseInt($('.rblTermo:checked', FiscalizacaoConsideracaoFinal.container).val()),
			TermoCompromissoJustificar: $('.txtTermoJustificar', FiscalizacaoConsideracaoFinal.container).val().trim(),
			Arquivo: null,
			Assinantes: [],
			Testemunhas: [],
			Anexos: $('.fsArquivos', FiscalizacaoConsideracaoFinal.container).arquivo('obterObjeto')
		};

		if (consideracaoFinal.HaReparacao == 1) {
			consideracaoFinal.HaReparacao = true;
			consideracaoFinal.Reparacao = $('.txtOpinarReparacao', FiscalizacaoConsideracaoFinal.container).val().trim();
		} else if (consideracaoFinal.HaReparacao == 0) {
			consideracaoFinal.HaReparacao = false;
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
				TestemunhaEndereco: $('.txtTestemunhaEndereco', item).val().trim(),
				Colocacao: $('.hdnColocacao', item).val(),
				TestemunhaSetorId: parseInt($('.ddlSetor', item).val())
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
		$('.divTermo', FiscalizacaoConsideracaoFinal.container).addClass('hide');
		if ($(this).val().toString() != "0") {
			$('.divTermo', FiscalizacaoConsideracaoFinal.container).removeClass('hide');
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

		$('.divFuncionario, .divDadosTestemunha, .divDadosEndereco', context).addClass('hide');
		$('input[type="text"]', context).val('').removeAttr('disabled').removeClass('disabled');
		$('.ddlTestemunha, .ddlSetor', context).val(0);
		$('.ddlSetor', context).val(0).attr('disabled', 'disabled').addClass('disabled');

		if (ddlFuncIDAF.val().toString() == "1") {
			$('.divFuncionario, .divDadosEndereco', context).removeClass('hide');
			$('.txtTestemunhaEndereco', FiscalizacaoLocalInfracao.container).attr('disabled', 'disabled').addClass('disabled');
		} else if (ddlFuncIDAF.val().toString() == "2") {
			$('.divDadosTestemunha, .divDadosEndereco', context).removeClass('hide');
		}
	},
	onChangeFunc: function () {

		var value = parseInt($(this).val());
		var context = $(this).closest('fieldset');
		var txtTestemunhaEndereco = $('.txtTestemunhaEndereco', context);
		var ddlSetor = $('.ddlSetor', context);

		txtTestemunhaEndereco.val('');
		ddlSetor.val(0).attr('disabled', 'disabled').addClass('disabled')

		if (!value) {
			return;
		}

		MasterPage.carregando(true);

		$.ajax({ url: FiscalizacaoConsideracaoFinal.settings.urls.obterSetores,
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
					ddlSetor.ddlLoad(response.Setores);
					if (response.Endereco) {
						txtTestemunhaEndereco.val(response.Endereco).attr('disabled', 'disabled');
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

// 9ª Aba
FiscalizacaoFinalizar = {
	settings: {
		urls: {
			finalizar: '',
			download: '',
			pdfAuto: '',
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
	onGerarPdfLaudo: function () {
		MasterPage.redireciona(FiscalizacaoFinalizar.settings.urls.pdfLaudo + "/" + $('.hdnFiscalizacaoId', Fiscalizacao.container).val());
	}
}
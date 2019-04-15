/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../moment.min.js" />
/// <reference path="../mensagem.js" />

TituloDeclaratorio = {
	settings: {
		urls: {
			modelosCadastrados: null,
			associarRequerimento: null,
			validarAssociarRequerimento: null,
			tituloCamposModelo: null,
			obterAssinanteCargos: null,
			obterAssinanteFuncionarios: null,
			especificidade: null,
			redirecionar: null,
			salvar: null
		},
		Mensagens: null,
		obterAtividadesFunc: null,
		obterEspecificidadeObjetoFunc: null,
		especificidadeLoadCallback: null,
		especificidadeObterObjetoCallback: null,
		especificidadeErroSalvarCallback: null,
		lstFnCallBackRequerimento: new Array(),
		lstFnCallBackRequerimentoBefore: new Array(),
		carregarEspecificidade: false,
		isVisualizar: false,
		isEditar: false
	},
	container: null,

	load: function (container, options) {

		TituloDeclaratorio.container = container;
		if (options) {
			$.extend(TituloDeclaratorio.settings, options);
		}

		container.delegate('.ddlModelos', 'change', TituloDeclaratorio.onChangeModelo);
		container.delegate('.btnBuscarRequerimento', 'click', TituloDeclaratorio.associarRequerimento);
		container.delegate('.btnLimparRequerimento', 'click', TituloDeclaratorio.limparRequerimento);

		container.delegate('.ddlAssinanteSetores', 'change', TituloDeclaratorio.onSelecionarSetor);
		container.delegate('.ddlAssinanteCargos', 'change', TituloDeclaratorio.onSelecionarCargo);
		container.delegate('.ddlAssinanteCargoId', 'change', TituloDeclaratorio.onDdlAssinanteCargoChange);
		container.delegate('.btnAdicionarAssinante', 'click', TituloDeclaratorio.onAdicionarAssinante);
		container.delegate('.btnExcluirAssinante', 'click', TituloDeclaratorio.onExcluirAssinante);

		container.delegate('.btnTituloSalvar', 'click', TituloDeclaratorio.onSalvarClick);
		container.delegate('.radioCpfCnpj', 'change', TituloDeclaratorio.onChangeCpfCnpj);
		container.delegate('.btnGerar', 'click', TituloDeclaratorio.gerarRelatorio);

		if (TituloDeclaratorio.settings.isVisualizar || TituloDeclaratorio.settings.carregarEspecificidade) {
			var params = {
				Id: parseInt($('.hdnRequerimentoId', TituloDeclaratorio.container).val())
			};

			TituloDeclaratorio.carregarEspecificidade($('.ddlModelos', TituloDeclaratorio.container).val(), params);
		}

		$('.ddlSetores', container).focus();
	},

	onDdlAssinanteCargoChange: function () {
		$('.hdnAssinanteCargoId' + parseInt($(this).attr('class')), TituloDeclaratorio.container).val($(this).val());
	},

	//Associar Requerimento
	associarRequerimento: function () {
		Modal.abrir(TituloDeclaratorio.settings.urls.associarRequerimento, null, function (container) {
			RequerimentoListar.load(container, { associarFuncao: TituloDeclaratorio.callBackAssociarRequerimento });
			Modal.defaultButtons(container);
		});
	},

	callBackAssociarRequerimento: function (requerimento) {
		if ($('.txtRequerimentoNumero', TituloDeclaratorio.container).val() == requerimento.Id) {
			return true;
		}

		var retorno = MasterPage.validarAjax(TituloDeclaratorio.settings.urls.validarAssociarRequerimento, { requerimento: requerimento, modeloId: $('.ddlModelos', TituloDeclaratorio.container).val() }, null, false);

		if (!retorno.EhValido) {
			return retorno.Msg;
		}

		$('.txtRequerimentoNumero,.hdnRequerimentoId', TituloDeclaratorio.container).val(requerimento.Id);
		$('.hdnEmpreendimentoId', TituloDeclaratorio.container).val(requerimento.EmpreendimentoId);
		$('.txtEmpDenominador', TituloDeclaratorio.container).val(requerimento.EmpreendimentoDenominador);

		$('.btnLimparRequerimento', TituloDeclaratorio.container).removeClass('hide');
		$('.btnBuscarRequerimento', TituloDeclaratorio.container).addClass('hide');

		TituloDeclaratorio.runCallbackRequerimento({ Id: requerimento.Id });

		if (requerimento.EmpreendimentoId == 0) {
			Mensagem.gerar(MasterPage.getContent(TituloDeclaratorio.container), [TituloDeclaratorio.settings.Mensagens.RequerimentoSemEmpreendimento]);
		}

		return true;
	},

	limparRequerimento: function () {

		$('.hdnRequerimentoId, .txtRequerimentoNumero, .txtEmpDenominador, .hdnEmpreendimentoId', TituloDeclaratorio.container).val('');
		$('.btnBuscarRequerimento', TituloDeclaratorio.container).removeClass('hide');
		$('.btnLimparRequerimento', TituloDeclaratorio.container).addClass('hide');
		TituloDeclaratorio.runCallbackRequerimento();
	},

	runCallbackRequerimento: function (parametro) {
		$.each(TituloDeclaratorio.settings.lstFnCallBackRequerimento || [], function (i, item) {
			(typeof (item.Acao) == 'function') && item.Acao(parametro);
		});
	},

	addCallbackRequerimento: function (acao, fixo) {
		TituloDeclaratorio.settings.lstFnCallBackRequerimento.push({ Acao: acao, Fixo: fixo });
	},

	addCallbackBeforeRequerimento: function (acao, fixo) {
		TituloDeclaratorio.settings.lstFnCallBackRequerimentoBefore.push({ Acao: acao, Fixo: fixo });
	},

	clearCallbackRequerimento: function () {
		var arrayFixo = new Array();
		for (var i = 0; i < TituloDeclaratorio.settings.lstFnCallBackRequerimento.length; i++) {
			if (TituloDeclaratorio.settings.lstFnCallBackRequerimento[i].Fixo) {
				arrayFixo.push(TituloDeclaratorio.settings.lstFnCallBackRequerimento[i]);
			}
		}
		TituloDeclaratorio.settings.lstFnCallBackRequerimento = arrayFixo;

		var arrayFixoBefore = new Array();
		for (var i = 0; i < TituloDeclaratorio.settings.lstFnCallBackRequerimentoBefore.length; i++) {
			if (TituloDeclaratorio.settings.lstFnCallBackRequerimentoBefore[i].Fixo) {
				arrayFixoBefore.push(TituloDeclaratorio.settings.lstFnCallBackRequerimentoBefore[i]);
			}
		}
		TituloDeclaratorio.settings.lstFnCallBackRequerimentoBefore = arrayFixoBefore;
	},

	chamarBeforeChangesRequerimento: function (parametros) {
		var response = { EhValido: true, Msg: [], responses: [] };
		$.each(TituloDeclaratorio.settings.lstFnCallBackRequerimentoBefore || [], function (i, item) {
			if (typeof (item.Acao) == 'function') {
				var ret = item.Acao(parametros);
				response.EhValido = response.EhValido & !(ret.EhValido === false);
				(data.Msg && data.Msg.length > 0) && response.Msg.push(ret.Msg);
				response.responses.push(ret);
			}
		});
		return response;
	},

	//Selecionar Modelo
	onChangeModelo: function () {
		TituloDeclaratorio.runCallbackRequerimento(null);
		TituloDeclaratorio.clearCallbackRequerimento();
		var modeloId = $('.ddlModelos', TituloDeclaratorio.container).val();

		if (modeloId <= 0) {
			$('.tituloRequerimento', TituloDeclaratorio.container).addClass('hide');
			$('.divTituloEspContent', TituloDeclaratorio.container).addClass('hide');
			$('.tituloValoresModelo', TituloDeclaratorio.container).addClass('hide');
			return;
		}

		Mensagem.limpar(MasterPage.getContent(TituloDeclaratorio.container));

		$('.tituloRequerimento', TituloDeclaratorio.container).removeClass('hide');
		TituloDeclaratorio.carregarEspecificidade(modeloId);
		TituloDeclaratorio.camposModelo(modeloId);
	},

	camposModelo: function (modeloId) {
		//ConfiguracaoModelo
		$.ajax({
			url: TituloDeclaratorio.settings.urls.tituloCamposModelo,
			data: { modeloId: modeloId },
			cache: false,
			async: true,
			type: 'GET',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (data, textStatus, XMLHttpRequest) {
				if (data.Msg && data.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(TituloDeclaratorio.container), data.Msg, true);
					return;
				}

				$('.tituloValoresModelo', TituloDeclaratorio.container).html(data.Html);
				$('.tituloValoresModelo', TituloDeclaratorio.container).removeClass('hide');
				MasterPage.redimensionar();
				MasterPage.botoes($('.tituloValoresModelo', TituloDeclaratorio.container));
			}
		});
	},

	carregarEspecificidade: function (modeloId, requerimento) {
		var especificidade = null;
		var valor = null;

		if (requerimento) {
			valor = requerimento;
		}

		//Carrega se possui e a url da Especificidade
		$.ajax({
			url: TituloDeclaratorio.settings.urls.especificidade,
			data: { ModeloId: modeloId },
			cache: false,
			async: false,
			type: 'GET',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (data, textStatus, XMLHttpRequest) {
				especificidade = data;
			}
		});

		if (especificidade == null || !especificidade.Possui) {
			$('.divTituloEspContent', TituloDeclaratorio.container).empty();
			return;
		}

		var params = { ModeloId: modeloId, isVisualizar: TituloDeclaratorio.settings.isVisualizar };
		if (valor && valor.Id > 0) {
			params.Id = valor.Id;
		}

		if (TituloDeclaratorio.settings.carregarEspecificidade) {
			params.tituloId = $('.hdnTituloId', TituloDeclaratorio.container).val();
			TituloDeclaratorio.settings.carregarEspecificidade = false;
		} else {
			params.tituloId = 0;
		}

		params.RequerimentoId = $('.hdnRequerimentoId', TituloDeclaratorio.container).val();

		//Carrega as Especificidade
		MasterPage.carregando(true);
		$.ajax({
			url: especificidade.Url,
			data: params,
			cache: false,
			async: true,
			type: 'GET',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (data, textStatus, XMLHttpRequest) {
				if (data.Msg && data.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(TituloDeclaratorio.container), data.Msg, true);

					if (!data.EhValido) {
						return;
					}
				}

				var especificidadeContainer = $('.divTituloEspContent', TituloDeclaratorio.container);
				$('.divTituloEspContent', TituloDeclaratorio.container).removeClass('hide');
				especificidadeContainer.html(data.Html);

				if (TituloDeclaratorio.settings.especificidadeLoadCallback && typeof TituloDeclaratorio.settings.especificidadeLoadCallback == 'function') {
					TituloDeclaratorio.settings.especificidadeLoadCallback(especificidadeContainer);
				}

				Mascara.load(especificidadeContainer);
				MasterPage.botoes(especificidadeContainer);
				MasterPage.redimensionar();
			}
		});
		MasterPage.carregando(false);
	},

	//Selecionar Assinante
	onSelecionarSetor: function () {

		var ddlA = $(".ddlAssinanteSetores", TituloDeclaratorio.container);
		var ddlB = $('.ddlAssinanteCargos', TituloDeclaratorio.container);
		var ddlC = $('.ddlAssinanteFuncionarios', TituloDeclaratorio.container);

		var modeloId = $('.ddlModelos', TituloDeclaratorio.container).val();
		var setorId = $('.ddlAssinanteSetores', TituloDeclaratorio.container).val();

		ddlA.ddlCascate(ddlB, { url: TituloDeclaratorio.settings.urls.obterAssinanteCargos, data: { modeloId: modeloId} });
		ddlB.ddlCascate(ddlC, { url: TituloDeclaratorio.settings.urls.obterAssinanteFuncionarios, data: { modeloId: modeloId, setorId: setorId} });

	},

	onSelecionarCargo: function () {

		var ddlA = $(".ddlAssinanteCargos", TituloDeclaratorio.container);
		var ddlB = $('.ddlAssinanteFuncionarios', TituloDeclaratorio.container);

		var modeloId = $('.ddlModelos', TituloDeclaratorio.container).val();
		var setorId = $('.ddlAssinanteSetores', TituloDeclaratorio.container).val();

		ddlA.ddlCascate(ddlB, { url: TituloDeclaratorio.settings.urls.obterAssinanteFuncionarios, data: { modeloId: modeloId, setorId: setorId} });
	},

	onAdicionarAssinante: function () {

		var mensagens = new Array();
		Mensagem.limpar(TituloDeclaratorio.container);
		var container = $('.fdsAssinante', TituloDeclaratorio.container);

		var item = {
			SetorId: $('.ddlAssinanteSetores :selected', container).val(),
			FuncionarioNome: $('.ddlAssinanteFuncionarios :selected', container).text(),
			FuncionarioId: $('.ddlAssinanteFuncionarios :selected', container).val(),
			FuncionarioCargoNome: $('.ddlAssinanteCargos :selected', container).text(),
			FuncionarioCargoId: $('.ddlAssinanteCargos :selected', container).val()
		};

		if (jQuery.trim(item.SetorId) == '0') {
			mensagens.push(jQuery.extend(true, {}, TituloDeclaratorio.settings.Mensagens.AssinanteSetorObrigatorio));
		}

		if (jQuery.trim(item.FuncionarioCargoId) == '0') {
			mensagens.push(jQuery.extend(true, {}, TituloDeclaratorio.settings.Mensagens.AssinanteCargoObrigatorio));
		}

		if (jQuery.trim(item.FuncionarioId) == '0') {
			mensagens.push(jQuery.extend(true, {}, TituloDeclaratorio.settings.Mensagens.AssinanteFuncionarioObrigatorio));
		}

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var itemAdd = (JSON.parse(obj));
				if (item.FuncionarioId == itemAdd.FuncionarioId && item.FuncionarioCargoId == itemAdd.FuncionarioCargoId) {
					mensagens.push(jQuery.extend(true, {}, TituloDeclaratorio.settings.Mensagens.AssinanteJaAdicionado));
				}
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(TituloDeclaratorio.container, mensagens);
			return;
		}

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(item));
		linha.find('.Funcionario').html(item.FuncionarioNome).attr('title', item.FuncionarioNome);
		linha.find('.Cargo').html(item.FuncionarioCargoNome).attr('title', item.FuncionarioCargoNome);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.ddlAssinanteSetores', container).ddlFirst();
		TituloDeclaratorio.onSelecionarSetor();
	},

	onExcluirAssinante: function () {
		var container = $('.fdsAssinante');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	},

	//Salvar Titulo
	ObterObjeto: function () {
		var titulo = {};

		titulo.Id = parseInt($('.hdnTituloId', TituloDeclaratorio.container).val()) || 0;
		titulo.Situacao = { Id: parseInt($('.hdnTituloSituacao', TituloDeclaratorio.container).val()) || 0 };
		titulo.LocalEmissao = { Id: $('.ddlLocal', TituloDeclaratorio.container).val() };
		titulo.Modelo = { Id: $('.ddlModelos', TituloDeclaratorio.container).val(), TipoDocumento: 2 };
		titulo.Numero = { Texto: $('.txtNumero', TituloDeclaratorio.container).val() };

		titulo.RequerimetoId = $('.hdnRequerimentoId', TituloDeclaratorio.container).val();

		titulo.EmpreendimentoId = $('.hdnEmpreendimentoId', TituloDeclaratorio.container).val();

		titulo.Atividades = typeof TituloDeclaratorio.settings.obterAtividadesFunc == 'function' ? TituloDeclaratorio.settings.obterAtividadesFunc() : [];

		titulo.Especificidade = { Json: JSON.stringify(typeof TituloDeclaratorio.settings.obterEspecificidadeObjetoFunc == 'function' ? TituloDeclaratorio.settings.obterEspecificidadeObjetoFunc(TituloDeclaratorio.container) : null) };

		titulo.Assinantes = [];
		var assinantesContainer = TituloDeclaratorio.container.find('.fdsAssinante');
		$('.hdnItemJSon', assinantesContainer).each(function () {
			var objAssinante = String($(this).val());
			if (objAssinante != '') {
				titulo.Assinantes.push(JSON.parse(objAssinante));
			}
		});

		return titulo;
	},

	validarFunction: function (funcao, objetoDefaul) {
		return typeof funcao == 'function' ? funcao(TituloDeclaratorio.container) : objetoDefaul;
	},

	onSalvarClick: function () {
		Mensagem.limpar(TituloDeclaratorio.container);
		MasterPage.carregando(true);
		var objParam = { Titulo: TituloDeclaratorio.ObterObjeto() };
		//ConfiguracaoModelo
		$.ajax({
			url: TituloDeclaratorio.settings.urls.salvar,
			data: JSON.stringify(objParam),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, TituloDeclaratorio.container);
				MasterPage.carregando(false);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					MasterPage.redireciona(response.UrlSucesso);
				} else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(TituloDeclaratorio.container), response.Msg);

					if (TituloDeclaratorio.settings.especificidadeErroSalvarCallback && typeof TituloDeclaratorio.settings.especificidadeErroSalvarCallback == 'function') {
						TituloDeclaratorio.settings.especificidadeErroSalvarCallback(response.Msg);
					}
				}

				MasterPage.carregando(false);
			}
		});

	},

	//Relatorio
	onChangeCpfCnpj: function () {
		if ($('.radioCpfCnpj:checked').val() == 1)
			$('.filterCpfCnpj').removeClass('maskCnpj').addClass('maskCpf');
		else
			$('.filterCpfCnpj').removeClass('maskCpf').addClass('maskCnpj');

		$('.filterCpfCnpj').val('');
		Mascara.load(TituloDeclaratorio.container);
	},

	gerarRelatorio: function () {
		filtro = {
			modelo: $('.filterModelo:visible').val(),
			inicioPeriodo: $('.filterInicioPeriodo').val(),
			fimPeriodo: $('.filterFImPeriodo').val(),
			nomeRazaoSocial: $('.filterNome').val(),
			cpfCnpj: $('.filterCpfCnpj').val(),
			isCpf: $('.radioCpfCnpj:checked').val() == 1 ? true : false,
			municipio: $('.filterMunicipio:visible').val()
		};

		if (!filtro.inicioPeriodo.isNullOrWhitespace() && filtro.fimPeriodo.isNullOrWhitespace() ||
			filtro.inicioPeriodo.isNullOrWhitespace() && !filtro.fimPeriodo.isNullOrWhitespace()) {
			Mensagem.gerar(TituloDeclaratorio.container, [TituloDeclaratorio.settings.Mensagens.PeriodoObrigatorio]);
			return;
		}

		if (!filtro.inicioPeriodo.isNullOrWhitespace() && !filtro.fimPeriodo.isNullOrWhitespace()) {
			if (filtro.inicioPeriodo.length < 10 || filtro.fimPeriodo.length < 10) {
				Mensagem.gerar(TituloDeclaratorio.container, [TituloDeclaratorio.settings.Mensagens.PeriodoFormato]);
				return;
			}

			if (!moment(filtro.inicioPeriodo).isValid() || !moment(filtro.fimPeriodo).isValid()) {
				Mensagem.gerar(TituloDeclaratorio.container, [TituloDeclaratorio.settings.Mensagens.PeriodoInvalido]);
				return;
			}
		}

		window.open(TituloDeclaratorio.settings.urls.urlGerar + '?paramsJson=' + JSON.stringify(filtro));
	}
}
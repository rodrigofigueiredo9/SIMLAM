/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

Titulo = {
	settings: {
		urls: {
			modelosCadastrados: '',
			associarProcesso: '',
			associarDocumento: '',
			validarAssociarProcDoc: '',
			obterProtocolo: '',
			associarEmpreendimento: '',
			especificidade: '',
			tituloProtocolo: '',
			tituloCamposModelo: '',
			salvar: '',
			redirecionar: '',
			obterDestinatarioEmails: '',
			enviarArquivo: '',
			obterAssinanteCargos: '',
			obterAssinanteFuncionarios: ''
		},
		Mensagens: null,
		obterRepresentanteFunc: null,
		obterAtividadesFunc: null,
		obterCondicionantesFunc: null,
		obterEspecificidadeObjetoFunc: null,
		obterRequerimentoFunc: null,
		especificidadeLoadCallback: null,
		especificidadeObterObjetoCallback: null,
		especificidadeErroSalvarCallback: null,
		obterTitulosAssociadoCallback: null,
		onChangeProcDocEmp: new Array(),
		onBeforeChangeProcDocEmp: new Array(),
		procDocContemEmp: false,
		carregarEspecificidade: false,
		protocoloSelecionado: null,
		protocolo: null,
		isVisualizar: false,
		isEditar: false
	},
	container: null,

	load: function (container, options) {

		Titulo.container = container;
		if (options) {
			$.extend(Titulo.settings, options);
		}

		container.delegate('.btnBuscarProtocolo', 'click', Titulo.onAssociarProtocoloClick);
		container.delegate('.rdbOpcaoBuscaProtocolo', 'change', Titulo.onChangeRdbOpcaoBusca);
		container.delegate('.btnBuscarEmpreendimento', 'click', Titulo.onAssociarEmpreendimento);
		container.delegate('.btnLimparNumero', 'click', Titulo.onLimparAssociar);

		container.delegate('.ddlSetores', 'change', Titulo.onDdlSetoresCadastro);
		container.delegate('.ddlModelos', 'change', Titulo.onChangeModelo);
		container.delegate('.ddlAssinanteCargoId', 'change', Titulo.onDdlAssinanteCargoChange);

		container.delegate('.btnArqComplementar', 'click', Titulo.onEnviarArquivoClick);
		container.delegate('.btnArqComplementarLimpar', 'click', Titulo.onLimparArquivoClick);

		container.delegate('.btnTituloSalvar', 'click', Titulo.onSalvarClick);

		container.delegate('.ddlAssinanteSetores', 'change', Titulo.onSelecionarSetor);
		container.delegate('.ddlAssinanteCargos', 'change', Titulo.onSelecionarCargo);

		container.delegate('.btnAdicionarAssinante', 'click', Titulo.onAdicionarAssinante);
		container.delegate('.btnExcluirAssinante', 'click', Titulo.onExcluirAssinante);

		if (Titulo.settings.isVisualizar || Titulo.settings.carregarEspecificidade) {
			var params = {
				Id: parseInt($('.hdnProtocoloId', Titulo.container).val()),
				IsProcesso: (parseInt($('.rdbOpcaoBuscaProtocolo:checked', Titulo.container).val()) == 1),
				EmpreendimentoId: parseInt($('.hdnEmpreendimentoId', Titulo.container).val())
			};
			if ($('.hdnProtocoloIsProcesso').length > 0) {
				params.IsProcesso = $('.hdnProtocoloIsProcesso').val();
			}
			Titulo.carregarEspecificidade($('.ddlModelos', Titulo.container).val(), params);
		}

		$('.ddlSetores', container).focus();
	},

	VisualizarLoad: function (container, options) {
		Titulo.container = container;
		if (options) {
			$.extend(Titulo.settings, options);
		}
		if (Titulo.settings.carregarEspecificidade) {
			Titulo.carregarEspecificidade($('.ddlModelos', Titulo.container).val(), {
				Id: parseInt($('.hdnProtocoloId', Titulo.container).val()),
				IsProcesso: ($('.hdnProtocoloIsProcesso', Titulo.container).val() == "True")
			});
		}
	},

	onDdlAssinanteCargoChange: function () {
		$('.hdnAssinanteCargoId' + parseInt($(this).attr('class')), Titulo.container).val($(this).val());
	},

	onDdlSetoresCadastro: function () {

		var ddlA = $(this, Titulo.container);
		var ddlB = $('.ddlModelos', Titulo.container);

		ddlA.ddlCascate(ddlB, { url: Titulo.settings.urls.modelosCadastrados });
		$('.tituloProtocolo, .divTituloEspContent, .tituloValoresModelo', Titulo.container).empty();

		Titulo.onChangeModelo();
	},

	//Associar Processo/Documento
	onChangeRdbOpcaoBusca: function () {

		if ($('.hdnProtocoloId').val() > 0) {
			Titulo.onLimparAssociar();
		} else {
			$('.hdnProtocoloId', Titulo.container).val('');
			$('.txtProtocoloNumero', Titulo.container).val('');
		}
	},

	onAssociarProtocoloClick: function () {
		var isProcesso = parseInt($('.rdbOpcaoBuscaProtocolo:checked', Titulo.container).val()) == 1;

		if (isProcesso) {
			Modal.abrir(Titulo.settings.urls.associarProcesso, null, function (container) {
				ProcessoListar.load(container, { associarFuncao: Titulo.callBackAssociarProtocolo });
				Modal.defaultButtons(container);
			});
		} else {
			Modal.abrir(Titulo.settings.urls.associarDocumento, null, function (container) {
				DocumentoListar.load(container, { associarFuncao: Titulo.callBackAssociarProtocolo });
				Modal.defaultButtons(container);
			});
		}
	},

	callBackAssociarProtocolo: function (protocolo) {
		var params = {
			id: protocolo.Id,
			setorId: $('.ddlSetores', Titulo.container).val(),
			isProcesso: parseInt($('.rdbOpcaoBuscaProtocolo:checked', Titulo.container).val()) == 1,
			modeloId: $('.ddlModelos', Titulo.container).val()
		};

		var retorno = MasterPage.validarAjax(Titulo.settings.urls.validarAssociarProcDoc, params, null, false);

		if (!retorno.EhValido) {
			return retorno.Msg;
		}

		// callbacks de validações impeditivas sobre mudanças de protocolo (por exemplo: Uma especificidade que obrigue interessado do processo ser jurídico)
		retorno = Titulo.chamarBeforeChangesProcDocEmp(params);
		if (!retorno.EhValido) {
			return retorno.Msg;
		}

		protocolo.IsProcesso = params.isProcesso;
		$('.hdnProtocoloId', Titulo.container).val(protocolo.Id);
		$('.txtProtocoloNumero', Titulo.container).val(protocolo.Numero);

		var mensagensInformacao = Titulo.mostrarDestinatarioEmails(protocolo, params.modeloId);
		protocolo.EmpreendimentoId = 0;

		//Obter Processo/Documento
		$.ajax({ url: Titulo.settings.urls.obterProtocolo,
			data: JSON.stringify(params),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Titulo.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Objeto.EmpreendimentoId > 0) {
					$('.hdnEmpreendimentoId', Titulo.container).val(response.Objeto.EmpreendimentoId);
					$('.txtEmpDenominador', Titulo.container).val(response.Objeto.EmpreendimentoNome);
					protocolo.EmpreendimentoId = response.Objeto.EmpreendimentoId;
					Titulo.settings.procDocContemEmp = true;
				} else {
					if (mensagensInformacao) {
						Mensagem.gerar(MasterPage.getContent(Titulo.container), mensagensInformacao);
					}

					var isProcesso = (parseInt($('.rdbOpcaoBuscaProtocolo:checked', Titulo.container).val()) == 1);
					var txtMsg = Titulo.settings.Mensagens.ProcDocSemEmpAssociado.Texto.replace('processo/documento', (isProcesso) ? 'processo' : 'documento');
					$('.txtEmpDenominador', Titulo.container).val('[' + txtMsg + ']');
				}
				protocolo.Representantes = response.Objeto.Representantes;
				$('.divBotoesEmp').addClass('hide');
			}
		});

		$('.hdnLicencaJSON', Titulo.container).val('');
		Titulo.chamarChangesProcDocEmp(protocolo);
		$('.divBotoesProtocolo', Titulo.container).find('.btnLimparContainer').removeClass('hide');

		return retorno.EhValido;
	},

	mostrarDestinatarioEmails: function (Protocolo, modeloId) {
		var mensagemInformacao = null;

		$.ajax({ url: Titulo.settings.urls.obterDestinatarioEmails,
			data: JSON.stringify({ protocolo: Protocolo, modeloId: modeloId }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Titulo.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				$('.divDestinatariosContainer', Titulo.containe).empty();
				$('.divDestinatarioEmail', Titulo.containe).toggleClass('hide', !response.IsEnviarEmail);
				$('.divNaoExisteDestinatario', Titulo.container).toggleClass('hide', response.Destinatarios.length > 0);

				if (!response.EhValido) {
					Mensagem.gerar(MasterPage.getContent(Titulo.container), response.Msg);
					return;
				} else if (response.Msg && response.Msg.length > 0) {
					mensagemInformacao = response.Msg;
				}

				$.each(response.Destinatarios, function () {
					var destinatario = this;
					var elem = $('.destinatarioEmailTemplate', Titulo.container).clone();
					elem.removeClass('hide destinatarioEmailTemplate');
					$('.checkDestinatarioEmail', elem).val(destinatario.PessoaId);
					$('.destinatarioEmailText', elem).text(destinatario.PessoaNome);
					$('.divDestinatariosContainer', Titulo.container).append(elem.contents());
				});
			}
		});

		return mensagemInformacao;
	},

	//Associar Empreendimento
	onAssociarEmpreendimento: function () {
		Modal.abrir(Titulo.settings.urls.associarEmpreendimento, null, function (container) {
			EmpreendimentoListar.load(container, { associarFuncao: Titulo.callBackAssociarEmpreendimento });
			Modal.defaultButtons(container);
		});
	},

	callBackAssociarEmpreendimento: function (Empreendimento) {
		$('.hdnEmpreendimentoId', Titulo.container).val(Empreendimento.Id);
		$('.txtEmpDenominador', Titulo.container).val(Empreendimento.Denominador);
		$('.divBotoesEmp', Titulo.container).find('.btnLimparContainer').removeClass('hide');
		$('.btnBuscarProtocolo', Titulo.container).addClass('hide');

		Titulo.chamarChangesProcDocEmp({ Id: 0, IsProcesso: false, EmpreendimentoId: Empreendimento.Id });
		return true;
	},

	onLimparAssociar: function () {
		$('.hdnProtocoloId,.txtProtocoloNumero,.txtEmpDenominador', Titulo.container).val('');
		$('.btnLimparContainer', Titulo.container).addClass('hide');

		if ($('.txtProtocoloNumero', Titulo.container).val() == '') {
			Titulo.chamarChangesProcDocEmp(null);
		}

		if (Titulo.settings.procDocContemEmp) {
			$('.divBotoesEmp', Titulo.container).find('.btnLimparContainer').addClass('hide');
			$('.divEmpTitulo', Titulo.container).find('input[type=text], input[type=hidden]').val('');
			Titulo.settings.procDocContemEmp = false;
		}
		$('.txtEmpDenominador', Titulo.container).val('');

		$('.btnBuscarProtocolo', Titulo.container).removeClass('hide');
		$('.divBotoesEmp', Titulo.container).removeClass('hide');
		$('.hdnLicencaJSON', Titulo.container).val('');
	},

	//Selecionar Modelo
	onChangeModelo: function () {
		Titulo.chamarChangesProcDocEmp(null);
		Titulo.clearCallbackProtocolo();
		var modeloId = $('.ddlModelos', Titulo.container).val();

		if (modeloId <= 0) {
			$('.tituloProtocolo', Titulo.container).addClass('hide');
			$('.divTituloEspContent', Titulo.container).addClass('hide');
			$('.tituloValoresModelo', Titulo.container).addClass('hide');
			return;
		}

		Mensagem.limpar(MasterPage.getContent(Titulo.container));
		$('.divDestinatarioEmail', Titulo.container).addClass('hide');

		Titulo.protocolo(modeloId);
		Titulo.carregarEspecificidade(modeloId);
		Titulo.camposModelo(modeloId);
	},

	addCallbackProtocolo: function (acao, fixo) {
		Titulo.settings.onChangeProcDocEmp.push({ Acao: acao, Fixo: fixo });
	},

	addCallbackBeforeProtocolo: function (acao, fixo) {
		Titulo.settings.onBeforeChangeProcDocEmp.push({ Acao: acao, Fixo: fixo });
	},

	clearCallbackProtocolo: function () {
		var arrayFixo = new Array();
		for (var i = 0; i < Titulo.settings.onChangeProcDocEmp.length; i++) {
			if (Titulo.settings.onChangeProcDocEmp[i].Fixo) {
				arrayFixo.push(Titulo.settings.onChangeProcDocEmp[i]);
			}
		}
		Titulo.settings.onChangeProcDocEmp = arrayFixo;

		var arrayFixoBefore = new Array();
		for (var i = 0; i < Titulo.settings.onBeforeChangeProcDocEmp.length; i++) {
			if (Titulo.settings.onBeforeChangeProcDocEmp[i].Fixo) {
				arrayFixoBefore.push(Titulo.settings.onBeforeChangeProcDocEmp[i]);
			}
		}
		Titulo.settings.onBeforeChangeProcDocEmp = arrayFixoBefore;
	},

	chamarChangesProcDocEmp: function (parametro) {
		$.each(Titulo.settings.onChangeProcDocEmp || [], function (i, item) {
			(typeof (item.Acao) == 'function') && item.Acao(parametro);
		});
	},

	chamarBeforeChangesProcDocEmp: function (parametros) {
		var response = { EhValido: true, Msg: [], responses: [] };
		$.each(Titulo.settings.onBeforeChangeProcDocEmp || [], function (i, item) {
			if (typeof (item.Acao) == 'function') {
				var ret = item.Acao(parametros);
				response.EhValido = response.EhValido & !(ret.EhValido === false);
				(data.Msg && data.Msg.length > 0) && response.Msg.push(ret.Msg);
				response.responses.push(ret);
			}
		});
		return response;
	},

	protocolo: function (modeloId) {
		//ConfiguracaoModelo
		$.ajax({ url: Titulo.settings.urls.tituloProtocolo,
			data: { modeloId: modeloId },
			cache: false,
			async: true,
			type: 'GET',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Titulo.container);
			},
			success: function (data, textStatus, XMLHttpRequest) {

				if (data.Msg && data.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Titulo.container), data.Msg, true);
					return;
				}

				$('.tituloProtocolo', Titulo.container).html(data.Html);
				$('.tituloProtocolo', Titulo.container).removeClass('hide');
				MasterPage.redimensionar();
				MasterPage.botoes($('.tituloProtocolo', Titulo.container));
				Mascara.load($('.tituloProtocolo', Titulo.container));
			}
		});
	},

	carregarEspecificidade: function (modeloId, protocolo) {
		var especificidade = null;
		var valor = null;

		if (protocolo) {
			valor = protocolo;
		}

		//Carrega se possui e a url da Especificidade
		$.ajax({ url: Titulo.settings.urls.especificidade,
			data: { ModeloId: modeloId },
			cache: false,
			async: false,
			type: 'GET',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Titulo.container);
			},
			success: function (data, textStatus, XMLHttpRequest) {
				especificidade = data;
			}
		});

		if (especificidade == null || !especificidade.Possui) {
			$('.divTituloEspContent', Titulo.container).empty();
			return;
		}

		var params = { ModeloId: modeloId, isVisualizar: Titulo.settings.isVisualizar };
		if (valor && valor.Id > 0) {
			params.ProtocoloId = valor.Id;
			params.IsProcesso = valor.IsProcesso;
			params.EmpreendimentoId = valor.EmpreendimentoId;
			params.AtividadeProcDocReqKey = Titulo.settings.protocoloSelecionado;
		}

		if (Titulo.settings.carregarEspecificidade) {
			params.tituloId = $('.hdnTituloId', Titulo.container).val();
			Titulo.settings.carregarEspecificidade = false;
		} else {
			params.tituloId = 0;
		}

		//Carrega as Especificidade
		$.ajax({ url: especificidade.Url,
			data: params,
			cache: false,
			async: true,
			type: 'GET',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Titulo.container);
			},
			success: function (data, textStatus, XMLHttpRequest) {
				if (data.Msg && data.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Titulo.container), data.Msg, true);

					if (!data.EhValido) {
						return;
					}
				}

				var especificidadeContainer = $('.divTituloEspContent', Titulo.container);
				$('.divTituloEspContent', Titulo.container).removeClass('hide');
				especificidadeContainer.html(data.Html);

				if (Titulo.settings.especificidadeLoadCallback && typeof Titulo.settings.especificidadeLoadCallback == 'function') {
					Titulo.settings.especificidadeLoadCallback(especificidadeContainer);
				}

				Mascara.load(especificidadeContainer);
				MasterPage.botoes(especificidadeContainer);
				MasterPage.redimensionar();
			}
		});
	},

	camposModelo: function (modeloId) {
		var setorId = parseInt($('.ddlSetores', Titulo.container).val());
		//ConfiguracaoModelo
		$.ajax({ url: Titulo.settings.urls.tituloCamposModelo,
			data: { modeloId: modeloId, setorId: setorId },
			cache: false,
			async: true,
			type: 'GET',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Titulo.container);
			},
			success: function (data, textStatus, XMLHttpRequest) {

				if (data.Msg && data.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Titulo.container), data.Msg, true);
					return;
				}

				$('.tituloValoresModelo', Titulo.container).html(data.Html);
				$('.tituloValoresModelo', Titulo.container).removeClass('hide');
				MasterPage.redimensionar();
				MasterPage.botoes($('.tituloValoresModelo', Titulo.container));
			}
		});
	},

	//--------------- ENVIAR ARQUIVO ---------------\\
	onEnviarArquivoClick: function () {
		var nomeArquivo = $('.inputFileArqComplementar', Titulo.container).val();

		if (nomeArquivo === '') {
			Mensagem.gerar(MasterPage.getContent(Titulo.container), new Array(Titulo.settings.Mensagens.ArquivoObrigatorio));
			return;
		}

		if (nomeArquivo.trim().toLowerCase().split('.').pop() != 'pdf') {
			Mensagem.gerar(Titulo.container, new Array(Titulo.settings.Mensagens.ArquivoTipoPdf));
			return;
		}

		$('.btnArqComplementar', Titulo.container).button({ disabled: true });
		var inputFile = $('.inputFileArqComplementar', Titulo.container);
		FileUpload.upload(Titulo.settings.urls.enviarArquivo, inputFile, Titulo.callBackArqEnviado);
	},

	callBackArqEnviado: function (controle, retorno, isHtml) {
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoNome', Titulo.container).val(ret.Arquivo.Nome);
			$('.hdnArquivoJson', Titulo.container).val(JSON.stringify(ret.Arquivo));

			$('.spanInputFile', Titulo.container).addClass('hide');
			$('.txtArquivoNome', Titulo.container).removeClass('hide');

			$('.btnArqComplementar', Titulo.container).addClass('hide');
			$('.btnArqComplementarLimpar', Titulo.container).removeClass('hide');
		} else {
			Titulo.onLimparArquivoClick();
		}

		Mensagem.gerar(MasterPage.getContent(Titulo.container), ret.Msg);
		$('.btnArqComplementar', Titulo.container).button({ disabled: false });
	},

	onLimparArquivoClick: function () {
		$('.hdnArquivoJson', Titulo.container).val('');
		$('.inputFileArqComplementar', Titulo.container).val('');

		$('.spanInputFile', Titulo.container).removeClass('hide');
		$('.txtArquivoNome', Titulo.container).addClass('hide');

		$('.btnArqComplementar', Titulo.container).removeClass('hide');
		$('.btnArqComplementarLimpar', Titulo.container).addClass('hide');
		$('.lnkArquivo', Titulo.container).remove();
	},

	//Salvar Titulo
	ObterObjeto: function () {
		var titulo = {};

		titulo.Id = parseInt($('.hdnTituloId', Titulo.container).val()) || 0;
		titulo.Situacao = { Id: parseInt($('.hdnTituloSituacao', Titulo.container).val()) || 0 };
		titulo.Setor = { Id: $('.ddlSetores', Titulo.container).val() };
		titulo.LocalEmissao = { Id: $('.ddlLocal', Titulo.container).val() };
		titulo.Modelo = { Id: $('.ddlModelos', Titulo.container).val() };
		titulo.Numero = { Texto: $('.txtNumero', Titulo.container).val() };
		titulo.Protocolo = {
			Id: $('.hdnProtocoloId', Titulo.container).val(),
			IsProcesso: (parseInt($('.rdbOpcaoBuscaProtocolo:checked', Titulo.container).val()) == 1),
			NumeroProtocolo: parseInt($('.txtProtocoloNumero', Titulo.container).val().split('/')[0]),
			Ano: parseInt($('.txtProtocoloNumero', Titulo.container).val().split('/')[1])
		};

		titulo.RequerimentoAtividades = typeof Titulo.settings.obterRequerimentoFunc == 'function' ? Titulo.settings.obterRequerimentoFunc() : 0;

		titulo.EmpreendimentoId = $('.hdnEmpreendimentoId', Titulo.container).val();

		titulo.Atividades = typeof Titulo.settings.obterAtividadesFunc == 'function' ? Titulo.settings.obterAtividadesFunc() : [];

		titulo.CondicionantesJson = typeof Titulo.settings.obterCondicionantesFunc == 'function' ? Titulo.settings.obterCondicionantesFunc(Titulo.container) : [];

		titulo.Especificidade = { Json: JSON.stringify(typeof Titulo.settings.obterEspecificidadeObjetoFunc == 'function' ? Titulo.settings.obterEspecificidadeObjetoFunc(Titulo.container) : null) };

		titulo.Representante = typeof Titulo.settings.obterRepresentanteFunc == 'function' ? Titulo.settings.obterRepresentanteFunc(Titulo.container) : {};

		titulo.Associados = Titulo.validarFunction(Titulo.settings.obterTitulosAssociadoCallback, new Array());

		titulo.Anexos = Titulo.validarFunction(Titulo.settings.obterAnexosCallback, new Array());

		titulo.ArquivoPdf = $.parseJSON($('.hdnArquivoJson', Titulo.container).val());

		titulo.Assinantes = [];
		var assinantesContainer = Titulo.container.find('.fdsAssinante');
		$('.hdnItemJSon', assinantesContainer).each(function () {
			var objAssinante = String($(this).val());
			if (objAssinante != '') {
				titulo.Assinantes.push(JSON.parse(objAssinante));
			}
		});

		titulo.DestinatarioEmails = [];
		$('.checkDestinatarioEmail:checked', Titulo.container).each(function () {
			titulo.DestinatarioEmails.push({ PessoaId: $(this).val(), PessoaNome: $(this).parent().find(".destinatarioEmailText").text() });
		});

		return titulo;
	},

	validarFunction: function (funcao, objetoDefaul) {
		return typeof funcao == 'function' ? funcao(Titulo.container) : objetoDefaul;
	},

	onSalvarClick: function () {
		Mensagem.limpar(Titulo.container);
		var objParam = { Titulo: Titulo.ObterObjeto() };
		//ConfiguracaoModelo
		$.ajax({ url: Titulo.settings.urls.salvar,
			data: JSON.stringify(objParam),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Titulo.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					MasterPage.redireciona(response.UrlSucesso);
				} else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Titulo.container), response.Msg);

					if (Titulo.settings.especificidadeErroSalvarCallback && typeof Titulo.settings.especificidadeErroSalvarCallback == 'function') {
						Titulo.settings.especificidadeErroSalvarCallback(response.Msg);
					}
				}
			}
		});
		MasterPage.carregando(false);
	},

	//Selecionar Assinante 
	onSelecionarSetor: function () {

		var ddlA = $(".ddlAssinanteSetores", Titulo.container);
		var ddlB = $('.ddlAssinanteCargos', Titulo.container);
		var ddlC = $('.ddlAssinanteFuncionarios', Titulo.container);

		var modeloId = $('.ddlModelos', Titulo.container).val();
		var setorId = $('.ddlAssinanteSetores', Titulo.container).val();

		ddlA.ddlCascate(ddlB, { url: Titulo.settings.urls.obterAssinanteCargos, data: { modeloId: modeloId} });
		ddlB.ddlCascate(ddlC, { url: Titulo.settings.urls.obterAssinanteFuncionarios, data: { modeloId: modeloId, setorId: setorId} });

	},

	onSelecionarCargo: function () {

		var ddlA = $(".ddlAssinanteCargos", Titulo.container);
		var ddlB = $('.ddlAssinanteFuncionarios', Titulo.container);

		var modeloId = $('.ddlModelos', Titulo.container).val();
		var setorId = $('.ddlAssinanteSetores', Titulo.container).val();

		ddlA.ddlCascate(ddlB, { url: Titulo.settings.urls.obterAssinanteFuncionarios, data: { modeloId: modeloId, setorId: setorId} });
	},

	onAdicionarAssinante: function () {

		var mensagens = new Array();
		Mensagem.limpar(Titulo.container);
		var container = $('.fdsAssinante', Titulo.container);

		var item = {
			SetorId: $('.ddlAssinanteSetores :selected', container).val(),
			FuncionarioNome: $('.ddlAssinanteFuncionarios :selected', container).text(),
			FuncionarioId: $('.ddlAssinanteFuncionarios :selected', container).val(),
			FuncionarioCargoNome: $('.ddlAssinanteCargos :selected', container).text(),
			FuncionarioCargoId: $('.ddlAssinanteCargos :selected', container).val()
		};

		if (jQuery.trim(item.SetorId) == '0') {
			mensagens.push(jQuery.extend(true, {}, Titulo.settings.Mensagens.AssinanteSetorObrigatorio));
		}

		if (jQuery.trim(item.FuncionarioCargoId) == '0') {
			mensagens.push(jQuery.extend(true, {}, Titulo.settings.Mensagens.AssinanteCargoObrigatorio));
		}

		if (jQuery.trim(item.FuncionarioId) == '0') {
			mensagens.push(jQuery.extend(true, {}, Titulo.settings.Mensagens.AssinanteFuncionarioObrigatorio));
		}

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var itemAdd = (JSON.parse(obj));
				if (item.FuncionarioId == itemAdd.FuncionarioId && item.FuncionarioCargoId == itemAdd.FuncionarioCargoId) {
					mensagens.push(jQuery.extend(true, {}, Titulo.settings.Mensagens.AssinanteJaAdicionado));
				}
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(Titulo.container, mensagens);
			return;
		}

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(item));
		linha.find('.Funcionario').html(item.FuncionarioNome).attr('title', item.FuncionarioNome);
		linha.find('.Cargo').html(item.FuncionarioCargoNome).attr('title', item.FuncionarioCargoNome);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.ddlAssinanteSetores', container).ddlFirst();
		Titulo.onSelecionarSetor();

	},

	onExcluirAssinante: function () {
		var container = $('.fdsAssinante');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	}
}
/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />

Empreendimento = {
	settings: {
		modoInline: false,
		callBackVisualizar: null,
		onResponsavelAssociarClick: null,
		onAtividadeAssociarClick: null,
		objetoEmpreendimento: null,
		container: {},
		urls: {
			avancar: '',
			voltar: '',
			salvarCadastrar: '',
			editar: '',
			verificarCnpj: '',
			visualizar: '',
			associarResponsavelModal: '',
			associarResponsavelEditarModal: '',
			associarAtividadeModal: '',
			enderecoMunicipio: '',
			pessoaAssociar: '',
			verificarCnpjEmpreendimento: '',
			coordenadaGeo: '',
			obterMunicipioPorCoordenada: '',
			obterEstadosMunicipiosPorCodIbge: ''
		},
		msgs: {},
		denominadoresSegmentos: null
	},

	modalPessoaResp: null,

	load: function (content, options) {

		if (options) {
			$.extend(Empreendimento.settings, options);
		}
		Empreendimento.settings.container = content;

		if ($('.modalLocalizarEmpreendimento', content).length > 0) {
			EmpreendimentoLocalizar.load();
			Empreendimento.settings.container.listarAjax({
				onBeforeSerializar: EmpreendimentoLocalizar.onBeforeSerializar,
				onAfterFiltrar: EmpreendimentoLocalizar.onAfterFiltrar,
				onBeforeFiltrar: EmpreendimentoLocalizar.onBeforeFiltrar,
				mensagemContainer: MasterPage.getContent(Empreendimento.settings.container)
			});
		} else if ($('.modalSalvarEmpreendimento', content).length > 0) {
			EmpreendimentoSalvar.load();
		}

		content.delegate('.btnBuscarCoordenada', 'click', Empreendimento.onBuscarCoordenada);
		content.delegate('.btnLimparAtividade', 'click', Empreendimento.onAtividadeLimparClick);
		content.delegate('.btnCopiarInterno', 'click', Empreendimento.onCopiarIdaf);
	},

	onCopiarIdaf: function (content) {
		Modal.confirma({
			btnOkLabel: 'Confirmar',
			titulo: 'Copiar?',
			conteudo: 'Ao confirmar a cópia de dados do IDAF, todos os campos da tela serão substituídos pelos dados que estão cadastrados na base de dados do IDAF. Tem certeza que deseja efetuar a cópia?',
			tamanhoModal: Modal.tamanhoModalMedia,
			btnOkCallback: function (content) {
				Empreendimento.copiarIdaf();
				
				Modal.fechar(content);
			}
		});
		
	},

	copiarIdaf: function () {
		var content = $('.empreendimentoPartial', MasterPage.getContent(Empreendimento.settings.container));
		var params = { id: $('.hdnEmpId', content).val(), internoId: $('.hdnEmpInternoId', content).val() };
		
		MasterPage.carregando(true);

		$.ajax({
			url: EmpreendimentoInline.settings.urls.copiarInterno,
			cache: false,
			data: params,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0 && !response.EhValido) {
					Mensagem.gerar(MasterPage.getContent(Empreendimento.settings.container), response.Msg);
					return;
				}

				content.empty();
				content.append(response);
				EmpreendimentoSalvar.load();
				MasterPage.botoes(content);
				Empreendimento.settings.container.find('.titTela').remove();
				$('.btnAsmEditar', Empreendimento.settings.container).removeClass('hide');
				$('.btnAsmAssociar', Empreendimento.settings.container).addClass('hide');
				Mascara.load(content);
			}
		});
		MasterPage.carregando(false);

	},

	onModalResponsavelAbrirClick: function () {
		Empreendimento.modalPessoaResp = new PessoaAssociar();
		
		Empreendimento.modalPessoaResp.associarAbrir(Empreendimento.settings.urls.pessoaAssociar, {
			onAssociarCallback: Empreendimento.settings.onResponsavelAssociarClick,
			tituloVerificar: 'Verificar CPF/CNPJ',
			tituloCriar: 'Cadastrar Responsável',
			tituloEditar: 'Editar Responsável',
			tituloVisualizar: 'Visualizar Responsável'
		});
	},

	onModalAtividadeAbrirClick: function () {
		Modal.abrir(Empreendimento.settings.urls.associarAtividadeModal, null, function (AssociarAtividade) {
			AtividadeListar.load(AssociarAtividade, { associarFuncao: Empreendimento.settings.onAtividadeAssociarClick });
		});
	},

	onAtividadeLimparClick: function () {
		$('.hdnAtividadeId', Empreendimento.settings.container).val(0);
		$('.txtAtividade', Empreendimento.settings.container).val('');
		$('.btnLimparAtividade', Empreendimento.settings.container).addClass('hide');
		$('.btnAssociarAtividade', Empreendimento.settings.container).removeClass('hide');
	},

	onTipoSegmentoChange: function () {
		Empreendimento.onSetarDenominador(parseInt($(this).val()));
	},

	onSetarDenominador: function (segmentoId) {
		var denominador = JSON.parse(Empreendimento.settings.denominadoresSegmentos).Denominadores[0].Text;

		$.each(JSON.parse(Empreendimento.settings.denominadoresSegmentos).Denominadores, function (i, item) {
			if (item.Value == segmentoId) {
				denominador = item.Text;
			}
		});

		if ($('.modalLocalizarEmpreendimento', Empreendimento.settings.container).length > 0) {
			$('.lblDenominador', Empreendimento.settings.container).text(denominador);
		} else {
			$('.lblDenominador', Empreendimento.settings.container).text(denominador + ' *');
		}

	},

	onBuscarCoordenada: function () {
		Modal.abrir(Empreendimento.settings.urls.coordenadaGeo, null, function (container) {
			Coordenada.load(container, {
				northing: $('.txtNorthing', Empreendimento.settings.container).val(),
				easting: $('.txtEasting', Empreendimento.settings.container).val(),
				pagemode: 'editMode',
				callBackSalvarCoordenada: Empreendimento.setarCoordenada
			});
			Modal.defaultButtons(container);
		},
		Modal.tamanhoModalGrande);
	},

	setarCoordenada: function (retorno) {
		retorno = JSON.parse(retorno);
		$('.txtNorthing', Empreendimento.settings.container).val(retorno.northing);
		$('.txtEasting', Empreendimento.settings.container).val(retorno.easting);
		$('.btnEmpAvancar', MasterPage.getContent(Empreendimento.settings.container)).button({ disabled: true });
		$('.btnBuscarCoordenada', Empreendimento.settings.container).focus();

		$.ajax({ url: Empreendimento.settings.urls.obterEstadosMunicipiosPorCoordenada + '?easting=' + retorno.easting + '&northing=' + retorno.northing,
			cache: false, async: false, type: 'GET',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Empreendimento.settings.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Empreendimento.settings.container), response.Msg);
					return;
				}

				var container = Empreendimento.settings.container.find('.divEnderecoLocalizacao');
				if (container && container.length > 0) {
					if (response.Municipio.Estado.Id > 0) {
						$('.ddlEstado', container).ddlLoad(response.Estados, { selecionado: response.Municipio.Estado.Id });
						$('.ddlMunicipio', container).ddlLoad(response.Municipios, { selecionado: response.Municipio.Id });
						$('.ddlEstado,.ddlMunicipio', container).addClass('disabled').attr('disabled', 'disabled');
					} else {
						$('.ddlEstado', container).ddlLoad(response.Estados);
						$('.ddlMunicipio', container).ddlClear();
					}
				}
			}
		});
	},

	avancar: function () {
		var content = $('.empreendimentoPartial', MasterPage.getContent(Empreendimento.settings.container));

		MasterPage.carregando(true);
		$.ajax({ url: Empreendimento.settings.urls.avancar, data: JSON.stringify(Empreendimento.settings.objetoEmpreendimento), type: 'POST', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				content.empty();
				content.append(response);
				EmpreendimentoSalvar.load();
				MasterPage.botoes(content);
				Mascara.load(content);
			}
		});
		MasterPage.carregando(false);
	},

	voltar: function () {
		var content = $('.empreendimentoPartial', MasterPage.getContent(Empreendimento.settings.container));

		MasterPage.carregando(true);
		$.ajax({ url: Empreendimento.settings.urls.voltar, data: JSON.stringify(Empreendimento.settings.objetoEmpreendimento), type: 'POST', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				content.empty();
				content.append(response);
				EmpreendimentoLocalizar.load();
				MasterPage.botoes(content);
				Mascara.load(content);
			}
		});
		MasterPage.carregando(false);
	},

	salvar: function (isRedirecionar) {
		var pai = MasterPage.getContent(Empreendimento.settings.container);
		var content = $('.modalSalvarEmpreendimento', pai);
		var empJson = null;
		var urlAcao;

		if (content.length <= 0) {
			content = $('.modalVisualizarEmpreendimento', pai);
		}

		EmpreendimentoSalvar.indexaItensResponsavel();

		$('*', content).removeAttr('disabled');
		var contentJson = EmpreendimentoSalvar.criarObjetoEmpreendimento(content);
		$('.disabled', content).attr('disabled', 'disabled');

		if (parseInt($('.hdnEmpId', Empreendimento.settings.container).val()) > 0) {
			urlAcao = Empreendimento.settings.urls.editar
		} else {
			urlAcao = Empreendimento.settings.urls.salvarCadastrar;
		}

		MasterPage.carregando(true);
		$.ajax({ url: urlAcao, data: JSON.stringify(contentJson), type: 'POST', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.IsEmpreendimentoSalvo) {
					//redireciona para o Listar, em caso de sucesso
					if (typeof response.UrlRedireciona != "undefined" && response.UrlRedireciona !== null && isRedirecionar) {
						MasterPage.redireciona(response.UrlRedireciona);
						return;
					}
					empJson = response.Empreendimento;
				}
				else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(pai, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
		return empJson;
	},

	abrirEditar: function () {
		
		var content = $('.empreendimentoPartial', MasterPage.getContent(Empreendimento.settings.container));
		var params = { id: $('.hdnEmpId', content).val(), internoId: $('.hdnEmpInternoId', content).val() };

		MasterPage.carregando(true);

		$.ajax({ url: Empreendimento.settings.urls.editar, data: params, cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				
				if (response.Msg && response.Msg.length > 0 && !response.EhValido) {
					Mensagem.gerar(MasterPage.getContent(Empreendimento.settings.container), response.Msg);
					return;
				}

				content.empty();
				content.append(response);
				EmpreendimentoSalvar.load();
				MasterPage.botoes(content);
				Mascara.load(content);
			}
		});
		MasterPage.carregando(false);
	},

	abrirVisualizar: function (parametros) {
		var content = $('.empreendimentoPartial', MasterPage.getContent(Empreendimento.settings.container));

		var params = { id: 0, internoId: 0, mostrarTituloTela: false };
		if (parametros && (parametros.id || parametros.internoId)) {
			params.id = parametros.id;
			params.internoId = parametros.internoId;
		} else {
			params.id = $(this).closest('tr').find('.hdnEmpreendimentoId').val();
			params.internoId = $(this).closest('tr').find('.hdnEmpreendimentoInternoId').val();
			params.mostrarTituloTela = true;
		}

		MasterPage.carregando(true);

		$.ajax({ url: Empreendimento.settings.urls.visualizar, data: params, cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				content.empty();
				content.append(response);
			}
		});

		if (Empreendimento.settings.callBackVisualizar) {
			Empreendimento.settings.callBackVisualizar({ mostrarImportar: params.id <= 0 && params.internoId > 0 });
		}

		MasterPage.carregando(false);
	},

	visualizar: function () {
		var id = $(this).closest("tr").find('.hdnEmpreendimentoId').val();
		Modal.abrir(Empreendimento.settings.urls.visualizar + "/" + id, null, function (container) {
			Modal.defaultButtons(container);
			EmpreendimentoLocalizar.bloquearBotoesVisualizar(container);
		});
	},

	obterEmpreendimentoIds: function () {
		return { id: parseInt($('.hdnEmpId', Empreendimento.settings.container).val()) || 0, internoId: $('.hdnEmpInternoId', Empreendimento.settings.container).val() || 0 };
	},

	onLimparResponsavel: function () {
		$('.hdnResponsavelId', Empreendimento.settings.container).val(0);
		$('.txtNomeResponsavel', Empreendimento.settings.container).val('');
		$('.txtCnpjResponsavel', Empreendimento.settings.container).val('');


		$('.divBtnLimparResponsavel', Empreendimento.settings.container).addClass('hide');
		$('.divBtnBuscarResponsavel', Empreendimento.settings.container).removeClass('hide');
	},

	setarEventos: function () {
		$('.btnAssociarResponsavel', Empreendimento.settings.container).click(Empreendimento.onModalResponsavelAbrirClick);
		$('.btnAssociarAtividade', Empreendimento.settings.container).click(Empreendimento.onModalAtividadeAbrirClick);
		$('.btnLimparResponsavel', Empreendimento.settings.container).click(Empreendimento.onLimparResponsavel);
		$('.ddlSegmento', Empreendimento.settings.container).change(Empreendimento.onTipoSegmentoChange);
		$('.ddlEstado', Empreendimento.settings.container).change(Aux.onEnderecoEstadoChange);
	}
}

EmpreendimentoLocalizar = {

	load: function () {
		Empreendimento.settings.onAtividadeAssociarClick = EmpreendimentoLocalizar.onAtividadeAssociarClick;
		Empreendimento.settings.onResponsavelAssociarClick = EmpreendimentoLocalizar.onResponsavelAssociarClick;

		$('.ddlSegmento', Empreendimento.settings.container).change(EmpreendimentoLocalizar.onDesabilitarAvancar);
		$('.txtDenominador, .maskAreaAbrangencia', Empreendimento.settings.container).keyup(EmpreendimentoLocalizar.onDesabilitarAvancar);

		Empreendimento.setarEventos();
		EmpreendimentoLocalizar.onConfigurarBtnLocalizar(MasterPage.getContent(Empreendimento.settings.container));

		$('.RadioEmpreendimentoCodigo', Empreendimento.settings.container).change(function () { EmpreendimentoLocalizar.onCodigoEmpreendimentoChange(true); });
		$('.btnVerificarCodigo', Empreendimento.settings.container).click(EmpreendimentoLocalizar.onVerificarCodigoEmpreendimentoClick);

		$('input:text:enabled', Empreendimento.settings.container).first().focus();

		if (MasterPage.getContent(Empreendimento.settings.container).hasClass('empreendimentoCriar')) {
			$('.hdnModoEmp', Empreendimento.settings.container).val(true);
		}

		if (Empreendimento.settings.objetoEmpreendimento !== null) {
			if ($('.rbCNPJNao', Empreendimento.settings.container).attr('checked')) {
				$('.divCnpjEmp', Empreendimento.settings.container).addClass('hide');
				$('.divFiltros', Empreendimento.settings.container).removeClass('hide');
				MasterPage.redimensionar();
			}
			EmpreendimentoLocalizar.onCodigoEmpreendimentoChange(false);
			$('.btnBuscar', Empreendimento.settings.container).click();
		}
	},

	onDesabilitarAvancar: function () {
		$('.btnEmpAvancar', MasterPage.getContent(Empreendimento.settings.container)).button({ disabled: true });
	},

	onConfigurarBtnLocalizar: function (context) {
		$('.btnEmpAvancar', context).removeClass('hide').button({ disabled: true });
		$('.btnEmpSalvar', context).addClass('hide');
		$('.btnEmpVoltar', context).addClass('hide');
	},

	bloquearBotoesVisualizar: function (container) {
		$('.btnAsmAdicionar', $('.modalVisualizarEmpreendimento', container)).button({ disabled: true }).addClass('hide');
		var responsaveis = $('.asmItens', $('.modalVisualizarEmpreendimento', container));
		$('.asmItemContainer', responsaveis).each(function (index, item) {
			$(item).find('.btnAsmExcluir').addClass('hide');
			$(item).find('.btnAsmAssociar').addClass('hide');
			$(item).find('.btnAsmEditar').addClass('hide');
			$(item).find('.txtDataVencResponsavel').attr('disabled', 'disabled').addClass('disabled');
			$(item).find('.ddlTipoResponsavel').attr('disabled', 'disabled').addClass('disabled');
		});
	},

	onAtividadeAssociarClick: function (Atividade) {
		$('.hdnAtividadeId', Empreendimento.settings.container).val(Atividade.id);
		$('.txtAtividade', Empreendimento.settings.container).val(Atividade.texto);
		$('.btnAssociarAtividade', Empreendimento.settings.container).addClass('hide');
		$('.btnLimparAtividade', Empreendimento.settings.container).removeClass('hide');
		EmpreendimentoLocalizar.onDesabilitarAvancar();
		return true;
	},

	onResponsavelAssociarClick: function (pessoaObj) {
		// validação de profissão de responsável técnico ter profissão (e qualquer outra validação que por ventura vier a calhar sobre este) não será mais feita ao associar e sim apenas no salvar do empreendimento
		//var retorno = MasterPage.validarAjax(Empreendimento.settings.urls.validarAssociarRespTecnico + '/' + pessoaObj.Id, null, null, false);

		$('.hdnResponsavelId', Empreendimento.settings.container).val(pessoaObj.Id);
		$('.txtNomeResponsavel', Empreendimento.settings.container).val(pessoaObj.NomeRazaoSocial);
		$('.txtCnpjResponsavel', Empreendimento.settings.container).val(pessoaObj.CPFCNPJ);
		EmpreendimentoLocalizar.onDesabilitarAvancar();

		if (pessoaObj.Id > 0) {
			$('.divBtnLimparResponsavel', Empreendimento.settings.container).removeClass('hide');
			$('.divBtnBuscarResponsavel', Empreendimento.settings.container).addClass('hide');
		}

		return true;
	},

	onBeforeSerializar: function () {
		$('*', Empreendimento.settings.container).removeAttr('disabled');
	},

	onAfterFiltrar: function (container, serializedData) {
		$('.disabled', Empreendimento.settings.container).attr('disabled', 'disabled');
		Empreendimento.settings.objetoEmpreendimento = serializedData;

		var context = MasterPage.getContent(Empreendimento.settings.container);
		var desabledAvancar = context.find('.dataGridTable tr').length > 0 && context.find('.txtCodigo').val().length > 0;

		if (context.find('.erroCampo').length <= 0 && context.find('.alerta').length <= 0) {
			$('.btnEmpAvancar', context).button({ disabled: desabledAvancar });
			$('.btnEditarEmpreendimento', Empreendimento.settings.container).click(Empreendimento.abrirVisualizar);
			Aux.scrollBottom(context);
		} else {
			$('.habilitarAvançar', Empreendimento.settings.container).closest('fieldset').addClass('hide');
		}

		if ($('.rbCodigoSim', Empreendimento.settings.container).is(':checked') && $('.txtCodigo').val()) {
			if ($(Empreendimento.settings.container).find('.dataGridTable tbody tr').length <= 0) {
				$('.rbCodigoNao', Empreendimento.settings.container).attr('checked', 'checked');
				EmpreendimentoLocalizar.onCodigoEmpreendimentoChange(true);
			}
		}
	},

	onBeforeFiltrar: function (container, serializedData) {
		serializedData.Filtros.Codigo = Mascara.getIntMask($(".txtCodigo", container).val()).toString();
		var teste = $(".txtCodigo", container).val();
	},

	onCodigoEmpreendimentoChange: function (apagarFiltros) {
		if (apagarFiltros) {
			EmpreendimentoLocalizar.limparCamposFiltro();
		}

		var buscarCodigo = $('.rbCodigoSim', Empreendimento.settings.container).is(':checked');
		$('.divCodigoEmp', Empreendimento.settings.container).toggleClass('hide', !buscarCodigo);
		$('.divFiltros', Empreendimento.settings.container).toggleClass('hide', buscarCodigo);
		
		MasterPage.redimensionar();
	},

	ocultarCampoCnpj: function () {
		EmpreendimentoLocalizar.limparCamposFiltro();

		$('.divCnpjEmp', Empreendimento.settings.container).addClass('hide').addClass('hide');
		$('.divFiltros', Empreendimento.settings.container).removeClass('hide').removeClass('hide');

		MasterPage.redimensionar();
	},

	limparCamposFiltro: function () {
		$('.gridContainer', Empreendimento.settings.container).empty();
		var container = $('.divFiltros, .divCodigoEmp', Empreendimento.settings.container);

		$(Empreendimento.settings.container).find(':enabled select option:first').attr('selected', 'selected');

		container.find('input:hidden').val(0);
		container.find('input:text').unmask().val('');

		Mascara.load(container);
		EmpreendimentoLocalizar.onDesabilitarAvancar();
	},

	onVerificarCodigoEmpreendimentoClick: function () {

		$('.btnBuscar', Empreendimento.settings.container).click();
		if ($(Empreendimento).find('.dataGridTable tbody tr').length > 0) {
			EmpreendimentoLocalizar.onDesabilitarAvancar();
		} else {
			$('.rbCNPJNao', Empreendimento.settings.container).change();
			EmpreendimentoLocalizar.onDesabilitarAvancar();
		}
	}
}

EmpreendimentoSalvar = {
	pessoaModalResp: null,

	load: function () {
		Empreendimento.settings.onAtividadeAssociarClick = EmpreendimentoSalvar.onAtividadeAssociarClick;
		Empreendimento.settings.onResponsavelAssociar = EmpreendimentoSalvar.onResponsavelAssociar;

		EmpreendimentoSalvar.setaDenominacao();
		Empreendimento.setarEventos();

		Empreendimento.settings.container.delegate('.ddlTipoResponsavel', 'change', EmpreendimentoSalvar.onTipoResponsavelChange);

		//$('.ddlTipoResponsavel', Empreendimento.settings.container).change(EmpreendimentoSalvar.onTipoResponsavelChange);

		$('.rdbTemCorrespondencia', Empreendimento.settings.container).change(EmpreendimentoSalvar.onCorrespondenciaChange);
		$('.btnVerificarCnpj', Empreendimento.settings.container).click(EmpreendimentoSalvar.verificarCnpj);
		Aux.scrollTop(Empreendimento.settings.container);

		if (parseInt($('.hdnEmpId', Empreendimento.settings.container).val()) > 0 || parseInt($('.hdnEmpInternoId', Empreendimento.settings.container).val()) > 0) {
			EmpreendimentoSalvar.loadEditar(Empreendimento.settings.container);
			$('.asmConteudoInterno', Empreendimento.settings.container).addClass('hide');
			$('.asmExpansivel', Empreendimento.settings.container).text('Clique aqui para ver mais detalhes');
		} else {
			$('.btnBuscarCoordenada', Empreendimento.settings.container).remove();
		}

		EmpreendimentoSalvar.pessoaModalResp = new PessoaAssociar();
		
		$('.divResponsaveis', Empreendimento.settings.container).associarMultiplo({
		    'onAssociar': EmpreendimentoSalvar.onResponsavelAssociar,
		    'associarUrl': Empreendimento.settings.urls.associarResponsavelModal,
		    'associarModalObject': EmpreendimentoSalvar.pessoaModalResp,
		    'associarModalLoadFunction': EmpreendimentoSalvar.pessoaModalResp.load,
		    'associarModalLoadParams': {
		        tituloCriar: 'Cadastrar Responsável',
		        tituloEditar: 'Editar Responsável',
		        tituloVisualizar: 'Visualizar Responsável',
		        editarVisualizar: !$(Empreendimento.settings.container).hasClass('modalVisualizarEmpreendimento')
			},
			'onAssociarCallback': EmpreendimentoSalvar.onResponsavelAssociar,

			'onEditar': EmpreendimentoSalvar.onResponsavelEditar,
			'editarUrl': Empreendimento.settings.urls.associarResponsavelEditarModal,
			'editarModalObject': EmpreendimentoSalvar.pessoaModalResp,
			'editarModalLoadFunction': EmpreendimentoSalvar.pessoaModalResp.load,
			'editarModalLoadParams': {
				tituloCriar: 'Cadastrar Responsável',
				tituloEditar: 'Editar Responsável',
				tituloVisualizar: 'Visualizar Responsável',
				editarVisualizar: !$(Empreendimento.settings.container).hasClass('modalVisualizarEmpreendimento'),
				isCopiado: $('.hdnIsCopiado', Empreendimento.settings.container).val()
			},
			'onEditarClick': EmpreendimentoSalvar.onResponsavelEditarClick,

			'expandirAutomatico': true,
			'minItens': 1,
			'tamanhoModal': Modal.tamanhoModalGrande,
			'onExpandirEsconder': function () { MasterPage.redimensionar(); },
			'msgObrigatoriedade': Empreendimento.settings.msgs.ResponsavelObrigatorio,

			'btnOkLabelExcluir': 'Remover',
			'tituloExcluir': 'Remover Responsável',
			'msgExcluir': function (item, extra) { return 'Tem certeza que deseja remover o responsável ' + $(item).find('.asmItemTexto').val() + ' do empreendimento?'; },
			'onExcluirClick': EmpreendimentoSalvar.onResponsavelExcluirClick,
			'onItemAdicionado': EmpreendimentoSalvar.onResponsavelCarregar
		});

		EmpreendimentoSalvar.onConfigurarBtnSalvar(MasterPage.getContent(Empreendimento.settings.container));
		$('.txtCnpj', Empreendimento.settings.container).focus();
	},

	onConfigurarBtnSalvar: function (context) {
		$('.btnEmpAvancar', context).addClass('hide');
		$('.btnEmpSalvar', context).removeClass('hide');

		if (Empreendimento.settings.objetoEmpreendimento) {
			$('.btnEmpVoltar', context).removeClass('hide');
		}
	},

	indexaItensResponsavel: function () {
		$('.asmItemContainer', Empreendimento.settings.container).each(function (index, item) {
			$(item).find('.divResponsavel').attr('id', 'Empreendimento_Responsaveis_' + index);
			$(item).find('.txtNomeResponsavel').attr('id', 'Empreendimento_Responsaveis_' + index + '__NomeRazao');
			$(item).find('.ddlTipoResponsavel').attr('id', 'Empreendimento_Responsaveis_' + index + '__Tipo');
			$(item).find('.txtDataVencResponsavel').attr('id', 'Empreendimento_Responsaveis_' + index + '__DataVencimentoTexto');
			$(item).find('.txtEspecificarTexto').attr('id', 'Empreendimento_Responsaveis_' + index + '__EspecificarTexto');
		});
	},

	onResponsavelEditarClick: function (container) {
	    $(container).addClass('editando');
	    
	},

	onResponsavelEditar: function (pessoaObj, item, extra) {

		var divItens = $('.asmItens', Empreendimento.settings.container);
		var erroMsg = new Array();

		if (EmpreendimentoSalvar.existeAssociadoEdicao(pessoaObj.Id.toString(), divItens, 'hdnResponsavelId')) {
			erroMsg.push(Empreendimento.settings.msgs.ResponsavelExistente);
			return erroMsg;
		}
		
		$('.hdnResponsavelId', item).val(pessoaObj.Id);
		$('.txtNomeResponsavel', item).val(pessoaObj.NomeRazaoSocial);
		$('.txtCnpjResponsavel', item).val(pessoaObj.CPFCNPJ);
		$('.hdnIsCopiado', item).val(false);

		$(item).closest('asmItemContainer').removeClass('editando');
		$('.btnAsmEditar', item).removeClass('hide');
		return true;
	},

	onResponsavelCarregar: function (container) {
		Mascara.load(container);
		$('.ddlTipoResponsavel', container).change(EmpreendimentoSalvar.onTipoResponsavelChange);
		$('.btnAsmEditar', container).addClass('hide');
		$('.btnAsmAssociar', container).removeClass('hide');
	},

	onResponsavelAssociar: function (pessoaObj, item, extra) {
		var divItens = $('.asmItens', Empreendimento.settings.container);
		var erroMsg = new Array();

		if (EmpreendimentoSalvar.existeAssociado(pessoaObj.Id.toString(), divItens, 'hdnResponsavelId')) {
			erroMsg.push(Empreendimento.settings.msgs.ResponsavelExistente);
			return erroMsg;
		}

		$('.hdnResponsavelId', item).val(pessoaObj.Id);
		$('.txtNomeResponsavel', item).val(pessoaObj.NomeRazaoSocial);
		$('.txtCnpjResponsavel', item).val(pessoaObj.CPFCNPJ);
		$('.btnAsmEditar', item).removeClass('hide');
		return true;
	},

	onResponsavelExcluirClick: function (item) {

		var IsRespBloqueado = $(item).find('.btnAsmExcluir').hasClass('bloqueado');
		if (IsRespBloqueado) {
			Mensagem.gerar(MasterPage.getContent(Empreendimento.settings.container), new Array(Empreendimento.settings.msgs.ResponsavelBloqueado));
			return false;
		}
		return true;
	},

	setaDenominacao: function () {
		var segmentoId = parseInt($('.ddlSegmento', Empreendimento.settings.container).val(), 10);
		Empreendimento.onSetarDenominador(segmentoId);
	},

	onAtividadeAssociarClick: function (Atividade) {
		$('.hdnAtividadeId', Empreendimento.settings.container).val(Atividade.id);
		$('.txtAtividade', Empreendimento.settings.container).val(Atividade.texto);
		$('.btnAssociarAtividade', Empreendimento.settings.container).addClass('hide');
		$('.btnLimparAtividade', Empreendimento.settings.container).removeClass('hide');
		return true;
	},

	buscarUltimoIndice: function () {
		var ultimoIndex = $('.divResponsaveis').find('.responsavel').length + 1;
		return ultimoIndex;
	},

	criarObjetoEmpreendimento: function (content) {
		var _responsaveis = new Array();
		$('.asmItens', content).find('.asmItemContainer').each(function (index, div) {
			var Responsavel = {
				Id: $(div).find('.hdnResponsavelId').val(),
				InternoId: $(div).find('.hdnResponsavelInternoId').val(),
				IdRelacionamento: $(div).find('.hdnResponsavelIdRelacionamento').val(),
				NomeRazao: $(div).find('.txtNomeResponsavel').val(),
				CpfCnpj: $(div).find('.txtCnpjResponsavel').val(),
				Tipo: $(div).find('.ddlTipoResponsavel').val(),
				DataVencimento: ($(div).find('.ddlTipoResponsavel').val() != 3) ? '' : $(div).find('.txtDataVencResponsavel').val(),
				EspecificarTexto: ($(div).find('.ddlTipoResponsavel').val() != 9) ? '' : $(div).find('.txtEspecificarTexto').val()
			};
			_responsaveis.push(Responsavel);
		});

		var _enderecos = new Array();
		content.find('.endereco').each(function (index, div) {
			var Endereco = {
				ZonaLocalizacaoId: null,
				Id: $(div).find('.hdnEnderecoId').val(),
				Correspondencia: $(div).find('.hdnCorrespondencia').val(),
				Cep: $(div).find('.txtCep').val(),
				Logradouro: $(div).find('.txtLogradouro').val(),
				Bairro: $(div).find('.txtBairro').val(),
				DistritoLocalizacao: $(div).find('.txtDistrito').val(),
				EstadoId: $(div).find('.ddlEstado').val(),
				MunicipioId: $(div).find('.ddlMunicipio').val(),
				CaixaPostal: $(div).find('.txtCaixaPostal').val(),
				Numero: $(div).find('.txtNumero').val(),
				Corrego: $(div).find('.txtCorrego').val(),
				Complemento: $(div).find('.txtComplemento').val()
			};
			_enderecos.push(Endereco);
		});

		_enderecos[0].ZonaLocalizacaoId = $('input.rdbZonaLocalizacao:checked', content).val();

		var contentJson = {
			Empreendimento: {
				Codigo: $('.txtCodigo', content).val(),
				Id: $('.hdnEmpId', content).val(),
				InternoId: $('.hdnEmpInternoId', content).val(),
				Segmento: $('.ddlSegmento', content).val(),
				CNPJ: $('.txtCnpj', content).val(),
				Denominador: $('.txtDenominador', content).val(),
				NomeFantasia: $('.txtNomeFantasia', content).val(),
				Atividade: {
					Atividade: $('.txtAtividade', content).val(),
					Id: $('.hdnAtividadeId', content).val()
				},
				Responsaveis: _responsaveis,
				Enderecos: _enderecos,
				TemCorrespondencia: $('input.rdbTemCorrespondencia:checked', content).val(),
				Coordenada: {
					Id: $('.hdnCoordenadaId', content).val(),
					Tipo: { Id: $('.ddlCoordenadaTipo', content).val() },
					Datum: { Id: $('.ddlDatum', content).val() },
					NorthingUtmTexto: $('.txtNorthing', content).val(),
					EastingUtmTexto: $('.txtEasting', content).val(),
					FusoUtm: $('.ddlFuso', content).val(),
					HemisferioUtm: $('.ddlHemisferio', content).val(),
					LocalColeta: $('.ddlLocalColeta', content).val(),
					FormaColeta: $('.ddlFormaColeta', content).val()
				}
			},

			Contato: {
				TelefoneId: $('.txtTelefoneId', content).val(),
				Telefone: $('.txtTelefone', content).val(),
				TelefoneFaxId: $('.txtTelFaxId', content).val(),
				TelefoneFax: $('.txtTelFax', content).val(),
				EmailId: $('.txtEmailId', content).val(),
				Email: $('.txtEmail', content).val(),
				NomeContatoId: $('.hdnNomeContatoId', content).val(),
				NomeContato: $('.txtNomeContato', content).val()
			}
		};
		return contentJson;
	},

	existeAssociado: function (item, div, itemClass) {
		var existe = false;
		var itens = $(div).find('.asmItemContainer');

		$.each(itens, function (key, elem) {
			if ($(elem).find('.' + itemClass) !== '') {
				var divItem = $(elem).find('.' + itemClass).val();
				existe = (item.toLowerCase().trim() === divItem.toLowerCase().trim());
				if (existe) {
					return false;
				}
			}
		});
		return existe;
	},

	existeAssociadoEdicao: function (item, div, itemClass) {
		var existe = false;

		var itens = $(div).find('.asmItemContainer');
		$.each(itens, function (key, elem) {
			if ($(elem).find('.' + itemClass) !== '') {
				var divItem = $(elem).find('.' + itemClass).val();
				existe = (item.toLowerCase().trim() === divItem.toLowerCase().trim()) && !$(elem).hasClass('editando');
				if (existe) {
					return false;
				}
			}
		});
		return existe;
	},

	onCorrespondenciaChange: function () {
		var correnpondencia = parseInt($(this, Empreendimento.settings.container).val());

		if (correnpondencia > 0) {
			$('.correspondenciaContainer', Empreendimento.settings.container).removeClass('hide');
		} else {
			$('.correspondenciaContainer', Empreendimento.settings.container).addClass('hide');
		}
	},

	verificarCnpj: function () {
		var cnpj = $('.txtCnpj', Empreendimento.settings.container).val();
		var id = parseInt($('.hdnEmpId', Empreendimento.settings.container).val());
		MasterPage.carregando(true);

		$.ajax({ url: Empreendimento.settings.urls.verificarCnpj, data: { Cnpj: cnpj, Id: id }, cache: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				var arrayMensagem = new Array();
				arrayMensagem.push(response.Msg);
				Mensagem.gerar(MasterPage.getContent(Empreendimento.settings.container), arrayMensagem);
			}
		});
		MasterPage.carregando(false);
	},

	loadEditar: function (container) {
		$("*:not(.txtCodigo)", container).removeAttr('disabled').removeClass('disabled');
		$(".txtAtividade,.txtNomeResponsavel,.txtCnpjResponsavel", container).addClass('disabled').attr('disabled', 'disabled');
		$(".divCamposCoordenadas", container).find("input[type=text], select").addClass('disabled').attr('disabled', 'disabled');

		if ($('.ddlEstado', container.find('.divEnderecoLocalizacao')).val() == $('.hdnEstadoDefault', Empreendimento.settings.container).val()) {
			$('.ddlEstado,.ddlMunicipio', container.find('.divEnderecoLocalizacao')).addClass('disabled').attr('disabled', 'disabled');
		}

		$('.btnEmpVoltar', MasterPage.getContent(container)).addClass('hide');
		$('.titTela', container).html('Editar Empreendimento');
	},

	bloquearCamposResponsavel: function (container) {
		$('.asmItemContainer', container).first().find('.btnAsmExcluir').addClass('bloqueado');
		$('.asmItemContainer', container).first().attr('id', 'Responsavel_Bloqueado');
	},

	onTipoResponsavelChange: function () {
	    
		var container = Empreendimento.settings.container;
		var tipoResponsavel = $(this, container).val();

		if (tipoResponsavel == 3) {
			$(this, container).closest('.asmConteudoFixo').find('.divDatavencimento').removeClass('hide');
		} else {
			$(this, container).closest('.asmConteudoFixo').find('.divDatavencimento').addClass('hide');
		}

		if (tipoResponsavel == 9/*Outro*/) {
			$(this, container).closest('.asmConteudoFixo').find('.divEspecificar').removeClass('hide');
		} else {
			$(this, container).closest('.asmConteudoFixo').find('.divEspecificar').addClass('hide');
		}
	}
}
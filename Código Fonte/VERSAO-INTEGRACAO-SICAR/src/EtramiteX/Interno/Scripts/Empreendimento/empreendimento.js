/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
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
			verificarLocalizaoEmpreendimento:'',
			visualizar: '',
			associarResponsavelModal: '',
			associarResponsavelEditarModal: '',
			associarAtividadeModal: '',
			enderecoMunicipio: '',
			pessoaAssociar: '',
			coordenadaGeo: '',
			obterMunicipioPorCoordenada: '',
			obterEstadosMunicipiosPorCodIbge: '',
			obterEnderecoResponsavel: '',
			obterListaPessoasAssociada: ''
		},
		msgs: {},
		idsTela: null,
		denominadoresSegmentos: null,
		obterEnderecoPessoaAssociada: null
	},

	load: function (content, options) {
		if (options) {
			$.extend(Empreendimento.settings, options);
		}
		Empreendimento.settings.container = content;

		if (typeof Empreendimento.settings.obterEnderecoPessoaAssociada != 'function') {
			Empreendimento.settings.obterEnderecoPessoaAssociada = Empreendimento.obterEnderecoPessoaAssociada;
		}

		if ($('.modalLocalizarEmpreendimento', content).length > 0) {
			EmpreendimentoLocalizar.load();
			Empreendimento.settings.container.listarAjax({
				onBeforeSerializar: EmpreendimentoLocalizar.onBeforeSerializar,
				onBeforeFiltrar: EmpreendimentoLocalizar.onBeforeFiltrar,
				onAfterFiltrar: EmpreendimentoLocalizar.onAfterFiltrar,
				mensagemContainer: MasterPage.getContent(Empreendimento.settings.container)
			});
		} else if ($('.modalSalvarEmpreendimento', content).length > 0) {
			EmpreendimentoSalvar.load();
		}

		content.undelegate('.btnBuscarCoordenada', 'click', Empreendimento.onBuscarCoordenada);
		content.undelegate('.btnLimparAtividade', 'click', Empreendimento.onAtividadeLimparClick);

		content.delegate('.btnBuscarCoordenada', 'click', Empreendimento.onBuscarCoordenada);
		content.delegate('.btnLimparAtividade', 'click', Empreendimento.onAtividadeLimparClick);
	},

	modalPessoaResp: null,

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
		var id = 0;

		// Quando não há o container de localizar pegar o de visualizar
		if (content.find('.modalLocalizarEmpreendimento').length <= 0) {
			id = $('.hdnEmpId', content).val();
		} else {
			id = $(this).closest("tr").find('.hdnEmpreendimentoId').val();
		}

		MasterPage.carregando(true);

		$.ajax({ url: Empreendimento.settings.urls.editar, data: { id: id }, cache: false, async: false,
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

	abrirVisualizar: function (empreendimentoId) {
		var content = $('.empreendimentoPartial', MasterPage.getContent(Empreendimento.settings.container));

		var id = 0;
		if (typeof empreendimentoId === "string") {
			id = empreendimentoId;
		} else {
			id = $(this).closest("tr").find('.hdnEmpreendimentoId').val();
		}

		MasterPage.carregando(true);

		$.ajax({ url: Empreendimento.settings.urls.visualizar,
			data: { id: id, mostrarTituloTela: false }, cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				content.empty();
				content.append(response);
			}
		});
		Empreendimento.settings.callBackVisualizar();
		MasterPage.carregando(false);
	},

	visualizar: function () {
		var id = $(this).closest("tr").find('.hdnEmpreendimentoId').val();
		Modal.abrir(Empreendimento.settings.urls.visualizar + "/" + id, null, function (container) {
			Modal.defaultButtons(container);
			EmpreendimentoLocalizar.bloquearBotoesVisualizar(container);
		});
	},

	obterIdEmpreendimento: function () {
		return $('.hdnEmpId', Empreendimento.settings.container).val();
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
		$('.btnLimparResponsavel', Empreendimento.settings.container).click(Empreendimento.onLimparResponsavel);
		$('.btnAssociarAtividade', Empreendimento.settings.container).click(Empreendimento.onModalAtividadeAbrirClick);
		$('.ddlSegmento', Empreendimento.settings.container).change(Empreendimento.onTipoSegmentoChange);
		$('.ddlEstado', Empreendimento.settings.container).change(Aux.onEnderecoEstadoChange);
	},

	carregarEndereco: function (endereco, container) {
		$('.txtCep', container).val(endereco.Cep);
		$('.txtLogradouro', container).val(endereco.Logradouro);
		$('.txtBairro', container).val(endereco.Bairro);
		$('.txtNumero', container).val(endereco.Numero);
		$('.txtDistrito', container).val(endereco.DistritoLocalizacao);
		$('.txtComplemento', container).val(endereco.Complemento);
		$('.txtCorrego', container).val(endereco.Corrego);

		$('.ddlEstado option', container).each(function () {
			if ($(this).val() == endereco.EstadoId) {
				$(this).attr('selected', 'selected');
			}
		});

		$('.ddlEstado', container).trigger('change');
		$('.ddlEstado', container).removeAttr('disabled');
		$('.ddlEstado', container).removeClass('disabled');

		$('.ddlMunicipio option', container).each(function () {
			if ($(this).val() == endereco.MunicipioId) {
				$(this).attr('selected', 'selected');
			}
		});
	},

	obterEnderecoPessoaAssociada: function (origemId, empreendimentoId) {
		var container = Empreendimento.settings.container;

		$.ajax({
			url: Empreendimento.settings.urls.obterListaPessoasAssociada,
			data: JSON.stringify({ tipo: origemId, empreendimentoId: empreendimentoId }),
			async: false,
			cache: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown, container) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Lista) {
					$('.ddlCopiarEnderecoCorrespondenciaTipo', container).ddlLoad(response.Lista);
					$('.ddlCopiarEnderecoCorrespondenciaTipo', container).trigger('change');
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(response.Msg);
				}
			}
		});


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

		$('.RadioEmpreendimentoCodigo', Empreendimento.settings.container).change(EmpreendimentoLocalizar.onCodigoEmpreendimentoChange);
		$('.btnVerificarCodigo', Empreendimento.settings.container).click(EmpreendimentoLocalizar.onVerificarCodigoEmpreendimentoClick);
		$('.txtCodigoEmpreendimento', Empreendimento.settings.container).focus();

		if (MasterPage.getContent(Empreendimento.settings.container).hasClass('empreendimentoCriar')) {
			$('.hdnModoEmp', Empreendimento.settings.container).val(true);
		}

		if (Empreendimento.settings.objetoEmpreendimento !== null) {
			if ($('.rbCNPJNao', Empreendimento.settings.container).attr('checked')) {
				$('.divCnpjEmp', Empreendimento.settings.container).addClass('hide').hide(1);
				$('.divFiltros', Empreendimento.settings.container).removeClass('hide').show(1);
				MasterPage.redimensionar();
			}
			$('.btnBuscar', Empreendimento.settings.container).click();
		}
	},

	onDesabilitarAvancar: function () {
		$('.btnEmpAvancar', MasterPage.getContent(Empreendimento.settings.container)).button({ disabled: true });
	},

	onConfigurarBtnLocalizar: function (context) {
		$('.btnEmpAvancar', context).show().button({ disabled: true });
		$('.btnEmpSalvar', context).hide();
		$('.btnEmpVoltar', context).hide();
	},

	bloquearBotoesVisualizar: function (container) {
		$('.btnAsmAdicionar', $('.modalVisualizarEmpreendimento', container)).button({ disabled: true }).hide();
		var responsaveis = $('.asmItens', $('.modalVisualizarEmpreendimento', container));
		$('.asmItemContainer', responsaveis).each(function (index, item) {
			$(item).find('.btnAsmExcluir').hide();
			$(item).find('.btnAsmAssociar').hide();
			$(item).find('.btnAsmEditar').hide();
			$(item).find('.txtDataVencResponsavel').attr('disabled', 'disabled').addClass('disabled');
			$(item).find('.txtEspecificarTexto').attr('disabled', 'disabled').addClass('disabled');
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

	onBeforeFiltrar: function (container, serializedData) {
		serializedData.Filtros.Codigo = Mascara.getIntMask($(".txtCodigo", container).val()).toString();
	},

	onAfterFiltrar: function (container, serializedData) {
		$('.disabled', Empreendimento.settings.container).attr('disabled', 'disabled');
		Empreendimento.settings.objetoEmpreendimento = serializedData;
		
		var context = MasterPage.getContent(Empreendimento.settings.container);
		var desabledAvancar = context.find('.dataGridTable tr').length > 0 && context.find('.txtCodigo').val().length > 0;

		if (context.find('.erroCampo').length <= 0 && context.find('.alerta').length <= 0) {
			$('.btnEmpAvancar', context).button({ disabled: desabledAvancar });
			$('.btnVisualizarEmpreendimento', Empreendimento.settings.container).click(Empreendimento.visualizar);

			if (Empreendimento.settings.modoInline) {
				$('.btnEditarEmpreendimento', Empreendimento.settings.container).click(Empreendimento.abrirVisualizar);
			} else {
				$('.btnEditarEmpreendimento', Empreendimento.settings.container).click(Empreendimento.abrirEditar);
			}
			Aux.scrollBottom(context);
		} else {
			$('.habilitarAvançar', Empreendimento.settings.container).closest('fieldset').hide();
		}

		if ($('.rbCodigoSim', Empreendimento.settings.container).is(':checked') && $('.txtCodigo').val())
		{
			if ($(Empreendimento.settings.container).find('.dataGridTable tbody tr').length <= 0)
			{
				$('.rbCodigoNao', Empreendimento.settings.container).attr('checked', 'checked');
				EmpreendimentoLocalizar.onCodigoEmpreendimentoChange();
			}
		}
	},

	onCodigoEmpreendimentoChange: function () {
		EmpreendimentoLocalizar.limparCamposFiltro();

		$('.divCodigoEmp', Empreendimento.settings.container).toggle();
		$('.divFiltros', Empreendimento.settings.container).toggle();

		MasterPage.redimensionar();
	},

	ocultarCampoCnpj: function () {
		EmpreendimentoLocalizar.limparCamposFiltro();

		$('.divCnpjEmp', Empreendimento.settings.container).addClass('hide').hide(1);
		$('.divFiltros', Empreendimento.settings.container).removeClass('hide').show(1);

		MasterPage.redimensionar();
	},

	limparCamposFiltro: function () {
		$('.gridContainer', Empreendimento.settings.container).empty();
		var container = $('.divFiltros, .divCodigoEmp', Empreendimento.settings.container);

		$(Empreendimento.settings.container).find(':enabled select option:first').attr('selected', 'selected');

		container.find('input:hidden').val(0);
		container.find('input:text').unmask().val('');
		container.find('txtCodigo').val("");

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

		$('.ddlTipoResponsavel', Empreendimento.settings.container).change(EmpreendimentoSalvar.onTipoResponsavelChange);
		$('.rdbTemCorrespondencia', Empreendimento.settings.container).change(EmpreendimentoSalvar.onCorrespondenciaChange);
		$('.rdbCopiarEnderecoCorrespondencia', Empreendimento.settings.container).change(EmpreendimentoSalvar.onCopiarEnderecoCorrespondencia);

		$('.ddlCopiarEnderecoCorrespondenciaOrigem', Empreendimento.settings.container).change(EmpreendimentoSalvar.onCopiarEnderecoCorrespondenciaOrigem);
		$('.ddlCopiarEnderecoCorrespondenciaTipo', Empreendimento.settings.container).change(EmpreendimentoSalvar.onCopiarEnderecoCorrespondenciaTipo);

		$('.btnCopiarEnderecoResponsavel', Empreendimento.settings.container).click(EmpreendimentoSalvar.onCopiarEnderecoResponsavel);
		$('.ddlResponsaveis', Empreendimento.settings.container).change(EmpreendimentoSalvar.onResponsavelChange);
		$('.btnVerificarCnpj', Empreendimento.settings.container).click(EmpreendimentoSalvar.verificarCnpj);
		Aux.scrollTop(Empreendimento.settings.container);

		if (parseInt($('.hdnEmpId', Empreendimento.settings.container).val()) > 0) {
			EmpreendimentoSalvar.loadEditar(Empreendimento.settings.container);
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
				editarVisualizar: !$(Empreendimento.settings.container).hasClass('modalVisualizarEmpreendimento')
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
			'onItemExcluido': EmpreendimentoSalvar.onObterListaResponsaveis,

			'onItemAdicionado': EmpreendimentoSalvar.onResponsavelCarregar
		});

		EmpreendimentoSalvar.onConfigurarBtnSalvar(MasterPage.getContent(Empreendimento.settings.container));
		$('.txtCnpj', Empreendimento.settings.container).focus();
	},

	onConfigurarBtnSalvar: function (context) {
		$('.btnEmpAvancar', context).hide();
		$('.btnEmpSalvar', context).show();

		if (Empreendimento.settings.objetoEmpreendimento) {
			$('.btnEmpVoltar', context).show();
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
		if ($('.hdnResponsavelId', container).val() <= 0) {
			return false;
		}

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

		$(item).closest('asmItemContainer').removeClass('editando');
		$('.btnAsmEditar', item).show();
		return true;
	},

	onResponsavelCarregar: function (container) {
		Mascara.load(container);
		$('.ddlTipoResponsavel', container).change(EmpreendimentoSalvar.onTipoResponsavelChange);
		$('.btnAsmEditar', container).hide();
		$('.btnAsmAssociar', container).show();
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
		$('.btnAsmEditar', item).show();

		EmpreendimentoSalvar.onObterListaResponsaveis();

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
		if (segmentoId > 0) {
			Empreendimento.onSetarDenominador(segmentoId);
		}
	},

	onAtividadeAssociarClick: function (Atividade) {
		$('.hdnAtividadeId', Empreendimento.settings.container).val(Atividade.id);
		$('.txtAtividade', Empreendimento.settings.container).val(Atividade.texto);
		$('.btnAssociarAtividade', Empreendimento.settings.container).addClass('hide');
		$('.btnLimparAtividade', Empreendimento.settings.container).removeClass('hide');
		return true;
	},
	
	limparCamposLocalizacaoEmpreendimento: function () {
		$('.txtCep', Empreendimento.settings.container).val('');
		$('.rdbZonaLocalizacao', Empreendimento.settings.container).val('');
		$('.txtLogradouro', Empreendimento.settings.container).val('');
		$('.txtBairro', Empreendimento.settings.container).val('');
		$('.txtDistrito', Empreendimento.settings.container).val('');
		$('.txtCorrego', Empreendimento.settings.container).val('');
		$('.txtComplemento', Empreendimento.settings.container).val('');
		$('.ddlLocalColeta', Empreendimento.settings.container).val('');
		$('.ddlFormaColeta', Empreendimento.settings.container).val('');
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
				Id: $('.hdnEmpId', content).val(),
				Tid: $('.hdnEmpTid', content).val(),
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
				TemCorrespondencia: 1/*Sempre terá endereco de correspondencia, mesmo que seja o mesmo do empreendimento*/,
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
			$('.correspondenciaContainer', Empreendimento.settings.container).show();
		} else {
			$('.correspondenciaContainer', Empreendimento.settings.container).hide();
		}
	},

	onCopiarEnderecoCorrespondencia: function(){
		var container = Empreendimento.settings.container;
		var isCopiar = +$('.rdbCopiarEnderecoCorrespondencia:checked', container).val() == 1;

		if (isCopiar) {
			$('.divCopiarEnderecoCorrespondencia', container).removeClass('hide');
		} else {
			$('.divCopiarEnderecoCorrespondencia', container).addClass('hide');
		}
	},

	onCopiarEnderecoCorrespondenciaOrigem: function () {
		var container = Empreendimento.settings.container;
		var origemId = $('.ddlCopiarEnderecoCorrespondenciaOrigem :selected', container).val();
		var origemTexto = $('.ddlCopiarEnderecoCorrespondenciaOrigem :selected', container).text();
		var empreendimentoId = $('.hdnEmpId', container).val();

		if (origemId == 0) {
			$('.divCopiarEnderecoCorrespondenciaTipo', container).addClass('hide');
			return;
		}

		if (origemId == Empreendimento.settings.idsTela.EnderecoTipoEmpreendimento) {
			var containerEnderecoEmp = $('.enderecoEmpreendimento', container);
			$('.divCopiarEnderecoCorrespondenciaTipo', container).addClass('hide');

			var endereco = {
				Cep: $('.txtCep', containerEnderecoEmp).val(),
				Logradouro: $('.txtLogradouro', containerEnderecoEmp).val(),
				Bairro: $('.txtBairro', containerEnderecoEmp).val(),
				Numero: $('.txtNumero', containerEnderecoEmp).val(),
				DistritoLocalizacao: $('.txtDistrito', containerEnderecoEmp).val(),
				Complemento: $('.txtComplemento', containerEnderecoEmp).val(),
				Corrego: $('.txtCorrego', containerEnderecoEmp).val(),
				EstadoId: $('.ddlEstado :selected', containerEnderecoEmp).val(),
				MunicipioId: $('.ddlMunicipio :selected', containerEnderecoEmp).val()
			}

			Empreendimento.carregarEndereco(endereco, $('.correspondenciaContainer', container));
			return;
		}

		$('.divCopiarEnderecoCorrespondenciaTipo', container).removeClass('hide');
		$('.spanNomeTipoEndereco', container).html('Nome do ' + origemTexto);

		if (origemId == Empreendimento.settings.idsTela.EnderecoTipoRepresentante) {

			var pai = MasterPage.getContent(container);
			var content = $('.modalSalvarEmpreendimento', pai);

			var list = EmpreendimentoSalvar.criarObjetoEmpreendimento(content).Empreendimento.Responsaveis;

			$.ajax({
				url: Empreendimento.settings.urls.obterListaResponsaveis,
				data: JSON.stringify({ responsaveis: list }),
				cache: false,
				type: 'POST',
				typeData: 'json',
				contentType: 'application/json; charset=utf-8',
				error: function (XMLHttpRequest, textStatus, erroThrown, container) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, container);
				},
				success: function (response, textStatus, XMLHttpRequest) {

					if (response.Responsaveis) {
						$('.ddlCopiarEnderecoCorrespondenciaTipo', container).ddlLoad(response.Responsaveis);
						$('.ddlCopiarEnderecoCorrespondenciaTipo', container).trigger('change');
					}

					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(response.Msg);
					}
				}
			});

			return;
		}

		Empreendimento.settings.obterEnderecoPessoaAssociada(origemId, empreendimentoId);

	},

	onCopiarEnderecoCorrespondenciaTipo: function () {
		var container = $(Empreendimento.settings.container).find('.correspondenciaContainer');
		var pessoa = parseInt($(this, container).val()) || 0;

		if (pessoa != 0) {
			MasterPage.carregando(true);

			$.ajax({
				url: Empreendimento.settings.urls.obterEnderecoResponsavel,
				data: { responsavelId: pessoa },
				cache: false,
				error: function (XMLHttpRequest, textStatus, erroThrown, container) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, container);
				},
				success: function (response, textStatus, XMLHttpRequest) {

					if (response.Endereco) {
						Empreendimento.carregarEndereco(response.Endereco, container);
					}

					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(response.Msg);
					}
				}
			});

			MasterPage.carregando(false);

		}
	},

	onCopiarEnderecoResponsavel: function () {
		$('.divResponsavelEnd', Empreendimento.settings.container).show();
	},

	onResponsavelChange: function () {
		var container = $(Empreendimento.settings.container).find('.enderecoEmpreendimento');
		var responsavel = parseInt($(this, container).val()) || 0;
		var empEstadoId = parseInt($('.ddlEstado', container).val());
		var empMunicipioId = parseInt($('.ddlMunicipio', container).val());
		var msg = null;
		if (responsavel != 0) {
			MasterPage.carregando(true);

			$.ajax({
				url: Empreendimento.settings.urls.obterEnderecoResponsavel,
				data: { empreendimentoEstadoId: empEstadoId, empreendimentoMunicipioId: empMunicipioId, responsavelId: responsavel },
				cache: false,
				error: function (XMLHttpRequest, textStatus, erroThrown, container) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, container);
				},
				success: function (response, textStatus, XMLHttpRequest) {


					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(container, response.Msg);
						$('.ddlEstado', container).attr('disabled', 'disabled').addClass('disabled');
						$('.ddlMunicipio', container).attr('disabled', 'disabled').addClass('disabled');
						return;
					}

					if (response.Endereco) {
						msg = EmpreendimentoSalvar.verificarLocalizacaoEmpreendimento($('.txtEasting', container).val(), $('.txtNorthing', container).val(), response.Endereco.EstadoId, response.Endereco.MunicipioId);

						if (msg && msg != null && msg.Texto != null) {
							Mensagem.gerar(MasterPage.getContent(Empreendimento.settings.container), [msg]);
							return;
						}

						Empreendimento.carregarEndereco(response.Endereco, container);
					}
										
				}
			});

			$('.divResponsavelEnd', Empreendimento.settings.container).hide();

			MasterPage.carregando(false);
		}
	},

	verificarLocalizacaoEmpreendimento: function (easting, northing, estadoID, municipioID) {
		var retorno = null;
		$.ajax({
			url: Empreendimento.settings.urls.verificarLocalizaoEmpreendimento,
			data: { easting: easting, northing: northing, estadoID: estadoID, municipioID: municipioID },
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				retorno = response.Msg;
			}
		});
		return retorno;
		
	},

	onObterListaResponsaveis: function () {
		var container = Empreendimento.settings.container;
		var pai = MasterPage.getContent(container);
		var content = $('.modalSalvarEmpreendimento', pai);

		var list = EmpreendimentoSalvar.criarObjetoEmpreendimento(content).Empreendimento.Responsaveis;

		$.ajax({
			url: Empreendimento.settings.urls.obterListaResponsaveis,
			data: JSON.stringify({ responsaveis: list }),
			cache: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown, container) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Responsaveis) {
					$('.ddlResponsaveis', container).ddlLoad(response.Responsaveis);
					$('.ddlResponsaveis', container).removeAttr('disabled').removeClass('disabled');

					$('.ddlResponsaveis', container).val(0);
					$('.ddlResponsaveis', container).trigger('change');
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(response.Msg);
				}

			}
		});
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
		$("*", container).removeAttr('disabled').removeClass('disabled');
		$(".txtCodigo, .txtAtividade,.txtNomeResponsavel,.txtCnpjResponsavel", container).addClass('disabled').attr('disabled', 'disabled');
		$(".divCamposCoordenadas", container).find("input[type=text], select").addClass('disabled').attr('disabled', 'disabled');

		if ($('.ddlEstado', container.find('.divEnderecoLocalizacao')).val() == $('.hdnEstadoDefault', Empreendimento.settings.container).val()) {
			$('.ddlEstado,.ddlMunicipio', container.find('.divEnderecoLocalizacao')).addClass('disabled').attr('disabled', 'disabled');
		}

		$('.btnEmpVoltar', MasterPage.getContent(container)).hide();
		$('.titTela', container).html('Editar Empreendimento');
	},

	bloquearCamposResponsavel: function (container) {
		$('.asmItemContainer', container).first().find('.btnAsmAssociar').hide();
		$('.asmItemContainer', container).first().find('.btnAsmExcluir').addClass('bloqueado');
		$('.asmItemContainer', container).first().attr('id', 'Responsavel_Bloqueado');
	},

	onTipoResponsavelChange: function () {
		var container = Empreendimento.settings.container;
		var tipoResponsavel = $(this, container).val();

		if (tipoResponsavel == 3) {
			$(this, container).closest('.asmConteudoFixo').find('.divDatavencimento').show();
		} else {
			$(this, container).closest('.asmConteudoFixo').find('.divDatavencimento').hide();
		}

		if (tipoResponsavel == 9/*Outro*/) {
			$(this, container).closest('.asmConteudoFixo').find('.divEspecificar').show();
		} else {
			$(this, container).closest('.asmConteudoFixo').find('.divEspecificar').hide();
		}
	}
}
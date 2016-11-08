/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />
/// <reference path="../jquery.tablesorter.js" />
/// <reference path="analisarItemAnalise.js" />

Agrotoxico = {
	settings: {
		urls: {
			salvar: '',
			obterMensagemAgrotoxicoDesativado: '',
			associarPessoa: '',
			ingredientesAtivos: '',
			agrotoxicoCulturaCriar: '',
			agrotoxicoCulturaEditar: '',
			agrotoxicoCulturaVisualizar: '',
			enviarArquivo:''
		},
		ingredienteAtivoUnidadeMedida: null,
		mensagens: null,
		idsTelaIngredienteAtivoSituacao: null,
		tiposArquivo:null
	},
	pessoaModal: null,
	container: null,

	load: function (container, options) {

		if (options) { $.extend(Agrotoxico.settings, options); }
		Agrotoxico.container = MasterPage.getContent(container);

		Agrotoxico.container.delegate(".btnSalvar", 'click', Agrotoxico.salvar);
		Agrotoxico.container.delegate(".RadioPossuiNumCadastro", 'change', Agrotoxico.radioPossuiNumCadastroChange);
		Agrotoxico.container.delegate(".RadioCadastroAtivo", 'change', Agrotoxico.radioCadastroAtivoChange);
		Agrotoxico.container.delegate(".btnBuscarTitularRegistro", 'click', Agrotoxico.abrirModalPessoas);
		Agrotoxico.container.delegate(".btnBuscarIngredienteAtivo", 'click', Agrotoxico.abrirModalIngredientesAtivos);
		Agrotoxico.container.delegate(".ddlIngredienteAtivoUnidadeMedida", 'change', Agrotoxico.changeIngredienteAtivoUnidadeMedida);

		Agrotoxico.container.delegate(".btnAddIngredienteAtivo", 'click', Agrotoxico.onAddIngredienteAtivo);
		Agrotoxico.container.delegate(".btnExcluir", 'click', Agrotoxico.excluirItemGrid);
		Agrotoxico.container.delegate(".btnAddGrupoQuimico", 'click', Agrotoxico.onAddGrupoQuimico);
		Agrotoxico.container.delegate(".btnAddCultura", 'click', Agrotoxico.abrirModalCultura);
		Agrotoxico.container.delegate(".btnEditarCultura", 'click', Agrotoxico.abrirModalCulturaEditar);
		Agrotoxico.container.delegate(".btnCopiar", 'click', Agrotoxico.abrirModalCulturaCopiar);
		Agrotoxico.container.delegate(".btnVisualizarCultura", 'click', Agrotoxico.abrirModalCulturaVisualizar);
		Agrotoxico.container.delegate(".btnAddArq", 'click', Agrotoxico.enviarArquivo);
		Agrotoxico.container.delegate(".btnLimparArq", 'click', Agrotoxico.onLimparArquivoClick);

		Agrotoxico.container.delegate('.titFiltros', 'click', Aux.expadirFieldSet);
	},

	radioPossuiNumCadastroChange: function () {
		Mensagem.limpar(Agrotoxico.container);
		if ($(this).val() == 1) {
			$('.txtNumeroCadastro', Agrotoxico.container).val('');
			$('.txtNumeroCadastro', Agrotoxico.container).removeClass('disabled');
			$('.txtNumeroCadastro', Agrotoxico.container).removeAttr('disabled');

			$('.txtNumeroCadastro', Agrotoxico.container).focus();
		} else {
			$('.txtNumeroCadastro', Agrotoxico.container).addClass('disabled');
			$('.txtNumeroCadastro', Agrotoxico.container).attr('disabled', 'disabled');
			$('.txtNumeroCadastro', Agrotoxico.container).val('Gerado automaticamente');
		}
	},

	radioCadastroAtivoChange: function () {
		if (+$(this).val() == 1) {
			$('.divMotivo', Agrotoxico.container).addClass('hide');
			$('.hdnMotivoId', Agrotoxico.container).val(0);
			$('.txtMotivo', Agrotoxico.container).val('');
		} else {
			Mensagem.limpar(Agrotoxico.container);
			Agrotoxico.obterMensagemAgrotoxicoDesativado();
			$('.divMotivo', Agrotoxico.container).removeClass('hide');
		}
	},

	abrirModalPessoas: function () {
		Agrotoxico.pessoaModal = new PessoaAssociar();
		Agrotoxico.pessoaModal.settings.tipoCadastro = 2;
		Agrotoxico.pessoaModal.associarAbrir(Agrotoxico.settings.urls.associarPessoa, {
			onAssociarCallback: Agrotoxico.callBackAssociarTitularRegistro,
			tituloVerificar: 'Verificar CPF/CNPJ',
			tituloCriar: 'Cadastrar Titular do Registro',
			tituloEditar: 'Editar Titular do Registro',
			tituloVisualizar: 'Visualizar Titular do Registro'
		});
	},

	callBackAssociarTitularRegistro: function (response) {
		$('.txtTitularRegistroNome', Agrotoxico.container).val(response.NomeRazaoSocial);
		$('.hdnTitularRegistroId', Agrotoxico.container).val(response.Id);
	},

	abrirModalIngredientesAtivos: function () {
		Modal.abrir(Agrotoxico.settings.urls.ingredientesAtivos, null,
			function (content) {
				IngredienteAtivoListar.load(content, { associarFuncao: Agrotoxico.callBackAssociarIngredienteAtivo });
				Modal.defaultButtons(content);
			}, Modal.tamanhoModalMedia, 'Associar Ingrediente Ativo');
	},

	callBackAssociarIngredienteAtivo: function (response) {
		Mensagem.limpar(Agrotoxico.container);

		if (response.SituacaoId != Agrotoxico.settings.idsTelaIngredienteAtivoSituacao.Ativo) {
			return [Agrotoxico.settings.mensagens.IngredienteAtivoDesativado];
		}

		var itemAdicionado = false;
		$('.gridIngredientesAtivos tbody tr:not(.trTemplate) .hdnItemJson', Agrotoxico.container).each(function () {
			var itemLinha = JSON.parse($(this).val());
			if (itemLinha.Id == response.Id) {
				itemAdicionado = true;
			}
		});

		if (itemAdicionado) {
			return [Agrotoxico.settings.mensagens.IngredienteAtivoAdicionado];
		}

		$('.txtIngredienteAtivo', Agrotoxico.container).val(response.Texto);
		$('.hdnIngredienteAtivoJson', Agrotoxico.container).val(JSON.stringify(response));
		return true;
	},

	changeIngredienteAtivoUnidadeMedida: function () {
		var ocultarCampoOutro = $('.ddlIngredienteAtivoUnidadeMedida', Agrotoxico.container).val() != Agrotoxico.settings.ingredienteAtivoUnidadeMedida.Outros;
		$('.divUnidadeMedidaOutro', Agrotoxico.container).toggleClass('hide', ocultarCampoOutro);

		if (ocultarCampoOutro) {
			$('.txtUnidadeMedidaOutro', Agrotoxico.container).val('');
		}
	},

	onAddIngredienteAtivo: function () {
		Mensagem.limpar(Agrotoxico.container);
		var msg = [];

		var IngredienteAtivo = JSON.parse($('.hdnIngredienteAtivoJson', Agrotoxico.container).val() || null);
		if (IngredienteAtivo == null) {
			IngredienteAtivo = {};
			msg.push(Agrotoxico.settings.mensagens.IngredienteAtivoObrigatorio);
		}

		var concentracaoUnidadeMedida = '';
		var ddlUnidadeMedida = $('.ddlIngredienteAtivoUnidadeMedida', Agrotoxico.container).ddlSelecionado();
		IngredienteAtivo.Concentracao = Mascara.getFloatMask($('.txtConcentracao', Agrotoxico.container).val());
		IngredienteAtivo.UnidadeMedidaId = ddlUnidadeMedida.Id;
		IngredienteAtivo.UnidadeMedidaTexto = ddlUnidadeMedida.Texto;
		IngredienteAtivo.UnidadeMedidaOutro = $('.txtUnidadeMedidaOutro', Agrotoxico.container).val();

		//if (IngredienteAtivo.Concentracao <= 0) {
		//	msg.push(Agrotoxico.settings.mensagens.ConcentracaoObrigatorio);
		//}

		//if (IngredienteAtivo.UnidadeMedidaId <= 0) {
		//	msg.push(Agrotoxico.settings.mensagens.UnidadeMedidaObrigatoria);
		//}

		if (IngredienteAtivo.UnidadeMedidaId == Agrotoxico.settings.ingredienteAtivoUnidadeMedida.Outros) {
			concentracaoUnidadeMedida = IngredienteAtivo.Concentracao + ' ' + IngredienteAtivo.UnidadeMedidaOutro;
			//if (IngredienteAtivo.UnidadeMedidaOutro == '') {
			//	msg.push(Agrotoxico.settings.mensagens.UnidadeMedidaOutroObrigatorio);
			//}
		} else {
			concentracaoUnidadeMedida = IngredienteAtivo.Concentracao + (IngredienteAtivo.UnidadeMedidaId > 0 ? ' ' + IngredienteAtivo.UnidadeMedidaTexto:'');
		}

		if (msg.length > 0) {
			Mensagem.gerar(MasterPage.getContent(Agrotoxico.container), msg);
			return;
		}

		var tabela = $('.gridIngredientesAtivos tbody', Agrotoxico.container);
		var linha = $('.trTemplate', tabela).clone();

		$('.lblNome', linha).append(IngredienteAtivo.Texto);
		$('.lblConcentracao', linha).append(concentracaoUnidadeMedida);
		$('.lblSituacao', linha).append(IngredienteAtivo.SituacaoTexto);
		$('.hdnItemJson', linha).val(JSON.stringify(IngredienteAtivo));

		$(linha).removeClass('hide');
		$(linha).removeClass('trTemplate');

		$(tabela).append(linha);
		Listar.atualizarEstiloTable(Agrotoxico.container);
		$('.ingredienteAtivo input', Agrotoxico.container).val('');
		$('.ddlIngredienteAtivoUnidadeMedida', Agrotoxico.container).ddlFirst();
		Agrotoxico.changeIngredienteAtivoUnidadeMedida();

		if (IngredienteAtivo.SituacaoId != Agrotoxico.settings.idsTelaIngredienteAtivoSituacao.Ativo) {
			$('.rbCadastroAtivoNao', Agrotoxico.container).trigger('click');
			$('.rbCadastroAtivoNao', Agrotoxico.container).trigger('change');
		}
	},

	onAddGrupoQuimico: function () {
		Mensagem.limpar(Agrotoxico.container);
		var msg = [];

		if ($('.ddlGrupoQuimico :selected', Agrotoxico.container).val() == 0) {
			msg.push(Agrotoxico.settings.mensagens.SelecioneUmGrupoQuimico);
		}

		if (msg.length > 0) {
			Mensagem.gerar(MasterPage.getContent(Agrotoxico.container), msg);
			return;
		}

		grupoQuimico = {
			Id: +$('.ddlGrupoQuimico :selected', Agrotoxico.container).val(),
			Texto: $('.ddlGrupoQuimico :selected', Agrotoxico.container).text()
		};

		var itemAdicionado = false;
		$('.gridGruposQuimicos tbody tr:not(.trTemplate) .hdnItemJson', Agrotoxico.container).each(function () {
			var itemLinha = JSON.parse($(this).val());
			if (itemLinha.Id == grupoQuimico.Id) {
				itemAdicionado = true;
			}
		});

		if (itemAdicionado) {
			Mensagem.gerar(MasterPage.getContent(Agrotoxico.container), [Agrotoxico.settings.mensagens.GrupoQuimicoAdicionado]);
			return;
		}

		var tabela = $('.gridGruposQuimicos tbody', Agrotoxico.container);
		var linha = $('.trTemplate', tabela).clone();

		$('.lblNome', linha).append(grupoQuimico.Texto);
		$('.hdnItemJson', linha).val(JSON.stringify(grupoQuimico));

		$(linha).removeClass('hide');
		$(linha).removeClass('trTemplate');

		$(tabela).append(linha);
		Listar.atualizarEstiloTable(Agrotoxico.container);
		$('.ddlGrupoQuimico', Agrotoxico.container).val(0);
	},

	abrirModalCultura: function () {
		$('.gridCulturas tbody tr', Agrotoxico.container).removeClass('itemEdicao');
		Modal.abrir(Agrotoxico.settings.urls.agrotoxicoCulturaCriar, null,
			function (content) {
				AgrotoxicoCultura.load(content, { onSalvarCallBack: Agrotoxico.callBackAdicionarCultura });
				Modal.defaultButtons(content, AgrotoxicoCultura.salvar, 'Adicionar');
			}, Modal.tamanhoModalGrande, 'Adicionar Cultura');
	},

	callBackAdicionarCultura: function (resposta) {

		var tabela = $('.gridCulturas tbody', Agrotoxico.container);
		var linha = $('.trTemplate', tabela).clone();

		$('.lblNome', linha).append(resposta.Cultura.Nome);
		$('.hdnItemJson', linha).val(JSON.stringify(resposta));

		$(linha).removeClass('hide');
		$(linha).removeClass('trTemplate');

		$(tabela).append(linha);			
		$('.gridCulturas', Agrotoxico.container).tablesorter({ sortList: [[0,0]] });
		Listar.atualizarEstiloTable($('.gridCulturas', Agrotoxico.container));
		return true;
	},

	abrirModalCulturaEditar: function () {
		$('.gridCulturas tbody tr', Agrotoxico.container).removeClass('itemEdicao');
		$(this).closest('tr').addClass('itemEdicao');
		var objeto = JSON.parse($('.hdnItemJson', $(this).closest('tr')).val());

		Modal.abrir(Agrotoxico.settings.urls.agrotoxicoCulturaEditar, objeto,
			function (content) {
				AgrotoxicoCultura.load(content, { onSalvarCallBack: Agrotoxico.callBackEditarCultura });
				Modal.defaultButtons(content, AgrotoxicoCultura.salvar, "Editar");
			},
			Modal.tamanhoModalGrande, 'Editar Cultura');
	},

	callBackEditarCultura: function (resposta) {
		var tabela = $('.gridCulturas tbody', Agrotoxico.container);
		var linha = $('.itemEdicao', tabela);

		$('.lblNome', linha).text(resposta.Cultura.Nome);
		$('.hdnItemJson', linha).val(JSON.stringify(resposta));

		$(linha).removeClass('itemEdicao');
		Listar.atualizarEstiloTable(Agrotoxico.container);
		return true;
	},

	abrirModalCulturaCopiar: function () {
		$('.gridCulturas tbody tr', Agrotoxico.container).removeClass('itemEdicao');
		var objeto = JSON.parse($('.hdnItemJson', $(this).closest('tr')).val());
		objeto.IntervaloSeguranca = '';
		objeto.Cultura = {};
		objeto.IdRelacionamento = 0;

		Modal.abrir(Agrotoxico.settings.urls.agrotoxicoCulturaCriar, objeto,
			function (content) {
				AgrotoxicoCultura.load(content, { onSalvarCallBack: Agrotoxico.callBackAdicionarCultura });
				Modal.defaultButtons(content, AgrotoxicoCultura.salvar, "Adicionar");
			}, Modal.tamanhoModalGrande, 'Copiar Cultura');
	},

	abrirModalCulturaVisualizar: function () {
		var objeto = JSON.parse($('.hdnItemJson', $(this).closest('tr')).val());

		Modal.abrir(Agrotoxico.settings.urls.agrotoxicoCulturaVisualizar, objeto,
			function (content) {
				//AgrotoxicoCultura.load(content, { onSalvarCallBack: Agrotoxico.callBackEditarCultura });
				Modal.defaultButtons(content);
			},
			Modal.tamanhoModalGrande, 'Visualizar Cultura');
	},

	excluirItemGrid: function () {
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable(Agrotoxico.container);
	},

	obterMensagemAgrotoxicoDesativado: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: Agrotoxico.settings.urls.obterMensagemAgrotoxicoDesativado,
			data: JSON.stringify({ agrotoxico: Agrotoxico.obter() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Valido) {
					$('.txtMotivo', Agrotoxico.container).val(response.MotivoTexto);
					$('.hdnMotivoId', Agrotoxico.container).val(response.MotivoId);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Agrotoxico.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	obter: function () {
		var agrotoxico = {
			Id: +$('.hdnAgrotoxicoId', Agrotoxico.container).val(),
			PossuiCadastro: $('.rbPossuiNumCadastroSim', Agrotoxico.container).is(':checked'),
			NumeroCadastro: +$('.txtNumeroCadastro', Agrotoxico.container).val() || 0,
			CadastroAtivo: $('.rbCadastroAtivoSim', Agrotoxico.container).is(':checked'),
			MotivoId: $('.hdnMotivoId', Agrotoxico.container).val(),
			NomeComercial: $('.txtNomeComercial', Agrotoxico.container).val(),
			NumeroRegistroMinisterio: +$('.txtNumeroRegistroMinisterio', Agrotoxico.container).val(),
			NumeroProcessoSep: +$('.txtNumeroProcessoSep', Agrotoxico.container).val(),
			ObservacaoInterna: $('.txtObservacaoInterna', Agrotoxico.container).val(),
			ObservacaoGeral: $('.txtObservacaoGeral', Agrotoxico.container).val(),
			TitularRegistro: {
				Id: +$('.hdnTitularRegistroId', Agrotoxico.container).val()
			},
			Bula: JSON.parse($('.hdnArquivoJson', Agrotoxico.container).val() || null),
			ClassificacaoToxicologica: {
				Id: +$('.ddlClassificacaoToxicologica :selected', Agrotoxico.container).val()
			},
			PericulosidadeAmbiental: {
				Id: +$('.ddlPericulosidadeAmbiental :selected', Agrotoxico.container).val()
			},
			FormaApresentacao: {
				Id: +$('.ddlFormaApresentacao :selected', Agrotoxico.container).val()
			},
			IngredientesAtivos: [],
			ClassesUso: [],
			GruposQuimicos: [],
			Culturas: []
		}

		$('.gridIngredientesAtivos tbody tr:not(.trTemplate) .hdnItemJson', Agrotoxico.container).each(function () {
			agrotoxico.IngredientesAtivos.push(JSON.parse($(this).val()));
		});

		$('.gridGruposQuimicos tbody tr:not(.trTemplate) .hdnItemJson', Agrotoxico.container).each(function () {
			agrotoxico.GruposQuimicos.push(JSON.parse($(this).val()));
		});

		$('.gridCulturas tbody tr:not(.trTemplate) .hdnItemJson', Agrotoxico.container).each(function () {
			agrotoxico.Culturas.push(JSON.parse($(this).val()));
		});

		$('.cbClasseUso:checked', Agrotoxico.container).each(function () {
			var item = $(this).closest('div').find('.hdnItemJson').val();
			agrotoxico.ClassesUso.push(JSON.parse(item));
		});

		return agrotoxico;
	},

	salvar: function () {
		Mensagem.limpar(Agrotoxico.container);
		MasterPage.carregando(true);
		$.ajax({
			url: Agrotoxico.settings.urls.salvar,
			data: JSON.stringify({ agrotoxico: Agrotoxico.obter() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Valido) {
					MasterPage.redireciona(response.Url);
				}

				if (response.Erros && response.Erros.length > 0) {
					Mensagem.gerar(Agrotoxico.container, response.Erros);
				}
			}
		});
		MasterPage.carregando(false);
	},

	enviarArquivo: function () {
		var nomeArquivo = $('#fileBula', Agrotoxico.container).val();

		erroMsg = new Array();

		var tam = nomeArquivo.length - 4;
		if (!Agrotoxico.validarTipoArquivo(nomeArquivo.toLowerCase().substr(tam))) {
			erroMsg.push(Agrotoxico.settings.mensagens.ArquivoDeveSerPDF);
		}

		if (erroMsg.length > 0) {
			Mensagem.gerar(Agrotoxico.container, erroMsg);
			return;
		}

		MasterPage.carregando(true);
		var inputFile = $('#fileBula', Agrotoxico.container);
		FileUpload.upload(Agrotoxico.settings.urls.enviarArquivo, inputFile, Agrotoxico.callBackArqEnviado);
	},

	callBackArqEnviado: function (controle, resposta, isHtml) {
		var ret = eval('(' + resposta + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoNome', Agrotoxico.container).text(ret.Arquivo.Nome);
			$('.hdnArquivoJson', Agrotoxico.container).val(JSON.stringify(ret.Arquivo));
			$('.txtArquivoNome', Agrotoxico.container).attr('href', '/Arquivo/BaixarTemporario?nomeTemporario=' + ret.Arquivo.TemporarioNome + '&contentType=' + ret.Arquivo.ContentType);

			$('.spanInputFile', Agrotoxico.container).addClass('hide');
			$('.txtArquivoNome', Agrotoxico.container).removeClass('hide');

			$('.btnAddArq', Agrotoxico.container).addClass('hide');
			$('.btnLimparArq', Agrotoxico.container).removeClass('hide');

			Mensagem.limpar(Agrotoxico.container);
			Mensagem.gerar(Agrotoxico.container, ret.Msg);
		} else {
			Agrotoxico.onLimparArquivoClick();
			Mensagem.gerar(Agrotoxico.container, ret.Msg);
		}

		MasterPage.carregando(false);
	},

	onLimparArquivoClick: function () {
		$('.hdnArquivoJson', Agrotoxico.container).val('');
		$('.inputFile', Agrotoxico.container).val('');

		$('.spanInputFile', Agrotoxico.container).removeClass('hide');
		$('.txtArquivoNome', Agrotoxico.container).addClass('hide');

		$('.btnAddArq', Agrotoxico.container).removeClass('hide');
		$('.btnLimparArq', Agrotoxico.container).addClass('hide');

		Mensagem.limpar(Agrotoxico.container);
	},

	validarTipoArquivo: function (tipo) {
		var tipoValido = false;
		$(Agrotoxico.settings.tiposArquivo).each(function (i, tipoItem) {
			if (tipoItem == tipo) {
				tipoValido = true;
			}
		});

		return tipoValido;
	}
}
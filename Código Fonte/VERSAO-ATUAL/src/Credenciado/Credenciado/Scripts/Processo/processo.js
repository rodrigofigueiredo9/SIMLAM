/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

Processo = {
	settings: {
		container: null,
		containerModal: null,
		urls: {
			enviarArquivo: '',
			associarCheckList: '',
			visualizarCheckList: '',
			associarRequerimento: '',
			associarRequerimentoValidar: '',
			obterRequerimento: '',
			pdfRequerimento: '',
			associarInteressado: '',
			editarInteressado: '',
			associarEmpreendimento: '',
			editarEmpreendimento: '',
			associarResponsavelModal: '',
			editarResponsavelModal: '',
			validarChecagem: '',
			validarChecagemTemTituloPendencia: '',
			salvar: '',
			autuar: ''
		},
		Mensagens: null,
		configuracoesProtocoloTipos: null,
		modo: 1 //Modo Cadastrar
	},

	pessoaModalInte: null,
	pessoaModalResp: null,

	load: function (container, options) {
		if (options) {
			$.extend(Processo.settings, options);
		}

		container = MasterPage.getContent(container);
		Processo.settings.container = container;
		Processo.settings.containerModal = $('.processoPartial', container);

		Processo.setarBotoes(Processo.settings.container);
		Processo.configurarAssociarMultiplo();
	},

	setarBotoes: function (container) {
		container.delegate('.ddlProcessoTipos', 'change', Processo.changeProcessoTipo);
		container.delegate('.btnLimparEmp', 'click', Processo.onLimparEmp);
		container.delegate('.btnLimparCheckList', 'click', Processo.onLimparCheckList);
		container.delegate('.btnLimparRequerimento', 'click', Processo.onLimparRequerimento);
		container.delegate('.btnAssociarInteressado', 'click', Processo.onAssociarInteressado);
		container.delegate('.btnEditarInteressado', 'click', Processo.onEditarInteressado);
		container.delegate('.btnAssociarEmp', 'click', Processo.onAssociarEmp);
		container.delegate('.btnEditarEmp', 'click', Processo.onEditarEmp);
		container.delegate('.btnArqComplementar', 'click', Processo.onEnviarArquivoClick);
		container.delegate('.btnArqComplementarLimpar', 'click', Processo.onLimparArquivoClick);
		container.delegate('.btnBuscarCheckList', 'click', Processo.onAssociarCheckList);
		container.delegate('.btnBuscarRequerimento', 'click', Processo.onAssociarRequerimento);
		container.delegate('.btnVisualizarChecagem', 'click', Processo.onAbrirVisualizarCheckList);
		container.delegate('.btnPdfRequerimento', 'click', Processo.onAbrirPdfRequerimento);
		container.delegate('.btnProcessoSalvar', 'click', Processo.salvar);
		container.delegate('.btnProcessoAutuar', 'click', Processo.autuar);
	},

	configurarAssociarMultiplo: function () {
		$('.divConteudoAtividadeSolicitada', Processo.settings.container).associarMultiplo({
			'associarModalLoadFunction': 'AtividadeSolicitadaListar.load',
			'expandirAutomatico': false,
			'minItens': 1,
			'tamanhoModal': Modal.tamanhoModalGrande
		});

		Processo.pessoaModalResp = new PessoaAssociar();

		$('.divConteudoResponsavelTec', Processo.settings.container).associarMultiplo({
			'onAssociar': Processo.associarResponsavelTecnico,
			'associarUrl': Processo.settings.urls.associarResponsavelModal,
			'associarModalObject': Processo.pessoaModalResp,
			'associarModalLoadFunction': Processo.pessoaModalResp.load,
			'associarModalLoadParams': {
				tituloCriar: 'Cadastrar Responsável',
				tituloEditar: 'Editar Responsável',
				tituloVisualizar: 'Visualizar Responsável',
				visualizando: true,
				editarVisualizar: false
			},

			'onEditar': Processo.onResponsavelEditar,
			'editarUrl': Processo.settings.urls.editarResponsavelModal,
			'editarModalObject': Processo.pessoaModalResp,
			'editarModalLoadFunction': Processo.pessoaModalResp.load,
			'editarModalLoadParams': {
				tituloCriar: 'Cadastrar Responsável',
				tituloEditar: 'Editar Responsável',
				tituloVisualizar: 'Visualizar Responsável',
				visualizando: true,
				editarVisualizar: false
			},

			'mostrarBtnLimpar': true,
			'expandirAutomatico': true,
			'minItens': 1,
			'tamanhoModal': Modal.tamanhoModalGrande,
			'onExpandirEsconder': function () { MasterPage.redimensionar(); },
			'msgObrigatoriedade': Processo.settings.Mensagens.ResponsavelTecnicoSemPreencher,

			'btnOkLabelExcluir': 'Remover',
			'tituloExcluir': 'Remover Responsável Técnico',
			'msgExcluir': Processo.onMensagemExcluirResponsavel
		});

		var containerLink = $('.conteudoResponsaveis .asmConteudoInterno', Processo.settings.container);
		if (!containerLink.hasClass('hide')) {
			$('.conteudoResponsaveis', Processo.settings.container).find('.btnAsmEditar').addClass('hide');
		}
	},

	obterConfiguracaoTipo: function () {
		var configuracao = null;
		$.each(Processo.settings.configuracoesProtocoloTipos, function (i, item) {
			if (item.Id == $('.ddlProcessoTipos', Processo.settings.container).val()) {
				configuracao = item;
			}
		});

		return configuracao;
	},

	changeProcessoTipo: function () {
		var checagemId = parseInt($('.txtCheckListId', Processo.settings.container).val());
		var requerimentoId = parseInt($('.txtNumeroReq', Processo.settings.container).val());

		var configuracao = Processo.obterConfiguracaoTipo();

		$('.containerChecagemRoteiro', Processo.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiChecagemRoteiro && !configuracao.ChecagemRoteiroObrigatorio)));
		$('.containerRequerimento', Processo.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiRequerimento && !configuracao.RequerimentoObrigatorio)));
		$('.containerInteressado', Processo.settings.container).toggleClass('hide', (configuracao == null || configuracao.RequerimentoObrigatorio || requerimentoId > 0));

		$('.containerChecagemRoteiro', Processo.settings.container).toggleClass('apagar', (configuracao == null || (!configuracao.PossuiChecagemRoteiro && !configuracao.ChecagemRoteiroObrigatorio)));
		$('.containerRequerimento', Processo.settings.container).toggleClass('apagar', (configuracao == null || (!configuracao.PossuiRequerimento && !configuracao.RequerimentoObrigatorio)));
		$('.containerInteressado', Processo.settings.container).toggleClass('apagar', (configuracao == null || configuracao.RequerimentoObrigatorio || requerimentoId > 0));

		$('.apagar', Processo.settings.container).find('input').val('');
		$('.apagar', Processo.settings.container).find('input[type=hidden]').val(0);
		$('.apagar', Processo.settings.container).find('.icone').closest('span').addClass('hide');
		$('.apagar', Processo.settings.container).find('.divRequerimento').empty();
	},

	onAssociarInteressado: function () {
		Processo.pessoaModalInte = new PessoaAssociar();

		Modal.abrir(Processo.settings.urls.associarInteressado, null, function (container) {
			Processo.pessoaModalInte.load(container, {
				tituloCriar: 'Cadastrar Interessado',
				tituloEditar: 'Editar Interessado',
				tituloVisualizar: 'Visualizar Interessado',
				onAssociarCallback: Processo.callBackEditarInteressado
			});
		});
	},

	onEditarInteressado: function () {
		var id = $('.hdnInteressadoId', Processo.settings.container).val();
		Processo.pessoaModalInte = new PessoaAssociar();

		Modal.abrir(Processo.settings.urls.editarInteressado + "/" + id, null, function (container) {
			Processo.pessoaModalInte.load(container, {
				tituloCriar: 'Cadastrar Interessado',
				tituloEditar: 'Editar Interessado',
				tituloVisualizar: 'Visualizar Interessado',
				onAssociarCallback: Processo.callBackEditarInteressado,
				visualizando: true,
				editarVisualizar: (Processo.settings.modo !== 3)//visualizar
			});
		});
	},

	callBackEditarInteressado: function (Pessoa) {
		$('.spanVisualizarInteressado', Processo.settings.container).removeClass('hide');
		$('.hdnInteressadoId', Processo.settings.container).val(Pessoa.Id);
		$('.txtIntNome', Processo.settings.container).val(Pessoa.NomeRazaoSocial);
		$('.txtIntCnpj', Processo.settings.container).val(Pessoa.CPFCNPJ);
		return true;
	},

	onAssociarEmp: function () {
		Modal.abrir(Processo.settings.urls.associarEmpreendimento, null, function (container) {
			EmpreendimentoListar.load(container, { associarFuncao: Processo.callBackEditarEmp });
			Modal.defaultButtons(container);
		});
	},

	onEditarEmp: function () {
		var id = $('.hdnEmpreendimentoId', Processo.settings.container).val();

		Modal.abrir(Processo.settings.urls.editarEmpreendimento + "/" + id, null, function (container) {
			EmpreendimentoAssociar.load(container, {
				onAssociarCallback: Processo.callBackEditarEmp,
				editarVisualizar: (Processo.settings.modo !== 3)//visualizar
			});
			Modal.defaultButtons(container);
		});
	},

	callBackEditarEmp: function (Empreendimento) {
		$('.hdnEmpreendimentoId', Processo.settings.container).val(Empreendimento.Id);
		$('.txtEmpDenominador', Processo.settings.container).val(Empreendimento.Denominador);
		$('.tctEmpCnpj', Processo.settings.container).val(Empreendimento.CNPJ);
		$('.spanBtnEditarEmp', Processo.settings.container).removeClass('hide');
		$('.spanBtnAssociarEmp', Processo.settings.container).addClass('hide');
		return true;
	},

	onLimparEmp: function () {
		$('.hdnEmpreendimentoId', Processo.settings.container).val('');
		$('.txtEmpDenominador', Processo.settings.container).val('');
		$('.tctEmpCnpj', Processo.settings.container).val('');
		$('.spanBtnEditarEmp', Processo.settings.container).addClass('hide');
		$('.spanBtnAssociarEmp', Processo.settings.container).removeClass('hide');
	},

	onAssociarCheckList: function () {
		if ($('.hdnProcessoId', Processo.settings.container).val() != '0' &&
			 (!MasterPage.validarAjax(Processo.settings.urls.validarChecagemTemTituloPendencia, { processoId: $('.hdnProcessoId', Processo.settings.container).val() }, Processo.settings.container, false).EhValido)) {
			return;
		}

		Modal.abrir(Processo.settings.urls.associarCheckList, null, function (container) {
			Modal.defaultButtons(container);
			ChecagemRoteirotListar.load(container, { associarFuncao: Processo.callBackAssociarCheckList });
		});
	},

	callBackAssociarCheckList: function (CheckList) {
		var retorno = MasterPage.validarAjax(Processo.settings.urls.validarChecagem,
			{ id: CheckList.Id, processoId: $('.hdnProcessoId', Processo.settings.container).val() }, null, false);

		if (!retorno.EhValido && retorno.Msg) {
			return retorno.Msg;
		}

		$('.txtCheckListId', Processo.settings.container).val(CheckList.Id);
		$('.spnVisualizarChecagem', Processo.settings.container).removeClass('hide');
		Processo.ocultarInteressado();

		$('.btnLimparCheckList', Processo.settings.container).removeClass('hide');
		$('.btnBuscarCheckList', Processo.settings.container).addClass('hide');
		return true;
	},

	onLimparCheckList: function () {
		if ($('.hdnProcessoId', Processo.settings.container).val() != '0' &&
			 (!MasterPage.validarAjax(Processo.settings.urls.validarChecagemTemTituloPendencia, { processoId: $('.hdnProcessoId', Processo.settings.container).val() }, Processo.settings.container, false).EhValido))
			return;

		$('.txtCheckListId', Processo.settings.container).val('');
		$('.btnLimparCheckList', Processo.settings.container).addClass('hide');
		$('.spnVisualizarChecagem', Processo.settings.container).addClass('hide');
		$('.btnBuscarCheckList', Processo.settings.container).removeClass('hide');

		var configuracao = Processo.obterConfiguracaoTipo();

		if ($('.txtNumeroReq', Processo.settings.container).val() == '' && (configuracao == null || !configuracao.RequerimentoObrigatorio)) {
			$('.containerInteressado', Processo.settings.container).removeClass('hide');
		}
	},

	ocultarInteressado: function () {
		$('.containerInteressado', Processo.settings.container).addClass('hide');
		$('.hdnInteressadoId', Processo.settings.container.find('.containerInteressado')).val('0');
		$('.txtIntNome', Processo.settings.container.find('.containerInteressado')).val('');
		$('.txtIntCnpj', Processo.settings.container.find('.containerInteressado')).val('');

		$('.spanVisualizarInteressado', Processo.settings.container).addClass('hide');
	},

	onAssociarRequerimento: function () {

		var params = { id: $('.txtNumeroReq', Processo.settings.container).val(), processoId: $('.hdnProcessoId', Processo.settings.container).val() };

		if (Processo.validarProcessoTituloAbrirModal(params)) {
			Modal.abrir(Processo.settings.urls.associarRequerimento, null, function (container) {
				RequerimentoListar.load(container, { associarFuncao: Processo.callBackAssociarRequerimento });
				Modal.defaultButtons(container);
			});
		}
	},

	validarProcessoTituloAbrirModal: function (params) {

		var abreModal = params.processoId == 0 || params.id == 0;

		if (params.processoId > 0 && params.id > 0) {
			$.ajax({ url: Processo.settings.urls.associarRequerimentoValidar, data: JSON.stringify(params), type: 'POST', typeData: 'json',
				contentType: 'application/json; charset=utf-8', cache: false, async: false,
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Processo.settings.container));
				},
				success: function (response, textStatus, XMLHttpRequest) {
					if (response.EhValido) {
						abreModal = response.EhValido;
					} else if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(MasterPage.getContent(Processo.settings.container), response.Msg);
					}
				}
			});
		}
		return abreModal;
	},

	callBackAssociarRequerimento: function (Requerimento) {
		if ($('.txtNumeroReq', Processo.settings.container).val() == Requerimento.Id) {
			return true;
		}

		var params = { id: Requerimento.Id, excetoId: $('.hdnProcessoId', Processo.settings.container).val() };
		var retorno = Processo.obterAjax(Processo.settings.urls.obterRequerimento, params, $('.divRequerimento', Processo.settings.containerModal));

		if (!retorno.EhValido) {
			return retorno.Msg;
		}

		$('.txtNumeroReq', Processo.settings.container).val(Requerimento.Id);
		$('.hdnRequerimentoSituacao', Processo.settings.container).val(Requerimento.SituacaoId);
		$('.spnPdfRequerimento', Processo.settings.container).removeClass('hide');

		Processo.configurarAssociarMultiplo();
		Processo.ocultarInteressado();

		$('.btnLimparRequerimento', Processo.settings.container).removeClass('hide');
		$('.btnBuscarRequerimento', Processo.settings.container).addClass('hide');
		return true;
	},

	onLimparRequerimento: function () {

		var params = { id: $('.txtNumeroReq', Processo.settings.container).val(), processoId: $('.hdnProcessoId', Processo.settings.container).val() };

		if (Processo.validarProcessoTituloAbrirModal(params)) {

			$('.txtNumeroReq', Processo.settings.container).val('');
			$('.btnLimparRequerimento', Processo.settings.container).addClass('hide');
			$('.spnPdfRequerimento', Processo.settings.container).addClass('hide');
			$('.btnBuscarRequerimento', Processo.settings.container).removeClass('hide');

			$('.divRequerimento', Processo.settings.container).empty();

			var configuracao = Processo.obterConfiguracaoTipo();

			if ($('.txtCheckListId', Processo.settings.container).val() == '' && (configuracao == null || !configuracao.RequerimentoObrigatorio)) {
				$('.containerInteressado', Processo.settings.container).removeClass('hide');
			}
		}
	},

	onAbrirVisualizarCheckList: function () {
		var id = parseInt($('.txtCheckListId', Processo.settings.container).val());
		var params = { id: id };

		Modal.abrir(Processo.settings.urls.visualizarCheckList, params, function (container) {
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);
	},

	onAbrirPdfRequerimento: function () {
		var id = parseInt($('.txtNumeroReq', Processo.settings.container).val());
		MasterPage.redireciona(Processo.settings.urls.pdfRequerimento + "?id=" + id);
		//MasterPage.carregando(false);
	},

	obterAjax: function (url, params, container) {
		//MasterPage.carregando(true);
		var retorno = null;

		$.ajax({ url: url, data: params, cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Processo.settings.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				Mensagem.limpar(MasterPage.getContent(Processo.settings.container));

				if (response.EhValido || response.SetarHtml) {
					$(container).empty();
					$(container).append(response.Html);
					Mascara.load(container);
					MasterPage.botoes(container);
					//MasterPage.redimensionar();
				}

				retorno = $(response).removeData('Html');
			}
		});
		//MasterPage.carregando(false);
		return retorno[0];
	},

	//--------------- RESPONSÁVEL TÉCNICO ---------------\\
	associarResponsavelTecnico: function (responsavel, item, extra) {
		var jaExiste = false;

		$('.asmItemContainer', $(item).closest('.asmItens')).each(function () {
			if ($('.hdnResponsavelId', this).val() == responsavel.Id) {
				jaExiste = true;
				return;
			}
		});

		if (jaExiste) {
			return new Array(Processo.settings.Mensagens.ResponsaveljaAdicionado);
		}

		$('.hdnResponsavelId', item).val(responsavel.Id);
		$('.nomeRazao', item).val(responsavel.NomeRazaoSocial);
		$('.cpfCnpj', item).val(responsavel.CPFCNPJ);
		$('.btnAsmEditar', item).removeClass('hide');
		return true;
	},

	onResponsavelEditar: function (pessoaObj, item, extra) {
		$('.hdnResponsavelId', item).val(pessoaObj.Id);
		$('.nomeRazao', item).val(pessoaObj.NomeRazaoSocial);
		$('.cpfCnpj', item).val(pessoaObj.CPFCNPJ);
	},

	onMensagemExcluirResponsavel: function (item, extra) {
		return Mensagem.replace(Processo.settings.Mensagens.ResponsavelTecnicoRemover, '#texto', $(item).find('.asmItemTexto').val()).Texto;
	},

	//--------------- ENVIAR ARQUIVO ---------------\\
	onEnviarArquivoClick: function () {
		var nomeArquivo = $('.inputFile', Processo.settings.container).val();

		if (nomeArquivo === '') {
			Mensagem.gerar(MasterPage.getContent(Processo.settings.container), new Array(Processo.settings.Mensagens.ArquivoObrigatorio));
			return;
		}

		$('.btnArqComplementar', Processo.settings.container).button({ disabled: true });
		var inputFile = $('.inputFile', Processo.settings.container);
		FileUpload.upload(Processo.settings.urls.enviarArquivo, inputFile, Processo.callBackArqEnviado);
	},

	callBackArqEnviado: function (controle, retorno, isHtml) {
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoNome', Processo.settings.container).val(ret.Arquivo.Nome);
			$('.hdnArquivoJson', Processo.settings.container).val(JSON.stringify(ret.Arquivo));

			$('.spanInputFile', Processo.settings.container).addClass('hide');
			$('.txtArquivoNome', Processo.settings.container).removeClass('hide');

			$('.btnArqComplementar', Processo.settings.container).addClass('hide');
			$('.btnArqComplementarLimpar', Processo.settings.container).removeClass('hide');
		} else {
			Processo.onLimparArquivoClick();
		}

		Mensagem.gerar(MasterPage.getContent(Processo.settings.container), ret.Msg);
		$('.btnArqComplementar', Processo.settings.container).button({ disabled: false });
	},

	onLimparArquivoClick: function () {
		$('.hdnArquivoJson', Processo.settings.container).val('');
		$('.inputFile', Processo.settings.container).val('');

		$('.spanInputFile', Processo.settings.container).removeClass('hide');
		$('.txtArquivoNome', Processo.settings.container).addClass('hide');

		$('.btnArqComplementar', Processo.settings.container).removeClass('hide');
		$('.btnArqComplementarLimpar', Processo.settings.container).addClass('hide');
		$('.lnkArquivo', Processo.settings.container).remove();
	},

	//--------------- SALVAR PROCESSO ---------------\\
	montarObjetoProcesso: function () {

		var objetoProcesso = {
			Id: 0,
			Numero: '',
			Tipo: { Id: 0 },
			DataCadastro: null,
			Volume: 0,
			IsProcesso: true,
			Arquivo: null,
			ChecagemRoteiro: { Id: 0 },
			Requerimento: { Id: 0, SituacaoId: 0 },
			Interessado: { Id: 0 },
			Empreendimento: { Id: 0 },
			Atividades: [],
			Responsaveis: []
		};

		objetoProcesso.Id = $('.hdnProcessoId', Processo.settings.container).val();
		objetoProcesso.Numero = $('.txtNumero', Processo.settings.container).val();
		objetoProcesso.Tipo.Id = $('.ddlProcessoTipos', Processo.settings.container).val();
		objetoProcesso.DataCadastro = { DataTexto: $('.txtDataCriacao', Processo.settings.container).val() };
		objetoProcesso.Volume = $('.txtQuantidadeVolumes', Processo.settings.container).val();
		objetoProcesso.Arquivo = $.parseJSON($('.hdnArquivoJson', Processo.settings.container).val());
		objetoProcesso.ChecagemRoteiro.Id = $('.txtCheckListId', Processo.settings.container).val();
		objetoProcesso.Requerimento.Id = $('.txtNumeroReq', Processo.settings.container).val();
		objetoProcesso.Requerimento.SituacaoId = $('.hdnRequerimentoSituacao', Processo.settings.container).val();
		objetoProcesso.Interessado.Id = $('.hdnInteressadoId', $('.hdnInteressadoId', Processo.settings.container).closest('fieldset:visible')).val();
		objetoProcesso.Empreendimento.Id = $('.hdnEmpreendimentoId', Processo.settings.container).val();
		objetoProcesso.SetorId = $('.ddlSetor', Processo.settings.container).val();

		objetoProcesso.Atividades = AtividadeSolicitadaAssociar.gerarObjeto(Processo.settings.container);

		// Início Responsáveis
		var responsaveis = $('.divConteudoResponsavelTec .asmItens .asmItemContainer', Processo.settings.container);

		function responsavelTecnico() {
			this.Id = 0;
			this.NomeRazao = '';
			this.CpfCnpj = 0;
			this.Funcao = 0;
			this.NumeroArt = '';
			this.IdRelacionamento = 0;
		};

		responsaveis.each(function () {
			if (!$('.asmConteudoLink', this).hasClass('hide')) {
				var responsavel = new responsavelTecnico();
				responsavel.Id = $('.hdnResponsavelId', this).val();
				responsavel.NomeRazao = $('.nomeRazao', this).val();
				responsavel.CpfCnpj = $('.cpfCnpj', this).val();
				responsavel.Funcao = $('.funcao', this).val();
				responsavel.NumeroArt = $('.art', this).val();
				responsavel.IdRelacionamento = $('.hdnIdRelacionamento', this).val();

				objetoProcesso.Responsaveis.push(responsavel);
			}
		});
		// Fim Responsáveis

		return objetoProcesso;
	},

	salvar: function (isRedirecionar) {
		var processo = Processo.montarObjetoProcesso();
		//MasterPage.carregando(true);
		var params = { processo: processo };

		$.ajax({ url: Processo.settings.urls.salvar, data: JSON.stringify(params), type: 'POST', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Processo.settings.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.IsProcessoSalvo) {
					if (typeof response.UrlRedireciona != "undefined" && response.UrlRedireciona !== null && isRedirecionar) {
						MasterPage.redireciona(response.UrlRedireciona);
						return;
					}
				} else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Processo.settings.container), response.Msg);
				}
			}
		});
		//MasterPage.carregando(false);
	},

	autuar: function () {
		//MasterPage.carregando(true);
		var params = { processo: { Id: $('.hdnProcessoId', Processo.settings.container).val()} };

		$.ajax({ url: Processo.settings.urls.autuar, data: JSON.stringify(params), type: 'POST', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Processo.settings.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Processo.settings.container, response.Msg);
				}

				if (response.EhValido) {
					$('.txtDataAutuacao', Processo.settings.container).val(response.Processo.DataAutuacao.DataTexto);
					$('.txtNumeroAutuacao', Processo.settings.container).val(response.Processo.NumeroAutuacao);
					$('.btnProcessoAutuar', Processo.settings.container).remove();
				}
			}
		});
		//MasterPage.carregando(false);
	}
}
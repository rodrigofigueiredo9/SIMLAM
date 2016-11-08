/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

Documento = {
	settings: {
		container: null,
		containerModal: null,
		urls: {
			enviarArquivo: '',
			associarProcesso: '',
			associarChecagemPendencia: '',
			associarCheckList: '',
			associarRequerimento: '',
			associarRequerimentoValidar: '',
			visualizarProcesso: '',
			visualizarCheckList: '',
			visualizarChecagemPendencia: '',
			obterRequerimento: '',
			pdfRequerimento: '',
			associarInteressado: '',
			editarInteressado: '',
			associarEmpreendimento: '',
			editarEmpreendimento: '',
			associarResponsavelModal: '',
			editarResponsavelModal: '',
			validarChecagem: '',
			validarChecagemPendencia: '',
			validarChecagemTemTituloPendencia: '',
			obterProtocolo: '',
			salvar: ''
		},
		Mensagens: null,
		configuracoesProtocoloTipos: null,
		modo: 1 //Modo Cadastrar
	},

	pessoaModalInte: null,
	pessoaModalResp: null,

	load: function (container, options) {
		if (options) {
			$.extend(Documento.settings, options);
		}

		container = MasterPage.getContent(container);
		Documento.settings.container = container;
		Documento.settings.containerModal = $('.documentoPartial', container);

		Documento.setarBotoes(Documento.settings.container);
		Documento.configurarAssociarMultiplo();
	},

	setarBotoes: function (container) {
		container.delegate('.ddlDocumentoTipos', 'change', Documento.changeDocumentoTipo);
		container.delegate('.rdbProtocoloAssociadoTipo', 'change', Documento.onRdbProtocoloAssociadoTipo);
		//container.delegate('.rdbProtocoloAssociadoTipo', 'click', Documento.onRdbProtocoloAssociadoTipo);
		container.delegate('.btnArqComplementar', 'click', Documento.onEnviarArquivoClick);
		container.delegate('.btnArqComplementarLimpar', 'click', Documento.onLimparArquivoClick);
		container.delegate('.btnBuscarProcesso', 'click', Documento.onAssociarProtocolo);
		container.delegate('.btnBuscarChecagemPendencia', 'click', Documento.onAssociarChecagemPendencia);
		container.delegate('.btnBuscarCheckList', 'click', Documento.onAssociarCheckList);
		container.delegate('.btnBuscarRequerimento', 'click', Documento.onAssociarRequerimento);
		container.delegate('.btnVisualizarProcesso', 'click', Documento.onAbrirVisualizarProtocolo);
		container.delegate('.btnVisualizarChecagemPendencia', 'click', Documento.onAbrirVisualizarChecagemPendencia);
		container.delegate('.btnVisualizarChecagem', 'click', Documento.onAbrirVisualizarCheckList);
		container.delegate('.btnPdfRequerimento', 'click', Documento.onAbrirPdfRequerimento);
		container.delegate('.btnEditarInteressado', 'click', Documento.onEditarInteressado);
		container.delegate('.btnAssociarInteressado', 'click', Documento.onAssociarInteressado);
		container.delegate('.btnEditarEmp', 'click', Documento.onEditarEmp);
		container.delegate('.btnAssociarEmp', 'click', Documento.onAssociarEmp);
		container.delegate('.btnLimparEmp', 'click', Documento.onLimparEmp);
		container.delegate('.btnDocumentoSalvar', 'click', Documento.salvar);
	},

	configurarAssociarMultiplo: function () {
		$('.divConteudoAtividadeSolicitada', Documento.settings.container).associarMultiplo({
			'associarModalLoadFunction': 'AtividadeSolicitadaListar.load',
			'expandirAutomatico': false,
			'minItens': 1,
			'tamanhoModal': Modal.tamanhoModalGrande
		});

		Documento.pessoaModalResp = new PessoaAssociar();

		$('.divConteudoResponsavelTec', Documento.settings.container).associarMultiplo({
			'onAssociar': Documento.associarResponsavelTecnico,
			'associarUrl': Documento.settings.urls.associarResponsavelModal,
			'associarModalObject': Documento.pessoaModalResp,
			'associarModalLoadFunction': Documento.pessoaModalResp.load,
			'associarModalLoadParams': {
				tituloCriar: 'Cadastrar Responsável',
				tituloEditar: 'Editar Responsável',
				tituloVisualizar: 'Visualizar Responsável',
				visualizando: true,
				editarVisualizar: false
			},

			'onEditar': Documento.onResponsavelEditar,
			'editarUrl': Documento.settings.urls.editarResponsavelModal,
			'editarModalObject': Documento.pessoaModalResp,
			'editarModalLoadFunction': Documento.pessoaModalResp.load,
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
			'msgObrigatoriedade': Documento.settings.Mensagens.ResponsavelTecnicoSemPreencher,

			'btnOkLabelExcluir': 'Remover',
			'tituloExcluir': 'Remover Responsável Técnico',
			'msgExcluir': Documento.onMensagemExcluirResponsavel
		});

		var containerLink = $('.conteudoResponsaveis .asmConteudoInterno', Documento.settings.container);
		if (!containerLink.hasClass('hide')) {
			$('.conteudoResponsaveis', Documento.settings.container).find('.btnAsmEditar').addClass('hide');
		}
	},

	onRdbProtocoloAssociadoTipo: function () {
		$('.hdnProtocoloAssociadoId', Documento.settings.container).val('0');
		$('.txtProtocoloAssociadoNumero', Documento.settings.container).val('');
		$('.btnVisualizarProcesso', Documento.settings.container).addClass('hide');
	},

	changeDocumentoTipo: function () {
		$('.hdnProtocoloAssociadoId', Documento.settings.container).val('0');
		$('.txtProtocoloAssociadoNumero', Documento.settings.container).val('');
		$('.btnVisualizarProcesso', Documento.settings.container).addClass('hide');

		var checagemId = parseInt($('.txtCheckListId', Documento.settings.container).val());
		var requerimentoId = parseInt($('.txtNumeroReq', Documento.settings.container).val());

		var configuracao = null;
		var ddlDocTipoVal = $('.ddlDocumentoTipos', Documento.settings.container).val();
		$.each(Documento.settings.configuracoesProtocoloTipos, function (i, item) {
			if (item.Id == ddlDocTipoVal) {
				configuracao = item;
			}
		});

		var containerProcessoDocumentoVisivel = configuracao != null && (configuracao.PossuiProcesso || configuracao.ProcessoObrigatorio);
		var esconderAssociarDocumento = configuracao != null && !(configuracao.PossuiDocumento || configuracao.DocumentoObrigatorio);
		var tituloGrupo = esconderAssociarDocumento ? 'Processo' : 'Processo/Documento';

		$('.grupoProtocoloAssociadoNome', Documento.settings.container).text(tituloGrupo);
		$('.visivelSeAssociaDocumento', Documento.settings.container).toggleClass('hide', esconderAssociarDocumento);

		$('.containerProcessoDocumento', Documento.settings.container).toggleClass('hide', !containerProcessoDocumentoVisivel);
		$('.containerChecagemPendencia', Documento.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiChecagemPendencia && !configuracao.ChecagemPendenciaObrigatorio)));
		$('.containerChecagemRoteiro', Documento.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiChecagemRoteiro && !configuracao.ChecagemRoteiroObrigatorio)));
		$('.containerRequerimento', Documento.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiRequerimento && !configuracao.RequerimentoObrigatorio)));
		$('.containerInteressado', Documento.settings.container).toggleClass('hide', (configuracao == null || configuracao.RequerimentoObrigatorio));

		$('.rdbProtocoloAssociadoTipo[value=1]', Documento.settings.container).get(0).checked = true;

		$('.hide', Documento.settings.container).find('input[type=text]').val('');
		$('.hide', Documento.settings.container).find('input[type=hidden]').val(0);
		$('.hide', Documento.settings.container).find('.icone').closest('span').addClass('hide');
		$('.hide', Documento.settings.container).find('.divRequerimento').empty();
	},

	onAssociarInteressado: function () {
		Documento.pessoaModalInte = new PessoaAssociar();

		Modal.abrir(Documento.settings.urls.associarInteressado, null, function (container) {
			Documento.pessoaModalInte.load(container, {
				tituloCriar: 'Cadastrar Interessado',
				tituloEditar: 'Editar Interessado',
				tituloVisualizar: 'Visualizar Interessado',
				onAssociarCallback: Documento.callBackEditarInteressado
			});
		});
	},

	onEditarInteressado: function () {
		var id = $('.hdnInteressadoId', Documento.settings.container).val();
		Documento.pessoaModalInte = new PessoaAssociar();

		Modal.abrir(Documento.settings.urls.editarInteressado + "/" + id, null, function (container) {
			Documento.pessoaModalInte.load(container, {
				onAssociarCallback: Documento.callBackEditarInteressado,
				visualizando: true,
				editarVisualizar: (Documento.settings.modo !== 3), //visualizar
				tituloCriar: 'Cadastrar Interessado',
				tituloEditar: 'Editar Interessado',
				tituloVisualizar: 'Visualizar Interessado'
			});
		});
	},

	callBackEditarInteressado: function (Pessoa) {
		$('.hdnInteressadoId', Documento.settings.container).val(Pessoa.Id);
		$('.txtIntNome', Documento.settings.container).val(Pessoa.NomeRazaoSocial);
		$('.txtIntCnpj', Documento.settings.container).val(Pessoa.CPFCNPJ);
		$('.containerBtnEditarInteressado', Documento.settings.container).removeClass('hide');
		return true;
	},

	onAssociarEmp: function () {
		Modal.abrir(Documento.settings.urls.associarEmpreendimento, null, function (container) {
			EmpreendimentoListar.load(container, { associarFuncao: Documento.callBackEditarEmp });
			Modal.defaultButtons(container);
		});
	},

	onEditarEmp: function () {
		var id = $('.hdnEmpreendimentoId', Documento.settings.container).val();

		Modal.abrir(Documento.settings.urls.editarEmpreendimento + "/" + id, null, function (container) {
			EmpreendimentoAssociar.load(container, {
				onAssociarCallback: Documento.callBackEditarEmp,
				editarVisualizar: (Documento.settings.modo !== 3)//visualizar
			});
			Modal.defaultButtons(container);
		});
	},

	callBackEditarEmp: function (Empreendimento) {
		$('.hdnEmpreendimentoId', Documento.settings.container).val(Empreendimento.Id);
		$('.txtEmpDenominador', Documento.settings.container).val(Empreendimento.Denominador);
		$('.tctEmpCnpj', Documento.settings.container).val(Empreendimento.CNPJ);
		$('.spanBtnEditarEmp', Documento.settings.container).removeClass('hide');
		$('.spanBtnAssociarEmp', Documento.settings.container).addClass('hide');
		return true;
	},

	onLimparEmp: function () {
		$('.hdnEmpreendimentoId', Documento.settings.container).val('');
		$('.txtEmpDenominador', Documento.settings.container).val('');
		$('.tctEmpCnpj', Documento.settings.container).val('');
		$('.spanBtnEditarEmp', Documento.settings.container).addClass('hide');
		$('.spanBtnAssociarEmp', Documento.settings.container).removeClass('hide');
	},

	onAssociarProtocolo: function () {
		var isProcesso = parseInt($('.rdbProtocoloAssociadoTipo:checked', Documento.settings.container).val()) == 1;

		if (isProcesso) {
			Modal.abrir(Documento.settings.urls.associarProcesso, null, function (container) {
				ProcessoListar.load(container, { associarFuncao: Documento.callBackAssociarProtocolo });
				Modal.defaultButtons(container);
			});
		} else {
			Modal.abrir(Documento.settings.urls.associarDocumento, null, function (container) {
				DocumentoListar.load(container, { associarFuncao: Documento.callBackAssociarProtocolo });
				Modal.defaultButtons(container);
			});
		}
	},

	callBackAssociarProtocolo: function (protocolo) {

		var isProtProcesso = parseInt($('.rdbProtocoloAssociadoTipo:checked', Documento.settings.container).val()) == 1;
		if (protocolo.SituacaoId == '2') {
			return [Documento.settings.Mensagens.ProcessoSituacaoInvalida];
		}

		var retorno = MasterPage.validarAjax(Documento.settings.urls.obterProtocolo, { id: protocolo.Id, isProcesso: isProtProcesso }, null, false);
		if (retorno.EhValido) {
			var prot = retorno.ObjResponse.Objeto;
			$('.hdnInteressadoId', Documento.settings.container).val(prot.Interessado.Id);
			$('.txtIntNome', Documento.settings.container).val(prot.Interessado.NomeRazaoSocial);
			$('.txtIntCnpj', Documento.settings.container).val(prot.Interessado.CPFCNPJ);
			$('.containerBtnEditarInteressado', Documento.settings.container).removeClass('hide');

			$('.hdnProtocoloAssociadoId', Documento.settings.container).val(prot.Id);
			$('.txtProtocoloAssociadoNumero', Documento.settings.container).val(prot.Numero);
			$('.spnVisualizarProcesso', Documento.settings.container).removeClass('hide');
			return true;
		} else {
			return retorno.Msg;
		}
	},

	onAssociarCheckList: function () {
		if ($('.hdnDocumentoId', Documento.settings.container).val() != '0' &&
			 !(MasterPage.validarAjax(Documento.settings.urls.validarChecagemTemTituloPendencia, { documentoId: $('.hdnDocumentoId', Documento.settings.container).val() }, Documento.settings.container, false).EhValido))
			return;

		Modal.abrir(Documento.settings.urls.associarCheckList, null, function (container) {
			Modal.defaultButtons(container);
			ChecagemRoteirotListar.load(container, { associarFuncao: Documento.callBackAssociarCheckList });
		});
	},

	callBackAssociarCheckList: function (CheckList) {
		var retorno = MasterPage.validarAjax(Documento.settings.urls.validarChecagem,
			{ id: CheckList.Id, documentoId: $('.hdnDocumentoId', Documento.settings.container).val() }, null, false);

		if (!retorno.EhValido && retorno.Msg) {
			return retorno.Msg;
		}

		$('.txtCheckListId', Documento.settings.container).val(CheckList.Id);
		$('.spnVisualizarChecagem', Documento.settings.container).removeClass('hide');

		return true;
	},

	onAssociarChecagemPendencia: function () {
		Modal.abrir(Documento.settings.urls.associarChecagemPendencia, null, function (container) {
			ChecagemPendenciaListar.load(container, { associarFuncao: Documento.callBackAssociarChecagemPendencia });
			Modal.defaultButtons(container);
		});
	},

	callBackAssociarChecagemPendencia: function (ChecagemPendencia) {
		var retorno = MasterPage.validarAjax(Documento.settings.urls.validarChecagemPendencia, { id: ChecagemPendencia.Id }, null, false);

		if (!retorno.EhValido && retorno.Msg) {
			return retorno.Msg;
		}

		$('.txtChecagemPendenciaId', Documento.settings.container).val(ChecagemPendencia.Id);
		$('.spnVisualizarChecagemPendencia', Documento.settings.container).removeClass('hide');

		return true;
	},

	onAssociarRequerimento: function () {

		var params = { id: $('.txtNumeroReq', Documento.settings.container).val(), documentoId: $('.hdnDocumentoId', Documento.settings.container).val() };
		var abreModal = false;

		if (params.documentoId > 0) {

			$.ajax({ url: Documento.settings.urls.associarRequerimentoValidar, data: JSON.stringify(params), type: 'POST', typeData: 'json',
				contentType: 'application/json; charset=utf-8', cache: false, async: false,
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Documento.settings.container));
				},
				success: function (response, textStatus, XMLHttpRequest) {
					if (response.EhValido) {
						abreModal = response.EhValido;
					} else if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(MasterPage.getContent(Documento.settings.container), response.Msg);
					}
				}
			});
		}

		if (abreModal || params.documentoId == 0) {
			Modal.abrir(Documento.settings.urls.associarRequerimento, null, function (container) {
				RequerimentoListar.load(container, { associarFuncao: Documento.callBackAssociarRequerimento });
				Modal.defaultButtons(container);
			});
		}
	},

	callBackAssociarRequerimento: function (Requerimento) {
		if ($('.txtNumeroReq', Documento.settings.container).val() == Requerimento.Id) {
			return true;
		}

		var params = { id: Requerimento.Id, excetoId: $('.hdnDocumentoId', Documento.settings.container).val() };
		var retorno = Documento.obterAjax(Documento.settings.urls.obterRequerimento, params, $('.divRequerimento', Documento.settings.containerModal));

		if (!retorno.EhValido) {
			return retorno.Msg;
		}

		$('.txtNumeroReq', Documento.settings.container).val(Requerimento.Id);
		$('.hdnRequerimentoSituacao', Documento.settings.container).val(Requerimento.SituacaoId);
		$('.spnPdfRequerimento', Documento.settings.container).removeClass('hide');

		Documento.configurarAssociarMultiplo();
		return true;
	},

	onAbrirVisualizarProtocolo: function () {
		var id = parseInt($('.hdnProtocoloAssociadoId', Documento.settings.container).val());
		var params = { id: id };

		Modal.abrir(Documento.settings.urls.visualizarProcesso, params, function (container) {
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);
	},

	onAbrirVisualizarChecagemPendencia: function () {
		var id = parseInt($('.txtChecagemPendenciaId', Documento.settings.container).val());
		var params = { id: id };

		Modal.abrir(Documento.settings.urls.visualizarChecagemPendencia, params, function (container) {
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);
	},

	onAbrirVisualizarCheckList: function () {
		var id = parseInt($('.txtCheckListId', Documento.settings.container).val());
		var params = { id: id };

		Modal.abrir(Documento.settings.urls.visualizarCheckList, params, function (container) {
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);
	},

	onAbrirPdfRequerimento: function () {
		var id = parseInt($('.txtNumeroReq', Documento.settings.container).val());
		MasterPage.redireciona(Documento.settings.urls.pdfRequerimento + "?id=" + id);
		//MasterPage.carregando(false);
	},

	obterAjax: function (url, params, container) {
		//MasterPage.carregando(true);
		var retorno = null;

		$.ajax({ url: url, data: params, cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Documento.settings.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				Mensagem.limpar(MasterPage.getContent(Documento.settings.container));

				if (response.EhValido || response.SetarHtml) {
					$(container).empty();
					$(container).append(response.Html);
					Mascara.load(container);
					MasterPage.load(container);
					MasterPage.redimensionar();
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
			return new Array(Documento.settings.Mensagens.ResponsaveljaAdicionado);
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
		return Mensagem.replace(Documento.settings.Mensagens.ResponsavelTecnicoRemover, '#texto', $(item).find('.asmItemTexto').val()).Texto;
	},

	//--------------- ENVIAR ARQUIVO ---------------\\
	onEnviarArquivoClick: function () {
		var nomeArquivo = $('.inputFile', Documento.settings.container).val();

		if (nomeArquivo === '') {
			Mensagem.gerar(Documento.settings.container, new Array(Documento.settings.Mensagens.ArquivoObrigatorio));
			return;
		}

		$('.btnArqComplementar', Documento.settings.container).button({ disabled: true });
		var inputFile = $('.inputFile', Documento.settings.container);
		FileUpload.upload(Documento.settings.urls.enviarArquivo, inputFile, Documento.callBackArqEnviado);
	},

	callBackArqEnviado: function (controle, retorno, isHtml) {
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoNome', Documento.settings.container).val(ret.Arquivo.Nome);
			$('.hdnArquivoJson', Documento.settings.container).val(JSON.stringify(ret.Arquivo));

			$('.spanInputFile', Documento.settings.container).addClass('hide');
			$('.txtArquivoNome', Documento.settings.container).removeClass('hide');

			$('.btnArqComplementar', Documento.settings.container).addClass('hide');
			$('.btnArqComplementarLimpar', Documento.settings.container).removeClass('hide');
		} else {
			Documento.onLimparArquivoClick();
		}

		Mensagem.gerar(Documento.settings.container, ret.Msg);
		$('.btnArqComplementar', Documento.settings.container).button({ disabled: false });
	},

	onLimparArquivoClick: function () {
		$('.hdnArquivoJson', Documento.settings.container).val('');
		$('.inputFile', Documento.settings.container).val('');

		$('.spanInputFile', Documento.settings.container).removeClass('hide');
		$('.txtArquivoNome', Documento.settings.container).addClass('hide');

		$('.btnArqComplementar', Documento.settings.container).removeClass('hide');
		$('.btnArqComplementarLimpar', Documento.settings.container).addClass('hide');
		$('.lnkArquivo', Documento.settings.container).remove();
	},

	//--------------- SALVAR DOCUMENTO ---------------\\
	montarObjetoDocumento: function () {

		var objetoDocumento = {
			Id: 0,
			Numero: '',
			Tipo: { Id: 0 },
			DataCadastroTexto: '',
			Nome: '',
			Processo: { Id: 0 },
			ChecagemPendencia: { Id: 0 },
			ChecagemRoteiro: { Id: 0 },
			Requerimento: { Id: 0, SituacaoId: 0 },
			Interessado: { Id: 0 },
			Empreendimento: { Id: 0 },
			Atividades: [],
			Responsaveis: []
		};

		objetoDocumento.Id = $('.hdnDocumentoId', Documento.settings.container).val();
		objetoDocumento.Numero = $('.txtNumero', Documento.settings.container).val();
		objetoDocumento.Tipo.Id = $('.ddlDocumentoTipos', Documento.settings.container).val();
		objetoDocumento.DataCadastro = { DataTexto: $('.txtDataCriacao', Documento.settings.container).val() };
		objetoDocumento.Volume = $('.txtQuantidadeDocumento', Documento.settings.container).val();
		objetoDocumento.Arquivo = $.parseJSON($('.hdnArquivoJson', Documento.settings.container).val());
		objetoDocumento.Nome = $('.txtNomeDocumento', Documento.settings.container).val();
		objetoDocumento.ChecagemRoteiro.Id = $('.txtCheckListId', Documento.settings.container).val();
		objetoDocumento.SetorId = $('.ddlSetor', Documento.settings.container).val();
		objetoDocumento.Interessado.Id = $('.hdnInteressadoId', $('.hdnInteressadoId', Documento.settings.container).closest('fieldset:visible')).val();

		objetoDocumento.ProtocoloAssociado = { Id: $('.hdnProtocoloAssociadoId', Documento.settings.container).val() };
		objetoDocumento.ChecagemPendencia.Id = $('.txtChecagemPendenciaId', Documento.settings.container).val();
		objetoDocumento.Requerimento.Id = $('.txtNumeroReq', Documento.settings.container).val();
		objetoDocumento.Requerimento.SituacaoId = $('.hdnRequerimentoSituacao', Documento.settings.container).val();

		if (objetoDocumento.Requerimento.Id != '') {
			objetoDocumento.Empreendimento.Id = $('.hdnEmpreendimentoId', Documento.settings.container).val();

			// Início Atividades
			objetoDocumento.Atividades = AtividadeSolicitadaAssociar.gerarObjeto(Documento.settings.container);
			// Fim Atividades

			// Início Responsáveis
			var responsaveis = $('.divConteudoResponsavelTec .asmItens .asmItemContainer', Documento.settings.container);

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

					objetoDocumento.Responsaveis.push(responsavel);
				}
			});
		}

		return objetoDocumento;
	},

	salvar: function (isRedirecionar) {
		var documento = Documento.montarObjetoDocumento();
		//MasterPage.carregando(true);
		var params = { documento: documento };
		var acao = documento.Id > 0 ? Documento.settings.urls.editar : Documento.settings.urls.criar;

		$.ajax({ url: acao, data: JSON.stringify(params), type: 'POST', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Documento.settings.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.IsDocumentoSalvo) {
					if (typeof response.UrlRedireciona != "undefined" && response.UrlRedireciona !== null && isRedirecionar) {
						MasterPage.redireciona(response.UrlRedireciona);
						return;
					}
				} else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Documento.settings.container), response.Msg);
				}
			}
		});
		//MasterPage.carregando(false);
	}
}
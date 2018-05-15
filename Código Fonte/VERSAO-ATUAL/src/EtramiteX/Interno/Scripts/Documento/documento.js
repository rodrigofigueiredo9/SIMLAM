/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

var Documento = function () {
	var primeiraVez = true;
	var _objRef = null;

	return {

		settings: {
			container: null,
			containerModal: null,
			urls: {
				enviarArquivo: '',
				associarProcesso: '',
				associarDocumento: '',
				associarChecagemPendencia: '',
				associarCheckList: '',
				associarRequerimento: '',
				associarRequerimentoValidar: '',
				visualizarProcesso: '',
				visualizarDocumento: '',
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
				funcionariosDestinatario: '',
				obterAssinanteCargos: '',
				obterAssinanteFuncionarios: '',
				salvar: ''
			},
			Mensagens: null,
			configuracoesProtocoloTipos: null,
			modo: 1 //Modo Cadastrar
		},

		pessoaModalInte: null,
		pessoaModalResp: null,

		load: function (container, options) {

			_objRef = this;

			if (options) {
				$.extend(_objRef.settings, options);
			}

			_objRef.settings.container = container;
			_objRef.settings.containerModal = $('.documentoPartial', container);

			_objRef.setarBotoes(container);
			_objRef.configurarAssociarMultiplo();
		},

		setarBotoes: function (container) {
			container.delegate('.ddlDocumentoTipos', 'change', _objRef.changeDocumentoTipo);
			container.delegate('.rdbProtocoloAssociadoTipo', 'change', _objRef.onRdbProtocoloAssociadoTipo);			
			//container.delegate('.rdbProtocoloAssociadoTipo', 'click', _objRef.onRdbProtocoloAssociadoTipo);
			container.delegate('.btnArqComplementar', 'click', _objRef.onEnviarArquivoClick);
			container.delegate('.btnArqComplementarLimpar', 'click', _objRef.onLimparArquivoClick);
			container.delegate('.btnBuscarProtocolo', 'click', _objRef.onAssociarProtocolo);
			container.delegate('.btnBuscarChecagemPendencia', 'click', _objRef.onAssociarChecagemPendencia);
			container.delegate('.btnLimparProtocolo', 'click', _objRef.onLimparProtocolo);
			container.delegate('.btnBuscarCheckList', 'click', _objRef.onAssociarCheckList);
			container.delegate('.btnBuscarRequerimento', 'click', _objRef.onAssociarRequerimento);
			container.delegate('.btnVisualizarProtocolo', 'click', _objRef.onAbrirVisualizarProtocolo);
			container.delegate('.btnVisualizarChecagemPendencia', 'click', _objRef.onAbrirVisualizarChecagemPendencia);
			container.delegate('.btnVisualizarChecagem', 'click', _objRef.onAbrirVisualizarCheckList);
			container.delegate('.btnPdfRequerimento', 'click', _objRef.onAbrirPdfRequerimento);
			container.delegate('.btnEditarInteressado', 'click', _objRef.onEditarInteressado);
			container.delegate('.btnAssociarInteressado', 'click', _objRef.onAssociarInteressado);
			container.delegate('.btnLimparInteressado', 'click', _objRef.limparInteressado);
			container.delegate('.btnEditarEmp', 'click', _objRef.onEditarEmp);
			container.delegate('.btnAssociarEmp', 'click', _objRef.onAssociarEmp);
			container.delegate('.btnLimparEmp', 'click', _objRef.onLimparEmp);			
			container.delegate('.btnDocumentoSalvar', 'click', _objRef.salvar);
			container.delegate('.btnBuscarFiscalizacao', 'click', _objRef.onAssociarFiscalizacao);
			container.delegate('.btnLimparFiscalizacao', 'click', _objRef.onLimparFiscalizacao);
			container.delegate('.btnVisualizarFiscalizacao', 'click', _objRef.onAbrirVisualizarFiscalizacao);
			container.delegate('.ddlSetoresDestinatario', 'click', _objRef.destinatarioSetorChange);


			container.delegate('.ddlAssinanteSetores', 'change', _objRef.onSelecionarSetor);
			container.delegate('.ddlAssinanteCargos', 'change', _objRef.onSelecionarCargo);

			container.delegate('.btnAdicionarAssinante', 'click', _objRef.onAdicionarAssinante);
			container.delegate('.btnExcluirAssinante', 'click', _objRef.onExcluirAssinante);
		},

		//Selecionar Assinante 
		onSelecionarSetor: function () {

			var ddlA = $(".ddlAssinanteSetores", _objRef.settings.container);
			var ddlB = $('.ddlAssinanteCargos', _objRef.settings.container);
			var ddlC = $('.ddlAssinanteFuncionarios', _objRef.settings.container);

			var setorId = $('.ddlAssinanteSetores', _objRef.settings.container).val();

			ddlA.ddlCascate(ddlB, { url: _objRef.settings.urls.obterAssinanteCargos });
			ddlB.ddlCascate(ddlC, { url: _objRef.settings.urls.obterAssinanteFuncionarios, data: { setorId: setorId } });

		},

		onSelecionarCargo: function () {
			var ddlA = $(".ddlAssinanteCargos", _objRef.settings.container);
			var ddlB = $('.ddlAssinanteFuncionarios', _objRef.settings.container);

			var setorId = $('.ddlAssinanteSetores', _objRef.settings.container).val();

			ddlA.ddlCascate(ddlB, { url: _objRef.settings.urls.obterAssinanteFuncionarios, data: { setorId: setorId } });
		},

		onAdicionarAssinante: function () {

			var mensagens = new Array();
			Mensagem.limpar(_objRef.settings.container);
			var container = $('.fdsAssinante', _objRef.settings.container);

			var item = {
				SetorId: $('.ddlAssinanteSetores :selected', _objRef.settings.container).val(),
				FuncionarioNome: $('.ddlAssinanteFuncionarios :selected', _objRef.settings.container).text(),
				FuncionarioId: $('.ddlAssinanteFuncionarios :selected', _objRef.settings.container).val(),
				FuncionarioCargoNome: $('.ddlAssinanteCargos :selected', _objRef.settings.container).text(),
				FuncionarioCargoId: $('.ddlAssinanteCargos :selected', _objRef.settings.container).val()
			};

			if (jQuery.trim(item.SetorId) == '0') {
				mensagens.push(jQuery.extend(true, {}, _objRef.settings.Mensagens.AssinanteSetorObrigatorio));
			}

			if (jQuery.trim(item.FuncionarioCargoId) == '0') {
				mensagens.push(jQuery.extend(true, {}, _objRef.settings.Mensagens.AssinanteCargoObrigatorio));
			}

			if (jQuery.trim(item.FuncionarioId) == '0') {
				mensagens.push(jQuery.extend(true, {}, _objRef.settings.Mensagens.AssinanteFuncionarioObrigatorio));
			}

			$('.hdnItemJSon', container).each(function () {
				var obj = String($(this).val());
				if (obj != '') {
					var itemAdd = (JSON.parse(obj));
					if (item.FuncionarioId == itemAdd.FuncionarioId && item.FuncionarioCargoId == itemAdd.FuncionarioCargoId) {
						mensagens.push(jQuery.extend(true, {}, _objRef.settings.Mensagens.AssinanteJaAdicionado));
					}
				}
			});

			if (mensagens.length > 0) {
				Mensagem.gerar(_objRef.settings.container, mensagens);
				return;
			}

			var linha = $('.trTemplateRow', _objRef.settings.container).clone().removeClass('trTemplateRow hide');
			linha.find('.hdnItemJSon').val(JSON.stringify(item));
			linha.find('.Funcionario').html(item.FuncionarioNome).attr('title', item.FuncionarioNome);
			linha.find('.Cargo').html(item.FuncionarioCargoNome).attr('title', item.FuncionarioCargoNome);

			$('.dataGridTable tbody:last', _objRef.settings.container).append(linha);
			Listar.atualizarEstiloTable(container.find('.dataGridTable'));

			$('.ddlAssinanteSetores', _objRef.settings.container).ddlFirst();
			_objRef.onSelecionarSetor();

		},

		onExcluirAssinante: function () {
			var container = $('.fdsAssinante');
			var linha = $(this).closest('tr');
			linha.remove();
			Listar.atualizarEstiloTable(container.find('.dataGridTable'));
		},

		destinatarioSetorChange: function () {
            if (primeiraVez) {
                primeiraVez = false;
                return;
            }		
			var ddlB = $('.ddlDestinatarios', _objRef.settings.container);
			var ddlA = $(this, _objRef.settings.container);
            var url = _objRef.settings.urls.funcionariosDestinatario;

			ddlA.ddlCascate(ddlB, { url: url, disabled: false });
		},

		configurarAssociarMultiplo: function () {
			$('.divConteudoAtividadeSolicitada', _objRef.settings.container).associarMultiplo({
				'associarModalLoadFunction': 'AtividadeSolicitadaListar.load',
				'expandirAutomatico': false,
				'minItens': 1,
				'tamanhoModal': Modal.tamanhoModalGrande
			});

			_objRef.pessoaModalResp = new PessoaAssociar();

			$('.divConteudoResponsavelTec', _objRef.settings.container).associarMultiplo({
				'onAssociar': _objRef.associarResponsavelTecnico,
				'associarUrl': _objRef.settings.urls.associarResponsavelModal,
				'associarModalObject': _objRef.pessoaModalResp,
				'associarModalLoadFunction': _objRef.pessoaModalResp.load,
				'associarModalLoadParams': {
					tituloCriar: 'Cadastrar Responsável',
					tituloEditar: 'Editar Responsável',
					tituloVisualizar: 'Visualizar Responsável',
					editarVisualizar: (_objRef.settings.modo !== 3)//visualizar
				},

				'onEditar': _objRef.onResponsavelEditar,
				'editarUrl': _objRef.settings.urls.editarResponsavelModal,
				'editarModalObject': _objRef.pessoaModalResp,
				'editarModalLoadFunction': _objRef.pessoaModalResp.load,
				'editarModalLoadParams': {
					tituloCriar: 'Cadastrar Responsável',
					tituloEditar: 'Editar Responsável',
					tituloVisualizar: 'Visualizar Responsável',
					editarVisualizar: (_objRef.settings.modo !== 3)//visualizar
				},

				'mostrarBtnLimpar': true,
				'expandirAutomatico': true,
				'minItens': 1,
				'tamanhoModal': Modal.tamanhoModalGrande,
				'onExpandirEsconder': function () { MasterPage.redimensionar(); },
				'msgObrigatoriedade': _objRef.settings.Mensagens.ResponsavelTecnicoSemPreencher,

				'btnOkLabelExcluir': 'Remover',
				'tituloExcluir': 'Remover Responsável Técnico',
				'msgExcluir': _objRef.onMensagemExcluirResponsavel
			});

			var containerLink = $('.conteudoResponsaveis .asmConteudoInterno', _objRef.settings.container);
			if (!containerLink.hasClass('hide')) {
				$('.conteudoResponsaveis', _objRef.settings.container).find('.btnAsmEditar').hide();
			}
		},		

		changeDocumentoTipo: function () {
			$('.hdnProtocoloAssociadoId', _objRef.settings.container).val('0');
			$('.txtProtocoloAssociadoNumero', _objRef.settings.container).val('');
			$('.spnVisualizarProtocolo', _objRef.settings.container).addClass('hide');

			var checagemId = parseInt($('.txtCheckListId', _objRef.settings.container).val());
			var requerimentoId = parseInt($('.txtNumeroReq', _objRef.settings.container).val());

			var configuracao = null;
			var ddlDocTipoVal = $('.ddlDocumentoTipos', _objRef.settings.container).val();
			$.each(_objRef.settings.configuracoesProtocoloTipos, function (i, item) {
				if (item.Id == ddlDocTipoVal) {
					configuracao = item;
				}
			});

			var containerProcessoDocumentoVisivel = configuracao != null && (configuracao.PossuiProcesso || configuracao.ProcessoObrigatorio);
			var esconderAssociarDocumento = configuracao != null && !(configuracao.PossuiDocumento || configuracao.DocumentoObrigatorio);
			var tituloGrupo = esconderAssociarDocumento ? 'Processo' : 'Processo/Documento';

			$('.grupoProtocoloAssociadoNome', _objRef.settings.container).text(tituloGrupo);
			$('.visivelSeAssociaDocumento', _objRef.settings.container).toggleClass('hide', esconderAssociarDocumento);

			$('.containerProcessoDocumento', _objRef.settings.container).toggleClass('hide', !containerProcessoDocumentoVisivel);
			$('.containerChecagemPendencia', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiChecagemPendencia && !configuracao.ChecagemPendenciaObrigatorio)));
			$('.containerChecagemRoteiro', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiChecagemRoteiro && !configuracao.ChecagemRoteiroObrigatorio)));
			$('.containerRequerimento', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiRequerimento && !configuracao.RequerimentoObrigatorio)));
			$('.containerInteressado', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiInteressado && !configuracao.InteressadoObrigatorio)));
			$('.containerInteressadoLivre', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiInteressadoLivre)));
		    //$('.containerInteressado', _objRef.settings.container).toggleClass('hide', (configuracao == null || configuracao.RequerimentoObrigatorio || (configuracao.PossuiFiscalizacao || configuracao.FiscalizacaoObrigatorio) ));
			$('.containerFiscalizacao', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiFiscalizacao && !configuracao.FiscalizacaoObrigatorio)));

			$('.labelInteressado', _objRef.settings.container).text(configuracao.LabelInteressado);
			$('.btnAssociarInteressado', _objRef.settings.container).toggleClass('hide', (configuracao.PossuiFiscalizacao || configuracao.FiscalizacaoObrigatorio));

			$('.qtdFolhas', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiInteressadoLivre)));
			$('.qtdDocumento', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiQuantidadeDocumento && !configuracao.QuantidadeDocumentoObrigatorio)));
			$('.nomeDocumento', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiNome && !configuracao.NomeObrigatorio)));
			$('.assunto', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiAssunto && !configuracao.AssuntoObrigatorio)));
			$('.descricao', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiDescricao && !configuracao.DescricaoObrigatoria)));
			$('.inputFileDiv', _objRef.settings.container).toggleClass('hide', (configuracao == null || (configuracao.PossuiAssunto || configuracao.AssuntoObrigatorio)));
			$('.spanBotoes', _objRef.settings.container).toggleClass('hide', (configuracao == null || (configuracao.PossuiAssunto || configuracao.AssuntoObrigatorio)));
			$('.destinatario', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiAssunto && !configuracao.AssuntoObrigatorio)));
			$('.assinantes', _objRef.settings.container).toggleClass('hide', (configuracao == null || (!configuracao.PossuiAssunto && !configuracao.AssuntoObrigatorio)));

			var isCnfProcesso = (configuracao != null && (configuracao.PossuiProcesso || configuracao.ProcessoObrigatorio));
			var isCnfDocumento = (configuracao != null && (configuracao.PossuiDocumento || configuracao.DocumentoObrigatorio));

			$('.rdbProtocoloAssociadoTipo[value=1]', _objRef.settings.container).get(0).checked = true;

			if (isCnfProcesso && !isCnfDocumento) {
			    $('.rdbProtocoloAssociadoTipo[value=1]', _objRef.settings.container).get(0).checked = true;
			} else if (!isCnfProcesso && isCnfDocumento) {
			    $('.rdbProtocoloAssociadoTipo[value=2]', _objRef.settings.container).get(0).checked = true;
			}

			$('.hide', _objRef.settings.container).find('input[type=text]').val('');
			$('.hide', _objRef.settings.container).find('input[type=hidden]').val(0);
			$('.hide', _objRef.settings.container).find('.icone').closest('span').addClass('hide');
			$('.hide', _objRef.settings.container).find('.divRequerimento').empty();

			_objRef.asterisco($('.lblProcesso', _objRef.settings.container),  configuracao.ProcessoObrigatorio);
			_objRef.asterisco($('.lblDocumento', _objRef.settings.container), configuracao.DocumentoObrigatorio);
			_objRef.asterisco($('.lblChecagemPendenciaNum', _objRef.settings.container), configuracao.ChecagemPendenciaObrigatorio);
			_objRef.asterisco($('.lblChecagemRoteiroNum', _objRef.settings.container), configuracao.ChecagemRoteiroObrigatorio);
			_objRef.asterisco($('.lblRequerimentoNum', _objRef.settings.container), configuracao.RequerimentoObrigatorio);
			_objRef.asterisco($('.lblFiscalizacaoNum', _objRef.settings.container), configuracao.FiscalizacaoObrigatorio);
			_objRef.asterisco($('.lblInteressadoNomeRazao', _objRef.settings.container), configuracao.InteressadoObrigatorio);
			_objRef.asterisco($('.lblInteressadoCpfCnpj', _objRef.settings.container), configuracao.InteressadoObrigatorio);
			_objRef.asterisco($('.lblQtdDocumento', _objRef.settings.container),  configuracao.QuantidadeDocumentoObrigatorio);
			_objRef.asterisco($('.lblNome', _objRef.settings.container), configuracao.NomeObrigatorio);
			_objRef.asterisco($('.lblAssunto', _objRef.settings.container), configuracao.AssuntoObrigatorio);
			_objRef.asterisco($('.lblDescricao', _objRef.settings.container), configuracao.DescricaoObrigatoria);
		},

		asterisco: function (control, exibir) {
		    
		    control.text(control.text().replace(' *', ''));

		    if (exibir){
		        control.text(control.text() + ' *');
		    }
		},

	    //--------------- INTERESSADO ---------------\\
		onAssociarInteressado: function () {
			_objRef.pessoaModalInte = new PessoaAssociar();

			Modal.abrir(_objRef.settings.urls.associarInteressado, null, function (container) {
				_objRef.pessoaModalInte.load(container, {
					tituloCriar: 'Cadastrar Interessado',
					tituloEditar: 'Editar Interessado',
					tituloVisualizar: 'Visualizar Interessado',
					onAssociarCallback: _objRef.callBackEditarInteressado
				});
			});
		},

		onEditarInteressado: function () {
			var id = $('.hdnInteressadoId', _objRef.settings.container).val();
			_objRef.pessoaModalInte = new PessoaAssociar();

			Modal.abrir(_objRef.settings.urls.editarInteressado + "/" + id, null, function (container) {
				_objRef.pessoaModalInte.load(container, {
					onAssociarCallback: _objRef.callBackEditarInteressado,
					editarVisualizar: (_objRef.settings.modo !== 3), //visualizar
					tituloCriar: 'Cadastrar Interessado',
					tituloEditar: 'Editar Interessado',
					tituloVisualizar: 'Visualizar Interessado'
				});
			});
		},

		callBackEditarInteressado: function (Pessoa) {
			$('.hdnInteressadoId', _objRef.settings.container).val(Pessoa.Id);
			$('.txtIntNome', _objRef.settings.container).val(Pessoa.NomeRazaoSocial);
			$('.txtIntCnpj', _objRef.settings.container).val(Pessoa.CPFCNPJ);
			$('.containerBtnEditarInteressado', _objRef.settings.container).removeClass('hide');

			$('.divLimparInteressado', _objRef.settings.container).removeClass('hide');
			$('.divBuscarInteressado', _objRef.settings.container).addClass('hide');
			return true;
		},

		callBackEditarInteressado: function (pessoa) {
		    $('.spanVisualizarInteressado', _objRef.settings.container).removeClass('hide');
		    $('.hdnInteressadoId', _objRef.settings.container).val(pessoa.Id);
		    $('.txtIntNome', _objRef.settings.container).val(pessoa.NomeRazaoSocial);
		    $('.txtIntCnpj', _objRef.settings.container).val(pessoa.CPFCNPJ);
		    
		    $('.divLimparInteressado', _objRef.settings.container).removeClass('hide');
		    $('.divBuscarInteressado', _objRef.settings.container).addClass('hide');
		    return true;
		},

		limparInteressado: function () {
		    $('.hdnInteressadoId', _objRef.settings.container.find('.containerInteressado')).val('0');
		    $('.txtIntNome', _objRef.settings.container.find('.containerInteressado')).val('');
		    $('.txtIntCnpj', _objRef.settings.container.find('.containerInteressado')).val('');

		    $('.spanVisualizarInteressado', _objRef.settings.container).addClass('hide');
		    $('.divLimparInteressado', _objRef.settings.container).addClass('hide');
		    $('.divBuscarInteressado', _objRef.settings.container).removeClass('hide');
		},
	    //--------------- Fim INTERESSADO ---------------\\

		obterConfiguracaoTipo: function () {
		    var configuracao = null;
		    $.each(_objRef.settings.configuracoesProtocoloTipos, function (i, item) {
		        if (item.Id == $('.ddlDocumentoTipos', _objRef.settings.container).val()) {
		            configuracao = item;
		        }
		    });

		    return configuracao;
		},

	    //--------------- FISCALIZACAO ---------------\\
		onAssociarFiscalizacao: function () {

		    Modal.abrir(_objRef.settings.urls.associarFiscalizacao, null, function (container) {
		        FiscalizacaoListar.load(container, { associarFuncao: _objRef.callBackAssociarFiscalizacao });
		        Modal.defaultButtons(container);
		    });
		},

		callBackAssociarFiscalizacao: function (Fiscalizacao) {
		    if ($('.txtNumeroFiscalizacao', _objRef.settings.container).val() == Fiscalizacao.Id) {
		        return true;
		    }

		    var params = { fiscalizacaoId: Fiscalizacao.Id };
		    var retorno = _objRef.obterAjax(_objRef.settings.urls.obterFiscalizacao, params, $('.divFiscalizacao', _objRef.settings.containerModal));

		    if (!retorno.EhValido) {
		        return retorno.Msg;
		    }

		    $('.txtNumeroFiscalizacao', _objRef.settings.container).val(Fiscalizacao.Id);
		    $('.hdnFiscalizacaoSituacao', _objRef.settings.container).val(Fiscalizacao.SituacaoId);

		    _objRef.configurarAssociarMultiplo();

		    $('.btnLimparFiscalizacao', _objRef.settings.container).removeClass('hide');
		    $('.btnBuscarFiscalizacao', _objRef.settings.container).addClass('hide');
		    $('.spnVisualizarFiscalizacao', _objRef.settings.container).removeClass('hide');

		    _objRef.callBackEditarInteressado(retorno.Autuado);

		    return true;
		},

		onLimparFiscalizacao: function () {

		    var fiscalizacao = $('.txtNumeroFiscalizacao', _objRef.settings.container).val() || 0;

		    if (!_objRef.validarDesassociarFicalizacao(fiscalizacao)) {
		        return;
		    }

		    $('.txtNumeroFiscalizacao', _objRef.settings.container).val('');
		    $('.btnLimparFiscalizacao', _objRef.settings.container).addClass('hide');
		    $('.btnLimparFiscalizacao', _objRef.settings.container).addClass('hide');
		    $('.btnBuscarFiscalizacao', _objRef.settings.container).removeClass('hide');
		    $('.spnVisualizarFiscalizacao', _objRef.settings.container).addClass('hide');

		    $('.divFiscalizacao', _objRef.settings.container).empty();

		    var configuracao = _objRef.obterConfiguracaoTipo();

		    _objRef.limparInteressado();
		},

		onLimparFiscalizacaoEmCadastro: function () {
		    $('.txtNumeroFiscalizacao', _objRef.settings.container).val('');
		    $('.btnLimparFiscalizacao', _objRef.settings.container).addClass('hide');
		    $('.btnLimparFiscalizacao', _objRef.settings.container).addClass('hide');
		    $('.btnBuscarFiscalizacao', _objRef.settings.container).removeClass('hide');
		    $('.spnVisualizarFiscalizacao', _objRef.settings.container).addClass('hide');
		},

		onAbrirVisualizarFiscalizacao: function () {
		    var id = parseInt($('.txtNumeroFiscalizacao', _objRef.settings.container).val());
		    var params = { id: id };

		    Modal.abrir(_objRef.settings.urls.visualizarFiscalizacao, params, function (container) {
		        Modal.defaultButtons(container);
		    }, Modal.tamanhoModalGrande);
		},

		validarDesassociarFicalizacao: function (fiscalizacaoId) {

		    var retorno = false;

		    if (fiscalizacaoId > 0) {
		        $.ajax({
		            url: _objRef.settings.urls.desassociarFiscalizacaoValidar, data: JSON.stringify({ fiscalizacaoId: fiscalizacaoId }), type: 'POST', typeData: 'json',
		            contentType: 'application/json; charset=utf-8', cache: false, async: false,
		            error: function (XMLHttpRequest, textStatus, erroThrown) {
		                Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(_objRef.settings.container));
		            },
		            success: function (response, textStatus, XMLHttpRequest) {
		                retorno = response.EhValido;
		                if (response.Msg && response.Msg.length > 0) {
		                    Mensagem.gerar(MasterPage.getContent(_objRef.settings.container), response.Msg);
		                }
		            }
		        });
		    }
		    return retorno;
		},
	    //--------------- Fim FISCALIZACAO ---------------\\


	    //--------------- EMPREENDIMENTO ---------------\\
		onAssociarEmp: function () {
			Modal.abrir(_objRef.settings.urls.associarEmpreendimento, null, function (container) {
				EmpreendimentoListar.load(container, { associarFuncao: _objRef.callBackEditarEmp });
				Modal.defaultButtons(container);
			});
		},

		onEditarEmp: function () {
			var id = $('.hdnEmpreendimentoId', _objRef.settings.container).val();

			Modal.abrir(_objRef.settings.urls.editarEmpreendimento + "/" + id, null, function (container) {
				EmpreendimentoAssociar.load(container, {
					onAssociarCallback: _objRef.callBackEditarEmp,
					editarVisualizar: (_objRef.settings.modo !== 3)//visualizar
				});
			});
		},

		callBackEditarEmp: function (Empreendimento) {
			$('.hdnEmpreendimentoId', _objRef.settings.container).val(Empreendimento.Id);
			$('.txtEmpDenominador', _objRef.settings.container).val(Empreendimento.Denominador);
			$('.tctEmpCnpj', _objRef.settings.container).val(Empreendimento.CNPJ);
			$('.spanBtnEditarEmp', _objRef.settings.container).removeClass('hide');
			$('.spanBtnAssociarEmp', _objRef.settings.container).addClass('hide');
			return true;
		},

		onLimparEmp: function () {
			$('.hdnEmpreendimentoId', _objRef.settings.container).val('');
			$('.txtEmpDenominador', _objRef.settings.container).val('');
			$('.tctEmpCnpj', _objRef.settings.container).val('');
			$('.spanBtnEditarEmp', _objRef.settings.container).addClass('hide');
			$('.spanBtnAssociarEmp', _objRef.settings.container).removeClass('hide');
		},
	    //--------------- Fim EMPREENDIMENTO ---------------\\


	    //--------------- PROTOCOLO ---------------\\
		onAssociarProtocolo: function () {
			var isProcesso = parseInt($('.rdbProtocoloAssociadoTipo:checked', _objRef.settings.container).val()) == 1;

			if (isProcesso) {
				Modal.abrir(_objRef.settings.urls.associarProcesso, null, function (container) {
					ProcessoListar.load(container, { associarFuncao: _objRef.callBackAssociarProtocolo });
					Modal.defaultButtons(container);
				});
			} else {
				Modal.abrir(_objRef.settings.urls.associarDocumento, null, function (container) {
					DocumentoListar.load(container, { associarFuncao: _objRef.callBackAssociarProtocolo });
					Modal.defaultButtons(container);
				});
			}
		},

		callBackAssociarProtocolo: function (protocolo) {

			var isProtProcesso = parseInt($('.rdbProtocoloAssociadoTipo:checked', _objRef.settings.container).val()) == 1;
			if (protocolo.SituacaoId == '2') {
				return [_objRef.settings.Mensagens.ProcessoSituacaoInvalida];
			}

			var configuracao = _objRef.obterConfiguracaoTipo();
			$('.btnLimparProtocolo', _objRef.settings.container).toggleClass('hide',
                (!(configuracao.PossuiProcesso && !configuracao.ProcessoObrigatorio) ||
                (configuracao.PossuiDocumento && !configuracao.DocumentoObrigatorio)));
			

			var retorno = MasterPage.validarAjax(_objRef.settings.urls.obterProtocolo, { id: protocolo.Id, isProcesso: isProtProcesso }, _objRef.settings.container, false);
			if (retorno.EhValido === true) {
				var prot = retorno.ObjResponse.Objeto;
				$('.hdnInteressadoId', _objRef.settings.container).val(prot.Interessado.Id);
				$('.txtIntNome', _objRef.settings.container).val(prot.Interessado.NomeRazaoSocial);
				$('.txtIntCnpj', _objRef.settings.container).val(prot.Interessado.CPFCNPJ);
				$('.containerBtnEditarInteressado', _objRef.settings.container).removeClass('hide');

				$('.hdnProtocoloAssociadoId', _objRef.settings.container).val(prot.Id);
				$('.txtProtocoloAssociadoNumero', _objRef.settings.container).val(prot.Numero);
				$('.spnVisualizarProtocolo', _objRef.settings.container).removeClass('hide');
				$('.btnBuscarProtocolo', _objRef.settings.container).addClass('hide');
				

				$('.divLimparInteressado', _objRef.settings.container).removeClass('hide');
				$('.divBuscarInteressado', _objRef.settings.container).addClass('hide');
				return true;
			} else {
				return retorno.Msg;
			}
		},

		onAbrirVisualizarProtocolo: function () {

		    var isProtProcesso = parseInt($('.rdbProtocoloAssociadoTipo:checked', _objRef.settings.container).val()) == 1;
		    var urlVisualizar = (isProtProcesso) ? _objRef.settings.urls.visualizarProcesso : _objRef.settings.urls.visualizarDocumento;

		    var id = parseInt($('.hdnProtocoloAssociadoId', _objRef.settings.container).val());
		    var params = { id: id };

		    Modal.abrir(urlVisualizar, params, function (container) {
		        Modal.defaultButtons(container);
		    }, Modal.tamanhoModalGrande);
		},

		onRdbProtocoloAssociadoTipo: function () {
		    _objRef.onLimparProtocolo();
		},

		onLimparProtocolo: function () {
		    $('.hdnProtocoloAssociadoId', _objRef.settings.container).val('0');
		    $('.txtProtocoloAssociadoNumero', _objRef.settings.container).val('');
		    $('.spnVisualizarProtocolo', _objRef.settings.container).addClass('hide');
		    $('.btnLimparProtocolo', _objRef.settings.container).addClass('hide');
		    $('.btnBuscarProtocolo', _objRef.settings.container).removeClass('hide');
		},
	    //--------------- Fim PROTOCOLO ---------------\\


	    //--------------- CHECKLIST ---------------\\
		onAssociarCheckList: function () {
			if ($('.hdnDocumentoId', _objRef.settings.container).val() != '0' &&
			 !(MasterPage.validarAjax(_objRef.settings.urls.validarChecagemTemTituloPendencia, { documentoId: $('.hdnDocumentoId', _objRef.settings.container).val() }, _objRef.settings.container, false).EhValido))
				return;

			Modal.abrir(_objRef.settings.urls.associarCheckList, null, function (container) {
				Modal.defaultButtons(container);
				ChecagemRoteirotListar.load(container, { associarFuncao: _objRef.callBackAssociarCheckList });
			});
		},

		callBackAssociarCheckList: function (CheckList) {
			var retorno = MasterPage.validarAjax(_objRef.settings.urls.validarChecagem,
			{ id: CheckList.Id, documentoId: $('.hdnDocumentoId', _objRef.settings.container).val() }, null, false);

			if (!retorno.EhValido && retorno.Msg) {
				return retorno.Msg;
			}

			$('.txtCheckListId', _objRef.settings.container).val(CheckList.Id);
			$('.spnVisualizarChecagem', _objRef.settings.container).removeClass('hide');

			return true;
		},

		onAbrirVisualizarCheckList: function () {
		    var id = parseInt($('.txtCheckListId', _objRef.settings.container).val());
		    var params = { id: id };

		    Modal.abrir(_objRef.settings.urls.visualizarCheckList, params, function (container) {
		        Modal.defaultButtons(container);
		    }, Modal.tamanhoModalGrande);
		},
	    //--------------- Fim CHECKLIST ---------------\\


	    //--------------- CHECAGEM PENDENCIA ---------------\\
		onAssociarChecagemPendencia: function () {
			Modal.abrir(_objRef.settings.urls.associarChecagemPendencia, null, function (container) {
				ChecagemPendenciaListar.load(container, { associarFuncao: _objRef.callBackAssociarChecagemPendencia });
				Modal.defaultButtons(container);
			});
		},

		callBackAssociarChecagemPendencia: function (ChecagemPendencia) {
			var retorno = MasterPage.validarAjax(_objRef.settings.urls.validarChecagemPendencia, { id: ChecagemPendencia.Id }, null, false);

			if (!retorno.EhValido && retorno.Msg) {
				return retorno.Msg;
			}

			$('.txtChecagemPendenciaId', _objRef.settings.container).val(ChecagemPendencia.Id);
			$('.spnVisualizarChecagemPendencia', _objRef.settings.container).removeClass('hide');

			return true;
		},

		onAbrirVisualizarChecagemPendencia: function () {
		    var id = parseInt($('.txtChecagemPendenciaId', _objRef.settings.container).val());
		    var params = { id: id };

		    Modal.abrir(_objRef.settings.urls.visualizarChecagemPendencia, params, function (container) {
		        Modal.defaultButtons(container);
		    }, Modal.tamanhoModalGrande);
		},
	    //--------------- Fim CHECAGEM PENDENCIA ---------------\\


	    //--------------- REQUERIMENTO ---------------\\
		onAssociarRequerimento: function () {

			var params = { id: $('.txtNumeroReq', _objRef.settings.container).val(), documentoId: $('.hdnDocumentoId', _objRef.settings.container).val() };
			var abreModal = false;

			if (params.documentoId > 0) {

				$.ajax({ url: _objRef.settings.urls.associarRequerimentoValidar, data: JSON.stringify(params), type: 'POST', typeData: 'json',
					contentType: 'application/json; charset=utf-8', cache: false, async: false,
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(_objRef.settings.container));
					},
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.EhValido) {
							abreModal = response.EhValido;
						} else if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(MasterPage.getContent(_objRef.settings.container), response.Msg);
						}
					}
				});
			}

			if (abreModal || params.documentoId == 0) {
				Modal.abrir(_objRef.settings.urls.associarRequerimento, null, function (container) {
					RequerimentoListar.load(container, { associarFuncao: _objRef.callBackAssociarRequerimento });
					Modal.defaultButtons(container);
				});
			}
		},

		callBackAssociarRequerimento: function (Requerimento) {
			if ($('.txtNumeroReq', _objRef.settings.container).val() == Requerimento.Id) {
				return true;
			}

			var params = { id: Requerimento.Id, excetoId: $('.hdnDocumentoId', _objRef.settings.container).val() };
			var retorno = _objRef.obterAjax(_objRef.settings.urls.obterRequerimento, params, $('.divRequerimento', _objRef.settings.containerModal));

			if (!retorno.EhValido) {
				return retorno.Msg;
			}

			$('.txtNumeroReq', _objRef.settings.container).val(Requerimento.Id);
			$('.hdnRequerimentoSituacao', _objRef.settings.container).val(Requerimento.SituacaoId);
			$('.spnPdfRequerimento', _objRef.settings.container).removeClass('hide');

			_objRef.configurarAssociarMultiplo();
			return true;
		},

		onAbrirPdfRequerimento: function () {
			var id = parseInt($('.txtNumeroReq', _objRef.settings.container).val());
			MasterPage.redireciona(_objRef.settings.urls.pdfRequerimento + "?id=" + id);
			MasterPage.carregando(false);
		},
	    //--------------- Fim REQUERIMENTO ---------------\\
        
		obterAjax: function (url, params, container) {
			MasterPage.carregando(true);
			var retorno = null;

			$.ajax({ url: url, data: params, cache: false, async: false,
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(_objRef.settings.container));
				},
				success: function (response, textStatus, XMLHttpRequest) {
					Mensagem.limpar(MasterPage.getContent(_objRef.settings.container));

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
			MasterPage.carregando(false);
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
				return new Array(_objRef.settings.Mensagens.ResponsaveljaAdicionado);
			}

			$('.hdnResponsavelId', item).val(responsavel.Id);
			$('.nomeRazao', item).val(responsavel.NomeRazaoSocial);
			$('.cpfCnpj', item).val(responsavel.CPFCNPJ);
			$('.btnAsmEditar', item).show();
			return true;
		},

		onResponsavelEditar: function (pessoaObj, item, extra) {
			$('.hdnResponsavelId', item).val(pessoaObj.Id);
			$('.nomeRazao', item).val(pessoaObj.NomeRazaoSocial);
			$('.cpfCnpj', item).val(pessoaObj.CPFCNPJ);
		},

		onMensagemExcluirResponsavel: function (item, extra) {
			return Mensagem.replace(_objRef.settings.Mensagens.ResponsavelTecnicoRemover, '#texto', $(item).find('.asmItemTexto').val()).Texto;
		},

		//--------------- ENVIAR ARQUIVO ---------------\\
		onEnviarArquivoClick: function () {
			var nomeArquivo = $('.inputFile', _objRef.settings.container).val();

			if (nomeArquivo === '') {
				Mensagem.gerar(_objRef.settings.container, new Array(_objRef.settings.Mensagens.ArquivoObrigatorio));
				return;
			}

			$('.btnArqComplementar', _objRef.settings.container).button({ disabled: true });
			var inputFile = $('.inputFile', _objRef.settings.container);
			FileUpload.upload(_objRef.settings.urls.enviarArquivo, inputFile, _objRef.callBackArqEnviado);
		},

		callBackArqEnviado: function (controle, retorno, isHtml) {
			var ret = eval('(' + retorno + ')');
			if (ret.Arquivo != null) {
				$('.txtArquivoNome', _objRef.settings.container).val(ret.Arquivo.Nome);
				$('.hdnArquivoJson', _objRef.settings.container).val(JSON.stringify(ret.Arquivo));

				$('.spanInputFile', _objRef.settings.container).addClass('hide');
				$('.txtArquivoNome', _objRef.settings.container).removeClass('hide');

				$('.btnArqComplementar', _objRef.settings.container).addClass('hide');
				$('.btnArqComplementarLimpar', _objRef.settings.container).removeClass('hide');
			} else {
				_objRef.onLimparArquivoClick();
			}

			Mensagem.gerar(_objRef.settings.container, ret.Msg);
			$('.btnArqComplementar', _objRef.settings.container).button({ disabled: false });
		},

		onLimparArquivoClick: function () {
			$('.hdnArquivoJson', _objRef.settings.container).val('');
			$('.inputFile', _objRef.settings.container).val('');

			$('.spanInputFile', _objRef.settings.container).removeClass('hide');
			$('.txtArquivoNome', _objRef.settings.container).addClass('hide');

			$('.btnArqComplementar', _objRef.settings.container).removeClass('hide');
			$('.btnArqComplementarLimpar', _objRef.settings.container).addClass('hide');
			$('.lnkArquivo', _objRef.settings.container).remove();
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
				Fiscalizacao: { Id: 0, SituacaoId: 0 },
				Empreendimento: { Id: 0 },
				Folhas: 0,
				InteressadoLivre: '',
				InteressadoLivreTelefone: '',
				DestinatarioSetor: { Id: 0 },
				Destinatario: { Id: 0 },
                Assinantes: [],
                Atividades: [],
				Responsaveis: []
			};

			objetoDocumento.Id = $('.hdnDocumentoId', _objRef.settings.container).val();
			objetoDocumento.Numero = $('.txtNumero', _objRef.settings.container).val();
			objetoDocumento.Tipo.Id = $('.ddlDocumentoTipos', _objRef.settings.container).val();
			objetoDocumento.DataCadastro = { DataTexto: $('.txtDataCriacao', _objRef.settings.container).val() };
			objetoDocumento.Volume = $('.txtQuantidadeDocumento', _objRef.settings.container).val();
			objetoDocumento.Arquivo = $.parseJSON($('.hdnArquivoJson', _objRef.settings.container).val());
			objetoDocumento.Nome = $('.txtNomeDocumento', _objRef.settings.container).val();
			objetoDocumento.Assunto = $('.txtAssunto', _objRef.settings.container).val();
			objetoDocumento.Descricao = $('.txtDescricao', _objRef.settings.container).val();
			objetoDocumento.ChecagemRoteiro.Id = $('.txtCheckListId', _objRef.settings.container).val();
			objetoDocumento.SetorId = $('.ddlSetor', _objRef.settings.container).val();
			objetoDocumento.Interessado.Id = $('.hdnInteressadoId', $('.hdnInteressadoId', _objRef.settings.container).closest('fieldset:visible')).val();

            objetoDocumento.DestinatarioSetor.Id = $('.ddlSetoresDestinatario', _objRef.settings.container).val();
            objetoDocumento.Destinatario.Id = $('.ddlDestinatarios', _objRef.settings.container).val();
			var assinantesContainer = _objRef.settings.container.find('.fdsAssinante');
			$('.hdnItemJSon', assinantesContainer).each(function () {
                var objAssinante = String($(this).val());
				if (objAssinante != '' && objAssinante != '0') {
                    objetoDocumento.Assinantes.push(JSON.parse(objAssinante));
                }
            });

			objetoDocumento.Fiscalizacao.Id = $('.txtNumeroFiscalizacao', _objRef.settings.container).val();
			objetoDocumento.Fiscalizacao.SituacaoId = $('.hdnFiscalizacaoSituacao', _objRef.settings.container).val();

			objetoDocumento.ProtocoloAssociado = { Id: $('.hdnProtocoloAssociadoId', _objRef.settings.container).val() };
			objetoDocumento.ChecagemPendencia.Id = $('.txtChecagemPendenciaId', _objRef.settings.container).val();
			objetoDocumento.Requerimento.Id = $('.txtNumeroReq', _objRef.settings.container).val();
			objetoDocumento.Requerimento.SituacaoId = $('.hdnRequerimentoSituacao', _objRef.settings.container).val();

			objetoDocumento.InteressadoLivre = $('.txtInteressadoLivre', _objRef.settings.container).val();
			objetoDocumento.InteressadoLivreTelefone = $('.txtInteressadoLivreTelefone', _objRef.settings.container).val();
			objetoDocumento.Folhas = $('.txtQuantidadeFolhas', _objRef.settings.container).val();

			if (objetoDocumento.Requerimento.Id != '') {
				objetoDocumento.Empreendimento.Id = $('.hdnEmpreendimentoId', _objRef.settings.container).val();

				// Início Atividades
				objetoDocumento.Atividades = AtividadeSolicitadaAssociar.gerarObjeto(_objRef.settings.container);
				// Fim Atividades

				// Início Responsáveis
				var responsaveis = $('.divConteudoResponsavelTec .asmItens .asmItemContainer', _objRef.settings.container);

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
			var documento = _objRef.montarObjetoDocumento();
			MasterPage.carregando(true);
			var params = { documento: documento };
			var acao = _objRef.Id > 0 ? _objRef.settings.urls.editar : _objRef.settings.urls.criar;

			Mensagem.limpar();

			$.ajax({ url: acao, data: JSON.stringify(params), type: 'POST', typeData: 'json',
				contentType: 'application/json; charset=utf-8', cache: false, async: false,
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(_objRef.settings.container));
				},
				success: function (response, textStatus, XMLHttpRequest) {

					if (response.IsDocumentoSalvo) {
						if (typeof response.UrlRedireciona != "undefined" && response.UrlRedireciona !== null && isRedirecionar) {
							MasterPage.redireciona(response.UrlRedireciona);
							return;
						}
					} else if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(MasterPage.getContent(_objRef.settings.container), response.Msg);
					}
				}
			});
			MasterPage.carregando(false);
		}
	};
}
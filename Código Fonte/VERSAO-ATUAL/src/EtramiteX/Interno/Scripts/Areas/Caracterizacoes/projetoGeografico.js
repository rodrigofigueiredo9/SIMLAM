/// <reference path="../../Lib/JQuery/jquery.json - 2.2.min.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../mensagem.js" />

ProjetoGeografico = {
	settings: {
		projetoId: null,
		dependencias: null,
		isCadastrarCaracterizacao: true,
		isDominialidade: false,
		possuiAPPNaoCaracterizada: false,
		possuiARLNaoCaracterizada: false,
		ArquivoEnviadoFilaTipo: null,
		isVisualizar: false,
		isFinalizado: false,
		urls: {
			verificarAreaNaoCaracterizada: null,
			coordenadaGeo: null,
			criarParcial: null,
			confirmarAlterarArea: null,
			alterarAreaAbrangencia: null,
			cancelarProcessamentoArquivosVet: null,
			refazer: null,
			atualizar: null,
			recarregar: null,
			finalizar: null,
			salvar: null,
			avancar: null,
			excluir: null,
			arquivos: [],
			baixarArquivosCredenciado: ''
		},
		atualizarDependenciasModalTitulo: '',
		textoMerge: null,
		isObteveMerge: false,
		isVisualizarCredenciado: false,
		EmpreendimentoEstaDentroAreaAbrangencia: true
	},
	mensagens: null,
	container: null,

	load: function (container, options) {
		if (options) { $.extend(ProjetoGeografico.settings, options); }
		ProjetoGeografico.container = MasterPage.getContent(container);

		$(ProjetoGeografico.container).delegate('.radioTiPoMecanismo', 'change', ProjetoGeografico.onChangeTipoElaboracao);
		$(ProjetoGeografico.container).delegate('.radioTiPoMecanismo', 'mousedown', ProjetoGeografico.onclickTipoElaboracao);
		$(ProjetoGeografico.container).delegate('.radioTiPoMecanismo', 'click', ProjetoGeografico.onclickTipoElaboracao);
		$(ProjetoGeografico.container).delegate('.btnSelecionarCoordenada', 'click', ProjetoGeografico.onAbrirNavegardorCoordenada);
		$(ProjetoGeografico.container).delegate('.btnAlterarCoordenada', 'click', ProjetoGeografico.onAlterarCoordenada);
		$(ProjetoGeografico.container).delegate('.btnObterCoordenadaAuto', 'click', ProjetoGeografico.onGerarCoordenadaAutomatico);

		$(ProjetoGeografico.container).delegate('.btnRefazer', 'click', ProjetoGeografico.onRefazer);
		$(ProjetoGeografico.container).delegate('.btnAtualizar', 'click', ProjetoGeografico.onAtualizar);
		$(ProjetoGeografico.container).delegate('.btnRecarregar', 'click', ProjetoGeografico.onRecarregar);
		$(ProjetoGeografico.container).delegate('.btnExluir', 'click', ProjetoGeografico.onExcluir);
		$(ProjetoGeografico.container).delegate('.btnFinalizar', 'click', ProjetoGeografico.onFinalizar);
		$(ProjetoGeografico.container).delegate('.btnSalvar ', 'click', ProjetoGeografico.onSalvar);
		$(ProjetoGeografico.container).delegate('.btnAvancar ', 'click', ProjetoGeografico.onAvancar);

		BaseReferencia.load($('.containerBaseReferencia', ProjetoGeografico.container), options);
		EnviarProjeto.load($('.containerEnviarProjeto', ProjetoGeografico.container), options);
		Desenhador.load($('.containerDesenhador', ProjetoGeografico.container), options);
		Sobreposicao.load($('.containerSobreposicao', ProjetoGeografico.container), options);

		if (ProjetoGeografico.settings.projetoId != 0) {
			BaseReferencia.obterSituacao();
			EnviarProjeto.obterSituacao();
			Desenhador.obterSituacao();
		}

		if (ProjetoGeografico.settings.textoMerge) {
			var finalizado = !$('.btnFinalizar', ProjetoGeografico.container).is(':visible');
			ProjetoGeografico.abrirModalRedireciona(ProjetoGeografico.settings.textoMerge, finalizado);

			if (!finalizado) {
				ProjetoGeografico.gerarenciarBotoes({ recarregar: true, excluir: true, finalizar: true, salvar: true });
			}
		}

		if (!ProjetoGeografico.settings.EmpreendimentoEstaDentroAreaAbrangencia) {
			Mensagem.gerar(ProjetoGeografico.container, [ProjetoGeografico.mensagens.EmpreendimentoForaAbrangencia]);
		}

		ProjetoGeografico.mensagemAreaNaoCaracterizada();
	},

	mensagemAreaNaoCaracterizada: function () {
		var situacao = +$('.hdnArquivoEnviadoSituacaoId', EnviarProjeto.container).val() || +$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val()

		if (ProjetoGeografico.settings.isDominialidade && (situacao == 14)) {//Processado
			if (ProjetoGeografico.settings.possuiAPPNaoCaracterizada) {
				$('.divAlertaAPP', ProjetoGeografico.container).removeClass('hide');
			} else {
			}

			if (ProjetoGeografico.settings.possuiARLNaoCaracterizada) {
				$('.divAlertaARL', ProjetoGeografico.container).removeClass('hide');
			} else {
			}
		}
	},

	abrirModalRedireciona: function (textoModal, finalizado) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', ProjetoGeografico.container).attr('href'));
			},
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				if (ProjetoGeografico.settings.isVisualizar) {
					Modal.fechar(conteudoModal);
				} else {
					if (finalizado) {
						ProjetoGeografico.callBackRefazer(ProjetoGeografico.container);
					} else {
						if ($('.radioTiPoMecanismo:checked', ProjetoGeografico.container).val() == 1) {
							EnviarProjeto.onCancelarProcessamento();
						} else {
							Desenhador.onCancelarProcessamento();
							Desenhador.obterSituacao(true);
						}
						Modal.fechar(conteudoModal);
					}
				}
			},
			conteudo: textoModal,
			titulo: ProjetoGeografico.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	onAvancar: function () {
		MasterPage.redireciona(ProjetoGeografico.settings.urls.avancar);
	},

	onSalvar: function (option) {
		var data = { projeto: ProjetoGeografico.montarObjeto(), url: ProjetoGeografico.settings.urls.avancar };
		var caracterizacaoTipo = $('.hdnCaracterizacaoTipo', ProjetoGeografico.container).val();
		var settings = { url: ProjetoGeografico.settings.urls.salvar, data: data, callBack: (caracterizacaoTipo == 4 ? ProjetoGeografico.callBackPost : ProjetoGeografico.callBackSalvar) };
		$.extend(settings, option);
		if (ProjetoGeografico.validarProjeto(data.projeto)) {

			ProjetoGeografico.postGet({
				url: settings.url,
				data: settings.data,
				callBack: settings.callBack,
				async: false,
				mostrarCarregando: true
			});
		}
	},

	callBackSalvar: function (retorno) {

		Mensagem.gerar(ProjetoGeografico.container, retorno.Msg);

		if (!retorno.EhValido) {
			if (!ProjetoGeografico.settings.isObteveMerge) {
				ProjetoGeografico.abrirModalMergeAtualizarDominio(retorno.Mensagem);
				return;
			}
		}

		if ($('.btnExluir', ProjetoGeografico.container).is(':visible')) {
			ProjetoGeografico.gerarenciarBotoes({ recarregar: true, excluir: true, salvar: true, finalizar: true });
		} else {
			ProjetoGeografico.gerarenciarBotoes({ salvar: true, finalizar: true });
		}

		ContainerAcoes.load(ProjetoGeografico.container, { limparContainer: false, botoes: new Array({ label: 'Finalizar', callBack: ProjetoGeografico.onFinalizar }) });
	},

	validarProjeto: function (obj) {
		var erroMsgs = new Array();

		if (obj.NivelPrecisaoId < 1) {
			erroMsgs.push(ProjetoGeografico.mensagens.NivelPrecisaoObrigatorio);
		}

		if ((Number(obj.MenorX) <= 0) || (Number(obj.MaiorY) <= 0) || (Number(obj.MaiorX) <= 0) || (Number(obj.MenorY) <= 0)) {
			erroMsgs.push(ProjetoGeografico.mensagens.AreaAbrangenciaObrigatorio);
		}

		if ($('.radioTiPoMecanismo:radio', ProjetoGeografico.container).is(':visible') && (obj.MecanismoElaboracaoId || 0) < 1) {
			erroMsgs.push(ProjetoGeografico.mensagens.MecanismoObrigatorio);
		}

		if (erroMsgs.length > 0) {
			Mensagem.gerar(ProjetoGeografico.container, erroMsgs);
			return false;
		}

		Mensagem.limpar(ProjetoGeografico.container);
		return true;
	},

	montarObjeto: function () {

		var obj = {};
		obj.Id = $('.hdnProjetoId', ProjetoGeografico.container).val();
		obj.EmpreendimentoEasting = $('.hdnEmpEasting', ProjetoGeografico.container).val();
		obj.EmpreendimentoNorthing = $('.hdnEmpNorthing', ProjetoGeografico.container).val();
		obj.EmpreendimentoId = $('.hdnEmpreendimentoId', ProjetoGeografico.container).val();
		obj.MecanismoElaboracaoId = $('.radioTiPoMecanismo:radio:checked', ProjetoGeografico.container).val();
		obj.CaracterizacaoId = $('.hdnCaracterizacaoTipo', ProjetoGeografico.container).val();
		obj.NivelPrecisaoId = $('.ddlNivel', ProjetoGeografico.container).val();
		obj.SituacaoId = $('.hdnProjetoSituacaoId', ProjetoGeografico.container).val();
		obj.SituacaoTexto = $('.SituacaoProjetoTexto', ProjetoGeografico.container).text();
		obj.Arquivo = EnviarProjeto.obterArquivo();

		obj.MenorX = $('.txtMenorX', ProjetoGeografico.container).val();
		obj.MenorY = $('.txtMenorY', ProjetoGeografico.container).val();

		obj.MaiorX = $('.txtMaiorX', ProjetoGeografico.container).val();
		obj.MaiorY = $('.txtMaiorY', ProjetoGeografico.container).val();

		obj.Dependencias = JSON.parse(ProjetoGeografico.settings.dependencias);

		obj.ArquivosOrtofotos = [];

		$('.gridArquivosRaster tbody tr', BaseReferencia.container).each(function (i, item) {
		    var arqOrtofoto = {
		        Id: $('.hdnOrtoFotoMosaicoId', item).val(),
		        Caminho: $('.hdnCaminho', item).val(),
		        Chave: $('.hdnChave', item).val(),
		        ChaveData: $('.hdnChaveData', item).val()
		    };

		    obj.ArquivosOrtofotos.push(arqOrtofoto);
		});

		if ($('.btnVerificarSobreposicao', ProjetoGeografico.container).data('Sobreposicoes')) {
			obj.Sobreposicoes = $('.btnVerificarSobreposicao', ProjetoGeografico.container).data('Sobreposicoes');
		}

		return obj;
	},

	onAbrirNavegardorCoordenada: function (options) {

		var settings = { callBack: ProjetoGeografico.callBackAreaAbrangencia };
		$.extend(settings, options);

		Modal.abrir(ProjetoGeografico.settings.urls.coordenadaGeo, null, function (container) {
			Coordenada.load(container, {
				easting: $('.txtMenorX', ProjetoGeografico.container).val(),
				northing: $('.txtMenorY', ProjetoGeografico.container).val(),
				easting2: $('.txtMaiorX', ProjetoGeografico.container).val(),
				northing2: $('.txtMaiorY', ProjetoGeografico.container).val(),
				empreendimentoNorthing: $('.hdnEmpNorthing', ProjetoGeografico.container).val(),
				empreendimentoEasting: $('.hdnEmpEasting', ProjetoGeografico.container).val(),
				callBackSalvarCoordenada: settings.callBack
			});
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalFull);
	},

	callBackAreaAbrangencia: function (retorno) {

		if (retorno.easting1 > retorno.easting2) {
			var aux = retorno.easting1;
			retorno.easting1 = retorno.easting2;
			retorno.easting2 = aux;
		}

		if (retorno.northing1 > retorno.northing2) {
			var aux = retorno.northing1;
			retorno.northing1 = retorno.northing2;
			retorno.northing2 = aux;
		}

		$('.txtMenorX', ProjetoGeografico.container).val(retorno.easting1);
		$('.txtMenorY', ProjetoGeografico.container).val(retorno.northing1);
		$('.txtMaiorX', ProjetoGeografico.container).val(retorno.easting2);
		$('.txtMaiorY', ProjetoGeografico.container).val(retorno.northing2);
		$('.divCoordenada', ProjetoGeografico.container).removeClass('hide');
		$('.btnSelecionarCoordenada', ProjetoGeografico.container).addClass('hide');
		$('.btnAlterarCoordenada', ProjetoGeografico.container).removeClass('hide');
		$('.fsMecanismo', ProjetoGeografico.container).removeClass('hide');
		$('.fsEnviarProjeto', ProjetoGeografico.container).addClass('hide');

		BaseReferencia.settings.gerouOrtoFotoMosaico = false;
	},

	onGerarCoordenadaAutomatico: function (options) {
		var empNorthing = +$('.hdnEmpNorthing', ProjetoGeografico.container).val();
		var empEasting = +$('.hdnEmpEasting', ProjetoGeografico.container).val();

		var abrangeObj = {
			easting1: empEasting - 5000,
			northing1: empNorthing - 5000,
			easting2: empEasting + 5000,
			northing2: empNorthing + 5000
		};

		if (ProjetoGeografico.settings.projetoId == 0) {
			ProjetoGeografico.callBackAreaAbrangencia(abrangeObj);
			return;
		}

		Modal.confirma({
			btnOkCallback: function (modalContainer) {
				Modal.fechar(modalContainer);
				ProjetoGeografico.callBackConfirmadoAlteracao(abrangeObj);
			},
			titulo: "Alterar Área de Abrangência",
			conteudo: ProjetoGeografico.mensagens.ConfirmarAreaAbrangencia.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	callBackPost: function (resultado, container) {
		MasterPage.carregando(false);

		if (!resultado.EhValido) {
			Mensagem.gerar(container, resultado.Msg);
			return;
		}
		if (resultado.Url != null && typeof resultado.Url != "undefined") {
			MasterPage.redireciona(resultado.Url);
		}
	},

	onFinalizar: function () {
		Modal.confirma({
			btnOkCallback: function (container) { ProjetoGeografico.callBackFinalizar(MasterPage.getContent(container)) },
			titulo: "Finalizar projeto geográfico",
			conteudo: ProjetoGeografico.mensagens.ConfirmacaoFinalizar.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	callBackFinalizar: function (container) {
		var dados = { projeto: ProjetoGeografico.montarObjeto(), url: ProjetoGeografico.settings.urls.avancar };
		ProjetoGeografico.postGet({
			url: ProjetoGeografico.settings.urls.finalizar,
			data: dados,
			callBack: ProjetoGeografico.callBackPost,
			container: container,
			async: false
		});
	},

	onExcluir: function () {
		Modal.confirma({
			btnOkCallback: function (container) { ProjetoGeografico.callBackExcluir(MasterPage.getContent(container)) },
			titulo: "Excluir rascunho projeto geográfico",
			conteudo: ProjetoGeografico.mensagens.ConfirmarExcluir.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	callBackExcluir: function (container) {
		var dados = { projeto: ProjetoGeografico.montarObjeto(), isCadastrarCaracterizacao: ProjetoGeografico.isCadastrarCaracterizacao };
		ProjetoGeografico.postGet({
			url: ProjetoGeografico.settings.urls.excluir,
			data: dados,
			callBack: ProjetoGeografico.callBackPost,
			async: false
		});
	},

	onRefazer: function () {
		Modal.confirma({
			btnOkCallback: function (container) { ProjetoGeografico.callBackRefazer(MasterPage.getContent(container)); },
			titulo: "Refazer projeto geográfico",
			conteudo: ProjetoGeografico.mensagens.ConfirmacaoRefazer.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	callBackRefazer: function (container) {
		var dados = { projeto: ProjetoGeografico.montarObjeto(), isCadastrarCaracterizacao: ProjetoGeografico.isCadastrarCaracterizacao };
		ProjetoGeografico.postGet({
			url: ProjetoGeografico.settings.urls.refazer,
			data: dados,
			callBack: function (resultado, container) {
				if ($('.radioTiPoMecanismo:checked', ProjetoGeografico.container).val() == 1) {
					EnviarProjeto.onCancelarProcessamento();
				} else {
					Desenhador.onCancelarProcessamento();
				}
				ProjetoGeografico.callBackPost(resultado, container);
			},
			container: container,
			async: false
		});
	},

	onAtualizar: function () {
		Modal.confirma({
			btnOkCallback: function (container) { ProjetoGeografico.callBackAtualizar(MasterPage.getContent(container)); },
			titulo: "Atualizar projeto geográfico",
			conteudo: ProjetoGeografico.mensagens.ConfirmacaoAtualizar.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	callBackAtualizar: function (container) {
		var dados = { projeto: ProjetoGeografico.montarObjeto(), isCadastrarCaracterizacao: ProjetoGeografico.isCadastrarCaracterizacao };
		ProjetoGeografico.postGet({
			url: ProjetoGeografico.settings.urls.atualizar,
			data: dados,
			callBack: function (resultado, container) {
				if ($('.radioTiPoMecanismo:checked', ProjetoGeografico.container).val() == 1) {
					EnviarProjeto.onCancelarProcessamento();
				} else {
					Desenhador.onCancelarProcessamento();
				}
				ProjetoGeografico.callBackPost(resultado, container);
			},
			container: container,
			async: false
		});
	},

	onRecarregar: function () {
		Modal.confirma({
			btnOkCallback: function (container) { ProjetoGeografico.callBackRecarregar(MasterPage.getContent(container)) },
			titulo: "Recarregar projeto geográfico",
			conteudo: ProjetoGeografico.mensagens.ConfirmacaoRecarregar.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	callBackRecarregar: function (container) {
		BaseReferencia.stopThread();
		EnviarProjeto.stopThread();
		Desenhador.stopThread();

		var dados = { projeto: ProjetoGeografico.montarObjeto(), isCadastrarCaracterizacao: ProjetoGeografico.isCadastrarCaracterizacao };
		ProjetoGeografico.postGet({
			url: ProjetoGeografico.settings.urls.recarregar,
			data: dados,
			callBack: ProjetoGeografico.callBackPost,
			async: false
		});
	},

	validarChange: function () {
		$('.divExistemProcessamentosAndamento', ProjetoGeografico.container).addClass('hide');
		$('.divExistemProcessamentosAndamento', ProjetoGeografico.container).hide();

	    var situacaoId = 0;
	    var isImportador = !$('.divImportador', ProjetoGeografico.container).hasClass('hide');

	    if (isImportador) {
	        situacaoId = +$('.hdnArquivoEnviadoSituacaoId', EnviarProjeto.container).val();
	    } else {
	        situacaoId = +$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val(); 
	    }

	    if ($.inArray(situacaoId, [0,3,4,5,8,9,10,13,14,15]) == -1) {
	        
	        $('.radioTiPoMecanismo', ProjetoGeografico.container).removeAttr('checked');

	        if (isImportador) {
	            $('.radioTiPoMecanismo[value=1]', ProjetoGeografico.container).attr('checked', 'checked');
	        } else {
	            $('.radioTiPoMecanismo[value=2]', ProjetoGeografico.container).attr('checked', 'checked');
	        }

	        $('.divExistemProcessamentosAndamento', ProjetoGeografico.container).fadeIn(600, function () {
	        	$('.divExistemProcessamentosAndamento', ProjetoGeografico.container).removeClass('hide');
	        });

	        return false;
	    }

	    return true;
	},

	onclickTipoElaboracao: function (evt) {

	    if (!ProjetoGeografico.validarChange()) {
	        evt.stopPropagation();
	        return false;
	    }
	},

	onChangeTipoElaboracao: function (evt) {
		
	    if (!ProjetoGeografico.validarChange()) {
	        evt.stopPropagation();
	        return false;
	    }

	    if ($('.hdnProjetoId', ProjetoGeografico.container).val() == 0) {
			ProjetoGeografico.criarProjetoGeografico();

			if ($('.hdnProjetoId', ProjetoGeografico.container).val() == 0) {
				$('.radioTiPoMecanismo', ProjetoGeografico.container).removeAttr("checked");
				return;
			}
	    }

	    EnviarProjeto.mostrar(false);
		Desenhador.mostrar(false);
		Sobreposicao.mostrar(false);

		if ($('.radioTiPoMecanismo:checked', ProjetoGeografico.container).val() == 1) {
			EnviarProjeto.mostrar(true);

			$('.gridImportarArquivos tbody tr', ProjetoGeografico.container).remove();
			$('.gridImportarArquivos tbody', ProjetoGeografico.container).append($('.desenhadorArquivosGrid tbody', Desenhador.container).html());

			$('.divImportarArquivo', ProjetoGeografico.container).toggleClass('hide', $('.gridImportarArquivos tbody tr', ProjetoGeografico.container).length <= 0);

			BaseReferencia.settings.gerouOrtoFotoMosaico = false;
			BaseReferencia.gerarOrtoMosaico();
		} else {
			Desenhador.mostrar(true);

			$('.desenhadorArquivosGrid tbody tr', Desenhador.container).remove();
			$('.desenhadorArquivosGrid tbody', Desenhador.container).append($('.gridImportarArquivos tbody', EnviarProjeto.container).html());

			$('.divDesenhadorArquivo', Desenhador.container).toggleClass('hide', $('.desenhadorArquivosGrid tbody tr', Desenhador.container).length <= 0);

			Desenhador.exibirReenviar();
		}

		var situacaoId = +$('.hdnArquivoEnviadoSituacaoId', EnviarProjeto.container).val();
		if (situacaoId == 0) {
		    situacaoId = +$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val();
		}

		if (situacaoId != 0) {//Processado
		    Mensagem.gerar(ProjetoGeografico.container, [ProjetoGeografico.mensagens.AlterarMecanismo]);
		}

		$('.divAlertaAPP', ProjetoGeografico.container).addClass('hide');
		$('.divAlertaARL', ProjetoGeografico.container).addClass('hide');
	},

	criarProjetoGeografico: function () {
		var data = { projeto: ProjetoGeografico.montarObjeto() };
		ProjetoGeografico.onSalvar({ url: ProjetoGeografico.settings.urls.criarParcial,
			callBack: ProjetoGeografico.callBackCriar,
			data: data
		});
	},

	callBackCriar: function (resultado) {

		if (!resultado.EhValido) {
			Mensagem.gerar(ProjetoGeografico.container, resultado.Msg);
			return;
		}

		$('.hdnProjetoId', ProjetoGeografico.container).val(resultado.Id)
		ProjetoGeografico.settings.projetoId = resultado.Id;
		BaseReferencia.gerarOrtoMosaico();
	},

	postGet: function (parametros) {

		var resultado;
		var configuracao = { url: null,
			data: null,
			type: 'POST',
			async: true,
			callBack: function (response) { if (!response.EhValido) { Mensagem.gerar(ProjetoGeografico.container, response.Msg); } },
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			mostrarCarregando: false,
			serializar: true,
			container: ProjetoGeografico.container
		};

		$.extend(configuracao, parametros);

		if (configuracao.mostrarCarregando) {
			MasterPage.carregando(true);
		}

		$.ajax({
			url: configuracao.url,
			type: configuracao.type,
			data: (configuracao.serializar ? JSON.stringify(configuracao.data) : configuracao.data),
			dataType: configuracao.dataType,
			contentType: configuracao.contentType,
			async: configuracao.async,
			cache: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, BaseReferencia.container);

				if (configuracao.mostrarCarregando) {
					MasterPage.carregando(false);
				}
			},
			success: function (response, textStatus, XMLHttpRequest) {
				configuracao.callBack(response, configuracao.container);

				if (configuracao.mostrarCarregando) {
					MasterPage.carregando(false);
				}
			}
		});
	},

	mostrarEnvioProjeto: function () {
		if ($('.gridArquivosVetoriais tbody .hdnArquivoIdRelacionamento[value=0]', ProjetoGeografico.container).length == 0) {
			$('.fsEnviarProjeto', ProjetoGeografico.container).removeClass('hide');
		}
	},

	onAlterarCoordenada: function () {

		if (ProjetoGeografico.settings.projetoId == 0) {
			ProjetoGeografico.onAbrirNavegardorCoordenada();
			return;
		}

		Modal.confirma({
			btnOkCallback: function (container) { ProjetoGeografico.onConfirmadoAlteracao(container) },
			titulo: "Alterar Área de Abrangência",
			conteudo: ProjetoGeografico.mensagens.ConfirmarAreaAbrangencia.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});

	},

	onConfirmadoAlteracao: function (modal) {
		Modal.fechar(modal);
		ProjetoGeografico.onAbrirNavegardorCoordenada({ callBack: ProjetoGeografico.callBackConfirmadoAlteracao });
	},

	callBackConfirmadoAlteracao: function (retorno) {

		ProjetoGeografico.callBackAreaAbrangencia(retorno);

		var data = { projeto: ProjetoGeografico.montarObjeto() };

		if (!ProjetoGeografico.validarProjeto(data.projeto)) {
			return;
		}

		BaseReferencia.stopThread();
		EnviarProjeto.stopThread();

		ProjetoGeografico.postGet({
			data: data,
			url: ProjetoGeografico.settings.urls.alterarAreaAbrangencia,
			callBack: ProjetoGeografico.callBackAlterarAreaAbrangencia,
			mostrarCarregando: true
		});
	},

	callBackAlterarAreaAbrangencia: function (response) {

		if (!response.EhValido) {
			Mensagem.gerar(ProjetoGeografico.container, response.Msg);
			return;
		}

		var linhaModelo = $('.gridArquivosVetoriais tbody tr.linhaModelo', BaseReferencia.container);

        //Importador de shape
		$('.gridImportarArquivos tbody tr', EnviarProjeto.container).remove();
		$('.gridArquivosVetoriais tbody tr', BaseReferencia.container).remove();
		$('.gridArquivosRaster tbody tr', BaseReferencia.container).remove();
		$('.gridDadosDominio tbody tr', BaseReferencia.container).remove();	    

		$('.divSituacaoProjeto', EnviarProjeto.container).addClass('hide');
		$('.divImportarArquivo', EnviarProjeto.container).addClass('hide');
		$('.divEnviarProjeto', EnviarProjeto.container).removeClass('hide');

	    //Desenahdor
		$('.desenhadorArquivosGrid tbody tr', Desenhador.container).remove();
		Desenhador.mostrarArquivo(false);
		Sobreposicao.mostrar(false);
		Desenhador.obterSituacao(true);

		var listaArqVet = $.parseJSON($(".hdnEstadoPadrao", BaseReferencia.container).val());

		$(listaArqVet).each(function (i, item) {
			var linha = $('.trTempletaArqVetorial', BaseReferencia.container).clone();

			linha.removeClass('trTempletaArqVetorial');

			$('.txtNome', linha).text(item.Texto);
			$('.txtSituacao', linha).text(item.SituacaoTexto);
			$('.hdnSituacaoId', linha).val(item.Situacao);
			$('.hdnArquivoId', linha).val(item.Id);
			$('.hdnArquivoIdRelacionamento', linha).val(item.IdRelacionamento);
			$('.hdnArquivoTipo', linha).val(item.Tipo);
			$('.hdnArquivoFilaTipo', linha).val(item.FilaTipo);

			BaseReferencia.gerenciarBotoesArqVetoriais(linha, { gerar: item.MostarGerar, baixar: item.MostarBaixar, regerar: item.MostarRegerar });

			$('.gridArquivosVetoriais tbody:last').append(linha);
		});

		$('.gridArquivosVetoriais tbody:last').append(linhaModelo);

		MasterPage.grid($('.gridArquivosVetoriais'));

		BaseReferencia.settings.gerouOrtoFotoMosaico = false;
		BaseReferencia.gerarOrtoMosaico();
	},

	gerarenciarBotoes: function (options) {

		var settings = { avancar: false, refazer: false, recarregar: false, excluir: false, finalizar: false, salvar: false };

		$.extend(settings, options);
		/*
		spanAvancar	-> 	btnAvancar
		spanRefazer	->	btnRefazer
		spanRecaregar -> btnRecarregar
		spanExcluir	-> 	btnExluir
		*/

		$('.spanAvancar', ProjetoGeografico.container).toggleClass('hide', !settings.avancar);
		$('.spanRefazer', ProjetoGeografico.container).toggleClass('hide', !settings.refazer);
		$('.spanRecaregar', ProjetoGeografico.container).toggleClass('hide', !settings.recarregar);
		$('.spanExcluir', ProjetoGeografico.container).toggleClass('hide', !settings.excluir);
		$('.spanFinalizar', ProjetoGeografico.container).toggleClass('hide', !settings.finalizar);
		$('.spnSalvar', ProjetoGeografico.container).toggleClass('hide', !settings.salvar);
		$('.spanOuCancelar', ProjetoGeografico.container).toggleClass('hide', !settings.salvar);
	},

	abrirModalMergeAtualizarDominio: function (textoModal) {

		Modal.confirma({
			removerFechar: true,
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				MasterPage.carregando(true);
				$.ajax({ url: ProjetoGeografico.settings.urls.obterMerge,
					data: JSON.stringify({ id: $('.hdnProjetoId', ProjetoGeografico.container).val() }),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.EhValido) {
							ProjetoGeografico.settings.isObteveMerge = true;
							$('.txtMenorX', ProjetoGeografico.container).val(response.Projeto.MenorX);
							$('.txtMaiorY', ProjetoGeografico.container).val(response.Projeto.MaiorY);
							$('.txtMaiorX', ProjetoGeografico.container).val(response.Projeto.MaiorX);
							$('.txtMenorY', ProjetoGeografico.container).val(response.Projeto.MenorY);

							var containerBaseReferencia = $('.containerBaseReferencia', ProjetoGeografico.container);
							$('.gridDadosDominio tbody', containerBaseReferencia).empty();

							$(response.Projeto.ArquivosDominio).each(function (i, item) {
								var trDominio = $('.templateDadosDominio', containerBaseReferencia).clone();
								item.Nome = item.Nome == "Croqui" ? item.Nome + " da dominialidade (PDF)" : item.Nome;
								$('.txtNome', trDominio).text(item.Nome).attr('title', item.Nome);
								$('.hdnArquivoId', trDominio).val(item.Id);
								trDominio.removeClass('templateDadosDominio');
								$('.gridDadosDominio tbody', containerBaseReferencia).append(trDominio);
							});
						}
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: ProjetoGeografico.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	}
}

BaseReferencia = {
	settings: {
		projetoId: null,
		urls: {
			atualizarArqVetoriais: null,
			gerarArquivoVetorial: null,
			gerarArquivoMosaio: null,
			baixarArquivoVetorial: null,
			baixarArquivoOrtoMosaico: null,
			salvarArquivoOrtoMosaico: null,
			baixarArquivos: null
		},
		gerouOrtoFotoMosaico: false,
		threadAtualizarSituacao: null,
		threadAtualizarSituacaoDelay: 1000 * 3,
		situacoesValidas: []
	},
	container: null,

	load: function (container, options) {

		if (options) { $.extend(BaseReferencia.settings, options); }

		BaseReferencia.container = container;

		$('.gridArquivosVetoriais', BaseReferencia.container).delegate('.btnGerar', 'click', BaseReferencia.onGerarArquivoVetorial);
		$('.gridArquivosVetoriais', BaseReferencia.container).delegate('.btnDownload', 'click', BaseReferencia.onBaixarArquivoVetorial);
		$('.gridArquivosVetoriais', BaseReferencia.container).delegate('.btnRegerar', 'click', BaseReferencia.onRegerarArquivoVetorial);
		$('.gridArquivosRaster', BaseReferencia.container).delegate('.btnDownload', 'click', BaseReferencia.onBaixarArquivoOrtoMosaico);
		$('.gridDadosDominio', BaseReferencia.container).delegate('.btnDownload', 'click', BaseReferencia.onBaixarDadosDominio);

		$('.gridArquivosVetoriais', BaseReferencia.container).delegate('.btnDownloadModelo', 'click', BaseReferencia.onBaixarArquivoModelo);
	},

	setarArquivoId: function (valor) {
		$('.hdnArquivoId', BaseReferencia.container).val(valor)
	},

	obterSituacao: function () {

		if (ProjetoGeografico.settings.isVisualizar) {
			return;
		}

		ProjetoGeografico.mostrarEnvioProjeto();
		var listaItens = new Array();

		$('.gridArquivosVetoriais tbody tr', BaseReferencia.container).each(function (i, linha) {
			var situacaoId = +$('.hdnSituacaoId', linha).val();

			if ($.inArray(situacaoId, BaseReferencia.settings.situacoesValidas) != -1) {
				listaItens.push({ IdRelacionamento: $('.hdnArquivoIdRelacionamento', linha).val() });
			}
		});

		if (listaItens.length > 0) {
			ProjetoGeografico.postGet({
				url: BaseReferencia.settings.urls.atualizarArqVetoriais,
				data: listaItens,
				callBack: BaseReferencia.callBackAtualizarSituacao
			});

			if (BaseReferencia.settings.threadAtualizarSituacao == null) {
				BaseReferencia.settings.threadAtualizarSituacao = setInterval(BaseReferencia.obterSituacao, BaseReferencia.settings.threadAtualizarSituacaoDelay);
			}
		} else {
			BaseReferencia.stopThread();
		}
	},

	callBackAtualizarSituacao: function (response) {

		if (BaseReferencia.settings.threadAtualizarSituacao == null) {
			return;
		}

		BaseReferencia.carregarArquivos(response);
	},

	carregarArquivos: function (response) {

		$('.gridArquivosVetoriais tbody tr', BaseReferencia.container).each(function (i, linha) {
			$(response.lista).each(function (j, item) {
				if ($('.hdnArquivoIdRelacionamento', linha).val() == item.IdRelacionamento) {
					$('.hdnArquivoId', linha).val(item.Id);
					$('.txtSituacao', linha).text(item.SituacaoTexto);
					$('.hdnSituacaoId', linha).val(item.Situacao);
					BaseReferencia.gerenciarBotoesArqVetoriais(linha, { gerar: item.MostarGerar, baixar: item.MostarBaixar, regerar: item.MostarRegerar });
				}
			});
		});
	},

	stopThread: function () {
		clearInterval(BaseReferencia.settings.threadAtualizarSituacao);
		BaseReferencia.settings.threadAtualizarSituacao = null;
	},

	gerarOrtoMosaico: function () {
		if (BaseReferencia.settings.gerouOrtoFotoMosaico) {
			return;
		}

		ProjetoGeografico.postGet({
			type: 'GET',
			serializar: false,
			url: BaseReferencia.settings.urls.gerarArquivoMosaio,
			data: 'projetoId=' + ProjetoGeografico.settings.projetoId,
			callBack: BaseReferencia.callBackOrtoFotoMosaico
		});
	},

	callBackOrtoFotoMosaico: function (response) {
		$('.gridArquivosRaster', BaseReferencia.container).addClass('hide');
		$('.gridArquivosRaster tbody tr', BaseReferencia.container).remove();

		if (!response.EhValido) {
			Mensagem.gerar(MasterPage.getContent(BaseReferencia.container), response.Msg);
			return;
		}

		BaseReferencia.settings.gerouOrtoFotoMosaico = true;

		if ($('.gridArquivosRaster tbody tr', BaseReferencia.container).length == 0) {
		    var dados = { projeto: ProjetoGeografico.montarObjeto() };
		    ProjetoGeografico.postGet({
		        url: BaseReferencia.settings.urls.salvarArquivoOrtoMosaico,
		        data: dados,
		        container: BaseReferencia.container,
		        async: true
		    });
		}

		$(response.Lista).each(function (i, item) {
			var template = $('.templateMosaico', BaseReferencia.container).clone();
			template.removeClass('templateMosaico');

			$('.hdnOrtoFotoMosaicoId', template).val(item.Id);
			$('.hdnCaminho', template).val(item.Caminho);
			$('.hdnChave', template).val(item.Chave);
			$('.hdnChaveData', template).val(item.ChaveData);
			$('.txtNome', template).text(item.Texto).attr('title', item.Texto);

			$('.gridArquivosRaster tbody:last', BaseReferencia.container).append(template);
		});

		if ($(response.Lista).length > 0) {
			$('.gridArquivosRaster', BaseReferencia.container).removeClass('hide');
		}
	},

	callBackDadosDominio: function (response) {

		if (!response.EhValido) {
			Mensagem.gerar(MasterPage.getContent(BaseReferencia.container), response.Msg);
			return;
		}

		$('.gridDadosDominio tbody tr', BaseReferencia.container).remove();

		if (response.Lista.length == 0) {
			$('.divDadosDominio', BaseReferencia.container).addClass('hide');
			return;
		}

		$('.divDadosDominio', BaseReferencia.container).removeClass('hide');

		$(response.Lista).each(function (i, item) {

			var template = $('.templateDadosDominio', BaseReferencia.container).clone();
			template.removeClass('templateDadosDominio');

			$('.txtNome', template).text(item.Texto);
			$('.txtNome', template).attr('title', item.Texto);

			$('.gridDadosDominio tbody:last', BaseReferencia.container).append(template);
		});
	},

	onBaixarDadosDominio: function () {

		var container = $(this).closest('tr');

		var id = $('.hdnArquivoId', container).val();
		var tipo = $('.hdnArquivoTipoId', container).val();
		var caracterizacaoTipo = 1/*Dominialidade*/;

		if (ProjetoGeografico.settings.isVisualizarCredenciado) {
			MasterPage.redireciona(ProjetoGeografico.settings.urls.baixarArquivosCredenciado + '?id=' + id);
			return;
		}

		var url;

		$(ProjetoGeografico.settings.urls.arquivos).each(function () {
			if (this.Tipo == tipo) {
				url = this.Url;
				return false;
			}
		});


		Aux.downloadAjax("downloadPrjGeo", url + '?id=' + id + '&tipo=' + caracterizacaoTipo, null, 'post');

	},

	onBaixarArquivoOrtoMosaico: function () {
		var caminho = $(this).closest('tr').find('.hdnCaminho').val();
		var chave = $(this).closest('tr').find('.hdnChave').val();
        //Erro do IFRAME de outro dominio
	    //Aux.downloadAjax("downloadPrjGeo", BaseReferencia.settings.urls.baixarArquivoOrtoMosaico + "?chave=" + chave, null, 'GET');
		MasterPage.redireciona(BaseReferencia.settings.urls.baixarArquivoOrtoMosaico + "?chave=" + chave);
	},

	onGerarArquivoVetorial: function () {

		var tr = $(this).closest('tr');

		ProjetoGeografico.postGet({
			url: BaseReferencia.settings.urls.gerarArquivoVetorial,
			data: {
				ProjetoId: ProjetoGeografico.settings.projetoId,
				IdRelacionamento: $('.hdnArquivoIdRelacionamento', tr).val(),
				Tipo: $('.hdnArquivoTipo', tr).val(),
				FilaTipo: $('.hdnArquivoFilaTipo', tr).val(),
				Mecanismo: $('.radioTiPoMecanismo:radio:checked', ProjetoGeografico.container).val()
			},
			callBack: BaseReferencia.callBackGerarArquivoVetorial,
			container: tr
		});
	},

	onRegerarArquivoVetorial: function () {

		var tr = $(this).closest('tr');

		ProjetoGeografico.postGet({
			url: BaseReferencia.settings.urls.gerarArquivoVetorial,
			data: {
				ProjetoId: ProjetoGeografico.settings.projetoId,
				IdRelacionamento: $('.hdnArquivoIdRelacionamento', tr).val(),
				Tipo: $('.hdnArquivoTipo', tr).val(),
				FilaTipo: $('.hdnArquivoFilaTipo', tr).val(),
				Mecanismo: $('.radioTiPoMecanismo:radio:checked', ProjetoGeografico.container).val()
			},
			callBack: BaseReferencia.callBackGerarArquivoVetorial,
			container: tr
		});
	},

	callBackGerarArquivoVetorial: function (resultado, container) {

		if (!resultado.EhValido) {
			Mensagem.gerar(MasterPage.getContent(BaseReferencia.container), resultado.Msg);
			return;
		}

		$('.hdnArquivoId', container).val(resultado.arquivo.Id);
		$('.hdnArquivoIdRelacionamento', container).val(resultado.arquivo.IdRelacionamento);
		$('.hdnSituacaoId', container).val(resultado.arquivo.Situacao);
		$('.txtSituacao', container).text(resultado.arquivo.SituacaoTexto);
		BaseReferencia.gerenciarBotoesArqVetoriais(container,
				{
					gerar: resultado.arquivo.MostarGerar,
					baixar: resultado.arquivo.MostarBaixar,
					regerar: resultado.arquivo.MostarRegerar
				});
		BaseReferencia.obterSituacao();
	},

	gerenciarBotoesArqVetoriais: function (linha, botoes) {

		var configuracao = { gerar: false, baixar: false, regerar: false };
		$.extend(configuracao, botoes);
		$('.btnGerar', linha).toggleClass('hide', !configuracao.gerar);
		$('.btnDownload', linha).toggleClass('hide', !configuracao.baixar);
		$('.btnRegerar', linha).toggleClass('hide', !configuracao.regerar);
	},

	onBaixarArquivoVetorial: function () {
		var id = $(this).closest('tr').find('.hdnArquivoId').val();
		Aux.downloadAjax("downloadPrjGeo", BaseReferencia.settings.urls.baixarArquivoVetorial + "/" + id, null, 'GET');
	},

	onBaixarArquivoModelo: function () {
	    Aux.downloadAjax("downloadPrjGeo", BaseReferencia.settings.urls.baixarArquivoModelo, null, 'GET');
	}
}

EnviarProjeto = {
	settings: {
		urls: {
			EnviarProjeto: null,
			cancelarProcessamento: null,
			reenviarArquivo: null,
			obterSituacao: null
		},
		threadAtualizarSituacao: null,
		threadAtualizarSituacaoDelay: 1000 * 10,
		situacoesValidas: []
	},
	mensagens: null,
	container: null,

	load: function (container, options) {
		if (options) { $.extend(EnviarProjeto.settings, options); }

		EnviarProjeto.container = container;

		$(container).delegate('.btnEnviarProjetoGeo', 'click', EnviarProjeto.onEnviaArquivo);
		$(container).delegate('.btnCancelarProcessamento', 'click', EnviarProjeto.onCancelarProcessamento);
		$(container).delegate('.btnReenviarArquivo', 'click', EnviarProjeto.onReenviarArquivo);
		$(container).delegate('.btnBaixar', 'click', EnviarProjeto.baixarArquivoProcessado);
	},

	onCancelarProcessamento: function () {
		var data = {
			ProjetoId: ProjetoGeografico.settings.projetoId,
			Tipo: $('.hdnArquivoEnviadoTipo').val(),
			FilaTipo: $('.hdnArquivoEnviadoFilaTipo').val(),
			Mecanismo: $('.radioTiPoMecanismo:radio:checked', ProjetoGeografico.container).val()
		};

		ProjetoGeografico.postGet({
			url: EnviarProjeto.settings.urls.cancelarProcessamento,
			data: data,
			callBack: EnviarProjeto.callBackCancelarProcessamento,
			async: false,
			mostrarCarregando: true
		});
	},

	callBackCancelarProcessamento: function (response) {

		if (!response.EhValido) {
			Mensagem.gerar(MasterPage.getContent(EnviarProjeto.container), response.Msg);
			return;
		}

		$('.hdnArquivoEnviadoId', EnviarProjeto.container).val(0);
		$('.lblSituacaoProcessamento', EnviarProjeto.container).text('');
		$('.hdnArquivoEnviadoSituacaoId', EnviarProjeto.container).val(0);

		EnviarProjeto.setarBotoes({ reenviar: response.Arquivo.MostrarReenviar, cancelar: response.Arquivo.MostrarCancelar });

		$('.hdnArquivo', EnviarProjeto.container).val('');
		$('.divSituacaoProjeto', EnviarProjeto.container).addClass("hide");
		$('.divEnviarProjeto', EnviarProjeto.container).removeClass("hide");

		EnviarProjeto.callBackReenviarArquivo();

		MasterPage.carregando(false);
	},

	onReenviarArquivo: function () {
		Modal.confirma({
			btnOkCallback: EnviarProjeto.callBackReenviarArquivo,
			titulo: "Reenviar Arquivo",
			conteudo: ProjetoGeografico.mensagens.ConfirmacaoReenviar.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	callBackReenviarArquivo: function (modal) {
		$('.divAlertaAPP', ProjetoGeografico.container).addClass('hide');
		$('.divAlertaARL', ProjetoGeografico.container).addClass('hide');

		$('.hdnArquivo', EnviarProjeto.container).val('');
		$('.divSituacaoProjeto', EnviarProjeto.container).addClass("hide");
		$('.divEnviarProjeto', EnviarProjeto.container).removeClass("hide");
		$('.divImportarArquivo', EnviarProjeto.container).addClass('hide');

		Sobreposicao.mostrar(false);
		Sobreposicao.limpar();

		EnviarProjeto.limparArquivosGrid();
		Modal.fechar(modal);
	},

	mostrar: function (flag) {
		$('.divImportador', ProjetoGeografico.container).toggleClass('hide', !flag);
	},

	limparArquivosGrid: function () {
		$(".gridImportarArquivos tbody tr", EnviarProjeto.container).remove();
	},

	onEnviaArquivo: function () {

		var erroMsg = new Array();
		var nome = $('.divEnviarProjeto input[type="file"]', EnviarProjeto.container).val();

		if (nome.toLowerCase().substr(nome.length - 4) !== ".zip") {
			erroMsg.push(EnviarProjeto.mensagens.ArquivoAnexoNaoEhZip);
		}

		if (erroMsg.length > 0) {
			Mensagem.gerar(MasterPage.getContent(EnviarProjeto.container), erroMsg);
			return;
		}

		var inputFile = $('.divEnviarProjeto input[type="file"]', EnviarProjeto.container);
		inputFile.attr("id", "ArquivoId_1");
		FileUpload.upload(EnviarProjeto.settings.urls.EnviarProjeto, inputFile, EnviarProjeto.callBackArquivoEnviado);
	},

	callBackArquivoEnviado: function (controle, retorno) {

		var ret = $.parseJSON(retorno);
		ret.Arquivo.ProjetoId = ProjetoGeografico.settings.projetoId;
		ret.Arquivo.Tipo = $('.hdnArquivoEnviadoTipo').val();
		ret.Arquivo.FilaTipo = $('.hdnArquivoEnviadoFilaTipo').val();
		ret.Arquivo.Mecanismo = $('.radioTiPoMecanismo:radio:checked', ProjetoGeografico.container).val();

		var projeto = { projeto: { Arquivos: new Array(ret.Arquivo)} };

		if (ret.Arquivo != null && typeof ret.Arquivo != "undefined") {

			$('.hdnArquivo', EnviarProjeto.container).val(JSON.stringify(ret.Arquivo));
			$('.divEnviarProjeto', EnviarProjeto.container).addClass("hide");
			$('.divSituacaoProjeto', EnviarProjeto.container).removeClass("hide");

			ProjetoGeografico.postGet({
				url: EnviarProjeto.settings.urls.EnviarParaProcessar,
				data: projeto,
				callBack: EnviarProjeto.callBackArquivoProcessamento, async: false
			});

		} else {
			$('.divEnviarProjeto .hdnArquivo', EnviarProjeto.container).val('');
			$('.divEnviarPeojeto input[type=file]').val('');
			if (ret.Msg.length > 0) {
				Mensagem.gerar(ProjetoGeografico.container, ret.Msg);
			}
		}
	},

	callBackArquivoProcessamento: function (response) {

		if (!response.EhValido) {
			Mensagem.gerar(MasterPage.getContent(EnviarProjeto.container), response.Msg);
			return;
		}

		$('.divImportarArquivo', EnviarProjeto.container).removeClass("hide");
		$('.hdnArquivoEnviadoId', EnviarProjeto.container).val(response.Arquivo.IdRelacionamento);
		$('.lblSituacaoProcessamento', EnviarProjeto.container).text(response.Arquivo.SituacaoTexto);
		$('.hdnArquivoEnviadoSituacaoId', EnviarProjeto.container).val(response.Arquivo.Situacao);

		EnviarProjeto.setarBotoes({ reenviar: response.Arquivo.MostrarReenviar, cancelar: response.Arquivo.MostrarCancelar });
		EnviarProjeto.obterSituacao();
	},

	obterSituacao: function () {
		var listaItens = new Array();

		if (ProjetoGeografico.settings.isVisualizar) {
			return;
		}

		var situacaoId = +$('.hdnArquivoEnviadoSituacaoId', EnviarProjeto.container).val();

		if ($.inArray(situacaoId, EnviarProjeto.settings.situacoesValidas) != -1) {
			listaItens.push({ IdRelacionamento: $('.hdnArquivoEnviadoId', EnviarProjeto.container).val() });
		}

		if (listaItens.length > 0) {
			ProjetoGeografico.postGet({
				url: EnviarProjeto.settings.urls.obterSituacao,
				data: { arquivos: listaItens,
					projetoId: ProjetoGeografico.settings.projetoId,
					arquivoEnviadoTipo: $('.hdnArquivoEnviadoTipo', ProjetoGeografico.container).val(),
				    arquivoEnviadoFilaTipo: $('.hdnArquivoEnviadoFilaTipo', ProjetoGeografico.container).val()
				},
				callBack: EnviarProjeto.callBackObterSituacao
			});

			if (EnviarProjeto.settings.threadAtualizarSituacao == null) {
				EnviarProjeto.settings.threadAtualizarSituacao = setInterval(EnviarProjeto.obterSituacao, EnviarProjeto.settings.threadAtualizarSituacaoDelay);
			}
		} else {
			EnviarProjeto.stopThread();
		}
	},

	callBackObterSituacao: function (response) {

		if (!response.EhValido) {
			Mensagem.gerar(MasterPage.getContent(BaseReferencia.container), response.Msg);
		}

		if (EnviarProjeto.settings.threadAtualizarSituacao == null) {
			return;
		}

		$(response.lista).each(function (i, item) {
			if ($('.hdnArquivoEnviadoId', EnviarProjeto.container).val() == item.IdRelacionamento) {
				$('.lblSituacaoProcessamento', EnviarProjeto.container).text(item.SituacaoTexto);
				$('.hdnArquivoEnviadoSituacaoId', EnviarProjeto.container).val(item.Situacao);
				EnviarProjeto.setarBotoes({ reenviar: item.MostrarReenviar, cancelar: item.MostrarCancelar });
			}
		});

		$('.gridImportarArquivos tbody tr', EnviarProjeto.container).remove();

		$(response.arquivosProcessados).each(function (i, item) {

			if (item.Texto == "Relatório de importação" && $('.hdnArquivoEnviadoSituacaoId', EnviarProjeto.container).val() <= 2) {
				return;
			}
			var linha = $('.trTemplateArqProcessado', EnviarProjeto.container).clone();

			linha.removeClass('trTemplateArqProcessado hide');

			$('.arquivoNome', linha).text(item.Texto);
			$('.hdnArquivoProcessadoId', linha).val(item.Id);
			$('.hdnArquivoProcessadoTipoId', linha).val(item.Tipo);

			$('.gridImportarArquivos tbody:last').append(linha);
		});

		if (response.arquivosProcessados.length > 0) {
			$('.divImportarArquivo', EnviarProjeto.container).removeClass('hide');
		} else {
			$('.divImportarArquivo', EnviarProjeto.container).addClass('hide');
		}

		var situacaoId = +$('.hdnArquivoEnviadoSituacaoId', EnviarProjeto.container).val();
		if (situacaoId == 14) {//Processado
			$('.divExistemProcessamentosAndamento', ProjetoGeografico.container).addClass('hide');
			$('.divExistemProcessamentosAndamento', ProjetoGeografico.container).hide();

			Sobreposicao.mostrar(true);
			Sobreposicao.limpar();

			ProjetoGeografico.postGet({
				url: ProjetoGeografico.settings.urls.verificarAreaNaoCaracterizada,
				data: {
					empreendimento: $('.hdnEmpreendimentoId', ProjetoGeografico.container).val()
				},
				callBack: function (retorno) {
					ProjetoGeografico.settings.possuiAPPNaoCaracterizada = retorno.PossuiAPPNaoCaracterizada;
					ProjetoGeografico.settings.possuiARLNaoCaracterizada = retorno.PossuiARLNaoCaracterizada;
					ProjetoGeografico.mensagemAreaNaoCaracterizada();
				}
			});
		}
	},

	stopThread: function () {
		clearInterval(EnviarProjeto.settings.threadAtualizarSituacao);
		EnviarProjeto.settings.threadAtualizarSituacao = null;
	},

	obterArquivo: function () {
		var arquivo = $.parseJSON($('.hdnArquivo', EnviarProjeto.container).val());
		return arquivo;
	},

	adicionarArquivoDownload: function (nome, url, tipo) {

		var novaLinha = $('.importarArquivosGridTemplate .trTemplate', EnviarProjeto.container).clone();

		novaLinha.removeClass('trTemplate').removeClass("hide");
		$('button', novaLinha).addClass(tipo == 0 ? "download" : "pdf");
		$('.arquivoNomeTd', novaLinha).text(nome);
		$(novaLinha).delegate('button', 'click', function () { return EnviarProjeto.baixarArquivoProcessado(); });

		$('.gridImportarArquivos tbody', EnviarProjeto.container).append(novaLinha);

		Listar.atualizarEstiloTable($('.gridImportarArquivos', EnviarProjeto.container));
	},

	baixarArquivoProcessado: function () {

		var container = $(this).closest('tr');

		var id = $('.hdnArquivoProcessadoId', container).val();
		var tipo = $('.hdnArquivoProcessadoTipoId', container).val();
		var caracterizacaoTipo = $('.hdnCaracterizacaoTipo', ProjetoGeografico.container).val();

		if (ProjetoGeografico.settings.isVisualizarCredenciado) {
			MasterPage.redireciona(ProjetoGeografico.settings.urls.baixarArquivosCredenciado + '?id=' + id);
			return;
		}

		var url;

		$(ProjetoGeografico.settings.urls.arquivos).each(function () {
			if (this.Tipo == tipo) {
				url = this.Url;
				return false;
			}
		});

		Aux.downloadAjax("downloadPrjGeo", url + '?id=' + id + '&tipo=' + caracterizacaoTipo, null, 'post');

	},

	setarBotoes: function (botoes) {

		var configuracao = { reenviar: false, cancelar: false };

		$.extend(configuracao, botoes);

		$('.btnReenviarArquivo', EnviarProjeto.container).toggleClass('hide', !configuracao.reenviar);
		$('.btnCancelarProcessamento', EnviarProjeto.container).toggleClass('hide', !configuracao.cancelar);
	}
}

Desenhador = {
	settings: {
		urls: {
			desenhador: null,
			cancelarProcessamento: null,
			processarArquivoDesenhador: null,
			reprocessar: null,
			obterSituacao: null,
			cancelarProcessamento: null,
			confirmarReenviarArquivo: null
		},
		threadAtualizarSituacao: null,
		threadAtualizarSituacaoDelay: 1000 * 1,
		situacoesValidas: []
	},
	container: null,

	load: function (container, options) {
		Desenhador.container = container;
		if (options) { $.extend(Desenhador.settings, options); }

		$(Desenhador.container).delegate('.btnDesenhador', 'click', Desenhador.onAbrirDesenhador);
		$(Desenhador.container).delegate('.btnReprocessarGeo', 'click', Desenhador.onReprocessarArquivo);
		$(Desenhador.container).delegate('.btnCancelarProcessamentoGeo', 'click', Desenhador.onCancelarProcessamento);

		$(Desenhador.container).delegate('.btnBaixar', 'click', function (e) {
			var id = $(e.currentTarget).closest('tr').find('.hdnArquivoProcessadoId').val();
			var tipo = $(e.currentTarget).closest('tr').find('.hdnArquivoProcessadoTipoId').val();
			Desenhador.onBaixarArquivo(id, tipo);
		});
	},

	onReprocessarArquivo: function () {
		Modal.confirma({
			btnOkCallback: Desenhador.callBackReprocessarArquivo,
			titulo: "Reprocessar Arquivo",
			conteudo: ProjetoGeografico.mensagens.ConfirmacaoReenviar.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	callBackReprocessarArquivo: function (modal) {
		$('.hdnArquivo', EnviarProjeto.container).val('');
		$('.divSituacaoProjeto', EnviarProjeto.container).addClass("hide");
		$('.divEnviarProjeto', EnviarProjeto.container).removeClass("hide");
		$('.divImportarArquivo', EnviarProjeto.container).addClass('hide');
		Desenhador.limparArquivosProcessados();
		Desenhador.onProcessar();

		Sobreposicao.mostrar(false);
		Sobreposicao.limpar();

		Modal.fechar(modal);
	},

	mostrar: function (flag) {
		$('.divDesenhador', ProjetoGeografico.container).toggleClass('hide', !flag);
	},

	mostrarArquivo: function (flag) {
		$('.divDesenhadorArquivo', Desenhador.container).toggleClass('hide', !flag);
	},

	mostrarSituacao: function (flag) {
		$('.divSituacao', Desenhador.container).addClass('hide', !flag);
	},

	onAbrirDesenhador: function () {
		var desenhadorModo = ((ProjetoGeografico.settings.isVisualizar || ProjetoGeografico.settings.isFinalizado) ? 2 : 1);

		Modal.abrir(Desenhador.settings.urls.desenhador + '?modo=' + desenhadorModo, null,
		function (container) {
			Navegador.load(container, {
				id: ProjetoGeografico.settings.projetoId,
				tipo: ProjetoGeografico.settings.ArquivoEnviadoFilaTipo,
				modo: desenhadorModo,
				onCancelar: Desenhador.onCancelarProcessamento,
				onProcessar: Desenhador.onProcessar,
				onBaixarArquivo: Desenhador.onBaixarArquivo,
				obterSituacaoInicial: Desenhador.obterSituacaoInicial,
				obterAreaAbrangencia: Desenhador.obterAreaAbrangencia,
				width: $(window).width() - 100,
				height: $(window).height() - 100
		    });

		}, Modal.tamanhoModalFull);
	},

	obterAreaAbrangencia: function () {
		return $.toJSON(ProjetoGeografico.montarObjeto());
	},

	obterSituacaoInicial: function () {

		var objeto = {
			SituacaoId: $('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val(),
			SituacaoTexto: $('.lblSituacaoProcessamento', Desenhador.container).text(),
			ArquivosProcessados: new Array()
		};

		$('.desenhadorArquivosGrid tbody tr', Desenhador.container).each(function (i, item) {
			objeto.ArquivosProcessados.push({
				Id: $('.hdnArquivoProcessadoId', item).val(),
				Tipo: $('.hdnArquivoProcessadoTipoId', item).val(),
				Texto: $('.arquivoNome', item).text(),
				IsPDF: $('.hdnArquivoProcessadoIsPdf', item).val()
			});
		});

		return $.toJSON(objeto);
	},

	limparArquivosProcessados: function () {
		$('.divSituacao', Desenhador.container).addClass('hide');
		$('.divDesenhadorArquivo', Desenhador.container).addClass('hide');
		$('.desenhadorArquivoGrid tbody tr', Desenhador.container).remove();
	},

	onCancelarProcessamento: function () {
		var data = {
			ProjetoId: ProjetoGeografico.settings.projetoId,
			Tipo: $('.hdnArquivoEnviadoTipo').val(),
			FilaTipo: $('.hdnArquivoEnviadoFilaTipo').val(),
			Mecanismo: $('.radioTiPoMecanismo:radio:checked', ProjetoGeografico.container).val()
		};

		ProjetoGeografico.postGet({
			url: EnviarProjeto.settings.urls.cancelarProcessamento,
			data: data,
			callBack: Desenhador.callBackCancelarProcessamento,
			async: false,
			mostrarCarregando: true
		});
	},

	callBackCancelarProcessamento: function (response) {
	},

	exibirReenviar: function () {

	    $('.hdnArquivo', EnviarProjeto.container).val('');
	    $('.divSituacaoProjeto', EnviarProjeto.container).addClass("hide");
	    $('.divEnviarProjeto', EnviarProjeto.container).removeClass("hide");
	    $('.divImportarArquivo', EnviarProjeto.container).addClass('hide');

	    var situacaoId = +$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val();
	    if (situacaoId == 14) {
	        Desenhador.setarBotoes({ reprocessar: true });
	    }
	},

	onBaixarArquivo: function (id, tipo) {
		var caracterizacaoTipo = $('.hdnCaracterizacaoTipo', ProjetoGeografico.container).val();

		if (ProjetoGeografico.settings.isVisualizarCredenciado) {
			MasterPage.redireciona(ProjetoGeografico.settings.urls.baixarArquivosCredenciado + '?id=' + id);
			return;
		}

		var url;

		$(ProjetoGeografico.settings.urls.arquivos).each(function () {
			if (this.Tipo == tipo) {
				url = this.Url;
				return false;
			}
		});

		Aux.downloadAjax("downloadPrjGeo", url + '?id=' + id + '&tipo=' + caracterizacaoTipo, null, 'post');

	},

	onProcessar: function () {
		var arquivo = {};
		arquivo.ProjetoId = ProjetoGeografico.settings.projetoId;
		arquivo.Tipo = $('.hdnArquivoEnviadoTipo', ProjetoGeografico.container).val();
		arquivo.FilaTipo = $('.hdnArquivoEnviadoFilaTipo', ProjetoGeografico.container).val();
		arquivo.IdRelacionamento = $('.hdnArquivoEnviadoId', Desenhador.container).val();
		arquivo.Mecanismo = $('.radioTiPoMecanismo:radio:checked', ProjetoGeografico.container).val();

		var params = { projeto: { Arquivos: new Array(arquivo) } };

		ProjetoGeografico.postGet({
			url: Desenhador.settings.urls.processarArquivoDesenhador,
			data: params,
			callBack: Desenhador.callBackProcessar
		});
	},

	callBackProcessar: function (response) {

		if (!response.EhValido) {
			Mensagem.gerar(MasterPage.getContent(Desenhador.container), response.Msg);
			return;
		}

		$('.divAlertaAPP', ProjetoGeografico.container).addClass('hide');
		$('.divAlertaARL', ProjetoGeografico.container).addClass('hide');
		$('.divImportarArquivo', Desenhador.container).removeClass("hide");
		$('.hdnArquivoEnviadoId', Desenhador.container).val(response.Arquivo.IdRelacionamento);
		$('.lblSituacaoProcessamento', Desenhador.container).text(response.Arquivo.SituacaoTexto);
		$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val(response.Arquivo.Situacao);

		Desenhador.setarBotoes({ reprocessar: response.Arquivo.MostrarReprocessar, cancelar: response.Arquivo.MostrarCancelar });
		Desenhador.obterSituacao();
	},

	obterSituacao: function (forcar) {
		if (ProjetoGeografico.settings.isVisualizar) {
			return;
		}

		var arra = new Array({ Id: 1, Texto: 'Texto' });
		var valor = $.toJSON({ SituacaoTexto: 'SituacaoTexto', SituacaoId: 1, ArquivosProcessados: arra });

		var situacaoId = +$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val();

		if ($.inArray(situacaoId, Desenhador.settings.situacoesValidas) != -1 || forcar) {
			ProjetoGeografico.postGet({
				url: Desenhador.settings.urls.obterSituacao,
				data: {
					projetoId: ProjetoGeografico.settings.projetoId,
					arquivoId: $('.hdnArquivoEnviadoId', Desenhador.container).val(),
					arquivoEnviadoTipo: $('.hdnArquivoEnviadoTipo', ProjetoGeografico.container).val(),
					arquivoEnviadoFilaTipo: $('.hdnArquivoEnviadoFilaTipo', ProjetoGeografico.container).val()
				},

				callBack: Desenhador.callBackObterSituacao
			});

			if (Desenhador.settings.threadAtualizarSituacao == null) {
				Desenhador.settings.threadAtualizarSituacao = setInterval(Desenhador.obterSituacao, Desenhador.settings.threadAtualizarSituacaoDelay);
			}
		} else {
			Desenhador.stopThread();
		}
	},

	callBackObterSituacao: function (response) {

		if (!response.EhValido) {
			Mensagem.gerar(MasterPage.getContent(BaseReferencia.container), response.Msg);
		}

		if (Desenhador.settings.threadAtualizarSituacao == null) {
			return;
		}

		$('.divSituacao', Desenhador.container).removeClass('hide');

		$('.lblSituacaoProcessamento', Desenhador.container).text(response.Arquivo.SituacaoTexto);
		$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val(response.Arquivo.Situacao);

		$('.desenhadorArquivosGrid tbody tr', Desenhador.container).remove();

		$(response.arquivosProcessados).each(function (i, item) {

			var linha = $('.trTemplateArqProcessado', Desenhador.container).clone();

			linha.removeClass('trTemplateArqProcessado');

			$('.arquivoNome', linha).text(item.Texto);
			$('.hdnArquivoProcessadoId', linha).val(item.Id);
			$('.hdnArquivoProcessadoTipoId', linha).val(item.Tipo);

			$('.desenhadorArquivosGrid tbody:last').append(linha);
		});

		if (response.arquivosProcessados.length > 0) {
			$('.divDesenhadorArquivo', Desenhador.container).removeClass('hide');
		} else {
			$('.divDesenhadorArquivo', Desenhador.container).addClass('hide');
		}

		Desenhador.setarBotoes({ reprocessar: response.Arquivo.MostrarReprocessar, cancelar: response.Arquivo.MostrarCancelar });

		var situacaoId = +$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val();
		Sobreposicao.setarSituacao(situacaoId);

		if (situacaoId == 14) {//Processado
			$('.divExistemProcessamentosAndamento', ProjetoGeografico.container).addClass('hide');
			$('.divExistemProcessamentosAndamento', ProjetoGeografico.container).hide();

			ProjetoGeografico.postGet({
				url: ProjetoGeografico.settings.urls.verificarAreaNaoCaracterizada,
				data: {
					empreendimento: $('.hdnEmpreendimentoId', ProjetoGeografico.container).val()
				},
				callBack: function (retorno) {
					ProjetoGeografico.settings.possuiAPPNaoCaracterizada = retorno.PossuiAPPNaoCaracterizada;
					ProjetoGeografico.settings.possuiARLNaoCaracterizada = retorno.PossuiARLNaoCaracterizada;
					ProjetoGeografico.mensagemAreaNaoCaracterizada();
				}
			});
		}

		var parametros = $.toJSON({
			SituacaoTexto: response.Arquivo.SituacaoTexto,
			SituacaoId: response.Arquivo.Situacao,
			ArquivosProcessados: response.arquivosProcessados
		});

		if (Navegador.settings.setSituacaoProcessamento) {
			Navegador.settings.setSituacaoProcessamento(parametros);
		}
	},

	stopThread: function () {
		clearInterval(Desenhador.settings.threadAtualizarSituacao);
		Desenhador.settings.threadAtualizarSituacao = null;
	},

	setarBotoes: function (botoes) {

		var configuracao = { reprocessar: false, cancelar: false };

		$.extend(configuracao, botoes);

		$('.btnReprocessarGeo', Desenhador.container).toggleClass('hide', !configuracao.reprocessar);

		$('.btnCancelarProcessamentoGeo', Desenhador.container).toggleClass('hide', !configuracao.cancelar);
	}
}

Sobreposicao = {
	settings: {
		urls: {
			verificarSobreposicao: null
		},
		sobreposicoesObjJon: null
	},
	container: null,

	load: function (container, options) {

		Sobreposicao.container = container;

		if (options) { $.extend(Sobreposicao.settings, options); }

		$('.btnVerificarSobreposicao', Sobreposicao.container).data('Sobreposicoes', Sobreposicao.settings.sobreposicoesObjJon);

		$(Sobreposicao.container).delegate('.btnVerificarSobreposicao', 'click', Sobreposicao.onVerificarSobreposicao);

	},

	onVerificarSobreposicao: function () {

		var params = { projetoId: ProjetoGeografico.settings.projetoId, tipo: $('.hdnCaracterizacaoTipo', ProjetoGeografico.container).val() };

		if ($(".btnVerificarSobreposicao", Sobreposicao.container).button("option", "disabled")) {
			return;
		}

		$('.btnVerificarSobreposicao', Sobreposicao.container).button({ disabled: true });
		$('.dataGrid', Sobreposicao.container).addClass('hide');
		$('.dataHoraVer', Sobreposicao.container).addClass('hide');
		$('.verificando', Sobreposicao.container).removeClass('hide');

		ProjetoGeografico.postGet({
			url: Sobreposicao.settings.urls.verificarSobreposicao,
			data: params,
			async: true,
			callBack: function (response) {

				$('.btnVerificarSobreposicao').button({ disabled: false });

				if (!response.EhValido) {
					Mensagem.gerar(ProjetoGeografico.container, response.Msg);
					return;
				}

				$('.sobreposicaoIdafGrid tbody').empty();
				$('.sobreposicaoGeoBasesGrid tbody').empty();

				$('.dataGrid', Sobreposicao.container).removeClass('hide');
				$('.dataHoraVer', Sobreposicao.container).removeClass('hide');
				$('.verificando', Sobreposicao.container).addClass('hide');

				$('.lblDataVerificacao', Sobreposicao.container).text(response.Sobreposicoes.DataVerificacao);

				$('.btnVerificarSobreposicao', Sobreposicao.container).data('Sobreposicoes', response.Sobreposicoes);

				$(response.Sobreposicoes.Itens).each(function (i, item) {

					var linha = $('.trTemplateSobreposicao', Sobreposicao.container).clone();

					linha.removeClass('trTemplateSobreposicao');
					
					$('.tipoTexto', linha).text(item.TipoTexto);
					$('.identificacao', linha).text(item.Identificacao);

					$(item.Base == 1 ? '.sobreposicaoIdafGrid tbody:last' : '.sobreposicaoGeoBasesGrid tbody:last').append(linha);

					Listar.atualizarEstiloTable();
				});
			}
		});

	},

	mostrar: function (flag) {
		$('.divSobreposicao', ProjetoGeografico.container).toggleClass('hide', !flag);
	},

	setarSituacao: function (situacaoId) {

		var situacaoIdLocal = +$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val() | situacaoId;

		if (situacaoIdLocal == 14) {//Processado
			Sobreposicao.mostrar(true);
			Sobreposicao.limpar();
		} else {
			Sobreposicao.mostrar(false);
		}
	},

	limpar: function () {
		$('.dataGrid', Sobreposicao.container).addClass('hide');
		$('.dataHoraVer', Sobreposicao.container).removeClass('hide');
		$('.verificando', Sobreposicao.container).addClass('hide');
		$('.divVerificarSobreposicao', Sobreposicao.container).removeClass('hide');
		$('.lblDataVerificacao', Sobreposicao.container).text('');
		$('.btnVerificarSobreposicao', Sobreposicao.container).data('Sobreposicoes', null);
		$('.sobreposicaoIdafGrid tbody').empty();
		$('.sobreposicaoGeoBasesGrid tbody').empty();
	}
}
/// <reference path="../../Lib/jquery.json-2.2.min.js" />
/// <reference path="../../Lib/JQuery/jquery.json - 2.2.min.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../Lib/JQuery/jquery-1.10.1-vsdoc.js" />

ProjetoGeografico = {
	settings: {
		projetoId: null,
		dependencias: null,
		isCadastrarCaracterizacao: true,
		isDominialidade: false,
		possuiAPPNaoCaracterizada: false,
		possuiARLNaoCaracterizada: false,
		isVisualizar: false,
		isFinalizado: false,
		isProcessado: false,
		urls: {
			verificarAreaNaoCaracterizada: null,
			coordenadaGeo: null,
			criarParcial: null,
			confirmarAlterarArea: null,
			alterarAreaAbrangencia: null,
			cancelarProcessamentoArquivosVet: null,
			refazer: null,
			recarregar: null,
			finalizar: null,
			salvar: null,
			avancar: null,
			excluir: null,
			processar: null,
			cancelarProcessamento: null,
			obterSituacao: null

		},
		idsTelaSitacaoProcessamento: null,
		idsTelaArquivoTipo: null,
		idsTelaEtapaProcessamento: null,
		idsTelaMecanismo: null,
		atualizarDependenciasModalTitulo: '',
		textoMerge: null,
		isObteveMerge: false,
		EmpreendimentoEstaDentroAreaAbrangencia: true,
		threadAtualizarSituacao: null,
		threadAtualizarSituacaoDelay: 1000 * 2,
		obterSituacaoSettings: {
			container: null,
			gerenciarBotoes: null,
			mecanismo: null,
		},
		situacoesValidas: null
	},
	mensagens: null,
	container: null,

	load: function (container, options) {
		if (options) { $.extend(ProjetoGeografico.settings, options); }
		ProjetoGeografico.container = MasterPage.getContent(container);

		$(ProjetoGeografico.container).delegate('.radioTiPoMecanismo', 'change', ProjetoGeografico.onChangeTipoElaboracao);
		$(ProjetoGeografico.container).delegate('.radioTiPoMecanismo', 'click', ProjetoGeografico.onclickTipoElaboracao);
		$(ProjetoGeografico.container).delegate('.btnSelecionarCoordenada', 'click', ProjetoGeografico.onAbrirNavegadorCoordenada);
		$(ProjetoGeografico.container).delegate('.btnAlterarCoordenada', 'click', ProjetoGeografico.onAlterarCoordenada);
		$(ProjetoGeografico.container).delegate('.btnObterCoordenadaAuto', 'click', ProjetoGeografico.onGerarCoordenadaAutomatico);

		$(ProjetoGeografico.container).delegate('.btnRefazer', 'click', ProjetoGeografico.onRefazer);
		$(ProjetoGeografico.container).delegate('.btnRecarregar', 'click', ProjetoGeografico.onRecarregar);
		$(ProjetoGeografico.container).delegate('.btnExluir', 'click', ProjetoGeografico.onExcluir);
		$(ProjetoGeografico.container).delegate('.btnFinalizar', 'click', ProjetoGeografico.onFinalizar);
		$(ProjetoGeografico.container).delegate('.btnSalvar ', 'click', ProjetoGeografico.onSalvar);
		$(ProjetoGeografico.container).delegate('.btnAvancar ', 'click', ProjetoGeografico.onAvancar);

		BaseReferencia.load($('.containerBaseReferencia', ProjetoGeografico.container), options);
		ImportadorShape.load($('.containerImportadorShape', ProjetoGeografico.container), options);
		Desenhador.load($('.containerDesenhador', ProjetoGeografico.container), options);
		Sobreposicao.load($('.containerSobreposicao', ProjetoGeografico.container), options);

		if (ProjetoGeografico.settings.projetoId != 0) {
			var mecanismo = +$('.radioTiPoMecanismo:checked', ProjetoGeografico.container).val();

			if (mecanismo == ProjetoGeografico.settings.idsTelaMecanismo.Desenhador) {
				Desenhador.obterSituacao();
			}

			if (mecanismo == ProjetoGeografico.settings.idsTelaMecanismo.ImportadorShape) {
				BaseReferencia.obterSituacao();
				ImportadorShape.obterSituacao();
			}
		}

		if (ProjetoGeografico.settings.textoMerge) {
			var finalizado = !$('.btnFinalizar', ProjetoGeografico.container).is(':visible');
			ProjetoGeografico.abrirModalRedireciona(ProjetoGeografico.settings.textoMerge, finalizado);

			if (!finalizado) {
				ProjetoGeografico.setarBotoes({ recarregar: true, excluir: true, finalizar: true, salvar: true });
			}
		}

		if (!ProjetoGeografico.settings.EmpreendimentoEstaDentroAreaAbrangencia) {
			Mensagem.gerar(ProjetoGeografico.container, [ProjetoGeografico.mensagens.EmpreendimentoForaAbrangencia]);
		}

		ProjetoGeografico.mensagemAreaNaoCaracterizada();
	},

	mensagemAreaNaoCaracterizada: function () {

		if (ProjetoGeografico.settings.isDominialidade && ProjetoGeografico.settings.isProcessado) {
			if (ProjetoGeografico.settings.possuiAPPNaoCaracterizada) {
				$('.divAlertaAPP', ProjetoGeografico.container).removeClass('hide');
			}

			if (ProjetoGeografico.settings.possuiARLNaoCaracterizada) {
				$('.divAlertaARL', ProjetoGeografico.container).removeClass('hide');
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
							ImportadorShape.onCancelarProcessamento();
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
		var data = { projeto: ProjetoGeografico.obter() };
		var settings = { url: ProjetoGeografico.settings.urls.salvar, data: data, callBack: ProjetoGeografico.callBackSalvar };

		$.extend(settings, option);

		if (ProjetoGeografico.validar(data.projeto)) {
			MasterPage.carregando(true);

			$.ajax({
				url: settings.url,
				data: JSON.stringify(settings.data),
				cache: false,
				async: false,
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					MasterPage.carregando(false);
				},
				success: function (response, textStatus, XMLHttpRequest) {
					settings.callBack(response);
				}
			});

			MasterPage.carregando(false);
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
			ProjetoGeografico.setarBotoes({ recarregar: true, excluir: true, salvar: true, finalizar: true });
		} else {
			ProjetoGeografico.setarBotoes({ salvar: true, finalizar: true });
		}

		ContainerAcoes.load(ProjetoGeografico.container, { limparContainer: false, botoes: new Array({ label: 'Finalizar', callBack: ProjetoGeografico.onFinalizar }) });
	},

	validar: function (obj) {
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

	obter: function () {
		var obj = {};		
		obj.Id = $('.hdnProjetoId', ProjetoGeografico.container).val();
		obj.ProjetoDigitalId = $('.hdnProjetoDigitalId', ProjetoGeografico.container).val();
		obj.EmpreendimentoEasting = $('.hdnEmpEasting', ProjetoGeografico.container).val();
		obj.EmpreendimentoNorthing = $('.hdnEmpNorthing', ProjetoGeografico.container).val();
		obj.EmpreendimentoId = $('.hdnEmpreendimentoId', ProjetoGeografico.container).val();
		obj.MecanismoElaboracaoId = $('.radioTiPoMecanismo:radio:checked', ProjetoGeografico.container).val();
		obj.CaracterizacaoId = $('.hdnCaracterizacaoTipo', ProjetoGeografico.container).val();
		obj.NivelPrecisaoId = $('.ddlNivel', ProjetoGeografico.container).val();
		obj.SituacaoId = $('.hdnProjetoSituacaoId', ProjetoGeografico.container).val();
		obj.SituacaoTexto = $('.SituacaoProjetoTexto', ProjetoGeografico.container).text();
		//obj.Arquivo = $('.hdnArquivo', ImportadorShape.container).val() != "" ? JSON.parse($('.hdnArquivo', ImportadorShape.container)) : null;
		obj.MenorX = $('.txtMenorX', ProjetoGeografico.container).val();
		obj.MenorY = $('.txtMenorY', ProjetoGeografico.container).val();
		obj.MaiorX = $('.txtMaiorX', ProjetoGeografico.container).val();
		obj.MaiorY = $('.txtMaiorY', ProjetoGeografico.container).val();
		obj.InternoId = $('.hdnInternoId', ProjetoGeografico.container).val();
		//obj.Dependencias = JSON.parse(ProjetoGeografico.settings.dependencias);

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

	onAbrirNavegadorCoordenada: function (options) {

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
		$('.fsImportadorShape', ProjetoGeografico.container).addClass('hide');

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
		var dados = { projeto: ProjetoGeografico.obter(), url: ProjetoGeografico.settings.urls.avancar };

		MasterPage.carregando(true);
		$.ajax({
			url: ProjetoGeografico.settings.urls.finalizar,
			data: JSON.stringify(dados),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
				MasterPage.carregando(false);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				ProjetoGeografico.callBackPost(response, container);
			}
		});
		MasterPage.carregando(false);

	},

	onExcluir: function () {
		Modal.confirma({
			btnOkCallback: function (container) {

				container = MasterPage.getContent(container);

				var dados = { projeto: ProjetoGeografico.obter(), isCadastrarCaracterizacao: ProjetoGeografico.isCadastrarCaracterizacao };

				MasterPage.carregando(true);
				$.ajax({
					url: ProjetoGeografico.settings.urls.excluir,
					data: JSON.stringify(dados),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
						MasterPage.carregando(false);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						ProjetoGeografico.callBackPost(response, container);
					}
				});
				MasterPage.carregando(false);
			},
			titulo: "Excluir rascunho projeto geográfico",
			conteudo: ProjetoGeografico.mensagens.ConfirmarExcluir.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	onRefazer: function () {
		Modal.confirma({
			btnOkCallback: function (container) {

				container = MasterPage.getContent(container);

				var dados = { projeto: ProjetoGeografico.obter(), isCadastrarCaracterizacao: ProjetoGeografico.isCadastrarCaracterizacao };

				$.ajax({
					url: ProjetoGeografico.settings.urls.refazer,
					data: JSON.stringify(dados),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
						MasterPage.carregando(false);
					},
					success: function (response, textStatus, XMLHttpRequest) {

						var mecanismo = +$('.radioTiPoMecanismo:checked', ProjetoGeografico.container).val();

						if (mecanismo == ProjetoGeografico.settings.idsTelaMecanismo.ImportadorShape) {
							ProjetoGeografico.cancelarProcesssamento({
								container: ImportadorShape.container,
								gerenciarBotoes: ImportadorShape.gerenciarBotoes,
								mecanismo: mecanismo,
								situacoesValidas: ImportadorShape.settings.situacoesValidas
							});
						}

						if (mecanismo == ProjetoGeografico.settings.idsTelaMecanismo.Desenhador) {
							ProjetoGeografico.cancelarProcesssamento({
                                
							    container: Desenhador.container,
								gerenciarBotoes: Desenhador.gerenciarBotoes,
								mecanismo: mecanismo,
								situacoesValidas: Desenhador.settings.situacoesValidas
							});

						}

						ProjetoGeografico.callBackPost(response, container);
					}
				});

			},
			titulo: "Refazer projeto geográfico",
			conteudo: ProjetoGeografico.mensagens.ConfirmacaoRefazer.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	onRecarregar: function () {

		var mecanismo = +$('.radioTiPoMecanismo:checked', ProjetoGeografico.container).val();

		Modal.confirma({
			btnOkCallback: function (container) {

				container = MasterPage.getContent(container);

				var dados = { projeto: ProjetoGeografico.obter(), isCadastrarCaracterizacao: ProjetoGeografico.isCadastrarCaracterizacao };

				MasterPage.carregando(true);
				$.ajax({
					url: ProjetoGeografico.settings.urls.recarregar,
					data: JSON.stringify(dados),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
						MasterPage.carregando(false);
					},
					success: function (response, textStatus, XMLHttpRequest) {

						ProjetoGeografico.callBackPost(response, container);

						if (mecanismo == ProjetoGeografico.settings.idsTelaMecanismo.Desenhador) {
							Desenhador.obterSituacao();
						}

						if (mecanismo == ProjetoGeografico.settings.idsTelaMecanismo.ImportadorShape) {
							BaseReferencia.obterSituacao();
							ImportadorShape.obterSituacao();
						}

					}
				});
				MasterPage.carregando(false);

			},
			titulo: "Recarregar projeto geográfico",
			conteudo: ProjetoGeografico.mensagens.ConfirmacaoRecarregar.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	validarChange: function () {

		$('.divExistemProcessamentosAndamento', ProjetoGeografico.container).addClass('hide');

		var situacaoId = 0;
		var isImportador = !$('.divImportador', ProjetoGeografico.container).hasClass('hide');

		if (isImportador) {
			situacaoId = +$('.hdnArquivoEnviadoSituacaoId', ProjetoGeografico.container).val();
		} else {
			situacaoId = +$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val();
		}

		if ($.inArray(situacaoId, [0, 3, 4, 5, 8, 9, 10, 13, 14, 15]) == -1) {

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

		ImportadorShape.mostrar(false);
		Desenhador.mostrar(false);
		Sobreposicao.mostrar(false);

		$('table.gridArquivos tbody tr', ProjetoGeografico.container).remove();

		if ($('.radioTiPoMecanismo:checked', ProjetoGeografico.container).val() == 1) {
			ImportadorShape.mostrar(true);

			$('.fsImportadorShape', ProjetoGeografico.container).toggleClass('hide', $('.gridImportarArquivos tbody tr', ProjetoGeografico.container).length <= 0);

			BaseReferencia.settings.gerouOrtoFotoMosaico = false;
			BaseReferencia.gerarOrtoMosaico();
			BaseReferencia.obterSituacao();
		} else {
			Desenhador.mostrar(true);

			$('.desenhadorArquivosGrid tbody', Desenhador.container).append($('.gridImportarArquivos tbody', ImportadorShape.container).html());
			$('.divDesenhadorArquivo', Desenhador.container).toggleClass('hide', $('.desenhadorArquivosGrid tbody tr', Desenhador.container).length <= 0);

			Desenhador.exibirReenviar();
		}

		var situacaoId = +$('.hdnArquivoEnviadoSituacaoId', ImportadorShape.container).val();

		if (situacaoId == 0) {
			situacaoId = +$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val();
		}

		if (situacaoId != 0) {//Processado
			Mensagem.gerar(ProjetoGeografico.container, [ProjetoGeografico.mensagens.AlterarMecanismo]);
		}

		$('.divAlertaAPP', ProjetoGeografico.container).addClass('hide');
		$('.divAlertaARL', ProjetoGeografico.container).addClass('hide');

		Listar.atualizarEstiloTable();
	},

	criarProjetoGeografico: function () {
		var data = { projeto: ProjetoGeografico.obter() };

		$.ajax({
			url: ProjetoGeografico.settings.urls.criarParcial,
			data: JSON.stringify(data),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ProjetoGeografico.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Redirecionar) {
					MasterPage.redireciona(response.Redirecionar);
				}

				if (!response.EhValido) {
					Mensagem.gerar(ProjetoGeografico.container, response.Msg);
					return;
				}

				$('.hdnProjetoId', ProjetoGeografico.container).val(response.Id)
				ProjetoGeografico.settings.projetoId = response.Id;
				BaseReferencia.gerarOrtoMosaico();

			}
		});

	},

	mostrarImportadorShape: function () {
		if ($('.gridArquivosVetoriais tbody .hdnProcessamentoFilaId[value=0]', ProjetoGeografico.container).length == 0) {
			$('.fsImportadorShape', ProjetoGeografico.container).removeClass('hide');
		}
	},

	onAlterarCoordenada: function () {

		if (ProjetoGeografico.settings.projetoId == 0) {
			ProjetoGeografico.onAbrirNavegadorCoordenada();
			return;
		}

		Modal.confirma({
			btnOkCallback: function (container) {
				Modal.fechar(container);
				ProjetoGeografico.onAbrirNavegadorCoordenada({ callBack: ProjetoGeografico.callBackConfirmadoAlteracao });

			},
			titulo: "Alterar Área de Abrangência",
			conteudo: ProjetoGeografico.mensagens.ConfirmarAreaAbrangencia.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});

	},

	callBackConfirmadoAlteracao: function (retorno) {

		ProjetoGeografico.callBackAreaAbrangencia(retorno);

		var data = { projeto: ProjetoGeografico.obter() };

		if (!ProjetoGeografico.validar(data.projeto)) {
			return;
		}

		$.ajax({
			url: ProjetoGeografico.settings.urls.alterarAreaAbrangencia,
			data: JSON.stringify(data),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(ProjetoGeografico.container, response.Msg);
					return;
				}

				var linhaModelo = $('.gridArquivosVetoriais tbody tr.linhaModelo', BaseReferencia.container);
				$('.divImportadorShape', ImportadorShape.container).removeClass('hide');

				Sobreposicao.mostrar(false);
				Desenhador.obterSituacao();

				var listaArqVet = $.parseJSON($(".hdnEstadoPadrao", BaseReferencia.container).val());

				$(listaArqVet).each(function (i, item) {
					var linha = $('.trTempletaArqVetorial', BaseReferencia.container).clone();

					linha.removeClass('trTempletaArqVetorial');

					$('.txtNome', linha).text(item.Texto);
					$('.hdnSituacaoId', linha).val(item.Situacao);
					$('.hdnArquivoId', linha).val(item.Id);
					$('.hdnProcessamentoFilaId', linha).val(item.IdRelacionamento);
					$('.hdnArquivoTipo', linha).val(item.Tipo);
					$('.hdnArquivoFilaTipo', linha).val(item.FilaTipo);
				});


				MasterPage.grid($('.gridArquivosVetoriais'));

				BaseReferencia.settings.gerouOrtoFotoMosaico = false;
				BaseReferencia.gerarOrtoMosaico();
			}
		});
	},

	setarBotoes: function (settings) {

		var botoes = { avancar: false, refazer: false, recarregar: false, excluir: false, finalizar: false, salvar: false };

		$.extend(botoes, settings);

		$('.spanAvancar', ProjetoGeografico.container).toggleClass('hide', !botoes.avancar);
		$('.spanRefazer', ProjetoGeografico.container).toggleClass('hide', !botoes.refazer);
		$('.spanRecaregar', ProjetoGeografico.container).toggleClass('hide', !botoes.recarregar);
		$('.spanExcluir', ProjetoGeografico.container).toggleClass('hide', !botoes.excluir);
		$('.spanFinalizar', ProjetoGeografico.container).toggleClass('hide', !botoes.finalizar);
		$('.spnSalvar', ProjetoGeografico.container).toggleClass('hide', !botoes.salvar);
		$('.spanOuCancelar', ProjetoGeografico.container).toggleClass('hide', !botoes.salvar);
	},

	obterBotoes: function (situacaoProcessamento) {

		var botoes = {
			reenviar: false,
			reprocessar: false,
			regerar: false,
			baixar: false,
			cancelar: false,
			gerar: false
		};

		var situacao = ProjetoGeografico.settings.idsTelaSitacaoProcessamento;

		switch (situacaoProcessamento) {
			case situacao.AguardandoValidacao:
			case situacao.AguardandoProcessamento:
			case situacao.AguardandoGeracaoPDF:
				botoes.cancelar = true;
				break;

			case situacao.ExecutantoValidacao:
			case situacao.Processando:
			case situacao.GerandoPDF:
				break;

			case situacao.ErroProcessamento:
				botoes.reenviar = true;
				botoes.reprocessar = true;
				botoes.regerar = true;
				break;

			case situacao.ErroValidacao:
			case situacao.Reprovado:
			case situacao.Cancelada:
			case situacao.Processado:
			case situacao.Cancelado:
			case situacao.ErroGerarPDF:
			case situacao.Cancelada:
				botoes.baixar = true;
				botoes.reenviar = true;
				botoes.reprocessar = true;
				botoes.regerar = true;
				break;

			case situacao.ProcessadoPDF:
				botoes.baixar = true;
				botoes.reenviar = true;
				botoes.regerar = true;
				botoes.reprocessar = true;
				break;

			default:
				botoes.gerar = true;
				break;
		}

		return botoes;

	},

	abrirModalMergeAtualizarDominio: function (textoModal) {

		Modal.confirma({
			removerFechar: true,
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				MasterPage.carregando(true);
				$.ajax({
					url: ProjetoGeografico.settings.urls.obterMerge,
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
	},

	processar: function (settings) {
		var container = settings.container;

		$('table.gridArquivos tbody tr', container).remove();
		$('.divArquivosProcessados', container).addClass('hide');

		var data = {
			Id: $('.hdnProcessamentoFilaId', container).val(),
			ProjetoId: ProjetoGeografico.settings.projetoId,
			Mecanismo: settings.mecanismo,
			Etapa: $('.hdnProcessamentoEtapa', container).val(),
			FilaTipo: $('.hdnArquivoEnviadoFilaTipo', ProjetoGeografico.container).val(),
			Situacao: $('.hdnArquivoEnviadoSituacaoId', container).val()
		};

		$.ajax({
			url: ProjetoGeografico.settings.urls.processar,
			data: JSON.stringify(data),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
				MasterPage.carregando(false);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(MasterPage.getContent(container), response.Msg);
					return;
				}

				$('.lblSituacaoProcessamento', container).text(response.ProcessamentoGeo.SituacaoTexto);
				$('.hdnArquivoEnviadoSituacaoId', container).val(response.ProcessamentoGeo.Situacao);
				$('.hdnProcessamentoFilaId', container).val(response.ProcessamentoGeo.Id);

				$('.divAlertaAPP', ProjetoGeografico.container).addClass('hide');
				$('.divAlertaARL', ProjetoGeografico.container).addClass('hide');
				$('.divImportarArquivo', container).removeClass("hide");

				settings.gerenciarBotoes(response.ProcessamentoGeo.Situacao);
				ProjetoGeografico.obterSituacao(settings);
			}
		});

	},

	cancelarProcesssamento: function (settings) {
		var container = settings.container;
		
		var data = {
			Id: $('.hdnProcessamentoFilaId', container).val(),
			ProjetoId: ProjetoGeografico.settings.projetoId,
			Mecanismo: settings.mecanismo,
			Etapa: $('.hdnProcessamentoEtapa', container).val(),
			FilaTipo: $('.hdnArquivoEnviadoFilaTipo').val(),
			Situacao: $('.hdnArquivoEnviadoSituacaoId', container).val()
		};


		$.ajax({
			url: ProjetoGeografico.settings.urls.cancelarProcessamento,
			data: JSON.stringify(data),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
				MasterPage.carregando(false);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(MasterPage.getContent(ProjetoGeografico.container), response.Msg);
					return;
				}

				var processamentoGeo = response.ProcessamentoGeo;
				settings.gerenciarBotoes(processamentoGeo.Situacao);

				$('.hdnProcessamentoFilaId', container).val(processamentoGeo.Id)
				$('.lblSituacaoProcessamento', container).text(processamentoGeo.SituacaoTexto);
				$('.hdnProcessamentoEtapa', container).val(processamentoGeo.Etapa);
				$('.hdnArquivoEnviadoFilaTipo', container).val(processamentoGeo.FilaTipo);
				$('.hdnArquivoEnviadoSituacaoId', container).val(processamentoGeo.Situacao);

				$('.divAlertaAPP', ProjetoGeografico.container).addClass('hide');
				$('.divAlertaARL', ProjetoGeografico.container).addClass('hide');

				Sobreposicao.mostrar(false);
				Sobreposicao.limpar();
				ProjetoGeografico.obterSituacao(settings);
			}
		});

	},

	obterSituacao: function (settings) {
		if (settings) { $.extend(ProjetoGeografico.settings.obterSituacaoSettings, settings); }
		settings = ProjetoGeografico.settings.obterSituacaoSettings;

		var container = settings.container;

		var situacaoId = +$('.hdnArquivoEnviadoSituacaoId', container).val();

		settings.gerenciarBotoes(situacaoId);

		if ($.inArray(situacaoId, settings.situacoesValidas) != -1) {

			/*$('.divArquivosProcessados tbody tr :not(.trTemplateArqProcessado)', container).remove();
			$('.divArquivosProcessados', container).addClass('hide');*/

			var data = {
				Id: $('.hdnProcessamentoFilaId', container).val(),
				ProjetoId: ProjetoGeografico.settings.projetoId,
				Mecanismo: settings.mecanismo,
				Etapa: $('.hdnProcessamentoEtapa', container).val(),
				FilaTipo: $('.hdnArquivoEnviadoFilaTipo', container).val(),
				Situacao: $('.hdnArquivoEnviadoSituacaoId', container).val()
			};

			$.ajax({
				url: ProjetoGeografico.settings.urls.obterSituacao,
				data: JSON.stringify(data),
				cache: false,
				async: false,
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
				},
				success: function (response, textStatus, XMLHttpRequest) {

					if (!response.EhValido) {
						Mensagem.gerar(ProjetoGeografico.container, response.Msg);
					}

					if (ProjetoGeografico.settings.threadAtualizarSituacao == null) {
						return;
					}

					$('.lblSituacaoProcessamento', container).text(response.ProcessamentoGeo.SituacaoTexto);
					$('.hdnArquivoEnviadoSituacaoId', container).val(response.ProcessamentoGeo.Situacao);
					$('.hdnProcessamentoFilaId', container).val(response.ProcessamentoGeo.Id);

					$('.divSituacaoProjeto', container).removeClass("hide");

					settings.gerenciarBotoes(response.ProcessamentoGeo.Situacao);

					if (settings.mecanismo == ProjetoGeografico.settings.idsTelaMecanismo.Desenhador) {

						if (Navegador.settings.setSituacaoProcessamento) {
							Navegador.settings.setSituacaoProcessamento(JSON.stringify({
								SituacaoTexto: response.ProcessamentoGeo.SituacaoTexto,
								SituacaoId: response.ProcessamentoGeo.Situacao,
								ArquivosProcessados: []
							}));

						}
					}

					Listar.atualizarEstiloTable();

				}
			});

			if (ProjetoGeografico.settings.threadAtualizarSituacao == null) {
				ProjetoGeografico.settings.threadAtualizarSituacao = setInterval(ProjetoGeografico.obterSituacao, ProjetoGeografico.settings.threadAtualizarSituacaoDelay);
			} else {
				Sobreposicao.gerenciar(+$('.hdnArquivoEnviadoSituacaoId', container).val());
			}

			return;
		}

		if (!ProjetoGeografico.settings.isVisualizar) {
			ProjetoGeografico.gerenciarArquivos(container);
		}

		ProjetoGeografico.stopThread();
	},

	gerenciarArquivos: function (container) {
		var arquivosDesenhador = [];

		$.ajax({
			url: ProjetoGeografico.settings.urls.obterArquivos,
			data: JSON.stringify(
			{
				projetoGeograficoId: ProjetoGeografico.settings.projetoId,
				isFinalizado: ProjetoGeografico.settings.isFinalizado
			}),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(MasterPage.getContent(ProjetoGeografico.container), response.Msg);
				}

				if (response.Arquivos.length > 0) {
					$('table.gridArquivos tbody tr', container).remove();
					$('.divArquivosProcessados', container).removeClass('hide');
				}

				$(response.Arquivos).each(function (i, item) {

					var isPDF = item.Tipo == ProjetoGeografico.settings.idsTelaArquivoTipo.Croqui;

					var linha = $('.trTemplateArqProcessado').clone();
					linha.removeClass('trTemplateArqProcessado download');

					$('.arquivoNome', linha).text(item.Nome);
					$('.hdnArquivoProcessadoId', linha).val(item.Id);
					$('.btnBaixar', linha).addClass(isPDF ? 'pdf' : 'download');

					$('.gridArquivos tbody:last', container).append(linha);

					arquivosDesenhador.push(
					{
						Id: item.Id,
						Texto: item.Nome,
						IsPDF: isPDF
					});
				});

				if (Navegador.settings.setSituacaoProcessamento) {
					Navegador.settings.setSituacaoProcessamento(JSON.stringify({
						SituacaoTexto: $('.lblSituacaoProcessamento', container).text(),
						SituacaoId: $('.hdnArquivoEnviadoSituacaoId', container).val(),
						ArquivosProcessados: arquivosDesenhador
					}));
				}

				Listar.atualizarEstiloTable();
			}

		});

	},

	stopThread: function () {
		clearInterval(ProjetoGeografico.settings.threadAtualizarSituacao);
		ProjetoGeografico.settings.threadAtualizarSituacao = null;
	}

}

BaseReferencia = {
	settings: {
		projetoId: null,
		urls: {
			obterSituacao: null,
			gerarArquivoVetorial: null,
			gerarArquivoOrtoFotoMosaico: null,
			baixarArquivoVetorial: null,
			baixarArquivoOrtoMosaico: null,
			salvarArquivoOrtoMosaico: null,
			baixarArquivos: null
		},
		gerouOrtoFotoMosaico: false,
		threadAtualizarSituacao: null,
		threadAtualizarSituacaoDelay: 1000 * 2,
		situacoesValidas: []
	},
	container: null,

	load: function (container, options) {

		if (options) { $.extend(BaseReferencia.settings, options); }

		BaseReferencia.container = container;

		$('.gridArquivosVetoriais', BaseReferencia.container).delegate('.btnGerar', 'click', BaseReferencia.onGerarArquivoVetorial);
		$('.gridArquivosVetoriais', BaseReferencia.container).delegate('.btnDownload', 'click', BaseReferencia.onBaixarArquivoVetorial);
		$('.gridArquivosVetoriais', BaseReferencia.container).delegate('.btnRegerar', 'click', BaseReferencia.onGerarArquivoVetorial);
		$('.gridArquivosRaster', BaseReferencia.container).delegate('.btnDownload', 'click', BaseReferencia.onBaixarArquivoOrtoMosaico);
		$('.gridDadosDominio', BaseReferencia.container).delegate('.btnDownload', 'click', BaseReferencia.onBaixarDadosDominio);

		$('.gridArquivosVetoriais', BaseReferencia.container).delegate('.btnDownloadModelo', 'click', BaseReferencia.onBaixarArquivoModelo);
	},

	obterSituacao: function () {
		//Removido por causa do visualizar
		//if (ProjetoGeografico.settings.isVisualizar) {
		//	return;
		//}

		ProjetoGeografico.mostrarImportadorShape();

		var _turn = 0;
		var getTurn = function () {
			if (!_turn) {
				_turn = 0;
			}

			return ++_turn;
		}

		var _obterSituacao = function () {
			var containers = [];
			$('.gridArquivosVetoriais tbody tr:not(.linhaModelo)', ProjetoGeografico.container).each(function (i, item) {
				containers.push(item);
				BaseReferencia.gerenciarBotoes(item, +$('.hdnArquivoEnviadoSituacaoId', item).val());
			});

			var container = containers[getTurn() % 2];
			if (!container) return;

			var situacaoId = +$('.hdnArquivoEnviadoSituacaoId', container).val();

			if ($.inArray(situacaoId, BaseReferencia.settings.situacoesValidas) != -1) {

				var data = {
					Id: $('.hdnProcessamentoFilaId', container).val(),
					ProjetoId: ProjetoGeografico.settings.projetoId,
					Mecanismo: $('.hdnProcessamentoMecanismo', container).val(),
					Etapa: ProjetoGeografico.settings.idsTelaEtapaProcessamento.Processamento,
					FilaTipo: $('.hdnArquivoEnviadoFilaTipo', container).val(),
					Situacao: $('.hdnArquivoEnviadoSituacaoId', container).val()
				};

				$.ajax({
					url: BaseReferencia.settings.urls.obterSituacao,
					data: JSON.stringify(data),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
					},
					success: function (response, textStatus, XMLHttpRequest) {

						if (!response.EhValido) {
							Mensagem.gerar(ProjetoGeografico.container, response.Msg);
						}

						if (BaseReferencia.settings.threadAtualizarSituacao == null) {
							return;
						}

						$('.txtSituacao', container).text(response.ProcessamentoGeo.SituacaoTexto);
						$('.hdnArquivoEnviadoSituacaoId', container).val(response.ProcessamentoGeo.Situacao);
						$('.hdnProcessamentoFilaId', container).val(response.ProcessamentoGeo.Id);

						BaseReferencia.gerenciarBotoes(container, response.ProcessamentoGeo.Situacao);

					}
				});

			}

			var countThread = 0;
			$(containers).each(function (i, item) {

				var situacao = +$('.hdnArquivoEnviadoSituacaoId', item).val();

				if ($.inArray(situacao, BaseReferencia.settings.situacoesValidas) != -1) {
					if (BaseReferencia.settings.threadAtualizarSituacao == null) {
						BaseReferencia.settings.threadAtualizarSituacao = setInterval(_obterSituacao, BaseReferencia.settings.threadAtualizarSituacaoDelay);
					}
				} else {
					countThread++;
				}
			});

			if (countThread > 1) {
				BaseReferencia.stopThread();
			}
		}

		_obterSituacao();

	},

	stopThread: function () {
		clearInterval(BaseReferencia.settings.threadAtualizarSituacao);
		BaseReferencia.settings.threadAtualizarSituacao = null;
	},

	gerarOrtoMosaico: function () {
		if (BaseReferencia.settings.gerouOrtoFotoMosaico) {
			return;
		}

		$.ajax({
			url: BaseReferencia.settings.urls.gerarArquivoOrtoFotoMosaico,
			data: { projetoId: ProjetoGeografico.settings.projetoId },
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				$('.gridArquivosRaster', BaseReferencia.container).addClass('hide');
				$('.gridArquivosRaster tbody tr', BaseReferencia.container).remove();

				if (!response.EhValido) {
					Mensagem.gerar(MasterPage.getContent(BaseReferencia.container), response.Msg);
					return;
				}

				BaseReferencia.settings.gerouOrtoFotoMosaico = true;

				if ($('.gridArquivosRaster tbody tr', BaseReferencia.container).length == 0) {
					var dados = { projeto: ProjetoGeografico.obter() };

					MasterPage.carregando(true);
					$.ajax({
						url: BaseReferencia.settings.urls.salvarArquivoOrtoMosaico,
						data: JSON.stringify(dados),
						cache: false,
						async: false,
						type: 'POST',
						dataType: 'json',
						contentType: 'application/json; charset=utf-8',
						error: function (XMLHttpRequest, textStatus, erroThrown) {
							Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
							MasterPage.carregando(false);
						},
						success: function (response, textStatus, XMLHttpRequest) {

						}
					});
					MasterPage.carregando(false);
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

				Listar.atualizarEstiloTable();
			}
		});

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
		var id = $(this).closest('tr').find('.hdnArquivoId').val();
		Aux.downloadAjax("downloadPrjGeo", BaseReferencia.settings.urls.baixarArquivos + "/" + id, null, 'GET');
	},

	onBaixarArquivoOrtoMosaico: function () {
		var caminho = $(this).closest('tr').find('.hdnCaminho').val();
		var chave = $(this).closest('tr').find('.hdnChave').val();

		MasterPage.redireciona(BaseReferencia.settings.urls.baixarArquivoOrtoMosaico + "?chave=" + chave);
	},

	onGerarArquivoVetorial: function () {

		var container = $(this).closest('tr');

		var data = {
			Id: $('.hdnProcessamentoFilaId', container).val(),
			ProjetoId: ProjetoGeografico.settings.projetoId,
			Mecanismo: ProjetoGeografico.settings.idsTelaMecanismo.ImportadorShape,
			Etapa: ProjetoGeografico.settings.idsTelaEtapaProcessamento.Processamento,
			FilaTipo: $('.hdnArquivoEnviadoFilaTipo', container).val(),
			Situacao: $('.hdnArquivoEnviadoSituacaoId', container).val()
		};

		$.ajax({
			url: BaseReferencia.settings.urls.gerarArquivoVetorial,
			data: JSON.stringify(data),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
				MasterPage.carregando(false);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(MasterPage.getContent(BaseReferencia.container), response.Msg);
					return;
				}

				var processamento = response.ProcessamentoGeo;

				$('.hdnProcessamentoFilaId', container).val(processamento.Id);
				$('.hdnArquivoEnviadoSituacaoId', container).val(processamento.Situacao);
				$('.txtSituacao', container).text(processamento.SituacaoTexto);

				BaseReferencia.gerenciarBotoes(container, processamento.Situacao);
				BaseReferencia.obterSituacao(container);
			}
		});

	},

	gerenciarBotoes: function (container, situacaoProcessamento) {

		var botoes = ProjetoGeografico.obterBotoes(situacaoProcessamento);

		$('.btnGerar', container).toggleClass('hide', !botoes.gerar);
		$('.btnDownload', container).toggleClass('hide', !botoes.baixar);
		$('.btnRegerar', container).toggleClass('hide', !botoes.regerar);
	},

	onBaixarArquivoVetorial: function () {
		var id = $(this).closest('tr').find('.hdnArquivoId').val();
		Aux.downloadAjax("downloadPrjGeo", BaseReferencia.settings.urls.baixarArquivoVetorial + "/" + id, null, 'GET');
	},

	onBaixarArquivoModelo: function () {
		Aux.downloadAjax("downloadPrjGeo", BaseReferencia.settings.urls.baixarArquivoModelo, null, 'GET');
	}
}

ImportadorShape = {
	settings: {
		urls: {
			ImportadorShape: null,
			baixarArquivos: null,
			reenviarArquivo: null,
			obterArquivos: null
		},
		situacoesValidas: []
	},
	mensagens: null,
	container: null,

	load: function (container, options) {
		if (options) { $.extend(ImportadorShape.settings, options); }

		ImportadorShape.container = container;

		$(container).delegate('.btnEnviarProjetoGeo', 'click', ImportadorShape.onEnviaArquivo);
		$(container).delegate('.btnCancelarProcessamento', 'click', ImportadorShape.onCancelarProcessamento);
		$(container).delegate('.btnReenviarArquivo', 'click', ImportadorShape.onReenviarArquivo);
		$(container).delegate('.btnBaixar', 'click', ImportadorShape.baixarArquivoProcessado);
	},

	onReenviarArquivo: function () {
		Modal.confirma({
			btnOkCallback: function (modal) {
				$('.divAlertaAPP', ProjetoGeografico.container).addClass('hide');
				$('.divAlertaARL', ProjetoGeografico.container).addClass('hide');

				$('.hdnArquivo', ImportadorShape.container).val('');

				$('.divSituacaoProjeto', ImportadorShape.container).addClass("hide");
				$('.divImportadorShape', ImportadorShape.container).removeClass("hide");
				$('.divImportarArquivo', ImportadorShape.container).addClass('hide');

				Sobreposicao.mostrar(false);
				Sobreposicao.limpar();

				Modal.fechar(modal);
			},
			titulo: "Reenviar Arquivo",
			conteudo: ProjetoGeografico.mensagens.ConfirmacaoReenviar.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	mostrar: function (flag) {
		$('.divImportador', ProjetoGeografico.container).toggleClass('hide', !flag);
	},

	onEnviaArquivo: function () {
		var mensagens = new Array();

		var nome = $('.divImportadorShape input[type="file"]', ImportadorShape.container).val();

		if (nome.toLowerCase().substr(nome.length - 4) !== ".zip") {
			mensagens.push(ImportadorShape.mensagens.ArquivoAnexoNaoEhZip);
		}

		if (mensagens.length > 0) {
			Mensagem.gerar(MasterPage.getContent(ImportadorShape.container), mensagens);
			return;
		}

		$('.divImportarArquivo', ImportadorShape.container).addClass('hide');

		var inputFile = $('.divImportadorShape input[type="file"]', ImportadorShape.container).attr("id", "ArquivoId_1");

		FileUpload.upload(ImportadorShape.settings.urls.ImportadorShape, inputFile, function (controle, retorno) {

			var arquivo = $.parseJSON(retorno).Arquivo;

			arquivo.ProjetoId = ProjetoGeografico.settings.projetoId;

			arquivo.Processamento = {
				ProjetoId: ProjetoGeografico.settings.projetoId,
				Tipo: $('.hdnArquivoEnviadoTipo', ImportadorShape.container).val(),
				Situacao: $('.hdnArquivoEnviadoSituacao', ImportadorShape.container).val(),
				Etapa: $('.hdnArquivoEnviadoEtapa', ImportadorShape.container).val(),
				FilaTipo: $('.hdnArquivoEnviadoFilaTipo', ProjetoGeografico.container).val(),
				Mecanismo: $('.radioTiPoMecanismo:radio:checked', ProjetoGeografico.container).val()
			}

			if (arquivo != null && typeof arquivo != "undefined") {
				$('.hdnArquivo', ImportadorShape.container).val(JSON.stringify(arquivo));
				$('.divImportadorShape', ImportadorShape.container).addClass("hide");
				$('.divSituacaoProjeto', ImportadorShape.container).removeClass("hide");

				$.ajax({
					url: ImportadorShape.settings.urls.EnviarParaProcessar,
					data: JSON.stringify(arquivo),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
						MasterPage.carregando(false);
					},
					success: function (response, textStatus, XMLHttpRequest) {

						if (!response.EhValido) {
							Mensagem.gerar(MasterPage.getContent(ImportadorShape.container), response.Msg);
							return;
						}

						var arquivo = response.Arquivo;
						var processamento = response.Arquivo.Processamento;

						$('.hdnArquivoEnviadoId', ImportadorShape.container).val(arquivo.Id);
						$('.lblSituacaoProcessamento', ImportadorShape.container).text(processamento.SituacaoTexto);
						$('.hdnArquivoEnviadoSituacaoId', ImportadorShape.container).val(processamento.Situacao);
						$('.hdnProcessamentoFilaId', ImportadorShape.container).val(processamento.Id);
						$('.hdnProcessamentoEtapa', ImportadorShape.container).val(processamento.Etapa);
						$('.hdnProcessamentoMecanismo', ImportadorShape.container).val(processamento.Mecanismo);

						ImportadorShape.obterSituacao();
					}
				});

			} else {
				$('.divImportadorShape .hdnArquivo', ImportadorShape.container).val('');
				$('.divEnviarProjeto input[type=file]').val('');
				if (ret.Msg.length > 0) {
					Mensagem.gerar(ProjetoGeografico.container, ret.Msg);
				}
			}

		});
	},

	obterSituacao: function () {
		ProjetoGeografico.obterSituacao({
			container: ImportadorShape.container,
			gerenciarBotoes: ImportadorShape.gerenciarBotoes,
			mecanismo: ProjetoGeografico.settings.idsTelaMecanismo.ImportadorShape,
			situacoesValidas: ImportadorShape.settings.situacoesValidas
		});
	},

	onCancelarProcessamento: function () {
		ProjetoGeografico.cancelarProcesssamento({
			container: ImportadorShape.container,
			gerenciarBotoes: ImportadorShape.gerenciarBotoes,
			mecanismo: ProjetoGeografico.settings.idsTelaMecanismo.ImportadorShape,
			situacoesValidas: ImportadorShape.settings.situacoesValidas
		});
	},

	adicionarArquivoDownload: function (nome, url, tipo) {
		var novaLinha = $('.importarArquivosGridTemplate .trTemplate', ImportadorShape.container).clone();

		novaLinha.removeClass('trTemplate').removeClass("hide");
		$('button', novaLinha).addClass(tipo == 0 ? "download" : "pdf");
		$('.arquivoNomeTd', novaLinha).text(nome);
		$(novaLinha).delegate('button', 'click', function () { return ImportadorShape.baixarArquivoProcessado(); });

		$('.gridImportarArquivos tbody', ImportadorShape.container).append(novaLinha);

		Listar.atualizarEstiloTable($('.gridImportarArquivos', ImportadorShape.container));
	},

	baixarArquivoProcessado: function () {
		var id = $(this).closest('tr').find('.hdnArquivoProcessadoId').val();
		Aux.downloadAjax("downloadPrjGeo", ImportadorShape.settings.urls.baixarArquivos + "/" + id, null, 'GET');
	},

	gerenciarBotoes: function (situacao) {
		var botoes = ProjetoGeografico.obterBotoes(situacao);

		$('.btnReenviarArquivo', ImportadorShape.container).toggleClass('hide', !botoes.reenviar);
		$('.btnCancelarProcessamento', ImportadorShape.container).toggleClass('hide', !botoes.cancelar);
	}
}

Desenhador = {
	settings: {
		urls: {
			desenhador: null,
			confirmarReenviarArquivo: null
		},
		situacoesValidas: [],
		isVisualizar: false
	},
	container: null,

	load: function (container, options) {
		Desenhador.container = container;
		if (options) { $.extend(Desenhador.settings, options); }

		$(Desenhador.container).delegate('.btnDesenhador', 'click', Desenhador.onAbrirDesenhador);
		$(Desenhador.container).delegate('.btnReprocessarGeo', 'click', Desenhador.onReprocessarArquivo);
		$(Desenhador.container).delegate('.btnCancelarProcessamentoGeo', 'click', Desenhador.onCancelarProcessamento);
		$(Desenhador.container).delegate('.btnBaixar', 'click', function (container) {
			var id = $(container.currentTarget).closest('tr').find('.hdnArquivoProcessadoId').val();
			Desenhador.onBaixarArquivo(id)
		});
	},

	onProcessar: function () {
		ProjetoGeografico.processar({
			container: Desenhador.container,
			gerenciarBotoes: Desenhador.gerenciarBotoes,
			mecanismo: ProjetoGeografico.settings.idsTelaMecanismo.Desenhador,
			situacoesValidas: Desenhador.settings.situacoesValidas
		});
	},

	onCancelarProcessamento: function () {
		ProjetoGeografico.cancelarProcesssamento({
			container: Desenhador.container,
			gerenciarBotoes: Desenhador.gerenciarBotoes,
			mecanismo: ProjetoGeografico.settings.idsTelaMecanismo.Desenhador,
			situacoesValidas: Desenhador.settings.situacoesValidas
		});
	},

	onReprocessarArquivo: function () {
		Modal.confirma({
			btnOkCallback: function (modal) {

				$('.hdnArquivo', ImportadorShape.container).val('');
				$('.divSituacaoProjeto', ImportadorShape.container).addClass("hide");
				$('.divImportadorShape', ImportadorShape.container).removeClass("hide");
				$('.divImportarArquivo', ImportadorShape.container).addClass('hide');
				Desenhador.limparArquivosProcessados();

				ProjetoGeografico.processar({
					container: Desenhador.container,
					gerenciarBotoes: Desenhador.gerenciarBotoes,
					mecanismo: ProjetoGeografico.settings.idsTelaMecanismo.Desenhador,
					situacoesValidas: Desenhador.settings.situacoesValidas
				});

				Sobreposicao.mostrar(false);
				Sobreposicao.limpar();

				Modal.fechar(modal);

			},

			titulo: "Reprocessar Arquivo",
			conteudo: ProjetoGeografico.mensagens.ConfirmacaoReenviar.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	obterSituacao: function () {
		ProjetoGeografico.obterSituacao({
			container: Desenhador.container,
			gerenciarBotoes: Desenhador.gerenciarBotoes,
			mecanismo: ProjetoGeografico.settings.idsTelaMecanismo.Desenhador,
			situacoesValidas: Desenhador.settings.situacoesValidas
		});
	},

	mostrar: function (flag) {
		$('.divDesenhador', ProjetoGeografico.container).toggleClass('hide', !flag);
	},

	onAbrirDesenhador: function () {

		Mensagem.limpar(ProjetoGeografico.container);

		var desenhadorModo = Desenhador.settings.isVisualizar ? 2 : 1;

		Modal.abrir(Desenhador.settings.urls.desenhador + '?modo=' + desenhadorModo, null,
		function (container) {

			Navegador.load(container, {
				id: ProjetoGeografico.settings.projetoId,
				modo: desenhadorModo,
				tipo: $('.hdnArquivoEnviadoFilaTipo', ProjetoGeografico.container).val(),
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
		return JSON.stringify({
			MenorX: $('.txtMenorX', ProjetoGeografico.container).val(),
			MenorY: $('.txtMenorY', ProjetoGeografico.container).val(),
			MaiorX: $('.txtMaiorX', ProjetoGeografico.container).val(),
			MaiorY: $('.txtMaiorY', ProjetoGeografico.container).val()
		});
	},

	obterSituacaoInicial: function () {

		var arquivos = [];

		$.ajax({
			url: ProjetoGeografico.settings.urls.obterArquivos,
			data: JSON.stringify({ projetoGeograficoId: ProjetoGeografico.settings.projetoId, isFinalizado: ProjetoGeografico.settings.isFinalizado }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(MasterPage.getContent(ProjetoGeografico.container), response.Msg);
				}

				$(response.Arquivos).each(function (i, item) {
					arquivos.push(
					{
						Id: item.Id,
						Texto: item.Nome,
						IsPDF: item.Tipo == ProjetoGeografico.settings.idsTelaArquivoTipo.Croqui
					});
				});
			}

		});

		return JSON.stringify({
			SituacaoId: $('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val(),
			SituacaoTexto: $('.lblSituacaoProcessamento', Desenhador.container).text(),
			ArquivosProcessados: arquivos
		});

		ProjetoGeografico.gerenciarArquivos(Desenhador.container);

	},

	limparArquivosProcessados: function () {
		$('.divSituacao', Desenhador.container).addClass('hide');
		$('.divDesenhadorArquivo', Desenhador.container).addClass('hide');
		$('.desenhadorArquivoGrid tbody tr', Desenhador.container).remove();
	},

	exibirReenviar: function () {

		$('.hdnArquivo', Desenhador.container).val('');
		$('.divSituacaoProjeto', Desenhador.container).addClass("hide");
		$('.divImportadorShape', Desenhador.container).removeClass("hide");
		$('.divImportarArquivo', Desenhador.container).addClass('hide');

		Desenhador.gerenciarBotoes(+$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val());

	},

	onBaixarArquivo: function (id) {
		Aux.downloadAjax("downloadPrjGeo", Desenhador.settings.urls.baixarArquivos + "/" + id, null, 'GET');
	},

	gerenciarBotoes: function (situacaoProcessamento) {

		var botoes = ProjetoGeografico.obterBotoes(situacaoProcessamento);

		$('.btnReprocessarGeo', Desenhador.container).toggleClass('hide', !botoes.reprocessar);
		$('.btnCancelarProcessamentoGeo', Desenhador.container).toggleClass('hide', !botoes.cancelar);
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

		MasterPage.carregando(true);
		$.ajax({
			url: Sobreposicao.settings.urls.verificarSobreposicao,
			data: JSON.stringify(params),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
				MasterPage.carregando(false);
			},
			success: function (response, textStatus, XMLHttpRequest) {
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

					if (item.Base == 1) {
						$('.sobreposicaoIdafGrid tbody:last').append(linha);
					} else {
						$('.sobreposicaoGeoBasesGrid tbody:last').append(linha);
					}

					Listar.atualizarEstiloTable();
				});
			}
		});
		MasterPage.carregando(false);

	},

	mostrar: function (flag) {
		$('.divSobreposicao', ProjetoGeografico.container).toggleClass('hide', !flag);
	},

	setarSituacao: function (situacaoId) {

		var situacaoIdLocal = +$('.hdnArquivoEnviadoSituacaoId', Desenhador.container).val() | situacaoId;

		if (situacaoIdLocal == ProjetoGeografico.settings.idsTelaSitacaoProcessamento.Processado) {
			Sobreposicao.mostrar(true);
			//Sobreposicao.limpar();
		} else {
			Sobreposicao.mostrar(false);
		}
	},

	gerenciar: function (situacaoProcessamento) {
		if (situacaoProcessamento == ProjetoGeografico.settings.idsTelaSitacaoProcessamento.ProcessadoPDF) {
			$('.divExistemProcessamentosAndamento', ProjetoGeografico.container).addClass('hide');

			Sobreposicao.mostrar(true);

			$.ajax({
				url: ProjetoGeografico.settings.urls.verificarAreaNaoCaracterizada,
				data: JSON.stringify({ empreendimento: $('.hdnEmpreendimentoId', ProjetoGeografico.container).val(), projeto: ProjetoGeografico.settings.projetoId }),
				cache: false,
				async: false,
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
				},
				success: function (response, textStatus, XMLHttpRequest) {

					ProjetoGeografico.settings.isProcessado = response.IsProcessado;
					ProjetoGeografico.settings.possuiAPPNaoCaracterizada = response.PossuiAPPNaoCaracterizada;
					ProjetoGeografico.settings.possuiARLNaoCaracterizada = response.PossuiARLNaoCaracterizada;

					ProjetoGeografico.mensagemAreaNaoCaracterizada();
				}
			});
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
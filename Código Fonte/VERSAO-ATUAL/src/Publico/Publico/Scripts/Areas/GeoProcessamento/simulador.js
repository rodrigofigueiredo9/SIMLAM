/// <reference path="../../Lib/JQuery/jquery.json - 2.2.min.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../mensagem.js" />

Simulador = {
	settings: {
		urls: {
			salvar: null,
			verificarCpf: null
		},
		situacoesValidas: null
	},
	mensagens: null,
	container: null,

	load: function (container, options) {

		if (options) { $.extend(Simulador.settings, options); }

		Simulador.container = MasterPage.getContent(container);

		Simulador.container.delegate('.btnVerificarCpf', 'click', Simulador.verificarCpf);
		Simulador.container.delegate('.btnLimparCpf', 'click', Simulador.limparCpf);

		$('.txtCpf', Simulador.container).keypress(Simulador.enterCpf);

		if (!$('.txtCpf').val()) {
			$('.txtCpf').focus();
		}

		BaseReferencia.load($('.containerBaseReferencia', Simulador.container), options);
		EnviarProjeto.load($('.containerEnviar', Simulador.container), options);
	},

	enterCpf: function (e) {
		if (e.keyCode != 13)
			return;

		Simulador.verificarCpf();
	},

	abrirModalRedireciona: function (textoModal, finalizado) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', Simulador.container).attr('href'));
			},
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				if (finalizado) {
					Simulador.callBackRefazer(Simulador.container);
				} else {
					Modal.fechar(conteudoModal);
				}
			},
			conteudo: textoModal,
			titulo: Simulador.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	obterId: function () {
		return $('.hdnSimuladorGeoId', Simulador.container).val();
	},

	onSalvar: function (option) {
		var data = { projeto: Simulador.montarObjeto() };
		var settings = { url: Simulador.settings.urls.salvar, data: data, callBack: Simulador.callBackSalvar };
		$.extend(settings, option);
		if (Simulador.validarProjeto(data.projeto)) {

			Simulador.postGet({
				url: settings.url,
				data: settings.data,
				callBack: settings.callBack,
				async: false,
				mostrarCarregando: true
			});
		}
	},

	callBackSalvar: function (retorno) {

		Mensagem.gerar(Simulador.container, retorno.Msg);

		if (!retorno.EhValido) {
			if (!Simulador.settings.isObteveMerge) {
				Simulador.abrirModalMergeAtualizarDominio(retorno.Mensagem);
				return;
			}
		}

		if ($('.btnExluir', Simulador.container).is(':visible')) {
			Simulador.gerarenciarBotoes({ recarregar: true, excluir: true, salvar: true, finalizar: true });
		} else {
			Simulador.gerarenciarBotoes({ salvar: true, finalizar: true });
		}

		ContainerAcoes.load(Simulador.container, { limparContainer: false, botoes: new Array({ label: 'Finalizar', callBack: Simulador.onFinalizar }) });
	},

	montarObjeto: function () {

		var obj = {};
		obj.Id = $('.hdnProjetoId', Simulador.container).val();
		obj.EmpreendimentoEasting = $('.hdnEmpEasting', Simulador.container).val();
		obj.EmpreendimentoNorthing = $('.hdnEmpNorthing', Simulador.container).val();

		return obj;
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

	verificarCpf: function () {
		var data = { cpf: $('.txtCpf', Simulador.container).val() };

		Simulador.postGet({
			url: Simulador.settings.urls.verificarCpf,
			data: data,
			callBack: Simulador.callBackVerificarCpf,
			async: false,
			mostrarCarregando: true
		});
	},

	callBackVerificarCpf: function (resultado) {

		if (!resultado.EhValido) {
			Mensagem.gerar(Simulador.container, resultado.Msg);
			return;
		}

		Mensagem.limpar(Simulador.container);

		var projetoObj = resultado.Vm.SimuladorGeo;

		$('.hdnSimuladorGeoId', Simulador.container).val(projetoObj.Id);
		$('.hdnSimuladorGeoMecanismoElaboracaoId', Simulador.container).val(projetoObj.MecanismoElaboracaoId);
		$('.hdnSimuladorGeoSituacaoId', Simulador.container).val(projetoObj.SituacaoId);

		$('.txtCpf', Simulador.container).addClass('disabled').attr('disabled', 'disabled');
		$('.btnVerificarCpf', Simulador.container).addClass('hide');
		$('.btnLimparCpf', Simulador.container).removeClass('hide');
		$('.SimuladorGeoContent', Simulador.container).removeClass('hide');

		var isArquivoEnviado = (projetoObj.ArquivoEnviado.IdRelacionamento > 0);

		if (isArquivoEnviado) {
			$('.txtEasting', Simulador.container).addClass('disabled').attr('disabled', 'disabled').val(projetoObj.Easting);
			$('.txtNorthing', Simulador.container).addClass('disabled').attr('disabled', 'disabled').val(projetoObj.Northing);
		}

		$('.divEnviarProjeto', EnviarProjeto.container).toggleClass("hide", isArquivoEnviado);
		$('.divSituacaoProjeto', EnviarProjeto.container).toggleClass("hide", !isArquivoEnviado);

		$('.hdnArquivo', EnviarProjeto.container).val(JSON.stringify(projetoObj.ArquivoEnviado));
		$('.hdnArquivoEnviadoId', EnviarProjeto.container).val(projetoObj.ArquivoEnviado.IdRelacionamento);
		$('.lblSituacaoProcessamento', EnviarProjeto.container).text(projetoObj.ArquivoEnviado.SituacaoTexto);

		//Força obter situacao
		$('.hdnArquivoEnviadoSituacaoId', EnviarProjeto.container).val(1);
		$('.gridArquivosVetoriais tbody tr .hdnSituacaoId', BaseReferencia.container).val(1);

		BaseReferencia.carregarArquivos({ lista: resultado.Vm.ArquivosVetoriais });
		BaseReferencia.obterSituacao();
		EnviarProjeto.obterSituacao();
	},

	limparCpf: function () {

		$('.hdnArquivoEnviadoId', Simulador.container).val('');
		$('.lblSituacaoProcessamento', Simulador.container).text('');
		$('.hdnArquivoEnviadoSituacaoId', Simulador.container).val('');

		$('.SimuladorGeoContent', Simulador.container).addClass('hide');
		$('.txtCpf', Simulador.container).removeClass('disabled').removeAttr('disabled').val('');
		$('.btnVerificarCpf', Simulador.container).removeClass('hide');
		$('.btnLimparCpf', Simulador.container).addClass('hide');

		$('.hdnArquivoId', BaseReferencia.container).val('');
		$('.hdnArquivoIdRelacionamento', BaseReferencia.container).val('');
		$('.hdnSituacaoId', BaseReferencia.container).val('');

		var linhas = $('.hdnArquivoTipo[value=1],.hdnArquivoTipo[value=2]', BaseReferencia.container).parents('tr');
		$('.txtSituacao', linhas).text('Aguardando solicitação');
		$('.btnDownload,btnRegerar', linhas).addClass('hide');
		$('.btnGerar', linhas).removeClass('hide');

		$('.txtEasting', Simulador.container).removeClass('disabled').removeAttr('disabled').val('');
		$('.txtNorthing', Simulador.container).removeClass('disabled').removeAttr('disabled').val('');

		BaseReferencia.obterSituacao();
	},

	postGet: function (parametros) {

		var resultado;
		var configuracao = { url: null,
			data: null,
			type: 'POST',
			async: true,
			callBack: function (response) { if (!response.Ehvalido) { Mensagem.gerar(Simulador.container, response.Msg); } },
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			mostrarCarregando: false,
			serializar: true,
			container: Simulador.container
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
		if ($('.gridArquivosVetoriais tbody .hdnArquivoIdRelacionamento[value=0]', Simulador.container).length == 0) {
			$('.fsEnviarProjeto', Simulador.container).removeClass('hide');
		}
	},

	gerarenciarBotoes: function (options) {

		var settings = { avancar: false, refazer: false, recarregar: false, excluir: false, finalizar: false, salvar: false };

		$.extend(settings, options);

		$('.spanAvancar', Simulador.container).toggleClass('hide', !settings.avancar);
		$('.spanRefazer', Simulador.container).toggleClass('hide', !settings.refazer);
		$('.spanRecaregar', Simulador.container).toggleClass('hide', !settings.recarregar);
		$('.spanExcluir', Simulador.container).toggleClass('hide', !settings.excluir);
		$('.spanFinalizar', Simulador.container).toggleClass('hide', !settings.finalizar);
		$('.spnSalvar', Simulador.container).toggleClass('hide', !settings.salvar);
		$('.spanOuCancelar', Simulador.container).toggleClass('hide', !settings.salvar);
	},

	abrirModalMergeAtualizarDominio: function (textoModal) {

		Modal.confirma({
			removerFechar: true,
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				MasterPage.carregando(true);
				$.ajax({ url: Simulador.settings.urls.obterMerge,
					data: JSON.stringify({ id: $('.hdnProjetoId', Simulador.container).val() }),
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
							Simulador.settings.isObteveMerge = true;
							$('.txtEasting', Simulador.container).val(response.Projeto.MenorX);
							$('.txtNorthing', Simulador.container).val(response.Projeto.MaiorY);
							$('.txtEasting2', Simulador.container).val(response.Projeto.MaiorX);
							$('.txtNorthing2', Simulador.container).val(response.Projeto.MenorY);

							var containerBaseReferencia = $('.containerBaseReferencia', Simulador.container);
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
			titulo: Simulador.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	}
}

BaseReferencia = {
	settings: {
		projetoId: null,
		urls: {
			obterSituacaoArquivoRef: null,
			gerarArquivoVetorial: null,
			gerarArquivoMosaio: null,
			baixarArquivoVetorial: null,
			baixarArquivoOrtoMosaico: null,
			ValidarArquivoOrtoMosaico: null,
			baixarArquivoRef: null,
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
		$('.gridArquivosVetoriais', BaseReferencia.container).delegate('.btnRegerar', 'click', BaseReferencia.onGerarArquivoVetorial);

		$('.gridArquivosVetoriais', BaseReferencia.container).delegate('.btnDownloadModelo', 'click', BaseReferencia.onBaixarArquivoModelo);
		$('.gridArquivosVetoriais', BaseReferencia.container).delegate('.btnDownloadManual', 'click', BaseReferencia.onBaixarArquivoManual);
	},

	setarArquivoId: function (valor) {
		$('.hdnArquivoId', BaseReferencia.container).val(valor)
	},

	obterSituacao: function () {

		Simulador.mostrarEnvioProjeto();
		var listaItens = new Array();

		$('.gridArquivosVetoriais tbody tr', BaseReferencia.container).each(function (i, linha) {
			var situacaoId = +$('.hdnSituacaoId', linha).val();

			if ($.inArray(situacaoId, BaseReferencia.settings.situacoesValidas) != -1) {
				listaItens.push({ IdRelacionamento: $('.hdnArquivoIdRelacionamento', linha).val() });
			}
		});

		if (listaItens.length > 0) {
			Simulador.postGet({
				url: BaseReferencia.settings.urls.obterSituacaoArquivoRef,
				data: { arquivos: listaItens, projetoId: Simulador.obterId() },
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
				if ($('.hdnArquivoTipo', linha).val() == item.Tipo) {
					$('.hdnArquivoId', linha).val(item.Id);
					$('.txtSituacao', linha).text(item.SituacaoTexto);
					$('.hdnSituacaoId', linha).val(item.Situacao);
					$('.hdnArquivoIdRelacionamento', linha).val(item.IdRelacionamento);
					BaseReferencia.gerenciarBotoesArqVetoriais(linha, { gerar: item.MostrarGerar, baixar: item.MostrarBaixar, regerar: item.MostrarRegerar });
				}
			});
		});
	},

	stopThread: function () {
		clearInterval(BaseReferencia.settings.threadAtualizarSituacao);
		BaseReferencia.settings.threadAtualizarSituacao = null;
	},

	onBaixarArquivoOrtoMosaico: function () {
		var caminho = $(this).closest('tr').find('.hdnCaminho').val();
		var chave = $(this).closest('tr').find('.hdnChave').val();

		MasterPage.redireciona(BaseReferencia.settings.urls.baixarArquivoOrtoMosaico + "?chave=" + chave);
	},

	onGerarArquivoVetorial: function () {

		var tr = $(this).closest('tr');

		Simulador.postGet({
			url: BaseReferencia.settings.urls.gerarArquivoVetorial,
			data: {
				ProjetoId: Simulador.obterId(),
				IdRelacionamento: $('.hdnArquivoIdRelacionamento', tr).val(),
				Tipo: $('.hdnArquivoTipo', tr).val()
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
					gerar: resultado.arquivo.MostrarGerar,
					baixar: resultado.arquivo.MostrarBaixar,
					regerar: resultado.arquivo.MostrarRegerar
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
		MasterPage.redireciona(BaseReferencia.settings.urls.baixarArquivoVetorial + "/" + id);
	},

	onBaixarArquivoModelo: function () {
		MasterPage.redireciona(BaseReferencia.settings.urls.baixarArquivoRef + '?tipo=1');
	},

	onBaixarArquivoManual: function () {
		MasterPage.redireciona(BaseReferencia.settings.urls.baixarArquivoRef + '?tipo=2');
	}
}

EnviarProjeto = {
	settings: {
		urls: {
			EnviarArquivo: null,
			EnviarProcessar: null,
			baixarArquivos: null,
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

		var arquivoEnviado = $.parseJSON($('.hdnArquivo', EnviarProjeto.container).val());

		var data = {
			ProjetoId: Simulador.obterId()
		};

		Simulador.postGet({
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
			conteudo: Simulador.mensagens.ConfirmacaoReenviar.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	callBackReenviarArquivo: function (modal) {

		$('.hdnArquivo', EnviarProjeto.container).val('');
		$('.divSituacaoProjeto', EnviarProjeto.container).addClass("hide");
		$('.divEnviarProjeto', EnviarProjeto.container).removeClass("hide");
		$('.divImportarArquivo', EnviarProjeto.container).addClass('hide');

		$('.txtEasting', Simulador.container).removeClass('disabled').removeAttr('disabled');
		$('.txtNorthing', Simulador.container).removeClass('disabled').removeAttr('disabled');

		EnviarProjeto.limparArquivosGrid();
		Modal.fechar(modal);
	},

	mostrar: function (flag) {
		$('.divImportador', Simulador.container).toggleClass('hide', !flag);
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

		if (!$('.txtEasting', Simulador.container).val()) {
			erroMsg.push(EnviarProjeto.mensagens.EastingObrigatorio);
		}

		if (!$('.txtNorthing', Simulador.container).val()) {
			erroMsg.push(EnviarProjeto.mensagens.NorthingObrigatorio);
		}

		if (erroMsg.length > 0) {
			Mensagem.gerar(MasterPage.getContent(EnviarProjeto.container), erroMsg);
			return;
		}

		var inputFile = $('.divEnviarProjeto input[type="file"]', EnviarProjeto.container);
		inputFile.attr("id", "ArquivoId_1");
		FileUpload.upload(EnviarProjeto.settings.urls.EnviarArquivo, inputFile, EnviarProjeto.callBackArquivoEnviado);
	},

	callBackArquivoEnviado: function (controle, retorno) {

		var ret = $.parseJSON(retorno);

		ret.Arquivo.ProjetoId = Simulador.obterId();

		var data = {
			projetoVM:
			{
				Id: ret.Arquivo.ProjetoId,
				MecanismoElaboracaoId: $('.hdnSimuladorGeoMecanismoElaboracaoId', Simulador.container).val(),
				SituacaoId: $('.hdnSimuladorGeoSituacaoId', Simulador.container).val(),
				Easting: $('.txtEasting', Simulador.container).val(),
				Northing: $('.txtNorthing', Simulador.container).val(),
				ArquivoEnviado: ret.Arquivo
			}
		};

		if (ret.Arquivo != null && typeof ret.Arquivo != "undefined") {

			$('.hdnArquivo', EnviarProjeto.container).val(JSON.stringify(ret.Arquivo));
			$('.divEnviarProjeto', EnviarProjeto.container).addClass("hide");
			$('.divSituacaoProjeto', EnviarProjeto.container).removeClass("hide");

			Simulador.postGet({
				url: EnviarProjeto.settings.urls.EnviarProcessar,
				data: data,
				callBack: EnviarProjeto.callBackArquivoProcessamento, async: false
			});

		} else {
			$('.divEnviarProjeto .hdnArquivo', EnviarProjeto.container).val('');
			$('.divEnviarPeojeto input[type=file]').val('');
			if (ret.Msg.length > 0) {
				Mensagem.gerar(Simulador.container, ret.Msg);
			}
		}
	},

	callBackArquivoProcessamento: function (response) {

		if (!response.EhValido) {
			Mensagem.gerar(MasterPage.getContent(EnviarProjeto.container), response.Msg);
			$('.divEnviarProjeto', EnviarProjeto.container).removeClass("hide");
			$('.divSituacaoProjeto', EnviarProjeto.container).addClass("hide");
			return;
		}

		Mensagem.limpar(MasterPage.getContent(EnviarProjeto.container));

		$('.txtEasting', Simulador.container).addClass('disabled').attr('disabled', 'disabled');
		$('.txtNorthing', Simulador.container).addClass('disabled').attr('disabled', 'disabled');

		$('.divImportarArquivo', EnviarProjeto.container).removeClass("hide");
		$('.hdnArquivoEnviadoId', EnviarProjeto.container).val(response.Arquivo.IdRelacionamento);
		$('.lblSituacaoProcessamento', EnviarProjeto.container).text(response.Arquivo.SituacaoTexto);
		$('.hdnArquivoEnviadoSituacaoId', EnviarProjeto.container).val(response.Arquivo.Situacao);

		EnviarProjeto.obterSituacao();
	},

	obterSituacao: function () {

		var listaItens = new Array();

		var situacaoId = +$('.hdnArquivoEnviadoSituacaoId', EnviarProjeto.container).val();

		if ($.inArray(situacaoId, EnviarProjeto.settings.situacoesValidas) != -1) {
			listaItens.push({ IdRelacionamento: $('.hdnArquivoEnviadoId', EnviarProjeto.container).val() });
		}

		if (listaItens.length > 0) {
			Simulador.postGet({
				url: EnviarProjeto.settings.urls.obterSituacao,
				data: { arquivos: listaItens,
					projetoId: Simulador.obterId(),
					arquivoEnviadoTipo: $('.hdnArquivoEnviadoTipo', Simulador.container).val()
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
				EnviarProjeto.setarBotoes({ gerar: item.MostrarGerar, reenviar: item.MostrarReenviar, cancelar: item.MostrarCancelar });
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

			$('.gridImportarArquivos tbody:last').append(linha);
		});

		if (response.arquivosProcessados.length > 0) {
			$('.divImportarArquivo', EnviarProjeto.container).removeClass('hide');
		} else {
			$('.divImportarArquivo', EnviarProjeto.container).addClass('hide');
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

		var id = $(this).closest('tr').find('.hdnArquivoProcessadoId').val();

		MasterPage.redireciona(EnviarProjeto.settings.urls.baixarArquivos + "/" + id);
	},

	setarBotoes: function (botoes) {

		var configuracao = { reenviar: false, cancelar: false };

		$.extend(configuracao, botoes);

		$('.btnReenviarArquivo', EnviarProjeto.container).toggleClass('hide', !configuracao.reenviar);
		$('.btnCancelarProcessamento', EnviarProjeto.container).toggleClass('hide', !configuracao.cancelar);
	}
}
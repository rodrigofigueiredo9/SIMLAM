﻿/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Pessoa/inline.js" />
/// <reference path="../Empreendimento/inline.js" />

Requerimento = {
	urlObterReqInterEmp: null,
	salvarEdicao: false,
	salvarTelaAtual: null,
	stepAtual: 1,
	Mensagens: null,
	containerMensagem: null,
	container: null,
	ReqInterEmp: null,

	load: function (container) {
		Requerimento.container = container;
		Requerimento.containerMensagem = MasterPage.getContent(container);

		RequerimentoObjetivoPedido.configurarAssociarMultiploAtividade();
		Requerimento.configurarStepWizard();
		Requerimento.salvarTelaAtual = RequerimentoObjetivoPedido.onSalvarObjetivoPedido;

		container.delegate('.btnSalvar', 'click', Requerimento.gerenciarWizard);
		container.delegate('.btnAvancar', 'click', Requerimento.gerenciarWizard);
		container.delegate('.btnVoltar', 'click', Requerimento.gerenciarWizard);
		container.delegate('.btnFinalizar', 'click', RequerimentoFinalizar.onFinalizarRequerimento);

		container.delegate('.btnRoteiroPdf', 'click', RequerimentoObjetivoPedido.onBaixarPdfClick);

		container.delegate('.btnCarregarRoteiro', 'click', RequerimentoObjetivoPedido.onCarregarRoteiro);
		Listar.atualizarEstiloTable($('.tabRoteiros', Requerimento.container));

		if ($('#hdnRequerimentoId', Requerimento.container).val() != 0) {
			RequerimentoObjetivoPedido.carregouRoteiro = true;
			Requerimento.botoes({ btnEditar: true, spnCancelarCadastro: true });
			RequerimentoObjetivoPedido.configurarBtnEditar();
		} else {
			Requerimento.salvarEdicao = true;
		}
	},

	gerenciarWizardAbas: function () {

		if (Requerimento.stepAtual === +$(this).data('step')) {
			return;
		}

		if (Requerimento.stepAtual != 5 && Requerimento.salvarEdicao) {
			if (!Requerimento.salvarTelaAtual()) {
				return;
			}
		} else {
			Mensagem.limpar(Requerimento.containerMensagem);
		}

		MasterPage.carregando(true);

		$('.divFinalizar, .divPdf', Requerimento.container).addClass('hide');
		var objeto = Requerimento.gerarObjetoWizard();
		objeto.step = Requerimento.obterStep(this);

		Requerimento.switchGerenciarWizard(objeto);
	},

	gerarObjetoWizard: function () {
		var param = { id: $('#hdnRequerimentoId', Requerimento.container).val() };
		return { step: 0, params: param };
	},

	switchGerenciarWizard: function (objeto) {

		switch (objeto.step) {

			case 1:
				Requerimento.obterReqInterEmp(Requerimento.urlObterReqInterEmp, objeto.params);
				objeto.params.id = Requerimento.ReqInterEmp.requerimentoId;
				Requerimento.onObterStep(RequerimentoObjetivoPedido.urlObterObjetivoPedidoVisualizar, objeto.params, RequerimentoObjetivoPedido.callBackObterObjetivoPedidoVisualizar);

				break;

			case 2:
				Requerimento.obterReqInterEmp(Requerimento.urlObterReqInterEmp, objeto.params);
				objeto.params.id = Requerimento.ReqInterEmp.interessadoId;
				Requerimento.onObterStep(RequerimentoInteressado.urlObterInteressado, objeto.params, RequerimentoInteressado.callBackObterInteressado);

				break;

			case 3:
				Requerimento.onObterStep(RequerimentoResponsavel.urlObterResponsavelVisualizar, objeto.params, RequerimentoResponsavel.callBackObterResponsavelVisualizar);

				break;

			case 4:
				Requerimento.obterReqInterEmp(Requerimento.urlObterReqInterEmp, objeto.params);
				objeto.params.id = Requerimento.ReqInterEmp.empreendimentoId;
				Requerimento.onObterStep(RequerimentoEmpreendimento.urlObterEmpreendimento, objeto.params, RequerimentoEmpreendimento.callBackObterEmpreendimento);

				break;

			case 5:

				Requerimento.onObterStep(RequerimentoFinalizar.urlObterFinalizar, objeto.params, RequerimentoFinalizar.callBackObterFinalizar);
				break;
		}
	},

	gerenciarWizard: function () {

		if (Requerimento.stepAtual === +$(this).data('step')) {
			return;
		}

		if (Requerimento.stepAtual != 5) {
			if (!Requerimento.salvarTelaAtual()) {
				return;
			}
		} else {
			Mensagem.limpar(Requerimento.containerMensagem);
		}

		MasterPage.carregando(true);
		$('.divFinalizar, .divPdf', Requerimento.container).addClass('hide');

		var objeto = Requerimento.gerarObjetoWizard();
		objeto.step = Requerimento.obterStep(this);
		Requerimento.switchGerenciarWizard(objeto);
	},

	obterStep: function (container) {

		if ($(container).data('step') !== null && $(container).data('step') !== NaN && $(container).data('step') !== undefined) {
			return +$(container).data('step');
		}

		if ($(container).hasClass('btnVoltar')) {
			return Requerimento.stepAtual - 1;
		}

		if ($(container).hasClass('btnAvancar') || $(container).hasClass('btnSalvar')) {
			return Requerimento.stepAtual + 1;
		}
	},

	configurarStepWizard: function () {

		$('.AbasRequerimento ul li').each(function (i) {
			$(this).data("step", (i + 1));
			$(this).click(Requerimento.gerenciarWizardAbas);
		});
	},

	alternarAbas: function () {
		$('.AbasRequerimento .ui-tabs-selected').removeClass('ui-tabs-selected');
		$('.AbasRequerimento ul li').each(function () {
			if (parseInt($(this).data('step')) === Requerimento.stepAtual) {
				$(this).addClass('ui-tabs-selected');
				return;
			}
		});
	},

	onObterStep: function (urlStep, params, callBack) {
		$.ajax({ url: urlStep,
			type: "GET",
			data: params,
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Requerimento.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				$('.conteudoRequerimento', Requerimento.container).empty();
				$('.conteudoRequerimento', Requerimento.container).append(response);
				callBack();
			}
		});
	},

	onSalvarStep: function (url, objetoStep, msg) {

		var isSalvo = false;

		$.ajax({
			url: url,
			type: "POST",
			data: JSON.stringify(objetoStep),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Requerimento.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Requerimento.containerMensagem, response.Msg);
					return;
				}

				Mensagem.gerar(Requerimento.containerMensagem, msg);
				isSalvo = true;
			}
		});
		return isSalvo;
	},

	obterReqInterEmp: function (urlBuscar, params) {
		if (Requerimento.ReqInterEmp && Requerimento.ReqInterEmp.empreendimentoId != 0) {
			return;
		}

		$.ajax({ url: urlBuscar,
			type: "GET",
			data: params,
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Requerimento.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Requerimento.containerMensagem, response.Msg);
				}
				else {
					Requerimento.ReqInterEmp = response.Requerimento;
				}
			}
		});
	},

	botoes: function (botoes) {
		$('.btnSalvar', Requerimento.container).val('Salvar');
		
		$(".divSalvar", Requerimento.container).toggleClass('hide', typeof botoes.btnSalvar == 'undefined');
		$(".divEditar", Requerimento.container).toggleClass('hide', typeof botoes.btnEditar == 'undefined');
		$(".divIntNovo", Requerimento.container).toggleClass('hide', typeof botoes.btnIntAssNovo == 'undefined');
		$(".divEmpAvancar", Requerimento.container).toggleClass('hide', typeof botoes.btnEmpAvancar == 'undefined');
		$(".divEmpNovo", Requerimento.container).toggleClass('hide', typeof botoes.btnEmpAssNovo == 'undefined');
		$(".divFinalizar", Requerimento.container).toggleClass('hide', typeof botoes.btnFinalizar == 'undefined');
		$(".divVoltar", Requerimento.container).toggleClass('hide', typeof botoes.btnVoltar == 'undefined');
		$(".divPdf", Requerimento.container).toggleClass('hide', typeof botoes.btnPdf == 'undefined');
		$(".spnCancelarEdicao", Requerimento.container).toggleClass('hide', typeof botoes.spnCancelarEdicao == 'undefined');
		$(".spnCancelarCadastro", Requerimento.container).toggleClass('hide', typeof botoes.spnCancelarCadastro == 'undefined');
	},

	bloquearCampos: function (container) {
		$('.modoVisualizar', container).remove();
		$('.bloquear', container).attr('disabled', 'disabled').addClass('disabled');
	},

	configurarBtnCancelarStep: function (step) {
		Aux.scrollTop(Requerimento.container);
		$('.linkCancelar', Requerimento.container).unbind('click');
		$(".linkCancelar", Requerimento.container).click(function () {
			var objeto = Requerimento.gerarObjetoWizard();
			objeto.step = step;
			Requerimento.switchGerenciarWizard(objeto);
		});
	}
}

RequerimentoObjetivoPedido = {

	urlAtividadeSolicitada: null,
	atividadeSolicitadaLink: null,
	visualizarRoteiroModalLink: null,
	urlObterObjetivoPedido: null,
	urlCriarObjetivoPedido: null,
	carregouRoteiro: false,
	urlBaixarPdf: null,
	urlObterRoteirosAtividade: null,

	configurarBtnEditar: function () {
		$(".btnEditar", Requerimento.container).unbind('click');
		$(".btnEditar", Requerimento.container).click(RequerimentoObjetivoPedido.onBtnEditar);
	},

	onBtnEditar: function () {

		MasterPage.carregando(true);

		var param = { id: $('#hdnRequerimentoId').val() };

		Requerimento.obterReqInterEmp(Requerimento.urlObterReqInterEmp, param);
		param.id = Requerimento.ReqInterEmp.requerimentoId;
		Requerimento.onObterStep(RequerimentoObjetivoPedido.urlObterObjetivoPedido, param, RequerimentoObjetivoPedido.callBackObterObjetivoPedido);
	},

	callBackObterObjetivoPedidoVisualizar: function () {
		Requerimento.salvarEdicao = false;
		Requerimento.botoes({ btnEditar: true, spnCancelarCadastro: true });
		RequerimentoObjetivoPedido.callBackObterObjetivoPedidoDefault();
		RequerimentoObjetivoPedido.configurarBtnEditar();
	},

	callBackObterObjetivoPedido: function () {
		Requerimento.salvarEdicao = true;
		Requerimento.botoes({ btnSalvar: true, spnCancelarEdicao: true });
		RequerimentoObjetivoPedido.callBackObterObjetivoPedidoDefault();
	},

	callBackObterObjetivoPedidoDefault: function () {
		RequerimentoObjetivoPedido.configurarAssociarMultiploAtividade();
		Requerimento.stepAtual = 1;
		Requerimento.salvarTelaAtual = RequerimentoObjetivoPedido.onSalvarObjetivoPedido;
		RequerimentoObjetivoPedido.atividadeSolicitadaExpansivel();
		Requerimento.alternarAbas();
		Requerimento.configurarBtnCancelarStep(1);
		MasterPage.botoes(Requerimento.container);

		MasterPage.carregando(false);
	},

	atividadeSolicitadaExpansivel: function () {
		$('.asmConteudoInternoExpander', $('.divConteudoAtividadeSolicitada .asmItens')).addClass('asmExpansivel');
	},

	associarAtividadeSolicitada: function (atividade, item, extra) {

		var mensagens = new Array();
		var jaExiste = false;
		var atividadeOutroSetor = false;
		var qtdAtividades = $('.asmItemContainer', $(item).closest('.asmItens')).length;


		$('.asmItemContainer', $(item).closest('.asmItens')).each(function () {
			if ($('.hdnAtividadeId', this).val() != 0) {

				if (!jaExiste && ($('.hdnAtividadeId', this).val() == atividade.Id)) {
					mensagens.push(Requerimento.Mensagens.AtividadejaAdicionada);
					jaExiste = true;
				}

				if (!atividadeOutroSetor && qtdAtividades > 1 && ($('.hdnAtividadeSetorId', this).val() != atividade.SetorId)) {
					mensagens.push(Requerimento.Mensagens.AtividadesSetoresDiferentes);
					atividadeOutroSetor = true;
				}
			}
		});

		if (mensagens.length > 0) {
			return mensagens;
		}

		$('.listaObjetos', item).empty();

		$('.nomeAtividade', item).val(atividade.Nome);
		$('.asmItemTexto', item).val(atividade.Nome);
		$('.hdnAtividadeId', item).val(atividade.Id);
		$('.hdnAtividadeSetorId', item).val(atividade.SetorId);

		$('.divFinalidadeConteudo', item).removeClass('hide');
		$('.listaObjetos ul li', item).empty();

		var linha = $('.templateFinalidade:first', item).clone();

		linha.addClass('Nenhum');
		$('.finalidadeTexto', linha).text('Não existe finalidade adicionada.');
		$('.tituloModeloTexto', linha).text('Não existe título adicionado.');
		$('.divDetalhes', linha).remove();
		$('.btnExcluirAtividade', linha).remove();
		linha.removeClass('hide templateFinalidade');
		$('.listaObjetos', item).append(linha);
		RequerimentoObjetivoPedido.carregouRoteiro = false;
	},

	onCallBackTituloAssociado: function () {
		RequerimentoObjetivoPedido.carregouRoteiro = false;
	},

	configurarAssociarMultiploAtividade: function () {

		$('.divConteudoAtividadeSolicitada').associarMultiplo({
			'associarModalLoadFunction': 'AtividadeSolicitadaListar.load',
			'associarUrl': RequerimentoObjetivoPedido.urlAtividadeSolicitada,
			'onAssociar': RequerimentoObjetivoPedido.associarAtividadeSolicitada,
			'expandirAutomatico': false,
			'minItens': 1,
			'tituloExcluir': 'Remover Atividade Solicitada',
			'btnOkLabelExcluir': 'Remover',
			'msgExcluir': RequerimentoObjetivoPedido.onMensagemExcluirAtividade,
			'msgObrigatoriedade': Requerimento.Mensagens.AtividadeObrigatorio,
			'tamanhoModal': Modal.tamanhoModalGrande
		});
	},

	onMensagemExcluirAtividade: function (item, extra) {
		return 'Tem certeza que deseja remover a atividade ' + $(item).find('.asmItemTexto').val() + ' do requerimento?';
	},

	onCarregarRoteiro: function () {

		Mensagem.limpar(Requerimento.containerMensagem);

		function atividadeSolicitada() {
			this.Id = 0;
			this.NomeAtividade = '';
			this.IdRelacionamento = 0;
			this.Finalidades = [];
		};

		var atividadesSolicitadas = [];

		$('.divConteudoAtividadeSolicitada .asmItens .asmItemContainer').each(function (i, itemAtividade) {
			if (!$('.nomeAtividade', itemAtividade).val()) {
				return;
			}

			var atividade = new atividadeSolicitada();

			atividade.Id = $('.hdnAtividadeId', itemAtividade).val();
			atividade.IdRelacionamento = $('.hdnAtividadeRelId', itemAtividade).val();
			atividade.NomeAtividade = $('.nomeAtividade', itemAtividade).val();
			atividade.SetorId = $('.hdnAtividadeSetorId', itemAtividade).val();

			var finalidades = new Array();

			$('.listaObjetos li', itemAtividade).each(function (i, item) {

				var finalidade = {};

				finalidade.Id = $('.hdnfinalidadeId', item).val();
				finalidade.IdRelacionamento = $('.hdnIdRelacionamento', item).val();
				finalidade.TituloModelo = $('.hdnTituloModeloId', item).val();
				finalidade.TituloModeloAnterior = $('.hdnModeloTituloAnterior', item).val();
				finalidade.NumeroDocumentoAnterior = $('.numeroDocumentoAnterior', item).text();

				finalidades.push(finalidade);
			});

			atividade.Finalidades = finalidades;
			atividadesSolicitadas.push(atividade);
		});

		$.ajax({
			url: RequerimentoObjetivoPedido.urlObterRoteirosAtividade,
			type: "POST",
			data: JSON.stringify({ AtividadesSolicitadas: atividadesSolicitadas }),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Requerimento.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(MasterPage.getContent(Requerimento.container), response.Msg);
					return;
				}

				$('.tabRoteiros > tbody tr').remove();

				$(response.Lista).each(function (i, item) {

					var linha = $('.trRoteiroTemplate').clone().removeClass('trRoteiroTemplate');

					linha.find('.hdnRoteiroId').val(item.Id);
					linha.find('.hdnTidRoteiro').val(item.Tid);
					linha.find('.trRoteiroNumero').text(item.Numero);
					linha.find('.trRoteiroNome').text(item.Nome);
					linha.find('.trRoteiroVersao').text(item.VersaoAtual);
					linha.find('.trRoteiroAtividade').text(item.AtividadeTexto);

					linha.find('.trRoteiroNumero').attr('title', item.Numero);
					linha.find('.trRoteiroNome').attr('title', item.Nome);
					linha.find('.trRoteiroVersao').attr('title', item.VersaoAtual);
					linha.find('.trRoteiroAtividade').attr('title', item.AtividadeTexto);

					$('.tabRoteiros > tbody:last').append(linha);
				});
				Listar.atualizarEstiloTable($('.tabRoteiros'));
				RequerimentoObjetivoPedido.carregouRoteiro = true;
			}
		});
	},

	onBaixarPdfClick: function () {
		var id = $(this).closest('td').find('.hdnRoteiroId').val();
		MasterPage.redireciona(RequerimentoObjetivoPedido.urlBaixarPdf + '/' + id);
	},

	onSalvarObjetivoPedido: function () {

		var isSalvo = false;
		var msg = new Array();

		function atividadeSolicitada() {
			this.Id = 0;
			this.NomeAtividade = '';
			this.IdRelacionamento = 0;
			this.Finalidades = [];
		};

		var objetivoPedido = {
			Id: '',
			DataCriacao: '',
			Roteiros: [],
			AtividadesSolicitadas: [],
			SetorId: 0,
			InformacaoComplementar: ''
		};

		objetivoPedido.Id = +$('#hdnRequerimentoId', Requerimento.container).val();
		objetivoPedido.DataCriacao = $('.dataCriacao', Requerimento.container).val();
		objetivoPedido.AgendamentoVistoriaId = $('.ddlAgendamentoVistoria', Requerimento.container).val();
		objetivoPedido.SetorId = $('.ddlSetores', Requerimento.container).val();
		objetivoPedido.InformacaoComplementar = $('.txtInformacaoComplementar', Requerimento.container).val();
		objetivoPedido.AtividadesSolicitadas = AtividadeSolicitadaAssociar.gerarObjeto(Requerimento.container);

		if (objetivoPedido.AtividadesSolicitadas.length < 1) {
			msg.push(Requerimento.Mensagens.DeveExitirAtividade);
		}

		if (!RequerimentoObjetivoPedido.carregouRoteiro) {
			msg.push(Requerimento.Mensagens.CarregarRoteiroObrigatorio);
		}

		if (msg.length > 0) {
			Mensagem.gerar(Requerimento.containerMensagem, msg);
			return;
		}

		$('.tabRoteiros tbody tr', Requerimento.container).each(function () {
			objetivoPedido.Roteiros.push({ Id: $('.hdnRoteiroId', this).val(), Tid: $('.hdnTidRoteiro', this).val() });
		});

		$.ajax({
			url: RequerimentoObjetivoPedido.urlCriarObjetivoPedido,
			type: "POST",
			data: JSON.stringify(objetivoPedido),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Requerimento.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Requerimento.container), response.Msg);
					return;
				}

				var arrayMensagem = new Array();

				if (objetivoPedido.Id === 0) {

					$('#hdnRequerimentoId').val(response.id);

					var objetoMensagem = Requerimento.Mensagens.RequerimentoSalvar;

					objetoMensagem.Texto = objetoMensagem.Texto.replace("#id#", response.id);

					arrayMensagem.push(objetoMensagem);

				} else {
					arrayMensagem.push(Requerimento.Mensagens.RequerimentoEditar);
				}
				Mensagem.gerar(MasterPage.getContent(Requerimento.container), arrayMensagem);
				isSalvo = true;
			}
		});
		return isSalvo;
	}
}

RequerimentoInteressado = {
	urlObterInteressado: null,
	urlAssociarInteressado: null,
	urlLimparInteressado: null,
	urlAlterarSituacao: null,
	pessoaInLineObj: null,

	callBackObterInteressado: function () {

		RequerimentoInteressado.pessoaInLineObj = new PessoaInline();

		RequerimentoInteressado.pessoaInLineObj.load(Requerimento.container, {
			onVisualizarEnter: RequerimentoInteressado.onVisualizarEnterInteressado,
			onEditarEnter: RequerimentoInteressado.onEditarEnterInteressado,
			onVerificarEnter: RequerimentoInteressado.onVerificarEnterInteressado,
			onCriarEnter: RequerimentoInteressado.onCriarEnterInteressado
		});

		Requerimento.stepAtual = 2;
		Requerimento.salvarTelaAtual = RequerimentoInteressado.SalvarInteressado;
		Requerimento.alternarAbas();

		Mascara.load(Requerimento.container);
		MasterPage.botoes(Requerimento.container);

		if (Requerimento.ReqInterEmp && Requerimento.ReqInterEmp.interessadoId > 0) {
			RequerimentoInteressado.pessoaInLineObj.onVisualizarEnter();

			$(".btnEditar", Requerimento.container).unbind('click');
			$(".btnEditar", Requerimento.container).click(function () {
				RequerimentoInteressado.pessoaInLineObj.onBtnEditarClick();
				Requerimento.salvarEdicao = true;
				Aux.scrollTop(Requerimento.container);
			});
		} else {
			Requerimento.botoes({ btnSalvar: true, spnCancelarCadastro: true });
		}

		Requerimento.salvarEdicao = false;
		$('.pessoaf', Requerimento.container).focus();
		MasterPage.carregando(false);
	},

	SalvarInteressado: function () {
		var interessadoValido = false;
		var param = { requerimentoId: null, interessadoId: null };

		param.interessadoId = RequerimentoInteressado.pessoaInLineObj.onSalvarClick();
		param.requerimentoId = $('#hdnRequerimentoId').val();

		if (param.interessadoId == -1) {

			var mensagem = [];

			if ($('.pessoaf').attr('checked')) {
				mensagem = new Array(Requerimento.Mensagens.CpfObrigatorio);
			} else {
				mensagem = new Array(Requerimento.Mensagens.CnpjObrigatorio);
			}

			Mensagem.gerar(Requerimento.containerMensagem, mensagem);
			return false;
		}

		//Caso tenha verificado e não foi feito cadastro
		if (param.interessadoId == 0) {
			return false;
		}

		if (Requerimento.ReqInterEmp == null ||
			(typeof (Requerimento.ReqInterEmp.interessadoId) !== 'undefined' && Requerimento.ReqInterEmp.interessadoId !== param.interessadoId)) {

			var variavelSohPraSegurarAExecucaoDoJavascript = $.ajax({
				url: RequerimentoInteressado.urlAssociarInteressado,
				type: "POST",
				data: JSON.stringify(param),
				dataType: "json",
				contentType: "application/json; charset=utf-8",
				cache: false,
				async: false,
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, Requerimento.container);
				},
				success: function (response, textStatus, XMLHttpRequest) {
					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(Requerimento.containerMensagem, response.Msg);
					}

					interessadoValido = response.EhValido;					
				}
			});
			if (interessadoValido) {
				Requerimento.ReqInterEmp['interessadoId'] = param.interessadoId;
			}
		} else {

			if (!RequerimentoInteressado.atualizarSituacaoRequerimento({ requerimentoId: param.requerimentoId })) {
				return false;
			} else {
				interessadoValido = true;
			}

			Mensagem.gerar(Requerimento.containerMensagem, new Array(Requerimento.Mensagens.InteressadoEditado));
		}
		return interessadoValido;
	},

	atualizarSituacaoRequerimento: function (data) {

		var ehValido = true;

		$.ajax({
			url: RequerimentoInteressado.urlAlterarSituacao,
			type: "POST",
			data: JSON.stringify(data),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Requerimento.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (!response.EhValido) {
					Mensagem.gerar(Requerimento.containerMensagem, response.Msg);
					ehValido = false;
				}
			}
		});
		return ehValido;
	},

	onCriarEnterInteressado: function () {
		Requerimento.configurarBtnCancelarStep(2);
		$(".btnIntAssNovo", Requerimento.container).unbind('click');
		$(".btnIntAssNovo", Requerimento.container).click(RequerimentoInteressado.pessoaInLineObj.onBtnLimparClick);
		Requerimento.botoes({ btnSalvar: true, btnIntAssNovo: true, spnCancelarCadastro: true });
	},

	onVisualizarEnterInteressado: function () {
		RequerimentoInteressado.configurarVisualizar();
	},

	onEditarEnterInteressado: function () {
		if (Requerimento.ReqInterEmp && Requerimento.ReqInterEmp.interessadoId > 0) {
			Requerimento.configurarBtnCancelarStep(2);
			Requerimento.salvarEdicao = true;
		} else {
			Requerimento.salvarEdicao = false;
			$('.linkCancelar', Requerimento.container).unbind('click');
			$('.linkCancelar', Requerimento.container).click(function () {
				RequerimentoInteressado.obterInteressado($('.pessoaId', Requerimento.container).val());
			});
		}
		Requerimento.botoes({ btnSalvar: true, spnCancelarEdicao: true });
	},

	configurarVisualizar: function () {
		var btnSalvarVisivel = (!Requerimento.ReqInterEmp || !Requerimento.ReqInterEmp.interessadoId || Requerimento.ReqInterEmp.interessadoId <= 0);

		$(".btnEditar", Requerimento.container).unbind('click');
		$(".btnEditar", Requerimento.container).click(function () {
			RequerimentoInteressado.pessoaInLineObj.onBtnEditarClick();
			Requerimento.salvarEdicao = true;
			Aux.scrollTop(Requerimento.container);
			$('.btnSalvar', Requerimento.container).val('Salvar');
		});

		$(".btnIntAssNovo", Requerimento.container).unbind('click');
		$(".btnIntAssNovo", Requerimento.container).click(RequerimentoInteressado.pessoaInLineObj.onBtnLimparClick);
		Requerimento.configurarBtnCancelarStep(2);
		if (Requerimento.ReqInterEmp && Requerimento.ReqInterEmp.interessadoId > 0) {
			Requerimento.botoes({ btnEditar: true, btnIntAssNovo: true, spnCancelarCadastro: true });

		} else {
			Requerimento.botoes({ btnSalvar: true, btnEditar: true, btnIntAssNovo: true, spnCancelarCadastro: true });
			Requerimento.salvarEdicao = true;
			$('.btnSalvar', Requerimento.container).val('Associar');
		}
	},

	onVerificarEnterInteressado: function () {
		Requerimento.ReqInterEmp.interessadoId = 0;
		Requerimento.botoes({ btnSalvar: true, spnCancelarCadastro: true });
		Requerimento.configurarBtnCancelarStep(2);
	},

	obterInteressado: function (interessadoId) {
		Requerimento.onObterStep(RequerimentoInteressado.urlObterInteressado, { id: interessadoId }, function () {
			RequerimentoInteressado.callBackObterInteressado();
			RequerimentoInteressado.configurarVisualizar();
		});
	}
}

RequerimentoResponsavel = {

	urlAssociarResponsavelModal: null,
	urlAssociarResponsavelEditarModal: null,
	urlObterResponsavel: null,
	urlExcluirResponsavel: null,
	urlObterResponsavelVisualizar: null,
	urlCriarResponsavel: null,
	mostrarBtnEditar: false,

	pessoaModalResp: null,


	callBackObterResponsavelVisualizar: function () {

		$('.btnSalvar', Requerimento.container).val('Salvar');
		Requerimento.botoes({ btnEditar: true, spnCancelarCadastro: true });
		Requerimento.stepAtual = 3;
		Requerimento.salvarEdicao = false;
		RequerimentoResponsavel.mostrarBtnEditar = false;
		Requerimento.salvarTelaAtual = RequerimentoResponsavel.onSalvarResponsavelTecnico;
		Requerimento.alternarAbas();

		if ($('.hdnResonsavelVisualizar', Requerimento.container).val() == 'false') {
			Requerimento.botoes({ btnSalvar: true, spnCancelarCadastro: true });
			RequerimentoResponsavel.mostrarBtnEditar = true;
		}

		$(".btnEditar", Requerimento.container).unbind('click');
		$(".btnEditar", Requerimento.container).click(RequerimentoResponsavel.onBtnEditar);

		RequerimentoResponsavel.configurarAssociarMultiploResponsavel();
		MasterPage.carregando(false);
	},

	callBackObterResponsavel: function () {

		Requerimento.stepAtual = 3;
		Requerimento.salvarEdicao = true;
		RequerimentoResponsavel.mostrarBtnEditar = true;
		RequerimentoResponsavel.configurarAssociarMultiploResponsavel();
		Requerimento.salvarTelaAtual = RequerimentoResponsavel.onSalvarResponsavelTecnico;
		Requerimento.alternarAbas();

		Requerimento.botoes({ btnSalvar: true, spnCancelarEdicao: true });

		Requerimento.configurarBtnCancelarStep(3);
		MasterPage.carregando(false);
	},

	onBtnEditar: function () {
		MasterPage.carregando(true);
		var param = { id: $('#hdnRequerimentoId').val() };
		Requerimento.onObterStep(RequerimentoResponsavel.urlObterResponsavel, param, RequerimentoResponsavel.callBackObterResponsavel);
	},

	configurarAssociarMultiploResponsavel: function () {

		RequerimentoResponsavel.pessoaModal = new PessoaAssociar();

		$('.divConteudoResponsavelTec', Requerimento.container).associarMultiplo({

			'associarModalObject': RequerimentoResponsavel.pessoaModal,
			'associarModalLoadFunction': RequerimentoResponsavel.pessoaModal.load,
			'associarModalLoadParams': {
				tituloVerificar: 'Verificar Responsável',
				tituloCriar: 'Cadastrar Responsável Técnico',
				tituloEditar: 'Editar Responsável Técnico',
				tituloVisualizar: 'Visualizar Responsável Técnico',
				editarVisualizar: RequerimentoResponsavel.mostrarBtnEditar,
				urls: {
					visualizarModal: RequerimentoResponsavel.urlAssociarResponsavelEditarModal
				}
			},

			'editarModalObject': RequerimentoResponsavel.pessoaModal,
			'editarModalLoadFunction': RequerimentoResponsavel.pessoaModal.load,
			'editarModalLoadParams': {
				tituloVerificar: 'Verificar Responsável',
				tituloCriar: 'Cadastrar Responsável Técnico',
				tituloEditar: 'Editar Responsável Técnico',
				tituloVisualizar: 'Visualizar Responsável Técnico',
				editarVisualizar: RequerimentoResponsavel.mostrarBtnEditar,
				urls: {
					visualizarModal: RequerimentoResponsavel.urlAssociarResponsavelEditarModal
				}
			},
			'associarUrl': RequerimentoResponsavel.urlAssociarResponsavelModal,
			'editarUrl': RequerimentoResponsavel.urlAssociarResponsavelEditarModal,

			'onEditar': RequerimentoResponsavel.onResponsavelEditar,
			'onEditarClick': RequerimentoResponsavel.onResponsavelEditarClick,
			'msgObrigatoriedade': Requerimento.Mensagens.ResponsavelObrigatorio,
			'btnOkLabelExcluir': 'Remover',
			'tituloExcluir': 'Remover Responsável Técnico',
			'msgExcluir': RequerimentoResponsavel.onMensagemExcluirResponsavel,
			'mostrarBtnLimpar': true,

			'onAssociar': RequerimentoResponsavel.associarResponsavelTecnico,
			'expandirAutomatico': false,
			'minItens': 1,
			'tamanhoModal': Modal.tamanhoModalGrande,
			'onLimparClick': RequerimentoResponsavel.onResponsavelLimpar
		});
	},

	onResponsavelLimpar: function (item, extra) {
		$('.asmItemId, select', item).val(0);
		$('.asmItemTexto', item).val('');
		$('input[type=text]', item).val('');
		$('cpfCnpj', item).val('');
		$('.asmConteudoLink', item).addClass('hide');
		$('.btnAsmEditar', item).hide().addClass('hide');
		$('.hdnResponsavelId', item).val('');
		$('.nomeRazao', item).val('');
		$(item).closest('asmItemContainer').removeClass('editando');

		if ($(".spnCancelarEdicao", Requerimento.container).hasClass('hide')) {
			Requerimento.salvarEdicao = false;
			$('.divConteudoResponsavelTec .asmItens .asmItemContainer').each(function (i, item) {
				if ($('.asmItemId', item).val() != '' && $('.asmItemId', item).val() != 0) {
					Requerimento.salvarEdicao = true;
					return;
				}
			});
		}
	},

	associarResponsavelTecnico: function (responsavel, item, extra) {

		var jaExiste = false;

		$('.asmItemContainer', $(item).closest('.asmItens')).each(function () {
			if ($('.hdnResponsavelId', this).val() == responsavel.Id) {
				jaExiste = true;
				return;
			}
		});

		if (jaExiste) {
			return new Array(Requerimento.Mensagens.ResponsaveljaAdicionado);
		}

		$('.hdnResponsavelId', item).val(responsavel.Id);
		$('.nomeRazao', item).val(responsavel.NomeRazaoSocial);
		$('.cpfCnpj', item).val(responsavel.CPFCNPJ);
		$('.btnAsmEditar', item).show();
		Requerimento.salvarEdicao = true;
		Requerimento.configurarBtnCancelarStep(3);
		return true;
	},

	onResponsavelEditarClick: function (container) {
		$(container).addClass('editando');
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

	onResponsavelEditar: function (pessoaObj, item, extra) {
		var divContainer = $(item).closest('.divConteudoResponsavelTec');
		var divItens = $('.asmItens', divContainer);
		var erroMsg = new Array();

		if (RequerimentoResponsavel.existeAssociadoEdicao(pessoaObj.Id.toString(), divItens, 'hdnResponsavelId')) {
			erroMsg.push(Requerimento.Mensagens.ResponsaveljaAdicionado);
			return erroMsg;
		}

		$('.hdnResponsavelId', item).val(pessoaObj.Id);
		$('.nomeRazao', item).val(pessoaObj.NomeRazaoSocial);
		$('.cpfCnpj', item).val(pessoaObj.CPFCNPJ);
		$(item).closest('asmItemContainer').removeClass('editando');
	},

	sair: function () {
		var sair = true;
		$('.divConteudoResponsavelTec .asmItens .asmItemContainer').each(function (i, item) {
			if ($('.asmItemId', item).val() != 0) {
				sair = false;
				return;
			}
		});
		return sair;
	},

	onSalvarResponsavelTecnico: function () {

		var responsaveis = $('.divConteudoResponsavelTec .asmItens .asmItemContainer');

		if (RequerimentoResponsavel.sair()) {
			RequerimentoResponsavel.onExcluirTodosResponsaveis({ id: $('#hdnRequerimentoId').val() });
			return true;
		}

		function responsavelTecnico() {
			this.Id = 0;
			this.NomeRazao = '';
			this.CpfCnpj = 0;
			this.Funcao = 0;
			this.NumeroArt = '';
			this.IdRelacionamento = 0;
		};

		var objetoResponsaveis = new Array();

		responsaveis.each(function () {

			var responsavel = new responsavelTecnico();

			responsavel.Id = $('.hdnResponsavelId', this).val();
			responsavel.NomeRazao = $('.nomeRazao', this).val();
			responsavel.CpfCnpj = $('.cpfCnpj', this).val();
			responsavel.Funcao = $('.funcao', this).val();
			responsavel.NumeroArt = $('.art', this).val();
			responsavel.IdRelacionamento = $('.hdnIdRelacionamento', this).val();

			objetoResponsaveis.push(responsavel);
		});

		RequerimentoResponsavel.indexarResponsaveis();

		var arrayMensagem = new Array();

		arrayMensagem.push(Requerimento.Mensagens.ResponsavelSalvar);

		var params = { id: $('#hdnRequerimentoId').val(), responsaveis: objetoResponsaveis };

		return Requerimento.onSalvarStep(RequerimentoResponsavel.urlCriarResponsavel, params, arrayMensagem);
	},

	indexarResponsaveis: function () {
		$('.asmItemContainer', Requerimento.container).each(function (index, item) {
			$(item).find('.nomeRazao').attr('id', 'resp_' + index + '__NomeRazao');
			$(item).find('.cpfCnpj').attr('id', 'resp_' + index + '__cpfCnpj');
			$(item).find('.funcao').attr('id', 'resp_' + index + '__funcao');
			$(item).find('.art').attr('id', 'resp_' + index + '__art');
		});
	},

	onMensagemExcluirResponsavel: function (item, extra) {
		return Mensagem.replace(Requerimento.Mensagens.ResponsavelExcluir, '#texto', $(item).find('.asmItemTexto').val()).Texto;
	},

	onExcluirTodosResponsaveis: function (objeto) {
		$.ajax({
			url: RequerimentoResponsavel.urlExcluirResponsavel,
			type: "POST",
			data: JSON.stringify(objeto),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Requerimento.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (!response.EhValido) {
					Mensagem.gerar(Requerimento.containerMensagem, response.Msg);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Requerimento.containerMensagem, response.Msg);
				}
			}
		});
	}
}

RequerimentoEmpreendimento = {

	urlObterEmpreendimento: null,
	urlAssociarEmpreendimento: null,
	salvarEmpreendimento: false,
	desassociarEmp: false,
	filtros: {},

	callBackObterEmpreendimento: function () {

		EmpreendimentoInline.load(Requerimento.container, {
			onIdentificacaoEnter: RequerimentoEmpreendimento.onIdentificacaoEnterEmpreendimento,
			onVisualizarEnter: RequerimentoEmpreendimento.onVisualizarEnterEmpreendimento,
			onEditarEnter: RequerimentoEmpreendimento.onEditarEnterEmpreendimento,
			onNovoEnter: RequerimentoEmpreendimento.onNovoEmpreendimentoEnter,
			obterEnderecoPessoaAssociada: RequerimentoEmpreendimento.obterEnderecoPessoaAssociada
		});

		Requerimento.stepAtual = 4;
		Requerimento.salvarTelaAtual = RequerimentoEmpreendimento.onSalvarEmpreendimento;
		Requerimento.alternarAbas();

		if (Requerimento.ReqInterEmp && parseInt(Requerimento.ReqInterEmp.empreendimentoId) > 0) {
			EmpreendimentoInline.modo = 2;
			RequerimentoEmpreendimento.onVisualizarEnterEmpreendimento();
		} else {
			RequerimentoEmpreendimento.onIdentificacaoEnterEmpreendimento();
		}

		$(".btnEmpAvancar", Requerimento.container).unbind('click');
		$('.btnEmpAvancar', Requerimento.container).click(EmpreendimentoInline.onAvancarEnter);

		$(".btnEmpAssNovo", Requerimento.container).unbind('click');
		$('.btnEmpAssNovo', Requerimento.container).click(RequerimentoEmpreendimento.onNovoEmpreendimentoClick);

		MasterPage.carregando(false);
	},

	obterEnderecoPessoaAssociada: function (origemId, empreendimentoId) {
		var requerimento = $('#hdnRequerimentoId').val();
		var container = Requerimento.container;

		$.ajax({
			url: Empreendimento.settings.urls.obterListaPessoasAssociada,
			data: JSON.stringify({ tipo: origemId, empreendimentoId: empreendimentoId, requerimentoId: requerimento}),
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
	},

	onSalvarEmpreendimento: function (partialContent, responseJson, isEditar) {

		var retorno = false;
		var param = { requerimentoId: null, empreendimentoId: null };

		param.requerimentoId = $('#hdnRequerimentoId').val();

		if (RequerimentoEmpreendimento.salvarEmpreendimento) {
			param.empreendimentoId = EmpreendimentoInline.onSalvarClick();

			if (param.empreendimentoId == 0) {
				return retorno;
			}
		} else {
			param.empreendimentoId = EmpreendimentoInline.obterIdEmpreendimento();
			if (typeof param.empreendimentoId == "undefined" || (Requerimento.ReqInterEmp &&
				typeof Requerimento.ReqInterEmp.empreendimentoId != "undefined" &&
				parseInt(param.empreendimentoId) == Requerimento.ReqInterEmp.empreendimentoId)) {
				if (RequerimentoEmpreendimento.desassociarEmp) {
					param.empreendimentoId = 0;
					RequerimentoEmpreendimento.desassociarEmp = false;
				} else {
					return true;
				}
			}
		}

		$.ajax({
			url: RequerimentoEmpreendimento.urlAssociarEmpreendimento,
			type: "POST",
			data: JSON.stringify(param),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Requerimento.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Requerimento.containerMensagem, response.Msg);
				}
				retorno = response.empAssociado;
			}
		});

		Requerimento.ReqInterEmp['empreendimentoId'] = param.empreendimentoId;
		return retorno;
	},

	onNovoEmpreendimentoEnter: function () {
		Requerimento.salvarEdicao = false;
		Requerimento.botoes({ btnSalvar: true, btnEmpAssNovo: true, spnCancelarCadastro: true });
	},

	onIdentificacaoEnterEmpreendimento: function () {
		Requerimento.salvarEdicao = false;
		RequerimentoEmpreendimento.salvarEmpreendimento = false;
		Requerimento.botoes({ btnEmpAvancar: true, spnCancelarCadastro: true });
		RequerimentoEmpreendimento.onConfigurarLayout();
	},

	onConfigurarLayout: function () {
		$('.filtroSerializarAjax', Requerimento.container).wrap('<fieldset class="box fieldset"></fieldset>');
		$('.filtroSerializarAjax', Requerimento.container).before('<legend>Identificação do empreendimento</legend>');
		$('.divFiltros, .divEstiloCnpj', Requerimento.container).removeClass('box');
	},

	onVisualizarEnterEmpreendimento: function () {

		if (Requerimento.ReqInterEmp && parseInt(Requerimento.ReqInterEmp.empreendimentoId) > 0) {
			Requerimento.botoes({ btnEditar: true, btnEmpAssNovo: true, spnCancelarCadastro: true });
		} else {
			Requerimento.botoes({ btnSalvar: true, btnEditar: true, btnEmpAssNovo: true, spnCancelarCadastro: true });
			$('.btnSalvar', Requerimento.container).val('Associar');
		}

		$(".btnEditar", Requerimento.container).unbind('click');
		$(".btnEditar", Requerimento.container).click(EmpreendimentoInline.onBtnEditarClick);

		$(".btnEmpAssNovo", Requerimento.container).unbind('click');
		$(".btnEmpAssNovo", Requerimento.container).click(RequerimentoEmpreendimento.onNovoEmpreendimentoClick);
		Requerimento.salvarEdicao = false;
		RequerimentoEmpreendimento.salvarEmpreendimento = false;
		Requerimento.configurarBtnCancelarStep(4);
	},

	onEditarEnterEmpreendimento: function () {

		RequerimentoEmpreendimento.salvarEmpreendimento = true;
		Requerimento.salvarEdicao = true;
		if (Requerimento.ReqInterEmp && parseInt(Requerimento.ReqInterEmp.empreendimentoId) > 0) {
			Requerimento.botoes({ btnSalvar: true, spnCancelarEdicao: true });
			Requerimento.configurarBtnCancelarStep(4);
		} else {
			Requerimento.botoes({ btnSalvar: true, btnVoltar: true, spnCancelarEdicao: true });
			RequerimentoEmpreendimento.configurarBtnCancelar();
		}
	},

	configurarBtnCancelar: function () {
		Aux.scrollTop(Requerimento.container);
		$('.linkCancelar', Requerimento.container).unbind('click');
		$(".linkCancelar", Requerimento.container).click(function () {
			EmpreendimentoInline.onBtnVisualizarClick($('.hdnEmpId', Requerimento.container).val());
		});
	},

	onNovoEmpreendimentoClick: function () {

		MasterPage.carregando(true);

		var confEmpreendimento = (Requerimento.ReqInterEmp && parseInt(Requerimento.ReqInterEmp.empreendimentoId) > 0);

		EmpreendimentoInline.onVoltarEnter();

		Requerimento.botoes({ btnEmpAvancar: true, spnCancelarCadastro: true });
		RequerimentoEmpreendimento.desassociarEmp = (Requerimento.ReqInterEmp && parseInt(Requerimento.ReqInterEmp.empreendimentoId) > 0);
		Requerimento.ReqInterEmp.empreendimentoId = 0;
		RequerimentoEmpreendimento.salvarEmpreendimento = false;
		Requerimento.salvarTelaAtual();
		MasterPage.carregando(false);

		if (confEmpreendimento) {
			RequerimentoEmpreendimento.carregarIndentificarEmpreendimento();
		}
	},

	carregarIndentificarEmpreendimento: function () {
		MasterPage.carregando(true);
		var objeto = { params: {} };
		objeto.params.id = 0;
		Requerimento.onObterStep(RequerimentoEmpreendimento.urlObterEmpreendimento, objeto.params, RequerimentoEmpreendimento.callBackObterEmpreendimento);
	}
}

RequerimentoFinalizar = {

	urlObterFinalizar: null,
	urlFinalizar: null,

	callBackObterFinalizar: function () {
		var container = $('.modalFinalizar', Requerimento.container);

		var mostrarBotoes = { btnPdf: true, spnCancelarCadastro: true };

		if ($('.requerimentoSituacao', container).val() == 1) {//Em Andamento
			mostrarBotoes.btnFinalizar = true;
		}

		Requerimento.botoes(mostrarBotoes);

		Requerimento.stepAtual = 5;
		RequerimentoObjetivoPedido.configurarAssociarMultiploAtividade();
		RequerimentoObjetivoPedido.atividadeSolicitadaExpansivel();
		RequerimentoResponsavel.configurarAssociarMultiploResponsavel();
		Requerimento.salvarTelaAtual = null;
		Requerimento.alternarAbas();
		Requerimento.bloquearCampos(container);
		Requerimento.configurarBtnCancelarStep(5);
		MasterPage.carregando(false);
	},

	onFinalizarRequerimento: function () {

		var interessado = {
			Id: $('.hdnInteressadoId').val()
		};

		var objeto = {
			Id: $('#hdnRequerimentoId').val(),
			Interessado: interessado
		};

		$.ajax({
			url: RequerimentoFinalizar.urlFinalizar,
			type: "POST",
			data: JSON.stringify(objeto),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Requerimento.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.redirect && response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Requerimento.containerMensagem, response.Msg);
					return;
				}

				if (response.redirect) {
					MasterPage.redireciona(response.redirect);
				}
			}
		});
	}
}
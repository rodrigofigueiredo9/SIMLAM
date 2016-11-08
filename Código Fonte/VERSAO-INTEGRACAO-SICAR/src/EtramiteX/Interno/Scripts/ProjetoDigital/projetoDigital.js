/// <reference path="../masterpage.js" />
/// <reference path="../mensagem.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Pessoa/inline.js" />

ProjetoDigital = {
	stepAtual: 1,
	Mensagens: null,
	container: null,
	containerMensagem: null,
	urlObterPessoa: null,
	requerimento: {},
	salvarStepAtual: false,
	isVisualizar: false,
	salvarTelaAtual: null,

	load: function (container) {
		ProjetoDigital.container = container;
		ProjetoDigital.containerMensagem = MasterPage.getContent(container);

		ProjetoDigital.configurarStepWizard();
		ProjetoDigitalObjetivoPedido.callBackObterObjetivoPedido();

		container.delegate('.btnAvancar', 'click', ProjetoDigital.gerenciarWizard);
		container.delegate('.btnRoteiroPdf', 'click', ProjetoDigitalObjetivoPedido.gerarPdfRoteiro);
		container.delegate('.btnPessoaComparar', 'click', ProjetoDigital.abrirResponsavel);
		container.delegate('.btnFinalizar', 'click', ProjetoDigitalFinalizar.finalizar);
		container.delegate('.btnRecusar', 'click', ProjetoDigitalFinalizar.abrirModalNotificacao);

		Listar.atualizarEstiloTable($('.tabRoteiros', ProjetoDigital.container));
		ProjetoDigitalObjetivoPedido.carregouRoteiro = true;
		ProjetoDigital.bloquearCampos(container);
	},

	configurarStepWizard: function () {
		$('.containerAbas ul li', ProjetoDigital.container).each(function (i) {
			$(this).data("step", (i + 1));
			$(this).click(ProjetoDigital.gerenciarWizard);
		});
	},

	bloquearCampos: function (container) {
		$('.bloquear', container).attr('disabled', 'disabled').addClass('disabled');
	},

	obterStep: function (container) {
		if ($(container).data('step') !== null && $(container).data('step') !== NaN && $(container).data('step') !== undefined) {
			return +$(container).data('step');
		}

		if ($(container).hasClass('btnVoltar')) {
			return ProjetoDigital.stepAtual - 1;
		}

		if ($(container).hasClass('btnAvancar') || $(container).hasClass('btnSalvar')) {
			return ProjetoDigital.stepAtual + 1;
		}
	},

	gerenciarWizard: function () {
		Mensagem.limpar(ProjetoDigital.containerMensagem);
		if (ProjetoDigital.stepAtual === +$(this).data('step')) {
			return;
		}

		if (ProjetoDigital.salvarStepAtual && !ProjetoDigital.isVisualizar && ProjetoDigital.salvarTelaAtual) {
			if (!ProjetoDigital.salvarTelaAtual()) {
				return;
			}
		}

		MasterPage.carregando(true);
		$('.divFinalizar', ProjetoDigital.container).addClass('hide');
		$('.divAvancar', ProjetoDigital.container).removeClass('hide');

		switch (ProjetoDigital.obterStep(this)) {
			case 1:
				var params = { id: ProjetoDigital.requerimento.Id };
				ProjetoDigital.onObterStep(ProjetoDigitalObjetivoPedido.urlObterObjetivoPedido, params, ProjetoDigitalObjetivoPedido.callBackObterObjetivoPedido);
				break;

			case 2:
				var params = ProjetoDigital.obterPessoa(ProjetoDigital.requerimento.InteressadoCpfCnpj);
				ProjetoDigital.onObterStep(ProjetoDigital.urlObterPessoa, params, ProjetoDigitalInteressado.callBackObterInteressado);
				break;

			case 3:
				var params = { id: ProjetoDigital.requerimento.Id };
				ProjetoDigital.onObterStep(ProjetoDigitalResponsavel.urlObterResponsavel, params, ProjetoDigitalResponsavel.callBackObterResponsavel);
				break;

			case 4:
				var params = { id: ProjetoDigital.requerimento.EmpreendimentoId, isVisualizar: ProjetoDigital.isVisualizar };
				ProjetoDigital.onObterStep(ProjetoDigitalEmpreendimento.urlObterEmpreendimento, params, ProjetoDigitalEmpreendimento.callBackObterEmpreendimento);
				break;

			case 5:
				var params = { requerimento: ProjetoDigitalFinalizar.obterRequerimento() };
				ProjetoDigital.onObterStep(ProjetoDigitalFinalizar.urlObterFinalizar, params, ProjetoDigitalFinalizar.callBackObterFinalizar);
				break;
		}
	},

	onObterStep: function (urlStep, params, callBack) {
		$.ajax({ url: urlStep,
			type: "POST",
			dataType: 'html',
			contentType: 'application/json; charset=utf-8',
			data: JSON.stringify(params),
			cache: false,
			async: true,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ProjetoDigital.container);
				MasterPage.carregando(false);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				$('.conteudoProjetoDigital', ProjetoDigital.container).empty();
				$('.conteudoProjetoDigital', ProjetoDigital.container).append(response);
				callBack();
			}
		});
	},

	alternarAbas: function () {
		$('.containerAbas .ui-tabs-selected', ProjetoDigital.container).removeClass('ui-tabs-selected');
		$('.containerAbas ul li', ProjetoDigital.container).each(function () {
			if (+$(this).data('step') === ProjetoDigital.stepAtual) {
				$(this).addClass('ui-tabs-selected');
				return;
			}
		});
	},

	informarMensagem: function (tela) {
		$('.conteudoProjetoDigital', ProjetoDigital.container).empty();

		var divMensagem = $('.divMensagemTemplate', ProjetoDigital.container).clone();
		divMensagem.removeClass('divMensagemTemplate');
		var mensagem = Mensagem.replace(ProjetoDigital.Mensagens.NaoExisteAssocicao, '#tela', tela);
		$('.lblMensagem', divMensagem).text(mensagem.Texto);
		divMensagem.removeClass('hide');

		$('.conteudoProjetoDigital', ProjetoDigital.container).append(divMensagem);
	},

	obterPessoa: function (cnpfCnpj) {
		return { id: ProjetoDigital.requerimento.Id, cnpfCnpj: cnpfCnpj, isVisualizar: ProjetoDigital.isVisualizar, pessoas: ProjetoDigital.requerimento.Pessoas };
	},

	abrirResponsavel: function () {
		Modal.abrir(
		ProjetoDigital.urlObterPessoa,
			ProjetoDigital.obterPessoa($(this).closest('.divItemGrupo').find('.cpfCnpjPessoa').text()),
			function (container) {
				Comparar.load(container);
				container.delegate('.btnPessoaComparar', 'click', ProjetoDigital.abrirResponsavel);
				Modal.defaultButtons(container);
			}, Modal.tamanhoModalGrande);
	},

	salvarPessoa: function (container) {
		return true;
	}
}

ProjetoDigitalObjetivoPedido = {
	urlPdfRoteiro: null,
	urlObterObjetivoPedido: null,
	carregouRoteiro: false,

	callBackObterObjetivoPedido: function () {
		ProjetoDigitalObjetivoPedido.configurarAssociarMultiploAtividade();
		ProjetoDigital.stepAtual = 1;
		ProjetoDigital.salvarStepAtual = true;
		ProjetoDigital.salvarTelaAtual = ProjetoDigitalObjetivoPedido.salvarObjetivoPedido;
		ProjetoDigitalObjetivoPedido.atividadeSolicitadaExpansivel();
		ProjetoDigital.alternarAbas();
		ProjetoDigital.bloquearCampos();
		Aux.carregando(ProjetoDigital.container, false);

		$('.ddlSetores', ProjetoDigital.container).val(ProjetoDigital.requerimento.SetorId);
	},

	atividadeSolicitadaExpansivel: function () {
		$('.asmConteudoInternoExpander', $('.divConteudoAtividadeSolicitada .asmItens', ProjetoDigital.container)).addClass('asmExpansivel');
	},

	configurarAssociarMultiploAtividade: function () {
		$('.divConteudoAtividadeSolicitada', ProjetoDigital.container).associarMultiplo({
			'expandirAutomatico': false,
			'minItens': 0,
			'tamanhoModal': Modal.tamanhoModalGrande
		});
	},

	gerarPdfRoteiro: function () {
		MasterPage.carregando(true);
		var id = $(this).closest('td').find('.hdnRoteiroId').val();
		MasterPage.redireciona(ProjetoDigitalObjetivoPedido.urlPdfRoteiro + '?id=' + id);
		MasterPage.carregando(false);
	},

	salvarObjetivoPedido: function () {

		var isValido = false;

		var requerimento = {
			Id: $('#hdnRequerimentoId', ProjetoDigital.container).val() || 0,
			SetorId: $('.ddlSetores', ProjetoDigital.container).val()
		};

		$.ajax({
			url: ProjetoDigitalFinalizar.urlValidarObjetivoPedido,
			type: "POST",
			data: JSON.stringify(requerimento),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ProjetoDigital.container);
			},

			success: function (response, textStatus, XMLHttpRequest) {

				isValido = response.EhValido;

				if (response.Msg.length > 0)
				{
					Mensagem.gerar(ProjetoDigital.containerMensagem, response.Msg);
				}
			}
		});

		ProjetoDigital.requerimento.SetorId = requerimento.SetorId;

		return isValido;

	}
}

ProjetoDigitalInteressado = {

	callBackObterInteressado: function () {
		ProjetoDigital.stepAtual = 2;
		ProjetoDigital.alternarAbas();
		ProjetoDigital.salvarStepAtual = true;
		ProjetoDigital.salvarTelaAtual = function () { return ProjetoDigital.salvarPessoa(ProjetoDigital.container); };

		Comparar.load(ProjetoDigital.container);
		Mascara.load(ProjetoDigital.container);
		Aux.carregando(ProjetoDigital.container, false);
	}
}

ProjetoDigitalResponsavel = {
	urlObterResponsavel: null,

	callBackObterResponsavel: function () {
		ProjetoDigitalResponsavel.configurarAssociarMultiploResponsavel();

		if ($('.divConteudoResponsavelTec .hdnQuantidadeItem', ProjetoDigital.container).val() == 0) {
			ProjetoDigital.informarMensagem('responsável técnico');
		}

		ProjetoDigital.salvarStepAtual = false;
		ProjetoDigital.stepAtual = 3;
		ProjetoDigital.alternarAbas();
		ProjetoDigital.bloquearCampos();
		Aux.carregando(ProjetoDigital.container, false);
	},

	configurarAssociarMultiploResponsavel: function () {
		$('.divConteudoResponsavelTec', ProjetoDigital.container).associarMultiplo({});
		$('.divConteudoResponsavelTec', ProjetoDigital.container).delegate('.btnAsmEditar', 'click', function () {
			Modal.abrir(
			ProjetoDigital.urlObterPessoa,
			ProjetoDigital.obterPessoa($('.cpfCnpj', $(this).closest('.conteudoResponsaveis')).val()),
			function (container) {
				Comparar.load(container);
				container.delegate('.btnPessoaComparar', 'click', ProjetoDigital.abrirResponsavel);
				Modal.defaultButtons(container);
			}, Modal.tamanhoModalGrande);
		});
	}
}

ProjetoDigitalEmpreendimento = {
	urlObterEmpreendimento: null,

	callBackObterEmpreendimento: function () {
		ProjetoDigital.stepAtual = 4;
		ProjetoDigital.alternarAbas();
		ProjetoDigital.salvarStepAtual = true;
		ProjetoDigital.salvarTelaAtual = function () { return ProjetoDigitalEmpreendimento.salvarEmpreendimento(ProjetoDigital.container); };

		Comparar.load(ProjetoDigital.container);
		Aux.carregando(ProjetoDigital.container, false);
	},

	salvarEmpreendimento: function () {
		return true;
	}
}

ProjetoDigitalFinalizar = {
	urlObterFinalizar: null,
	urlFinalizar: null,
	urlModalNotificacao: null,
	urlRecusar: null,

	callBackObterFinalizar: function () {
		var container = $('.modalFinalizar', ProjetoDigital.container);
		var pai = container.closest('.projetoDigitalPartial');

		$('.divAvancar', pai).addClass('hide');
		$('.divFinalizar', pai).removeClass('hide');

		ProjetoDigital.stepAtual = 5;
		ProjetoDigital.salvarStepAtual = false;
		ProjetoDigitalResponsavel.configurarAssociarMultiploResponsavel();
		ProjetoDigitalObjetivoPedido.configurarAssociarMultiploAtividade();
		ProjetoDigitalObjetivoPedido.atividadeSolicitadaExpansivel();
		ProjetoDigital.alternarAbas();
		ProjetoDigital.bloquearCampos();
		Aux.carregando(ProjetoDigital.container, false);
	},

	obterRequerimento: function () {
		var pessoa = ProjetoDigital.obterPessoa(ProjetoDigital.requerimento.InteressadoCpfCnpj);

		return {
			Id: ProjetoDigital.requerimento.Id,
			ProjetoDigitalId: ProjetoDigital.requerimento.ProjetoDigitalId,
			IdRelacionamento: ProjetoDigital.requerimento.IdRelacionamento,
			Tid: ProjetoDigital.requerimento.Tid,
			CredenciadoId: ProjetoDigital.requerimento.CredenciadoId,
			SetorId: ProjetoDigital.requerimento.SetorId,
			Interessado: {
				Id: pessoa.id,
				InternoId: pessoa.internoId,
				CPFCNPJ: pessoa.cnpfCnpj
			},
			Empreendimento: {
				Id: ProjetoDigital.requerimento.EmpreendimentoId,
				InternoId: ProjetoDigital.requerimento.EmpreendimentoInternoId,
				Denominador: ProjetoDigital.requerimento.EmpreendimentoDenominador
			},
			Pessoas: ProjetoDigital.requerimento.Pessoas
		};
	},

	finalizar: function () {
		var objeto = ProjetoDigitalFinalizar.obterRequerimento();

		$.ajax({
			url: ProjetoDigitalFinalizar.urlFinalizar,
			type: "POST",
			data: JSON.stringify(objeto),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ProjetoDigital.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				} else {
					Mensagem.gerar(ProjetoDigital.containerMensagem, response.Msg);
				}
			}
		});
	},

	abrirModalNotificacao: function () {
		Modal.confirma({ btnOkLabel: 'Salvar', url: ProjetoDigitalFinalizar.urlModalNotificacao, btnOkCallback: ProjetoDigitalFinalizar.notificar, tamanhoModal: Modal.tamanhoModalMedia });
	},

	notificar: function () {
		$.ajax({
			url: ProjetoDigitalFinalizar.urlRecusar,
			type: "POST",
			data: JSON.stringify({ requerimentoId: ProjetoDigital.requerimento.Id, motivo: $('.txtMotivoRecusa', $('.motivoContainer')).val() }),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ProjetoDigital.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.Url);
				} else {
					Mensagem.gerar(ProjetoDigital.containerMensagem, response.Msg);
				}
			}
		});
	}
}
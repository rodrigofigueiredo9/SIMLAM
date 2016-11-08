/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Pessoa/inline.js" />

RequerimentoVis = {
	urlIndex: null,
	urlPdf: null,
	urlObterReqInterEmp: null,

	stepAtual: 1,
	Mensagens: null,
	containerMensagem: null,
	container: null,
	ReqInterEmp: null,
	mostrarBtnEditar: true,

	load: function (container) {
		RequerimentoVis.container = container;
		RequerimentoVis.containerMensagem = MasterPage.getContent(container);
		RequerimentoObjetivoPedido.configurarAssociarMultiploAtividade();
		RequerimentoVis.configurarStepWizard();
		container.delegate('.btnRoteiroPdf', 'click', RequerimentoObjetivoPedido.onBaixarPdfClick);
		$(".linkCancelar", RequerimentoVis.container).click(RequerimentoVis.onCancelarPadrao);

		RequerimentoVis.bloquearCampos(container);
	},

	gerenciarWizard: function () {
		if (RequerimentoVis.stepAtual === +$(this).data('step'))
			return;

		Aux.carregando(RequerimentoVis.container, true);
		$('.btnFinalizar, .btnPdf').parent().addClass('hide');
		var params = { id: $('#hdnRequerimentoId').val()}

		switch (RequerimentoVis.obterStep(this)) {

			case 1:

				RequerimentoVis.onObterStep(RequerimentoObjetivoPedido.urlObterObjetivoPedido, params, RequerimentoObjetivoPedido.callBackObterObjetivoPedido);
				break;

			case 2:

				RequerimentoVis.obterReqInterEmp(RequerimentoVis.urlObterReqInterEmp, params);
				params.id = RequerimentoVis.ReqInterEmp.interessadoId;
				if (params.id == 0) {
					RequerimentoVis.informarMensagem('interessado', 2);
					return;
				}

				RequerimentoVis.onObterStep(RequerimentoInteressado.urlObterInteressado, params, RequerimentoInteressado.callBackObterInteressado);
				break;

			case 3:

				RequerimentoVis.onObterStep(RequerimentoResponsavel.urlObterResponsavel, params, RequerimentoResponsavel.callBackObterResponsavel);
				break;

			case 4:

				RequerimentoVis.obterReqInterEmp(RequerimentoVis.urlObterReqInterEmp, params);
				params.id = RequerimentoVis.ReqInterEmp.empreendimentoId;

				if (params.id == 0) {
					RequerimentoVis.informarMensagem('empreendimento', 4);
					return;
				}

				RequerimentoVis.onObterStep(RequerimentoEmpreendimento.urlObterEmpreendimento, params, RequerimentoEmpreendimento.callBackObterEmpreendimento);
				break;

			case 5:

				RequerimentoVis.onObterStep(RequerimentoFinalizar.urlObterFinalizar, params, RequerimentoFinalizar.callBackObterFinalizar);
				break;
		}
	},

	obterStep: function (container) {
		if ($(container).data('step') !== null && $(container).data('step') !== NaN && $(container).data('step') !== undefined) {

			return +$(container).data('step');
		}

		if ($(container).hasClass('btnVoltar')) {
			return RequerimentoVis.stepAtual - 1;
		}

		if ($(container).hasClass('btnAvancar') || $(container).hasClass('btnSalvar')) {
			return RequerimentoVis.stepAtual + 1;
		}
	},

	configurarStepWizard: function () {
		$('.AbasRequerimento ul li').each(function (i) {
			$(this).data("step", (i + 1));
			$(this).click(RequerimentoVis.gerenciarWizard);
		});
	},

	alternarAbas: function () {
		$('.AbasRequerimento .ui-tabs-selected').removeClass('ui-tabs-selected');
		$('.AbasRequerimento ul li').each(function () {

			if (+$(this).data('step') === RequerimentoVis.stepAtual) {
				$(this).addClass('ui-tabs-selected');
				return;
			}
		});
	},

	onAbrirPdfClick: function () {
		var id = $('#hdnRequerimentoId').val();
		MasterPage.redireciona(RequerimentoVis.urlPdf + "?id=" + id);
		MasterPage.carregando(false);
	},

	onObterStep: function (urlStep, params, callBack) {
		$.ajax({ url: urlStep,
			type: "GET",
			data: params,
			cache: false,
			async: true,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {

				$('.conteudoRequerimento', RequerimentoVis.container).empty();
				$('.conteudoRequerimento', RequerimentoVis.container).append(response);
				callBack();
			}
		});
	},

	obterReqInterEmp: function (urlBuscar, params) {
		if (RequerimentoVis.ReqInterEmp) {
			return;
		}

		$.ajax({ url: urlBuscar,
			type: "GET",
			data: params,
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(RequerimentoVis.containerMensagem, response.Msg);
				}
				else {
					RequerimentoVis.ReqInterEmp = response.Requerimento;
				}
			}
		});
	},

	bloquearCampos: function (container) {
		$('.modoVisualizar', container).remove();
		$('.bloquear', container).attr('disabled', 'disabled').addClass('disabled');
	},

	informarMensagem: function (tela, step) {

		$('.conteudoRequerimento', RequerimentoVis.container).empty();

		var divMensagem = $('.divMensagemTemplate', RequerimentoVis.container).clone();
		
		divMensagem.removeClass('divMensagemTemplate');

		var mensagem = Mensagem.replace(RequerimentoVis.Mensagens.NaoExisteAssocicao, '#tela', tela);

		$('.lblMensagem', divMensagem).text(mensagem.Texto);

		divMensagem.removeClass('hide');

		$('.conteudoRequerimento', RequerimentoVis.container).append(divMensagem);

		RequerimentoVis.stepAtual = step;
		RequerimentoVis.alternarAbas();
		Aux.carregando(RequerimentoVis.container, false);
	},

	onCancelarPadrao: function () {
		MasterPage.redireciona(RequerimentoVis.urlIndex);
	}
}

RequerimentoObjetivoPedido = {
	visualizarRoteiroModalLink: null,
	urlObterObjetivoPedido: null,

	callBackObterObjetivoPedido: function () {
		RequerimentoObjetivoPedido.configurarAssociarMultiploAtividade();
		RequerimentoVis.stepAtual = 1;
		
		RequerimentoObjetivoPedido.atividadeSolicitadaExpansivel();
		RequerimentoObjetivoPedido.configurarNumeroAnterior();
		RequerimentoVis.alternarAbas();
		RequerimentoVis.bloquearCampos();
		Aux.carregando(RequerimentoVis.container, false);
	},

	configurarNumeroAnterior: function () {
		$('.asmItens .asmItemContainer').each(function () {
			if (RequerimentoObjetivoPedido.mostrarCampoNumeroAnterior(this)) {
				$('.numeroAnterior', this).removeClass('hide');
			} else {
				$('.divTipoDocumento .NumeroDocumentoAnt', this).each(function () {
					$('.NumeroDocumentoAnt', this).val("");
				});
				$('.numeroAnterior', this).addClass('hide');
			}
		});
	},

	atividadeSolicitadaExpansivel: function () {
		$('.asmConteudoInternoExpander', $('.divConteudoAtividadeSolicitada .asmItens')).addClass('asmExpansivel');
	},

	mostrarCampoNumeroAnterior: function (container) {
		var res = false;
		$('.divFinalidade .checkboxFinalidade', container).each(function () {
			var checkValue = $(this).val();
			if (checkValue === "4" ||
				checkValue === "8" ||
				checkValue === "16") {
				if (this.checked) {
					res = true;
					return;
				}
			}
		});
		return res;
	},

	configurarAssociarMultiploAtividade: function () {
		$('.divConteudoAtividadeSolicitada').associarMultiplo({
			'associarModalLoadFunction': 'AtividadeSolicitadaListar.load',
			'expandirAutomatico': false,
			'minItens': 0,
			'tamanhoModal': Modal.tamanhoModalGrande
		});
	},

	onBaixarPdfClick: function () {
		var id = $(this).closest('td').find('.hdnRoteiroId').val();
		MasterPage.redireciona(RequerimentoObjetivoPedido.urlBaixarPdf + '/' + id);
	}
}

RequerimentoInteressado = {
	urlObterInteressado: null,
	urlAssociarInteressado: null,
	pessoaInlineObj: null,

	callBackObterInteressado: function () {
		RequerimentoInteressado.pessoaInlineObj = new PessoaInline();

		RequerimentoInteressado.pessoaInlineObj.load(RequerimentoVis.container, {
			onVisualizarEnter: RequerimentoInteressado.onVisualizarEnterInteressado,
			onEditarEnter: RequerimentoInteressado.onEditarEnterInteressado,
			onVerificarEnter: RequerimentoInteressado.onVerificarEnterInteressado
		});

		RequerimentoVis.stepAtual = 2;
		RequerimentoVis.alternarAbas();

		Mascara.load(RequerimentoVis.container);

		if (RequerimentoVis.ReqInterEmp && RequerimentoVis.ReqInterEmp.interessadoId > 0) {
			RequerimentoInteressado.pessoaInlineObj.onVisualizarEnter();
		}

		Aux.carregando(RequerimentoVis.container, false);
	},

	SalvarInteressado: function (partialContent, responseJson, isEditar) {
		var param = { requerimentoId: null, interessadoId: null };

		param.interessadoId = RequerimentoInteressado.pessoaInlineObj.onSalvarClick();
		param.requerimentoId = $('#hdnRequerimentoId').val();

		if (param.interessadoId == 0) {
			return false;
		}

		$.ajax({
			url: RequerimentoInteressado.urlAssociarInteressado,
			type: "POST",
			data: JSON.stringify(param),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(RequerimentoVis.containerMensagem, response.Msg);
				}
			}
		});

		RequerimentoVis.ReqInterEmp['interessadoId'] = param.interessadoId;
		return true;
	},

	onVisualizarEnterInteressado: function () {
		$(".btnEditar", RequerimentoVis.container).unbind('click');
		$(".btnEditar", RequerimentoVis.container).click(RequerimentoInteressado.pessoaInlineObj.onBtnEditarClick);
	},

	onEditarEnterInteressado: function () {	// mostra botão cancelar e salvar
	},

	onVerificarEnterInteressado: function () {
	}
}

RequerimentoResponsavel = {
	urlObterResponsavel: null,
	modalPessoaResp: null,

	callBackObterResponsavel: function () {
		RequerimentoResponsavel.configurarAssociarMultiploResponsavel();

		if ($('.divConteudoResponsavelTec .hdnQuantidadeItem', RequerimentoVis.container).val() == 0) {
			RequerimentoVis.informarMensagem('responsável técnico', 3);
		} else {
			RequerimentoVis.stepAtual = 3;
			RequerimentoVis.alternarAbas();
			RequerimentoVis.bloquearCampos();
			Aux.carregando(RequerimentoVis.container, false);
		}
	},

	configurarAssociarMultiploResponsavel: function () {
		RequerimentoResponsavel.modalPessoaResp = new PessoaAssociar();

		$('.divConteudoResponsavelTec').associarMultiplo({
			'editarModalObject': RequerimentoResponsavel.modalPessoaResp,
			'editarModalLoadFunction': RequerimentoResponsavel.modalPessoaResp.load,
			'editarModalLoadParams': {
				tituloVisualizar: 'Visualizar Responsável Técnico',
				visualizando: true,
				editarVisualizar: false,
				urls: {
					visualizarModal: RequerimentoResponsavel.urlAssociarResponsavelEditarModal
				}
			},
			'editarUrl': RequerimentoResponsavel.urlAssociarResponsavelEditarModal,
			'expandirAutomatico': true,
			'minItens': 0,
			'tamanhoModal': Modal.tamanhoModalGrande
		});
	}
}

RequerimentoEmpreendimento = {

	urlObterEmpreendimento: null,
	urlAssociarEmpreendimento: null,

	callBackObterEmpreendimento: function () {

		EmpreendimentoInline.load(RequerimentoVis.container, {
			onIdentificacaoEnter: RequerimentoEmpreendimento.onIdentificacaoEnterEmpreendimento,
			onVisualizarEnter: RequerimentoEmpreendimento.onVisualizarEnterEmpreendimento,
			onEditarEnter: RequerimentoEmpreendimento.onEditarEnterEmpreendimento
		});

		RequerimentoVis.stepAtual = 4;
		RequerimentoVis.alternarAbas();

		if (RequerimentoVis.ReqInterEmp && parseInt(RequerimentoVis.ReqInterEmp.empreendimentoId) > 0) {
			EmpreendimentoInline.modo = 2;
			RequerimentoEmpreendimento.onVisualizarEnterEmpreendimento();
		} else {
			RequerimentoEmpreendimento.onIdentificacaoEnterEmpreendimento();
		}

		Aux.carregando(RequerimentoVis.container, false);
	},

	SalvarEmpreendimento: function (partialContent, responseJson, isEditar) {

		var param = { requerimentoId: null, empreendimentoId: null };

		param.empreendimentoId = EmpreendimentoInline.onSalvarClick();
		param.requerimentoId = $('#hdnRequerimentoId').val();

		if (param.empreendimentoId == 0) {
			return false;
		}

		$.ajax({
			url: RequerimentoEmpreendimento.urlAssociarEmpreendimento,
			type: "POST",
			data: JSON.stringify(param),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(RequerimentoVis.containerMensagem, response.Msg);
				}
			}
		});

		return true;
	},

	onIdentificacaoEnterEmpreendimento: function () {
		$('.btnAvancar', RequerimentoVis.container).parent().addClass('hide');
		$('.btnEmpAvancar', RequerimentoVis.container).parent().removeClass('hide');

		$(".btnEmpAvancar", RequerimentoVis.container).unbind('click');
		$('.btnEmpAvancar', RequerimentoVis.container).click(EmpreendimentoInline.onAvancarEnter);
		$('.btnEmpAvancar', RequerimentoVis.container).button({ disabled: true });
	},

	onVisualizarEnterEmpreendimento: function () {
		$(".btnEmpAssNovo", RequerimentoVis.container).parent().removeClass('hide');
		$(".btnEditar", RequerimentoVis.container).parent().removeClass('hide');
		$(".btnSalvar", RequerimentoVis.container).parent().addClass('hide');

		$(".btnEditar", RequerimentoVis.container).unbind('click');
		$(".btnEditar", RequerimentoVis.container).click(EmpreendimentoInline.onBtnEditarClick);

		$(".btnEmpAssNovo", RequerimentoVis.container).unbind('click');
		$(".btnEmpAssNovo", RequerimentoVis.container).click(RequerimentoEmpreendimento.onNovoEmpreendimentoClick);
	},

	onEditarEnterEmpreendimento: function () {
		$(".btnEditar", RequerimentoVis.container).parent().addClass('hide');
		$(".btnSalvar", RequerimentoVis.container).parent().removeClass('hide');
	},

	onNovoEmpreendimentoClick: function () {

		Aux.carregando(RequerimentoVis.container, true);
		RequerimentoVis.ReqInterEmp.empreendimentoId = 0;
		var params = { id: RequerimentoVis.ReqInterEmp.empreendimentoId }

		RequerimentoVis.onObterStep(RequerimentoEmpreendimento.urlObterEmpreendimento, params, RequerimentoEmpreendimento.callBackObterEmpreendimento);
	}
}

RequerimentoFinalizar = {

	urlObterFinalizar: null,
	urlFinalizar: null,

	callBackObterFinalizar: function () {

		var container = $('.modalFinalizar');
		var pai = container.closest('.requerimentoPartial');

		$('.divAvancar', pai).addClass('hide');
		$('.btnSalvar', pai).parent().addClass('hide');
		$('.btnFinalizar, .btnPdf', pai).parent().removeClass('hide');

		RequerimentoVis.bloquearCampos(container);
		RequerimentoVis.stepAtual = 5;
		RequerimentoObjetivoPedido.configurarAssociarMultiploAtividade();
		RequerimentoObjetivoPedido.atividadeSolicitadaExpansivel();
		RequerimentoResponsavel.configurarAssociarMultiploResponsavel();
		RequerimentoObjetivoPedido.configurarNumeroAnterior();
		RequerimentoVis.alternarAbas();
		Aux.carregando(RequerimentoVis.container, false);
	}
}
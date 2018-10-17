/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.ddl.js" />

Solicitacao = {
	settings: {
		urls: {
			salvar: '',
			associarProjetoDigital: '',
			obterProjetoDigital: '',
			obterAtividades: '',
			visualizarRequerimento: ''
		}
	},
	container: null,
	Mensagens: {},

	load: function (container, options) {

		if (options) {
			$.extend(Solicitacao.settings, options);
		}

		Solicitacao.container = MasterPage.getContent(container);
		Solicitacao.container = container;
		Solicitacao.container.delegate('.btnSalvar', 'click', Solicitacao.salvar);
		Solicitacao.container.delegate('.btnBuscarProjeto', 'click', Solicitacao.onBuscarProjetoDigital);
		Solicitacao.container.delegate('.btnLimparProjeto', 'click', Solicitacao.onLimparProjetoDigital);
		Solicitacao.container.delegate('.btnVisualizarRequerimento', 'click', Solicitacao.onVisualizarRequerimento);
	},

	onVisualizarRequerimento: function () {
		var projetoId = parseInt($('.hdnProjetoId', Solicitacao.container).val());
		var requerimentoId = parseInt($('.txtRequerimento', Solicitacao.container).val());

		Modal.abrir(Solicitacao.settings.urls.visualizarRequerimento, { id: requerimentoId, projetoDigitalId: projetoId, isVisualizar: 'True' }, function (context) {
			Modal.defaultButtons(context);
			RequerimentoVis.mostrarBtnEditar = false;
		});
	},

	onBuscarProjetoDigital: function () {
		Modal.abrir(Solicitacao.settings.urls.associarProjetoDigital, null, function (container) {
			Modal.defaultButtons(container);
			ProjetoDigitalListar.load(container, { associarFuncao: Solicitacao.callBackAssociarProjetoDigital, urlVisualizarRequerimento: Solicitacao.settings.urls.visualizarRequerimento });
		}, Modal.tamanhoModalGrande);
	},

	onLimparProjetoDigital: function () {
		var container = Solicitacao.container;

		$('.hdnProjetoId', container).val(0);
		$('.hdnEmpreendimentoId', container).val(0);

		$('.txtProjetoId', container).val('');
		$('.txtRequerimento', container).val('');
		$('.txtEmpreendimentoCodigo', container).val('');
		$('.txtEmpreendimentoNomeRazao', container).val('');

		$('.ddlAtividade', container).ddlClear();

		$('.ddlDeclarante', container).ddlClear();

		Solicitacao.gerenciarBotoes();
	},

	callBackAssociarProjetoDigital: function (objeto) {
		var container = Solicitacao.container;
		Mensagem.limpar(container);
		Solicitacao.onLimparProjetoDigital();
		objeto = JSON.parse(objeto);
		var retorno;

		$.ajax({
			url: Solicitacao.settings.urls.obterProjetoDigital,
			data: JSON.stringify({ projetoId: objeto.Id }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Solicitacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido && response.ProjetoDigital) {
					$('.txtProjetoId', container).val(response.ProjetoDigital.RequerimentoId);
					$('.hdnProjetoId', container).val(response.ProjetoDigital.Id);
					$('.txtRequerimento', container).val(response.ProjetoDigital.RequerimentoId);

					$('.hdnEmpreendimentoId', container).val(response.Empreendimento.Id);
					$('.txtEmpreendimentoCodigo', container).val(response.Empreendimento.Codigo);
					$('.txtEmpreendimentoNomeRazao', container).val(response.Empreendimento.Denominador);

					$('.ddlAtividade', container).ddlLoad(response.AtividadesLst, { textoDefault: undefined });
					$('.ddlAtividade', container).removeClass('disabled');
					$('.ddlAtividade', container).removeAttr('disabled');

					$('.ddlDeclarante', container).ddlLoad(response.DeclaranteLst);
					$('.ddlDeclarante', container).removeClass('disabled');
					$('.ddlDeclarante', container).removeAttr('disabled');

					$('.spnVisualizarRequerimento', container).removeClass('hide');

					Solicitacao.gerenciarBotoes();

					retorno = true;
				}

				if (!response.EhValido) {
					retorno = response.Msg;
				}
			}
		});

		return retorno;
	},

	gerenciarBotoes: function () {
		var container = Solicitacao.container;
		var projetoId = $('.hdnProjetoId', container).val();

		$('.spnLimparContainer', container).addClass('hide');
		$('.spnBuscarProjeto', container).removeClass('hide');
		$('.spnVisualizarRequerimento', container).addClass('hide');

		if (projetoId > 0) {
			$('.spnBuscarProjeto', container).addClass('hide');
			$('.spnLimparContainer', container).removeClass('hide');
			$('.spnVisualizarRequerimento', container).removeClass('hide');
		}
	},

	obter: function () {
		return {
			Id: $('.hdnSolicitacaoId', Solicitacao.container).val(),
			Numero: $('.txtNumero', Solicitacao.container).val(),
			DataEmissao: { DataTexto: $('.txtDataEmissao', Solicitacao.container).val() },
			SituacaoId: $('.ddlSituacao :selected', Solicitacao.container).val(),
			SituacaoTexto: $('.ddlSituacao :selected', Solicitacao.container).text(),
			ProjetoId: $('.hdnProjetoId', Solicitacao.container).val(),
			Atividade: { Id: $('.ddlAtividade :selected', Solicitacao.container).val() },
			Requerimento: { Id: $('.txtRequerimento', Solicitacao.container).val() },
			Empreendimento: {
				Id: $('.hdnEmpreendimentoId', Solicitacao.container).val(),
				Codigo: $('.txtEmpreendimentoCodigo', Solicitacao.container).val(),
				NomeRazao: $('.txtEmpreendimentoNomeRazao', Solicitacao.container).val()

			},
			Declarante: { Id: $('.ddlDeclarante :selected', Solicitacao.container).val() }
		};
	},

	salvar: function () {
		Mensagem.limpar(Solicitacao.container);

		console.log("ENTREI");
		$(this.currentTarget).attr('disabled', 'disabled');
		MasterPage.carregando(true);
		$.ajax({
			url: Solicitacao.settings.urls.salvar,
			data: JSON.stringify(Solicitacao.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Solicitacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.urlRetorno);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Solicitacao.container, response.Msg);
				}
			}
		});

		setTimeout(function () {
			$(this.currentTarget).removeAttr('disabled');
		}, 2000);
		MasterPage.carregando(false);
	}
}
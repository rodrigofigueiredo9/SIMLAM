/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

Solicitacao = {
	settings: {
		urls: {
			salvar: '',
			associarProtocolo: '',
			obterProcessosDocumentos: '',
			obterAtividades: '',
			visualizarProtocolo: ''
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
		Solicitacao.container.delegate('.btnBuscarProtocolo', 'click', Solicitacao.onBuscarProtocolo);
		Solicitacao.container.delegate('.btnLimparProtocolo', 'click', Solicitacao.onLimparProtocolo);
		Solicitacao.container.delegate('.btnVisualizarProtocolo', 'click', Solicitacao.onVisualizarProtocolo);
		Solicitacao.container.delegate('.ddlRequerimento', 'change', Solicitacao.onChangeRequerimento);
	},

	onVisualizarProtocolo: function () {
		var protocoloId = parseInt($('.hdnProtocoloId', Solicitacao.container).val());

		Modal.abrir(Solicitacao.settings.urls.visualizarProtocolo, { id: protocoloId }, function (context) {
			Modal.defaultButtons(context);
		});
	},

	onChangeRequerimento: function () {
		var container = Solicitacao.container;
		var protocolo = $('.ddlRequerimento :selected', container).val().split('@')[0];

		$.ajax({
			url: Solicitacao.settings.urls.obterAtividades,
			data: JSON.stringify({ protocoloId: protocolo }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Solicitacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Atividades) {
					$('.ddlAtividade', container).ddlLoad(response.Atividades);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Solicitacao.container, response.Msg);
				}
			}
		});
	},

	onBuscarProtocolo: function(){
		Modal.abrir(Solicitacao.settings.urls.associarProtocolo, null, function (container) {
			ProtocoloListar.load(container, { associarFuncao: Solicitacao.callBackAssociarProtocolo });
		}, Modal.tamanhoModalGrande);
	},

	onLimparProtocolo: function () {
		var container = Solicitacao.container;

		$('.hdnProtocoloId', container).val(0);
		$('.txtProtocoloNumero', container).val('');
		$('.hdnEmpreendimentoId', container).val(0);
		$('.txtEmpreendimentoCodigo', container).val('');
		$('.txtEmpreendimentoNomeRazao', container).val('');

		$('.ddlAtividade', container).ddlClear();
		$('.ddlRequerimento', container).ddlClear();
		$('.ddlDeclarante', container).ddlClear();

		Solicitacao.gerenciarBotoesProtocolo();
	},

	callBackAssociarProtocolo: function (objeto) {
		var container = Solicitacao.container;
		Mensagem.limpar(container);

		Solicitacao.onLimparProtocolo();
		var retorno;

		$.ajax({
			url: Solicitacao.settings.urls.obterProcessosDocumentos,
			data: JSON.stringify({protocoloId: objeto.Id}),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Solicitacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido && response.ProcessosDocumentos) {
					$('.txtProtocoloNumero', container).val(objeto.Numero);
					$('.hdnProtocoloId', container).val(objeto.Id);

					$('.ddlRequerimento', container).ddlLoad(response.ProcessosDocumentos);

					$('.hdnEmpreendimentoId', container).val(response.Empreendimento.Id);
					$('.txtEmpreendimentoCodigo', container).val(response.Empreendimento.Codigo);
					$('.txtEmpreendimentoNomeRazao', container).val(response.Empreendimento.Denominador);

					$('.ddlDeclarante', container).ddlLoad(response.DeclaranteLst);

					Solicitacao.onChangeRequerimento();
					Solicitacao.gerenciarBotoesProtocolo();

					retorno = true;
				}

				if (!response.EhValido) {
					retorno = response.Msg;
				}
			}
		});

		return retorno;
	},

	gerenciarBotoesProtocolo: function () {
		var container = Solicitacao.container;
		var protocoloId = $('.hdnProtocoloId', container).val();

		$('.spnLimparContainer', container).addClass('hide');
		$('.spnBuscarProtocolo', container).removeClass('hide');
		$('.spnVisualizarProtocolo', container).addClass('hide');

		if (protocoloId > 0) {
			$('.spnBuscarProtocolo', container).addClass('hide');
			$('.spnVisualizarProtocolo', container).removeClass('hide');
			$('.spnLimparContainer', container).removeClass('hide');
		}
	},

	obter: function () {
		return {
			Id: $('.hdnSolicitacaoId', Solicitacao.container).val(),
			Numero: $('.txtNumero', Solicitacao.container).val(),
			DataEmissao: { DataTexto: $('.txtDataEmissao', Solicitacao.container).val() },
			SituacaoId: $('.ddlSituacao :selected', Solicitacao.container).val(),
			SituacaoTexto: $('.ddlSituacao :selected', Solicitacao.container).text(),
			Protocolo: { Id: $('.hdnProtocoloId', Solicitacao.container).val() },
			Requerimento: { Id: $('.ddlRequerimento :selected', Solicitacao.container).val().split('@')[2] },
			ProtocoloSelecionado: { Id: $('.ddlRequerimento :selected', Solicitacao.container).val().split('@')[0] },
			Atividade: { Id: $('.ddlAtividade :selected', Solicitacao.container).val() },
			Empreendimento: { 
				Id: $('.hdnEmpreendimentoId', Solicitacao.container).val(), 
				Codigo: $('.txtEmpreendimentoCodigo', Solicitacao.container).val(),
				NomeRazao: $('.txtEmpreendimentoNomeRazao', Solicitacao.container).val()

			},
			Declarante: { Id: $('.ddlDeclarante :selected', Solicitacao.container).val() }
		}
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

				console.log(response.Msg);

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
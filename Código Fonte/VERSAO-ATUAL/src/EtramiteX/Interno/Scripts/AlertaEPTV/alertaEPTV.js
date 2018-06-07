/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="masterpage.js" />
/// <reference path="mensagem.js" />

AlertaEPTV = {
	settings: {
		urls: {
			urlEPTVAguardandoAnalise: ''
		},
		botoes: new Array(), //[{ label: '', url: '', callBack: null}]
		limparContainer: true
	},
	container: null,

	load: function (container, options) {
		AlertaEPTV.container = container;

		if (options) {
			$.extend(AlertaEPTV.settings, options);
		}

		$(".fecharMensagem").click(AlertaEPTV.onFecharDaMensagemClick);

		Mensagem.limparCallbacks['containerAcoesAlertaPTV'] = AlertaEPTV.onMensagemLimpar;

		if ($(AlertaEPTV.container).hasClass("containerAcoesAlertaPTV")) {
			$(AlertaEPTV.container).removeClass("hide");
		} else {
			$(".containerAcoesAlertaPTV", AlertaEPTV.container).removeClass("hide");
		}

		//if (ContainerAcoes.settings.urls.urlVisualizarPdfFiscalizacao) {
		//	ContainerAcoes.settings.botoes.push({ label: 'Visualizar PDF', url: ContainerAcoes.settings.urls.urlVisualizarPdfFiscalizacao, callBack: ContainerAcoes.onAcaoClickAbrirModal });
		//}

		if (AlertaEPTV.settings.urls.urlEPTVAguardandoAnalise) {
			AlertaEPTV.settings.botoes.push({
				label: 'Analisar E-PTV',
				url: AlertaEPTV.settings.urls.urlEPTVAguardandoAnalise
			});
		}

		$(".divAcoesAlertaEPTV .containerBotoesAlertaEPTV", AlertaEPTV.container).empty();

		var divAcoesAlertaEPTV = $(".divAcoesAlertaEPTV .containerBotoesAlertaEPTV", AlertaEPTV.container);

		$.each(AlertaEPTV.settings.botoes, function (idx, item) {

			botaoAcao = AlertaEPTV.clone();

			var settings = { callBack: AlertaEPTV.onAcaoClick, url: '', label: '' };

			$.extend(settings, item);

			botaoAcao.find("button")
				.data("url", settings.url)
				.text(settings.label)
				.attr('title', settings.label)
				.removeClass("hide")
				.click(settings.callBack);
			divAcoesAlertaEPTV.append(botaoAcao);
		});

		MasterPage.botoes(divAcoesAlertaEPTV);
	},

	exibirMensagemValidacao: function (texto) {
		var mensagem = '\
			<div class=\"mensagemSistema info ui-draggable\" style=\"position: relative;\">\
				<div class=\"textoMensagem \">\
					<a class=\"fecharMensagem\" title=\"Fechar Mensagem\">Fechar Mensagem</a>\
					<p> Mensagem do Sistema</p>\
					<ul>\
						<li>' + texto + '</li>\
					</ul>\
				</div>\
				<div class=\"redirecinamento block containerAcoesAlertaEPTV\">\
					<h5> O que deseja fazer agora ?</h5>\
					<p class=\"hide\">#DESCRICAO</p>\
					<div class=\"coluna100 margem0 divAcoesAlertaEPTV\">\
						<p class=\"floatLeft margem0 append1\"><button title=\"[title]\" class=\"btnTemplateAcao hide ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only\" role=\"button\" aria-disabled=\"false\"><span class=\"ui-button-text\">[ACAO]</span></button></p>\
						<div class=\"containerBotoesAlertaEPTV\"></div>\
					</div>\
				</div>\
			</div>';

		$('.mensagemSistemaHolder')[0].innerHTML = mensagem;
	},

	onAcaoClick: function () {

		var url = $(this).data("url");
		MasterPage.redireciona(url);
	},

	clone: function () {
		var ctr = $('.btnTemplateAcao').parent().clone();
		ctr.find('button').removeClass('btnTemplateAcao');
		return ctr;
	},

	onFecharDaMensagemClick: function () {
		AlertaEPTV.settings.limparContainer = true;
		if ($(AlertaEPTV.container).hasClass("containerAcoesAlertaPTV")) {
			$(AlertaEPTV.container).addClass("hide");
		} else {
			$(".containerAcoesAlertaPTV", AlertaEPTV.container).addClass("hide");
		}
	}
}


//ContainerAcoes = {
//	settings: {
//		urls: {
//			urlGerarPdf: '',
//			urlAlterarSituacao: '',
//			urlEditar: '',
//			urlRequerimento: '',
//			urlProcesso: '',
//			urlDocumento: '',
//			urlTramitacao: '',
//			urlFiscalizacao: '',
//			urlVisualizarPdfFiscalizacao: '',
//			urlGerarRelatorio: '',
//			urlAtribuirExecutor: '',
//			urlConcluirCadastro: '',
//			urlListar: '',
//			urlAtivar: '',
//			urlNovo: ''
//		},
//		botoes: new Array(), //[{ label: '', url: '', callBack: null}]
//		limparContainer: true
//	},
//	container: null,

//	load: function (container, options) {
//		ContainerAcoes.container = container;

//		if (options) {
//			$.extend(ContainerAcoes.settings, options);
//		}

//		$(".fecharMensagem").click(ContainerAcoes.onFecharDaMensagemClick);

//		Mensagem.limparCallbacks['containerAcoes'] = ContainerAcoes.onMensagemLimpar;

//		if ($(ContainerAcoes.container).hasClass("containerAcoes")) {
//			$(ContainerAcoes.container).removeClass("hide");
//		} else {
//			$(".containerAcoes", ContainerAcoes.container).removeClass("hide");
//		}

//		if (ContainerAcoes.settings.urls.urlVisualizarPdfFiscalizacao) {
//			ContainerAcoes.settings.botoes.push({ label: 'Visualizar PDF', url: ContainerAcoes.settings.urls.urlVisualizarPdfFiscalizacao, callBack: ContainerAcoes.onAcaoClickAbrirModal});
//		}

//		if (ContainerAcoes.settings.urls.urlGerarPdf) {
//			ContainerAcoes.settings.botoes.push({ label: 'Gerar PDF', url: ContainerAcoes.settings.urls.urlGerarPdf });
//		}

//		if (ContainerAcoes.settings.urls.urlAlterarSituacao) {
//			ContainerAcoes.settings.botoes.push({ label: 'Alterar situação', url: ContainerAcoes.settings.urls.urlAlterarSituacao });
//		}

//		if (ContainerAcoes.settings.urls.urlAtribuirExecutor) {
//		    ContainerAcoes.settings.botoes.push({ label: 'Atribuir Executor', url: ContainerAcoes.settings.urls.urlAtribuirExecutor });
//		}

//		if (ContainerAcoes.settings.urls.urlGerarRelatorio) {
//		    ContainerAcoes.settings.botoes.push({ label: 'Gerar Relatório', url: ContainerAcoes.settings.urls.urlGerarRelatorio });
//		}

//		if (ContainerAcoes.settings.urls.urlEditar) {
//			ContainerAcoes.settings.botoes.push({ label: 'Editar', url: ContainerAcoes.settings.urls.urlEditar });
//		}

//		if (ContainerAcoes.settings.urls.urlRequerimento) {
//			ContainerAcoes.settings.botoes.push({ label: 'Requerimento', url: ContainerAcoes.settings.urls.urlRequerimento });
//		}

//		if (ContainerAcoes.settings.urls.urlProcesso) {
//			ContainerAcoes.settings.botoes.push({ label: 'Cadastrar Processo', url: ContainerAcoes.settings.urls.urlProcesso });
//		}

//		if (ContainerAcoes.settings.urls.urlDocumento) {
//			ContainerAcoes.settings.botoes.push({ label: 'Cadastrar Documento', url: ContainerAcoes.settings.urls.urlDocumento });
//		}

//		if (ContainerAcoes.settings.urls.urlTramitacao) {
//			ContainerAcoes.settings.botoes.push({ label: 'Tramitação', url: ContainerAcoes.settings.urls.urlTramitacao });
//		}

//		if (ContainerAcoes.settings.urls.urlFiscalizacao) {
//			ContainerAcoes.settings.botoes.push({ label: 'Cadastrar Fiscalização', url: ContainerAcoes.settings.urls.urlFiscalizacao });
//		}

//		if (ContainerAcoes.settings.urls.urlConcluirCadastro) {
//			ContainerAcoes.settings.botoes.push({ label: 'Concluir Cadastro', url: ContainerAcoes.settings.urls.urlConcluirCadastro });
//		}

//		if (ContainerAcoes.settings.urls.urlListar) {
//			ContainerAcoes.settings.botoes.push({ label: 'Listar', url: ContainerAcoes.settings.urls.urlListar});
//		}

//		if (ContainerAcoes.settings.urls.urlAtivar) {
//			ContainerAcoes.settings.botoes.push({ label: 'Ativar', url: ContainerAcoes.settings.urls.urlAtivar });
//		}

//		if (ContainerAcoes.settings.urls.urlNovo) {
//			ContainerAcoes.settings.botoes.push({ label: 'Novo', url: ContainerAcoes.settings.urls.urlNovo });
//		}


//		$(".divAcoesContainer .containerBotoes", ContainerAcoes.container).empty();

//		var divAcoesContainer = $(".divAcoesContainer .containerBotoes", ContainerAcoes.container);

//		$.each(ContainerAcoes.settings.botoes, function (idx, item) {

//			botaoAcao = ContainerAcoes.clone();

//			var settings = { callBack: ContainerAcoes.onAcaoClick, url: '', label: '' };

//			$.extend(settings, item);

//			botaoAcao.find("button")
//				.data("url", settings.url)
//				.text(settings.label)
//				.attr('title', settings.label)
//				.removeClass("hide")
//				.click(settings.callBack);
//			divAcoesContainer.append(botaoAcao);
//		});

//		MasterPage.botoes(divAcoesContainer);
//	},

//	clone: function () {
//		var ctr = $('.btnTemplateAcao').parent().clone();
//		ctr.find('button').removeClass('btnTemplateAcao');
//		return ctr;
//	},

//	onMensagemLimpar: function (msgContainer) {

//		// Apenas limpa o containner de acões que está no mesmo nível e logo após a mensagem que está sendo limpa (ver $.next())
//		if (ContainerAcoes.settings.limparContainer) {
//			$('.mensagemContent', msgContainer).next('.containerAcoes').empty().addClass('hide');
//		}
//	},

//	onAcaoClick: function () {

//		var url = $(this).data("url");
//		MasterPage.redireciona(url);
//	},

//	onAcaoClickAbrirModal: function () {
//		var url = $(this).data("url");
//		Modal.abrir(url,null, null, Modal.tamanhoModalMedia);
//	},

//	onFecharDaMensagemClick: function () {
//		ContainerAcoes.settings.limparContainer = true;
//		if ($(ContainerAcoes.container).hasClass("containerAcoes")) {
//			$(ContainerAcoes.container).addClass("hide");
//		} else {
//			$(".containerAcoes", ContainerAcoes.container).addClass("hide");
//		}
//	}
//}
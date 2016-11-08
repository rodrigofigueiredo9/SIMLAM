/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="masterpage.js" />
/// <reference path="mensagem.js" />

ContainerAcoes = {
	settings: {
		urls: {
			urlGerarPdf: '',
			urlAlterarSituacao: '',
			urlEditar: '',
			urlRequerimento: '',
			urlProcesso: '',
			urlDocumento: '',
			urlTramitacao: ''
		},
		botoes: new Array(), //[{ label: '', url: '', callBack: null}]
		limparContainer: true
	},
	container: null,

	load: function (container, options) {
		ContainerAcoes.container = container;

		if (options) {
			$.extend(ContainerAcoes.settings, options);
		}

		$(".fecharMensagem").click(ContainerAcoes.onFecharDaMensagemClick);

		Mensagem.limparCallbacks['containerAcoes'] = ContainerAcoes.onMensagemLimpar;

		if ($(ContainerAcoes.container).hasClass("containerAcoes")) {
			$(ContainerAcoes.container).removeClass("hide");
		} else {
			$(".containerAcoes", ContainerAcoes.container).removeClass("hide");
		}

		if (ContainerAcoes.settings.urls.urlGerarPdf) {
			ContainerAcoes.settings.botoes.push({ label: 'Gerar PDF', url: ContainerAcoes.settings.urls.urlGerarPdf });
		}

		if (ContainerAcoes.settings.urls.urlAlterarSituacao) {
			ContainerAcoes.settings.botoes.push({ label: 'Alterar situação', url: ContainerAcoes.settings.urls.urlAlterarSituacao });
		}

		if (ContainerAcoes.settings.urls.urlEditar) {
			ContainerAcoes.settings.botoes.push({ label: 'Editar', url: ContainerAcoes.settings.urls.urlEditar });
		}

		if (ContainerAcoes.settings.urls.urlRequerimento) {
			ContainerAcoes.settings.botoes.push({ label: 'Requerimento', url: ContainerAcoes.settings.urls.urlRequerimento });
		}

		if (ContainerAcoes.settings.urls.urlProcesso) {
			ContainerAcoes.settings.botoes.push({ label: 'Cadastrar Processo', url: ContainerAcoes.settings.urls.urlProcesso });
		}

		if (ContainerAcoes.settings.urls.urlDocumento) {
			ContainerAcoes.settings.botoes.push({ label: 'Cadastrar Documento', url: ContainerAcoes.settings.urls.urlDocumento });
		}

		if (ContainerAcoes.settings.urls.urlTramitacao) {
			ContainerAcoes.settings.botoes.push({ label: 'Tramitação', url: ContainerAcoes.settings.urls.urlTramitacao });
		}

		$(".divAcoesContainer .containerBotoes", ContainerAcoes.container).empty();

		var divAcoesContainer = $(".divAcoesContainer .containerBotoes", ContainerAcoes.container);

		$.each(ContainerAcoes.settings.botoes, function (idx, item) {

			botaoAcao = ContainerAcoes.clone();

			var settings = { callBack: ContainerAcoes.onAcaoClick, url: '', label: '' };

			$.extend(settings, item);

			botaoAcao.find("button")
				.data("url", settings.url)
				.text(settings.label)
				.attr('title', settings.label)
				.removeClass("hide")
				.click(settings.callBack);
			divAcoesContainer.append(botaoAcao);
		});

		MasterPage.botoes(divAcoesContainer);
	},

	clone: function () {
		var ctr = $('.btnTemplateAcao').parent().clone();
		ctr.find('button').removeClass('btnTemplateAcao');
		return ctr;
	},

	onMensagemLimpar: function (msgContainer) {

		// Apenas limpa o containner de acões que está no mesmo nível e logo após a mensagem que está sendo limpa (ver $.next())
		if (ContainerAcoes.settings.limparContainer) {
			$('.mensagemContent', msgContainer).next('.containerAcoes').empty().addClass('hide');
		}
	},

	onAcaoClick: function () {

		var url = $(this).data("url");
		MasterPage.redireciona(url);
	},

	onFecharDaMensagemClick: function () {
		ContainerAcoes.settings.limparContainer = true;
		if ($(ContainerAcoes.container).hasClass("containerAcoes")) {
			$(ContainerAcoes.container).addClass("hide");
		} else {
			$(".containerAcoes", ContainerAcoes.container).addClass("hide");
		}
	}
}
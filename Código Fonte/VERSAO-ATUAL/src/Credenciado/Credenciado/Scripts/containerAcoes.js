/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="masterpage.js" />
/// <reference path="mensagem.js" />

ContainerAcoes = {
	settings: {
		botoes: new Array(), //[{ label: '', url: '', callBack: null, adicionarId: false }]
		limparContainer: true,
		id: 0
	},
	container: null,

	load: function (container, options) {
		ContainerAcoes.container = container;

		if (options) {
			$.extend(ContainerAcoes.settings, options);
		}

		ContainerAcoes.settings.id = parseInt($('.hdnIdAcao', ContainerAcoes.container).val());
		$(".fecharMensagem").click(ContainerAcoes.onFecharDaMensagemClick);
		Mensagem.limparCallbacks['containerAcoes'] = ContainerAcoes.onMensagemLimpar;

		if ($(ContainerAcoes.container).hasClass("containerAcoes")) {
			$(ContainerAcoes.container).removeClass("hide");
		} else {
			$(".containerAcoes", ContainerAcoes.container).removeClass("hide");
		}

		$(".divAcoesContainer .containerBotoes", ContainerAcoes.container).empty();
		var divAcoesContainer = $(".divAcoesContainer .containerBotoes", ContainerAcoes.container);

		$.each(ContainerAcoes.settings.botoes, function (idx, item) {
		    botaoAcao = ContainerAcoes.clone();
		    var settings = null;          
		    if (item.abrirModal){
		        settings = { label: '', url: '', callBack: item.abrirModal, adicionarId: false };
		    }
		    else{
		        settings = { label: '', url: '', callBack: ContainerAcoes.onAcaoClick, adicionarId: false };
		    }			

			$.extend(settings, item);

			botaoAcao.find("button")
				.text(settings.label)
				.attr('title', settings.label)
				.data("url", settings.url + (settings.adicionarId ? ('/' + ContainerAcoes.settings.id) : ''))
				.click(settings.callBack)
				.removeClass("hide");
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

	onAcaoClickAbrirModal: function () {
		var url = $(this).data("url");
		Modal.abrir(url, null, function (container) {
			Mensagem.limpar();
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalMedia);
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
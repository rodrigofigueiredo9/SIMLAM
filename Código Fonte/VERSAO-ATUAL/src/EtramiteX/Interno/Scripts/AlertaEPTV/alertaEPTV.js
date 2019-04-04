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
					<div class=\"coluna100 margem0 divAcoesAlertaEPTV\">\
						<p class=\"floatLeft margem0 append1\"><button title=\"[title]\" class=\"btnTemplateAcaoAlertaEPTV hide ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only\" role=\"button\" aria-disabled=\"false\"><span class=\"ui-button-text\">[ACAO]</span></button></p>\
						<div class=\"containerBotoesAlertaEPTV\"></div>\
					</div>\
				</div>\
			</div>';

		$('.mensagemSistemaHolder').append(mensagem);
	},

	onAcaoClick: function () {

		var url = $(this).data("url");
		MasterPage.redireciona(url);
	},

	clone: function () {
		var ctr = $('.btnTemplateAcaoAlertaEPTV').parent().clone();
		ctr.find('button').removeClass('btnTemplateAcaoAlertaEPTV');
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
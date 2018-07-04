/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="masterpage.js" />
/// <reference path="mensagem.js" />

AlertaChegadaMensagemEPTV = {
	settings: {
		urls: {
			urlComunicadorPTV: ''
		},
		id: 0,
		botoes: new Array(), //[{ label: '', url: '', callBack: null}]
		limparContainer: true
	},
	container: null,

	load: function (container, options) {
		AlertaChegadaMensagemEPTV.container = container;

		if (options) {
			$.extend(AlertaChegadaMensagemEPTV.settings, options);
		}

		$(".fecharMensagem").click(AlertaChegadaMensagemEPTV.onFecharDaMensagemClick);

		Mensagem.limparCallbacks['containerAcoesAlertaChegadaMensagemPTV'] = AlertaChegadaMensagemEPTV.onMensagemLimpar;

		if ($(AlertaChegadaMensagemEPTV.container).hasClass("containerAcoesAlertaChegadaMensagemPTV")) {
			$(AlertaChegadaMensagemEPTV.container).removeClass("hide");
		} else {
			$(".containerAcoesAlertaChegadaMensagemPTV", AlertaChegadaMensagemEPTV.container).removeClass("hide");
		}

		//if (ContainerAcoes.settings.urls.urlVisualizarPdfFiscalizacao) {
		//	ContainerAcoes.settings.botoes.push({ label: 'Visualizar PDF', url: ContainerAcoes.settings.urls.urlVisualizarPdfFiscalizacao, callBack: ContainerAcoes.onAcaoClickAbrirModal });
		//}

		if (AlertaChegadaMensagemEPTV.settings.urls.urlComunicadorPTV) {
			AlertaChegadaMensagemEPTV.settings.botoes.push({
				label: 'Ver mensagem',
				url: AlertaChegadaMensagemEPTV.settings.urls.urlComunicadorPTV,
				idPTV: AlertaChegadaMensagemEPTV.settings.id
			});
		}

		$(".divAcoesAlertaChegadaMensagemEPTV .containerBotoesAlertaChegadaMensagemEPTV", AlertaChegadaMensagemEPTV.container).empty();

		var divAcoesAlertaChegadaMensagemEPTV = $(".divAcoesAlertaChegadaMensagemEPTV .containerBotoesAlertaChegadaMensagemEPTV", AlertaChegadaMensagemEPTV.container);

		$.each(AlertaChegadaMensagemEPTV.settings.botoes, function (idx, item) {

			botaoAcao = AlertaChegadaMensagemEPTV.clone();

			var settings = { callBack: AlertaChegadaMensagemEPTV.onAcaoClick, url: '', idPTV: '', label: '' };

			$.extend(settings, item);

			botaoAcao.find("button")
				.data({ url: settings.url, idPTV: settings.idPTV })
				.text(settings.label)
				.attr('title', settings.label)
				.removeClass("hide")
				.click(settings.callBack);
			divAcoesAlertaChegadaMensagemEPTV.append(botaoAcao);
		});

		MasterPage.botoes(divAcoesAlertaChegadaMensagemEPTV);
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
				<div class=\"redirecinamento block containerAcoesAlertaChegadaMensagemEPTV\">\
					<h5> O que deseja fazer agora ?</h5>\
					<div class=\"coluna100 margem0 divAcoesAlertaChegadaMensagemEPTV\">\
						<p class=\"floatLeft margem0 append1\"><button title=\"[title]\" class=\"btnTemplateAcaoAlertaChegadaMensagemEPTV hide ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only\" role=\"button\" aria-disabled=\"false\"><span class=\"ui-button-text\">[ACAO]</span></button></p>\
						<div class=\"containerBotoesAlertaChegadaMensagemEPTV\"></div>\
					</div>\
				</div>\
			</div>';

		$('.mensagemSistemaHolder').append(mensagem);
	},

	onAcaoClick: function () {
		var id = $(this).data("idPTV");
		var url = $(this).data("url");
		
		Modal.abrir(
			url,
			{ id: id },
			function (container) {
				AlertaChegadaMensagemEPTV.load(container, {
					callBackSalvar: AlertaChegadaMensagemEPTV.onAcaoClick
				});
			},
			Modal.tamanhoModalMedia);
	},

	clone: function () {
		var ctr = $('.btnTemplateAcaoAlertaChegadaMensagemEPTV').parent().clone();
		ctr.find('button').removeClass('btnTemplateAcaoAlertaChegadaMensagemEPTV');
		return ctr;
	},

	onFecharDaMensagemClick: function () {
		AlertaChegadaMensagemEPTV.settings.limparContainer = true;
		if ($(AlertaChegadaMensagemEPTV.container).hasClass("containerAcoesAlertaChegadaMensagemPTV")) {
			$(AlertaChegadaMensagemEPTV.container).addClass("hide");
		} else {
			$(".containerAcoesAlertaChegadaMensagemPTV", AlertaChegadaMensagemEPTV.container).addClass("hide");
		}
	}
}
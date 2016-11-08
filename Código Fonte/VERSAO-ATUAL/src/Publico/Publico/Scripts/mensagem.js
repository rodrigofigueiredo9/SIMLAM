/// <reference path="Lib/JQuery/jquery-1.10.1-vsdoc.js" />

Mensagem = {

	load: function () {
		/////Script para expandir e recolher a mensagem
		$('body').delegate(".linkVejaMaisMensagens", "click", function () {
			if ($(this).parent().find('.textoMensagem').height() > 75) {
				$(this).parent().find('.textoMensagem').animate({ height: 75 }, 'fast');
				$(this).toggleClass('ativo');
			} else {
				$(this).parent().find('.textoMensagem').animate({ height: $('.textoMensagem')[0].scrollHeight }, 'fast');
				$(this).toggleClass('ativo');
			}
		});

		/////Script para fechar a mensagem
		$('body').delegate('.fecharMensagem', "click", function () {
			$(this).parent().parent().slideUp('fast');
		});
	},

	template: function () {
		return $("<div class='mensagemSistema'>" +
		"<div class='textoMensagem'>" +
			"<a class='fecharMensagem' title='Fechar Mensagem'>Fechar Mensagem</a>" +
			"<p>Mensagem do Sistema</p>" +
			"<ul></ul>" +
		"</div>" +
		"<a class='linkVejaMaisMensagens' style='display: none;' title='Clique aqui para ver mais detalhes desta mensagem'>Clique aqui para ver mais detalhes desta mensagem</a>" +
	"</div>");
	},

	limparCallbacks: {},

	limpar: function (controle) {
		if (Mensagem.limparCallbacks) {
			$.each(Mensagem.limparCallbacks, function (nome, funcao) {
				if (funcao) {
					funcao(controle);
				}
			});
		}

		controle.find(".mensagemSistema").each(function () { $(this).remove(); });
		$('.erroCampo', controle).removeClass('erroCampo');
	},

	gerarHtml: function (arrayMsg, tipo) {

		var arrayCss = ["erro", "alerta", "sucesso", "info"];
		var msg = Mensagem.template();
		msg.addClass(arrayCss[tipo]);
		var ul = msg.find("ul");
		var totalLength = 0;

		for (var i = 0; i < arrayMsg.length; i++) {
			ul.append($("<li></li>").text(arrayMsg[i].Texto));
			totalLength += arrayMsg[i].Texto.length;
		}

		if (totalLength >= 300 || arrayMsg.length >= 4) {
			msg.find(".linkVejaMaisMensagens").show();
		}

		return msg;
	},

	marcarCampo: function (controle, arrayMsg) {
		for (var i = 0; i < arrayMsg.length; i++) {
			if ((typeof arrayMsg[i].Campo != 'undefined') && arrayMsg[i].Campo !== '') {
				controle.find("#" + arrayMsg[i].Campo).addClass("erroCampo");
			}
		}
	},

	scrollTop: function (container) {
		var modalBox = $(container).closest('.boxModal');
		if (modalBox.length > 0) { // dentro de modal
			modalBox.scrollTop(0, 'slow');
		} else { // na master
			$('html, body').animate({ scrollTop: 0 }, 'slow');
		}
	},

	gerar: function (controle, arrayMsg, manter) {

		var msgErro = new Array();
		var msgInfo = new Array();
		var msgAdv = new Array();
		var msgSuss = new Array();

		/*
		Informacao,
		Confirmacao,
		Sucesso,
		Advertencia,
		Erro
		*/

		if (arrayMsg) {
			for (var idx = 0; idx < arrayMsg.length; idx++) {
				switch (arrayMsg[idx].Tipo) {
					case 0:
						msgInfo.push(arrayMsg[idx]);
						break;

					case 2:
						msgSuss.push(arrayMsg[idx]);
						break;

					case 3:
						msgAdv.push(arrayMsg[idx]);
						break;

					case 4:
						msgErro.push(arrayMsg[idx]);
						break;

					default:
						break;
				}
			}
		}

		if (!manter) {
			Mensagem.limpar(controle);
		}

		if (msgSuss != null && msgSuss.length > 0) {
			controle.prepend(Mensagem.gerarHtml(msgSuss, 2));
			Mensagem.marcarCampo(controle, msgSuss);
		}
		if (msgAdv != null && msgAdv.length > 0) {
			controle.prepend(Mensagem.gerarHtml(msgAdv, 1));
			Mensagem.marcarCampo(controle, msgAdv);
		}
		if (msgInfo != null && msgInfo.length > 0) {
			controle.prepend(Mensagem.gerarHtml(msgInfo, 3));
			Mensagem.marcarCampo(controle, msgInfo);
		}
		if (msgErro != null && msgErro.length > 0) {
			controle.prepend(Mensagem.gerarHtml(msgErro, 0));
			Mensagem.marcarCampo(controle, msgErro);
		}

		if (arrayMsg != null && arrayMsg.length > 0) {
			Mensagem.scrollTop(controle);
		}
	},

	replace: function (mensagem, chave, textoNovo) {
		var mensagem = jQuery.extend(true, {}, mensagem);
		mensagem.Texto = mensagem.Texto.replace(chave, textoNovo);
		return mensagem;
	}
}
$(document).ready(Mensagem.load);
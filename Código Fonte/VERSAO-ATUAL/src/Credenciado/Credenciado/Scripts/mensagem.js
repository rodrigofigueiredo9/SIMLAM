/// <reference path="Lib/JQuery/jquery-1.10.1-vsdoc.js" />

Mensagem = {
	load: function () {
		/////Script para expandir e recolher a mensagem
		$(document).delegate('.linkVejaMaisMensagens', 'click', function () {
			if ($(this).parent().find('.textoMensagem').height() > 75) {
				$(this).parent().find('.textoMensagem').animate({ height: 75 }, 'fast');
				$(this).toggleClass('ativo');

			} else {
				$(this).parent().find('.textoMensagem').animate({ height: $(this).parent().find('.textoMensagem')[0].scrollHeight }, 'fast');
				$(this).toggleClass('ativo');
			}
		});

		$('.mensagemSistemaHolder .mensagemSistema').draggable({ handle: '.textoMensagem' });

		/////Script para fechar a mensagem
		$(document).delegate('.fecharMensagem', 'click', function () {
			$(this).parent().parent().slideUp('fast',
				function () {
					$(this).remove();
				}
			);
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

		$('.erroCampo', controle).removeClass('erroCampo');

		if ($('.mensagemSistemaHolder').length > 0) {
			controle = $('.mensagemSistemaHolder');
		}

		controle.find(".mensagemSistema").each(function () { $(this).remove(); });
	},

	gerarHtml: function (arrayMsg, tipo) {

		var arrayCss = ["erro", "alerta", "sucesso", "info"];
		var msg = Mensagem.template();
		msg.addClass(arrayCss[tipo]);
		var ul = msg.find("ul");
		var totalLength = 0;
		var msgLength = 0;

		for (var i = 0; i < arrayMsg.length; i++) {
			var textoQuebrado = arrayMsg[i].Texto.split('\n');
			if (textoQuebrado.length > 0) {
				textoQuebrado.forEach(function (texto) {
					ul.append($("<li></li>").text(texto));
				});
			}
			else {
				ul.append($("<li></li>").text(arrayMsg[i].Texto));
			}
			totalLength += arrayMsg[i].Texto.length;
			msgLength += textoQuebrado.length > 0 ? textoQuebrado.length : 1;
		}

		if (totalLength >= 230 || msgLength >= 3) {
			msg.find(".linkVejaMaisMensagens").show();
		}

		msg.draggable({ handle: '.textoMensagem', cancel: 'ul' });

		return msg;
	},

	marcarCampo: function (controle, arrayMsg) {
		for (var i = 0; i < arrayMsg.length; i++) {
			if ((typeof arrayMsg[i].Campo != 'undefined') && arrayMsg[i].Campo !== '') {
				controle.find("#" + arrayMsg[i].Campo).addClass("erroCampo");
			}
		}
	},

	gerar: function (controle, arrayMsg, manter) {

		var msgErro = new Array();
		var msgInfo = new Array();
		var msgAdv = new Array();
		var msgSuss = new Array();

		var msgContainer = $('.mensagemSistemaHolder');

		if (arrayMsg) {
			for (var idx = 0; idx < arrayMsg.length; idx++) {
				switch (arrayMsg[idx].Tipo) {
					case 0://Informacao
						msgInfo.push(arrayMsg[idx]);
						break;

					case 2://Sucesso
						msgSuss.push(arrayMsg[idx]);
						break;

					case 3://Advertencia
						msgAdv.push(arrayMsg[idx]);
						break;

					case 4://Erro
						msgErro.push(arrayMsg[idx]);
						break;

					default://Confirmacao/Ajuda
						break;
				}
			}
		}

		if (!manter) {
			Mensagem.limpar(msgContainer);
		}

		if (msgSuss != null && msgSuss.length > 0) {
			msgContainer.prepend(Mensagem.gerarHtml(msgSuss, 2));
			Mensagem.marcarCampo(controle, msgSuss);
		}
		if (msgAdv != null && msgAdv.length > 0) {
			msgContainer.prepend(Mensagem.gerarHtml(msgAdv, 1));
			Mensagem.marcarCampo(controle, msgAdv);
		}
		if (msgInfo != null && msgInfo.length > 0) {
			msgContainer.prepend(Mensagem.gerarHtml(msgInfo, 3));
			Mensagem.marcarCampo(controle, msgInfo);
		}
		if (msgErro != null && msgErro.length > 0) {
			msgContainer.prepend(Mensagem.gerarHtml(msgErro, 0));
			Mensagem.marcarCampo(controle, msgErro);
		}
	},

	replace: function (mensagem, chave, textoNovo) {
		var mensagem = jQuery.extend(true, {}, mensagem);
		mensagem.Texto = mensagem.Texto.replace(chave, textoNovo);
		return mensagem;
	}
}
$(document).ready(Mensagem.load);
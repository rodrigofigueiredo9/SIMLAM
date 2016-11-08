/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

ComunicadorPTV = {
	settings: {
		urls: {
			urlEnviar: null,
		},
		Mensagens: null,
		manterEnviar: false,
		callBackSalvar: null
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(ComunicadorPTV.settings, options);
		}
		ComunicadorPTV.container = MasterPage.getContent(container);

		if (ComunicadorPTV.settings.manterEnviar) {
			Modal.defaultButtons(ComunicadorPTV.container, ComunicadorPTV.enviar, "Enviar");
		} else {
			Modal.defaultButtons(ComunicadorPTV.container);
		}

		Aux.setarFoco($('.txtJustificativa', ComunicadorPTV.container));
		Aux.scrollBottom(ComunicadorPTV.container);
	},

	//----------ANEXOS - ENVIAR ARQUIVO---------------
	onEnviarAnexoArquivoClick: function (url) {
		var nome = "enviando ...";

		var nomeArquivo = $('.inputFile').val();

		if (nomeArquivo === '') {
			Mensagem.gerar(ComunicadorPTV.container, [ComunicadorPTV.settings.Mensagens.ArquivoObrigatorio]);
			return;
		}

		if (nomeArquivo !== '') {
			var tam = nomeArquivo.length - 4;
			if (nomeArquivo.toLowerCase().substr(tam) !== ".zip" && nomeArquivo.toLowerCase().substr(tam) !== ".rar") {
				Mensagem.gerar(ComunicadorPTV.container, [ComunicadorPTV.settings.Mensagens.ArquivoTipoInvalido]);
				return;
			}
		}

		var inputFile = $('.inputFileDiv input[type="file"]');

		inputFile.attr("id", "ArquivoId");

		FileUpload.upload(url, inputFile, ComunicadorPTV.msgArqEnviado);

		$('.inputFile').val('');
	},

	msgArqEnviado: function (controle, retorno, isHtml) {
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoNome', ComunicadorPTV.container).val(ret.Arquivo.Nome);
			$('.hdnAnexoArquivoJson', ComunicadorPTV.container).val(JSON.stringify(ret.Arquivo));

			$('.spanInputFile', ComunicadorPTV.container).addClass('hide');
			$('.txtArquivoNome', ComunicadorPTV.container).removeClass('hide');

			$('.btnArq', ComunicadorPTV.container).addClass('hide');
			$('.btnArqLimpar', ComunicadorPTV.container).removeClass('hide');

		} else {
			ComunicadorPTV.onLimparArquivoClick();
		}

		Mensagem.gerar(MasterPage.getContent(ComunicadorPTV.container), ret.Msg);
	},

	onLimparArquivo: function () {

		//implementar Limpar
		$('.txtArquivoNome', ComunicadorPTV.container).data('arquivo', null);
		$('.txtArquivoNome', ComunicadorPTV.container).val("");
		$('.hdnAnexoArquivoJson', ComunicadorPTV.container).val("");

		$('.spanInputFile', ComunicadorPTV.container).removeClass('hide');
		$('.txtArquivoNome', ComunicadorPTV.container).addClass('hide');

		$('.btnArq', ComunicadorPTV.container).removeClass('hide');
		$('.btnArqLimpar', ComunicadorPTV.container).addClass('hide');

		$('.lnkArquivo', ComunicadorPTV.container).remove();
	},
	//----------ANEXOS - ENVIAR ARQUIVO---------------

	enviar: function () {
		if ($('.txtJustificativa', ComunicadorPTV.container).val() == '') {
			Mensagem.gerar(ComunicadorPTV.container, [ComunicadorPTV.settings.Mensagens.JustificativaObrigatoria]);
			return false;
		}

		var ptvComunicador = {
			Id: $('.hdnId', ComunicadorPTV.container).val(),
			PTVId: $('.hdnIdPTV', ComunicadorPTV.container).val(),
			ArquivoInternoId: $('.hdnArqInternoId', ComunicadorPTV.container).val(),
			ArquivoCredenciadoId: $('.hdnArqCredenciadoId', ComunicadorPTV.container).val(),
			liberadoCredenciado: $('.hdnLiberadoCredenciado', ComunicadorPTV.container).val(),
			ArquivoCredenciado: {},
			Conversas: new Array()
		}

		ptvComunicador.Conversas.push({
			Texto: $('.txtJustificativa', ComunicadorPTV.container).val(),
			ArquivoNome: $('.txtArquivoNome', ComunicadorPTV.container).val()
		});

		if ($('.hdnAnexoArquivoJson', ComunicadorPTV.container).val() != "") {
			ptvComunicador.ArquivoCredenciado = JSON.parse($('.hdnAnexoArquivoJson', ComunicadorPTV.container).val())
		}

		MasterPage.carregando(true);
		$.ajax({
			url: ComunicadorPTV.settings.urls.urlEnviar,
			data: JSON.stringify(ptvComunicador),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ComunicadorPTV.container, response.Msg);
				}

				if (response.EhValido) {
					Modal.fechar(ComunicadorPTV.container);

					//if (ComunicadorPTV.settings.callBackSalvar != null) {
					//	ComunicadorPTV.settings.callBackSalvar();
					//}
				}
			}
		});
		MasterPage.carregando(false);
	}
}
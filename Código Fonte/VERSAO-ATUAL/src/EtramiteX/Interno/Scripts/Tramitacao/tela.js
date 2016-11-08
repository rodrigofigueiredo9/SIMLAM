/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

EnviarTela = {
	settings: {
		urls: {
			enviar: '',
			setoresRemetente: '',
			funcionariosDestinatario: '',
			todasTramitacoes: '',
			addTramitacaoNumeroProcDoc: '',
			carregarPartialTemplateTramitacoes: '',
			visualizarHistorico: '',
			abrirPdf: '',
			validarTipoSetor: ''
		},

		msgs: {
		}
	},

	content: null,

	load: function (content, options) {
		EnviarTela.content = content;

		if (options) {
			$.extend(EnviarTela.settings, options);
		}

		Enviar.load($('.enviarPartial', content), {

			onEnviar: EnviarTela.onEnviar,
			urls: EnviarTela.settings.urls,
			msgs: EnviarTela.settings.msgs
		});

		//$('.btnEnviar', EnviarTela.content).click(EnviarTela.enviarClik);
		$('.btnEnviar', EnviarTela.content).click(EnviarTela.enviarClick);

	},

	enviarClick: function () {
		Enviar.enviar();
	},

	onEnviar: function () {
	}
}
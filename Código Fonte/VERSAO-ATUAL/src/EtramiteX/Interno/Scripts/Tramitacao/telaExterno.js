/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

EnviarTelaExterno = {
	settings: {
		urls: {
			enviar: '',
			setoresRemetente: '',
			todasTramitacoes: '',
			addTramitacaoNumeroProcDoc: '',
			carregarPartialTemplateTramitacoes: ''
		},

		msgs: {
		}
	},

	content: null,

	load: function (content, options) {
		EnviarTelaExterno.content = content;

		if (options) {
			$.extend(EnviarTelaExterno.settings, options);
		}

		EnviarExterno.load($('.enviarExternoPartial', content), {
			onEnviar: EnviarTelaExterno.onEnviar,
			urls: EnviarTelaExterno.settings.urls,
			msgs: EnviarTelaExterno.settings.msgs
		});

		$('.btnEnviar', EnviarTelaExterno.content).click(EnviarTelaExterno.enviarClick);
	},

	enviarClick: function () {
		EnviarExterno.enviar();
	},

	onEnviar: function () {
	}}
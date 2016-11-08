	/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
	/// <reference path="../masterpage.js" />
	/// <reference path="../jquery.json-2.2.min.js" />

	EnviarRegistroTela = {
		settings: {
			urls: {
				enviarRegistro: '',
				remetentes: '',
				funcionariosDestinatario: '',
				todasTramitacoesRegistro: '',
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
			EnviarRegistroTela.content = content;

			if (options) {
				$.extend(EnviarRegistroTela.settings, options);
			}

			EnviarRegistro.load($('.enviarRegistroPartial', content), {

				onEnviar: EnviarRegistroTela.onEnviar,
				urls: EnviarRegistroTela.settings.urls,
				msgs: EnviarRegistroTela.settings.msgs
			});

			$('.btnEnviar', EnviarRegistroTela.content).click(EnviarRegistroTela.enviarClick);

		},

		enviarClick: function () {
			EnviarRegistro.enviar();
		},

		onEnviar: function () {
		}
	}
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

TituloDeclaratorioConfiguracao = {
	settings: {
		urls: {
			salvar: ''
		}
	},
	container: null,

	load: function (container, options) {
		TituloDeclaratorioConfiguracao.container = container;
		if (options) {
			$.extend(TituloDeclaratorioConfiguracao.settings, options);
		}
	}
};
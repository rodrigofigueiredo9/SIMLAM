/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

LicencaOperacaoFomento = {
	container: null,

	load: function (especificidadeRef) {
		LicencaOperacaoFomento.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
		TituloCondicionante.load($('.condicionantesContainer', LicencaOperacaoFomento.container));
	},

	obterLicencaOperacaoFomento: function (protocolo) { },

	obterObjeto: function () { return {}; }
};

Titulo.settings.especificidadeLoadCallback = LicencaOperacaoFomento.load;
Titulo.settings.obterEspecificidadeObjetoFunc = LicencaOperacaoFomento.obterObjeto;
Titulo.addCallbackProtocolo(LicencaOperacaoFomento.obterLicencaOperacaoFomento);
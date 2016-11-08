/// <reference path="../../Lib/JQuery/jquery.json - 2.2.min.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../mensagem.js" />

Coordenada = {
	settings: {
		northing: '',
		easting: '',
		northing2: '',
		easting2: '',
		empreendimentoNorthing: '',
		empreendimentoEasting: '',
		callBackSalvarCoordenada: null,
		pagemode: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Coordenada.settings, options); }
		Coordenada.container = MasterPage.getContent(container);
	}
}
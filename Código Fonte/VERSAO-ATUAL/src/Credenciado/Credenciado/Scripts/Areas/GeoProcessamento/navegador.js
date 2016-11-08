/// <reference path="../../Lib/JQuery/jquery.json - 2.2.min.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../mensagem.js" />

Navegador = {
	settings:
	{
		urls: {
			atualizarSessao: ''
		},
		coordenada: {
			northing: '0',
			easting: '0',
			easting2: '0',
			northing2: '0'
		},
		id: 0,
		tipo: 0,
		modo: 1,
		empreendimentoEasting: '',
		empreendimentoNorthing: '',
		pagemode: null,
		onCancelar: null,
		onProcessar: null,
		setSituacaoProcessamento: null,
		onBaixarArquivo: null,
		obterSituacaoInicial: null,
		obterAreaAbrangencia: null
	},
	container: null,
	navegadorElemento: null,

	load: function (container, options) {

		if (options) { $.extend(Navegador.settings, options); }

		Navegador.container = MasterPage.getContent(container);
	},

	atualizarSessao: function () {
		$.ajax({
			url: Navegador.settings.url,
			type: 'GET',
			contentType: 'application/json; charset=utf-8',
			async: true,
			cache: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Navegador.container);
			}
		});
	}
}
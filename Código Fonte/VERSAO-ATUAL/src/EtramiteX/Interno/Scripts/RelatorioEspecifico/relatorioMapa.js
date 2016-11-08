/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

RelatorioMapa = {
	settings: {
		urlPdf: null,
		urlXlsx: null,
		Mensagens: null,
	},
	container: null,
	

	load: function (container, options) {
		if (options) { $.extend(RelatorioMapa.settings, options); }
	    RelatorioMapa.container = MasterPage.getContent(container);
	    RelatorioMapa.container.listarAjax();

	    RelatorioMapa.container.delegate('.btnRelatorioPDF', 'click', RelatorioMapa.PDF);
	    RelatorioMapa.container.delegate('.btnRelatorioExcel', 'click', RelatorioMapa.Excel);

	    Aux.setarFoco(container);
	},

	obter: function () {
		var gridContainer = $('.gridLocalVistoria tbody', RelatorioMapa.container);

		var RelatorioMapaObj = {
			TipoRelatorio: $('.ddlTipoRelatorio option:selected', RelatorioMapa.container).val(),
			DataInicial: $('.txtDataInicial', RelatorioMapa.container).val(),
			DataFinal: $('.txtDataFinal', RelatorioMapa.container).val()
		};

		return RelatorioMapaObj;
	},

	validar: function (objRelatorio) {
		if (objRelatorio.TipoRelatorio == 0){
			Mensagem.gerar(MasterPage.getContent(RelatorioMapa.container), [RelatorioMapa.settings.Mensagens.TipodoRelatorio]);
			$('.ddlTipoRelatorio', RelatorioMapa.container).focus();
			return false;
		}
		if (objRelatorio.DataInicial == "") {
			Mensagem.gerar(MasterPage.getContent(RelatorioMapa.container), [RelatorioMapa.settings.Mensagens.DataInicialObrigatorio]);
			$('.txtDataInicial', RelatorioMapa.container).focus();
			return false;
		}

		if (objRelatorio.DataFinal == "") {
			Mensagem.gerar(MasterPage.getContent(RelatorioMapa.container), [RelatorioMapa.settings.Mensagens.DataFinalObrigatorio]);
			$('.txtDataFinal', RelatorioMapa.container).focus();
			return false;
		}
		return true
	},

	Excel: function () {
		var objRelatorio = RelatorioMapa.obter();
		if (RelatorioMapa.validar(objRelatorio)) {
			MasterPage.redireciona(RelatorioMapa.settings.urlXlsx + '?paramsJson=' + JSON.stringify(objRelatorio));
		}
		else
			return;
	},

	PDF: function () {
		var objRelatorio = RelatorioMapa.obter();
		if (RelatorioMapa.validar(objRelatorio)) {
		MasterPage.redireciona(RelatorioMapa.settings.urlPdf + '?paramsJson=' + JSON.stringify(objRelatorio));
		}
		else
			return;
	}


}
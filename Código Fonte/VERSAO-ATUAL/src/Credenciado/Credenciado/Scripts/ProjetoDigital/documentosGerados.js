/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

ProjetoDigitalDocumentoGerados = {
	urlRoteiro: null,
	urlRequerimento: null,
	urlRelatorioTecnico: null,
	urlArquivo: null,
	urlFichaInscricaoUnidadeProducao: null,
	urlFichaInscricaoUnidadeConsolidacao: null,
	projetoDigitalID: 0,
	tiposPDF: {},
	container: null,

	load: function (container) {
		ProjetoDigitalDocumentoGerados.container = MasterPage.getContent(container);
		ProjetoDigitalDocumentoGerados.container.delegate('.btnPdf', 'click', ProjetoDigitalDocumentoGerados.abrir);
		ProjetoDigitalDocumentoGerados.container.delegate('.btnCroqui', 'click', ProjetoDigitalDocumentoGerados.abrirCroqui);
	},

	obterItemJson: function (container) {
		return $.parseJSON($(container).closest('tr').find('.itemJson').val());
	},

	abrir: function () {
		var item = ProjetoDigitalDocumentoGerados.obterItemJson(this);

		switch (item.Tipo) {
			case ProjetoDigitalDocumentoGerados.tiposPDF.Roteiro:
				MasterPage.redireciona(ProjetoDigitalDocumentoGerados.urlRoteiro + '/' + item.Id);
				break;

			case ProjetoDigitalDocumentoGerados.tiposPDF.Requerimento:
				MasterPage.redireciona(ProjetoDigitalDocumentoGerados.urlRequerimento + '/' + item.Id);
				break;

			case ProjetoDigitalDocumentoGerados.tiposPDF.RelatorioTecnico:
				MasterPage.redireciona(ProjetoDigitalDocumentoGerados.urlRelatorioTecnico + '/' + ProjetoDigitalDocumentoGerados.projetoDigitalID + '?caracterizacaoTipo=' + item.Id);
				break;

			case ProjetoDigitalDocumentoGerados.tiposPDF.FichaInscricaoUnidadeProducao:
				MasterPage.redireciona(ProjetoDigitalDocumentoGerados.urlFichaInscricaoUnidadeProducao + '/' + ProjetoDigitalDocumentoGerados.projetoDigitalID);
				break;

			case ProjetoDigitalDocumentoGerados.tiposPDF.FichaInscricaoUnidadeConsolidacao:
				MasterPage.redireciona(ProjetoDigitalDocumentoGerados.urlFichaInscricaoUnidadeConsolidacao + '/' + ProjetoDigitalDocumentoGerados.projetoDigitalID);
				break;
		}
	},

	abrirCroqui: function () {
		var anexoId = $(this).closest('tr').find('.anexoId').val();
		MasterPage.redireciona(ProjetoDigitalDocumentoGerados.urlArquivo + '/' + anexoId);
	}
}
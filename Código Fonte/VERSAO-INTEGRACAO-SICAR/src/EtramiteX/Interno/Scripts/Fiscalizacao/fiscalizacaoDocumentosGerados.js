﻿/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

FiscalizacaoDocumentosGerados = {
	settings: {
		urls: {
			download: '',
			pdfAuto: '',
			pdfLaudo: '',
			pdfAcompanhamento: ''
		},
		situacao: 0
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(FiscalizacaoDocumentosGerados.settings, options); }
		FiscalizacaoDocumentosGerados.container = container;

		Mensagem.limpar();

		FiscalizacaoDocumentosGerados.container.delegate('.btnAnexo', 'click', FiscalizacaoDocumentosGerados.obterAnexo);
		FiscalizacaoDocumentosGerados.container.delegate('.btnPdfAuto', 'click', FiscalizacaoDocumentosGerados.obterPdfAuto);
		FiscalizacaoDocumentosGerados.container.delegate('.btnPdfLaudo', 'click', FiscalizacaoDocumentosGerados.obterPdfLaudo);
		FiscalizacaoDocumentosGerados.container.delegate('.btnAnexoCroqui', 'click', FiscalizacaoDocumentosGerados.obterAnexo);
		FiscalizacaoDocumentosGerados.container.delegate('.btnAcompanhamento', 'click', FiscalizacaoDocumentosGerados.obterAcompanhamento);

		FiscalizacaoDocumentosGerados.container.delegate('.btnAnexoCancelado', 'click', FiscalizacaoDocumentosGerados.obterAnexoCancelado);
		FiscalizacaoDocumentosGerados.container.delegate('.btnPdfAutoCancelado', 'click', FiscalizacaoDocumentosGerados.obterPdfAutoCancelado);
		FiscalizacaoDocumentosGerados.container.delegate('.btnPdfLaudoCancelado', 'click', FiscalizacaoDocumentosGerados.obterPdfLaudoCancelado);
		FiscalizacaoDocumentosGerados.container.delegate('.btnAnexoCroquiCancelado', 'click', FiscalizacaoDocumentosGerados.obterAnexoCancelado);
	},

	obterAnexo: function () {
		MasterPage.redireciona(FiscalizacaoDocumentosGerados.settings.urls.download + "/" + $('.hdnArquivoId', $(this).closest('td')).val());
	},

	obterPdfAuto: function () {
		MasterPage.redireciona(FiscalizacaoDocumentosGerados.settings.urls.pdfAuto + "/" + $('.hdnFiscalizacaoId', FiscalizacaoDocumentosGerados.container).val());
	},

	obterPdfLaudo: function () {
		MasterPage.redireciona(FiscalizacaoDocumentosGerados.settings.urls.pdfLaudo + "/" + $('.hdnFiscalizacaoId', FiscalizacaoDocumentosGerados.container).val());
	},

	obterAcompanhamento: function () {
		MasterPage.redireciona(FiscalizacaoDocumentosGerados.settings.urls.pdfAcompanhamento + "/" + $('.hdnAcompanhamentoId', $(this).closest('td')).val());
	},

	obterAnexoCancelado: function () {
		var url = FiscalizacaoDocumentosGerados.settings.urls.download + "/?id=" + $('.hdnArquivoId', $(this).closest('td')).val()
																		+ '&historico=' + $('.hdnHistoricoId', $(this).closest('td')).val();
		MasterPage.redireciona(url);
	},

	obterPdfAutoCancelado: function () {
		var url = FiscalizacaoDocumentosGerados.settings.urls.pdfAuto + "/?arquivo=" + $('.hdnArquivoId', $(this).closest('td')).val()
																		+ '&historico=' + $('.hdnHistoricoId', $(this).closest('td')).val()
																		+ '&id=' + $('.hdnFiscalizacaoId', FiscalizacaoDocumentosGerados.container).val();
		MasterPage.redireciona(url);
	},

	obterPdfLaudoCancelado: function () {
		var url = FiscalizacaoDocumentosGerados.settings.urls.pdfLaudo + "/?arquivo=" + $('.hdnArquivoId', $(this).closest('td')).val()
																		+ '&historico=' + $('.hdnHistoricoId', $(this).closest('td')).val()
																		+ '&id=' + $('.hdnFiscalizacaoId', FiscalizacaoDocumentosGerados.container).val();
		MasterPage.redireciona(url);
	}

}
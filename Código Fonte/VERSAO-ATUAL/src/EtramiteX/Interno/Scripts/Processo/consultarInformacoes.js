/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ConsultarInformacoes = {

	urlPdfHistoricoTramitacao: '',
	urlPdfAnalise: '',
	urlPdfDocumentosJuntados: '',
	urlPdfProcessosApensados: '',
	urlGerarEntregaPdf: '',
	urlPdfArquivamento: '',
	urlAbrirMapa: '',
	container: {},

	load: function (container) {
		Modal.defaultButtons(container);

		ConsultarInformacoes.container = MasterPage.getContent(container);

		container.delegate('.btnHistoricoTramit', 'click', function () { ConsultarInformacoes.visualizarTelaClick(this, ConsultarInformacoes.urlPdfHistoricoTramitacao, container) });
		container.delegate('.btnPdfAnalise', 'click', function () { ConsultarInformacoes.abrirPdfClick(this, ConsultarInformacoes.urlPdfAnalise, container) });
		container.delegate('.btnPdfDocumentosJuntados', 'click', function () { ConsultarInformacoes.abrirPdfClick(this, ConsultarInformacoes.urlPdfDocumentosJuntados, container) });

		container.delegate('.btnPdfArquivamento ', 'click', function () { ConsultarInformacoes.abrirPdfClick(this, ConsultarInformacoes.urlPdfArquivamento, container) });

		container.delegate('.btnPdfProcessosApensados', 'click', function () { ConsultarInformacoes.abrirPdfClick(this, ConsultarInformacoes.urlPdfProcessosApensados, container) });

		container.delegate('.btnPdfEntrega ', 'click', ConsultarInformacoes.onAbrirEntrega);
		container.delegate('.btnMapa ', 'click', ConsultarInformacoes.onAbrirMapaClick);
		container.delegate('.btnPdfRecebimento ', 'click', ConsultarInformacoes.onAbrirRecebimento);

		Listar.atualizarEstiloTable($('.tabInformacoes', container));
	},

	onAbrirEntrega: function () {
		var id = $(this).closest('tr').find('.hdnId').val();
		MasterPage.redireciona(ConsultarInformacoes.urlGerarEntregaPdf + '?id=' + id);
	},

	onAbrirRecebimento: function () {
		var id = $(this).closest('tr').find('.hdnId').val();
		MasterPage.redireciona(ConsultarInformacoes.urlGerarRecebimentoPdf + '/' + id);
	},


	abrirPdfClick: function (btnPdf, url, container) {
		var id = isNaN(parseInt($('.hdnProcessoId', container).val())) ? 0 : parseInt($('.hdnProcessoId', container).val());
		if (id > 0) {
			MasterPage.redireciona(url + "?id=" + id + "&tipo=1");
			MasterPage.carregando(false);
		}
	},

	onAbrirMapaClick: function () {
		var id = isNaN(parseInt($('.hdnProcessoId', ConsultarInformacoes.container).val())) ? 0 : parseInt($('.hdnProcessoId', ConsultarInformacoes.container).val());
		var tipoMapa = $(this).closest('tr').find('.hdnId').val();
		if (id > 0) {
			MasterPage.redireciona(ConsultarInformacoes.urlAbrirMapa + "?protocoloId=" + id + "&isProcesso=true&tipo=" + tipoMapa);
			MasterPage.carregando(false);
		}
	},

	visualizarTelaClick: function (btn, url, container) {
		var id = parseInt($('.hdnProcessoId', container).val());
		Modal.abrir(url, { id: id, tipo: 1 }, function (context) {
			Modal.defaultButtons(context);
		});
	}
}
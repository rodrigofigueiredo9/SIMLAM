/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />


ConsultarInformacoes = {

	urlPdfHistoricoTramitacao: '',
	urlPdfAnalise: '',
	urlPdfArquivamento: '',
    urlGerarEntregaPdf: '',
    urlGerarCIPdf: '',
	urlAbrirMapa: '',
	container: null,

	load: function (container) {
		ConsultarInformacoes.container = container;

		Modal.defaultButtons(container);

		container.delegate('.btnHistoricoTramit', 'click', ConsultarInformacoes.visualizarTelaClick);
		container.delegate('.btnPdfAnalise', 'click', ConsultarInformacoes.abrirPdfAnaliseClick);
		container.delegate('.btnPdfArquivamento ', 'click', ConsultarInformacoes.abrirPdfArquivamentoClick);
		container.delegate('.btnPdfEntrega ', 'click', ConsultarInformacoes.onAbrirEntrega);
		container.delegate('.btnPdfRecebimento ', 'click', ConsultarInformacoes.onAbrirRecebimento);
		container.delegate('.btnPdfCI ', 'click', ConsultarInformacoes.onAbrirCI);
		
		container.delegate('.btnMapa ', 'click', ConsultarInformacoes.onAbrirMapaClick);
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

    onAbrirCI: function() {
        var id = $(this).closest('tr').find('.hdnId').val();
        MasterPage.redireciona(ConsultarInformacoes.urlGerarCIPdf + '/' + id);
    },

	abrirPdfArquivamentoClick: function () {
		var id = parseInt($('.hdnDocumentoId', ConsultarInformacoes.container).val());
		MasterPage.redireciona(ConsultarInformacoes.urlPdfArquivamento + "?id=" + id + "&tipo=2");
	},

	abrirPdfAnaliseClick: function () {
		var id = parseInt($('.hdnDocumentoId', ConsultarInformacoes.container).val());
		MasterPage.redireciona(ConsultarInformacoes.urlPdfAnalise + "?id=" + id + "&tipo=2");
	},

	onAbrirMapaClick: function () {
		var id = isNaN(parseInt($('.hdnDocumentoId', ConsultarInformacoes.container).val())) ? 0 : parseInt($('.hdnDocumentoId', ConsultarInformacoes.container).val());
		var tipoMapa = $(this).closest('tr').find('.hdnId').val();
		if (id > 0) {
			MasterPage.redireciona(ConsultarInformacoes.urlAbrirMapa + "?protocoloId=" + id + "&isProcesso=false&tipo=" + tipoMapa);
			MasterPage.carregando(false);
		}
	},

	visualizarTelaClick: function (btn, container) {
		var id = parseInt($('.hdnDocumentoId', ConsultarInformacoes.container).val());

		Modal.abrir(ConsultarInformacoes.urlPdfHistoricoTramitacao, { id: id, tipo: 2 }, function (context) {
			Modal.defaultButtons(context);
		});
	}
}
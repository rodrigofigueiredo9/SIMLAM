/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
IndicadoresModal = {

	urlIndicadoresArquivo: "",

	load: function (modalRef) {
		modalRef.find(".botaoGerarXLS").click(IndicadoresModal.onGerarXLS);
		modalRef.find(".botaoGerarPDF").click(IndicadoresModal.onGerarPDF);
		var botoes = modalRef.find(".indicadoreBotoes");
		Modal.buttons(modalRef, botoes);
	},

	onGerarXLS: function () {
		IndicadoresModal.gerarArquivo($(this), "xls");
	},

	onGerarPDF: function () {
		IndicadoresModal.gerarArquivo($(this), "pdf");
	},

	gerarArquivo: function (ctr, extensao) {
		var modalContent = Modal.getFundoModal(ctr).find(".modalContent");
		modalContent.find(".indicadorExtensao").val(extensao);
		modalContent.find(".formIndicadores").submit();
		Modal.fechar(ctr);
	}
};

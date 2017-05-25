/// <reference path="../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="habilitacaoCFOAlterarSituacao.js" />

HabilitarEmissaoCFOCFOCListar = {
	alterarSituacaoLink: null,
	visualizarLink: null,
	urlEditar: null,
	urlPdf: null,
    historicoLink: null,
	container: null,

	load: function (container) {
		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnVisualizar', 'click', HabilitarEmissaoCFOCFOCListar.visualizar);
		container.delegate('.btnAltStatus', 'click', HabilitarEmissaoCFOCFOCListar.alterarSituacao);
		container.delegate('.btnEditar', 'click', HabilitarEmissaoCFOCFOCListar.editar);
		container.delegate('.btnPDF', 'click', HabilitarEmissaoCFOCFOCListar.gerarPdf);
		container.delegate('.btnHistorico', 'click', HabilitarEmissaoCFOCFOCListar.historico);

		Aux.setarFoco(container);
		HabilitarEmissaoCFOCFOCListar.container = container;
		Mascara.load(HabilitarEmissaoCFOCFOCListar.container);
	},

	gerarPdf: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(HabilitarEmissaoCFOCFOCListar.urlPdf + "/" + itemId);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(HabilitarEmissaoCFOCFOCListar.urlEditar + '/' + itemId);
	},

	alterarSituacao: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		Modal.confirma({
			url: HabilitarEmissaoCFOCFOCListar.alterarSituacaoLink + '/' + itemId,
			tamanhoModal: Modal.tamanhoModalMedia,
			btnOkLabel: 'Salvar',
			onLoadCallbackName: function (conteudoModal) { HabilitacaoCFOAlterarSituacao.load(conteudoModal); },
			btnOkCallback: HabilitacaoCFOAlterarSituacao.alterarSituacao
		});
	},

	visualizar: function () {
		var id = parseInt($(this).closest('tr').find('.itemId:first').val());
		var content = MasterPage.getContent($(this, HabilitarEmissaoCFOCFOCListar.container));

		MasterPage.redireciona($('.urlVisualizar', content).val() + "/" + id);
	},

	obter: function(a){
	    var itemId = parseInt($(a).closest('tr').find('.itemId:first').val());
	    var nome = JSON.stringify($(a).closest('tr').find('.responsavelNomeRazaoSocial').text());

	    var obj = {
	        id: itemId,
	        nome: nome
	    }

	    return obj;
	},

	historico: function () {
	    var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
	    var nome = JSON.stringify($(this).closest('tr').find('.responsavelNomeRazaoSocial').text());

	    
	    Modal.confirma({
	        url: HabilitarEmissaoCFOCFOCListar.historicoLink + '?id=' + itemId + '&nome=' + nome,
	        //data: JSON.stringify(HabilitarEmissaoCFOCFOCListar.obter(this)),
	        tamanhoModal: Modal.tamanhoModalMedia,
	        btnOkLabel: 'Salvar',
	        onLoadCallbackName: function (conteudoModal) { HabilitacaoCFOAlterarSituacao.load(conteudoModal); },
	        btnOkCallback: HabilitacaoCFOAlterarSituacao.alterarSituacao
	    });
	},
}
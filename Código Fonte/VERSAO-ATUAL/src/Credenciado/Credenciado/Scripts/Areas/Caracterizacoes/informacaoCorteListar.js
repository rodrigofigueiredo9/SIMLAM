/// <reference path="../../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../../masterpage.js" />

InformacaoCorte = {
	settings: {
		urls: {
			adicionarInformacao: '',
			editar: '',
			visualizar: '',
			excluirConfirm: '',
			excluir: '',
			voltar: ''
		},
		empreendimentoID: 0,
		projetoDigitalID: 0
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(InformacaoCorte.settings, options); }
		InformacaoCorte.container = MasterPage.getContent(container);

		InformacaoCorte.container.delegate('.btnAdicionarInformacao', 'click', InformacaoCorte.adicionarInformacao);
		InformacaoCorte.container.delegate('.btnVoltarInformacao', 'click', InformacaoCorte.voltar);
		InformacaoCorte.container.delegate('.btnVisualizarInformacaoCorte', 'click', InformacaoCorte.visualizar);
		InformacaoCorte.container.delegate('.btnEditarInformacaoCorte', 'click', InformacaoCorte.editar);
		InformacaoCorte.container.delegate('.btnExcluirInformacaoCorte', 'click', InformacaoCorte.excluir);
		
		Aux.setarFoco(InformacaoCorte.container);
	},

	adicionarInformacao: function () {
		MasterPage.redireciona(InformacaoCorte.settings.urls.adicionarInformacao + '/' + InformacaoCorte.settings.empreendimentoID + '?projetoDigitalId=' + InformacaoCorte.settings.projetoDigitalID);
	},

	voltar: function () {
		MasterPage.redireciona(InformacaoCorte.settings.urls.voltar);		
	},

	visualizar: function () {
		var linha = $(this).closest('tr');
		var id = $('.itemId', linha).val();

		MasterPage.redireciona(InformacaoCorte.settings.urls.visualizar + '/' + id + '?projetoDigitalId=' + InformacaoCorte.settings.projetoDigitalID);
	},

	editar: function () {
		var linha = $(this).closest('tr');
		var id = $('.itemId', linha).val();

		MasterPage.redireciona(InformacaoCorte.settings.urls.editar + '/' + id + '?projetoDigitalId=' + InformacaoCorte.settings.projetoDigitalID);
	},

	excluir: function () {
		var linha = $(this).closest('tr');
		var id = $('.itemId', linha).val();

		Modal.excluir({
			'urlConfirm': InformacaoCorte.settings.urls.excluirConfirm + '/' + id + '?projetoDigitalId=' + InformacaoCorte.settings.projetoDigitalID,
			'urlAcao': InformacaoCorte.settings.urls.excluir,
			'data': { id: id, projetoDigitalId: InformacaoCorte.settings.projetoDigitalID },
			'callBack': InformacaoCorte.callBackExcluir,
			'naoExecutarUltimaBusca': true
		});
	},

	callBackExcluir: function (data) {
		MasterPage.redireciona(data.urlRedireciona);
	}
};
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

ProjetoDigitalListar = {
	urlImportar: '',
	urlPdfRequerimento: '',
	container: null,

	load: function (container) {
		container = MasterPage.getContent(container);
		container.listarAjax();
		ProjetoDigitalListar.container = container;

		container.delegate('.btnPdfRequerimento', 'click', ProjetoDigitalListar.abrirPdf);
		container.delegate('.btnVisualizar', 'click', ProjetoDigitalListar.visualizar);
		container.delegate('.btnImportar', 'click', ProjetoDigitalListar.importar);
		container.delegate('.btnPendencia', 'click', ProjetoDigitalListar.abrirModalNotificacao);
		container.delegate('.radioInteressadoCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);

		Aux.onChangeRadioCpfCnpjMask($('.radioInteressadoCpfCnpj', container));

		Aux.setarFoco(container);
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	visualizar: function () {
		var objeto = ProjetoDigitalListar.obter(this);
		MasterPage.redireciona($('.urlVisualizar', ProjetoDigitalListar.container).val() + "/" + objeto.Id);
	},

	abrirPdf: function () {
		MasterPage.carregando(true);
		var objeto = ProjetoDigitalListar.obter(this);
		MasterPage.redireciona(ProjetoDigitalListar.urlPdfRequerimento + '/' + objeto.Id);
		MasterPage.carregando(false);
	},

	abrirModalNotificacao: function () {
		var objeto = ProjetoDigitalListar.obter(this);
		Modal.abrir('ProjetoDigital/VisualizarNotificacao/?requerimentoId=' + objeto.Id, null, function (container) { Modal.defaultButtons(container); }, Modal.tamanhoModalMedia);
	},

	importar: function () {
		var objeto = ProjetoDigitalListar.obter(this);
		MasterPage.redireciona(ProjetoDigitalListar.urlImportar + '/' + objeto.Id);
	}
	
}
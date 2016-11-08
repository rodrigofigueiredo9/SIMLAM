/// <reference path="Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

CARSolicitacaoListar = {
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container) {
		container = MasterPage.getContent(container);

		container.listarAjax({
			onBeforeFiltrar: CARSolicitacaoListar.onBeforeFiltrar
		});

		CARSolicitacaoListar.container = container;

		container.delegate('.btnVisualizar', 'click', CARSolicitacaoListar.visualizar);
		container.delegate('.btnPDF', 'click', CARSolicitacaoListar.gerarPDF);
		container.delegate('.btnPDFTitulo', 'click', CARSolicitacaoListar.gerarPDFTitulo);
		container.delegate('.radioDeclaranteCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		container.delegate('.radioSolicitacaoTituloNumero', 'change', CARSolicitacaoListar.onChangeRadioSolicitacaoTituloNumero);
		Aux.onChangeRadioCpfCnpjMask($('.radioDeclaranteCpfCnpj', container));

		Aux.setarFoco(container);
	},


	onBeforeFiltrar: function (container, serializedData)
	{
        serializedData.Filtros.IsCPF = $('.radioCPF', CARSolicitacaoListar.container).is(':checked');
	    serializedData.Filtros.IsCNPJ = !serializedData.Filtros.IsCPF;

	    serializedData.Filtros.IsSolicitacaoNumero = $('.radioSolicitacao', CARSolicitacaoListar.container).is(':checked');
	    serializedData.Filtros.IsTituloNumero = !serializedData.Filtros.IsSolicitacaoNumero;

		//serializedData.Filtros.EmpreendimentoCodigo = Mascara.getIntMask($(".txtCodigo", CARSolicitacaoListar.container).val()).toString();
	},

	onChangeRadioSolicitacaoTituloNumero: function () {
	    
	    $('.txtSolicitacaoTituloNumero', CARSolicitacaoListar.container).val('');

	    $('.txtSolicitacaoTituloNumero', CARSolicitacaoListar.container).focus();
	    Mascara.load(CARSolicitacaoListar.container);
	},

	visualizar: function () {
		var objeto = CARSolicitacaoListar.obter(this);

		if (CARSolicitacaoListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', CARSolicitacaoListar.container).val() + "/" + objeto.Id, null, function (context) {
				Modal.defaultButtons(context);
			}, Modal.tamanhoModalGrande);
		} else {
			MasterPage.redireciona($('.urlVisualizar', CARSolicitacaoListar.container).val() + "/" + objeto.Id);
		}
	},

	gerarPDF: function () {
		var container = $(this).closest('tr');
		var solicitacaoId = $('.itemId', container).val();

		MasterPage.redireciona($('.urlPdf', CARSolicitacaoListar.container).val() + "/" + solicitacaoId);
	},

	gerarPDFTitulo: function () {
		var tituloId = $('.itemId', $(this).closest('tr')).val();
		MasterPage.redireciona($('.urlPdfTitulo', CARSolicitacaoListar.container).val() + "/" + tituloId);
	}
}
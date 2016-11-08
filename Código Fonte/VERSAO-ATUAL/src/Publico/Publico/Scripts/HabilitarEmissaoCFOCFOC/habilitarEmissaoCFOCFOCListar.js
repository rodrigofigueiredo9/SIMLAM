/// <reference path="../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

HabilitarEmissaoCFOCFOCListar = {
	visualizarLink: '',
	container: null,

	load: function (container) {
		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnVisualizar', 'click', HabilitarEmissaoCFOCFOCListar.visualizar);		

		Aux.setarFoco(container);
		HabilitarEmissaoCFOCFOCListar.container = container;

		Mascara.load(HabilitarEmissaoCFOCFOCListar.container);
	},

	visualizar: function () {
		var id = parseInt($(this).closest('tr').find('.itemId:first').val());
		var content = MasterPage.getContent($(this, HabilitarEmissaoCFOCFOCListar.container));
		
		Modal.abrir(HabilitarEmissaoCFOCFOCListar.visualizarLink + "/" + id, null, function (context) {
			Modal.defaultButtons(context);
		});
	}
}
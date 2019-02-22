/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

TituloDeclaratorioConfiguracaoListar = {
	container: null,

	load: function (container) {
		container.listarAjax();
		TituloDeclaratorioConfiguracaoListar.container = MasterPage.getContent(container);
		
		TituloDeclaratorioConfiguracaoListar.container.delegate('.radioCpfCnpjInteressado', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioCpfCnpjInteressado', FiscalizacaoListar.container));
		Aux.setarFoco(container);
	}
}
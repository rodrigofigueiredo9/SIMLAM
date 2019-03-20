/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

TituloDeclaratorioConfiguracaoListar = {
	container: null,

	load: function (container) {
		$($('.itemMenu')[0]).removeClass('ativo');	//para resolver o problema do menu que ficava selecionado
		container.listarAjax();
		TituloDeclaratorioConfiguracaoListar.container = MasterPage.getContent(container);
		
		TituloDeclaratorioConfiguracaoListar.container.delegate('.radioCpfCnpjInteressado', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioCpfCnpjInteressado', TituloDeclaratorioConfiguracaoListar.container));
		Aux.setarFoco(container);
	}
}
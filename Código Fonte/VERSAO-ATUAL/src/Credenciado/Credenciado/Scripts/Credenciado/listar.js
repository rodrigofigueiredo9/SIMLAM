/// <reference path="../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

CredenciadoListar = {
	settings: {
		onAssociarCallback: null
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(CredenciadoListar.settings, options);
		}
		container = MasterPage.getContent(container);

		container.listarAjax();
		container.delegate('.btnAssociar', 'click', CredenciadoListar.associar);
		container.delegate('.radioPessoaCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioPessoaCpfCnpj', container));

		Aux.setarFoco(container);
		CredenciadoListar.container = container;

		Mascara.load(CredenciadoListar.container);
	},

	associar: function () {
		var id = parseInt($(this).closest('tr').find('.credenciadoId:first').val());
		var sucesso = CredenciadoListar.settings.onAssociarCallback(id);
		if (sucesso) {
			Modal.fechar(CredenciadoListar.container);
		} else {
			Mensagem.gerar(CredenciadoListar.container, msgErro);
		}
	}
}
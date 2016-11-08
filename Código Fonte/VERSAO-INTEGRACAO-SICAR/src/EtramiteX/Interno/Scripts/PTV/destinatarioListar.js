/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../mensagem.js" />

DestinatarioPTVListar = {
	settings: {
		onAssociarCallback: null
	},
	urlVisualizar: null,
	urlEditar: null,
	urlExcluirConfirm: null,
	urlExcluir: null,
	container: null,

	load: function (container, options) {
		if (options) { $.extend(DestinatarioPTVListar.settings, options); }

		container = MasterPage.getContent(container);
		DestinatarioPTVListar.container = container;
		container.listarAjax();

		container.delegate('.btnEditar', 'click', DestinatarioPTVListar.editar);
		container.delegate('.btnVisualizar', 'click', DestinatarioPTVListar.visualizar)
		container.delegate('.btnAssociar', 'click', DestinatarioPTVListar.associar);
		container.delegate('.btnExcluir', 'click', DestinatarioPTVListar.excluir);

		container.delegate('.radioPessoaCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioPessoaCpfCnpj', container));
		Aux.setarFoco(container);
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	editar: function () {
		var objeto = DestinatarioPTVListar.obter(this);
		MasterPage.redireciona(DestinatarioPTVListar.urlEditar + '/' + objeto.Id);
	},

	excluir: function () {
		var objeto = DestinatarioPTVListar.obter(this);
		Modal.excluir({
			'urlConfirm': DestinatarioPTVListar.urlExcluirConfirm,
			'urlAcao': DestinatarioPTVListar.urlExcluir,
			'id': objeto.Id,
			'btnExcluir': (this)
		});
	},

	visualizar: function () {
		var objeto = DestinatarioPTVListar.obter(this);
		MasterPage.redireciona(DestinatarioPTVListar.urlVisualizar + '/' + objeto.Id);
	},

	associar: function () {
		var objeto = DestinatarioPTVListar.obter(this);
		var sucesso = DestinatarioPTVListar.settings.onAssociarCallback(objeto);
		if (sucesso) {
			Modal.fechar(DestinatarioPTVListar.container);
		} else {
			Mensagem.gerar(DestinatarioPTVListar.container, msgErro);
		}
	}
}
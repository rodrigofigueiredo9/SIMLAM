///<reference path="../masterpage.js" />
///<reference path="../jquery.json-2.2.min.js" />
/// <reference path="../mensagem.js" />

OrgaoParceiroConveniadoListar = {
	urlEditar: null,
	urlVisualizar: null,
	urlGerenciar: null,
	urlAlterarSituacao: null,
	Mensagens: null,
	container: null,

	load: function (container) {
		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnVisualizar', 'click', OrgaoParceiroConveniadoListar.visualizar);
		container.delegate('.btnAlterarSituacao', 'click', OrgaoParceiroConveniadoListar.alterarSituacao);
		container.delegate('.btnEditar', 'click', OrgaoParceiroConveniadoListar.editar);
		container.delegate('.btnGerenciar', 'click', OrgaoParceiroConveniadoListar.gerenciar);

		OrgaoParceiroConveniadoListar.container = container
		Aux.setarFoco(container);
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	visualizar: function () {
		var objeto = OrgaoParceiroConveniadoListar.obter(this);
		MasterPage.redireciona(OrgaoParceiroConveniadoListar.urlVisualizar + "/" + objeto.Id);
	},

	alterarSituacao: function () {
		var objeto = OrgaoParceiroConveniadoListar.obter(this);
		MasterPage.redireciona(OrgaoParceiroConveniadoListar.urlAlterarSituacao + "/" + objeto.Id);
	},

	editar: function () {
		var objeto = OrgaoParceiroConveniadoListar.obter(this);

		if (objeto.Situacao == 2) {
			var mensagem = Mensagem.replace(OrgaoParceiroConveniadoListar.Mensagens.OrgaoParceiroBloqueado, '#nome#', objeto.Nome);
			Mensagem.gerar(OrgaoParceiroConveniadoListar.container, [mensagem]);
			return;
		}

		MasterPage.redireciona(OrgaoParceiroConveniadoListar.urlEditar + "/" + objeto.Id);
	},

	gerenciar: function () {
		var objeto = OrgaoParceiroConveniadoListar.obter(this);

		if (objeto.Situacao == 2)
		{
			var mensagem = Mensagem.replace(OrgaoParceiroConveniadoListar.Mensagens.OrgaoParceiroBloqueado, '#nome#', objeto.Nome);
			Mensagem.gerar(OrgaoParceiroConveniadoListar.container, [mensagem]);
			return;
		}

		MasterPage.redireciona(OrgaoParceiroConveniadoListar.urlGerenciar + "/" + objeto.Id);
	}
}
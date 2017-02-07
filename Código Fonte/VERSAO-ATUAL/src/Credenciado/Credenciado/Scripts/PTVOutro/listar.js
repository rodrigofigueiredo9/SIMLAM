/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
PTVListar = {
	settings: {
		urls: {
			urlVisualizar: null,
			urlConfirmCancel: null,
			urlCancelar: null
		}
	},

	container: null,

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(PTVListar.settings, options); }

		container.listarAjax();
		container.delegate('.btnVisualizar', 'click', PTVListar.visualizar);
		container.delegate('.btnCancelar', 'click', PTVListar.cancelar);
		container.delegate('.btnEditar', 'click', PTVListar.editar);

		Aux.setarFoco(container);
		PTVListar.container = container;
	},

	editar: function () {
	    var objeto = PTVListar.obter(this);
	    MasterPage.redireciona(PTVListar.settings.urls.urlEditar + '/' + objeto.Id);
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	visualizar: function () {
		var objeto = PTVListar.obter(this);
		MasterPage.redireciona(PTVListar.settings.urls.urlVisualizar + '/' + objeto.Id);
	},

	cancelar: function () {
		var item = PTVListar.obter(this);

		Modal.abrir(PTVListar.settings.urls.urlConfirmCancel + '/' + item.Id, null, function (container) {
			Modal.defaultButtons(container, function (container) {
				var objeto = item;
				objeto.DataCancelamento = DataCancelamento = { DataTexto: $('.txtDataCancelamento', container).val() };

				MasterPage.carregando(true);
				$.ajax({
					url: PTVListar.settings.urls.urlCancelar,
					data: JSON.stringify(objeto),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: Aux.error,
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.EhValido) {
							PTVListar.container.listarAjax('ultimaBusca');
							Modal.fechar(container);
						}

						if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(container, response.Msg);
						}
					}
				});
				MasterPage.carregando(false);
			}, 'Salvar');
		}, Modal.tamanhoModalPequena);
	}
}
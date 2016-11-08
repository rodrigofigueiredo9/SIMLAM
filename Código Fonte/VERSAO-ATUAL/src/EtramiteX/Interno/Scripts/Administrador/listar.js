/// <reference path="../JQuery/jquery-1.4.3.js"/>
/// <reference path="../masterpage.js"/>

AdministradorListar = {
	urlLogout: '',
	urlEditar: '',
	urlVisualizar: '',
	urlAlterarSituacao: '',
	urlPromoverParaSistemaConfirm: '',
	urlPromoverParaSistema: '',
	container: null,

	load: function (container) {
		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnEditar', 'click', AdministradorListar.editar);
		container.delegate('.btnVisualizar', 'click', AdministradorListar.visualizar);
		container.delegate('.btnAlterarSituacao', 'click', AdministradorListar.alterarSituacao);
		container.delegate('.btnPromoverParaSistema', 'click', AdministradorListar.promoverParaSistema);

		AdministradorListar.container = container;
		Aux.setarFoco(AdministradorListar.container);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(AdministradorListar.urlEditar + '/' + itemId);
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(AdministradorListar.urlVisualizar + '/' + itemId);
	},

	alterarSituacao: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(AdministradorListar.urlAlterarSituacao + '/' + itemId);
	},

	promoverParaSistema: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		Modal.confirma({
			url: AdministradorListar.urlPromoverParaSistemaConfirm + '/' + itemId,
			tamanhoModal: Modal.tamanhoModalMedia,
			btnOkLabel: 'Transferir',
			onLoadCallbackName: function (modalContent) { $('.txtMotivo', MasterPage.getContent(modalContent)).focus(); },
			btnOkCallback: function (modalContent) {
				modalContent = MasterPage.getContent(modalContent);
				$.ajax({
					url: AdministradorListar.urlPromoverParaSistema,
					data: JSON.stringify({ id: $('.hdnAdmId', modalContent).val(), motivo: $('.txtMotivo', modalContent).val() }),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					cache: false,
					async: false,
					error: function (XMLHttpRequest, textStatus, errorThrown) {
						Aux.error(XMLHttpRequest, textStatus, errorThrown, modalContent);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(modalContent, response.Msg);
						} else {
							MasterPage.redireciona(MasterPage.urlLogin);
						}
					}
				});
			}
		});
	}
}
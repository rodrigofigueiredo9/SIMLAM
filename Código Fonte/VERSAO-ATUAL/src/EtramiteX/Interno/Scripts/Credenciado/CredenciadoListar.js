/// <reference path="../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

CredenciadoListar = {
	settings: {
		onAssociarCallback:null
	},

	alterarSituacaoLink: '',
	visualizarLink: '',
	regerarChaveLink: '',
	editarHabilitarLink: '',
	salvarHabilitarEmissaoLink: '',
	obterCredenciadoHabilitar:'',
	container: null,
	
	load: function (container, options) {
		if (options) {
			$.extend(CredenciadoListar.settings, options);
		}

		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnVisualizar', 'click', CredenciadoListar.visualizar);
		container.delegate('.btnAltStatus', 'click', CredenciadoListar.alterarSituacao);
		container.delegate('.btnRegerar', 'click', CredenciadoListar.regerar);
		container.delegate('.btnAssociar', 'click', CredenciadoListar.associar);

		container.delegate('.btnHabilitar', 'click', CredenciadoListar.editarHabilitar);


		container.delegate('.radioPessoaCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioPessoaCpfCnpj', container));

		Aux.setarFoco(container);
		CredenciadoListar.container = container;

		Mascara.load(CredenciadoListar.container);
	},

	regerar: function () {
		var itemId = parseInt($(this).closest('tr').find('.credenciadoId:first').val());
		Modal.confirma({
			url: CredenciadoListar.regerarChaveLink + '/' + itemId,
			tamanhoModal: Modal.tamanhoModalMedia,
			btnOkLabel: 'Regerar Chave',
			btnOkCallback: function (modalContent) {
				modalContent = MasterPage.getContent(modalContent);
				$.ajax({
					url: CredenciadoListar.regerarChaveLink,
					data: JSON.stringify({ id: $('.credId', modalContent).val() }),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					cache: false,
					async: false,
					error: function (XMLHttpRequest, textStatus, errorThrown) {
						Aux.error(XMLHttpRequest, textStatus, errorThrown, modalContent);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.EhValido) {
							Modal.fechar(modalContent);
							CredenciadoListar.container.listarAjax('ultimaBusca');
							Mensagem.gerar(CredenciadoListar.container, response.Msg);
						}
						else {
							Mensagem.gerar(modalContent, response.Msg);
						}
					}
				});
			}
		});
	},

	alterarSituacao: function () {
		var itemId = parseInt($(this).closest('tr').find('.credenciadoId:first').val());

		Modal.confirma({
			url: CredenciadoListar.alterarSituacaoLink + '/' + itemId,
			tamanhoModal: Modal.tamanhoModalMedia,
			btnOkLabel: 'Alterar Situação',
			onLoadCallbackName: function (modalContent) { $('.txtMotivo', MasterPage.getContent(modalContent)).focus(); },
			btnOkCallback: function (modalContent) {
				modalContent = MasterPage.getContent(modalContent);
				$.ajax({
					url: CredenciadoListar.alterarSituacaoLink,
					data: JSON.stringify({ credenciadoId: $('.credenciadoId', modalContent).val(), nome: $('.nomeCred').val(), situacao: $('.novaSituacaoCred').val(), motivo: $('.motivoCred', modalContent).val() }),
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					cache: false,
					async: false,
					error: function (XMLHttpRequest, textStatus, errorThrown) {
						Aux.error(XMLHttpRequest, textStatus, errorThrown, modalContent);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.EhValido) {
							Modal.fechar(modalContent);
							CredenciadoListar.container.listarAjax('ultimaBusca');
							Mensagem.gerar(CredenciadoListar.container, response.Msg);
						}
						else {
							Mensagem.gerar(modalContent, response.Msg);
						}
					}
				});
			}
		});
	},

	editarHabilitar: function () {
		var itemId = parseInt($(this).closest('tr').find('.credenciadoId:first').val());
		var habilitar = null;
		$.ajax({
			url: CredenciadoListar.obterCredenciadoHabilitar + '/' + itemId,
			cache: false,
			async: false,
			type: 'GET',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, CredenciadoListar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					habilitar = response.Habilitar;

					Modal.confirma({
						url: CredenciadoListar.editarHabilitarLink,
						urlData: response.Habilitar,
						tamanhoModal: Modal.tamanhoModalGrande,
						btnOkLabel: 'Salvar',
						onLoadCallbackName: null,
						btnOkCallback: function (modalContent) {
							modalContent = MasterPage.getContent(modalContent);
							$.ajax({
								url: CredenciadoListar.salvarHabilitarEmissaoLink,
								data: JSON.stringify(HabilitarEmissaoCFOCFOC.obter()),
								type: 'POST',
								dataType: 'json',
								contentType: 'application/json; charset=utf-8',
								cache: false,
								async: false,
								error: function (XMLHttpRequest, textStatus, errorThrown) {
									Aux.error(XMLHttpRequest, textStatus, errorThrown, modalContent);
								},
								success: function (response, textStatus, XMLHttpRequest) {
									if (response.IsSalvo) {
										Modal.fechar(modalContent);
										Mensagem.gerar(CredenciadoListar.container, response.Msg);
									}
									else {
										Mensagem.gerar(modalContent, response.Msg);
									}
								}
							});
						}
					});
				}
				else {
					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(CredenciadoListar.container, response.Msg);
					}
				}
			}
		});
	},

	visualizar: function () {
		var id = parseInt($(this).closest('tr').find('.credenciadoId:first').val());
		var content = MasterPage.getContent($(this, CredenciadoListar.container));

		MasterPage.redireciona($('.urlVisualizar', content).val() + "/" + id);
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
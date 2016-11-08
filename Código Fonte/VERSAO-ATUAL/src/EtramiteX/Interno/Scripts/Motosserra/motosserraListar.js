/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

MotosserraListar = {
	urlEditar: '',
	urlExcluir: '',
	urlExcluirConfirm: '',
	urlDesativar: '',
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(MotosserraListar.settings, options); }
		MotosserraListar.container = MasterPage.getContent(container);
		MotosserraListar.container.listarAjax();

		MotosserraListar.container.delegate('.btnExcluir', 'click', MotosserraListar.excluir);
		MotosserraListar.container.delegate('.btnVisualizar', 'click', MotosserraListar.visualizar);
		MotosserraListar.container.delegate('.btnAssociar', 'click', MotosserraListar.associar);
		MotosserraListar.container.delegate('.btnEditar', 'click', MotosserraListar.editar);
		MotosserraListar.container.delegate('.btnDesativar', 'click', MotosserraListar.desativar);

		MotosserraListar.container.listarAjax({ onAfterFiltrar: MotosserraListar.gerenciarSituacao });

		MotosserraListar.container.delegate('.radioCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioCpfCnpj', MotosserraListar.container));
		Aux.setarFoco(container);

		if (MotosserraListar.settings.associarFuncao) {
			$('.hdnIsAssociar', MotosserraListar.container).val(true);
		}
	},

	desativar: function () {
		Mensagem.limpar(MotosserraListar.container);

		var tr = $(this).closest('tr');
		var item = JSON.parse($('.itemJson', tr).val());

		if (item.SituacaoId == 2) {
			return false;
		}

		Modal.confirma({
			removerFechar: true,
			btnOkLabel: 'Desativar',
			btnOkCallback: function (conteudoModal) {

				MasterPage.carregando(true);

				$.ajax({
					url: MotosserraListar.urlDesativar,
					data: JSON.stringify({ MotosserraId: item.Id }),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {

						if (response.EhValido) {
							$('.btnDesativar', tr).button({
								disabled: true
							});

							$('.tdItemSituacao', tr).text('Desativo');

							Modal.fechar(conteudoModal);

							Listar.atualizarEstiloTable(MotosserraListar.container.find('.dataGridTable'));
						}

						if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(MotosserraListar.container, response.Msg);
						}
					}
				});

				MasterPage.carregando(false);

			},
			conteudo: 'Está ação irá desativar o cadastro de motosserra Nº ' + item.RegistroNumero + ', não podendo reativá-lo novamente. Deseja realmente desativar?',
			titulo: 'Desativar Motosserra',
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	gerenciarSituacao: function () {
		var container = MotosserraListar.container;

		$('.itemSituacaoId', container).each(function () {
			var isAtivo = $(this).val() == 1;
			var containerAux = $(this).closest('tr');

			if (isAtivo) {
				$('.btnDesativar', containerAux).button({
					disabled: false
				});
			} else {
				$('.btnDesativar', containerAux).button({
					disabled: true
				});
			}
		});
	},

	editar: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		MasterPage.redireciona(MotosserraListar.urlEditar + '?id=' + itemId);
	},

	visualizar: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		var urlVisualizarPessoa = $('.urlVisualizarPessoa', MotosserraListar.container).val();
		var urlAssociarPessoa = $('.urlAssociarPessoa', MotosserraListar.container).val();

		if (MotosserraListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', MotosserraListar.container).val() + "/" + itemId, null, function (context) {

				Motosserra.load(context);
				Motosserra.settings.urls.associarPessoa = urlAssociarPessoa;
				Motosserra.settings.urls.visualizarPessoa = urlVisualizarPessoa;

				Modal.defaultButtons(context);
			}, Modal.tamanhoModalGrande);
		} else {
			MasterPage.redireciona($('.urlVisualizar', MotosserraListar.container).val() + "/" + itemId);
		}
	},

	associar: function () {
		var objeto = $.parseJSON($(this).closest('tr').find('.itemJson').val());
		var retorno = MotosserraListar.settings.associarFuncao(objeto);

		if (retorno === true) {
			Modal.fechar(MotosserraListar.container);
		} else {
			Mensagem.gerar(MotosserraListar.container, retorno);
		}
	},

	excluir: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;

		Modal.excluir({
			'urlConfirm': MotosserraListar.urlExcluirConfirm,
			'urlAcao': MotosserraListar.urlExcluir,
			'id': itemId,
			'btnExcluir': this
		});
	}
}
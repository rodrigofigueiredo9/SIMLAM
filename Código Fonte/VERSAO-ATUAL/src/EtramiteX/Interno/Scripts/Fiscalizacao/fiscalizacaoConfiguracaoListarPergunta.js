/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ConfigurarListarPergunta = {
	container: null,
	settings: {
		urls: {
			urlExcluirConfirm: '',
			urlEditar: '',
			urlVisualizar: '',
			urlExcluir: '',
			urlAlterarSituacao: ''
		}
	},

	load: function (container, options) {
		if (options) { $.extend(ConfigurarListarPergunta.settings, options); }
		ConfigurarListarPergunta.container = MasterPage.getContent(container);
		ConfigurarListarPergunta.container.listarAjax({ onAfterFiltrar: ConfigurarListarPergunta.gerenciarSituacao });

		ConfigurarListarPergunta.container.delegate('.btnExcluir', 'click', ConfigurarListarPergunta.excluir);
		ConfigurarListarPergunta.container.delegate('.btnVisualizar', 'click', ConfigurarListarPergunta.visualizar);
		ConfigurarListarPergunta.container.delegate('.btnEditar', 'click', ConfigurarListarPergunta.editar);

		ConfigurarListarPergunta.container.delegate('.btnDesativar', 'click', ConfigurarListarPergunta.desativar);
		ConfigurarListarPergunta.container.delegate('.btnAtivar', 'click', ConfigurarListarPergunta.ativar);

		Aux.setarFoco(container);
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': ConfigurarListarPergunta.settings.urls.urlExcluirConfirm,
			'urlAcao': ConfigurarListarPergunta.settings.urls.urlExcluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(ConfigurarListarPergunta.settings.urls.urlVisualizar + "/" + itemId);
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(ConfigurarListarPergunta.settings.urls.urlEditar + '/' + itemId);
	},

	gerenciarSituacao: function () {
		var container = ConfigurarListarPergunta.container;

		$('.itemSituacao', container).each(function () {
			var isAtivo = $(this).val() == 1;
			var containerAux = $(this).closest('tr');

			if (isAtivo) {
				$('.btnDesativar', containerAux).button({
					disabled: false
				});

				$('.btnAtivar', containerAux).button({
					disabled: true
				});
			} else {
				$('.btnDesativar', containerAux).button({
					disabled: true
				});

				$('.btnAtivar', containerAux).button({
					disabled: false
				});
			}

		});
	},

	desativar: function () {
		var container = $(this).closest('tr');
		Mensagem.limpar(ConfigurarListarPergunta.container);
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		MasterPage.carregando(true);
		$.ajax({ url: ConfigurarListarPergunta.settings.urls.urlAlterarSituacao,
			data: JSON.stringify({ tipoId: itemId, situacaoNova: 0 }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ConfigurarListarPergunta.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.btnBuscar', ConfigurarListarPergunta.container).click();
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ConfigurarListarPergunta.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},

	ativar: function () {
		var container = $(this).closest('tr');
		Mensagem.limpar(ConfigurarListarPergunta.container);
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		MasterPage.carregando(true);
		$.ajax({ url: ConfigurarListarPergunta.settings.urls.urlAlterarSituacao,
			data: JSON.stringify({ tipoId: itemId, situacaoNova: 1 }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ConfigurarListarPergunta.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.btnBuscar', ConfigurarListarPergunta.container).click();
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ConfigurarListarPergunta.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	}
}
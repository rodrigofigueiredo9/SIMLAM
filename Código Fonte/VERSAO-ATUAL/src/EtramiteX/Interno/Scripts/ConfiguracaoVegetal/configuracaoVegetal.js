/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

ConfiguracaoVegetal = {
	settings: {
		urls: {
			salvar: null,
			obter: null
		}
	},
	container: null,
	Mensagens: {},

	load: function (container, options) {
		if (options) {
			$.extend(ConfiguracaoVegetal.settings, options);
		}

		ConfiguracaoVegetal.container = MasterPage.getContent(container);
		ConfiguracaoVegetal.container = container;
		ConfiguracaoVegetal.container.delegate('.btnSalvar', 'click', ConfiguracaoVegetal.salvar);
		ConfiguracaoVegetal.container.delegate('.btnEditar', 'click', ConfiguracaoVegetal.onEditar);
	},

	obter: function () {
		return {
			Id: $('.hdnId', ConfiguracaoVegetal.container).val(),
			Texto: $('.txtValor', ConfiguracaoVegetal.container).val(),
			Tid: ''
		}
	},

	salvar: function () {
		Mensagem.limpar(ConfiguracaoVegetal.container);
		MasterPage.carregando(true);

		$.ajax({
			url: ConfiguracaoVegetal.settings.urls.salvar,
			data: JSON.stringify(ConfiguracaoVegetal.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ConfiguracaoVegetal.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					if (response.UrlRedirecionar) {
						MasterPage.redireciona(response.UrlRedirecionar);
						return;
					}

					$('.gridContainer', ConfiguracaoVegetal.container).html(response.Grid);
					$('.limpar', ConfiguracaoVegetal.container).val('');
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ConfiguracaoVegetal.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	onEditar: function () {
		Mensagem.limpar(ConfiguracaoVegetal.container);

		$.ajax({
			url: ConfiguracaoVegetal.settings.urls.obter,
			data: JSON.stringify({ id: $(this).closest('tr').find('.hdnItemId').val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ConfiguracaoVegetal.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('body').animate({ scrollTop: $('body').offset().top }, 300);
					$('.hdnId', ConfiguracaoVegetal.container).val(response.ConfiguracaoVegetalItem.Id);
					$('.txtValor', ConfiguracaoVegetal.container).val(response.ConfiguracaoVegetalItem.Texto);
					$('.txtValor', ConfiguracaoVegetal.container).focus();
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ConfiguracaoVegetal.container, response.Msg);
				}
			}
		});
	}
}
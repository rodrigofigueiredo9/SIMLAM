/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

CondicionanteDescricaoSalvar = {
	settings: {
		urls: {
			salvar: ''
		},
		onSalvar: null,
		editando: false
	},
	container: null,

	load: function (container, options) {
		CondicionanteDescricaoSalvar.container = container;
		if (options) { $.extend(CondicionanteDescricaoSalvar.settings, options); }

		var id = parseInt($('.hdnItemId', CondicionanteDescricaoSalvar.container).val()) || 0;
		CondicionanteDescricaoSalvar.editando = id > 0;
		Modal.defaultButtons(
			CondicionanteDescricaoSalvar.container,
			CondicionanteDescricaoSalvar.onBtnSalvarClick,
			CondicionanteDescricaoSalvar.editando ? 'Editar' : 'Salvar'
		);
		$('.txtDescricao', CondicionanteDescricaoSalvar.container).focus();
	},

	onBtnSalvarClick: function () {
		var condicionante = {
			Id: parseInt($('.hdnItemId', CondicionanteDescricaoSalvar.container).val()) || 0,
			Texto: $('.txtDescricao', CondicionanteDescricaoSalvar.container).val()
		};

		Mensagem.limpar(MasterPage.getContent(CondicionanteDescricaoSalvar.container));
		MasterPage.carregando(true);

		$.ajax({
			url: CondicionanteDescricaoSalvar.settings.urls.salvar,
			data: JSON.stringify(condicionante),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CondicionanteDescricaoSalvar.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(CondicionanteDescricaoSalvar.container), response.Msg);
				}

				if (response.EhValido) {
					var retorno = CondicionanteDescricaoSalvar.settings.onSalvar(condicionante.Texto, false);
					if (retorno !== true) {
						Mensagem.gerar(MasterPage.getContent(CondicionanteDescricaoSalvar.container), response.Msg);
					} else {
						Modal.fechar(CondicionanteDescricaoSalvar.container);
					}
				}
			}
		});
		MasterPage.carregando(false);
	}
}
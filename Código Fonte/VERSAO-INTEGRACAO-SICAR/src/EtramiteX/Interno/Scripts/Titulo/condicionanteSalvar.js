/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

CondicionanteSalvar = {
	settings: {
		urls: {
			salvar: '',
			buscarDescricao: ''
		},
		btnSalvarLabel: '',
		onSalvar: null
	},
	
	container: null,

	load: function (container, options) {
		CondicionanteSalvar.container = container;
		if (options) {
			$.extend(CondicionanteSalvar.settings, options);
		}

		container.delegate('.radPossuiPrazo', 'change', CondicionanteSalvar.onRadPossuiPrazoChange);
		container.delegate('.radPossuiPeriodicidade', 'change', CondicionanteSalvar.onRadPossuiPeriodicidadeChange);
		$('.btnBuscarDescricoes', container).click(CondicionanteSalvar.onBtnBuscarDescricoesClick);

		Modal.defaultButtons(CondicionanteSalvar.container, CondicionanteSalvar.onBtnSalvarClick, CondicionanteSalvar.settings.btnSalvarLabel);
	},

	onBtnBuscarDescricoesClick: function () {
		Modal.abrir(CondicionanteSalvar.settings.urls.buscarDescricao, null, function (container) {
			CondicionanteDescricaoListar.load(container, {
				onAssociar: CondicionanteSalvar.onAssociarDescricao
			});
		});
	},

	onAssociarDescricao: function (descricao) {
		$('.txtDescricao', CondicionanteSalvar.container).val(descricao);
		return true;
	},

	onRadPossuiPrazoChange: function () {
		var possuiPrazo = $('input.radPossuiPrazo:checked', CondicionanteSalvar.container).val() == 'True';
		$('.containerParaPrazo', CondicionanteSalvar.container).toggleClass('hide', !possuiPrazo);
	},

	onRadPossuiPeriodicidadeChange: function () {
		var possuiPeriodicidade = $('input.radPossuiPeriodicidade:checked', CondicionanteSalvar.container).val() == 'True';
		$('.containerParaPeriodicidade', CondicionanteSalvar.container).toggleClass('hide', !possuiPeriodicidade);
	},
	
	onBtnSalvarClick: function () {
		var condicionante = {
			Id: parseInt($('.hdnCondicionanteId', CondicionanteSalvar.container).val()),
			Descricao: $('.txtDescricao', CondicionanteSalvar.container).val(),
			PossuiPrazo: $('input.radPossuiPrazo:checked', CondicionanteSalvar.container).val() == 'True',
			Prazo: parseInt($('.txtPrazo', CondicionanteSalvar.container).val()),
			PossuiPeriodicidade: $('input.radPossuiPeriodicidade:checked', CondicionanteSalvar.container).val() == 'True',
			PeriodicidadeValor: $('.txtPeriodicidade', CondicionanteSalvar.container).val(),
			PeriodicidadeTipo: { Id: parseInt($('.ddlPeriodicidadeTipo', CondicionanteSalvar.container).val()) },
			Titulo: {
				Id: parseInt($('.hdnTituloId', CondicionanteSalvar.container).val())
			},
			Situacao: {
				Id:  parseInt($('.hdnSituacao', CondicionanteSalvar.container).val())
			}
		};

		Mensagem.limpar(MasterPage.getContent(CondicionanteSalvar.container));
		MasterPage.carregando(true);

		$.ajax({
			url: CondicionanteSalvar.settings.urls.salvar,
			data: JSON.stringify(condicionante),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CondicionanteSalvar.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(CondicionanteSalvar.container), response.Msg);
				}

				if (response.EhValido) {
					var retorno = CondicionanteSalvar.settings.onSalvar(response.condicionante, response.condicionanteJson);
					if (retorno !== true) {
						Mensagem.gerar(MasterPage.getContent(CondicionanteSalvar.container), response.Msg);
					} else {
						Modal.fechar(CondicionanteSalvar.container);
					}
				}
			}
		});
		MasterPage.carregando(false);
	}
}
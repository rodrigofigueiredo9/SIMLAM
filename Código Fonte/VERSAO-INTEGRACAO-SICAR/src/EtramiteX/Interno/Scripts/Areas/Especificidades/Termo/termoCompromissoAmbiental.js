/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />
/// <reference path="../../../mensagem.js" />

TermoCompromissoAmbiental = {
	container: null,

	settings: {
		urls: {
			obterDadosTermoCompromissoAmbiental: '',
			obterDadosTermoCompromissoAmbientalRepresentantes: '',
			associarTitulo: '',
		},

		modelosCodigosTitulos: null,
		Mensagens: null
	},

	load: function (especificidadeRef) {

		TermoCompromissoAmbiental.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
		TermoCompromissoAmbiental.container.delegate('.btnBuscarTitulo', 'click', TermoCompromissoAmbiental.onAssociarTitulo);
		TermoCompromissoAmbiental.container.delegate('.btnLimparTituloNumero', 'click', TermoCompromissoAmbiental.onLimparAssociar);
		TermoCompromissoAmbiental.container.delegate('.ddlDestinatarios', 'change', TermoCompromissoAmbiental.onChangeDestinatarios);
		MasterPage.botoes(especificidadeRef);
	},

	onAssociarTitulo: function () {

		Modal.abrir(TermoCompromissoAmbiental.settings.urls.associarTitulo, { modelosCodigos: TermoCompromissoAmbiental.settings.modelosCodigosTitulos.LicencaAmbientalRegularizacao },
			function (container) {
				TituloListar.load(container, { associarFuncao: TermoCompromissoAmbiental.callBackAssociarTitulo });
				Modal.defaultButtons(container);
			});
	},

	onLimparAssociar: function () {
		var container = TermoCompromissoAmbiental.container;

		$('.ddlDestinatarios', container).ddlClear();
		$('.ddlRepresentantes', container).ddlClear();

		$('.hdnLicencaId,.txtLicencaNumero', container).val('');
		$('.btnLimparTituloContainer', container).addClass('hide');
		$('.txtLicencaNumero', container).removeClass('disabled').removeAttr('disabled');
		$('.btnBuscarTitulo', container).removeClass('hide');
	},

	callBackAssociarTitulo: function (titulo) {
		var container = TermoCompromissoAmbiental.container;
		var retorno = {};

		Mensagem.limpar(container);

		$.ajax({
			url: TermoCompromissoAmbiental.settings.urls.obterDadosTermoCompromissoAmbiental,
			data: JSON.stringify({ titulo: titulo.Id }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					$('.hdnLicencaId', container).val(titulo.Id);
					$('.txtLicencaNumero', container).val(titulo.Numero + " - " + titulo.ModeloSigla);
					$('.txtLicencaNumero', container).addClass('disabled').attr('disabled', 'disabled');

					$('.btnLimparTituloContainer', container).removeClass('hide');
					$('.btnBuscarTitulo', container).addClass('hide');

					$('.ddlDestinatarios', container).ddlLoad(response.Destinatarios);

					TermoCompromissoAmbiental.onChangeDestinatarios();

					retorno.FecharModal = true;
				}

				if (!response.EhValido) {
					retorno = response;
				}
			}
		});

		return retorno;

	},

	onChangeDestinatarios: function () {
		var container = TermoCompromissoAmbiental.container;
		var destinatario = $('.ddlDestinatarios :selected', container).val();

		if (destinatario > 0) {

			$.ajax({
				url: TermoCompromissoAmbiental.settings.urls.obterDadosTermoCompromissoAmbientalRepresentantes,
				data: JSON.stringify({ destinatario: destinatario }),
				cache: false,
				async: false,
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown);
				},
				success: function (response, textStatus, XMLHttpRequest) {

					if (response.EhValido) {
						if (response.MostrarRepresentantes) {
							$('.divRepresentantes', container).removeClass('hide');
							$('.ddlRepresentantes', container).ddlLoad(response.Representantes);
						} else {
							$('.divRepresentantes', container).addClass('hide');
						}
					}

					if (response.Msgs && response.Msgs.length > 0) {
						Mensagem.gerar(container, response.Msgs);
						return;
					}
				}
			});
		}
	},

	obter: function () {
		var container = TermoCompromissoAmbiental.container;
		Mensagem.limpar(container);

		var especificidade = {
			Destinatario: $('.ddlDestinatarios :selected', container).val() || 0,
			Representante: $('.ddlRepresentantes :selected', container).val() || 0,
			Licenca: $('.hdnLicencaId', container).val() || 0,
			Descricao: $('.txtDescricao', container).val()
		};

		return especificidade;
	}
}

Titulo.settings.especificidadeLoadCallback = TermoCompromissoAmbiental.load;
Titulo.settings.obterEspecificidadeObjetoFunc = TermoCompromissoAmbiental.obter;
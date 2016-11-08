/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

LicencaPorteUsoMotosserra = {
	container: null,
	settings: {
		urls: {
			obterDadosLicencaPorteUsoMotosserra: '',
			associarMotosserra: '',
			obterMotosserra: ''
		},
		modelos: {}
	},

	load: function (especificidadeRef) {
		LicencaPorteUsoMotosserra.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);

		LicencaPorteUsoMotosserra.container.delegate('.ddlVias', 'change', LicencaPorteUsoMotosserra.changeDdlVias);
		LicencaPorteUsoMotosserra.container.delegate('.btnBuscarMotosserra', 'click', LicencaPorteUsoMotosserra.onAssociarMotosserra);
		LicencaPorteUsoMotosserra.container.delegate('.btnLimparMotosserra', 'click', LicencaPorteUsoMotosserra.onLimparMotosserra);
	},

	obterDadosLicencaPorteUsoMotosserra: function (protocolo) {

		if (protocolo == null) {
			$('.ddlDestinatarios', LicencaPorteUsoMotosserra.container).ddlClear();
			return;
		}

		$.ajax({ url: LicencaPorteUsoMotosserra.settings.urls.obterDadosLicencaPorteUsoMotosserra,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LicencaPorteUsoMotosserra.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios.length > 0) {
					$('.ddlDestinatarios', LicencaPorteUsoMotosserra.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	onAssociarMotosserra: function () {
		Modal.abrir(LicencaPorteUsoMotosserra.settings.urls.associarMotosserra, null, function (container) {
			MotosserraListar.load(container, { associarFuncao: LicencaPorteUsoMotosserra.callBackAssociarMotosserra });
			Modal.defaultButtons(container);
		});
	},

	changeDdlVias: function () {
		var via = $('.ddlVias :selected', LicencaPorteUsoMotosserra.container).val();
		$('.divViasOutra', LicencaPorteUsoMotosserra.container).addClass('hide');
		LicencaPorteUsoMotosserra.container.find('.txtViasOutra').val('');

		if (via == 6) {
			$('.divViasOutra', LicencaPorteUsoMotosserra.container).removeClass('hide');
			return;
		}
	},

	callBackAssociarMotosserra: function (Motosserra) {
		Mensagem.limpar(LicencaPorteUsoMotosserra.container);

		var retorno = false;
		var destinatario = $('.ddlDestinatarios :selected', LicencaPorteUsoMotosserra.container).val();
		var titulo = $('.hdnTituloId').val();

		$.ajax({ url: LicencaPorteUsoMotosserra.settings.urls.obterMotosserra,
			data: JSON.stringify({ motosserraId: Motosserra.Id, destinatarioId: destinatario, tituloId: titulo }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LicencaPorteUsoMotosserra.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Motosserra) {

					$('.hdnMotosserraId', LicencaPorteUsoMotosserra.settings.container).val(response.Motosserra.Id);
					$('.hdnMotosserraProprietarioId', LicencaPorteUsoMotosserra.settings.container).val(response.Motosserra.ProprietarioId); 
					$('.hdnMotosserraTid', LicencaPorteUsoMotosserra.settings.container).val(response.Motosserra.Tid);
					$('.txtNumeroRegistro', LicencaPorteUsoMotosserra.settings.container).val(response.Motosserra.NumeroRegistro);
					$('.txtNumeroFabricacao', LicencaPorteUsoMotosserra.settings.container).val(response.Motosserra.NumeroFabricacao);
					$('.txtMarcaModelo', LicencaPorteUsoMotosserra.settings.container).val(response.Motosserra.Marca);
					$('.txtNotaFiscal', LicencaPorteUsoMotosserra.settings.container).val(response.Motosserra.NotaFiscal);

					$('.btnLimparContainerMotosserra', LicencaPorteUsoMotosserra.settings.container).removeClass('hide');
					$('.btnBuscarMotosserra', LicencaPorteUsoMotosserra.settings.container).addClass('hide');

					retorno = true;
				}

				if (response.Msg && response.Msg.length > 0) {
					retorno = response.Msg;
				}
			}
		});

		return retorno;
	},

	onLimparMotosserra: function () {

		$('.hdnMotosserraId', LicencaPorteUsoMotosserra.settings.container).val(0);
		$('.hdnMotosserraProprietarioId', LicencaPorteUsoMotosserra.settings.container).val(0);
		$('.hdnMotosserraTid', LicencaPorteUsoMotosserra.settings.container).val('');
		$('.txtNumeroRegistro', LicencaPorteUsoMotosserra.settings.container).val('');
		$('.txtNumeroFabricacao', LicencaPorteUsoMotosserra.settings.container).val('');
		$('.txtMarcaModelo', LicencaPorteUsoMotosserra.settings.container).val('');
		$('.txtNotaFiscal', LicencaPorteUsoMotosserra.settings.container).val('');

		$('.btnLimparContainerMotosserra', LicencaPorteUsoMotosserra.settings.container).addClass('hide');
		$('.btnBuscarMotosserra', LicencaPorteUsoMotosserra.settings.container).removeClass('hide');
	},

	obterObjeto: function () {

		var vias = $('.ddlVias :selected', LicencaPorteUsoMotosserra.container).val();

		if (vias == 6) {
			vias = LicencaPorteUsoMotosserra.container.find('.txtViasOutra').val();
		}

		return {
			Destinatario: LicencaPorteUsoMotosserra.container.find('.ddlDestinatarios').val(),
			Vias: vias,
			AnoExercicio: LicencaPorteUsoMotosserra.container.find('.txtExercicio').val(),
			Motosserra: {
				Id: Number($('.hdnMotosserraId', LicencaPorteUsoMotosserra.settings.container).val()) || 0,
				ProprietarioId: Number($('.hdnMotosserraProprietarioId', LicencaPorteUsoMotosserra.settings.container).val()) || 0,
				Tid: $('.hdnMotosserraTid', LicencaPorteUsoMotosserra.settings.container).val()
			}
		};
	}
};



Titulo.settings.especificidadeLoadCallback = LicencaPorteUsoMotosserra.load;
Titulo.settings.obterEspecificidadeObjetoFunc = LicencaPorteUsoMotosserra.obterObjeto;
Titulo.addCallbackProtocolo(LicencaPorteUsoMotosserra.obterDadosLicencaPorteUsoMotosserra);

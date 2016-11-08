/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />

LaudoAuditoriaFomentoFlorestal = {
	container: null,
	urlEspecificidade: null,
	urlObterDadosLaudoAuditoriaFomentoFlorestal: null,
	idsTela: null,

	load: function (especificidadeRef) {
		LaudoAuditoriaFomentoFlorestal.container = especificidadeRef;
		AtividadeEspecificidade.load(LaudoAuditoriaFomentoFlorestal.container);
		TituloCondicionante.load($('.condicionantesContainer', LaudoAuditoriaFomentoFlorestal.container));

		LaudoAuditoriaFomentoFlorestal.container.delegate('input[type="radio"]', 'change', LaudoAuditoriaFomentoFlorestal.gerenciarCampos);
		LaudoAuditoriaFomentoFlorestal.container.delegate('.ddlResultados', 'change', LaudoAuditoriaFomentoFlorestal.gerenciarResultado);
		LaudoAuditoriaFomentoFlorestal.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });

		LaudoAuditoriaFomentoFlorestal.gerenciarCampos();
		LaudoAuditoriaFomentoFlorestal.gerenciarResultado();
	},


	gerenciarCampos: function () {
		var container = LaudoAuditoriaFomentoFlorestal.container;
		var plantioApp = $('.rdbPlantioAPP:checked', container).val() == 1;
		var plantioMudas = $('.rdbPlantioMudasEspeciesFlorestNativas:checked', container).val() == 1;
		var preparoSolo = $('.rdbPreparoSolo:checked', container).val() == 1;

		$('.divPlantioAPPArea', container).addClass('hide');
		$('.divPlantioMudasEspeciesFlorestNativasQtd', container).addClass('hide');
		$('.divPlantioMudasEspeciesFlorestNativasArea', container).addClass('hide');
		$('.divPreparoSoloArea', container).addClass('hide');

		if (plantioApp) {
			$('.divPlantioAPPArea', container).removeClass('hide');
		}

		if (plantioMudas) {
			$('.divPlantioMudasEspeciesFlorestNativasQtd', container).removeClass('hide');
			$('.divPlantioMudasEspeciesFlorestNativasArea', container).removeClass('hide');
		}

		if (preparoSolo) {
			$('.divPreparoSoloArea', container).removeClass('hide');
		}
	},

	gerenciarResultado: function () {
		var container = LaudoAuditoriaFomentoFlorestal.container;
		var resultado = $('.ddlResultados :selected', container).val();
		var texto = '';

		$('.divQuais', container).addClass('hide');

		if (resultado == LaudoAuditoriaFomentoFlorestal.idsTela.EspecificidadeResultadoConforme) {
			texto = 'O contrato vistoriado foi implantado segundo as recomendações do projeto. Não foram constatadas inconformidades.';
		}

		if (resultado == LaudoAuditoriaFomentoFlorestal.idsTela.EspecificidadeResultadoNaoConforme) {
			texto = 'O contrato vistoriado não foi implantado segundo as recomendações do projeto.';
			$('.divQuais', container).removeClass('hide');
		}

		$('.txtParecerDescricao', container).val(texto);

	},

	obterObjeto: function () {
		var container = LaudoAuditoriaFomentoFlorestal.container;
		var obj = {
			Id: Number(LaudoAuditoriaFomentoFlorestal.container.find('.hdnLaudoAuditoriaFomentoFlorestal').val()) || 0,
			Destinatario: LaudoAuditoriaFomentoFlorestal.container.find('.ddlDestinatarios').val(),
			Objetivo: $('.txtObjetivo', container).val(),
			ParecerDescricao: $('.txtParecerDescricao', container).val(),
			PlantioAPP: $('.rdbPlantioAPP:checked', container).val(),
			PlantioAPPArea: $('.txtPlantioAPPArea', container).val(),
			PlantioMudasEspeciesFlorestNativas: $('.rdbPlantioMudasEspeciesFlorestNativas:checked', container).val(),
			PlantioMudasEspeciesFlorestNativasQtd: $('.txtPlantioMudasEspeciesFlorestNativasQtd', container).val(),
			PlantioMudasEspeciesFlorestNativasArea: $('.txtPlantioMudasEspeciesFlorestNativasArea', container).val(),
			PreparoSolo: $('.rdbPreparoSolo:checked', container).val(),
			PreparoSoloArea: $('.txtPreparoSoloArea', container).val(),
			ResultadoTipo: $('.ddlResultados :selected', container).val(),
			ResultadoTipoTexto: $('.ddlResultados :selected', container).text(),
			ResultadoQuais: $('.txtResultadoQuais', container).val(),
			DataVistoria: { DataTexto: $('.txtDataVistoria', container).val() },
			Anexos: LaudoAuditoriaFomentoFlorestal.obterAnexosObjeto()
		};


		if (obj.PlantioAPP != "1") {
			obj.PlantioAPPArea = '';
		}

		if (obj.PlantioMudasEspeciesFlorestNativas != "1") {
			obj.PlantioMudasEspeciesFlorestNativasQtd = '';
			obj.PlantioMudasEspeciesFlorestNativasArea = '';
		}

		if (obj.PreparoSolo != "1") {
			obj.PreparoSoloArea = '';
		}

		if (obj.ResultadoTipo == LaudoAuditoriaFomentoFlorestal.idsTela.EspecificidadeResultadoConforme) {
			obj.ResultadoQuais = '';
		}

		return obj;
	},

	obterDadosLaudoAuditoriaFomentoFlorestal: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', LaudoAuditoriaFomentoFlorestal.container).ddlClear();
			return;
		}

		$.ajax({
			url: LaudoAuditoriaFomentoFlorestal.urlObterDadosLaudoAuditoriaFomentoFlorestal,
			data: JSON.stringify({ id: protocolo.Id, empreendimento: protocolo.EmpreendimentoId }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LaudoAuditoriaFomentoFlorestal.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', LaudoAuditoriaFomentoFlorestal.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	obterAnexosObjeto: function () {
		var anexos = new Array();
		anexos = LaudoAuditoriaFomentoFlorestal.container.find('.fsArquivos').arquivo('obterObjeto');
		return anexos;
	}
};

Titulo.settings.especificidadeLoadCallback = LaudoAuditoriaFomentoFlorestal.load;
Titulo.addCallbackProtocolo(LaudoAuditoriaFomentoFlorestal.obterDadosLaudoAuditoriaFomentoFlorestal);
Titulo.settings.obterEspecificidadeObjetoFunc = LaudoAuditoriaFomentoFlorestal.obterObjeto;
Titulo.settings.obterAnexosCallback = LaudoAuditoriaFomentoFlorestal.obterAnexosObjeto;
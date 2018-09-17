/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />

LaudoVistoriaFlorestal = {
	container: null,
	urlEspecificidade: null,
	urlObterDadosLaudoVistoriaFlorestal: null,
	idsTela: null,

	load: function (especificidadeRef) {
		LaudoVistoriaFlorestal.container = especificidadeRef;
		AtividadeEspecificidade.load(LaudoVistoriaFlorestal.container);
		TituloCondicionante.load($('.condicionantesContainer', LaudoVistoriaFlorestal.container));
		LaudoVistoriaFlorestal.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });

		LaudoVistoriaFlorestal.container.delegate('.ddlEspecificidadeConclusoes', 'change', LaudoVistoriaFlorestal.changeDdlEspecificidadeConclusoes);
		LaudoVistoriaFlorestal.gerenciarCampos();
	},

	changeDdlEspecificidadeConclusoes: function () {
		var conclusao = $('.ddlEspecificidadeConclusoes', LaudoVistoriaFlorestal.container).val();
		$('.divRestricao', LaudoVistoriaFlorestal.container).toggleClass('hide', conclusao != LaudoVistoriaFlorestal.idsTela.EspecificidadeConclusaoFavoravelId);
		LaudoVistoriaFlorestal.gerenciarCampos();
	},

	gerenciarCampos: function () {
		var conclusao = $('.ddlEspecificidadeConclusoes', LaudoVistoriaFlorestal.container).val();
		if (conclusao == LaudoVistoriaFlorestal.idsTela.EspecificidadeConclusaoFavoravelId) {
			$('.divRestricao', LaudoVistoriaFlorestal.container).removeClass('hide');
		} else {
			$('.divRestricao', LaudoVistoriaFlorestal.container).addClass('hide');
		}
	},

	obterObjeto: function () {
		return {
			Id: Number(LaudoVistoriaFlorestal.container.find('.hdnLaudoVistoriaFlorestal').val()) || 0,
			Destinatario: LaudoVistoriaFlorestal.container.find('.ddlDestinatarios').val(),
			DataVistoria: { DataTexto: LaudoVistoriaFlorestal.container.find('.txtDataVistoria').val() },
			Objetivo: LaudoVistoriaFlorestal.container.find('.txtObjetivo').val(),
			Responsavel: LaudoVistoriaFlorestal.container.find('.ddlResponsaveisTecnico').val(),
			Caracterizacao: LaudoVistoriaFlorestal.container.find('.ddlCaracterizacoes').val(),
			Consideracao: LaudoVistoriaFlorestal.container.find('.txtConsideracao').val(),
			ParecerDescricao: LaudoVistoriaFlorestal.container.find('.txtDescricao').val(),
			Conclusao: LaudoVistoriaFlorestal.container.find('.ddlEspecificidadeConclusoes').val(),
			Restricao: LaudoVistoriaFlorestal.container.find('.txtRestricao:visible').val(),
			Anexos: LaudoVistoriaFlorestal.container.find('.fsArquivos').arquivo('obterObjeto')
		};
	},

	obterDadosLaudoVistoriaFlorestal: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', LaudoVistoriaFlorestal.container).ddlClear();
			$('.ddlCaracterizacoes', LaudoVistoriaFlorestal.container).ddlClear();
			$('.ddlResponsaveisTecnico', LaudoVistoriaFlorestal.container).ddlClear();
			return;
		}

		$.ajax({
			url: LaudoVistoriaFlorestal.urlObterDadosLaudoVistoriaFlorestal,
			data: JSON.stringify({ id: protocolo.Id, empreendimento: protocolo.EmpreendimentoId }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LaudoVistoriaFlorestal.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', LaudoVistoriaFlorestal.container).ddlLoad(response.Destinatarios);
				}
				if (response.Caracterizacoes) {
					$('.ddlCaracterizacoes', LaudoVistoriaFlorestal.container).ddlLoad(response.Caracterizacoes);
				}

				if (response.ResponsaveisTecnico) {
					$('.ddlResponsaveisTecnico', LaudoVistoriaFlorestal.container).ddlLoad(response.ResponsaveisTecnico);
				}
			}
		});
	},

	obterAnexosObjeto: function () {
		var anexos = new Array();
		anexos = LaudoVistoriaFlorestal.container.find('.fsArquivos').arquivo('obterObjeto');
		return anexos;
	}
};

Titulo.settings.especificidadeLoadCallback = LaudoVistoriaFlorestal.load;
Titulo.addCallbackProtocolo(LaudoVistoriaFlorestal.obterDadosLaudoVistoriaFlorestal);
Titulo.settings.obterEspecificidadeObjetoFunc = LaudoVistoriaFlorestal.obterObjeto;
Titulo.settings.obterAnexosCallback = LaudoVistoriaFlorestal.obterAnexosObjeto;
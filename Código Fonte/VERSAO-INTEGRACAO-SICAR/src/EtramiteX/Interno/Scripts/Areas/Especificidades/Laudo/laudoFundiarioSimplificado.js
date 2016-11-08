/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />

LaudoFundiarioSimplificado = {
	container: null,
	urlEspecificidade: null,
	urlObterDadosLaudoFundiarioSimplificado: null,
	idsTela: null,

	load: function (especificidadeRef) {
		LaudoFundiarioSimplificado.container = especificidadeRef;
		AtividadeEspecificidade.load(LaudoFundiarioSimplificado.container);
		LaudoFundiarioSimplificado.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });
	},
    
	obterObjeto: function () {
		return {
			Id: Number(LaudoFundiarioSimplificado.container.find('.hdnLaudoFundiarioSimplificado').val()) || 0,
			Destinatario: LaudoFundiarioSimplificado.container.find('.ddlDestinatarios').val(),
			DataVistoria: { DataTexto: LaudoFundiarioSimplificado.container.find('.txtDataVistoria').val() },
			Objetivo: LaudoFundiarioSimplificado.container.find('.txtObjetivo').val(),
			ParecerDescricao: LaudoFundiarioSimplificado.container.find('.txtDescricao').val(),
			Anexos: LaudoFundiarioSimplificado.container.find('.fsArquivos').arquivo('obterObjeto')
		};
	},

	obterDadosLaudoFundiarioSimplificado: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', LaudoFundiarioSimplificado.container).ddlClear();
			return;
		}

		$.ajax({
			url: LaudoFundiarioSimplificado.urlObterDadosLaudoFundiarioSimplificado,
			data: JSON.stringify({ id: protocolo.Id, empreendimento: protocolo.EmpreendimentoId }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LaudoFundiarioSimplificado.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', LaudoFundiarioSimplificado.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	obterAnexosObjeto: function () {
		var anexos = new Array();
		anexos = LaudoFundiarioSimplificado.container.find('.fsArquivos').arquivo('obterObjeto');
		return anexos;
	}
};

Titulo.settings.especificidadeLoadCallback = LaudoFundiarioSimplificado.load;
Titulo.addCallbackProtocolo(LaudoFundiarioSimplificado.obterDadosLaudoFundiarioSimplificado);
Titulo.settings.obterEspecificidadeObjetoFunc = LaudoFundiarioSimplificado.obterObjeto;
Titulo.settings.obterAnexosCallback = LaudoFundiarioSimplificado.obterAnexosObjeto;
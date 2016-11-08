/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />

LaudoVistoriaLicenciamento = {
	container: null,
	urlEspecificidade: null,
	urlObterDadosLaudoVistoriaLicenciamento: null,
	idsTela: null,

	load: function (especificidadeRef) {
		LaudoVistoriaLicenciamento.container = especificidadeRef;
		AtividadeEspecificidade.load(LaudoVistoriaLicenciamento.container, { changeAtividade: LaudoVistoriaLicenciamento.callbackChangeAtividade});
		TituloCondicionante.load($('.condicionantesContainer', LaudoVistoriaLicenciamento.container));
		LaudoVistoriaLicenciamento.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });

		LaudoVistoriaLicenciamento.container.delegate('.ddlEspecificidadeConclusoes', 'change', LaudoVistoriaLicenciamento.changeDdlEspecificidadeConclusoes);
		LaudoVistoriaLicenciamento.gerenciarCampos();
	},

	changeDdlEspecificidadeConclusoes: function () {
		var conclusao = $('.ddlEspecificidadeConclusoes', LaudoVistoriaLicenciamento.container).val();
		$('.divRestricao', LaudoVistoriaLicenciamento.container).toggleClass('hide', conclusao != LaudoVistoriaLicenciamento.idsTela.EspecificidadeConclusaoFavoravelId);
		LaudoVistoriaLicenciamento.gerenciarCampos();
	},

	gerenciarCampos: function () {
		var conclusao = $('.ddlEspecificidadeConclusoes', LaudoVistoriaLicenciamento.container).val();
		if (conclusao == LaudoVistoriaLicenciamento.idsTela.EspecificidadeConclusaoFavoravelId) {
			$('.divRestricao', LaudoVistoriaLicenciamento.container).removeClass('hide');
		} else {
			$('.divRestricao', LaudoVistoriaLicenciamento.container).addClass('hide');
		}
	},

	obterObjeto: function () {
		return {
			Id: Number(LaudoVistoriaLicenciamento.container.find('.hdnLaudoVistoriaLicenciamento').val()) || 0,
			Destinatario: LaudoVistoriaLicenciamento.container.find('.ddlDestinatarios').val(),
			DataVistoria: { DataTexto: LaudoVistoriaLicenciamento.container.find('.txtDataVistoria').val() },
			Objetivo: LaudoVistoriaLicenciamento.container.find('.txtObjetivo').val(),
			Responsavel: LaudoVistoriaLicenciamento.container.find('.ddlResponsaveisTecnico').val(),
			Consideracao: LaudoVistoriaLicenciamento.container.find('.txtConsideracao').val(),
			ParecerDescricao: LaudoVistoriaLicenciamento.container.find('.txtDescricao').val(),
			Conclusao: LaudoVistoriaLicenciamento.container.find('.ddlEspecificidadeConclusoes').val(),
			Restricao: LaudoVistoriaLicenciamento.container.find('.txtRestricao:visible').val(),
			Anexos: LaudoVistoriaLicenciamento.container.find('.fsArquivos').arquivo('obterObjeto')
		};
	},

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/*Obs: Esse Modelo de Titulo nao Escuta o Evento de associar o Protocolo, 
	* pois ele espera o protocolo ser selecionado (No caso de protocolos apensados) 
	* para obter os responsaveis tecnicos do requerimento, mas o mesmo ainda escuta eventos
	* de atividade da especificidade que por sua vez chama o metodo de callback de protocolo para obter tambem
	* os destinatarios do protocolo selecionado.
	*///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	obterDadosLaudoVistoriaLicenciamento: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', LaudoVistoriaLicenciamento.container).ddlClear();
			$('.ddlResponsaveisTecnico', LaudoVistoriaLicenciamento.container).ddlClear();
			return;
		}

		$.ajax({
			url: LaudoVistoriaLicenciamento.urlObterDadosLaudoVistoriaLicenciamento,
			data: JSON.stringify({ id: protocolo.Id, empreendimento: protocolo.EmpreendimentoId }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LaudoVistoriaLicenciamento.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', LaudoVistoriaLicenciamento.container).ddlLoad(response.Destinatarios);
				}

				if (response.ResponsaveisTecnico) {
					$('.ddlResponsaveisTecnico', LaudoVistoriaLicenciamento.container).ddlLoad(response.ResponsaveisTecnico);
				}
			}
		});
	},

	callbackChangeAtividade: function () {
		var protocolo = {
			Id: 0,
			IsProcesso: false,
			ReqId: 0,
			EmpreendimentoId: 0
		};

		var procDoc = ($('.ddlProcessosDocumentos', LaudoVistoriaLicenciamento.container).val() || '0@0@0').split('@');
		protocolo.Id = procDoc[0] || 0;
		protocolo.IsProcesso = procDoc[1] == '1';
		protocolo.ReqId = procDoc[2] || 0;
		protocolo.EmpreendimentoId = 0;

		LaudoVistoriaLicenciamento.obterDadosLaudoVistoriaLicenciamento(protocolo);
	},

	obterAnexosObjeto: function () {
		var anexos = new Array();
		anexos = LaudoVistoriaLicenciamento.container.find('.fsArquivos').arquivo('obterObjeto');
		return anexos;
	}
};

Titulo.settings.especificidadeLoadCallback = LaudoVistoriaLicenciamento.load;
Titulo.settings.obterEspecificidadeObjetoFunc = LaudoVistoriaLicenciamento.obterObjeto;
Titulo.settings.obterAnexosCallback = LaudoVistoriaLicenciamento.obterAnexosObjeto;
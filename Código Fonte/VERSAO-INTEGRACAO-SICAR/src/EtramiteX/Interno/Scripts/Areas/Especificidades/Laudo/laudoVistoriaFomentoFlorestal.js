/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />

LaudoVistoriaFomentoFlorestal = {
	container: null,
	urlEspecificidade: null,
	urlObterDadosLaudoVistoriaFomentoFlorestal: null,
	idsTela: null,

	load: function (especificidadeRef) {
		LaudoVistoriaFomentoFlorestal.container = especificidadeRef;
		AtividadeEspecificidade.load(LaudoVistoriaFomentoFlorestal.container);
		TituloCondicionante.load($('.condicionantesContainer', LaudoVistoriaFomentoFlorestal.container));

		LaudoVistoriaFomentoFlorestal.container.delegate('.ddlConclusoes', 'change', LaudoVistoriaFomentoFlorestal.gerenciarCampos);
		LaudoVistoriaFomentoFlorestal.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });

		LaudoVistoriaFomentoFlorestal.gerenciarCampos();
	},


	gerenciarCampos: function () {
		var container = LaudoVistoriaFomentoFlorestal.container;
		var conclusao = $('.ddlConclusoes :selected', container).val();

		$('.divRestricoes', container).addClass('hide');

		if (conclusao == LaudoVistoriaFomentoFlorestal.idsTela.EspecificidadeConclusaoFavoravel) {
			$('.divRestricoes', container).removeClass('hide');
		}
	},

	obterObjeto: function () {
		var container = LaudoVistoriaFomentoFlorestal.container;
		var obj = {
			Id: Number(LaudoVistoriaFomentoFlorestal.container.find('.hdnLaudoVistoriaFomentoFlorestal').val()) || 0,
			Destinatario: LaudoVistoriaFomentoFlorestal.container.find('.ddlDestinatarios').val(),
			Objetivo: $('.txtObjetivo', container).val(),
			Consideracoes: $('.txtConsideracoes', container).val(),
			DescricaoParecer: $('.txtDescricaoParecer', container).val(),
			ConclusaoTipo: $('.ddlConclusoes :selected', container).val(),
			ConclusaoTipoTexto: $('.ddlConclusoes :selected', container).text(),
			Restricoes: $('.txtRestricoes', container).val(),
			Observacoes: $('.txtObservacoes', container).val(),
			DataVistoria: { DataTexto: $('.txtDataVistoria', container).val() },
			Anexos: LaudoVistoriaFomentoFlorestal.obterAnexosObjeto()
		};

		if (obj.ConclusaoTipo != LaudoVistoriaFomentoFlorestal.idsTela.EspecificidadeConclusaoFavoravel) {
			obj.Restricoes = '';
		}

		return obj;
	},

	obterDadosLaudoVistoriaFomentoFlorestal: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', LaudoVistoriaFomentoFlorestal.container).ddlClear();
			return;
		}

		$.ajax({
			url: LaudoVistoriaFomentoFlorestal.urlObterDadosLaudoVistoriaFomentoFlorestal,
			data: JSON.stringify({ id: protocolo.Id, empreendimento: protocolo.EmpreendimentoId }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LaudoVistoriaFomentoFlorestal.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', LaudoVistoriaFomentoFlorestal.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	obterAnexosObjeto: function () {
		var anexos = new Array();
		anexos = LaudoVistoriaFomentoFlorestal.container.find('.fsArquivos').arquivo('obterObjeto');
		return anexos;
	}
};

Titulo.settings.especificidadeLoadCallback = LaudoVistoriaFomentoFlorestal.load;
Titulo.addCallbackProtocolo(LaudoVistoriaFomentoFlorestal.obterDadosLaudoVistoriaFomentoFlorestal);
Titulo.settings.obterEspecificidadeObjetoFunc = LaudoVistoriaFomentoFlorestal.obterObjeto;
Titulo.settings.obterAnexosCallback = LaudoVistoriaFomentoFlorestal.obterAnexosObjeto;
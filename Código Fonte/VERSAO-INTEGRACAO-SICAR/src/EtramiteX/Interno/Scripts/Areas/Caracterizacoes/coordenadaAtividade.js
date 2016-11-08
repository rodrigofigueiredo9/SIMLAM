/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

CoordenadaAtividade = {
	settings: {
		urls: {
			salvar: '',
			urlObterDadosCoordenadaAtividade: '',
			urlObterDadosTipoGeometria: ''
		},
		mensagens: null,
		isSetEvento: false
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(CoordenadaAtividade.settings, options); }
		CoordenadaAtividade.container = MasterPage.getContent(container);

		if (CoordenadaAtividade.settings.isSetEvento) {
			CoordenadaAtividade.container.delegate('.ddlCoordenadaTipoGeometria', 'change', CoordenadaAtividade.obterDadosCoordenadaAtividade);
		}
	},

	obter: function () {
		var container = CoordenadaAtividade.container;
		var coord = $('.ddlCoordenadaAtividade :selected', container).val().toString().split('|');

		var obj = {
			Id: Number(coord[0]),
			Tipo: Number($('.ddlCoordenadaTipoGeometria :selected', container).val()),
			CoordX: coord[1] || 0,
			CoordY: coord[2] || 0
		}
		return obj;
	},

	obterDadosCoordenadaAtividade: function () {
		var container = CoordenadaAtividade.container;
		var empreendimento = $('.hdnEmpreendimentoId', container).val(); 
		var tipo = $('.ddlCoordenadaTipoGeometria :selected', container).val();

		if (empreendimento == null) {
			$('.ddlCoordenadaAtividade', container).ddlClear();
			return;
		}

		$.ajax({
			url: CoordenadaAtividade.settings.urls.urlObterDadosCoordenadaAtividade,
			data: JSON.stringify({ empreendimentoId: empreendimento, tipoGeometria: tipo }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.CoordenadaAtividade) {
					$('.ddlCoordenadaAtividade', container).ddlClear();
					$('.ddlCoordenadaAtividade', container).ddlLoad(response.CoordenadaAtividade);
				}
			}
		});
	},

	obterDadosTipoGeometria: function () {
		var container = CoordenadaAtividade.container;

		var empreendimento = $('.hdnEmpreendimentoId', CoordenadaAtividade.container).val();
		var caracterizacao = $('.hdnCaracterizacaoTipo', CoordenadaAtividade.container).val();

		if (empreendimento == null) {
			$('.ddlCoordenadaTipoGeometria', container).ddlClear();
			return;
		}

		$.ajax({
			url: CoordenadaAtividade.settings.urls.urlObterDadosTipoGeometria,
			data: JSON.stringify({ empreendimentoId: empreendimento, caracterizacaoTipo: caracterizacao }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TiposGeometricos) {
					$('.ddlCoordenadaTipoGeometria', container).ddlLoad(response.TiposGeometricos);
				}

			}
		});
	},

	validar: function () {
		var mensagens = new Array();
		Mensagem.limpar(CoordenadaAtividade.container);

		var obj = CoordenadaAtividade.obter();

		if (obj.Tipo <= 0) {
			mensagens.push(jQuery.extend(true, {}, CoordenadaAtividade.settings.mensagens.GeometriaTipoObrigatorio));
		}

		if (obj.CoordenadasAtividade <= 0) {
			mensagens.push(jQuery.extend(true, {}, CoordenadaAtividade.settings.mensagens.CoordenadaAtividadeObrigatoria));
		}

		if (mensagens.length > 0) {
			Mensagem.gerar(CoordenadaAtividade.container, mensagens);
			return false;
		}

		return true;

	}
}
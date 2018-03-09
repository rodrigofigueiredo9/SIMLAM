/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />

Cobranca = {
	settings: {
		urls: {
			salvar: '',
			carregar: ''
		},

		mensagens: null
	},

	container: null,

	load: function (container, options) {
		if (options) { $.extend(Cobranca.settings, options); }
		Cobranca.container = container;

		container.delegate('.btnSalvar', 'click', Cobranca.salvar);

		$('.txtProcessoNumero', container).focus();
	},

	obter: function () {
		var container = Cobranca.container;

		var obj = {
			Id: $('.hdnCobrancaId', container).val(),
			ProcessoNumero: $('.txtProcessoNumero', container).val(),
			NumeroAutos: $('.txtNumeroAutos', container).val(),
			NumeroFiscalizacao: $('.txtFiscalizacao', container).val(),
			NumeroIUF: $('.txtNumeroIUF', container).val(),
			SerieId: $('.hdnSerieId', container).val(),
			SerieTexto: $('.txtSerie', container).val(),
			DataLavratura: { DataTexto: $('.txtDataLavratura', container).val() },
			DataIUF: { DataTexto: $('.txtDataIUF', container).val() },
			DataJIAPI: { DataTexto: $('.txtDataJIAPI', container).val() },
			DataCORE: { DataTexto: $('.txtDataCORE', container).val() },
			CodigoReceitaId: $('.ddlCodigoReceita :selected', container).val(),
			AutuadoPessoaId: $('.hdnAutuadoPessoaId', container).val(),
			UltimoParcelamento: JSON.parse($('.hdnParcelamento', container).val())
		}
		obj.UltimoParcelamento.ValorMulta = $('.txtValorMulta', container).val();
		obj.UltimoParcelamento.QuantidadeParcelas = $('.ddlParcelas :selected', container).val();
		obj.UltimoParcelamento.Data1Vencimento = { DataTexto: $('.txtData1Vencimento', container).val() };
		obj.UltimoParcelamento.DataEmissao = { DataTexto: $('.txtDataEmissao', container).val() };
		obj.UltimoParcelamento.DUAS = Cobranca.obterListaParcelamento();

		return obj;
	},

	obterListaParcelamento: function () {
		var lista = [];

		$($('.tabParcelas tbody tr:not(.trTemplateRow) .hdnItemJSon', Cobranca.container)).each(function () {
			lista.push(JSON.parse($(this).val()));
		});

		return lista;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: Cobranca.settings.urls.salvar,
			data: JSON.stringify(Cobranca.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Cobranca.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Cobranca.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	gerarParcelas: function () {
		var obj = Cobranca.obter();

		MasterPage.carregando(true);
		$.ajax({
			url: Cobranca.settings.urls.salvar,
			data: JSON.stringify(obj),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Cobranca.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(Cobranca.settings.urls.carregar + "/" + obj.NumeroFiscalizacao);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Cobranca.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}

String.prototype.trim = function () {
	return this.replace(/^\W+|\W+$/g, "");
}
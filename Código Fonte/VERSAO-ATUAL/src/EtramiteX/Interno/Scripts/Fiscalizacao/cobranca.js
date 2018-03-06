/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />

Cobranca = {
	settings: {
		urls: {
			salvar: ''
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
			Serie: $('.txtSerie', container).val(),
			DataLavratura: { DataTexto: $('.txtDataLavratura', container).val() },
			DataIUF: { DataTexto: $('.txtDataIUF', container).val() },
			DataJIAPI: { DataTexto: $('.txtDataJIAPI', container).val() },
			DataCORE: { DataTexto: $('.txtDataCORE', container).val() },
			CodigoReceitaId: $('.ddlCodigoReceita :selected', container).val(),
			ValorMulta: $('.txtValorMulta', container).val(),
			QuantidadeParcelas: $('.ddlParcelas :selected', container).val(),
			Data1Vencimento: { DataTexto: $('.txtData1Vencimento', container).val() },
			DataEmissao: { DataTexto: $('.txtDataEmissao', container).val() },
		}

		return obj;
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
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ConfigurarParametrizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ConfigurarParametrizacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}

String.prototype.trim = function () {
	return this.replace(/^\W+|\W+$/g, "");
}
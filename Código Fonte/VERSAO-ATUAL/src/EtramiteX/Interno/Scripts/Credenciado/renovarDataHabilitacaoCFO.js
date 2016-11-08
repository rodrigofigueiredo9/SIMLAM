/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />

RenovarDataHabilitacaoCFO = {
	settings: {
		urls: {
			renovarData: null
		},
		situacaoMotivo: null
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(RenovarDataHabilitacaoCFO.settings, options);
		}
		RenovarDataHabilitacaoCFO.container = MasterPage.getContent(container);
		RenovarDataHabilitacaoCFO.container.delegate('.btnOk', 'click', RenovarDataHabilitacaoCFO.renovarDatas);
	},

	obter: function () {
		var container = RenovarDataHabilitacaoCFO.container;

		var obj = {
			DataInicialHabilitacao: $('.txtDataInicial', container).val(),
			DataFinalHabilitacao: $('.txtDataFinal', container).val()
		}
		return obj;
	},

	renovarDatas: function () {

		Mensagem.limpar(RenovarDataHabilitacaoCFO.container);

		var dataRenovada = RenovarDataHabilitacaoCFO.obter();

		MasterPage.carregando(true);
		$.ajax({
			url: RenovarDataHabilitacaoCFO.settings.urls.renovarData,
			data: JSON.stringify(dataRenovada),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(RenovarDataHabilitacaoCFO.container, response.Msg);
				}

				if (response.EhValido) {
					Modal.fechar(RenovarDataHabilitacaoCFO.container);
					HabilitarEmissaoCFOCFOC.callBackRenovarPraga(dataRenovada);
				}
			}
		});
		MasterPage.carregando(false);

	}
}
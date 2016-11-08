/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

CondicionanteSituacaoAlterar = {
	settings: {
		urls: {
			visualizar: '',
			atender: '',
			prorrogar: ''
		}
	},
	container: null,

	load: function (container, options) {
		CondicionanteSituacaoAlterar.container = container;
		if (options) {
			$.extend(CondicionanteSituacaoAlterar.settings, options);
		}

		$('.btnVisualizarCond', container).click(CondicionanteSituacaoAlterar.onBtnVisualizarClick);
		$('.bntAtenderCond', container).click(CondicionanteSituacaoAlterar.onBntAtenderClick);
		$('.btnProrrogarCond', container).click(CondicionanteSituacaoAlterar.onBtnProrrogarClick);

		Modal.defaultButtons(CondicionanteSituacaoAlterar.container, null, null);
	},

	onBtnVisualizarClick: function () {
		var condicionanteId = parseInt($(this).closest('tr').find('.hdnCondicionanteId').val());
		Modal.abrir(CondicionanteSituacaoAlterar.settings.urls.visualizar + '/?condicionanteId=' + condicionanteId, null, function (container) {
			CondicionanteVisualizar.load(container);
		});
	},

	onBtnProrrogarClick: function () {
		var condicionanteId = parseInt($(this).closest('tr').find('.hdnCondicionanteId').val());
		var periodicidadeId = parseInt($(this).closest('tr').find('.hdnCondicionantePeriodicidadeId').val());

		var url = CondicionanteSituacaoAlterar.settings.urls.prorrogar + '/?condicionanteId=' + condicionanteId + '&periodicidadeId=' + periodicidadeId;

		Modal.abrir(url, null,
		function (container) {
			CondicionanteProrrogar.load(container, {
				onSalvar: CondicionanteSituacaoAlterar.onProrrogar
			});
		}, Modal.tamanhoModalMedia);
	},

	onProrrogar: function (condicionante, periodicidade) {

		var hdn = null;
		var situacao = null;
		var dataVenc = null;

		if (!periodicidade) {
			hdn = $(".hdnCondicionanteId[value=" + condicionante.Id + "]", CondicionanteSituacaoAlterar.container);
			situacao = condicionante.Situacao.Texto;
			dataVenc = condicionante.DataVencimento.DataTexto;
		} else {
			hdn = $(".hdnCondicionantePeriodicidadeId[value=" + periodicidade.Id + "]", CondicionanteSituacaoAlterar.container);
			situacao = periodicidade.Situacao.Texto;
			dataVenc = periodicidade.DataVencimento.DataTexto;
		}

		var tr = hdn.closest('tr');
		$(".CondSituacao", tr).text(situacao);
		$(".CondVencimento", tr).text(dataVenc);

		return true;
	},

	onBntAtenderClick: function () {
		var condicionanteId = parseInt($(this).closest('tr').find('.hdnCondicionanteId').val());
		var periodicidadeId = parseInt($(this).closest('tr').find('.hdnCondicionantePeriodicidadeId').val());

		var url = CondicionanteSituacaoAlterar.settings.urls.atender + '/?condicionanteId=' + condicionanteId + '&periodicidadeId=' + periodicidadeId;

		Modal.abrir(url, null,
		function (container) {
			CondicionanteAtender.load(container, {
				onSalvar: CondicionanteSituacaoAlterar.onAtender
			});
		}, Modal.tamanhoModalMedia);
	},

	onAtender: function (condicionante, periodicidade) {
		var hdn = null;
		var situacao = null;
		var dataVenc = null;

		if (!periodicidade) {
			hdn = $(".hdnCondicionanteId[value=" + condicionante.Id + "]", CondicionanteSituacaoAlterar.container);
			situacao = condicionante.Situacao.Texto;
			dataVenc = condicionante.DataVencimento.DataTexto;
		} else {
			hdn = $(".hdnCondicionantePeriodicidadeId[value=" + periodicidade.Id + "]", CondicionanteSituacaoAlterar.container);
			situacao = periodicidade.Situacao.Texto;
			dataVenc = periodicidade.DataVencimento.DataTexto;
		}

		var tr = hdn.closest('tr');
		$(".CondSituacao", tr).text(situacao);
		$(".CondVencimento", tr).text(dataVenc);

		return true;
	}
}
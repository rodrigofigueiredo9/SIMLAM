/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

TituloAutorizacaoExploracaoFlorestal = {
	settings: {
		afterChangeProcDoc: null,
	},
	container: null,
	protocolo: null,

	load: function (especificidadeRef, options) {
		if (options) {
			$.extend(TituloAutorizacaoExploracaoFlorestal.settings, options);
		}

		TituloAutorizacaoExploracaoFlorestal.container = especificidadeRef;

		TituloAutorizacaoExploracaoFlorestal.container.delegate('.ddlExploracoes', 'change', TituloAutorizacaoExploracaoFlorestal.onChangeExploracao);
		TituloAutorizacaoExploracaoFlorestal.container.delegate('.btnExcluirExploracao', 'click', TituloAutorizacaoExploracaoFlorestal.excluirExploracao);
	},

	obterExploracoes: function () {
		var exploracao = {
			ExploracaoFlorestalId: $('.ddlExploracoes', TituloAutorizacaoExploracaoFlorestal.container).val(),
			TituloExploracaoFlorestalExploracaoList: []
		};

		$('.exploracaoId', TituloAutorizacaoExploracaoFlorestal.container).toArray().filter(x => x.value > 0).map(x =>
			exploracao.TituloExploracaoFlorestalExploracaoList.push({ ExploracaoFlorestalExploracaoId: x.value })
		);

		return exploracao;
	},

	onChangeExploracao: function () {
		var id = $('.ddlExploracoes', TituloAutorizacaoExploracaoFlorestal.container).val();
		if (id == 0 || id == "") {
			$('.tabExploracoes > tbody > tr:not(:first)', TituloAutorizacaoExploracaoFlorestal.container).remove();
			return;
		}

		var exploracoes = JSON.parse($('.ddlExploracoes option:selected', TituloLaudoExploracaoFlorestal.container).attr('detalhes').replaceAll("'", '"'));

		$(exploracoes).each((i, detalhe) => {
			var linha = '';
			linha = $('.trTemplateRow', TituloAutorizacaoExploracaoFlorestal.container).clone();

			linha.find('.descricao').text(detalhe.ExploracaoFlorestalExploracaoTexto);
			linha.find('.descricao').attr('title', detalhe.ExploracaoFlorestalExploracaoTexto);
			linha.find('.exploracaoId').val(detalhe.ExploracaoFlorestalExploracaoId);

			linha.removeClass('trTemplateRow hide');
			$('.tabExploracoes > tbody:last', TituloAutorizacaoExploracaoFlorestal.container).append(linha);
		});

		Listar.atualizarEstiloTable($('.tabExploracoes', TituloAutorizacaoExploracaoFlorestal.container));
	},

	excluirExploracao: function () {
		$(this).closest('tr').remove();
	}
};

Titulo.settings.obterExploracoesFunc = TituloAutorizacaoExploracaoFlorestal.obterExploracoes;
Titulo.addCallbackProtocolo(TituloAutorizacaoExploracaoFlorestal.onChangeProtocolo, true);
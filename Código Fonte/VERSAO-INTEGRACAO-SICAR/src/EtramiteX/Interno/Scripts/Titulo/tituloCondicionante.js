/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

TituloCondicionante = {
	settings: {
		urls: {
			adicionar: '',
			editar: '',
			visualizar: ''
		},
		isVisualizar: false
	},
	container: null,

	load: function (container, options) {
		TituloCondicionante.container = container;

		if (options) {
			$.extend(TituloCondicionante.settings, options);
		}

		$('.botaoAdicionarIcone', TituloCondicionante.container).click(TituloCondicionante.onBtnAdicionarClick);

		// find out if calling delegate is the correct thing.
		container.delegate('.btnDescer', 'click', TituloCondicionante.onBtnDescerClick);
		container.delegate('.btnSubir', 'click', TituloCondicionante.onBtnSubirClick);
		container.delegate('.btnEditar', 'click', TituloCondicionante.onBtnEditarClick);
		container.delegate('.btnExcluir', 'click', TituloCondicionante.onBtnExcluirClick);
		container.delegate('.btnVisualizar', 'click', TituloCondicionante.onBtnVisualizarClick);

		TituloCondicionante.atualizaEstiloGrid();

		MasterPage.botoes(container);

		if (TituloCondicionante.settings.isVisualizar) {
			$(".hide", TituloCondicionante.container).hide();
			$(".acoesCol", TituloCondicionante.container).width("10%");
		}
	},

	onBtnDescerClick: function () {
		var tr = $(this).closest('tr');
		tr.next().after(tr);
		TituloCondicionante.atualizaEstiloGrid();
	},

	onBtnSubirClick: function () {
		var tr = $(this).closest('tr');
		tr.prev().before(tr);
		TituloCondicionante.atualizaEstiloGrid();
	},

	atualizaEstiloGrid: function () {
		var table = TituloCondicionante.container.find('.dgCondicionantes');
		Listar.atualizarEstiloTable(table);

		var rows = $('tbody tr:visible', table).removeClass('selecionado');
		rows.each(function (index, elem) {
			var btnSubir = $(elem).find('.btnSubir');
			var btnDescer = $(elem).find('.btnDescer');

			if (index == 0) {
				btnSubir.addClass('desativado');
			} else {
				btnSubir.removeClass('desativado');
			}

			if (index >= rows.length - 1) {
				btnDescer.addClass('desativado');
			} else {
				btnDescer.removeClass('desativado');
			}
		});
	},

	onBtnVisualizarClick: function () {
		var tr = $(this).closest('tr');
		var condJson = tr.find('.hdnItemJson').val();
		Modal.abrir(TituloCondicionante.settings.urls.visualizar, { condicionanteId: condJson.Id, condicionanteJson: condJson }, function (container) {
			CondicionanteVisualizar.load(container, {});
		});
	},

	onBtnEditarClick: function () {
		var tr = $(this).closest('tr');
		tr.addClass('editando');
		var condJson = tr.find('.hdnItemJson').val();
		Modal.abrir(TituloCondicionante.settings.urls.editar, { condicionanteJson: condJson }, function (container) {
			CondicionanteSalvar.load(container, { onSalvar: TituloCondicionante.onEditar });
		});
	},

	onEditar: function (condicionante, condicionanteJson) {
		var tr = TituloCondicionante.container.find('tr.editando').removeClass('editando');
		TituloCondicionante.preencheCondicionanteTr(tr, condicionante, condicionanteJson);
		return true;
	},

	onBtnExcluirClick: function () {
		$(this).closest('tr').remove();
		TituloCondicionante.atualizaEstiloGrid();
	},

	onBtnAdicionarClick: function () {
		Modal.abrir(TituloCondicionante.settings.urls.adicionar, null, function (container) {
			CondicionanteSalvar.load(container, { onSalvar: TituloCondicionante.onAdicionar, btnSalvarLabel: 'Adicionar' });
		});
	},

	onAdicionar: function (condicionante, condicionanteJson) {
		var novoTr = $('.trCondTemplate', TituloCondicionante.container).clone().removeClass('trCondTemplate hide');
		$('.dgCondicionantes tbody', TituloCondicionante.container).append(novoTr);
		TituloCondicionante.preencheCondicionanteTr(novoTr, condicionante, condicionanteJson);
		MasterPage.redimensionar();
		TituloCondicionante.atualizaEstiloGrid();
		return true;
	},

	preencheCondicionanteTr: function (tr, condicionante, condicionanteJson) {
		$('.tdDescricao', tr).attr('title', condicionante.Descricao).html(Aux.htmlEncode(condicionante.Descricao));
		$('.tdSituacao', tr).attr('title', condicionante.Situacao).html(Aux.htmlEncode(condicionante.Situacao.Texto));
		$('.hdnItemId', tr).val(parseInt(condicionante.Id) || 0);
		$('.hdnItemJson', tr).val(condicionanteJson);
		tr.data('condicionante', condicionante);
	},

	obterCondicionantes: function (container) {
		var condicionantes = [];
		$('.dgCondicionantes tbody tr:visible .hdnItemJson', container).each(function () {
			var condicionanteJson = $(this).val();
			condicionantes.push(condicionanteJson);
		});
		return condicionantes;
	}
}
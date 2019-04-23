/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

Dua = {
	urlGerarPDF: null,
	urlReemitirDUA: null,
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(Dua.settings, options); }
		Dua.container = MasterPage.getContent(container);
		Dua.container.listarAjax({ onBeforeFiltrar: Dua.onBeforeFiltrar });

		container.delegate('.btnPDF', 'click', Dua.gerarPdf);
		container.delegate('.btnReemitir', 'click', Dua.reemitirDua);

		Aux.setarFoco(container);
	},

	obter: function () {

	},

	editar: function () {
		var id = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		MasterPage.redireciona(Dua.urlEditar + "/" + id);
	},

	visualizar: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;

		if (Dua.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', Dua.container).val() + "/" + itemId, null, function (context) {
				Modal.defaultButtons(context);
			}, Modal.tamanhoModalGrande);
		} else {
			MasterPage.redireciona($('.urlVisualizar', Dua.container).val() + "/" + itemId);
		}
	},

	excluir: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		Modal.excluir({
			'urlConfirm': Dua.urlExcluirConfirm,
			'urlAcao': Dua.urlExcluir,
			'id': itemId,
			'btnExcluir': this
		});
	},

	gerarPdf: function () {
		var item = $.parseJSON($(this).closest('tr').find('.itemJson').val());
		var numeroDua = item.numeroDua;
		var cpfcnpj = item.cpfCnpj;

		var urlGerar = Dua.urlGerarPDF + "/SefazDua/ObterPdfDua/" + numeroDua + "/DocumentoPessoa/" + cpfcnpj;
		Aux.downloadAjax("downloadPdfDua", urlGerar, null, 'get');
	},

	reemitirDua: function () {
		var item = $.parseJSON($(this).closest('tr').find('.itemJson').val());
		var titulo = $('.hdnTituloId').val();

		MasterPage.carregando(true);
		$.ajax({
			url: Dua.urlReemitirDUA,
			data: JSON.stringify({ dua: item.numeroDua, titulo: titulo }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Dua.container);
				MasterPage.carregando(false);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				var tabela = $('.gridDua');
				$('.gridDua > tbody > tr:not(".trTemplate")').remove();

				$.each(response.lstDua, function (i, item) {
				//.forEach(function (item) {
					var linha = $('.trTemplate', tabela).clone();	
					$(linha).removeClass('hide trTemplate');

					//adicionar na grid
					$('.hdnItemJson', linha).val(JSON.stringify(item));
					$('.lblCodigo', linha).html(item.Codigo).attr('title', item.Codigo);
					$('.lblValor', linha).html(item.Valor).attr('title', item.Valor);
					$('.lblSituacao', linha).html(item.SituacaoTexto).attr('title', item.SituacaoTexto);
					$('.lblNumero', linha).html(item.Numero).attr('title', item.Numero);
					$('.lblValidade', linha).html(item.Validade.DataTexto).attr('title', item.Validade.DataTexto);

					$('tbody', tabela).append(linha);
				});
				
				
			}
		});
		MasterPage.carregando(false);
	}
};
﻿/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

ExploracaoFlorestalExploracao = {
	settings: {
		mensagens: null,
		idsTela: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(ExploracaoFlorestalExploracao.settings, options); }
		ExploracaoFlorestalExploracao.container = MasterPage.getContent(container);

		ExploracaoFlorestalExploracao.container.delegate('.btnAdicionarProduto', 'click', ExploracaoFlorestalExploracao.adicionar);
		ExploracaoFlorestalExploracao.container.delegate('.btnExcluirProduto', 'click', ExploracaoFlorestalExploracao.excluir);
		ExploracaoFlorestalExploracao.container.delegate('.asmConteudoInternoExpander.asmExpansivel', 'click', ExploracaoFlorestalExploracao.gerenciarExpandir);
		ExploracaoFlorestalExploracao.container.delegate('.ddlProduto', 'change', ExploracaoFlorestalExploracao.onSelecionarProduto);

		ExploracaoFlorestalExploracao.atualizarMascaras();

	},

	atualizarMascaras: function () {
		$('.maskInteger').each(function () {
			if (!isNaN($(this).val())) {
				$(this).val(Globalize.format(Number($(this).val()), "n0"));
			}
		});

		$('.maskDecimalPonto').each(function () {
			if (!isNaN($(this).val())) {
				$(this).val(Globalize.format(Number($(this).val()), "n2"));
			}
		});
	},

	gerenciarExpandir: function () {
		var container = $(this).closest('fieldset');

		$('.asmConteudoInterno', container).toggle('fast', function () {
			var visivel = $('.asmConteudoInterno', container).is(':visible');

			if (visivel) {
				$('.asmExpansivel', container).text('Clique aqui para ocultar detalhes');
			} else {
				$('.asmExpansivel', container).text('Clique aqui para ver mais detalhes');
			}
			$('.linkVejaMaisCampos', container).toggleClass('ativo', visivel);
		});

		return false;
	},

	onSelecionarProduto: function () {
		var container = $(this).closest('fieldset');
		var produto = $('.ddlProduto', container).val();

		if (produto == ExploracaoFlorestalExploracao.settings.idsTela.ProdutoSemRendimento) {
			$('.txtQuantidade').val('');
			$('.txtQuantidade', container).addClass('disabled').attr('disabled', 'disabled');
		}
		else {
			$('.txtQuantidade', container).removeClass('disabled').removeAttr('disabled');

			if (produto == ExploracaoFlorestalExploracao.settings.idsTela.ProdutoMouroesEstacas ||
				produto == ExploracaoFlorestalExploracao.settings.idsTela.ProdutoEscoras ||
				produto == ExploracaoFlorestalExploracao.settings.idsTela.ProdutoPalmito) {

				$('.txtQuantidade', container).addClass('maskInteger').removeClass('maskDecimalPonto').attr('maxlength', '9');

			} else {
				$('.txtQuantidade', container).addClass('maskDecimalPonto').removeClass('maskInteger').attr('maxlength', '12');
			}

			$('.txtQuantidade', container).val('');
			Mascara.load(container);
		}
	},

	adicionar: function () {
		Mensagem.limpar(ExploracaoFlorestalExploracao.container);

		var container = $(this).closest('fieldset');
		produto = {
			Id: 0,
			ProdutoId: Number($('.ddlProduto', container).val()),
			ProdutoTexto: $('.ddlProduto :selected', container).text(),
			Quantidade: $('.txtQuantidade', container).val()
		}

		if (produto.ProdutoId == ExploracaoFlorestalExploracao.settings.idsTela.ProdutoSemRendimento) {
			$('.tabExploracaoFlorestalExploracaoProduto tbody tr[class!="trTemplateRow hide"]', container).remove();
			$('.ddlProduto', container).addClass('disabled').attr('disabled', 'disabled');
			$('.btnAdicionarProduto', container).hide();
		}

		var mensagens = new Array();

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var prod = (JSON.parse(obj));
				if (prod.ProdutoId == produto.ProdutoId) {
					mensagens.push(jQuery.extend(true, {}, ExploracaoFlorestalExploracao.settings.mensagens.ProdutoDuplicado));
				}
			}
		});

		if (produto.ProdutoId == 0) {
			mensagens.push(jQuery.extend(true, {}, ExploracaoFlorestalExploracao.settings.mensagens.ProdutoTipoObrigatorio));
		}

		if (produto.Quantidade == '' && produto.ProdutoId != ExploracaoFlorestalExploracao.settings.idsTela.ProdutoSemRendimento) {
			mensagens.push(jQuery.extend(true, {}, ExploracaoFlorestalExploracao.settings.mensagens.QuantidadeObrigatoria));
		}

		if (mensagens.length > 0) {
			var sufixo = container.find('.txtIdentificacao').val();
			$(mensagens).each(function () {
				this.Campo += sufixo;
			});
			Mensagem.gerar(ExploracaoFlorestalExploracao.container, mensagens);
			return;
		}

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(produto));
		linha.find('.produto').html(produto.ProdutoTexto).attr('title', produto.ProdutoTexto);
		linha.find('.quantidade').html(produto.Quantidade).attr('title', produto.Quantidade);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.txtQuantidade', container).val('');
		$('.ddlProduto', container).find('option:first').attr('selected', 'selected');
	},

	excluir: function () {
		var container = $(this).closest('fieldset');

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var prod = (JSON.parse(obj));

				if (prod.ProdutoId == ExploracaoFlorestalExploracao.settings.idsTela.ProdutoSemRendimento) {
					$('.ddlProduto, .txtQuantidade', container).removeClass('disabled').removeAttr('disabled');
					$('.btnAdicionarProduto', container).show();
				}
			}
		});

		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	},

	obter: function () {
		var exploracoes = [];
		$('.divExploracaoFlorestalExploracao', ExploracaoFlorestalExploracao.container).each(function () {
		    var objeto = {
		        Id: $('.hdnExploracaoId', this).val(),
		        Identificacao: $('.txtIdentificacao', this).val(),
		        GeometriaTipoId: Number($('.hdnGeometriaId', this).val()),
		        ClassificacaoVegetacaoId: $('.ddlClassificacoesVegetais', this).val(),
		        ExploracaoTipoId: $('.ddlExploracaoTipo', this).val(),
		        AreaCroqui: Number($('.hdnAreaCroqui', this).val()),
		        QuantidadeArvores: $('.txtQuantidadeArvores', this).val(),
		        AreaRequerida: Mascara.getFloatMask($('.txtAreaRequerida', this).val()),
		        AreaRequeridaTexto: $('.txtAreaRequerida', this).val(),
		        ArvoresRequeridas: $('.txtArvoresRequeridas', this).val(),
		        Produtos: []
		    };

			if (objeto.GeometriaTipoId != 3) {
				objeto.QuantidadeArvores = objeto.QuantidadeArvores.toString().trim().replace(/\./g, '');
				objeto.ArvoresRequeridas = objeto.ArvoresRequeridas.toString().trim().replace(/\./g, '');
			}

			$('.hdnItemJSon', this).each(function (i, item) {
				var obj = String($(item).val());
				if (obj != '') {
					objeto.Produtos.push(JSON.parse(obj));
				}
			});

			exploracoes.push(objeto);
		});

		return exploracoes;
	}
}
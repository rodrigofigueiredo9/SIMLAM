/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

ExploracaoFlorestalExploracao = {
	settings: {
		mensagens: null,
		idsTela: null,
		getEspecie: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(ExploracaoFlorestalExploracao.settings, options); }
		ExploracaoFlorestalExploracao.container = MasterPage.getContent(container);

		ExploracaoFlorestalExploracao.container.delegate('.btnAdicionarProduto', 'click', ExploracaoFlorestalExploracao.adicionar);
		ExploracaoFlorestalExploracao.container.delegate('.btnExcluirProduto', 'click', ExploracaoFlorestalExploracao.excluir);
		ExploracaoFlorestalExploracao.container.delegate('.asmConteudoInternoExpander.asmExpansivel', 'click', ExploracaoFlorestalExploracao.gerenciarExpandir);
		ExploracaoFlorestalExploracao.container.delegate('.ddlProduto', 'change', ExploracaoFlorestalExploracao.onSelecionarProduto);
		ExploracaoFlorestalExploracao.container.delegate('.ddlFinalidade', 'change', ExploracaoFlorestalExploracao.onChangeFinalidade);
		ExploracaoFlorestalExploracao.container.delegate('.rbParecerFavoravel', 'change', ExploracaoFlorestalExploracao.onChangeParecerFavoravel);

		$('.ddlFinalidade', ExploracaoFlorestalExploracao.container).each(function () {
			ExploracaoFlorestalExploracao.gerenciarFinalidades(this.parentElement.parentElement.parentElement);
		});

		$('.rbParecerFavoravel', ExploracaoFlorestalExploracao.container).each(function () {
			$(this).attr('initialize', true);
			$(this).change();
		});

		ExploracaoFlorestalExploracao.atualizarMascaras();
		ExploracaoFlorestalExploracao.loadAutocomplete();
	},

	onChangeParecerFavoravel: function () {
		var container = this.parentElement.parentElement.parentElement.parentElement;
		var showConteudo = $(this).val() > 0;
		if ($(this).attr('initialize')) {
			showConteudo = this.checked;
			$(this).removeAttr('initialize');
		} 

		var area = $("label[for='ExploracaoFlorestal_Exploracoes_AreaCroqui']", container);
		var qtd = $("label[for='ExploracaoFlorestal_Exploracoes_QuantidadeArvores']", container);
		if (showConteudo) {
			$('.asmConteudoLink', container).show();
			if (area.length > 0)
				area[0].textContent = 'Área da atividade croqui (m²) – Autorizada';
			if (qtd.length > 0) {
				qtd[0].textContent = 'N° de árvores autorizadas';
				$("divQuantidade", container).show();
			}
		} else {
			$('.asmConteudoLink', container).hide();
			if (area.length > 0)
				area[0].textContent = 'Área da atividade croqui (m²)';
			if (qtd.length > 0) {
				qtd[0].textContent = 'N° de árvores';
				$("divQuantidade", container).hide();
			}
		}
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

		if (produto == 1) {
			$('.lblEspecie', container)[0].textContent = 'Nome cientifíco/comum';
		} else {
			$('.lblEspecie', container)[0].textContent = 'Nome cientifíco/comum *';
		}
	},

	adicionar: function () {
		Mensagem.limpar(ExploracaoFlorestalExploracao.container);

		var container = $(this).closest('fieldset');
		var destinacao = Number($('.ddlDestinacaoMaterial', container).val());
		produto = {
			Id: 0,
			ProdutoId: Number($('.ddlProduto', container).val()),
			ProdutoTexto: $('.ddlProduto :selected', container).text(),
			Quantidade: $('.txtQuantidade', container).val(),
			EspeciePopularId: $('.hdnEspecieId', container).val(),
			EspeciePopularTexto: $('.txtEspecie', container).val(),
			DestinacaoMaterialId: destinacao > 0 ? destinacao : null,
			DestinacaoMaterialTexto: destinacao > 0 ? $('.ddlDestinacaoMaterial :selected', container).text() : ""
		};

		if (produto.ProdutoId == ExploracaoFlorestalExploracao.settings.idsTela.ProdutoSemRendimento) {
			$('.tabExploracaoFlorestalExploracaoProduto tbody tr[class!="trTemplateRow hide"]', container).remove();
			$('.ddlProduto', container).addClass('disabled').attr('disabled', 'disabled');
			$('.txtEspecie', container).addClass('disabled').attr('disabled', 'disabled');
			$('.btnAdicionarProduto', container).hide();
		}

		var mensagens = new Array();

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var prod = (JSON.parse(obj));
				if (prod.ProdutoId == produto.ProdutoId && prod.EspecieId == produto.EspecieId) {
					mensagens.push(jQuery.extend(true, {}, ExploracaoFlorestalExploracao.settings.mensagens.ProdutoDuplicado));
				}
			}
		});

		if (produto.ProdutoId == 0) {
			mensagens.push(jQuery.extend(true, {}, ExploracaoFlorestalExploracao.settings.mensagens.ProdutoTipoObrigatorio));
		}

		if (produto.EspeciePopularId == 0 && produto.ProdutoId != 1) {
			mensagens.push(jQuery.extend(true, {}, ExploracaoFlorestalExploracao.settings.mensagens.EspecieObrigatoria));
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
		linha.find('.especie').html(produto.EspeciePopularTexto).attr('title', produto.EspeciePopularTexto);
		linha.find('.especieId').html(produto.EspeciePopularId).attr('value', produto.EspeciePopularId);
		linha.find('.destinacaoMaterial').html(produto.DestinacaoMaterialTexto).attr('title', produto.DestinacaoMaterialTexto);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.txtQuantidade', container).val('');
		$('.txtEspecie', container).val('');
		$('.hdnEspecieId', container).val('');
		$('.ddlProduto', container).find('option:first').attr('selected', 'selected');
		$('.ddlDestinacaoMaterial', container).find('option:first').attr('selected', 'selected');
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

	obter: function (container) {
		if (!container)
			container = ExploracaoFlorestalExploracao.container;
		var exploracoes = [];
		$('.divExploracaoFlorestalExploracao', container).each(function () {
			var parecerFavoravel = null;
			if (Array.from($('.rbParecerFavoravel', this)).filter(x => x.checked).length > 0)
				parecerFavoravel = Array.from($('.rbParecerFavoravel', this)).filter(x => x.checked)[0].value > 0;

		    var objeto = {
		        Id: $('.hdnExploracaoId', this).val(),
				ParecerFavoravel: parecerFavoravel,
		        Identificacao: $('.txtIdentificacao', this).val(),
		        GeometriaTipoId: Number($('.hdnGeometriaId', this).val()),
		        ClassificacaoVegetacaoId: $('.ddlClassificacoesVegetais', this).val(),
		        ExploracaoTipoId: $('.ddlExploracaoTipo', this).val(),
		        AreaCroqui: Number($('.hdnAreaCroqui', this).val()),
		        QuantidadeArvores: $('.txtQuantidadeArvores', this).val(),
		        AreaRequerida: Mascara.getFloatMask($('.txtAreaRequerida', this).val()),
		        AreaRequeridaTexto: $('.txtAreaRequerida', this).val(),
				ArvoresRequeridas: $('.txtArvoresRequeridas', this).val(),
				FinalidadeExploracao: $('.ddlFinalidade option:selected', this).val(),
				FinalidadeEspecificar: $('.txtFinalidadeEspecificar', this).val(),
				Produtos: [],
				ExploracaoFlorestalGeo: null
			};

			if ($('.hdnDetalheGeo', this).val() != "")
				objeto.ExploracaoFlorestalGeo = JSON.parse($('.hdnDetalheGeo', this).val());

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
	},

	loadAutocomplete: function () {
		$(".txtEspecie").autocomplete({
			maxShowItems: 20,
			source: function (request, response) {
				var tags = [];
				$.ajax({
					url: ExploracaoFlorestalExploracao.settings.getEspecie,
					data: { "Search": request.term, "PageSize": 20 },
					type: 'GET',
					dataType: 'json',
					contentType: 'application/json;charset=UTF-8',
					success: function (result) {
						if (result.data != null) {
							tags = result.data.map(x => JSON.parse('{ "label": \"' + x.nomeAmigavel + '\", "value": \"' + x.nomeAmigavel + '\", "id": \"' + x.especiePopularId +'\" }'));
						}
						response(tags);
					},
				});
			},
			select: function (event, ui) {
				$(".hdnEspecieId").val(ui.item.id);
			}
		});
	},
	
	onChangeFinalidade: function () {
		var container = this.parentElement.parentElement.parentElement;
		$('.divEspecificarFinalidade', container).removeClass('hide');
		$('.divEspecificarFinalidade', container).addClass('hide');
		ExploracaoFlorestalExploracao.gerenciarFinalidades(container);
	},

	gerenciarFinalidades: function (container) {
		if ($('.ddlFinalidade option:selected', container).val() == 8) {
			$('.divEspecificarFinalidade', container).removeClass('hide');
		}
	}	
}
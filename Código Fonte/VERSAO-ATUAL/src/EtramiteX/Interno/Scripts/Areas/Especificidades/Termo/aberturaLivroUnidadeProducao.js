/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

AberturaLivroUnidadeProducao = {
	settings: {
		Mensagens: null,
		urls: {
			obterDadosAberturaLivroUnidadeProducao: null,
			obterUnidadeProducaoItem: null
		}
	},

	container: null,

	load: function (especificidadeRef) {
		AberturaLivroUnidadeProducao.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);

		$('.btnAddUnidade', AberturaLivroUnidadeProducao.container).click(AberturaLivroUnidadeProducao.adicionarUnidade);
		AberturaLivroUnidadeProducao.container.delegate('.btnExcluir', 'click', AberturaLivroUnidadeProducao.excluirLinha);
	},

	obterDadosAberturaLivroUnidadeProducao: function (protocolo) {
		if (protocolo == null) {
			$('.ddlUnidadeProducao', AberturaLivroUnidadeProducao.container).ddlClear();
			return;
		}

		$.ajax({
			url: AberturaLivroUnidadeProducao.settings.urls.obterDadosAberturaLivroUnidadeProducao,
			data: JSON.stringify({ id: protocolo.Id }),
			cache: false,
			async: true,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(AberturaLivroUnidadeProducao.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.UnidadesProducoes) {
					$('.ddlUnidadeProducao', AberturaLivroUnidadeProducao.container).ddlLoad(response.UnidadesProducoes, { disabled: false });
				}
			}
		});
	},

	adicionarUnidade: function () {
		Mensagem.limpar(Titulo.container);
		var mensagens = new Array();
		var tabela = $('.tbUnidade', AberturaLivroUnidadeProducao.container);
		var item = $('.ddlUnidadeProducao', AberturaLivroUnidadeProducao.container).ddlSelecionado();

		if (item.Id == 0) {
			mensagens.push(jQuery.extend(true, {}, AberturaLivroUnidadeProducao.settings.Mensagens.UnidadeProducaoObrigatorio));
		}

		var JsonParser = JsonBigint();

		$('tbody tr:not(.trTemplateRow) .hdnItemJSon', tabela).each(function () {
			var itemAdd = JsonParser.parse($(this).val());
			if (item.Id == itemAdd.Id) {
				mensagens.push(jQuery.extend(true, {}, AberturaLivroUnidadeProducao.settings.Mensagens.UnidadeProducaoJaAdicionada));
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(Titulo.container, mensagens);
			return;
		}

		$.ajax({
			url: AberturaLivroUnidadeProducao.settings.urls.obterUnidadeProducaoItem,
			data: JSON.stringify({ id: item.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Titulo.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				var validResponse = JsonParser.parse(XMLHttpRequest.responseText)

				if (validResponse.EhValido) {
					item = validResponse.Item;
				}
			}
		});

		var linha = $('.trTemplateRow', tabela).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JsonParser.stringify(item));
		linha.find('.UnidadeProducao').html(item.CodigoUP.toString()).attr('title', item.CodigoUP.toString());
		linha.find('.CulturaCultivar').html(item.CulturaTexto + ' ' + item.CultivarTexto).attr('title', item.CulturaTexto + ' ' + item.CultivarTexto);
		linha.find('.QuantidadeAno').html(String(item.EstimativaProducaoQuantidadeAno).replace('.', ',')).attr('title', String(item.EstimativaProducaoQuantidadeAno).replace('.', ','));

		$('tbody:last', tabela).append(linha);
		Listar.atualizarEstiloTable(tabela);

		$('.ddlUnidadeProducao', AberturaLivroUnidadeProducao.container).ddlFirst();
	},

	excluirLinha: function () {
		var container = $(this).closest('table');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container);
	},

	obter: function () {
		Mensagem.limpar(container);
		var container = AberturaLivroUnidadeProducao.container;

		var especificidade = {
			TotalPaginasLivro: $('.txtTotalPaginasLivro', container).val(),
			PaginaInicial: $('.txtPaginaInicial', container).val(),
			PaginaFinal: $('.txtPaginaFinal', container).val(),
			Unidades: []
		};

		var JsonParser = JsonBigint();

		//Unidades
		$('.tbUnidade tbody tr:not(.trTemplateRow) .hdnItemJSon', AberturaLivroUnidadeProducao.container).each(function () {
			especificidade.Unidades.push(JsonParser.parse($(this).val()));
		});

		return especificidade;
	}
};

Titulo.settings.especificidadeLoadCallback = AberturaLivroUnidadeProducao.load;
Titulo.addCallbackProtocolo(AberturaLivroUnidadeProducao.obterDadosAberturaLivroUnidadeProducao);
Titulo.settings.obterEspecificidadeObjetoFunc = AberturaLivroUnidadeProducao.obter;
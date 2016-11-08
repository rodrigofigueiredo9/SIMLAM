/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

AberturaLivroUnidadeConsolidacao = {
	settings: {
		urls: {
			obterDadosAberturaLivroUnidadeConsolidacao: ''
		},
		mensagens: null
	},

	container: null,

	load: function (especificidadeRef) {
		AberturaLivroUnidadeConsolidacao.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);

		$('.btnAdicionarCultura', AberturaLivroUnidadeConsolidacao.container).click(AberturaLivroUnidadeConsolidacao.adicionarCultura);
		AberturaLivroUnidadeConsolidacao.container.delegate('.btnExcluirItem', 'click', AberturaLivroUnidadeConsolidacao.excluirCultura);
	},

	obterDadosAberturaLivroUnidadeConsolidacao: function (protocolo) {
		if (protocolo == null) {
			$('.ddlUnidadeConsolidacao', AberturaLivroUnidadeConsolidacao.container).ddlClear();
			return;
		}

		$.ajax({
			url: AberturaLivroUnidadeConsolidacao.settings.urls.obterDadosAberturaLivroUnidadeConsolidacao,
			data: JSON.stringify({ id: protocolo.Id }),
			cache: false,
			async: true,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(AberturaLivroUnidadeConsolidacao.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Culturas) {
					$('.ddlCultura', AberturaLivroUnidadeConsolidacao.container).ddlLoad(response.Culturas);
				}
			}
		});
	},

	adicionarCultura: function () {
		var tabela = $('.gridCulturas', AberturaLivroUnidadeConsolidacao.container);
		var item = $('.ddlCultura', AberturaLivroUnidadeConsolidacao.container).ddlSelecionado();

		if (item.Id == 0) {
			Mensagem.gerar(MasterPage.getContent(AberturaLivroUnidadeConsolidacao.container), [AberturaLivroUnidadeConsolidacao.settings.mensagens.CulturaObrigatoria]);
			return;
		}

		var itemAdicionado = false;
		$('tbody tr', tabela).each(function () {
			if ($('.hdnItemId', this).val() == item.Id) {
				itemAdicionado = true;
			}
		});

		if (itemAdicionado) {
			Mensagem.gerar(MasterPage.getContent(AberturaLivroUnidadeConsolidacao.container), [AberturaLivroUnidadeConsolidacao.settings.mensagens.CulturaJaAdicionada]);
			return;
		}

		var linha = $('.trTemplate', tabela).clone();
		$('.lblNome', linha).append(item.Texto);
		$('.hdnItemId', linha).val(item.Id);

		$(linha).removeClass('hide').removeClass('trTemplate');
		$('tbody', tabela).append(linha);

		Listar.atualizarEstiloTable(tabela);
		$('.ddlCultura', UnidadeProducao.container).ddlFirst();
	},

	excluirCultura: function () {
		var tabela = $(this).closest('table');
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable(tabela);
	},

	obter: function () {
		var container = AberturaLivroUnidadeConsolidacao.container;
		Mensagem.limpar(container);

		var objeto = {
			TotalPaginasLivro: $('.txtTotalPaginasLivro', container).val(),
			PaginaInicial: $('.txtPaginaInicial', container).val(),
			PaginaFinal: $('.txtPaginaFinal', container).val(),
			Culturas: []
		};

		$('.gridCulturas tbody tr:not(.trTemplate)', container).each(function () {
			objeto.Culturas.push({
				Id: +$('.hdnItemId', this).val(),
				IdRelacionamento: +$('.hdnRelacionamentoId', this).val(),
				Nome: $('.lblNome', this).text()
			});
		});

		return objeto;
	}
};

Titulo.settings.especificidadeLoadCallback = AberturaLivroUnidadeConsolidacao.load;
Titulo.addCallbackProtocolo(AberturaLivroUnidadeConsolidacao.obterDadosAberturaLivroUnidadeConsolidacao);
Titulo.settings.obterEspecificidadeObjetoFunc = AberturaLivroUnidadeConsolidacao.obter;
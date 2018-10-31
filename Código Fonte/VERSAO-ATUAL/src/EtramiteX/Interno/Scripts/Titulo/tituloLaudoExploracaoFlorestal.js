/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

TituloLaudoExploracaoFlorestal = {
	settings: {
		afterChangeProcDoc: null,
		Mensagens: null
	},
	container: null,
	protocolo: null,

	load: function (especificidadeRef, options) {
		if (options) {
			$.extend(TituloLaudoExploracaoFlorestal.settings, options);
		}

		TituloLaudoExploracaoFlorestal.container = especificidadeRef;

		TituloLaudoExploracaoFlorestal.container.delegate('.btnAddCaracterizacao', 'click', TituloLaudoExploracaoFlorestal.adicionarCaracterizacao);
		TituloLaudoExploracaoFlorestal.container.delegate('.btnExcluirExploracao', 'click', TituloLaudoExploracaoFlorestal.excluirCaracterizacao);
	},

	obterExploracoes: function () {
		return $('.hdnItemJSon', TituloLaudoExploracaoFlorestal.container).toArray().filter(x => x.value != "").map(x => JSON.parse(x.value));
	},

	publicarMensagem: function (mensagens) {
		if (mensagens.length > 0) {
			Mensagem.gerar(TituloLaudoExploracaoFlorestal.container, mensagens);
			return true;
		}
		return false;
	},

	adicionarCaracterizacao: function () {
		Mensagem.limpar(TituloLaudoExploracaoFlorestal.container);
		var mensagens = new Array();
		var tabela = $('.tabCaracterizacao tbody tr', TituloLaudoExploracaoFlorestal.container);

		var id = $('.ddlCaracterizacoes', TituloLaudoExploracaoFlorestal.container).val();
		if (id == 0 || id == "") return;
		var descricao = $('.ddlCaracterizacoes option:selected', TituloLaudoExploracaoFlorestal.container).html();
		var parecerFavoravel = $('.ddlCaracterizacoes option:selected', TituloLaudoExploracaoFlorestal.container).attr('parecerfavoravel');
		var parecerDesfavoravel = $('.ddlCaracterizacoes option:selected', TituloLaudoExploracaoFlorestal.container).attr('parecerdesfavoravel');

		$(tabela).each(function (i, cod) {
			if ($('.exploracaoId', cod).val() == id) {
				mensagens.push(TituloLaudoExploracaoFlorestal.Mensagens.CaracterizacaoDuplicada);
			}
		});

		if (TituloLaudoExploracaoFlorestal.publicarMensagem(mensagens)) {
			return false;
		}

		//monta o objeto 
		var objeto = {
			ExploracaoFlorestalId: id,
			ExploracaoFlorestalTexto: descricao
		};

		var linha = '';
		linha = $('.trTemplateRow', TituloLaudoExploracaoFlorestal.container).clone();

		linha.find('.hdnItemJSon').val(JSON.stringify(objeto));
		linha.find('.descricao').text(descricao);
		linha.find('.descricao').attr('title', descricao);
		linha.find('.exploracaoId').val(id);
		linha.find('.parecerFavoravel').val(parecerFavoravel);
		linha.find('.parecerDesfavoravel').val(parecerDesfavoravel);

		linha.removeClass('trTemplateRow hide');
		$('.tabCaracterizacao > tbody:last', TituloLaudoExploracaoFlorestal.container).append(linha);

		Listar.atualizarEstiloTable($('.tabCaracterizacao', TituloLaudoExploracaoFlorestal.container));

		//limpa os campos de texto 
		$('.ddlCaracterizacoes', TituloLaudoExploracaoFlorestal.container).val('');
	},

	excluirCaracterizacao: function () {
		$(this).closest('tr').remove();
	}
};


Titulo.settings.obterExploracoesFunc = TituloLaudoExploracaoFlorestal.obterExploracoes;
Titulo.addCallbackProtocolo(TituloLaudoExploracaoFlorestal.onChangeProtocolo, true);
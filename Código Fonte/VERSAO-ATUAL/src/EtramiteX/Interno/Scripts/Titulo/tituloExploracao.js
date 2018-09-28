/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

TituloExploracao = {
	settings: {
		afterChangeProcDoc: null,
		Mensagens: null
	},
	container: null,
	protocolo: null,

	load: function (especificidadeRef, options) {
		if (options) {
			$.extend(TituloExploracao.settings, options);
		}

		TituloExploracao.container = especificidadeRef;

		TituloExploracao.container.delegate('.btnAddCaracterizacao', 'click', TituloExploracao.adicionarCaracterizacao);
		TituloExploracao.container.delegate('.btnExcluirExploracao', 'click', TituloExploracao.excluirCaracterizacao);
	},

	obterExploracoes: function () {
		return $('.hdnItemJSon', TituloExploracao.container).toArray().filter(x => x.value != "").map(x => JSON.parse(x.value));
	},

	publicarMensagem: function (mensagens) {
		if (mensagens.length > 0) {
			Mensagem.gerar(TituloExploracao.container, mensagens);
			return true;
		}
		return false;
	},

	adicionarCaracterizacao: function () {
		Mensagem.limpar(TituloExploracao.container);
		var mensagens = new Array();
		var tabela = $('.tabCaracterizacao tbody tr', TituloExploracao.container);

		var id = $('.ddlCaracterizacoes', TituloExploracao.container).val();
		if (id == 0 || id == "") return;
		var descricao = $('.ddlCaracterizacoes option:selected', TituloExploracao.container).html();
		var parecerFavoravel = $('.ddlCaracterizacoes option:selected', TituloExploracao.container).attr('parecerfavoravel');
		var parecerDesfavoravel = $('.ddlCaracterizacoes option:selected', TituloExploracao.container).attr('parecerdesfavoravel');

		$(tabela).each(function (i, cod) {
			if ($('.exploracaoId', cod).val() == id) {
				mensagens.push(TituloExploracao.Mensagens.CaracterizacaoDuplicada);
			}
		});

		if (TituloExploracao.publicarMensagem(mensagens)) {
			return false;
		}

		//monta o objeto 
		var objeto = {
			ExploracaoFlorestalId: id,
			ExploracaoFlorestalTexto: descricao
		};

		var linha = '';
		linha = $('.trTemplateRow', TituloExploracao.container).clone();

		linha.find('.hdnItemJSon').val(JSON.stringify(objeto));
		linha.find('.descricao').text(descricao);
		linha.find('.descricao').attr('title', descricao);
		linha.find('.exploracaoId').val(id);
		linha.find('.parecerFavoravel').val(parecerFavoravel);
		linha.find('.parecerDesfavoravel').val(parecerDesfavoravel);

		linha.removeClass('trTemplateRow hide');
		$('.tabCaracterizacao > tbody:last', TituloExploracao.container).append(linha);

		Listar.atualizarEstiloTable($('.tabCaracterizacao', TituloExploracao.container));

		//limpa os campos de texto 
		$('.ddlCaracterizacoes', TituloExploracao.container).val('');
	},

	excluirCaracterizacao: function () {
		$(this).closest('tr').remove();
	}
};


Titulo.settings.obterExploracoesFunc = TituloExploracao.obterExploracoes;
Titulo.addCallbackProtocolo(TituloExploracao.onChangeProtocolo, true);
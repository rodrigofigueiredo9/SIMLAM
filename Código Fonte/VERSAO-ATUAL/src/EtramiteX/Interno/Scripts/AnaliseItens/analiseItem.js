/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />

AnaliseItem = {
	settings: {
		urls: {
			salvarAnaliseItem: ''
		}
	},

	associarFuncao: null,
	container: null,
	mensagens: null,
	situacoes: null,

	load: function (container) {
		Modal.defaultButtons(container, AnaliseItem.adicionar, "Analisar");
		AnaliseItem.container = MasterPage.getContent(container);

		container.delegate('.ddlSituacao', 'change', AnaliseItem.onChangeSituacao);
		AnaliseItem.onChangeSituacao();
	},

	onChangeSituacao: function () {

		var situacaoId = +$('.ddlSituacao', AnaliseItem.container).val();

		switch (situacaoId) {

			case AnaliseItem.situacoes.Reprovado:
			case AnaliseItem.situacoes.Dispensado:
				$('.divMotivo').removeClass('hide');
				break;

			default:
				$('.divMotivo').addClass('hide');
				$('.txtMotivo', AnaliseItem.container).val('');
				break;
		}
	},

	adicionar: function (container) {
		var mensagens = new Array();
		var container = $(container).find('.modalContent');

		var Item = {
			DataAnalise: $('.txtDataAnalise', container).val(),
			Situacao: $('.ddlSituacao :selected', container).val(),
			SituacaoTexto: $('.ddlSituacao :selected', container).text(),
			Motivo: $('.txtMotivo', container).val()
		};

		if (Item.Situacao <= 0) {
			mensagens.push(AnaliseItem.mensagens.SituacaoObrigatorio);
		}

		if ((Item.Situacao == AnaliseItem.situacoes.Reprovado ||
			Item.Situacao == AnaliseItem.situacoes.Dispensado) && $('.txtMotivo', container).val() == '') {
			mensagens.push(AnaliseItem.mensagens.MotivoObrigatorio);
		}

		if (mensagens.length > 0) {
			Mensagem.gerar(AnaliseItem.container, mensagens);
			return;
		}

		AnaliseItem.associarFuncao(Item);
		Modal.fechar(container);

	}
}
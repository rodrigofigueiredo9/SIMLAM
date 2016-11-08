/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

AtividadeListar = {
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		if (options) { $.extend(AtividadeListar.settings, options); }
		AtividadeListar.container = MasterPage.getContent(container);
		AtividadeListar.container.listarAjax({ mensagemContainer: Modal.getModalContent(container) });
		Modal.defaultButtons(AtividadeListar.container);

		$('.txtAtividadeNome', AtividadeListar.container).focus();
		AtividadeListar.container.delegate('.btnAssociarAtividadeEmp', 'click', AtividadeListar.associar);
	},

	associar: function () {
		var linha = $(this).closest('tr');
		var idAtividade = linha.find('.atividadeEmpId').val();
		var textoAtividade = linha.find('.atividadeEmpTexto').text();
		textoAtividade = $.trim(textoAtividade).replace("\n", "");

		var retorno = AtividadeListar.settings.associarFuncao({ id: idAtividade, texto: textoAtividade });
		if (retorno === true) {
			Modal.fechar(AtividadeListar.container);
		} else {
			Mensagem.gerar(AtividadeListar.container, data.Msg);
		}
	}
}
/// <reference path="../JQuery/jquery-1.4.3.js"/>
/// <reference path="../masterpage.js"/>
/// <reference path="../mensagem.js"/>


Administrador = {
	requestResults: {},
	lastProcessedIndex: 0,
	totalLength: -1,
	consumirAtivo: false,

	load: function () {
		$('.celulaSeletorLinha').click(Administrador.onAdicionarPapel);
		$('[name="AlterarSenha"]').change(Administrador.onAlterarSenhaChange);
		$('.alterarSenhaContainer').toggle($('[name="AlterarSenha"]').is(':checked'));
	},

	onAlterarSenhaChange: function () {
		$('[name="Senha"], [name="ConfirmarSenha"]').val('');
		$('.alterarSenhaContainer').toggle($(this).is(':checked'));
	},

	onAdicionarPapel: function (e) {
		SetClickCheckBox(e, this);
		var valor = $("#tablePapeis input").serializeArray();
		var txtPermissoes = $("#TextoPermissoes");

		$.get(urlPermissao, valor, function (data, textStatus, XMLHttpRequest) {

			if (Aux.errorGetPost(data, textStatus, XMLHttpRequest, $("#central"))) {
				return;
			}

			Administrador.totalLength++;
			if (data) {
				Administrador.requestResults[Administrador.totalLength] = data;
			} else {
				Administrador.requestResults[Administrador.totalLength] = -1;
			}
		}, "text");

		Administrador.consumirTexto();
	},

	consumirTexto: function () {

		if (!Administrador.consumirAtivo) {

			Administrador.consumirAtivo = true;
			var txtPermissoes = $("#TextoPermissoes");

			var intervalId = setInterval(function () {
				if (Administrador.requestResults[Administrador.lastProcessedIndex]) {
					if (Administrador.requestResults[Administrador.lastProcessedIndex] != -1) {
						txtPermissoes.val(Administrador.requestResults[Administrador.lastProcessedIndex]);
					} else {
						txtPermissoes.val("");
					}
					Administrador.lastProcessedIndex++;
				} else if (Administrador.totalLength != -1 && Administrador.lastProcessedIndex >= Administrador.totalLength) {
					Administrador.consumirAtivo = false;
					Administrador.requestResults = {};
					Administrador.lastProcessedIndex = 0;
					Administrador.totalLength = -1;
					clearInterval(intervalId);
				}
			}, 800);
		}
	},

	onExcluirLinha: function (ctr) {
		var tabela = $(ctr).parents(".dataGridTable");
		$(ctr).parents("tr").remove();
		Listar.atualizarEstiloTable(tabela);
	}
}

$(Administrador.load);
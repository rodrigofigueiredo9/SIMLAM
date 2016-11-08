/// <reference path="../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../mensagem.js" />

PersonalizadoAtribuirExecutor = {
	container: null,
	settings: {
		urls: {
			associar: '',
			atribuir: ''
		}
	},

	load: function (container, options) {
		if (options) {
			$.extend(PersonalizadoAtribuirExecutor.settings, options);
		}

		container = MasterPage.getContent(container);
		PersonalizadoAtribuirExecutor.container = container;

		container.delegate('.btnAssociar', 'click', PersonalizadoAtribuirExecutor.associar);
		container.delegate('.btnExcluir', 'click', PersonalizadoAtribuirExecutor.excluir);
		container.delegate('.btnAtribuir', 'click', PersonalizadoAtribuirExecutor.atribuir);
	},

	associar: function () {
		Modal.abrir(PersonalizadoAtribuirExecutor.settings.urls.associar, null, function (container) {
			FuncionarioListar.load(container, { associarFuncao: PersonalizadoAtribuirExecutor.callBackAssociar });
			Modal.defaultButtons(container);
		});
	},

	callBackAssociar: function (funcionario, container) {
		var tabela = $('.tabFuncionarios', PersonalizadoAtribuirExecutor.container);
		var arrayMensagem = new Array();

		$(tabela).find('.hdnItemId').each(function () {
			if ($(this).val() == funcionario.Id) {
				arrayMensagem.push({ Tipo: 3, Texto: 'Funcionário já está adicionado.' });
				return;
			}
		});

		if (arrayMensagem && arrayMensagem.length > 0) {
			return arrayMensagem;
		}

		var linha = $('.trTemplate', PersonalizadoAtribuirExecutor.container).clone().removeClass('trTemplate');
		linha.find('.hdnItemId').val(funcionario.Id);
		linha.find('.tdNome').text(funcionario.Nome).attr('title', funcionario.Nome);

		tabela.append(linha);
		Listar.atualizarEstiloTable(tabela);

		arrayMensagem.push({ Tipo: 2, Texto: 'Funcionário adicionado com sucesso.' });
		return arrayMensagem;
	},

	excluir: function () {
		var tabela = $(this).closest('tbody');
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable(tabela);
	},

	atribuir: function () {
		var usuarios = [];
		$('.tabFuncionarios tbody tr', PersonalizadoAtribuirExecutor.container).each(function () {
			usuarios.push({ Id: $('.hdnItemId', this).val(), Nome: $('.tdNome', this).text() });
		});

		var relatorio = {
			Id: $('.relatorioId', PersonalizadoAtribuirExecutor.container).val(),
			UsuariosPermitidos: usuarios
		};

		$.ajax({
			url: PersonalizadoAtribuirExecutor.settings.urls.atribuir,
			type: "POST",
			data: JSON.stringify(relatorio),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido && response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(PersonalizadoAtribuirExecutor.container, response.Msg);
					return;
				} else {
					MasterPage.redireciona(response.urlRedirecionar);
				}
			}
		});
	}
}
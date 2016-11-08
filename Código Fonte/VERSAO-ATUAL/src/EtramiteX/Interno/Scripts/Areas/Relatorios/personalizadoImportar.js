/// <reference path="../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />

PersonalizadoImportar = {
	container: null,
	settings: {
		mensagens: null,
		urls: {
			importar: '',
			importarSalvar: ''
		}
	},

	load: function (container, options) {
		if (options) {
			$.extend(PersonalizadoImportar.settings, options);
		}

		container = MasterPage.getContent(container);
		PersonalizadoImportar.container = container;

		container.delegate('.btnImportar', 'click', PersonalizadoImportar.importar);
	},

	validar: function () {
		var array = new Array();
		if ($('.txtNome', PersonalizadoImportar.container).val() == '') {
			array.push(PersonalizadoImportar.settings.mensagens.NomeObrigatorio);
		}

		if ($('.txtDescricao', PersonalizadoImportar.container).val() == '') {
			array.push(PersonalizadoImportar.settings.mensagens.DescricaoObrigatoria);
		}

		if ($('.inputFile', PersonalizadoImportar.container).val() == '') {
			array.push(PersonalizadoImportar.settings.mensagens.SelecioneArquivo);
		}

		if (array.length > 0) {
			Mensagem.gerar(PersonalizadoImportar.container, array);
			return false;
		}
		return true;
	},

	importar: function () {
		if (PersonalizadoImportar.validar()) {
			$('.btnImportar', PersonalizadoImportar.container).button({ disabled: true });
			var inputFile = $('.inputFile', PersonalizadoImportar.container);
			FileUpload.upload(PersonalizadoImportar.settings.urls.importar, inputFile, PersonalizadoImportar.callImportar);
		}
	},

	callImportar: function (controle, response, isHtml) {
		var retorno = eval('(' + response + ')');
		if (retorno.EhValido) {
			PersonalizadoImportar.finalizar(retorno.conteudo);
		} else {
			Mensagem.gerar(MasterPage.getContent(PersonalizadoImportar.container), retorno.Msg);
			$('.btnImportar', PersonalizadoImportar.container).button({ disabled: false });
		}
	},

	finalizar: function (configuracao) {
		var relatorio = {
			Nome: $('.txtNome', PersonalizadoImportar.container).val(),
			Descricao: $('.txtDescricao', PersonalizadoImportar.container).val(),
			ConfiguracaoSerializada: configuracao
		};
		$.ajax({
			url: PersonalizadoImportar.settings.urls.importarSalvar,
			type: "POST",
			data: JSON.stringify(relatorio),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (!response.EhValido && response.Msg && response.Msg.length > 0) {
					$('.btnImportar', PersonalizadoImportar.container).button({ disabled: false });
					Mensagem.gerar(PersonalizadoImportar.container, response.Msg);
					return;
				} else {
					MasterPage.redireciona(response.urlRedirecionar);
				}
			}
		});
	}
}
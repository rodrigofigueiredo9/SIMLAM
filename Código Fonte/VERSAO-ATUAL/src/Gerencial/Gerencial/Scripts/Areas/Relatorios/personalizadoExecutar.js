/// <reference path="../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../mensagem.js" />

PersonalizadoExecutar = {
	container: null,
	settings: {
		urls: {
			validar: '',
			gerar: ''
		}
	},

	load: function (container, options) {
		if (options) {
			$.extend(PersonalizadoExecutar.settings, options);
		}

		container = MasterPage.getContent(container);
		PersonalizadoExecutar.container = container;

		///muda a cor dos check selecionados
		$('.labelBig', PersonalizadoExecutar.container).click(PersonalizadoExecutar.checkBoxArccordion);
		$('.labelBig', PersonalizadoExecutar.container).each(function () {
			if ($(this).find('input:checkbox').is(':checked')) {
				$(this).addClass('ativo');
			} else {
				$(this).removeClass('ativo');
			};
		});

		container.delegate('.btnExecutar', 'click', PersonalizadoExecutar.executar);
		Aux.setarFoco(container);
	},

	checkBoxArccordion: function () {
		if ($(this).find('input:checkbox').is(':checked')) {
			$(this).addClass('ativo');
		} else {
			$(this).removeClass('ativo');
		};
	},

	executar: function () {
		var termos = [];
		$('.divTermo', PersonalizadoExecutar.container).each(function () {
			var objeto = JSON.parse($('.hdnTermoJSON', this).val());
			if (objeto.Campo.TipoDados == 5) {//Bitand
				objeto.Valor = $('.divChecks', this).cbVal();
			} else {
				objeto.Valor = $('.valorFiltro', this).val();
			}
			termos.push(objeto);
		});

		var params = {
			id: $('.relatorioId', PersonalizadoExecutar.container).val(),
			tipo: $('[name=tipoArquivo]:checked', PersonalizadoExecutar.container).val(),
			setor: $('.ddlSetor', PersonalizadoExecutar.container).val(),
			termos: termos
		};

		var retorno = MasterPage.validarAjax(PersonalizadoExecutar.settings.urls.validar, params, PersonalizadoExecutar.container, false);

		if (retorno.EhValido) {
			Aux.download(PersonalizadoExecutar.settings.urls.gerar, params);
		}
	}
}
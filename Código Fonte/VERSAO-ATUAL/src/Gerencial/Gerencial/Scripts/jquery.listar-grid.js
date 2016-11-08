/// <reference path="JQuery/jquery-1.4.3.js"/>

/**
 * Plugin jQuery para lidar com listagens.
 * Uso: $('formDaListagem').listarGrid(options);
 *
 * options:
 *	- submitCallback: (opcional) Função a ser chamada quando o form é "submetido" (troca de página, altera itens por página ou buscar). 
 *	É passado para a callback o elemento form. Se esta opcao não for especificada, é feito um .submit() no form. Esta opção pode ser usada, 
 *	por exemplo, para submits assíncronos.
 *
 *	- onBeforeSubmit: (opcional) Função chamada antes do submit. Pode usar ela para, por exemplo, remover campos indesejáveis do request.
 *
 **/
(function ($) {
	$.fn.listarGrid = function (options) {
		var settings = {
			'submitCallback': null,
			'onBeforeSubmit': null
		};

		return this.each(function () {
			if (options) {
				$.extend(settings, options);
			}

			var form = $(this);

			var hdnPaginaAtual = $('.listarNumPaginaAtual', form);
			var hdnPaginaFinal = $('.listarNumPaginaFinal', form);
			var hdnOrdenarPor = $('.listarHdnOrdenarPor', form);
			var hdnUltimaBusca = $('.listarUltimaBusca', form);

			var submitForm = function (theForm) {
				if (settings.onBeforeSubmit) {
					settings.onBeforeSubmit(theForm);
				}

				if (settings.submitCallback) {
					settings.submitCallback(theForm);
				}
				else {
					theForm.submit();
				}
			};

			$('.listarBtnBuscar', form).click(function () {
				hdnPaginaAtual.val(1);
				hdnUltimaBusca.val('');
				submitForm(form);
			});

			$('.listarQuantPaginacao', form).change(function () {
				hdnPaginaAtual.val(1);
				var indice = parseInt($('.listarQuantPaginacao', form).attr('value'));
				Cookie.set('QuantidadePorPagina', indice, Listar.QuantidadePaginaTimeOut);
				submitForm(form);
			});

			$('.listarBtnPag', form).click(function () {
				var pagPretendida = parseInt($(this).attr('class'));
				var pagAtual = parseInt(hdnPaginaAtual.val());

				if (pagAtual != pagPretendida) {
					hdnPaginaAtual.val(pagPretendida);
					submitForm(form);
				}
			});

			$('.listarBtnPagAnterior', form).click(function () {
				var pagAtual = parseInt(hdnPaginaAtual.val());
				if (pagAtual > 1) {
					hdnPaginaAtual.val(pagAtual - 1);
					submitForm(form);
				}
			});

			$('.listarBtnPagProxima', form).click(function () {
				var pagAtual = parseInt(hdnPaginaAtual.val());
				var pagFinal = parseInt(hdnPaginaFinal.val());

				if (pagAtual < pagFinal) {
					hdnPaginaAtual.val(pagAtual + 1);
					submitForm(form);
				}
			});

			$('.ordenavel th:not(.semOrdenacao)', form).click(function () {
				var indexColuna = $(this).parent().children().index(this) + 1;
				hdnOrdenarPor.val(indexColuna);
				submitForm(form);
				Listar.atualizarEstiloOrdenar(indexColuna);
			});
		});
	};
})(jQuery);
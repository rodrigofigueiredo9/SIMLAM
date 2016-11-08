/// <reference path="../jquery.json-2.2.min.js" />

Comparar = {
	load: function (container) {
		///Abre e fecha as informações
		function equalHeight(group) {
			var tallest = 0;
			group.each(function () {
				var thisHeight = $(this).height();
				if (thisHeight > tallest) {
					tallest = thisHeight;
				}
			});
			group.height(tallest);
		}

		$('.titToggle', container).each(function () {
			equalHeight($('.titToggle:contains(' + $(this).html() + ')', container).parent().find('.corpoToggle'));
		});

		$('.titToggle', container).click(
			function () {
				$('.titToggle:contains(' + $(this).html() + ')').parent().find('.corpoToggle').slideToggle('fast');
				$('.titToggle:contains(' + $(this).html() + ')').toggleClass('aberto');
			}
		);

		///Highlight das linhas de comparação
		$('.quadroToggle ul li', container).mouseenter(
			function () {
				var indexElemnt = $(this).index();
				var indexUl = $(this).parent().parent().index();
				$('.titToggle:contains(' + $(this).parents('.quadroToggle').find('.titToggle').html() + ')').each(function () {
					$(this).next().children().eq(indexUl).find('li').eq(indexElemnt).addClass('higlight').siblings().removeClass();
				});
			}
		);

		$('.quadroToggle ul li', container).mouseleave(
			function () {
				$('.quadroToggle ul li', container).removeClass('higlight');
			}
		);

		///Highlight a seleção da Base
		$('.radioSelect', container).click(
			function () {
				if ($(this).select()) {
					$('fieldset', container).removeClass('bgSelecionado');
					$(this).parents('fieldset').addClass('bgSelecionado');
				}
			}
		);

		if ($('.radioSelect:checked', container).length > 0) {
			$('.radioSelect:checked', container).parents('fieldset').addClass('bgSelecionado');
		}
	}
}
/// <reference path="Lib/JQuery/jquery-1.10.1-vsdoc.js" />

/* DropDownList------------------------------------------------------------------- */
/***********************************************************************************/
(function ($) {
	$.fn.ddlLoad = function (valores, options) {
		var settings = {
			textoDefault: DropDownText.selecione,
			disabled: true,
			disabledQtd: 1,
			limpar: true,
			selecionado: null
		};

		return this.each(function () {
			if (options) {
				$.extend(settings, options);
			}

			var dropDown = $(this);
			var selectId = null;

			if (valores.length == 1) {
				selectId = valores[0].Id;
			}

			if (settings.selecionado) {
				selectId = settings.selecionado;
			}

			dropDown.removeAttr('disabled').removeClass('disabled');
			if (settings.disabled && valores.length <= settings.disabledQtd) {
				dropDown.attr('disabled', 'disabled').addClass('disabled');
			}

			if (settings.textoDefault) {
				valores.splice(0, 0, { Id: 0, Texto: settings.textoDefault });
			}

			if (settings.limpar) {
				dropDown.find('option').remove();
			}

			$.each(valores, function () {
				dropDown.append('<option value="' + this.Id + '">' + this.Texto + '</option>');
			});

			if (selectId) {
				dropDown.val(selectId);
			}
		});
	};

	$.fn.ddlClear = function (options) {
		var settings = {
			textoDefault: DropDownText.selecione,
			disabled: true
		};

		return this.each(function () {
			if (options) {
				$.extend(settings, options);
			}
			
			var dropDown = $(this);
			var valores = [];
			
			dropDown.removeAttr('disabled').removeClass('disabled');
			if (settings.disabled) {
				dropDown.attr('disabled', 'disabled').addClass('disabled');
			}

			if (settings.textoDefault) {
				valores.splice(0, 0, { Id: 0, Texto: settings.textoDefault });
			}

			dropDown.find('option').remove();
			$.each(valores, function () {
				dropDown.append('<option value="' + this.Id + '">' + this.Texto + '</option>');
			});
		});
	};

	$.fn.ddlFirst = function (options) {
		var settings = {
			disabled: false
		};

		return this.each(function () {
			if (options) {
				$.extend(settings, options);
			}

			var dropDown = $(this);

			dropDown.removeAttr('disabled').removeClass('disabled');
			if (settings.disabled) {
				dropDown.attr('disabled', 'disabled').addClass('disabled');
			}

			dropDown.find('option:first').attr('selected', 'selected');
		});
	};

	$.fn.ddlSelect = function (options) {
		var settings = {
			selecionado: 0
		};

		return this.each(function () {
			if (options) {
				$.extend(settings, options);
			}

			var dropDown = $(this);
			var item = dropDown.find('option[value=' + settings.selecionado + ']');

			if (item.length > 0) {
				item.attr('selected', 'selected');
			} else {
				dropDown.find('option:first').attr('selected', 'selected');
			}
		});
	};

	$.fn.ddlSelecionado = function (options) {
		var settings = {
		};

		if (options) {
			$.extend(settings, options);
		}

		var dropDown = $(this);
		return { Id: +dropDown.val(), Texto: dropDown.find(':selected').text() };
	};

	$.fn.ddlCascate = function (dropDownB, options) {
		var settings = {
			url: null,
			data: null,
			disabled: true,
			autoFocus: true,
			callBack: null
		};

		return this.each(function () {
			if (options) {
				$.extend(settings, options);
			}

			var dropDownA = $(this);
			var id = parseInt(dropDownA.val());

			dropDownB.find('option').remove();

			var isDropDownBdisabled = dropDownB.is(':disabled');

			dropDownB.addClass('disabled').attr('disabled', 'disabled');

			if (settings.data) {
				settings.data.Id = id;
			} else {
				settings.data = { Id: id };
			}

			if (typeof id == 'undefined' || isNaN(id) || id <= 0) {
				dropDownB.append('<option value="0">' + DropDownText.selecione + '</option>');
			} else {
				dropDownB.append('<option value="0">*** Carregando valores ***</option>');

				$.ajax({ url: settings.url, data: settings.data, async: false, cache: false,
					error: function (jqXHR, status, errorThrown) {
						Aux.error(jqXHR, status, errorThrown, MasterPage.getContent(dropDownA));
					},
					success: function (valores) {
						dropDownB.ddlLoad(valores, options);

						if (isDropDownBdisabled && settings.autoFocus) {
							dropDownB.focus();
						}

						if (settings.callBack) {
							settings.callBack();
						}
					}
				});
			}
		});
	};
})(jQuery);

DropDownText = {
	selecione: '*** Selecione ***',
	todos: '*** Todos ***'
}
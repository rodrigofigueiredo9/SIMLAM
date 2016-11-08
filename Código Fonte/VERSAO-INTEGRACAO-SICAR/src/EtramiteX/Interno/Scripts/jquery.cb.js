/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />

/* CheckBoxList------------------------------------------------------------------- */
/***********************************************************************************/
(function ($) {
	$.fn.cbLoad = function (valores, options) {
		var settings = {
			disabled: false,
			tamanhoColuna: 50,
			tipoRelatorio: false
		};

		return this.each(function () {
			if (options) {
				$.extend(settings, options);
			}

			var containerCheckBox = $(this);

			if (settings.tipoRelatorio) {
				containerCheckBox.find('.spanCheck').remove();
				$.each(valores, function () {
					containerCheckBox.append(
					'<span class="spanCheck">' +
						'<label class="labelBig">' +
							'<input type="checkbox" class="cbCampo" title="' + this.Texto + '" value="' + this.Codigo + '" />' +
							'<span>' + this.Texto + '</span>' +
						'</label>' +
					'</span>');
				});
			} else {
				containerCheckBox.find('.labelCheckBox').remove();
				$.each(valores, function () {
					containerCheckBox.append(
					'<label class="labelCheckBox margemBottom coluna' + settings.tamanhoColuna + '">' +
						'<input type="checkbox" title="' + this.Texto + '" value="' + this.Codigo + '" />' + this.Texto +
					'</label>');
				});
			}

			if (settings.disabled) {
				containerCheckBox.find('input[type="checkbox"]').removeAttr('checked').attr('disabled', 'disabled').addClass('disabled');
			}
		});
	};

	$.fn.cbClear = function (options) {
		var settings = {
			esconder: true
		};

		return this.each(function () {
			if (options) {
				$.extend(settings, options);
			}

			var containerCheckBox = $(this);
			containerCheckBox.find('.labelCheckBox').remove();
		});
	};

	$.fn.cbDisabled = function (options) {
		var settings = {
			disabled: false
		};

		return this.each(function () {
			if (options) {
				$.extend(settings, options);
			}

			var containerCheckBox = $(this);
			
			if (settings.disabled) {
				containerCheckBox.find('input[type="checkbox"]').removeAttr('checked').attr('disabled', 'disabled').addClass('disabled');
			} else {
				containerCheckBox.find('input[type="checkbox"]').removeAttr('disabled').removeClass('disabled');
			}
		});
	};

	$.fn.cbVal = function (options) {
		var retorno = 0;
		var settings = {
		};

		if (options) {
			$.extend(settings, options);
		}

		var containerCheckBox = $(this);

		containerCheckBox.find('input[type="checkbox"]').each(function (index, check) {
			if ($(check).attr('checked')) {
				retorno += +$(check).val();
			}
		});

		return retorno;
	};

	$.fn.cbText = function (options) {
		var retorno = '';
		var settings = {
		};

		if (options) {
			$.extend(settings, options);
		}

		var containerCheckBox = $(this);

		containerCheckBox.find('input[type="checkbox"]').each(function (index, check) {
			if ($(check).attr('checked')) {
				retorno += $(check).closest('label').text() + '/';
			}
		});

		return retorno.substring(0, retorno.length - 1);
	};

	$.fn.cbSelectAll = function (options) {
		var settings = {
			select: true
		};

		return this.each(function () {
			if (options) {
				$.extend(settings, options);
			}

			var containerCheckBox = $(this);

			if (settings.select) {
				containerCheckBox.find('input[type="checkbox"]').attr('checked', 'checked');
			} else {
				containerCheckBox.find('input[type="checkbox"]').removeAttr('checked');
			}
		});
	};

})(jQuery);
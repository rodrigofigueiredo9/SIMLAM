/// <reference path="../JQuery/jquery-1.4.3.js"/>

/*
 *
 * Copyright (c) 2010 C. F., Wong (<a href="http://cloudgen.w0ng.hk">Cloudgen Examplet Store</a>)
 * Licensed under the MIT License:
 * http://www.opensource.org/licenses/mit-license.php
 *
 */
﻿(function(k,e,i,j){k.fn.caret=function(b,l){var a,c,f=this[0],d=k.browser.msie;if(typeof b==="object"&&typeof b.start==="number"&&typeof b.end==="number"){a=b.start;c=b.end}else if(typeof b==="number"&&typeof l==="number"){a=b;c=l}else if(typeof b==="string")if((a=f.value.indexOf(b))>-1)c=a+b[e];else a=null;else if(Object.prototype.toString.call(b)==="[object RegExp]"){b=b.exec(f.value);if(b!=null){a=b.index;c=a+b[0][e]}}if(typeof a!="undefined"){if(d){d=this[0].createTextRange();d.collapse(true);
d.moveStart("character",a);d.moveEnd("character",c-a);d.select()}else{this[0].selectionStart=a;this[0].selectionEnd=c}this[0].focus();return this}else{if(d){c=document.selection;if(this[0].tagName.toLowerCase()!="textarea"){d=this.val();a=c[i]()[j]();a.moveEnd("character",d[e]);var g=a.text==""?d[e]:d.lastIndexOf(a.text);a=c[i]()[j]();a.moveStart("character",-d[e]);var h=a.text[e]}else{a=c[i]();c=a[j]();c.moveToElementText(this[0]);c.setEndPoint("EndToEnd",a);g=c.text[e]-a.text[e];h=g+a.text[e]}}else{g=
f.selectionStart;h=f.selectionEnd}a=f.value.substring(g,h);return{start:g,end:h,text:a,replace:function(m){return f.value.substring(0,g)+m+f.value.substring(h,f.value[e])}}}}})(jQuery,"length","createRange","duplicate");


/**
 * Plugin jQuery para lidar com listagens.
 * Uso;// $('formDaListagem').listarGrid(options);
 *
 * options;//
 *	- submitCallback;// (opcional) Função a ser chamada quando o form é "submetido" (troca de página, altera itens por página ou buscar). 
 *	É passado para a callback o elemento form. Se esta opcao não for especificada, é feito um .submit() no form. Esta opção pode ser usada, 
 *	por exemplo, para submits assíncronos.
 *
 *	- onBeforeSubmit;// (opcional) Função chamada antes do submit. Pode usar ela para, por exemplo, remover campos indesejáveis do request.
 *
 **/


(function ($) {
	$.fn.simlamMask = function () {

		return this.each(function () {

			var ctr = $(this);

			var objMask = new TextInputMask.Construtor(TextInputMask.REGEX_GMS_LAT, TextInputMask.AUTOCOMPLETE_GMS);

			//W3C
			ctr.keypress(objMask.sparkTextInsertedAction);
			ctr.bind("input", objMask.sparkTextRemovedAction);
			//IE
			var elemDom = document.getElementById(ctr.attr("id"));
			elemDom.onpropertychange = objMask.sparkTextRemovedAction;

		});
	};
})(jQuery);

TextInputMask =
{
	REGEX_GMS_LAT: /^(-?(((90)|([0-8]\d)|\d)(:([0-5]?\d(:([0-5]?\d(\.\d{0,4})?)?)?)?)?)?)$/,
	REGEX_GMS_LAT_NSWE: /^((((90)|([0-8]\d)|\d)(:([0-5]?\d(:([0-5]?\d(\.\d{0,4})?)?)?)?)?)?)$/,
	REGEX_GMS_LNG: /^(-?(([0-9]{1,2}|1[0-7][0-9]|180)(:([0-5]?\d(:([0-5]?\d(\.\d{0,4})?)?)?)?)?)?)$/,
	REGEX_GMS_LNG_NSWE: /^((([0-9]{1,2}|1[0-7][0-9]|180)(:([0-5]?\d(:([0-5]?\d(\.\d{0,4})?)?)?)?)?)?)$/,
	AUTOCOMPLETE_GMS: [',', '.', '0', ':', ':0'],

	REGEX_GDEC_LAT: /^(-?(((90)|([0-8]\d)|\d)(\.\d{0,7})?)?)$/,
	REGEX_GDEC_LAT_NSWE: /^((((90)|([0-8]\d)|\d)(\.\d{0,7})?)?)$/,
	REGEX_GDEC_LNG: /^(-?((([0-9]{1,2})|(1[0-7][0-9])|(180))(\.\d{0,7})?)?)$/,
	REGEX_GDEC_LNG_NSWE: /^(((([0-9]{1,2})|(1[0-7][0-9])|(180))(\.\d{0,7})?)?)$/,
	AUTOCOMPLETE_GDEC: [',', '.', '0'],

	REGEX_AZIMUTH_GDEC: /^(((([0-9]{1,2})|([12][0-9]{2})|(3[0-5][0-9])|(360))(\.\d{0,4})?)?)$/,
	REGEX_AZIMUTH_GMS: /^((([0-9]{1,2})|([12][0-9]{2})|(3[0-5][0-9])|(360))(:([0-5]?\d(:([0-5]?\d(\.\d{0,4})?)?)?)?)?)$/,
	AUTOCOMPLETE_AZIMUTH_GDEC: [',', '.', '0'],
	AUTOCOMPLETE_AZIMUTH_GMS: [',', '.', '0', ':', ':0'],

	REGEX_DISTANCE: /^(-?(\d{0,9}(\.\d{0,4})?)?)$/,
	AUTOCOMPLETE_DISTANCE: [',', '.', '0'],

	REGEX_EASTING: /^(-?(\d{0,9}(\.\d{0,4})?)?)$/,
	AUTOCOMPLETE_EASTING: [',', '.', '0'],

	REGEX_NORTHING: /^(-?(\d{0,9}(\.\d{0,4})?)?)$/,
	AUTOCOMPLETE_NORTHING: [',', '.', '0'],

	Construtor: function (mask, autoCompleteValues) {

		var _enabled = true;
		var _target;
		var _mask = mask;
		var _autoCompleteValues = autoCompleteValues;

		var _lastValue;
		var _lastSelectionIdx;
		var _validating = false;
		var _objRef = this;

		this.sparkTextRemovedAction = function (event) {

			event = event || window.event;
			if (typeof event.propertyName != "undefined" && event.propertyName != "value")
				return;

			if (!_enabled || !_mask)
				return;

			var keyCode = event.which || event.keyCode;

			if (!_validating) {
				_validating = true;

				_target = this; //event.target || event.srcElement;

				var idxCursorInicio = $(_target).caret().start;

				var sizeDif = _target.value.length - (_lastValue)?_lastValue.length:0;

				if (keyCode > 0)
					return;

				if (!_mask.test(_target.value)) {
					_target.value = _lastValue;

					//_target.selectionStart = idxCursorInicio;
					//_target.selectionEnd = idxCursorInicio;
					$(_target).caret({ start: idxCursorInicio, end: idxCursorInicio });
				}
				else {
					_lastValue = _target.value;
				}

				_validating = false;
			}
		};

		/*this.getSelection = function (o, start) {

			if (o.createTextRange) {
				var r = document.selection.createRange().duplicate()

				if (start == 1) {
					return o.value.lastIndexOf(r.text);
				}
				else {
					return r.text.length;
				}

			} else {
				return (start == 1) ? o.selectionStart : o.selectionEnd;
			}
		};*/

		this.sparkValidateMask = function (value, target, autoCompleteChars, maskRegExp, trySwitchingDotsAndComma) {
			if (!_enabled || !_mask)
				return true;

			value = value.replace(/[\r?\n?]/g, "");


			var objCart = $(target).caret();
			var selectionIni = objCart.start;
			var selectionEnd = objCart.end;

			var textoAnt = target.value.substring(0, selectionIni);
			var textoPos = target.value.substr(selectionEnd);

			var texto = textoAnt + value + textoPos;
			var pos = textoAnt.length + value.length;

			var valueIsValid = false;


			//validando o texto
			if (maskRegExp.test(texto)) {
				target.value = texto;
				$(target).caret({ start: pos, end: pos });
				//target.selectionStart = pos;
				//target.selectionEnd = pos;
				valueIsValid = true;
			}
			else {
				if (autoCompleteChars != null) {
					var i; //uint;
					for (i = 0; i < autoCompleteChars.length; i++) {
						texto = textoAnt + autoCompleteChars[i].toString() + value + textoPos;
						pos = textoAnt.length + value.length + autoCompleteChars[i].toString().length;
						if (maskRegExp.test(texto)) {
							target.value = texto;
							$(target).caret({ start: pos, end: pos });
							//target.selectionStart = pos;
							//target.selectionEnd = pos;
							valueIsValid = true;
							break;
						}
					}

					if (!valueIsValid && value.length == 1 && textoPos.length > 0) {
						texto = textoAnt + value + textoPos.substr(1);
						pos = textoAnt.length + 1;
						if (maskRegExp.test(texto)) {
							target.value = texto;
							$(target).caret({ start: pos, end: pos });
							//target.selectionStart = pos;
							//target.selectionEnd = pos;
							valueIsValid = true;
						}
						else if (textoPos.length > 1) {

							for (i = 0; i < autoCompleteChars.length && !valueIsValid; i++) {
								texto = textoAnt + autoCompleteChars[i].toString() + value + textoPos.substr(autoCompleteChars[i].toString().length + value.length);
								pos = textoAnt.length + value.length + autoCompleteChars[i].toString().length;
								if (maskRegExp.test(texto)) {
									target.value = texto;
									$(target).caret({ start: pos, end: pos });
									//target.selectionStart = pos;
									//target.selectionEnd = pos;
									valueIsValid = true;
								}
							}
						}
					}
				}
			}

			if (trySwitchingDotsAndComma) {
				if (value.indexOf(',') >= 0)
					return _objRef.sparkValidateMask(value.replace(",", "."), target, autoCompleteChars, maskRegExp, false);
				else if (value.indexOf('.') >= 0)
					return _objRef.sparkValidateMask(value.replace(".", ","), target, autoCompleteChars, maskRegExp, false);
			}

			return valueIsValid;
		};


		this.sparkTextInsertedAction = function (event) {

			if (!_enabled || !_mask)
				return true;

			event = event || window.event;
			var keyCode = event.which || event.keyCode;

			_target = event.target;
			_lastSelectionIdx = $(_target).caret().start;//_target.selectionStart;

			//if (!_validating && !(event.ctrlKey && keyCode > 46/*(keyCode == 118 || keyCode == 86)*/)) {
			if (!_validating && keyCode > 46 && !(event.ctrlKey && (keyCode == 86 || keyCode == 67))) {

				_validating = true;

				var text = "";

				if (keyCode > 0) {
					event.preventDefault();
					text = String.fromCharCode(keyCode);
					_objRef.sparkValidateMask(text, _target, _autoCompleteValues, _mask);
				}

				_lastValue = _target.value;
				_validating = false;
			}

			return true;
		};

		this.generateMask = function (format, numberIdentifier, alphaIdentifier, alphanumericIdentifier) {
			var exp = "";
			for (var i = format.length; i > 0; i--) {
				var char = format.charAt(i - 1);
				if (char == numberIdentifier)
					char = "[0-9]";
				else if (char == alphaIdentifier)
					char = "[A-Za-z]";
				else if (char == alphanumericIdentifier)
					char = "[0-9A-Za-z]";
				else
					if (isNaN(parseInt(char)))
						char = "\\" + char;

				exp = "(" + char + exp + ")?";
			}

			return new RegExp("^(" + exp + ")$");
		}

		return this;
	}
}

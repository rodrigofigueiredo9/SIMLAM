/* =========================================================
	Arquivo adaptado para Tecnomapas Ltda
 * ========================================================= */

String.prototype.insert = function (index, value) {

    var result = this.split('');
    var aux = '';
    var count = result.length;

    for (var i = index; i <= count; i++) {
        aux = result[i];
        result[i] = value;
        value = aux;
    }

    return result.join('');
};

String.prototype.remove = function (begin, end) {

    var result = this.split('');
    result.splice(begin, ((end || begin + 1) - begin));
    return result.join('');
};

Array.prototype.contains = function (element) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == element) {
            return true;
        }
    }
    return false;
};

(function ($) {

    $.mask = {

        definitions: {
            'a': "[A-Za-z]",
            '*': "[A-Za-z0-9]",
            '1': "[0-1]",
            '2': "[0-2]",
            '3': "[0-3]",
            '5': "[0-5]",
            '9': "[0-9]",
            'l': "[A-Za-z0-9_]",
            '@': "[A-Za-z0-9_@.-]",
            'd': "[0-9,]"
        }
    };

    $.fn.extend({
        caret: function (begin, end) {
            if (this.length == 0) return;
            if (typeof begin == 'number') {
                end = (typeof end == 'number') ? end : begin;
                return this.each(function () {
                    if (this.setSelectionRange) {
                        this.focus();
                        if ($(this).is(':enabled:visible')) {
                            this.setSelectionRange(begin, end);
                        }
                    } else if (this.createTextRange) {
                        var range = this.createTextRange();
                        range.collapse(true);
                        range.moveEnd('character', end);
                        range.moveStart('character', begin);
                        range.select();
                    }
                });
            } else {
                if (this[0].setSelectionRange) {
                    begin = this[0].selectionStart;
                    end = this[0].selectionEnd;
                } else if (document.selection && document.selection.createRange) {
                    var range = document.selection.createRange();
                    begin = 0 - range.duplicate().moveStart('character', -100000);
                    end = begin + range.text.length;
                }
                return { begin: begin, end: end };
            }
        },

        bindMaskCustom: function (validator, character, pos, autoCompleteValues, ignoreCaret) {
            var text = '';
            var posOld = '';
            var input = '';

            posOld = ((pos && pos.begin) || 0);

            input = $(this);
            pos = (ignoreCaret) ? 0 : $(input).caret();
            text = $(input).val();


            if (character) {
                if (pos.begin != pos.end)
                    text = text.remove(pos.begin, pos.end);
                text = text.insert(pos.begin, character);
            }

            if (!validator.test(text)) {
                if (!character)
                    return false;

                var inc = 0;
                for (var i = 0, count = autoCompleteValues.length; i < count; i++) {
                    if (validator.test(text.insert(pos.begin, autoCompleteValues[i]))) {
                        text = text.insert(pos.begin, autoCompleteValues[i]);
                        inc = autoCompleteValues[i].length
                        break;
                    }
                }

                if (inc == 0)
                    return false;

                pos.begin += inc;
            }

            $(input).val(text);

            if (character) {
                pos.begin++;
            }

            if (!ignoreCaret)
                $(input).caret(pos.begin);

            $(input).attr("lastCorrectValue", text);

            return true;
        },

        bindMask: function (validator, character, pos, autoCompleteValues, ignoreCaret) {

            if (validator.test) {
                return this.bindMaskCustom(validator, character, pos, autoCompleteValues, ignoreCaret);
            }

            var text = '';
            var posOld = '';
            var input = '';

            posOld = ((pos && pos.begin) || 0);

            input = $(this);
            pos = (ignoreCaret) ? 0 : $(input).caret();
            text = $(input).val();

            if (character) {
                text = text.remove(pos.begin, pos.end);
                text = text.insert(pos.begin, character);
            }

            for (var i = (text.length - 1) ; i >= 0; i--) {
                if (validator.contains(text[i])) {
                    text = text.remove(i);
                }
            }

            for (var i = 0; i < validator.length && i < text.length; i++) {

                if (validator[i].test) {
                    if (!validator[i].test(text[i])) {
                        return false;
                    }
                }
                else {
                    text = text.insert(i, validator[i]);
                    if ((pos.begin === i) || (!character && posOld <= i && i <= pos.begin)) {
                        pos.begin++;
                    }
                }
            }

            text = text.remove(validator.length, (text.length));

            $(input).val(text);

            if (character) {
                pos.begin++;
            }

            if (!ignoreCaret)
                $(input).caret(pos.begin);

            $(input).attr("lastCorrectValue", text);
            return true;
        },

        unmask: function () { return this.trigger("unmask"); },

        mask: function (mask, autoCompleteValues) {

            var defs = $.mask.definitions;

            var validator = [];

            if (mask.test) {
                validator = mask;

                if (!autoCompleteValues)
                    autoCompleteValues = [];
            }
            else {
                $(mask.split('')).each(function (i, item) {

                    if (defs[item]) {
                        validator.push(new RegExp(defs[item]));
                    }
                    else {
                        validator.push(item);
                    }

                });
            }
            return this.each(function (i, a) {
                var input = $(this);

                function keypressEvent(event) {

                    var character = String.fromCharCode(event.which);

                    if (event.charCode === 0 || event.ctrlKey) {
                        return true;
                    }

                    $(input).bindMask(validator, character, null, autoCompleteValues);

                    return false;
                };

                function BlurEvent() {
                    if (!($(input).bindMask(validator, null, 0, autoCompleteValues, true)))
                        $(input).val($(input).attr("lastCorrectValue"));
                };

                function PasteEvent() {

                    //var oldValue = $(input).val();
                    var pos = $(input).caret();

                    setTimeout(function () {

                        if (!($(input).bindMask(validator, null, pos, autoCompleteValues))) {
                            //$(input).val(oldValue);
                            $(input).val($(input).attr("lastCorrectValue"));
                            $(input).caret(pos.begin);
                        }

                    }, 0);
                };

                if (!input.attr("readonly")) {
                    input.attr("lastCorrectValue", "");
                    input.bind("keypress.mask", keypressEvent);
                    input.bind("paste.mask", PasteEvent);
                    input.bind("drop.mask", PasteEvent);
                    input.bind("blur.mask", BlurEvent);
                    input.one("unmask", function () {
                        input
						.unbind(".mask")
						.removeData($.mask.dataName);
                    });
                }

            });
        }

    });

})(jQuery);
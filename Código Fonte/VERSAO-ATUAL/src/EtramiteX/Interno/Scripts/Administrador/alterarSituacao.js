
$("#NovaSituacao").change(function () {
	var isAusente = $('.hdnIsAusente').val() == '1';
	var ddlValor = $(this).val();

	$("#divMotivo").toggleClass('hide', !(ddlValor == 4 || (ddlValor == 0 && isAusente)));

	if (ddlValor == 0 && isAusente) {
		$('#Motivo').addClass('disabled').attr('disabled', 'disabled');
	} else {
		$('#Motivo').removeClass('disabled').removeAttr('disabled');
	}

	if (!isAusente) {
		$("#Motivo").val('');
	}
});
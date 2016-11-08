
$("#NovaSituacao").change(function () {
	if ($(this).val() == 4) { //Ausente (id = 4)
		$("#divMotivo").removeClass('hide');
	} else {
		$("#divMotivo").addClass('hide');
		$("#Motivo").val("");
	}
});
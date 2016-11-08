
$("#NovaSituacao").change(function () {
	//Bloqueado (id = 3)
	if ($(this).val() == 3) {

		$("#divMotivo").removeClass('hide');

	} else {
		$("#divMotivo").addClass('hide');
		$("#Motivo").val("");
	}
});
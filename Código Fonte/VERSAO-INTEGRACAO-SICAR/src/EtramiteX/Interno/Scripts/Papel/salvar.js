/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />

Papel = {
	load: function () {
		$('.labelCheckBox').click(Aux.marcarCheck);
		$('.txtNome').focus();
	}
}

$(Papel.load);
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

CondicionanteVisualizar = {
	settings: {
		urls: {
		}
	},
	container: null,

	load: function (container, options) {
		CondicionanteVisualizar.container = container;
		Modal.defaultButtons(CondicionanteVisualizar.container, null, null);
	}
}
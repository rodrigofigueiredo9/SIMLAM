/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.ddl.js" />

CFOEmitir = {
	settings: {
	},
	container: null,

	load: function (container, options) {

		if (options) {
			$.extend(CFOEmitir.settings, options);
		}

		CFOEmitir.container = MasterPage.getContent(container);
		Mascara.load(CFOEmitir.container);
	}
}
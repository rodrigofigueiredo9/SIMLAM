/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.ddl.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />

Lote = {
	settings: {
		urls: {
		}
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(Lote.settings, options);
		}

		Lote.container = MasterPage.getContent(container);
		Mascara.load(Lote.container);
	}
}
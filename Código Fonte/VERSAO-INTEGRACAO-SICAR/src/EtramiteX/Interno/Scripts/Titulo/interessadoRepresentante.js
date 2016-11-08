/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

InteressadoRepresentante = {
	settings: {
		urlObterRepresentantes: ''
	},
	container: null,

	load: function (especificidadeRef, options) {
		InteressadoRepresentante.container = especificidadeRef;
		if (options) { $.extend(InteressadoRepresentante.settings, options); }
	},

	changeProtocolo: function (protocolo) {
		//quando é o change do protocolo
		if (protocolo != null && protocolo.Representantes && protocolo.Representantes.length > 0) {
			if (protocolo.Representantes.length == 1 && !protocolo.Representantes[0].Texto) {
				InteressadoRepresentante.container.addClass('hide');
				$('.hdnInteressadoRepresentanteId', InteressadoRepresentante.container).val(protocolo.Representantes[0].Id);
			} else {
				InteressadoRepresentante.container.removeClass('hide');
				$('.hdnInteressadoRepresentanteId', InteressadoRepresentante.container).val(0);
				$('.ddlRepresentantesInteressado', InteressadoRepresentante.container).ddlLoad(protocolo.Representantes);
			}
		} else {
			InteressadoRepresentante.container.addClass('hide');
			$('.hdnInteressadoRepresentanteId', InteressadoRepresentante.container).val(0);
			$('.ddlRepresentantesInteressado', InteressadoRepresentante.container).ddlClear();
		}
	},

	obter: function (container) {
		var interessadoId = parseInt($('.hdnInteressadoRepresentanteId', InteressadoRepresentante.container).val());

		if (interessadoId > 0) {
			return {
				Id: interessadoId,
				IdRelacionamento: $('.hdnInteressadoRepresentanteIdRelacionamento', InteressadoRepresentante.container).val() 
			};
		} else {
			return {
				Id: $('.ddlRepresentantesInteressado', InteressadoRepresentante.container).val(),
				IdRelacionamento: $('.hdnInteressadoRepresentanteIdRelacionamento', InteressadoRepresentante.container).val() 
			};
		}
	}
}

Titulo.settings.obterRepresentanteFunc = InteressadoRepresentante.obter;
Titulo.addCallbackProtocolo(InteressadoRepresentante.changeProtocolo);
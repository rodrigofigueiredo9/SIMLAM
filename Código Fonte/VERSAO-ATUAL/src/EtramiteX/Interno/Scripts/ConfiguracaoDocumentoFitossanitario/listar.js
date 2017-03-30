/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" 
/// <reference path="../masterpage.js" />

ConfigDocFitossanitarioListar = {
	settings: {
		//urls: {
		//    buscar: '',
		//},
		associarFunc: null
	},

	container: null,

	load: function (container, options) {
	    
	    if (options) {
	        $.extend(ConfigDocFitossanitarioListar.settings, options);
	    }
	    
	    ConfigDocFitossanitarioListar.container = MasterPage.getContent(container);

	    container.delegate('.btnBuscarIntervalos', 'click', ConfigDocFitossanitarioListar.buscarIntervalos);
	    container.delegate('.btnBuscarConsolidado', 'click', ConfigDocFitossanitarioListar.buscarConsolidado);

	    container.listarAjax();

	    if (ConfigDocFitossanitarioListar.settings.associarFunc) {
	        $('.hdnIsAssociar', ConfigDocFitossanitarioListar.container).val(true);
	    }

	    Aux.setarFoco(container);
	},

	buscarIntervalos: function () {
	    $(this).closest('.principal').find('.txtTipoBuscaIntervalos').val('true');
	    $(this).closest('.principal').find('.txtTipoBuscaConsolidado').val('false');

	    $(this).closest('.principal').find('.gridContainerIntervalos').addClass('gridContainer');
	    $(this).closest('.principal').find('.gridContainerConsolidado').removeClass('gridContainer');
	},

	buscarConsolidado: function () {
	    $(this).closest('.principal').find('.txtTipoBuscaConsolidado').val('true');
	    $(this).closest('.principal').find('.txtTipoBuscaIntervalos').val('false');

	    $(this).closest('.principal').find('.gridContainerConsolidado').addClass('gridContainer');
	    $(this).closest('.principal').find('.gridContainerIntervalos').removeClass('gridContainer');
	}

	

}

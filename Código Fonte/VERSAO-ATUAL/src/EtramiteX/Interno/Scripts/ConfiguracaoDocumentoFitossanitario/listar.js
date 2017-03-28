/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" 
/// <reference path="../masterpage.js" />

ConfigDocFitossanitarioListar = {
	settings: {
		urls: {
		    buscar: '',
            validarBusca: '',
		},
		associarFunc: null
	},

	container: null,

	load: function (container, options) {
	    
	    if (options) {
	        $.extend(ConfigDocFitossanitarioListar.settings, options);
	    }
	    
	    ConfigDocFitossanitarioListar.container = MasterPage.getContent(container);
	    container.listarAjax();

	    //container.delegate('.btnBuscarNumero', 'click', ConfigDocFitossanitarioListar.buscarIntervalos);

	    if (ConfigDocFitossanitarioListar.settings.associarFunc) {
	        $('.hdnIsAssociar', ConfigDocFitossanitarioListar.container).val(true);
	    }

	    Aux.setarFoco(container);
	},

	buscarIntervalos: function () {
	    var container = $(this).closest('.filtroExpansivo');
		var ddlTipoDocumento = container.find('.ddlTipoDocumento');
		var ddlTipoDocSelecionado = ddlTipoDocumento.ddlSelecionado();
		var ddlTipoNumeracao = container.find('.ddlTipoNumeracao');
		var ddlTipoNumSelecionada = ddlTipoNumeracao.ddlSelecionado();
		var ano = $('.txtAno', container).val();

		var retorno = MasterPage.validarAjax(ConfigDocFitossanitarioListar.settings.urls.validarBusca, { idTipoDoc: ddlTipoDocSelecionado.Id, idTipoNum: ddlTipoNumSelecionada.Id, anoStr: ano }, ConfigDocFitossanitarioListar.container, false);
		if (!retorno.EhValido) {
			return;
		}

		Mensagem.limpar(ConfigDocFitossanitarioListar.container);
		MasterPage.carregando(true);

		$.ajax({
		    url: ConfigDocFitossanitarioListar.settings.urls.buscar,
		    data: JSON.stringify({
		        idTipoDoc: ddlTipoDocSelecionado.Id,
		        idTipoNum: ddlTipoNumSelecionada.Id,
		        anoStr: ano,
		    }),
		    cache: false,
		    async: false,
		    type: 'POST',
		    dataType: 'json',
		    contentType: 'application/json; charset=utf-8',
		    error: Aux.error,
		    success: function () {
		        alert('ok');
		        if (response.EhValido) {
		            MasterPage.redireciona(response.Url);
		        }

		        if (response.Msg && response.Msg.length > 0) {
		            Mensagem.gerar(ConfigDocFitossanitarioListar.container, response.Msg);
		        }
		    }
		});

		MasterPage.carregando(false);

		//Limpa os controles, mas mantém o tipo de documento selecionado
		//$('.txtNumeroInicial', container).val('');
		//$('.txtNumeroFinal', container).val('');
	},

	

}

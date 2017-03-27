/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" 
/// <reference path="../masterpage.js" />

ConfigDocFitossanitario = {
	settings: {
		urls: {
		    buscar: '',
            validarBusca: '',
		},
		Mensagens: null
	},

	container: null,

	load: function (container, options) {

	    if (options) { $.extend(ConfigDocFitossanitario.settings, options); }
	    ConfigDocFitossanitario.container = MasterPage.getContent(container);

	    container.delegate('.btnBuscarNumero', 'click', ConfigDocFitossanitario.buscarIntervalos);

	    Aux.setarFoco(container);
	},

	buscarIntervalos: function () {
	    var container = $(this).closest('.filtroEspansivo');
		var ddlTipoDocumento = container.find('.ddlTipoDocumento');
		var ddlTipoDocSelecionado = ddlTipoDocumento.ddlSelecionado();
		var ddlTipoNumeracao = container.find('.ddlTipoNumeracao');
		var ddlTipoNumSelecionada = ddlTipoNumeracao.ddlSelecionado();
		var ano = $('.txtAno', container).val();

		var retorno = MasterPage.validarAjax(ConfigDocFitossanitario.settings.urls.validarBusca, { idTipoDoc: ddlTipoDocSelecionado.Id, idTipoNum: ddlTipoNumSelecionada.Id, anoStr: ano}, ConfigDocFitossanitario.container, false);
		if (!retorno.EhValido) {
			return;
		}

		Mensagem.limpar(ConfigDocFitossanitario.container);
		MasterPage.carregando(true);

		$.ajax({
		    url: ConfigDocFitossanitario.settings.urls.buscar,
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
		        //if (response.EhValido) {
		        //    MasterPage.redireciona(response.Url);
		        //}

		        //if (response.Msg && response.Msg.length > 0) {
		        //    Mensagem.gerar(ConfigDocFitossanitario.container, response.Msg);
		        //}
		    }
		});

		MasterPage.carregando(false);

		//Limpa os controles, mas mantém o tipo de documento selecionado
		//$('.txtNumeroInicial', container).val('');
		//$('.txtNumeroFinal', container).val('');
	},

	salvar: function (modal) {
		Modal.fechar(modal);
		Mensagem.limpar(ConfigDocFitossanitario.container);
		MasterPage.carregando(true);

		$.ajax({
			url: ConfigDocFitossanitario.settings.urls.salvar,
			data: JSON.stringify({ configuracao: ConfigDocFitossanitario.obter() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.Url);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ConfigDocFitossanitario.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},

}

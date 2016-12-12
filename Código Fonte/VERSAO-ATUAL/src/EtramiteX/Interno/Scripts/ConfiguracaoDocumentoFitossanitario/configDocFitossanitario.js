/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" 
/// <reference path="../masterpage.js" />

ConfigDocFitossanitario = {
	settings: {
		urls: {
			salvar: '',
			validarIntervalo: ''
		},
		Mensagens: null
	},

	container: null,

	load: function (container, options) {
        
		if (options) { $.extend(ConfigDocFitossanitario.settings, options); }
		ConfigDocFitossanitario.container = MasterPage.getContent(container);

		container.delegate('.btnAdicionarNumero', 'click', ConfigDocFitossanitario.adicionarIntervalo);
		container.delegate('.btnSalvar', 'click', ConfigDocFitossanitario.abrirModalConfirmarSalvar);
		container.delegate('.ddlTipoDocumento', 'change', ConfigDocFitossanitario.toggleMask);

		Aux.setarFoco(container);
	},

	adicionarIntervalo: function () {
		Mensagem.limpar(ConfigDocFitossanitario.container);
		var mensagens = new Array();

		var container = $(this).closest('fieldset');
		var ehBloco = container.find('.dgNumerosBloco').length > 0;
		var ddl = container.find('.ddlTipoDocumento');
		var ddlSelecionado = ddl.ddlSelecionado();

		var item = {
			TipoDocumentoID: ddlSelecionado.Id,
			TipoDocumentoTexto: ddlSelecionado.Texto,
			Tipo: (ehBloco ? 1 : 2),
			NumeroInicial: $('.txtNumeroInicial', container).val(),
			NumeroFinal: $('.txtNumeroFinal', container).val()
		};

		var itens = [];
		$($('.dgNumeros tbody tr:not(.trTemplateRow) .hdnItemJSon', ConfigDocFitossanitario.container)).each(function () {
			itens.push(JSON.parse($(this).val()));
		});

		var retorno = MasterPage.validarAjax(ConfigDocFitossanitario.settings.urls.validarIntervalo, { intervalo: item, intervalos: itens }, ConfigDocFitossanitario.container, false);
		if (!retorno.EhValido) {
			return;
		}

		ConfigDocFitossanitario.atualizarDataGrid(container, item);

		//Limpa os controles
		ddl.ddlFirst();
		$('.txtNumeroInicial', container).val('');
		$('.txtNumeroFinal', container).val('');
	},

	abrirModalConfirmarSalvar: function () {
		var html = '<p>Após salvo os dados não poderão mais ser alterados. Deseja confirmar a ação?</p>';
		var settings = {
			titulo: 'Confirmar',
			onLoadCallbackName: function (content) {
				Modal.defaultButtons(content, function () { ConfigDocFitossanitario.salvar(content) }, 'Sim');
			}
		};
		Modal.abrirHtml(html, settings);
	},

	atualizarDataGrid: function (container, item) {
		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');

		linha.find('.hdnItemJSon').val(JSON.stringify(item));
		linha.find('.TipoDocumentoTexto').html(item.TipoDocumentoTexto).attr('title', item.TipoDocumentoTexto);
		linha.find('.NumeroInicial').html(item.NumeroInicial).attr('title', item.NumeroInicial);
		linha.find('.NumeroFinal').html(item.NumeroFinal).attr('title', item.NumeroFinal);

		$('tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container);
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

	obter: function () {
		var objeto = {
			ID: $('.configuracaoID', ConfigDocFitossanitario.container).val(),
			DocumentoFitossanitarioIntervalos: []
		}

		$($('.dgNumeros tbody tr:not(.trTemplateRow) .hdnItemJSon', ConfigDocFitossanitario.container)).each(function () {
			objeto.DocumentoFitossanitarioIntervalos.push(JSON.parse($(this).val()));
		});

		return objeto;
	},

	toggleMask: function (evt) {
        function toggleClass(element, txt) {
	        switch (txt) {
	            case "CFO":
	            case "CFOC":
	                element.classList.remove("maskNum10");
	                element.classList.add("maskNum8");
	                break;

	            default:
	                element.classList.remove("maskNum8");
	                element.classList.add("maskNum10");
	        }
	    }

        var target = evt.target
        var txt = target.selectedOptions[0].text

	    var isBloco = target.classList.contains("ddlBloco")
	    var isDigital = target.classList.contains("ddlDigital")
	    var complemento = isBloco ? ".txtBloco" : ".txtDigital"

	    var campoInicial = document.querySelector(".txtNumeroInicial" + complemento)
	    var campoFinal = document.querySelector(".txtNumeroFinal" + complemento)

	    toggleClass(campoInicial, txt)
	    toggleClass(campoFinal, txt)

	    $(".maskNum8" + complemento)
            .unmask()
            .mask("99999999")
            .val("");

	    $(".maskNum10" + complemento)
            .unmask()
            .mask("9999999999")
            .val("");
	}
}

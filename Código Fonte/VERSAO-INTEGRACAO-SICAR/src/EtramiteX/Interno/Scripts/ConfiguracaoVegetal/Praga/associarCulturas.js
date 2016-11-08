/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../masterpage.js" />

AssociarCulturas = {
    settings: {
        urls: {
        	salvar: '',
			associar: '',
        },
        Mensagens: null
    },
    container: null,

    load: function (container, options) {

        if (options) {
        	$.extend(AssociarCulturas.settings, options);
        }

        AssociarCulturas.container = MasterPage.getContent(container);
        AssociarCulturas.container = container;
        AssociarCulturas.container.delegate('.btnSalvar', 'click', AssociarCulturas.salvar);
        AssociarCulturas.container.delegate('.btnBuscarCultura', 'click', AssociarCulturas.abrirModalCulturas);
        AssociarCulturas.container.delegate('.btnExcluir', 'click', AssociarCulturas.excluirCultura);

    },
    
    abrirModalCulturas: function () {
    	Modal.abrir(AssociarCulturas.settings.urls.associar, null, function (content) {
    		CulturaListar.load(content, { onAssociarCallback: AssociarCulturas.callBackAssociar});
    		Modal.defaultButtons(content);

    	}, Modal.tamanhoModalMedia)
    },

    callBackAssociar: function (cultura) {
		var validacao = true;

		$('.gridCulturas tbody tr:not(.hide)', AssociarCulturas.container).each(function () {
    		if ($('.hdnItemId', this).val() == cultura.Id) {
    			validacao = false;
    		}
    	});

    	if (!validacao) {
    		Mensagem.gerar(MasterPage.getContent(AssociarCulturas.container), [AssociarCulturas.settings.Mensagens.CulturaJaAdicionada]);
    		return false;
    	}

    	var linha = $('.gridCulturas', AssociarCulturas.container).find('.hide').clone();

    	$('.nome', linha).html(cultura.Nome);
    	$('.hdnItemId', linha).val(cultura.Id);

    	$(linha).removeClass('hide');
    	$('.gridCulturas tbody', AssociarCulturas.container).append(linha);
    	Listar.atualizarEstiloTable($('.gridCulturas ', AssociarCulturas.container));
    	Mensagem.limpar(AssociarCulturas.container);
    	return true;
    },

    salvar: function () {
    	var objeto = AssociarCulturas.obter();

    	if (objeto.Culturas.length < 1) {
    		Mensagem.gerar(MasterPage.getContent(AssociarCulturas.container), [AssociarCulturas.settings.Mensagens.CulturaObrigatorio]);
    		return;
    	}

        Mensagem.limpar(AssociarCulturas.container);
        MasterPage.carregando(true);

        $.ajax({
        	url: AssociarCulturas.settings.urls.salvar,
            data: JSON.stringify(objeto),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: function (XMLHttpRequest, textStatus, erroThrown) {
            	Aux.error(XMLHttpRequest, textStatus, erroThrown, AssociarCulturas.container);
            },
            success: function (response, textStatus, XMLHttpRequest) {
            	if (response.EhValido) {
            		if (response.Url) {
            			MasterPage.redireciona(response.Url);
            		}
            	}

                if (response.Msg && response.Msg.length > 0) {
                	Mensagem.gerar(AssociarCulturas.container, response.Msg);
                }
            }
        });
        MasterPage.carregando(false);
    },
        
    obter: function () {
        var PragaObj = {
            Id: $('.hdnId', AssociarCulturas.container).val(),
            NomeCientifico: '',
            NomeComum: '',

			Culturas: []
        };

        $('.gridCulturas tbody tr:not(.hide)', AssociarCulturas.container).each(function () {
        	var cultura = {
        	    Id: $('.hdnItemId', this).val(),
        	    IdRelacionamento: $('.hdnItemIdRelacionamento', this).val(),
				Nome: $('.nome', this).html()
        	};
        	PragaObj.Culturas.push(cultura);
        });

        return PragaObj;
    },

    excluirCultura: function () {
    	$(this).closest('tr').remove();
    	Listar.atualizarEstiloTable($('.gridCulturas ', AssociarCulturas.container));
    }
}
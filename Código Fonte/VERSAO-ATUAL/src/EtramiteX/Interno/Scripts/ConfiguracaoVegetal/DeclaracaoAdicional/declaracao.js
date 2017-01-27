/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../masterpage.js" />

DeclaracaoAdicional = {
    settings: {
        urls: {
            salvar: ''
        },
        Mensagens: null
    },
    container: null,

    load: function (container, options) {

        if (options) {
            $.extend(DeclaracaoAdicional.settings, options);
        }

        DeclaracaoAdicional.container = MasterPage.getContent(container);
        DeclaracaoAdicional.container = container;
        DeclaracaoAdicional.container.delegate('.btnSalvar', 'click', DeclaracaoAdicional.salvar);
        
    },
    
    salvar: function () {
        
        

        Mensagem.limpar(DeclaracaoAdicional.container);
        MasterPage.carregando(true);

        $.ajax({
            url: DeclaracaoAdicional.settings.urls.salvar,
            data: JSON.stringify(DeclaracaoAdicional.obter()),
            cache: false,
            async: false,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: function (XMLHttpRequest, textStatus, erroThrown) {
                Aux.error(XMLHttpRequest, textStatus, erroThrown, Praga.container);
            },
            success: function (response, textStatus, XMLHttpRequest) {
            	if (response.EhValido) {
            		if (response.Url) {
            			MasterPage.redireciona(response.Url);
            		}
            	}

                if (response.Msg && response.Msg.length > 0) {
                    Mensagem.gerar(DeclaracaoAdicional.container, response.Msg);
                }
            }
        });
        MasterPage.carregando(false);
    },
        
    obter: function () {
        var DeclaracaoAdicionalObj = {
            Id: $('.hdnId', DeclaracaoAdicional.container).val(),
            Texto: $('.txtTexto', DeclaracaoAdicional.container).val(),
            TextoFormatado: $('.txtTextoFormatado', DeclaracaoAdicional.container).val(),
            OutroEstado: $('.rdbOutroEstado:visible:checked', DeclaracaoAdicional.container).val()
        };

        return DeclaracaoAdicionalObj;
    }
}
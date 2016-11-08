/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../masterpage.js" />

Praga = {
    settings: {
        urls: {
            salvar: ''
        },
        Mensagens: null
    },
    container: null,

    load: function (container, options) {

        if (options) {
            $.extend(Praga.settings, options);
        }

        Praga.container = MasterPage.getContent(container);
        Praga.container = container;
        Praga.container.delegate('.btnSalvar', 'click', Praga.salvar);
        
    },
    
    salvar: function () {
        
        if ($('.txtNomeCientifico', Praga.container).val() == '') {
            Mensagem.gerar(MasterPage.getContent(Praga.container), [Praga.settings.Mensagens.NomeCientificoObrigatorio]);
            return;
        }

        Mensagem.limpar(Praga.container);
        MasterPage.carregando(true);

        $.ajax({
            url: Praga.settings.urls.salvar,
            data: JSON.stringify(Praga.obter()),
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
                    Mensagem.gerar(Praga.container, response.Msg);
                }
            }
        });
        MasterPage.carregando(false);
    },
        
    obter: function () {
        var PragaObj = {
            Id: $('.hdnId', Praga.container).val(),
            NomeCientifico: $('.txtNomeCientifico', Praga.container).val(),
            NomeComum: $('.txtNomeComum', Praga.container).val()
        };

        return PragaObj;
    }
}
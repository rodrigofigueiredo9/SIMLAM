/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

LicencaOperacao = {
    container: null,
    settings: {
        urls: {
            obterDadosLicencaOperacao: ''
        },
        modelos: {}
    },

    load: function (especificidadeRef) {
        LicencaOperacao.container = especificidadeRef;
        AtividadeEspecificidade.load(especificidadeRef);
        TituloCondicionante.load($('.condicionantesContainer', LicencaOperacao.container));
    },

    obterLicencaOperacao: function (protocolo) {
        
        if (protocolo == null) {
            $('.ddlDestinatarios', LicencaOperacao.container).ddlClear();
            return;
        }

        $.ajax({ url: LicencaOperacao.settings.urls.obterDadosLicencaOperacao,
            data: JSON.stringify(protocolo),
            cache: false,
            async: true,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: function (XMLHttpRequest, textStatus, erroThrown) {
                Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LicencaOperacao.container));
            },
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.Destinatarios.length > 0) {
                    $('.ddlDestinatarios', LicencaOperacao.container).ddlLoad(response.Destinatarios);
                }
            }
        });
    },
    obterObjeto: function () {
        return {
        	Destinatario: LicencaOperacao.container.find('.ddlDestinatarios').val(),
        	BarragemId: AtividadeEspecificidade.Barragem.gerarObjeto().barragemId
        };
    }
};

Titulo.settings.especificidadeLoadCallback = LicencaOperacao.load;
Titulo.settings.obterEspecificidadeObjetoFunc = LicencaOperacao.obterObjeto;
Titulo.addCallbackProtocolo(LicencaOperacao.obterLicencaOperacao);
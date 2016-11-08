/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

LicencaSimplificada = {
    container: null,
    settings: {
        urls: {
            obterDadosLicencaSimplificada: ''
        },
        modelos: {}
    },

    load: function (especificidadeRef) {
        LicencaSimplificada.container = especificidadeRef;
        AtividadeEspecificidade.load(especificidadeRef);
        TituloCondicionante.load($('.condicionantesContainer', LicencaSimplificada.container));
    },

    obterLicencaSimplificada: function (protocolo) {
        
        if (protocolo == null) {
            $('.ddlDestinatarios', LicencaSimplificada.container).ddlClear();
            return;
        }

        $.ajax({ url: LicencaSimplificada.settings.urls.obterDadosLicencaSimplificada,
            data: JSON.stringify(protocolo),
            cache: false,
            async: true,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: function (XMLHttpRequest, textStatus, erroThrown) {
                Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LicencaSimplificada.container));
            },
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.Destinatarios.length > 0) {
                    $('.ddlDestinatarios', LicencaSimplificada.container).ddlLoad(response.Destinatarios);
                }
            }
        });
    },
    obterObjeto: function () {
        return {
        	Destinatario: LicencaSimplificada.container.find('.ddlDestinatarios').val(),
        	BarragemId: AtividadeEspecificidade.Barragem.gerarObjeto().barragemId
        };
    }
};

Titulo.settings.especificidadeLoadCallback = LicencaSimplificada.load;
Titulo.settings.obterEspecificidadeObjetoFunc = LicencaSimplificada.obterObjeto;
Titulo.addCallbackProtocolo(LicencaSimplificada.obterLicencaSimplificada);
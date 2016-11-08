/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

LicencaAmbientalUnica = {
    container: null,
    settings: {
        urls: {
            obterDadosLicencaAmbientalUnica: ''
        },
        modelos: {}
    },

    load: function (especificidadeRef) {
        LicencaAmbientalUnica.container = especificidadeRef;
        AtividadeEspecificidade.load(especificidadeRef);
        TituloCondicionante.load($('.condicionantesContainer', LicencaAmbientalUnica.container));
    },

    obterLicencaAmbientalUnica: function (protocolo) {
        
        if (protocolo == null) {
            $('.ddlDestinatarios', LicencaAmbientalUnica.container).ddlClear();
            return;
        }

        $.ajax({ url: LicencaAmbientalUnica.settings.urls.obterDadosLicencaAmbientalUnica,
            data: JSON.stringify(protocolo),
            cache: false,
            async: true,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: function (XMLHttpRequest, textStatus, erroThrown) {
                Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LicencaAmbientalUnica.container));
            },
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.Destinatarios.length > 0) {
                    $('.ddlDestinatarios', LicencaAmbientalUnica.container).ddlLoad(response.Destinatarios);
                }
            }
        });
    },
    obterObjeto: function () {
        return {
        	Destinatario: LicencaAmbientalUnica.container.find('.ddlDestinatarios').val(),
        	BarragemId: AtividadeEspecificidade.Barragem.gerarObjeto().barragemId
        };
    }
};

Titulo.settings.especificidadeLoadCallback = LicencaAmbientalUnica.load;
Titulo.settings.obterEspecificidadeObjetoFunc = LicencaAmbientalUnica.obterObjeto;
Titulo.addCallbackProtocolo(LicencaAmbientalUnica.obterLicencaAmbientalUnica);
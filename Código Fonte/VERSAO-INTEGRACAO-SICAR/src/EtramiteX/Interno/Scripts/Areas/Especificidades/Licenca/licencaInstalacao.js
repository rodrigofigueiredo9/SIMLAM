/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

LicencaInstalacao = {
    container: null,
    settings: {
        urls: {
            obterDadosLicencaInstalacao: ''
        },
        modelos: {}
    },

    load: function (especificidadeRef) {
        LicencaInstalacao.container = especificidadeRef;
        AtividadeEspecificidade.load(especificidadeRef);
        TituloCondicionante.load($('.condicionantesContainer', LicencaInstalacao.container));
    },

    obterLicencaInstalacao: function (protocolo) {
        
        if (protocolo == null) {
            $('.ddlDestinatarios', LicencaInstalacao.container).ddlClear();
            return;
        }

        $.ajax({ url: LicencaInstalacao.settings.urls.obterDadosLicencaInstalacao,
            data: JSON.stringify(protocolo),
            cache: false,
            async: true,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: function (XMLHttpRequest, textStatus, erroThrown) {
                Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LicencaInstalacao.container));
            },
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.Destinatarios.length > 0) {
                    $('.ddlDestinatarios', LicencaInstalacao.container).ddlLoad(response.Destinatarios);
                }
            }
        });
    },
    obterObjeto: function () {
        return {
        	Destinatario: LicencaInstalacao.container.find('.ddlDestinatarios').val(),
        	BarragemId: AtividadeEspecificidade.Barragem.gerarObjeto().barragemId
        };
    }
};

Titulo.settings.especificidadeLoadCallback = LicencaInstalacao.load;
Titulo.settings.obterEspecificidadeObjetoFunc = LicencaInstalacao.obterObjeto;
Titulo.addCallbackProtocolo(LicencaInstalacao.obterLicencaInstalacao);
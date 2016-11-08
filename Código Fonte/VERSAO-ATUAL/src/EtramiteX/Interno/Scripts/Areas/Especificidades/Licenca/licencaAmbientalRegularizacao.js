/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

LicencaAmbientalRegularizacao = {
    container: null,
    settings: {
        urls: {
            obterDadosLicencaAmbientalRegularizacao: ''
        },
        modelos: {}
    },

    load: function (especificidadeRef) {
        LicencaAmbientalRegularizacao.container = especificidadeRef;
        AtividadeEspecificidade.load(especificidadeRef);
        TituloCondicionante.load($('.condicionantesContainer', LicencaAmbientalRegularizacao.container));
    },

    obterLicencaAmbientalRegularizacao: function (protocolo) {
        
        if (protocolo == null) {
            $('.ddlDestinatarios', LicencaAmbientalRegularizacao.container).ddlClear();
            return;
        }

        $.ajax({ url: LicencaAmbientalRegularizacao.settings.urls.obterDadosLicencaAmbientalRegularizacao,
            data: JSON.stringify(protocolo),
            cache: false,
            async: true,
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: function (XMLHttpRequest, textStatus, erroThrown) {
                Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LicencaAmbientalRegularizacao.container));
            },
            success: function (response, textStatus, XMLHttpRequest) {
                if (response.Destinatarios.length > 0) {
                    $('.ddlDestinatarios', LicencaAmbientalRegularizacao.container).ddlLoad(response.Destinatarios);
                }
            }
        });
    },
    obterObjeto: function () {
        return {
        	Destinatario: LicencaAmbientalRegularizacao.container.find('.ddlDestinatarios').val(),
        	BarragemId: AtividadeEspecificidade.Barragem.gerarObjeto().barragemId
        };
    }
};

Titulo.settings.especificidadeLoadCallback = LicencaAmbientalRegularizacao.load;
Titulo.settings.obterEspecificidadeObjetoFunc = LicencaAmbientalRegularizacao.obterObjeto;
Titulo.addCallbackProtocolo(LicencaAmbientalRegularizacao.obterLicencaAmbientalRegularizacao);
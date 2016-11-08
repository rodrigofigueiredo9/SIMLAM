/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

CertidaoCartaAnuencia = {
	container: null,
	urlObterDadosCertidaoCartaAnuencia: '',

	load: function (especificidadeRef) {
		CertidaoCartaAnuencia.container = especificidadeRef;
		AtividadeEspecificidade.load(CertidaoCartaAnuencia.container);
		DestinatarioEspecificidade.load(CertidaoCartaAnuencia.container);
	},

	obterDadosCertidaoCartaAnuencia: function (protocolo) {
		if (protocolo == null) {
			DestinatarioEspecificidade.clear();
			$('.ddlDominios', CertidaoCartaAnuencia.container).ddlClear();
			return;
		}

		$.ajax({
			url: CertidaoCartaAnuencia.urlObterDadosCertidaoCartaAnuencia,
			data: JSON.stringify({ id: protocolo.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CertidaoCartaAnuencia.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Dominios) {
					$('.ddlDominios', CertidaoCartaAnuencia.container).ddlLoad(response.Dominios);
				}
			}
		});
	},

	obterObjeto: function () {
		return {
			Destinatarios: DestinatarioEspecificidade.obter(),
			Dominio: CertidaoCartaAnuencia.container.find('.ddlDominios').val(),
			Descricao: $('.txtDescricao', CertidaoCartaAnuencia.container).val()
		};
	}
};

Titulo.settings.especificidadeLoadCallback = CertidaoCartaAnuencia.load;
Titulo.addCallbackProtocolo(CertidaoCartaAnuencia.obterDadosCertidaoCartaAnuencia);
Titulo.settings.obterEspecificidadeObjetoFunc = CertidaoCartaAnuencia.obterObjeto;
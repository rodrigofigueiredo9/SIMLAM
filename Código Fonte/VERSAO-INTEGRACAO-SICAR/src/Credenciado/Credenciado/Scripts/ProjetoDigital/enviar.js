/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

ProjetoDigitalEnviar = {
	settings: {
		projetoDigitalId: 0,
		urls: {
			enviar: null
		}
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(ProjetoDigitalEnviar.settings, options);
		}

		ProjetoDigitalEnviar.container = MasterPage.getContent(container);
		ProjetoDigitalEnviar.container.delegate('.btnConcluir', 'click', ProjetoDigitalEnviar.enviar);
		ProjetoDigitalEnviar.configurarAssociarMultiploResponsavel();
		$('.conteudoResponsaveis', ProjetoDigitalEnviar.container).closest('.boxBranca').removeClass('boxBranca').addClass('box');
	},
	
	configurarAssociarMultiploResponsavel: function () {
		$('.divConteudoResponsavelTec').associarMultiplo({
			'expandirAutomatico': true,
			'minItens': 0
		});
	},

	enviar: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: ProjetoDigitalEnviar.settings.urls.enviar, data: JSON.stringify({ id: ProjetoDigitalEnviar.settings.projetoDigitalId }),
			cache: false, async: false, type: 'POST', typeData: 'json', contentType: 'application/json; charset=utf-8', 
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ProjetoDigitalEnviar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ProjetoDigitalEnviar.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
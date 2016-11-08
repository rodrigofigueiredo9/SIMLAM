/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../masterpage.js" />

ProjetoDigital = {
	settings: {
		projetoDigitalId: 0,
		projetoDigitalEtapa: 0,
		projetoDigitalSituacao: 0,
		situacoesEditaveis: [],
		modoVisualizar: false,
		desativarPasso4: false,
		urls: {
			requerimento: null,
			caracterizacao: null,
			enviar: null,
			imprimirDocumentos: null,
			editarCaracterizacaoValidar: null,
			alterarDados: null
		}
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(ProjetoDigital.settings, options);
		}

		ProjetoDigital.container = MasterPage.getContent(container);

		if (!ProjetoDigital.settings.modoVisualizar && $.inArray(ProjetoDigital.settings.projetoDigitalSituacao, ProjetoDigital.settings.situacoesEditaveis) >= 0) {
			$('.linkProjDigital1', ProjetoDigital.container).click(function () { MasterPage.redireciona(ProjetoDigital.settings.urls.requerimento); });

			if (ProjetoDigital.settings.projetoDigitalEtapa > 1) {
				$('.linkProjDigital2', ProjetoDigital.container).click(ProjetoDigital.editarCaracterizacaoValidar);
			}

			if (ProjetoDigital.settings.projetoDigitalEtapa > 2) {
				$('.linkProjDigital3', ProjetoDigital.container).click(function () { MasterPage.redireciona(ProjetoDigital.settings.urls.enviar); });
			}
		}

		if (!ProjetoDigital.settings.desativarPasso4 && ProjetoDigital.settings.projetoDigitalEtapa == 4) {
			$('.linkProjDigital4', ProjetoDigital.container).click(function () { MasterPage.redireciona(ProjetoDigital.settings.urls.imprimirDocumentos); });
		}
	},

	editarCaracterizacaoValidar: function () {
		Modal.confirma({
			btnOkLabel: 'Confirmar',
			url: ProjetoDigital.settings.urls.editarCaracterizacaoValidar + '/' + ProjetoDigital.settings.projetoDigitalId,
			forcarLoadCallback: true,
			onLoadCallbackName: function (modalContent) { ProjetoDigital.redirecionar(modalContent, ProjetoDigital.settings.urls.caracterizacao); },
			btnOkCallback: function (modalContent) { ProjetoDigital.editarDadosCallBack(modalContent, ProjetoDigital.settings.urls.caracterizacao); }
		});
	},

	editarDadosCallBack: function (modalContent, url) {
		$.ajax({
			url: ProjetoDigital.settings.urls.alterarDados, type: "POST", cache: false, async: false,
			data: { id: ProjetoDigital.settings.projetoDigitalId },
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				Aux.error(XMLHttpRequest, textStatus, errorThrown, MasterPage.getContent(modalContent));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(url);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(modalContent), response.Msg);
				}
			}
		});
	},

	redirecionar: function (modalContent, url) {
		if (!$('.titTela', modalContent).is('*')) {
			Modal.fechar(modalContent);
			MasterPage.redireciona(url);
		}
	}
}
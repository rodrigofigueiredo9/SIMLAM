/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

Notificacao = {
	settings: {
		urls: {
			salvar: '',
			editar: '',
			cobranca: ''
		},
		salvarCallBack: null,
		mensagens: {},
		idsTela: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Notificacao.settings, options); }
		Notificacao.container = MasterPage.getContent(container);
		Notificacao.container.delegate('.btnSalvar', 'click', Notificacao.salvar);
		Notificacao.container.delegate('.btnEditar', 'click', Notificacao.editar);
		Notificacao.container.delegate('.btnCadastrarCobranca', 'click', Notificacao.cobranca);
		Notificacao.container.delegate('.btnVisualizarCobranca', 'click', Notificacao.cobrancaVisualizar);
		Notificacao.container.delegate('.rdbFormaIUF', 'change', Notificacao.alterarVisibilidadeData);
		Notificacao.container.delegate('.rdbFormaJIAPI', 'change', Notificacao.alterarVisibilidadeData);
		Notificacao.container.delegate('.rdbFormaCORE', 'change', Notificacao.alterarVisibilidadeData);
		$('.fsArquivos', Notificacao.container).arquivo({ extPermitidas: ['pdf'] });
		Mascara.load();
		Notificacao.alterarVisibilidadeData();
	},

	obter: function () {
		var container = Notificacao.container;

		var obj = {
			Id: $('.hdnNotificacaoId', container).val(),
			FiscalizacaoId: $('.hdnFiscalizacaoId', container).val(),
			FormaIUF: $('.rdbFormaIUF:checked', container).val(),
			FormaJIAPI: $('.rdbFormaJIAPI:checked', container).val(),
			FormaCORE: $('.rdbFormaCORE:checked', container).val(),
			DataIUF: { DataTexto: $('.txtDataIUF', container).val() },
			DataJIAPI: { DataTexto: $('.txtDataJIAPI', container).val() },
			DataCORE: { DataTexto: $('.txtDataCORE', container).val() },
			Anexos: $('.fsArquivos', Notificacao.container).arquivo('obterObjeto')
		};

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: Notificacao.settings.urls.salvar,
			data: JSON.stringify(Notificacao.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Notificacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Notificacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	editar: function () {
		var obj = Notificacao.obter();		
		MasterPage.redireciona(Notificacao.settings.urls.editar + "/" + obj.FiscalizacaoId);
	},

	alterarVisibilidadeData: function () {
		var obj = Notificacao.obter();		

		if (obj.FormaIUF > 0) {
			$('.rdbFormaJIAPI').css('visibility', '');
			$('.lblFormaJIAPI').css('visibility', '');
		}
		else {
			$('.rdbFormaJIAPI').css('visibility', 'hidden');
			$('.lblFormaJIAPI').css('visibility', 'hidden');
		}

		if (obj.FormaJIAPI > 0) {
			$('.jiapi').css('visibility', '');
			$('.rdbFormaCORE').css('visibility', '');
			$('.lblFormaCORE').css('visibility', '');
		}
		else {
			$('.jiapi').css('visibility', 'hidden');
			$('.rdbFormaCORE').css('visibility', 'hidden');
			$('.lblFormaCORE').css('visibility', 'hidden');
		}

		if (obj.FormaCORE  > 0) 
			$('.core').css('visibility', '');
		else
			$('.core').css('visibility', 'hidden');
	},

	cobranca: function () {
		var obj = Notificacao.obter();
		MasterPage.redireciona(Notificacao.settings.urls.cobranca + "/" + obj.FiscalizacaoId);
	},

	cobrancaVisualizar: function () {
		var obj = Notificacao.obter();
		MasterPage.redireciona(Notificacao.settings.urls.cobrancaVisualizar + "/" + obj.FiscalizacaoId);
	}
}
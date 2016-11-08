Profissao = {
	container: null,
	settings: {		
		urls: {			
			salvar: null
		}
	},

	load: function (container, options) {
		if (options) {
			$.extend(Profissao.settings, options);
		}

		container = MasterPage.getContent(container);
		Profissao.container = container;

		container.delegate('.btnProfissaoSalvar', 'click', Profissao.salvar);
	},

	obter: function () {
		var objeto = {
			Id: $('.hdnArtefatoId', Profissao.container).val(),
			Texto: $('.txtTexto', Profissao.container).val()			
		};

		return objeto;
	},

	salvar: function(){		
		var params = { profissao: Profissao.obter() };
		MasterPage.carregando(true);

		$.ajax({
			url: Profissao.settings.urls.salvar, data: JSON.stringify(params), type: 'POST', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Profissao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				} else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Profissao.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	}

}
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

EPTVAnalisar = {
	settings: {
		urls: {
			salvar: null
		},
		idsTela: null
	},
	container: null,

	load: function (container, options) {
		EPTVAnalisar.container = container;
		if (options) {
			$.extend(EPTVAnalisar.settings, options);
		}

		container.delegate('.rdbOpcaoSituacao', 'change', EPTVAnalisar.situacaoChange);
		container.delegate('.btnSalvar', 'click', EPTVAnalisar.salvar);
	},

	limparCampos: function (container) {
		var container = $('.divCamposSituacao', EPTVAnalisar.container);
		$('.txtSituacaoMotivo, .txtDataValidade', container).val('');
		$('.ddlLocalEmissao', container).ddlFirst();
		Mascara.load(container);
	},

	situacaoChange: function () {
		EPTVAnalisar.limparCampos();

		switch (parseInt($('.rdbOpcaoSituacao:checked', EPTVAnalisar.container).val())) {
			case EPTVAnalisar.settings.idsTela.Aprovado:
				container = $('.divMotivo', EPTVAnalisar.container).addClass('hide');
				container = $('.divAprovar', EPTVAnalisar.container).removeClass('hide');
				$('.txtDataValidade', container).focus();
				break;

			case EPTVAnalisar.settings.idsTela.AgendarFiscalizacao:
				//container = $('.divAprovar', EPTVAnalisar.container).addClass('hide');
				//container = $('.divMotivo', EPTVAnalisar.container).addClass('hide');
				//break;

			case EPTVAnalisar.settings.idsTela.Rejeitado:
			case EPTVAnalisar.settings.idsTela.Bloqueado:
				container = $('.divAprovar', EPTVAnalisar.container).addClass('hide');
				container = $('.divMotivo', EPTVAnalisar.container).removeClass('hide');
				$('.txtSituacaoMotivo', container).focus();
				break;
		}
	},

	salvar: function () {
		var objeto = EPTVAnalisar.obter();
		
		MasterPage.carregando(true);

		$.ajax({
			url: EPTVAnalisar.settings.urls.salvar,
			data: JSON.stringify({ eptv: objeto }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				} else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(EPTVAnalisar.container), response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},
	obter: function(){
		var objeto = {
			Id: +$('.hdnEmissaoId', EPTVAnalisar.container).val(),
			Situacao: +$('.rdbOpcaoSituacao:checked', EPTVAnalisar.container).val(),
			SituacaoMotivo: $('.txtSituacaoMotivo', EPTVAnalisar.container).val(),
			LocalEmissaoId: +$('.ddlLocalEmissao', EPTVAnalisar.container).val(),
			ResponsavelTecnicoId: $('.hdnResponsavelTecnicoId', EPTVAnalisar.container).val()
		};

		if (objeto.Situacao == 3/*Aprovado*/) {
			objeto.ValidoAte = { DataTexto: $('.txtDataValidade', EPTVAnalisar.container).val() };
		}

		return objeto;
	}
}
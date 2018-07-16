/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

EPTVAnalisar = {
	settings: {
		urls: {
			salvar: null
		},
		idsTela: null,
		Mensagens: null
	},
	container: null,

	load: function (container, options) {
		EPTVAnalisar.container = container;
		if (options) {
			$.extend(EPTVAnalisar.settings, options);
		}

		container.delegate('.rdbOpcaoSituacao', 'change', EPTVAnalisar.situacaoChange);
		container.delegate('.btnSalvar', 'click', EPTVAnalisar.salvar);
		container.delegate('.rbPartidaLacradaOrigem', 'change', EPTVAnalisar.onChangePartidaLacrada);
	},

	limparCampos: function (container) {
		var container = $('.divCamposSituacao', EPTVAnalisar.container);
		$('.txtSituacaoMotivo, .txtDataValidade', container).val('');
		$('.ddlLocalEmissao', container).ddlFirst();
		Mascara.load(container);
	},

	situacaoChange: function () {
		EPTVAnalisar.limparCampos();

		container = $('.divAgendarFiscalizacao', EPTVAnalisar.container).addClass('hide');
		switch (parseInt($('.rdbOpcaoSituacao:checked', EPTVAnalisar.container).val())) {
			case EPTVAnalisar.settings.idsTela.Aprovado:
				container = $('.divMotivo', EPTVAnalisar.container).addClass('hide');
				container = $('.divAprovar', EPTVAnalisar.container).removeClass('hide');
				$('.txtDataValidade', container).focus();
				break;

			case EPTVAnalisar.settings.idsTela.AgendarFiscalizacao:
				container = $('.divAprovar', EPTVAnalisar.container).addClass('hide');
				container = $('.divMotivo', EPTVAnalisar.container).addClass('hide');
				container = $('.divAgendarFiscalizacao', EPTVAnalisar.container).removeClass('hide');
				break;

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

		if (!EPTVAnalisar.validarHora(objeto.HoraFiscalizacao)) {
			Mensagem.gerar(MasterPage.getContent(EPTVAnalisar.container), [EPTVAnalisar.settings.Mensagens.HoraInvalida]);
			return;
		}

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

	validarHora: function (hora) {
		if (hora.length != 5) {
			return false;
		}

		hrs = hora.substring(0, 2);
		min = hora.substring(3, 5);

		if ((hrs == "") || (min == "")) {
			return false;
		}
		if ((hrs < 00) || (hrs > 23) || (min < 00) || (min > 59)) {
			return false;
		}
		return true;
	},

	onChangePartidaLacrada: function () {
		if ($(this).val() == 1) {
			$('.partida_lacrada', EPTVAnalisar.container).removeClass('hide');
			$('.txtNumeroLacre', EPTVAnalisar.container).focus();
		}
		else {
			$('.partida_lacrada', EPTVAnalisar.container).addClass('hide');
			$('.txtNumeroLacre, .txtNumeroPorao, .txtNumeroContainer', EPTVAnalisar.container).val('');
		}
	},

	obter: function(){
		var objeto = {
			Id: +$('.hdnEmissaoId', EPTVAnalisar.container).val(),
			Situacao: +$('.rdbOpcaoSituacao:checked', EPTVAnalisar.container).val(),
			SituacaoMotivo: $('.txtSituacaoMotivo', EPTVAnalisar.container).val(),
			LocalEmissaoId: +$('.ddlLocalEmissao', EPTVAnalisar.container).val(),
			ResponsavelTecnicoId: $('.hdnResponsavelTecnicoId', EPTVAnalisar.container).val(),
			LocalFiscalizacao: $('.txtLocalFiscalizacao', EPTVAnalisar.container).val(),
			HoraFiscalizacao: $('.txtHoraFiscalizacao', EPTVAnalisar.container).val(),
			InformacoesAdicionais: $('.txtInformacoesAdicionais', EPTVAnalisar.container).val(),
			PartidaLacradaOrigem: $('.rbPartidaLacradaOrigem:checked', EPTVAnalisar.container).val(),
			LacreNumero: $('.txtNumeroLacre', EPTVAnalisar.container).val(),
			PoraoNumero: $('.txtNumeroPorao', EPTVAnalisar.container).val(),
			ContainerNumero: $('.txtNumeroContainer', EPTVAnalisar.container).val(),
		};

		if (objeto.Situacao == 3/*Aprovado*/) {
			objeto.ValidoAte = { DataTexto: $('.txtDataValidade', EPTVAnalisar.container).val() };
		}

		return objeto;
	}
}
/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="../../mensagem.js" />

TituloAdicionar = {
	settings: {
		urls: {
			buscarTitulo: null
		}
	},

	load: function (container) {
		container.delegate('.rbOrgaoEmissor', 'click', TituloAdicionar.changeOrgaoEmissor);
		container.delegate('.btnBuscarNumero', 'click', TituloAdicionar.buscarTitulo);
	},

	limparCampos: function (ocultarCampos, container, manterOrgao) {
		$('.txtTituloValidadeData', container.find('.divOrgao')).attr('disabled', 'disabled');
		$('.txtProtocoloNumero', container.find('.divOrgao')).attr('disabled', 'disabled');

		if (!manterOrgao) {
			$('.rbOrgaoEmissor', container).removeAttr('checked');
		}
		$('select', container).ddlFirst();
		$('input[type=text]', container).unmask().val('');
		$('input[type=hidden]', container).unmask().val('0');

		if (ocultarCampos) {
			$('.divOrgao, .divOrgaoExterno', container).addClass('hide');
		}
	},

	changeOrgaoEmissor: function () {
		var container = $(this).closest('.divTituloAdicionar');
		var emissor = $('.rbOrgaoEmissor:checked', container).val();

		$('.divOrgao', container).toggleClass('hide', (emissor != $('.rbEmitidoIDAF', container).val()));
		$('.divOrgaoExterno', container).toggleClass('hide', (emissor != $('.rbEmitidoOutroOrgao', container).val()));

		TituloAdicionar.limparCampos(false, container, true);
		Mascara.load(container);
	},

	obter: function (container) {
		var emissor = $('.rbOrgaoEmissor:checked', container).val();

		var obj = {
			Id: $('.hdnFinalidadeID', container).val(),
			EmitidoPorInterno: emissor,
			TituloModelo: null,
			TituloModeloTexto: null,
			TituloId: $('.hdnTituloID', container).val(),
			TituloNumero: null,
			TituloValidadeData: null,
			ProtocoloNumero: null,
			ProtocoloRenovacaoData: null,
			ProtocoloRenovacaoNumero: null,
			OrgaoExpedidor: null
		};

		if (emissor == $('.rbEmitidoIDAF', container).val()) {
			container = container.find('.divOrgao');
			obj.EmitidoPorInterno = true;
			obj.TituloModelo = $('.ddlTituloModelo', container).val();
			obj.TituloModeloTexto = $('.ddlTituloModelo :selected', container).text();
		} else if (emissor == $('.rbEmitidoOutroOrgao', container).val()) {
			container = container.find('.divOrgaoExterno');
			obj.EmitidoPorInterno = false;
			obj.TituloModeloTexto = $('.txtTituloModelo', container).val();
		}

		obj.TituloNumero = $('.txtTituloNumero', container).val(),
		obj.TituloValidadeData = { DataTexto: $('.txtTituloValidadeData', container).val() },
		obj.ProtocoloNumero = $('.txtProtocoloNumero', container).val(),
		obj.ProtocoloRenovacaoData = { DataTexto: $('.txtProtocoloRenovacaoData', container).val() },
		obj.ProtocoloRenovacaoNumero = $('.txtProtocoloRenovacaoNumero', container).val(),
		obj.OrgaoExpedidor = $('.txtOrgaoExpedidor', container).val()

		return obj;
	},

	buscarTitulo: function () {
		var containerCentral = MasterPage.getContent(this);
		Mensagem.limpar(containerCentral);

		var container = $(this).closest('.divTituloAdicionar');
		var modelo = $('.ddlTituloModelo', container).val();
		var modeloTexto = $('.ddlTituloModelo :selected', container).text();
		var numero = $('.txtTituloNumero', container).val();

		var index = $('.ddlTituloModelo', container).attr('id').replace('TituloModelo', '');

		MasterPage.carregando(true);
		$.ajax({
			url: TituloAdicionar.settings.urls.buscarTitulo,
			data: JSON.stringify({ Modelo: modelo, ModeloTexto: modeloTexto, Numero: numero, indice: index}),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					if (response.Titulo == null || response.Titulo.Id == 0) {
						$('.txtTituloValidadeData', container).removeClass('disabled').removeAttr('disabled');
						$('.txtProtocoloNumero', container).removeClass('disabled').removeAttr('disabled');
					} else {
						$('.hdnTituloID', container).val(response.Titulo.Id);
						$('.txtTituloValidadeData', container).val(response.Titulo.DataVencimento.DataTexto);
						$('.txtProtocoloNumero', container).val(response.Titulo.Protocolo.Numero);

						$('.txtTituloValidadeData', container.find('.divOrgao')).attr('disabled', 'disabled');
						$('.txtProtocoloNumero', container.find('.divOrgao')).attr('disabled', 'disabled');
					}
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(containerCentral, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
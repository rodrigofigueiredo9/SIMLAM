/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />
/// <reference path="../../../Titulo/titulo.js" />

OutrosInformacaoCorte = {
	urlObterDadosRequerimento: '',
	container: null,

	load: function (especificidadeRef) {
		OutrosInformacaoCorte.container = especificidadeRef;
		OutrosInformacaoCorte.container.delegate('.ddlVinculo', 'change', OutrosInformacaoCorte.onChange_ddlVinculo);
	},
	
	
	onChange_ddlVinculo: function () {
		$('.txtVinculoPropOutro', OutrosInformacaoCorte.container).val('');

		if ($(this).val() === '3') {
			$('.divOutros', OutrosInformacaoCorte.container).removeClass('hide');
		}
		else {
			$('.divOutros', OutrosInformacaoCorte.container).addClass('hide');
		}
	},

	obterDadosRequerimento: function (requerimento) {
		Mensagem.limpar(OutrosInformacaoCorte.container);
		var ddlAtividades = $('.ddlAtividades', OutrosInformacaoCorte.container);
		var ddlInformacaoCorte = $('.ddlInformacaoCorte', OutrosInformacaoCorte.container);
		var interessado = $('.txtInteressado', OutrosInformacaoCorte.container);

		if (!requerimento) {
			ddlAtividades.ddlClear();
			ddlInformacaoCorte.ddlClear();
			$('.txtInteressado', OutrosInformacaoCorte.container).val('');
			$('.txtValidadeInfCorte', OutrosInformacaoCorte.container).val('');
			return;
		}

		MasterPage.carregando(true);
		$.ajax({
			url: OutrosInformacaoCorte.urlObterDadosRequerimento,
			data: JSON.stringify({ requerimentoId: requerimento.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Atividades) {
					ddlAtividades.ddlLoad(response.Atividades).addClass('disabled').attr('disabled', 'disabled');
				}
				if (response.Interessado) {
					interessado.val(response.Interessado.Fisica.Nome || response.Interessado.Juridica.RazaoSocial);
					interessado.addClass('disabled').attr('disabled', 'disabled');
				}
				if (response.InformacoesDeCorte) {
					ddlInformacaoCorte.ddlLoad(response.InformacoesDeCorte);
				}
				
			}
		});
		MasterPage.carregando(false);
	},

	obterAtividades: function () {
		return [{ Id: parseInt($('.ddlAtividades', OutrosInformacaoCorte.container).val()) }];
	},

	obterObjeto: function () {
		var obj = {
			Atividade: $('.ddlAtividades', OutrosInformacaoCorte.container).val(),
			InformacaoCorte: $('.ddlInformacaoCorte', OutrosInformacaoCorte.container).val(),
			Interessado: $('.txtInteressado', OutrosInformacaoCorte.container).val(),
			Validade: $('.txtValidadeInfCorte', OutrosInformacaoCorte.container).val()
		};

		return obj;
	}
};

TituloDeclaratorio.settings.obterAtividadesFunc = OutrosInformacaoCorte.obterAtividades;
TituloDeclaratorio.settings.especificidadeLoadCallback = OutrosInformacaoCorte.load;
TituloDeclaratorio.addCallbackRequerimento(OutrosInformacaoCorte.obterDadosRequerimento);
TituloDeclaratorio.settings.obterEspecificidadeObjetoFunc = OutrosInformacaoCorte.obterObjeto;
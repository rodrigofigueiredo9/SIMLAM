/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />
/// <reference path="../../../Titulo/titulo.js" />

CertidaoDispensaLicenciamentoAmbiental = {
	urlObterDadosRequerimento: '',
	container: null,

	load: function (especificidadeRef) {
		CertidaoDispensaLicenciamentoAmbiental.container = especificidadeRef;
		CertidaoDispensaLicenciamentoAmbiental.container.delegate('.ddlVinculo', 'change', CertidaoDispensaLicenciamentoAmbiental.onChange_ddlVinculo);
	},

	onChange_ddlVinculo: function () {
		$('.txtVinculoPropOutro', CertidaoDispensaLicenciamentoAmbiental.container).val('');

		if ($(this).val() == '3') {
			$('.divOutros', CertidaoDispensaLicenciamentoAmbiental.container).removeClass('hide');
		}
		else {
			$('.divOutros', CertidaoDispensaLicenciamentoAmbiental.container).addClass('hide');
		}
	},

	obterDadosRequerimento: function (requerimento) {
		Mensagem.limpar(CertidaoDispensaLicenciamentoAmbiental.container);
		var ddlAtividades = $('.ddlAtividades', CertidaoDispensaLicenciamentoAmbiental.container);

		if (!requerimento) {
			ddlAtividades.ddlClear();
			$('.txtInteressado', CertidaoDispensaLicenciamentoAmbiental.container).val('');
			return;
		}

		MasterPage.carregando(true);
		$.ajax({
			url: CertidaoDispensaLicenciamentoAmbiental.urlObterDadosRequerimento,
			data: JSON.stringify({ requerimentoId: requerimento.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Atividades) {
					ddlAtividades.ddlLoad(response.Atividades).removeClass('disabled').removeAttr('disabled');
				}
				if (response.Interessado) {
					$('.txtInteressado', CertidaoDispensaLicenciamentoAmbiental.container).val(response.Interessado.Fisica.Nome || response.Interessado.Juridica.RazaoSocial);
				}
			}
		});
		MasterPage.carregando(false);
	},

	obterAtividades: function () {
		return [{ Id: parseInt($('.ddlAtividades', CertidaoDispensaLicenciamentoAmbiental.container).val()) }];
	},

	obterObjeto: function () {
		var obj = {
			Atividade: $('.ddlAtividades', CertidaoDispensaLicenciamentoAmbiental.container).val(),
			VinculoPropriedade: $('.ddlVinculo', CertidaoDispensaLicenciamentoAmbiental.container).val(),
			VinculoPropriedadeOutro: $('.txtVinculoPropOutro', CertidaoDispensaLicenciamentoAmbiental.container).val()
		};

		return obj;
	}
};

TituloDeclaratorio.settings.obterAtividadesFunc = CertidaoDispensaLicenciamentoAmbiental.obterAtividades;
TituloDeclaratorio.settings.especificidadeLoadCallback = CertidaoDispensaLicenciamentoAmbiental.load;
TituloDeclaratorio.addCallbackRequerimento(CertidaoDispensaLicenciamentoAmbiental.obterDadosRequerimento);
TituloDeclaratorio.settings.obterEspecificidadeObjetoFunc = CertidaoDispensaLicenciamentoAmbiental.obterObjeto;
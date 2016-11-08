/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../mensagem.js" />

TermoAprovacaoMedicao = {
	container: null,

	settings: {
		urls: {
			urlObterDadosTermoAprovacaoMedicao: ''
		},
		Mensagens: null
	},

	load: function (especificidadeRef) {
		TermoAprovacaoMedicao.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
		TermoAprovacaoMedicao.container.delegate('input[type="radio"]', 'click', TermoAprovacaoMedicao.onClickRadio);
	},

	obterDadosTermoAprovacaoMedicao: function (protocolo) {
		if (protocolo == null) {
			return;
		}

		$.ajax({ url: TermoAprovacaoMedicao.settings.urls.urlObterDadosTermoAprovacaoMedicao,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Titulo.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.ddlDestinatarios', TermoAprovacaoMedicao.container).ddlLoad(response.Destinatarios);
					$('.ddlFuncionario', TermoAprovacaoMedicao.container).ddlLoad(response.Funcionario);
					$('.ddlTecnicos', TermoAprovacaoMedicao.container).ddlLoad(response.Tecnicos);
					$('.ddlSetoresUsuario', TermoAprovacaoMedicao.container).ddlLoad(response.Setores);
					$('.txtDataMedicao', TermoAprovacaoMedicao.container).val(response.Termo.DataMedicao.DataTexto);

					if (response.Destinatarios.length == 1) {
						$('.ddlDestinatarios', TermoAprovacaoMedicao.container).val(response.Destinatarios[0].Id).attr('disabled', 'disabled');
					}
					if (response.Funcionario.length == 1) {
						$('.ddlFuncionario', TermoAprovacaoMedicao.container).val(response.Funcionario[0].Id).attr('disabled', 'disabled');
					}
					if (response.Tecnicos.length == 1) {
						$('.ddlTecnicos', TermoAprovacaoMedicao.container).val(response.Tecnicos[0].Id).attr('disabled', 'disabled');
					}
					if (response.Setores.length == 1) {
						$('.ddlSetoresUsuario', TermoAprovacaoMedicao.container).val(response.Setores[0].Id).attr('disabled', 'disabled');
					}

					if (response.Termo.Id > 0) {
						$('.ddlDestinatarios', TermoAprovacaoMedicao.container).val(response.Termo.Destinatario);
						$('.ddlFuncionario', TermoAprovacaoMedicao.container).val(response.Termo.Funcionario);
						$('.ddlTecnicos', TermoAprovacaoMedicao.container).val(response.Termo.ResponsavelMedicao);
						$('.ddlSetoresUsuario', TermoAprovacaoMedicao.container).val(response.Termo.SetorCadastro);
					}

					$('.spanFuncionario, .spanTecnico', TermoAprovacaoMedicao.container).addClass('hide');
					$('.rdbTecIDAF,.rdbTecTercerizado', TermoAprovacaoMedicao.container).removeAttr('checked');

					if (response.Termo.TipoResponsavel == 1) {
						$('.rdbTecIDAF', TermoAprovacaoMedicao.container).attr('checked', 'checked');
						$('.spanFuncionario', TermoAprovacaoMedicao.container).removeClass('hide');

					} else if (response.Termo.TipoResponsavel == 2) {
						$('.rdbTecTercerizado', TermoAprovacaoMedicao.container).attr('checked', 'checked');
						$('.spanTecnico', TermoAprovacaoMedicao.container).removeClass('hide');
					}
				}
			}
		});
	},

	onClickRadio: function () {
		var option;

		$('.spanFuncionario, .spanTecnico, .divSetorCadastro', TermoAprovacaoMedicao.container).addClass('hide');

		if ($(this).val() == 1) {
			$('.spanFuncionario, .divSetorCadastro', TermoAprovacaoMedicao.container).removeClass('hide');

			option = $('.ddlFuncionario', TermoAprovacaoMedicao.container).find('option');
			if (option && option.length == 2) {
				$('.ddlFuncionario', TermoAprovacaoMedicao.container).val($(option[1]).attr('value'));
			}

			option = $('.ddlSetoresUsuario', TermoAprovacaoMedicao.container).find('option');
			if (option && option.length == 2) {
				$('.ddlSetoresUsuario', TermoAprovacaoMedicao.container).val($(option[1]).attr('value'));
			}

		} else {
			$('.spanTecnico', TermoAprovacaoMedicao.container).removeClass('hide');
			option = $('.ddlTecnicos', TermoAprovacaoMedicao.container).find('option');
			if (option && option.length == 2) {
				$('.ddlTecnicos', TermoAprovacaoMedicao.container).val($(option[1]).attr('value'));
			}
		}
	},

	obterObjeto: function () {

		var especificidade = {
			Destinatario: $('.ddlDestinatarios', TermoAprovacaoMedicao.container).val(),
			DataMedicao: { DataTexto: $('.txtDataMedicao', TermoAprovacaoMedicao.container).val() },
			ResponsavelMedicao: null,
			SetorCadastro: null,
			TipoResponsavel: $('.divTipoResp', TermoAprovacaoMedicao.container).find('input[type="radio"]:checked').val(),
			Funcionario: null
		};

		if (especificidade.TipoResponsavel == 1) {
			especificidade.Funcionario = $('.ddlFuncionario', TermoAprovacaoMedicao.container).val();
			especificidade.SetorCadastro = $('.ddlSetoresUsuario', TermoAprovacaoMedicao.container).val();
		} else if (especificidade.TipoResponsavel == 2) {
			especificidade.ResponsavelMedicao = $('.ddlTecnicos', TermoAprovacaoMedicao.container).val();
		}

		return especificidade;
	}
};

Titulo.settings.especificidadeLoadCallback = TermoAprovacaoMedicao.load;
Titulo.settings.obterEspecificidadeObjetoFunc = TermoAprovacaoMedicao.obterObjeto;
Titulo.addCallbackProtocolo(TermoAprovacaoMedicao.obterDadosTermoAprovacaoMedicao);
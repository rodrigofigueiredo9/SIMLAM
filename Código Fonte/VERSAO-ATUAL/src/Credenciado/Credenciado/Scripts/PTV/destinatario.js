/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../mensagem.js" />
/// <reference path="../jquery.ddl.js" />

DestinatarioPTV = {
	settings: {
		urls: {
			verificarCPFCNPJ: '',
			salvar: null
		},
		associarFuncao: null,
		destinatarioCPFCNPJ: null,
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(DestinatarioPTV.settings, options);
			
			//Documento vindo da tela de Emitir Permissão de Trânsito de Vegetais.
			if (DestinatarioPTV.settings.destinatarioCPFCNPJ) {
				$('.txtCPFCNPJ', DestinatarioPTV.container).val(DestinatarioPTV.settings.destinatarioCPFCNPJ);
				if (DestinatarioPTV.settings.destinatarioCPFCNPJ.length > 14) {
					$('.rbPessoaTipoCNPJ', DestinatarioPTV.container).attr('checked', true);
					$('.lblCPFCNPJ', DestinatarioPTV.container).text("CNPJ *");
					$('.maskCpf', DestinatarioPTV.container).addClass('hide');
					$('.maskCnpj', DestinatarioPTV.container).removeClass('hide');
				} else {
					$('.rbPessoaTipoCPF', DestinatarioPTV.container).attr('checked', true);
					$('.lblCPFCNPJ', DestinatarioPTV.container).text("CPF *");
					$('.maskCpf', DestinatarioPTV.container).removeClass('hide');
					$('.maskCnpj', DestinatarioPTV.container).addClass('hide');
				}

				$('#DivTipoPessoa', DestinatarioPTV.container).removeClass('coluna30').addClass('coluna40');
			}
		}

		DestinatarioPTV.container = MasterPage.getContent(container);

		DestinatarioPTV.container.delegate('.rbPessoaTipo', 'change', DestinatarioPTV.tipoPessoaChange);
		DestinatarioPTV.container.delegate('.btnLimpar', 'click', DestinatarioPTV.limpar);
		DestinatarioPTV.container.delegate('.btnValidar', 'click', DestinatarioPTV.verificarCPFCNPJ);
		DestinatarioPTV.container.delegate('.ddlEstado', 'change', Aux.onEnderecoEstadoChange);
		DestinatarioPTV.container.delegate('.btnSalvar', 'click', DestinatarioPTV.salvar);

		Aux.setarFoco(DestinatarioPTV.container);
		if (parseInt($('.hdnDestinatarioID', DestinatarioPTV.container).val()) > 0) {
			DestinatarioPTV.habilitarCampos(false);

			if ($('.rbPessoaTipoCPF', DestinatarioPTV.container).is(':checked')) {
				$('.lblCPFCNPJ', DestinatarioPTV.container).text("CPF *");
				$('.maskCpf', DestinatarioPTV.container).removeClass('hide');
				$('.maskCnpj', DestinatarioPTV.container).addClass('hide');
			}
			else {
				$('.lblCPFCNPJ', DestinatarioPTV.container).text("CNPJ *");
				$('.maskCpf', DestinatarioPTV.container).addClass('hide');
				$('.maskCnpj', DestinatarioPTV.container).removeClass('hide');
			}
		}		
	},

	tipoPessoaChange: function () {
		$('.txtCPFCNPJ', DestinatarioPTV.container).val('');

		if ($('.rbPessoaTipoCPF', DestinatarioPTV.container).is(':checked')) {
			$('.lblCPFCNPJ', DestinatarioPTV.container).text("CPF *");
			$('.maskCpf', DestinatarioPTV.container).removeClass('hide');
			$('.maskCnpj', DestinatarioPTV.container).addClass('hide');
		}
		else {
			$('.lblCPFCNPJ', DestinatarioPTV.container).text("CNPJ *");
			$('.maskCpf', DestinatarioPTV.container).addClass('hide');
			$('.maskCnpj', DestinatarioPTV.container).removeClass('hide');
		}

		$('.txtCPFCNPJ', DestinatarioPTV.container).focus();
	},

	verificarCPFCNPJ: function () {
		Mensagem.limpar(DestinatarioPTV.container);
		var pessoaTipo = ($('.rbPessoaTipoCPF', DestinatarioPTV.container).is(':checked') ? 1 : 2);
		var CPFCNPJ = $('.txtCPFCNPJ:visible', DestinatarioPTV.container).val();

		$.ajax({
			url: DestinatarioPTV.settings.urls.verificarCPFCNPJ,
			data: JSON.stringify({ pessoaTipo: pessoaTipo, CPFCNPJ: CPFCNPJ }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Erros && response.Erros.length > 0) {
					Mensagem.gerar(DestinatarioPTV.container, response.Erros);
				}

				if (response.Valido) {
					$('.btnSalvar', DestinatarioPTV.container).removeClass('hide');
					$('.btnModalOu', DestinatarioPTV.container).removeClass('hide');

					DestinatarioPTV.habilitarCampos(false);
				}
			}
		});
	},

	limpar: function () {
		$('.hdnDestinatarioID', DestinatarioPTV.container).val('');
		$('.txtCPFCNPJ, .txtNomeRazaoSocial, .txtEndereco, .txtItinerario', DestinatarioPTV.container).val('');
		$('.ddlEstado', DestinatarioPTV.container).ddlFirst().change();
		$('.btnSalvar, .btnModalOu', DestinatarioPTV.container).addClass('hide');

		DestinatarioPTV.habilitarCampos(true);
	},

	habilitarCampos: function (habilitado) {
		if (habilitado) {
			$('.btnValidar', DestinatarioPTV.container).removeClass('hide');
			$('.btnLimpar', DestinatarioPTV.container).addClass('hide');
			$('.rbPessoaTipo', DestinatarioPTV.container).removeAttr('disabled');
			$('.txtCPFCNPJ', DestinatarioPTV.container).removeAttr('disabled');
			$('.txtCPFCNPJ', DestinatarioPTV.container).removeClass('disabled');
			$('.esconder', DestinatarioPTV.container).addClass('hide');
		} else {
			$('.btnValidar', DestinatarioPTV.container).addClass('hide');
			$('.btnLimpar', DestinatarioPTV.container).removeClass('hide');
			$('.rbPessoaTipo', DestinatarioPTV.container).attr('disabled', 'disabled');
			$('.txtCPFCNPJ', DestinatarioPTV.container).attr('disabled', 'disabled');
			$('.txtCPFCNPJ', DestinatarioPTV.container).addClass('disabled');
			$('.esconder', DestinatarioPTV.container).removeClass('hide');
		}
	},

	obter: function () {
		var objeto = {
			ID: $('.hdnDestinatarioID', DestinatarioPTV.container).val(),
			PessoaTipo: ($('.rbPessoaTipoCPF', DestinatarioPTV.container).is(':checked') ? 1 : 2),
			CPFCNPJ: $('.txtCPFCNPJ:visible', DestinatarioPTV.container).val(),
			NomeRazaoSocial: $('.txtNomeRazaoSocial', DestinatarioPTV.container).val(),
			Endereco: $('.txtEndereco', DestinatarioPTV.container).val(),
			EstadoID: $('.ddlEstado', DestinatarioPTV.container).val(),
			EstadoSigla: $('.ddlEstado :selected', DestinatarioPTV.container).text(), 
			EstadoTexto: $('.ddlEstado :selected', DestinatarioPTV.container).text(),
			MunicipioID: $('.ddlMunicipio', DestinatarioPTV.container).val(),
			MunicipioTexto: $('.ddlMunicipio :selected', DestinatarioPTV.container).text(),
			Itinerario: $('.txtItinerario', DestinatarioPTV.container).val()
		};

		return objeto;
	},

	salvar: function () {
		Mensagem.limpar(DestinatarioPTV.container);
		MasterPage.carregando(true);
		var objeto = DestinatarioPTV.obter();

		$.ajax({
			url: DestinatarioPTV.settings.urls.salvar,
			data: JSON.stringify({ destinatario: objeto }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Valido) {
					objeto.ID = response.ID;

					if (DestinatarioPTV.settings.associarFuncao) {
						Modal.fechar(DestinatarioPTV.container);
						DestinatarioPTV.settings.associarFuncao(objeto);
					} else {
						MasterPage.redireciona(response.Url);
					}
				}

				if (response.Erros && response.Erros.length > 0) {
					Mensagem.gerar(DestinatarioPTV.container, response.Erros);
				}
			}
		});

		MasterPage.carregando(false);
	}
}
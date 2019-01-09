/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../mensagem.js" />
/// <reference path="../jquery.ddl.js" />

DestinatarioPTV = {
	settings: {
		urls: {
			verificarCPFCNPJ: '',
			salvar: null,
			verificarExportacao: ''
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
					$('.maskCpf', DestinatarioPTV.container).hide();
					$('.maskCnpj', DestinatarioPTV.container).show();
				} else {
					$('.rbPessoaTipoCPF', DestinatarioPTV.container).attr('checked', true);
					$('.lblCPFCNPJ', DestinatarioPTV.container).text("CPF *");
					$('.maskCpf', DestinatarioPTV.container).show();
					$('.maskCnpj', DestinatarioPTV.container).hide();
				}

				$('#DivTipoPessoa', DestinatarioPTV.container).removeClass('coluna30').addClass('coluna40');
			}
		}

		DestinatarioPTV.container = MasterPage.getContent(container);

		DestinatarioPTV.container.delegate('.rbPessoaTipo', 'change', DestinatarioPTV.tipoPessoaChange);
		DestinatarioPTV.container.delegate('.btnLimparCPFCNPJ', 'click', DestinatarioPTV.limparCPFCNPJ);
		DestinatarioPTV.container.delegate('.btnValidarCPFCNPJ', 'click', DestinatarioPTV.verificarCPFCNPJ);
		DestinatarioPTV.container.delegate('.btnLimparExportacao', 'click', DestinatarioPTV.limparExportacao);
		DestinatarioPTV.container.delegate('.btnValidarExportacao', 'click', DestinatarioPTV.verificarExportacao);
		DestinatarioPTV.container.delegate('.ddlEstado', 'change', Aux.onEnderecoEstadoChange);
		DestinatarioPTV.container.delegate('.btnSalvar', 'click', DestinatarioPTV.salvar);

		Aux.setarFoco(DestinatarioPTV.container);
		if (parseInt($('.hdnDestinatarioID', DestinatarioPTV.container).val()) > 0) {
			DestinatarioPTV.habilitarCampos(false, false);

			if ($('.rbPessoaTipoCPF', DestinatarioPTV.container).is(':checked')) {
				$('.lblCPFCNPJ', DestinatarioPTV.container).text("CPF *");
				$('.maskCpf', DestinatarioPTV.container).show();
				$('.maskCnpj', DestinatarioPTV.container).hide();
			}
			else {
				$('.lblCPFCNPJ', DestinatarioPTV.container).text("CNPJ *");
				$('.maskCpf', DestinatarioPTV.container).hide();
				$('.maskCnpj', DestinatarioPTV.container).show();
			}
		}		
	},

	tipoPessoaChange: function () {
		$('.txtCPFCNPJ', DestinatarioPTV.container).val('');

		if ($('.rbPessoaTipoCPF', DestinatarioPTV.container).is(':checked')) {
			$('.maskCpf', DestinatarioPTV.container).show();
			$('.maskCnpj', DestinatarioPTV.container).hide();
			$('.lblCPFCNPJ', DestinatarioPTV.container).show();
			$('.esconder', DestinatarioPTV.container).hide();
			$('.btnValidarCPFCNPJ', DestinatarioPTV.container).show();
			$('.btnValidarExportacao', DestinatarioPTV.container).hide();
			$('.divPais', DestinatarioPTV.container).hide();

			$('.lblCPFCNPJ', DestinatarioPTV.container).text("CPF *");
			$('.maskCpf', DestinatarioPTV.container).show();
			$('.maskCnpj', DestinatarioPTV.container).hide();

			$('.lblUF', DestinatarioPTV.container).text("UF *");
			$('.lblMunicipio', DestinatarioPTV.container).text("Município *");
			$('.lblItinerario', DestinatarioPTV.container).text("Itinerário *");

			$('.txtCPFCNPJ', DestinatarioPTV.container).focus();
		}
		else if ($('.rbPessoaTipoCNPJ', DestinatarioPTV.container).is(':checked')) {
			$('.maskCnpj', DestinatarioPTV.container).show();
			$('.maskCpf', DestinatarioPTV.container).hide();
			$('.lblCPFCNPJ', DestinatarioPTV.container).show();
			$('.esconder', DestinatarioPTV.container).hide();
			$('.btnValidarCPFCNPJ', DestinatarioPTV.container).show();
			$('.btnValidarExportacao', DestinatarioPTV.container).hide();
			$('.divPais', DestinatarioPTV.container).hide();

			$('.lblCPFCNPJ', DestinatarioPTV.container).text("CNPJ *");
			$('.maskCpf', DestinatarioPTV.container).hide();
			$('.maskCnpj', DestinatarioPTV.container).show();

			$('.lblUF', DestinatarioPTV.container).text("UF *");
			$('.lblMunicipio', DestinatarioPTV.container).text("Município *");
			$('.lblItinerario', DestinatarioPTV.container).text("Itinerário *");

			$('.txtCPFCNPJ', DestinatarioPTV.container).focus();
		}
		else if ($('.rbPessoaTipoExportacao', DestinatarioPTV.container).is(':checked')) {
			$('.txtCPFCNPJ', DestinatarioPTV.container).hide();
			$('.lblCPFCNPJ', DestinatarioPTV.container).hide();
			$('.divNomeRazaoSocial', DestinatarioPTV.container).show();
			$('.btnValidarCPFCNPJ', DestinatarioPTV.container).hide();
			$('.btnValidarExportacao', DestinatarioPTV.container).show();
			$('.divPais', DestinatarioPTV.container).show();

			$('.lblUF', DestinatarioPTV.container).text("UF");
			$('.lblMunicipio', DestinatarioPTV.container).text("Município");
			$('.lblItinerario', DestinatarioPTV.container).text("Itinerário");

			$('.txtNomeRazaoSocial', DestinatarioPTV.container).focus();
		}
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
					$('.btnSalvar', DestinatarioPTV.container).show();
					$('.btnModalOu', DestinatarioPTV.container).show();

					DestinatarioPTV.habilitarCampos(false, false);
				}
			}
		});
	},

	verificarExportacao: function () {
		Mensagem.limpar(DestinatarioPTV.container);
		var NomeRazao = $('.txtNomeRazaoSocial', DestinatarioPTV.container).val().trim();

		$.ajax({
			url: DestinatarioPTV.settings.urls.verificarExportacao,
			data: JSON.stringify({ nomeRazaoSocial: NomeRazao }),
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
					$('.btnSalvar', DestinatarioPTV.container).show();
					$('.btnModalOu', DestinatarioPTV.container).show();

					DestinatarioPTV.habilitarCampos(false, true);
				}
			}
		});
	},

	limparCPFCNPJ: function () {
		$('.hdnDestinatarioID', DestinatarioPTV.container).val('');
		$('.txtCPFCNPJ, .txtNomeRazaoSocial, .txtEndereco, .txtItinerario', DestinatarioPTV.container).val('');
		$('.ddlEstado', DestinatarioPTV.container).ddlFirst().change();
		$('.btnSalvar, .btnModalOu', DestinatarioPTV.container).hide();

		DestinatarioPTV.habilitarCampos(true, false);
	},

	limparExportacao: function () {
		$('.hdnDestinatarioID', DestinatarioPTV.container).val('');
		$('.txtCPFCNPJ, .txtNomeRazaoSocial, .txtEndereco, .txtItinerario', DestinatarioPTV.container).val('');
		$('.ddlEstado', DestinatarioPTV.container).ddlFirst().change();
		$('.btnSalvar, .btnModalOu', DestinatarioPTV.container).hide();

		DestinatarioPTV.habilitarCampos(true, true);
	},

	habilitarCampos: function (habilitado, exportacao) {
		if (exportacao == true) {
			if (habilitado) {
				$('.btnValidarExportacao', DestinatarioPTV.container).show();
				$('.btnLimparExportacao', DestinatarioPTV.container).hide();
				$('.rbPessoaTipo', DestinatarioPTV.container).removeAttr('disabled');
				$('.txtNomeRazaoSocial', DestinatarioPTV.container).removeAttr('disabled');
				$('.txtNomeRazaoSocial', DestinatarioPTV.container).removeClass('disabled');
				$('.esconder', DestinatarioPTV.container).hide();
				$('.divNomeRazaoSocial', DestinatarioPTV.container).show();
			} else {
				$('.btnValidarExportacao', DestinatarioPTV.container).hide();
				$('.btnLimparExportacao', DestinatarioPTV.container).show();
				$('.rbPessoaTipo', DestinatarioPTV.container).attr('disabled', 'disabled');
				$('.txtNomeRazaoSocial', DestinatarioPTV.container).attr('disabled', 'disabled');
				$('.txtNomeRazaoSocial', DestinatarioPTV.container).addClass('disabled');
				$('.esconder', DestinatarioPTV.container).show();
			}
		} else {
			if (habilitado) {
				$('.btnValidarCPFCNPJ', DestinatarioPTV.container).show();
				$('.btnLimparCPFCNPJ', DestinatarioPTV.container).hide();
				$('.rbPessoaTipo', DestinatarioPTV.container).removeAttr('disabled');
				$('.txtCPFCNPJ', DestinatarioPTV.container).removeAttr('disabled');
				$('.txtCPFCNPJ', DestinatarioPTV.container).removeClass('disabled');
				$('.esconder', DestinatarioPTV.container).hide();
			} else {
				$('.btnValidarCPFCNPJ', DestinatarioPTV.container).hide();
				$('.btnLimparCPFCNPJ', DestinatarioPTV.container).show();
				$('.rbPessoaTipo', DestinatarioPTV.container).attr('disabled', 'disabled');
				$('.txtCPFCNPJ', DestinatarioPTV.container).attr('disabled', 'disabled');
				$('.txtCPFCNPJ', DestinatarioPTV.container).addClass('disabled');
				$('.esconder', DestinatarioPTV.container).show();
			}
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
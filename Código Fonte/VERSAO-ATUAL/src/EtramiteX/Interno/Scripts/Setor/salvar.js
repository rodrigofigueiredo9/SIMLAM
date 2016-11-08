/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

SetorSalvar = {
	settings: {
		urls: {
			salvar: '',
			urlObterMunicipiosPorEstado: ''
		}
	},
	container: null,
	Mensagens: {},

	load: function (container, options) {
		if (options) {
			$.extend(SetorSalvar.settings, options);
		}

		SetorSalvar.container = MasterPage.getContent(container);
		SetorSalvar.container = container;
		SetorSalvar.container.delegate('.btnSalvarSetor', 'click', SetorSalvar.salvar);
		SetorSalvar.container.delegate('.ddlEstado', 'change', SetorSalvar.obterMunicipiosPorEstado);
	},

	obter: function () {
		return {
			Id: $('.hdnSetorId', SetorSalvar.container).val(),
			Sigla: $('.txtSigla', SetorSalvar.container).val(),
			Nome: $('.ddlSetor :selected', SetorSalvar.container).text(),
			Responsavel: $('.txtResponsavel', SetorSalvar.container).val(),
			Agrupador: $('.ddlAgrupador :selected', SetorSalvar.container).val(),
			AgrupadorTexto: $('.ddlAgrupador :selected', SetorSalvar.container).text(),
			Endereco: {
				Logradouro: $('.txtLogradouro', SetorSalvar.container).val(),
				Numero: $('.txtNumero', SetorSalvar.container).val(),
				Bairro: $('.txtBairro', SetorSalvar.container).val(),
				Cep: $('.txtCep', SetorSalvar.container).val(),
				EstadoId: $('.ddlEstado :selected', SetorSalvar.container).val(),
				EstadoTexto: $('.ddlEstado :selected', SetorSalvar.container).text(),
				MunicipioId: $('.ddlMunicipio :selected', SetorSalvar.container).val(),
				MunicipioTexto: $('.ddlMunicipio :selected', SetorSalvar.container).text(),
				Fone: $('.txtTelefone', SetorSalvar.container).val(),
				Fax: $('.txtTelFax', SetorSalvar.container).val(),
				Complemento: $('.txtComplemento', SetorSalvar.container).val()
			}
		}
	},

	obterMunicipiosPorEstado: function () {
		var estado = $(this).val();
		if (estado == null) {
			$('.ddlMunicipio', SetorSalvar.container).ddlClear();
			return;
		}

		$.ajax({ url: SetorSalvar.settings.urls.urlObterMunicipiosPorEstado,
			data: JSON.stringify({estado: estado}),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(SetorSalvar.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Municipios.length > 0) {
					$('.ddlMunicipio', SetorSalvar.container).ddlLoad(response.Municipios);
				}
			}
		});
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: SetorSalvar.settings.urls.salvar,
			data: JSON.stringify(SetorSalvar.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, SetorSalvar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					SetorSalvar.abrirModalMerge(response.TextoMerge);
					return;
				}

				if (response.EhValido) {
					MasterPage.redireciona(response.urlRetorno);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(SetorSalvar.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
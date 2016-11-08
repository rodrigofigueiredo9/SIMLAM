/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

MotosserraVerificar = {
	settings: {
		urls: {
			associarPessoa: null,
			visualizarPessoa: null,
			salvar: null,
			verificar: null,
			obterPartialCriar: null,
			editarMotosserra: null,
			visualizarMotosserra: null,
			validarEditar: null,
			associarPessoa: null,
			visualizarPessoa: null
		}
	},

	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(MotosserraVerificar.settings, options);
		}

		MotosserraVerificar.container = MasterPage.getContent(container);
		MotosserraVerificar.container.delegate('.btnVerificar', 'click', MotosserraVerificar.onVerificar);
		MotosserraVerificar.container.delegate('.txtSerieNumero', 'change', MotosserraVerificar.onVerificar);
		MotosserraVerificar.container.delegate('.btnLimpar', 'click', MotosserraVerificar.onLimpar);
		MotosserraVerificar.container.delegate('.btnNovo', 'click', MotosserraVerificar.onCriarNovo);
		MotosserraVerificar.container.delegate('.btnVisualizar', 'click', MotosserraVerificar.visualizarMotosserra);
		MotosserraVerificar.container.delegate('.btnEditar', 'click', MotosserraVerificar.editarMotosserra);

		Aux.setarFoco(container);

		MotosserraVerificar.container.delegate('.txtSerieNumero', 'keyup', function (e) {
			if (e.keyCode == 13) {
				$('.btnVerificar', MotosserraVerificar.container).trigger('click');
			}
		});
	},

	onCriarNovo: function () {
		Mensagem.limpar(MotosserraVerificar.container);

		$.ajax({
			url: MotosserraVerificar.settings.urls.obterPartialCriar,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MotosserraVerificar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido && response.Html) {
					$('.containerMotosserra', MotosserraVerificar.container).html(response.Html);

					Motosserra.load(MotosserraVerificar.container,
					{
						urls: {
							associarPessoa: MotosserraVerificar.settings.urls.associarPessoa,
							visualizarPessoa: MotosserraVerificar.settings.urls.visualizarPessoa,
							salvar: MotosserraVerificar.settings.urls.salvar
						}
					})

					$('.btnNovo', MotosserraVerificar.container).addClass('hide');
					$('.btnMotosserraSalvar', MotosserraVerificar.container).removeClass('hide');
					$('.fsMotosserras', MotosserraVerificar.container).addClass('hide');

					$('.txtSerieNumero', MotosserraVerificar.container).attr('disabled', 'disabled').addClass('disabled');
					$('.btnVerificar', MotosserraVerificar.container).addClass('hide');
					$('.btnLimpar', MotosserraVerificar.container).removeClass('hide');

					MasterPage.botoes(MotosserraVerificar.container);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MotosserraVerificar.container, response.Msg);
				}
			}
		});
	},

	onVerificar: function () {
		Mensagem.limpar(MotosserraVerificar.container);

		var numero = $('.txtSerieNumero', MotosserraVerificar.container).val().trim();
		$('.txtSerieNumero', MotosserraVerificar.container).val(numero);

		$('.btnNovo', MotosserraVerificar.container).addClass('hide');
		$('.btnModalOu', MotosserraVerificar.container).addClass('hide');

		$.ajax({
			url: MotosserraVerificar.settings.urls.verificar,
			data: JSON.stringify({numero: numero}),
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MotosserraVerificar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					if (response.Html) {
						$('.fsMotosserras', MotosserraVerificar.container).removeClass('hide');
						$('.fsMotosserras .dataGridView', MotosserraVerificar.container).html(response.Html);

						Listar.atualizarEstiloTable();
					}

					if (response.PodeCriarNovo) {
						$('.btnNovo', MotosserraVerificar.container).removeClass('hide');
						$('.btnModalOu', MotosserraVerificar.container).removeClass('hide');
					}
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MotosserraVerificar.container, response.Msg);
				}
			}
		});

	},

	onLimpar: function () {
		$('.containerMotosserra', MotosserraVerificar.container).empty();
		$('.btnVerificar', MotosserraVerificar.container).removeClass('hide');
		$('.btnLimpar', MotosserraVerificar.container).addClass('hide');

		$('.btnNovo', MotosserraVerificar.container).addClass('hide');
		$('.btnMotosserraSalvar', MotosserraVerificar.container).addClass('hide');

		$('.txtSerieNumero', MotosserraVerificar.container).val('').removeClass('disabled').attr('disabled', false);
		$('.btnModalOu', MotosserraVerificar.container).addClass('hide');

		Aux.setarFoco(MotosserraVerificar.container);
	},

	editarMotosserra: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;
		var retorno = MasterPage.validarAjax(MotosserraVerificar.settings.urls.validarEditar, { motosserraId: itemId }, MotosserraVerificar.container, false);

		if (retorno.EhValido) {
			MasterPage.redireciona(MotosserraVerificar.settings.urls.editarMotosserra + '?id=' + itemId);
		}
	},

	visualizarMotosserra: function () {
		var itemId = $.parseJSON($(this).closest('tr').find('.itemJson').val()).Id;

		Modal.abrir(MotosserraVerificar.settings.urls.visualizarMotosserra + '/' + itemId, null, function (container) {
			Motosserra.load(container);
			Motosserra.settings.urls.associarPessoa = MotosserraVerificar.settings.urls.associarPessoa;
			Motosserra.settings.urls.visualizarPessoa = MotosserraVerificar.settings.urls.visualizarPessoa;

			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);
	}
}

Motosserra = {
	container: null,
	pessoaModal: null,
	settings: {
		possuiRegistro: false,
		registroNumero: null,
		urls: {
			associarPessoa: null,
			visualizarPessoa: null,
			salvar: null
		}
	},

	load: function (container, options) {
		if (options) {
			$.extend(Motosserra.settings, options);
		}

		container = MasterPage.getContent(container);
		Motosserra.container = container;

		container.delegate('.rdbPossuiRegistro', 'change', Motosserra.possuiRegistroChange);
		container.delegate('.btnAssociarPessoa', 'click', Motosserra.associarPessoa);
		container.delegate('.btnLimparPessoa', 'click', Motosserra.limparPessoa);
		container.delegate('.btnVisualizarPessoa', 'click', Motosserra.visualizarPessoa);
		container.delegate('.btnMotosserraSalvar', 'click', Motosserra.salvar);
	},

	possuiRegistroChange: function () {
		if ($('.rdbPossuiRegistro:checked', Motosserra.container).val().toLowerCase() == 'true') {
			$('.txtRegistroNumero', Motosserra.container).val(((Motosserra.settings.possuiRegistro && Motosserra.settings.registroNumero > 0) ? Motosserra.settings.registroNumero : ''));
			$('.txtRegistroNumero', Motosserra.container).removeAttr('disabled').removeClass('disabled');
		} else {
			$('.txtRegistroNumero', Motosserra.container).val(((!Motosserra.settings.possuiRegistro && Motosserra.settings.registroNumero > 0) ? Motosserra.settings.registroNumero : 'Gerado automaticamente'));
			$('.txtRegistroNumero', Motosserra.container).attr('disabled', 'disabled').addClass('disabled');
		}
	},

	associarPessoa: function () {
		Motosserra.pessoaModal = new PessoaAssociar();

		Modal.abrir(Motosserra.settings.urls.associarPessoa, null, function (container) {
			Motosserra.pessoaModal.load(container, {
				tituloCriar: 'Cadastrar Proprietário',
				tituloEditar: 'Editar Proprietário',
				tituloVisualizar: 'Visualizar Proprietário',
				onAssociarCallback: Motosserra.callBackPessoa
			});
		});
	},

	visualizarPessoa: function () {
		
		var id = $('.hdnPessoaId', Motosserra.container).val();
		Motosserra.pessoaModal = new PessoaAssociar();

		Modal.abrir(Motosserra.settings.urls.visualizarPessoa + "/" + id, null, function (container) {
			Motosserra.pessoaModal.load(container, {
				tituloCriar: 'Cadastrar Proprietário',
				tituloEditar: 'Editar Proprietário',
				tituloVisualizar: 'Visualizar Proprietário',
				onAssociarCallback: Motosserra.callBackPessoa,
				editarVisualizar: $('.btnAssociarPessoa', Motosserra.container).is(':visible')
			});
		});
	},

	limparPessoa: function () {

		$('.spanVisualizarPessoa', Motosserra.container).addClass('hide');
		$('.hdnPessoaId', Motosserra.container).val(0);
		$('.txtPessoaNomeRazaoSocial', Motosserra.container).val('');
		$('.txtPessoaCPFCNPJ', Motosserra.container).val('');

		$('.btnLimparContainerPessoa', Motosserra.container).addClass('hide');
		$('.btnAssociarPessoa', Motosserra.container).removeClass('hide');
	},

	callBackPessoa: function (pessoa) {
		$('.spanVisualizarPessoa', Motosserra.container).removeClass('hide');
		$('.hdnPessoaId', Motosserra.container).val(pessoa.Id);
		$('.txtPessoaNomeRazaoSocial', Motosserra.container).val(pessoa.NomeRazaoSocial);
		$('.txtPessoaCPFCNPJ', Motosserra.container).val(pessoa.CPFCNPJ);

		$('.btnLimparContainerPessoa', Motosserra.container).removeClass('hide');
		$('.btnAssociarPessoa', Motosserra.container).addClass('hide');

		return true;
	},

	obter: function () {
		var objeto = {
			Id: $('.hdnArtefatoId', Motosserra.container).val(),
			PossuiRegistro: $('.rdbPossuiRegistro:checked', Motosserra.container).val(),
			RegistroNumero: $('.txtRegistroNumero', Motosserra.container).val(),
			SerieNumero: $('.txtSerieNumero', Motosserra.container).val(),
			Modelo: $('.txtModelo', Motosserra.container).val(),
			NotaFiscalNumero: $('.txtNotaFiscalNumero', Motosserra.container).val(),
			Proprietario: { Id: $('.hdnPessoaId', Motosserra.container).val() },
			SituacaoId: $('.hdnSituacaoId', Motosserra.container).val()
		};

		return objeto;
	},

	salvar: function () {
		var params = { motosserra: Motosserra.obter() };
		MasterPage.carregando(true);

		$.ajax({ url: Motosserra.settings.urls.salvar, data: JSON.stringify(params), type: 'POST', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Motosserra.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				} else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Motosserra.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
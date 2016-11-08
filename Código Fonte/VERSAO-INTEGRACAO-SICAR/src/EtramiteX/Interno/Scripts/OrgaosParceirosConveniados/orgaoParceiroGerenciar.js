///<reference path="../masterpage.js" />
///<reference path="../jquery.json-2.2.min.js" />

OrgaoParceiroGerenciar = {
	settings: {
		urls: {
			gerarChave: null,
			bloquear: null,
			desbloquear: null,
			visualizar: null,
			buscar:null
		},
		Mensagens:null,
	},
	container: null,

	load: function (container, options) {

		if (options) {
			$.extend(OrgaoParceiroGerenciar.settings, options);
		}
		OrgaoParceiroGerenciar.container = container;
		OrgaoParceiroGerenciar.container.delegate(".btnBuscar", 'click', OrgaoParceiroGerenciar.buscar);
		OrgaoParceiroGerenciar.container.delegate('.btnVisualizar', 'click', OrgaoParceiroGerenciar.onVisualizarPessoa);
		OrgaoParceiroGerenciar.container.delegate('.btnGerarChave', 'click', OrgaoParceiroGerenciar.onConfirmEnviarEmail);
		OrgaoParceiroGerenciar.container.delegate(".cbMarcarTodos", 'change', OrgaoParceiroGerenciar.onMarcarTodos);
		OrgaoParceiroGerenciar.container.delegate(".cb", 'click', OrgaoParceiroGerenciar.onMarcar);
		OrgaoParceiroGerenciar.container.delegate(".btnBloquear", 'click', OrgaoParceiroGerenciar.onBloquear);
		OrgaoParceiroGerenciar.container.delegate(".btnDesbloquear", 'click', OrgaoParceiroGerenciar.onDesbloquear);
	},

	buscar: function () {
		$.ajax({
			url: OrgaoParceiroGerenciar.settings.urls.buscar,
			data: JSON.stringify({ idOrgaoParceiro: $('.hdnIdOrgao', OrgaoParceiroGerenciar.container).val(), idUnidade: $('.ddlUnidades', OrgaoParceiroGerenciar.container).val() }),
			async: false,
			type: 'POST',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, OrgaoParceiroGerenciar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Url) {
					MasterPage.redireciona(response.Url);
				}

				if (response.EhValido) {

					if (response.HtmlAguardandoAtivacao) {
						$('.fsCredenciadosAguardandoAtivacao', OrgaoParceiroGerenciar.container).removeClass('hide');
						$('.divAguardando').html(response.HtmlAguardandoAtivacao);

					} else {
						$('.fsCredenciadosAguardandoAtivacao', OrgaoParceiroGerenciar.container).addClass('hide');
					}

					if (response.HtmlAtivos) {
						$('.fsCredenciadosAtivos', OrgaoParceiroGerenciar.container).removeClass('hide');
						$('.divAtivos').html(response.HtmlAtivos);
					} else {
						$('.fsCredenciadosAtivos', OrgaoParceiroGerenciar.container).addClass('hide');
					}

					if (response.HtmlBloqueados) {
						$('.fsCredenciadosBloqueados', OrgaoParceiroGerenciar.container).removeClass('hide');
						$('.divBloqueados').html(response.HtmlBloqueados);
					} else {
						$('.fsCredenciadosBloqueados', OrgaoParceiroGerenciar.container).addClass('hide');
					}

					Listar.atualizarEstiloTable($('.dataGridTable', OrgaoParceiroGerenciar.container));
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(OrgaoParceiroGerenciar.container, response.Msg);
				}
			}
		});
	},

	onVisualizarPessoa: function ()	{
		var credenciado = JSON.parse($(this).closest("tr").find(".hdnItemJson").val());
		Modal.abrir(OrgaoParceiroGerenciar.settings.urls.visualizar , { Id: credenciado.Id}, function (container) {
			$('.titTela', container).text('Visualizar Credenciado');
			Modal.defaultButtons(container);
		});
	},

	onMarcarTodos: function () {
		var container = $(this, OrgaoParceiroGerenciar.container).closest(".dataGridTable");
		$('.cb', container).each(function () { this.checked = $('.cbMarcarTodos', container).is(':checked'); });
	},

	onMarcar: function () {
		var container = $(this, OrgaoParceiroGerenciar.container).closest(".dataGridTable");
		var checks = $(".cb", container);
		
		for (var i = 0; i < checks.length; i++) {
			if (!checks[i].checked) {
				$('.cbMarcarTodos', container).each(function () { this.checked = checks[i].checked; });
				return;
			}
		}
		$('.cbMarcarTodos', container).each(function () { this.checked = true; });
	},

	onConfirmEnviarEmail: function () {
		var container = $(this, OrgaoParceiroGerenciar.container).closest(".fsCredenciadosAguardandoAtivacao");

		if (!$('.cb', container).is(':checked'))
		{
			Mensagem.gerar(OrgaoParceiroGerenciar.container, [OrgaoParceiroGerenciar.settings.Mensagens.SelecioneUmCredenciado]);
			return;
		}

		var credenciados = [];
		var obj = {};
		
		$('.cb:checked', container).each(function () {
			obj = JSON.parse($(this).closest('tr').find('.hdnItemJson').val());
			credenciados.push({ Id: obj.Id, Nome: obj.Nome, Chave: obj.Chave, OrgaoParceiroId: obj.OrgaoParceiroId ,Pessoa: { MeiosContatos: [{ TipoContatoInteiro: 5, Valor: obj.Email }] } });
			
		});

		Modal.confirma({
			btnOkCallback: function (modalContent) {
				OrgaoParceiroGerenciar.enviarEmail(credenciados);
				Modal.fechar(modalContent);
			},
			conteudo: OrgaoParceiroGerenciar.settings.Mensagens.ConfirmGerarChave.Texto,
			titulo: OrgaoParceiroGerenciar.settings.Mensagens.TituloConfirmGerarChave.Texto
		});
	},

	enviarEmail: function (credenciados) {
		$.ajax({
			url: OrgaoParceiroGerenciar.settings.urls.gerarChave,
			data: JSON.stringify({ credenciados: credenciados, idOrgaoParceiro: $('.hdnIdOrgao', OrgaoParceiroGerenciar.container).val(), idUnidade: $('.ddlUnidades', OrgaoParceiroGerenciar.container).val() }),
			async: false,
			type: 'POST',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, OrgaoParceiroGerenciar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					if (response.HtmlRetorno) {
						$('.fsCredenciadosAguardandoAtivacao', OrgaoParceiroGerenciar.container).removeClass('hide');
						$('.divAguardando').html(response.HtmlRetorno);

					} else {
						$('.fsCredenciadosAguardandoAtivacao', OrgaoParceiroGerenciar.container).addClass('hide');
					}
					Mensagem.gerar(OrgaoParceiroGerenciar.container, response.Msg);
					Listar.atualizarEstiloTable($('.dataGridTable', OrgaoParceiroGerenciar.container));
				}

				if (response.Url) {
					MasterPage.redireciona(response.Url);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(OrgaoParceiroGerenciar.container, response.Msg);
					
				}
			}
		});
	},

	onBloquear: function () {
		var container = $(this, OrgaoParceiroGerenciar.container).closest(".fsCredenciadosAtivos");

		if (!$('.cb', container).is(':checked')) {
			Mensagem.gerar(OrgaoParceiroGerenciar.container, [OrgaoParceiroGerenciar.settings.Mensagens.SelecioneUmCredenciado]);
			return;
		}

		var credenciados = [];
		var obj = {};

		$('.cb:checked', container).each(function () {
			obj = JSON.parse($(this).closest('tr').find('.hdnItemJson').val());
			credenciados.push({ Id: obj.Id, Nome: obj.Nome, Chave: obj.Chave, OrgaoParceiroId: obj.OrgaoParceiroId, Pessoa: { MeiosContatos: [{ TipoContatoInteiro: 5, Valor: obj.Email }] } });
		});

		Modal.confirma({
			btnOkCallback: function (modalContent) {
				OrgaoParceiroGerenciar.bloquear(credenciados);
				Modal.fechar(modalContent);
			},
			conteudo: OrgaoParceiroGerenciar.settings.Mensagens.ConfirmBloquear.Texto,
			titulo: OrgaoParceiroGerenciar.settings.Mensagens.TituloBloquear.Texto
		});
	},

	bloquear: function (credenciados) {
		$.ajax({
			url: OrgaoParceiroGerenciar.settings.urls.bloquear,
			data: JSON.stringify({ credenciados: credenciados, idOrgaoParceiro: $('.hdnIdOrgao', OrgaoParceiroGerenciar.container).val(), idUnidade: $('.ddlUnidades', OrgaoParceiroGerenciar.container).val() }),
			async: false,
			type: 'POST',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, OrgaoParceiroGerenciar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.HtmlRetornoAtivos) {
					$('.fsCredenciadosAtivos', OrgaoParceiroGerenciar.container).removeClass('hide');
					$('.divAtivos').html(response.HtmlRetornoAtivos);

				} else {
					$('.fsCredenciadosAtivos', OrgaoParceiroGerenciar.container).addClass('hide');
				}

				if (response.HtmlRetornoBloqueados) {
					$('.fsCredenciadosBloqueados', OrgaoParceiroGerenciar.container).removeClass('hide');
					$('.divBloqueados').html(response.HtmlRetornoBloqueados);

				} else {
					$('.fsCredenciadosBloqueados', OrgaoParceiroGerenciar.container).addClass('hide');
				}

				Mensagem.gerar(OrgaoParceiroGerenciar.container, response.Msg);
				Listar.atualizarEstiloTable($('.dataGridTable', OrgaoParceiroGerenciar.container));
				
				if (response.Url) {
					MasterPage.redireciona(response.Url);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(OrgaoParceiroGerenciar.container, response.Msg);
				}
			}
		});
	},

	onDesbloquear: function () {
		var container = $(this, OrgaoParceiroGerenciar.container).closest(".fsCredenciadosBloqueados");

		if (!$('.cb', container).is(':checked')) {
			Mensagem.gerar(OrgaoParceiroGerenciar.container, [OrgaoParceiroGerenciar.settings.Mensagens.SelecioneUmCredenciado]);
			return;
		}

		var credenciados = [];
		var obj = {};

		$('.cb:checked', container).each(function () {
			obj = JSON.parse($(this).closest('tr').find('.hdnItemJson').val());
			credenciados.push({ Id: obj.Id, Nome: obj.Nome, Chave: obj.Chave, OrgaoParceiroId: obj.OrgaoParceiroId, Pessoa: { MeiosContatos: [{ TipoContatoInteiro: 5, Valor: obj.Email }] } });
		});

		Modal.confirma({
			btnOkCallback: function (modalContent) {
				OrgaoParceiroGerenciar.desbloquear(credenciados);
				Modal.fechar(modalContent);
			},
			conteudo: OrgaoParceiroGerenciar.settings.Mensagens.ConfirmDesbloquear.Texto,
			titulo: OrgaoParceiroGerenciar.settings.Mensagens.TituloDesbloquear.Texto
		});
	},

	desbloquear: function (credenciados) {
		$.ajax({
			url: OrgaoParceiroGerenciar.settings.urls.desbloquear,
			data: JSON.stringify({ credenciados: credenciados, idOrgaoParceiro: $('.hdnIdOrgao', OrgaoParceiroGerenciar.container).val(), idUnidade: $('.ddlUnidades', OrgaoParceiroGerenciar.container).val() }),
			async: false,
			type: 'POST',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, OrgaoParceiroGerenciar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.HtmlRetornoBloqueados) {
					$('.fsCredenciadosBloqueados', OrgaoParceiroGerenciar.container).removeClass('hide');
					$('.divBloqueados').html(response.HtmlRetornoBloqueados);

				} else {
					$('.fsCredenciadosBloqueados', OrgaoParceiroGerenciar.container).addClass('hide');
				}
								
				Mensagem.gerar(OrgaoParceiroGerenciar.container, response.Msg);
				Listar.atualizarEstiloTable($('.dataGridTable', OrgaoParceiroGerenciar.container));

				if (response.Url) {
					MasterPage.redireciona(response.Url);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(OrgaoParceiroGerenciar.container, response.Msg);
				}
			}
		});
	}
}
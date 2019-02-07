/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../mensagem.js" />

Caracterizacao = {
	settings: {
		urls: {
			CriarProjetoGeo: null,
			EditarProjetoGeo: null,
			VisualizarProjetoGeo: null,
			AssociarCaracterizacao: null,
			DesassociarCaracterizacao: null,
			CopiarDadosInstitucional: null,
			FinalizarPasso: null
		},
		empreendimentoID: 0,
		projetoDigitalID: 0,
		caracterizacoesPossivelCopiar: [],
		dependenciaTipos: {},
		mensagens: {}
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Caracterizacao.settings, options); }
		Caracterizacao.container = MasterPage.getContent(container);

		Caracterizacao.container.delegate('.btnCopiar', 'click', Caracterizacao.confirmarcopiar);
		Caracterizacao.container.delegate('.btnAdicionar', 'click', Caracterizacao.adicionar);
		Caracterizacao.container.delegate('.btnVisualizar', 'click', Caracterizacao.visualizar);
		Caracterizacao.container.delegate('.btnProjetoGeografico', 'click', Caracterizacao.projetoGeografico);
		Caracterizacao.container.delegate('.btnEditar', 'click', Caracterizacao.editar);
		Caracterizacao.container.delegate('.btnExcluir', 'click', Caracterizacao.excluir);
		Caracterizacao.container.delegate('.btnAssociar', 'click', Caracterizacao.associar);
		Caracterizacao.container.delegate('.btnDesassociar', 'click', Caracterizacao.desassociar);
		Caracterizacao.container.delegate('.btnFinalizarPasso', 'click', Caracterizacao.confirmarFinalizar);
		Caracterizacao.container.delegate('.btnListar', 'click', Caracterizacao.listar);

		Listar.atualizarEstiloTable();
	},

	adicionar: function () {
		var linha = $(this).closest('tr');

		if ($('.hdnPossuiProjetoGeo', linha).val() == 'true') {
			var caracterizacaoTipo = $('.hdnCaracterizacaoTipo', linha).val();
			var isCadastrarCaracterizacao = !linha.closest('fieldset').hasClass('fsCadastradas');
			var projeto = $('.hdnProjetoGeograficoId', linha).val();
			var isCadastrar = !$(this).hasClass('projetoGeografico');
			var urlProjeto = (isCadastrar) ? Caracterizacao.settings.urls.CriarProjetoGeo : Caracterizacao.settings.urls.EditarProjetoGeo;

			if (isCadastrar) {
				urlProjeto = urlProjeto +
					'?empreendimento=' + Caracterizacao.settings.empreendimentoID +
					'&tipo=' + caracterizacaoTipo +
					'&isCadastrarCaracterizacao=' + isCadastrarCaracterizacao +
					'&projetoDigitalId=' + Caracterizacao.settings.projetoDigitalID;
			} else {
				urlProjeto = urlProjeto +
					'?id=' + projeto +
					'&empreendimento=' + Caracterizacao.settings.empreendimentoID +
					'&tipo=' + caracterizacaoTipo +
					'&isCadastrarCaracterizacao=' + isCadastrarCaracterizacao +
					'&projetoDigitalId=' + Caracterizacao.settings.projetoDigitalID;
			}
			MasterPage.redireciona(urlProjeto);
		} else {
			MasterPage.redireciona($('.hdnUrlCriar', linha).val() + '/' + Caracterizacao.settings.empreendimentoID + '?projetoDigitalId=' + Caracterizacao.settings.projetoDigitalID);
		}
	},

	visualizar: function () {
		MasterPage.redireciona($('.hdnUrlVisualizar', $(this).closest('tr')).val() + '/' +
			Caracterizacao.settings.empreendimentoID +
			'?projetoDigitalId=' + Caracterizacao.settings.projetoDigitalID +
			'&retornarVisualizar=' + ($('.btnFinalizarPasso', Caracterizacao.container).length <= 0));
	},

	projetoGeografico: function () {
		var linha = $(this).closest('tr');
		var projeto = $('.hdnProjetoGeograficoId', linha).val();
		var caracterizacaoTipo = $('.hdnCaracterizacaoTipo', linha).val();
		var isCadastrarCaracterizacao = !linha.closest('fieldset').hasClass('fsCadastradas') && !linha.closest('fieldset').hasClass('fsAssociadas');
		var isCadastrar = !$('.btnProjetoGeografico', Caracterizacao.container).hasClass('projetoGeografico');
		var urlProjeto = (isCadastrar) ? Caracterizacao.settings.urls.CriarProjetoGeo : Caracterizacao.settings.urls.EditarProjetoGeo;
		var isVisualizar = $('.hdnProjetoGeograficoVisualizar', linha).val() == 'True';

		urlProjeto = (isVisualizar) ? Caracterizacao.settings.urls.VisualizarProjetoGeo : urlProjeto;

		if (isCadastrar) {
			urlProjeto = urlProjeto +
				'?empreendimento=' + Caracterizacao.settings.empreendimentoID +
				'&tipo=' + caracterizacaoTipo +
				'&isCadastrarCaracterizacao=' + isCadastrarCaracterizacao +
				'&projetoDigitalId=' + Caracterizacao.settings.projetoDigitalID;
		} else {
			urlProjeto = urlProjeto +
				'?id=' + projeto +
				'&empreendimento=' + Caracterizacao.settings.empreendimentoID +
				'&tipo=' + caracterizacaoTipo +
				'&isCadastrarCaracterizacao=' + isCadastrarCaracterizacao +
				'&projetoDigitalId=' + Caracterizacao.settings.projetoDigitalID +
				'&retornarVisualizar=' + ($('.btnFinalizarPasso', Caracterizacao.container).length <= 0);
		}

		MasterPage.redireciona(urlProjeto);
	},

	editar: function () {
		MasterPage.redireciona($('.hdnUrlEditar', $(this).closest('tr')).val() + '/' + Caracterizacao.settings.empreendimentoID + '?projetoDigitalId=' + Caracterizacao.settings.projetoDigitalID);
	},

	excluir: function () {
		var linha = $(this).closest('tr');

		Modal.excluir({
			'urlConfirm': $('.hdnUrlExcluirConfirm', linha).val() + '/' + Caracterizacao.settings.empreendimentoID + '?projetoDigitalId=' + Caracterizacao.settings.projetoDigitalID,
			'urlAcao': $('.hdnUrlExcluir', linha).val(),
			'data': { id: Caracterizacao.settings.empreendimentoID, projetoDigitalId: Caracterizacao.settings.projetoDigitalID },
			'callBack': Caracterizacao.callBackExcluirCaracterizacao,
			'naoExecutarUltimaBusca': true
		});
	},

	callBackExcluirCaracterizacao: function (data) {
		MasterPage.redireciona(data.urlRedireciona);
	},

	associar: function () {
		var linha = $(this).closest('tr');
		var caracterizacaoTipo = $('.hdnCaracterizacaoTipo', linha).val();

		var objeto = {
			Id: Caracterizacao.settings.projetoDigitalID,
			EmpreendimentoId: Caracterizacao.settings.empreendimentoID,
			Dependencias: [{ DependenciaTipo: Caracterizacao.settings.dependenciaTipos.TipoCaracterizacao, DependenciaCaracterizacao: caracterizacaoTipo }]
		};

		MasterPage.carregando(true);
		$.ajax({
			url: Caracterizacao.settings.urls.AssociarCaracterizacao,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Caracterizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Caracterizacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	desassociar: function () {
		var linha = $(this).closest('tr');
		var caracterizacaoTipo = $('.hdnCaracterizacaoTipo', linha).val();

		var objeto = {
			Id: Caracterizacao.settings.projetoDigitalID,
			EmpreendimentoId: Caracterizacao.settings.empreendimentoID,
			Dependencias: [{ DependenciaTipo: Caracterizacao.settings.dependenciaTipos.TipoCaracterizacao, DependenciaCaracterizacao: caracterizacaoTipo }]
		};

		MasterPage.carregando(true);
		$.ajax({
			url: Caracterizacao.settings.urls.DesassociarCaracterizacao,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Caracterizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Caracterizacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	confirmarcopiar: function () {
		var linha = $(this).closest('tr');
		var caracterizacaoTipo = +$('.hdnCaracterizacaoTipo', linha).val();
		var caracterizacaoTexto = $('.tipoTexto', linha).html();

		if ($(this).closest('table').hasClass('gridCaracterizacao')) {
			Caracterizacao.copiar(linha);
			return;
		}

		if ($.inArray(caracterizacaoTipo, Caracterizacao.settings.caracterizacoesPossivelCopiar) >= 0) {
			Modal.confirma({
				btnOkLabel: 'Confirmar',
				titulo: 'Copiar ' + caracterizacaoTexto,
				conteudo: Mensagem.replace(Caracterizacao.settings.mensagens.CopiarConfirmar, "#TEXTO#", caracterizacaoTexto).Texto,
				btnOkCallback: function () { Caracterizacao.copiar(linha); }
			});
		} else {
			Mensagem.gerar(Caracterizacao.container, [Caracterizacao.settings.mensagens.CopiarDadosJaAtualizados]);
		}
	},

	copiar: function (linha) {
		var caracterizacaoTipo = $('.hdnCaracterizacaoTipo', linha).val();

		var objeto = {
			id: Caracterizacao.settings.empreendimentoID,
			projetoDigitalID: Caracterizacao.settings.projetoDigitalID,
			caracterizacaoTipo: caracterizacaoTipo
		};

		MasterPage.carregando(true);
		$.ajax({
			url: Caracterizacao.settings.urls.CopiarDadosInstitucional,
			data: objeto,
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Caracterizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Caracterizacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	confirmarFinalizar: function () {
		if ($('.gridCaractCadastrada', Caracterizacao.container).is(':visible')) {
			Modal.confirma({
				btnOkLabel: 'Confirmar',
				titulo: 'Finalizar Passo 2',
				conteudo: Caracterizacao.settings.mensagens.CadastradasNaoAssociadaConfirmar.Texto,
				btnOkCallback: Caracterizacao.finalizar
			});
		} else {
			Caracterizacao.finalizar();
		}
	},

	finalizar: function () {
		var objeto = {
			id: Caracterizacao.settings.empreendimentoID,
			projetoDigitalID: Caracterizacao.settings.projetoDigitalID
		};

		MasterPage.carregando(true);
		$.ajax({
			url: Caracterizacao.settings.urls.FinalizarPasso,
			data: objeto,
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Caracterizacao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Caracterizacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	listar: function () {
		debugger;
		MasterPage.redireciona($('.hdnUrlListar', $(this).closest('tr')).val() + '/' +
			Caracterizacao.settings.empreendimentoID +
			'?projetoDigitalId=' + Caracterizacao.settings.projetoDigitalID +
			'&retornarVisualizar=' + ($('.btnFinalizarPasso', Caracterizacao.container).length <= 0) +
			'&isVisualizar=' + $('.hdnIsVisualizar').val());
	}
}
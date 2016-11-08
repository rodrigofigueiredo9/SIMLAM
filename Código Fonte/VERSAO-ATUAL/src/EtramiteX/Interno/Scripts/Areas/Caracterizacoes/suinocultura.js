/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="coordenadaAtividade.js" />

Suinocultura = {
	settings: {
		urls: {
			salvar: '',
			editar: '',
			mergiar: '',
			visualizar: ''
		},
		salvarCallBack: null,
		mensagens: {},
		idsTela: null,
		textoAbrirModal: null,
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Suinocultura.settings, options); }
		Suinocultura.container = MasterPage.getContent(container);

		Suinocultura.container.delegate('.btnSalvar', 'click', Suinocultura.salvar);
		Suinocultura.container.delegate('.ddlCoordenadaTipoGeometria', 'change', CoordenadaAtividade.obterDadosCoordenadaAtividade);
		Suinocultura.container.delegate('.ddlAtividade', 'change', Suinocultura.gerenciarAtividade);

		var editar = $('.hdnIsEditar', container).val();
		if (!editar) {
			CoordenadaAtividade.obterDadosTipoGeometria();
			CoordenadaAtividade.obterDadosCoordenadaAtividade();
		}

		CoordenadaAtividade.load(container);
		Suinocultura.gerenciarAtividade();

		if (Suinocultura.settings.textoMerge) {
			Suinocultura.abrirModalRedireciona(Suinocultura.settings.textoMerge, Suinocultura.settings.atualizarDependenciasModalTitulo);
		}
	},

	gerenciarAtividade: function () {

		var atividade = $('.ddlAtividade :selected', Suinocultura.container).val();
		if (atividade == 0) {
			$('.divNumeroMatrizes', Suinocultura.container).addClass('hide');
			$('.divNumeroCabecas', Suinocultura.container).addClass('hide');
		} else {
			if (atividade == Suinocultura.settings.idsTela.SuinoculturaCicloCompleto || atividade == Suinocultura.settings.idsTela.SuinoculturaExclusivoTerminacao || atividade == Suinocultura.settings.idsTela.SuinoculturaComLançamentoEfluenteLiquidos) {
				$('.divNumeroMatrizes', Suinocultura.container).addClass('hide');
				$('.divNumeroCabecas', Suinocultura.container).removeClass('hide');
			}

			if (atividade == Suinocultura.settings.idsTela.SuinoculturaExclusivoProducaoLeitoes) {
				$('.divNumeroCabecas', Suinocultura.container).addClass('hide');
				$('.divNumeroMatrizes', Suinocultura.container).removeClass('hide');
			}

		}

	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', Suinocultura.container).attr('href'));
			},
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: titulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	abrirModalMerge: function (textoModal) {
		Modal.confirma({
			removerFechar: true,
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				MasterPage.carregando(true);
				$.ajax({ url: Suinocultura.settings.urls.mergiar,
					data: JSON.stringify(Suinocultura.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', Suinocultura.container);
						container.empty();
						container.append(response.Html);
						Suinocultura.settings.dependencias = response.Dependencias;
						Suinocultura.gerenciarAtividade();
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: Suinocultura.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	obter: function () {
		var container = Suinocultura.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
			Atividade: $('.ddlAtividade :selected', container).val(),
			Dependencias: JSON.parse(Suinocultura.settings.dependencias),
			Fase: $('.ddlFases :selected', container).val(),
			NumeroCabecas: $('.txtNumeroCabecas', container).val(),
			NumeroMatrizes: $('.txtNumeroMatrizes', container).val(),
			ExisteBiodigestor: $('.rdbExisteBiodigestor:checked', container).val(),
			PossuiFabricaRacao: $('.rdbPossuiFabricaRacao:checked', container).val(),
			CoordenadaAtividade: CoordenadaAtividade.obter()
		};

		if (obj.Atividade == Suinocultura.settings.idsTela.SuinoculturaExclusivoProducaoLeitoes) {
			obj.NumeroCabecas = '';
		} else {
			obj.NumeroMatrizes = '';
		}

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: Suinocultura.settings.urls.salvar,
			data: JSON.stringify(Suinocultura.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Suinocultura.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					Suinocultura.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Suinocultura.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
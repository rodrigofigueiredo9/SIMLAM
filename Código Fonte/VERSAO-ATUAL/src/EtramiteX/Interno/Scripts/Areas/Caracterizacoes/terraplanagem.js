/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="coordenadaAtividade.js" />

Terraplanagem = {
	settings: {
		urls: {
			salvar: '',
			editar: '',
			mergiar: '',
			visualizar: ''
		},
		salvarCallBack: null,
		mensagens: {},
		textoAbrirModal: null,
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Terraplanagem.settings, options); }
		Terraplanagem.container = MasterPage.getContent(container);

		Terraplanagem.container.delegate('.btnSalvar', 'click', Terraplanagem.salvar);
		Terraplanagem.container.delegate('.ddlCoordenadaTipoGeometria', 'change', CoordenadaAtividade.obterDadosCoordenadaAtividade);

		var editar = $('.hdnIsEditar', container).val();
		if (!editar) {
			CoordenadaAtividade.obterDadosTipoGeometria();
			CoordenadaAtividade.obterDadosCoordenadaAtividade();
		}

		CoordenadaAtividade.load(container);

		if (Terraplanagem.settings.textoMerge) {
			Terraplanagem.abrirModalRedireciona(Terraplanagem.settings.textoMerge, Terraplanagem.settings.atualizarDependenciasModalTitulo);
		}
	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', Terraplanagem.container).attr('href'));
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
				$.ajax({ url: Terraplanagem.settings.urls.mergiar,
					data: JSON.stringify(Terraplanagem.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', Terraplanagem.container);
						container.empty();
						container.append(response.Html);
						Terraplanagem.settings.dependencias = response.Dependencias;
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: Terraplanagem.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	obter: function () {
		var container = Terraplanagem.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
			Atividade: $('.ddlAtividade :selected', container).val(),
			Area: $('.txtArea', container).val(),
			VolumeMovimentado: $('.txtVolumeMovimentado', container).val(),
			Dependencias: JSON.parse(Terraplanagem.settings.dependencias),
			CoordenadaAtividade: CoordenadaAtividade.obter()
		};

		return obj;
	},

	tratarStringDecimal: function (strDecimal) {
		return strDecimal.replace(/\.{1}/gi,"");
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: Terraplanagem.settings.urls.salvar,
			data: JSON.stringify(Terraplanagem.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Terraplanagem.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					Terraplanagem.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Terraplanagem.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
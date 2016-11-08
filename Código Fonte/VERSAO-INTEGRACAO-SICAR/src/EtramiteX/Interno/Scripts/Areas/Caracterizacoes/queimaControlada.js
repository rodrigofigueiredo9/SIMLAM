/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />

QueimaControlada = {
	settings: {
		urls: {
			salvar: '',
			mergiar: ''
		},
		mensagens: {},
		textoAbrirModal: null,
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(QueimaControlada.settings, options); }
		QueimaControlada.container = MasterPage.getContent(container);
		QueimaControlada.container.delegate('.btnSalvar', 'click', QueimaControlada.salvar);
		QueimaControladaQueima.load(container);


		if (QueimaControlada.settings.textoMerge) {
			QueimaControlada.abrirModalRedireciona(QueimaControlada.settings.textoMerge, QueimaControlada.settings.atualizarDependenciasModalTitulo);
		}

		if (QueimaControlada.settings.textoAbrirModal) {
			QueimaControlada.abrirModalRedireciona(QueimaControlada.settings.textoAbrirModal, 'Área de Vegetação Nativa em Estágio Desconhecido de Regeneração');
		}
	},

	obter: function () {
		var objeto = {
			Id: $('.hdnCaracterizacaoId', QueimaControlada.container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', QueimaControlada.container).val(),
			Dependencias: JSON.parse(QueimaControlada.settings.dependencias),
			QueimasControladas: QueimaControladaQueima.obter()
		}
		return objeto;
	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', QueimaControlada.container).attr('href'));
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
				$.ajax({ url: QueimaControlada.settings.urls.mergiar,
					data: JSON.stringify(QueimaControlada.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', QueimaControlada.container);
						container.empty();
						container.append(response.Html);
						QueimaControlada.settings.dependencias = response.Dependencias;
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: QueimaControlada.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: QueimaControlada.settings.urls.salvar,
			data: JSON.stringify(QueimaControlada.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, QueimaControlada.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					QueimaControlada.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(QueimaControlada.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}

}
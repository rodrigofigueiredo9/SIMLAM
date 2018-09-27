/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />

ExploracaoFlorestal = {
	settings: {
		urls: {
			salvar: '',
			mergiar: ''
		},
		idsTela: null,
		mensagens: {},
		textoAbrirModal: null,
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null
	},

	load: function (container, options) {
		if (options) { $.extend(ExploracaoFlorestal.settings, options); }
		ExploracaoFlorestal.container = MasterPage.getContent(container);

		ExploracaoFlorestal.container.delegate('.btnSalvar', 'click', ExploracaoFlorestal.salvar);
		ExploracaoFlorestalExploracao.load(container, { idsTela: ExploracaoFlorestal.settings.idsTela });

		if (ExploracaoFlorestal.settings.textoMerge) {
			ExploracaoFlorestal.abrirModalRedireciona(ExploracaoFlorestal.settings.textoMerge, ExploracaoFlorestal.settings.atualizarDependenciasModalTitulo);
		}

		if (ExploracaoFlorestal.settings.textoAbrirModal) {
			ExploracaoFlorestal.abrirModalRedireciona(ExploracaoFlorestal.settings.textoAbrirModal, 'Área de Vegetação Nativa em Estágio Desconhecido de Regeneração');
		}

		if ($('.hdnIsVisualizar').val() == "True") {
			$('.asmConteudoInternoExpander').click();
		}
	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', ExploracaoFlorestal.container).attr('href'));
			},
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				ExploracaoFlorestal.settings.dependencias = null;
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
				$.ajax({
					url: ExploracaoFlorestal.settings.urls.mergiar,
					data: JSON.stringify(ExploracaoFlorestal.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', ExploracaoFlorestal.container);
						container.empty();
						container.append(response.Html);
						ExploracaoFlorestal.settings.dependencias = response.Dependencias;
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: ExploracaoFlorestal.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	obter: function () {
		var objetoList = [];
		$('.localizador', ExploracaoFlorestal.container).each(function () {
			var objeto = {
				Id: $('.hdnCaracterizacaoId', this).val(),
				EmpreendimentoId: $('.hdnEmpreendimentoId', this).val(),
				Dependencias: JSON.parse(ExploracaoFlorestal.settings.dependencias),
				Exploracoes: ExploracaoFlorestalExploracao.obter(this),
				CodigoExploracao: $('.hdnCodigoExploracao', this).val(),
				TipoExploracao: $('.ddlTipoExploracao option:selected', this).val()
			};
			objetoList.push(objeto);
		});

		return objetoList;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: ExploracaoFlorestal.settings.urls.salvar,
			data: JSON.stringify(ExploracaoFlorestal.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ExploracaoFlorestal.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					ExploracaoFlorestal.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ExploracaoFlorestal.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
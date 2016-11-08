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
		ExploracaoFlorestal.container.delegate('.checkboxFinalidadeExploracao', 'change', ExploracaoFlorestal.onChangeFinalidade);

		ExploracaoFlorestal.gerenciarFinalidades();

		ExploracaoFlorestalExploracao.load(container, { idsTela: ExploracaoFlorestal.settings.idsTela });

		if (ExploracaoFlorestal.settings.textoMerge) {
			ExploracaoFlorestal.abrirModalRedireciona(ExploracaoFlorestal.settings.textoMerge, ExploracaoFlorestal.settings.atualizarDependenciasModalTitulo);
		}

		if (ExploracaoFlorestal.settings.textoAbrirModal) {
			ExploracaoFlorestal.abrirModalRedireciona(ExploracaoFlorestal.settings.textoAbrirModal, 'Área de Vegetação Nativa em Estágio Desconhecido de Regeneração');
		}

		$('.exploracoesFlorestais', container).each(function () {
			var container = this;
			if ($('.hdnGeometriaId', container).val() == ExploracaoFlorestal.settings.idsTela.GeometriaTipoPonto) {
				$('.ddlExploracaoTipo option:eq(2)', container).attr('selected', 'selected');
			}
		});
	},

	onChangeFinalidade: function () {
		$('.divEspecificarFinalidade', ExploracaoFlorestal.container).removeClass('hide');
		$('.divEspecificarFinalidade', ExploracaoFlorestal.container).addClass('hide');
		ExploracaoFlorestal.gerenciarFinalidades();
	},

	gerenciarFinalidades: function () {
		if ($('.checkboxOutros:checked', ExploracaoFlorestal.container)[0]) {
			$('.divEspecificarFinalidade', ExploracaoFlorestal.container).removeClass('hide');
		}
	},

	validarFinalidadeEspecificar: function () {
		var container = ExploracaoFlorestal.container;
		if ($('.checkboxOutros:checked', container)[0] && $('.txtFinalidadeEspecificar', container).val() == '') {
			var mensagens = new Array();
			mensagens.push(ExploracaoFlorestal.settings.mensagens.FinalidadeExploracaoEspecificarObrigatorio);
			Mensagem.gerar(container, mensagens);
			return false;
		}
		return true;
	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', ExploracaoFlorestal.container).attr('href'));
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
		var objeto = {
			Id: $('.hdnCaracterizacaoId', ExploracaoFlorestal.container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', ExploracaoFlorestal.container).val(),
			FinalidadeExploracao: 0,
			FinalidadeEspecificar: $('.txtFinalidadeEspecificar', ExploracaoFlorestal.container).val(),
			Dependencias: JSON.parse(ExploracaoFlorestal.settings.dependencias),
			Exploracoes: ExploracaoFlorestalExploracao.obter()
		}

		$('.checkboxFinalidadeExploracao:checked', ExploracaoFlorestal.container).each(function () {
			objeto.FinalidadeExploracao += parseInt($(this).val());
		});

		return objeto;
	},

	salvar: function () {
		if (ExploracaoFlorestal.validarFinalidadeEspecificar()) {
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
}
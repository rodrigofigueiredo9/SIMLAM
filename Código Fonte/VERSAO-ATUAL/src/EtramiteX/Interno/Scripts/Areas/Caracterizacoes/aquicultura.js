/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="aquiculturaAquicult.js" />

Aquicultura = {
	settings: {
		urls: {
			salvar: '',
			editar: '',
			mergiar: '',
			visualizar: '',
			obterTemplate: ''
		},
		idsTela: null,
        isVisualizar: false,
		salvarCallBack: null,
		mensagens: {},
		textoAbrirModal: null,
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Aquicultura.settings, options); }
		Aquicultura.container = MasterPage.getContent(container);

		Aquicultura.container.delegate('.btnSalvar', 'click', Aquicultura.salvar);
		Aquicultura.container.delegate('.btnAdicionarAtividade', 'click', Aquicultura.adicionarAtividade);
		Aquicultura.container.delegate('.btnExluirAtividade', 'click', Aquicultura.removerAtividade);

		Aquicultura.gerenciarButtonAdicionarAtividade();

		if (Aquicultura.settings.textoMerge) {
			Aquicultura.abrirModalRedireciona(Aquicultura.settings.textoMerge, Aquicultura.settings.atualizarDependenciasModalTitulo);
		}

		if (Aquicultura.settings.isVisualizar) {
		    return;
		}

		AquiculturaAquicult.load(container);
	},

	adicionarAtividade: function () {

		$.ajax({
			url: Aquicultura.settings.urls.obterTemplate,
			data: null,
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Aquicultura.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.html) {
					$('.containerAquicultura', Aquicultura.container).append(response.html);
				}
			}
		});

		Aquicultura.loadComponentes();

	},

	loadComponentes: function () {
		Aquicultura.gerenciarButtonAdicionarAtividade();
		$('.fsAquicultura').each(function () {
			AquiculturaAquicult.gerenciarAtividade(null, this);
			$(".maskDecimal", this).maskMoney();
		});
		MasterPage.botoes();
	},

	gerenciarButtonAdicionarAtividade: function () {
		var cont = 0;
		$('.fsAquicultura', Aquicultura.container).each(function () {
			cont++;
		});

		if (cont >= $('.hdnQtdAtividade').val()) {
			$('.divAdicionarAtividade', Aquicultura.container).addClass('hide');
		} else {
			$('.divAdicionarAtividade', Aquicultura.container).removeClass('hide');
		}
	},

	removerAtividade: function () {
		var container = $(this).closest('fieldset');
		var cont = 0;
		$('.fsAquicultura', Aquicultura.container).each(function () {
			cont++;
		});

		if (cont <= 1) {
			var mensagens = [];
			mensagens.push(jQuery.extend(true, {}, AquiculturaAquicult.settings.mensagens.ListaAtividadeObrigatoria));
			Mensagem.gerar(CoordenadaAtividade.container, mensagens);
		} else {
			$(container).remove();
		}

		Aquicultura.gerenciarButtonAdicionarAtividade();

	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', Aquicultura.container).attr('href'));
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
				$.ajax({ url: Aquicultura.settings.urls.mergiar,
					data: JSON.stringify(Aquicultura.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', Aquicultura.container);
						container.empty();
						container.append(response.Html);
						Aquicultura.settings.dependencias = response.Dependencias;
						Aquicultura.loadComponentes();
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: Aquicultura.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	obter: function () {
		var container = Aquicultura.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
			AquiculturasAquicult: AquiculturaAquicult.obter(),
			Dependencias: JSON.parse(Aquicultura.settings.dependencias)
		};

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: Aquicultura.settings.urls.salvar,
			data: JSON.stringify(Aquicultura.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Aquicultura.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					Aquicultura.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Aquicultura.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
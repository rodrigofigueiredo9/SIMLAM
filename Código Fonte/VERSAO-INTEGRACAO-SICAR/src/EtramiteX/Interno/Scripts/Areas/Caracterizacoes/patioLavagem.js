/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="coordenadaAtividade.js" />

PatioLavagem = {
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
		if (options) { $.extend(PatioLavagem.settings, options); }
		PatioLavagem.container = MasterPage.getContent(container);

		PatioLavagem.container.delegate('.btnSalvar', 'click', PatioLavagem.salvar);
		PatioLavagem.container.delegate('.ddlCoordenadaTipoGeometria', 'change', CoordenadaAtividade.obterDadosCoordenadaAtividade);

		var editar = $('.hdnIsEditar', container).val();
		if (!editar) {
			CoordenadaAtividade.obterDadosTipoGeometria();
			CoordenadaAtividade.obterDadosCoordenadaAtividade();
		}

		CoordenadaAtividade.load(container);

		if (PatioLavagem.settings.textoMerge) {
			PatioLavagem.abrirModalRedireciona(PatioLavagem.settings.textoMerge, PatioLavagem.settings.atualizarDependenciasModalTitulo);
		}
	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', PatioLavagem.container).attr('href'));
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

	obter: function () {
		var container = PatioLavagem.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
			Atividade: $('.ddlAtividade :selected', container).val(),
			AreaTotal: Mascara.getFloatMask($('.txtAreaTotal', container).val()),
			Dependencias: JSON.parse(PatioLavagem.settings.dependencias),
			CoordenadaAtividade: CoordenadaAtividade.obter()
		};

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: PatioLavagem.settings.urls.salvar,
			data: JSON.stringify(PatioLavagem.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, PatioLavagem.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					PatioLavagem.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(PatioLavagem.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
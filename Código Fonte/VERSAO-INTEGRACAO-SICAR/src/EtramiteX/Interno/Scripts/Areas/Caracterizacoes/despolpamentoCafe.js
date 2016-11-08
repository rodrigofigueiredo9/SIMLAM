/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="coordenadaAtividade.js" />

DespolpamentoCafe = {
	settings: {
		urls: {
			salvar: '',
			editar: '',
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
		if (options) { $.extend(DespolpamentoCafe.settings, options); }
		DespolpamentoCafe.container = MasterPage.getContent(container);

		DespolpamentoCafe.container.delegate('.btnSalvar', 'click', DespolpamentoCafe.salvar);
		DespolpamentoCafe.container.delegate('.ddlCoordenadaTipoGeometria', 'change', CoordenadaAtividade.obterDadosCoordenadaAtividade);

		var editar = $('.hdnIsEditar', container).val();
		if (!editar) {
			CoordenadaAtividade.obterDadosTipoGeometria();
			CoordenadaAtividade.obterDadosCoordenadaAtividade();
		}

		CoordenadaAtividade.load(container);

		if (DespolpamentoCafe.settings.textoMerge) {
			DespolpamentoCafe.abrirModalRedireciona(DespolpamentoCafe.settings.textoMerge, DespolpamentoCafe.settings.atualizarDependenciasModalTitulo);
		}
	},

	obter: function () {
		var container = DespolpamentoCafe.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
			Atividade: $('.ddlAtividade :selected', container).val(),
			Dependencias: JSON.parse(DespolpamentoCafe.settings.dependencias),
			CapacidadeTotalInstalada: $('.txtCapacidadeTotalInstalada', container).val(),
			AguaResiduariaCafe: $('.txtAguaResiduariaCafe', container).val(),
			Dependencias: JSON.parse(DespolpamentoCafe.settings.dependencias),
			CoordenadaAtividade: CoordenadaAtividade.obter()
		};

		return obj;

	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', DespolpamentoCafe.container).attr('href'));
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
				$.ajax({ url: DespolpamentoCafe.settings.urls.mergiar,
					data: JSON.stringify(DespolpamentoCafe.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', DespolpamentoCafe.container);
						container.empty();
						container.append(response.Html);
						DespolpamentoCafe.settings.dependencias = response.Dependencias;
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: DespolpamentoCafe.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});

		Listar.atualizarEstiloTable(DespolpamentoCafe.container.find('.dataGridTable'));
	},


	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: DespolpamentoCafe.settings.urls.salvar,
			data: JSON.stringify(DespolpamentoCafe.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, DespolpamentoCafe.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					DespolpamentoCafe.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(DespolpamentoCafe.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
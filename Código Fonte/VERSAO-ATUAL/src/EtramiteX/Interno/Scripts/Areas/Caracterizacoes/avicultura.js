/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="coordenadaAtividade.js" />

Avicultura = {
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
		if (options) { $.extend(Avicultura.settings, options); }
		Avicultura.container = MasterPage.getContent(container);

		Avicultura.container.delegate('.btnSalvar', 'click', Avicultura.salvar);
		Avicultura.container.delegate('.btnAdicionarConfinamento', 'click', Avicultura.adicionarConfinamento);
		Avicultura.container.delegate('.btnExcluirConfinamento', 'click', Avicultura.excluirConfinamento);
		Avicultura.container.delegate('.ddlCoordenadaTipoGeometria', 'change', CoordenadaAtividade.obterDadosCoordenadaAtividade);

		Avicultura.atualizarAreaTotal();

		var editar = $('.hdnIsEditar', container).val();
		if (!editar) {
			CoordenadaAtividade.obterDadosTipoGeometria();
			CoordenadaAtividade.obterDadosCoordenadaAtividade();
		}

		CoordenadaAtividade.load(container);

		if (Avicultura.settings.textoMerge) {
			Avicultura.abrirModalRedireciona(Avicultura.settings.textoMerge, Avicultura.settings.atualizarDependenciasModalTitulo);
		}
	},

	atualizarAreaTotal: function () {
		var container = $('.divConfinamentos');
		var total = 0;
		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var objeto = (JSON.parse(obj));
				total += Number(objeto.Area.toString().replace(',', '.'));
			}
		});

		$('.txtAreaTotalConfinamento', container).val(total.toString().replace('.', ','));

	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', Avicultura.container).attr('href'));
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
				$.ajax({ url: Avicultura.settings.urls.mergiar,
					data: JSON.stringify(Avicultura.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', Avicultura.container);
						container.empty();
						container.append(response.Html);
						Avicultura.settings.dependencias = response.Dependencias;
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: Avicultura.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	adicionarConfinamento: function () {
		var mensagens = new Array();
		Mensagem.limpar(Avicultura.container);
		var container = $('.divConfinamentos');

		var confinamento = {
			Id: 0,
			Tid: '',
			Finalidade: $('.ddlConfinamentoFinalidades :selected', container).val(),
			FinalidadeTexto: $('.ddlConfinamentoFinalidades :selected', container).text(),
			Area: $('.txtConfinamentoArea', container).val().replace('.', '').replace(',', '.')
		}

		if (confinamento.Finalidade == 0) {
			mensagens.push(jQuery.extend(true, {}, Avicultura.settings.mensagens.ConfinamentoFinalidadeObrigatorio));
		}

		if (confinamento.Area == '') {
			mensagens.push(jQuery.extend(true, {}, Avicultura.settings.mensagens.ConfinamentoAreaObrigatoria));
		} else {
			if (isNaN(confinamento.Area)) {
				mensagens.push(jQuery.extend(true, {}, Avicultura.settings.mensagens.ConfinamentoAreaInvalida));
			}
			if (Number(confinamento.Area) <= 0) {
				mensagens.push(jQuery.extend(true, {}, Avicultura.settings.mensagens.ConfinamentoAreaMaiorZero));
			}
		}



		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var conf = (JSON.parse(obj));
				if (conf.FinalidadeTexto == confinamento.FinalidadeTexto) {
					mensagens.push(jQuery.extend(true, {}, Avicultura.settings.mensagens.ConfinamentoFinalidadeDuplicada));
				}
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(Avicultura.container, mensagens);
			return;
		}

		confinamento.Area = confinamento.Area.replace(".", ",");

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(confinamento));
		linha.find('.finalidade').html(confinamento.FinalidadeTexto).attr('title', confinamento.FinalidadeTexto);
		linha.find('.area').html(confinamento.Area).attr('title', confinamento.Area);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.txtConfinamentoArea', container).val('');
		$('.ddlConfinamentoFinalidades', container).ddlFirst();

		Avicultura.atualizarAreaTotal();

	},

	excluirConfinamento: function () {
		var container = $('.divConfinamentos');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
		Avicultura.atualizarAreaTotal();
	},

	obter: function () {
		var container = Avicultura.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
			Atividade: $('.ddlAtividade :selected', container).val(),
			Dependencias: JSON.parse(Avicultura.settings.dependencias),
			CoordenadaAtividade: CoordenadaAtividade.obter(),
			Confinamentos: []
		};

		//Confinamentos
		container = Avicultura.container.find('.divConfinamentos');
		$('.hdnItemJSon', container).each(function () {
			var objConfinamento = String($(this).val());
			if (objConfinamento != '') {
				obj.Confinamentos.push(JSON.parse(objConfinamento));
			}
		});

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: Avicultura.settings.urls.salvar,
			data: JSON.stringify(Avicultura.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Avicultura.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					Avicultura.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Avicultura.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
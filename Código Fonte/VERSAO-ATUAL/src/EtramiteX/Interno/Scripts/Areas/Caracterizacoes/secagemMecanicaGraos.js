/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="coordenadaAtividade.js" />
/// <reference path="materiaPrimaFlorestalConsumida.js" />

SecagemMecanicaGraos = {
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
		if (options) { $.extend(SecagemMecanicaGraos.settings, options); }
		SecagemMecanicaGraos.container = MasterPage.getContent(container);

		SecagemMecanicaGraos.container.delegate('.btnSalvar', 'click', SecagemMecanicaGraos.salvar);
		SecagemMecanicaGraos.container.delegate('.btnAdicionarSecador', 'click', SecagemMecanicaGraos.adicionarSecador);
		SecagemMecanicaGraos.container.delegate('.btnExcluirSecador', 'click', SecagemMecanicaGraos.excluirSecador);
		SecagemMecanicaGraos.container.delegate('.txtNumSecadores', 'blur', SecagemMecanicaGraos.gerenciarNumeroSecadores);
		SecagemMecanicaGraos.container.delegate('.ddlCoordenadaTipoGeometria', 'change', CoordenadaAtividade.obterDadosCoordenadaAtividade);

		var editar = $('.hdnIsEditar', container).val();
		if (!editar) {
			CoordenadaAtividade.obterDadosTipoGeometria();
			CoordenadaAtividade.obterDadosCoordenadaAtividade();
		}

		CoordenadaAtividade.load(container);
		MateriaPrimaFlorestalConsumida.load(container);

		if (SecagemMecanicaGraos.settings.textoMerge) {
			SecagemMecanicaGraos.abrirModalRedireciona(SecagemMecanicaGraos.settings.textoMerge, SecagemMecanicaGraos.settings.atualizarDependenciasModalTitulo);
		}
	},

	gerenciarNumeroSecadores: function () {
		var qtdSecadores = $('.txtNumSecadores', SecagemMecanicaGraos.container).val();
		qtdSecadores = !isNaN(qtdSecadores) ? Number(qtdSecadores) : 0;
		var container = SecagemMecanicaGraos.container.find('.divSecadores');
		var qtdSecadoresAdicionados = 0;

		$('.hdnItemJSon', container).each(function () {
			if ($(this).val() != '') {
				qtdSecadoresAdicionados++;
			}
		});

		if (qtdSecadoresAdicionados <= 0) {
			if (qtdSecadores > 0) {
				$('.txtIdentificador', container).val(1);
				$('.txtCapacidade', container).focus();
			} else {
				$('.txtIdentificador', container).val('');
			}
		}
	},

	atualizarCapacidadeTotal: function () {
		var container = $('.divSecadores');
		var total = 0;
		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var objeto = (JSON.parse(obj));
				total += Number(objeto.Capacidade.toString().replace(',', '.'));
			}
		});

		$('.txtCapacidadeTotalSecadores', container).val(total.toString().replace('.', ','));

	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', SecagemMecanicaGraos.container).attr('href'));
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
				$.ajax({ url: SecagemMecanicaGraos.settings.urls.mergiar,
					data: JSON.stringify(SecagemMecanicaGraos.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', SecagemMecanicaGraos.container);
						container.empty();
						container.append(response.Html);
						SecagemMecanicaGraos.settings.dependencias = response.Dependencias;
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: SecagemMecanicaGraos.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	adicionarSecador: function () {
		var mensagens = new Array();
		Mensagem.limpar(SecagemMecanicaGraos.container);
		var container = $('.divSecadores');
		var qtdSecadores = $('.txtNumSecadores', SecagemMecanicaGraos.container).val();

		if (qtdSecadores == '') {
			mensagens.push(jQuery.extend(true, {}, SecagemMecanicaGraos.settings.mensagens.NumeroSecadorObrigatorio));
			Mensagem.gerar(SecagemMecanicaGraos.container, mensagens);
			return;

		}

		qtdSecadores = !isNaN(qtdSecadores) ? Number(qtdSecadores) : 0;

		if (qtdSecadores <= 0) {
			mensagens.push(jQuery.extend(true, {}, SecagemMecanicaGraos.settings.mensagens.NumeroSecadorMaiorZero));
			Mensagem.gerar(SecagemMecanicaGraos.container, mensagens);
			return;
		}

		var identificador = $('.txtIdentificador', container).val();
		identificador = !isNaN(identificador) ? Number(identificador) : 0;

		if (identificador == 0) identificador++;

		if (identificador > qtdSecadores) {
			mensagens.push(jQuery.extend(true, {}, SecagemMecanicaGraos.settings.mensagens.SecadoresJaAdicionados));
			Mensagem.gerar(SecagemMecanicaGraos.container, mensagens);
			return;
		}

		var secador = {
			Id: 0,
			Tid: '',
			Identificador: identificador,
			Capacidade: $('.txtCapacidade', container).val().replace('.', '').replace(',', '.')
		}

		if (secador.Capacidade == '') {
			mensagens.push(jQuery.extend(true, {}, SecagemMecanicaGraos.settings.mensagens.CapacidadeSecadorObrigatorio));
			Mensagem.gerar(SecagemMecanicaGraos.container, mensagens);
			return;
		} else {
			if (isNaN(secador.Capacidade)) {
				mensagens.push(jQuery.extend(true, {}, SecagemMecanicaGraos.settings.mensagens.CapacidadeSecadorInvalido));
				Mensagem.gerar(SecagemMecanicaGraos.container, mensagens);
				return;
			}
			if (Number(secador.Capacidade) <= 0) {
				mensagens.push(jQuery.extend(true, {}, SecagemMecanicaGraos.settings.mensagens.CapacidadeSecadorMaiorZero));
				Mensagem.gerar(SecagemMecanicaGraos.container, mensagens);
				return;
			}
		}

		identificador++;

		secador.Capacidade = secador.Capacidade.replace(".", ",");

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(secador));
		linha.find('.identificador').html(secador.Identificador).attr('title', secador.Identificador);
		linha.find('.capacidadeSecador').html(secador.Capacidade).attr('title', secador.Capacidade);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.txtCapacidade', container).val('');
		$('.txtIdentificador', container).val(identificador);

		//colocando foco no campo capacidade enquanto pode adicionar secadores
		if (identificador != qtdSecadores + 1) {
			$('.txtCapacidade', container).focus();
		}

		SecagemMecanicaGraos.atualizarCapacidadeTotal();

	},

	excluirSecador: function () {
		var container = $('.divSecadores');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		var identificador = $('.txtIdentificador', container).val();
		identificador = !isNaN(identificador) ? Number(identificador) : 0;
		identificador--;
		$('.txtIdentificador', container).val(identificador);

		var cont = 1;
		$('.hdnItemJSon', container).each(function (i, item) {
			var objSecador = String($(item).val());
			if (objSecador != '') {
				var obj = JSON.parse(objSecador);
				obj.Identificador = cont;
				linha = $(item).closest('tr');
				linha.find('.hdnItemJSon').val(JSON.stringify(obj));
				linha.find('.identificador').html(obj.Identificador).attr('title', obj.Identificador);
				cont++;
			}
		});

		SecagemMecanicaGraos.atualizarCapacidadeTotal();
	},

	obter: function () {
		var container = SecagemMecanicaGraos.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
			Atividade: $('.ddlAtividade :selected', container).val(),
			Dependencias: JSON.parse(SecagemMecanicaGraos.settings.dependencias),
			NumeroSecadores: $('.txtNumSecadores', container).val(),
			CoordenadaAtividade: CoordenadaAtividade.obter(),
			Secadores: [],
			MateriasPrimasFlorestais: MateriaPrimaFlorestalConsumida.obter()
		};

		//Secadores
		container = SecagemMecanicaGraos.container.find('.divSecadores');
		$('.hdnItemJSon', container).each(function () {
			var objSecador = String($(this).val());
			if (objSecador != '') {
				obj.Secadores.push(JSON.parse(objSecador));
			}
		});

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: SecagemMecanicaGraos.settings.urls.salvar,
			data: JSON.stringify(SecagemMecanicaGraos.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, SecagemMecanicaGraos.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					SecagemMecanicaGraos.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(SecagemMecanicaGraos.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="beneficiamentoMadeiraBeneficiamento.js" />

BeneficiamentoMadeira = {
	settings: {
		urls: {
			salvar: '',
			editar: '',
			mergiar: '',
			visualizar: '',
			obterTemplate: ''
		},
		idsTela: null,
		salvarCallBack: null,
		mensagens: {},
		textoAbrirModal: null,
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(BeneficiamentoMadeira.settings, options); }
		BeneficiamentoMadeira.container = MasterPage.getContent(container);

		BeneficiamentoMadeira.container.delegate('.btnSalvar', 'click', BeneficiamentoMadeira.salvar);
		BeneficiamentoMadeira.container.delegate('.btnAdicionarAtividade', 'click', BeneficiamentoMadeira.adicionarAtividade);
		BeneficiamentoMadeira.container.delegate('.btnExluirAtividade', 'click', BeneficiamentoMadeira.removerAtividade);

		BeneficiamentoMadeira.gerenciarButtonAdicionarAtividade();

		if (BeneficiamentoMadeira.settings.textoMerge) {
			BeneficiamentoMadeira.abrirModalRedireciona(BeneficiamentoMadeira.settings.textoMerge, BeneficiamentoMadeira.settings.atualizarDependenciasModalTitulo);
		}

		BeneficiamentoMadeiraBeneficiamento.load(container);
	},

	adicionarAtividade: function () {

		$.ajax({
			url: BeneficiamentoMadeira.settings.urls.obterTemplate,
			data: null,
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(BeneficiamentoMadeira.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.html) {
					$('.containerBeneficiamento', BeneficiamentoMadeira.container).append(response.html);
				}
			}
		});

		BeneficiamentoMadeira.loadComponentes();

	},

	loadComponentes: function () {
		BeneficiamentoMadeira.gerenciarButtonAdicionarAtividade();
		$('.fsBeneficiamento').each(function () {
			BeneficiamentoMadeiraBeneficiamento.gerenciarAtividade(null, this);
			$(".maskDecimal", this).maskMoney();
		});
		MasterPage.botoes();
	},

	gerenciarButtonAdicionarAtividade: function () {
		var cont = 0;
		$('.fsBeneficiamento', BeneficiamentoMadeira.container).each(function () {
			cont++;
		});

		if (cont >= $('.hdnQtdAtividade').val()) {
			$('.divAdicionarAtividade', BeneficiamentoMadeira.container).addClass('hide');
		} else {
			$('.divAdicionarAtividade', BeneficiamentoMadeira.container).removeClass('hide');
		}
	},

	removerAtividade: function () {
		var container = $(this).closest('fieldset');
		var cont = 0;
		$('.fsBeneficiamento', BeneficiamentoMadeira.container).each(function () {
			cont++;
		});

		if (cont <= 1) {
			var mensagens = [];
			mensagens.push(jQuery.extend(true, {}, BeneficiamentoMadeiraBeneficiamento.settings.mensagens.ListaAtividadeObrigatoria));
			Mensagem.gerar(CoordenadaAtividade.container, mensagens);
		} else {
			$(container).remove();
		}

		BeneficiamentoMadeira.gerenciarButtonAdicionarAtividade();

	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', BeneficiamentoMadeira.container).attr('href'));
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
				$.ajax({ url: BeneficiamentoMadeira.settings.urls.mergiar,
					data: JSON.stringify(BeneficiamentoMadeira.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', BeneficiamentoMadeira.container);
						container.empty();
						container.append(response.Html);
						BeneficiamentoMadeira.settings.dependencias = response.Dependencias;
						BeneficiamentoMadeira.loadComponentes();
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: BeneficiamentoMadeira.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	obter: function () {
		var container = BeneficiamentoMadeira.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
			Beneficiamentos: BeneficiamentoMadeiraBeneficiamento.obter(),
			Dependencias: JSON.parse(BeneficiamentoMadeira.settings.dependencias)
		};

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: BeneficiamentoMadeira.settings.urls.salvar,
			data: JSON.stringify(BeneficiamentoMadeira.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, BeneficiamentoMadeira.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					BeneficiamentoMadeira.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(BeneficiamentoMadeira.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
MateriaPrimaFlorestalConsumida = {
	settings: {
		urls: {
			salvar: ''
		},
		mensagens: {}
	},
	container: null,

	load: function (container, options) {

		if (options) { $.extend(MateriaPrimaFlorestalConsumida.settings, options); }

		MateriaPrimaFlorestalConsumida.container = MasterPage.getContent(container);
		MateriaPrimaFlorestalConsumida.container.delegate('.btnAdicionarMateria', 'click', MateriaPrimaFlorestalConsumida.adicionarMateria);
		MateriaPrimaFlorestalConsumida.container.delegate('.btnExcluirMateria', 'click', MateriaPrimaFlorestalConsumida.excluirMateria);
		MateriaPrimaFlorestalConsumida.container.delegate('.ddlMateriaPrima', 'change', MateriaPrimaFlorestalConsumida.gerenciarMateriaPrimaOutros);
	},

	gerenciarMateriaPrimaOutros: function (e, container) {
		if(!container) container = $(this).closest('fieldset');

		var materia = $('.ddlMateriaPrima :selected', container).text().trim().toLowerCase();
		if (materia == 'outros' || materia == 'outras' || materia == 'outro' || materia == 'outra') {
			$('.divEspecificar', container).removeClass('hide');
		} else {
			$('.divEspecificar', container).addClass('hide');
		}

	},

	obter: function (container, associarMultiplo) {
		if (!associarMultiplo) {
			container = MateriaPrimaFlorestalConsumida.container.find('.divMateriasPrima');
		} else {
			if (!container) {
				container = $(this).closest('fieldset');
			}
		}

		var obj = [];
		$('.hdnItemJSon', container).each(function () {
			var objMateria = String($(this).val());
			if (objMateria != '') {
				obj.push(JSON.parse(objMateria));
			}
		});

		return obj;
	},

	adicionarMateria: function () {
		var mensagens = new Array();
		Mensagem.limpar(MateriaPrimaFlorestalConsumida.container);
		var container = $(this).closest('fieldset');

		var sufixo = $('.hdnIdentificador', $(container).closest('.fsBeneficiamento')).val();

		var materia = {
			Id: 0,
			Tid: '',
			MateriaPrimaConsumida: $('.ddlMateriaPrima :selected', container).val(),
			MateriaPrimaConsumidaTexto: $('.ddlMateriaPrima :selected', container).text(),
			Unidade: $('.ddlUnidade :selected', container).val(),
			UnidadeTexto: $('.ddlUnidade :selected', container).text(),
			EspecificarTexto: $('.txtEspecificar', container).val(),
			Quantidade: $('.txtQuantidade', container).val().replace('.', '').replace(',', '.')
		}

		if (materia.MateriaPrimaConsumida <= 0) {
			mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.MateriaPrimaFlorestalConsumidaObrigatoria));
		}

		if (materia.Unidade <= 0) {
			mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.UnidadeMateriaPrimaObrigatoria));
		}

		if (materia.Quantidade == '') {
			mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.QuantidadeMateriaPrimaObrigatoria));
		} else {

			if (isNaN(materia.Quantidade)) {
				mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.QuantidadeMateriaPrimaInvalida));
			}

			if (materia.Quantidade <= 0) {
				mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.QuantidadeMateriaPrimaMaiorZero));
			}
		}

		var materiatext = $('.ddlMateriaPrima :selected', container).text().trim().toLowerCase();
		if (materiatext == 'outros' || materiatext == 'outras' || materiatext == 'outro' || materiatext == 'outra') {

			if (materia.EspecificarTexto == '') {
				mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.EspecificarMateriaPrimaObrigatorio));
			} else {
				materia.MateriaPrimaConsumidaTexto = materia.EspecificarTexto;
			}
		}

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var mat = (JSON.parse(obj));
				if (mat.MateriaPrimaConsumidaTexto == materia.MateriaPrimaConsumidaTexto) {
					mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.MateriaPrimaFlorestalConsumidaDuplicada));
					Mensagem.gerar(MateriaPrimaFlorestalConsumida.container, mensagens);
					return;
				}
			}
		});

		if (mensagens.length > 0) {
			$(mensagens).each(function () {
				this.Campo += sufixo;
			});
			Mensagem.gerar(MateriaPrimaFlorestalConsumida.container, mensagens);
			return;
		}

		materia.Quantidade = materia.Quantidade.replace(".", ",");

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(materia));
		linha.find('.materiaPrimaConsumida').html(materia.MateriaPrimaConsumidaTexto).attr('title', materia.MateriaPrimaConsumidaTexto);
		linha.find('.unidade').html(materia.UnidadeTexto).attr('title', materia.UnidadeTexto);
		linha.find('.quantidade').html(materia.Quantidade).attr('title', materia.Quantidade);

		$('.dataGridTable tbody:last', container).append(linha);
		$('.txtQuantidade', container).val('');
		$('.txtEspecificar', container).val('');
		$('.ddlMateriaPrima', container).ddlFirst();
		MateriaPrimaFlorestalConsumida.gerenciarMateriaPrimaOutros(null, container);

		Listar.atualizarEstiloTable(MateriaPrimaFlorestalConsumida.container.find('.dataGridTable'));
	},

	excluirMateria: function () {
		var container = $(this).closest('fieldset');

		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	}
}
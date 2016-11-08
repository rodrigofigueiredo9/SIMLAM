/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="silviculturaSilvicult.js" />

Silvicultura = {
	settings: {
		urls: {
			salvar: '',
			mergiar: ''
		},
		mensagens: {},
		textoAbrirModal: null,
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null,
		temARL: false,
		temARLDesconhecida: false
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Silvicultura.settings, options); }
		Silvicultura.container = MasterPage.getContent(container);
		Silvicultura.container.delegate('.btnSalvar', 'click', Silvicultura.salvar);
		SilviculturaSilvicult.load(container);


		if (Silvicultura.settings.textoMerge) {
			Silvicultura.abrirModalRedireciona(Silvicultura.settings.textoMerge, Silvicultura.settings.atualizarDependenciasModalTitulo);
		} else {
			Silvicultura.abrirModalARL();
		}

		if (Silvicultura.settings.textoAbrirModal) {
			Silvicultura.abrirModalRedireciona(Silvicultura.settings.textoAbrirModal, 'Área de Vegetação Nativa em Estágio Desconhecido de Regeneração');
		}

		Silvicultura.agruparCulturas();
	},

	abrirModalARL: function () {

		var fnModal = function (texto) {
			Modal.confirma({
				removerFechar: true,
				btnOkLabel: 'Confirmar',
				btnOkCallback: function (conteudoModal) {
					Modal.fechar(conteudoModal);
				},
				btCancelLabel: 'Cancelar',
				btnCancelCallback: function (conteudoModal) {
					MasterPage.redireciona($('.linkCancelar', Silvicultura.container).attr('href'));
				},
				conteudo: texto,
				titulo: 'Área de Reserva Legal',
				tamanhoModal: Modal.tamanhoModalMedia
			});
		};

		if (!Silvicultura.settings.temARL) {
			fnModal(Silvicultura.settings.mensagens.SemARLConfirm.Texto);
		} else if (Silvicultura.settings.temARLDesconhecida) {
			fnModal(Silvicultura.settings.mensagens.ARLDesconhecidaConfirm.Texto);
		}
	},

	agruparCulturas: function () {
		var container = Silvicultura.container;

		var cults = [];
		var silviculturas = SilviculturaSilvicult.obter();

		var culturas = [];
		$(silviculturas).each(function () {
			$(this.Culturas).each(function (i, item) {
				culturas.push(item);
			});
		});

		var podeAdicionar = false;
		var culturasPercorridas = [];

		function contains(str) {
			for (var i = 0; i < culturasPercorridas.length; i++) {
				if (culturasPercorridas[i] == str) {
					return true;
				}
			}
			return false;
		}

		for (var i = 0; i < culturas.length; i++) {
			var NomeCultura = culturas[i].CulturaTipoTexto;

			culturaSelecionadaAux = {
				Id: 0,
				CulturaTipo: culturas[i].CulturaTipo,
				CulturaTipoTexto: culturas[i].CulturaTipoTexto,
				Especificar: culturas[i].Especificar,
				AreaCulturaHa: 0
			}

			for (var j = 0; j < culturas.length; j++) {
				if (NomeCultura == culturas[j].CulturaTipoTexto && !contains(NomeCultura)) {
					culturaSelecionadaAux.AreaCulturaHa += culturas[j].AreaCulturaHa;
					podeAdicionar = true;
				}
			}
			culturasPercorridas.push(NomeCultura);

			if (podeAdicionar) {
				cults.push(culturaSelecionadaAux);
			}

			podeAdicionar = false;
		}

		//Limpando tabela
		$('.groupCulturas', container).each(function () {
			$(this).remove();
		});

		$(cults).each(function (i, cultura) {
			var linha = $('.trTemplateRowCulturas', container).clone().removeClass('trTemplateRowCulturas hide');
			linha.find('.CulturaTipo').html(cultura.CulturaTipoTexto).attr('title', cultura.CulturaTipoTexto);
			linha.find('.AreaCultura').html(Globalize.format(cultura.AreaCulturaHa, "n4")).attr('title', Globalize.format(cultura.AreaCulturaHa, "n4"));
			linha.addClass('groupCulturas');
			$('.tableCulturas tbody:last', container).append(linha);
		});

		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		return cults;
	},

	obter: function () {
		var objeto = {
			Id: $('.hdnCaracterizacaoId', Silvicultura.container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', Silvicultura.container).val(),
			Dependencias: JSON.parse(Silvicultura.settings.dependencias),
			Areas: Silvicultura.obterAreas(),
			Silviculturas: SilviculturaSilvicult.obter()
		}


		return objeto;
	},

	obterAreas: function () {26.65
		var areas = [];
		$('.divArea', Silvicultura.container).each(function () {
			var obj = {
				Id: 0,
				Tipo: 0,
				TipoTexto: '',
				Valor: 0
			};
			obj.Id = $('.hdnAreaId', this).val();
			obj.Tipo = $('.hdnAreaTipo', this).val();
			obj.TipoTexto = $('.hdnAreaTipo', this).text();
			obj.Valor = Number($('.hdnAreaValor', this).val()) || 0;
			obj.ValorTexto = $('.txtArea', this).val();

			areas.push(obj);
		});

		return areas;
	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', Silvicultura.container).attr('href'));
			},
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				Modal.fechar(conteudoModal);

				Silvicultura.abrirModalARL();
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
				$.ajax({ url: Silvicultura.settings.urls.mergiar,
					data: JSON.stringify(Silvicultura.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', Silvicultura.container);
						container.empty();
						container.append(response.Html);
						Silvicultura.settings.dependencias = response.Dependencias;
						Silvicultura.agruparCulturas();
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: Silvicultura.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: Silvicultura.settings.urls.salvar,
			data: JSON.stringify(Silvicultura.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Silvicultura.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					Silvicultura.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Silvicultura.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}

}
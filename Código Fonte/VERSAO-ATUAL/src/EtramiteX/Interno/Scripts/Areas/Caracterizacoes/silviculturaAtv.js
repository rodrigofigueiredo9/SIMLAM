/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="SilviculturaATVCaracteristica.js" />

SilviculturaATV = {
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
		if (options) { $.extend(SilviculturaATV.settings, options); }
		SilviculturaATV.container = MasterPage.getContent(container);
		SilviculturaATV.container.delegate('.btnSalvar', 'click', SilviculturaATV.salvar);
		SilviculturaATV.container.delegate('.keyCalcular', 'focusout', SilviculturaATV.onCalcular);		
		
		SilviculturaATVCaracteristica.load(container);

		if (SilviculturaATV.settings.textoMerge) {
			SilviculturaATV.abrirModalRedireciona(SilviculturaATV.settings.textoMerge, SilviculturaATV.settings.atualizarDependenciasModalTitulo);
		}

		if (SilviculturaATV.settings.textoAbrirModal) {
			SilviculturaATV.abrirModalRedireciona(SilviculturaATV.settings.textoAbrirModal, 'Área de Vegetação Nativa em Estágio Desconhecido de Regeneração');
		}		
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
					MasterPage.redireciona($('.linkCancelar', SilviculturaATV.container).attr('href'));
				},
				conteudo: texto,
				titulo: 'Área de Reserva Legal',
				tamanhoModal: Modal.tamanhoModalMedia
			});
		};

		if (!SilviculturaATV.settings.temARL) {
			fnModal(SilviculturaATV.settings.mensagens.SemARLConfirm.Texto);
		} else if (SilviculturaATV.settings.temARLDesconhecida) {
			fnModal(SilviculturaATV.settings.mensagens.ARLDesconhecidaConfirm.Texto);
		}
	},	
	obter: function () {		
		return {
			Id: $('.hdnCaracterizacaoId', SilviculturaATV.container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', SilviculturaATV.container).val(),
			Dependencias: JSON.parse(SilviculturaATV.settings.dependencias),
			Areas: SilviculturaATV.obterAreas(),
			Caracteristicas: SilviculturaATVCaracteristica.obter()
		};
	},
	obterAreas: function () {
		var areas = [];
		$('.divArea', SilviculturaATV.container).each(function () {
			var obj = {
				Id: 0,
				Tipo: 0,
				TipoTexto: '',
				Valor: 0
			};
			obj.Id = $('.hdnAreaId', this).val();
			obj.Tipo = $('.hdnAreaTipo', this).val();
			obj.TipoTexto = $('.hdnAreaTipo', this).text();

			var valor = Number($('.hdnAreaValor', this).val());
			if (!valor) {
			    valor = Mascara.getFloatMask($('.txtArea', this).val());
			}
			obj.Valor = valor || 0;
			obj.ValorTexto = $('.txtArea', this).val();

			areas.push(obj);
		});

		return areas;
	},
	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', SilviculturaATV.container).attr('href'));
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
				$.ajax({ url: SilviculturaATV.settings.urls.mergiar,
					data: JSON.stringify(SilviculturaATV.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', SilviculturaATV.container);
						container.empty();
						container.append(response.Html);
						SilviculturaATV.settings.dependencias = response.Dependencias;						
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: SilviculturaATV.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},
	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: SilviculturaATV.settings.urls.salvar,
			data: JSON.stringify(SilviculturaATV.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, SilviculturaATV.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					SilviculturaATV.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(SilviculturaATV.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},
	onCalcular: function () {
	    var nativaValue = Number($('.hdnNativa', SilviculturaATV.container).val());
	    var plantadaValue = Number($('.hdnPlantada', SilviculturaATV.container).val());
		var plantioValue = Mascara.getFloatMask( $(this).val() );
		$('.txtTotalFloresta', SilviculturaATV.container).val(Mascara.getStringMask(nativaValue + plantadaValue + plantioValue));
		$('.hdnTotalFloresta', SilviculturaATV.container).val(nativaValue + plantadaValue + plantioValue);
	}
}

SilviculturaATVCaracteristica = {
	settings: {
		urls: {},
		mensagens: {}
	},
	container: null,
	load: function (container, options) {
		if (options) { $.extend(SilviculturaATVCaracteristica.settings, options); }
		SilviculturaATVCaracteristica.container = MasterPage.getContent(container);

		SilviculturaATVCaracteristica.container.delegate('.btnAdicionarCultura', 'click', SilviculturaATVCaracteristica.adicionar);
		SilviculturaATVCaracteristica.container.delegate('.btnExcluirCultura', 'click', SilviculturaATVCaracteristica.excluir);
	},
	adicionar: function () {
		Mensagem.limpar(SilviculturaATVCaracteristica.container);
		var mensagens = new Array();
		var container = $(this).closest('fieldset');
		var identificacao = $('.txtIdentificacao', container).val();
		var jaAdd = false;
		var cultura = {
			Id: 0,
			CulturaTipo: $('.ddlTipoCultura', container).val(),
			CulturaTipoTexto: $('.ddlTipoCultura :selected', container).text(),
			AreaCultura: $('.txtAreaCultura', container).val().replace('.', '').replace(',', '.')
		}
		var objMsg = function (obj) {
			return { Campo: obj.Campo + identificacao, Texto: obj.Texto, Tipo: obj.Tipo };
		};

		$('.hdnItemJSon', container).each(function () {
			if (jaAdd) {
				return;
			}
			var obj = String($(this).val());
			if (obj) {
				var cult = JSON.parse(obj);				
				if (cult.CulturaTipo.toString() == cultura.CulturaTipo.toString()) {
					jaAdd = true;
				}
			}
		});

		if (jaAdd) {
			mensagens.push(objMsg(SilviculturaATVCaracteristica.settings.mensagens.TipoCulturaJaAdicionado));
			Mensagem.gerar(SilviculturaATVCaracteristica.container, mensagens);
			return;
		}

		if (cultura.CulturaTipo == 0) {
			mensagens.push(objMsg(SilviculturaATVCaracteristica.settings.mensagens.TipoCulturaObrigatorio));
		}

		if (cultura.AreaCultura == '') {
			mensagens.push(objMsg(SilviculturaATVCaracteristica.settings.mensagens.AreaCulturaObrigatoria));
		} else {
			if (isNaN(cultura.AreaCultura)) {
				mensagens.push(objMsg(SilviculturaATVCaracteristica.settings.mensagens.AreaCulturaInvalida));
			} else if (Number(cultura.AreaCultura) <= 0) {
				mensagens.push(objMsg(SilviculturaATVCaracteristica.settings.mensagens.AreaCulturaMaiorZero));
			}
		}

		if (mensagens.length > 0) {
			Mensagem.gerar(SilviculturaATVCaracteristica.container, mensagens);
			return;
		}

		cultura.AreaCultura = cultura.AreaCultura.replace(".", ",");

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(cultura));
		linha.find('.CulturaTipo').html(cultura.CulturaTipoTexto).attr('title', cultura.CulturaTipoTexto);
		linha.find('.AreaCultura').html(cultura.AreaCultura).attr('title', cultura.AreaCultura);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.txtEspecificar', container).val('');
		$('.txtAreaCultura', container).val('');
		$('.ddlTipoCultura', container).ddlFirst();
	},
	obter: function () {
		var silviculturas = [];

		$('.fsSilvicultura', SilviculturaATVCaracteristica.container).each(function () {
			var objeto = {
				Id: $('.hdnSilviculturaSilvicultId', this).val(),
				Identificacao: $('.txtIdentificacao', this).val(),
				GeometriaTipo: $('.ddlGeometriaTipo :selected', this).val(),
				GeometriaTipoTexto: $('.ddlGeometriaTipo :selected', this).text(),
				FomentoId: $('.rblFomento:checked', this).val(),
				Declividade: $('.txtDeclividadeCarac', this).val(),
				TotalRequerida: $('.txtTotalRequerida', this).val(),
				TotalCroqui: Number($('.hdnTotalCroqui', this).val()),
				TotalPlantadaComEucalipto: $('.txtTotalPlantadaComEucalipto', this).val(),
				Culturas: []
			}

			$('.hdnItemJSon', this).each(function (i, item) {
				var obj = String($(item).val());
				if (obj != '') {
					objeto.Culturas.push(JSON.parse(obj));
				}
			});

			silviculturas.push(objeto);
		});

		return silviculturas;
	},
	excluir: function () {
		var container = $(this).closest('fieldset');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	}
}
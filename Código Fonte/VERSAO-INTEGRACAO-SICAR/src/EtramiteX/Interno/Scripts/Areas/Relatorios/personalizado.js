/// <reference path="../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../Lib/json2.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../mensagem.js" />

Personalizado = {
	container: null,
	salvarTelaAtual: null,
	settings: {
		mensagens: null,
		stepAtual: 0,
		isVisualizar: false,
		configuracaoRelatorio: null,
		stepCallBacks: [],
		stepUrls: [],
		urls: {
			obterCamposFiltro: '',
			validarFiltros: '',
			finalizar: ''
		}
	},

	load: function (container, options) {
		if (options) {
			$.extend(Personalizado.settings, options);
		}

		container = MasterPage.getContent(container);
		Personalizado.container = container;

		container.delegate('.btnAvancar', 'click', Personalizado.gerenciarWizard);
		container.delegate('.btnVoltar', 'click', Personalizado.gerenciarWizard);
		container.delegate('.btnFinalizar', 'click', PersonalizadoFinalizar.finalizar);

		Personalizado.settings.configuracaoRelatorio = JSON.parse($('.jsonCnfRelatorio', Personalizado.container).val()); 

		Personalizado.configurar();
	},

	configurar: function () {
		///Deixa a largura dos itens da Wizard distribuida de forma igual
		$('.wizBar ul li').css('width', 100 / $('.wizBar li').length + '%');

		/////Botões
		$('.btnAvancar', Personalizado.container).button({
			icons: {
				secondary: 'ui-icon-avancar'
			}
		});

		$('.btnVoltar').button({
			icons: {
				primary: 'ui-icon-voltar'
			}
		});

		Personalizado.settings.stepCallBacks.push(PersonalizadoOpcoes.callBack);
		Personalizado.settings.stepCallBacks.push(PersonalizadoOrdenarColunas.callBack);
		Personalizado.settings.stepCallBacks.push(PersonalizadoOrdenarValores.callBack);
		Personalizado.settings.stepCallBacks.push(PersonalizadoFiltros.callBack);
		Personalizado.settings.stepCallBacks.push(PersonalizadoSumarizar.callBack);
		Personalizado.settings.stepCallBacks.push(PersonalizadoDimensionar.callBack);
		Personalizado.settings.stepCallBacks.push(PersonalizadoAgrupar.callBack);
		Personalizado.settings.stepCallBacks.push(PersonalizadoFinalizar.callBack);

		var callBack = Personalizado.settings.stepCallBacks[Personalizado.settings.stepAtual];
		if (callBack) {
			callBack();
		}

		Aux.setarFoco(Personalizado.container);
	},

	configurarAbas: function () {
		$('.wizBar ul li', Personalizado.container).each(function (i, item) {
			if (i < Personalizado.settings.stepAtual) {
				$(this).removeClass('ativo');
				$(this).addClass('anterior');
			}

			if (i === Personalizado.settings.stepAtual) {
				$(this).removeClass('anterior');
				$(this).addClass('ativo');
			}

			if (i > Personalizado.settings.stepAtual) {
				$(this).removeClass('ativo');
				$(this).removeClass('anterior');
			}
		});
	},

	gerenciarWizard: function () {
		Mensagem.limpar(Personalizado.container);

		if (!Personalizado.settings.isVisualizar && Personalizado.salvarTelaAtual) {
			if (!Personalizado.salvarTelaAtual()) {
				return;
			}
		}

		Personalizado.obterStep(this);
	},

	obterStep: function (elemento, params) {
	    MasterPage.carregando(true);
	    var stepAtual = Personalizado.settings.stepAtual;

		if ($(elemento).hasClass('btnVoltar')) {
			stepAtual--;
		}

		if ($(elemento).hasClass('btnAvancar')) {
			stepAtual++;
		}

		var urlStep = Personalizado.settings.stepUrls[stepAtual];
		var callBack = Personalizado.settings.stepCallBacks[stepAtual];
		if (params) {
			params['configuracao'] = Personalizado.settings.configuracaoRelatorio;
		} else {
			params = { configuracao: Personalizado.settings.configuracaoRelatorio };
		}
		params.configuracao.FonteDados = { Id: params.configuracao.FonteDados.Id };

		$.ajax({ url: urlStep,
			type: "POST",
			dataType: 'html',
			contentType: 'application/json; charset=utf-8',
			data: JSON.stringify(params),
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
			    Personalizado.settings.stepAtual = stepAtual;
				var container = $('.conteudoRelatorio', Personalizado.container);
				container.empty();
				container.append(response);
				MasterPage.botoes(container);
				Mascara.load(container);
				Personalizado.configurarAbas();

				if (Personalizado.settings.stepAtual == ($('.wizBar ul li', Personalizado.container).length - 1)) {
					$('.btnAvancar', Personalizado.container).closest('div').hide();
					$('.btnFinalizar', Personalizado.container).closest('div').show();
				} else {
					$('.btnAvancar', Personalizado.container).closest('div').show();
					$('.btnFinalizar', Personalizado.container).closest('div').hide();
				}

				if (Personalizado.settings.stepAtual > 0) {
					$('.btnVoltar', Personalizado.container).closest('div').show();
				} else {
					$('.btnVoltar', Personalizado.container).closest('div').hide();
				}

				if (callBack) {
					callBack();
				}
			}
		});
		MasterPage.carregando(false);
	}
}

PersonalizadoOpcoes = {

	callBack: function () {
		///Inicia o acordion
		$("#accordion", Personalizado.container).accordion({
			autoHeight: false,
			navigation: true
		});

		///muda a cor dos elementos selecionados quando a tela carrega
		$('#accordion', Personalizado.container).find('input:checked').each(function () {
			$(this).parent().addClass('ativo');
			$(this).parent().parent().parent().prev('h3').addClass('ativo');
		});

		///muda a cor dos elementos selecionados
		$('.labelBig', Personalizado.container).click(PersonalizadoOpcoes.checkBoxArccordion);

		///Botão para marcar todos os checkboxes
		$('.checkAll', Personalizado.container).click(PersonalizadoOpcoes.checkAll);

		///Botão para desmarcar todos os checkboxes
		$('.uncheckAll', Personalizado.container).click(PersonalizadoOpcoes.unCheckAll);

		///Botão para inverter as marcações
		$('.invertkAll', Personalizado.container).click(PersonalizadoOpcoes.invertAll);

		$('.ddlFatos', Personalizado.container).change(PersonalizadoOpcoes.fatoChange);
		Personalizado.salvarTelaAtual = PersonalizadoOpcoes.salvar;
	},

	checkBoxArccordion: function () {
		if ($(this).find('input:checkbox').is(':checked')) {
			$(this).addClass('ativo');
		} else {
			$(this).removeClass('ativo');
		};

		if ($(this).closest('div').find('input:checkbox').is(':checked')) {
			$(this).closest('div').prev('h3').addClass('ativo');
		} else {
			$(this).closest('div').prev('h3').removeClass('ativo');
		};
	},

	checkAll: function () {
		$(this).closest('h3').addClass('ativo');
		$(this).parent().next('div').find('input:checkbox').each(function () {
			$(this).attr('checked', true);
			$(this).closest('.labelBig').addClass('ativo');
		});
	},

	unCheckAll: function () {
		$(this).closest('h3').removeClass('ativo');
		$(this).parent().next('div').find('input:checkbox').each(function () {
			$(this).attr('checked', false);
			$(this).closest('.labelBig').removeClass('ativo');
		});
	},

	invertAll: function () {
		$(this).closest('h3').next('div').find('input:checkbox').each(function () {
			if ($(this).is(':checked')) {
				$(this).attr('checked', false);
				$(this).closest('.labelBig').removeClass('ativo');
			} else {
				$(this).attr('checked', true);
				$(this).closest('.labelBig').addClass('ativo');
			};
		});

		if ($(this).closest('h3').next('div').find('input:checkbox').is(':checked')) {
			$(this).closest('h3').addClass('ativo');
		} else {
			$(this).closest('h3').removeClass('ativo');
		};
	},

	fatoChange: function () {
		Mensagem.limpar(Personalizado.container);
		Personalizado.settings.configuracaoRelatorio.FonteDados.Id = $('.ddlFatos', Personalizado.container).val();
		Personalizado.obterStep(this, { obterCamposFato: true });
	},

	salvar: function () {
		if ($('.ddlFatos', Personalizado.container).val() == 0) {
			Mensagem.gerar(Personalizado.container, new Array(Personalizado.settings.mensagens.RelatorioTipoObrigatorio));
			return false;
		}

		if ($('.cbCampo:checked', Personalizado.container).length <= 0) {
			Mensagem.gerar(Personalizado.container, new Array(Personalizado.settings.mensagens.CampoSelecionarObrigatorio));
			return false;
		}

		Personalizado.settings.configuracaoRelatorio.CamposSelecionados = [];
		$('.cbCampo:checked', Personalizado.container).each(function () {
			var item = JSON.parse($(this).closest('span').find('.hdnCampoJSON').val());
			item.Alias = item.Campo.Alias;
			Personalizado.settings.configuracaoRelatorio.CamposSelecionados.push(item);
		});

		return true;
	}
}

PersonalizadoOrdenarColunas = {

	callBack: function () {
		///Inicia a Lista Ordenável
		$('.sortable', Personalizado.container).sortable({
			placeholder: 'holder',
			start: function (event, ui) {
				//PlaceHolder prejudica a altura do elemento, é necessário configurar manualmente
				$('.holder', Personalizado.container).css('height', ui.item.height());

				if (ui.item.hasClass('grouped')) {
					ui.item.data('i', $('.sortable .grouped', Personalizado.container).index(ui.item));
					$('.sortable .grouped:not(.holder)', Personalizado.container).not(ui.item).each(function () {
						$(this).data('n', $('.sortable li:not(.holder)').index(this));
					}).appendTo('#helper');

					//Não deixa que os itens '.grouped' continue indexado
					$('.sortable', Personalizado.container).sortable('refresh');

					$('#helper').show();

					//Ajusta o PlaceHolder para ter a altura dos objetos
					$('.holder', Personalizado.container).css('height', (($('#helper li').length + 1) * ui.item.outerHeight()) + 'px');

					//Cancelar a ordenação em grupo
					$(window).one('keyup', function (e) {
						if (e.keyCode == 27) { // esc. PANIC!
							$('#helper li').each(function () {
								if ($('.sortable li:not(.holder)', Personalizado.container).length > $(this).data('n')) { //Pode ser o último item
									$(this).insertBefore($('.sortable li:not(.holder)')[$(this).data('n')]);
								} else { //Se é o último ele é posicionado de volta.
									$('.sortable', Personalizado.container).append(this)
								}
							});
							$('.sortable', Personalizado.container).sortable('refresh');
							//Altura do PlaceHolder precisa ser reconfigurada
							$('.holder', Personalizado.container).css('height', ui.item.height());
						} // if
					});
				}
			},
			//Tenta atualizar mesmo se o DOM não mudar
			stop: function (event, ui) {
				if ($('#helper li').length) {
					$('#helper').hide();
					ui.item.after($('#helper li'));

					var pos = ui.item.data('i');
					if (pos > 0) ui.item.insertAfter($('.sortable .grouped')[pos]);

					//Confirma se o 'sortable' sabe sobre os itens '.grouped' realocados
					$('.sortable', Personalizado.container).sortable('refresh');
				}
				$('.sortable li', Personalizado.container).removeClass('grouped');
			},
			sort: function (event, ui) { //Move o 'helper' com o item sendo ordenado
				if ($('#helper li').length) {
					var offset = ui.item.offset();
					$('#helper').css({
						left: (offset.left - 43) + 'px',
						top: (offset.top + 10) + 'px'
					});
				}
			},
			revert: 100,
			opacity: 0.6,
			containment: 'document'
		}).disableSelection();

		//Agrupa os itens quando com a tecla SHIFT.
		$('.sortable li', Personalizado.container).click(function (e) {
			if (e.shiftKey) {
				$(this).toggleClass('grouped');
			}
		});

		$('body').append($('<ul id="helper">'));

		Personalizado.salvarTelaAtual = PersonalizadoOrdenarColunas.salvar;
	},

	salvar: function () {
		$('.sortable li', Personalizado.container).each(function (i, elemento) {
			var campoId = $('.hdnCampoId', elemento).val();

			$.each(Personalizado.settings.configuracaoRelatorio.CamposSelecionados, function (idx, item) {
				if (item.Campo.Id == campoId) {
					item['Posicao'] = i;
				}
			});
		});

		return true;
	}
}

PersonalizadoOrdenarValores = {

	callBack: function () {
		///Inicia a Lista Ordenável
		$('.sortable').sortable({
			placeholder: 'holder',
			start: function (event, ui) {

				//PlaceHolder prejudica a altura do elemento, é necessário configurar manualmente
				$('.holder').css('height', ui.item.height());

				if (ui.item.hasClass('grouped')) {
					ui.item.data('i', $('.sortable .grouped').index(ui.item));
					$('.sortable .grouped:not(.holder)').not(ui.item).each(function () {
						$(this).data('n', $('.sortable li:not(.holder)').index(this));
					}).appendTo('#helper');

					//Não deixa que os itens '.grouped' continue indexado
					$('.sortable').sortable('refresh');

					$('#helper').show();

					//Ajusta o PlaceHolder para ter a altura dos objetos
					$('.holder').css('height', (($('#helper li').length + 1) * ui.item.outerHeight()) + 'px');

					//Cancelar a ordenação em grupo
					$(window).one('keyup', function (e) {
						if (e.keyCode == 27) { // esc. PANIC!
							$('#helper li').each(function () {
								if ($('.sortable li:not(.holder)').length > $(this).data('n')) { //Pode ser o último item
									$(this).insertBefore($('.sortable li:not(.holder)')[$(this).data('n')]);
								} else { //Se é o último ele é posicionado de volta.
									$('.sortable').append(this)
								}
							});
							$('.sortable').sortable('refresh');
							//Altura do PlaceHolder precisa ser reconfigurada
							$('.holder').css('height', ui.item.height());
						} // if
					});
				}
			},
			//Tenta atualizar mesmo se o DOM não mudar
			stop: function (event, ui) {
				if ($('#helper li').length) {
					$('#helper').hide();
					ui.item.after($('#helper li'));

					var pos = ui.item.data('i');
					if (pos > 0) ui.item.insertAfter($('.sortable .grouped')[pos]);

					//Confirma se o 'sortable' sabe sobre os itens '.grouped' realocados
					$('.sortable').sortable('refresh');
				}
				$('.sortable li').removeClass('grouped');
			},
			sort: function (event, ui) { //Move o 'helper' com o item sendo ordenado
				if ($('#helper li').length) {
					var offset = ui.item.offset();
					$('#helper').css({
						left: (offset.left - 43) + 'px',
						top: (offset.top + 10) + 'px'
					});
				}
			},
			remove: function (event, ui) { //Adiciona mensagem quando a lista fica vazia
				if ($(this).find('li').length == 0) {
					$(this).html('<span class="quiet">Arraste os item aqui.</span>');
					$(this).addClass('box dashed');
				}
			},
			receive: function (event, ui) { //Remove mensagem quando a lista não é mais vazia
				$(this).find('span.quiet').remove();
				$(this).removeClass('box dashed');
			},
			connectWith: ".sortable",
			revert: 100,
			opacity: 0.6,
			containment: 'document'
		}).disableSelection();

		//Agrupa os itens quando com a tecla SHIFT .
		$('.sortable li').live('click', function (e) {
			if (e.shiftKey) {
				$(this).toggleClass('grouped');
			}
		});

		$('body').append($('<ul id="helper">'));

		//Botão de Ordem alfabética
		$('.alfaOrder').mouseup(function () {
			$(this).toggleClass('ativo');

			var legAlfa;
			if ($(this).is('.ativo')) {
				legAlfa = 'Z > A';
			} else {
				legAlfa = 'A > Z';
			}
			$(this).text(legAlfa);
		});

		$('.ddlAgrupador', Personalizado.container).change(PersonalizadoOrdenarValores.agrupadorChange);
		Personalizado.salvarTelaAtual = PersonalizadoOrdenarValores.salvar;
	},

	agrupadorChange: function () {
		$('.colunaA .sortable', Personalizado.container).hide();
		$('.colunaA .agrupador' + $(this).find('option:selected').index(), Personalizado.container).show();
	},

	salvar: function () {
		Personalizado.settings.configuracaoRelatorio.Ordenacoes = [];
		$('.colunaB .sortable li', Personalizado.container).each(function (i, elemento) {
			var substr = $('.campoExibicao', elemento).text().split(' – ');

			Personalizado.settings.configuracaoRelatorio.Ordenacoes.push({
				Prioridade: i,
				Crescente: !$('.alfaOrder', elemento).hasClass('ativo'),
				Campo: {
					Id: $('.hdnCampoId', elemento).val(),
					Alias: substr[1],
					DimensaoNome: substr[0]
				}
			});
		});

		return true;
	}
}

PersonalizadoFiltros = {

	callBack: function () {
		///Inicia "Draggable" dos elementos
		$('.areaFiltroMultiplo', Personalizado.container).sortable({
			revert: 50,
			placeholder: 'ui-state-highlight',
			receive: function (event, ui) {
				$(this).find('span').removeClass('ui-button-text');
			},
			start: function (event, ui) {
				ui.placeholder.width(ui.item.width());
			},
			stop: function (event, ui) {
				ui.item.removeClass('dell');
			},
			containment: 'document'
		});

		$('.dragButtons li', Personalizado.container).draggable({
			connectToSortable: '.areaFiltroMultiplo',
			helper: 'clone',
			revert: 'invalid',
			revertDuration: 50,
			stop: function (event, ui) {
				$('.areaFiltroMultiplo', Personalizado.container).sortable("option", "revert", 50);
			},
			containment: 'document'
		});

		$('ul, li', Personalizado.container).disableSelection(); ///Desabilita a seleção de textos dos elementos

		///Deletar elementos
		$('.areaFiltroMultiplo', Personalizado.container).delegate('li', 'click', function (e) {
			$(this).toggleClass('dell');
		});

		$('button.dell', Personalizado.container).click(function (e) {
			$('.areaFiltroMultiplo li.dell', Personalizado.container).remove();
		});

		//Botões Jquery
		$('.dragButtons li', Personalizado.container).button();

		$('.btnAdicionar', Personalizado.container).click(PersonalizadoFiltros.adicionarFiltro);
		$('.ddlOperador', Personalizado.container).change(PersonalizadoFiltros.operadorChange);
		$('.cbDefinirExecucao', Personalizado.container).change(PersonalizadoFiltros.definirExecucaoChange);
		$('.ddlCampo', Personalizado.container).change(PersonalizadoFiltros.campoChange);
		$('.ddlCampo', Personalizado.container).change();
		Personalizado.salvarTelaAtual = PersonalizadoFiltros.salvar;
	},

	campoChange: function () {
		var campo = JSON.parse($(this).val());

		$('.ddlFiltro', Personalizado.container).ddlClear();
		$('.divChecks', Personalizado.container).cbClear();

		$('.txtFiltro', Personalizado.container).show();
		$('.ddlFiltro', Personalizado.container).hide();
		$('.cbDefinirExecucao', Personalizado.container).removeAttr('disabled').removeClass('disabled');

		switch (campo.TipoDados) {
			case 0:
				$('.txtFiltro', Personalizado.container).attr('disabled', 'disabled').addClass('disabled');
				$('.cbDefinirExecucao', Personalizado.container).attr('disabled', 'disabled').addClass('disabled');
				break;
			case 5://Bitand
				$('.txtFiltro', Personalizado.container).attr('disabled', 'disabled').addClass('disabled');
				$('.divChecks', Personalizado.container).cbLoad(campo.Lista, { 'tamanhoColuna': 23, 'tipoRelatorio': true });
				$('.divFiltroCheck', Personalizado.container).removeClass('hide');
				PersonalizadoFiltros.configurarCheck();
				break;
			default:
				$('.divFiltroCheck', Personalizado.container).addClass('hide');
				if (campo.PossuiListaDeValores) {
					$('.ddlFiltro', Personalizado.container).ddlLoad(campo.Lista);
					$('.ddlFiltro', Personalizado.container).ddlFirst();
					$('.ddlFiltro', Personalizado.container).show();
					$('.txtFiltro', Personalizado.container).hide();
				} else {
					$('.txtFiltro', Personalizado.container).removeClass('maskData').unmask().val('');
					$('.txtFiltro', Personalizado.container).removeClass('maskNumInt').unmaskMoney().val('');
					$('.txtFiltro', Personalizado.container).removeClass('maskDecimal').unmaskMoney().val('');
					$('.txtFiltro', Personalizado.container).removeAttr('disabled').removeClass('disabled');
				}
				break;
		}

		$.ajax({ url: Personalizado.settings.urls.obterCamposFiltro,
			type: "POST",
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			data: JSON.stringify(campo),
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				$('.ddlOperador', Personalizado.container).ddlLoad(response.Operadores);
				$('.txtFiltro', Personalizado.container).addClass(response.Mascara);
				Mascara.load($('.txtFiltro', Personalizado.container).closest('div'));
			}
		});

		$('.areaFiltroMultiplo', Personalizado.container).sortable("refresh");
	},

	configurarCheck: function () {
		///Inicia o acordion
		$("#accordion", Personalizado.container).accordion({
			autoHeight: false,
			navigation: true
		});

		///muda a cor dos elementos selecionados quando a tela carrega
		$('#accordion', Personalizado.container).find('input:checked').each(function () {
			$(this).parent().addClass('ativo');
			$(this).parent().parent().parent().prev('h3').addClass('ativo');
		});

		///muda a cor dos elementos selecionados
		$('.labelBig', Personalizado.container).click(PersonalizadoOpcoes.checkBoxArccordion);

		///Botão para marcar todos os checkboxes
		$('.checkAll', Personalizado.container).click(PersonalizadoOpcoes.checkAll);

		///Botão para desmarcar todos os checkboxes
		$('.uncheckAll', Personalizado.container).click(PersonalizadoOpcoes.unCheckAll);

		///Botão para inverter as marcações
		$('.invertkAll', Personalizado.container).click(PersonalizadoOpcoes.invertAll);
	},

	operadorChange: function () {
		var valor = $(this).val();
		if (valor == 9 || valor == 10) {
			$('.campoFiltro', Personalizado.container).addClass('disabled').attr('disabled', 'disabled');
			$('.cbDefinirExecucao', Personalizado.container).addClass('disabled').attr('disabled', 'disabled');
		} else {
			if (!$('.cbDefinirExecucao', Personalizado.container).is(':checked')) {
				if ($('.divFiltroCheck', Personalizado.container).hasClass('hide')) {
					$('.campoFiltro', Personalizado.container).removeClass('disabled').removeAttr('disabled');
				}
			}
			$('.cbDefinirExecucao', Personalizado.container).removeClass('disabled').removeAttr('disabled');
		}
	},

	definirExecucaoChange: function () {
		if ($(this).is(':checked')) {
			$('.campoFiltro', Personalizado.container).addClass('disabled').attr('disabled', 'disabled');
			$('.divChecks', Personalizado.container).cbDisabled({ 'disabled': true });
			$('.checkAll, .uncheckAll, .invertkAll', Personalizado.container).hide();
		} else {
			if ($('.divFiltroCheck', Personalizado.container).hasClass('hide')) {
				$('.campoFiltro', Personalizado.container).removeClass('disabled').removeAttr('disabled');
			}
			$('.divChecks', Personalizado.container).cbDisabled({ 'disabled': false });
			$('.checkAll, .uncheckAll, .invertkAll', Personalizado.container).show();
		}
	},

	adicionarFiltro: function () {
		var array = [];
		if ($('.ddlCampo', Personalizado.container).val() == $('.ddlCampo option', Personalizado.container).first().val()) {
			array.push(Personalizado.settings.mensagens.CampoFiltroObrigatorio);
		}

		if ($('.ddlOperador', Personalizado.container).val() == 0 && $('.ddlOperador:enabled', Personalizado.container).val() == '0') {
			array.push(Personalizado.settings.mensagens.OperadorObrigatorio);
		}

		if (($('.txtFiltro', Personalizado.container).is(':visible') && $('.txtFiltro:enabled', Personalizado.container).val() == '') ||
			($('.ddlFiltro', Personalizado.container).is(':visible') && $('.ddlFiltro:enabled', Personalizado.container).val() == '0')) {
			array.push(Personalizado.settings.mensagens.FiltroObrigatorio);
		}

		if (array.length > 0) {
			Mensagem.gerar(Personalizado.container, array);
			return false;
		}

		var campo = JSON.parse($('.ddlCampo :selected', Personalizado.container).val());
		var operador = $('.ddlOperador :selected', Personalizado.container);
		var campoFiltro = null;
		var campoValor = null;

		if (campo.TipoDados == 5) {//Bitand
			campoFiltro = $('.divChecks', Personalizado.container).cbText();
			campoValor = $('.divChecks', Personalizado.container).cbVal();
		}
		else {
			campoFiltro = $('.txtFiltro:visible:enabled', Personalizado.container).val() || $('.ddlFiltro:visible:enabled :selected', Personalizado.container).text()
			campoValor = $('.campoFiltro:visible:enabled', Personalizado.container).val();
		}

		if (operador.val() == 9 || operador.val() == 10) {
			campoFiltro = '';
		} else if ($('.cbDefinirExecucao', Personalizado.container).is(':checked')) {
			campoFiltro = 'Definido na Execução';
		}

		var termo = {
			Campo: campo,
			Valor: campoValor,
			DefinirExecucao: $('.cbDefinirExecucao:enabled', Personalizado.container).is(':checked'),
			Operador: operador.val(),
			Tipo: 1
		};

		if (!termo.Valor) {
			termo.Valor = '';
		}
		var filtro = '<li class="filtro"><input type="hidden" class="termo" value="" /><span>' +
		campo.Alias + '</span><em title="' + operador.text() + '">' + operador.text() + '</em><span>' + campoFiltro + '</span></li>';

		$('.areaFiltroMultiplo', Personalizado.container).append(filtro);
		$('.termo:last', Personalizado.container).val(JSON.stringify(termo));

		Mensagem.limpar(Personalizado.container);
		$('.ddlOperador', Personalizado.container).ddlFirst();
		$('.ddlFiltro', Personalizado.container).ddlFirst();
		$('.txtFiltro', Personalizado.container).unmask().val('');
		$('.divChecks', Personalizado.container).cbSelectAll({ 'select': false });
		$('.checkAll, .uncheckAll, .invertkAll', Personalizado.container).show();
		$('.uncheckAll', Personalizado.container).click();
		$('.cbDefinirExecucao', Personalizado.container).removeAttr('checked');
		Mascara.load($('.txtFiltro', Personalizado.container).closest('div'));
		$('.cbDefinirExecucao', Personalizado.container).removeClass('disabled').removeAttr('disabled');
		if ($('.divFiltroCheck', Personalizado.container).hasClass('hide')) {
			$('.campoFiltro', Personalizado.container).removeClass('disabled').removeAttr('disabled');
		} else {
			$('.divChecks', Personalizado.container).cbDisabled();
		}
	},

	salvar: function () {
		Personalizado.settings.configuracaoRelatorio.Termos = [];
		$('.areaFiltroMultiplo li', Personalizado.container).each(function (i, elemento) {
			var termo = JSON.parse($(elemento).find('.termo').val());
			termo['Ordem'] = i;
			Personalizado.settings.configuracaoRelatorio.Termos.push(termo);
		});

		var retorno = MasterPage.validarAjax(Personalizado.settings.urls.validarFiltros, Personalizado.settings.configuracaoRelatorio, Personalizado.container, false);

		if (!retorno.EhValido) {
			return false;
		}

		return true;
	}
}

PersonalizadoSumarizar = {

	callBack: function () {
		$('.tabSumarios tbody tr td', Personalizado.container).click(Aux.selecionarCheckGrid);
		$('.tabSumarios thead tr th', Personalizado.container).click(Aux.selecionarTodosCheckGrid);
		Personalizado.salvarTelaAtual = PersonalizadoSumarizar.salvar;
	},

	salvar: function () {
		Personalizado.settings.configuracaoRelatorio.ContarRegistros = $('.cbContarRegistros', Personalizado.container).is(':checked');
		Personalizado.settings.configuracaoRelatorio.Sumarios = [];
		$('.tabSumarios tbody tr', Personalizado.container).each(function () {
			if ($('input:checkbox', this).is(':checked')) {
				Personalizado.settings.configuracaoRelatorio.Sumarios.push({
					Campo: { Id: $('.itemId', this).val() },
					Contar: $('.cbConta', this).is(':checked'),
					Somar: $('.cbSoma', this).is(':checked'),
					Media: $('.cbMedia', this).is(':checked'),
					Maximo: $('.cbMaximo', this).is(':checked'),
					Minimo: $('.cbMinimo', this).is(':checked')
				});
			}
		});

		return true;
	}
}

PersonalizadoDimensionar = {

	callBack: function () {
		$('.campoColuna', Personalizado.container).keyup(function () {
			$('.text0001').text($(this).val());
			$('.text0002').text($(this).attr('id'));
			$('.previaColunas').find('th[title=#' + $(this).attr('id') + ']').css('width', $(this).val() + '%');
			PersonalizadoDimensionar.configurarTela();
		});

		PersonalizadoDimensionar.configurarTela();
		Personalizado.salvarTelaAtual = PersonalizadoDimensionar.salvar;
	},

	somarColunas: function () {
		var soma = 0;
		$('.campoColuna', Personalizado.container).each(function () {
			if (!isNaN(this.value) && this.value.length != 0) {
				soma += parseFloat(this.value);
			}
		});

		return soma;
	},

	configurarTela: function () {
		var soma = PersonalizadoDimensionar.somarColunas();
		$('.somatoria', Personalizado.container).html(soma.toFixed(0));

		if (soma > 100) {
			$('.somatoria', Personalizado.container).css('color', '#a80000');
		} else if (soma == 100) {
			$('.somatoria', Personalizado.container).css('color', '#205791');
		} else {
			$('.somatoria', Personalizado.container).css('color', '#000000');
		}
	},

	salvar: function () {
		if (PersonalizadoDimensionar.somarColunas() != 100) {
			Mensagem.gerar(Personalizado.container, new Array(Personalizado.settings.mensagens.SomaColunasInvalida));
			return false;
		}

		Personalizado.settings.configuracaoRelatorio.OrientacaoRetrato = $('.radioRetrato', Personalizado.container).attr('checked');

		$('.divColuna', Personalizado.container).each(function (i, elemento) {
			var campoId = $('.hdnCampoId', elemento).val();

			$.each(Personalizado.settings.configuracaoRelatorio.CamposSelecionados, function (idx, item) {
				if (item.Campo.Id == campoId) {
					item['Tamanho'] = $('.campoColuna', elemento).val();
				}
			});
		});

		return true;
	}
}

PersonalizadoAgrupar = {

	callBack: function () {
		Personalizado.salvarTelaAtual = PersonalizadoAgrupar.salvar;
	},

	salvar: function () {
		var agrupador = $('.ddlAgrupador :selected', Personalizado.container);
		Personalizado.settings.configuracaoRelatorio.Agrupamentos = [];
		if (agrupador.val() != 0) {
			Personalizado.settings.configuracaoRelatorio.Agrupamentos.push({ Campo: { id: agrupador.val() }, Prioridade: 0, Alias: agrupador.text() });
		}

		return true
	}
}

PersonalizadoFinalizar = {

	callBack: function () {
		Personalizado.salvarTelaAtual = PersonalizadoFinalizar.salvar;
	},

	salvar: function () {
		var array = new Array();
		if ($('.txtNome', Personalizado.container).val() == '') {
			array.push(Personalizado.settings.mensagens.NomeObrigatorio);
		}

		if ($('.txtDescricao', Personalizado.container).val() == '') {
			array.push(Personalizado.settings.mensagens.DescricaoObrigatoria);
		}

		if (array.length > 0) {
			Mensagem.gerar(Personalizado.container, array);
			return false;
		}

		Personalizado.settings.configuracaoRelatorio.Nome = $('.txtNome', Personalizado.container).val();
		Personalizado.settings.configuracaoRelatorio.Descricao = $('.txtDescricao', Personalizado.container).val();
		return true;
	},

	finalizar: function () {
		PersonalizadoFinalizar.salvar();
		var relatorio = {
			Id: $('.hdnRelatorioId', Personalizado.container).val(),
			Nome: Personalizado.settings.configuracaoRelatorio.Nome,
			Descricao: Personalizado.settings.configuracaoRelatorio.Descricao,
			ConfiguracaoRelatorio: Personalizado.settings.configuracaoRelatorio
		};

		$.ajax({
			url: Personalizado.settings.urls.finalizar,
			type: "POST",
			data: JSON.stringify(relatorio),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido && response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Personalizado.container, response.Msg);
					return;
				} else {
					MasterPage.redireciona(response.urlRedirecionar);
				}
			}
		});
	}
}
/// <reference path="Lib/JQuery/jquery-1.10.1.min.js"/>

/**
* Plugin jQuery para associar múltiplos itens.
* Uso: $('.seuDivMultiplosItemsContainer').associarMultiplo(options);
*/

/****************** EXEMPLO DE HTML
<div class="asmContainer">
	<div class="asmItens">
		<% for (int i; i < items.count; i++) { %>
			<div class="asmItemContainer"></div>
			<div class="asmItemContainer"></div>
			<div class="asmItemContainer"></div>
		<% } %>
	</div>
	<div class="block box">
		<button type="button" class="btnAdicionar">Adicionar</button>
		
	</div>

	<div class="asmItemTemplate itemContainer hide">
		<div class="asmConteudoFixo">
			<div class="coluna50"><label1><campo1></div>
			<div class="coluna30 prepend2"><label2><campo2></div>
			<div class="coluna12 prepend2">
				<button type="button" class="btnAsmAssociar"></button>
				<button type="button" class="btnAsmEditar"></button>
				<button type="button" class="btnAsmExcluir"></button>
			</div>
		<div class="asmConteudoInternoExpander asmExpansivel">Expandir</div>
		<div class="asmConteudoInterno hide">
			<div class="coluna50">
				<label3>
				<campo3>
			</div>
			<div class="coluna30 prepend2">
				<label4>
				<campo4>
			</div>
		</div>
	</div>
</div>



******************/

// NOTE: seria interessante que os callbacks pudessem ter acesso a todos os itens, o container, etc, etc.. o máximo de informação possível
(function ($) {
	$.fn.associarMultiplo = function (options) {
		/* sempre que um callback recebe o parametro "extra", este geralmente contém: {
		index: índice do item atual (0-n), 
		container: objeto jquery do container de todos os itens, 
		itemJson: objeto json do item serializado
		} */

		var defaults = {
			'associarModalObject': null, //Objeto de referencia para a funcao load
			'associarModalLoadFunction': '', // função load a ser chamada quando a modal abrir, pode ser uam string ou uma function
			'associarModalLoadParams': null, // objeto com propriedades que são parâmetros para se passar para a função load da modal. se isto for uma função, 
			// ela será executada, receberá parâmetros (item, extra) e deverá retornar tal objeto com as propriedades a serem passadas para o função load da modal
			'associarUrl': '', // se for uma string, é usada como url a ser usada para abir a modal de associar. se for uma function, é passado para ela item, 
			// extra e ela deve retornar uma string que é a url a ser chamada para associar naquele item
			'onAssociarClick': null, // função que é executada quando o botão associar é clicado, se esta retornar false a ação de associar é cancelada
			'onAssociar': null, // função que roda depois que um item é associado. ela recebe o retono da associação e o elemento HTML em que se está 
			// associando e deve retornar true em caso de sucesso ou um object que pode conter mensagens de erros em obj.Msg 
			// para que a modal o mostre.

			'editarModalObject': null, //Objeto de referencia para a funcao load
			'editarModalLoadFunction': '', // função load a ser chamada quando a modal abrir, pode ser uam string ou uma function
			'editarModalLoadParams': null, // objeto com propriedades que são parâmetros para se passar para a função load da modal. se isto for uma função, 
			// ela será executada, receberá parâmetros (item, extra) e deverá retornar tal objeto com a propriedades
			'editarUrl': '', // se for uma string, é usada como url a ser usada para abir a modal de editar. se for uma function, é passado para ela item, 
			'isVisualizarHistorico': false, //Utilizado quando for invocar metodo/acao para visualizacao a partir do historico
			// extra e ela deve retornar uma string que é a url a ser chamada para associar naquele item
			'onEditarClick': null,  // função que é executada quando o botão editar é clicado, se esta retornar false a ação de associar é cancelada
			'onEditar': null, // função que roda depois que um item é editado. ela recebe o retono da associação e o elemento HTML em que se está 
			// editado e deve retornar true em caso de sucesso ou um object que pode conter mensagens de erros em obj.Msg 
			// para que a modal o mostre.

			'tamanhoModal': Modal.tamanhoModalGrande,

			'itemTemplate': null,
			'expandirAutomatico': true,
			'mostrarConteudoInterno': false,
			'onExpandirEsconder': null,

			'onAdicionarClick': null, // função a ser chamada quando o botão adicionar é clicado, se esta retornar falso, o evento de adicionar é cancelado
			'onItemAdicionado': null, // função chamada após um item ser adicionado. é passado para ela o elemento HTML que acaba de ser adiconado

			'onExcluirClick': null, // função que é chamada quando o botão excluir é clicado, se esta retornar falso, o evento de excluir é cancelado
			'onItemExcluido': null, // função chamada após um item ser excluido
			'tituloExcluir': null,
			'msgExcluir': 'Deseja excluir este item?',
			'btnOkLabelExcluir': 'Sim',
			'btCancelLabelExcluir': 'Cancelar',
			'onLoadCallbackNameExcluir': '',
			'mostrarModalExcluir': true,
			'mostrarBtnExcluir': false,

			'msgObrigatoriedade': { Tipo: 3, Texto: 'Item obrigatório.' },
			'mostrarBtnLimpar': false,
			'onLimparClick': null,

			'minItens': 1, // se um número mínimo de itens for atingido os botões .btnExcluir são desabilitados/escondidos
			'maxItens': Number.MAX_VALUE, // se um número máximo de itens for atingido o botão .btnAdicionar é desabilitado/escondido
			'adicionarVarios': false
		};

		return this.each(function () {
			/****************** INICIALIZACAO ******************/
			var _container = $(this);
			var settings = $.extend(defaults, options);
			MasterPage.botoes(_container);
			$('.btnAsmAdicionar', _container).button({ icons: { primary: 'ui-icon-plusthick'} });

			var _getBlankItem = function () {
				var newItem = null;
				if (settings.itemTemplate) {
					newItem = $(settings.itemTemplate).clone();
				} else { // tenta achar asmItemTemplateContainer dentro de _container
					newItem = $('.asmItemTemplateContainer.asmItemContainer', _container).clone();
				}
				newItem.removeClass('hide asmItemTemplateContainer');
				return newItem;
			};

			if ($('.asmItens .asmItemContainer', _container).size() < settings.minItens) {
				_getBlankItem().appendTo($('.asmItens', _container));
			}

			var _ocultarAsmExcluir = function () {
				if (settings.mostrarBtnExcluir) {
					$('.asmItens .asmItemContainer .btnAsmExcluir', _container).closest('p').removeClass('hide');
				} else {
					if ($('.asmItens .asmItemContainer', _container).size() <= settings.minItens) {
						$('.asmItens .asmItemContainer .btnAsmExcluir', _container).closest('p').addClass('hide');
					}
				}
			}

			_ocultarAsmExcluir();

			/****************** EXPANDER ******************/
			_container.delegate('.asmConteudoInternoExpander.asmExpansivel', 'click', function () {
				var item = _getItemFromChild(this);

				$('.asmConteudoInterno', item).toggle('fast', function () {
					var visivel = $('.asmConteudoInterno', item).is(':visible');
					if (typeof settings.onExpandirEsconder == 'function') {
						settings.onExpandirEsconder(visivel);
					}
					if (visivel) {
						$('.asmExpansivel', item).text('Clique aqui para ocultar detalhes');
					} else {
						$('.asmExpansivel', item).text('Clique aqui para ver mais detalhes');
					}
					$('.linkVejaMaisCampos', item).toggleClass('ativo', visivel);
				});
				return false;
			});

			/****************** EXCLUIR ******************/
			_container.delegate('.btnAsmExcluir ', 'click', function () {
				var shouldRemoveItem = false;
				var item = _getItemFromChild(this);
				var extra = _getItemExtra(item);

				if (_isFunction(settings.onExcluirClick)) {
					if (settings.onExcluirClick(item, extra) !== false) {
						shouldRemoveItem = true;
					}
				} else {
					shouldRemoveItem = true;
				}

				if ($(item).find('.asmItemId').val() < 1) {
					_excluiItem(item, extra);
				} else {
					if (shouldRemoveItem) {
						if (_confirmaExcluir(item, extra)) {
							_excluiItem(item, extra);
						}
					}
				}
				_ocultarAsmExcluir();

				return false;
			});

			var _confirmaExcluir = function (item, extra) {
				if (!settings.msgExcluir) return true;
				var msgExcluir = '';
				if (typeof settings.msgExcluir == 'function') {
					msgExcluir = settings.msgExcluir(item, extra);
				} else {
					msgExcluir = settings.msgExcluir;
				}

				Modal.confirma({
					btnOkCallback: function (conteudoModal) {
						_excluiItem(item, extra);
						Modal.fechar(conteudoModal); return false;
					},
					btnOkLabel: settings.btnOkLabelExcluir,         // Label do botão [Ok] ("Ok", "Excluir" , "Confirmar", "Sim", etc..)
					btCancelLabel: settings.btCancelLabelExcluir, // Label do botão [Cancelar] ("Cancelar", "Voltar", "Não" , etc...)
					conteudo: msgExcluir, 		   // Conteúdo para preencher a modal. Pode-se usar "url" ao invés de "conteudo" para preencher a modal
					onLoadCallbackName: settings.onLoadCallbackNameExcluir, //Funcao a ser chamada depois que modal carregar, pode ser string ("Processo.load") ou funcao (Processo.load)
					titulo: settings.tituloExcluir,
					tamanhoModal: Modal.tamanhoModalPequena
				});
			};

			var _excluiItem = function (item, extra) {
				item.fadeOut(200, function () {
					item.remove();
					_ocultarAsmExcluir();
					if (_isFunction(settings.onItemExcluido)) {
						settings.onItemExcluido(_container, extra);
					}
				});
			};

			/****************** ADICIONAR ******************/
			_container.delegate('.btnAsmAdicionar', 'click', function () {
				if (_isFunction(settings.onAdicionarClick)) {
					var onAdicionarExtra = {
						numItens: $('.asmItens .asmItemContainer', _container).size(),
						minItens: settings.minItens,
						maxItens: settings.maxItens
					};
					if (settings.onAdicionarClick(_container, onAdicionarExtra) !== false) {
						_adicionarClick(this);
					}
				} else {
					_adicionarClick(this);
				}
				return false;
			});

			// adiciona item na lista e chama onItemAdicionado
			var _adicionarClick = function (_this) {
				if (!settings.adicionarVarios) {
					var naoAdicionar = false;
					var itemNaoAdicionado = null;
					$('.asmItens .asmItemContainer', _container).each(function () {
						var itemId = $(this).find('.asmItemId').val();
						var itemInternoId = $(this).find('.asmItemInternoId').val();

						if ((itemId == '' || itemId == '0' || typeof itemId == 'undefined') && (itemInternoId == '' || itemInternoId == '0' || typeof itemInternoId == 'undefined')) {
							naoAdicionar = true;
							itemNaoAdicionado = $(this);
							return;
						}
					});

					if (naoAdicionar) {
						var msgTexto = (typeof settings.msgObrigatoriedade == 'function') ? settings.msgExcluir(item, extra) : settings.msgObrigatoriedade;

						Mensagem.gerar(MasterPage.getContent(itemNaoAdicionado), new Array(msgTexto));
						itemNaoAdicionado.find('input[type=text], select').addClass('erroCampo');
						return;
					}
				}
				var novoItem = _getBlankItem();
				$('.btnAsmEditar', novoItem).addClass('hide');
				$('.btnAsmAssociar', novoItem).removeClass('hide');
				$('.asmConteudoInterno', novoItem).addClass('hide');
				novoItem.addClass('hide').appendTo($('.asmItens', _container)).fadeIn(200, function () {
					var newNumItens = $('.asmItens .asmItemContainer', _container).size();
					if (settings.expandirAutomatico) {
						novoItem.addClass('asmExpansivel');
					}

					if (settings.mostrarConteudoInterno) {
						$('.asmConteudoLink', novoItem).removeClass('hide');
					} else {
						$('.asmConteudoLink', novoItem).addClass('hide');
					}

					if (_isFunction(settings.onItemAdicionado)) {
						var onAdicionadoExtra = {
							numItens: newNumItens,
							minItens: settings.minItens,
							maxItens: settings.maxItens
						};
						if (settings.onItemAdicionado(novoItem, onAdicionadoExtra) !== false) {
							_onItemAdicionado(novoItem);
						}
					} else {
						_onItemAdicionado(novoItem);
					}
				});
				_associarClick(novoItem);
			};

			var _onItemAdicionado = function (novoItem) {
				// desabilita [adicionar] caso tenha atingido o limite de itens
				var numItens = $('.asmItens .asmItemContainer', _container).size();
				if (numItens >= settings.maxItens) {
					$('.btnAsmAdicionar', novoItem)
						.addClass('disabled')
						.attr('disabled', 'disabled');
				}

				// habilita botões de excluir que estavam desabilitados caso numItens > settings.minItens
				if (numItens > settings.minItens) {
					$('.asmItens .asmItemContainer .btnAsmExcluir', _container).closest('p').removeClass('hide');
				}
			}

			/****************** EDITAR ******************/
			_container.delegate('.btnAsmEditar', 'click', function () {
				if (_isFunction(settings.onEditarClick)) {
					var item = _getItemFromChild(this);
					var extra = _getItemExtra(item);
					if (settings.onEditarClick(item, extra) !== false) {
						_editarClick(this);
					}
				} else {
					_editarClick(this);
				}
				return false;
			});

			var _editarClick = function (_this) {

				var item = _getItemFromChild(_this);
				var extra = _getItemExtra(item);
				var retorno = null;
				if (_isFunction(settings.onEditarClick)) {
					// retorno deve ser 1) false: cancela o request ou 2) object: retorna parametros a serem passados para a modal
					var retorno = settings.onEditarClick(item, extra);
					if (retorno === false) return;
					if (typeof retorno != 'object') {
						retorno = null;
					}
				}

				var fnModalLoad = null;
				if (typeof (settings.editarModalLoadFunction) == 'object') {
					fnModalLoad = settings.editarModalLoadFunction.load;
				}
				else {
					fnModalLoad = _toFunction(settings.editarModalLoadFunction);
				}

				if (fnModalLoad == null) {
					// erro, nenhuma função válida de modal load foi passada. não é possível fazer editar sem isto
					return;
				}

				Modal.abrir(_getEditarUrl(item), retorno, function (content) {
					// mergear opções customizadas com a onAssociarCallback nossa para que possamos adicionar um layer entre as duas modais a fim de 
					// fornecer mais informações para o pai da modal quando o usuário clica em [editar] na modal
					var optionInternal = {
						'onAssociarCallback': function (objNovo) {
							return settings.onEditar(objNovo, item, extra);
						},
						'editando': true
					};
					var optionExternal = _getEditarModalLoadParams(item, extra);
					var mergedOptions = $.extend({}, optionInternal, optionExternal);

					if (settings.editarModalObject) {
						fnModalLoad.apply(settings.editarModalObject, [content, mergedOptions]);
					} else {
						fnModalLoad(content, mergedOptions);
					}

				}, settings.tamanhoModal);
			};

			var _getEditarUrl = function (item) {
				var asmItemId = $('.asmItemId:first', item);
				var asmItemTid = $('.asmItemTid:first', item).val() || '';
				var asmItemInternoId = $('.asmItemInternoId:first', item);

				if (asmItemId.size() > 0) {
					asmItemId = asmItemId.val();
				} else {
					asmItemId = '';
				}

				if (asmItemInternoId.size() > 0) {
					asmItemInternoId = asmItemInternoId.val();
				} else {
					asmItemInternoId = '';
				}

				if (settings.editarUrl) {
					if (asmItemInternoId && asmItemInternoId != 0) {
						return settings.editarUrl + "?id=" + asmItemId + '&internoId=' + asmItemInternoId;
					} else {
						if (settings.isVisualizarHistorico == true) {
							return settings.editarUrl + "/?id=" + asmItemId + '&tid=' + asmItemTid;
						}

						return settings.editarUrl + "/" + asmItemId;
					}
				}

				if (_isFunction(settings.editarUrlFunc)) return settings.editarUrlFunc(item);
			};

			/****************** Limpar ******************/
			var _limparCampos = function (_this) {
				var item = _getItemFromChild(_this);

				$('select option :first', item).attr('selected', 'selected');
				$('.asmItemId, select', item).val(0);
				$('.asmItemTexto', item).val('');
				$('input[type=text]', item).val('');
				$('.asmConteudoLink', item).addClass('hide');
				$('.btnAsmEditar', item).addClass('hide');
			};

			_container.delegate('.btnAsmLimpar', 'click', function () {
				var item = _getItemFromChild(this);
				if (_isFunction(settings.onLimparClick)) {
					var extra = _getItemExtra(item);
					settings.onLimparClick(item, extra);
				} else {
					_limparCampos(this);
				}
				_mostrarBtnLimpar(item);
			});


			/****************** ASSOCIAR ******************/
			_container.delegate('.btnAsmAssociar', 'click', function () {

				if (_isFunction(settings.onAssociarClick)) {
					var item = _getItemFromChild(this);
					var extra = _getItemExtra(item);
					if (settings.onAssociarClick(item, extra) !== false) {
						_associarClick(this);
					}
				} else {
					_associarClick(this);
				}

				return false;
			});

			var _associarClick = function (_this) {
				var item = _getItemFromChild(_this);
				var extra = _getItemExtra(item);
				var retorno = null;
				if (_isFunction(settings.onAssociarClick)) {
					// retorno deve ser 1) false: cancela o request ou 2) object: retorna parametros a serem passados para a modal
					var retorno = settings.onAssociarClick(item, extra);
					if (retorno === false) return;
					if (typeof retorno != 'object') {
						retorno = null;
					}
				}

				var fnModalLoad = _toFunction(settings.associarModalLoadFunction);
				if (fnModalLoad == null) {
					// erro, nenhuma função válida de modal load foi passada. não é possível fazer associar sem isto
					return;
				}

				Modal.abrir(_getAssociarUrl(item), retorno, function (content) {
					// mergear opções customizadas com a onAssociarCallback nossa para que possamos adicionar um layer entre as duas modais a fim de 
					// fornecer mais informações para o pai da modal quando o usuário clica em [associar] na modal
					var optionInternal = {
						'onAssociarCallback': function (objNovo) {
							var retorno = settings.onAssociar(objNovo, item, extra);
							if (retorno === true) {

								_mostrarBtnLimpar(item);

								$('.asmConteudoLink', item).removeClass('hide');
								$('.asmConteudoInterno', item).removeClass('hide');
								$('.linkVejaMaisCampos', item).addClass('ativo');
								$('.asmExpansivel', item).text('Clique aqui para ocultar detalhes');
								Mensagem.limpar(item);
							}
							return retorno;
						}
					};
					var optionExternal = _getAssociarModalLoadParams(item, extra);
					var mergedOptions = $.extend({}, optionInternal, optionExternal);

					if (settings.associarModalObject) {
						fnModalLoad.apply(settings.associarModalObject, [content, mergedOptions]);
					} else {
						fnModalLoad(content, mergedOptions);
					}

				}, settings.tamanhoModal);
			};

			var _getAssociarUrl = function (item) {
				if (settings.associarUrl) return settings.associarUrl;
				if (_isFunction(settings.associarUrlFunc)) return settings.associarUrlFunc(item);
			};

			/****************** AUXILIARES ******************/

			var _mostrarBtnLimpar = function (item) {

				if (settings.mostrarBtnLimpar) {
					_altenarBotoesLimparBuscar(item);
				}
			};

			var _altenarBotoesLimparBuscar = function (item) {

				if ($('.btnAsmLimpar', item).hasClass('hide')) {
					$('.btnAsmAssociar', item).addClass('hide');
					$('.btnAsmLimpar', item).removeClass('hide');
				} else {
					$('.btnAsmAssociar', item).removeClass('hide');
					$('.btnAsmLimpar', item).addClass('hide');
				}
			};

			var _toFunction = function (functionOrName) {
				if (typeof functionOrName == 'string') return eval(functionOrName);
				if (typeof functionOrName == 'function') return functionOrName;
				else return null;
			};

			var _getAssociarModalLoadParams = function (item, extra) {
				if (typeof settings.associarModalLoadParams == 'object') return settings.associarModalLoadParams;
				if (typeof settings.associarModalLoadParams == 'function') return settings.associarModalLoadParams(item, extra);
			};

			var _getEditarModalLoadParams = function (item, extra) {
				if (typeof settings.editarModalLoadParams == 'object') return settings.editarModalLoadParams;
				if (typeof settings.editarModalLoadParams == 'function') return settings.editarModalLoadParams(item, extra);
			};

			var _isFunction = function (param) {
				return (typeof param == 'function');
			};

			var _getItemFromChild = function (_itemChild) {
				if ($(_itemChild).hasClass('asmItemContainer')) return $(_itemChild);
				else return $(_itemChild).closest('.asmItemContainer');
			};

			var _getItemExtra = function (_itemChild) {
				var item = _getItemFromChild(_itemChild);
				var _itemJson = _jsonContents(item);
				return { index: item.index(), container: _container, itemJson: _itemJson };
			};

			var _jsonContents = function (ctr) {
				var st = $('*', ctr).serializeArray();
				var obj = {};
				$.each(st, function (idx, item) {
					if (typeof (item.name) !== 'undefined') {
						obj[item.name] = item.value;
					}
				});
				obj['asmItemId'] = $('.asmItemId', ctr).val();
				return obj;
			};
		});
	};
})(jQuery);
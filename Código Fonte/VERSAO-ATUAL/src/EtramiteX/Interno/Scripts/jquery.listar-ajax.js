/// <reference path="JQuery/jquery-1.4.3.js"/>

// Uso:
// ('.Container').listarAjax()
// ('.Container').listarAjax('ultimaBusca')

// Sempre usar Div para bindar o Grid

// Estrutura de uma listagem
/***********.filtroSerializarAjax***************************************************************
* Filtros.Filtro1, Filtros.Filtro2												[.btnBuscar]
* hdn.ItensPorPagina
* hdn.PaginaAtual
* hdn.UrlFiltrar
* hdn.OrdenarPor
* hdn.UltimaBusca
************.gridContainer *************************************************************
* select.comboItensPorPagina						class="1 pagina" ... class="10 pagina"
* hdn.NumeroDePaginas
* .ordenavel th:not(.semOrdenacao)
*
***********************************************************************************/

(function ($) {
	$.fn.listarAjax = function (method) {
		if (methods[method]) {
			return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
		} else if (typeof method === 'object' || !method) {
			return methods.init.apply(this, arguments);
		} else {
			$.error('Erro de sistema: Método "' + method + '" não existe em jQuery.listarAjax');
		}
	};

	/******* VARIÁVEIS GLOBAIS *******/
	var objetoSerializado = null;

	/******* MÉTODOS GLOBAIS *******/
	var methods = {
		init: function (options) {
			return this.each(function () {
				var settings = {
					'onBeforeFiltrar': null,
					'onBeforeSerializar': null,
					'onAfterFiltrar': null,
					'customFiltrarFunction': null,
					'mensagemContainer': null,
					'itensPorPaginaTimeout': 1440
				};

				if (options) {
					$.extend(settings, options);
				}

				var container = $(this);
				var hdnPaginaAtual = $('.paginaAtual', container);

				if (Aux.isModal(container)) {
					container.delegate('tr', 'dblclick', function () {
						$(this).find('.associar').click();
					});
				}

				container.data('listarajax', { 'settings': settings });

				container.delegate('.comboItensPorPagina', 'change', function () {
					hdnPaginaAtual.val(1);
					var indice = parseInt($(this).attr('value'));
					Cookie.set('QuantidadePorPagina', indice, settings.itensPorPaginaTimeout);
					filtrar(container);
				});

				container.delegate('.btnBuscar', 'click', function () {
					hdnPaginaAtual.val(1);
					$('.ultimaBusca', container).val('');
					filtrar(container);
					filtrosJsonString = JSON.stringify(objetoSerializado);
					$('.ultimaBusca', container).val(filtrosJsonString);
				});

				container.delegate('.filtroSerializarAjax', 'keyup', function (e) {
					if (e.keyCode == 13) $('.btnBuscar', container).click();
				});

				container.delegate('.paginar', 'click', function () {
					var pagPretendida = parseInt($(this).attr('class'));
					var pagAtual = parseInt(hdnPaginaAtual.val());
					var pagFinal = parseInt($('.numeroPaginas', container).val());

					if (pagPretendida != pagAtual && pagPretendida <= pagFinal && pagPretendida > 0) {
						hdnPaginaAtual.val(pagPretendida);
						filtrar(container);
					}
				});

				container.delegate('.ordenavel th:not(.semOrdenacao)', 'click', function () {
					var indexColuna = $(this).parent().children().index(this) + 1;
					$('.ordenarPor', container).val(indexColuna);
					hdnPaginaAtual.val(1);
					filtrar(container);
				});
			});
		},
		ultimaBusca: function () {
			filtrar($(this));
		},

		ultimaBuscaVoltarPagina: function () {
			$(this).data('listarajax').settings.onBeforeFiltrar = methods.beforeExcluir;
			filtrar($(this));
			$(this).data('listarajax').settings.onBeforeFiltrar = null;
		},

		beforeExcluir: function (container, serializedData) {
			serializedData.Paginacao.PaginaAtual = serializedData.Paginacao.PaginaAtual - 1;
			$('.paginaAtual', container).val(serializedData.Paginacao.PaginaAtual);
		}
	};

	/******* FUNÇÕES COMPARTILHADAS *******/

	// transforma <input name="Obj1.Obj2.Obj3" value="valor"> em Obj1: {Obj2: {Obj3: valor}}
	var setObjFromFieldName = function (obj, itemName, itemValue) {
		var itemNames = itemName.split('.');
		var numNames = itemNames.length;
		var objRoot = obj;
		for (var i = 0; i <= numNames - 1; i++) {
			if (typeof objRoot[itemNames[i]] === 'undefined') {
				objRoot[itemNames[i]] = {};
			}
			if (i >= numNames - 1) {
				objRoot[itemNames[i]] = itemValue;
			} else {
				objRoot = objRoot[itemNames[i]];
			}
		}
	};

	var jsonContents = function (ctr) {
		// TODO: melhorar código abaixo depois de melhorar a lib de máscara.
		// Trima os campos com máscara de número antes serializar (corrige bug da máscara que deixa espaços em alguns campos)
		$('.maskCoordGms, .maskCoordGdec, .maskUtm, .maskCep, .maskCcir, .maskNirf, .maskFone, .maskCpf, .maskCpfParcial, .maskCnpj, ' +
		'.maskCnpjParcial, .maskData, .maskNum3, maskNum4, .maskNum15,.maskNum38, .maskNumEndereco, .maskAreaAbrangencia, ' +
		'.maskDocumentoAnterior, .maskProc, .maskQuantidadeVolumes, .maskArea54, .maskArea74, .maskHora, .maskNumInt', ctr)
		.filter(':enabled')
		.each(function () {
			$(this).val($(this).val().toString().trim());
		});

		var st = $('*', ctr).serializeArray();
		var objFields = {};
		$.each(st, function (idx, item) {
			// pega campos com nomes complexos como "Filtros.NumeroProcesso" e seta objFields.Filtros.NumeroProcesso = 12
			setObjFromFieldName(objFields, item.name, item.value);
		});
		return objFields;
	};

	var filtrar = function (container) {
		Mensagem.limpar(MasterPage.getContent(container));
		var jsonReturned = null;
		var data = container.data('listarajax');
		var settings = data.settings;

		if (settings.onBeforeSerializar) {
			settings.onBeforeSerializar(container);
		}
		
		objetoSerializado = jsonContents($('.filtroSerializarAjax', container));

	    //Solução para a mascara que não deixava filtrar certo, replace do "." que estava retornando uma consulta incorreta
		if (objetoSerializado.Filtros.SolicitacaoNumero != null) objetoSerializado.Filtros.SolicitacaoNumero = objetoSerializado.Filtros.SolicitacaoNumero.split('.').join('')
		
		if (settings.onBeforeFiltrar) {
			settings.onBeforeFiltrar(container, objetoSerializado);
		}

		if (settings.customFiltrarFunction) {
			jsonReturned = settings.customFiltrarFunction(container, objetoSerializado);
		} else {
			$.ajax({
				url: $('.urlFiltrar', container).val(), data: JSON.stringify(objetoSerializado), type: 'POST', typeData: 'json',
				contentType: 'application/json; charset=utf-8', cache: false, async: false,
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					var containerMensagem = null;
					if (settings.mensagemContainer) {
						containerMensagem = settings.mensagemContainer;
					} else {
						containerMensagem = MasterPage.getContent(container);
					}
					Aux.error(XMLHttpRequest, textStatus, erroThrown, containerMensagem);
				},

				success: function (response, textStatus, XMLHttpRequest) {
					if (response.Msg && response.Msg.length > 0) {
						if (settings.mensagemContainer) {
							Mensagem.gerar(MasterPage.getContent(settings.mensagemContainer), response.Msg);
						} else {
							Mensagem.gerar(MasterPage.getContent(container), response.Msg);
						}
					}

					if (response.Html && response.Html.length > 0) {
						$('.gridContainer', container).html(response.Html).removeClass('hide');
						Listar.atualizarEstiloOrdenar($('.ordenarPor', container).val(), container);
						Listar.atualizarEstiloTable($('.gridContainer .dataGridTable', container));
					}

					if (settings.onAfterFiltrar) {
						settings.onAfterFiltrar(container, objetoSerializado, jsonReturned);
					}
					MasterPage.redimensionar();
				}
			});
		}
	};
})(jQuery);
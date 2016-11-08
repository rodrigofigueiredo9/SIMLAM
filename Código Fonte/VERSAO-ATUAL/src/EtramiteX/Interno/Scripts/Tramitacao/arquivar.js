/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Mensagem.js" />

Arquivar = {
	settings: {
		urls: {
			arquivarObterTodos: '',
			arquivosCadastrados: '',
			arquivarAdicionarItem: '',
			arquivarBuscarEstantes: '',
			arquivarBuscarPrateleiras: '',
			arquivar: '',
			abrirPdfItem: '',
			abrirPdfHistoricoTramitacao: '',
			visualizarHistorico: '',
			visualizarProc: null,
			visualizarDoc: null
		},
		Mensagens: {}
	},
	container: {},

	load: function (content, options) {
		Arquivar.container = content;

		if (options) {
			$.extend(Arquivar.settings, options);
		}

		content.delegate('.ddlSetoresOrigem', 'change', Arquivar.onDdlSetoresOrigemChange);
		content.delegate('.ddlArquivo', 'change', Arquivar.onDdlArquivoChange);
		content.delegate('.ddlEstantes', 'change', Arquivar.onDdlEstanteChange);
		content.delegate('.ddlPrateleirasModo', 'change', Arquivar.onDdlPrateleirasModoChange);
		content.delegate('.trTramitacao', 'click', Arquivar.onTrTramitacaoClick);
		content.delegate('.btnPdf', 'click', Arquivar.onAbrirPdfClick);
		content.delegate('.btnHistorico', 'click', Arquivar.onAbrirHistoricoTramitacaoClick);
		content.delegate('.btnRemover', 'click', Arquivar.onTrExcluirLinha);
		content.delegate('.btnVisualizar', 'click', Arquivar.onVisualizarProtocolo);

		Arquivar.animateGridCheckBoxes('.tabItens', content);

		$('.radFiltroTipo', content).change(Arquivar.onRadFiltroTipoChange);
		$('.btnAddProcDoc', content).click(Arquivar.onBtnAddProcDocClick);
		$('.btnArquivar', content).click(Arquivar.onBtnArquivarClick);

		if ($('select.ddlSetoresOrigem', content).val() != 0) {
			$('select.ddlSetoresOrigem', content).trigger('change', true);
		}

		Arquivar.container.delegate('.txtNumeroProcDoc', 'keyup', function (e) {
			if (e.keyCode == 13) {
				$('.btnAddProcDoc', Arquivar.container).click();
			}
		});

		$('.ddlSetoresOrigem', Arquivar.container).focus();
	},

	animateGridCheckBoxes: function (gridClass, content) {
		content.delegate(gridClass + ' .ckbCheckAllInMyColumn', 'change', function () {
			$('tbody .ckbSelecionavel', $(this).closest('table')).attr('checked', $(this).is(':checked'));
		});

		content.delegate('.ckbSelecionavel', 'change', function () {
			var table = $(this).closest('table');
			var checkBoxes = table.find('.ckbSelecionavel');
			var todosSelecionados = !checkBoxes.is(':not(:checked)');

			table.find('.ckbCheckAllInMyColumn').attr('checked', todosSelecionados);
		});
	},

	onTrTramitacaoClick: function (e) {
		if ($(e.target).is('input')) return;
		var selecionado = $('.ckbSelecionavel', this).is(':checked');
		$('.ckbSelecionavel', this).attr('checked', !selecionado).trigger('change');
		$(this).toggleClass('linhaSelecionada', !selecionado);
	},

	onDdlArquivoChange: function () {
		MasterPage.carregando(true);

		// Pega estantes
		$(this).ddlCascate($('.ddlEstantes', Arquivar.container), { url: Arquivar.settings.urls.arquivarBuscarEstantes, disabled: false, autoFocus: false });
		$('.ddlEstantes', Arquivar.container).change();

		MasterPage.carregando(false);
	},

	onDdlEstanteChange: function () {
		MasterPage.carregando(true);

		$('.ddlPrateleirasModo', Arquivar.container).ddlFirst();
		$('.ddlIdentificacao', Arquivar.container).ddlClear();

		if ($(this).val() == 0) {
			$('.ddlPrateleirasModo', Arquivar.container).attr('disabled', 'disabled').addClass('disabled');
		}

		MasterPage.carregando(false);
	},

	onDdlPrateleirasModoChange: function () {
		MasterPage.carregando(true);

		// Pega Identificacoes
		$(this).ddlCascate($('.ddlIdentificacao', Arquivar.container), {
			url: Arquivar.settings.urls.arquivarBuscarPrateleiras,
			data: { Id: 0, estanteId: $('.ddlEstantes', Arquivar.container).val() },
			disabled: false,
			autoFocus: false
		});

		MasterPage.carregando(false);
	},

	onDdlSetoresOrigemChange: function (event, manterMsg) {
		if (!manterMsg) {
			Mensagem.limpar(MasterPage.getContent(Arquivar.container));
		}
		MasterPage.carregando(true);

		var setorId = parseInt($('.ddlSetoresOrigem', Arquivar.container).val());
		$('.tabItens tbody', Arquivar.container).empty();

		$('.containerProcessos, .containerDadosArquivamento', Arquivar.container).toggleClass('hide', !setorId);

		if (setorId) {
			$('.radFiltroTipo', Arquivar.container).removeAttr('checked');
			$('.divNumeroProcessoDoc', Arquivar.container).addClass('hide');
		}

		$(this).ddlCascate($('.ddlArquivo', Arquivar.container), { url: Arquivar.settings.urls.arquivosCadastrados, disabled: false, autoFocus: false });

		if ($('.ddlArquivo', Arquivar.container).val() != 0) {
			$('.ddlArquivo', Arquivar.container).trigger('change', true);
		}

		$('.radFiltroTipo:first', Arquivar.container).attr('checked', 'checked');

		$('.containerProcessos, .containerDadosArquivamento').toggleClass('hide', $('.ddlArquivo option', Arquivar.container).length <= 1);

		if (setorId == 0) {
			Mensagem.limpar(Arquivar.container);
		}
		else {
			Arquivar.onRadFiltroTipoChange(null, true);
			if ($('.ddlArquivo option', Arquivar.container).size() == 1 && $('.ddlArquivo', Arquivar.container).val() == 0) {
				Mensagem.gerar(Arquivar.container, [Arquivar.settings.Mensagens.SetorSemArquivo]);
			}
		}

		MasterPage.redimensionar();
		MasterPage.carregando(false);

		Listar.atualizarEstiloTable(Arquivar.container.find('.dataGridTable'));
	},

	onRadFiltroTipoChange: function (event, manterMsg) {
		var tipoFiltro = parseInt($('input.radFiltroTipo:checked', Arquivar.container).val());
		$('.tabItens tbody', Arquivar.container).empty();
		$('.divNumeroProcessoDoc', Arquivar.container).toggleClass('hide', tipoFiltro == 1);
		$('.ckbCheckAllInMyColumn').removeAttr('checked');
		if (tipoFiltro == 1) { // todos
			Arquivar.buscarTodos(manterMsg);
		}
		Listar.atualizarEstiloTable(Arquivar.container.find('.dataGridTable'));
	},

	buscarTodos: function (manterMsg) {
		var setorId = parseInt($('.ddlSetoresOrigem', Arquivar.container).val());

		if (!manterMsg) {
			Mensagem.limpar(MasterPage.getContent(Arquivar.container));
		}

		MasterPage.carregando(true);

		$.ajax({
			url: Arquivar.settings.urls.arquivarObterTodos,
			data: { 'setorId': setorId },
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Arquivar.container), response.Msg, manterMsg);
				}

				if (response.EhValido) {
					$('.tabItens tbody', Arquivar.container).html(response.ItensHtml);
				}

				Mascara.load(Arquivar.container);
				MasterPage.redimensionar();
			}
		});
		MasterPage.carregando(false);
	},

	onTrExcluirLinha: function () {
		$(this).closest('tr').remove();
	},

	onVisualizarProtocolo: function(){
		var linha = $(this, Arquivar.container).closest('tr')
		var id = $('.hdnProtocoloId', linha).val();
		var url = ($('.hdnIsProcesso', linha).val() == "True") ? Arquivar.settings.urls.visualizarProc : Arquivar.settings.urls.visualizarDoc;
		Modal.abrir(url + '/' + id, null, function (container) { Modal.defaultButtons(container); }, Modal.tamanhoModalGrande);
	},

	onBtnAddProcDocClick: function () {
		var setorId = parseInt($('.ddlSetoresOrigem', Arquivar.container).val());

		Mensagem.limpar(MasterPage.getContent(Arquivar.container));
		MasterPage.carregando(true);

		var tab = $('.tabItens', Arquivar.container);
		var erroMsg = [];

		$.ajax({
			url: Arquivar.settings.urls.arquivarAdicionarItem,
			data: { 'setorId': setorId, 'numero': $('.txtNumeroProcDoc', Arquivar.container).val() },
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				var tram = response.Tramitacao;
				if (tram) {
					if (Arquivar.existeAssociado(tram.Protocolo.Id.toString(), tab, 'hdnProtocoloId')) {
						erroMsg.push(Arquivar.settings.Mensagens.ProtocoloJaAdicionado);
						Mensagem.gerar(Arquivar.container, erroMsg);
						return;
					}
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Arquivar.container), response.Msg);
				}

				if (response.EhValido) {
					$('.tabItens tbody', Arquivar.container).append($(response.ItemHtml));
					$('.txtNumeroProcDoc', Arquivar.container).val('');
				}

				MasterPage.redimensionar();
			}
		});
		MasterPage.carregando(false);
	},

	onBtnArquivarClick: function () {
		var ArquivarVM = {
			Arquivar: {
				SetorId: parseInt($('.ddlSetoresOrigem', Arquivar.container).val()),
				ObjetivoId: parseInt($('.ddlObjetivos', Arquivar.container).val()),
				ArquivoId: parseInt($('.ddlArquivo', Arquivar.container).val()),
				EstanteId: parseInt($('.ddlEstantes', Arquivar.container).val()),
				PrateleiraModoId: parseInt($('.ddlPrateleirasModo', Arquivar.container).val()),
				PrateleiraId: $('.ddlIdentificacao', Arquivar.container).val(),
				Despacho: $('.txtDespacho', Arquivar.container).val()
			},
			ItensJson: []
		};

		$('.tabItens tr .ckbSelecionavel:checked').each(function () {
			ArquivarVM.ItensJson.push($('.hdnTramitacaoJson', $(this).closest('tr')).val());
		});

		Mensagem.limpar(MasterPage.getContent(Arquivar.container));
		MasterPage.carregando(true);

		$.ajax({
			url: Arquivar.settings.urls.arquivar,
			data: JSON.stringify(ArquivarVM),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedireciona);
				} else {
					Mensagem.gerar(MasterPage.getContent(Arquivar.container), response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	onAbrirPdfClick: function () {
		var id = parseInt($(this).closest('tr').find('.hdnHistorico').val());
		var tipo = (($(this).closest('tr').find('.hdnIsProcesso').val()).toLowerCase() == "true" ? 1 : 2);

		MasterPage.redireciona(Arquivar.settings.urls.abrirPdfItem + "?id=" + id + "&tipo=" + tipo + "&obterHistorico=" + true);
		MasterPage.carregando(false);
	},

	onAbrirHistoricoTramitacaoClick: function () {
		var id = parseInt($(this).closest('tr').find('.hdnProtocoloId').val());
		var tipo = (($(this).closest('tr').find('.hdnIsProcesso').val()).toLowerCase() == "true" ? 1 : 2);

		Modal.abrir(Arquivar.settings.urls.visualizarHistorico, { id: id, tipo: tipo }, function (context) {
			Modal.defaultButtons(context);
		});
	},

	existeAssociado: function (item, tab, itemClass) {
		var existe = false;
		var itens = $(tab).find('tr');

		$.each(itens, function (key, elem) {
			if ($(elem).find('.' + itemClass) !== '') {
				var trItem = $(elem).find('.' + itemClass).val();
				existe = (trItem != null && trItem.toLowerCase().trim() === item.toLowerCase().trim());
				if (existe) {
					return false;
				}
			}
		});
		return existe;
	}
}
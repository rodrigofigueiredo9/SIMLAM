/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../Mensagem.js" />

Desarquivar = {
	container: null,
	settings: {
		urls: {
			arquivosCadastrados: '',
			visualizarHistorico: '',
			abrirPdf: '',
			filtrar: '',
			desarquivar: '',
			desarquivarAdicionarItem: '',
			desarquivarBuscarEstantes: '',
			desarquivarBuscarPrateleiras: '',
			visualizarProc: null,
			visualizarDoc: null
		},
		Mensagens: {}
	},

	load: function (content, options) {
		if (options) {
			$.extend(Desarquivar.settings, options);
		}
		Desarquivar.container = content;
		$('.btnDesarquivar', content).click(Desarquivar.desarquivar);
		$('.btnBuscarArquivo', content).click(Desarquivar.onFiltrar);
		$('.ddlRemetenteSetor', content).change(Desarquivar.onDdlSetoresOrigem);
		$('.ddlArquivos', content).change(function () { $('.tabTramitacoesArquivadas tbody tr:visible', Desarquivar.container).remove(); });
		content.delegate('.ddlArquivos', 'change', Desarquivar.onDdlArquivoChange);
		content.delegate('.ddlEstantes', 'change', Desarquivar.onDdlEstanteChange);
		content.delegate('.ddlPrateleirasModo', 'change', Desarquivar.onDdlPrateleirasModoChange);
		content.delegate('.btnHistorico', 'click', Desarquivar.onVisualizarHistorico);
		content.delegate('.btnPdf', 'click', Desarquivar.onAbrirPdf);
		content.delegate('.btnVisualizar', 'click', Desarquivar.onVisualizarProtocolo);
		Desarquivar.animateGridCheckBoxes('.tabTramitacoesArquivadas', content);
		content.delegate('.trTramitacao', 'click ', Desarquivar.onSelecionarTrTramitacao);
		$('.ddlEstado', content).change(Aux.onEnderecoEstadoChange);

		$('.radFiltroTipo', content).change(Desarquivar.onRadFiltroTipoChange);
		$('.btnAddProcDoc', content).click(Desarquivar.onBtnAddProcDocClick);

		Desarquivar.container.delegate('.txtNumeroProcDoc', 'keyup', function (e) {
			if (e.keyCode == 13) {
				$('.btnAddProcDoc', Desarquivar.container).click();
			}
		});

		if ($('.ddlRemetenteSetor', content).val() != 0) {
			$('.ddlRemetenteSetor', content).trigger('change', true);
		}

		setTimeout(function () {
			$('.ddlRemetenteSetor', Desarquivar.container).focus();
		}, 500);
	},

	onVisualizarProtocolo: function () {
		var linha = $(this, Desarquivar.container).closest('tr')
		var id = $('.hdnProtocoloId', linha).val();
		var url = ($('.hdnIsProcesso', linha).val() == "True") ? Desarquivar.settings.urls.visualizarProc : Desarquivar.settings.urls.visualizarDoc;
		Modal.abrir(url + '/' + id, null, function (container) { Modal.defaultButtons(container); }, Modal.tamanhoModalGrande);
	},

	onDdlArquivoChange: function () {
		MasterPage.carregando(true);

		// Pega estantes
		$(this).ddlCascate($('.ddlEstantes', Desarquivar.container), { url: Desarquivar.settings.urls.desarquivarBuscarEstantes, disabled: false, autoFocus: false });
		$('.ddlEstantes', Desarquivar.container).change();

		MasterPage.carregando(false);
	},

	onDdlEstanteChange: function () {
		MasterPage.carregando(true);

		$('.ddlPrateleirasModo', Desarquivar.container).ddlFirst();
		$('.ddlIdentificacao', Desarquivar.container).ddlClear();

		if ($(this).val() == 0) {
			$('.ddlPrateleirasModo', Desarquivar.container).attr('disabled', 'disabled').addClass('disabled');
		}

		MasterPage.carregando(false);
	},

	onDdlPrateleirasModoChange: function () {
		MasterPage.carregando(true);

		// Pega Identificacoes
		$(this).ddlCascate($('.ddlIdentificacao', Desarquivar.container), {
			url: Desarquivar.settings.urls.desarquivarBuscarPrateleiras,
			data: { Id: 0, estanteId: $('.ddlEstantes', Desarquivar.container).val() },
			disabled: false,
			autoFocus: false
		});

		MasterPage.carregando(false);
	},

	onRadFiltroTipoChange: function (event, manterMsg) {
		var tipoFiltro = parseInt($('input.radFiltroTipo:checked', Desarquivar.container).val());
		$('.tabItens tbody', Desarquivar.container).empty();
		$('.divNumeroProcessoDoc', Desarquivar.container).toggleClass('hide', tipoFiltro == 1);
		$('.ckbCheckAllInMyColumn').removeAttr('checked');
		$('.txtNumeroProcDoc', Desarquivar.container).val('');

		$('.fsFiltrosDesarquivamento', Desarquivar.container).addClass('hide');

		if (tipoFiltro == 1) { // todos
			$('.fsFiltrosDesarquivamento', Desarquivar.container).removeClass('hide');
		}
		Listar.atualizarEstiloTable(Desarquivar.container.find('.dataGridTable'));
	},

	onBtnAddProcDocClick: function () {
		var setorId = +$('.ddlRemetenteSetor', Desarquivar.container).val();
		var arquivoId = +$('.ddlArquivos', Desarquivar.container).val();
		var protocoloNumero = $('.txtNumeroProcDoc', Desarquivar.container).val();

		var arquivoEstanteId = $('.ddlEstantes', Desarquivar.container).val();
		var arquivoPrateleiraModoId = $('.ddlPrateleirasModo', Desarquivar.container).val();
		var arquivoIdentificacao = $('.ddlIdentificacao :selected', Desarquivar.container).val();
		var arquivoIdentificacaoTexto = '';

		if (arquivoIdentificacao != 0) {
			arquivoIdentificacaoTexto = $('.ddlIdentificacao :selected', Desarquivar.container).text();
		}

		Mensagem.limpar(MasterPage.getContent(Desarquivar.container));
		MasterPage.carregando(true);

		var tab = $('.tabItens', Desarquivar.container);
		var erroMsg = [];

		$.ajax({
			url: Desarquivar.settings.urls.desarquivarAdicionarItem,
			data: { 'setorId': setorId, 'arquivoId': arquivoId, 'ArquivoEstanteId': arquivoEstanteId, 'ArquivoPrateleiraModoId': arquivoPrateleiraModoId, 'ArquivoIdentificacao': arquivoIdentificacaoTexto, 'numero': protocoloNumero },
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Desarquivar.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				var tram = response.Tramitacao;
				if (tram) {
					if (Desarquivar.existeAssociado(tram.Protocolo.Id.toString(), tab, 'hdnProtocoloId')) {
						erroMsg.push(Desarquivar.settings.Mensagens.ProtocoloJaAdicionado);
						Mensagem.gerar(Desarquivar.container, erroMsg);
						return;
					}
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Desarquivar.container), response.Msg);
				}

				if (response.EhValido) {
					$('.tabItens tbody', Desarquivar.container).append($(response.ItemHtml));
					$('.txtNumeroProcDoc', Desarquivar.container).val('');
				}

				Listar.atualizarEstiloTable(Desarquivar.container);
				MasterPage.redimensionar();
			}
		});
		MasterPage.carregando(false);
	},

	onSelecionarTrTramitacao: function (e) {
		if ($(e.target).is('input')) return;
		var selecionado = $('.ckbIsSelecionado', this).is(':checked');
		$('.ckbIsSelecionado', this).attr('checked', !selecionado).trigger('change');
		$(this).toggleClass('linhaSelecionada', !selecionado);

	},

	animateGridCheckBoxes: function (gridClass, content) {
		content.delegate(gridClass + ' .ckbIsTodosSelecionado', 'change', function () {
			$('tbody .ckbIsSelecionado', $(this).closest('table')).attr('checked', $(this).is(':checked'));
		});

		content.delegate('.ckbIsSelecionado', 'change', function () {
			var table = $(this).closest('table');
			var checkBoxes = table.find('.ckbIsSelecionado');
			var todosSelecionados = !checkBoxes.is(':not(:checked)');

			table.find('.ckbIsTodosSelecionado').attr('checked', todosSelecionados);
		});
	},

	onTrTramitacaoClick: function (e) {
		if ($(e.target).is('input')) return;
		$('.ckbIsSelecionado', this).attr('checked', !$('.ckbIsSelecionado', this).is(':checked'));
		$('.ckbIsSelecionado', this).trigger('change');
		$(this).toggleClass('linhaSelecionada', $('.ckbIsSelecionado', this).is(':checked'));
	},

	onDdlSetoresOrigem: function () {
		Mensagem.limpar(Desarquivar.container);
		MasterPage.carregando(true);
		var origem = $(this).val();
		var ddlB = $('.ddlArquivos', Desarquivar.container);
		var ddlA = $(this, Desarquivar.container);
		var url = Desarquivar.settings.urls.arquivosCadastrados;
		var setorId = ddlA.val();

		if (origem == 0) {
			$('.ddlDestinatarioSetor', Desarquivar.container).val(0);
		}

		ddlA.ddlCascate(ddlB, { url: url, disabled: false });

		if (ddlB != 0) {
			$('.ddlArquivos', Desarquivar.container).trigger('change');
		}

		$('.tabTramitacoesArquivadas tbody tr:visible', Desarquivar.container).remove();

		var esconderContainerDadosDesarquivamento = false;
		if (setorId == 0) {
			esconderContainerDadosDesarquivamento = true;
		}
		else {
			if ($('.ddlArquivos option', Desarquivar.container).size() == 1 && $('.ddlArquivos', Desarquivar.container).val() == 0) {
				Mensagem.gerar(Desarquivar.container, [Desarquivar.settings.Mensagens.SetorSemArquivo]);
				esconderContainerDadosDesarquivamento = true;
			}
		}

		$('.radFiltroTipo:first', Desarquivar.container).attr('checked', 'checked');
		$('.radFiltroTipo', Desarquivar.container).trigger('change');

		$('.containerDadosFiltroDesarquivamento', Desarquivar.container).toggleClass('hide', esconderContainerDadosDesarquivamento);
		$('.fsDadosDesarquivamento', Desarquivar.container).toggleClass('hide', esconderContainerDadosDesarquivamento);
		$('.containerProcessos', Desarquivar.container).toggleClass('hide', esconderContainerDadosDesarquivamento);

		MasterPage.carregando(false);
	},

	onVisualizarHistorico: function () {
		var id = parseInt($('.hdnProtocoloId', $(this).closest('tr')).val());
		var tipo = $('.hdnIsProcesso', $(this).closest('tr')).val().toLowerCase() === "true" ? 1 : 2;

		Modal.abrir(Desarquivar.settings.urls.visualizarHistorico, { id: id, tipo: tipo }, function (context) {
			Modal.defaultButtons(context);
		});
	},

	onAbrirPdf: function () {
		var id = $('.hdnProtocoloId', $(this).closest('tr')).val();
		var tipo = $('.hdnIsProcesso', $(this).closest('tr')).val().toLowerCase() === "true" ? 1 : 2;

		if (id > 0) {
			MasterPage.redireciona(Desarquivar.settings.urls.abrirPdf + "?id=" + id + '&tipo=' + tipo);
			MasterPage.carregando(false);
		}
	},

	onFiltrar: function () {
		Mensagem.limpar(MasterPage.getContent(Desarquivar.container));
		var filtros = {
			RemetenteSetorId: $('.ddlRemetenteSetor', Desarquivar.container).attr('disabled', false).val(),
			DestinatarioSetorId: $('.ddlDestinatarioSetor', Desarquivar.container).attr('disabled', false).val(),
			ArquivoId: $('.ddlArquivos', Desarquivar.container).attr('disabled', false).val(),
			Protocolo: { NumeroTexto: $('.txtNumeroProcDoc', Desarquivar.container.find('.fsFiltrosDesarquivamento')).val() },
			AtividadeId: $('.ddlAtividades', Desarquivar.container).val(),
			AtividadeSituacaoId: $('.ddlSituacaoAtividade', Desarquivar.container).val(),
			InteressadoNomeRazao: $('.txtNomeInteressado', Desarquivar.container).val(),
			InteressadoCpfCnpj: $('.txtCpfCnpjInteressado', Desarquivar.container).val(),
			EmpreendimentoUf: $('.ddlEstado', Desarquivar.container).val(),
			EmpreendimentoMunicipio: $('.ddlMunicipioEmpreendimento', Desarquivar.container).val(),
			ArquivoEstanteId: $('.ddlEstantes', Desarquivar.container).val(),
			ArquivoPrateleiraModoId: $('.ddlPrateleirasModo', Desarquivar.container).val(),
			ArquivoIdentificacao: $('.ddlIdentificacao :selected', Desarquivar.container).text()
		};

		if ($('.ddlIdentificacao :selected', Desarquivar.container).val() == 0) {
			filtros.ArquivoIdentificacao = '';
		}

		MasterPage.carregando(true);
		$.ajax({
			url: Desarquivar.settings.urls.filtrar,
			data: JSON.stringify(filtros),
			dataType: 'json',
			cache: false,
			async: false,
			type: 'POST',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Desarquivar.container), response.Msg);
				}

				if (response.EhValido) {
					$('.tabItens tbody', Desarquivar.container).html(response.HtmlItens);
				}

				Listar.atualizarEstiloTable(Desarquivar.container);
				MasterPage.redimensionar();
			}
		});
		MasterPage.carregando(false);
		if ($('.ddlRemetenteSetor option', Desarquivar.container).length <= 1) {
			$('.ddlRemetenteSetor', Desarquivar.container).attr('disabled', 'disabled')
		}
		if ($('.ddlArquivos option', Desarquivar.container).length <= 1) {
			$('.ddlArquivos', Desarquivar.container).attr('disabled', 'disabled')
		}
	},

	desarquivar: function () {
		Mensagem.limpar(MasterPage.getContent(Desarquivar.container));

		var DesarquivarVM = {
			RemetenteSetor: { Id: $('.ddlRemetenteSetor', Desarquivar.container).attr('disabled', false).val() },
			DestinatarioSetor: { Id: $('.ddlDestinatarioSetor', Desarquivar.container).attr('disabled', false).val() },
			Tramitacoes: [],
			Arquivo: { Id: $('.ddlArquivos', Desarquivar.container).val() }
		};

		if (DesarquivarVM.RemetenteSetor.Id <= 0) {
			var arrayMensagem = new Array();
			arrayMensagem.push(Desarquivar.settings.Mensagens.SetorOrigemObrigatorio);
			Mensagem.gerar(Desarquivar.container, arrayMensagem);
			return;
		} else {
			if (DesarquivarVM.DestinatarioSetor.Id <= 0) {
				var arrayMensagem = new Array();
				arrayMensagem.push(Desarquivar.settings.Mensagens.SetorDestinoObrigratorio);
				Mensagem.gerar(Desarquivar.container, arrayMensagem);
				return;
			}
		}

		$('.tabTramitacoesArquivadas tbody tr:visible').each(function (index, tr) {
			if ($('.ckbIsSelecionado', tr).is(':checked')) {
				var Tramitacao = {
					Id: $(tr).find('.hdnTramitacaoId').val(),
					Protocolo: {
						Id: $(tr).find('.hdnProtocoloId').val(),
						Numero: $(tr).find('.hdnProtocoloNumero').val(),
						IsProcesso: $(tr).find('.hdnIsProcesso').val().toLowerCase() === "true" ? true : false
					}
				};
				DesarquivarVM.Tramitacoes.push(Tramitacao);
			}
		});

		MasterPage.carregando(true);
		$.ajax({ url: Desarquivar.settings.urls.desarquivar,
			data: JSON.stringify(DesarquivarVM),
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
					Mensagem.gerar(MasterPage.getContent(Desarquivar.container), response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
		if ($('.ddlRemetenteSetor option', Desarquivar.container).length <= 1) {
			$('.ddlRemetenteSetor', Desarquivar.container).attr('disabled', 'disabled')
		}
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
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

EnviarExterno = {
	settings: {
		objetoEnviar: null,
		container: {},

		urls: {
			enviar: '',
			setoresRemetente: '',
			funcionariosDestinatario: '',
			todasTramitacoes: '',
			addTramitacaoNumeroProcDoc: '',
			carregarPartialTemplateTramitacoes: '',
			visualizarHistorico: '',
			abrirPdf: '',
			visualizarProc: null,
			visualizarDoc: null
		},
		msgs: null
	},

	load: function (content, options) {
		if (options) {
			$.extend(EnviarExterno.settings, options);
		}

		EnviarExterno.settings.container = MasterPage.getContent(content);

		$('.ddlSetoresRemetente', content).change(EnviarExterno.remetenteSetorChange);
		$('.rdbOpcaoBuscaProcesso', content).click(EnviarExterno.opcaoBuscarTramitacoes);
		$('.btnAddProcDoc', content).click(EnviarExterno.addTramitacaoPorNumeroProtocolo);
		content.delegate('.ckbIsTodosSelecionado', 'change', EnviarExterno.checkTodasTramitacoes);
		content.delegate('.trTramitacao', 'click ', EnviarExterno.onTrTramitacaoClick);
		content.delegate('.ckbIsSelecionado', 'change', EnviarExterno.onCkbSelecionavelChange);
		content.delegate('.btnCancelarEnvio', 'click', EnviarExterno.onTrExcluirLinha);
		content.delegate('.btnHistorico', 'click', EnviarExterno.visualizarTelaClick);
		content.delegate('.btnPdf', 'click', EnviarExterno.abrirPdfClick);
		content.delegate('.btnVisualizar', 'click', EnviarExterno.onVisualizarProtocolo);

		$('.ddlSetoresRemetente', content).focus();
		EnviarExterno.marcarTodos();
	},

	onVisualizarProtocolo: function () {
		var linha = $(this, EnviarExterno.container).closest('tr')
		var id = $('.hdnProtocoloId', linha).val();
		var url = ($('.hdnIsProcesso', linha).val() == "True") ? EnviarExterno.settings.urls.visualizarProc : EnviarExterno.settings.urls.visualizarDoc;
		Modal.abrir(url + '/' + id, null, function (container) { Modal.defaultButtons(container); }, Modal.tamanhoModalGrande);
	},

	remetenteSetorChange: function () {
		Mensagem.limpar(EnviarExterno.settings.container);
		var container = EnviarExterno.settings.container;
		var setorDestinatarioId = parseInt($('.ddlSetoresRemetente', container).val());

		$('.divEnviarExternoContent', container).addClass('hide');
		$('input.rdbOpcaoBuscaProcesso:checked', container).removeAttr('checked');
		$('input.rdbOpcaoBuscaProcesso[value=1]', container).attr('checked', 'checked');

		if (setorDestinatarioId == 0) {
			$('.dataGridTramitacoes', container).remove();
			$('.btnEnviar', container).button({ disabled: false });
			return;
		}

		$('input.rdbOpcaoBuscaProcesso:checked', container).removeAttr('checked');
		$('input.rdbOpcaoBuscaProcesso[value=1]', container).attr('checked', 'checked');
		EnviarExterno.opcaoBuscarTramitacoes();

		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	},

	opcaoBuscarTramitacoes: function () {
		var container = EnviarExterno.settings.container;
		var opcaoBusca = parseInt($('input.rdbOpcaoBuscaProcesso:checked', container).val());

		switch (opcaoBusca) {
			case 1:
				$('.divNumeroProtocolo', container).hide();
				EnviarExterno.listarTodasTramitacoes();
				break;

			case 2:
				$('.divNumeroProtocolo,.divTipoProcesso', container).show();
				$('.divTipoDocumento', container).hide();
				EnviarExterno.addTabTramitacoesTemplate();
				break;

			case 3:
				$('.divNumeroProtocolo,.divTipoDocumento', container).show();
				$('.divTipoProcesso', container).hide();
				EnviarExterno.addTabTramitacoesTemplate();
				break;
		}

		container.find('.divNumeroProtocolo select option:first').attr('selected', 'selected');
		Listar.atualizarEstiloTable($('.dataGridTable', container));
	},

	addTabTramitacoesTemplate: function () {
		var container = EnviarExterno.settings.container;
		var tab = $('.dataGridTramitacoes', container);
		var div = $('.divTramitacoes', container);
		Mensagem.limpar(MasterPage.getContent(container));

		if ($('.hdnIsTabTodos', tab).val() === 'true') {
			$.ajax({ url: EnviarExterno.settings.urls.carregarPartialTemplateTramitacoes, cache: false, async: false, type: 'GET',
				error: function (jqXHR, status, errorThrown) {
					Aux.error(jqXHR, status, errorThrown, MasterPage.getContent(EnviarExterno.settings.container));
				},
				success: function (response, textStatus, XMLHttpRequest) {
					$('.dataGridTramitacoes', container).remove();
					$(div, container).append(response);
					$('.hdnIsTabTodos', div).val(false);
					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(container, response.Msg);
					}
				}
			});
		}
	},

	onTrExcluirLinha: function () {
		$(this).closest('tr').remove();
	},

	limparProtocolosEmPosse: function () {
		$('.rdbOpcaoBuscaProcesso', EnviarExterno.settings.container).removeAttr('checked');
		$('.dataGridTramitacoes', EnviarExterno.settings.container).remove();
	},

	listarTodasTramitacoes: function () {
		var container = EnviarExterno.settings.container;
		Mensagem.limpar(MasterPage.getContent(container));

		var json = {
			remetenteId: parseInt($('.hdnRemetenteId', container).val()),
			SetorId: (parseInt($('.ddlSetoresRemetente', container).val()) == NaN) ? 0 : parseInt($('.ddlSetoresRemetente', container).val())
		};
		var div = $('.divTramitacoes', container);

		if (json.Id <= 0) {
			return;
		}

		$.ajax({ url: EnviarExterno.settings.urls.todasTramitacoes, data: json, cache: false, async: false, type: 'GET',
			error: function (jqXHR, status, errorThrown) {
				Aux.error(jqXHR, status, errorThrown, MasterPage.getContent(EnviarExterno.settings.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				$('.dataGridTramitacoes', container).remove();
				$(div, container).append(response);

				if ($(div, container).find(".trTramitacao").length > 0) {
					$('.divEnviarExternoContent', container).removeClass("hide");
					$('.btnEnviar', container).button({ disabled: false });
				} else {
					$('.btnEnviar', container).button({ disabled: true });
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(container, response.Msg);
				}
			}
		});
	},

	checkTodasTramitacoes: function () {
		$('.tabTramitacoes tbody .ckbIsSelecionado', EnviarExterno.settings.container)
			.attr('checked', $(this).is(':checked'))
			.each(function () {
				$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
			});
	},

	onTrTramitacaoClick: function (e) {
		if ($(e.target).is('input')) return;
		$('.ckbIsSelecionado', this).attr('checked', !$('.ckbIsSelecionado', this).is(':checked'));
		$(this).toggleClass('linhaSelecionada', $('.ckbIsSelecionado', this).is(':checked'));
		$('.tabTramitacoes .ckbIsTodosSelecionado', EnviarExterno.settings.container).attr('checked', false);
		EnviarExterno.marcarTodos();
	},

	onCkbSelecionavelChange: function () {
		$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
		$('.tabTramitacoes .ckbIsTodosSelecionado', EnviarExterno.settings.container).attr('checked', false);
		EnviarExterno.marcarTodos();
	},

	marcarTodos: function () {
		var isAllChk = true;
		$('.tabTramitacoes tbody', EnviarExterno.settings.container).find("tr").each(function (idx, item) {
			isAllChk = isAllChk && $('.ckbIsSelecionado', item).is(':checked');
		});
		$('.tabTramitacoes .ckbIsTodosSelecionado', EnviarExterno.settings.container).attr('checked', isAllChk);
	},

	addTramitacaoPorNumeroProtocolo: function () {
		var container = EnviarExterno.settings.container;
		Mensagem.limpar(MasterPage.getContent(container));

		var tab = $('.tabTramitacoes', container);
		var json = {};
		var opcaoBusca = parseInt($('input.rdbOpcaoBuscaProcesso:checked', container).val());

		var numero = $.trim($('.txtNumeroProcDoc', container).val());
		var tipo = 0;
		var isProcesso = false;

		if (opcaoBusca == 2) { //processo
			tipo = parseInt($('.ddlProcessoTipo', container).val());
			isProcesso = true;

		} else if (opcaoBusca == 3) { //documento
			tipo = parseInt($('.ddlDocumentoTipo', container).val());
			isProcesso = false;
		}

		var dado = {
			SetorId: isNaN(parseInt($('.ddlSetoresRemetente', container).val())) ? 0 : parseInt($('.ddlSetoresRemetente', container).val()),
			IsProcesso: isProcesso,
			Numero: numero,
			Tipo: tipo
		};

		$.ajax({ url: EnviarExterno.settings.urls.addTramitacaoNumeroProcDoc, data: dado, type: 'GET', cache: false,
			async: false, dataType: 'json', contentType: "application/json; charset=utf-8",
			error: function (jqXHR, status, errorThrown) {
				Aux.error(jqXHR, status, errorThrown, MasterPage.getContent(EnviarExterno.settings.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				var tram = response.Tramitacao;
				if (tram) {
					var linha = $('.trTramitacaoTemplate').clone().removeClass('trTramitacaoTemplate');

					linha.find('.hdnTramitacaoId').val(tram.Id);
					linha.find('.hdnTramitacaoHistoricoId').val(tram.HistoricoId);
					linha.find('.hdnProtocoloId').val(tram.Protocolo.Id);
					linha.find('.hdnProtocoloNumero').val(tram.Protocolo.Numero);
					linha.find('.hdnIsProcesso').val(tram.Protocolo.IsProcesso);

					linha.find('.trNumero').html(tram.Protocolo.Numero).attr('title', tram.Protocolo.Numero);
					linha.find('.trDataEnvio').html(tram.DataEnvio.DataHoraTexto).attr('title', tram.DataEnvio.DataHoraTexto);
					linha.find('.trSetorRemetente').html(tram.RemetenteSetor.Sigla).attr('title', tram.RemetenteSetor.Nome);
					linha.find('.trObjetivo').html(tram.Objetivo.Texto).attr('title', tram.Objetivo.Texto);
					linha.find('.trDataRecebido').html(tram.DataRecebido.DataTexto).attr('title', tram.DataRecebido.DataHoraTexto);

					if (!tram.HistoricoId || tram.HistoricoId <= 0) {
						linha.find('.btnHistorico, .btnPdf').remove();
					}

					if (isProcesso) {
						linha.find('.btnDocumento').remove();
					} else {
						linha.find('.btnProcesso').remove();
					}

					$('tbody:last', tab).append(linha);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(container, response.Msg);
				} else {
					container.find('.divNumeroProtocolo select option:first').attr('selected', 'selected');
					$('.txtNumeroProcDoc', container).val('');
				}
			}
		});
	},

	visualizarTelaClick: function () {
		var id = parseInt($('.hdnProtocoloId', $(this).closest('tr')).val());
		var tipo = $('.hdnIsProcesso', $(this).closest('tr')).val().toLowerCase() === "true" ? 1 : 2;

		Modal.abrir(EnviarExterno.settings.urls.visualizarHistorico, { id: id, tipo: tipo }, function (context) {
			Modal.defaultButtons(context);
		});
	},

	abrirPdfClick: function () {
		var id = isNaN(parseInt($('.hdnTramitacaoHistoricoId', $(this).closest('tr')).val())) ? 0 : parseInt($('.hdnTramitacaoHistoricoId', $(this).closest('tr')).val());
		var tipo = $('.hdnIsProcesso', $(this).closest('tr')).val().toLowerCase() === "true" ? 1 : 2;

		if (id > 0) {
			MasterPage.redireciona(EnviarExterno.settings.urls.abrirPdf + "?id=" + id + '&tipo=' + tipo + "&obterHistorico=" + true);
			MasterPage.carregando(false);
		}
	},

	criarObjetoEnviarTramitacao: function (content) {
		var _tramitacoes = new Array();
		$('.tabTramitacoes tr', content).each(function (index, tr) {
			if ($(tr).find('.ckbIsSelecionado').attr('checked')) {
				var Tramitacao = {
					Id: $(tr).find('.hdnTramitacaoId').val(),
					HistoricoId: $(tr).find('.hdnTramitacaoHistoricoId').val(),
					Protocolo: {
						Id: $(tr).find('.hdnProtocoloId').val(),
						IsProcesso: $(tr).find('.hdnIsProcesso').val().toLowerCase() === "true" ? true : false,
						Numero: $(tr).find('.hdnProtocoloNumero').val()
					}
				};
				_tramitacoes.push(Tramitacao);
			}
		});

		var contentJson = {
			Tramitacoes: _tramitacoes,
			Enviar: {
				ObjetivoId: $('.ddlObjetivos', content).val(),
				Despacho: $('.txtDespacho', content).val(),
				RemetenteSetor: { Id: $('.ddlSetoresRemetente', content).val() },
				OrgaoExterno: { Id: $('.ddlOrgaoExterno', content).val() },
				Remetente: { Id: $('.hdnRemetenteId', content).val() },
				Destinatario: { Nome: $('.txtFuncExterno', content).val() },
				TramitacaoTipo: $('.hdnEnviarTramitacaoTipo', content).val(),
				SituacaoId: $('.hdnEnviarSituacaoId', content).val()
			}
		};

		return contentJson;
	},

	enviar: function () {
		var container = MasterPage.getContent(EnviarExterno.settings.container);
		var contentJson = EnviarExterno.criarObjetoEnviarTramitacao(container);

		MasterPage.carregando(true);
		$.ajax({ url: EnviarExterno.settings.urls.enviar, data: JSON.stringify(contentJson), type: 'POST', cache: false,
			async: false, dataType: 'json', contentType: "application/json; charset=utf-8",
			error: function (jqXHR, status, errorThrown) {
				Aux.error(jqXHR, status, errorThrown, container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.IsTramitacoesEnviadas) {
					//redireiciona para o Listar, em caso de sucesso
					if (typeof response.UrlRedireciona != "undefined" && response.UrlRedireciona !== null) {
						MasterPage.redireciona(response.UrlRedireciona);
						return;
					}
				}
				else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
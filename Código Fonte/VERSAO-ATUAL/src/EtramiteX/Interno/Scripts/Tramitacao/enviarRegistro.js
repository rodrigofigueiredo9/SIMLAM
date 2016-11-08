/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

EnviarRegistro = {
	settings: {
		objetoEnviar: null,
		container: {},

		urls: {
			enviarRegistro: '',
			remetentes: '',
			funcionariosDestinatario: '',
			todasTramitacoesRegistro: '',
			addTramitacaoNumeroProcDoc: '',
			carregarPartialTemplateTramitacoes: '',
			visualizarHistorico: '',
			abrirPdf: '',
			validarTipoSetor: ''
		},
		msgs: null
	},

	load: function (content, options) {

		if (options) {
			$.extend(EnviarRegistro.settings, options);
		}

		EnviarRegistro.settings.container = MasterPage.getContent(content);

		$('.ddlRemetentes', content).change(EnviarRegistro.remetenteFuncionarioChange);
		$('.ddlSetoresRemetente', content).change(EnviarRegistro.remetenteSetorChange);
		$('.ddlSetoresDestinatario', content).change(EnviarRegistro.destinatarioSetorChange);
		$('.rdbOpcaoBuscaProcesso', content).change(EnviarRegistro.opcaoBuscarTramitacoes);
		$('.btnAddProcDoc', content).click(EnviarRegistro.addTramitacaoPorNumeroProtocolo);

		content.delegate('.ckbIsTodosSelecionado', 'change', EnviarRegistro.checkTodasTramitacoes);
		content.delegate('.trTramitacao', 'click ', EnviarRegistro.onTrTramitacaoClick);
		content.delegate('.ckbIsSelecionado', 'change', EnviarRegistro.onCkbSelecionavelChange);
		content.delegate('.btnCancelarEnvio', 'click', EnviarRegistro.onTrExcluirLinha);
		content.delegate('.btnHistorico', 'click', EnviarRegistro.visualizarTelaClick);
		content.delegate('.btnPdf', 'click', EnviarRegistro.abrirPdfClick);

		EnviarRegistro.marcarTodos();
		$('.ddlSetoresRemetente', content).focus();
		if ($('.ddlSetoresRemetente', content).val() == 0) {
			$('.ddlRemetentes', content).attr('disabled', 'disabled').addClass('disabled');
		}
	},

	remetenteSetorChange: function () {
		Mensagem.limpar(EnviarRegistro.settings.container);
		var container = EnviarRegistro.settings.container;
		EnviarRegistro.limparProtocolosEmPosse();

		$('.divNumeroProtocolo', container).hide();
		$('.divEnviarRegistroContent', container).addClass("hide");
		$('.ddlRemetentes', container).ddlClear({ disabled: false });

		$('input.rdbOpcaoBuscaProcesso:checked', container).removeAttr('checked');
		$('input.rdbOpcaoBuscaProcesso[value=1]', container).attr('checked', 'checked');

		var dado = { setorId: isNaN(parseInt($('.ddlSetoresRemetente', container).val())) ? 0 : parseInt($('.ddlSetoresRemetente', container).val()) };

		if (dado.setorId == 0) {
			$('.dataGridTramitacoes', container).remove();
			$('.btnEnviar', container).button({ disabled: false });
			$('.ddlRemetentes', container).attr('disabled', 'disabled').addClass('disabled');
			return;
		}

		$.ajax({ url: EnviarRegistro.settings.urls.validarTipoSetor, data: dado, cache: false, async: false,
			type: 'GET', dataType: 'json', contentType: "application/json; charset=utf-8",
			error: function (jqXHR, status, errorThrown) {
				Aux.error(jqXHR, status, errorThrown, container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.IsSetorValido) {
					EnviarRegistro.setarChecksTramitacoes();

					var confCascate = { url: EnviarRegistro.settings.urls.remetentes, disabled: false };
					$('.ddlSetoresRemetente', container).ddlCascate($('.ddlRemetentes', container), confCascate);
					EnviarRegistro.remetenteFuncionarioChange();
				}
				else {
					$('.dataGridTramitacoes', container).remove();
					EnviarRegistro.setarChecksTramitacoes();

					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar($(container).closest('.divEnviarRegistro'), response.Msg);
					}
				}
			}
		});
	},

	setarChecksTramitacoes: function () {
		var container = EnviarRegistro.settings.container;

		if (parseInt($('.ddlRemetentes', container).val()) > 0 && $('.divEnviarRegistroContent', container).hasClass('hide')) {
			$('.btnEnviar', container).button({ disabled: true });
		} else {
			$('.btnEnviar', container).button({ disabled: false });
		}
	},

	remetenteFuncionarioChange: function () {
		Mensagem.limpar(EnviarRegistro.settings.container);
		var container = EnviarRegistro.settings.container;
		var dado = { setorId: isNaN(parseInt($('.ddlSetoresRemetente', container).val())) ? 0 : parseInt($('.ddlSetoresRemetente', container).val()) };

		$('.divEnviarRegistroContent', container).addClass("hide");

		if (parseInt($('.ddlRemetentes', container).val()) === 0) {
			$('.dataGridTramitacoes', container).remove();
			$('.btnEnviar', container).button({ disabled: false });
			return;
		}
		else {
			$('input.rdbOpcaoBuscaProcesso:checked', container).removeAttr('checked');
			$('input.rdbOpcaoBuscaProcesso[value=1]', container).attr('checked', 'checked');
			EnviarRegistro.opcaoBuscarTramitacoes();
		}
	},

	destinatarioSetorChange: function () {
		var ddlB = $('.ddlDestinatarios', EnviarRegistro.settings.container);
		var ddlA = $(this, EnviarRegistro.settings.container);
		var url = EnviarRegistro.settings.urls.funcionariosDestinatario;

		ddlA.ddlCascate(ddlB, { url: url, disabled: false });
	},

	opcaoBuscarTramitacoes: function () {
		var container = EnviarRegistro.settings.container;
		var opcaoBusca = parseInt($('input.rdbOpcaoBuscaProcesso:checked', container).val());

		if (opcaoBusca == 1) {
			$('.divNumeroProtocolo', container).hide();
			EnviarRegistro.listarTodasTramitacoes();
		} else { // 2 = processo/documento
			$('.divNumeroProtocolo,.divTipoProcesso', container).show();
			$('.divTipoDocumento', container).hide();
			EnviarRegistro.addTabTramitacoesTemplate();
		}

		container.find('.divNumeroProtocolo select option:first').attr('selected', 'selected');
	},

	addTabTramitacoesTemplate: function () {
		var container = EnviarRegistro.settings.container;
		var tab = $('.dataGridTramitacoes', container);
		var div = $('.divTramitacoes', container);

		if ($('.hdnIsTabTodos', tab).val() === 'true') {
			$.ajax({ url: EnviarRegistro.settings.urls.carregarPartialTemplateTramitacoes, cache: false, async: false, type: 'GET',
				error: function (jqXHR, status, errorThrown) {
					Aux.error(jqXHR, status, errorThrown, container);
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

	limparProtocolosEmPosse: function () {
		$('.dataGridTramitacoes', EnviarRegistro.settings.container).remove();
	},

	listarTodasTramitacoes: function () {
		var container = EnviarRegistro.settings.container;
		Mensagem.limpar(container);

		var json = {
			remetenteId: isNaN(parseInt($('.ddlRemetentes', container).val())) ? 0 : parseInt($('.ddlRemetentes', container).val()),
			SetorId: isNaN(parseInt($('.ddlSetoresRemetente', container).val())) ? 0 : parseInt($('.ddlSetoresRemetente', container).val())
		};
		var div = $('.divTramitacoes', container);

		if (json.remetenteId <= 0) {
			return;
		}

		$.ajax({ url: EnviarRegistro.settings.urls.todasTramitacoesRegistro, data: json, cache: false, async: false, type: 'GET',
			error: function (jqXHR, status, errorThrown) {
				Aux.error(jqXHR, status, errorThrown, container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				$('.dataGridTramitacoes', container).remove();
				$(div, container).append(response);

				if ($(div, container).find(".trTramitacao").length > 0) {
					$('.divEnviarRegistroContent', container).removeClass("hide");
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(container, response.Msg);
				}
				EnviarRegistro.setarChecksTramitacoes();
			}
		});
	},

	checkTodasTramitacoes: function () {
		$('.tabTramitacoes tbody .ckbIsSelecionado', EnviarRegistro.settings.container)
			.attr('checked', $(this).is(':checked'))
			.each(function () {
				$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
			});
	},

	onTrTramitacaoClick: function (e) {
		if ($(e.target).is('input')) return;
		$('.ckbIsSelecionado', this).attr('checked', !$('.ckbIsSelecionado', this).is(':checked'));
		$(this).toggleClass('linhaSelecionada', $('.ckbIsSelecionado', this).is(':checked'));
		EnviarRegistro.marcarTodos();
	},

	onCkbSelecionavelChange: function () {
		$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
		EnviarRegistro.marcarTodos();
	},

	marcarTodos: function () {
		var isAllChk = true;
		$('.tabTramitacoes tbody', EnviarRegistro.settings.container).find("tr").each(function (idx, item) {
			isAllChk = isAllChk && $('.ckbIsSelecionado', item).is(':checked');
		});
		$('.tabTramitacoes .ckbIsTodosSelecionado', EnviarRegistro.settings.container).attr('checked', isAllChk);
	},

	addTramitacaoPorNumeroProtocolo: function () {
		var container = EnviarRegistro.settings.container;
		Mensagem.limpar(container);
		var tab = $('.tabTramitacoes', container);
		var opcaoBusca = parseInt($('input.rdbOpcaoBuscaProcesso:checked', container).val());
		var erroMsg = [];

		var parametros = {
			SetorId: isNaN(parseInt($('.ddlSetoresRemetente', container).val())) ? 0 : parseInt($('.ddlSetoresRemetente', container).val()),
			Numero: $('.txtNumeroProcDoc', container).val().trim(),
			FuncionarioId: isNaN(parseInt($('.ddlRemetentes', container).val())) ? 0 : parseInt($('.ddlRemetentes', container).val())
		};

		$.ajax({ url: EnviarRegistro.settings.urls.addTramitacaoNumeroProcDoc, data: parametros, type: 'GET', cache: false,
			async: false, dataType: 'json', contentType: 'application/json; charset=utf-8',
			error: function (jqXHR, status, errorThrown) {
				Aux.error(jqXHR, status, errorThrown, container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				var tram = response.Tramitacao;
				if (tram) {

					if (EnviarRegistro.existeAssociado(tram.Protocolo.Id.toString(), tab, 'hdnProtocoloId')) {
						erroMsg.push(EnviarRegistro.settings.msgs.ProtocoloJaAdicionado);
						Mensagem.gerar(container, erroMsg);
						return;
					}

					var linha = $('.trTramitacaoTemplate').clone().removeClass('trTramitacaoTemplate');

					linha.find('.hdnTramitacaoId').val(tram.Id);
					linha.find('.hdnTramitacaoHistoricoId').val(tram.HistoricoId);

					linha.find('.hdnProtocoloId').val(tram.Protocolo.Id);
					linha.find('.hdnProtocoloNumero').val(tram.Protocolo.Numero);
					linha.find('.hdnIsProcesso').val(tram.Protocolo.IsProcesso);

					linha.find('.trNumero').html(tram.Protocolo.Numero).attr('title', tram.Protocolo.Numero);
					linha.find('.trDataEnvio').html(tram.DataEnvio.DataHoraTexto).attr('title', tram.DataEnvio.DataHoraTexto);
					linha.find('.trSetorRemetente').html(tram.RemetenteSetor.Sigla).attr('title', tram.RemetenteSetor.Nome);
					linha.find('.trRemetente').html(tram.Remetente.Nome).attr('title', tram.Remetente.Nome);
					linha.find('.trObjetivo').html(tram.Objetivo.Texto).attr('title', tram.Objetivo.Texto);
					linha.find('.trDataRecebido').html(tram.DataRecebido.DataTexto).attr('title', tram.DataRecebido.DataHoraTexto);

					if (!tram.HistoricoId || tram.HistoricoId <= 0) {
						linha.find('.btnHistorico, .btnPdf').remove();
					}

					if (tram.Protocolo.IsProcesso) {
						linha.find('.btnDocumento').remove();
					} else {
						linha.find('.btnProcesso').remove();
					}

					$('tbody:last', tab).append(linha);

					EnviarRegistro.setarChecksTramitacoes();
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

	onTrExcluirLinha: function () {
		$(this).closest('tr').remove();
	},

	criarObjetoEnviarRegistroTramitacao: function (content) {
		var _tramitacoes = new Array();
		$('.tabTramitacoes tr.linhaSelecionada', content).each(function (index, tr) {
			if ($(tr).find('.ckbIsSelecionado').attr('checked')) {
				var Tramitacao = {
					Id: $(tr).find('.hdnTramitacaoId').val(),
					Tid: $(tr).find('.hdnTramitacaoTid').val(),
					Tipo: $(tr).find('.hdnTramitacaoTipo').val(),
					RemetenteId: $(tr).find('.ddlRemetentes').val(),
					RemetenteSetorId: $(tr).find('.hdnRemetenteSetorId').val(),
					ObjetivoId: $(tr).find('.hdnObjetivoId').val(),
					DataEnvio: { DataTexto: $(tr).find('.trDataEnvio').val() },
					DataRecebido: { DataTexto: $(tr).find('.trDataRecebido').val() },
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
				Remetente: { Id: $('.ddlRemetentes', content).val() },
				Destinatario: { Id: $('.ddlDestinatarios', content).val() },
				DestinatarioSetor: { Id: $('.ddlSetoresDestinatario', content).val() },
				Executor: { Id: $('.hdnExecutorId', content).val() },
				TramitacaoTipo: $('.hdnEnviarTramitacaoTipo', content).val(),
				SituacaoId: $('.hdnEnviarSituacaoId', content).val()
			}
		};

		return contentJson;
	},

	enviar: function () {
		var container = EnviarRegistro.settings.container;
		var contentJson = EnviarRegistro.criarObjetoEnviarRegistroTramitacao(container);

		MasterPage.carregando(true);
		$.ajax({ url: EnviarRegistro.settings.urls.enviarRegistro, data: JSON.stringify(contentJson), type: 'POST', cache: false,
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
	},

	visualizarTelaClick: function () {
		var tr = $(this).closest('tr');
		var url = EnviarRegistro.settings.urls.visualizarHistorico;
		var id = isNaN(parseInt($('.hdnProtocoloId', tr).val())) ? 0 : parseInt($('.hdnProtocoloId', tr).val());
		var tipo = $('.hdnIsProcesso', tr).val().toLowerCase() === "true" ? 1 : 2;

		Modal.abrir(url, { id: id, tipo: tipo }, function (context) {
			Modal.defaultButtons(context);
		});
	},

	abrirPdfClick: function () {
		var url = EnviarRegistro.settings.urls.abrirPdf;
		var tr = $(this).closest('tr');
		var id = isNaN(parseInt($('.hdnTramitacaoHistoricoId', tr).val())) ? 0 : parseInt($('.hdnTramitacaoHistoricoId', tr).val());
		var tipo = $('.hdnIsProcesso', tr).val().toLowerCase() === "true" ? 1 : 2;
		if (id > 0) {
			MasterPage.redireciona(url + "?id=" + id + "&tipo=" + tipo + "&obterHistorico=" + true);
			MasterPage.carregando(false);
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
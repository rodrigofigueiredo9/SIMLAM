/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

Enviar = {
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
			validarTipoSetor: '',
			visualizarProc: null,
			visualizarDoc: null
		},
		msgs: {}
	},

	load: function (content, options) {

		if (options) {
			$.extend(Enviar.settings, options);
		}

		Enviar.settings.container = MasterPage.getContent(content);

		$('.ddlObjetivos', content).change(Enviar.motivoChange);
		$('.ddlSetoresRemetente', content).change(Enviar.remetenteSetorChange);
		$('.ddlSetoresDestinatario', content).change(Enviar.destinatarioSetorChange);
		$('.ddlFormaEnvio', content).change(Enviar.formaEnvioChange);
		$('.rdbOpcaoBuscaProcesso', content).click(Enviar.opcaoBuscarTramitacoes);
		$('.btnAddProcDoc', content).click(Enviar.addTramitacaoPorNumeroProtocolo);

		content.delegate('.ckbIsTodosSelecionado', 'change', Enviar.checkTodasTramitacoes);
		content.delegate('.ckbIsSelecionado', 'change', Enviar.onCkbSelecionavelChange);
		content.delegate('.trTramitacao', 'click', Enviar.onTrTramitacaoClick);

		content.delegate('.btnCancelarEnvio', 'click', Enviar.onTrExcluirLinha);
		content.delegate('.btnHistorico', 'click', Enviar.visualizarTelaClick);
		content.delegate('.btnPdf', 'click', Enviar.abrirPdfClick);
		content.delegate('.btnVisualizar', 'click', Enviar.onVisualizarProtocolo);

		$('.ddlSetoresRemetente', content).focus();
		Enviar.marcarTodos();
	},

	onVisualizarProtocolo: function(){
		var linha = $(this, Enviar.container).closest('tr')
		var id = $('.hdnProtocoloId', linha).val();
		var url = ($('.hdnIsProcesso', linha).val() == "True") ? Enviar.settings.urls.visualizarProc : Enviar.settings.urls.visualizarDoc;
		Modal.abrir(url + '/' + id, null, function (container) { Modal.defaultButtons(container); }, Modal.tamanhoModalGrande);
	},

	remetenteSetorChange: function () {
		var container = Enviar.settings.container;
		Mensagem.limpar(container);
		var dado = { setorId: isNaN(parseInt($('.ddlSetoresRemetente', container).val())) ? 0 : parseInt($('.ddlSetoresRemetente', container).val()) };

		$('input.rdbOpcaoBuscaProcesso:checked', container).removeAttr('checked');
		$('input.rdbOpcaoBuscaProcesso[value=1]', container).attr('checked', 'checked');
		$('.divNumeroProtocolo', container).hide();
		var opcaoBusca = parseInt($('input.rdbOpcaoBuscaProcesso:checked', container).val());

		if (!$('.divEnviarContent', container).hasClass('hide')) {
			$('.divEnviarContent', container).addClass('hide');
		}

		if (dado.setorId == 0) {
			$('.dataGridTramitacoes', container).remove();
			$('.btnEnviar', container).button({ disabled: false });
			return;
		}

		$.ajax({ url: Enviar.settings.urls.validarTipoSetor, data: dado, cache: false, async: false,
			type: 'GET', dataType: 'json', contentType: "application/json; charset=utf-8",
			error: function (jqXHR, status, errorThrown) {
				Aux.error(jqXHR, status, errorThrown, container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(container, response.Msg);
				}

				if (response.IsSetorValido) {
					$('.divEnviarContent', container).removeClass('hide');

					if (opcaoBusca === 1 && dado.setorId > 0) {
						Enviar.listarTodasTramitacoes();
					}
				}
				Enviar.setarChecksTramitacoes();
			}
		});

		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	},

	setarChecksTramitacoes: function () {
		var container = Enviar.settings.container;

		if (parseInt($('.ddlSetoresRemetente', container).val()) > 0 && $('.divEnviarContent', container).hasClass('hide')) {
			$('.btnEnviar', container).button({ disabled: true });
		} else {
			$('.btnEnviar', container).button({ disabled: false });
		}
	},

	motivoChange: function () {
		var ddlA = $(this, Enviar.settings.container);
		$(".ddlFuncionario", Enviar.settings.container).toggleClass('hide', ddlA.val() == '19');
        $(".numAutuacao", Enviar.settings.container).toggleClass('hide', !(ddlA.val() == '19'));
        var doc = $('.hdnProtocoloTipo', Enviar.settings.container).toArray().find(x => x.value = 'Documento Avulso' && x.parentElement.parentElement.children[0].children[0].checked);
		if(!doc)
			Enviar.asterisco($('.lblDespacho', Enviar.settings.container), false);
		else
            Enviar.asterisco($('.lblDespacho', Enviar.settings.container), doc.value);
	},

	asterisco: function (control, exibir) {

		control.text(control.text().replace(' *', ''));

		if (exibir) {
			control.text(control.text() + ' *');
		}
	},

	destinatarioSetorChange: function () {
		var ddlB = $('.ddlDestinatarios', Enviar.settings.container);
		var ddlA = $(this, Enviar.settings.container);
		var url = Enviar.settings.urls.funcionariosDestinatario;

        ddlA.ddlCascate(ddlB, { url: url, disabled: false });

		var outros = $('.ddlSetoresDestinatario :selected', Enviar.settings.container)[0].label == 'Outros';
		$(".ddlFuncionario", Enviar.settings.container).toggleClass('hide', outros);
		$(".ddlFuncionario", Enviar.settings.container).toggleClass('hide', outros);
		$(".numAutuacao", Enviar.settings.container).toggleClass('hide', !outros);
		$(".pnlOutros", Enviar.settings.container).toggleClass('hide', !outros);
	},

	formaEnvioChange: function () {
		var ddlA = $(this, Enviar.settings.container);
		$(".rastreio", Enviar.settings.container).toggleClass('hide', !(ddlA.val() == '1' || ddlA.val() == '2'));
	},

	opcaoBuscarTramitacoes: function () {
		var container = Enviar.settings.container;
		var opcaoBusca = parseInt($('input.rdbOpcaoBuscaProcesso:checked', container).val());

		if (opcaoBusca == 1) {
			$('.divNumeroProtocolo', container).hide();
			Enviar.listarTodasTramitacoes();
		} else { // 2 = processo/documento
			$('.divNumeroProtocolo,.divTipoProcesso', container).show();
			$('.divTipoDocumento', container).hide();
			Enviar.addTabTramitacoesTemplate();
		}

		container.find('.divNumeroProtocolo select option:first').attr('selected', 'selected');

		Listar.atualizarEstiloTable($('.dataGridTable', container));
	},

	addTabTramitacoesTemplate: function () {
		var container = Enviar.settings.container;
		Mensagem.limpar(container);

		var tab = $('.dataGridTramitacoes', container);
		var div = $('.divTramitacoes', container);

		if ($('.hdnIsTabTodos', tab).val() === 'true') {
			$.ajax({ url: Enviar.settings.urls.carregarPartialTemplateTramitacoes, cache: false, async: false, type: 'GET',
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

	onTrExcluirLinha: function () {
		$(this).closest('tr').remove();
	},

	listarTodasTramitacoes: function () {
		var container = Enviar.settings.container;
		Mensagem.limpar(container);

		var json = {
			remetenteId: parseInt($('.hdnRemetenteId', container).val()),
			SetorId: isNaN(parseInt($('.ddlSetoresRemetente', container).val())) ? 0 : parseInt($('.ddlSetoresRemetente', container).val())
		};
		var div = $('.divTramitacoes', container);

		if (!$('.divEnviarContent', container).hasClass('hide')) {
			$('.divEnviarContent', container).addClass('hide');
		}

		if (json.remetenteId <= 0) {
			return;
		}

		$.ajax({ url: Enviar.settings.urls.todasTramitacoes, data: json, cache: false, async: false, type: 'GET',
			error: function (jqXHR, status, errorThrown) {
				Aux.error(jqXHR, status, errorThrown, container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				$('.dataGridTramitacoes', container).remove();
				$(div, container).append(response);
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(container, response.Msg);
				}
				else {
					$('.divEnviarContent', container).removeClass('hide');
				}

				Enviar.setarChecksTramitacoes();
			}
		});
	},

	checkTodasTramitacoes: function () {
		$('.tabTramitacoes tbody .ckbIsSelecionado', Enviar.settings.container)
			.attr('checked', $(this).is(':checked'))
			.each(function () {
				$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
			});
	},

	onTrTramitacaoClick: function (e) {
		if ($(e.target).is('input')) return;
		$('.ckbIsSelecionado', this).attr('checked', !$('.ckbIsSelecionado', this).is(':checked'));
		$(this).toggleClass('linhaSelecionada', $('.ckbIsSelecionado', this).is(':checked'));
		Enviar.marcarTodos();
	},

	onCkbSelecionavelChange: function () {
		$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
		Enviar.marcarTodos();
	},

	marcarTodos: function () {
		var isAllChk = true;
		$('.tabTramitacoes tbody', Enviar.settings.container).find("tr").each(function (idx, item) {
			isAllChk = isAllChk && $('.ckbIsSelecionado', item).is(':checked');
		});
		$('.tabTramitacoes .ckbIsTodosSelecionado', Enviar.settings.container).attr('checked', isAllChk);
	},

	addTramitacaoPorNumeroProtocolo: function () {
		var container = Enviar.settings.container;
		Mensagem.limpar(container);

		var tab = $('.tabTramitacoes', container);
		var opcaoBusca = parseInt($('input.rdbOpcaoBuscaProcesso:checked', container).val());
		var erroMsg = [];

		var parametros = {
			SetorId: isNaN(parseInt($('.ddlSetoresRemetente', container).val())) ? 0 : parseInt($('.ddlSetoresRemetente', container).val()),
			Numero: $('.txtNumeroProcDoc', container).val().trim()
		};

		$.ajax({ url: Enviar.settings.urls.addTramitacaoNumeroProcDoc, data: parametros, type: 'GET', cache: false,
			async: false, dataType: 'json', contentType: 'application/json; charset=utf-8',
			error: function (jqXHR, status, errorThrown) {
				Aux.error(jqXHR, status, errorThrown, container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				var tram = response.Tramitacao;
				if (tram) {

					if (Enviar.existeAssociado(tram.Protocolo.Id.toString(), tab, 'hdnProtocoloId')) {
						erroMsg.push(Enviar.settings.msgs.ProtocoloJaAdicionado);
						Mensagem.gerar(container, erroMsg);
						return;
					}

					var linha = $('.trTramitacaoTemplate', container).clone().removeClass('trTramitacaoTemplate');

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

					Enviar.setarChecksTramitacoes();
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

	criarObjetoEnviarTramitacao: function (content) {
		var _tramitacoes = new Array();
		$('.tabTramitacoes tr', content).each(function (index, tr) {
			if ($(tr).find('.ckbIsSelecionado').attr('checked')) {
				var Tramitacao = {
					Id: $(tr).find('.hdnTramitacaoId').val(),
					Tid: $(tr).find('.hdnTramitacaoTid').val(),
					Tipo: $(tr).find('.hdnTramitacaoTipo').val(),
					Remetente: { Id: $(tr).find('.hdnRemetenteId').val() },
					RemetenteSetor: { Id: $(tr).find('.hdnRemetenteSetorId').val() },
					Objetivo: { Id: $(tr).find('.hdnObjetivoId').val() },
					DataEnvio: { DataHoraTexto: $(tr).find('.trDataEnvio').val() },
					DataRecebido: { DataHoraTexto: $(tr).find('.trDataRecebido').val() },
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
				DestinatarioSetor: { Id: $('.ddlSetoresDestinatario', content).val() },
				Remetente: { Id: $('.hdnRemetenteId', content).val() },
				Destinatario: { Id: $('.ddlDestinatarios', content).val() },
				TramitacaoTipo: $('.hdnEnviarTramitacaoTipo', content).val(),
				SituacaoId: $('.hdnEnviarSituacaoId', content).val(),
				DestinoExterno: $('.txtDestinoExterno', content).val(),
				CodigoRastreio: $('.txtCodigoRastreio', content).val(),
				FormaEnvio: $('.ddlFormaEnvio :selected', content).val(),
				NumeroAutuacao: $('.txtNumeroAutuacao', content).val()
			}
		};
		return contentJson;
	},

	enviar: function () {
		var container = Enviar.settings.container;
		var contentJson = Enviar.criarObjetoEnviarTramitacao(container);

		MasterPage.carregando(true);
		$.ajax({ url: Enviar.settings.urls.enviar, data: JSON.stringify(contentJson), type: 'POST', cache: false,
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
		var url = Enviar.settings.urls.visualizarHistorico;
		var id = isNaN(parseInt($('.hdnProtocoloId', tr).val())) ? 0 : parseInt($('.hdnProtocoloId', tr).val());
		var tipo = $('.hdnIsProcesso', tr).val().toLowerCase() === "true" ? 1 : 2;

		Modal.abrir(url, { id: id, tipo: tipo }, function (context) {
			Modal.defaultButtons(context);
		});
	},

	abrirPdfClick: function () {
		var url = Enviar.settings.urls.abrirPdf;
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
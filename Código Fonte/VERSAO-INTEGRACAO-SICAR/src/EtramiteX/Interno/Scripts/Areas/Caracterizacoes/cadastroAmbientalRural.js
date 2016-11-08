/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />

CadastroAmbientalRural = {
	settings: {
		urls: {
			obterModuloFiscal: null,
			processar: null,
			obterSituacaoProcessamento: null,
			cancelar: null,
			obterArquivosProjeto: null,
			baixarArquivos: null,
			obterAreasProcessadas: null,
			mergiar: null,
			finalizar: null,
			desenhador: null,
			listarEmpreendimentos: null,
			arquivos: []
		},
		isEditar: false,
		isVisualizar: false,
		mensagens: {},
		textoAbrirModal: null,
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null,
		threadAtualizarSituacao: null,
		threadAtualizarSituacaoDelay: 1000 * 3,
		idsTelaProjetoGeograficoSituacao: null,
		idsTelaAreas: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(CadastroAmbientalRural.settings, options); }

		CadastroAmbientalRural.container = MasterPage.getContent(container);

		CadastroAmbientalRural.container.delegate('.ddlOcorreuAlteracaoApos2008', 'change', CadastroAmbientalRural.alterarAreaATP2008);
		CadastroAmbientalRural.container.delegate('.btnCancelarProcessamento', 'click', CadastroAmbientalRural.cancelar);
		CadastroAmbientalRural.container.delegate('.btnProcessar', 'click', CadastroAmbientalRural.processar);
		CadastroAmbientalRural.container.delegate('.btnReprocessar', 'click', CadastroAmbientalRural.reprocessar);
		CadastroAmbientalRural.container.delegate('.btnFinalizar', 'click', CadastroAmbientalRural.finalizar);
		CadastroAmbientalRural.container.delegate('.btnMapaVisualiar', 'click', CadastroAmbientalRural.abrirModalMapaVisualizar);
		CadastroAmbientalRural.container.delegate('.btnBaixar', 'click', CadastroAmbientalRural.baixarArquivoProcessado);
		CadastroAmbientalRural.container.delegate('.txtAtp2008', 'keyup', CadastroAmbientalRural.onChangeATPArea2008);
		CadastroAmbientalRural.container.delegate('.ddlMunicipio', 'change', CadastroAmbientalRural.onChangeMunicipio);
		CadastroAmbientalRural.container.delegate('.ddlVistoriaAprovacaoCAR', 'change', CadastroAmbientalRural.onChangeVistoriaAprovacaoCAR);
		CadastroAmbientalRural.container.delegate('.cbReservaLegalEmOutroCAR', 'click', CadastroAmbientalRural.onCBReservaLegalEmOutroCAR);
		CadastroAmbientalRural.container.delegate('.cbReservaLegalDeOutroCAR', 'click', CadastroAmbientalRural.onCBReservaLegalDeOutroCAR);
		CadastroAmbientalRural.container.delegate('.btnBuscarEmpreendimento', 'click', CadastroAmbientalRural.abrirModalListarEmpreendimento);

		if (CadastroAmbientalRural.settings.textoMerge) {
			CadastroAmbientalRural.abrirModalRedireciona(CadastroAmbientalRural.settings.textoMerge, CadastroAmbientalRural.settings.atualizarDependenciasModalTitulo);
		}

		Aux.setarFoco(CadastroAmbientalRural.container);

		if (CadastroAmbientalRural.settings.threadAtualizarSituacao == null) {
			var situacao = CadastroAmbientalRural.settings.idsTelaProjetoGeograficoSituacao;

			switch (+$('.hdnSituacaoProcessamentoId', CadastroAmbientalRural.container).val()) {
				case situacao.AguardandoProcessamento:
				case situacao.Processando:
				case situacao.Processado:
				case situacao.GerandoPDF:
				case situacao.AguardandoGeracaoPDF:
					CadastroAmbientalRural.settings.threadAtualizarSituacao = setInterval(CadastroAmbientalRural.atualizarSituacao, CadastroAmbientalRural.settings.threadAtualizarSituacaoDelay);
					break;
			}
		}
	},

	abrirModalListarEmpreendimento: function () {
		var funcaoCallBack = $(this).hasClass('btnreceptor') ? CadastroAmbientalRural.associarEmpreendimentoReceptor : CadastroAmbientalRural.associarEmpreendimentoCedente;

		Modal.abrir(CadastroAmbientalRural.settings.urls.listarEmpreendimentos, null, function (content) {
			EmpreendimentoListar.load(content, { associarFuncao: funcaoCallBack });
			Modal.defaultButtons(content);
		}, Modal.tamanhoModalMedia);
	},

	associarEmpreendimentoCedente: function (resposta) {
		if (!resposta) {
			return false;
		}
		
		$('.hdnEmpreendimentoCedenteId', CadastroAmbientalRural.container).val(resposta.Id);
		$('.txtCodigoEmpreendimentoCedente', CadastroAmbientalRural.container).val(resposta.Codigo);
		return true;
	},

	associarEmpreendimentoReceptor: function (resposta) {
		if (!resposta) {
			return false;
		}

		$('.hdnEmpreendimentoReceptorId', CadastroAmbientalRural.container).val(resposta.Id);
		$('.txtCodigoEmpreendimentoReceptor', CadastroAmbientalRural.container).val(resposta.Codigo);
		return true;
	},

	cancelar: function () {
		if ($('.lblSituacaoProcessamento', CadastroAmbientalRural.container).html() == 'Cancelado') {
			return false;
		}

		$.ajax({
			url: CadastroAmbientalRural.settings.urls.cancelar,
			data: JSON.stringify(CadastroAmbientalRural.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, CadastroAmbientalRural.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					CadastroAmbientalRural.stopThread();
					CadastroAmbientalRural.atualizarSituacao();

					$('.hdnSituacaoId', CadastroAmbientalRural.container).val(response.Situacao.Id);
					$('.divMapaVisualiar', CadastroAmbientalRural.container).addClass('hide');
					$('.dataGridArquivosProcessados', CadastroAmbientalRural.container).addClass('hide');
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(CadastroAmbientalRural.container, response.Msg);
				}
			}
		});

		return true;
	},

	bloquearCamposEditaveis: function (isBloquear) {

		$('.campoEditavel', CadastroAmbientalRural.container).each(function (i, item) {
			if (isBloquear) {
				$(item).attr('disabled', 'disabled');
				$(item).addClass('disabled');
				$('.btnBuscarEmpreendimento', CadastroAmbientalRural.container).addClass('hide');
			}
			else {
				$(item).removeAttr('disabled', 'disabled');
				$(item).removeClass('disabled');
				$('.btnBuscarEmpreendimento', CadastroAmbientalRural.container).removeClass('hide');
			}
		});
	},

	onChangeMunicipio: function () {
		valorSelecionado = $('.ddlMunicipio', CadastroAmbientalRural.container).val();

		if (valorSelecionado <= 0) {
			$('.txtModuloFiscalHa', CadastroAmbientalRural.container).val(valorSelecionado);
			return;
		}
		$.ajax({
			url: CadastroAmbientalRural.settings.urls.obterModuloFiscal,
			data: { municipioID: valorSelecionado },
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CadastroAmbientalRural.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Retorno) {
					$('.hdnModuloFiscalId', CadastroAmbientalRural.container).val(response.Retorno.Id);
					$('.txtModuloFiscalHa', CadastroAmbientalRural.container).val(response.Retorno.Ha);
					CadastroAmbientalRural.calcularQtdModuloFiscal();
				}
			}
		});
	},

	calcularQtdModuloFiscal: function () {
		if (!$('.txtModuloFiscalHa', CadastroAmbientalRural.container).val() || $('.ddlOcorreuAlteracaoApos2008 :selected', CadastroAmbientalRural.container).val() < 0) {
			$('.txtPercentMaxRecuperacaoApp', CadastroAmbientalRural.container).val('10');
			$('.txtQtdModuloFiscalAtp', CadastroAmbientalRural.container).val(Mascara.getStringMask(0, 'n2'));
			return;
		}

		var atpHA = 0, moduloFiscal = 0, result = 0;

		if ($('.ddlOcorreuAlteracaoApos2008 :selected', CadastroAmbientalRural.container).val() == 1) {
			if ($('.txtAtp2008', CadastroAmbientalRural.container).val() && Globalize.parseFloat($('.txtAtp2008', CadastroAmbientalRural.container).val()) > 0) {
				atpHA = (Globalize.parseFloat($('.txtAtp2008', CadastroAmbientalRural.container).val()) / 10000);
			}
		}
		else {
			atpHA = Globalize.parseFloat($('.hdnAtpCroquiHA', CadastroAmbientalRural.container).val());
		}

		if (!atpHA) {
			$('.txtPercentMaxRecuperacaoApp', CadastroAmbientalRural.container).val('10');
			$('.txtQtdModuloFiscalAtp', CadastroAmbientalRural.container).val(Mascara.getStringMask(0, 'n2'));
			return;
		}
		moduloFiscal = Globalize.parseFloat($('.txtModuloFiscalHa', CadastroAmbientalRural.container).val());
		result = atpHA / moduloFiscal;

		$('.txtQtdModuloFiscalAtp', CadastroAmbientalRural.container).val(Mascara.getStringMask(result, 'n2'));

		if (result >= 0 && result <= 2) {
			$('.txtPercentMaxRecuperacaoApp', CadastroAmbientalRural.container).val('10');
		}

		if (result > 2 && result <= 4) {
			$('.txtPercentMaxRecuperacaoApp', CadastroAmbientalRural.container).val('20');
		}

		if (result > 4) {
			$('.txtPercentMaxRecuperacaoApp', CadastroAmbientalRural.container).val('Não se aplica');
		}
	},

	onCBReservaLegalEmOutroCAR: function () {
		$('.hdnEmpreendimentoReceptorId', CadastroAmbientalRural.container).val('');
		$('.txtCodigoEmpreendimentoReceptor', CadastroAmbientalRural.container).val('');

		$('.cedente', CadastroAmbientalRural.container).toggleClass('hide', !$('.cbReservaLegalEmOutroCAR', CadastroAmbientalRural.container).is(':checked'));
	},

	onCBReservaLegalDeOutroCAR: function () {
		$('.hdnEmpreendimentoCedenteId', CadastroAmbientalRural.container).val('');
		$('.txtCodigoEmpreendimentoCedente', CadastroAmbientalRural.container).val('');

		$('.receptor', CadastroAmbientalRural.container).toggleClass('hide', !$('.cbReservaLegalDeOutroCAR', CadastroAmbientalRural.container).is(':checked'));
	},

	onChangeVistoriaAprovacaoCAR: function () {
		$('.txtDataVistoriaAprovacao', CadastroAmbientalRural.container).val('');
		$('.txtDataVistoriaAprovacao', CadastroAmbientalRural.container).closest('div').toggleClass('hide', +$('.ddlVistoriaAprovacaoCAR', CadastroAmbientalRural.container).val() <= 0)
	},

	onChangeATPArea2008: function () {
		var valorSelecionado = $('.ddlMunicipio', CadastroAmbientalRural.container).val();

		if (!valorSelecionado || valorSelecionado <= 0) {
			$('.txtQtdModuloFiscalAtp', CadastroAmbientalRural.container).val('0');
			return;
		}

		CadastroAmbientalRural.calcularQtdModuloFiscal();
	},

	processar: function () {
		CadastroAmbientalRural.bloquearCamposEditaveis(true);
		Mensagem.limpar(CadastroAmbientalRural.container);

		$.ajax({
			url: CadastroAmbientalRural.settings.urls.processar,
			data: JSON.stringify(CadastroAmbientalRural.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, CadastroAmbientalRural.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.hdnCaracterizacaoId', CadastroAmbientalRural.container).val(response.Caracterizacao.Id);
					$('.hdnProjetoGeoId', CadastroAmbientalRural.container).val(response.Caracterizacao.ProjetoGeoId);

					$('.dataGridArquivosProcessados', CadastroAmbientalRural.container).addClass('hide');

					$('.dataGridArquivosProcessados tbody tr:not(.trTemplateArqProcessado)').each(function () {
						$(this).remove();
					});

					CadastroAmbientalRural.atualizarSituacao();

					if (CadastroAmbientalRural.settings.threadAtualizarSituacao == null) {
						CadastroAmbientalRural.settings.threadAtualizarSituacao = setInterval(CadastroAmbientalRural.atualizarSituacao, CadastroAmbientalRural.settings.threadAtualizarSituacaoDelay);
					}
				} else {
					CadastroAmbientalRural.bloquearCamposEditaveis(false);
				}

				if (response.Msg != null && response.Msg.length > 0) {
					Mensagem.gerar(CadastroAmbientalRural.container, response.Msg);
				}
			}
		});
	},

	reprocessar: function () {
		CadastroAmbientalRural.abrirModalReprocessamento();
	},

	stopThread: function () {
		clearInterval(CadastroAmbientalRural.settings.threadAtualizarSituacao);
		CadastroAmbientalRural.settings.threadAtualizarSituacao = null;
	},

	baixarArquivoProcessado: function () {
		var container = $(this).closest('tr');

		var id = $('.hdnArquivoProcessadoId', container).val();
		var tipo = $('.hdnArquivoProcessadoTipoId', container).val();

		var url;

		$(CadastroAmbientalRural.settings.urls.arquivos).each(function () {
			if (this.Tipo == tipo) {
				url = this.Url;
				return false;
			}
		});

		Aux.downloadAjax("downloadPrjGeo", url + '?id=' + id, null, 'post');

	},

	obterAreasProcessadas: function () {
		var projetoId = $('.hdnProjetoGeoId', CadastroAmbientalRural.container).val();
		var porcMaxRecuperar = 0;

		switch ($('.txtPercentMaxRecuperacaoApp', CadastroAmbientalRural.container).val()) {
			case '10':
			case '20':
				porcMaxRecuperar = +$('.txtPercentMaxRecuperacaoApp', CadastroAmbientalRural.container).val();
				break;

			default:
				porcMaxRecuperar = 0;
				break;
		}

		$.ajax({
			url: CadastroAmbientalRural.settings.urls.obterAreasProcessadas,
			data: JSON.stringify({ projetoId: projetoId, porcMaxRecuperar: porcMaxRecuperar }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, CadastroAmbientalRural.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				var area = CadastroAmbientalRural.settings.idsTelaAreas;

				if (response.Msg != null && response.Msg.length > 0) {
					Mensagem.gerar(CadastroAmbientalRural.container, response.Msg);
				}
				if (response.EhValido) {
					$(response.AreasProcessadas).each(function (i, item) {
						var percent = 0;
						var atp = JSON.parse($('.hdnAtpCroquiM', CadastroAmbientalRural.container).val());
						switch (item.Tipo) {
							case area.AREA_USO_ALTERNATIVO:
								$('.txtAreaUsoAlternativo', CadastroAmbientalRural.container).val(Mascara.getStringMask(item.ValorHA, 'n4'));
								$('.hdnAreaUsoAlternativo', CadastroAmbientalRural.container).val(JSON.stringify(item));
								break;

							case area.APP_RECUPERAR_EFETIVA:
								percent = 0;
								$('.txtRecuperarEfetiva', CadastroAmbientalRural.container).val(Mascara.getStringMask(item.Valor, 'n2'));
								$('.hdnAreaAPPRecuperarEfeitva', CadastroAmbientalRural.container).val(JSON.stringify(item));

								percent = (item.Valor * 100) / atp.Valor;
								$('.txtPercentRecuperarEfetiva', CadastroAmbientalRural.container).val(Mascara.getStringMask(percent, 'n2'));
								break;

							case area.APP_USO_CONSOLIDADO:
								percent = 0;
								$('.txtUsoConsolidado', CadastroAmbientalRural.container).val(Mascara.getStringMask(item.Valor, 'n2'));
								$('.hdnAPPUsoConsolidado', CadastroAmbientalRural.container).val(JSON.stringify(item));

								percent = percent = (item.Valor * 100) / atp.Valor;
								$('.txtPercentUsoConsolidado', CadastroAmbientalRural.container).val(Mascara.getStringMask(percent, 'n2'));
								break;

							case area.APP_RECUPERAR_CALCULADO:
								percent = 0;
								$('.txtRecuperarCalculado', CadastroAmbientalRural.container).val(Mascara.getStringMask(item.Valor, 'n2'));
								$('.hdnAPPRecuperarCalculado', CadastroAmbientalRural.container).val(JSON.stringify(item));

								percent = percent = (item.Valor * 100) / atp.Valor;
								$('.txtPercentRecuperarCalculado', CadastroAmbientalRural.container).val(Mascara.getStringMask(percent, 'n2'));
								break;
						}
					});
					return;
				}
			}
		});
	},

	atualizarSituacao: function () {
		var empreendimentoId = $('.hdnEmpreendimentoId', CadastroAmbientalRural.container).val();

		$.ajax({
			url: CadastroAmbientalRural.settings.urls.obterSituacaoProcessamento,
			data: { empreendimentoId: empreendimentoId },
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {

					$('.lblSituacaoProcessamento', CadastroAmbientalRural.container).html(response.Situacao.Texto);
					$('.hdnSituacaoProcessamentoId', CadastroAmbientalRural.container).val(response.Situacao.Id);
					CadastroAmbientalRural.alterarDisposicaoTela(response.Situacao.Id);
				}

				if (response.Msg != null && response.Msg.length > 0) {
					Mensagem.gerar(CadastroAmbientalRural.container, response.Msg);
				}
			}
		});
	},

	alterarDisposicaoTela: function (situacaoID) {
		if (!situacaoID || situacaoID <= 0) {
			return;
		}

		var situacao = CadastroAmbientalRural.settings.idsTelaProjetoGeograficoSituacao;
		if ($('.lblSituacaoProcessamento', CadastroAmbientalRural.container).html() == 'Processado') {
			situacaoID = situacao.ProcessadoPDF;
		}

		CadastroAmbientalRural.bloquearCamposEditaveis(true);

		switch (situacaoID) {
			case situacao.AguardandoProcessamento:
			case situacao.Processando:
			case situacao.Processado:
			case situacao.AguardandoGeracaoPDF:
			case situacao.GerandoPDF:
				CadastroAmbientalRural.mostrarBotoes(new Array('.spanCancelarProcessamento'));
				$('.btnBuscarEmpreendimento', CadastroAmbientalRural.container).addClass('hide');

				break;

			case situacao.ErroProcessamento:
			case situacao.ErroGerarPDF:
				CadastroAmbientalRural.stopThread();
				CadastroAmbientalRural.bloquearCamposEditaveis(false);
				CadastroAmbientalRural.mostrarBotoes(new Array('.spanReprocessar', '.btnModalOu'));
				break;

			case situacao.ProcessadoPDF:
				CadastroAmbientalRural.stopThread();
				CadastroAmbientalRural.mostrarBotoes(new Array('.spanReprocessar', '.spanFinalizar', '.btnModalOu'));

				$('.dataGridArquivosProcessados', CadastroAmbientalRural.container).removeClass('hide');
				CadastroAmbientalRural.obterArquivosProcessados();
				CadastroAmbientalRural.obterAreasProcessadas();
				break;

			case situacao.Cancelado:
			case situacao.CanceladaPDF:
				CadastroAmbientalRural.stopThread();
				CadastroAmbientalRural.mostrarBotoes(new Array('.spanReprocessar', '.btnModalOu'));
				CadastroAmbientalRural.bloquearCamposEditaveis(false);
				break;
		}
	},

	mostrarBotoes: function (mostrar) {
		var botoes = ['.spanReprocessar', '.spanCancelarProcessamento', '.spanFinalizar', '.btnModalOu'];

		if (!CadastroAmbientalRural.settings.isEditar) {
			botoes.push('.spanProcessar');
		}

		$(botoes).each(function (i, item) {
			if ($.inArray(item, mostrar) !== -1) {
				$(item, CadastroAmbientalRural.container).removeClass('hide');
			} else {
				$(item, CadastroAmbientalRural.container).addClass('hide');
			}
		});
	},

	obterArquivosProcessados: function () {
		var projetoId = $('.hdnProjetoGeoId', CadastroAmbientalRural.container).val();

		$.ajax({
			url: CadastroAmbientalRural.settings.urls.obterArquivosProjeto,
			data: { projetoId: projetoId },
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, CadastroAmbientalRural.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {

					$(response.Arquivos).each(function (i, item) {
						$('.dataGridArquivosProcessados', CadastroAmbientalRural.container).removeClass('hide');
						item.IsPdf = item.Tipo == 7;

						var linha = $('.trTemplateArqProcessado', CadastroAmbientalRural.container).clone();
						linha.removeClass('trTemplateArqProcessado hide');

						$('.arquivoNome', linha).text(item.IsPdf ? item.Nome + ' da CAR (PDF)' : item.Nome);
						$('.hdnArquivoProcessadoId', linha).val(item.Id);
						$('.hdnArquivoProcessadoTipoId', linha).val(item.Tipo);
						$('.btnBaixar', linha).removeClass(!item.IsPdf ? 'pdf' : 'download');
						$('.btnBaixar', linha).addClass(item.IsPdf ? 'pdf' : 'download');
						$('.dataGridArquivosProcessados tbody:last').append(linha);

						Listar.atualizarEstiloTable($('table.dataGridArquivosProcessados', CadastroAmbientalRural.container));
					});
				}

				if (response.Msg != null && response.Msg.length > 0) {
					Mensagem.gerar(CadastroAmbientalRural.container, response.Msg);
				}
			}
		});
	},

	alterarAreaATP2008: function () {
		$('.txtAtp2008', CadastroAmbientalRural.container).val('');
		$('.txtQtdModuloFiscalAtp', CadastroAmbientalRural.container).val('0');
		CadastroAmbientalRural.calcularQtdModuloFiscal();

		Aux.setarFoco(CadastroAmbientalRural.container);
		if ($('.ddlOcorreuAlteracaoApos2008 :selected', CadastroAmbientalRural.container).val() == 1) {
			$('.divAtp2008', CadastroAmbientalRural.container).removeClass('hide');
		} else {
			$('.divAtp2008', CadastroAmbientalRural.container).addClass('hide');
		}
	},

	abrirModalReprocessamento: function () {
		Modal.confirma({
			removerFechar: true,
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				CadastroAmbientalRural.processar();
				Modal.fechar(conteudoModal);
			},
			conteudo: CadastroAmbientalRural.settings.mensagens.ConfirmReprocessarProjeto.Texto,
			titulo: 'Reprocessar Arquivo',
			tamanhoModal: Modal.tamanhoModalPequena
		});
	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', CadastroAmbientalRural.container).attr('href'));
			},
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				if (!CadastroAmbientalRural.settings.isVisualizar) {
					if (CadastroAmbientalRural.cancelar()) {//não pode ser assincrono
						CadastroAmbientalRural.callBackMergiar(CadastroAmbientalRural.container);
					}
				}
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: titulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	abrirModalMerge: function (textoModal, container) {
		Modal.confirma({
			removerFechar: true,
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				CadastroAmbientalRural.cancelar();
				CadastroAmbientalRural.callBackMergiar(conteudoModal);
				Modal.fechar(conteudoModal);
				if (container) {
					Modal.fechar(container);
				}
			},
			conteudo: textoModal,
			titulo: CadastroAmbientalRural.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	callBackMergiar: function (conteudoModal) {
		MasterPage.carregando(true);
		$.ajax({
			url: CadastroAmbientalRural.settings.urls.mergiar,
			data: JSON.stringify(CadastroAmbientalRural.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				var container = $('.divCaracterizacao', CadastroAmbientalRural.container);
				container.empty();
				container.append(response.Html);
				CadastroAmbientalRural.settings.dependencias = response.Dependencias;
				MasterPage.botoes(container);
				Mascara.load(container);
			}
		});
		MasterPage.carregando(false);
	},

	//Mapa---------------------------------------------------- 
	obterSituacaoPrjDesenhador: function () {
		var objeto = {
			SituacaoId: $('.hdnSituacaoProcessamentoId', CadastroAmbientalRural.container).val(),
			SituacaoTexto: $('.lblSituacaoProcessamento', CadastroAmbientalRural.container).html(),
			ArquivosProcessados: new Array()
		};

		$('.hdnArquivoProcessadoId', CadastroAmbientalRural.container).each(function (i, item) {

			if (!item || $(item).val() == 0)
				return;

			var tr = $(item).parents('tr');
			var btnBaixar = tr.find('.btnBaixar');
			var lblNome = tr.find('.arquivoNome');

			objeto.ArquivosProcessados.push({
				Id: $(item).val(),
				Texto: lblNome.html(),
				Tipo: $('.hdnArquivoProcessadoTipoId', tr).val(),
				IsPDF: btnBaixar.hasClass('pdf')
			});
		});

		return $.toJSON(objeto);
	},

	obterAreaAbrangenciaDesenhador: function () {
		return $('.hdnAbrangenciaMBR', CadastroAmbientalRural.container).val();
	},

	baixarArquivoProcessadoDesenhador: function (id, tipo) {
		var url;

		$(CadastroAmbientalRural.settings.urls.arquivos).each(function () {
			if (this.Tipo == tipo) {
				url = this.Url;
				return false;
			}
		});

		Aux.downloadAjax("downloadPrjGeo", url + '?id=' + id, null, 'post');

	},

	abrirModalMapaVisualizar: function () {
		var desenhadorModo = 2;//Visualizar

		Modal.abrir(CadastroAmbientalRural.settings.urls.desenhador + '?modo=' + desenhadorModo, null,
			function (container) {
				Navegador.load(CadastroAmbientalRural.container, {
					id: $('.hdnProjetoGeoId', CadastroAmbientalRural.container).val(),
					modo: desenhadorModo,//Visualizar
					tipo: 7,//CAR
					onCancelar: null,
					onProcessar: null,
					onBaixarArquivo: CadastroAmbientalRural.baixarArquivoProcessadoDesenhador,
					obterSituacaoInicial: CadastroAmbientalRural.obterSituacaoPrjDesenhador,
					obterAreaAbrangencia: CadastroAmbientalRural.obterAreaAbrangenciaDesenhador,
					width: $(window).width() - 100,
					height: $(window).height() - 100
				});
			}, Modal.tamanhoModalFull);

	},
	//--------------------------------------------------------

	obter: function () {
		var container = CadastroAmbientalRural.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			ProjetoGeoId: $('.hdnProjetoGeoId', container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
			OcorreuAlteracaoApos2008: $('.ddlOcorreuAlteracaoApos2008 :selected', container).val(),
			ATPDocumento2008: Mascara.getFloatMask($('.txtAtp2008', container).val()),
			VistoriaAprovacaoCAR: $('.ddlVistoriaAprovacaoCAR :selected', container).val(),
			DataVistoriaAprovacao: { DataTexto: $('.txtDataVistoriaAprovacao:visible', container).val() },
			Situacao: { Id: $('.hdnSituacaoId', container).val() },
			MunicipioId: $('.ddlMunicipio :selected', container).val(),
			ModuloFiscalId: $('.hdnModuloFiscalId', container).val(),
			ModuloFiscalHA: $('.txtModuloFiscalHa', container).val(),
			ATPQuantidadeModuloFiscal: Mascara.getFloatMask($('.txtQtdModuloFiscalAtp', container).val()),
			APPRecuperarCalculada: Mascara.getFloatMask($('.txtRecuperarCalculado', container).val()),
			Dependencias: JSON.parse(CadastroAmbientalRural.settings.dependencias),
			DispensaARL: $('.cbDispensaARL', container).is(':checked'),

			ReservaLegalEmOutroCAR: $('.cbReservaLegalEmOutroCAR:visible', CadastroAmbientalRural.container).is(':checked'),
			EmpreendimentoReceptorId: $('.hdnEmpreendimentoReceptorId', CadastroAmbientalRural.container).val(),

			ReservaLegalDeOutroCAR: $('.cbReservaLegalDeOutroCAR:visible', CadastroAmbientalRural.container).is(':checked'),
			EmpreendimentoCedenteId: $('.hdnEmpreendimentoCedenteId', CadastroAmbientalRural.container).val(),

			Areas: []
		}

		$('.divArea', container).each(function () {
			obj.Areas.push(JSON.parse($('.hdnAreaJson', this).val()));
		});

		return obj;
	},

	finalizar: function () {
		Modal.confirma({
			btnOkCallback: function (container) { CadastroAmbientalRural.callBackFinalizar(container); },
			titulo: "Finalizar projeto geográfico",
			conteudo: CadastroAmbientalRural.settings.mensagens.ConfirmFinalizarProjeto.Texto,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	callBackFinalizar: function (container) {
		MasterPage.carregando(true);
		$.ajax({
			url: CadastroAmbientalRural.settings.urls.finalizar,
			data: JSON.stringify(CadastroAmbientalRural.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, CadastroAmbientalRural.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					CadastroAmbientalRural.abrirModalMerge(response.TextoMerge, container);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(CadastroAmbientalRural.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
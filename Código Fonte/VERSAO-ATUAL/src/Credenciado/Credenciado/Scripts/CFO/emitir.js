/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.ddl.js" />

CFOEmitir = {
	settings: {
		urls: {
			verificarNumeroCFO: null,
			obterEmpreendimentos: null,
			obterUPs: null,
			obterCulturaUP: null,
			obterPragas: null,
			validarIdentificacaoProduto: null,
			validarPraga: null,
			validarTratamentoFitossanitario: null,
			verificarCredenciadoHabilitado: null,
			obterDeclaracaoAdicional: null,
			salvar: null
		},
		Mensagens: null,
		idsTela: null,
		horaServidor: null
	},
	container: null,

	load: function (container, options) {

		if (options) {
			$.extend(CFOEmitir.settings, options);
		}

		CFOEmitir.container = MasterPage.getContent(container);

		CFOEmitir.container.delegate('.rbTipoNumero', 'change', CFOEmitir.tipoNumeroChange);
		CFOEmitir.container.delegate('.btnVerificarCFO', 'click', CFOEmitir.verificarNumero);
		$('.divNumeroEnter', CFOEmitir.container).keyup(CFOEmitir.verificarNumeroEnter);

		CFOEmitir.container.delegate('.btnLimparCFO', 'click', CFOEmitir.limparNumero);
		CFOEmitir.container.delegate('.ddlProdutores', 'change', CFOEmitir.obterEmpreendimentos);
		CFOEmitir.container.delegate('.ddlEmpreendimentos', 'change', function () { CFOEmitir.obterUps(false); });
		CFOEmitir.container.delegate('.ddlUnidadesProducao', 'change', CFOEmitir.obterCulturaUP);
		CFOEmitir.container.delegate('.btnAddIdentificacaoProduto', 'click', CFOEmitir.addIdentificacaoProduto);
		CFOEmitir.container.delegate('.btnAddPraga', 'click', CFOEmitir.addPraga);
		CFOEmitir.container.delegate('.btnExcluir', 'click', CFOEmitir.excluirItemGrid);
		CFOEmitir.container.delegate('.rbPossuiLaudo', 'change', CFOEmitir.possuiLaudoChange);
		CFOEmitir.container.delegate('.ddlEstado', 'change', Aux.onEnderecoEstadoChange);
		CFOEmitir.container.delegate('.rbPossuiTratamento', 'change', CFOEmitir.possuiTratamentoFitossanitario);
		CFOEmitir.container.delegate('.btnAddTratamento', 'click', CFOEmitir.addTratamentoFitossanitario);
		CFOEmitir.container.delegate('.rbPartidaLacrada', 'change', CFOEmitir.partidaChange);
		CFOEmitir.container.delegate('.btnSalvar', 'click', CFOEmitir.verificarSalvar);

		Aux.setarFoco(CFOEmitir.container);
		Mascara.load(CFOEmitir.container);

		if (+$('.hdnEmissaoId', CFOEmitir.container).val() > 0) {
			CFOEmitir.carregarPragas();
		}
	},

	verificarNumeroEnter: function (e) {
		if (e.keyCode == MasterPage.keyENTER) {
			$('.btnVerificarCFO', CFOEmitir.container).click();
		}
		return false;
	},

	tipoNumeroChange: function () {
		if ($('.rbTipoNumero:checked', CFOEmitir.container).val() == CFOEmitir.settings.idsTela.tipoNumeroBloco) {
			$('.txtNumero', CFOEmitir.container).removeClass('disabled').removeAttr('disabled');
			$('.txtNumero', CFOEmitir.container).addClass('maskNumInt');
			$('.txtDataEmissao', CFOEmitir.container).removeClass('disabled').removeAttr('disabled');
			$('.txtNumero', CFOEmitir.container).val('');
			$('.txtNumero', CFOEmitir.container).focus();
			$('.txtDataEmissao', CFOEmitir.container).val(CFOEmitir.settings.horaServidor);
		} else {
			$('.txtNumero', CFOEmitir.container).addClass('disabled').attr('disabled', 'disabled');
			$('.txtNumero', CFOEmitir.container).val('Gerado automaticamente');
			$('.txtDataEmissao', CFOEmitir.container).addClass('disabled').attr('disabled', 'disabled');
			$('.txtDataEmissao', CFOEmitir.container).val(CFOEmitir.settings.horaServidor);
		}
	},

	verificarNumero: function () {
		Mensagem.limpar();
		$.ajax({
			url: CFOEmitir.settings.urls.verificarNumeroCFO,
			data: JSON.stringify({ numero: $('.txtNumero', CFOEmitir.container).val(), tipoNumero: +$('.rbTipoNumero:checked', CFOEmitir.container).val() || 0 }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.txtNumero', CFOEmitir.container).val(response.Numero);
					$('.btnVerificarCFO', CFOEmitir.container).addClass('hide');
					$('.campoTela, .btnLimparCFO', CFOEmitir.container).removeClass('hide');
					$('.rbTipoNumero, .txtNumero', CFOEmitir.container).addClass('disabled').attr('disabled', 'disabled');

					if ($('.ddlProdutores', CFOEmitir.container).val() != 0) {
						CFOEmitir.obterEmpreendimentos();
					}
				}

				Mensagem.gerar(CFOEmitir.container, response.Msg);
			}
		});
	},

	limparNumero: function () {
		$('.campoTela, .btnLimparCFO', CFOEmitir.container).addClass('hide');
		$('.btnVerificarCFO', CFOEmitir.container).removeClass('hide');
		$('.rbTipoNumero', CFOEmitir.container).removeClass('disabled').removeAttr('disabled');
		$(".rbTipoNumero", CFOEmitir.container).attr("checked", false);

		CFOEmitir.tipoNumeroChange();
	},

	obterEmpreendimentos: function () {
		Mensagem.limpar();

		$.ajax({
			url: CFOEmitir.settings.urls.obterEmpreendimentos,
			data: JSON.stringify({ produtorId: $('.ddlProdutores', CFOEmitir.container).val() || 0 }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.ddlEmpreendimentos', CFOEmitir.container).ddlLoad(response.Lista, { disabledQtd: 0 });
					CFOEmitir.obterUps(false);
				} else {
					Mensagem.gerar(CFOEmitir.container, response.Msg);
				}
			}
		});
	},

	obterUps: function (manterProdutos) {
		Mensagem.limpar();

		if (!manterProdutos) {
			CFOEmitir.limparIdentificacaoProduto();
			$('.gridProdutos tbody tr:not(.trTemplate)', CFOEmitir.container).remove();
			CFOEmitir.carregarPragas();
			CFOEmitir.obterDeclaracaoAdicional();
		}

		$.ajax({
			url: CFOEmitir.settings.urls.obterUPs,
			data: JSON.stringify({ empreendimentoId: $('.ddlEmpreendimentos :selected', CFOEmitir.container).val() || 0, produtorId: $('.ddlProdutores', CFOEmitir.container).val() || 0 }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					CFOEmitir.limparIdentificacaoProduto();
					$('.ddlUnidadesProducao', CFOEmitir.container).ddlLoad(response.Lista, { disabledQtd: 0 });
					$('.ddlUnidadesProducao', CFOEmitir.container).change();
				} else {
					Mensagem.gerar(CFOEmitir.container, response.Msg);
				}
			}
		});
	},

	obterCulturaUP: function () {
		Mensagem.limpar();

		var id = +$('.ddlUnidadesProducao :selected', CFOEmitir.container).val() || 0;

		if (id <= 0) {
			return;
		}

		$.ajax({
			url: CFOEmitir.settings.urls.obterCulturaUP,
			data: JSON.stringify({ unidadeProducaoId: id }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.txtProdutoCultura', CFOEmitir.container).val(response.Cultivar.CulturaTexto);
					$('.txtProdutoCultivar', CFOEmitir.container).val(response.Cultivar.Nome);

					if (response.Cultivar.UnidadeMedidaTexto == "T") {


					    $('#CFO_Produto_UnidadeMedida')
                                     .replaceWith('<select id="CFO_Produto_UnidadeMedida" class="text txtProdutoUnidadeMedida" name="CFO_Produto_UnidadeMedida">' +
                                           '<option value="T">T</option>' +
                                           '<option value="KG">KG</option>' +
                                         '</select>');

					}
					else {

					    $('#CFO_Produto_UnidadeMedida')
                                    .replaceWith('<input class="text txtProdutoUnidadeMedida disabled" disabled="disabled" id="CFO_Produto_UnidadeMedida" name="CFO.Produto.UnidadeMedida" type="text" value="">');

					    $('.txtProdutoUnidadeMedida', CFOEmitir.container).val(response.Cultivar.UnidadeMedidaTexto);
					}


					
					$('.hdnCulturaId', CFOEmitir.container).val(response.Cultivar.CulturaId);
					$('.hdnCultivarId', CFOEmitir.container).val(response.Cultivar.Id);
					$('.hdnUnidadeMedidaId', CFOEmitir.container).val(response.Cultivar.UnidadeMedida);
				} else {
					Mensagem.gerar(CFOEmitir.container, response.Msg);
				}
			}
		});
	},

	obterDeclaracaoAdicional: function () {
		var produtos = CFOEmitir.obterIdentificacoes();
		var pragas = CFOEmitir.obterPragas();

		MasterPage.carregando(true);
		$.ajax({
			url: CFOEmitir.settings.urls.obterDeclaracaoAdicional,
			data: JSON.stringify({ produtos: produtos, pragas: pragas }),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.txtDeclaracaoAdicional', CFOEmitir.container).html(response.DeclaracoesAdicionais);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(CFOEmitir.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	limparIdentificacaoProduto: function () {
		var container = $('.identificacao_produto', CFOEmitir.container);
		$('input[type=text]', container).val('');

		if ($('select option', container).length > 1) {
			$('select', container).ddlFirst();
		}
	},

	addIdentificacaoProduto: function () {
		Mensagem.limpar(CFOEmitir.container);

		var IdentificacoesAdicionadas = CFOEmitir.obterIdentificacoes();

		var txtUnid = $('.txtProdutoUnidadeMedida', CFOEmitir.container).val();

		var bExibeKg = txtUnid.indexOf("KG") >= 0;


		var objeto = {
			UnidadeProducao: +$('.ddlUnidadesProducao :selected', CFOEmitir.container).val() || 0,
			Quantidade: Mascara.getFloatMask($('.txtProdutoQuantidade', CFOEmitir.container).val()),
			DataInicioColheita: { DataTexto: $('.txtProdutoInicioColheita', CFOEmitir.container).val() },
			DataFimColheita: { DataTexto: $('.txtProdutoFimColheita', CFOEmitir.container).val() },
			CulturaId: +$('.hdnCulturaId', CFOEmitir.container).val() || 0,
			CultivarId: +$('.hdnCultivarId', CFOEmitir.container).val() || 0,
			UnidadeMedidaId: +$('.hdnUnidadeMedidaId', CFOEmitir.container).val() || 0,
			ExibeQtdKg: bExibeKg
		};

		var retorno = MasterPage.validarAjax(
			CFOEmitir.settings.urls.validarIdentificacaoProduto, {
				cfo: $('.hdnEmissaoId', CFOEmitir.container).val(),
				empreendimento: $('.ddlEmpreendimentos', CFOEmitir.container).val(),
				item: objeto,
				lista: IdentificacoesAdicionadas
			}, null, false);

		if (!retorno.EhValido && retorno.Msg) {
			Mensagem.gerar(CFOEmitir.container, retorno.Msg);
			return;
		}

		var tabela = $('.gridProdutos', CFOEmitir.container);
		var linha = $('.trTemplate', tabela).clone();
		$('.codigoUP', linha).text($('.ddlUnidadesProducao :selected', CFOEmitir.container).text());
		$('.cultura_cultivar', linha).text($('.txtProdutoCultura', CFOEmitir.container).val() + ' - ' + $('.txtProdutoCultivar', CFOEmitir.container).val());
		$('.quantidade', linha).text($('.txtProdutoQuantidade', CFOEmitir.container).val() + ' ' + $('.txtProdutoUnidadeMedida', CFOEmitir.container).val());
		$('.periodo', linha).text($('.txtProdutoInicioColheita', CFOEmitir.container).val() + ' a ' + $('.txtProdutoFimColheita', CFOEmitir.container).val());
		$('.hdnItemJson', linha).val(JSON.stringify(objeto));

		$('tbody', tabela).append(linha);
		$(linha).removeClass('hide').removeClass('trTemplate');
		Listar.atualizarEstiloTable(tabela);
		CFOEmitir.limparIdentificacaoProduto();
		CFOEmitir.carregarPragas();
		CFOEmitir.obterDeclaracaoAdicional();
	},

	obterIdentificacoes: function () {
		var retorno = [];
		$('.gridProdutos tbody tr:not(.trTemplate)', CFOEmitir.container).each(function () {
			retorno.push(JSON.parse($('.hdnItemJson', this).val()));
		});

		return retorno;
	},

	carregarPragas: function () {
		$('.ddlPragas', CFOEmitir.container).ddlClear();
		var produtos = CFOEmitir.obterIdentificacoes();

		$.ajax({
			url: CFOEmitir.settings.urls.obterPragas,
			data: JSON.stringify({ produtos: produtos }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.ddlPragas', CFOEmitir.container).ddlLoad(response.Lista);
				} else {
					Mensagem.gerar(CFOEmitir.container, response.Msg);
				}
			}
		});

		$('.gridPragas tbody tr:not(.trTemplate)', CFOEmitir.container).each(function () {
			var item = JSON.parse($('.hdnItemJson', this).val());

			var possui = false;
			$('.ddlPragas option', CFOEmitir.container).each(function () {
				if ($(this).val() == item.Id) {
					possui = true;
				}
			});

			if (!possui) {
				$(this).remove();
			}
		});
	},

	addPraga: function () {
		Mensagem.limpar(CFOEmitir.container);

		var praga = $('.ddlPragas', CFOEmitir.container).ddlSelecionado();
		var lista = CFOEmitir.obterPragas();
		var item = {
			Id: praga.Id,
			NomeCientifico: praga.Texto.split('-')[0],
			NomeComum: praga.Texto.split('-')[1]
		};

		var retorno = MasterPage.validarAjax(CFOEmitir.settings.urls.validarPraga, { item: item, lista: lista }, null, false);

		if (!retorno.EhValido && retorno.Msg) {
			Mensagem.gerar(CFOEmitir.container, retorno.Msg);
			return;
		}

		var tabela = $('.gridPragas', CFOEmitir.container);
		var linha = $('.trTemplate', tabela).clone();

		$('.nome_cientifico', linha).text(item.NomeCientifico);
		$('.nome_comum', linha).text(item.NomeComum);
		$('.hdnItemJson', linha).val(JSON.stringify(item));

		$('tbody', tabela).append(linha);
		$(linha).removeClass('hide').removeClass('trTemplate');

		Listar.atualizarEstiloTable(tabela);
		$('.ddlPragas', CFOEmitir.container).ddlFirst();
		CFOEmitir.obterDeclaracaoAdicional();
	},

	obterPragas: function () {
		var retorno = [];
		$('.gridPragas tbody tr:not(.trTemplate)', CFOEmitir.container).each(function () {
			retorno.push(JSON.parse($('.hdnItemJson', this).val()));
		});

		return retorno;
	},

	excluirItemGrid: function () {
		var tabela = $(this).closest('.dataGridTable');
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable(tabela);

		if (tabela.hasClass('gridProdutos') || tabela.hasClass('gridPragas')) {
			CFOEmitir.carregarPragas();
			CFOEmitir.obterDeclaracaoAdicional();
		}
	},

	possuiLaudoChange: function () {
		$('.laudo', CFOEmitir.container).find('input[type=text]').val('');
		$('.ddlEstado', CFOEmitir.container).ddlFirst();
		$('.ddlEstado', CFOEmitir.container).change();

		if ($(this).val() == $('.rbPossuiLaudoSim', CFOEmitir.container).val()) {
			$('.laudo', CFOEmitir.container).removeClass('hide');
		} else {
			$('.laudo', CFOEmitir.container).addClass('hide');
		}
	},

	possuiTratamentoFitossanitario: function () {
		$(this).closest('fieldset').find('input[type=text]').val('');
		$('.gridTratamento tbody tr:not(.trTemplate)', CFOEmitir.container).remove();

		if ($(this).val() == 1) {
			$('.tratamento', CFOEmitir.container).removeClass('hide');
		} else {
			$('.tratamento', CFOEmitir.container).addClass('hide');
		}
	},

	addTratamentoFitossanitario: function () {
		Mensagem.limpar(CFOEmitir.container);

		var lista = CFOEmitir.obterTratamentoFitossanitario();
		var item = {
			ProdutoComercial: $('.txtTratamentoNomeProduto', CFOEmitir.container).val(),
			IngredienteAtivo: $('.txtTratamentoIngredienteAtivo', CFOEmitir.container).val(),
			Dose: Mascara.getFloatMask($('.txtTratamentoDose', CFOEmitir.container).val()),
			PragaProduto: $('.txtTratamentoPragaProduto', CFOEmitir.container).val(),
			ModoAplicacao: $('.txtTratamentoModoAplicacao', CFOEmitir.container).val()
		};

		var retorno = MasterPage.validarAjax(CFOEmitir.settings.urls.validarTratamentoFitossanitario, { item: item, lista: lista }, null, false);

		if (!retorno.EhValido && retorno.Msg) {
			Mensagem.gerar(CFOEmitir.container, retorno.Msg);
			return;
		}

		var tabela = $('.gridTratamento', CFOEmitir.container);
		var linha = $('.trTemplate', tabela).clone();

		$('.nome_produto', linha).text(item.ProdutoComercial);
		$('.ingrediente', linha).text(item.IngredienteAtivo);
		$('.dose', linha).text(Mascara.getStringMask(item.Dose, 'n4'));
		$('.produto_praga', linha).text(item.PragaProduto);
		$('.modo_aplicacao', linha).text(item.ModoAplicacao);
		$('.hdnItemJson', linha).val(JSON.stringify(item));

		$('tbody', tabela).append(linha);
		$(linha).removeClass('hide').removeClass('trTemplate');

		Listar.atualizarEstiloTable(tabela);
		$(this).closest('fieldset').find('input[type=text]').val('');
	},

	obterTratamentoFitossanitario: function () {
		var retorno = [];
		$('.gridTratamento tbody tr:not(.trTemplate)', CFOEmitir.container).each(function () {
			retorno.push(JSON.parse($('.hdnItemJson', this).val()));
		});

		return retorno;
	},

	partidaChange: function () {
		$('.partida', CFOEmitir.container).find('input[type=text]').val('');

		if ($(this).val() == $('.rbPartidaLacradaSim', CFOEmitir.container).val()) {
			$('.partida', CFOEmitir.container).removeClass('hide');
		} else {
			$('.partida', CFOEmitir.container).addClass('hide');
		}
	},

	obter: function () {
		var produtor = $('.ddlProdutores', CFOEmitir.container).ddlSelecionado();
		var empreendimento = $('.ddlEmpreendimentos', CFOEmitir.container).ddlSelecionado();

		var objeto = {
			Id: +$('.hdnEmissaoId', CFOEmitir.container).val(),
			TipoNumero: +$('.rbTipoNumero:checked', CFOEmitir.container).val(),
			Numero: $('.txtNumero', CFOEmitir.container).val(),
			SituacaoId: $('.ddlSituacoes :selected', CFOEmitir.container).val(),
			DataEmissao: { DataTexto: $('.txtDataEmissao', CFOEmitir.container).val() },
			ProdutorId: produtor.Id,
			ProdutorTexto: produtor.Texto,
			EmpreendimentoId: empreendimento.Id,
			EmpreendimentoTexto: empreendimento.Texto,
			PossuiLaudoLaboratorial: $('.rbPossuiLaudo:checked', CFOEmitir.container).val(),
			NomeLaboratorio: $('.txtNomeLaboratorio', CFOEmitir.container).val(),
			NumeroLaudoResultadoAnalise: $('.txtNumeroLaudoResultadoAnalise', CFOEmitir.container).val(),
			EstadoId: $('.ddlEstado', CFOEmitir.container).val(),
			MunicipioId: $('.ddlMunicipio', CFOEmitir.container).val(),
			ProdutoEspecificacao: 0,
			PossuiTratamentoFinsQuarentenario: $('.rbPossuiTratamento:checked', CFOEmitir.container).val(),
			PartidaLacradaOrigem: $('.rbPartidaLacrada:checked', CFOEmitir.container).val() == 1,
			NumeroLacre: $('.txtNumeroLacre', CFOEmitir.container).val(),
			NumeroPorao: $('.txtNumeroPorao', CFOEmitir.container).val(),
			NumeroContainer: $('.txtNumeroContainer', CFOEmitir.container).val(),
			ValidadeCertificado: $('.txtValidadeCertificado', CFOEmitir.container).val(),
			DeclaracaoAdicional: $('.txtDeclaracaoAdicional', CFOEmitir.container).val(),
			EstadoEmissaoId: $('.ddlEstadoEmissao', CFOEmitir.container).val(),
			MunicipioEmissaoId: $('.ddlMunicipioEmissao', CFOEmitir.container).val(),
			Produtos: [],
			Pragas: [],
			TratamentosFitossanitarios: []
		};

		objeto.Produtos = CFOEmitir.obterIdentificacoes();


		for (var i = 0; i < objeto.Produtos.length; i++)
		    if (objeto.Produtos[i].ExibeQtdKg)
		        objeto.Produtos[i].Quantidade = objeto.Produtos[i].Quantidade / 1000;


		objeto.Pragas = CFOEmitir.obterPragas();
		objeto.TratamentosFitossanitarios = CFOEmitir.obterTratamentoFitossanitario();

		if (objeto.PossuiLaudoLaboratorial) {
			objeto.PossuiLaudoLaboratorial = objeto.PossuiLaudoLaboratorial == 1;
		}

		if (objeto.PossuiTratamentoFinsQuarentenario) {
			objeto.PossuiTratamentoFinsQuarentenario = objeto.PossuiTratamentoFinsQuarentenario == 1;
		}

		$('.cbEspecificacoes').each(function (index, linha) {
			if ($(this).is(':checked')) {
				objeto.ProdutoEspecificacao += +$(this).val();
			}
		});

		return objeto;
	},

	verificarSalvar: function () {
		var objeto = CFOEmitir.obter();
		var retorno = MasterPage.validarAjax(CFOEmitir.settings.urls.verificarCredenciadoHabilitado, { entidade: objeto }, null, false);

		if (!retorno.EhValido) {
			Mensagem.gerar(CFOEmitir.container, retorno.Msg);
			return;
		} else {
			if (retorno.Msg && retorno.Msg.length > 0) {
				Modal.confirma({
					btnOkLabel: 'Salvar',
					titulo: 'Confirmar',
					conteudo: retorno.Msg[0].Texto,
					tamanhoModal: Modal.tamanhoModalMedia,
					btnOkCallback: function (content) {
						CFOEmitir.salvar();
						Modal.fechar(content);
					}
				});
			} else {
				CFOEmitir.salvar();
			}
		}
	},

	salvar: function () {
		Mensagem.limpar(CFOEmitir.container);
		var objeto = CFOEmitir.obter();

		MasterPage.carregando(true);
		$.ajax({
			url: CFOEmitir.settings.urls.salvar,
			data: JSON.stringify(CFOEmitir.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(CFOEmitir.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
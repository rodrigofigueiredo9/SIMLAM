/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.ddl.js" />
/// <reference path="../mensagem.js" />

CFOCEmitir = {
	settings: {
		urls: {
			verificarNumero: null,
			listarLote: null,
			obterMunicipios: null,
			validarIdentificacaoProduto: null,
			obterPragas: null,
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
			$.extend(CFOCEmitir.settings, options);
		}

		CFOCEmitir.container = MasterPage.getContent(container);

		CFOCEmitir.container.delegate('.rbTipoNumero', 'change', CFOCEmitir.tipoNumeroChange);
		CFOCEmitir.container.delegate('.btnVerificarNumero', 'click', CFOCEmitir.verificarNumero);
		$('.divNumeroEnter', CFOCEmitir.container).keyup(CFOCEmitir.verificarNumeroEnter);

		CFOCEmitir.container.delegate('.btnLimparNumero', 'click', CFOCEmitir.limparNumero);
		CFOCEmitir.container.delegate('.ddlEmpreendimentos', 'change', function () { CFOCEmitir.limparLotes(false); });
		CFOCEmitir.container.delegate('.btnAssociarLote', 'click', CFOCEmitir.listarLote);
		CFOCEmitir.container.delegate('.btnAddIdentificacaoProduto', 'click', CFOCEmitir.addIdentificacaoProduto);
		CFOCEmitir.container.delegate('.btnAddPraga', 'click', CFOCEmitir.addPraga);
		CFOCEmitir.container.delegate('.btnExcluir', 'click', CFOCEmitir.excluirItemGrid);
		CFOCEmitir.container.delegate('.rbPossuiLaudo', 'change', CFOCEmitir.possuiLaudoChange);
		CFOCEmitir.container.delegate('.ddlEstado', 'change', Aux.onEnderecoEstadoChange);
		CFOCEmitir.container.delegate('.rbPossuiTratamento', 'change', CFOCEmitir.possuiTratamentoFitossanitario);
		CFOCEmitir.container.delegate('.btnAddTratamento', 'click', CFOCEmitir.addTratamentoFitossanitario);
		CFOCEmitir.container.delegate('.rbPartidaLacrada', 'change', CFOCEmitir.partidaChange);
		CFOCEmitir.container.delegate('.btnSalvar', 'click', CFOCEmitir.verificarSalvar);

		Aux.setarFoco(CFOCEmitir.container);
		Mascara.load(CFOCEmitir.container);

		if (+$('.hdnEmissaoId', CFOCEmitir.container).val() > 0) {
			CFOCEmitir.carregarPragas();
		}
	},

	verificarNumeroEnter: function (e) {
		if (e.keyCode == MasterPage.keyENTER) {
			$('.btnVerificarNumero', CFOCEmitir.container).click();
		}
		return false;
	},

	tipoNumeroChange: function () {
		if ($('.rbTipoNumero:checked', CFOCEmitir.container).val() == CFOCEmitir.settings.idsTela.tipoNumeroBloco) {
			$('.txtNumero', CFOCEmitir.container).removeClass('disabled').removeAttr('disabled');
			$('.txtNumero', CFOCEmitir.container).addClass('maskNumInt');
			$('.txtDataEmissao', CFOCEmitir.container).removeClass('disabled').removeAttr('disabled');
			$('.txtNumero', CFOCEmitir.container).val('');
			$('.txtNumero', CFOCEmitir.container).focus();
			$('.txtDataEmissao', CFOCEmitir.container).val(CFOCEmitir.settings.horaServidor);
		} else {
			$('.txtNumero', CFOCEmitir.container).addClass('disabled').attr('disabled', 'disabled');
			$('.txtNumero', CFOCEmitir.container).val('Gerado automaticamente');
			$('.txtDataEmissao', CFOCEmitir.container).addClass('disabled').attr('disabled', 'disabled');
			$('.txtDataEmissao', CFOCEmitir.container).val(CFOCEmitir.settings.horaServidor);
		}
	},

	verificarNumero: function () {
		Mensagem.limpar();
		$.ajax({
			url: CFOCEmitir.settings.urls.verificarNumero,
			data: JSON.stringify({ numero: $('.txtNumero', CFOCEmitir.container).val(), tipoNumero: +$('.rbTipoNumero:checked', CFOCEmitir.container).val() || 0 }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.txtNumero', CFOCEmitir.container).val(response.Numero);
					$('.btnVerificarNumero', CFOCEmitir.container).addClass('hide');
					$('.campoTela, .btnLimparNumero', CFOCEmitir.container).removeClass('hide');
					$('.rbTipoNumero, .txtNumero', CFOCEmitir.container).addClass('disabled').attr('disabled', 'disabled');

					if ($('.ddlEmpreendimentos', CFOCEmitir.container).val() != 0) {
						CFOCEmitir.limparLotes(true);
					}
				}

				Mensagem.gerar(CFOCEmitir.container, response.Msg);
			}
		});
	},

	limparNumero: function () {
		$('.campoTela, .btnLimparNumero', CFOCEmitir.container).addClass('hide');
		$('.btnVerificarNumero', CFOCEmitir.container).removeClass('hide');
		$('.rbTipoNumero', CFOCEmitir.container).removeClass('disabled').removeAttr('disabled');
		$(".rbTipoNumero", CFOCEmitir.container).attr("checked", false);

		CFOCEmitir.tipoNumeroChange();
	},

	limparLotes: function (manterProdutos) {
		if (!manterProdutos) {
			CFOCEmitir.limparIdentificacaoProduto();
			$('.gridProdutos tbody tr:not(.trTemplate)', CFOCEmitir.container).remove();
			CFOCEmitir.carregarPragas();
			CFOCEmitir.obterDeclaracaoAdicional();
		}
	},

	listarLote: function () {
		Mensagem.limpar(CFOCEmitir.container);
		var empreendimento = $('.ddlEmpreendimentos', CFOCEmitir.container).val();

		if (empreendimento == 0) {
			Mensagem.gerar(CFOCEmitir.container, [CFOCEmitir.settings.Mensagens.EmpreendimentoObrigatorio]);
			return;
		}

		Modal.abrir(CFOCEmitir.settings.urls.listarLote + "?empreendimento=" + empreendimento, null, function (container) {
			Modal.defaultButtons(container);
			LoteListar.load(container, { associarFuncao: CFOCEmitir.associarLote });
		}, Modal.tamanhoModalGrande);
	},

	associarLote: function (objeto) {
		Mensagem.limpar(CFOCEmitir.container);

		$('.hdnProdutoLoteId', CFOCEmitir.container).val(objeto.Id);
		$('.txtProdutoLote', CFOCEmitir.container).val(objeto.NumeroCompleto);
		$('.hdnCulturaId', CFOCEmitir.container).val(objeto.Item.Cultura);
		$('.txtProdutoCultura', CFOCEmitir.container).val(objeto.Item.CulturaTexto);
		$('.hdnCultivarId', CFOCEmitir.container).val(objeto.Item.Cultivar);
		$('.txtProdutoCultivar', CFOCEmitir.container).val(objeto.Item.CultivarTexto);
		
		//$('.txtProdutoQuantidade', CFOCEmitir.container).val(Mascara.getStringMask(0, 'n4'));

		//$('.txtProdutoUnidadeMedida', CFOCEmitir.container).val(objeto.Item.UnidadeMedidaTexto);

		
		if (objeto.Item.UnidadeMedidaTexto == "T") {

		    $('#CFOC_Produto_UnidadeMedida')
                            .replaceWith('<select id="CFO_Produto_UnidadeMedida" class="text txtProdutoUnidadeMedida" name="CFOC_Produto_UnidadeMedida">' +
                                '<option value="T">T</option>' +
                                '<option value="KG">KG</option>' +
                                '</select>');

		}
		else {

		    $('#CFOC_Produto_UnidadeMedida')
                        .replaceWith('<input class="text txtProdutoUnidadeMedida disabled" disabled="disabled" id="CFOC_Produto_UnidadeMedida" name="CFOC.Produto.UnidadeMedida" type="text" value="">');

		    $('.txtProdutoUnidadeMedida', CFOCEmitir.container).val(objeto.Item.UnidadeMedidaTexto);
		}


		//if (objeto.Item.ExibeKg) {
		//    $('.txtProdutoQuantidade', CFOCEmitir.container).val(Mascara.getStringMask(objeto.Item.Quantidade * 1000, 'n4'));
		//    $('.txtProdutoUnidadeMedida', CFOCEmitir.container).val("KG");
		//}
		

		$('.txtProdutoConsolidacao', CFOCEmitir.container).val(objeto.DataCriacao.DataTexto);
		$('.hdnUnidadeMedidaId', CFOCEmitir.container).val(objeto.Item.UnidadeMedida);

		return true;
	},

	obterDeclaracaoAdicional: function () {
		var produtos = CFOCEmitir.obterIdentificacoes();
		var pragas = CFOCEmitir.obterPragas();

		MasterPage.carregando(true);
		$.ajax({
			url: CFOCEmitir.settings.urls.obterDeclaracaoAdicional,
			data: JSON.stringify({ produtos: produtos, pragas: pragas }),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.txtDeclaracaoAdicional', CFOCEmitir.container).html(response.DeclaracoesAdicionais);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(CFOCEmitir.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	limparIdentificacaoProduto: function () {
		var container = $('.identificacao_produto', CFOCEmitir.container);
		$('input[type=text]', container).val('');

		if ($('select option', container).length > 1) {
			$('select', container).ddlFirst();
		}
	},

	addIdentificacaoProduto: function () {
		Mensagem.limpar(CFOCEmitir.container);



		var IdentificacoesAdicionadas = CFOCEmitir.obterIdentificacoes();

	
		var txtUnid = $('.txtProdutoUnidadeMedida', CFOCEmitir.container).val();
		
		var bExibeKg = txtUnid.indexOf("KG") >= 0;

		var objeto = {
			LoteId: +$('.hdnProdutoLoteId', CFOCEmitir.container).val() || 0,
			LoteCodigo: $('.txtProdutoLote', CFOCEmitir.container).val(),
			Quantidade: Mascara.getFloatMask($('.txtProdutoQuantidade', CFOCEmitir.container).val()),
			UnidadeMedida: $('.txtProdutoUnidadeMedida', CFOCEmitir.container).val(),
			CulturaTexto: $('.txtProdutoCultura', CFOCEmitir.container).val(),
			CultivarTexto: $('.txtProdutoCultivar', CFOCEmitir.container).val(),
			DataConsolidacao: { DataTexto: $('.txtProdutoConsolidacao', CFOCEmitir.container).val() },
			CulturaId: +$('.hdnCulturaId', CFOCEmitir.container).val() || 0,
			CultivarId: +$('.hdnCultivarId', CFOCEmitir.container).val() || 0,
			UnidadeMedidaId: +$('.hdnUnidadeMedidaId', CFOCEmitir.container).val() || 0,
			ExibeQtdKg: bExibeKg
		};

		var retorno = MasterPage.validarAjax(
			CFOCEmitir.settings.urls.validarIdentificacaoProduto, {
				cfoc: $('.hdnEmissaoId', CFOCEmitir.container).val(),
				empreendimento: $('.ddlEmpreendimentos', CFOCEmitir.container).val(),
				item: objeto,
				lista: IdentificacoesAdicionadas
			}, null, false);

		if (!retorno.EhValido && retorno.Msg) {
			Mensagem.gerar(CFOCEmitir.container, retorno.Msg);
			return;
		}

		var tabela = $('.gridProdutos', CFOCEmitir.container);
		var linha = $('.trTemplate', tabela).clone();
		$('.codigo', linha).text(objeto.LoteCodigo);
		$('.cultura_cultivar', linha).text(objeto.CulturaTexto + ' ' + objeto.CultivarTexto);
		$('.quantidade', linha).text(Mascara.getStringMask(objeto.Quantidade, 'n4') + ' ' + objeto.UnidadeMedida);
		$('.data_consolidacao', linha).text(objeto.DataConsolidacao.DataTexto);
		$('.hdnItemJson', linha).val(JSON.stringify(objeto));

		$('tbody', tabela).append(linha);
		$(linha).removeClass('hide').removeClass('trTemplate');
		Listar.atualizarEstiloTable(tabela);
		CFOCEmitir.limparIdentificacaoProduto();
		CFOCEmitir.carregarPragas();
		CFOCEmitir.obterDeclaracaoAdicional();
	},

	obterIdentificacoes: function () {
		var retorno = [];
		$('.gridProdutos tbody tr:not(.trTemplate)', CFOCEmitir.container).each(function () {
			retorno.push(JSON.parse($('.hdnItemJson', this).val()));
		});

		return retorno;
	},

	carregarPragas: function () {
		$('.ddlPragas', CFOCEmitir.container).ddlClear();
		var produtos = CFOCEmitir.obterIdentificacoes();

		$.ajax({
			url: CFOCEmitir.settings.urls.obterPragas,
			data: JSON.stringify({ produtos: produtos }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.ddlPragas', CFOCEmitir.container).ddlLoad(response.Lista);
				} else {
					Mensagem.gerar(CFOCEmitir.container, response.Msg);
				}
			}
		});

		$('.gridPragas tbody tr:not(.trTemplate)', CFOCEmitir.container).each(function () {
			var item = JSON.parse($('.hdnItemJson', this).val());

			var possui = false;
			$('.ddlPragas option', CFOCEmitir.container).each(function () {
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
		Mensagem.limpar(CFOCEmitir.container);

		var praga = $('.ddlPragas', CFOCEmitir.container).ddlSelecionado();
		var lista = CFOCEmitir.obterPragas();
		var item = {
			Id: praga.Id,
			NomeCientifico: praga.Texto.split('-')[0],
			NomeComum: praga.Texto.split('-')[1]
		};

		var retorno = MasterPage.validarAjax(CFOCEmitir.settings.urls.validarPraga, { item: item, lista: lista }, null, false);

		if (!retorno.EhValido && retorno.Msg) {
			Mensagem.gerar(CFOCEmitir.container, retorno.Msg);
			return;
		}

		var tabela = $('.gridPragas', CFOCEmitir.container);
		var linha = $('.trTemplate', tabela).clone();

		$('.nome_cientifico', linha).text(item.NomeCientifico);
		$('.nome_comum', linha).text(item.NomeComum);
		$('.hdnItemJson', linha).val(JSON.stringify(item));

		$('tbody', tabela).append(linha);
		$(linha).removeClass('hide').removeClass('trTemplate');

		Listar.atualizarEstiloTable(tabela);
		$('.ddlPragas', CFOCEmitir.container).ddlFirst();
		CFOCEmitir.obterDeclaracaoAdicional();
	},

	obterPragas: function () {
		var retorno = [];
		$('.gridPragas tbody tr:not(.trTemplate)', CFOCEmitir.container).each(function () {
			retorno.push(JSON.parse($('.hdnItemJson', this).val()));
		});

		return retorno;
	},

	excluirItemGrid: function () {
		var tabela = $(this).closest('.dataGridTable');
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable(tabela);

		if (tabela.hasClass('gridProdutos') || tabela.hasClass('gridPragas')) {
			CFOCEmitir.carregarPragas();
			CFOCEmitir.obterDeclaracaoAdicional();
		}
	},

	possuiLaudoChange: function () {
		$('.laudo', CFOCEmitir.container).find('input[type=text]').val('');
		$('.ddlEstado', CFOCEmitir.container).ddlFirst();
		$('.ddlEstado', CFOCEmitir.container).change();

		if ($(this).val() == $('.rbPossuiLaudoSim', CFOCEmitir.container).val()) {
			$('.laudo', CFOCEmitir.container).removeClass('hide');
		} else {
			$('.laudo', CFOCEmitir.container).addClass('hide');
		}
	},

	estadoChange: function () {
		$('.ddlEstado', CFOCEmitir.container).ddlCascate($('.ddlMunicipio', CFOCEmitir.container),
		{
			url: CFOCEmitir.settings.urls.obterMunicipios,
			data: { estadoId: +$('.ddlEstado :selected', CFOCEmitir.container).val() },
		});
	},

	possuiTratamentoFitossanitario: function () {
		$(this).closest('fieldset').find('input[type=text]').val('');
		$('.gridTratamento tbody tr:not(.trTemplate)', CFOCEmitir.container).remove();

		if ($(this).val() == 1) {
			$('.tratamento', CFOCEmitir.container).removeClass('hide');
		} else {
			$('.tratamento', CFOCEmitir.container).addClass('hide');
		}
	},

	addTratamentoFitossanitario: function () {
		Mensagem.limpar(CFOCEmitir.container);

		var lista = CFOCEmitir.obterTratamentoFitossanitario();
		var item = {
			ProdutoComercial: $('.txtTratamentoNomeProduto', CFOCEmitir.container).val(),
			IngredienteAtivo: $('.txtTratamentoIngredienteAtivo', CFOCEmitir.container).val(),
			Dose: Mascara.getFloatMask($('.txtTratamentoDose', CFOCEmitir.container).val()),
			PragaProduto: $('.txtTratamentoPragaProduto', CFOCEmitir.container).val(),
			ModoAplicacao: $('.txtTratamentoModoAplicacao', CFOCEmitir.container).val()
		};

		var retorno = MasterPage.validarAjax(CFOCEmitir.settings.urls.validarTratamentoFitossanitario, { item: item, lista: lista }, null, false);

		if (!retorno.EhValido && retorno.Msg) {
			Mensagem.gerar(CFOCEmitir.container, retorno.Msg);
			return;
		}

		var tabela = $('.gridTratamento', CFOCEmitir.container);
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
		$('.gridTratamento tbody tr:not(.trTemplate)', CFOCEmitir.container).each(function () {
			retorno.push(JSON.parse($('.hdnItemJson', this).val()));
		});

		return retorno;
	},

	partidaChange: function () {
		$('.partida', CFOCEmitir.container).find('input[type=text]').val('');

		if ($(this).val() == $('.rbPartidaLacradaSim', CFOCEmitir.container).val()) {
			$('.partida', CFOCEmitir.container).removeClass('hide');
		} else {
			$('.partida', CFOCEmitir.container).addClass('hide');
		}
	},

	obter: function () {
		var empreendimento = $('.ddlEmpreendimentos', CFOCEmitir.container).ddlSelecionado();

		var objeto = {
			Id: +$('.hdnEmissaoId', CFOCEmitir.container).val(),
			TipoNumero: +$('.rbTipoNumero:checked', CFOCEmitir.container).val(),
			Numero: $('.txtNumero', CFOCEmitir.container).val(),
			SituacaoId: $('.ddlSituacoes :selected', CFOCEmitir.container).val(),
			DataEmissao: { DataTexto: $('.txtDataEmissao', CFOCEmitir.container).val() },
			EmpreendimentoId: empreendimento.Id,
			EmpreendimentoTexto: empreendimento.Texto,
			PossuiLaudoLaboratorial: $('.rbPossuiLaudo:checked', CFOCEmitir.container).val(),
			NomeLaboratorio: $('.txtNomeLaboratorio', CFOCEmitir.container).val(),
			NumeroLaudoResultadoAnalise: $('.txtNumeroLaudoResultadoAnalise', CFOCEmitir.container).val(),
			EstadoId: $('.ddlEstado', CFOCEmitir.container).val(),
			MunicipioId: $('.ddlMunicipio', CFOCEmitir.container).val(),
			ProdutoEspecificacao: 0,
			PossuiTratamentoFinsQuarentenario: $('.rbPossuiTratamento:checked', CFOCEmitir.container).val(),
			PartidaLacradaOrigem: $('.rbPartidaLacrada:checked', CFOCEmitir.container).val() == 1,
			NumeroLacre: $('.txtNumeroLacre', CFOCEmitir.container).val(),
			NumeroPorao: $('.txtNumeroPorao', CFOCEmitir.container).val(),
			NumeroContainer: $('.txtNumeroContainer', CFOCEmitir.container).val(),
			ValidadeCertificado: $('.txtValidadeCertificado', CFOCEmitir.container).val(),
			DeclaracaoAdicional: $('.txtDeclaracaoAdicional', CFOCEmitir.container).val(),
			EstadoEmissaoId: $('.ddlEstadoEmissao', CFOCEmitir.container).val(),
			MunicipioEmissaoId: $('.ddlMunicipioEmissao', CFOCEmitir.container).val(),
			Produtos: [],
			Pragas: [],
			TratamentosFitossanitarios: []
		};

		objeto.Produtos = CFOCEmitir.obterIdentificacoes();

		for (var i = 0; i < objeto.Produtos.length; i++)
		    if (objeto.Produtos[i].ExibeQtdKg)
		        objeto.Produtos[i].Quantidade = objeto.Produtos[i].Quantidade / 1000;


		objeto.Pragas = CFOCEmitir.obterPragas();
		objeto.TratamentosFitossanitarios = CFOCEmitir.obterTratamentoFitossanitario();

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
		var objeto = CFOCEmitir.obter();
		var retorno = MasterPage.validarAjax(CFOCEmitir.settings.urls.verificarCredenciadoHabilitado, { entidade: objeto }, null, false);

		if (!retorno.EhValido) {
			Mensagem.gerar(CFOCEmitir.container, retorno.Msg);
			return;
		} else {
			if (retorno.Msg && retorno.Msg.length > 0) {
				Modal.confirma({
					btnOkLabel: 'Salvar',
					titulo: 'Confirmar',
					conteudo: retorno.Msg[0].Texto,
					tamanhoModal: Modal.tamanhoModalMedia,
					btnOkCallback: function (content) {
						CFOCEmitir.salvar();
						Modal.fechar(content);
					}
				});
			} else {
				CFOCEmitir.salvar();
			}
		}
	},

	salvar: function () {
		Mensagem.limpar(CFOCEmitir.container);
		var objeto = CFOCEmitir.obter();

		MasterPage.carregando(true);
		$.ajax({
			url: CFOCEmitir.settings.urls.salvar,
			data: JSON.stringify(CFOCEmitir.obter()),
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
					Mensagem.gerar(CFOCEmitir.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
/// <reference path="../../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../../masterpage.js" />

InformacaoCorte = {
	settings: {
		urls: {
			salvar: '',
			obterProdutos: ''
		},
		mensagens: null,
		textoMerge: null,
		dependencias: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(InformacaoCorte.settings, options); }
		InformacaoCorte.container = MasterPage.getContent(container);

		InformacaoCorte.container.delegate('.btnSalvar', 'click', InformacaoCorte.salvar);
		InformacaoCorte.container.delegate('.btnAdicionar', 'click', InformacaoCorte.adicionar);
		InformacaoCorte.container.delegate('.btnExcluir', 'click', InformacaoCorte.excluir);
		InformacaoCorte.container.delegate('.btnAdicionarTipo', 'click', InformacaoCorte.adicionarTipo);
		InformacaoCorte.container.delegate('.btnExcluirTipo', 'click', InformacaoCorte.excluir);
		InformacaoCorte.container.delegate('.btnLimparTipo', 'click', InformacaoCorte.limparTipo);
		InformacaoCorte.container.delegate('.btnAdicionarDestinacao', 'click', InformacaoCorte.adicionarDestinacao);
		InformacaoCorte.container.delegate('.btnExcluirDestinacao', 'click', InformacaoCorte.excluir);
		InformacaoCorte.container.delegate('.btnAdicionarInformacao', 'click', InformacaoCorte.adicionarInformacao);
		InformacaoCorte.container.delegate('.btnExcluirInformacao', 'click', InformacaoCorte.excluirInformacao);
		InformacaoCorte.container.delegate('.ddlDestinacaoMaterial', 'change', InformacaoCorte.carregarProdutos);

		Aux.setarFoco(InformacaoCorte.container);

		if (InformacaoCorte.settings.textoMerge) {
			InformacaoCorte.abrirModalRedireciona(InformacaoCorte.settings.textoMerge);
		}
	},

	adicionar: function () {
		var container = InformacaoCorte.container;
		Mensagem.limpar(container);

		//monta o objeto
		var objeto = {
			NumeroLicenca: container.find('.numeroLicenca').val(),
			TipoLicenca: container.find('.tipoLicenca').val(),
			Atividade: container.find('.atividade').val(),
			AreaLicenca: container.find('.areaLicenciada').val(),
			DataVencimento: { DataTexto: container.find('.dataVencimento').val() }
		};

		var msgs = [];

		if (objeto.NumeroLicenca == '') {
			msgs.push(InformacaoCorte.settings.mensagens.NumeroLicencaObrigatoria);
		}

		if (objeto.TipoLicenca == '') {
			msgs.push(InformacaoCorte.settings.mensagens.TipoLicencaObrigatoria);
		}

		if (objeto.Atividade == '') {
			msgs.push(InformacaoCorte.settings.mensagens.AtividadeObrigatoria);
		}

		if (objeto.AreaLicenca == '') {
			msgs.push(InformacaoCorte.settings.mensagens.AreaLicencaObrigatoria);
		}

		if (objeto.DataVencimento.DataTexto == '') {
			msgs.push(InformacaoCorte.settings.mensagens.DataVencimentoObrigatoria);
		}
		
		if (msgs.length > 0) {
			Mensagem.gerar(container, msgs);
			return;
		}

		var linha = ''; 
		linha = $('.trTemplateRow', $('.tabLicencas', container)).clone();

		//Monta a nova linha e insere na tabela 
		linha.find('.itemJson').val(JSON.stringify(objeto));
		linha.find('.numero').text(objeto.NumeroLicenca);
		linha.find('.numero').attr('title', objeto.NumeroLicenca);
		linha.find('.tipoLicenca').text(objeto.TipoLicenca);
		linha.find('.tipoLicenca').attr('title', objeto.TipoLicenca); 
		linha.find('.atividade').text(objeto.Atividade); 
		linha.find('.atividade').attr('title', objeto.Atividade); 
		linha.find('.areaPlantada').text(objeto.AreaLicenca); 
		linha.find('.areaPlantada').attr('title', objeto.AreaLicenca); 
		linha.find('.dataVencimento').text(objeto.DataVencimento.DataTexto); 
		linha.find('.dataVencimento').attr('title', objeto.DataVencimento.DataTexto);

		linha.removeClass('trTemplateRow hide');
		$('.tabLicencas > tbody:last', container).append(linha); 
		Listar.atualizarEstiloTable($('.tabLicencas', container));

		//limpa os campos de texto 
		$('.numeroLicenca', container).val('');
		$('.tipoLicenca', container).val('');
		$('.atividade', container).val('');
		$('.areaLicenciada', container).val('');
		$('.dataVencimento', container).val('');
	},

	excluir: function () {
		$(this).closest('tr').remove();
	},

	adicionarTipo: function () {
		var container = InformacaoCorte.container;
		Mensagem.limpar(container);

		var objeto = {
			TipoCorte: $('.tipoCorte option:selected', container).val(),
			Especie: $('.especieInformada option:selected', container).val(),
			AreaCorte: $('.areaCorte', container).val(),
			IdadePlantio: $('.idadePlantio', container).val()
		};

		var msgs = [];

		if (objeto.TipoCorte <= 0) {
			msgs.push(InformacaoCorte.settings.mensagens.TipoCorteObrigatorio);
		}

		if (objeto.Especie <= 0) {
			msgs.push(InformacaoCorte.settings.mensagens.EspecieObrigatoria);
		}

		if (objeto.AreaCorte <= 0) {
			msgs.push(InformacaoCorte.settings.mensagens.AreaCorteObrigatoria);
		}

		if (objeto.IdadePlantio <= 0) {
			msgs.push(InformacaoCorte.settings.mensagens.IdadePlantioObrigatoria);
		}

		if (msgs.length > 0) {
			Mensagem.gerar(InformacaoCorte.container, msgs);
			return;
		}

		$('.adicionarTipo', container).hide();
		$('.limparTipo', container).show();

		InformacaoCorte.configurarTipo(true);
		InformacaoCorte.configurarDestinacao(false);
	},

	limparTipo: function () {
		var container = InformacaoCorte.container;
		$('.adicionarTipo', container).show();
		$('.limparTipo', container).hide();

		InformacaoCorte.configurarTipo(false);
		InformacaoCorte.configurarDestinacao(true);

		$('.tipoCorte', container).val('0');
		$('.especieInformada', container).val('0');
		$('.areaCorte', container).val('');
		$('.idadePlantio', container).val('');
		$('.ddlDestinacaoMaterial', container).val('');
		$('.ddlProduto', container).val('');
		$('.txtQuantidade', container).val('');
	},

	configurarTipo: function (disabled) {
		var container = InformacaoCorte.container;
		$('.tipoCorte', container).toggleClass('disabled', disabled);
		$('.especieInformada', container).toggleClass('disabled', disabled);
		$('.areaCorte', container).toggleClass('disabled', disabled);
		$('.idadePlantio', container).toggleClass('disabled', disabled);
		$('.btnAdicionarInformacao', container).button({ disabled: true });
		if (disabled) {
			$('.tipoCorte', container).attr('disabled', disabled);
			$('.especieInformada', container).attr('disabled', disabled);
			$('.areaCorte', container).attr('disabled', disabled);
			$('.idadePlantio', container).attr('disabled', disabled);
		} else {
			$('.tipoCorte', container).removeAttr('disabled');
			$('.especieInformada', container).removeAttr('disabled');
			$('.areaCorte', container).removeAttr('disabled');
			$('.idadePlantio', container).removeAttr('disabled');
		}
	},

	configurarDestinacao: function (disabled) {
		var container = InformacaoCorte.container;

		$('.ddlDestinacaoMaterial', container).toggleClass('disabled', disabled);
		$('.ddlProduto', container).toggleClass('disabled', disabled);
		$('.txtQuantidade', container).toggleClass('disabled', disabled);
		$('.btnAdicionarDestinacao', container).button({ disabled: disabled });
		if (disabled) {
			$('.ddlDestinacaoMaterial', container).attr('disabled', disabled);
			$('.ddlProduto', container).attr('disabled', disabled);
			$('.txtQuantidade', container).attr('disabled', disabled);
		} else {
			$('.ddlDestinacaoMaterial', container).removeAttr('disabled');
			$('.ddlProduto', container).removeAttr('disabled');
			$('.txtQuantidade', container).removeAttr('disabled');
		}
	},

	adicionarDestinacao: function () {
		var container = InformacaoCorte.container;
        Mensagem.limpar(container);

		var objeto = {
			DestinacaoMaterial: $('.ddlDestinacaoMaterial option:selected', container).val(),
			DestinacaoMaterialTexto: $('.ddlDestinacaoMaterial option:selected', container).text(),
			Produto: $('.ddlProduto option:selected', container).val(),
			ProdutoTexto: $('.ddlProduto option:selected', container).text(),
			Quantidade: $('.txtQuantidade', container).val()
		};
		
		var msgs = [];

		if (objeto.DestinacaoMaterial <= 0) {
			msgs.push(InformacaoCorte.settings.mensagens.DestinacaoMaterialObrigatoria);
		}

		if (objeto.Produto <= 0) {
			msgs.push(InformacaoCorte.settings.mensagens.ProdutoObrigatorio);
		}

		if (objeto.Quantidade <= 0 || objeto.Quantidade == '') {
			msgs.push(InformacaoCorte.settings.mensagens.QuantidadeObrigatoria);
		}

		if (msgs.length > 0) {
			Mensagem.gerar(InformacaoCorte.container, msgs);
			return;
		}

		var linha = '';
		linha = $('.trTemplateRow', $('.tabDestinacao', container)).clone();

		//Monta a nova linha e insere na tabela 
		linha.find('.itemJson').val(JSON.stringify(objeto));
		linha.find('.destinacaoMaterial').text(objeto.DestinacaoMaterialTexto);
		linha.find('.destinacaoMaterial').attr('title', objeto.DestinacaoMaterialTexto);
		linha.find('.produto').text(objeto.ProdutoTexto);
		linha.find('.produto').attr('title', objeto.ProdutoTexto);
		linha.find('.quantidade').text(objeto.Quantidade);
		linha.find('.quantidade').attr('title', objeto.Quantidade);

		linha.removeClass('trTemplateRow hide');
		$('.tabDestinacao > tbody:last', container).append(linha);
		Listar.atualizarEstiloTable($('.tabDestinacao', container));

		//limpa os campos de texto 
		$('.ddlDestinacaoMaterial', container).val('0');
		$('.ddlProduto', container).val('0');
		$('.txtQuantidade', container).val('');

		$('.btnAdicionarInformacao', container).button({ disabled: false });
	},

	adicionarInformacao: function () {
		var container = InformacaoCorte.container;
		$('.btnAdicionarInformacao', container).button({ disabled: true });

		var objeto = {
			Id: 0,
			TipoCorte: $('.tipoCorte option:selected', container).val(),
			TipoCorteTexto: $('.tipoCorte option:selected', container).text(),
			Especie: $('.especieInformada option:selected', container).val(),
			EspecieTexto: $('.especieInformada option:selected', container).text(),
			AreaCorte: $('.areaCorte', container).val(),
			IdadePlantio: $('.idadePlantio', container).val(),
			DestinacaoId: 0,
			DestinacaoMaterial: '',
			DestinacaoMaterialTexto: '',
			Produto: '',
			ProdutoTexto: '',
			Quantidade: 0,
			Linhas: $('.tabDestinacao > tbody > tr:not(".trTemplateRow")', container).length
		};

		var first = true;

		$('.tabDestinacao > tbody > tr:not(".trTemplateRow")', container).toArray().forEach(function (x) {
			var item = JSON.parse($(x).find('.itemJson').val());
			var linha = '';
			linha = $('.trTemplateRow', $('.tabInformacaoCorte', container)).clone();

			//Monta a nova linha e insere na tabela 
			if (first) {
				first = false;
				linha.find('.tipoCorte').text(objeto.TipoCorteTexto);
				linha.find('.tipoCorte').attr('title', objeto.TipoCorteTexto);
				linha.find('.tipoCorte').parent().attr('rowspan', objeto.Linhas);
				linha.find('.especie').text(objeto.EspecieTexto);
				linha.find('.especie').attr('title', objeto.EspecieTexto);
				linha.find('.especie').parent().attr('rowspan', objeto.Linhas);
				linha.find('.areaCorte').text(objeto.AreaCorte);
				linha.find('.areaCorte').attr('title', objeto.AreaCorte);
				linha.find('.areaCorte').parent().attr('rowspan', objeto.Linhas);
				linha.find('.idadePlantio').text(objeto.IdadePlantio);
				linha.find('.idadePlantio').attr('title', objeto.IdadePlantio);
				linha.find('.idadePlantio').parent().attr('rowspan', objeto.Linhas);
			} else {
				linha.find('.tipoCorte').parent().remove();
				linha.find('.especie').parent().remove();
				linha.find('.areaCorte').parent().remove();
				linha.find('.idadePlantio').parent().remove();
			}

			objeto.DestinacaoMaterial = item.DestinacaoMaterial;
			objeto.DestinacaoMaterialTexto = item.DestinacaoMaterialTexto;
			objeto.Produto = item.Produto;
			objeto.ProdutoTexto = item.ProdutoTexto;
			objeto.Quantidade = item.Quantidade;
			linha.find('.itemJson').val(JSON.stringify(objeto));
			linha.find('.destinacaoMaterial').text(objeto.DestinacaoMaterialTexto);
			linha.find('.destinacaoMaterial').attr('title', objeto.DestinacaoMaterialTexto);
			linha.find('.produto').text(objeto.ProdutoTexto);
			linha.find('.produto').attr('title', objeto.ProdutoTexto);
			linha.find('.quantidade').text(objeto.Quantidade);
			linha.find('.quantidade').attr('title', objeto.Quantidade);

			linha.removeClass('trTemplateRow hide');
			$('.tabInformacaoCorte > tbody:last', container).append(linha);
			Listar.atualizarEstiloTable($('.tabInformacaoCorte', container));
		});

		InformacaoCorte.limparTipo();
		$('.tabDestinacao > tbody > tr:not(".trTemplateRow")', InformacaoCorte.container).remove();
	},

	excluirInformacao: function () {
		var linha = $(this).parent().parent();
		if (linha.find('.tipoCorte').length > 0) {
			var rowspan = linha.find('.tipoCorte').parent().attr('rowspan');
			if (rowspan == 1) {
				$(this).closest('tr').remove();
				Listar.atualizarEstiloTable($('.tabInformacaoCorte', InformacaoCorte.container));
				return;
			}

			var nextLinha = $($('.tabInformacaoCorte > tbody > tr:not(".trTemplateRow")')[linha.index()]);
			if (linha.index() == nextLinha.index())
				nextLinha = $($('.tabInformacaoCorte > tbody > tr:not(".trTemplateRow")')[linha.index() + 1]);
			$(this).closest('tr').remove();
			if (nextLinha.find('.tipoCorte').length > 0) return;

			var tipoCorte = linha.find('.tipoCorte').parent().clone();
			var especie = linha.find('.especie').parent().clone();
			var areaCorte = linha.find('.areaCorte').parent().clone();
			var idadePlantio = linha.find('.idadePlantio').parent().clone();

			tipoCorte.attr('rowspan', rowspan - 1);
			especie.attr('rowspan', rowspan - 1);
			areaCorte.attr('rowspan', rowspan - 1);
			idadePlantio.attr('rowspan', rowspan - 1);

			var children = nextLinha.children();
			children.remove();
			nextLinha.append(tipoCorte);
			nextLinha.append(especie);
			nextLinha.append(areaCorte);
			nextLinha.append(idadePlantio);
			nextLinha.append(children);
		} else if (linha.index() < 0) {
			$(this).closest('tr').remove();
		}
		else {
			InformacaoCorte.downRowspan(linha.index());
			$(this).closest('tr').remove();
		}
		Listar.atualizarEstiloTable($('.tabInformacaoCorte', InformacaoCorte.container));
	},

	downRowspan: function (index) {
		if (index < 0) {
			$(this).closest('tr').remove();
		} else {
			var previousRow = $($('.tabInformacaoCorte > tbody > tr:not(".trTemplateRow")')[index - 1]);
			if (previousRow.find('.tipoCorte').length > 0) {
				var rowspan = previousRow.find('.tipoCorte').parent().attr('rowspan');
				previousRow.find('.tipoCorte').parent().attr('rowspan', rowspan - 1);
				previousRow.find('.especie').parent().attr('rowspan', rowspan - 1);
				previousRow.find('.areaCorte').parent().attr('rowspan', rowspan - 1);
				previousRow.find('.idadePlantio').parent().attr('rowspan', rowspan - 1);
			} else {
				InformacaoCorte.downRowspan(index - 1);
			}
		}
	},

	obter: function () {
		var container = InformacaoCorte.container;
		var informacaoCorte = {
			Id: $('.codigoInformacaoCorte', container).val() > 0 ? $('.codigoInformacaoCorte', container).val() : 0,
			Empreendimento: { Id: $('.hdnEmpreendimentoId', container).val() },
			DataInformacao: { DataTexto: $('.dataInformacao', container).val() },
			AreaFlorestaPlantada: $('.areaPlantada', container).val(),
			AreaImovel: $('.areaImovelInf', container).val(),
			InformacaoCorteLicenca: [],
			InformacaoCorteTipo: []
		};
		var informacaoCorteTipo = {
			Id: 0,
			TipoCorte: '',
			EspecieInformada: '',
			AreaCorte: '',
			IdadePlantio: '',
			InformacaoCorteDestinacao: []
		};

		$('.tabLicencas > tbody > tr:not(".trTemplateRow") > td.tdAcoes > input.itemJson').toArray().map(x =>
			informacaoCorte.InformacaoCorteLicenca.push(JSON.parse(x.value))
		);

		$('.tabInformacaoCorte > tbody > tr:not(".trTemplateRow") > td.tdAcoes > input.itemJson', container).toArray().map(function (x) {
			var item = JSON.parse(x.value);

			var informacaoCorteDestinacao = {
				Id: item.DestinacaoId,
				DestinacaoMaterial: item.DestinacaoMaterial,
				Produto: item.Produto,
				Quantidade: item.Quantidade
			};

			if (!(informacaoCorteTipo.TipoCorte == item.TipoCorte &&
				informacaoCorteTipo.EspecieInformada == item.Especie &&
				informacaoCorteTipo.AreaCorte == item.AreaCorte &&
				informacaoCorteTipo.IdadePlantio == item.IdadePlantio)) {
				informacaoCorteTipo = {
					Id: item.Id,
					TipoCorte: item.TipoCorte,
					EspecieInformada: item.Especie,
					AreaCorte: item.AreaCorte,
					IdadePlantio: item.IdadePlantio,
					InformacaoCorteDestinacao: []
				};
			}
			informacaoCorteTipo.InformacaoCorteDestinacao.push(informacaoCorteDestinacao);
			if (!informacaoCorte.InformacaoCorteTipo.contains(informacaoCorteTipo))
				informacaoCorte.InformacaoCorteTipo.push(informacaoCorteTipo);
		});

		return informacaoCorte;
	},

	salvar: function () {
		var container = InformacaoCorte.container;
		Mensagem.limpar(container);

		var msgValidacao = [];
		var objeto = InformacaoCorte.obter();

		if (objeto.AreaFlorestaPlantada <= 0) {
			msgValidacao.push(InformacaoCorte.settings.mensagens.AreaPlantadaObrigatoria);
		}

		if (!$('.ckbDeclaracaoVerdadeira', container).prop('checked')) {
			msgValidacao.push(InformacaoCorte.settings.mensagens.Declaracao1Obrigatoria);
		}

		if (!$('.ckbResponsabilidadePelasDeclaracoes', container).prop('checked')) {
			msgValidacao.push(InformacaoCorte.settings.mensagens.Declaracao2Obrigatoria);
		}

		if (msgValidacao.length > 0) {
			Mensagem.gerar(InformacaoCorte.container, msgValidacao);
			return;
		}

		MasterPage.carregando(true);
		$.ajax({
			url: InformacaoCorte.settings.urls.salvar,
			data: JSON.stringify({ caracterizacao: objeto, projetoDigitalId: $('.hdnProjetoDigitalId', InformacaoCorte.container).val() }),
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
					Mensagem.gerar(InformacaoCorte.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	carregarProdutos: function () {
		$.get(InformacaoCorte.settings.urls.obterProdutos, { destinacaoId: $('.ddlDestinacaoMaterial', InformacaoCorte.container).val() }, function (lista) {
			$('.ddlProduto', InformacaoCorte.container).ddlLoad(lista);
		});
	}
};
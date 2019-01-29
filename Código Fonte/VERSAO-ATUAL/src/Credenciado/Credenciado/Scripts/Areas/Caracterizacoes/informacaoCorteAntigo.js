/// <reference path="../../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

InformacaoCorte = {
	settings: {
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(InformacaoCorte.settings, options); }
		InformacaoCorte.container = MasterPage.getContent(container);
		InformacoesCortesInformacoes.load(container);
	},

	atualizarEstilo: function () {
		MasterPage.botoes();
		Listar.atualizarEstiloTable(InformacaoCorte.container.find('.dataGridTable'));
	},

	atualizarInformacoesTotalEmpreendimento: function () {
		var informacoes = InformacaoCorte.obter();

		var arvoresIsoladasRestanteTotal = 0;
		var areaCorteRestanteTotal = 0;
		var arvoresIsoladas = 0;
		var areaCorte = 0;

		$(informacoes.InformacoesCortes).each(function () {
			if (!isNaN(this.ArvoresIsoladasRestantes)) {
				arvoresIsoladasRestanteTotal += Number(this.ArvoresIsoladasRestantes);
			}
		});

		$(informacoes.InformacoesCortes).each(function () {
			areaCorteRestanteTotal += Number(Mascara.getFloatMask(this.AreaCorteRestante.toString()));
		});

		$(informacoes.InformacoesCortes).each(function () {
			if (!isNaN(this.ArvoresIsoladas)) {
				arvoresIsoladas += Number(this.ArvoresIsoladas);
			}
		});

		$(informacoes.InformacoesCortes).each(function () {
			areaCorte += Number(Mascara.getFloatMask(this.AreaCorte.toString()));
		});

		$('.txtArvoresIsoladasRestanteTotal', InformacaoCorte.container).val(arvoresIsoladasRestanteTotal);
		$('.txtAreaCorteRestanteTotal', InformacaoCorte.container).val(areaCorteRestanteTotal.toString().replace('.', ','));
		$('.txtArvoresIsoladasTotal', InformacaoCorte.container).val(arvoresIsoladas);
		$('.txtAreaCorteTotal', InformacaoCorte.container).val(areaCorte.toString().replace('.', ','));

	},

	obter: function () {
		var container = InformacaoCorte.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			Dependencias: JSON.parse(InformacaoCorte.settings.dependencias),
			EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
			ArvoresIsoladasTotal: $('.txtArvoresIsoladasTotal', container).val(),
			AreaCorteTotal: $('.txtAreaCorteTotal', container).val(),
			ArvoresIsoladasRestanteTotal: $('.txtArvoresIsoladasRestanteTotal', container).val(),
			AreaCorteRestanteTotal: $('.txtAreaCorteRestanteTotal', container).val(),
			InformacoesCortes: []
		};

		$('.hdnItemJSon', container).each(function () {
			var objInf = String($(this).val());
			if (objInf != '') {
				obj.InformacoesCortes.push(JSON.parse(objInf));
			}
		});

		return obj;
	}
};

InformacoesCortesInformacoes = {
	settings: {
		urls: {
			salvar: '',
			editar: '',
			visualizar: '',
			Excluir: '',
			ExcluirConfirm: ''
		},
		salvarCallBack: null,
		mensagens: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(InformacoesCortesInformacoes.settings, options); }
		InformacoesCortesInformacoes.container = MasterPage.getContent(container);

		InformacoesCortesInformacoes.container.delegate('.btnAdicionarInformacaoCorte', 'click', InformacoesCortesInformacoes.adicionar);
		InformacoesCortesInformacoes.container.delegate('.btnVisualizarInformacaoCorte', 'click', InformacoesCortesInformacoes.visualizar);
		InformacoesCortesInformacoes.container.delegate('.btnEditarInformacaoCorte', 'click', InformacoesCortesInformacoes.editar);
		InformacoesCortesInformacoes.container.delegate('.btnExcluirInformacaoCorte', 'click', InformacoesCortesInformacoes.excluir);
	},

	visualizar: function () {
		MasterPage.carregando(true);

		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		$.ajax({
			url: InformacoesCortesInformacoes.settings.urls.visualizar + '/' + itemId,
			data: null,
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, InformacaoCorte.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Html) {

					$('.divInformacao', InformacaoCorte.container).empty();
					$('.divInformacao', InformacaoCorte.container).append(response.Html);
					$('.divLinkVoltar', InformacaoCorte.container).hide()
					InformacaoCorteInformacao.load(InformacaoCorte.container, { mensagens: InformacoesCortesInformacoes.settings.mensagens });

					InformacaoCorte.atualizarEstilo();
					MasterPage.botoes(InformacaoCorte.container);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(InformacaoCorte.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	adicionar: function () {
		$(this).closest('fieldset').find('.dataGridTable tbody tr').removeClass('editando');

		MasterPage.carregando(true);
		$.ajax({
			url: InformacoesCortesInformacoes.settings.urls.salvar,
			data: null,
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, InformacaoCorte.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Html) {

					$('.divInformacao', InformacaoCorte.container).empty();
					$('.divInformacao', InformacaoCorte.container).html(response.Html);
					$('.divLinkVoltar', InformacaoCorte.container).hide()
					InformacaoCorteInformacao.load(InformacaoCorte.container, { mensagens: InformacoesCortesInformacoes.settings.mensagens });

					MasterPage.botoes(InformacaoCorte.container);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(InformacaoCorte.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	editar: function () {
		MasterPage.carregando(true);

		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		$.ajax({
			url: InformacoesCortesInformacoes.settings.urls.editar + '/' + itemId,
			data: null,
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, InformacaoCorte.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Html) {

					$('.divInformacao', InformacaoCorte.container).empty();
					$('.divInformacao', InformacaoCorte.container).append(response.Html);
					$('.divLinkVoltar', InformacaoCorte.container).hide()
					InformacaoCorteInformacao.load(InformacaoCorte.container, { mensagens: InformacoesCortesInformacoes.settings.mensagens });

					InformacaoCorte.atualizarEstilo();
					MasterPage.botoes(InformacaoCorte.container);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(InformacaoCorte.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	excluir: function () {
		Mensagem.limpar(InformacaoCorte.container);

		var id = $(this).closest('tr').find('.itemId:first').val();
		var empreendimentoId = $('.hdnEmpreendimentoId', InformacaoCorte.container).val()

		Modal.excluir({
			'urlConfirm': InformacoesCortesInformacoes.settings.urls.ExcluirConfirm,
			'urlAcao': InformacoesCortesInformacoes.settings.urls.Excluir + '?itemId=' + id + '&empreendimentoId=' + empreendimentoId,
			'id': id,
			'callBack': InformacoesCortesInformacoes.callBackExcluir,
			'naoExecutarUltimaBusca': true
		});
	},

	callBackExcluir: function (data) {
		if (data.Msg && data.Msg.length > 0) {
			Mensagem.gerar(InformacaoCorte.container, data.Msg);
		}

		$('.divCaracterizacao').empty();
		$('.divCaracterizacao').html(data.Html);

		InformacaoCorte.atualizarEstilo();
	}
};

InformacaoCorteInformacao = {
	settings: {
		urls: {
			visualizar: ''
		},
		isVisualizar: false
	},

	container: null,

	load: function (container, options) {
		if (options) { $.extend(InformacaoCorteInformacao.settings, options); }
		InformacaoCorteInformacao.container = container.find('.divInformacaoCorteInformacao');

		InformacaoCorteInformacao.container.delegate(".linkCancelar", "click", InformacaoCorteInformacao.limparInformacaoCorte);

		Especie.load(container);
		Produto.load(container);

		Mascara.load();
	},

	limparInformacaoCorte: function () {
		$('.divInformacaoCorteInformacao', InformacaoCorte.container).empty();
		$('.divLinkVoltar', InformacaoCorte.container).show();
	},

	obter: function () {
		var container = InformacaoCorteInformacao.container;

		var obj = InformacaoCorte.obter();
		obj.InformacaoCorteInformacao = {
			Id: $('.hdnInformacaoCorteInformacaoId', container).val(),
			CaracterizacaoId: $('.hdnCaracterizacaoId', InformacaoCorte.container).val(),
			ArvoresIsoladasRestantes: $('.txtArvoresIsoladasRestantes', container).val(),
			AreaCorteRestante: $('.txtAreaCorteRestante', container).val(),
			DataInformacao: { DataTexto: $('.txtDataInformacao', container).val() },
			ArvoresIsoladas: 0,
			AreaCorte: 0,
			Especies: Especie.obter(),
			Produtos: Produto.obter()
		};

		$(obj.Especies).each(function () {
			if (!isNaN(this.ArvoresIsoladas)) {
				obj.ArvoresIsoladas += Number(this.ArvoresIsoladas);
			}
		});

		$(obj.Especies).each(function () {
			if (this.AreaCorte) {
				obj.AreaCorte += Number(Mascara.getFloatMask(this.AreaCorte.toString()));
			}
		});

		return obj;
	}
};

Especie = {
	settings: {
		idsTela: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Especie.settings, options); }
		Especie.container = container.find('.informacaoCorteInformacaoEspecieContainer');

		Especie.container.delegate('.btnAdicionarEspecie', 'click', Especie.adicionar);
		Especie.container.delegate('.btnExcluirEspecie', 'click', Especie.excluir);
		Especie.container.delegate('.ddlEspecieTipo', 'change', Especie.gerenciarEspecie);
	},

	gerenciarEspecie: function () {
		var especie = $('.ddlEspecieTipo :selected', Especie.container).val();

		$('.divEspecificar', Especie.container).addClass('hide');
		if (especie == 4) {
			$('.divEspecificar', Especie.container).removeClass('hide');
		}
	},

	adicionar: function () {
		var mensagens = new Array();
		Mensagem.limpar(InformacaoCorte.container);
		var container = Especie.container;

		var especie = {
			Id: 0,
			Tid: '',
			EspecieTipo: $('.ddlEspecieTipo :selected', container).val(),
			EspecieTipoTexto: $('.ddlEspecieTipo :selected', container).text(),
			EspecieEspecificarTexto: $('.txtEspecieEspecificarTexto', container).val(),
			ArvoresIsoladas: $('.txtArvoresIsoladas', container).val(),
			AreaCorte: Mascara.getFloatMask($('.txtAreaCorte', container).val()),
			AreaCorteTexto: $('.txtAreaCorte', container).val(),
			IdadePlantio: $('.txtIdadePlantio', container).val()
		}

		if (especie.EspecieTipo <= 0) {
			mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.EspecieTipoObrigatorio));
		} else {
			if (especie.EspecieTipo == 4) {
				if (especie.EspecieEspecificarTexto == '') {
					mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.EspecieEspecificarObrigatorio));
				}
				especie.EspecieTipoTexto = especie.EspecieEspecificarTexto
			}
		}

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var esp = (JSON.parse(obj));
				if (esp.EspecieTipoTexto.toLowerCase() == especie.EspecieTipoTexto.toLowerCase()) {
					if (especie.EspecieTipo == 4) {
						mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.EspecieEspecificarDuplicada));
					} else {
						mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.EspecieDuplicada));
					}
					Mensagem.gerar(InformacaoCorte.container, mensagens);
					return;
				}
			}
		});

		if (especie.ArvoresIsoladas == '' && especie.AreaCorte == '') {
			mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.EspecieArvoresOuAreaObrigatorio));
		} else {

			if (especie.ArvoresIsoladas != '') {
				if (isNaN(especie.ArvoresIsoladas)) {
					mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.EspecieArvoresIsoladasInvalido));
				} else {
					if (Number(especie.ArvoresIsoladas) <= 0) {
						mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.EspecieArvoresIsoladasZero));
					}
				}
			}

			if (especie.AreaCorte != '') {
				if (isNaN(especie.AreaCorte)) {
					mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.EspecieAreaCorteInvalido));
				} else {
					if (Number(especie.AreaCorte) <= 0) {
						mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.EspecieAreaCorteZero));
					}
				}
			}
		}

		if (especie.IdadePlantio != '') {
			if (isNaN(especie.IdadePlantio)) {
				mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.EspecieIdadePlantioInvalido));
			} else {
				if (Number(especie.IdadePlantio) <= 0) {
					mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.EspecieIdadePlantioMaiorZero));
				}
			}
		}

		if (mensagens.length > 0) {
			Mensagem.gerar(InformacaoCorte.container, mensagens);
			return;
		}

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(especie));

		linha.find('.especieTipo').html(especie.EspecieTipoTexto).attr('title', especie.EspecieTipoTexto);
		linha.find('.arvoresIsoladas').html(especie.ArvoresIsoladas).attr('title', especie.ArvoresIsoladas);
		linha.find('.areaCorte').html(especie.AreaCorteTexto).attr('title', especie.AreaCorteTexto);
		linha.find('.idadePlantio').html(especie.IdadePlantio).attr('title', especie.IdadePlantio);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.ddlEspecieTipo', container).ddlFirst();
		$('.txtArvoresIsoladas', container).val('');
		$('.txtEspecieEspecificarTexto', container).val('');
		$('.txtAreaCorte', container).val('');
		$('.txtIdadePlantio', container).val('');

		Especie.gerenciarEspecie();

	},

	excluir: function () {
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	},

	obter: function () {
		var container = Especie.container;
		var objeto = [];

		$('.hdnItemJSon', container).each(function () {
			var objEspecie = String($(this).val());
			if (objEspecie != '') {
				objeto.push(JSON.parse(objEspecie));
			}
		});

		return objeto;
	}
};

Produto = {
	settings: {
		idsTela: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Produto.settings, options); }
		Produto.container = container.find('.informacaoCorteInformacaoProdutoContainer');

		Produto.container.delegate('.btnAdicionarProduto', 'click', Produto.adicionar);
		Produto.container.delegate('.btnExcluirProduto', 'click', Produto.excluir);
	},

	adicionar: function () {
		var mensagens = new Array();
		Mensagem.limpar(InformacaoCorte.container);
		var container = Produto.container;

		var produto = {
			Id: 0,
			Tid: '',
			ProdutoTipo: $('.ddlProdutoTipo :selected', container).val(),
			ProdutoTipoTexto: $('.ddlProdutoTipo :selected', container).text(),
			DestinacaoTipo: $('.ddlDestinacaoTipo :selected', container).val(),
			DestinacaoTipoTexto: $('.ddlDestinacaoTipo :selected', container).text(),
			Quantidade: Mascara.getFloatMask($('.txtProdutoQuantidade', container).val()),
			QuantidadeTexto: $('.txtProdutoQuantidade', container).val()
		}

		if (produto.ProdutoTipo <= 0) {
			mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.ProdutoTipoObrigatorio));
		} else {
			$('.hdnItemJSon', container).each(function () {
				var obj = String($(this).val());
				if (obj != '') {
					var prod = (JSON.parse(obj));
					if (prod.ProdutoTipo == produto.ProdutoTipo && prod.DestinacaoTipo == produto.DestinacaoTipo) {
						mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.ProdutoDuplicado));
					}
				}
			});
		}

		if (produto.DestinacaoTipo <= 0) {
			mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.ProdutoDestinacaoObrigatorio));
		}

		if (produto.Quantidade == '') {
			mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.ProdutoQuantidadeObrigatorio));
		} else {
			if (isNaN(produto.Quantidade)) {
				mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.ProdutoQuantidadeInvalido));
			} else {
				if (Number(produto.Quantidade) <= 0) {
					mensagens.push(jQuery.extend(true, {}, Especie.settings.mensagens.ProdutoQuantidadeMaiorZero));
				}
			}
		}

		if (mensagens.length > 0) {
			Mensagem.gerar(InformacaoCorte.container, mensagens);
			return;
		}


		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');

		linha.find('.hdnItemJSon').val(JSON.stringify(produto));

		linha.find('.produtoTipo').html(produto.ProdutoTipoTexto).attr('title', produto.ProdutoTipoTexto);
		linha.find('.destinacaoTipo').html(produto.DestinacaoTipoTexto).attr('title', produto.DestinacaoTipoTexto);
		linha.find('.quantidade').html(produto.QuantidadeTexto).attr('title', produto.QuantidadeTexto);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.ddlProdutoTipo', container).ddlFirst();
		$('.ddlDestinacaoTipo', container).ddlFirst();
		$('.txtProdutoQuantidade', container).val('');

	},

	excluir: function () {
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	},

	obter: function () {
		var container = Produto.container;
		var objeto = [];

		$('.hdnItemJSon', container).each(function () {
			var objProduto = String($(this).val());
			if (objProduto != '') {
				objeto.push(JSON.parse(objProduto));
			}
		});

		return objeto;
	}
};
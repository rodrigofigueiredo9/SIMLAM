/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />

PTVOutroEmitir = {
	settings: {
		urls: {
			urlVerificarNumeroPTV: null,
			urlAssociarCultura: null,
			urlObterCultivar: null,
			urlObterMunicipio: null,
			urlAdicionarProdutos: null,
			urlValidarDocumento: null,
			urlAssociarDestinatario: null,
			urlObterDestinatario: null,
			urlSalvar: null
		}
	},
	container: null,

	ajax: function (request) {
		MasterPage.carregando(true);
		Mensagem.limpar(PTVOutroEmitir.container);
		$.ajax({
			url: request.url,
			data: JSON.stringify(request.data),
			cache: false,
			async: true,
			type: request.type ? request.type : 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				try {
					request.success(response, textStatus, XMLHttpRequest);
				} catch (e) {
					throw e;
				}
				finally {
					if (response.Erros && response.Erros.length > 0) {
						Mensagem.gerar(PTVOutroEmitir.container, response.Erros);
					} else {
						Mensagem.gerar(PTVOutroEmitir.container, response.Msg);
					}
					MasterPage.carregando(false);
				}
			}
		});
	},

	load: function (container, options) {
		if (options) { $.extend(PTVOutroEmitir.settings, options); }

		PTVOutroEmitir.container = MasterPage.getContent(container);
		PTVOutroEmitir.container.delegate('.btnVerificarPTV', 'click', PTVOutroEmitir.onVerificarPTV);
		PTVOutroEmitir.container.delegate('.btnLimparPTV', 'click', PTVOutroEmitir.onLimparNumeroPTV);

		//1-Identificação do Produto
		PTVOutroEmitir.container.delegate('.ddlOrigemTipo', 'change', PTVOutroEmitir.onChangeOrigemTipo);
		PTVOutroEmitir.container.delegate('.btnIdentificacaoProduto', 'click', PTVOutroEmitir.onAdicionarIdentificacaoProduto);
		PTVOutroEmitir.container.delegate('.btnExcluir', 'click', PTVOutroEmitir.onExcluirIdentificacaoProduto);

		//2-Identificação do Produto
		PTVOutroEmitir.container.delegate('.btnAssociarCultura', 'click', PTVOutroEmitir.associarCultura);
		PTVOutroEmitir.container.delegate('.rbTipoDocumento', 'change', PTVOutroEmitir.onTipoPessoaChange);
		PTVOutroEmitir.container.delegate('.btnVerificarDestinatario', 'click', PTVOutroEmitir.onValidarDocumento);
		PTVOutroEmitir.container.delegate('.btnLimparDestinatario', 'click', PTVOutroEmitir.onLimparDestinatario);
		PTVOutroEmitir.container.delegate('.btnNovoDestinatario', 'click', PTVOutroEmitir.onAssociarDestinatario);

		PTVOutroEmitir.container.delegate('.ddlProdutoUnidadeMedida', 'change', function () { $('.txtProdutoQuantidade', PTVOutroEmitir.container).focus(); })

		PTVOutroEmitir.container.delegate('.ddlEstados', 'change', PTVOutroEmitir.onObterMunicipio);
		PTVOutroEmitir.container.delegate('.btnSalvar', 'click', PTVOutroEmitir.onSalvar);

		PTVOutroEmitir.container.delegate('input[name="CnpjCpf"]', 'click', PTVOutroEmitir.onClickRadioCnpjCpf);
		PTVOutroEmitir.container.delegate('.ddlEstadosInteressado', 'change', PTVOutroEmitir.onObterMunicipioInteressado);

		if (parseInt($('.hdnID', PTVOutroEmitir.container).val()) > 0) {
			PTVOutroEmitir.habilitarCampos(false);
			$('.btnLimparPTV', PTVOutroEmitir.container).hide();
		}
		PTVOutroEmitir.onTipoPessoaChange();
		Aux.setarFoco(container);
	},

	onLoadMunicipio: function (element, estadoId) {
		PTVOutroEmitir.ajax({
			url: PTVOutroEmitir.settings.urls.urlObterMunicipio,
			data: { estado: estadoId },
			success: function (response, textStatus, XMLHttpRequest) {
				element.ddlLoad(response.Municipios);
			}
		});
	},

	onClickRadioCnpjCpf: function () {
		var txtInteressadoCpfCnpj = $('.txtInteressadoCpfCnpj', PTVOutroEmitir.container);
		txtInteressadoCpfCnpj.val('');
		txtInteressadoCpfCnpj.removeAttr('disabled').removeClass('disabled');
		if ($(this).hasClass('rbTipoPessoaFisicaPF')) { /*1-CPF rbTipoPessoaFisica*/
			txtInteressadoCpfCnpj.unmask().mask("999.999.999-99");
		} else { /*2-CNPJ rbTipoPessoaJuridica*/
			txtInteressadoCpfCnpj.unmask().mask("99.999.999/9999-99");
		}
	},

	onObterMunicipioInteressado: function () {
		var ddl = PTVOutroEmitir.container.find('.ddlEstadosInteressado');
		var ddlSelecionado = ddl.ddlSelecionado();
		PTVOutroEmitir.onLoadMunicipio($('.ddlMunicipiosInteressado', PTVOutroEmitir.container), ddlSelecionado.Id);
	},

	onVerificarPTV: function () {
		var txtNumero = $('.txtNumero', PTVOutroEmitir.container);
		PTVOutroEmitir.ajax({
			url: PTVOutroEmitir.settings.urls.urlVerificarNumeroPTV,
			data: { numero: txtNumero.val() },
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Valido) {
					var interessado = $('.interessado', PTVOutroEmitir.container);
					$('input, select', interessado).removeClass('disabled').removeAttr('disabled');
					$('.btnVerificarPTV', PTVOutroEmitir.container).addClass('hide');
					$('.campoTela, .btnLimparPTV', PTVOutroEmitir.container).removeClass('hide');
					txtNumero.addClass('disabled').attr('disabled', 'disabled');
				}
			}
		});
	},

	onLimparNumeroPTV: function () {
		$('.campoTela, .btnLimparPTV', PTVOutroEmitir.container).addClass('hide');
		$('.btnVerificarPTV', PTVOutroEmitir.container).removeClass('hide');
		$('.hdnEmpreendimentoID,.txtEmpreendimento', PTVOutroEmitir.container).val("");
		$('.ddlResponsaveis', PTVOutroEmitir.container).val("");
		$('.rbTipoNumero, .txtNumero', PTVOutroEmitir.container).removeClass('disabled').removeAttr('disabled');
		$(".rbTipoNumero", PTVOutroEmitir.container).attr("checked", false);
		$('.btnModalOu', PTVOutroEmitir.container).hide();
		$('.txtNumero', PTVOutroEmitir.container).val('');
		$('.ddlOrigemTipo', PTVOutroEmitir.container).ddlFirst();
		$('.ddlNumeroOrigem, .ddlProdutoCultura, .ddlProdutoCultivar', PTVOutroEmitir.container).ddlClear();
		$('.txtProdutoQuantidade', PTVOutroEmitir.container).val('');
		$('.ddlProdutoUnidadeMedida', PTVOutroEmitir.container).val('');

		//##
		var interessado = $('.interessado', PTVOutroEmitir.container);
		$('input, select', interessado).addClass('disabled').attr('disabled', 'disabled').val('');
		$('input[type=radio]', interessado).each(function () { this.checked = false; });
	},

	onChangeOrigemTipo: function () {
		Mensagem.limpar(PTVOutroEmitir.container);
		var labelOrigem = $('.labelOrigem', PTVOutroEmitir.container);
		labelOrigem.text('');
		var option = $('option:selected', this);
		if (option.val() != '' && option.val() != '0') {
			labelOrigem.text(option.text());
		}
	},

	associarCultura: function () {
		Modal.abrir(PTVOutroEmitir.settings.urls.urlAssociarCultura, null, function (container) {
			CulturaListar.load(container, { onAssociarCallback: PTVOutroEmitir.callBackAssociarCultura });
			Modal.defaultButtons(container);
		});
	},

	callBackAssociarCultura: function (response) {

		$('.txtCultura', PTVOutroEmitir.container).val(response.Nome);
		$('.hdnCulturaId', PTVOutroEmitir.container).val(response.Id);
		$('.ddlProdutoCultura', PTVOutroEmitir.container).append(new Option(response.Nome, response.Id, true, true));

		if (response.Id != '0') {

			var origemTipo = $('.ddlOrigemTipo', PTVOutroEmitir.container).val();
			var culturaID = $('.ddlProdutoCultura', PTVOutroEmitir.container).val();

			PTVOutroEmitir.ajax({
				url: PTVOutroEmitir.settings.urls.urlObterCultivar,
				data: { origemTipo: origemTipo, culturaID: culturaID },
				success: function (response, textStatus, XMLHttpRequest) {
					$('.ddlProdutoCultivar', PTVOutroEmitir.container).ddlLoad(response.Cultivar);
				}
			});
		}
		return true;
	},

	onAdicionarIdentificacaoProduto: function () {
		Mensagem.limpar(PTVOutroEmitir.container);
		var validacao = true;

		var container = $(this).closest('.identificacao_produto');
		var tabela = $('.gridProdutos', container);
		var IsNumeroOrigem = ($('.ddlOrigemTipo', container).val() > 3);
		var NumeroOrigemTexto = IsNumeroOrigem ? $('.txtNumeroOrigem', container).val() : $('.ddlNumeroOrigem option:selected', container).text();

		var item = {
			OrigemTipo: $('.ddlOrigemTipo', container).val(),
			OrigemTipoTexto: $('.ddlOrigemTipo option:selected', container).text(),
			OrigemNumero: $('.txtNumeroOrigem', container).val(),
			Cultura: $('.hdnCulturaId', container).val(),
			CulturaTexto: $('.txtCultura', container).val(),
			Cultivar: $('.ddlProdutoCultivar', container).val(),
			CultivarTexto: $('.ddlProdutoCultivar option:selected', container).text(),
			UnidadeMedida: $('.ddlProdutoUnidadeMedida option:selected', container).val(),
			UnidadeMedidaTexto: $('.ddlProdutoUnidadeMedida option:selected', container).text(),
			Quantidade: $('.txtProdutoQuantidade', container).val()
		};

		//Valida Item já adicionado na Grid	
		var _objeto = { Produtos: [] }
		$($('.gridProdutos tbody tr:not(.trTemplate) .hdnItemJson', container)).each(function () {
			_objeto.Produtos.push(JSON.parse($(this).val()));
		});

		if (_objeto.Produtos.length <= 0) {
			_objeto.Produtos = null;
		}

		var ehValido = MasterPage.validarAjax(PTVOutroEmitir.settings.urls.urlAdicionarProdutos, { item: item, lista: _objeto.Produtos }, PTVOutroEmitir.container, false).EhValido;
		if (!ehValido) {
			return;
		}

		var linha = $('.trTemplate', tabela).clone();
		$(linha).removeClass('hide trTemplate');

		//adicionar na grid
		$('.hdnItemJson', linha).val(JSON.stringify(item));
		$('.lblOrigemTipo', linha).html(item.OrigemTipoTexto + '-' + NumeroOrigemTexto).attr('title', item.OrigemTipoTexto + '-' + NumeroOrigemTexto);
		$('.lblCulturaCultivar', linha).html(item.CulturaTexto + '/' + item.CultivarTexto).attr('title', item.CulturaTexto + '/' + item.CultivarTexto);
		$('.lblQuantidade', linha).html(item.Quantidade).attr('title', item.Quantidade);
		$('.lblUnidadeMedida', linha).html(item.UnidadeMedidaTexto).attr('title', item.UnidadeMedidaTexto);

		$('tbody', tabela).append(linha);

		$('select', container).ddlFirst();
		$('.ddlProdutoCultivar', container).ddlClear({ disabled: false });
		$('input[type=text]', container).val("");
		$('.ddlOrigemTipo', container).focus();

		Listar.atualizarEstiloTable(tabela);
	},

	onObterMunicipio: function () {
		var ddl = PTVOutroEmitir.container.find('.ddlEstados');
		var ddlSelecionado = ddl.ddlSelecionado();
		PTVOutroEmitir.onLoadMunicipio($('.ddlMunicipios', PTVOutroEmitir.container), ddlSelecionado.Id);
	},

	habilitarCampos: function (habilita) {
		if (habilitado) {
			$('.btnValidar', PTVOutroEmitir.container).show();
			$('.btnLimpar', PTVOutroEmitir.container).hide();
			$('.rbTipoDocumento', PTVOutroEmitir.container).removeAttr('disabled');
		} else {
			$('.btnValidar', PTVOutroEmitir.container).hide();
			$('.btnLimpar', PTVOutroEmitir.container).show();
			$('.block', PTVOutroEmitir.container).removeClass('hide');
			$('.rbTipoDocumento', PTVOutroEmitir.container).attr('disabled', 'disabled');
		}
	},

	onExcluirIdentificacaoProduto: function () {
		Mensagem.limpar(PTVOutroEmitir.container);
		var container = $(this).closest('.gridIdentificacaoProdutos');
		$(this).closest('tr').toggle(
			function () {
				$(this).remove();
			});
		Listar.atualizarEstiloTable(container);
	},

	onTipoPessoaChange: function () {

		var container = $(this).closest('.destinatario');

		$('.txtDocumentoCpfCnpj', container).val('');
		$('.btnNovoDestinatario', container).addClass('hide');

		if ($('.rbTipoPessoaFisica').attr('checked')) {
			$('.lblCPFCNPJ', container).html('CPF *');
			$('.txtDocumentoCpfCnpj', container).removeClass('maskCnpj').unmask().addClass('maskCpf').mask("999.999.999-99");
		} else {
			$('.txtDocumentoCpfCnpj', container).removeClass('maskCpf').unmask().addClass('maskCnpj').mask("99.999.999/9999-99");
			$('.lblCPFCNPJ', container).html('CNPJ *');
		}

		$('.txtDocumentoCpfCnpj', container).removeAttr('disabled').removeClass('disabled').focus();
	},

	onValidarDocumento: function () {
		Mensagem.limpar(PTVOutroEmitir.container);
		var container = $(this).closest('.destinatario');
		var pessoaTipo = $('.rbTipoDocumento:checked', container).val() ? $('.rbTipoDocumento:checked', container).val() : 0;
		var CpfCnpj = $('.txtDocumentoCpfCnpj', container).val();

		MasterPage.carregando(true);
		$.ajax({
			url: PTVOutroEmitir.settings.urls.urlValidarDocumento,
			data: JSON.stringify({ pessoaTipo: pessoaTipo, CpfCnpj: CpfCnpj }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					PTVOutroEmitir.callBackAssociarDestinatario(response.Destinatario);
				} else if (response.NovoDestinatario) {
					$('.btnNovoDestinatario', container).removeClass('hide');
				}
				Mensagem.gerar(PTVOutroEmitir.container, response.Msg);
			}
		});
		MasterPage.carregando(false);
	},

	obterDestinatario: function (destinatarioID) {
		MasterPage.carregando(true);
		$.ajax({
			url: PTVOutroEmitir.settings.urls.urlObterDestinatario,
			data: JSON.stringify({ destinatarioID: destinatarioID }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.hdnDestinatarioID', PTVOutroEmitir.container).val(response.Destinatario.ID);
					$('.txtNomeDestinatario', PTVOutroEmitir.container).val(response.Destinatario.NomeRazaoSocial);
					$('.txtEndereco', PTVOutroEmitir.container).val(response.Destinatario.Endereco);
					$('.hdnUfID', PTVOutroEmitir.container).val(response.Destinatario.EstadoID);
					$('.txtUF', PTVOutroEmitir.container).val(response.Destinatario.EstadoTexto);
					$('.hdnMunicipioID', PTVOutroEmitir.container).val(response.Destinatario.MunicipioID);
					$('.txtMunicipio', PTVOutroEmitir.container).val(response.Destinatario.MunicipioTexto);
					$('.txtItinerario', PTVOutroEmitir.container).val(response.Destinatario.Itinerario);

					$('.btnVerificarDestinatario, .btnNovoDestinatario', PTVOutroEmitir.container).addClass('hide');
					$('.btnLimparDestinatario, .destinatarioDados', PTVOutroEmitir.container).removeClass('hide');
					$('.rbTipoDocumento, .txtDocumentoCpfCnpj', PTVOutroEmitir.container).addClass('disabled').attr('disabled', 'disabled');
				}

				Mensagem.gerar(PTVOutroEmitir.container, response.Msg);
			}
		});
		MasterPage.carregando(false);
	},

	onLimparDestinatario: function () {
		var container = $(this).closest('.destinatario');
		$('.txtDocumentoCpfCnpj', container).val("");
		$('.txtNomeDestinatario', container).val("");
		$('.txtEndereco', container).val("");
		$('.txtUF', container).val("");
		$('.txtMunicipio', container).val("");

		$('.btnVerificarDestinatario', container).removeClass('hide');
		$('.btnLimparDestinatario, .destinatarioDados, .btnNovoDestinatario', container).addClass('hide');
		$('.rbTipoDocumento, .txtDocumentoCpfCnpj', container).removeClass('disabled').removeAttr('disabled');
		$('.txtDocumentoCpfCnpj', container).focus();
	},

	onAssociarDestinatario: function () {
		Modal.abrir(PTVOutroEmitir.settings.urls.urlAssociarDestinatario, null, function (container) {
			DestinatarioPTV.load(container, {
				associarFuncao: PTVOutroEmitir.callBackAssociarDestinatario
			});
			Modal.defaultButtons(container, DestinatarioPTV.salvar, "Salvar");
		}, Modal.tamanhoModalMedia);
	},

	callBackAssociarDestinatario: function (destinatario) {
		$('.hdnDestinatarioID', PTVOutroEmitir.container).val(destinatario.ID);
		$('.txtNomeDestinatario', PTVOutroEmitir.container).val(destinatario.NomeRazaoSocial);
		$('.txtEndereco', PTVOutroEmitir.container).val(destinatario.Endereco);
		$('.txtUF', PTVOutroEmitir.container).val(destinatario.EstadoSigla);
		$('.txtMunicipio', PTVOutroEmitir.container).val(destinatario.MunicipioTexto);
		$('.txtItinerario', PTVOutroEmitir.container).val(destinatario.Itinerario);

		$('.btnVerificarDestinatario, .btnNovoDestinatario', PTVOutroEmitir.container).addClass('hide');
		$('.btnLimparDestinatario, .destinatarioDados', PTVOutroEmitir.container).removeClass('hide');
		$('.rbTipoDocumento, .txtDocumentoCpfCnpj', PTVOutroEmitir.container).addClass('disabled').attr('disabled', 'disabled');
	},

	onSalvar: function () {
		
		PTVOutroEmitir.ajax({
			url: PTVOutroEmitir.settings.urls.urlSalvar,
			data: PTVOutroEmitir.obter(),
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.Url);
				}
			}
		});
		
	},

	obter: function () {

		var pessoaTipo = $('.rbTipoDocumento:checked', PTVOutroEmitir.container).val();

		var objeto = {

			Id: $('.hdnEmissaoId', PTVOutroEmitir.container).val(),
			Numero: $('.txtNumero', PTVOutroEmitir.container).val(),
			DataEmissao: { DataTexto: $('.txtDataEmissao', PTVOutroEmitir.container).val() },
			Situacao: $('.ddlSituacoes', PTVOutroEmitir.container).val(),
			SituacaoTexto: $('.ddlSituacoes option:selected', PTVOutroEmitir.container).text(),
			DestinatarioID: $('.hdnDestinatarioID', PTVOutroEmitir.container).val(),
			ValidoAte: { DataTexto: $('.txtDataValidade', PTVOutroEmitir.container).val() },
			LocalEmissaoId: $('.ddlLocalEmissao', PTVOutroEmitir.container).val(),
			Interessado: $('.txtInteressado', PTVOutroEmitir.container).val(),
			InteressadoCnpjCpf: $('.txtInteressadoCpfCnpj', PTVOutroEmitir.container).val(),
			InteressadoEndereco: $('.txtInteressadoEndereco', PTVOutroEmitir.container).val(),
			InteressadoEstadoId: $('.ddlEstadosInteressado', PTVOutroEmitir.container).val(),
			InteressadoEstadoTexto: $('.ddlEstadosInteressado option:selected', PTVOutroEmitir.container).text(),
			InteressadoMunicipioId: $('.ddlMunicipiosInteressado', PTVOutroEmitir.container).val(),
			InteressadoMunicipioTexto: $('.ddlMunicipiosInteressado option:selected', PTVOutroEmitir.container).text(),
			RespTecnico: $('.txtTecnico', PTVOutroEmitir.container).val(),
			RespTecnicoNumHab: $('.txtNumHab', PTVOutroEmitir.container).val(),
			Estado: $('.ddlEstados', PTVOutroEmitir.container).val(),
			Municipio: $('.ddlMunicipios', PTVOutroEmitir.container).val(),

			Produtos: []
		}

		var retorno = [];

		$('.gridProdutos tbody tr:not(.trTemplate)', PTVOutroEmitir.container).each(function () {
			retorno.push(JSON.parse($('.hdnItemJson', this).val()));
		});

		objeto.Produtos = retorno;

		return objeto;
	}
}
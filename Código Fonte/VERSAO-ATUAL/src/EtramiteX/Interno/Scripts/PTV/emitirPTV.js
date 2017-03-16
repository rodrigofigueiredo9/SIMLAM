/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

PTVEmitir = {
	settings: {
		urls: {
			urlSalvar: null,
			urlObterMunicipio: null,
			urlVerificarNumeroPTV: null,
			urlAssociarEmpreendimento: null,
			urlAssociarCultura: null,
			urlAssociarDestinatario: null,
			urlObterResponsaveisEmpreend: null,
			urlObterNumeroOrigem: null,
			urlObterCultura: null,
			urlObterCultivar: null,
			urlObterUnidadeMedida: null,
			urlAdicionarProdutos: null,
			urlValidarDocumento: null,
			urlObterDestinatario: null,
			urlObterLaboratorio: null,
			urlObterMunicipio: null,
			urlObterDadosLaboratorio: null,
			urlObterTratamentoFisso: null,
			urlObterItinerario: null,
			urlVerificarDocumentoOrigem: null
		},
		Mensagens: null,
		idsTela: null,
		idsOrigem: null,
		dataAtual: null,
		onChangeProcDocEmp: new Array()
	},

	container: null,

	cidadeID: null,

	load: function (container, options) {
		if (options) { $.extend(PTVEmitir.settings, options); }

		PTVEmitir.container = MasterPage.getContent(container);

		PTVEmitir.container.delegate('.btnVerificarDua', 'click', PTVEmitir.onVerificarDua);
		PTVEmitir.container.delegate('.btnLimparDua', 'click', PTVEmitir.onLimparDua);
		$('.divDUA', PTVEmitir.container).keyup(PTVEmitir.verificarDUAEnter);

		PTVEmitir.container.delegate('.rbTipoNumero', 'change', PTVEmitir.onTipoNumeroChange);
		PTVEmitir.container.delegate('.btnVerificarPTV', 'click', PTVEmitir.onVerificarPTV);
		PTVEmitir.container.delegate('.btnLimparPTV', 'click', PTVEmitir.onLimparNumeroPTV);
		$('.divNumeroEnter', PTVEmitir.container).keyup(PTVEmitir.verificarNumeroEnter);

		//1-Identificação do Produto
		PTVEmitir.container.delegate('.ddlOrigemTipo', 'change', PTVEmitir.onChangeOrigemTipo);
		PTVEmitir.container.delegate('.ddlProdutoCultura', 'change', PTVEmitir.onChangeCultura);
		PTVEmitir.container.delegate('.ddlProdutoCultivar', 'change', PTVEmitir.chageCultivar);
		PTVEmitir.container.delegate('.btnIdentificacaoProduto', 'click', PTVEmitir.onAdicionarIdentificacaoProduto);
		PTVEmitir.container.delegate('.btnExcluir', 'click', PTVEmitir.onExcluirIdentificacaoProduto);

		//2-Identificação do Produto
		PTVEmitir.container.delegate('.btnAssociarCultura', 'click', PTVEmitir.associarCultura);
		PTVEmitir.container.delegate('.rbPartidaLacradaOrigem', 'change', PTVEmitir.onChangePartidaLacrada);
		PTVEmitir.container.delegate('.rbTipoDocumento', 'change', PTVEmitir.onTipoPessoaChange);
		PTVEmitir.container.delegate('.btnVerificarDestinatario', 'click', PTVEmitir.onValidarDocumento);
		PTVEmitir.container.delegate('.btnLimparDestinatario', 'click', PTVEmitir.onLimparDestinatario);
		PTVEmitir.container.delegate('.btnNovoDestinatario', 'click', PTVEmitir.onAssociarDestinatario);
		PTVEmitir.container.delegate('.ddlTipoTransporte', 'change', function () { if ($(this).val() != '0') { $('.txtIdentificacaoVeiculo', PTVEmitir.container).focus(); } });
		PTVEmitir.container.delegate('.btnVerificarDocumentoOrigem', 'click', PTVEmitir.verificarDocumentoOrigem);
		$('.divNumeroDocumentoEnter', PTVEmitir.container).keyup(PTVEmitir.verificarNumeroDocumentoEnter);

		PTVEmitir.container.delegate('.rdbPessaoTipo', 'change', PTVEmitir.onTipoPessoaDuaChange);

		PTVEmitir.container.delegate('.ddlProdutoUnidadeMedida', 'change', function () {
		    
		    var txtunidade = $('.ddlProdutoUnidadeMedida option:selected', container).text();

		   
		    
		    if (txtunidade == "KG") {
		        $('.txtProdutoQuantidade').unmaskMoney().maskMoney({ decimal: ',', thousands: '.', precision: 2 });
		        $('.txtProdutoQuantidade').attr('maxlength','8');
		    } else {
		        $('.txtProdutoQuantidade').unmaskMoney().maskMoney({ decimal: ',', thousands: '.', precision: 4 });
		        $('.txtProdutoQuantidade').attr('maxlength', '12');
		    }

		    $('.txtProdutoQuantidade', PTVEmitir.container).focus();
		});

		PTVEmitir.container.delegate('.rdbRotaTransitoDefinida', 'change', PTVEmitir.onChangeRotaTransitoDefinida);
		PTVEmitir.container.delegate('.rdbApresentacaoNotaFiscal', 'change', PTVEmitir.onChangeNumeroNotaFiscal);
		PTVEmitir.container.delegate('.btnSalvar', 'click', PTVEmitir.onSalvar);

		PTVEmitir.container.delegate('.rbTipoDocumento', 'change', PTVEmitir.onTipoPessoaChange);

		if (parseInt($('.hdnID', PTVEmitir.container).val()) > 0) {
			PTVEmitir.habilitarCampos(false);
			$('.btnLimparPTV', PTVEmitir.container).hide();
		}

		$('.txtNumeroDua', PTVEmitir.container).focus();
	},

	verificarNumeroEnter: function (e) {
		if (e.keyCode == MasterPage.keyENTER) {
			$('.btnVerificarPTV', PTVEmitir.container).click();
		}
		return false;
	},

	verificarNumeroDocumentoEnter: function (e) {
		if (e.keyCode == MasterPage.keyENTER) {
			$('.btnVerificarDocumentoOrigem', PTVEmitir.container).click();
		}
		return false;
	},

	onTipoNumeroChange: function () {
		Mensagem.limpar();
		$('.ddlSituacoes', PTVEmitir.container).ddlFirst();

		if ($('.rbTipoNumero:checked', PTVEmitir.container).val() == PTVEmitir.settings.idsTela.tipoNumeroBloco) {
			$('.txtNumero', PTVEmitir.container).removeClass('disabled').removeAttr('disabled');
			$('.txtDataEmissao', PTVEmitir.container).removeClass('disabled').removeAttr('disabled');
			$('.txtNumero', PTVEmitir.container).val('');
			$('.txtNumero', PTVEmitir.container).focus();
			$('.txtDataEmissao', PTVEmitir.container).val(PTVEmitir.settings.dataAtual);
		} else {
			$('.txtNumero', PTVEmitir.container).addClass('disabled').attr('disabled', 'disabled');
			$('.txtDataEmissao', PTVEmitir.container).addClass('disabled').attr('disabled', 'disabled');
			$('.txtNumero', PTVEmitir.container).val('Gerado automaticamente');
			$('.txtDataEmissao', PTVEmitir.container).val(PTVEmitir.settings.dataAtual).addClass('disabled').attr('disabled', 'disabled');;
			$('.ddlSituacoes', PTVEmitir.container).val('1').addClass('disabled').attr('disabled', 'disabled');;

		}
	},

	onVerificarPTV: function () {
		Mensagem.limpar(PTVEmitir.container);
		$.ajax({
			url: PTVEmitir.settings.urls.urlVerificarNumeroPTV,
			data: JSON.stringify({ numero: $('.txtNumero', PTVEmitir.container).val(), tipoNumero: +$('.rbTipoNumero:checked', PTVEmitir.container).val() || 0 }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Valido) {
					$('.btnVerificarPTV', PTVEmitir.container).addClass('hide');
					$('.campoTela, .btnLimparPTV', PTVEmitir.container).removeClass('hide');
					$('.rbTipoNumero, .txtNumero', PTVEmitir.container).addClass('disabled').attr('disabled', 'disabled');
				}
				Mensagem.gerar(PTVEmitir.container, response.Msg);
			}
		});
	},

	onTipoPessoaChange: function () {
	    Mensagem.limpar(PTVEmitir.container);
	    var container = $(this).closest('.destinatario');
	    $('.txtDocumentoCpfCnpj', container).val('');
	    $('.novoDestinatario', container).addClass('hide');

	    if ($('.rbTipoPessoaFisica').is(':checked')) {
	        $('.lblCPFCNPJ', container).html('CPF *');
	        $('.txtDocumentoCpfCnpj', container).removeClass('maskCnpj').unmask().addClass('maskCpf');
	    } else {
	        $('.txtDocumentoCpfCnpj', container).removeClass('maskCpf').unmask().addClass('maskCnpj');
	        $('.lblCPFCNPJ', container).html('CNPJ *');
	    }

	    Mascara.load(container);
	    $('.txtDocumentoCpfCnpj', container).focus();
	},

	onTipoPessoaDuaChange: function () {

	    $('.txtCPFDUA', PTVEmitir.container).val('');
	    $('.txtCNPJDUA', PTVEmitir.container).val('');

	    if ($('.rdbPessaoTipo:checked', PTVEmitir.container).val() == '1') {
	        $('.CnpjPessoaJuridicaContainer', PTVEmitir.container).addClass('hide');
	        $('.CpfPessoaFisicaContainer', PTVEmitir.container).removeClass('hide');
	    } else {
	        $('.CpfPessoaFisicaContainer', PTVEmitir.container).addClass('hide');
	        $('.CnpjPessoaJuridicaContainer', PTVEmitir.container).removeClass('hide');
	    }
	},

	onVerificarDua: function () {
	    Mensagem.limpar(PTVEmitir.container);
	    var Cpfcnpj = '';

	    if ($('.rdbPessaoTipo:checked', PTVEmitir.container).val() == '1') {
	        Cpfcnpj = $('.txtCPFDUA', PTVEmitir.container).val();
	    } else {
	        Cpfcnpj = $('.txtCNPJDUA', PTVEmitir.container).val();
	    }

	    PTVEmitir.RequisicaoDUA = { numero: $('.txtNumeroDua', PTVEmitir.container).val(), cpfcnpj: Cpfcnpj, tipo: $('.rdbPessaoTipo:checked', PTVEmitir.container).val(), ptvId: $('.hdnEmissaoId', PTVEmitir.container).val() };

	    MasterPage.carregando(true);

	    $.ajax({
	        url: PTVEmitir.settings.urls.urlGravarVerificacaoDUA,
	        data: JSON.stringify(PTVEmitir.RequisicaoDUA),
	        cache: false,
	        async: true,
	        type: 'POST',
	        dataType: 'json',
	        contentType: 'application/json; charset=utf-8',
	        error: Aux.error,
	        success: function (response, textStatus, XMLHttpRequest) {
	            if (!response.Valido) {
	                MasterPage.carregando(false);
	                Mensagem.gerar(PTVEmitir.container, response.Msg);
	                return;
	            }

	            PTVEmitir.RequisicaoDUA.filaID = response.FilaID;

	            clearTimeout(PTVEmitir.settings.timeoutID);
	            PTVEmitir.settings.timeoutID =
					setTimeout(function () {
					    PTVEmitir.onChecarRetornoDUA();
					}, 5000);
	        }
	    });
	},

	onChecarRetornoDUA: function () {
	    $.ajax({
	        url: PTVEmitir.settings.urls.urlVerificarConsultaDUA,
	        data: JSON.stringify(PTVEmitir.RequisicaoDUA),
	        cache: false,
	        async: true,
	        type: 'POST',
	        dataType: 'json',
	        contentType: 'application/json; charset=utf-8',
	        error: Aux.error,
	        success: function (response, textStatus, XMLHttpRequest) {
	            if (!response.Valido) {
	                MasterPage.carregando(false);
	                Mensagem.gerar(PTVEmitir.container, response.Msg);
	                return;
	            }

	            if (!response.Consultado) {
	                clearTimeout(PTVEmitir.settings.timeoutID);
	                PTVEmitir.settings.timeoutID =
						setTimeout(function () {
						    PTVEmitir.onChecarRetornoDUA();
						}, 5000);

	                return;
	            }

	            $('.linhaConteudo2', PTVEmitir.container).removeClass('hide');
	            $('.btnVerificarDua', PTVEmitir.container).addClass('hide');
	            $('.btnLimparDua', PTVEmitir.container).removeClass('hide');
	            //$('.campoTela', PTVEmitir.container).removeClass('hide');

	            $('.txtNumeroDua', PTVEmitir.container).addClass('disabled').attr('disabled', 'disabled');
	            $('.txtCNPJDUA', PTVEmitir.container).addClass('disabled').attr('disabled', 'disabled');
	            $('.txtCPFDUA', PTVEmitir.container).addClass('disabled').attr('disabled', 'disabled');
	            $('.rdbPessaoTipo', PTVEmitir.container).addClass('disabled').attr('disabled', 'disabled');

	            MasterPage.carregando(false);
	        }
	    });
	},

	onLimparDua: function () {
	    $('.linhaConteudo', PTVEmitir.container).addClass('hide');
	    $('.txtNumeroDua', PTVEmitir.container).removeClass('disabled').removeAttr('disabled');
	    $('.txtCNPJDUA', PTVEmitir.container).removeClass('disabled').removeAttr('disabled');
	    $('.txtCPFDUA', PTVEmitir.container).removeClass('disabled').removeAttr('disabled');

	    $('.rdbPessaoTipo', PTVEmitir.container).removeClass('disabled').removeAttr('disabled');

	    $('.campoTela', PTVEmitir.container).addClass('hide');


	    $('.btnVerificarDua', PTVEmitir.container).removeClass('hide');
	    $('.btnLimparDua', PTVEmitir.container).addClass('hide');

	},

	verificarDUAEnter: function (e) {
	    if (e.keyCode == MasterPage.keyENTER) {
	        $('.btnVerificarDua', PTVEmitir.container).click();
	    }
	    return false;
	},

	onLimparNumeroPTV: function () {
		$('.campoTela, .btnLimparPTV', PTVEmitir.container).addClass('hide');
		$('.btnVerificarPTV', PTVEmitir.container).removeClass('hide');
		$('.hdnEmpreendimentoID,.txtEmpreendimento', PTVEmitir.container).val("");
		$('.ddlResponsaveis', PTVEmitir.container).val("");

		$(".rbTipoNumero", PTVEmitir.container).attr("checked", false).removeAttr('disabled');;

		$('.btnModalOu', PTVEmitir.container).hide();
		$('.ddlOrigemTipo', PTVEmitir.container).ddlFirst();
		$('.ddlProdutoCultura, .ddlProdutoCultivar', PTVEmitir.container).ddlClear();
		$('.txtProdutoQuantidade', PTVEmitir.container).val('');
		$('.ddlProdutoUnidadeMedida', PTVEmitir.container).val('');

		PTVEmitir.onTipoNumeroChange();
	},

	onObterResposaveisEmpreend: function (empreendimentoID) {
		var produtos = [];
		$($('.gridProdutos tbody tr:not(.trTemplate) .hdnItemJson', PTVEmitir.container)).each(function () {
			produtos.push(JSON.parse($(this).val()));
		});

		$.ajax({
			url: PTVEmitir.settings.urls.urlObterResponsaveisEmpreend,
			data: JSON.stringify({ empreendimentoID: empreendimentoID, produtos: produtos }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textSatus, XMLHttpRequest) {
				$('.ddlResponsaveis', PTVEmitir.container).ddlLoad(response.Responsaveis);
			}
		});
	},

	chamarChangesProcDocEmp: function (parametro) {
		$.each(PTVEmitir.settings.onChangeProcDocEmp || [], function (i, item) {
			(typeof (item.Acao) == 'function') && item.Acao(parametro);
		});
	},

	onChangeOrigemTipo: function () {
		Mensagem.limpar(PTVEmitir.container);

		var labelOrigem = $('.labelOrigem', PTVEmitir.container);
		labelOrigem.text('');
		var option = $('option:selected', this);
		if (option.val() != '' && option.val() != '0') {
			labelOrigem.text(option.text());
		}

		$('.ddlProdutoCultura, .ddlProdutoUnidadeMedida, .ddlProdutoCultivar', PTVEmitir.container).ddlClear();
		$('.txtProdutoQuantidade, .txtNumeroOrigem', PTVEmitir.container).val("");
		$('.txtNumeroOrigem ', PTVEmitir.container).val('');
		$('.hdnNumeroOrigem', PTVEmitir.container).val('0');
		$('.hdnEmpreendimentoOrigemID', PTVEmitir.container).val('0');
		$('.hdnEmpreendimentoOrigemNome', PTVEmitir.container).val('');

		if (($(this).val() <= PTVEmitir.settings.idsOrigem.origemPTVOutroEstado)) {
			$('.btnVerificarDocumentoOrigem', PTVEmitir.container).removeClass('hide');
			$('.identificacaoCultura', PTVEmitir.container).addClass('hide');
			$('.culturaBuscar', PTVEmitir.container).addClass('hide');
		} else {
			$('.btnVerificarDocumentoOrigem', PTVEmitir.container).addClass('hide');
			$('.identificacaoCultura', PTVEmitir.container).removeClass('hide');
			$('.culturaBuscar', PTVEmitir.container).removeClass('hide');
		}

		if (($(this).val() > PTVEmitir.settings.idsOrigem.origemPTVOutroEstado)) {

		    $('#EmpreendimentoTexto').removeAttr('disabled');
		    $('#EmpreendimentoTexto').removeClass('disabled');

		    if ($(this).val() == "7") {

		        $('.txtNumeroOrigem', PTVEmitir.container).addClass('hide');
		        $('.labelOrigem', PTVEmitir.container).addClass('hide');

		        $('label[for="NumeroOrigem"]').hide();
		       
		    } else {
		        $('.txtNumeroOrigem', PTVEmitir.container).removeClass('hide');
		        $('.labelOrigem', PTVEmitir.container).removeClass('hide');
		        $('label[for="NumeroOrigem"]').show();
		       
		    }
		    
		    $('#ResponsavelEmpreendimento').replaceWith('<input class="text ddlResponsaveis" id="ResponsavelEmpreendimento" name="ResponsavelEmpreendimento" type="text" value="">');
		}
		else {

		    $('#EmpreendimentoTexto').attr('disabled', 'disabled');
		    $('#EmpreendimentoTexto').addClass('disabled');

		    $('.txtNumeroOrigem', PTVEmitir.container).removeClass('hide');
		    $('.labelOrigem', PTVEmitir.container).removeClass('hide');
		    $('label[for="NumeroOrigem"]').show();

		    $('#ResponsavelEmpreendimento').replaceWith('<select id="ResponsavelEmpreendimento" class="text ddlResponsaveis disabled" disabled="disabled" name="ResponsavelEmpreendimento">' +
                                        
                                        '</select>');
		}

		$('.txtNumeroOrigem', PTVEmitir.container).focus();
	},

	onChangeNumeroOrigem: function () {
		Mensagem.limpar(PTVEmitir.container);
		MasterPage.carregando(true);
		var origemTipo = $('.ddlOrigemTipo', PTVEmitir.container).val();
		var numeroOrigem = $('.txtNumeroOrigem', PTVEmitir.container).val();

		$.ajax({
			url: PTVEmitir.settings.urls.urlObterCultura,
			data: JSON.stringify({ origemTipo: origemTipo, numeroOrigem: numeroOrigem }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				$('.ddlProdutoCultura', PTVEmitir.container).ddlLoad(response.Cultura);

				if ($('.ddlProdutoCultura', PTVEmitir.container).val() != '0') {
					PTVEmitir.onChangeCultura();
				}
			}
		});
		MasterPage.carregando(false);
	},

	onChangeCultura: function () {
		Mensagem.limpar(PTVEmitir.container);
		MasterPage.carregando(true);
		var origemID = 0;
		var origemTipo = $('.ddlOrigemTipo', PTVEmitir.container).val();
		var culturaID = $('.ddlProdutoCultura', PTVEmitir.container).val();
		if (origemTipo <= PTVEmitir.settings.idsOrigem.origemPTVOutroEstado) {
			origemID = +$('.hdnNumeroOrigem', PTVEmitir.container).val();
		}

		$.ajax({
			url: PTVEmitir.settings.urls.urlObterCultivar,
			data: JSON.stringify({ origemTipo: origemTipo, origemID: origemID, culturaID: culturaID }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				$('.ddlProdutoCultivar', PTVEmitir.container).ddlLoad(response.Cultivar);
				$('.txtProdutoQuantidade', PTVEmitir.container).focus();
				PTVEmitir.onObterUnidadeMedida();
			}
		});
		MasterPage.carregando(false);
	},

	chageCultivar: function () {
		PTVEmitir.onObterUnidadeMedida();
	},

	verificarDocumentoOrigem: function () {
		Mensagem.limpar(PTVEmitir.container);
		var ddl = $('.ddlOrigemTipo', PTVEmitir.container).ddlSelecionado();
		var origemNumero = +$('.txtNumeroOrigem', PTVEmitir.container).val();

		$.ajax({
			url: PTVEmitir.settings.urls.urlVerificarDocumentoOrigem,
			data: JSON.stringify({ origemTipo: ddl.Id, origemTipoTexto: ddl.Texto, numero: origemNumero }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.identificacaoCultura', PTVEmitir.container).removeClass('hide');
					$('.btnVerificarDocumentoOrigem', PTVEmitir.container).addClass('hide');

					$('.ddlProdutoCultura', PTVEmitir.container).ddlLoad(response.Cultura);
					$('.hdnNumeroOrigem', PTVEmitir.container).val(response.OrigemID);
					$('.hdnEmpreendimentoOrigemID', PTVEmitir.container).val(response.EmpreendimentoID);
					$('.hdnEmpreendimentoOrigemNome', PTVEmitir.container).val(response.EmpreendimentoDenominador);

					if ($('.ddlProdutoCultura', PTVEmitir.container).val() != '0') {
						PTVEmitir.onChangeCultura();
					}
				}
				Mensagem.gerar(PTVEmitir.container, response.Msg);
			}
		});

	},

	//Associoar Cultura
	associarCultura: function () {
		Modal.abrir(PTVEmitir.settings.urls.urlAssociarCultura, null, function (container) {
			CulturaListar.load(container, { onAssociarCallback: PTVEmitir.callBackAssociarCultura });
			Modal.defaultButtons(container);
		});
	},

	callBackAssociarCultura: function (response) {
		$('.ddlProdutoCultura', PTVEmitir.container).append(new Option(response.Nome, response.Id, true, true));

		if ($('.ddlProdutoCultura', PTVEmitir.container).val() != '0') {
			Mensagem.limpar(PTVEmitir.container);
			MasterPage.carregando(true);
			var origemTipo = $('.ddlOrigemTipo', PTVEmitir.container).val();
			var culturaID = $('.ddlProdutoCultura', PTVEmitir.container).val();

			$.ajax({
				url: PTVEmitir.settings.urls.urlObterCultivar,
				data: JSON.stringify({ origemTipo: origemTipo, origemID: 0, culturaID: culturaID }),
				cache: false,
				async: false,
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				error: Aux.error,
				success: function (response, textStatus, XMLHttpRequest) {
					$('.ddlProdutoCultivar', PTVEmitir.container).ddlLoad(response.Cultivar);
				}
			});
			MasterPage.carregando(false);
		}
		return true;
	},

	onAdicionarIdentificacaoProduto: function () {
		Mensagem.limpar(PTVEmitir.container);
		var validacao = true;

		var container = $(this).closest('.identificacao_produto');
		var tabela = $('.gridProdutos', container);
		var NumeroOrigem = $('.ddlOrigemTipo option:selected', container).val();
		var vOrigem = $('.txtNumeroOrigem', container).val();
		if (NumeroOrigem <= PTVEmitir.settings.idsOrigem.origemPTVOutroEstado) {
			vOrigem = $('.hdnNumeroOrigem', PTVEmitir.container).val();
		}

		

		var txtUnid = $('.ddlProdutoUnidadeMedida option:selected', container).text();

		var bExibeKg = txtUnid.indexOf("KG") >= 0;

		var NumeroOrigemTexto = $('.txtNumeroOrigem', container).val();



		var item = {
			PTV: $('.txtNumero', PTVEmitir.container).val(),
			OrigemTipo: $('.ddlOrigemTipo', container).val(),
			OrigemTipoTexto: $('.ddlOrigemTipo option:selected', container).text(),
			Origem: vOrigem,
			OrigemNumero: $('.txtNumeroOrigem', container).val(),
			Cultura: $('.ddlProdutoCultura', container).val(),
			CulturaTexto: $('.ddlProdutoCultura option:selected', container).text(),
			Cultivar: $('.ddlProdutoCultivar', container).val(),
			CultivarTexto: $('.ddlProdutoCultivar option:selected', container).text(),
			UnidadeMedidaTexto: $('.ddlProdutoUnidadeMedida option:selected', container).text(),
			UnidadeMedida: $('.ddlProdutoUnidadeMedida option:selected', container).val(),
			Quantidade: Mascara.getFloatMask($('.txtProdutoQuantidade', container).val()),
			EmpreendimentoId: $('.hdnEmpreendimentoOrigemID', PTVEmitir.container).val(),
			EmpreendimentoDeclaratorio: $('.hdnEmpreendimentoOrigemNome', PTVEmitir.container).val(),
			ExibeQtdKg: bExibeKg,
			SemDoc: $('.ddlOrigemTipo option:selected', container).val() == "7"
		};

		//Valida Item já adicionado na Grid
		var _objeto = { Produtos: [] }
		$($('.gridProdutos tbody tr:not(.trTemplate) .hdnItemJson', container)).each(function () {
			_objeto.Produtos.push(JSON.parse($(this).val()));
		});

		var DataEmissao = { DataTexto: $('.txtDataEmissao', PTVEmitir.container).val() };
		var ehValido = MasterPage.validarAjax(PTVEmitir.settings.urls.urlAdicionarProdutos, { item: item, dataEmissao: DataEmissao, lista: _objeto.Produtos, ptvID: +$('.hdnEmissaoId', PTVEmitir.container).val() }, PTVEmitir.container, false).EhValido;
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
		$('.ddlOrigemTipo', container).focus();

		if ( (!item.SemDoc) && $('.txtEmpreendimento', PTVEmitir.container).text() == '') {

			$('.hdnEmpreendimentoID', PTVEmitir.container).val(item.EmpreendimentoId);
			$('.txtEmpreendimento', PTVEmitir.container).val(item.EmpreendimentoDeclaratorio);
             
			if ( item.OrigemTipo <= PTVEmitir.settings.idsOrigem.origemPTVOutroEstado )
			    PTVEmitir.onObterResposaveisEmpreend($('.hdnEmpreendimentoID', PTVEmitir.container).val());
		}
		
		Listar.atualizarEstiloTable(tabela);
		PTVEmitir.onTratamentoFitossanitário();
		PTVEmitir.onPossuiLaudoLaboratorial();
	},

	onObterUnidadeMedida: function () {
		MasterPage.carregando(true);
		var origemID = 0;
		var origemTipo = $('.ddlOrigemTipo', PTVEmitir.container).val();
		var cultivarID = +$('.ddlProdutoCultivar', PTVEmitir.container).val();
		var culturaID = +$('.ddlProdutoCultura', PTVEmitir.container).val();
		if (origemTipo <= PTVEmitir.settings.idsOrigem.origemPTVOutroEstado) {
			origemID = +$('.hdnNumeroOrigem', PTVEmitir.container).val();
		}

		$.ajax({
			url: PTVEmitir.settings.urls.urlObterUnidadeMedida,
			data: JSON.stringify({ origemTipo: origemTipo, origemID: origemID, culturaID: culturaID, cultivarID: cultivarID }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {



			    $('.ddlProdutoUnidadeMedida', PTVEmitir.container).ddlLoad(response.UnidadeMedida);

			 
			   

			    var possuiTon = false;

			  
			 
			    for (var i = 0 ; i < response.UnidadeMedida.length; i++) {

			        if (response.UnidadeMedida[i].Texto == "T")
			            possuiTon = true;
			    }
			    

			    if (possuiTon) {
			        $('.ddlProdutoUnidadeMedida').append($('<option>', {
			            value: 2,
			            text: 'KG'
			        }));

			        $('.ddlProdutoUnidadeMedida').removeAttr('disabled');
			        $('.ddlProdutoUnidadeMedida').removeClass('disabled');
			    }
			   

			}
		});

		MasterPage.carregando(false);
	},

	habilitarCampos: function (habilita) {
		if (habilitado) {
			$('.btnValidar', PTVEmitir.container).show();
			$('.btnLimpar', PTVEmitir.container).hide();
			$('.rbTipoDocumento', PTVEmitir.container).removeAttr('disabled');
		} else {
			$('.btnValidar', PTVEmitir.container).hide();
			$('.btnLimpar', PTVEmitir.container).show();
			$('.block', PTVEmitir.container).removeClass('hide');
			$('.rbTipoDocumento', PTVEmitir.container).attr('disabled', 'disabled');
		}
	},

	onExcluirIdentificacaoProduto: function () {
		Mensagem.limpar(PTVEmitir.container);
		var container = $(this).closest('.gridIdentificacaoProdutos');
		var containerProdutos = $('.identificacao_produto', PTVEmitir.container);

		var _objetoExcluido = { Produtos: [] }
		$($('.hdnItemJson', $(this).closest('tr'))).each(function () {
			_objetoExcluido.Produtos.push(JSON.parse($(this).val()));
		});

		$(this).closest('tr').toggle(
			function () {
				$(this).remove();
				PTVEmitir.onPossuiLaudoLaboratorial();
				PTVEmitir.onTratamentoFitossanitário();
		});

		var _objeto = { Produtos: [] }
		$($('.gridProdutos tbody tr:not(.trTemplate) .hdnItemJson', containerProdutos)).each(function () {
			_objeto.Produtos.push(JSON.parse($(this).val()));
		});

		var _temEmpreendimento = false;
		if (_objeto.Produtos.length > 0 && _objetoExcluido.Produtos.length > 0) {
			$.each(_objeto.Produtos, function (i, val) {
				if ((_objetoExcluido.Produtos[0].OrigemTipo != val.OrigemTipo ||
                    _objetoExcluido.Produtos[0].OrigemNumero != val.OrigemNumero ||
                    _objetoExcluido.Produtos[0].Cultura != val.Cultura ||
                    _objetoExcluido.Produtos[0].Cultivar != val.Cultivar ||
                    _objetoExcluido.Produtos[0].UnidadeMedida != val.UnidadeMedida ||
                    _objetoExcluido.Produtos[0].Quantidade != val.Quantidade ||
                    _objetoExcluido.Produtos[0].ExibeQtdKg != val.ExibeQtdKg) &&
                    val.EmpreendimentoId > 0) {
					_temEmpreendimento = true;
				}
			});
		}

		if (!_temEmpreendimento) {
			$('.hdnEmpreendimentoID', PTVEmitir.container).val('0');
			$('.txtEmpreendimento', PTVEmitir.container).val('');
			$('.ddlResponsaveis', PTVEmitir.container).ddlClear();
		}

		Listar.atualizarEstiloTable(container);
	},

	onChangePartidaLacrada: function () {
		if ($(this).val() == 1) {
			$('.partida_lacrada', PTVEmitir.container).removeClass('hide');
			$('.txtNumeroLacre', PTVEmitir.container).focus();
		}
		else {
			$('.partida_lacrada', PTVEmitir.container).addClass('hide');
			$('.txtNumeroLacre, .txtNumeroPorao, .txtNumeroContainer', PTVEmitir.container).val('');
		}
	},

	onTipoPessoaChange: function () {
		Mensagem.limpar(PTVEmitir.container);
		var container = $(this).closest('.destinatario');
		$('.txtDocumentoCpfCnpj', container).val('');
		$('.novoDestinatario', container).addClass('hide');

		if ($('.rbTipoPessoaFisica').is(':checked')) {
			$('.lblCPFCNPJ', container).html('CPF *');
			$('.txtDocumentoCpfCnpj', container).removeClass('maskCnpj').unmask().addClass('maskCpf');
		} else {
			$('.txtDocumentoCpfCnpj', container).removeClass('maskCpf').unmask().addClass('maskCnpj');
			$('.lblCPFCNPJ', container).html('CNPJ *');
		}

		Mascara.load(container);
		$('.txtDocumentoCpfCnpj', container).focus();
	},

	onValidarDocumento: function () {
		Mensagem.limpar(PTVEmitir.container);
		var container = $(this).closest('.destinatario');
		var pessoaTipo = $('.rbTipoDocumento:checked', container).val() ? $('.rbTipoDocumento:checked', container).val() : 0;
		var CpfCnpj = $('.txtDocumentoCpfCnpj', container).val();

		MasterPage.carregando(true);
		$.ajax({
			url: PTVEmitir.settings.urls.urlValidarDocumento,
			data: JSON.stringify({ pessoaTipo: pessoaTipo, CpfCnpj: CpfCnpj }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					PTVEmitir.callBackAssociarDestinatario(response.Destinatario);
				} else if (response.NovoDestinatario) {
					$('.novoDestinatario', container).removeClass('hide');
				}
				Mensagem.gerar(PTVEmitir.container, response.Msg);
			}
		});
		MasterPage.carregando(false);
	},

	obterDestinatario: function (destinatarioID) {
		MasterPage.carregando(true);
		//var container = $(this).closest('.destinatario');
		$.ajax({
			url: PTVEmitir.settings.urls.urlObterDestinatario,
			data: JSON.stringify({ destinatarioID: destinatarioID }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.hdnDestinatarioID', PTVEmitir.container).val(response.Destinatario.ID);
					$('.txtNomeDestinatario', PTVEmitir.container).val(response.Destinatario.NomeRazaoSocial);
					$('.txtEndereco', PTVEmitir.container).val(response.Destinatario.Endereco);
					$('.hdnUfID', PTVEmitir.container).val(response.Destinatario.EstadoID);
					$('.txtUF', PTVEmitir.container).val(response.Destinatario.EstadoTexto);
					$('.hdnMunicipioID', PTVEmitir.container).val(response.Destinatario.MunicipioID);
					$('.txtMunicipio', PTVEmitir.container).val(response.Destinatario.MunicipioTexto);
					$('.txtItinerario', PTVEmitir.container).val(response.Destinatario.Itinerario);

					$('.btnVerificarDestinatario, .novoDestinatario', PTVEmitir.container).addClass('hide');
					$('.btnLimparDestinatario, .destinatarioDados', PTVEmitir.container).removeClass('hide');
					$('.rbTipoDocumento, .txtDocumentoCpfCnpj, .txtNomeDestinatario, .txtEndereco, .txtUF, .txtMunicipio', PTVEmitir.container).addClass('disabled').attr('disabled', 'disabled');
				}

				Mensagem.gerar(PTVEmitir.container, response.Msg);
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
		$('.btnLimparDestinatario, .destinatarioDados, .novoDestinatario', container).addClass('hide');
		$('.rbTipoDocumento, .txtDocumentoCpfCnpj, .txtNomeDestinatario, .txtEndereco, .txtUF, .txtMunicipio', container).removeClass('disabled').removeAttr('disabled');
		$('.txtDocumentoCpfCnpj', container).focus();
	},

	//Associar Destinatario
	onAssociarDestinatario: function () {
		Modal.abrir(PTVEmitir.settings.urls.urlAssociarDestinatario, null, function (container) {
			DestinatarioPTV.load(container, {
				associarFuncao: PTVEmitir.callBackAssociarDestinatario,
				destinatarioCPFCNPJ: $('.txtDocumentoCpfCnpj', PTVEmitir.container).val()
			});
			Modal.defaultButtons(container, DestinatarioPTV.salvar, "Salvar");
		}, Modal.tamanhoModalMedia);
	},

	callBackAssociarDestinatario: function (destinatario) {
		$('.hdnDestinatarioID', PTVEmitir.container).val(destinatario.ID);
		$('.txtNomeDestinatario', PTVEmitir.container).val(destinatario.NomeRazaoSocial);
		$('.txtEndereco', PTVEmitir.container).val(destinatario.Endereco);
		$('.txtUF', PTVEmitir.container).val(destinatario.EstadoSigla);
		$('.txtMunicipio', PTVEmitir.container).val(destinatario.MunicipioTexto);
		$('.txtItinerario', PTVEmitir.container).val(destinatario.Itinerario);

		$('.btnVerificarDestinatario, .novoDestinatario', PTVEmitir.container).addClass('hide');
		$('.btnLimparDestinatario, .destinatarioDados', PTVEmitir.container).removeClass('hide');
		$('.rbTipoDocumento, .txtDocumentoCpfCnpj, .txtNomeDestinatario, .txtEndereco, .txtUF, .txtMunicipio', PTVEmitir.container).addClass('disabled').attr('disabled', 'disabled');

		PTVEmitir.onChangeRotaTransitoDefinida();
	},

	onPossuiLaudoLaboratorial: function () {
		var container = $('.identificacao_produto', PTVEmitir.container);
		var _objeto = { Produtos: [] }
		$($('.gridProdutos tbody tr:not(.trTemplate) .hdnItemJson', container)).each(function () {
			_objeto.Produtos.push(JSON.parse($(this).val()));
		});

		MasterPage.carregando(true);
		$.ajax({
			url: PTVEmitir.settings.urls.urlObterLaboratorio,
			data: JSON.stringify({ lista: _objeto.Produtos }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				var tabela = $('.gridLaudoLaboratorial', PTVEmitir.container);
				tabela.find('tbody tr:not(.trTemplate)').empty();

				if (response.Lista.length > 0) {
					$.each(response.Lista, function (i, item) {
						var linha = $('.trTemplate', tabela).clone();
						$(linha).removeClass('hide trTemplate');

						$('.loboratorioNome', linha).html(item.Nome).attr('title', item.Nome);
						$('.laboratorioNumero', linha).html(item.LaudoResultadoAnalise).attr('title', item.LaudoResultadoAnalise);
						$('.laboratorioUF', linha).html(item.EstadoTexto).attr('title', item.EstadoTexto);
						$('.laboratorioMunicipio', linha).html(item.MunicipioTexto).attr('title', item.MunicipioTexto);
						$('tbody', tabela).append(linha);
					});

					$('.laudo', PTVEmitir.container).removeClass('hide');
					$('.rbPossuiLaudoSim', PTVEmitir.container).attr('checked', 'checked');
					$('.rbPossuiLaudoNao', PTVEmitir.container).removeAttr('checked');
				} else {
					$('.laudo', PTVEmitir.container).addClass('hide');
					$('.rbPossuiLaudoSim', PTVEmitir.container).removeAttr('checked');
					$('.rbPossuiLaudoNao', PTVEmitir.container).attr('checked', 'checked');
				}
			}
		});
		MasterPage.carregando(false);
	},

	onTratamentoFitossanitário: function () {
		MasterPage.carregando(true);
		var container = $('.tratamentoFitossanitario', PTVEmitir.container);

		var _objeto = { TratamentoFitossa: [] }
		$($('.gridProdutos tbody tr:not(.trTemplate) .hdnItemJson', PTVEmitir.container)).each(function () {
			_objeto.TratamentoFitossa.push(JSON.parse($(this).val()));
		});

		$.ajax({
			url: PTVEmitir.settings.urls.urlObterTratamentoFisso,
			data: JSON.stringify({ lista: _objeto.TratamentoFitossa }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {

				var tabela = $('.gridTratamentoFitossa', container);
				$($('tbody tr:not(.trTemplate)', tabela)).remove();
				var linha = null;

				//adicionar na grid	
				$.each(response.TratamentoFitossa, function (i, val) {
					linha = $('.trTemplate', tabela).clone();
					$(linha).removeClass('hide trTemplate');
					$('.lblProdutoComercial', linha).html(val.ProdutoComercial).attr('title', val.ProdutoComercial);
					$('.lblIngrediente_ativo', linha).html(val.IngredienteAtivo).attr('title', val.IngredienteAtivo);
					$('.lblDose', linha).html(val.Dose).attr('title', val.Dose);
					$('.lblPraga_produto', linha).html(val.PragaProduto).attr('title', val.PragaProduto);
					$('.lblModo_aplicacao', linha).html(val.ModoAplicacao).attr('title', val.ModoAplicacao);
					$('.hdnItemJsonFitossanitario', linha).val(JSON.stringify(response.TratamentoFitossa));
					$('tbody', tabela).append(linha);
				});
				Listar.atualizarEstiloTable(tabela);
			}
		});
		MasterPage.carregando(false);
	},

	onChangeRotaTransitoDefinida: function () {
		if ($('.rdbRotaTransitoDefinidaSim', PTVEmitir.container).is(':checked')) {
			$('.rota', PTVEmitir.container).removeClass('hide');
			var pessoaTipo = $('.rbTipoDocumento:checked', PTVEmitir.container).val();
			$.ajax({
				url: PTVEmitir.settings.urls.urlObterItinerario,
				data: JSON.stringify({ destinatarioId: $('.hdnDestinatarioID', PTVEmitir.container).val() }),
				cache: false,
				async: false,
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				error: Aux.error,
				success: function (response, textStatus, XMLHttpRequest) {
					$('.txtItinerario', PTVEmitir.container).removeClass('disabled').removeAttr('disabled');

					if (response.Destinatario != null) {
						$('.txtItinerario', PTVEmitir.container).val(response.Destinatario.Itinerario);
						$('.txtItinerario', PTVEmitir.container).focus();
					}
				}
			});
		}
		else {
			$('.txtItinerario', PTVEmitir.container).val('');
			$('.rota', PTVEmitir.container).addClass('hide');
		}
	},

	onChangeNumeroNotaFiscal: function () {
		if ($(this).val() == 1) {
			$('.nota_fical', PTVEmitir.container).removeClass('hide');
			$('.txtNotaFiscalNumero', PTVEmitir.container).removeClass('disabled').removeAttr('disabled');
			$('.txtNotaFiscalNumero', PTVEmitir.container).focus();
		} else {
			$('.txtNotaFiscalNumero', PTVEmitir.container).val('');
			$('.nota_fical', PTVEmitir.container).addClass('hide');
		}
	},

	onSalvar: function () {
		Mensagem.limpar(PTVEmitir.container);
		MasterPage.carregando(true);

		var objeto = PTVEmitir.obter();
		$.ajax({
			url: PTVEmitir.settings.urls.urlSalvar,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.Url);
				}

				if (response.Erros && response.Erros.length > 0) {
					Mensagem.gerar(PTVEmitir.container, response.Erros);
				}
			}
		});
		MasterPage.carregando(false);
	},

	obter: function () {
	    var pessoaTipo = $('.rbTipoDocumento:checked', PTVEmitir.container).val();

	 
	    var dados = '';

	    if ($('.rdbPessaoTipo:checked', PTVEmitir.container).val() == '1') {
	        dados = $('.txtCPFDUA', PTVEmitir.container).val();
	    } else {
	        dados = $('.txtCNPJDUA', PTVEmitir.container).val();
	    }





		var objeto = {
			Id: +$('.hdnEmissaoId', PTVEmitir.container).val(),
			NumeroTipo: +$('.rbTipoNumero:checked', PTVEmitir.container).val(),
			Numero: $('.txtNumero', PTVEmitir.container).val(),
			NumeroDua: $('.txtNumeroDua', PTVEmitir.container).val(),
			CPFCNPJDUA: dados,
			DataEmissao: { DataTexto: $('.txtDataEmissao', PTVEmitir.container).val() },
			Situacao: $('.ddlSituacoes', PTVEmitir.container).val(),
			Empreendimento: $('.hdnEmpreendimentoID', PTVEmitir.container).val(),
			EmpreendimentoTexto: $('.txtEmpreendimento', PTVEmitir.container).val(),
			ResponsavelEmpreendimento: $('.ddlResponsaveis', PTVEmitir.container).val(),
			PartidaLacradaOrigem: $('.rbPartidaLacradaOrigem:checked', PTVEmitir.container).val(),
			LacreNumero: $('.txtNumeroLacre', PTVEmitir.container).val(),
			PoraoNumero: $('.txtNumeroPorao', PTVEmitir.container).val(),
			ContainerNumero: $('.txtNumeroContainer', PTVEmitir.container).val(),
			Tipo: $('.rbTipoDocumento:checked', PTVEmitir.container).val(),
			DestinatarioID: $('.hdnDestinatarioID', PTVEmitir.container).val(),
			PossuiLaudoLaboratorial: $('.rbPossuiLaudo:checked', PTVEmitir.container).val(),
			TransporteTipo: $('.ddlTipoTransporte', PTVEmitir.container).val(),
			VeiculoIdentificacaoNumero: $('.txtIdentificacaoVeiculo', PTVEmitir.container).val(),
			RotaTransitoDefinida: $('.rdbRotaTransitoDefinida:checked', PTVEmitir.container).val(),
			Itinerario: $('.txtItinerario', PTVEmitir.container).val(),
			NotaFiscalApresentacao: $('.rdbApresentacaoNotaFiscal:checked', PTVEmitir.container).val(),
			NotaFiscalNumero: $('.txtNotaFiscalNumero', PTVEmitir.container).val(),
			ValidoAte: { DataTexto: $('.txtDataValidade', PTVEmitir.container).val() },
			ResponsavelTecnicoId: $('.hdnResponsavelTecnicoId', PTVEmitir.container).val(),
			LocalEmissaoId: $('.ddlLocalEmissao', PTVEmitir.container).val(),
			EmpreendimentoSemDoc: $('.txtEmpreendimento', PTVEmitir.container).val(),
			ResponsavelSemDoc: $('.ddlResponsaveis', PTVEmitir.container).val(),
			Produtos: []
		}

		var retorno = [];
		$('.gridProdutos tbody tr:not(.trTemplate)', PTVEmitir.container).each(function () {
			retorno.push(JSON.parse($('.hdnItemJson', this).val()));
		});


		for (var i = 0; i < retorno.length; i++)
		    if (retorno[i].ExibeQtdKg) {
		        retorno[i].Quantidade = retorno[i].Quantidade / 1000;
		    }

		objeto.Produtos = retorno;

		

		return objeto;
	}
}
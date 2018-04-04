/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />

Cobranca = {
	settings: {
		urls: {
			confirm: '',
			salvar: '',
			visualizar: '',
			carregar: '',
			lista: '',
			notificacao: '',
			novoParcelamento: '',
			recalcular: '',

			editarAutuadoPessoa: '',
			fiscalizacaoPessoaModal: '',
			associarAutuado: '',
			associarFiscalizacao: '',
			obterFiscalizacao: '',
			visualizarFiscalizacao: ''
		},
		containerModal: null,
		mensagens: null
	},

	container: null,
	pessoaModalInte: null,

	load: function (container, options) {
		if (options) { $.extend(Cobranca.settings, options); }
		Cobranca.container = container;

		container.delegate('.btnSalvar', 'click', Cobranca.salvar);
		container.delegate('.btnEditar', 'click', Cobranca.editar);
		container.delegate('.btnAddSubparcela', 'click', Cobranca.addSubparcela);
		container.delegate('.btnRecalcular', 'click', Cobranca.recalcular);
		container.delegate('.btnNovoParcelamento', 'click', Cobranca.novoParcelamento);
		container.delegate('.btnParcelamentoAnterior', 'click', Cobranca.parcelamentoAnterior);
		container.delegate('.btnParcelamentoPosterior', 'click', Cobranca.parcelamentoPosterior);
		container.delegate('.btnAssociarAutuado', 'click', Cobranca.abrirModalFiscalizacaoPessoa);
		container.delegate('.btnVerificarPessoa', 'click', Cobranca.abrirModalPessoa);
		container.delegate('.btnEditarAutuado', 'click', Cobranca.onClickEditarVisualizar);
		container.delegate('.ddlParcelas', 'change', Cobranca.alterarParcelas);
		container.delegate('.linkCancelar', 'click', Cobranca.cancelar);
		container.delegate('.txtData1Vencimento', 'blur', Cobranca.alterarParcelas);
		
		$('.txtProcessoNumero', container).focus();
	},

	abrirModalFiscalizacaoPessoa: function () {
		var container = Cobranca.container;
		Modal.abrir(Cobranca.settings.urls.fiscalizacaoPessoaModal, Cobranca.obter());
	},

	obter: function () {
		var container = Cobranca.container;

		var obj = {
			Id: $('.hdnCobrancaId', container).val(),
			ProcessoNumero: $('.txtProcessoNumero', container).val(),
			NumeroAutuacao: $('.txtNumeroAutuacao', container).val(),
			NumeroFiscalizacao: $('.txtFiscalizacao', container).val(),
			NumeroIUF: $('.txtNumeroIUF', container).val(),
			SerieId: $('.hdnSerieId', container).val(),
			SerieTexto: $('.txtSerie', container).val(),
			DataEmissaoIUF: { DataTexto: $('.txtDataEmissaoIUF', container).val() },
			DataIUF: { DataTexto: $('.txtDataIUF', container).val() },
			DataJIAPI: { DataTexto: $('.txtDataJIAPI', container).val() },
			DataCORE: { DataTexto: $('.txtDataCORE', container).val() },
			CodigoReceitaId: $('.ddlCodigoReceita :selected', container).val(),
			AutuadoPessoa: { NomeRazaoSocial: $('.txtAutuadoNome', container).val(), CPFCNPJ: $('.txtAutuadoCpfCnpj', container).val()  },
			AutuadoPessoaId: $('.hdnAutuadoPessoaId', container).val(),
			UltimoParcelamento: JSON.parse($('.hdnParcelamento', container).val())
		}
		obj.UltimoParcelamento.ValorMulta = $('.txtValorMulta', container).val();
		obj.UltimoParcelamento.ValorMultaAtualizado = $('.txtValorMultaAtualizado', container).val();
		obj.UltimoParcelamento.QuantidadeParcelas = $('.ddlParcelas :selected', container).val();
		obj.UltimoParcelamento.Data1Vencimento = { DataTexto: $('.txtData1Vencimento', container).val() };
		obj.UltimoParcelamento.DataEmissao = { DataTexto: $('.txtDataEmissao', container).val() };
		obj.UltimoParcelamento.DUAS = Cobranca.obterListaParcelamento();
		
		return obj;
	},

	obterListaParcelamento: function () {
		var lista = [];

		$($('.tabParcelas tbody tr:not(.trTemplateRow) .hdnItemJSon', Cobranca.container)).each(function () {
			var item = JSON.parse($(this).val());
			var itensHtml = Array.from(this.parentElement.parentElement.children).filter(x => x.innerHTML.indexOf('input') > -1)
			item.NumeroDUA = itensHtml[0].children[0].value;
			if (itensHtml.length == 5) {
				item.ValorPago = parseFloat(itensHtml[1].children[0].value.replaceAll('.', '').replaceAll(',', '.'));
				item.DataPagamento = { DataTexto: itensHtml[2].children[0].value };
				item.InformacoesComplementares = itensHtml[3].children[0].value;
			} else {
				item.DataVencimento = { DataTexto: itensHtml[1].children[0].value };
				item.ValorPago = parseFloat(itensHtml[2].children[0].value.replaceAll('.', '').replaceAll(',', '.'));
				item.DataPagamento = { DataTexto: itensHtml[3].children[0].value };
				item.InformacoesComplementares = itensHtml[4].children[0].value;

				var parcela = Array.from(this.parentElement.parentElement.children).filter(x => x.innerHTML.indexOf('parcela') > -1);
				item.Parcela = parcela[0].children[0].innerText;
				var vrteHtml = Array.from(this.parentElement.parentElement.children).filter(x => x.innerHTML.indexOf('vrte') > -1);
				item.VRTE = parseFloat(vrteHtml[0].children[0].innerText.replaceAll('.', '').replaceAll(',', '.'));
				var situacao = Array.from(this.parentElement.parentElement.children).filter(x => x.innerHTML.indexOf('situacao') > -1);
				item.Situacao = situacao[0].children[0].innerText;
				var valorDua = Array.from(this.parentElement.parentElement.children).filter(x => x.innerHTML.indexOf('valorDUA') > -1);
				item.ValorDUA = parseFloat(valorDua[0].children[0].innerText.replaceAll('.', '').replaceAll(',', '.'));
				item.Tid = "";
				if (lista.filter(x => x.Id == item.Id).length > 0) {
					item.ParcelaPaiId = item.Id;
					item.Id = 0;
				}
			}
			lista.push(item);
		});

		return lista;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: Cobranca.settings.urls.salvar,
			data: JSON.stringify(Cobranca.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Cobranca.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Cobranca.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	alterarParcelas: function () {
		Modal.confirma({
			btCancelLabel: 'Não',
			url: Cobranca.settings.urls.confirm,
			urlData: { tipo: 0 },
			tamanhoModal: Modal.tamanhoModalMedia,
			btnOkCallback: function (modalContent) {
				Modal.fechar(modalContent);

				MasterPage.carregando(true);
				$.ajax({
					url: Cobranca.settings.urls.recalcular,
					data: JSON.stringify(Cobranca.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, Cobranca.container);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(Cobranca.container, response.Msg);
						}

						if (response.Html && response.Html.length > 0) {
							$('.cobrancaPartial', Cobranca.container).html(response.Html);
						}
						MasterPage.load();
						MasterPage.redimensionar();
						Mascara.load(Cobranca.container);
					}
				});
				MasterPage.carregando(false);
			}
		});
	},

	editar: function () {
		MasterPage.carregando(true);

		var container = Cobranca.container;
		var fiscalizacaoId = $('.txtFiscalizacao', container).val();
		MasterPage.redireciona(Cobranca.settings.urls.carregar + "/" + fiscalizacaoId);

		MasterPage.carregando(false);
	},

	cancelar: function () {
		MasterPage.carregando(true);

		if ($('.hdnOrigem', Cobranca.container).val() == 'notificacao') {
			var fiscalizacaoId = $('.txtFiscalizacao', Cobranca.container).val();
			MasterPage.redireciona(Cobranca.settings.urls.notificacao + "/" + fiscalizacaoId);
		}
		else 
			MasterPage.redireciona(Cobranca.settings.urls.lista);

		MasterPage.carregando(false);
	},

	recalcular: function () {
		Modal.confirma({
			btCancelLabel: 'Não',
			url: Cobranca.settings.urls.confirm,
			urlData: { tipo: 1 },
			tamanhoModal: Modal.tamanhoModalMedia,
			btnOkCallback: function (modalContent) {
				Modal.fechar(modalContent);

				MasterPage.carregando(true);
				$.ajax({
					url: Cobranca.settings.urls.recalcular,
					data: JSON.stringify(Cobranca.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, Cobranca.container);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(Cobranca.container, response.Msg);
						}

						if (response.Html && response.Html.length > 0) {
							$('.cobrancaPartial', Cobranca.container).html(response.Html);
						}
						MasterPage.load();
						MasterPage.redimensionar();
						Mascara.load(Cobranca.container);
					}
				});
				MasterPage.carregando(false);
			}
		});
	},

	addSubparcela: function () {
		var newRow = $(this.parentElement.parentElement).clone();
		if (newRow[0].children[7].children[0].innerText == "Pago Parcial") {
			$(this.parentElement.parentElement)[0].children[7].children[0].innerText = "Pago";
			var acao = Array.from($(this.parentElement.parentElement)[0].children).filter(x => x.className == 'tdAcoes')[0].children[2];
			acao.disabled = true;
			$(acao).addClass('ui-button-disabled ui-state-disabled');

			var input = "<input class='text dataVencimento maskData' title='' style='width=100%' />";
			newRow[0].innerHTML = newRow[0].innerHTML.replaceAll(newRow[0].children[2].innerHTML, input);
			newRow[0].children[0].children[0].innerText = newRow[0].children[0].children[0].innerText + ' - Subparcela';
			newRow[0].children[1].children[0].value = "";

			var valorDua = parseFloat(newRow[0].children[3].children[0].innerText.replaceAll('.', '').replaceAll(',', '.'));
			var valorPago = parseFloat(newRow[0].children[4].children[0].value.replaceAll('.', '').replaceAll(',', '.'));
			var vrte = parseFloat(newRow[0].children[5].children[0].innerText.replaceAll('.', '').replaceAll(',', '.'));
			vrte = valorDua / vrte;
			var valorRestante = valorDua - valorPago;

			newRow[0].children[3].children[0].innerText = "";
			newRow[0].children[4].children[0].value = "";
			newRow[0].children[5].children[0].innerText = (valorRestante / vrte).formatMoney(4, ',', '.');;
			newRow[0].children[6].children[0].value = "";
			newRow[0].children[7].children[0].innerText = "Em Aberto";
			$('.tabParcelas tbody').append(newRow);
			Mascara.load(Cobranca.container);
		} else {
			ExibirMensagemValidacao('É permitido adicionar subparcela apenas para uma parcela com situação \"Pago Parcial\".');
		}
	},

	novoParcelamento: function () {
		Modal.confirma({
			btCancelLabel: 'Não',
			url: Cobranca.settings.urls.confirm,
			urlData: { tipo: 2 },
			tamanhoModal: Modal.tamanhoModalMedia,
			btnOkCallback: function (modalContent) {
				Modal.fechar(modalContent);

				MasterPage.carregando(true);
				$.ajax({
					url: Cobranca.settings.urls.novoParcelamento,
					data: JSON.stringify(Cobranca.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, Cobranca.container);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.EhValido) {
							MasterPage.redireciona(response.UrlRedirecionar);
						}
						if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(Cobranca.container, response.Msg);
						}
					}
				});
				MasterPage.carregando(false);
			}
		});
	},

	parcelamentoAnterior: function () {
		MasterPage.carregando(true);
		var container = Cobranca.container;
		var fiscalizacaoId = $('.txtFiscalizacao', container).val();
		MasterPage.redireciona(Cobranca.settings.urls.visualizar + "?index=" + ($('.hdnIndexParcelamento', container).val() - 1));
		MasterPage.carregando(false);
	},

	parcelamentoPosterior: function () {
		MasterPage.carregando(true);
		var container = Cobranca.container;
		var fiscalizacaoId = $('.txtFiscalizacao', container).val();
		MasterPage.redireciona(Cobranca.settings.urls.visualizar + "?index=" + ($('.hdnIndexParcelamento', container).val() + 1));
		MasterPage.carregando(false);
	},

	abrirModalPessoa: function () {
		Cobranca.pessoaModalInte = new PessoaAssociar();

		//Quando tipoCadastro = 1, o modal Pessoa exibirá apenas a busca por pessoa física.
		//Se o objeto não for passado para o modal (null), ele exibe a busca normal (CPF/CNPJ).
		var dataPessoa = {
			cpfCnpj: null,
			tipoPessoa: null,
			tipoCadastro: '1'
		};

		Modal.abrir(Cobranca.settings.urls.associarAutuado, dataPessoa, function (container) {
			Cobranca.pessoaModalInte.load(container, {
				tituloCriar: 'Cadastrar Autuado',
				tituloEditar: 'Editar Autuado',
				tituloVisualizar: 'Visualizar Autuado',
				onAssociarCallback: Cobranca.callBackEditarAutuado,
				isFiscalizacao: true
			});
		});
	},

	abrirModalFiscalizacao: function () {
		var container = Cobranca.container;
		var params = { id: parseInt($('.txtFiscalizacao', container).val()), processoId: 0 };

		Modal.abrir(Cobranca.settings.urls.associarFiscalizacao, null, function (container) {
			FiscalizacaoListar.load(container, { associarFuncao: Cobranca.callBackAssociarFiscalizacao });
			Modal.defaultButtons(container);
		});
	},

	callBackAssociarFiscalizacao: function (Fiscalizacao) {
		if ($('.txtFiscalizacao', Cobranca.container).val() == Fiscalizacao.Id) {
			return true;
		}
		MasterPage.carregando(true);

		var params = { fiscalizacaoId: Fiscalizacao.Id };
		var retorno = Cobranca.obterAjax(Cobranca.settings.urls.obterFiscalizacao, params, $('.divFiscalizacao', Cobranca.container));
		
		if (!retorno.EhValido) {
			MasterPage.carregando(false);
			return retorno.Msg;
		}

		MasterPage.redireciona(Cobranca.settings.urls.carregar + "/" + Fiscalizacao.Id);
		MasterPage.carregando(false);

		return true;
	},

	callBackEditarAutuado: function (Pessoa) {
		$('.spanVisualizarAutuado', Cobranca.container).removeClass('hide');
		$('.hdnAutuadoPessoaId', Cobranca.container).val(Pessoa.Id);
		$('.txtAutuadoNome', Cobranca.container).val(Pessoa.NomeRazaoSocial);
		$('.txtAutuadoCpfCnpj', Cobranca.container).val(Pessoa.CPFCNPJ);
		return true;
	},

	onClickEditarVisualizar: function () {
		var id = $('.hdnAutuadoPessoaId', Cobranca.container).val();
		var pessoaModalInte = new PessoaAssociar();

		var params = { fiscalizacaoId: $('.txtFiscalizacao', Cobranca.container).val() };
		var retorno = Cobranca.obterAjax(Cobranca.settings.urls.obterFiscalizacao, params, $('.divFiscalizacao', Cobranca.container));

		if (retorno.Fiscalizacao.Id > 0) {
			var params = { id: $('.txtFiscalizacao', Cobranca.container).val() };

			Modal.abrir(Cobranca.settings.urls.visualizarFiscalizacao, params, function (container) {
				Modal.defaultButtons(container);
			}, Modal.tamanhoModalGrande);
		}
		else {
			var url = Cobranca.settings.urls.editarAutuadoPessoa + "/?id=" + id;

			Modal.abrir(url, null, function (container) {
				pessoaModalInte.load(container, {
					tituloCriar: 'Cadastrar Autuado',
					tituloEditar: 'Editar Autuado',
					tituloVisualizar: 'Visualizar Autuado',
					onAssociarCallback: Cobranca.callBackEditarAutuado
				});
			});
		}
	},

	obterAjax: function (url, params, container) {
		MasterPage.carregando(true);
		var retorno = null;

		$.ajax({
			url: url, data: params, cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Cobranca.settings.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				Mensagem.limpar(MasterPage.getContent(Cobranca.settings.container));

				if (response.EhValido || response.SetarHtml) {
					$(container).empty();
					$(container).append(response.Html);
					Mascara.load(container);
					MasterPage.botoes(container);
					MasterPage.redimensionar();
				}

				retorno = $(response).removeData('Html');
			}
		});
		MasterPage.carregando(false);
		return retorno[0];
	},
}

String.prototype.trim = function () {
	return this.replace(/^\W+|\W+$/g, "");
}

Number.prototype.formatMoney = function (c, d, t) {
	var n = this,
		c = isNaN(c = Math.abs(c)) ? 2 : c,
		d = d == undefined ? "." : d,
		t = t == undefined ? "," : t,
		s = n < 0 ? "-" : "",
		i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
		j = (j = i.length) > 3 ? j % 3 : 0;
	return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

function ExibirMensagemValidacao(erro) {
	var mensagem = '\
			<div class=\"mensagemSistema alerta ui-draggable\" style=\"position: relative;\">\
				<div class=\"textoMensagem \">\
					<a class=\"fecharMensagem\" title=\"Fechar Mensagem\">Fechar Mensagem</a>\
					<p> Mensagem do Sistema</p>\
					<ul>\
						<li>' + erro + '</li>\
					</ul>\
				</div>\
				<div class=\"redirecinamento block containerAcoes hide\">\
					<h5> O que deseja fazer agora ?</h5>\
					<p class=\"hide\">#DESCRICAO</p>\
					<div class=\"coluna100 margem0 divAcoesContainer\">\
						<p class=\"floatLeft margem0 append1\"><button title=\"[title]\" class=\"btnTemplateAcao hide ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only\" role=\"button\" aria-disabled=\"false\"><span class=\"ui-button-text\">[ACAO]</span></button></p>\
						<div class=\"containerBotoes\"></div>\
					</div>\
				</div>\
			</div>';
	$('.mensagemSistemaHolder')[0].innerHTML = mensagem;
}
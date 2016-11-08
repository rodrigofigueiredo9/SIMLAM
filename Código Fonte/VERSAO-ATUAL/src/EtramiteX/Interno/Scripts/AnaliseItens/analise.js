/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />
/// <reference path="analisarItemAnalise.js" />

Analise = {
	settings: {
		urls: {
			obterRequerimento: '',
			obterAnalisePartial: '',
			criarItem: '',
			analiseItem: '',
			visualizarHistorico: '',
			atividadesSolicitadas: '',
			pdfRequerimento: '',
			pdfAnalise: '',
			pdfRoteiro: '',
			modalAtualizarRoteiro: '',
			salvar: '',
			validarPossuiTitPendencia: '',
			criarTitPendencia: '',
			visualizarChecagem: '',
			versaoRoteiros: '',
			analisar: '',
			projetoGeografico: '',
			caracterizacoes: [],
			importarProjetoDigital: ''
		},
		isPendente: false,
	},

	mensagens: null,
	container: null,
	Atualizar: null,
	containerLinha: null,
	isAtualizar: false,
	continuar: false,
	tipos: null,
	situacoes: null,

	load: function (container, options) {

		if (options) { $.extend(Analise.settings, options); }

		Analise.container = MasterPage.getContent(container);
		Analise.container.delegate('.btnVerificar', 'click', Analise.verificarProtocolo);
		Analise.container.delegate('.btnLimpar', 'click', Analise.limpar);
		Analise.container.delegate('.btnAddItem', 'click', Analise.adicionarItem);
		Analise.container.delegate('.btnAnalisarItem', 'click', Analise.analisarItem);
		Analise.container.delegate('.btnHistoricoItem', 'click', Analise.visualizarHistorico);
		Analise.container.delegate('.btnEditarItem', 'click', Analise.editarItem);
		Analise.container.delegate('.btnExcluirItem', 'click', Analise.excluirItem);
		Analise.container.delegate('.checkRequerimento', 'click', Analise.selecionarRequerimento);
		Analise.container.delegate('.btnSalvarAnalise', 'click', Analise.salvar);
		Analise.container.delegate('.btnGerarPdfAnalise', 'click', Analise.gerarPDFAnalise);
		Analise.container.delegate('.btnPDFRequerimento', 'click', Analise.gerarPDFRequerimento);
		Analise.container.delegate('.btnEmitirTitPen', 'click', Analise.gerarTituloPendencia);
		Analise.container.delegate('.btnVisualizarAtividades', 'click', Analise.visualizarAtividade);
		Analise.container.delegate('.btnVisualizarPDF', 'click', Analise.gerarPDF);
		Analise.container.delegate('.btnVisChecagem', 'click', Analise.visualizarChecagem);

		Analise.container.delegate('.btnProjetoGeografico', 'click', Analise.visualizarProjetoGeografico);
		Analise.container.delegate('.btnVisualizarCaracterizacao', 'click', Analise.visualizarCaracterizacao);
		Analise.container.delegate('.btnImportarProjetoDigital', 'click', Analise.importarProjetoDigital);

		Analise.container.delegate('.txtNumeroProtocolo', 'keyup', function (e) {
			if (e.keyCode == 13) $('.btnVerificar', Analise.container).click();
		});
		$('.txtNumeroProtocolo', Analise.container).focus();

		if (!Analise.settings.isPendente) {
			Analise.onValidarChecagem(Analise.container);
		}
	},

	obterItens: function () {
		return $('tr', Analise.container.find('.tbItens tbody'));
	},

	onValidarChecagem: function (container) {

		var objeto = {
			ChecagemId: $('.hdnChecagemNumero', $('.AnalisandoRequerimento', container)).val(),
			IsProcesso: $('.hdnProtocoloTipo', container).val() == '1',
			NumeroRequerimento: $('.hdnRequerimentoSelecionadoId', Analise.container).val()
		};

		$.ajax({
			url: Analise.settings.urls.versaoRoteiros,
			type: "POST",
			data: objeto,
			cache: false,
			async: true,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					ModalAtualizarRoteiro.mensagem = response.Msg;
					Analise.abrirModalMensagem(objeto.ChecagemId);
					Analise.isAtualizar = true;
					return false;
				}

				Analise.isAtualizar = false;
				return true;
			}
		});
	},

	selecionarRequerimento: function () {
		var linha = $(this).closest('tr');

		$('.rdbRequerimentoNumero', linha).attr('checked', 'checked');
		$('.spanGerarPdfAnalise', Analise.container).addClass('hide');
		$('.hdnRequerimentoSelecionadoId', Analise.container).val($('.hdnRequerimentoNumero', linha).val());

		Analise.obterRequerimentoPartial(linha);

		if (Analise.continuar) {
			Analise.onValidarChecagem(Analise.container);
			Analise.verificarItensPendente();
		}
	},

	obterRequerimentoPartial: function (container) {
		Mensagem.limpar(Analise.container);

		$('.spanGerarPdfAnalise', Analise.container).addClass('hide');
		$('.btnSalvarAnalise', Analise.container).addClass('hide');
		$('.btnModalOu', Analise.container).addClass('hide');

		container = $('.hdnRequerimentoNumero[value="' + $('.hdnRequerimentoSelecionadoId').val() + '"]', container).closest('tr');

		$('.AnalisandoRequerimento', Analise.container).removeClass('AnalisandoRequerimento');
		container.addClass('AnalisandoRequerimento');

		$('.divConteudoAnalisePartial', Analise.container).empty();

		var objeto = {
			NumeroRequerimento: $('.hdnRequerimentoNumero', container).val(),
			ProtocoloId: $('.hdnProtocoloId', container).val(),
			IsProcesso: $('.hdnProtocoloTipo', container).val() == '1',
			ChecagemId: $('.hdnChecagemNumero', container).val(),
			Atualizar: Analise.isAtualizar
		};

		Analise.continuar = false;

		Analise.manterUrl({ protocoloId: $('.hdnProcDocId', Analise.container).val(), requerimentoId: objeto.NumeroRequerimento });

		$('.spanGerarTituloAnalise', Analise.container).addClass('hide');

		Analise.obterPartial(Analise.settings.urls.obterAnalisePartial, objeto, function (response) {

			$('.divConteudoAnalisePartial', Analise.container).append(response.Html);
			$('.hdnAnaliseId', $('.AnalisandoRequerimento', Analise.container)).val(response.analiseId);
			$('.hdnProjetoDigitalId', container).val(response.projetoDigitalId);

			if (response.MsgInfo && response.MsgInfo.length > 0) {
				Mensagem.gerar(Analise.container, response.MsgInfo);
			}

			$('.spanGerarPdfAnalise', Analise.container).removeClass('hide');
			$('.btnSalvarAnalise', Analise.container).removeClass('hide');
			$('.btnModalOu', Analise.container).removeClass('hide');

			MasterPage.carregando(false);
		});

		Listar.atualizarEstiloTable(Analise.container);
	},

	manterUrl: function (params) {

		var url = Analise.montarUrl(params);

		if ($.browser.msie) {
			//MasterPage.redireciona(url);
			return;
		}

		url = Analise.settings.urls.analisar + url;
		history.pushState(null, null, url);
	},

	montarUrl: function (params) {
		var parametros = '?';

		for (var name in params) {
			parametros += name + '=' + params[name] + '&';
		}

		return parametros.slice(0, parametros.length - 1);
	},

	abrirModalMensagem: function (id) {
		Modal.confirma({
			btnOkCallback: ModalAtualizarRoteiro.onAtualizar,
			btnOkLabel: 'Atualizar',
			onLoadCallbackName: 'ModalAtualizarRoteiro.load',
			url: Analise.settings.urls.modalAtualizarRoteiro,
			urlData: { checagemId: id },
			tamanhoModal: Modal.tamanhoModalGrande
		});
	},

	verificarProtocolo: function () {
		Mensagem.limpar(Analise.container);
		$('.divConteudoAnalisePartial', Analise.container).empty();
		MasterPage.carregando(true);
		var parametros = { numero: $('.txtNumeroProtocolo', Analise.container).val() };

		$.ajax({
			url: Analise.settings.urls.obterRequerimento,
			type: "GET",
			data: parametros,
			cache: false,
			async: true,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Analise.container));
				MasterPage.carregando(false);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					$('.fsRequerimento', Analise.container).addClass('hide');
					$('.divRequerimentoContainer', Analise.container).empty();
					Mensagem.gerar(Analise.container, response.Msg);
					MasterPage.carregando(false);

					return;

				} else {

					$('.hdnProcDocId', Analise.container).val(response.ProtocoloId);
					$('.hdnIsProcesso', Analise.container).val(response.IsProcesso);
					$('.fsRequerimento .divRequerimentoContainer', Analise.container).empty();
					$('.fsRequerimento .divRequerimentoContainer', Analise.container).append(response.Html);
					$('.fsRequerimento').removeClass('hide');

					MasterPage.carregando(false);
					$('.erroCampo', Analise.container).removeClass('erroCampo');
					MasterPage.redimensionar();
					Analise.bloquearCampos();
				}

				Listar.atualizarEstiloTable(Analise.container);
			}
		});
	},

	verificarItensPendente: function () {

		if (Analise.existeItensPendentes(Analise.obterItens())) {
			$('.spanGerarTituloAnalise', Analise.container).removeClass('hide');
			return;
		}

		$('.spanGerarTituloAnalise', Analise.container).addClass('hide');
	},

	adicionarItem: function () {
		var container = $(this).closest('.divItens');

		var item = {
			Tipo: $('.hdnTipoItem', container).val(),
			AnaliseNumero: $('.txtNumeroProtocolo', Analise.container).val()
		};

		Analise.abrirModalItem(item);
	},

	editarItem: function () {
		$('.editandoItem', Analise.container).removeClass('editandoItem');
		var linha = $(this).closest('tr');
		linha.addClass('editandoItem');

		var item = {
			IsEdicao: true,
			Tipo: $('.hdnTipoItem', $(this).closest('div')).val(),
			Id: $('.hdnItemNumero', linha).val(),
			Nome: $('.hdnItemNome', linha).val(),
			ProcedimentoAnalise: $('.hdnItemProcedimento', linha).val(),
			AnaliseNumero: $('.txtNumeroProtocolo', Analise.container).val()
		};

		Analise.abrirModalItem(item);
	},

	excluirItem: function () {
		var linha = $(this).closest('tr');
		var jsItem = $.parseJSON($('.itemJSON', linha).val());
		var flag = (jsItem.IdRelacionamento != 'undefined' && jsItem.IdRelacionamento > 0);

		linha.remove();
		$(Analise.obterItens()).each(function () {
			var item = $.parseJSON($('.itemJSON', this).val());
			if ((item.IdRelacionamento == 'undefined' || item.IdRelacionamento == 0)) {
				flag = 'true';
			}
		});
		$('.hdItemAtualizado', Analise.container).val(flag);
		Listar.atualizarEstiloTable(Analise.container.find('.dataGridTable'));
	},

	excluirItemListar: function (item) {
		var lista = Analise.obterItens();
		var flag = false;

		$(lista).each(function (i, linha) {
			objeto = $.parseJSON($(linha).find('.itemJSON').val());

			if (item.Id == objeto.Id) {
				$(linha).remove();
				$(Analise.obterItens()).each(function () {
					var item = $.parseJSON($('.itemJSON', this).val());
					if ((item.IdRelacionamento == 'undefined' || item.IdRelacionamento == 0) && !flag) {
						flag = true;
					}
				});
				$('.hdItemAtualizado', Analise.container).val(flag);
				return;
			}
		});
	},

	analisarItem: function () {

		$('.editandoItem', Analise.container).removeClass('editandoItem');
		var linha = $(this).closest('tr');
		linha.addClass('editandoItem');

		Modal.abrir(Analise.settings.urls.analiseItem + '/' + $.parseJSON($('.itemJSON', linha).val()).Tipo, null, function (container) {

			AnaliseItem.load(container);
			MasterPage.carregando(true);

			var linha = $('.editandoItem', Analise.container);
			var item = $.parseJSON($('.itemJSON', linha).val());

			$('.txtNome', container).val(item.Nome);
			$('.txtProcedimento', container).val(item.ProcedimentoAnalise);
			$('.txtDataAnalise', container).val(item.DataAnalise ? item.DataAnalise.split(' ')[0] : "");
			$('.ddlSituacao', container).ddlSelect({ selecionado: item.Situacao });

			if (item.Situacao == Analise.situacoes.Reprovado || item.Situacao == Analise.situacoes.Dispensado) {
				$('.txtMotivo', container).val(item.Motivo);
				$('.tabMotivos, .divMotivo', container).removeClass('hide');
			}

			MasterPage.carregando(false);

		}, Modal.tamanhoModalGrande);

		AnaliseItem.associarFuncao = function (obj) {

			var container = $('.editandoItem', Analise.container);
			var item = $.parseJSON($('.itemJSON', container).val());

			var changeSituacao = obj.Situacao != item.Situacao;

			item.DataAnalise = obj.DataAnalise;
			item.Situacao = obj.Situacao;
			item.SituacaoTexto = obj.SituacaoTexto;
			item.Motivo = obj.Motivo;
			item.Editado = 'true';

			$('.itemJSON', container).val($.toJSON(item));
			$('.itemSituacaoTexto', container).text(item.SituacaoTexto);

			$('.hdnItemAtualizado', Analise.container).val('true');

			if (item.Tipo == Analise.tipos.ProjetoDigital && changeSituacao) {
				Analise.gerenciarImportarDados();
			}
		};
	},

	visualizarHistorico: function () {
		$('.editandoItem', Analise.container).removeClass('editandoItem');
		var linha = $(this).closest('tr');
		var objetoItem = $.parseJSON($('.itemJSON', linha).val());
		objetoItem.ChecagemId = $('.hdnChecagemNumero', '.AnalisandoRequerimento').val();

		Modal.abrir(Analise.settings.urls.visualizarHistorico, objetoItem, function (container) {
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);
	},

	visualizarAtividade: function () {
		var container = $(this).closest('tr');

		var requerimento = { ProtocoloId: $('.hdnProtocoloId', container).val() };
		requerimento.IsProcesso = $('.hdnProtocoloTipo', container).val() == '1';
		requerimento.NumeroProtocolo = $('.txtNumeroProtocolo', Analise.container).val();
		requerimento.Tipo = $(requerimento.IsProcesso ? ".ddlTiposProcesso" : ".ddlTiposDocumento", Analise.container).val();
		requerimento.NumeroRequerimento = $('.hdnRequerimentoNumero', container).val();
		requerimento.DataCriacaoRequerimento = $('.hdnRequerimentoData', container).val();

		Modal.abrir(Analise.settings.urls.atividadesSolicitadas, requerimento, function (container) {
			AtividadesSolicitadas.load(container);
		});
	},

	limpar: function () {
		$('.erroCampo', Analise.container).removeClass('erroCampo');
		$('.divConteudoAnalisePartial', Analise.container).empty();
		$('.fsRequerimento .divRequerimentoContainer', Analise.container).empty();
		$('.fsRequerimento', Analise.container).addClass('hide');
		Analise.desbloquearCampos();
		$('.spanGerarPdfAnalise', Analise.container).addClass('hide');

		$('.spanGerarTituloAnalise', Analise.container).addClass('hide');
		$('.btnSalvarAnalise', Analise.container).addClass('hide');
		$('.btnModalOu', Analise.container).addClass('hide');

		Mensagem.limpar(Analise.container);
	},

	bloquearCampos: function () {
		$('.txtNumeroProtocolo', Analise.container).attr('disabled', 'disabled').addClass('disabled');
		$('.ddlTiposProcesso', Analise.container).attr('disabled', 'disabled').addClass('disabled');
		$('.ddlTiposDocumento', Analise.container).attr('disabled', 'disabled').addClass('disabled');
		$('.btnLimpar', Analise.container).removeClass('hide');
		$('.btnVerificar', Analise.container).addClass('hide');
	},

	desbloquearCampos: function () {
		$('.rdbRequerimentoNumero', Analise.container).removeAttr('disabled', 'disabled');

		$('.txtNumeroProtocolo', Analise.container).removeAttr('disabled', 'disabled').removeClass('disabled');
		$('.ddlTiposProcesso', Analise.container).removeAttr('disabled', 'disabled').removeClass('disabled');
		$('.ddlTiposDocumento', Analise.container).removeAttr('disabled', 'disabled').removeClass('disabled');

		$('.btnLimpar', Analise.container).addClass('hide');
		$('.btnVerificar', Analise.container).removeClass('hide');

		$('.txtNumeroProtocolo', Analise.container).val('').focus();
	},

	existeItensPendentes: function (lista) {
		for (var i = 0; i < lista.length; i++) {
			var item = $.parseJSON($('.itemJSON', lista[i]).val());
			if (item.Situacao == 2 || item.Situacao == 4) {
				return true;
			}
		}

		return false;
	},

	associarItemAnalise: function (item) {
		var mensagens = new Array();
		var container = null;

		switch (item.Tipo) {
			case Analise.tipos.Tecnico:
				container = $('.divItensTecnico');
				break;

			case Analise.tipos.Administrativo:
				container = $('.divItensAdmin');
				break;

			case Analise.tipos.ProjetoDigital:
				container = $('.divItensProjetoDigital');
				break;
		}

		$('tbody tr', container).each(function () {
			var itemJSON = $.parseJSON($('.itemJSON', this).val());
			if (itemJSON.Id === item.Id && mensagens.length <= 0) {
				mensagens.push(Mensagem.replace(Analise.mensagens.ItemJaAdicionado, '#ITEM#', itemJSON.Nome));
			}
		});

		if (mensagens.length > 0) {
			return mensagens;
		}

		var linha = $('.trItemTemplate', Analise.container).clone();
		linha.removeClass('trItemTemplate', Analise.container);

		item.Avulso = true;
		item.Editado = true;
		item.Situacao = Analise.situacoes.Pendente;
		item.SituacaoTexto = 'Pendente';
		item.DataAnalise = new Date().toString('dd/MM/yyyy');

		$('.itemJSON', linha).val($.toJSON(item));
		$('.itemNome', linha).text(item.Nome).attr('title', item.Nome);
		$('.itemCondicionante', linha).text(item.Condicionante).attr('title', item.Condicionante);
		$('.itemSituacaoTexto', linha).text(item.SituacaoTexto).attr('title', item.SituacaoTexto);

		$('tbody:last', container).append(linha);
		$('.hdnItemAtualizado', Analise.container).val('true');

		Listar.atualizarEstiloTable(Analise.container);

		return mensagens;
	},

	abrirModalItem: function (objetoItem) {
		Modal.abrir(Analise.settings.urls.criarItem, objetoItem, function (container) {
			ItemListar.load(container, {
				associarFuncao: Analise.associarItemAnalise,
				excluirFuncao: Analise.excluirItemListar,
				editarFuncao: function (item) { }
			});
		}, Modal.tamanhoModalGrande);
	},

	obterPartial: function (url, param, callBack) {
		MasterPage.carregando(true);
		$.ajax({
			url: url,
			data: JSON.stringify(param),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Analise.container, response.Msg);
					MasterPage.carregando(false);
					return;
				}

				Analise.continuar = true;
				callBack(response);
				MasterPage.botoes(Analise.container);
			}
		});
	},

	salvar: function (e, salvarModal, params) {
		var atualizarRoteiro = false;
		var isSalvo = false;
		var container = MasterPage.getContent(this);
		var analise = { Roteiros: [], Itens: [] };

		if (salvarModal) {
			container = MasterPage.getContent(e);
			analise = params;
			atualizarRoteiro = true;
		} else {
			$('.tabRoteiros > tbody > tr', Analise.container).each(function () {
				analise.Roteiros.push({
					Id: $('.hdnRoteiroNumero', this).val(),
					IdRelacionamento: $('.hdnIdRelacionamento', this).val(),
					Tid: $('.hdnRoteiroTid', this).val()
				});
			});

			var lista = Analise.obterItens();

			$(lista).each(function () {
				var item = $.parseJSON($('.itemJSON', this).val());
				analise.Itens.push(item);
				if (item.Editado == 'true') {
					item.DataAnalise = new Date().toString('dd/MM/yyyy HH:mm:ss');
				}
				$('.itemJSON', this).val($.toJSON(item));
			});
		}

		analise.Id = $('.hdnAnaliseId', '.AnalisandoRequerimento', Analise.container).val();
		analise.ChecagemId = $('.hdnChecagemNumero', '.AnalisandoRequerimento', Analise.container).val();
		analise.ProtocoloId = $('.hdnProtocoloId', '.AnalisandoRequerimento', Analise.container).val();
		analise.ProtocoloPai = $('.txtNumeroProtocolo', Analise.container).val();

		MasterPage.carregando(true);
		$.ajax({
			url: Analise.settings.urls.salvar,
			type: "POST",
			data: JSON.stringify({ vm: analise, atualizarRoteiro: atualizarRoteiro }),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Analise.container, response.Msg);
					MasterPage.carregando(false);
					return;
				}

				//Atualiza os valores dos itens depois que foi salvo
				if (!salvarModal) {
					$(lista).each(function (i, linha) {
						var itemTela = $.parseJSON($('.itemJSON', linha).val());
						$(response.Itens).each(function (i, itemSalvo) {
							if (itemSalvo.Id == itemTela.Id) {
								itemSalvo.Editado = false;
								$('.itemJSON', linha).val($.toJSON(itemSalvo));
							}
						});
					});
				}

				$('.hdnItemAtualizado', Analise.container).val('false');
				$('.hdnAnaliseId', '.AnalisandoRequerimento', Analise.container).val(response.AnaliseId);

				mostrar = $('.hdnIsAtualizarRoteiro', Analise.container).val() == 'false';

				if (mostrar && !salvarModal) {
					Mensagem.gerar(container, new Array(Analise.mensagens.Editar));
				}
				Analise.verificarItensPendente();
				MasterPage.carregando(false);
				$('.hdnIsAtualizarRoteiro', Analise.container).val(false);

				isSalvo = true;
			}
		});

		return isSalvo;
	},

	gerarPDF: function () {
		var roteiroId = $('.hdnRoteiroNumero', $(this).closest('tr')).val();
		var tid = $('.hdnRoteiroTid', $(this).closest('tr')).val();
		MasterPage.redireciona(Analise.settings.urls.pdfRoteiro + "?id=" + roteiroId + '&tid=' + tid);
		MasterPage.carregando(false);
	},

	gerarPDFRequerimento: function () {
		var id = $('.hdnRequerimentoId', $(this).closest('tr')).val();
		MasterPage.redireciona(Analise.settings.urls.pdfRequerimento + "?id=" + id);
		MasterPage.carregando(false);
	},

	gerarPDFAnalise: function () {
		if (Analise.validar()) {
			var id = +$('.hdnAnaliseId', '.AnalisandoRequerimento').val();
			console.log(id);
			MasterPage.redireciona(Analise.settings.urls.pdfAnalise + "?id=" + id);
			MasterPage.carregando(false);
		}
	},

	validar: function () {
		var msg = new Array();
		var lista = Analise.obterItens();
		var itemAtualizado = $('.hdItemAtualizado', Analise.container).val();
		$(lista).each(function () {
			var item = $.parseJSON($('.itemJSON', this).val());
			if ((item.Editado == 'true' || itemAtualizado == 'true') && msg.length == 0) {
				msg.push(Analise.mensagens.ItensAtualizados);
				Mensagem.gerar(Analise.container, msg);
			}
		});

		return msg.length == 0;
	},

	gerarTituloPendencia: function () {
		if (Analise.validar()) {
			var protocolo = {
				Id: $('.hdnProcDocId', Analise.container).val(),
				IsProcesso: $('.hdnIsProcesso', Analise.container).val()
			}

			var retorno = MasterPage.validarAjax(Analise.settings.urls.validarPossuiTitPendencia, protocolo, Analise.container, false);

			if (retorno.EhValido) {

				var params = {
					modeloCodigo: 1,
					protocoloId: protocolo.Id,
					isProcesso: protocolo.IsProcesso,
					protocoloSelecionado: $('.hdnProtocoloId', '.AnalisandoRequerimento').val() +
										  (($('.hdnProtocoloTipo', '.AnalisandoRequerimento').val() == '1') ? "@1@" : "@2@") +
										  $('.hdnRequerimentoId', '.AnalisandoRequerimento').val()
				};

				MasterPage.redireciona(Analise.settings.urls.criarTitPendencia + Analise.montarUrl(params));
			}
		}
	},

	visualizarChecagem: function () {
		var checagemId = parseInt($('.txtNumeroChecagem', Analise.container).val());
		Modal.abrir(Analise.settings.urls.visualizarChecagem, { id: checagemId }, function (container) { Modal.defaultButtons(container); }, Modal.tamanhoModalGrande);
	},

	visualizarProjetoGeografico: function () {
		var container = $(this).closest('tr');
		var isAlterado = $('.hdnItemAtualizado', Analise.container).val() == 'true';

		var params = {
			projetoDigitalId: $('.AnalisandoRequerimento .hdnProjetoDigitalId', Analise.container).val(),
			dependenciaTipo: $('.hdnCaracterizacaoTipoId', container).val(),
			requerimentoId: $('.AnalisandoRequerimento .hdnRequerimentoNumero', Analise.container).val(),
			protocoloId: $('.hdnProcDocId', Analise.container).val()
		}

		var url = Analise.settings.urls.projetoGeografico + Analise.montarUrl(params);

		if (isAlterado) {
			Analise.confirm(Analise.mensagens.SairSemSalvarDados.Texto, 'Continuar Análise?', function () { MasterPage.redireciona(url) });
			return;
		}

		MasterPage.redireciona(url);

	},

	visualizarCaracterizacao: function () {
		var container = $(this).closest('tr');
		var isAlterado = $('.hdnItemAtualizado', Analise.container).val() == 'true';

		var params = {
			projetoDigitalId: $('.AnalisandoRequerimento .hdnProjetoDigitalId', Analise.container).val(),
			protocoloId: $('.hdnProcDocId', Analise.container).val()
		}

		var caracterizacaoTipo = $('.hdnCaracterizacaoTipoId', container).val();
		var url = Analise.obterUrlCaracterizacao(caracterizacaoTipo) + Analise.montarUrl(params);

		if (isAlterado) {
			Analise.confirm(Analise.mensagens.SairSemSalvarDados.Texto, 'Continuar Análise?', function () { MasterPage.redireciona(url) });
			return;
		}

		MasterPage.redireciona(url);
	},

	obterUrlCaracterizacao: function (caracterizacaoTipo) {
		var url = '';
		for (var i = 0; i < Analise.settings.urls.caracterizacoes.length; i++) {
			if (Analise.settings.urls.caracterizacoes[i].Tipo == caracterizacaoTipo) {
				url = Analise.settings.urls.caracterizacoes[i].Url;
				break;
			}
		}

		return url;

	},

	confirm: function (textoModal, titulo, btnOkCallback) {

		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				Modal.fechar(conteudoModal);
			},
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				btnOkCallback(conteudoModal);
			},
			conteudo: textoModal,
			titulo: titulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	gerenciarImportarDados: function () {
		var exibir = true;
		var isImportado = $('.AnalisandoRequerimento .hdnIsProjetoDigitalImportado', Analise.container).val() == 'true';

		$('.divItensProjetoDigital .tbItens tbody tr .itemJSON', Analise.container).each(function () {
			var item = JSON.parse($(this).val());

			if(!exibir) return;

			if (item.Situacao != Analise.situacoes.AguardandoImportacao) {
				exibir = false;
			}

			exibir = !isImportado;
		});

		if (exibir) {
			$('.btnImportarProjetoDigital', Analise.container).removeClass('hide');
		} else {
			$('.btnImportarProjetoDigital', Analise.container).addClass('hide');
		}
	},

	importarProjetoDigital: function () {
		var isAlterado = $('.hdnItemAtualizado', Analise.container).val() == 'true';

		if (isAlterado) {
			Mensagem.gerar(Analise.container, [Analise.mensagens.SalvarDadosObrigatorio]);
			return;
		}

		Analise.confirm(Analise.mensagens.ImportarProjetoDigital.Texto, 'Finalizar Análise', function (container) {

			Modal.fechar(container);
			MasterPage.carregando(true);

			var analiseId = $('.hdnAnaliseId', $('.AnalisandoRequerimento', Analise.container)).val() || 0;
			var requerimentoId = $('.AnalisandoRequerimento .hdnRequerimentoNumero', Analise.container).val() || 0;
			var protocoloId = $('.hdnProcDocId', Analise.container).val() || 0;
			$.ajax({
				url: Analise.settings.urls.importarProjetoDigital,
				type: "POST",
				data: JSON.stringify({ analiseId: analiseId, requerimentoId: requerimentoId, protocoloId: protocoloId }),
				dataType: "json",
				contentType: "application/json; charset=utf-8",
				cache: false,
				async: true,
				error: Aux.error,
				success: function (response, textStatus, XMLHttpRequest) {
					MasterPage.carregando(false);

					if (response.IsImportado) {
						MasterPage.redireciona(response.Url);
						return;
					}

					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(Analise.container, response.Msg);
					}
				}
			});
		});
	}
}

ModalAtualizarRoteiro = {
	itensJSON: null,
	roteirosJSON: null,
	mensagem: null,
	container: null,

	load: function (container) {
		ModalAtualizarRoteiro.container = container;
		Mensagem.gerar($('.divMensagemRoteiro'), ModalAtualizarRoteiro.mensagem);
	},

	onAtualizar: function () {
		Analise.isAtualizar = true;
		$('.hdnIsAtualizarRoteiro', Analise.container).val(false);
		var analise = { Roteiros: ModalAtualizarRoteiro.roteirosJSON, Itens: ModalAtualizarRoteiro.itensJSON };

		if (Analise.salvar(this, true, analise)) {
			Modal.fechar(ModalAtualizarRoteiro.container);
			$('.hdItemAtualizado', Analise.container).val('true');
			Analise.obterRequerimentoPartial(Analise.container);
		}
	}
}
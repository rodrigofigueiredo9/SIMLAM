/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ChecagemRoteiroSalvar = {
	associarItemRoteiroModalLink: '',
	visualizarRoteiroModalLink: '',
	editarItemRoteiroModalLink: '',
	urlObterItensRoteiro: '',
	urlAssociarRoteiro: '',
	urlCheckListRoteiroPdfObj: '',
	urlCheckListRoteiroPdfObjValidar: '',
	urlValidarAssociarRoteiro: '',
	Mensagens: {},
	container: null,
	ContainerModalDispensa: null,

	load: function (container) {
		ChecagemRoteiroSalvar.container = container;
		$('.btnAssociarRoteiro', container).click(ChecagemRoteiroSalvar.onModalRoteiroAbrirClick);
		$('.btnExcluirRoteiro', container).live('click', ChecagemRoteiroSalvar.onExcluirRoteiroClick);
		$('.btnVisualizarRoteiro', container).live('click', ChecagemRoteiroSalvar.onVisualizarRoteiroClick);

		$('.btnAssociarItemRoteiro', container).click(ChecagemRoteiroSalvar.onModalAdicionarItemRoteiroAbrirClick);
		$('.btnExcluirItemRoteiro', container).live('click', ChecagemRoteiroSalvar.onExcluirItemRoteiroClick);
		$('.btnDispensarItemRoteiro', container).live('click', ChecagemRoteiroSalvar.onDispensarItemRoteiroClick);
		$('.btnConferirItemRoteiro', container).live('click', ChecagemRoteiroSalvar.onConferirItemRoteiroClick);
		$('.btnCancelarItemRoteiro', container).live('click', ChecagemRoteiroSalvar.onCancelarRecbDispItemRoteiroClick);

		$('.btnSalvarCheckList', container).click(ChecagemRoteiroSalvar.reorganizarTabelas);
		$('.btnEditarCheckList', container).click(ChecagemRoteiroSalvar.reorganizarTabelas);

		$('.btnGerarPdfPendencia', container).click(ChecagemRoteiroSalvar.gerarPdfPendencia);

		$('.txtInteressado', container).focus();
		ChecagemRoteiroSalvar.setarBotoesGrid();
	},

	gerarPdfPendencia: function () {
		var checagem = {
			Interessado: $('.txtInteressado', ChecagemRoteiroSalvar.container).val(),
			Roteiros: new Array(),
			Itens: new Array()
		};

		$('.tabRoteiros tbody tr').each(function (i, linha) {
			checagem.Roteiros.push({
				Nome: $('.trRoteiroNome', linha).text(),
				Versao: $('.trRoteiroVersao', linha).text()
			});
		});

		$('.tabItensRoteiro tbody tr').each(function () {
			var item = $.parseJSON($('.hdnItemJson', this).val());
			checagem.Itens.push({
				Nome: item.Nome,
				Condicionante: item.Condicionante,
				Situacao: item.Situacao,
				Motivo: item.Motivo
			});
		});

		var retorno = MasterPage.validarAjax(ChecagemRoteiroSalvar.urlCheckListRoteiroPdfObjValidar, checagem, ChecagemRoteiroSalvar.container, false);
		if (!retorno.EhValido) {
			if (retorno.Msg && retorno.Msg.length > 0) {
				Mensagem.gerar(ChecagemRoteiroSalvar.container, retorno.Msg);
			}
		} else {
			Aux.download(ChecagemRoteiroSalvar.urlCheckListRoteiroPdfObj, checagem);
		}
	},

	onModalRoteiroAbrirClick: function () {
		Modal.abrir(ChecagemRoteiroSalvar.urlAssociarRoteiro, null, function (container) {
			RoteiroListar.load(container, ChecagemRoteiroSalvar.associarRoteiro);
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);
	},

	onModalAdicionarItemRoteiroAbrirClick: function () {
		Modal.abrir(ChecagemRoteiroSalvar.associarItemRoteiroModalLink, null,
		function (container) {
			ItemListar.load(container, {
				associarFuncao: ChecagemRoteiroSalvar.onAssociarItemRoteiro,
				excluirFuncao: ChecagemRoteiroSalvar.onExcluirItemListar,
				editarFuncao: ChecagemRoteiroSalvar.onAtualizarItemListar
			});
		}, Modal.tamanhoModalGrande);
	},

	onExcluirItemListar: function (item) {
		$('.tabItensRoteiro tr', ChecagemRoteiroSalvar.container).each(function () {
			if (item.Id == $(this).find('.hdnItemId').val()) {
				$(this).remove();
			}
		});
	},

	onAtualizarItemListar: function (item) {
		$('.tabItensRoteiro tr', ChecagemRoteiroSalvar.container).each(function () {
			if (item.Id == $(this).find('.hdnItemId').val()) {
				$('.trItemRoteiroNome', this).text(item.Nome);
				$('.trItemRoteiroCondicionante', this).text(item.Condicionante);
			}
		});
	},

	onAssociarItemRoteiro: function (item) {

		var msgErros = new Array();

		item['Situacao'] = 1;
		item['SituacaoTexto'] = 'Pendente';

		var itemJsonString = $.toJSON(item);
		var linha;

		var isExiste = false;
		$('.tabItensRoteiro tbody tr').each(function (i) {
			linha = $(this);

			var itemTabela = $.parseJSON(linha.find('.hdnItemJson').val());

			if (itemTabela.Id === item.Id) {
				isExiste = true;
			}
		});

		if (isExiste) {
			ChecagemRoteiroSalvar.setarBotoesGrid();
			msgErros.push(ChecagemRoteiroSalvar.Mensagens.ItemJaAdicionado);
			return msgErros;
		}
		linha = $('.trItemRoteiroTemplate').clone().removeClass('trItemRoteiroTemplate');

		var ultimoIndex = $('.tabItensRoteiro tbody tr').length;
		linha.addClass((ultimoIndex % 2) === 0 ? 'par' : 'impar');

		linha.find('.hdnItemId').val(item.Id);
		linha.find('.hdnItemJson').val(itemJsonString);
		linha.find('.trItemRoteiroNome').text(item.Nome);
		linha.find('.trItemRoteiroNome').attr('title', item.Nome);

		linha.find('.trItemRoteiroCondicionante').text(item.Condicionante);
		linha.find('.trItemRoteiroCondicionante').attr('title', item.Condicionante);

		linha.find('.trItemRoteiroSituacaoTexto').text(item.SituacaoTexto);
		linha.find('.trItemRoteiroSituacaoTexto').attr('title', item.SituacaoTexto);

		$('.tabItensRoteiro > tbody:last').append(linha);

		ChecagemRoteiroSalvar.setarBotoesGrid();
		return msgErros;
	},

	onExcluirRoteiroClick: function () {
		var idRoteiro = $(this).closest("tr").find('.hdnRoteiroId').val();

		$('.tabItensRoteiro tbody tr', ChecagemRoteiroSalvar.container).each(function (i) {
			linha = $(this);

			var item = $.parseJSON(linha.find('.hdnItemJson').val());

			if (typeof item.Roteiros != 'undefined' && item.Roteiros != null && item.Roteiros.length > 0) {
				if (item.Roteiros.length > 1) {
					for (var j = 0; j < item.Roteiros.length; j++) {
						if (item.Roteiros[j] == idRoteiro) {
							item.Roteiros = $.grep(item.Roteiros, function (val) { return val != idRoteiro; });
							linha.find('.hdnItemJson').val($.toJSON(item));
						}
					}
				}
				else if (item.Roteiros[0] == idRoteiro) {
					linha.remove();
				}
			}
		})

		$(this).closest("tr").remove();

		if ($('.tabItensRoteiro tbody tr', ChecagemRoteiroSalvar.container).length == 0) {
			$('.btnGerarPdfPendencia', ChecagemRoteiroSalvar.container).addClass('hide');
		}
	},

	onVisualizarRoteiroClick: function () {
		var idRoteiro = $(this).closest("tr").find('.hdnRoteiroId').val();
		Modal.abrir(ChecagemRoteiroSalvar.visualizarRoteiroModalLink + "/" + idRoteiro, null, function (container) {
			Modal.defaultButtons(container);
		});
	},

	associarRoteiro: function (id, numero, texto, versao, tid) {
		var arrayMensagem = new Array();

		// validar associar roteiro
		var roteiroIds = [];
		$('.tabRoteiros tbody tr .hdnRoteiroId').each(function (i) {
			roteiroIds.push($(this).val());
		});

		var parametros = {
			roteirosAssociados: roteiroIds,
			roteiroAssociado: id
		};
		var retorno = MasterPage.validarAjax(ChecagemRoteiroSalvar.urlValidarAssociarRoteiro, parametros, ChecagemRoteiroSalvar.container, false);
		if (retorno.EhValido === false) {
			if (retorno.Msg && retorno.Msg.length > 0) {
				$.extend(arrayMensagem, retorno.Msg);
			}
		}

		var roteiro = {
			Id: id,
			IdRelacionamento: 0,
			Numero: numero,
			Nome: texto,
			Versao: versao,
			Tid: tid
		};

		var itemJsonString = $.toJSON(roteiro);


		if (arrayMensagem.length <= 0) {
			var ultimoIndex = $('.tabRoteiros tbody tr').length;

			if (typeof id == 'undefined' || id == null || id == '' || id <= 0) {
				return;
			}

			var linha = $('.trRoteiroTemplate').clone().removeClass('trRoteiroTemplate');
			linha.addClass((ultimoIndex % 2) === 0 ? 'par' : 'impar');

			linha.find('.hdnRoteiroJson').val(itemJsonString);
			linha.find('.hdnRoteiroId').val(id);
			linha.find('.hdnRoteiroIdRelacionamento').val(0);
			linha.find('.hdnRoteiroNumero').val(numero);
			linha.find('.hdnRoteiroNome').val(texto);
			linha.find('.hdnRoteiroVersao').val(versao);
			linha.find('.hdnRoteiroTid').val(tid);

			linha.find('.trRoteiroNumero').text(numero);
			linha.find('.trRoteiroNome').text(texto);
			linha.find('.trRoteiroVersao').text(versao);
			linha.find('.trRoteiroNumero').attr('title', numero);
			linha.find('.trRoteiroNome').attr('title', texto);
			linha.find('.trRoteiroVersao').attr('title', versao);

			$('.tabRoteiros > tbody:last').append(linha);

			ChecagemRoteiroSalvar.adicionarItensRoteiro(id);
		}

		$('.btnGerarPdfPendencia', ChecagemRoteiroSalvar.container).removeClass('hide');
		return arrayMensagem;
	},

	adicionarItensRoteiro: function (id) {
		$.getJSON(ChecagemRoteiroSalvar.urlObterItensRoteiro, "idRoteiro=" + id, function (data, textStatus, xhr) {

			if (Aux.errorGetPost(data, textStatus, xhr, $(".mensagemContent"))) {
				return;
			}

			if (typeof data.Msg != 'undefined' && data.Msg != null && data.Msg.length > 0) {
				Mensagem.gerar($(".mensagemContent"), data.Msg);
				return;
			}
			else {
				for (var i = 0; i < data.Itens.length; i++) {
					var itemRepetido = false;

					$('.tabItensRoteiro tbody tr').each(function (j) {
						linha = $(this);

						var item = $.parseJSON(linha.find('.hdnItemJson').val());

						if (item.Id === data.Itens[i].Id) {
							itemRepetido = true;

							if (item.Tid !== data.Itens[i].Tid) {
								var roteiros = item.Roteiros;
								item = data.Itens[i];
								item.Roteiros = roteiros;
								linha.find('.trItemRoteiroNome').text(item.Nome).attr('title', item.Nome);
								linha.find('.trItemRoteiroCondicionante').text(item.Condicionante).attr('title', item.Condicionante);
							}

							if (typeof item.Roteiros == 'undefined' || item.Roteiros === null) {
								item.Roteiros = new Array();
							}
							item.Roteiros[item.Roteiros.length] = id;
							linha.find('.hdnItemJson').val($.toJSON(item));

							ChecagemRoteiroSalvar.setarBotaoGrid(item, linha);
						}
					});

					if (!itemRepetido) {
						data.Itens[i].Roteiros[0] = id;
						var itemJsonString = $.toJSON(data.Itens[i]);
						var ultimoIndex = $('.tabItensRoteiro tbody tr').length;
						var linha = $('.trItemRoteiroTemplate').clone().removeClass('trItemRoteiroTemplate');

						linha.addClass((ultimoIndex % 2) === 0 ? 'par' : 'impar');

						linha.find('.hdnItemJson').val(itemJsonString);

						linha.find('.hdnItemId').val(data.Itens[i].Id);
						linha.find('.trItemRoteiroNome').text(data.Itens[i].Nome);
						linha.find('.trItemRoteiroNome').attr('title', data.Itens[i].Nome);
						linha.find('.trItemRoteiroCondicionante').text(data.Itens[i].Condicionante);
						linha.find('.trItemRoteiroCondicionante').attr('title', data.Itens[i].Condicionante);
						linha.find('.trItemRoteiroSituacaoTexto').text(data.Itens[i].SituacaoTexto);
						linha.find('.trItemRoteiroSituacaoTexto').attr('title', data.Itens[i].SituacaoTexto);

						ChecagemRoteiroSalvar.setarBotaoGrid(data.Itens[i], linha);

						$('.tabItensRoteiro > tbody:last').append(linha);
					}
				}
			}
		});
	},

	onExcluirItemRoteiroClick: function () {
		var item = $.parseJSON($(this).closest("tr").find('.hdnItemJson').val());

		if (typeof item.Roteiros != 'undefined' && item.Roteiros != null && item.Roteiros.length > 0) {
			var arrayMensagem = new Array();
			var modalContent = $('#central');
			arrayMensagem.push(ChecagemRoteiroSalvar.Mensagens.RemoverItemAssociadoRoteiro);
			Mensagem.gerar(modalContent, arrayMensagem);
			return;
		}

		if (item.Situacao != 1) {
			var arrayMensagem = new Array();
			var modalContent = $('#central');
			arrayMensagem.push(ChecagemRoteiroSalvar.Mensagens.RemoverItemSituacaoInvalida);
			arrayMensagem[0].Texto = arrayMensagem[0].Texto.replace('#situacao#', item.SituacaoTexto);
			Mensagem.gerar(modalContent, arrayMensagem);
			arrayMensagem[0].Texto = arrayMensagem[0].Texto.replace(item.SituacaoTexto, '#situacao#');
			return;
		}

		$(this).closest("tr").remove();
	},

	onLoadDispensar: function (container) {

		ChecagemRoteiroSalvar.ContainerModalDispensa = $('.modalBranco', container);

		$('.btnSalvarMotivo', container).click(ChecagemRoteiroSalvar.onValidarDispensa);
		$('.btnFechar', container).click(function () { Modal.fechar(this); });
		$('.txtMotivo', container).focus();
	},

	onDispensarItemRoteiroClick: function () {

		$('.alterandoSituacao', ChecagemRoteiroSalvar.container).removeClass('alterandoSituacao');

		var linha = $(this).closest("tr");

		var item = $.parseJSON(linha.find('.hdnItemJson').val());

		if (item.Situacao != 1) {
			var arrayMensagem = new Array();
			var modalContent = $('#central');
			arrayMensagem.push(ChecagemRoteiroSalvar.Mensagens.DispensarItemSituacaoIvalida);
			arrayMensagem[0].Texto = arrayMensagem[0].Texto.replace('#situacao#', item.SituacaoTexto);
			Mensagem.gerar(modalContent, arrayMensagem);
			arrayMensagem[0].Texto = arrayMensagem[0].Texto.replace(item.SituacaoTexto, '#situacao#');
			return;
		}

		linha.addClass('alterandoSituacao');

		var item = $.parseJSON($(this).closest("tr").find('.hdnItemJson').val());

		var conteudo = $('.divConteudoMotivo', ChecagemRoteiroSalvar.container).html();

		Modal.abrirHtml(conteudo, { onLoadCallbackName: ChecagemRoteiroSalvar.onLoadDispensar, titulo: 'Adicionar Motivo', html: true });
	},

	onValidarDispensa: function () {

		var motivo = $('.txtMotivo', ChecagemRoteiroSalvar.ContainerModalDispensa).val();

		if (motivo == '') {
			Mensagem.gerar(ChecagemRoteiroSalvar.ContainerModalDispensa, new Array(ChecagemRoteiroSalvar.Mensagens.MotivoObrigatorio));
			return false;
		}

		ChecagemRoteiroSalvar.onDispensarItemRoteiro(motivo);
		$('.btnFechar', ChecagemRoteiroSalvar.ContainerModalDispensa).click();
	},

	onDispensarItemRoteiro: function (motivo) {

		var linhatr = $('.alterandoSituacao', ChecagemRoteiroSalvar.container);

		var item = $.parseJSON(linhatr.find('.hdnItemJson').val());

		item.Situacao = 3;
		item.SituacaoTexto = 'Dispensado';
		item.Motivo = motivo;

		linhatr.find('.hdnItemJson').val($.toJSON(item));

		linhatr.find('.trItemRoteiroSituacaoTexto').text('Dispensado');
		linhatr.find('.trItemRoteiroSituacaoTexto').attr('title', 'Dispensado');

		ChecagemRoteiroSalvar.setarBotaoGrid(item, linhatr);
		ChecagemRoteiroSalvar.mostrarBotoaoGegarPDFPend();
		$('.alterandoSituacao', ChecagemRoteiroSalvar.container).removeClass('alterandoSituacao');
	},

	onConferirItemRoteiroClick: function () {
		var item = $.parseJSON($(this).closest("tr").find('.hdnItemJson').val());

		if (item.Situacao != 1) {
			var arrayMensagem = new Array();
			var modalContent = $('#central');
			arrayMensagem.push(ChecagemRoteiroSalvar.Mensagens.ConferirItemSituacaoIvalida);
			arrayMensagem[0].Texto = arrayMensagem[0].Texto.replace('#situacao#', item.SituacaoTexto);
			Mensagem.gerar(modalContent, arrayMensagem);
			arrayMensagem[0].Texto = arrayMensagem[0].Texto.replace(item.SituacaoTexto, '#situacao#');
			return;
		}

		item.Situacao = 2;
		item.SituacaoTexto = 'Conferido';
		item.Motivo = '';

		var linha = $(this).closest("tr");
		linha.find('.hdnItemJson').val($.toJSON(item));

		linha.find('.trItemRoteiroSituacaoTexto').text('Conferido');
		linha.find('.trItemRoteiroSituacaoTexto').attr('title', 'Conferido');

		ChecagemRoteiroSalvar.setarBotaoGrid(item, linha);
		ChecagemRoteiroSalvar.mostrarBotoaoGegarPDFPend();
	},

	onCancelarRecbDispItemRoteiroClick: function () {
		var item = $.parseJSON($(this).closest("tr").find('.hdnItemJson').val());

		if (item.Situacao != 2 && item.Situacao != 3) {
			var arrayMensagem = new Array();
			var modalContent = $('#central');
			arrayMensagem.push(ChecagemRoteiroSalvar.Mensagens.CancelarDispRecebItemSituacaoIvalida);
			arrayMensagem[0].Texto = arrayMensagem[0].Texto.replace('#situacao#', item.SituacaoTexto);
			Mensagem.gerar(modalContent, arrayMensagem);
			arrayMensagem[0].Texto = arrayMensagem[0].Texto.replace(item.SituacaoTexto, '#situacao#');
			return;
		}

		item.Situacao = 1;
		item.SituacaoTexto = 'Pendente';
		item.Motivo = '';

		var linha = $(this).closest("tr");
		linha.find('.hdnItemJson').val($.toJSON(item));

		linha.find('.trItemRoteiroSituacaoTexto').text('Pendente');
		linha.find('.trItemRoteiroSituacaoTexto').attr('title', 'Pendente');

		ChecagemRoteiroSalvar.setarBotaoGrid(item, linha);
		ChecagemRoteiroSalvar.mostrarBotoaoGegarPDFPend();
	},

	setarBotoesGrid: function () {
		var mostrarBtnGerarPendencia = false;
		$('.tabItensRoteiro tbody tr', ChecagemRoteiroSalvar.container).each(function (i) {
		    linha = $(this);
		    var item = $.parseJSON(linha.find('.hdnItemJson').val());
		    ChecagemRoteiroSalvar.setarBotaoGrid(item, linha);
		    if (item.Situacao.toString() == '1') {
		        mostrarBtnGerarPendencia = true;
		    }
		});

		if (mostrarBtnGerarPendencia) {
			$('.btnGerarPdfPendencia', ChecagemRoteiroSalvar.container).removeClass('hide');
		} else {
			$('.btnGerarPdfPendencia', ChecagemRoteiroSalvar.container).addClass('hide');
		}
	},

	mostrarBotoaoGegarPDFPend: function () {
		var mostrarBtnGerarPendencia = false;
		$('.tabItensRoteiro tbody tr').each(function (i, linha) {
			var item = $.parseJSON($(linha).find('.hdnItemJson').val());
			if (item.Situacao.toString() == '1') {
				mostrarBtnGerarPendencia = true;
			}
		})

		if (mostrarBtnGerarPendencia) {
			$('.btnGerarPdfPendencia', ChecagemRoteiroSalvar.container).removeClass('hide');
		} else {
			$('.btnGerarPdfPendencia', ChecagemRoteiroSalvar.container).addClass('hide');
		}
	},

	setarBotaoGrid: function (item, linha) {
		linha.find('.btnEditarItemRoteiro, .btnCancelarItemRoteiro, .btnConferirItemRoteiro, .btnDispensarItemRoteiro').removeClass('ui-state-default');
		linha.find('.btnEditarItemRoteiro, .btnCancelarItemRoteiro, .btnConferirItemRoteiro, .btnDispensarItemRoteiro').removeClass('ui-state-disabled');
		linha.find('.btnEditarItemRoteiro, .btnCancelarItemRoteiro, .btnConferirItemRoteiro, .btnDispensarItemRoteiro').addClass('ui-state-default');

		if (typeof item.Roteiros != 'undefined' && item.Roteiros != null && item.Roteiros.length > 0) {
			linha.find('.btnEditarItemRoteiro').removeClass('ui-state-default');
			linha.find('.btnEditarItemRoteiro').addClass('ui-state-disabled');
			linha.find('.btnExcluirItemRoteiro').hide();
		} else {
			linha.find('.btnExcluirItemRoteiro').show();
		}

		switch (item.Situacao.toString()) {
			case '1':
				linha.find('.btnCancelarItemRoteiro').removeClass('ui-state-default');
				linha.find('.btnCancelarItemRoteiro').addClass('ui-state-disabled');
				mostrarBtnGerarPendencia = true;
				break;
			case '2':
			case '3':
				linha.find('.btnConferirItemRoteiro, .btnDispensarItemRoteiro, .btnExcluirItemRoteiro, .btnEditarItemRoteiro').removeClass('ui-state-default');
				linha.find('.btnConferirItemRoteiro, .btnDispensarItemRoteiro, .btnExcluirItemRoteiro, .btnEditarItemRoteiro').addClass('ui-state-disabled');
				break;
		}
	},

	reorganizarTabelas: function () {
		$('.tabRoteiros tbody tr').each(function (i) {
			linha = $(this);

			linha.find('.hdnRoteiroJson').attr('name', 'vm.RoteirosJson[' + i + ']');
			linha.find('.hdnRoteiroId').attr('name', 'vm.RoteirosJson[' + i + '].Id');
			linha.find('.hdnRoteiroIdRelacionamento').attr('name', 'vm.RoteirosJson[' + i + '].IdRelacionamento');
			linha.find('.hdnRoteiroNumero').attr('name', 'vm.RoteirosJson[' + i + '].Numero');
			linha.find('.hdnRoteiroNome').attr('name', 'vm.RoteirosJson[' + i + '].Nome');
			linha.find('.hdnRoteiroVersao').attr('name', 'vm.RoteirosJson[' + i + '].Versao');
			linha.find('.hdnRoteiroTid').attr('name', 'vm.RoteirosJson[' + i + '].Tid');

			linha.removeClass();
			linha.addClass((i % 2) === 0 ? 'par' : 'impar');
		})

		$('.tabItensRoteiro tbody tr').each(function (i) {
			linha = $(this);
			linha.find('.hdnItemJson').attr('name', 'vm.ItensJson[' + i + ']');

			linha.removeClass();
			linha.addClass((i % 2) === 0 ? 'par' : 'impar');
		})
	}
}
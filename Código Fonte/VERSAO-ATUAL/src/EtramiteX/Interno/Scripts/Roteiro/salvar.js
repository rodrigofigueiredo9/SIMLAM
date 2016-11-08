/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

RoteiroSalvar = {
	urlAssociarRoteiroCopiar: '',
	urlPdfRoteiro: '',
	urlConfirmarEditar: '',
	urlAssociarCopiar: '',
	urlListarItem: '',
	urlValidarAssociarAtividade: '',
	urlListarAtividade: null,
	listaAtividades: [],
	listarModelos: [],
	container: null,
	Mensagens: {},

	load: function (content) {
		RoteiroSalvar.container = MasterPage.getContent(content);
		$('.btnRoteiroNovo', content).click(RoteiroSalvar.onRoteiroNovoClick);
		$('.btnRoteiroCopiar', content).click(RoteiroSalvar.onRoteiroCopiarAssociarClick);
		$('.btnAddAnexo', content).click(RoteiroSalvar.onAdicionarAnexoClick);
		$('.btnAddPalavraChave', content).click(RoteiroSalvar.onAdicionarPalavraChaveClick);
		$('.btnExcluirLinha', content).live('click', RoteiroSalvar.onExcluirLinha);
		$('.bntEditarRoteiro', content).click(RoteiroSalvar.onRoteiroEditarClick);
		$('.bntSalvarRoteiro', content).click(RoteiroSalvar.onRoteiroSalvarClick);

		content.delegate('.btnAssociarItem', 'click', RoteiroSalvar.onListarItem);

		content.delegate('.btnDescerLinhaTab', 'click', function (container) { RoteiroSalvar.onBtnDescerClick(container, 'tabItens'); });
		content.delegate('.btnSubirLinhaTab', 'click', function (container) { RoteiroSalvar.onBtnSubirClick(container, 'tabItens'); });

		content.delegate('.btnDescerLinha', 'click', function (container) { RoteiroSalvar.onBtnDescerClick(container, 'tabAnexos'); });
		content.delegate('.btnSubirLinha', 'click', function (container) { RoteiroSalvar.onBtnSubirClick(container, 'tabAnexos'); });

		RoteiroSalvar.atualizaEstiloGrid('tabItens');
		RoteiroSalvar.atualizaEstiloGrid('tabAnexos');

		$('.btnAssociarAtividade', content).live('click', RoteiroSalvar.onAbrirListarAtividade);
		$('.btnExcluirAtividade', content).live('click', RoteiroSalvar.onExcluirAtividade);
		$('.ddlSetor', content).focus();
	},

	onBtnDescerClick: function (container, tab) {
		var tr = $(container.currentTarget).closest('tr');
		tr.next().after(tr);
		RoteiroSalvar.atualizaEstiloGrid(tab);
	},

	onBtnSubirClick: function (container, tab) {
		var tr = $(container.currentTarget).closest('tr');
		tr.prev().before(tr);
		RoteiroSalvar.atualizaEstiloGrid(tab);
	},

	atualizaEstiloGrid: function (tab) {
		var table = RoteiroSalvar.container.find('.' + tab);
		Listar.atualizarEstiloTable(table);

		var rows = $('tbody tr:visible', table).removeClass('selecionado');
		rows.each(function (index, elem) {
			var btnDescer = $(elem).find('.btnDescerLinhaTab,.btnDescerLinha');
			var btnSubir = $(elem).find('.btnSubirLinhaTab,.btnSubirLinha');

			if (index == 0) {
				btnSubir.addClass('desativado');
			} else {
				btnSubir.removeClass('desativado');
			}

			if (index >= rows.length - 1) {
				btnDescer.addClass('desativado');
			} else {
				btnDescer.removeClass('desativado');
			}
		});
	},

	onAbrirListarAtividade: function () {

		Modal.abrir(RoteiroSalvar.urlListarAtividade, null, function (container) {
			AtividadeSolicitadaListar.load(container);
			AtividadeSolicitadaListar.settings.onAssociarCallback = RoteiroSalvar.onAssociarAtividade;
			Modal.defaultButtons(container, null, null, null, null, "Voltar", null, null);
		}, Modal.tamanhoModalGrande);
	},

	onExcluirAtividade: function () {

		$(this).closest('tr').remove();

		if ($('.tabAtividades tbody tr', RoteiroSalvar.container).length < 1) {
			$('.linhaModeloTitulos', RoteiroSalvar.container).addClass('hide');
			$('.divFinalidade', RoteiroSalvar.container).addClass('hide');
			$('.linhaModeloTitulos .modelosConteudo').empty();
			$('.tabAtividades', RoteiroSalvar.container).addClass('hide');
			$('.lblNenhumaAtividade', RoteiroSalvar.container).removeClass('hide');
		} else {
			Listar.atualizarEstiloTable($('.tabAtividades', RoteiroSalvar.container));
			RoteiroSalvar.configurarAtividades();
		}
	},

	configurarAtividades: function () {

		RoteiroSalvar.AtualizarListaAtividades();
		RoteiroSalvar.AtualizarListaModelos();

		$.ajax({
			url: RoteiroSalvar.urlObterModelosAtividades,
			data: JSON.stringify(RoteiroSalvar.listaAtividades),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(RoteiroSalvar.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				$('.linhaModeloTitulos .modelosConteudo').empty();

				if (response.Msg.length > 0) {
					Mensagem.gerar(RoteiroSalvar.container, response.Msg);
				}

				var linhaConteudo = $('.linhaModeloTitulos .modelosConteudo', RoteiroSalvar.container);
				$(response.Lista).each(function (i) {
					var item = this;
					$(RoteiroSalvar.listarModelos).each(function () {

						if (item.Id == this.Id) {
							item.IsAtivo = true;
						}
					});

					var modelo = $('.templateModeloTitulo').clone();

					modelo.removeClass('templateModeloTitulo');

					modelo.find('.labelCheck').attr('title', item.Texto);
					modelo.find('.labelCheck').attr('for', 'ck' + i);

					modelo.find('.chkModelos').val(item.Id);
					modelo.find('.chkModelos').attr('title', item.Texto);

					modelo.find('.chkModelos').attr('id', 'ck' + i);
					modelo.find('.labelCheck').text(item.Texto);

					if (item.IsAtivo) {
						modelo.find('.chkModelos').attr('checked', 'checked');
					}

					modelo.removeClass('hide');
					$(linhaConteudo).append(modelo);
					$('.divFinalidade', RoteiroSalvar.container).removeClass('hide');
					$('.linhaModeloTitulos', RoteiroSalvar.container).removeClass('hide');
				});
			}
		});
		MasterPage.redimensionar();
	},

	onAssociarAtividade: function (objeto) {

		var mensagem = [];

		$('.tabAtividades tbody tr', RoteiroSalvar.container).each(function () {
			if ($('.hdnAtividadeId', this).val() == objeto.Id) {
				mensagem.push(RoteiroSalvar.Mensagens.AtividadeJaAdicionado);
			}
		});

		if (mensagem.length > 0) {
			return mensagem;
		}

		var retorno = MasterPage.validarAjax(
			RoteiroSalvar.urlValidarAssociarAtividade,
			{ roteiroSetor: $('.ddlSetor', RoteiroSalvar.container).val(), atividadeId: objeto.Id },
			null, false
		);

		if (!retorno.EhValido) {
			$.extend(mensagem, retorno.Msg);
		} else {
			var linha = $('.trItemTemplateAtividade', RoteiroSalvar.container).clone();

			linha.removeClass('trItemTemplateAtividade');
			linha.find('.hdnAtividadeId').val(objeto.Id);
			linha.find('.AtividadeTexto').text(objeto.Nome);
			linha.find('.AtividadeTexto').attr('title', objeto.Nome);

			$('.tabAtividades > tbody', RoteiroSalvar.container).append(linha);
			Listar.atualizarEstiloTable($('.tabAtividades', RoteiroSalvar.container));
			RoteiroSalvar.configurarAtividades();

			$('.tabAtividades', RoteiroSalvar.container).removeClass('hide');
			$('.lblNenhumaAtividade', RoteiroSalvar.container).addClass('hide');

			var msg = {};
			$.extend(msg, RoteiroSalvar.Mensagens.AtividadeAssociada);
			msg.Texto = msg.Texto.replace('#ATIVIDADE#', objeto.Nome);

			mensagem.push(msg);
		}

		return mensagem;
	},

	AtualizarListaModelos: function () {
		RoteiroSalvar.listarModelos = new Array();
		$('.linhaModeloTitulos .modelosConteudo .modeloTitulo', RoteiroSalvar.container).each(function () {
			if ($('.chkModelos', this).attr('checked')) {
				RoteiroSalvar.listarModelos.push({ Id: $('.chkModelos', this).val(), IdRelacionamento: $('.hdnModeloIdRelacionamento', this).val(), Texto: $('.labelCheck', this).attr('title') });
			}
		});
	},

	AtualizarListaAtividades: function () {
		RoteiroSalvar.listaAtividades = new Array();
		$('.tabAtividades tbody tr', RoteiroSalvar.container).each(function () {
			RoteiroSalvar.listaAtividades.push({ Id: $('.hdnAtividadeId', this).val(), IdRelacionamento: $('.hdnAtividadeIdRelacionamento', this).val(), Texto: $('.AtividadeTexto', this).attr('title') });
		});
	},

	onListarItem: function () {
		Modal.abrir(RoteiroSalvar.urlListarItem, null,
		function (container) {
			ItemListar.load(container, {
				associarFuncao: RoteiroSalvar.onAssociarItem,
				excluirFuncao: RoteiroSalvar.onExcluirItemListar,
				editarFuncao: RoteiroSalvar.onAtualizarItemListar
			});
		}, Modal.tamanhoModalGrande);
	},

	onExcluirItemListar: function (item) {
		$('.tabItens tr', RoteiroSalvar.container).each(function (i, itemTr) {
			if (item.Id == $(itemTr).find('.hdnItemId').val()) {
				$(itemTr).remove();
			}
		})

		RoteiroSalvar.atualizaEstiloGrid('tabItens');
	},

	onAtualizarItemListar: function (item) {
		$('.tabItens tr', RoteiroSalvar.container).each(function (i, itemTr) {
			if (item.Id == $(itemTr).find('.hdnItemId').val()) {
				$('.ItemNome', itemTr).text(item.Nome);
				$('.ItemCondicionante', itemTr).text(item.Condicionante);
				$('.ItemTipo', itemTr).text(item.TipoTexto);
			}
		})
	},

	onAssociarItem: function (item) {

		var id = item.Id;
		var tipo = item.Tipo;
		var tipoTexto = item.TipoTexto;
		var nome = item.Nome;
		var condicionante = item.Condicionante;
		var procedimento = item.ProcedimentoAnalise;

		var tabItens = $('.tabItens', RoteiroSalvar.container);
		var trElem = $('tr', tabItens);

		$('.txtItemNome').removeClass('erroCampo');

		erroMsg = new Array();

		if (RoteiroSalvar.existeAssociado(item.Id + '', tabItens, 'hdnItemId')) {
			erroMsg.push(Mensagem.replace(RoteiroSalvar.Mensagens.ItemExistente, '#nome', nome));
		}

		if (erroMsg.length > 0) {
			return erroMsg;
		}

		var lastIndex = RoteiroSalvar.buscarUltimoIndice(tabItens);

		var linha = $('.trItemTemplate').clone().removeClass('trItemTemplate');

		linha.find('.hdnItemIndex').val(lastIndex).attr('name', 'Roteiro.Itens.Index');
		linha.find('.hdnItemOrdem').attr('name', 'Roteiro.Itens[' + lastIndex + '].Ordem').val(lastIndex);

		linha.find('.hdnItemId').val(id).attr('name', 'Roteiro.Itens[' + lastIndex + '].Id');

		linha.find('.hdnItemTipo').val(tipo).attr('name', 'Roteiro.Itens[' + lastIndex + '].Tipo');
		linha.find('.hdnItemNome').val(nome).attr('name', 'Roteiro.Itens[' + lastIndex + '].Nome');
		linha.find('.hdnItemCondicionante').val(nome).attr('name', 'Roteiro.Itens[' + lastIndex + '].Condicionante');

		linha.find('.hdnItemProcedimento').val(procedimento).attr('name', 'Roteiro.Itens[' + lastIndex + '].ProcedimentoAnalise');

		linha.find('.ItemTipo').html(tipoTexto);
		linha.find('.ItemTipo').attr('title', tipoTexto);
		linha.find('.ItemNome').html(nome);
		linha.find('.ItemNome').attr('title', nome);
		linha.find('.ItemCondicionante').html(condicionante);
		linha.find('.ItemCondicionante').attr('title', condicionante);
		linha.find('.ItemProcedimento').html(procedimento);

		$('tbody:last', tabItens).append(linha);

		RoteiroSalvar.reorganizarEstiloTab(tabItens);
		RoteiroSalvar.atualizaEstiloGrid('tabItens');
		return erroMsg;
	},

	onDescerLinhaTabClick: function () {
		var linha = $(this).closest('tr');
		var tab = linha.closest('table.dataGridTable tbody');
		var index = parseInt(linha.find('input[name$=Index]').val());

		if (index < parseInt($('tr:last-child', tab).find('input[name$=Index]').val())) {

			var linhaAtual = $('tr:nth-child(' + index + ')', tab);
			var linhaProx = $('tr:nth-child(' + (index + 1) + ')', tab);

			var aux = parseInt(linhaProx.find('input[name$=Index]').val());
			linhaProx.find('input[name$=Index]').val(parseInt(linhaAtual.find('input[name$=Index]').val()));
			linhaAtual.find('input[name$=Index]').val(aux);

			linhaAtual.insertAfter(linhaProx);
			RoteiroSalvar.reorganizarEstiloTab(tab);
		}
	},

	onSubirLinhaTabClick: function () {
		var linha = $(this).closest('tr');
		var tab = linha.closest('table.dataGridTable tbody');
		var index = parseInt(linha.find('input[name$=Index]').val());

		if (index > parseInt($('tr:first-child', tab).find('input[name$=Index]').val())) {

			var linhaAtual = $('tr:nth-child(' + index + ')', tab);
			var linhaAnt = $('tr:nth-child(' + (index - 1) + ')', tab);

			var aux = parseInt(linhaAnt.find('input[name$=Index]').val());
			linhaAnt.find('input[name$=Index]').val(parseInt(linhaAtual.find('input[name$=Index]').val()));
			linhaAtual.find('input[name$=Index]').val(aux);

			linhaAtual.insertBefore(linhaAnt);
			RoteiroSalvar.reorganizarEstiloTab(tab);
		}
	},

	reorganizarEstiloTab: function (tab) {
		$(tab).find('tr').each(function (i, linha) {
			$(linha).removeClass();
			$(linha).addClass((i % 2) === 0 ? 'par' : 'impar');
		});
	},

	reorganizarIndicesTab: function (tab) {
		$(tab).find('tr').each(function (i, linha) {
			$(linha).find('input[name$=Index]').val(i + 1);
		});
	},

	buscarUltimoIndice: function (tab) {
		var ultimoIndex = $(tab).find('tbody tr').length + 1;
		return ultimoIndex;
	},

	onRoteiroNovoClick: function () {
		RoteiroSalvar.trocaBotoes();
	},

	trocaBotoes: function () {
		$('.roteiroContent').show();
		$('.botoesSalvarCancelar').show(1);
		$('.botoesNovoCopiar').hide(1);
		$('.hdnCadastrandoItens').val("True");
		MasterPage.redimensionar();
	},

	onRoteiroCopiarAssociarClick: function () {
		Modal.abrir(RoteiroSalvar.urlAssociarRoteiroCopiar, null, function (container) {
			RoteiroListar.load(container, RoteiroSalvar.associarRoteiro);
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);
	},

	associarRoteiro: function (id, texto, callback) {

		var objKeyValue = { id: parseInt(id, 10) };
		var arrayMsg = null;


		$.ajax({ url: RoteiroSalvar.urlRoteiroCopiar, data: objKeyValue, cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (textStatus === "error") {

					if (XMLHttpRequest.status == 500 || XMLHttpRequest.status == 404 || XMLHttpRequest.status == 400) {
						var resp = $(response);
						content.empty();
						content.append(resp.find("h2"));
						content.append(resp.find("table"));
					}

					arrayMsg = [{ Tipo: 4, Texto: msg}];
				}
			},
			success: function (data, textStatus, XMLHttpRequest) {
				content = $('.roteiroContent', RoteiroSalvar.container).children();
				content.remove();
				$(data).appendTo('.roteiroContent', RoteiroSalvar.container);
				RoteiroSalvar.trocaBotoes();
				RoteiroSalvar.recarregarEventosClick(RoteiroSalvar.container);
				MasterPage.botoes(RoteiroSalvar.container);
				Mascara.load(RoteiroSalvar.container);
				MasterPage.redimensionar();
			}
		});
		Listar.atualizarEstiloTable(RoteiroSalvar.container.find('.dataGridTable'));
	},

	recarregarEventosClick: function (content) {
		$('.btnAddItem', content).click(RoteiroSalvar.onAdicionarItemClick);
		$('.btnAddPalavraChave', content).click(RoteiroSalvar.onAdicionarPalavraChaveClick);
	},

	existeAssociado: function (item, tab, itemClass) {
		var existe = false;

		var trs = $(tab).find('tbody tr');
		$.each(trs, function (key, trElem) {
			if ($(trElem).find('.' + itemClass) !== '') {
				var trItem = $(trElem).find('.' + itemClass).val();
				existe = (item.toLowerCase().trim() === trItem.toLowerCase().trim());
				if (existe) {
					return false;
				}
			}
		});
		return existe;
	},

	onExcluirLinha: function () {
		var tab = $(this).closest('table.dataGridTable tbody');

		if (tab.closest('table').hasClass('tabAnexos')) {
			RoteiroSalvar.atualizaEstiloGrid('tabAnexos');
			FileUpload.cancelar(JSON.parse($(this).closest('tr').find('.hdnAnexoArquivoJson').val()).Id);
		}

		$(this).closest('tr').remove();
		RoteiroSalvar.reorganizarEstiloTab(tab);
		RoteiroSalvar.reorganizarIndicesTab(tab);

		if (tab.closest('table').hasClass('tabItens')) {
			RoteiroSalvar.atualizaEstiloGrid('tabItens');
		}

		if (tab.find('tr').length < 1 && !tab.closest('table').hasClass('tabItens')) {
			tab.closest('table').addClass('hide');
			$('.lblGridVazio', tab.closest('fieldset')).removeClass('hide');
		}
	},

	onAdicionarItemClick: function () {
		var tipo = $('input.rdbItemTipo:checked').val();
		var nome = $('.txtItemNome').val().trim();
		var procedimento = $('.txtItemProcedimento').val();

		var tabItens = $('.tabItens', RoteiroSalvar.container);
		var trElem = $('tr', tabItens);

		$('.txtItemNome').removeClass('erroCampo');

		erroMsg = new Array();

		if (RoteiroSalvar.existeAssociado(nome, tabItens, 'hdnItemNome')) {
			erroMsg.push(RoteiroSalvar.Mensagens.ItemExistente);
		}

		if (erroMsg.length > 0) {
			Mensagem.gerar(MasterPage.getContent(RoteiroSalvar.container), erroMsg);
			return;
		}

		var lastIndex = RoteiroSalvar.buscarUltimoIndice(tabItens);

		var linha = $('.trItemTemplate').clone().removeClass('trItemTemplate');

		linha.find('.hdnItemIndex').val(lastIndex).attr('name', 'Roteiro.Itens.Index');
		linha.find('.hdnItemOrdem').attr('name', 'Roteiro.Itens[' + lastIndex + '].Ordem').val(lastIndex);
		linha.find('.hdnItemTipo').val(tipo).attr('name', 'Roteiro.Itens[' + lastIndex + '].Tipo');
		linha.find('.hdnItemNome').val(nome).attr('name', 'Roteiro.Itens[' + lastIndex + '].Nome');
		linha.find('.hdnItemCondicionante').val(nome).attr('name', 'Roteiro.Itens[' + lastIndex + '].Condicionante');
		linha.find('.hdnItemProcedimento').val(procedimento).attr('name', 'Roteiro.Itens[' + lastIndex + '].ProcedimentoAnalise');

		linha.find('.ItemTipo').html((tipo === '1') ? "Técnico" : "Administrativo");
		linha.find('.ItemNome').html(nome);
		linha.find('.ItemProcedimento').html(procedimento);

		$('tbody:last', tabItens).append(linha);

		$('.txtItemNome').val('');
		$('.txtItemProcedimento').val('');

		RoteiroSalvar.reorganizarEstiloTab(tabItens);
	},

	onAdicionarPalavraChaveClick: function () {
		var palavra = $('.txtPalavraChaveNome').val().trim();

		var tabPalavraChaves = $('.tabPalavraChaves', RoteiroSalvar.container);
		var trElem = $('tr', tabPalavraChaves);

		$('.txtPalavraChaveNome').removeClass('erroCampo');

		erroMsg = new Array();

		if (RoteiroSalvar.existeAssociado(palavra, tabPalavraChaves, 'hdnPalavraChaveNome')) {
			erroMsg.push(RoteiroSalvar.Mensagens.PalavraChaveExistente);
		}

		if (!palavra) {
			erroMsg.push(RoteiroSalvar.Mensagens.PalavraChaveObrigatoria);
		}

		if (erroMsg.length > 0) {
			Mensagem.gerar(MasterPage.getContent(RoteiroSalvar.container), erroMsg);
			return;
		}

		var lastIndex = RoteiroSalvar.buscarUltimoIndice(tabPalavraChaves);

		var linha = $('.trPalavraChavesTemplate').clone().removeClass('trPalavraChavesTemplate');

		linha.find('.hdnPalavraChaveIndex').val(lastIndex).attr('name', 'Roteiro.PalavraChaves.Index');
		linha.find('.hdnPalavraChaveId').val(0).attr('name', 'Roteiro.PalavraChaves[' + lastIndex + '].Id');
		linha.find('.hdnPalavraChaveNome').val(palavra).attr('name', 'Roteiro.PalavraChaves[' + lastIndex + '].Nome');

		linha.find('.PalavraChaveNome').html(palavra);
		linha.find('.PalavraChaveNome').attr('title', palavra);

		$('tbody:last', tabPalavraChaves).append(linha);
		tabPalavraChaves.removeClass('hide');
		$('.lblGridVazio', tabPalavraChaves.closest('fieldset')).addClass('hide');

		$('.txtPalavraChaveNome').val('');

		RoteiroSalvar.reorganizarEstiloTab(tabPalavraChaves);
	},

	//----------ANEXOS - ENVIAR ARQUIVO---------------

	onEnviarAnexoArquivoClick: function (url) {
		var nome = "enviando ...";
		var nomeArquivo = $('.inputFile').val();
		var descricao = $('.txtAnexoDescricao').val();
		var content = MasterPage.getContent(RoteiroSalvar.container);

		var tabAnexos = $('.tabAnexos', RoteiroSalvar.container);
		var trElem = $('tr', tabAnexos);

		erroMsg = new Array();

		if (nomeArquivo === '') {
			erroMsg.push(RoteiroSalvar.Mensagens.ArquivoAnexoObrigatorio);
		}

		if (descricao === '') {
			erroMsg.push(RoteiroSalvar.Mensagens.DescricaoAnexoObrigatorio);
		}

		if (nomeArquivo !== '' && descricao !== '') {
			if (RoteiroSalvar.existeAssociado(nomeArquivo, tabAnexos, "hdnArquivoNome")) {
				erroMsg.push(RoteiroSalvar.Mensagens.ArquivoExistente);
			}

			var tam = nomeArquivo.length - 4;

			if (nomeArquivo.toLowerCase().substr(tam) !== ".pdf") {
				erroMsg.push(RoteiroSalvar.Mensagens.ArquivoAnexoNaoEhPDF);
			}
		}

		if (erroMsg.length > 0) {
			$('.txtAnexoDescricao').addClass('erroCampo');
			Mensagem.gerar(MasterPage.getContent(RoteiroSalvar.container), erroMsg);
			return;
		}

		var lastIndex = RoteiroSalvar.buscarUltimoIndice(tabAnexos);
		var linha = $('.trAnexoTemplate').clone().removeClass('trAnexoTemplate');
		var id = "ArquivoId_" + lastIndex;

		linha.find('.hdnAnexoIndex').val(lastIndex).attr('name', 'Roteiro.Anexos.Index');
		linha.find('.hdnArquivoNome').val(nomeArquivo).attr('name', 'Roteiro.Anexos[' + lastIndex + '].Arquivo.Nome');
		linha.find('.hdnArquivoExtensao').val('').attr('name', 'Roteiro.Anexos[' + lastIndex + '].Extensao');
		linha.find('.hdnAnexoOrdem').val(lastIndex).attr('name', 'Roteiro.Anexos[' + lastIndex + '].Ordem');
		linha.find('.hdnAnexoArquivoJson').val(JSON.stringify({ Id: id })).attr('name', 'Roteiro.Anexos[' + lastIndex + '].ArquivoJson');
		linha.find('.hdnAnexoDescricao').val(descricao).attr('name', 'Roteiro.Anexos[' + lastIndex + '].Descricao');

		linha.find('.ArquivoNome').html(nome).attr('title', nome);
		linha.find('.AnexoDescricao').html(descricao).attr('title', descricao);

		$('tbody:last', tabAnexos).append(linha);
		tabAnexos.removeClass('hide');
		$('.lblGridVazio', tabAnexos.closest('fieldset')).addClass('hide');
		$('.txtArquivoNome, .txtAnexoDescricao').val('');

		var inputFile = $('.inputFileDiv input[type="file"]');
		inputFile.attr("id", id);
		RoteiroSalvar.atualizaEstiloGrid('tabAnexos');

		FileUpload.upload(url, inputFile, RoteiroSalvar.msgArqEnviado);
		$('.inputFile').val('');
	},

	msgArqEnviado: function (controle, retorno, isHtml) {
		var idCtr = controle.attr('id');
		var index = parseInt(idCtr.substr(10));
		var tr = $('.tabAnexos tbody tr:nth-child(' + index + ')');
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.ArquivoNome', tr).html(ret.Arquivo.Nome).attr('title', ret.Arquivo.Nome);
			$('.hdnArquivoNome', tr).val(ret.Arquivo.Nome);
			$('.hdnArquivoExtensao', tr).val(ret.Arquivo.Extensao);
			$('.hdnAnexoArquivoJson', tr).val(JSON.stringify(ret.Arquivo));
		} else {
			RoteiroSalvar.onLimparArquivoClick();
			tr.remove();
		}

		RoteiroSalvar.reorganizarEstiloTab($('.tabAnexos tbody'));
		$(".btnAddAnexoArquivo").show();
		Mensagem.gerar(MasterPage.getContent(RoteiroSalvar.container), ret.Msg);
	},

	onLimparArquivoClick: function () {
		//implementar Limpar

		$('.txtArquivoNome').data('arquivo', null);
		$('.txtArquivoNome').val("");
		$('.inputFileArquivo').val("");
		$('.hdnFileArquivo').val("");
	},

	// --- Salvar ---

	GerarObjeto: function () {

		var objeto = { Itens: [], Anexos: [], PalavraChaves: [], Atividades: [], Modelos: [], Finalidade: null };

		objeto.Id = $('.hdnRoteiroId').val();
		objeto.Numero = $('.txtNumero').val();
		objeto.Versao = $('.txtVersao').val();
		objeto.Nome = $('.txtNome').val();
		objeto.Setor = $('.ddlSetor').val();
		objeto.Situacao = $('.hdnSituacao').val();
		objeto.Observacoes = $('.txtObservacao').html();

		$('.tabItens').find('tbody tr').each(function (index, linha) {
			objeto.Itens.push({ Id: $(linha).find(".hdnItemId").val(),
				Tipo: $(linha).find(".hdnItemTipo").val(),
				Ordem: (index + 1),
				Nome: $(linha).find(".hdnItemNome").val(),
				ProcedimentoAnalise: $(linha).find(".hdnItemProcedimento").val()
			});
		});

		function Arquivo() {
			this.Nome = '';
			this.Extensao = '';
		};

		$('.tabAnexos').find('tbody tr').each(function (index, linha) {
			var arquivo = new Arquivo();
			arquivo.Nome = $(linha).find(".hdnArquivoNome").val();
			arquivo.Extensao = $(linha).find(".hdnArquivoExtensao").val();

			objeto.Anexos.push({ Ordem: (index + 1), Descricao: $(linha).find('.hdnAnexoDescricao').val(),
				ArquivoJson: $(linha).find(".hdnAnexoArquivoJson").val(), Arquivo: arquivo
			});
		});

		$('.tabPalavraChaves').find('tbody tr').each(function (index, linha) {
			objeto.PalavraChaves.push({ Id: $(linha).find(".hdnPalavraChaveId").val(), Nome: $(linha).find(".hdnPalavraChaveNome").val() });
		});

		$('.finalidades .finalidade').each(function (index, linha) {
			if ($(linha).find('.checkboxFinalidade').attr('checked')) {
				objeto.Finalidade += +$(linha).find('.checkboxFinalidade').val();
			}
		});

		RoteiroSalvar.AtualizarListaAtividades();
		RoteiroSalvar.AtualizarListaModelos();
		objeto.Atividades = RoteiroSalvar.listaAtividades;
		objeto.Modelos = RoteiroSalvar.listarModelos;

		return { Roteiro: objeto };
	},

	onRoteiroSalvarClick: function (modalRef) {

		MasterPage.carregando(true);
		if (typeof modalRef.context != 'undefined') {
			Modal.fechar(modalRef);
		}

		if (FileUpload.Count > 0) {
			MasterPage.carregando(false);
			Mensagem.gerar(MasterPage.getContent(RoteiroSalvar.container), new Array(RoteiroSalvar.Mensagens.TransferenciaAndamento));
			return false;
		}

		$.ajax({ url: RoteiroSalvar.urlCriarRoteiro,
			data: JSON.stringify(RoteiroSalvar.GerarObjeto()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(RoteiroSalvar.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					MasterPage.carregando(false);
					MasterPage.redireciona(response.urlRetorno);
				} else {
					Mensagem.gerar(RoteiroSalvar.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
		return false;
	},

	onRoteiroEditarClick: function () {
		var id = parseInt($('.hdnRoteiroId').val());

		Modal.confirma({
			btnOkCallback: RoteiroSalvar.onRoteiroSalvarClick,
			btnOkLabel: 'Alterar',
			url: RoteiroSalvar.urlConfirmarEditar + "/" + id,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	}
}

$(function () {
	RoteiroSalvar.load($('#central'));
});
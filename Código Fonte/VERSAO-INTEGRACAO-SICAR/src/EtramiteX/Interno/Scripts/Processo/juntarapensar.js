/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

JuntarApensar = {
	mensagens: null,
	container: null,
	urlLimpar: '',
	urlSalvar: '',
	urlSalvado: '',
	urlVerificar: '',
	urlVisualizarProcesso: '',
	urlVisualizarDocumento: '',
	urlJuntarDocumentoVerificar: '',
	urlApensarProcessoVerificar: '',
	container: '',

	load: function (container) {
		JuntarApensar.container = container;
		container.delegate('.btnVerificar', 'click', JuntarApensar.onBtnVerificarClick);
		container.delegate('.btnExcluir', 'click', JuntarApensar.onBtnExcluirClick);
		container.delegate('.btnLimpar', 'click', JuntarApensar.onBtnLimparClick);
		container.delegate('.tabProcessosApensados .btnVisualizar ', 'click', JuntarApensar.onBtnVisualizarProcessoClick);
		container.delegate('.tabDocumentosJuntados .btnVisualizar ', 'click', JuntarApensar.onBtnVisualizarDocumentoClick);
		container.delegate('.btnAdicionarDocumento', 'click', JuntarApensar.onBtnAdicionarDocumentoClick);
		container.delegate('.btnAdicionarProcesso', 'click', JuntarApensar.btnAdicionarProcessoClick);

		container.delegate('.txtProcessoNumero', 'keyup', function (e) {
			if (e.keyCode == 13) $('.btnVerificar', container).click();
		});
		
		$('.btnSalvar', JuntarApensar.container).click(JuntarApensar.onBtnSalvarClick);

		setTimeout(function () {
			$('input:visible:enabled:first', JuntarApensar.container).focus();
		}, 200);
	},

	onBtnVisualizarDocumentoClick: function () {
		var itemId = $('.hdnItemId', $(this).closest('tr')).val();
		Modal.abrir(JuntarApensar.urlVisualizarDocumento + '/' + itemId, null, function (container) {
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);
	},

	onBtnVisualizarProcessoClick: function () {
		var itemId = $('.hdnItemId', $(this).closest('tr')).val();
		Modal.abrir(JuntarApensar.urlVisualizarProcesso + '/' + itemId, null, function (container) {
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);
	},

	onBtnExcluirClick: function () {
		var table = $(this).closest('table');
		$(this).closest('tr').remove();
		if ($('tbody tr:visible', table).length <= 0) {
			$('.trSemItens', table).removeClass('hide');
		}
	},

	onBtnVerificarClick: function () {
		var numeroProcesso = $('.txtProcessoNumero', JuntarApensar.container).val();
		MasterPage.carregando(true);

		$.ajax({ url: JuntarApensar.urlVerificar, data: { numero: numeroProcesso }, cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				Mensagem.limpar(JuntarApensar.container);

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(JuntarApensar.container, response.Msg);
				} else {
					$('.juntarApensarPartialContainer', JuntarApensar.container).html(response.Html);
					$('.ouContainer', JuntarApensar.container).removeClass('hide');
					$('.btnSalvar', JuntarApensar.container).removeClass('hide');
					MasterPage.redimensionar();
					MasterPage.load();
					$('input:visible:enabled:first', JuntarApensar.container).focus();

					Listar.atualizarEstiloTable($('.dataGridTable', JuntarApensar.container));
				}
			}
		});
		MasterPage.carregando(false);
	},

	onBtnLimparClick: function () {
		MasterPage.redireciona(JuntarApensar.urlLimpar);
	},

	onBtnAdicionarDocumentoClick: function () {
		var procIdVal = $('.hdnProcessoId', JuntarApensar.container).val();
		var docNumero = $('.txtNovoDocumentoNumero', JuntarApensar.container).val();

		var existeDocumento = false;
		$('.tabDocumentosJuntados tbody tr:visible', JuntarApensar.container).each(function () {
			if (docNumero == $('.hdnItemNumero', this).val()) {
				existeDocumento = true;
				return;
			}
		});

		if (existeDocumento) {
			Mensagem.gerar(JuntarApensar.container, new Array(JuntarApensar.mensagens.DocumentoJaListadoJuntar));
			return;
		}

		// valida se número existe, se é de formato válido, se tipo escolhido, se existe processo, se já não está apensado, etc..
		var retorno = MasterPage.validarAjax(JuntarApensar.urlJuntarDocumentoVerificar, { procId: procIdVal, numero: docNumero }, JuntarApensar.container, false);
		if (retorno.Msg && retorno.Msg.length > 0) {
			Mensagem.gerar(JuntarApensar.container, retorno.Msg);
			return;
		}

		if (retorno.EhValido) {
			var doc = retorno.ObjResponse.doc;
			$('.tabDocumentosJuntados tbody .trSemItens', JuntarApensar.container).addClass('hide');

			var novoTr = $('.tabDocumentosJuntados tbody .trTemplate', JuntarApensar.container).clone().removeClass('hide trTemplate');
			$('.tabDocumentosJuntados tbody', JuntarApensar.container).append(novoTr);

			$('.hdnItemId', novoTr).val(doc.Id);
			$('.hdnItemNumero', novoTr).val(doc.Numero);

			$('.docNum', novoTr).text(doc.Numero).attr('title', doc.Numero);
			$('.docTipo', novoTr).text(doc.Tipo.Texto).attr('title', doc.Tipo.Texto);
			$('.docNome', novoTr).text(doc.Nome).attr('title', doc.Nome);

			$('.txtNovoDocumentoNumero', JuntarApensar.container).val('');

			MasterPage.redimensionar();

			Listar.atualizarEstiloTable(JuntarApensar.container.find('.dataGridTable'));
		}
	},

	btnAdicionarProcessoClick: function () {
		var procIdVal = $('.hdnProcessoId', JuntarApensar.container).val();
		var procNumero = $('.txtNovoProcessoNumero', JuntarApensar.container).val();

		// processo já adicionado
		var existeProcesso = false;
		$('.tabProcessosApensados tr:visible', JuntarApensar.container).each(function () {
			if ($('.hdnItemNumero', this).val() == procNumero) {
				existeProcesso = true;
				return false;
			}
		});

		if (existeProcesso) {
			Mensagem.gerar(JuntarApensar.container, new Array(JuntarApensar.mensagens.ProcessoJaListadoJuntar));
			return;
		}

		// valida se número existe, se é de formato válido, se tipo escolhido, se existe processo, se já não está apensado, etc..
		var retorno = MasterPage.validarAjax(JuntarApensar.urlApensarProcessoVerificar, { procId: procIdVal, numero: procNumero }, JuntarApensar.container, false);
		if (retorno.Msg && retorno.Msg.length > 0) {
			Mensagem.gerar(JuntarApensar.container, retorno.Msg);
			return;
		}

		if (retorno.EhValido) {
			var proc = retorno.ObjResponse.proc;
			$('.tabProcessosApensados .trSemItens', JuntarApensar.container).addClass('hide');

			var novoTr = $('.tabProcessosApensados .trTemplate', JuntarApensar.container).clone().removeClass('hide trTemplate');
			$('.tabProcessosApensados tbody', JuntarApensar.container).append(novoTr);

			$('.hdnItemId', novoTr).val(proc.Id);
			$('.hdnItemNumero', novoTr).val(proc.Numero);

			$('.procNum', novoTr).text(proc.Numero).attr('title', proc.Numero);
			$('.procTipo', novoTr).text(proc.Tipo.Texto).attr('title', proc.Tipo.Texto);

			if (proc.Empreendimento.Denominador != null) {
				$('.procEmp', novoTr).text(proc.Empreendimento.Denominador).attr('title', proc.Empreendimento.Denominador);
			}
			$('.txtNovoProcessoNumero', JuntarApensar.container).val('');

			MasterPage.redimensionar();

			Listar.atualizarEstiloTable(JuntarApensar.container.find('.dataGridTable'));
		}
	},

	onBtnSalvarClick: function () {
		var JuntarApensarVM = {
			Processo: {
				Id: $('.hdnProcessoId', JuntarApensar.container).val(),
				Processos: [],
				Documentos: []
			}
		};

		$('.tabDocumentosJuntados tbody .trDocumento:visible', JuntarApensar.container).each(function () {
			JuntarApensarVM.Processo.Documentos.push({
				Id: parseInt($('.hdnItemId', this).val()),
				Numero: $('.hdnItemNumero', this).val()
			});
		});

		$('.tabProcessosApensados tbody .trProcesso:visible', JuntarApensar.container).each(function () {
			JuntarApensarVM.Processo.Processos.push({
				Id: parseInt($('.hdnItemId', this).val()),
				Numero: $('.hdnItemNumero', this).val()
			});
		});

		$.ajax({ url: JuntarApensar.urlSalvar,
			data: JSON.stringify(JuntarApensarVM),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				MasterPage.carregando(false);
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				Mensagem.limpar(JuntarApensar.container);

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(JuntarApensar.container, response.Msg);
				} else {
					if (response.UrlRedireciona) {
						MasterPage.redireciona(response.UrlRedireciona);
					}
				}
				MasterPage.carregando(false);
			}
		});
	}
}
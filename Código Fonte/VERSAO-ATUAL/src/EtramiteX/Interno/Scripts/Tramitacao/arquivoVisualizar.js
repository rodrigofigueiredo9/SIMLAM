/// <reference path="../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../Lib/JQuery/jquery.json - 2.2.min.js" />
/// <reference path="../masterpage.js" />

TramitacaoArquivoVisualizar = {
	settings: {
		container: null,
		urls: null,
		Mensagens: null,
		id: 0
	},
	load: function (context, options) {
		if (options) {
			$.extend(TramitacaoArquivo.settings, options);
		}
		TramitacaoArquivo.settings.container = context;

		$(".btnTramitacaoArqSalvar", TramitacaoArquivo.settings.container).click(TramitacaoArquivo.onSalvar);
		$('.btnExcluirTramitacaoArquivoEstanteItem', TramitacaoArquivo.settings.container).live('click', TramitacaoArquivo.onExcluirEstanteItem);
		$('.btnExcluirTramitacaoArquivoPrateleiraItem', TramitacaoArquivo.settings.container).live('click', TramitacaoArquivo.onExcluirPrateleiraItem);
		$('#btnAddTramitacaoArquivoEstante', TramitacaoArquivo.settings.container).live('click', TramitacaoArquivo.onAddEstanteItem);
		$('#btnAddTramitacaoArquivoPrateleira', TramitacaoArquivo.settings.container).live('click', TramitacaoArquivo.onAddPrateleiraItem);
	},
	onSalvar: function () {

		var msg = TramitacaoArquivo.validarCampos();

		if (msg.length > 0) {
			TramitacaoArquivo.showMsg(msg);
			return;
		}

		var TramitacaoArquivoEntidade = {
			Id: TramitacaoArquivo.settings.id,
			Nome: $.trim($('.nomeArquivo', TramitacaoArquivo.settings.container).val()),
			SetorId: $('.setorArquivo', TramitacaoArquivo.settings.container).val(),
			TipoId: $('.tipoArquivo', TramitacaoArquivo.settings.container).val(),
			Estantes: TramitacaoArquivo.getItensGrid('#tabTramitacaoArquivoEstante'),
			Prateleiras: TramitacaoArquivo.getItensGrid('#tabTramitacaoArquivoPrateleira'),
			ProcessoSituacao: TramitacaoArquivo.getSituacaoDocProc('#processoSituacao'),
			DocumentoSituacao: TramitacaoArquivo.getSituacaoDocProc('#documentoSituacao')
		};

		MasterPage.carregando(true);

		$.ajax({ url: TramitacaoArquivo.settings.urls.salvar,
			data: JSON.stringify(TramitacaoArquivoEntidade),
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.IsArquivoSalvo) {
					MasterPage.redireciona(response.UrlRedireciona);
					return;
				}
				else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(TramitacaoArquivo.settings.container), response.Msg);
				}

				MasterPage.carregando(false);
			}
		});
	},
	onAddtrTemplateRowTramitacaoArquivo: function (estanteOuPrateleira, nome) {
		var item = {
			Texto: $.trim(nome)
		};
		Mensagem.limpar(MasterPage.getContent(TramitacaoArquivo.settings.container));
		var ultimoIndex = $('#tabTramitacaoArquivo' + estanteOuPrateleira + ' tbody tr', TramitacaoArquivo.settings.container).length;
		var rowTr = $('#trTemplateRowTramitacaoArquivo', TramitacaoArquivo.settings.container).clone();
		rowTr.removeAttr('id');
		rowTr.addClass((ultimoIndex % 2) === 0 ? 'par' : 'impar');
		rowTr.find('.TramitacaoArquivoItem').attr('class', 'TramitacaoArquivo' + estanteOuPrateleira + 'Item').html(nome);
		rowTr.find('.hdnTramitacaoArquivoItem').attr('class', 'hdnTramitacaoArquivo' + estanteOuPrateleira + 'Item').val(JSON.stringify(item));
		rowTr.find('.btnExcluirTramitacaoArquivotem').removeClass('btnExcluirTramitacaoArquivotem').addClass('btnExcluirTramitacaoArquivo' + estanteOuPrateleira + 'Item');
		$('#tabTramitacaoArquivo' + estanteOuPrateleira, TramitacaoArquivo.settings.container).append(rowTr);
	},
	onAddEstanteItem: function () {

		if ($.trim($('.txtTramitacaoArquivoEstante', TramitacaoArquivo.settings.container).val()) == '') {
			var arrayMsgTramitacaoArquivo = new Array();
			arrayMsgTramitacaoArquivo.push(TramitacaoArquivo.settings.Mensagens.EstanteItemArquivoObrigratorio);
			TramitacaoArquivo.showMsg(arrayMsgTramitacaoArquivo);
			return;
		}

		var isRepetido = false;

		$('#tabTramitacaoArquivoEstante tbody tr td span', TramitacaoArquivo.settings.container).each(function () {
			if ($.trim($(this).html().toLowerCase()) === $.trim($('.txtTramitacaoArquivoEstante', TramitacaoArquivo.settings.container).val().toLowerCase()) && !isRepetido) {
				isRepetido = true;
				return;
			}
		});

		if (isRepetido) {
			var arrayMsgTramitacaoArquivo = [TramitacaoArquivo.settings.Mensagens.EstanteItemArquivoJaAdicionada];
			TramitacaoArquivo.showMsg(arrayMsgTramitacaoArquivo);
			return;
		}

		TramitacaoArquivo.onAddtrTemplateRowTramitacaoArquivo('Estante', $('.txtTramitacaoArquivoEstante', TramitacaoArquivo.settings.container).val());
		$('.txtTramitacaoArquivoEstante', TramitacaoArquivo.settings.container).val('').removeClass('erroCampo');

	},
	onAddPrateleiraItem: function () {

		if ($.trim($('.txtTramitacaoArquivoPrateleira', TramitacaoArquivo.settings.container).val()) == '') {
			var arrayMsgTramitacaoArquivo = new Array();
			arrayMsgTramitacaoArquivo.push(TramitacaoArquivo.settings.Mensagens.PrateleiraItemArquivoObrigratorio);
			TramitacaoArquivo.showMsg(arrayMsgTramitacaoArquivo);
			return;
		}

		var isRepetido = false;

		$('#tabTramitacaoArquivoPrateleira tbody tr td span', TramitacaoArquivo.settings.container).each(function () {
			if ($.trim($(this).html().toLowerCase()) === $.trim($('.txtTramitacaoArquivoPrateleira', TramitacaoArquivo.settings.container).val().toLowerCase()) && !isRepetido) {
				isRepetido = true;
				return;
			}
		});

		if (isRepetido) {
			var arrayMsgTramitacaoArquivo = [TramitacaoArquivo.settings.Mensagens.PrateleiraItemArquivoJaAdicionada];
			TramitacaoArquivo.showMsg(arrayMsgTramitacaoArquivo);
			return;
		}

		TramitacaoArquivo.onAddtrTemplateRowTramitacaoArquivo('Prateleira', $('.txtTramitacaoArquivoPrateleira', TramitacaoArquivo.settings.container).val());
		$('.txtTramitacaoArquivoPrateleira', TramitacaoArquivo.settings.container).val('').removeClass('erroCampo');
	},
	onExcluirEstanteItem: function () {
		TramitacaoArquivo.removeItemTr($(this));
		TramitacaoArquivo.colorirGrid("#tabTramitacaoArquivoEstante");
	},
	onExcluirPrateleiraItem: function () {
		TramitacaoArquivo.removeItemTr($(this));
		TramitacaoArquivo.colorirGrid("#tabTramitacaoArquivoPrateleira");
	},
	removeItemTr: function (trDom) {
		trDom.closest("tr").remove();
	},
	colorirGrid: function (tableControl) {
		$(tableControl, TramitacaoArquivo.settings.container).find(".impar").removeClass("impar");
		$(tableControl, TramitacaoArquivo.settings.container).find(".par").removeClass("par");
		$(tableControl + " tr:even", TramitacaoArquivo.settings.container).addClass("impar");
		$(tableControl + " tr:odd", TramitacaoArquivo.settings.container).addClass("par");
	},
	showMsg: function (arrayMsg) {
		Mensagem.gerar(TramitacaoArquivo.settings.container, arrayMsg);
	},
	getItensGrid: function (idTableControl) {
		var itens = new Array();
		$(idTableControl + ' tbody tr td input[type="hidden"]', TramitacaoArquivo.settings.container).each(function () {
			//itens.push({ Texto: $(this).val() });
			var item = JSON.parse($(this).val());
			itens.push(item);
		});
		return itens;
	},
	getSituacaoDocProc: function (contexto) {
		var countSituacao = 0;
		$(contexto, TramitacaoArquivo.settings.container).find('input[type="checkbox"]:checked').each(function () {
			countSituacao = countSituacao + parseInt($(this).val());
		});
		return countSituacao;
	},
	validarCampos: function () {
		var msgArray = new Array();

		if ($.trim($('.nomeArquivo', TramitacaoArquivo.settings.container).val()) == '') {
			msgArray.push(TramitacaoArquivo.settings.Mensagens.NomeArquivoObrigratorio);
		}

		if ($('.setorArquivo', TramitacaoArquivo.settings.container).val() == 0) {
			msgArray.push(TramitacaoArquivo.settings.Mensagens.SetorArquivoObrigratorio);
		}

		if ($('.tipoArquivo', TramitacaoArquivo.settings.container).val() == 0) {
			msgArray.push(TramitacaoArquivo.settings.Mensagens.TipoArquivoObrigratorio);
		}

		if ($('#tabTramitacaoArquivoEstante tbody tr', TramitacaoArquivo.settings.container).length == 0) {
			msgArray.push(TramitacaoArquivo.settings.Mensagens.EstanteArquivoObrigratorio);
		}

		if ($('#tabTramitacaoArquivoPrateleira tbody tr', TramitacaoArquivo.settings.container).length == 0) {
			msgArray.push(TramitacaoArquivo.settings.Mensagens.PrateleiraArquivoObrigratorio);
		}

		var countCkProcesso = $('#processoSituacao', TramitacaoArquivo.settings.container).find('.ckProcesso:checked').length;
		var countCkDocumento = $('#documentoSituacao', TramitacaoArquivo.settings.container).find('.ckDocumento:checked').length;

		if (countCkProcesso == 0 && countCkDocumento == 0) {
			msgArray.push(TramitacaoArquivo.settings.Mensagens.ProcessoOuDocumentoArqObrigratorio);
		}

		return msgArray;
	}
}
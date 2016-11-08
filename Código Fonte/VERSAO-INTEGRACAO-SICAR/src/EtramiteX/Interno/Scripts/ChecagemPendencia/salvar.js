/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ChecagemPendencia = {
	container: null,
	settings: {
		urls: {
			pdfTitulo: '',
			pdfPendencia: '',
			associarTitulo: '',
			associarTituloModal: '',
			salvar: ''
		},
		situacoes: new Array({ Id: 1, Texto: 'Não conferido' }, { Id: 2, Texto: 'Conferido' }),
		mensagens: {}
	},

	load: function (container) {
		ChecagemPendencia.container = container;
		$('.btnAssociarTitulo', container).click(ChecagemPendencia.onBtnAssociarTituloClick);
		container.delegate('.btnMarcadorItemRoteiro', 'click', ChecagemPendencia.onMarcadorItemRoteiroClick);
		$('.btnPdfTitulo').click(ChecagemPendencia.gerarPdfTitulo);
		$('.btnGerarPdfPendencia').click(ChecagemPendencia.gerarPdfPendencia);
		$('.btnSalvar').click(ChecagemPendencia.onBntSalvarClick);
		$('.btnAssociarTitulo', container).focus();
	},

	gerarPdfPendencia: function () {
		var checagem = ChecagemPendencia.obterChecagemPendenciaObj();
		Aux.download(ChecagemPendencia.settings.urls.pdfPendencia, checagem);
	},

	obterChecagemPendenciaObj: function () {
		var checagem = {
			TituloId: $('.hdnTituloId', ChecagemPendencia.container).val(),
			TituloNumero: $('.txtTituloNumero', ChecagemPendencia.container).val(),
			TituloTipoSigla: $('.hdnTituloTipo', ChecagemPendencia.container).val(),
			TituloVencimento: { DataTexto: $('.hdnTituloVencimento', ChecagemPendencia.container).val() },
			InteressadoNome: $('.txtInteressadoNome', ChecagemPendencia.container).val(),
			ProtocoloNumero: $('.txtProcessoNumero', ChecagemPendencia.container).val(),
			Itens: []
		};

		$('.trCheckItem').each(function () {
			tr = $(this);
			checagem.Itens.push({
				Id: parseInt(tr.find('.hdnId').val()) || 0,
				Nome: tr.find('.spnItemNome').text(),
				Tid: tr.find('.hdnTid').val() || '',
				IdRelacionamento: parseInt(tr.find('.hdnIdRelacionamento').val()) || 0,
				SituacaoId: parseInt(tr.find('.hdnItemSituacaoId').val()) || 0
			});
		});

		return checagem;
	},

	onBntSalvarClick: function () {

		var checagemPendenciaObj = ChecagemPendencia.obterChecagemPendenciaObj();

		$.ajax({ url: ChecagemPendencia.settings.urls.salvar,
			data: JSON.stringify({ checagemPendencia: checagemPendenciaObj }),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ChecagemPendencia.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (!response.EhValido && response.Msg != null && response.Msg.length > 0) {
					Mensagem.gerar($(ChecagemPendencia.container), response.Msg);
					return;
				} else {
					MasterPage.redireciona(response.UrlRedireciona);
				}
			}
		});
	},

	onMarcadorItemRoteiroClick: function () {
		var tr = $(this).closest('tr');
		var novaSituacao = $(this).hasClass('btnNaoConferido') ? 1 : 2;
		tr.find('.hdnItemSituacaoId').val(novaSituacao);
		tr.find('.btnNaoConferido').toggleClass('desativado', novaSituacao == 1);
		tr.find('.btnConferido').toggleClass('desativado', novaSituacao == 2);
		tr.find('.trItemRoteiroSituacaoTexto').text(novaSituacao == 2 ? ChecagemPendencia.settings.situacoes[1].Texto : ChecagemPendencia.settings.situacoes[0].Texto);
		tr.find('.trItemRoteiroSituacaoTexto').attr('title', novaSituacao == 2 ? ChecagemPendencia.settings.situacoes[1].Texto : ChecagemPendencia.settings.situacoes[0].Texto);

		$('.btnGerarPdfPendencia', ChecagemPendencia.container).attr('disabled', 'disabled').button({ disabled: true });
		$(this).closest('tbody').find('tr').each(function (i, linha) {
			if ($(linha).find('.hdnItemSituacaoId').val() == 1) {
				$('.btnGerarPdfPendencia', ChecagemPendencia.container).removeAttr('disabled').button({ disabled: false });
			}
		});
	},

	gerarPdfTitulo: function () {
		var itemId = parseInt($('.hdnTituloId', ChecagemPendencia.container).val()) || 0;
		if (itemId > 0) {
			MasterPage.redireciona(ChecagemPendencia.settings.urls.pdfTitulo + '/' + itemId);
		}
	},

	onBtnAssociarTituloClick: function () {
		Modal.abrir(ChecagemPendencia.settings.urls.associarTituloModal, null, function (container) {
			Modal.defaultButtons(container);
			TituloListar.load(container, { associarFuncao: ChecagemPendencia.onAssociarTitulo });
		}, Modal.tamanhoModalGrande);
	},

	onAssociarTitulo: function (titulo) {
		var resposta = { EhValido: null, Msg: [], FecharModal: false };

		$.ajax({ url: ChecagemPendencia.settings.urls.associarTitulo,
			data: JSON.stringify({ tituloId: titulo.Id }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ChecagemPendencia.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				resposta.EhValido = response.EhValido;

				if (!response.EhValido) {
					resposta.Msg = response.Msg;
				} else {
					$('.txtTituloNumero', ChecagemPendencia.container).val(response.ChecagemPendencia.TituloNumero);
					$('.txtProcessoNumero', ChecagemPendencia.container).val(response.ChecagemPendencia.ProtocoloNumero);
					$('.txtInteressadoNome', ChecagemPendencia.container).val(response.ChecagemPendencia.InteressadoNome);

					$('.hdnTituloId', ChecagemPendencia.container).val(titulo.Id);
					$('.hdnTituloTipo', ChecagemPendencia.container).val(response.ChecagemPendencia.TituloTipoSigla);
					$('.hdnTituloVencimento', ChecagemPendencia.container).val(response.ChecagemPendencia.TituloVencimento.DataTexto);

					$('.dgItens tbody', ChecagemPendencia.container).html(response.HtmlItens);

					$('.btnSalvar', ChecagemPendencia.container)
						.removeClass('disabled')
						.removeAttr('disabled')
						.button('enable');

					$('.spnPdfTitulo', ChecagemPendencia.container).removeClass('hide');
					$('.btnGerarPdfPendencia', ChecagemPendencia.container).removeClass('hide');
				}
			}
		});
		resposta.FecharModal = true;
		if (resposta.Msg && resposta.Msg.length > 0) {
			resposta.FecharModal = false;
		}

		Listar.atualizarEstiloTable(ChecagemPendencia.container.find('.dataGridTable'));

		return resposta;
	}
}
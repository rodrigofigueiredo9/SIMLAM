/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ReceberExterno = {
	settings: {
		urls: {
			receberFiltrar: '',
			receberSalvar: '',
			receberSucesso: '',
			visualizarHistorico: '',
			abrirPdf: ''
		},
		Mensagens: {}
	},
	container: {},

	load: function (content, options) {
		if (options) {
			$.extend(ReceberExterno.settings, options);
		}

		ReceberExterno.container = MasterPage.getContent(content);
		$('.btnReceber', content).click(ReceberExterno.onBtnReceberClick);

		content.delegate('.ddlOrgaoExterno', 'change', ReceberExterno.onDdlOrgaoExternoChange);
		content.delegate('.ckbCheckAllInMyColumn', 'change', ReceberExterno.onCkbCheckAllInMyColumnChange);
		content.delegate('.ckbSelecionavel', 'change', ReceberExterno.onCkbSelecionavelChange);
		content.delegate('.trTramitacao', 'click', ReceberExterno.onTrTramitacaoClick);
		content.delegate('.btnHistorico', 'click', ReceberExterno.visualizarHistoricoClick);
		content.delegate('.btnPdf', 'click', ReceberExterno.abrirPdfClick);
		content.delegate('.ddlSetorDestinatarioId', 'change', ReceberExterno.onSetorChange);

		$('.ddlSetorDestinatarioId', ReceberExterno.container).focus();
		ReceberExterno.marcarTodos();
	},

	onSetorChange: function () {
		Mensagem.limpar(ReceberExterno.container);
		ReceberExterno.setarBotaoReceber();
		$('.mostrarSeTiverSetor', ReceberExterno.container).toggleClass('hide', parseInt($(this).val()) <= 0);
	},

	visualizarHistoricoClick: function () {
		var id = parseInt($('.hdnProtocoloId', $(this).closest('tr')).val());
		var tipo = parseInt($('.hdnIsProcesso', $(this).closest('tr')).val());

		Modal.abrir(ReceberExterno.settings.urls.visualizarHistorico, { id: id, tipo: tipo }, function (context) {
			Modal.defaultButtons(context);
		});
	},

	abrirPdfClick: function () {
		var id = isNaN(parseInt($('.hdnTramitacaoId', $(this).closest('tr')).val())) ? 0 : parseInt($('.hdnTramitacaoId', $(this).closest('tr')).val());
		var tipo = parseInt($('.hdnIsProcesso', $(this).closest('tr')).val());
		if (id > 0) {
			MasterPage.redireciona(ReceberExterno.settings.urls.abrirPdf + "?id=" + id + '&tipo=' + tipo + "&obterHistorico=" + false);
			MasterPage.carregando(false);
		}
	},

	onCkbSelecionavelChange: function (e) {
		$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
		ReceberExterno.marcarTodos();
	},

	onCkbCheckAllInMyColumnChange: function (e) {
		$('tbody .ckbSelecionavel', $(this).closest('.tabTramitacoesRecebidas')).attr('checked', $(this).is(':checked'));
		$('tbody .ckbSelecionavel', $(this).closest('.tabTramitacoesRecebidas')).each(function () {
			$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
		});
	},

	onTrTramitacaoClick: function (e) {
		if ($(e.target).is('input')) return;
		$('.ckbSelecionavel', this).attr('checked', !$('.ckbSelecionavel', this).is(':checked'));
		$(this).toggleClass('linhaSelecionada', $('.ckbSelecionavel', this).is(':checked'));
		ReceberExterno.marcarTodos();
	},

	marcarTodos: function () {
		var isAllChk = true;
		$('.tabTramitacoesRecebidas tbody', ReceberExterno.container).find("tr").each(function (idx, item) {
			isAllChk = isAllChk && $('.ckbSelecionavel', item).is(':checked');
		});
		$('.tabTramitacoesRecebidas .ckbCheckAllInMyColumn', ReceberExterno.container).attr('checked', isAllChk);
	},

	setarBotaoReceber: function () {
		if ($('.ddlSetorDestinatarioId', ReceberExterno.container).val() != 0 &&
			$('.ddlOrgaoExterno', ReceberExterno.container).val() != 0 &&
			$('.tabTramitacoesRecebidas tbody tr', ReceberExterno.container).length <= 0) {
			$('.btnReceber', ReceberExterno.container).button({ disabled: true });
		} else {
			$('.btnReceber', ReceberExterno.container).button({ disabled: false });
		}
	},

	onDdlOrgaoExternoChange: function () {
		var id = parseInt($('.ddlOrgaoExterno', ReceberExterno.container).val());
		var orgaoTexto = $('.ddlOrgaoExterno option:selected', ReceberExterno.container).text();
		Mensagem.limpar(ReceberExterno.container);

		MasterPage.carregando(true);
		$.ajax({
			url: ReceberExterno.settings.urls.receberFiltrar,
			data: { orgaoId: id, orgaoTexto: orgaoTexto },
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ReceberExterno.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ReceberExterno.container, response.Msg);
				}

				if (response.EhValido) {
					$('.divTramitacoesReceber', ReceberExterno.container).html(response.HtmlTramitacoes);
				}

				ReceberExterno.setarBotaoReceber();
				MasterPage.redimensionar();
			}
		});
		MasterPage.carregando(false);
	},

	onBtnReceberClick: function () {
		Mensagem.limpar(ReceberExterno.container);

		var ReceberVM = {
			OrgaoExterno: { Id: parseInt($('.ddlOrgaoExterno', ReceberExterno.container).val()) },
			SetorDestinatario: { Id: parseInt($('.ddlSetorDestinatarioId', ReceberExterno.container).val()) },
			TramitacoesJson: []
		};

		$('.tabTramitacoesRecebidas tbody tr:visible').each(function () {
			if ($('.ckbSelecionavel', this).is(':checked')) {
				ReceberVM.TramitacoesJson.push($('.hdnTramitacaoJson', this).val());
			}
		});

		MasterPage.carregando(true);
		$.ajax({ url: ReceberExterno.settings.urls.receberSalvar,
			data: JSON.stringify(ReceberVM),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ReceberExterno.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedireciona);
				} else {
					Mensagem.gerar(ReceberExterno.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
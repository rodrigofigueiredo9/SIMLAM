/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

Receber = {
	settings: {
		urls: {
			receberFiltrar: '',
			receberSalvar: '',
			receberSucesso: '',
			visualizarHistorico: '',
			abrirPdf: ''
		},
		msgs: {}
	},
	container: {},

	load: function (content, options) {
		if (options) {
			$.extend(Receber.settings, options);
		}

		Receber.container = MasterPage.getContent(content);

		$('.ddlSetorDestinatarioId', content).change(Receber.onDdlSetorDestinatarioIdChange);
		content.delegate('.ckbCheckAllInMyColumn', 'change', Receber.onCkbCheckAllInMyColumnChange);
		content.delegate('.ckbSelecionavel', 'change', Receber.onCkbSelecionavelChange);
		content.delegate('.trTramitacao', 'click', Receber.onTrTramitacaoClick);
		$('.btnReceber', content).click(Receber.onBtnReceberClick);
		content.delegate('.btnHistorico', 'click', Receber.visualizarHistoricoDaTramitacaoClick);
		content.delegate('.btnPdf', 'click', Receber.abrirPdfClick);

		$('.ddlSetorDestinatarioId', content).focus();
		Receber.marcarTodos();
	},

	visualizarHistoricoDaTramitacaoClick: function () {
		var id = parseInt($('.hdnProtocoloId', $(this).closest('tr')).val());
		var tipo = parseInt($('.hdnIsProcesso', $(this).closest('tr')).val());

		Modal.abrir(Receber.settings.urls.visualizarHistorico, { id: id, tipo: tipo }, function (context) {
			Modal.defaultButtons(context);
		});
	},

	abrirPdfClick: function () {
		var id = isNaN(parseInt($('.hdnTramitacaoId', $(this).closest('tr')).val())) ? 0 : parseInt($('.hdnTramitacaoId', $(this).closest('tr')).val());
		var tipo = parseInt($('.hdnIsProcesso', $(this).closest('tr')).val());
		if (id > 0) {
			MasterPage.redireciona(Receber.settings.urls.abrirPdf + "?id=" + id + '&tipo=' + tipo + "&obterHistorico=" + false);
			MasterPage.carregando(false);
		}
	},

	onTrTramitacaoClick: function (e) {
		if ($(e.target).is('input')) return;
		$('.ckbSelecionavel', this).attr('checked', !$('.ckbSelecionavel', this).is(':checked'));
		$(this).toggleClass('linhaSelecionada', $('.ckbSelecionavel', this).is(':checked'));
		Receber.marcarTodos();
	},

	onCkbCheckAllInMyColumnChange: function (e) {
		$('tbody .ckbSelecionavel', $(this).closest('.tabTramitacoesRecebidas')).attr('checked', $(this).is(':checked'));
		$('tbody .ckbSelecionavel', $(this).closest('.tabTramitacoesRecebidas')).each(function () {
			$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
		});
	},

	onCkbSelecionavelChange: function () {
		$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
		Receber.marcarTodos();
	},

	marcarTodos: function () {
		$('.tabTramitacoesRecebidas', Receber.container).each(function (idx, tabela) {
			var linhas = $(tabela).find("tbody tr");
			var isAllChk = true;

			if (linhas.length > 0) {
				linhas.each(function (idx, item) {
					isAllChk = isAllChk && $('.ckbSelecionavel', item).is(':checked');
				});
				$(tabela).find('.ckbCheckAllInMyColumn').attr('checked', isAllChk);
			}
		});
	},

	onDdlSetorDestinatarioIdChange: function () {
		Mensagem.limpar(Receber.container);
		
		var id = parseInt($('.ddlSetorDestinatarioId', Receber.container).val());

		if (id == 0) {
			$('.mostrarSeSetorSelecionado', Receber.container).addClass('hide');
			$('.btnReceber', Receber.container).button({ disabled: false });
			return;
		}

		MasterPage.carregando(true);
		$.ajax({
			url: Receber.settings.urls.receberFiltrar,
			data: { setorId: id },
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
			    
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Receber.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Receber.container, response.Msg);
				}

				if (!response.EhValido) {
					$('.mostrarSeSetorSelecionado', Receber.container).addClass('hide');
					$('.btnReceber', Receber.container).button({ disabled: true });
					return;
				}
				
				$('.divTramitacoesReceber', Receber.container).html(response.HtmlTramitacoes);
				$('.divTramitacoesReceberSetor', Receber.container).html(response.HtmlTramitacoesSetor);

				$('.mostrarSeSetorSelecionado', Receber.container).toggleClass(
					'hide',
					(response.SetorDestinatarioId <= 0) || (response.NumeroTramitacoes <= 0)
				);

				if ((response.SetorDestinatarioId > 0) && (response.NumeroTramitacoes <= 0)) {
					$('.btnReceber', Receber.container).button({ disabled: true });
				} else {
					$('.btnReceber', Receber.container).button({ disabled: false });
				}
				MasterPage.redimensionar();
			}
		});
		MasterPage.carregando(false);
	},

	onBtnReceberClick: function () {
		Mensagem.limpar(Receber.container);
		if ($('.ddlSetorDestinatarioId', Receber.container).val() == 0) {
			Mensagem.gerar(Receber.container, new Array(Receber.settings.msgs.SetorDestinoObrigratorio));
			return;
		}

		var ReceberVM = {
			TramitacoesJson: []
		};

		$('.tabTramitacoesRecebidas tbody tr:visible', Receber.container).each(function () {
			if ($('.ckbSelecionavel', this).is(':checked')) {
				ReceberVM.TramitacoesJson.push($('.hdnTramitacaoJson', this).val());
			}
		});

		MasterPage.carregando(true);
		$.ajax({ url: Receber.settings.urls.receberSalvar,
			data: JSON.stringify(ReceberVM),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Receber.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.urlRedireciona);
				} else {
					Mensagem.gerar(Receber.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

ReceberRegistro = {
	settings: {
		urls:
		{
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
			$.extend(ReceberRegistro.settings, options);
		}

		ReceberRegistro.container = MasterPage.getContent(content);

		$('.ddlSetorDestinatarioId', content).change(ReceberRegistro.onDdlSetorDestinatarioIdChange);
		content.delegate('.ckbCheckAllInMyColumn', 'change', ReceberRegistro.onCkbCheckAllInMyColumnChange);
		content.delegate('.ckbSelecionavel', 'change', ReceberRegistro.onCkbSelecionavelChange);
		content.delegate('.trTramitacao', 'click', ReceberRegistro.onTrTramitacaoClick);
		$('.btnReceber', content).click(ReceberRegistro.onBtnReceberClick);
		content.delegate('.btnHistorico', 'click', ReceberRegistro.visualizarHistoricoDaTramitacaoClick);
		content.delegate('.btnPdf', 'click', ReceberRegistro.abrirPdfClick);

		$('.ddlSetorDestinatarioId', content).focus();
		ReceberRegistro.marcarTodos();
	},

	visualizarHistoricoDaTramitacaoClick: function () {
		var id = parseInt($('.hdnProtocoloId', $(this).closest('tr')).val());
		var tipo = parseInt($('.hdnIsProcesso', $(this).closest('tr')).val());

		Modal.abrir(ReceberRegistro.settings.urls.visualizarHistorico, { id: id, tipo: tipo }, function (context) {
			Modal.defaultButtons(context);
		});
	},

	abrirPdfClick: function () {
		var id = isNaN(parseInt($('.hdnTramitacaoId', $(this).closest('tr')).val())) ? 0 : parseInt($('.hdnTramitacaoId', $(this).closest('tr')).val());
		var tipo = parseInt($('.hdnIsProcesso', $(this).closest('tr')).val());
		if (id > 0) {
			MasterPage.redireciona(ReceberRegistro.settings.urls.abrirPdf + "?id=" + id + '&tipo=' + tipo + "&obterHistorico=" + false);
			MasterPage.carregando(false);
		}
	},

	onTrTramitacaoClick: function (e) {
		if ($(e.target).is('input')) return;
		$('.ckbSelecionavel', this).attr('checked', !$('.ckbSelecionavel', this).is(':checked'));
		$(this).toggleClass('linhaSelecionada', $('.ckbSelecionavel', this).is(':checked'));
		ReceberRegistro.marcarTodos();
	},

	onCkbSelecionavelChange: function (e) {
		$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
		ReceberRegistro.marcarTodos();
	},

	onCkbCheckAllInMyColumnChange: function () {
		$('tbody .ckbSelecionavel', $(this).closest('.tabTramitacoesRecebidas')).attr('checked', $(this).is(':checked'));
		$('tbody .ckbSelecionavel', $(this).closest('.tabTramitacoesRecebidas')).each(function () {
			$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
		});
	},

	marcarTodos: function () {
		$('.tabTramitacoesRecebidas', ReceberRegistro.container).each(function (idx, tabela) {
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
		var id = parseInt($('.ddlSetorDestinatarioId', ReceberRegistro.container).val());
		Mensagem.limpar(ReceberRegistro.container);

		if (id == 0) {
			$('.mostrarSeSetorSelecionado', ReceberRegistro.container).addClass('hide');
			$('.btnReceber', ReceberRegistro.container).button({ disabled: false });
			return;
		}

		MasterPage.carregando(true);

		$.ajax({
			url: ReceberRegistro.settings.urls.receberFiltrar,
			data: { setorId: id },
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ReceberRegistro.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ReceberRegistro.container, response.Msg);
				}

				if (!response.EhValido) {
					$('.mostrarSeSetorSelecionado', ReceberRegistro.container).addClass('hide');
					$('.btnReceber', ReceberRegistro.container).button({ disabled: true });
					return;
				}

				$('.divTramitacoesReceber', ReceberRegistro.container).html(response.HtmlTramitacoes);
				$('.divTramitacoesReceberSetor', ReceberRegistro.container).html(response.HtmlTramitacoesSetor);

				$('.mostrarSeSetorSelecionado', ReceberRegistro.container).toggleClass(
					'hide',
					(response.vm.SetorDestinatario.Id <= 0) || (response.vm.NumeroTramitacoes <= 0)
				);

				if ((response.vm.SetorDestinatario.Id > 0) && (response.vm.NumeroTramitacoes <= 0)) {
					$('.btnReceber', ReceberRegistro.container).button({ disabled: true });
				} else {
					$('.btnReceber', ReceberRegistro.container).button({ disabled: false });
				}

				MasterPage.redimensionar();
			}
		});
		MasterPage.carregando(false);
	},

	onBtnReceberClick: function () {

		if (parseInt($('.ddlSetorDestinatarioId', ReceberRegistro.container).val()) == 0) {
			Mensagem.gerar(ReceberRegistro.container, new Array(ReceberRegistro.settings.msgs.SetorDestinoObrigratorio));
			return;
		}

		var ReceberRegistroVM = {
			FuncionarioDestinatario: { Id: parseInt($('.ddlFuncionariosSetorId', ReceberRegistro.container).val()) },
			TramitacoesJson: new Array(),
			TramitacoesSetorJson: new Array()
		};

		$('.divTramitacoesReceber .tabTramitacoesRecebidas tbody tr:visible').each(function () {
			if ($('.ckbSelecionavel', this).is(':checked')) {
				ReceberRegistroVM.TramitacoesJson.push($('.hdnTramitacaoJson', this).val());
			}
		});

		$('.divTramitacoesReceberSetor .tabTramitacoesRecebidas tbody tr:visible').each(function () {
			if ($('.ckbSelecionavel', this).is(':checked')) {
				ReceberRegistroVM.TramitacoesSetorJson.push($('.hdnTramitacaoJson', this).val());
			}
		});

		MasterPage.carregando(true);
		$.ajax({ url: ReceberRegistro.settings.urls.receberSalvar,
			data: JSON.stringify(ReceberRegistroVM),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ReceberRegistro.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.urlRedireciona);
				} else {
					Mensagem.gerar(ReceberRegistro.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
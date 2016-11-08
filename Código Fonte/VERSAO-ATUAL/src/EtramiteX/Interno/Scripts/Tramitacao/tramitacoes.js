/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

Tramitacoes = {
	urlObterFuncionarios: null,
	urlObterTramitacaoSetor: null,
	urlCancelar: null,
	urlVisualizarProc: null,
	urlVisualizarDoc: null,
	container: null,

	visualizarHistorico: null,

	urlValidarTramitacaoEnviar: null,
	urlTramitacaoEnviar: null,
	urlTramitacaoEnviarRegistro: null,

	urlValidarTramitacaoReceber: null,
	urlTramitacaoReceber: null,
	urlTramitacaoReceberRegistro: null,
    
	urlTramitacaoGerarPdf: null,

	Mensagens: {},

	load: function (container) {

		Tramitacoes.container = container;

		$('.hdnMostrarFunc', Tramitacoes.container).val('False')
		Tramitacoes.configurarDropDowns(+$('.hdnMostrarSetor', container).val(), false);
		
		container.delegate('.titFiltros', 'click', Tramitacoes.expadirFiltro);

		container.delegate('.ddlSetor', 'change', Tramitacoes.onSetorChange);
		container.delegate('.ddlFuncionario', 'change', Tramitacoes.onFuncionarioChange);
		container.delegate('.btnCancelarTramitacao', 'click', Tramitacoes.onCancelarTramitacao);

		container.delegate('.btnHistorico', 'click', Tramitacoes.onHistoricoTramitacao);
		container.delegate('.btnVisualizar', 'click', Tramitacoes.onVisualizarProtocolo);
		container.delegate('.btnEnviar', 'click', Tramitacoes.onEnviarProtocolo);
		container.delegate('.btnReceber', 'click', Tramitacoes.onReceberProtocolo);
	},

	obterProtocoloJson: function (ref) {
	    var linha = $(ref, Tramitacoes.container).closest('tr');
	    return JSON.parse($('.hdnProtocoloJSon', linha).val());
	},

    expadirFiltro: function () {

		var container = $(this).closest('fieldset');

		$('.titFiltros', container).toggleClass('fAberto');

		if ($('.titFiltro', container).parent().find('.fixado').length == 0) {
			if ($('.filtroCorpo', container).is(':animated')) {
				$('.filtroCorpo', container).stop(true, true);
				$('.titFiltros', container).toggleClass('fAberto');
			} else {
				$('.filtroCorpo', container).slideToggle('normal');
			}
		} else {
			if ($('.filtroCorpo > div', container).children().not('.fixado').is(':animated')) {
				$('.filtroCorpo > div', container).children().not('.fixado').stop(true, true);
				$('.titFiltros', container).toggleClass('fAberto');
			} else {
				$('.titFiltros > div', container).children().not('.fixado').slideToggle('normal');
			}
		}
	},

	onHistoricoTramitacao: function () {

	    var protocoloJSON = Tramitacoes.obterProtocoloJson(this);

	    Modal.abrir(Tramitacoes.visualizarHistorico, { id: protocoloJSON.Id, tipo: (protocoloJSON.IsProcesso ? 1 : 2) }, function (context) {
			Modal.defaultButtons(context);
		});
	},

	onVisualizarProtocolo: function () {

	    var protocoloJSON = Tramitacoes.obterProtocoloJson(this);
	    var url = protocoloJSON.IsProcesso ? Tramitacoes.urlVisualizarProc : Tramitacoes.urlVisualizarDoc;

	    Modal.abrir(url + '/' + protocoloJSON.Id, null, function (container) { Modal.defaultButtons(container); }, Modal.tamanhoModalGrande);
	},

	onTrTramitacaoClick: function () {
		$('.ckbSelecionavel', this).attr('checked', !$('.ckbSelecionavel', this).is(':checked'));
		$(this).toggleClass('linhaSelecionada', $('.ckbSelecionavel', this).is(':checked'));
	},

	configurarDropDowns: function (setor, ddlFuncionario) {

		if (setor == 1) {
			$('.divSetor', Tramitacoes.container).remove();
			$('.divFunc', Tramitacoes.container).removeClass('prepend2');
		}

		if (ddlFuncionario) {
			$('.divFunc', Tramitacoes.container).removeClass('hide');
		}
		else {
			$('.divFunc', Tramitacoes.container).addClass('hide');
		}
	},

	gerarObjetoFiltroTramitacao: function () {

		var objeto = {};

		if ($('.hdnMostrarFunc', Tramitacoes.container).val().toLowerCase() == "true") {
			objeto.FuncionarioId = $('.ddlFuncionario', Tramitacoes.container).val();
		} else {
			objeto.FuncionarioId = $('.hdnFuncionario', Tramitacoes.container).val();
		}

		if (+$('.hdnMostrarSetor', Tramitacoes.container).val() == 1) {
			objeto.SetorId = $('.hdnSetorId', Tramitacoes.container).val();
		} else {
			objeto.SetorId = $('.ddlSetor', Tramitacoes.container).val();
		}

		return objeto;
	},

	onCancelarTramitacao: function () {

		var objeto = Tramitacoes.gerarObjetoFiltroTramitacao();

		var protocoloJSON = Tramitacoes.obterProtocoloJson(this);

		//objeto.SetorId = $('.hdnSetorId', linha).val();
		objeto.TramitacaoId = protocoloJSON.TramitacaoId;

		objeto.IsProcesso = protocoloJSON.IsProcesso;
		objeto.ProtocoloId = protocoloJSON.Id;
		objeto.ProtocoloNumero = protocoloJSON.Numero;

		$.ajax({ url: Tramitacoes.urlCancelar,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				MasterPage.carregando(false);
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Tramitacoes.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Tramitacoes.container), response.Msg);
					MasterPage.carregando(false);
					return;
				}

				var successMsg = new Array();
				successMsg.push(Tramitacoes.Mensagens.Cancelar);
				Mensagem.gerar(MasterPage.getContent(Tramitacoes.container), successMsg);

				MasterPage.carregando(false);
				Tramitacoes.onFuncionarioChange();
			}
		});
	},

	onFuncionarioChange: function () {

		MasterPage.carregando(true);

		var objeto = Tramitacoes.gerarObjetoFiltroTramitacao();

		$.ajax({ url: Tramitacoes.urlObterTramitacaoSetor,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				MasterPage.carregando(false);
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Tramitacoes.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				$('.divConteudoTramitacao', Tramitacoes.container).empty();

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Tramitacoes.container), response.Msg);
				}

				Tramitacoes.carregarConteudo(response.Html);
			}
		});

		MasterPage.carregando(false);
	},

	onSetorChange: function () {

		MasterPage.carregando(true);

		if ($(this).val() == 0) {
			$('.hdnMostrarFunc', Tramitacoes.container).val('False');
			$('.divFunc', Tramitacoes.container).addClass('hide');
		}

		var objeto = Tramitacoes.gerarObjetoFiltroTramitacao();

		if ($('.hdnMostrarFunc', Tramitacoes.container).val().toLowerCase() == 'true') {
			objeto.FuncionarioId = 0;
		}

		$.ajax({ url: Tramitacoes.urlObterTramitacaoSetor,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				MasterPage.carregando(false);
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Tramitacoes.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				$('.divConteudoTramitacao', Tramitacoes.container).empty();

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Tramitacoes.container), response.Msg);
				}

				Tramitacoes.configurarDropDowns(response.VM.MostrarSetor, response.VM.MostrarFuncionario);

				$('.hdnMostrarFunc', Tramitacoes.container).val(response.VM.MostrarFuncionario);
				$('.ddlFuncionario', Tramitacoes.container).ddlLoad(response.VM.FuncionariosLst, {textoDefault: DropDownText.todos});
				$('.ddlFuncionario', Tramitacoes.container).val($('.hdnFuncionario', Tramitacoes.container).val());

				Tramitacoes.carregarConteudo(response.Html);
			}
		});

		MasterPage.carregando(false);
	},

	carregarConteudo: function (html) {
		$('.divConteudoTramitacao', Tramitacoes.container).append(html);
		Listar.atualizarEstiloTable($('.dataGridTable', Tramitacoes.container));
		MasterPage.redimensionar();
	},

	onddlSetorDestinoIdChange: function () {
		var id = parseInt($(this).val());

		MasterPage.carregando(true);

		$.ajax({
			url: Tramitacoes.settings.urls.receberFiltrar,
			data: { setorId: id },
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				MasterPage.carregando(false);
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Tramitacoes.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				Mensagem.limpar(MasterPage.getContent(Tramitacoes.container));

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Tramitacoes.container), response.Msg);
				} else {
					$('.divTramitacoesReceber', Tramitacoes.container).html(response.HtmlTramitacoes);
					$('.divTramitacoesReceberSetor', Tramitacoes.container).html(response.ddlSetorDestinoId);
					Listar.atualizarEstiloTable($('.dataGridTable', Tramitacoes.container));
				}

				MasterPage.redimensionar();
			}
		});
		MasterPage.carregando(false);
	},

	onCkbSelecionavelChange: function () {
		$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
	},

	onEnviarProtocolo: function () {

	    var protocoloJSON = Tramitacoes.obterProtocoloJson(this);

	    $.ajax({
	        url: Tramitacoes.urlValidarTramitacaoEnviar + '?protocoloId=' + protocoloJSON.Id,
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				MasterPage.carregando(false);
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Tramitacoes.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Tramitacoes.container), response.Msg);
					return;
				}

				var url = '';
				var id = response.Protocolo.Id;
				var setor = response.Protocolo.SetorId;

				if (response.EnviarRegistro) {
					url = Tramitacoes.urlTramitacaoEnviarRegistro;
				} else {
					url = Tramitacoes.urlTramitacaoEnviar;
				}
				MasterPage.redireciona(url + '?setorId=' + setor + '&protocoloId=' + id);
			}
		});
		MasterPage.carregando(false);
	},

	onReceberProtocolo: function () {

	    var protocoloJSON = Tramitacoes.obterProtocoloJson(this);

	    $.ajax({
	        url: Tramitacoes.urlValidarTramitacaoReceber + '/' + protocoloJSON.TramitacaoId,
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				MasterPage.carregando(false);
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Tramitacoes.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Tramitacoes.container), response.Msg);
					return;
				}

				var url;
				if (response.ReceberRegistro) {
					url = Tramitacoes.urlTramitacaoReceberRegistro;
				} else {
					url = Tramitacoes.urlTramitacaoReceber;
				}
				MasterPage.redireciona(url + '/' + response.Id);
			}
		});

		MasterPage.carregando(false);
	},

	onGerarPdf: function (obterHistorico,control) {

	    var protocoloJSON = Tramitacoes.obterProtocoloJson(control);
	    var id = obterHistorico ? protocoloJSON.HistorioId : protocoloJSON.TramitacaoId;
	    var tipo = protocoloJSON.IsProcesso ? 1 : 2;

	    MasterPage.redireciona(Tramitacoes.urlTramitacaoGerarPdf + '?id=' + id + '&tipo=' + tipo + '&obterHistorico=' + obterHistorico);
	},

	onCkbCheckAllInMyColumnChange: function () {
		$('tbody .ckbSelecionavel', $(this).closest('.tabTramitacoesRecebidas')).attr('checked', $(this).is(':checked'));
		$('tbody .ckbSelecionavel', $(this).closest('.tabTramitacoesRecebidas')).each(function () {
			$(this).closest('tr').toggleClass('linhaSelecionada', $(this).is(':checked'));
		});
	},

	onBtnReceberClick: function () {
		var ReceberVM = {
			SetorDestinatarioId: 0,
			TramitacoesJson: []
		};

		$('.tabTramitacoesRecebidas tbody tr:visible').each(function () {
			if ($('.ckbSelecionavel', this).is(':checked')) {
				ReceberVM.TramitacoesJson.push($('.hdnTramitacaoJson', this).val());
			}
		});


		$.ajax({ url: Tramitacoes.settings.urls.receberSalvar,
			data: JSON.stringify(ReceberVM),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				MasterPage.carregando(false);
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Tramitacoes.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(Tramitacoes.container), response.Msg);
				}
				MasterPage.carregando(false);
			}
		});
	}
}
/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

UnidadeConsolidacao = {
	settings: {
		urls: {
			listarCulturas: null,
			adicionarCultivar: null,
			listarCredenciados: null,
			adicionarResponsavelTecnico: null,
			obterResponsavelTecnico: null,
			salvar: null,
			obterLstCultivares: null
		}
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(UnidadeConsolidacao.settings, options);
		}

		container.delegate('.RadioCodigoUC', 'change', UnidadeConsolidacao.changePossuiCodigoUC);
		container.delegate('.btnBuscarCultura', 'click', UnidadeConsolidacao.abrirModalCulturas);
		container.delegate('.btnAddCultura', 'click', UnidadeConsolidacao.adicionarCultura);
		container.delegate('.btnBuscarRespTec', 'click', UnidadeConsolidacao.abrirModalCredenciados);
		container.delegate('.RadioARTCargoFuncao', 'click', UnidadeConsolidacao.changeARTCargoFuncao);
		container.delegate('.btnAddRespTec', 'click', UnidadeConsolidacao.adicionarResponsavelTecnico);

		container.delegate('.btnExcluir', 'click', UnidadeConsolidacao.excluirLinha);
		container.delegate('.btnSalvar', 'click', UnidadeConsolidacao.salvar);
		UnidadeConsolidacao.container = MasterPage.getContent(container);
	},

	changePossuiCodigoUC: function () {
		if ($(this).val() == 'True') {
			$('.txtCodigoUC', UnidadeConsolidacao.container).removeClass('disabled');
			$('.txtCodigoUC', UnidadeConsolidacao.container).removeAttr('disabled');
			$('.txtCodigoUC', UnidadeConsolidacao.container).val('');
		} else {
			$('.txtCodigoUC', UnidadeConsolidacao.container).addClass('disabled');
			$('.txtCodigoUC', UnidadeConsolidacao.container).attr('disabled', 'disabled');
			$('.txtCodigoUC', UnidadeConsolidacao.container).val('Gerado automaticamente');
		}
	},

	abrirModalCulturas: function () {
		Modal.abrir(UnidadeConsolidacao.settings.urls.listarCulturas, null,
		function (content) {
			CulturaListar.load(content, { onAssociarCallback: UnidadeConsolidacao.associarCulturas });
			Modal.defaultButtons(content);
		}, Modal.tamanhoModalMedia);
	},

	associarCulturas: function (response) {
		$('.txtCulturaTexto', UnidadeConsolidacao.container).val(response.Nome);
		$('.hdnCulturaId', UnidadeConsolidacao.container).val(response.Id);

		$.ajax({
			url: UnidadeConsolidacao.settings.urls.obterLstCultivares,
			data: JSON.stringify({ culturaId: response.Id }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (retorno, textStatus, XMLHttpRequest) {
				if (retorno.Valido) {
					$('.ddlCultivar', UnidadeConsolidacao.container).ddlLoad(retorno.LstCultivar);
				}

				if (retorno.Msg && retorno.Msg.length > 0) {
					Mensagem.gerar(UnidadeConsolidacao.container, retorno.Msg);
				}
			}
		});

		return true;
	},

	adicionarCultura: function () {
		Mensagem.limpar(UnidadeConsolidacao.container);
		var container = UnidadeConsolidacao.container;
		var tabela = $('.gridCultivares', container);
		var item = {
			Id: +$('.ddlCultivar :selected', container).val(),
			Nome: $('.ddlCultivar :selected', container).text(),
			CapacidadeMes: Mascara.getFloatMask($('.txtCapacidadeMes', container).val()),
			UnidadeMedida: +$('.ddlUnidadeMedida', container).val(),
			UnidadeMedidaTexto: $('.ddlUnidadeMedida :selected', container).text(),
			CulturaTexto: $('.txtCulturaTexto', container).val(),
			CulturaId: +$('.hdnCulturaId', container).val()
		};

		var lista = new Array();
		$('tbody tr:not(.trTemplate)', tabela).each(function (i, linha) {
			lista.push(JSON.parse($('.hdnItemJson', linha).val()));
		});

		var ehValido = MasterPage.validarAjax(
			UnidadeConsolidacao.settings.urls.adicionarCultivar,
			{ cultivarLista: lista, cultivar: item },
			container, false).EhValido;

		if (!ehValido) {
			return;
		}

		var linha = $('.trTemplate', tabela).clone();
		$(linha).removeClass('hide trTemplate');

		$('.hdnItemJson', linha).val(JSON.stringify(item));
		$('.lblCultura', linha).text(item.CulturaTexto + (item.Id > 0 ? ' ' + item.Nome : ''));
		$('.lblCapacidadeMes', linha).append(Mascara.getStringMask(item.CapacidadeMes, 'n4') + ' ' + item.UnidadeMedidaTexto);
		$('tbody', tabela).append(linha);

		$('.ddlUnidadeMedida', container).ddlFirst();
		$('.ddlCultivar', container).ddlClear();

		$('.cultivarContainer .clear', container).unmask().val('');
		Mascara.load($('.cultivarContainer', container));

		Listar.atualizarEstiloTable(tabela);
		$('.btnBuscarCultivar', container).focus();
		UnidadeConsolidacao.configurarBuscarResponsavel();
	},

	abrirModalCredenciados: function () {
		Modal.abrir(UnidadeConsolidacao.settings.urls.listarCredenciados, null,
		function (content) {
			CredenciadoListar.load(content, { onAssociarCallback: UnidadeConsolidacao.associarResponsavelTecnico });
			Modal.defaultButtons(content);
		}, Modal.tamanhoModalMedia);
	},

	configurarBuscarResponsavel: function () {
		var container = UnidadeConsolidacao.container;
		$('.btnBuscarRespTec', container).toggleClass('hide', $('.gridCultivares tbody tr:not(.trTemplate)', container).length < 1);
	},

	associarResponsavelTecnico: function (credenciadoId) {
		Mensagem.limpar(UnidadeConsolidacao.container);
		var sucesso = false;
		var cultivarLista = UnidadeConsolidacao.obter().Cultivares;

		MasterPage.carregando(true);
		$.ajax({
			url: UnidadeConsolidacao.settings.urls.obterResponsavelTecnico,
			data: JSON.stringify({ credenciadoId: credenciadoId, cultivares: cultivarLista }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				sucesso = response.EhValido;

				if (response.EhValido) {
					$('.txtResponsavelNome', UnidadeConsolidacao.container).val(response.Habilitacao.Responsavel.Nome);
					$('.hdnResponsavelId', UnidadeConsolidacao.container).val(credenciadoId);
					$('.txtResponsavelNumCFOCFOC', UnidadeConsolidacao.container).val(response.Habilitacao.NumeroHabilitacaoES);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(UnidadeConsolidacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
		return sucesso;
	},

	adicionarResponsavelTecnico: function () {
		Mensagem.limpar(UnidadeConsolidacao.container);
		var container = UnidadeConsolidacao.container;
		var tabela = $('.gridResponsaveis', container);
		var item = {
			Id: +$('.hdnResponsavelId', container).val(),
			NomeRazao: $('.txtResponsavelNome', container).val(),
			CFONumero: $('.txtResponsavelNumCFOCFOC', container).val(),
			NumeroArt: $('.txtResponsavelNumART', container).val(),
			ArtCargoFuncao: $('.rbARTCargoFuncaoSim', container).is(':checked'),
			DataValidadeART: $('.txtResponsavelDataValidadeART', container).val()
		};

		var lista = new Array();
		$('tbody tr:not(.trTemplate)', tabela).each(function (i, linha) {
			lista.push(JSON.parse($('.hdnItemJson', linha).val()));
		});

		var ehValido = MasterPage.validarAjax(
			UnidadeConsolidacao.settings.urls.adicionarResponsavelTecnico,
			{ responsavelTecnicoLista: lista, responsavelTecnico: item },
			container, false).EhValido;

		if (!ehValido) {
			return;
		}

		var linha = $('.trTemplate', tabela).clone();
		$(linha).removeClass('hide trTemplate');

		$('.hdnItemJson', linha).val(JSON.stringify(item));
		$('.lblNome', linha).append(item.NomeRazao);
		$('.lblNumeroHabilitacao', linha).append(item.CFONumero);
		$('.lblDataValidadeART', linha).append(item.DataValidadeART);
		$(tabela).append(linha);

		$(".responsavelClear", container).val('');
		$(".rbARTCargoFuncaoSim", container).click();

		Listar.atualizarEstiloTable(tabela);
		$('.btnBuscarRespTec', container).focus();
	},

	changeARTCargoFuncao: function () {
		if ($(this).val() == 'True') {
			$('.txtResponsavelDataValidadeART', UnidadeConsolidacao.container).addClass('disabled');
			$('.txtResponsavelDataValidadeART', UnidadeConsolidacao.container).attr('disabled', 'disabled');
			$('.txtResponsavelDataValidadeART', UnidadeConsolidacao.container).val('');
			$('.divDataValidadeART', UnidadeConsolidacao.container).addClass('hide');
		} else {
			$('.txtResponsavelDataValidadeART', UnidadeConsolidacao.container).removeClass('disabled');
			$('.txtResponsavelDataValidadeART', UnidadeConsolidacao.container).removeAttr('disabled');
			$('.divDataValidadeART', UnidadeConsolidacao.container).removeClass('hide');
		}
	},

	excluirLinha: function () {
		var tabela = $(this).closest('table');
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable(tabela);
		UnidadeConsolidacao.configurarBuscarResponsavel();
	},

	obter: function () {
		var container = UnidadeConsolidacao.container;
		var objeto = {
			Id: +$('.hdnUnidadeConsolidacaoId', container).val(),
			Empreendimento: { Id: +$('.hdnEmpreendimentoId', container).val() },
			PossuiCodigoUC: $('.rbPossuiCodigoSim', container).is(':checked'),
			CodigoUC: Mascara.getIntMask($('.txtCodigoUC', container).val()),
			LocalLivroDisponivel: $('.txtLocalLivroDisponivel', container).val(),
			TipoApresentacaoProducaoFormaIdentificacao: $('.txtTipoApresentacao', container).val(),
			Cultivares: [],
			ResponsaveisTecnicos: []
		};

		$('.gridCultivares tbody tr:not(.trTemplate)', container).find('.hdnItemJson').each(function () {
			objeto.Cultivares.push(JSON.parse($(this).val()));
		});

		$('.gridResponsaveis tbody tr:not(.trTemplate)', container).find('.hdnItemJson').each(function () {
			objeto.ResponsaveisTecnicos.push(JSON.parse($(this).val()));
		});

		return objeto;
	},

	salvar: function () {
		Mensagem.limpar(UnidadeConsolidacao.container);
		var objeto = UnidadeConsolidacao.obter();

		MasterPage.carregando(true);
		$.ajax({
			url: UnidadeConsolidacao.settings.urls.salvar,
			data: JSON.stringify({ caracterizacao: objeto, projetoDigitalId: $('.hdnProjetoDigitalId', UnidadeConsolidacao.container).val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(UnidadeConsolidacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
/// <reference path="../../mensagem.js" />
/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="coordenadaAtividade.js" />

UnidadeProducaoItem = {
	settings: {
		urls: {
			salvar: null,
			coordenadaGeo: null,
			listarCulturas: null,
			listarCredenciados: null,
			obterResponsavelTecnico: null,
			validarUnidadeProducaoItem: null,
			obterLstCultivares: null
		},
		empreendimentoID: 0,
		onSalvarCallback: null,
		responsaveisTecnicos: []
	},
	mensagens: null,
	idsTelaTipoProducao: null,
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(UnidadeProducaoItem.settings, options);
		}

		UnidadeProducaoItem.container = MasterPage.getContent(container);

		UnidadeProducaoItem.container.delegate('.RadioTipoProducao', 'change', UnidadeProducaoItem.onRadioTipoProducaoChange);
		UnidadeProducaoItem.container.delegate('.RadioPossuiCodigoUP', 'change', UnidadeProducaoItem.onRadioPossuiCodigoUP);
		UnidadeProducaoItem.container.delegate('.RadioARTCargoFuncao', 'click', UnidadeProducaoItem.onRadioARTCargoFuncao);
		UnidadeProducaoItem.container.delegate('.btnBuscarCoordenada', 'click', UnidadeProducaoItem.onBuscarCoordenada);
		UnidadeProducaoItem.container.delegate('.btnBuscarCultura', 'click', UnidadeProducaoItem.abrirModalCulturas);

		UnidadeProducaoItem.container.delegate('.btnAdicionarProdutor', 'click', UnidadeProducaoItem.onAddProdutor);
		UnidadeProducaoItem.container.delegate('.btnExcluirProdutor', 'click', UnidadeProducaoItem.excluirProdutor);

		UnidadeProducaoItem.container.delegate('.btnBuscarRespTec', 'click', UnidadeProducaoItem.abrirModalCredenciados);
		UnidadeProducaoItem.container.delegate('.btnAddRespTec', 'click', UnidadeProducaoItem.onAddResponsavelTecnico);
		UnidadeProducaoItem.container.delegate('.btnExcluirResponsavelTecnico', 'click', UnidadeProducaoItem.onResponsavelTecnicoExcluir);
	},

	onRadioPossuiCodigoUP: function () {
		Mensagem.limpar(UnidadeProducaoItem.container);

		if ($(this).val() == 'True') {
			$('.txtCodigoUP', UnidadeProducaoItem.container).val('');
			$('.txtCodigoUP', UnidadeProducaoItem.container).removeAttr('disabled');
			$('.txtCodigoUP', UnidadeProducaoItem.container).removeClass('disabled');
			$('.txtCodigoUP', UnidadeProducaoItem.container).focus();
		} else {
			$('.txtCodigoUP', UnidadeProducaoItem.container).attr('disabled', 'disabled');
			$('.txtCodigoUP', UnidadeProducaoItem.container).addClass('disabled');
			$('.txtCodigoUP', UnidadeProducaoItem.container).val('Gerado automaticamente');
		}
	},

	onRadioTipoProducaoChange: function () {
		Mensagem.limpar(UnidadeProducaoItem.container);
		UnidadeProducaoItem.displayTipoProducao(+$(this).val());
	},

	displayTipoProducao: function (opcao) {
		$('.txtRenasemNumero, .txtDataValidadeRenasem', UnidadeProducaoItem.container).addClass('disabled');
		$('.divMatProp', UnidadeProducaoItem.container).addClass('hide');
		$('.txtRenasemNumero, .txtDataValidadeRenasem', UnidadeProducaoItem.container).attr('disabled', 'disabled');
		$('.txtRenasemNumero, .txtDataValidadeRenasem', UnidadeProducaoItem.container).val('');

		switch (opcao) {
			case UnidadeProducaoItem.idsTelaTipoProducao.Frutos:
				$('.txtEstimativaProducaoUnidadeMedida', UnidadeProducaoItem.container).val('T');
				break;
			case UnidadeProducaoItem.idsTelaTipoProducao.Madeira:
				$('.txtEstimativaProducaoUnidadeMedida', UnidadeProducaoItem.container).val('m³');
				break;
			case UnidadeProducaoItem.idsTelaTipoProducao.MaterialPropagacao:
				$('.txtRenasemNumero, .txtDataValidadeRenasem', UnidadeProducaoItem.container).removeClass('disabled');
				$('.divMatProp', UnidadeProducaoItem.container).removeClass('hide');
				$('.txtRenasemNumero, .txtDataValidadeRenasem', UnidadeProducaoItem.container).removeAttr('disabled');
				$('.txtRenasemNumero', UnidadeProducaoItem.container).focus();
				$('.txtEstimativaProducaoUnidadeMedida', UnidadeProducaoItem.container).val('Und');
				break;
		}
	},

	onRadioARTCargoFuncao: function () {
		Mensagem.limpar(UnidadeProducaoItem.container);

		if ($(this).val() == 'True') {
			$('.txtResponsavelDataValidadeART', UnidadeProducaoItem.container).addClass('disabled');
			$('.txtResponsavelDataValidadeART', UnidadeProducaoItem.container).attr('disabled', 'disabled');
			$('.divDataValidadeART', UnidadeProducaoItem.container).addClass('hide');
			$('.txtResponsavelDataValidadeART', UnidadeProducaoItem.container).val('');

		} else {
			$('.txtResponsavelDataValidadeART', UnidadeProducaoItem.container).removeClass('disabled');
			$('.txtResponsavelDataValidadeART', UnidadeProducaoItem.container).removeAttr('disabled');
			$('.divDataValidadeART', UnidadeProducaoItem.container).removeClass('hide');
			$('.txtResponsavelDataValidadeART', UnidadeProducaoItem.container).focus();
		}
	},

	onBuscarCoordenada: function () {
		Modal.abrir(UnidadeProducaoItem.settings.urls.coordenadaGeo, null, function (container) {
			Coordenada.load(container, {
				northing: $('.txtNorthing', UnidadeProducaoItem.settings.container).val(),
				easting: $('.txtEasting', UnidadeProducaoItem.settings.container).val(),
				pagemode: 'editMode',
				callBackSalvarCoordenada: UnidadeProducaoItem.setarCoordenada
			});
			Modal.defaultButtons(container);
		},
		Modal.tamanhoModalGrande);
	},

	setarCoordenada: function (retorno) {
		retorno = JSON.parse(retorno);

		$('.txtNorthing', UnidadeProducaoItem.settings.container).val(retorno.northing);
		$('.txtEasting', UnidadeProducaoItem.settings.container).val(retorno.easting);
		$('.btnBuscarCoordenada', UnidadeProducaoItem.settings.container).focus();
	},

	abrirModalCulturas: function () {
		Modal.abrir(UnidadeProducaoItem.settings.urls.listarCulturas, null, function (content) {
			CulturaListar.load(content, { onAssociarCallback: UnidadeProducaoItem.callBackAssociarCultura });
			Modal.defaultButtons(content);
		}, Modal.tamanhoModalMedia);
	},

	callBackAssociarCultura: function (response) {
		$('.txtCulturaTexto', UnidadeProducaoItem.container).val(response.Nome);
		$('.hdnCulturaId', UnidadeProducaoItem.container).val(response.Id);

		$.ajax({
			url: UnidadeProducaoItem.settings.urls.obterLstCultivares,
			data: JSON.stringify({ culturaId: response.Id }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (retorno, textStatus, XMLHttpRequest) {
				if (retorno.Valido) {
					$('.ddlCultivar', UnidadeProducaoItem.container).ddlLoad(retorno.LstCultivar);
				}

				if (retorno.Msg && retorno.Msg.length > 0) {
					Mensagem.gerar(UnidadeProducaoItem.container, retorno.Msg);
				}
			}
		});

		$('.btnBuscarRespTec', UnidadeProducaoItem.container).removeClass('hide');
		return true;
	},

	onAddProdutor: function () {
		var tabela = $('.gridProdutores', UnidadeProducaoItem.container);
		var Responsavel = {
			Id: +$('.ddlProdutores :selected', UnidadeProducaoItem.container).val(),
			NomeRazao: $('.ddlProdutores :selected', UnidadeProducaoItem.container).text()
		};

		if (Responsavel.Id < 1) {
			Mensagem.gerar(MasterPage.getContent(UnidadeProducaoItem.container), [UnidadeProducaoItem.settings.mensagens.ProdutorObrigatorio]);
			return;
		}

		var itemAdicionado = false;
		$('tbody tr', tabela).each(function () {
			if ($('.hdnProdutorItemId', this).val() == Responsavel.Id) {
				itemAdicionado = true;
			}
		});

		if (itemAdicionado) {
			Mensagem.gerar(MasterPage.getContent(UnidadeProducaoItem.container), [UnidadeProducaoItem.settings.mensagens.ProdutorJaAdicionado]);
			return;
		}

		var linha = $('.trTemplate', tabela).clone();
		$('.lblNomeProdutor', linha).append(Responsavel.NomeRazao);
		$('.hdnProdutorItemId', linha).val(Responsavel.Id);

		$(linha).removeClass('hide').removeClass('trTemplate');
		$('tbody', tabela).append(linha);
		Listar.atualizarEstiloTable(tabela);

		$('.ddlProdutores', UnidadeProducaoItem.container).ddlFirst();
	},

	excluirProdutor: function () {
		$(this).closest('tr').remove();
	},

	abrirModalCredenciados: function () {
		Modal.abrir(UnidadeProducaoItem.settings.urls.listarCredenciados, null, function (content) {
			CredenciadoListar.load(content, { onAssociarCallback: UnidadeProducaoItem.callBackAssociarResponsavelTecnico });
			Modal.defaultButtons(content);
		}, Modal.tamanhoModalMedia);
	},

	callBackAssociarResponsavelTecnico: function (credenciadoId) {
		var sucesso = false;
		Mensagem.limpar(UnidadeProducaoItem.container);
		MasterPage.carregando(true);

		var Cultura = {
			Id: +$('.hdnCulturaId', UnidadeProducaoItem.container).val(),
			Nome: $('.txtCulturaTexto', UnidadeProducaoItem.container).val()
		}

		$.ajax({
			url: UnidadeProducaoItem.settings.urls.obterResponsavelTecnico,
			data: JSON.stringify({ credenciadoId: credenciadoId, cultura: Cultura }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.txtResponsavelNome', UnidadeProducaoItem.container).val(response.Habilitacao.Responsavel.Nome);
					$('.hdnResponsavelId', UnidadeProducaoItem.container).val(credenciadoId);
					$('.txtResponsavelNumCFOCFOC', UnidadeProducaoItem.container).val(response.Habilitacao.NumeroHabilitacaoES);
				}
				sucesso = response.EhValido;

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(UnidadeProducaoItem.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
		return sucesso;
	},

	onAddResponsavelTecnico: function () {
		Mensagem.limpar();
		var mensagensValidacao = [];
		var responsavelTecnicoObj = {
			Id: +$('.hdnResponsavelId', UnidadeProducaoItem.container).val(),
			NomeRazao: $('.txtResponsavelNome', UnidadeProducaoItem.container).val(),
			CFONumero: $(".txtResponsavelNumCFOCFOC", UnidadeProducaoItem.container).val(),
			NumeroArt: $(".txtResponsavelNumART", UnidadeProducaoItem.container).val(),
			ArtCargoFuncao: $('.rbARTCargoFuncaoSim', UnidadeProducaoItem.container).is(':checked'),
			DataValidadeART: $('.txtResponsavelDataValidadeART', UnidadeProducaoItem.container).val()
		};

		if (responsavelTecnicoObj.Id < 1) {
			mensagensValidacao.push(UnidadeProducaoItem.mensagens.ResponsavelTecnicoObrigatorio);
		}

		if (responsavelTecnicoObj.NumeroArt == '' || responsavelTecnicoObj.NumeroArt < 1) {
			mensagensValidacao.push(UnidadeProducaoItem.mensagens.NumeroARTObrigatorio);
		}

		if (!responsavelTecnicoObj.ArtCargoFuncao && responsavelTecnicoObj.DataValidadeART == '') {
			mensagensValidacao.push(UnidadeProducaoItem.mensagens.DataValidadeARTObrigatorio);
		}

		var tabela = $('.gridResponsaveis tbody', UnidadeProducaoItem.container);

		var itemAdicionado = false;
		$('tr:not(.trTemplate)', tabela).each(function () {
			if ($('.hdnItemResponsavelId', this).val() == responsavelTecnicoObj.Id) {
				itemAdicionado = true;
			}
		});

		if (itemAdicionado) {
			mensagensValidacao.push(UnidadeProducaoItem.mensagens.ResponsavelTecnicoJaAdicionado);
		}

		if (mensagensValidacao.length > 0) {
			Mensagem.gerar(UnidadeProducaoItem.container, mensagensValidacao);
			return;
		}

		var linha = $('.trTemplate', tabela).clone();

		$('.lblNome', linha).append(responsavelTecnicoObj.NomeRazao);
		$('.lblNumeroHabilitacao', linha).append(responsavelTecnicoObj.CFONumero);
		$('.lblDataValidadeART', linha).append(responsavelTecnicoObj.DataValidadeART);
		$('.hdnItemResponsavelId', linha).val(responsavelTecnicoObj.Id);
		$('.hdnItemResponsavelRelacionamentoId', linha).val(responsavelTecnicoObj.Id);

		$(linha).removeClass('hide');
		$(linha).removeClass('trTemplate');
		$(tabela).append(linha);

		$(".responsavelClear", UnidadeProducaoItem.container).val('');
		$(".rbARTCargoFuncaoSim", UnidadeProducaoItem.container).trigger('click');
		UnidadeProducaoItem.settings.responsaveisTecnicos.push(responsavelTecnicoObj);

		Listar.atualizarEstiloTable(UnidadeProducaoItem.container);
	},

	onResponsavelTecnicoExcluir: function () {
		var linhaDelete = $(this).closest('tr');
		for (var i = 0; i < UnidadeProducaoItem.settings.responsaveisTecnicos.length; i++) {
			if (UnidadeProducaoItem.settings.responsaveisTecnicos[i].Id == +$('.hdnItemResponsavelId', linhaDelete).val()) {
				UnidadeProducaoItem.settings.responsaveisTecnicos.splice(i);
				break;
			}
		}
		$(linhaDelete).remove();
		Listar.atualizarEstiloTable(UnidadeProducaoItem.container);
	},

	obter: function () {
		var unidadeProducaoItemObj = {
			Id: +$('.hdnId', UnidadeProducaoItem.container).val(),
			PossuiCodigoUP: $('.rbCodigoSim', UnidadeProducaoItem.container).is(':checked'),
			CodigoUP: $('.txtCodigoUP', UnidadeProducaoItem.container).val() || 0,
			TipoProducao: +$('.RadioTipoProducao:checked', UnidadeProducaoItem.container).val(),
			RenasemNumero: $('.txtRenasemNumero', UnidadeProducaoItem.container).val(),
			DataValidadeRenasem: $('.txtDataValidadeRenasem', UnidadeProducaoItem.container).val(),
			AreaHA: Mascara.getFloatMask($('.txtAreaHA', UnidadeProducaoItem.container).val()),
			CulturaId: +$('.hdnCulturaId', UnidadeProducaoItem.container).val(),
			CulturaTexto: $('.txtCulturaTexto', UnidadeProducaoItem.container).val(),
			CultivarId: +$('.ddlCultivar :selected', UnidadeProducaoItem.container).val(),
			CultivarTexto: $('.ddlCultivar :selected', UnidadeProducaoItem.container).text(),
			Coordenada: {
				Id: +$('.hdnCoordenadId', UnidadeProducaoItem.container).val(),
				Tipo: { Id: +$('.ddlCoordenadaTipo :selected', UnidadeProducaoItem.container).val() },
				Datum: { Id: +$('.ddlDatum :selected', UnidadeProducaoItem.container).val() },
				FusoUtm: +$('.ddlFuso :selected', UnidadeProducaoItem.container).val(),
				EastingUtmTexto: $('.txtEasting', UnidadeProducaoItem.container).val(),
				NorthingUtmTexto: $('.txtNorthing', UnidadeProducaoItem.container).val(),
				HemisferioUtm: $('.ddlHemisferio :selected', UnidadeProducaoItem.container).val()
			},
			DataPlantioAnoProducao: $('.txtDataPlantioAnoProducao', UnidadeProducaoItem.container).val(),
			Produtores: [],
			ResponsaveisTecnicos: UnidadeProducaoItem.settings.responsaveisTecnicos,
			EstimativaProducaoQuantidadeAno: Mascara.getFloatMask($('.txtEstimativaProducaoQuantAno', UnidadeProducaoItem.container).val()),
			EstimativaProducaoUnidadeMedida: $('.txtEstimativaProducaoUnidadeMedida', UnidadeProducaoItem.container).val()
		}

		$('.gridProdutores tbody tr:not(.trTemplate)', UnidadeProducaoItem.container).each(function () {
			unidadeProducaoItemObj.Produtores.push({
				Id: +$('.hdnProdutorItemId', this).val(),
				IdRelacionamento: +$('.hdnProdutorRelacionamentoId', this).val(),
				NomeRazao: $('.lblNomeProdutor', this).text()
			});
		});

		return unidadeProducaoItemObj;
	},

	salvar: function () {
		var unidade = UnidadeProducaoItem.obter();
		var msgValidacao = [];

		$.ajax({
			url: UnidadeProducaoItem.settings.urls.validarUnidadeProducaoItem,
			data: JSON.stringify({ unidade: unidade, empreendimento: UnidadeProducaoItem.settings.empreendimentoID }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (!response.EhValido) {
					msgValidacao = response.Msg;
				}
			}
		});

		if (msgValidacao.length > 0) {
			Mensagem.gerar(MasterPage.getContent(UnidadeProducaoItem.container), msgValidacao);
			return;
		}

		var sucesso = UnidadeProducaoItem.settings.onSalvarCallback(unidade);
		if (sucesso === true) {
			Modal.fechar(UnidadeProducaoItem.container);
		} else {
			Mensagem.gerar(UnidadeProducaoItem.container, sucesso);
		}
	}
}
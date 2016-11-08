/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../mensagem.js" />

DescricaoLicenciamentoAtividade = {
	settings: {
		urls: {
			Salvar: '',
			Visualizar: '',
			Redirecionar: ''
		},
		textoMerge: null,
		atualizarDependenciasModalTitulo: null,
		Mensagens: null
	},
	FontesAbastecimentoAgua: null,
	PontosLancamentoEfluente: null,
	FontesGeracaoOutrosId: 0,
	TratamentoOutrasFormasId: 0,
	DestinoFinalOutrosId: 0,
	container: null,
	load: function (container, options) {

		if (options) { $.extend(DescricaoLicenciamentoAtividade.settings, options); }
		DescricaoLicenciamentoAtividade.container = MasterPage.getContent(container);

		DescricaoLicenciamentoAtividade.container.delegate('input[name="rdbZonaUC"]', 'click', DescricaoLicenciamentoAtividade.onClickZonaUC);
		DescricaoLicenciamentoAtividade.container.delegate('input[name="rdbResidentesEntorno"]', 'click', DescricaoLicenciamentoAtividade.onClickResidentesEntorno);
		DescricaoLicenciamentoAtividade.container.delegate('.ddlFontesAbastecimentoAguaTipo', 'change', DescricaoLicenciamentoAtividade.onChangeFontesAbastecimentoAguaTipo);
		DescricaoLicenciamentoAtividade.container.delegate('.ddlPontosLancamentoEfluenteTipo', 'change', DescricaoLicenciamentoAtividade.onChangePontosLancamentoEfluenteTipo);
		DescricaoLicenciamentoAtividade.container.delegate('.ddlFontesGeracaoTipo', 'change', DescricaoLicenciamentoAtividade.onChangeFontesGeracaoTipo);
		DescricaoLicenciamentoAtividade.container.delegate('.divCkbTratamento label input[type="checkbox"]', 'click', DescricaoLicenciamentoAtividade.onClickCkTratamento);
		DescricaoLicenciamentoAtividade.container.delegate('.divCkbDestinoFinal label input[type="checkbox"]', 'click', DescricaoLicenciamentoAtividade.onClickCkDestinoFinal);
		DescricaoLicenciamentoAtividade.container.delegate('.btnExcluirLinha', 'click', DescricaoLicenciamentoAtividade.onClickRemoverTR);
		DescricaoLicenciamentoAtividade.container.delegate('.btnAddFonteAbastecimento', 'click', DescricaoLicenciamentoAtividade.onClickAddFonteAbastecimento);
		DescricaoLicenciamentoAtividade.container.delegate('.btnAddPontoLancamento', 'click', DescricaoLicenciamentoAtividade.onClickAddPontoLancamento);
		DescricaoLicenciamentoAtividade.container.delegate('.btnAddEfluenteLiquido', 'click', DescricaoLicenciamentoAtividade.onClickAddEfluenteLiquido);
		DescricaoLicenciamentoAtividade.container.delegate('.btnAddResiduoSolido', 'click', DescricaoLicenciamentoAtividade.onClickAddResiduoSolido);
		DescricaoLicenciamentoAtividade.container.delegate('.btnAddEmissaoAtmosferica', 'click', DescricaoLicenciamentoAtividade.onClickAddEmissaoAtmosferica);
		DescricaoLicenciamentoAtividade.container.delegate('.btnSalvar', 'click', DescricaoLicenciamentoAtividade.onClickSalvar);
		DescricaoLicenciamentoAtividade.container.delegate('.btnAvancar', 'click', DescricaoLicenciamentoAtividade.onClickAvancar);

		if (DescricaoLicenciamentoAtividade.settings.textoMerge) {
			DescricaoLicenciamentoAtividade.abrirModalRedireciona(DescricaoLicenciamentoAtividade.settings.textoMerge);
		}
	},
	getValorTexto: function (elemento) {
		return $(elemento, DescricaoLicenciamentoAtividade.container).val().trim();
	},
	getValorInt: function (elemento) {
		var valor = parseInt($(elemento, DescricaoLicenciamentoAtividade.container).val());
		return isNaN(valor) ? 0 : valor;
	},
	getValorFloat: function (elemento) {
		var valor = parseFloat($(elemento, DescricaoLicenciamentoAtividade.container).val());
		return isNaN(valor) ? 0 : valor;
	},
	getIschecked: function (elemento) {
		return $(elemento + ':checked', DescricaoLicenciamentoAtividade.container).length > 0;
	},
	gerarObjeto: function () {

		var descricaoLicenciamentoAtividade = {
			Id: DescricaoLicenciamentoAtividade.getValorInt('.hdnDscLicAtividadeId'),
			EmpreendimentoId: DescricaoLicenciamentoAtividade.getValorInt('.hdnEmpreendimentoId'),
			Tipo: DescricaoLicenciamentoAtividade.getValorInt('.hdnCaracterizacaoTipo'),
			RespAtividade: DescricaoLicenciamentoAtividade.getValorInt('.ddlRespAtiv'),
			BaciaHidrografica: DescricaoLicenciamentoAtividade.getValorTexto('.txtBaciaHidrografica'),
			ExisteAppUtil: DescricaoLicenciamentoAtividade.getIschecked('.rdbExisteAppSim'),
			TipoVegetacaoUtilCodigo: 0,
			ZonaAmortUC: parseInt($('#rdbZonaUC:checked', DescricaoLicenciamentoAtividade.container).val()),
			ZonaAmortUCNomeOrgaoAdm: DescricaoLicenciamentoAtividade.getValorTexto('.txtZonaAmortUCNome'),
			LocalizadaUC: DescricaoLicenciamentoAtividade.getIschecked('.rdbLocalizadoUCSim'),
			LocalizadaUCNomeOrgaoAdm: DescricaoLicenciamentoAtividade.getValorTexto('.txtLocalizadaUCNome'),
			PatrimonioHistorico: parseInt($('#rdbPatrimonioHistorico:checked', DescricaoLicenciamentoAtividade.container).val()),
			ResidentesEntorno: parseInt($('#rdbResidentesEntorno:checked', DescricaoLicenciamentoAtividade.container).val()),
			ResidentesEnternoDistancia: DescricaoLicenciamentoAtividade.getValorTexto('.txtResidentesEnternoDistancia'),
			AreaTerreno: DescricaoLicenciamentoAtividade.getValorTexto('.hdnAreaTerreno'),
			AreaUtil: Mascara.getFloatMask($('.txtAreaUtil', DescricaoLicenciamentoAtividade.container).val()) || 0.0,
			TotalFuncionarios: parseInt($('.txtTotalFuncionarios', DescricaoLicenciamentoAtividade.container).val()),
			HorasDias: DescricaoLicenciamentoAtividade.getValorTexto('.txtHorasDias'),
			DiasMes: parseInt($('.txtDiasMes', DescricaoLicenciamentoAtividade.container).val()),
			TurnosDia: parseInt($('.txtTurnosDia', DescricaoLicenciamentoAtividade.container).val()),
			ConsumoAguaLs: DescricaoLicenciamentoAtividade.getValorTexto('.txtConsumoAguaLs'),
			ConsumoAguaMh: DescricaoLicenciamentoAtividade.getValorTexto('.txtConsumoAguaMh'),
			ConsumoAguaMdia: DescricaoLicenciamentoAtividade.getValorTexto('.txtConsumoAguaMdia'),
			ConsumoAguaMmes: DescricaoLicenciamentoAtividade.getValorTexto('.txtConsumoAguaMmes'),
			TipoOutorgaId: DescricaoLicenciamentoAtividade.getValorInt('.ddlOutorgaAguaTipo'),
			Numero: DescricaoLicenciamentoAtividade.getValorTexto('.txtNumero'),
			FonteAbastecimentoAguaTipoId: null,
			PontoLancamentoTipoId: null,
			FontesAbastecimentoAgua: [],
			PontosLancamentoEfluente: [],
			EfluentesLiquido: [],
			ResiduosSolidosNaoInerte: [],
			EmissoesAtmosfericas: []
		};

		$('.ckbVegetacaoAreaUtil:checked', DescricaoLicenciamentoAtividade.container).each(function () {
			descricaoLicenciamentoAtividade.TipoVegetacaoUtilCodigo += DescricaoLicenciamentoAtividade.getValorInt(this);
		});

		if (DescricaoLicenciamentoAtividade.getValorInt('.ddlFontesAbastecimentoAguaTipo') == DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.NaoPossui.Id) {
			descricaoLicenciamentoAtividade.FonteAbastecimentoAguaTipoId = DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.NaoPossui.Id;
		}
		else {
			$('.gridFontesAbastecimentoAgua', DescricaoLicenciamentoAtividade.container).find('.hdnItemFonteAbastecimento').each(function (i) {
				var item = JSON.parse($(this).val());
				descricaoLicenciamentoAtividade.FontesAbastecimentoAgua.push(item);
			});
		}

		if (DescricaoLicenciamentoAtividade.getValorInt('.ddlPontosLancamentoEfluenteTipo') == DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.NaoPossui.Id) {
			descricaoLicenciamentoAtividade.PontoLancamentoTipoId = DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.NaoPossui.Id;
		}
		else {
			$('.gridPontoLancamento', DescricaoLicenciamentoAtividade.container).find('.hdnItemPontoLancamento').each(function (i) {
				var item = JSON.parse($(this).val());
				descricaoLicenciamentoAtividade.PontosLancamentoEfluente.push(item);
			});
		}

		$('.gridEfluenteLiquido', DescricaoLicenciamentoAtividade.container).find('.hdnItemEfluenteLiquido').each(function (i) {
			var item = JSON.parse($(this).val());
			item.Vazao = item.Vazao.toString();
			descricaoLicenciamentoAtividade.EfluentesLiquido.push(item);
		});

		$('.gridResiduoSolido', DescricaoLicenciamentoAtividade.container).find('.hdnItemResiduoSolidoNaoInerte').each(function (i) {
			var item = JSON.parse($(this).val());
			descricaoLicenciamentoAtividade.ResiduosSolidosNaoInerte.push(item);
		});

		$('.gridEmissaoAtmosferica', DescricaoLicenciamentoAtividade.container).find('.hdnItemEmissaoAtmosferica').each(function (i) {
			var item = JSON.parse($(this).val());
			descricaoLicenciamentoAtividade.EmissoesAtmosfericas.push(item);
		});

		return descricaoLicenciamentoAtividade;
	},
	abrirModalRedireciona: function (textoModal) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', DescricaoLicenciamentoAtividade.container).attr('href'));
			},
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: DescricaoLicenciamentoAtividade.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},
	onClickZonaUC: function () {
		$('.divZonaAmortUC', DescricaoLicenciamentoAtividade.container).addClass('hide');
		if (parseInt($(this).val()) == 1) {
			$('.divZonaAmortUC', DescricaoLicenciamentoAtividade.container).removeClass('hide');
			$('.txtZonaAmortUCNome', DescricaoLicenciamentoAtividade.container).val('');
		}
	},
	onClickPatrimonioHistorico: function () {
		$('.divPatrimonioHistorico,.divResidentesEntorno', DescricaoLicenciamentoAtividade.container).addClass('hide');
		$('.rdbrdbResidentesEntornoNao', DescricaoLicenciamentoAtividade.container).attr('checked', 'checked');
		if (parseInt($(this).val()) == 1) {
			$('.divPatrimonioHistorico', DescricaoLicenciamentoAtividade.container).removeClass('hide');
		}
	},
	onClickResidentesEntorno: function () {
		$('.divResidentesEntorno', DescricaoLicenciamentoAtividade.container).addClass('hide');
		if (parseInt($(this).val()) == 1) {
			$('.divResidentesEntorno', DescricaoLicenciamentoAtividade.container).removeClass('hide');
		}
	},
	onChangeFontesAbastecimentoAguaTipo: function () {

		var itemId = parseInt($(this).val());
		var divConsumo = [];

		$('.divFonteUso,.divGridFontesAbastecimentoAgua,.botaoAddFonteAbastecimento,.divConsumo,.fsEfluentesLiquidos', DescricaoLicenciamentoAtividade.container).removeClass('hide');
		$('.lblFonteUso', DescricaoLicenciamentoAtividade.container).empty();

		switch (itemId) {
			case DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.NaoPossui.Id:
				$('.divFonteUso,.divGridFontesAbastecimentoAgua,.botaoAddFonteAbastecimento,.divConsumo,.fsEfluentesLiquidos', DescricaoLicenciamentoAtividade.container).addClass('hide');
				divConsumo = $('.divConsumo', DescricaoLicenciamentoAtividade.container);
				divConsumo.find('input[type="text"]').val('');
				divConsumo.find('select').val(0);
				$('.gridFontesAbastecimentoAgua tbody', DescricaoLicenciamentoAtividade.container).empty();
				$('.gridEfluenteLiquido tbody', DescricaoLicenciamentoAtividade.container).empty();
				break;
			case DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.RedePublica.Id:
				$('.lblFonteUso', DescricaoLicenciamentoAtividade.container).text(DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.RedePublica.CampoRespectivo);
				break;
			case DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.Pocos.Id:
				$('.lblFonteUso', DescricaoLicenciamentoAtividade.container).text(DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.Pocos.CampoRespectivo);
				break;
			case DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.CursoDaguaRiosCorregosRiachos.Id:
				$('.lblFonteUso', DescricaoLicenciamentoAtividade.container).text(DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.CursoDaguaRiosCorregosRiachos.CampoRespectivo);
				break;
			case DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.LagoLagoa.Id:
				$('.lblFonteUso', DescricaoLicenciamentoAtividade.container).text(DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.LagoLagoa.CampoRespectivo);
				break;
			case DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.AguasCosteiras.Id:
				$('.lblFonteUso', DescricaoLicenciamentoAtividade.container).text(DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.AguasCosteiras.CampoRespectivo);
				break;
			default:
				$('.divFonteUso', DescricaoLicenciamentoAtividade.container).addClass('hide');
				break;
		}
	},
	onChangePontosLancamentoEfluenteTipo: function () {

		var itemId = parseInt($(this).val());

		$('.divPontoLancamento,.divGridPontoLancamento,.botaoAddPontoLancamento', DescricaoLicenciamentoAtividade.container).removeClass('hide');
		$('.lblPontoLancamento', DescricaoLicenciamentoAtividade.container).empty();

		switch (itemId) {
			case DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.NaoPossui.Id:
				$('.divPontoLancamento,.divGridPontoLancamento,.botaoAddPontoLancamento', DescricaoLicenciamentoAtividade.container).addClass('hide');
				$('.gridPontoLancamento tbody', DescricaoLicenciamentoAtividade.container).empty();
				break;
			case DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.CursoDaguaRiosCorregosRiachos.Id:
				$('.lblPontoLancamento', DescricaoLicenciamentoAtividade.container).text(DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.CursoDaguaRiosCorregosRiachos.CampoRespectivo);
				break;
			case DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.LagoLagoa.Id:
				$('.lblPontoLancamento', DescricaoLicenciamentoAtividade.container).text(DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.LagoLagoa.CampoRespectivo);
				break;
			case DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.AguasCosteiras.Id:
				$('.lblPontoLancamento', DescricaoLicenciamentoAtividade.container).text(DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.AguasCosteiras.CampoRespectivo);
				break;
			case DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.RedePluvialPublica.Id:
				$('.lblPontoLancamento', DescricaoLicenciamentoAtividade.container).text(DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.RedePluvialPublica.CampoRespectivo);
				break;
			case DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.RedeEsgoto.Id:
				$('.lblPontoLancamento', DescricaoLicenciamentoAtividade.container).text(DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.RedeEsgoto.CampoRespectivo);
				break;
			default:
				$('.divPontoLancamento', DescricaoLicenciamentoAtividade.container).addClass('hide');
				break;
		}
	},
	onChangeFontesGeracaoTipo: function () {

		var itemId = parseInt($(this).val());

		$('.divLiquidoEspecificar', DescricaoLicenciamentoAtividade.container).addClass('hide');

		if (itemId == DescricaoLicenciamentoAtividade.FontesGeracaoOutrosId) {
			$('.divLiquidoEspecificar', DescricaoLicenciamentoAtividade.container).removeClass('hide');
		}
	},
	onClickCkTratamento: function () {
		var itemId = parseInt($(this).val().split('|')[0]);
		if (itemId == DescricaoLicenciamentoAtividade.TratamentoOutrasFormasId && this.checked) {
			$('.divTratamentoEspecificar', DescricaoLicenciamentoAtividade.container).toggleClass('hide');
		}
		else if (itemId == DescricaoLicenciamentoAtividade.TratamentoOutrasFormasId && !this.checked) {
			$('.divTratamentoEspecificar', DescricaoLicenciamentoAtividade.container).toggleClass('hide');
		}
	},
	onClickCkDestinoFinal: function () {
		var itemId = parseInt($(this).val().split('|')[0]);
		if (itemId == DescricaoLicenciamentoAtividade.DestinoFinalOutrosId && this.checked) {
			$('.divDestinoFinalEspecificar', DescricaoLicenciamentoAtividade.container).toggleClass('hide');
		}
		else if (itemId == DescricaoLicenciamentoAtividade.DestinoFinalOutrosId && !this.checked) {
			$('.divDestinoFinalEspecificar', DescricaoLicenciamentoAtividade.container).toggleClass('hide');
		}
	},
	onClickRemoverTR: function () {
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable(DescricaoLicenciamentoAtividade.container.find('.dataGridTable'));
	},
	onClickAddFonteAbastecimento: function () {

		var arrayMsg = [];
		var trTemplate = null;
		var ddlFontesAbastecimentoAguaTipo = $('.ddlFontesAbastecimentoAguaTipo', DescricaoLicenciamentoAtividade.container);
		var txtFonteUso = $('.txtFonteUso', DescricaoLicenciamentoAtividade.container);
		var campo = '';
		var itemJaAdd = false;

		var fonteAbastecimento = {
			TipoId: parseInt(ddlFontesAbastecimentoAguaTipo.val()),
			TipoTexto: ddlFontesAbastecimentoAguaTipo.find('option').filter(':selected').text(),
			Descricao: txtFonteUso.val().trim()
		};

		var msg = {
			Tipo: DescricaoLicenciamentoAtividade.settings.Mensagens.InformeFonteUso.Tipo,
			Campo: DescricaoLicenciamentoAtividade.settings.Mensagens.InformeFonteUso.Campo,
			Texto: DescricaoLicenciamentoAtividade.settings.Mensagens.InformeFonteUso.Texto
		};

		if (fonteAbastecimento.TipoId == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.SelecioneTipoFonteAbastecimento);
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
			return;
		}

		$('.hdnItemFonteAbastecimento', '.gridFontesAbastecimentoAgua').each(function (i) {
			var item = JSON.parse($(this).val());
			if (fonteAbastecimento.TipoId == item.TipoId) {
				itemJaAdd = true;
				return;
			}
		});

		if (itemJaAdd) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.FonteAguaExistente);
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
			return;
		}

		switch (fonteAbastecimento.TipoId) {
			case DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.RedePublica.Id:
				campo = DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.RedePublica.CampoRespectivo.replace(' *', '');
				if (fonteAbastecimento.Descricao == "") {
					msg.Texto = msg.Texto.replace('{0}', campo);
					arrayMsg.push(msg);
				}
				else {
					fonteAbastecimento.Descricao = campo + ' - ' + fonteAbastecimento.Descricao;
				}
				break;
			case DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.Pocos.Id:
				campo = DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.Pocos.CampoRespectivo.replace(' *', '');
				if (fonteAbastecimento.Descricao == "") {
					msg.Texto = msg.Texto.replace('{0}', campo);
					arrayMsg.push(msg);
				}
				else {
					fonteAbastecimento.Descricao = campo + ' - ' + fonteAbastecimento.Descricao;
				}
				break;
			case DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.CursoDaguaRiosCorregosRiachos.Id:
				campo = DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.CursoDaguaRiosCorregosRiachos.CampoRespectivo.replace(' *', '');
				if (fonteAbastecimento.Descricao == "") {
					msg.Texto = msg.Texto.replace('{0}', campo);
					arrayMsg.push(msg);
				}
				else {
					fonteAbastecimento.Descricao = campo + ' - ' + fonteAbastecimento.Descricao;
				}
				break;
			case DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.LagoLagoa.Id:
				campo = DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.LagoLagoa.CampoRespectivo.replace(' *', '');
				if (fonteAbastecimento.Descricao == "") {
					msg.Texto = msg.Texto.replace('{0}', campo);
					arrayMsg.push(msg);
				}
				else {
					fonteAbastecimento.Descricao = campo + ' - ' + fonteAbastecimento.Descricao;
				}
				break;
			case DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.AguasCosteiras.Id:
				campo = DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.AguasCosteiras.CampoRespectivo.replace(' *', '');
				if (fonteAbastecimento.Descricao == "") {
					msg.Texto = msg.Texto.replace('{0}', campo);
					arrayMsg.push(msg);
				}
				else {
					fonteAbastecimento.Descricao = campo + ' - ' + fonteAbastecimento.Descricao;
				}
				break;
		}

		if (arrayMsg.length > 0) {
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
			return;
		}

		trTemplate = $('.trFonteAbastecimentoTemplate', DescricaoLicenciamentoAtividade.container).clone();
		trTemplate.removeAttr('class');
		$('.spanTipoTexto', trTemplate).text(fonteAbastecimento.TipoTexto).attr('title', fonteAbastecimento.TipoTexto);
		$('.spanDescricao', trTemplate).text(fonteAbastecimento.Descricao).attr('title', fonteAbastecimento.Descricao);

		$('.hdnItemFonteAbastecimento', trTemplate).val($.toJSON(fonteAbastecimento));
		$('.gridFontesAbastecimentoAgua tbody', DescricaoLicenciamentoAtividade.container).append(trTemplate);

		txtFonteUso.val('');
		ddlFontesAbastecimentoAguaTipo.val(0).change();
		Listar.atualizarEstiloTable(DescricaoLicenciamentoAtividade.container.find('.dataGridTable'));
		Mensagem.gerar(DescricaoLicenciamentoAtividade.container, []);
	},
	onClickAddPontoLancamento: function () {

		var arrayMsg = [];
		var trTemplate = null;
		var ddlPontosLancamentoEfluenteTipo = $('.ddlPontosLancamentoEfluenteTipo', DescricaoLicenciamentoAtividade.container);
		var txtPontoLancamento = $('.txtPontoLancamento', DescricaoLicenciamentoAtividade.container);
		var campo = '';
		var itemJaAdd = false;

		var pontoLancamento = {
			TipoId: parseInt(ddlPontosLancamentoEfluenteTipo.val()),
			TipoTexto: ddlPontosLancamentoEfluenteTipo.find('option').filter(':selected').text(),
			Descricao: txtPontoLancamento.val().trim()
		};

		var msg = {
			Tipo: DescricaoLicenciamentoAtividade.settings.Mensagens.InformePontoLancamento.Tipo,
			Campo: DescricaoLicenciamentoAtividade.settings.Mensagens.InformePontoLancamento.Campo,
			Texto: DescricaoLicenciamentoAtividade.settings.Mensagens.InformePontoLancamento.Texto
		};

		if (pontoLancamento.TipoId == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.SelecionePontoLancamento);
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
			return;
		}

		$('.hdnItemPontoLancamento', '.gridPontoLancamento').each(function (i) {
			var item = JSON.parse($(this).val());
			if (pontoLancamento.TipoId == item.TipoId) {
				itemJaAdd = true;
				return;
			}
		});

		if (itemJaAdd) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.PontoLancamentoExistente);
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
			return;
		}

		switch (pontoLancamento.TipoId) {
			case DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.CursoDaguaRiosCorregosRiachos.Id:
				campo = DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.CursoDaguaRiosCorregosRiachos.CampoRespectivo.replace(' *', '');
				if (pontoLancamento.Descricao == "") {
					msg.Texto = msg.Texto.replace('{0}', campo);
					arrayMsg.push(msg);
				}
				else {
					pontoLancamento.Descricao = campo + ' - ' + pontoLancamento.Descricao;
				}
				break;
			case DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.LagoLagoa.Id:
				campo = DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.LagoLagoa.CampoRespectivo.replace(' *', '');
				if (pontoLancamento.Descricao == "") {
					msg.Texto = msg.Texto.replace('{0}', campo);
					arrayMsg.push(msg);
				}
				else {
					pontoLancamento.Descricao = campo + ' - ' + pontoLancamento.Descricao;
				}
				break;
			case DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.AguasCosteiras.Id:
				campo = DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.AguasCosteiras.CampoRespectivo.replace(' *', '');
				if (pontoLancamento.Descricao == "") {
					msg.Texto = msg.Texto.replace('{0}', campo);
					arrayMsg.push(msg);
				}
				else {
					pontoLancamento.Descricao = campo + ' - ' + pontoLancamento.Descricao;
				}
				break;
			case DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.RedePluvialPublica.Id:
				campo = DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.RedePluvialPublica.CampoRespectivo.replace(' *', '');
				if (pontoLancamento.Descricao == "") {
					msg.Texto = msg.Texto.replace('{0}', campo);
					arrayMsg.push(msg);
				}
				else {
					pontoLancamento.Descricao = campo + ' - ' + pontoLancamento.Descricao;
				}
				break;
			case DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.RedeEsgoto.Id:
				campo = DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.RedeEsgoto.CampoRespectivo.replace(' *', '');
				if (pontoLancamento.Descricao == "") {
					msg.Texto = msg.Texto.replace('{0}', campo);
					arrayMsg.push(msg);
				}
				else {
					pontoLancamento.Descricao = campo + ' - ' + pontoLancamento.Descricao;
				}
				break;
			case DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.NaoPossui.Id:
				campo = DescricaoLicenciamentoAtividade.PontosLancamentoEfluente.RedeEsgoto.CampoRespectivo.replace(' *', '');
				if (pontoLancamento.Descricao == "") {
					msg.Texto = msg.Texto.replace('{0}', campo);
					arrayMsg.push(msg);
				}
				else {
					pontoLancamento.Descricao = campo + ' - ' + pontoLancamento.Descricao;
				}
				break;
		}

		if (arrayMsg.length > 0) {
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
			return;
		}

		trTemplate = $('.trPontoLancamentoTemplate', DescricaoLicenciamentoAtividade.container).clone();
		trTemplate.removeAttr('class');
		$('.spanTipoTexto', trTemplate).text(pontoLancamento.TipoTexto).attr('title', pontoLancamento.TipoTexto);
		$('.spanDescricao', trTemplate).text(pontoLancamento.Descricao).attr('title', pontoLancamento.Descricao);

		$('.hdnItemPontoLancamento', trTemplate).val($.toJSON(pontoLancamento));
		$('.gridPontoLancamento tbody', DescricaoLicenciamentoAtividade.container).append(trTemplate);

		txtPontoLancamento.val('');
		ddlPontosLancamentoEfluenteTipo.val(0).change();
		Listar.atualizarEstiloTable(DescricaoLicenciamentoAtividade.container.find('.dataGridTable'));
		Mensagem.gerar(DescricaoLicenciamentoAtividade.container, []);
	},
	onClickAddEfluenteLiquido: function () {

		var arrayMsg = [];
		var trTemplate = null;
		var ddlFontesGeracaoTipo = $('.ddlFontesGeracaoTipo', DescricaoLicenciamentoAtividade.container);
		var txtVazao = $('.txtVazao', DescricaoLicenciamentoAtividade.container);
		var ddlUnidadeTipo = $('.ddlUnidadeTipo', DescricaoLicenciamentoAtividade.container);
		var txtSisTratamento = $('.txtSisTratamento', DescricaoLicenciamentoAtividade.container);
		var txtEflLiquidoEspecificar = $('.txtEflLiquidoEspecificar', DescricaoLicenciamentoAtividade.container);
		var itemJaAdd = false;

		var efluenteLiquido = {
			TipoId: parseInt(ddlFontesGeracaoTipo.val()),
			TipoTexto: ddlFontesGeracaoTipo.find('option').filter(':selected').text(),
			Vazao: txtVazao.val().trim(),
			UnidadeId: parseInt(ddlUnidadeTipo.val()),
			UnidadeTexto: ddlUnidadeTipo.find('option').filter(':selected').text(),
			SistemaTratamento: txtSisTratamento.val().trim(),
			Descricao: txtEflLiquidoEspecificar.val().trim()
		};

		$('.hdnItemEfluenteLiquido', '.gridEfluenteLiquido').each(function (i) {
			var item = JSON.parse($(this).val());
			if (efluenteLiquido.TipoId == item.TipoId && item.TipoId != DescricaoLicenciamentoAtividade.FontesGeracaoOutrosId) {
				itemJaAdd = true;
				return;
			}
		});

		if (itemJaAdd) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.TipoFonteGeracaoExistente);
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
			return;
		}

		if (efluenteLiquido.TipoId == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.SelecioneTipoFonteGeracao);
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
		}

		if (efluenteLiquido.Vazao == "") {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeVazao);
		} else if (!new RegExp(/^(\d{1,6}|\d{1,6},\d{0,2})$/).test(efluenteLiquido.Vazao)) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeVazaoValida);
		}

		if (efluenteLiquido.UnidadeId == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeUnidade);
		}

		if (efluenteLiquido.SistemaTratamento == "") {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeSisTratamento);
		}

		if (efluenteLiquido.TipoId == DescricaoLicenciamentoAtividade.FontesGeracaoOutrosId) {
			if (efluenteLiquido.Descricao == "") {
				arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeOutraFonteGeracao);
			}
		}
		else {
			efluenteLiquido.Descricao = '';
		}

		if (arrayMsg.length > 0) {
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
			return;
		}

		trTemplate = $('.trEfluenteLiquidoTemplate', DescricaoLicenciamentoAtividade.container).clone();
		trTemplate.removeAttr('class');

		$('.spanFonteGeracao', trTemplate)
			.text(((efluenteLiquido.TipoId != DescricaoLicenciamentoAtividade.FontesGeracaoOutrosId) ? efluenteLiquido.TipoTexto : efluenteLiquido.Descricao))
			.attr('title', ((efluenteLiquido.TipoId != DescricaoLicenciamentoAtividade.FontesGeracaoOutrosId) ? efluenteLiquido.TipoTexto : efluenteLiquido.Descricao));
		$('.spanVazao', trTemplate).text(efluenteLiquido.Vazao).attr('title', efluenteLiquido.Vazao);
		$('.spanUnidade', trTemplate).text(efluenteLiquido.UnidadeTexto).attr('title', efluenteLiquido.UnidadeTexto);
		$('.spanSisTratamento', trTemplate).text(efluenteLiquido.SistemaTratamento).attr('title', efluenteLiquido.SistemaTratamento);

		$('.hdnItemEfluenteLiquido', trTemplate).val($.toJSON(efluenteLiquido));
		$('.gridEfluenteLiquido tbody', DescricaoLicenciamentoAtividade.container).append(trTemplate);

		ddlFontesGeracaoTipo.val(0).change();
		txtVazao.val('');
		ddlUnidadeTipo.val(0).change();
		txtSisTratamento.val('');
		txtEflLiquidoEspecificar.val('');
		Listar.atualizarEstiloTable(DescricaoLicenciamentoAtividade.container.find('.dataGridTable'));
		Mensagem.gerar(DescricaoLicenciamentoAtividade.container, []);
	},
	onClickAddResiduoSolido: function () {
		var arrayMsg = [];
		var trTemplate = null;
		var txtClasseRediduo = $('.txtClasseRediduo', DescricaoLicenciamentoAtividade.container);
		var txtTipoRediduo = $('.txtTipoRediduo', DescricaoLicenciamentoAtividade.container);
		var txtTratamentoEspecificar = $('.txtTratamentoEspecificar', DescricaoLicenciamentoAtividade.container);
		var txtDestinoFinalEspecificar = $('.txtDestinoFinalEspecificar', DescricaoLicenciamentoAtividade.container);
		var arrayAcondicionamento = $('.divCkbAcondicionamento label input[type="checkbox"]:checked', DescricaoLicenciamentoAtividade.container);
		var arrayEstocagem = $('.divCkbEstocagem label input[type="checkbox"]:checked', DescricaoLicenciamentoAtividade.container);
		var arrayTratamento = $('.divCkbTratamento label input[type="checkbox"]:checked', DescricaoLicenciamentoAtividade.container);
		var arrayDestinoFinal = $('.divCkbDestinoFinal label input[type="checkbox"]:checked', DescricaoLicenciamentoAtividade.container);
		var txtTratamentoEspecificarVisivel = $('.txtTratamentoEspecificar:visible', DescricaoLicenciamentoAtividade.container).length > 0;
		var txtDestinoFinalEspecificarVisivel = $('.txtDestinoFinalEspecificar:visible', DescricaoLicenciamentoAtividade.container).length > 0;

		var residuoSolido = {
			ClasseResiduo: txtClasseRediduo.val().trim(),
			Tipo: txtTipoRediduo.val().trim(),
			AcondicionamentoCodigo: 0,
			AcondicionamentoTexto: '',
			EstocagemCodigo: 0,
			EstocagemTexto: '',
			TratamentoCodigo: 0,
			TratamentoTexto: '',
			TratamentoDescricao: txtTratamentoEspecificar.val().trim(),
			DestinoFinalCodigo: 0,
			DestinoFinalTexto: '',
			DestinoFinalDescricao: txtDestinoFinalEspecificar.val().trim()
		};

		//Descrição do value
		//checkbox[value="Id|Codigo"]

		arrayAcondicionamento.each(function (i) {
			residuoSolido.AcondicionamentoCodigo += parseInt($(this).val().split('|')[1]);
			residuoSolido.AcondicionamentoTexto += $(this).attr('title') + '; '
		});

		arrayEstocagem.each(function (i) {
			residuoSolido.EstocagemCodigo += parseInt($(this).val().split('|')[1]);
			residuoSolido.EstocagemTexto += $(this).attr('title') + '; '
		});

		arrayTratamento.each(function (i) {
			var codigo = parseInt($(this).val().split('|')[1]);
			var id = parseInt($(this).val().split('|')[0]);
			residuoSolido.TratamentoCodigo += codigo;
			if (id == DescricaoLicenciamentoAtividade.TratamentoOutrasFormasId) {
				residuoSolido.TratamentoTexto += $(this).attr('title') + ' - ' + residuoSolido.TratamentoDescricao + '; ';
			} else {
				residuoSolido.TratamentoTexto += $(this).attr('title') + '; ';
			}
		});

		arrayDestinoFinal.each(function (i) {
			var codigo = parseInt($(this).val().split('|')[1]);
			var id = parseInt($(this).val().split('|')[0]);
			residuoSolido.DestinoFinalCodigo += parseInt($(this).val().split('|')[1]);
			if (id == DescricaoLicenciamentoAtividade.DestinoFinalOutrosId) {
			    residuoSolido.DestinoFinalTexto += $(this).attr('title') + ' - ' + residuoSolido.DestinoFinalDescricao + '; ';
			} else {
				residuoSolido.DestinoFinalTexto += $(this).attr('title') + '; ';
			}
		});

		if (!txtTratamentoEspecificarVisivel) {
			residuoSolido.TratamentoDescricao = '';
		}

		if (!txtDestinoFinalEspecificarVisivel) {
			residuoSolido.DestinoFinalDescricao = '';
		}

		if (residuoSolido.ClasseResiduo == '') {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeClasseResiduo);
		}

		if (residuoSolido.Tipo == '') {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeTipoResiduo);
		}

		if (residuoSolido.AcondicionamentoCodigo == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeAcondicionamento);
		}

		if (residuoSolido.EstocagemCodigo == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeEstocagem);
		}

		if (residuoSolido.TratamentoCodigo == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeTratamento);
		}

		if (residuoSolido.DestinoFinalCodigo == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeDestinoFinal);
		}

		if (txtTratamentoEspecificarVisivel && residuoSolido.TratamentoDescricao == '') {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeTratamentoOutro);
		}

		if (txtDestinoFinalEspecificarVisivel && residuoSolido.DestinoFinalDescricao == '') {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeDestinoFinalOutro);
		}

		if (arrayMsg.length > 0) {
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
			return;
		}

		trTemplate = $('.trResiduoSolidoTemplate', DescricaoLicenciamentoAtividade.container).clone();
		trTemplate.removeAttr('class');

		$('.spanClasse', trTemplate).text(residuoSolido.ClasseResiduo).attr('title', residuoSolido.ClasseResiduo);
		$('.spanTipo', trTemplate).text(residuoSolido.Tipo).attr('title', residuoSolido.Tipo);
		$('.spanAcondicionamento', trTemplate).text(residuoSolido.AcondicionamentoTexto).attr('title', residuoSolido.AcondicionamentoTexto);
		$('.spanEstocagem', trTemplate).text(residuoSolido.EstocagemTexto).attr('title', residuoSolido.EstocagemTexto);
		$('.spanTratamento', trTemplate).text(residuoSolido.TratamentoTexto).attr('title', residuoSolido.TratamentoTexto);
		$('.spanDestinoFinal', trTemplate).text(residuoSolido.DestinoFinalTexto).attr('title', residuoSolido.DestinoFinalTexto);

		$('.hdnItemResiduoSolidoNaoInerte', trTemplate).val($.toJSON(residuoSolido));
		$('.gridResiduoSolido tbody', DescricaoLicenciamentoAtividade.container).append(trTemplate);

		txtClasseRediduo.val('');
		txtTipoRediduo.val('');
		txtTratamentoEspecificar.val('');
		txtDestinoFinalEspecificar.val('');
		arrayAcondicionamento.each(function () { this.checked = true; }).click();
		arrayEstocagem.each(function () { this.checked = true; }).click();
		arrayTratamento.each(function () { this.checked = true; }).click();
		arrayDestinoFinal.each(function () { this.checked = true; }).click();
		Listar.atualizarEstiloTable(DescricaoLicenciamentoAtividade.container.find('.dataGridTable'));

		Mensagem.gerar(DescricaoLicenciamentoAtividade.container, []);
	},
	onClickAddEmissaoAtmosferica: function () {
		var arrayMsg = [];
		var trTemplate = null;
		var ddlCombustivel = $('.ddlCombustivel', DescricaoLicenciamentoAtividade.container);
		var txtSubstanciaEmitida = $('.txtSubstanciaEmitida', DescricaoLicenciamentoAtividade.container);
		var txtEquipamentoControle = $('.txtEquipamentoControle', DescricaoLicenciamentoAtividade.container);
		var itemJaAdd = false;

		var emissaoAtmosferica = {
			TipoCombustivelId: parseInt(ddlCombustivel.val()),
			TipoCombustivelTexto: ddlCombustivel.find('option').filter(':selected').text(),
			SubstanciaEmitida: txtSubstanciaEmitida.val().trim(),
			EquipamentoControle: txtEquipamentoControle.val().trim()
		};

		$('.hdnItemEmissaoAtmosferica', '.gridEmissaoAtmosferica').each(function (i) {
			var item = JSON.parse($(this).val());
			if (emissaoAtmosferica.TipoCombustivelId == item.TipoCombustivelId) {
				itemJaAdd = true;
				return;
			}
		});

		if (itemJaAdd) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.CombustivelExistente);
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
			return;
		}

		if (emissaoAtmosferica.TipoCombustivelId == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.SelecioneCombustivel);
		}

		if (emissaoAtmosferica.SubstanciaEmitida == "") {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeSubstancia);
		}

		if (emissaoAtmosferica.EquipamentoControle == "") {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeEquipamento);
		}

		if (arrayMsg.length > 0) {
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
			return;
		}

		trTemplate = $('.trEmissaoAtmosferica', DescricaoLicenciamentoAtividade.container).clone();
		trTemplate.removeAttr('class');

		$('.spanCombutivel', trTemplate).text(emissaoAtmosferica.TipoCombustivelTexto).attr('title', emissaoAtmosferica.TipoCombustivelTexto);
		$('.spanSubstanciaEmitida', trTemplate).text(emissaoAtmosferica.SubstanciaEmitida).attr('title', emissaoAtmosferica.SubstanciaEmitida);
		$('.spanEquipamentoControle', trTemplate).text(emissaoAtmosferica.EquipamentoControle).attr('title', emissaoAtmosferica.EquipamentoControle);

		$('.hdnItemEmissaoAtmosferica', trTemplate).val($.toJSON(emissaoAtmosferica));
		$('.gridEmissaoAtmosferica tbody', DescricaoLicenciamentoAtividade.container).append(trTemplate);

		ddlCombustivel.val(0).change();
		txtSubstanciaEmitida.val('');
		txtEquipamentoControle.val('');
		Listar.atualizarEstiloTable(DescricaoLicenciamentoAtividade.container.find('.dataGridTable'));

		Mensagem.gerar(DescricaoLicenciamentoAtividade.container, []);
	},
	onClickSalvar: function () {
		Mensagem.limpar(DescricaoLicenciamentoAtividade.container);

		var arrayMsg = [];
		var descricaoLicenciamentoAtividade = DescricaoLicenciamentoAtividade.gerarObjeto();

		if (descricaoLicenciamentoAtividade.RespAtividade == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.SelecioneResponsavel);
		}

		if (isNaN(descricaoLicenciamentoAtividade.ZonaAmortUC)) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeZonaUC);
		} else {
			descricaoLicenciamentoAtividade.ZonaAmortUC = descricaoLicenciamentoAtividade.ZonaAmortUC == 1;
		}

		if (isNaN(descricaoLicenciamentoAtividade.PatrimonioHistorico)) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformePatrimonio);
		} else {
			descricaoLicenciamentoAtividade.PatrimonioHistorico = descricaoLicenciamentoAtividade.PatrimonioHistorico == 1;
		}

		if (isNaN(descricaoLicenciamentoAtividade.ResidentesEntorno)) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeResidencia);
		} else {
			descricaoLicenciamentoAtividade.ResidentesEntorno = descricaoLicenciamentoAtividade.ResidentesEntorno == 1;
		}

		descricaoLicenciamentoAtividade.ResidentesEnternoDistancia = parseFloat(descricaoLicenciamentoAtividade.ResidentesEnternoDistancia);
		descricaoLicenciamentoAtividade.ResidentesEnternoDistancia = isNaN(descricaoLicenciamentoAtividade.ResidentesEnternoDistancia) ? 0 : descricaoLicenciamentoAtividade.ResidentesEnternoDistancia;

		if (descricaoLicenciamentoAtividade.ResidentesEntorno && descricaoLicenciamentoAtividade.ResidentesEnternoDistancia == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeDistancia);
		} else {
			descricaoLicenciamentoAtividade.ResidentesEnternoDistancia = descricaoLicenciamentoAtividade.ResidentesEnternoDistancia.toString();
		}

		if (descricaoLicenciamentoAtividade.AreaUtil == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeAreaUtil);
		}

		if (!descricaoLicenciamentoAtividade.FonteAbastecimentoAguaTipoId) {
			if (descricaoLicenciamentoAtividade.FontesAbastecimentoAgua.length == 0) {
				arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.AddFonteAbastecimento);
			}
		}

		if (!descricaoLicenciamentoAtividade.PontoLancamentoTipoId) {
			if (descricaoLicenciamentoAtividade.PontosLancamentoEfluente.length == 0) {
				arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.AddPontoLancamento);
			}
		}

		if (descricaoLicenciamentoAtividade.FonteAbastecimentoAguaTipoId != DescricaoLicenciamentoAtividade.FontesAbastecimentoAgua.NaoPossui.Id) {
			if (Mascara.getFloatMask(descricaoLicenciamentoAtividade.ConsumoAguaLs) == 0 &&
				Mascara.getFloatMask(descricaoLicenciamentoAtividade.ConsumoAguaMdia) == 0 &&
				Mascara.getFloatMask(descricaoLicenciamentoAtividade.ConsumoAguaMh) == 0 &&
				Mascara.getFloatMask(descricaoLicenciamentoAtividade.ConsumoAguaMmes) == 0) {
				arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.InformeConsumo);
			}

			if (descricaoLicenciamentoAtividade.EfluentesLiquido.length == 0) {
				arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.AddFonteGeracao);
			}
		}

		if (descricaoLicenciamentoAtividade.ResiduosSolidosNaoInerte.length == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.AddResiduoSolidoNaoInerte);
		}

		if (descricaoLicenciamentoAtividade.EmissoesAtmosfericas.length == 0) {
			arrayMsg.push(DescricaoLicenciamentoAtividade.settings.Mensagens.AddEmissoesAtm);
		}

		if (arrayMsg.length > 0) {
			Mensagem.gerar(DescricaoLicenciamentoAtividade.container, arrayMsg);
			return;
		}

		if (descricaoLicenciamentoAtividade.TipoOutorgaId == 0) {
			descricaoLicenciamentoAtividade.TipoOutorgaId = null;
		}

		MasterPage.carregando(true);

		$.ajax({ url: DescricaoLicenciamentoAtividade.settings.urls.Salvar,
			data: JSON.stringify({ dscLicAtividade: descricaoLicenciamentoAtividade, isCadastrarCaracterizacao: $('.hdnIsCadastrarCaracterizacao', DescricaoLicenciamentoAtividade.container).val().toLocaleLowerCase() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, DescricaoLicenciamentoAtividade.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					$('.spanAvancar', DescricaoLicenciamentoAtividade.container).removeClass('hide');
					Mensagem.gerar(DescricaoLicenciamentoAtividade.container, [DescricaoLicenciamentoAtividade.settings.Mensagens.DscLicAtvSalvoSucesso]);
					DescricaoLicenciamentoAtividade.settings.urls.Redirecionar = response.UrlRedirecionar;
					ContainerAcoes.load(DescricaoLicenciamentoAtividade.container, { limparContainer: false, botoes: new Array({ label: 'Avançar', callBack: DescricaoLicenciamentoAtividade.onClickAvancar }) });
					$('.hdnDscLicAtividadeId', DescricaoLicenciamentoAtividade.container).val(response.AtividadeId);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(DescricaoLicenciamentoAtividade.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},
	onClickAvancar: function () {
		MasterPage.redireciona(DescricaoLicenciamentoAtividade.settings.urls.Redirecionar);
	}
};
/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

RegistroAtividadeFlorestal = {
	settings: {
		caracterizacaoID: 0,
		empreendimentoID: 0,
		urls: {
			salvar: null
		},
		mensagens: null,
		idsTela: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(RegistroAtividadeFlorestal.settings, options); }

		RegistroAtividadeFlorestal.container = MasterPage.getContent(container);
		RegistroAtividadeFlorestal.container.delegate('.btnSalvar', 'click', RegistroAtividadeFlorestal.salvar);
		RegistroAtividadeFlorestal.container.delegate('.rdoPossuiNumero', 'click', RegistroAtividadeFlorestal.clickPossuiNumero);

		Consumo.load(RegistroAtividadeFlorestal.container);
		TituloAdicionar.load(RegistroAtividadeFlorestal.container);
		$('input,textarea', RegistroAtividadeFlorestal.container).attr('autocomplete', 'off');
	},

	clickPossuiNumero: function () {
		var value = $(this).val().toString();
		var numero = $('.hdnNumero', RegistroAtividadeFlorestal.container).val();

		if (value == "1") {
			$('.txtNumeroRegistro', RegistroAtividadeFlorestal.container).val('').removeAttr('disabled').removeClass('disabled');
		} else {
			$('.txtNumeroRegistro', RegistroAtividadeFlorestal.container).val(numero ? numero : 'Gerado automaticamente').attr('disabled', 'disabled').addClass('disabled');
		}
	},

	obter: function () {
		var container = RegistroAtividadeFlorestal.container.find('.divPossuiNumero');

		var obj = {
			Id: RegistroAtividadeFlorestal.settings.caracterizacaoID,
			EmpreendimentoId: RegistroAtividadeFlorestal.settings.empreendimentoID,
			Consumos: Consumo.obterConsumos(),
			NumeroRegistro: $('.txtNumeroRegistro', container).val(),
			PossuiNumero: $('.rdoPossuiNumero:checked', container).val()
		};

		if (obj.PossuiNumero == $('.rbSim', container).val()) {
			obj.PossuiNumero = true;
		} else if (obj.PossuiNumero == $('.rbNao', container).val()) {
			obj.PossuiNumero = false;
			obj.NumeroRegistro = $('.hdnNumero', container).val();
		}

		return obj;
	},

	salvar: function () {
		Mensagem.limpar(RegistroAtividadeFlorestal.container);
		var obj = RegistroAtividadeFlorestal.obter();

		MasterPage.carregando(true);
		$.ajax({
			url: RegistroAtividadeFlorestal.settings.urls.salvar,
			data: JSON.stringify(obj),
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
					Mensagem.gerar(RegistroAtividadeFlorestal.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}

Consumo = {
	settings: {
		urls: {
			validarConsumo: null,
			validarFonte: null
		}
	},
	container: null,

	load: function (container) {
		Consumo.container = container;
		container.delegate('.rbPossuiLicencaAmbiental', 'click', Consumo.clickPossuiLicenca);
		container.delegate('.btnAddFonte', 'click', Consumo.addFonte);
		container.delegate('.btnExcluirFonte', 'click', Consumo.excluirFonte);
		container.delegate('.ddlAtividades', 'change', Consumo.changeAtividade);

		Consumo.configurarAssociar();
		$('.fsConsumo').each(function () { Consumo.changeAtividade(null, this); });

		Listar.atualizarEstiloTable($('.tabFontes', container));
		container.delegate('.rbDispensadoOrgaoEmissor', 'change', Consumo.orgaoEmissorDispensado);
	},

	orgaoEmissorDispensado: function () {
		var containerDispensado = $(this).closest('.divDispensado');
		if ($('.rbEmitidoIDAF', containerDispensado).is(':checked')) {
			$('.txtOrgaoExpedidor', containerDispensado).addClass('disabled').attr('disabled', 'disabled');
			$('.txtOrgaoExpedidor', containerDispensado).val('Instituto de Defesa Agropecuária e Florestal do Espírito Santo');
		} else {
			$('.txtOrgaoExpedidor', containerDispensado).removeClass('disabled').removeAttr('disabled');
			$('.txtOrgaoExpedidor', containerDispensado).val('');
		}
	},

	changeAtividade: function (e, container) {
		if (!container) {
			container = $(this).closest('fieldset');
		}

		var atividade = $('.ddlAtividades', container).val();
		var ocultarFonte = (
			atividade == RegistroAtividadeFlorestal.settings.idsTela.FabricanteMotosserra ||
			atividade == RegistroAtividadeFlorestal.settings.idsTela.ComercianteMotosserra);

		$('.divConteudoFontesEnergia', container).toggleClass('hide', ocultarFonte);
	},

	configurarAssociar: function () {
		$('.divConteudoConsumos', Consumo.container).associarMultiplo({
			'minItens': 1,
			'onAdicionarClick': Consumo.addConsumo,
			'onItemAdicionado': Consumo.afterAddConsumo,
			'msgObrigatoriedade': RegistroAtividadeFlorestal.settings.mensagens.CamposObrigatorio,
			'tituloExcluir': 'Excluir Consumo',
			'msgExcluir': RegistroAtividadeFlorestal.settings.mensagens.ExcluirConsumo.Texto
		});
	},

	configurarIndex: function (index, container) {
		var containerConsumo = $('.fsConsumo', container);
		containerConsumo.attr('id', containerConsumo.attr('id') + index);
		$('.hdnConsumoIndex', container).val(index);

		containerConsumo.find('input[type=text], input[type=radio], select').each(function (i, item) {
			$(item).attr('id', $(item).attr('id') + index);
			$(item).attr('name', $(item).attr('name') + index);
		});
	},

	clickPossuiLicenca: function () {
		var container = $(this).closest('.divConteudoConsumo');
		var value = $(this).val();

		$('.divTituloAdicionar', container).toggleClass('hide', (value != $('.rbSim', container).val()));
		$('.divDispensado', container).toggleClass('hide', (value != $('.rbDispensado', container).val()));

		TituloAdicionar.limparCampos(true, $('.divTituloAdicionar', container));

		var containerDispensado = $('.divDispensado', container);
		$('.rbDispensadoOrgaoEmissor', containerDispensado).attr('checked', false);
		$('.txtTituloModelo', containerDispensado).val('');
		$('.txtProtocoloNumero', containerDispensado).val('');
		$('.txtOrgaoExpedidor', containerDispensado).val('').removeClass('disabled').removeAttr('disabled');
	},

	addConsumo: function (item, extra) {
		Mensagem.limpar(Consumo.container);

		var lista = Consumo.obterConsumos();
		var ehValido = MasterPage.validarAjax(
			Consumo.settings.urls.validarConsumo,
			{ consumoLista: lista },
			Consumo.container, false).EhValido;

		return ehValido;
	},

	afterAddConsumo: function (novoItem) {
		Mascara.load(novoItem);
		$('.hdnConsumoId', novoItem).val(-1);

		var contador = $('.asmItens .asmItemContainer', Consumo.container).length - 1;
		Consumo.configurarIndex(contador, novoItem);
	},

	addFonte: function () {
		Mensagem.limpar(Consumo.container);
		var container = $(this).closest('.fsConsumo');
		var atividade = $('.ddlAtividades', container).val();

		if (atividade == RegistroAtividadeFlorestal.settings.idsTela.FabricanteMotosserra ||
			atividade == RegistroAtividadeFlorestal.settings.idsTela.ComercianteMotosserra) {
			return;
		}

		var item = {
			Id: 0,
			TipoId: $('.ddlFonteTipos', container).val(),
			TipoTexto: $('.ddlFonteTipos :selected', container).text(),
			UnidadeId: $('.ddlUnidades', container).val(),
			UnidadeTexto: $('.ddlUnidades :selected', container).text(),
			Qde: Mascara.getFloatMask($('.txtQde', container).val()),
			QdeFlorestaPlantada: Mascara.getFloatMask($('.txtQdeFlorestaPlantada', container).val()),
			QdeFlorestaNativa: Mascara.getFloatMask($('.txtQdeFlorestaNativa', container).val()),
			QdeOutroEstado: Mascara.getFloatMask($('.txtQdeOutroEstado', container).val())
		};

		var lista = new Array();
		$('.tabFontes tbody tr', container).each(function (i, linha) {
			lista.push(JSON.parse($('.hdnFonteJSON', linha).val()));
		});

		var consumoIndex = $('.hdnConsumoIndex', container).val();
		var ehValido = MasterPage.validarAjax(
			Consumo.settings.urls.validarFonte,
			{ fonteLista: lista, fonte: item, indice: consumoIndex },
			Consumo.container, false).EhValido;

		if (!ehValido) {
			return;
		}

		var tr = $('.tabFonteTemplate tr', container).clone();
		var qde = $('.txtQde', container).val();
		var qdeFlorestaPlantada = $('.txtQdeFlorestaPlantada', container).val();
		var qdeFlorestaNativa = $('.txtQdeFlorestaNativa', container).val();
		var qdeOutroEstado = $('.txtQdeOutroEstado', container).val();

		$('.hdnFonteJSON', tr).val(JSON.stringify(item));
		tr.find('.trFonteTipoTexto').text(item.TipoTexto + "/" + item.UnidadeTexto).attr('title', item.TipoTexto + "/" + item.UnidadeTexto);
		tr.find('.trQde').text(qde).attr('title', qde);
		tr.find('.trQdeFlorestaPlantada').text(qdeFlorestaPlantada).attr('title', qdeFlorestaPlantada);
		tr.find('.trQdeFlorestaNativa').text(qdeFlorestaNativa).attr('title', qdeFlorestaNativa);
		tr.find('.trQdeOutroEstado').text(qdeOutroEstado).attr('title', qdeOutroEstado);

		$('.tabFontes tbody', container).append(tr);
		Listar.atualizarEstiloTable($('.tabFontes', container));

		var containerFonte = container.find('.divConteudoFontesEnergia');
		$('select', containerFonte).ddlFirst();
		$('input[type=text]', containerFonte).unmask().val('');
		Mascara.load(containerFonte);

		$('.ddlFonteTipos', containerFonte).focus();
	},

	excluirFonte: function () {
		var containerFonte = $(this).closest('.divConteudoFontesEnergia');

		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable($('.tabFontes', containerFonte));
	},

	obterConsumos: function () {
		var consumos = new Array();

		function ConsumoObj() {
			this.Id = 0;
			this.Atividade = 0;
			this.PossuiLicencaAmbiental = null;
			this.Licenca = {};
			this.FontesEnergia = new Array();
			this.DUANumero = null;
			this.DUAValor = null;
			this.DUADataPagamento = null;
		};

		$('.divConteudoConsumos .asmItens .asmItemContainer', Consumo.container).each(function (i, itemConsumo) {
			var consumo = new ConsumoObj();
			var containerTitulo = $('.divTituloAdicionar', itemConsumo);
			consumo.Id = $('.hdnConsumoId', itemConsumo).val();
			consumo.Atividade = $('.ddlAtividades', itemConsumo).val();
			consumo.PossuiLicencaAmbiental = $('.rbPossuiLicencaAmbiental:checked', itemConsumo).val();
			consumo.Licenca = TituloAdicionar.obter(containerTitulo);

			consumo.DUANumero = $('.txtDUANumero', itemConsumo).val();
			consumo.DUAValor = Mascara.getFloatMask($('.txtDUAValor', itemConsumo).val());
			consumo.DUADataPagamento = { DataTexto: $('.txtDUADataPagamento', itemConsumo).val() };

			//Dispensado
			if (consumo.PossuiLicencaAmbiental == $('.rbDispensado', itemConsumo).val()) {
				var containerDispensado = $('.divDispensado', itemConsumo);
				var emissor = $('.rbDispensadoOrgaoEmissor:checked', containerDispensado).val();

				var TituloObj = {
					EmitidoPorInterno: emissor,
					TituloModeloTexto: null,
					ProtocoloNumero: null,
					OrgaoExpedidor: null
				};

				if (emissor == $('.rbEmitidoIDAF', containerDispensado).val()) {
					TituloObj.EmitidoPorInterno = true;
				} else if (emissor == $('.rbEmitidoOutroOrgao', containerDispensado).val()) {
					TituloObj.EmitidoPorInterno = false;
				}

				TituloObj.TituloModeloTexto = $('.txtTituloModelo', containerDispensado).val();
				TituloObj.ProtocoloNumero = $('.txtProtocoloNumero', containerDispensado).val();
				TituloObj.OrgaoExpedidor = $('.txtOrgaoExpedidor', containerDispensado).val();

				consumo.Licenca = TituloObj;
			}

			$('.tabFontes tbody tr', itemConsumo).each(function (j, itemFonte) {
				consumo.FontesEnergia.push(JSON.parse($('.hdnFonteJSON', itemFonte).val()));
			});

			consumos.push(consumo);
		});

		return consumos;
	}
}
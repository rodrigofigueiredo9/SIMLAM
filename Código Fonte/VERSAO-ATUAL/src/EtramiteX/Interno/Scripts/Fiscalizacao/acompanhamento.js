/// <reference path="../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

Acompanhamento = {
	settings: {
		urls: {
			salvar: '',
			editar: '',
			visualizar: '',
			concluirCadastro: '',
			enviarArquivo: '',
			obterAssinanteCargos: '',
			obterAssinanteFuncionarios: ''
		},
		salvarCallBack: null,
		mensagens: {},
		idsTela: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Acompanhamento.settings, options); }
		Acompanhamento.container = MasterPage.getContent(container);

		Acompanhamento.container.delegate('.btnSalvar', 'click', Acompanhamento.salvar);
		Acompanhamento.container.delegate('.btnConcluirCadastro', 'click', Acompanhamento.concluirCadastro);
		
		$('.fsArquivos', Acompanhamento.container).arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });

		$('.btnAddArq', Acompanhamento.container).click(Acompanhamento.onEnviarArquivoClick);
		$('.btnLimparArq', Acompanhamento.container).click(Acompanhamento.onLimparArquivoClick);

		$('.ddlAssinanteSetores', Acompanhamento.container).change(Acompanhamento.onSelecionarSetor);
		$('.ddlAssinanteCargos', Acompanhamento.container).change(Acompanhamento.onSelecionarCargo);
		$('.ddlResultouErosao', Acompanhamento.container).change(Acompanhamento.onChangeErosao);

		$('.rdbAtividadeAreaEmbargada', Acompanhamento.container).change(Acompanhamento.gerenciarAtividadeAreaEmbargada);
		$('.rdbHouveDesrespeitoTAD', Acompanhamento.container).change(Acompanhamento.gerenciarHouveDesrespeitoTAD);
		$('.rdbRepararDanoAmbiental', Acompanhamento.container).change(Acompanhamento.gerenciarRepararDanoAmbiental);
		$('.rdbFirmouTermoRepararDanoAmbiental', Acompanhamento.container).change(Acompanhamento.gerenciarFirmouTermoRepararDanoAmbiental);

		$('.checkboxReservasLegais', Acompanhamento.container).change(Acompanhamento.gerenciarReservasLegais);

		$('.btnAdicionarAssinante', Acompanhamento.container).click(Acompanhamento.onAdicionarAssinante);
		$('.btnExcluirAssinante', Acompanhamento.container).click(Acompanhamento.onExcluirAssinante);

		Acompanhamento.gerenciarAtividadeAreaEmbargada();
		Acompanhamento.gerenciarHouveDesrespeitoTAD();
		Acompanhamento.gerenciarRepararDanoAmbiental();
		Acompanhamento.gerenciarFirmouTermoRepararDanoAmbiental();

		Mascara.load();
	},

	gerenciarReservasLegais: function () {
		var container = Acompanhamento.container;
		var current = $(this).val();

		var averbada = $('.checkbox1', container).is(':checked');
		var proposta = $('.checkbox2', container).is(':checked');
		var naoInformado = $('.checkbox3', container).is(':checked');
		var naoPossui = $('.checkbox4', container).is(':checked');

		if (averbada || proposta) {
			$('.checkbox3', container).removeAttr('checked');
			$('.checkbox4', container).removeAttr('checked');
		}

		if (naoInformado && current == 4) {
			$('.checkbox1', container).removeAttr('checked');
			$('.checkbox2', container).removeAttr('checked');
			$('.checkbox3', container).attr('checked', 'checked');
			$('.checkbox4', container).removeAttr('checked');
		}

		if (naoPossui && current == 8) {
			$('.checkbox1', container).removeAttr('checked');
			$('.checkbox2', container).removeAttr('checked');
			$('.checkbox3', container).removeAttr('checked');
			$('.checkbox4', container).attr('checked', 'checked');
		}
	},

	onEnviarArquivoClick: function () {
		var nomeArquivo = $('#fileTermo', Acompanhamento.container).val();

		erroMsg = new Array();

		if (nomeArquivo == '') {
			erroMsg.push(Acompanhamento.settings.mensagens.ArquivoObrigatorio);
		} else {
			var tam = nomeArquivo.length - 4;
			if (!Acompanhamento.validarTipoArquivo(nomeArquivo.toLowerCase().substr(tam))) {
				erroMsg.push(Acompanhamento.settings.mensagens.ArquivoNaoEhPdf);
			}
		}

		if (erroMsg.length > 0) {
			Mensagem.gerar(Acompanhamento.container, erroMsg);
			return;
		}

		MasterPage.carregando(true);
		var inputFile = $('#fileTermo', Acompanhamento.container);
		FileUpload.upload(Acompanhamento.settings.urls.enviarArquivo, inputFile, Acompanhamento.callBackArqEnviado);
	},

	onLimparArquivoClick: function () {
		$('.hdnArquivoJson', Acompanhamento.container).val('');
		$('.inputFile', Acompanhamento.container).val('');

		$('.spanInputFile', Acompanhamento.container).removeClass('hide');
		$('.txtArquivoNome', Acompanhamento.container).addClass('hide');

		$('.btnAddArq', Acompanhamento.container).removeClass('hide');
		$('.btnLimparArq', Acompanhamento.container).addClass('hide');

		Mensagem.limpar(Acompanhamento.container);
	},

	validarTipoArquivo: function (tipo) {

		var tipoValido = false;
		$(Acompanhamento.TiposArquivo).each(function (i, tipoItem) {
			if (tipoItem == tipo) {
				tipoValido = true;
			}
		});

		return tipoValido;
	},

	callBackArqEnviado: function (controle, retorno, isHtml) {
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoNome', Acompanhamento.container).text(ret.Arquivo.Nome);
			$('.hdnArquivoJson', Acompanhamento.container).val(JSON.stringify(ret.Arquivo));
			$('.txtArquivoNome', Acompanhamento.container).attr('href', '/Arquivo/BaixarTemporario?nomeTemporario=' + ret.Arquivo.TemporarioNome + '&contentType=' + ret.Arquivo.ContentType);

			$('.spanInputFile', Acompanhamento.container).addClass('hide');
			$('.txtArquivoNome', Acompanhamento.container).removeClass('hide');

			$('.btnAddArq', Acompanhamento.container).addClass('hide');
			$('.btnLimparArq', Acompanhamento.container).removeClass('hide');

			Mensagem.limpar(Acompanhamento.container);
			Mensagem.gerar(Acompanhamento.container, ret.Msg);
		} else {
			Acompanhamento.onLimparArquivoClick();
			Mensagem.gerar(Acompanhamento.container, ret.Msg);
		}

		Listar.atualizarEstiloTable(Acompanhamento.container.find('.dataGrid'));

		MasterPage.carregando(false);
	},

	onChangeErosao: function () {
		$('.divTxtErosao', Acompanhamento.container).addClass('hide');
		$('.txtErosao', Acompanhamento.container).val('');
		if ($(this).val() === "1") {
			$('.divTxtErosao', Acompanhamento.container).removeClass('hide');
		}
	},

	onSelecionarSetor: function () {

		var ddlA = $(".ddlAssinanteSetores", Acompanhamento.container);
		var ddlB = $('.ddlAssinanteCargos', Acompanhamento.container);
		var ddlC = $('.ddlAssinanteFuncionarios', Acompanhamento.container);

		var setorId = $('.ddlAssinanteSetores', Acompanhamento.container).val();

		ddlA.ddlCascate(ddlB, { url: Acompanhamento.settings.urls.obterAssinanteCargos, data: { setorId: setorId }, callBack: function () {			
			var cargoId = $('.ddlAssinanteCargos', Acompanhamento.container).val();
			ddlB.ddlCascate(ddlC, { url: Acompanhamento.settings.urls.obterAssinanteFuncionarios, data: { setorId: setorId, cargoId: cargoId} });
		} });
	},

	onSelecionarCargo: function () {

		var ddlA = $('.ddlAssinanteCargos', Acompanhamento.container);
		var ddlB = $('.ddlAssinanteFuncionarios', Acompanhamento.container);

		var setorId = $('.ddlAssinanteSetores', Acompanhamento.container).val();
		var cargoId = $('.ddlAssinanteCargos', Acompanhamento.container).val();

		ddlA.ddlCascate(ddlB, { url: Acompanhamento.settings.urls.obterAssinanteFuncionarios, data: { setorId: setorId, cargoId: cargoId} });
	},

	onAdicionarAssinante: function () {
		var mensagens = new Array();
		Mensagem.limpar(Acompanhamento.container);
		var container = $('.fdsAssinante', Acompanhamento.container);

		var item = {
			SetorId: $('.ddlAssinanteSetores :selected', container).val(),
			FuncionarioNome: $('.ddlAssinanteFuncionarios :selected', container).text(),
			FuncionarioId: $('.ddlAssinanteFuncionarios :selected', container).val(),
			FuncionarioCargoNome: $('.ddlAssinanteCargos :selected', container).text(),
			FuncionarioCargoId: $('.ddlAssinanteCargos :selected', container).val()
		};

		if (jQuery.trim(item.SetorId) == '0') {
			mensagens.push(jQuery.extend(true, {}, Acompanhamento.settings.mensagens.AssinanteSetorObrigatorio));
		}

		if (jQuery.trim(item.FuncionarioCargoId) == '0') {
			mensagens.push(jQuery.extend(true, {}, Acompanhamento.settings.mensagens.AssinanteCargoObrigatorio));
		}

		if (jQuery.trim(item.FuncionarioId) == '0') {
			mensagens.push(jQuery.extend(true, {}, Acompanhamento.settings.mensagens.AssinanteFuncionarioObrigatorio));
		}

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var itemAdd = (JSON.parse(obj));
				if (item.FuncionarioId == itemAdd.FuncionarioId && item.FuncionarioCargoId == itemAdd.FuncionarioCargoId) {
					mensagens.push(jQuery.extend(true, {}, Acompanhamento.settings.mensagens.AssinanteJaAdicionado));
				}
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(Acompanhamento.container, mensagens);
			return;
		}

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(item));
		linha.find('.Funcionario').html(item.FuncionarioNome).attr('title', item.FuncionarioNome);
		linha.find('.Cargo').html(item.FuncionarioCargoNome).attr('title', item.FuncionarioCargoNome);

		$('.btnExcluirAssinante', linha).click(Acompanhamento.onExcluirAssinante);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.ddlAssinanteSetores', container).ddlFirst();
		Acompanhamento.onSelecionarSetor();

	},

	onExcluirAssinante: function () {
		var container = $('.fdsAssinante');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	},

	gerenciarAtividadeAreaEmbargada: function () {
		var container = Acompanhamento.container;
		var rdb = $('.rdbAtividadeAreaEmbargada:checked', container).val();

		$('.divAtividadeAreaEmbargadaEspecificarTexto', container).addClass('hide');
		if (rdb == 1) {
			$('.divAtividadeAreaEmbargadaEspecificarTexto', container).removeClass('hide');
		}
	},

	gerenciarHouveDesrespeitoTAD: function () {
		var container = Acompanhamento.container;
		var rdb = $('.rdbHouveDesrespeitoTAD:checked', container).val();

		$('.divHouveDesrespeitoTAD', container).addClass('hide');
		if (rdb == 1) {
			$('.divHouveDesrespeitoTAD', container).removeClass('hide');
		}
	},

	gerenciarRepararDanoAmbiental: function () {
		var container = Acompanhamento.container;
		var rdb = $('.rdbRepararDanoAmbiental:checked', container).val();

		$('.divRepararDanoAmbientalEspecificar', container).addClass('hide');
		if (rdb == 1) {
			$('.divRepararDanoAmbientalEspecificar', container).removeClass('hide');
		}
	},

	gerenciarFirmouTermoRepararDanoAmbiental: function () {
		var container = Acompanhamento.container;
		var rdb = $('.rdbFirmouTermoRepararDanoAmbiental:checked', container).val();

		$('.divFirmouTermoRepararDanoAmbientalEspecificar, .divArquivo', container).addClass('hide');

		if (rdb == 0) {
			$('.divFirmouTermoRepararDanoAmbientalEspecificar', container).removeClass('hide');
		}

		if(rdb == 1){
			$('.divArquivo', container).removeClass('hide');
		}
	},

	obter: function () {
		var container = Acompanhamento.container;

		var obj = {
			Id: $('.hdnAcompanhamentoId', container).val(),
			FiscalizacaoId: $('.hdnFiscalizacaoId', container).val(),
			Numero: $('.txtNumeroSufixo', container).val(),
			NumeroSufixo: '',
			DataVistoria: { DataTexto: $('.txtDataVistoria', container).val() },
			SituacaoId: $('.hdnSituacaoId', container).val(),
			AgenteId: $('.hdnAgenteId', container).val(),
			SetorId: $('.ddlSetores :selected', container).val(),
			SetorTexto: $('.ddlSetores :selected', container).text(),
			AreaTotal:  Mascara.getFloatMask($('.txtAreaTotal', container).val()),
			AreaFlorestalNativa: Mascara.getFloatMask($('.txtAreaFlorestalNativa', container).val()),
			ReservalegalTipo: 0,
			PossuiAreaEmbargadaOuAtividadeInterditada: $('.hdnPossuiAreaEmbargadaOuAtividadeInterditada', container).val(),
			OpniaoAreaEmbargo: $('.txtOpniaoAreaEmbargo', container).val(),
			AtividadeAreaEmbargada: $('.rdbAtividadeAreaEmbargada:checked', container).val(),
			AtividadeAreaEmbargadaEspecificarTexto: $('.txtAtividadeAreaEmbargadaEspecificarTexto', container).val(),
			UsoAreaSoloDescricao: $('.txtUsoAreaSoloDescricao', container).val(),
			CaracteristicaSoloAreaDanificada: 0,
			AreaDeclividadeMedia: $('.txtAreaDeclividadeMedia', container).val(),
			InfracaoResultouErosao: $('.ddlResultouErosao :selected', container).val(),
			InfracaoResultouErosaoEspecificar: $('.txtInfracaoResultouErosaoEspecificar', container).val(),
			HouveApreensaoMaterial: $('.hdnHouveApreensaoMaterial', container).val(),
			OpniaoDestMaterialApreend: $('.txtOpniaoDestMaterialApreend', container).val(),
			HouveDesrespeitoTAD: $('.rdbHouveDesrespeitoTAD:checked', container).val(),
			HouveDesrespeitoTADEspecificar: $('.txtHouveDesrespeitoTADEspecificar', container).val(),
			InformacoesRelevanteProcesso: $('.txtInformacoesRelevanteProcesso', container).val(),
			RepararDanoAmbiental: $('.rdbRepararDanoAmbiental:checked', container).val(),
			RepararDanoAmbientalEspecificar: $('.txtRepararDanoAmbientalEspecificar', container).val(),
			FirmouTermoRepararDanoAmbiental: $('.rdbFirmouTermoRepararDanoAmbiental:checked', container).val(),
			FirmouTermoRepararDanoAmbientalEspecificar: $('.txtFirmouTermoRepararDanoAmbientalEspecificar', container).val(),
			Arquivo: null,
			Anexos: $('.fsArquivos', container).arquivo('obterObjeto'),
			Assinantes: []
		};

		$('.hdnItemJSon', container.find('.fdsAssinante')).each(function () {
			var objAssinante = String($(this).val());
			if (objAssinante != '') {
				obj.Assinantes.push(JSON.parse(objAssinante));
			}
		});

		//Caracteristicas de uso de solo
		$('.checkboxCaracteristicasSolo:checked', container).each(function () {
			obj.CaracteristicaSoloAreaDanificada += parseInt($(this).val());
		});

		//Reservas legais
		$('.checkboxReservasLegais:checked', container).each(function () {
			obj.ReservalegalTipo += parseInt($(this).val());
		});

		if (obj.Id > 0) {
			obj.NumeroSufixo = obj.Numero.split('-')[1];
		}

		if (obj.FirmouTermoRepararDanoAmbiental == 1) {
			obj.Arquivo = $.parseJSON($('.hdnArquivoJson', container).val());
		}

		if (obj.InfracaoResultouErosao == 2) {
			obj.InfracaoResultouErosaoEspecificar = '';
		}

		if (obj.AtividadeAreaEmbargada == 0) {
			obj.AtividadeAreaEmbargadaEspecificarTexto = '';
		}

		if (obj.HouveDesrespeitoTAD == 0) {
			obj.HouveDesrespeitoTADEspecificar = '';
		}

		if (obj.RepararDanoAmbiental == 0) {
			obj.RepararDanoAmbientalEspecificar = '';
		}

		if (obj.FirmouTermoRepararDanoAmbiental == 1) {
			obj.FirmouTermoRepararDanoAmbientalEspecificar = '';
		}

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: Acompanhamento.settings.urls.salvar,
			data: JSON.stringify(Acompanhamento.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Acompanhamento.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Acompanhamento.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	gerenciarConcluirCadastro: function(){
		var acompanhamentoId = $('.hdnAcompanhamentoId', Acompanhamento.container).val();

		$('.spnConcluirCadastro', Acompanhamento.container).addClass('hide');
		if (acompanhamentoId > 0) {
			$('.spnConcluirCadastro', Acompanhamento.container).removeClass('hide');
		}
	},

	concluirCadastro: function () {
		var acompanhamentoId = $('.hdnAcompanhamentoId', Acompanhamento.container).val();

		MasterPage.redireciona(Acompanhamento.settings.urls.concluirCadastro+'/'+acompanhamentoId);
	}
}
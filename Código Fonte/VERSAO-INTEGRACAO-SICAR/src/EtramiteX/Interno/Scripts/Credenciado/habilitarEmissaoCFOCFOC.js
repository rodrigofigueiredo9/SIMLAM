/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../masterpage.js" />

HabilitarEmissaoCFOCFOC = {
	settings: {
		urls: {
			salvar: null,
			salvarCadastrar: null,
			voltar: null,
			editar: null,
			visualizar: null,
			visualizarResponsavel: null,
			pessoaModal: null,
			obter: null,
			renovarDatasPragas: null,
			validarPraga: null,
			associarPragas: null
		},
		Mensagens: null
	},
	container: null,
	pragaAux: null,
	responsavelModal: null,

	load: function (container, options) {

		if (options) {
			$.extend(HabilitarEmissaoCFOCFOC.settings, options);
		}

		HabilitarEmissaoCFOCFOC.container = MasterPage.getContent(container);

		HabilitarEmissaoCFOCFOC.container.delegate('.btnAssociarResponsavel', 'click', HabilitarEmissaoCFOCFOC.onAssociarResponsavel);
		HabilitarEmissaoCFOCFOC.container.delegate('.btnVisualizarResponsavel', 'click', HabilitarEmissaoCFOCFOC.abrirVisualizarResponsavel);

		HabilitarEmissaoCFOCFOC.container.delegate('.btnArqLimpar', 'click', HabilitarEmissaoCFOCFOC.onLimparArquivo);
		HabilitarEmissaoCFOCFOC.container.delegate('.btnSalvar', 'click', HabilitarEmissaoCFOCFOC.salvar);
		HabilitarEmissaoCFOCFOC.container.delegate('.botaoLocalizarPraga', 'click', HabilitarEmissaoCFOCFOC.abrirModalPragas);
		HabilitarEmissaoCFOCFOC.container.delegate('.botaoAdicionarPraga', 'click', HabilitarEmissaoCFOCFOC.onAdicionarPraga);
		HabilitarEmissaoCFOCFOC.container.delegate('.btnRenovarDatas', 'click', HabilitarEmissaoCFOCFOC.renovarDatasPragas);
		HabilitarEmissaoCFOCFOC.container.delegate('.btnExcluirPraga', 'click', HabilitarEmissaoCFOCFOC.onExcluirPraga);
		HabilitarEmissaoCFOCFOC.container.delegate('.rdbExtensaoHabilitacao', 'change', HabilitarEmissaoCFOCFOC.onExtensaoHabilitacao);
		HabilitarEmissaoCFOCFOC.container.delegate('.ddlUf', 'change', HabilitarEmissaoCFOCFOC.onUfChange);

		HabilitarEmissaoCFOCFOC.container.delegate('.CpfPessoaContainer', 'keyup', HabilitarEmissaoCFOCFOC.verificarCPF);
		Aux.setarFoco(HabilitarEmissaoCFOCFOC.container);

		if ($('.hdnIsEditar', HabilitarEmissaoCFOCFOC.container).val() == 'True') {
			HabilitarEmissaoCFOCFOC.onHabilitarCampos();
		}
	},

	verificarCPF: function (e) {
		if (e.keyCode == 13) $('.btnAssociarResponsavel', HabilitarEmissaoCFOCFOC.container).click();
	},

	onHabilitarCampos: function () {

		$('.botaoLocalizarPraga', HabilitarEmissaoCFOCFOC.container).show();
		$('.botaoAdicionarPraga', HabilitarEmissaoCFOCFOC.container).show();

		$('.txtNumeroHabilitacao', HabilitarEmissaoCFOCFOC.container).removeClass('disabled');
		$('.txtNumeroHabilitacao', HabilitarEmissaoCFOCFOC.container).attr('disabled', false);

		$('.txtValidadeRegistro', HabilitarEmissaoCFOCFOC.container).removeClass('disabled');
		$('.txtValidadeRegistro', HabilitarEmissaoCFOCFOC.container).attr('disabled', false);

		$('.txtNumeroDua', HabilitarEmissaoCFOCFOC.container).removeClass('disabled');
		$('.txtNumeroDua', HabilitarEmissaoCFOCFOC.container).attr('disabled', false);

		$('.rdbExtensaoHabilitacao', HabilitarEmissaoCFOCFOC.container).removeClass('disabled');
		$('.rdbExtensaoHabilitacao', HabilitarEmissaoCFOCFOC.container).attr('disabled', false);

		$('.txtHabOrigem', HabilitarEmissaoCFOCFOC.container).removeClass('disabled');
		$('.txtHabOrigem', HabilitarEmissaoCFOCFOC.container).attr('disabled', false);

		$('.txtNumeroVistoCrea', HabilitarEmissaoCFOCFOC.container).removeClass('disabled');
		$('.txtNumeroVistoCrea', HabilitarEmissaoCFOCFOC.container).attr('disabled', false);

		$('.ddlUf', HabilitarEmissaoCFOCFOC.container).removeClass('disabled');
		$('.ddlUf', HabilitarEmissaoCFOCFOC.container).attr('disabled', false);

		$('.txtDataInicialHabilitacao', HabilitarEmissaoCFOCFOC.container).removeClass('disabled');
		$('.txtDataInicialHabilitacao', HabilitarEmissaoCFOCFOC.container).attr('disabled', false);

		$('.txtDataFinalHabilitacao', HabilitarEmissaoCFOCFOC.container).removeClass('disabled');
		$('.txtDataFinalHabilitacao', HabilitarEmissaoCFOCFOC.container).attr('disabled', false);
	},

	renovarDatasPragas: function () {

		Modal.confirma({
			url: HabilitarEmissaoCFOCFOC.settings.urls.renovarDatasPragas,
			tamanhoModal: Modal.tamanhoModalMedia,
			btnOkLabel: 'Atualizar',
			onLoadCallbackName: function (conteudoModal) { RenovarDataHabilitacaoCFO.load(conteudoModal); },
			btnOkCallback: RenovarDataHabilitacaoCFO.renovarDatas
		});
	},

	callBackRenovarPraga: function (datas) {
		var gridContainer = $('.gridPraga tbody');
		$('tr:not(.tr_template)', gridContainer).each(function () {

			$('.lblDataInicialHabilitacao', this).html(datas.DataInicialHabilitacao);
			$('.lblDataFinalHabilitacao', this).html(datas.DataFinalHabilitacao);
		});
		return true;
	},

	onExtensaoHabilitacao: function () {
		if ($('input.rdbExtensaoHabilitacao:checked', HabilitarEmissaoCFOCFOC.container).val() == 'Sim') {
			$('.divNumeroHabilitacaoOrigem', HabilitarEmissaoCFOCFOC.container).show();
		}
		else {
			$('.divNumeroHabilitacaoOrigem', HabilitarEmissaoCFOCFOC.container).hide();
		}
	},

	onUfChange: function () {
		if ($('.ddlUf', HabilitarEmissaoCFOCFOC.container).val() != '8') {
			$('.divRegistroCrea', HabilitarEmissaoCFOCFOC.container).show();
		}
		else {
			$('.divRegistroCrea', HabilitarEmissaoCFOCFOC.container).hide();
		}
	},

	abrirModalPragas: function () {

		Modal.abrir(HabilitarEmissaoCFOCFOC.settings.urls.associarPragas, null, function (content) {
			PragaListar.load(content, { onAssociarCallback: HabilitarEmissaoCFOCFOC.callBackAssociarPraga });
			Modal.defaultButtons(content);
		}, Modal.tamanhoModalMedia)
	},

	callBackAssociarPraga: function (praga) {
		pragaAux = praga;
		$('.txtNomePraga', HabilitarEmissaoCFOCFOC.container).val(praga.NomeCientifico);
		return true;
	},

	onAdicionarPraga: function () {
		Mensagem.limpar(HabilitarEmissaoCFOCFOC.container);

		var item = {
			Id: 0,
			Praga:
			{
				Id: 0, NomeCientifico: $('.txtNomePraga', HabilitarEmissaoCFOCFOC.container).val()
			},
			DataInicialHabilitacao: $('.txtDataInicialHabilitacao', HabilitarEmissaoCFOCFOC.container).val(),
			DataFinalHabilitacao: $('.txtDataFinalHabilitacao', HabilitarEmissaoCFOCFOC.container).val()
		};

		var habilitarObj =
		{
			Id: $('.hdnHabilitarId', HabilitarEmissaoCFOCFOC.container).val(),
			Pragas: [],
			Arquivo: null,
			Tid: ''
		};

		var gridContainer = $('.gridPraga tbody');

		$('tr:not(.tr_template)', gridContainer).each(function () {
			habilitarObj.Pragas.push(
				{
					Id: $('.hdnItemId', this).val(),
					Praga:
					{
						Id: $('.hdnPragaId', this).val(), NomeCientifico: $('.lblNomeCientifico', this).html(), NomeComum: $('.lblNomeComun', this).html()
					},
					DataInicialHabilitacao: $('.lblDataInicialHabilitacao', this).html(),
					DataFinalHabilitacao: $('.lblDataFinalHabilitacao', this).html(),
					Tid: ''
				});
		});

		var ehValido = MasterPage.validarAjax(
			HabilitarEmissaoCFOCFOC.settings.urls.validarPraga,
			{ habilitar: habilitarObj, praga: item },
			HabilitarEmissaoCFOCFOC.container, false).EhValido;

		if (!ehValido) {
			return;
		}

		var linha = $('.tr_template', HabilitarEmissaoCFOCFOC.container).clone();

		$('.hdnPragaId', linha).val(pragaAux.Id);
		$('.lblNomeCientifico', linha).html(pragaAux.NomeCientifico);
		$('.lblNomeComun', linha).html(pragaAux.NomeComum);
		$('.lblCultura', linha).html(pragaAux.Cultura);
		$('.lblDataInicialHabilitacao', linha).html($('.txtDataInicialHabilitacao', HabilitarEmissaoCFOCFOC.container).val());
		$('.lblDataFinalHabilitacao', linha).html($('.txtDataFinalHabilitacao', HabilitarEmissaoCFOCFOC.container).val());
		$('.hdnItemIndex', linha).val($('.gridPraga tbody tr:not(.tr_template)').length);

		//Add mais uma praga
		$(linha).removeClass('hide');
		$(linha).removeClass('tr_template');
		$('.gridPraga tbody', HabilitarEmissaoCFOCFOC.container).append($(linha));

		//Limpa os campos
		$('.txtNomePraga', HabilitarEmissaoCFOCFOC.container).val('');
		$('.txtDataInicialHabilitacao', HabilitarEmissaoCFOCFOC.container).val('');
		$('.txtDataFinalHabilitacao', HabilitarEmissaoCFOCFOC.container).val('');

		Listar.atualizarEstiloTable($('.gridPraga', HabilitarEmissaoCFOCFOC.container));
	},

	onExcluirPraga: function () {
		pragaAux = null;
		$(this).closest("tr").remove();
	},

	abrirVisualizarResponsavel: function () {
		var url = HabilitarEmissaoCFOCFOC.settings.urls.visualizarResponsavel + '/' + $('.hdnResponsavelId', HabilitarEmissaoCFOCFOC.container).val();

		Modal.abrir(url, null, function (content) {
			PragaListar.load(container, {});
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande, 'Visualizar Responsável Técnico')
	},

	onAssociarResponsavel: function () {

		if ($('.txtResponsavelCpf', HabilitarEmissaoCFOCFOC.container).val() == '') {
			Mensagem.gerar(HabilitarEmissaoCFOCFOC.container, [HabilitarEmissaoCFOCFOC.settings.Mensagens.CpfObrigatorio]);
			return;
		}

		var urlAcao = HabilitarEmissaoCFOCFOC.settings.urls.obter + '?CpfCnpj=' + $('.txtResponsavelCpf', HabilitarEmissaoCFOCFOC.container).val();

		Mensagem.limpar(HabilitarEmissaoCFOCFOC.container);
		MasterPage.carregando(true);

		$.ajax({
			url: urlAcao,
			cache: false,
			async: false,
			type: 'GET',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, HabilitarEmissaoCFOCFOC.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					HabilitarEmissaoCFOCFOC.associarResponsavel(response.Responsavel);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(HabilitarEmissaoCFOCFOC.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);

	},

	associarResponsavel: function (responsavel) {
		$('.hdnResponsavelId', HabilitarEmissaoCFOCFOC.container).val(responsavel.Id);
		$('.txtResponsavelNome', HabilitarEmissaoCFOCFOC.container).val(responsavel.Pessoa.NomeRazaoSocial);
		$('.txtResponsavelNome', HabilitarEmissaoCFOCFOC.container).attr('title', responsavel.Pessoa.NomeRazaoSocial);
		$('.txtResponsavelCpf', HabilitarEmissaoCFOCFOC.container).val(responsavel.Pessoa.CPFCNPJ);
		$('.txtResponsavelCpf', HabilitarEmissaoCFOCFOC.container).attr('title', responsavel.Pessoa.CPFCNPJ);
		$('.btnVisualizarResponsavel', HabilitarEmissaoCFOCFOC.container).removeClass('hide');
		$('.txtRegistroCrea', HabilitarEmissaoCFOCFOC.container).val(responsavel.RegistroNumero);

		HabilitarEmissaoCFOCFOC.onHabilitarCampos();

		return true;
	},

	//----------ANEXOS - ENVIAR ARQUIVO---------------
	onEnviarAnexoArquivoClick: function (url) {
		var nome = "enviando ...";

		var nomeArquivo = $('.inputFile').val();

		if (nomeArquivo === '') {
			Mensagem.gerar(HabilitarEmissaoCFOCFOC.container, [HabilitarEmissaoCFOCFOC.settings.Mensagens.ArquivoObrigatorio]);
			return;
		}

		if (nomeArquivo !== '') {
			var tam = nomeArquivo.length - 4;
			if (nomeArquivo.toLowerCase().substr(tam) !== ".jpg" && nomeArquivo.toLowerCase().substr(tam) !== ".gif"
				&& nomeArquivo.toLowerCase().substr(tam) !== ".bmp") {
				Mensagem.gerar(HabilitarEmissaoCFOCFOC.container, [HabilitarEmissaoCFOCFOC.settings.Mensagens.ArquivoNaoImagem]);
				return;
			}
		}

		var inputFile = $('.inputFileDiv input[type="file"]');

		inputFile.attr("id", "ArquivoId");

		FileUpload.upload(url, inputFile, HabilitarEmissaoCFOCFOC.msgArqEnviado);

		$('.inputFile').val('');
	},

	msgArqEnviado: function (controle, retorno, isHtml) {
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoNome', HabilitarEmissaoCFOCFOC.container).val(ret.Arquivo.Nome);
			$('.hdnAnexoArquivoJson', HabilitarEmissaoCFOCFOC.container).val(JSON.stringify(ret.Arquivo));

			$('.spanInputFile', HabilitarEmissaoCFOCFOC.container).addClass('hide');
			$('.txtArquivoNome', HabilitarEmissaoCFOCFOC.container).removeClass('hide');

			$('.btnArq', HabilitarEmissaoCFOCFOC.container).addClass('hide');
			$('.btnArqLimpar', HabilitarEmissaoCFOCFOC.container).removeClass('hide');

		} else {
			HabilitarEmissaoCFOCFOC.onLimparArquivoClick();
		}

		Mensagem.gerar(MasterPage.getContent(HabilitarEmissaoCFOCFOC.container), ret.Msg);
	},

	onLimparArquivo: function () {

		//implementar Limpar
		$('.txtArquivoNome', HabilitarEmissaoCFOCFOC.container).data('arquivo', null);
		$('.txtArquivoNome', HabilitarEmissaoCFOCFOC.container).val("");
		$('.hdnAnexoArquivoJson', HabilitarEmissaoCFOCFOC.container).val("");

		$('.spanInputFile', HabilitarEmissaoCFOCFOC.container).removeClass('hide');
		$('.txtArquivoNome', HabilitarEmissaoCFOCFOC.container).addClass('hide');

		$('.btnArq', HabilitarEmissaoCFOCFOC.container).removeClass('hide');
		$('.btnArqLimpar', HabilitarEmissaoCFOCFOC.container).addClass('hide');

		$('.lnkArquivo', HabilitarEmissaoCFOCFOC.container).remove();
	},

	obter: function () {
	    var gridContainer = $('.gridPraga tbody');
		var habilitarObj =
		{
			HabilitarEmissao:
			{
				Id: $('.hdnHabilitarId', HabilitarEmissaoCFOCFOC.container).val(),
				Responsavel: { Id: $('.hdnResponsavelId', HabilitarEmissaoCFOCFOC.container).val(), Pessoa: { Fisica: { Nome: $('.txtResponsavelNome', HabilitarEmissaoCFOCFOC.container).val(), CPF: $('.txtResponsavelCpf', HabilitarEmissaoCFOCFOC.container).val() } } },
				NumeroHabilitacao: $('.txtNumeroHabilitacao', HabilitarEmissaoCFOCFOC.container).val(),
				ValidadeRegistro: $('.txtValidadeRegistro', HabilitarEmissaoCFOCFOC.container).val(),
				Situacao: 1,
				NumeroDua: $('.txtNumeroDua', HabilitarEmissaoCFOCFOC.container).val(),
				ExtensaoHabilitacao: (($('input.rdbExtensaoHabilitacao:checked', HabilitarEmissaoCFOCFOC.container).val() == 'Sim') ? 1 : 0),
				NumeroHabilitacaoOrigem: $('.txtHabOrigem', HabilitarEmissaoCFOCFOC.container).val(),
				RegistroCrea: $('.txtRegistroCrea', HabilitarEmissaoCFOCFOC.container).val(),
				NumeroVistoCrea: $('.txtNumeroVistoCrea', HabilitarEmissaoCFOCFOC.container).val(),
				UF: $('.ddlUf', HabilitarEmissaoCFOCFOC.container).val(),
				Pragas: [],
				Arquivo: null,
				Tid: ''
			},

		};

		var arquivo = $('.hdnAnexoArquivoJson', HabilitarEmissaoCFOCFOC.container).val();
		habilitarObj.HabilitarEmissao.Arquivo = $.parseJSON(arquivo);

		$('tr:not(.tr_template)', gridContainer).each(function () {
			habilitarObj.HabilitarEmissao.Pragas.push(
				{
					Id: $('.hdnItemId', this).val(),
					Praga:
					{
						Id: $('.hdnPragaId', this).val(), NomeCientifico: $('.lblNomeCientifico', this).html(), NomeComum: $('.lblNomeComun', this).html()
					},
					DataInicialHabilitacao: $('.lblDataInicialHabilitacao', this).html(),
					DataFinalHabilitacao: $('.lblDataFinalHabilitacao', this).html(),
					Tid: ''
				});
		});

		return habilitarObj;
	},

	validar: function () {

		Mensagem.limpar(HabilitarEmissaoCFOCFOC.container);

		var mensagem = [];

		if ($('.txtResponsavelNome', HabilitarEmissaoCFOCFOC.container).val() == '') {
			mensagem.push(HabilitarEmissaoCFOCFOC.settings.Mensagens.ResponsavelObrigatorio);
		}

		if ($('.txtNumeroHabilitacao', HabilitarEmissaoCFOCFOC.container).val() == '') {
			mensagem.push(HabilitarEmissaoCFOCFOC.settings.Mensagens.NumeroHabilitacaoObrigatorio);
		}

		if ($('.txtValidadeRegistro', HabilitarEmissaoCFOCFOC.container).val() == '') {
			mensagem.push(HabilitarEmissaoCFOCFOC.settings.Mensagens.ValidadeRegistroObrigatorio);
		}

		if ($('.txtNumeroDua', HabilitarEmissaoCFOCFOC.container).val() == '') {
			mensagem.push(HabilitarEmissaoCFOCFOC.settings.Mensagens.NumeroDuaObrigatorio);
		}

		if ($('input.rdbExtensaoHabilitacao:checked', HabilitarEmissaoCFOCFOC.container).val() == 'Sim' && $('.txtHabOrigem', HabilitarEmissaoCFOCFOC.container).val() == '') {
			mensagem.push(HabilitarEmissaoCFOCFOC.settings.Mensagens.NumeroHabilitacaoOrigemObrigatorio);
		}

		if ($('.ddlUf option:selected').val() == '0') {
			mensagem.push(HabilitarEmissaoCFOCFOC.settings.Mensagens.UFObrigatorio);
		}

		if ($('.ddlUf option:selected').val() != '8' && $('.txtNumeroVistoCrea', HabilitarEmissaoCFOCFOC.container).val() == '') {
			mensagem.push(HabilitarEmissaoCFOCFOC.settings.Mensagens.NumeroVistoCreaObrigatorio);
		}

		if ($('.gridPraga tbody tr:not(.tr_template)').length == 0) {
			mensagem.push(HabilitarEmissaoCFOCFOC.settings.Mensagens.PragaObrigatorio);
		}

		if (mensagem.length > 0) {
			Mensagem.gerar(MasterPage.getContent(HabilitarEmissaoCFOCFOC.container), mensagem);
			return false;
		}

		return true;
	},

	salvar: function () {

		if (HabilitarEmissaoCFOCFOC.validar()) {

			Mensagem.limpar(HabilitarEmissaoCFOCFOC.container);

			MasterPage.carregando(true);

			var objeto = JSON.stringify(HabilitarEmissaoCFOCFOC.obter());

			$.ajax({
				url: HabilitarEmissaoCFOCFOC.settings.urls.salvar,
				data: objeto,
				cache: false,
				async: false,
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, HabilitarEmissaoCFOCFOC.container);
				},
				success: function (response, textStatus, XMLHttpRequest) {
					if (response.IsSalvo) {

						if (response.UrlRedireciona != '') {
							MasterPage.redireciona(response.UrlRedireciona);
						}
					}
					else {
						if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(HabilitarEmissaoCFOCFOC.container, response.Msg);
						}
					}

				}
			});

			MasterPage.carregando(false);
		}
	}
}
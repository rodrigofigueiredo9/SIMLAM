/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />
/// <reference path="analisarItemAnalise.js" />

AgrotoxicoCultura = {
	settings: {
		urls: {
			salvar: '',
			listarCulturas: '',
			listarPragas: '',
			onSalvarCallBack: null,
		},
		mensagens: null,
		idsTelaIngredienteAtivoSituacao:null
	},
	container: null,

	load: function (container, options) {

		if (options) { $.extend(AgrotoxicoCultura.settings, options); }

		AgrotoxicoCultura.container = MasterPage.getContent(container);
		AgrotoxicoCultura.container.delegate(".btnBuscarCultura", 'click', AgrotoxicoCultura.onAbrirCulturas);
		AgrotoxicoCultura.container.delegate(".btnBuscarPraga", 'click', AgrotoxicoCultura.onAbrirPraga);
		AgrotoxicoCultura.container.delegate(".btnExcluir", 'click', AgrotoxicoCultura.excluirItem);
	},

	onAbrirCulturas: function () {
		Modal.abrir(AgrotoxicoCultura.settings.urls.listarCulturas, null,
			function (content) {
				CulturaListar.load(content, { onAssociarCallback: AgrotoxicoCultura.callBackAdicionarCultura });
				Modal.defaultButtons(content);
			}, Modal.tamanhoModalMedia);
	},

	callBackAdicionarCultura: function (response) {
		$('.hdnCulturaId', AgrotoxicoCultura.container).val(response.Id);
		$('.txtCulturaNome', AgrotoxicoCultura.container).val(response.Nome);
		return true;
	},

	onAbrirPraga: function () {
		Modal.abrir(
		AgrotoxicoCultura.settings.urls.listarPragas, null,
			function (content) {
				PragaListar.load(content, { onAssociarCallback: AgrotoxicoCultura.callBackAdicionarPraga });
				Modal.defaultButtons(content);
			}, Modal.tamanhoModalMedia)

	},

	callBackAdicionarPraga: function (response) {
		Mensagem.limpar();
		var itemAdicionado = false;
		$('.gridPraga tbody tr:not(.trTemplate) .hdnItemJson', AgrotoxicoCultura.container).each(function () {
			var itemLinha = JSON.parse($(this).val());
			if (itemLinha.Id == response.Id) {
				itemAdicionado = true;
			}
		});

		if (itemAdicionado) {
			Mensagem.gerar(MasterPage.getContent(AgrotoxicoCultura.container),  [AgrotoxicoCultura.settings.mensagens.PragaJaAdicionada]);
			return false;
		}

		var tabela = $('.gridPraga tbody', AgrotoxicoCultura.container);
		var linha = $('.trTemplate', tabela).clone();

		$('.lblNome', linha).append(response.NomeCientifico);
		$('.hdnItemJson', linha).val(JSON.stringify(response));

		$(linha).removeClass('hide');
		$(linha).removeClass('trTemplate');

		$(tabela).append(linha);		
		Listar.atualizarEstiloTable(AgrotoxicoCultura.container);		
		return false;
	},

	excluirItem: function () {
		$(this).closest('tr').remove();
	},

	obter: function () {
		var agrotoxicoCultura = {
			IdRelacionamento: +$('.hdnAgrotoxicoCulturaIdRelacionamento', AgrotoxicoCultura.container).val(),
			IntervaloSeguranca: $('.txtIntervaloSeguranca', AgrotoxicoCultura.container).val(),
			Cultura: {
				Id: +$('.hdnCulturaId', AgrotoxicoCultura.container).val(),
				Nome: $('.txtCulturaNome', AgrotoxicoCultura.container).val()
			},
			Pragas: [],
			ModalidadesAplicacao:[]
		};

		$('.gridPraga tbody tr:not(.trTemplate) .hdnItemJson', AgrotoxicoCultura.container).each(function () {
			agrotoxicoCultura.Pragas.push(JSON.parse($(this).val()));
		});

		$('.cbModalidadeAplicacao:checked', AgrotoxicoCultura.container).each(function () {
			var item = $(this).closest('div').find('.hdnItemJson').val();
			agrotoxicoCultura.ModalidadesAplicacao.push(JSON.parse(item));
		});

		return agrotoxicoCultura;
	},

	salvar: function () {
		var msg = [];
		if (+$('.hdnCulturaId', AgrotoxicoCultura.container).val() <= 0) {
			msg.push(AgrotoxicoCultura.settings.mensagens.CulturaObrigatorio);
		}

		if (+$('.gridPraga tbody tr:not(.trTemplate)', AgrotoxicoCultura.container).length <= 0) {
			msg.push(AgrotoxicoCultura.settings.mensagens.PragaObrigatorio);
		}

		if ($('.cbModalidadeAplicacao:checked').length <= 0) {
			msg.push(AgrotoxicoCultura.settings.mensagens.ModalidadeAplicacaoObrigatorio);
		}

		if ($('.txtIntervaloSeguranca').val() == '') {
			msg.push(AgrotoxicoCultura.settings.mensagens.IntervaloSegurancaObrigatorio);
		}

		if (msg.length > 0) {
			Mensagem.gerar(MasterPage.getContent(AgrotoxicoCultura.container), msg);
			return;
		}

		var objeto = AgrotoxicoCultura.obter();
		var retorno = AgrotoxicoCultura.settings.onSalvarCallBack(objeto);

		if (retorno === true) {
			Modal.fechar(AgrotoxicoCultura.container);
		} else {
			Mensagem.gerar(AgrotoxicoCultura.container, retorno);
		}
	}
}
/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="unidadeProducaoItem.js" />
/// <reference path="coordenadaAtividade.js" />

UnidadeProducao = {
	settings: {
		urls: {
			salvar: null,
			editar: null,
			visualizar: null,
			AdicionarUnidadeProducao: null
		},
		mensagens: {},
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(UnidadeProducao.settings, options);
		}

		UnidadeProducao.container = MasterPage.getContent(container);
		UnidadeProducao.container.delegate('.btnSalvar', 'click', UnidadeProducao.salvar);
		UnidadeProducao.container.delegate('.RadioPropriedadeCodigo', 'change', UnidadeProducao.onRbPossuiCodigoPropriedadeChange);
		UnidadeProducao.container.delegate('.btnAdicionarUnidadeProducao', 'click', UnidadeProducao.onAbrirUnidadeProducaoModal);
		UnidadeProducao.container.delegate('.btnEditarUnidadeProducao', 'click', UnidadeProducao.editarUnidadeProducao);
		UnidadeProducao.container.delegate('.btnVisualizarUP', 'click', UnidadeProducao.onVisualizar);
		UnidadeProducao.container.delegate('.btnExcluirUnidadeProducao', 'click', UnidadeProducao.excluirUnidadeProducao);
	},

	onRbPossuiCodigoPropriedadeChange: function () {
		if ($(this).val() == 'True') {
			$('.txtCodigoPropriedade', UnidadeProducao.container).removeClass('disabled');
			$('.txtCodigoPropriedade', UnidadeProducao.container).removeAttr('disabled');
			$('.txtCodigoPropriedade', UnidadeProducao.container).val('');
			$('.txtCodigoPropriedade', UnidadeProducao.container).focus();
		} else {
			$('.txtCodigoPropriedade', UnidadeProducao.container).addClass('disabled');
			$('.txtCodigoPropriedade', UnidadeProducao.container).attr('disabled', 'disabled');
			$('.txtCodigoPropriedade', UnidadeProducao.container).val('Gerado automaticamente');
		}
	},

	onAbrirUnidadeProducaoModal: function () {
		var empreendimento = +$('.hdnEmpreendimentoId', UnidadeProducao.container).val();

		Modal.abrir(UnidadeProducao.settings.urls.AdicionarUnidadeProducao, { empreendimento: empreendimento },
		function (content) {
			UnidadeProducaoItem.load(content, { onSalvarCallback: UnidadeProducao.callBackAdicionarUnidadeProducao, empreendimentoID: empreendimento });
			Modal.defaultButtons(content, UnidadeProducaoItem.salvar, "Adicionar");
		},
		Modal.tamanhoModalGrande);
	},

	callBackAdicionarUnidadeProducao: function (unidade) {
		var tabela = $('.gridUnidadeProducao tbody', UnidadeProducao.container);
		var linha = $('.trTemplate', tabela).clone();

		$('.lblCodigoUp', linha).append(unidade.CodigoUP < 1 ? 'Gerado automaticamente' : unidade.CodigoUP);
		$('.lblAreaHa', linha).append(Mascara.getStringMask(unidade.AreaHA, 'n4'));
		$('.lblCultura', linha).text(unidade.CulturaTexto + (unidade.CultivarId > 0 ? ' ' + unidade.CultivarTexto : ''));
		$('.lblEstimativaQuantidadeAno', linha).append(Mascara.getStringMask(unidade.EstimativaProducaoQuantidadeAno, 'n4') + ' ' + unidade.EstimativaProducaoUnidadeMedida);
		$('.hdnItemObjeto', linha).val(JSON.stringify(unidade));

		var itemAdicionado = false;
		$('tr:not(.trTemplate) .hdnItemObjeto', tabela).each(function () {
			var unidadeLinha = JSON.parse($(this).val());

			var seqAtual = unidade.CodigoUP.toString().substr(13, 4);
			var seqNova = unidadeLinha.CodigoUP.toString().substr(13, 4);

			if (unidade.PossuiCodigoUP && (unidadeLinha.CodigoUP == unidade.CodigoUP || seqAtual == seqNova)) {
				itemAdicionado = true;
				return;
			}
		});

		if (itemAdicionado) {
			return [UnidadeProducao.settings.mensagens.CodigoUPJaExiste];
		}

		$(linha).removeClass('hide');
		$(linha).removeClass('trTemplate');
		$(tabela).append(linha);
		Listar.atualizarEstiloTable(UnidadeProducao.container);
		return true;
	},

	excluirUnidadeProducao: function () {
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable(UnidadeProducao.container);
	},

	editarUnidadeProducao: function () {
		$('.itemEdicao', UnidadeProducao.container).removeClass('itemEdicao');
		$(this).closest('tr').addClass('itemEdicao');

		var JsonParser = JsonBigint()

		var objeto = JsonParser.parse($('.hdnItemObjeto', $(this).closest('tr')).val());
		var empreendimento = +$('.hdnEmpreendimentoId', UnidadeProducao.container).val();

		Modal.abrir(UnidadeProducao.settings.urls.AdicionarUnidadeProducao, { empreendimento: empreendimento, unidade: objeto },
		function (content) {
			UnidadeProducaoItem.load(content, { onSalvarCallback: UnidadeProducao.callBackEditarUnidadeProducao, empreendimentoID: empreendimento });
			Modal.defaultButtons(content, UnidadeProducaoItem.salvar, "Editar");
		},
		Modal.tamanhoModalGrande, 'Editar Unidade de Produção');
	},

	callBackEditarUnidadeProducao: function (unidade) {
		var tabela = $('.gridUnidadeProducao tbody', UnidadeProducao.container);
		var linha = $('.itemEdicao', tabela);

		var itemAdicionado = false;
		$('tr:not(.trTemplate, .itemEdicao)', tabela).find('.hdnItemObjeto').each(function () {

		    var JsonParser = new JsonBigint();

		    var unidadeLinha = JsonParser.parse($(this).val());

			var seqAtual = unidade.CodigoUP.toString().substr(13, 4);
			var seqNova = unidadeLinha.CodigoUP.toString().substr(13, 4);

			if (unidade.PossuiCodigoUP && (unidadeLinha.CodigoUP == unidade.CodigoUP || seqAtual == seqNova)) {
				itemAdicionado = true;
				return;
			}
		});

		if (itemAdicionado) {
			return [UnidadeProducao.settings.mensagens.CodigoUPJaExiste];
		}

		$('.lblCodigoUp', linha).text(unidade.CodigoUP < 1 ? 'Gerado automaticamente' : unidade.CodigoUP);
		$('.lblAreaHa', linha).text(Mascara.getStringMask(unidade.AreaHA, 'n4'));
		$('.lblCultura', linha).text(unidade.CulturaTexto + (unidade.CultivarId > 0 ? ' ' + unidade.CultivarTexto : ''));
		$('.lblEstimativaQuantidadeAno', linha).text(Mascara.getStringMask(unidade.EstimativaProducaoQuantidadeAno, 'n4') + ' ' + unidade.EstimativaProducaoUnidadeMedida);
		$('.hdnItemObjeto', linha).val(JSON.stringify(unidade));

		$(linha).removeClass('itemEdicao');
		Listar.atualizarEstiloTable(UnidadeProducao.container);
		return true;
	},

	onVisualizar: function () {
	    var JsonParser = JsonBigint();

	    var objeto = JsonParser.parse($('.hdnItemObjeto', $(this).closest('tr')).val());
		var empreendimento = +$('.hdnEmpreendimentoId', UnidadeProducao.container).val();

		Modal.abrir(UnidadeProducao.settings.urls.AdicionarUnidadeProducao, { empreendimento: empreendimento, unidade: objeto, visualizar: true },
		function (content) {
			UnidadeProducaoItem.load(content, { empreendimentoID: empreendimento});
			Modal.defaultButtons(content);
		},
		Modal.tamanhoModalGrande, 'Visualizar Unidade de Produção');
	},

	obter: function () {

	    var JsonParser = JsonBigint();

		var unidadeProducaoObj = {
			Id: +$('.hdnUnidadeProducaoId', UnidadeProducao.container).val(),
			LocalLivroDisponivel: $('.txtLocalLivroDisponivel', UnidadeProducao.container).val(),
			PossuiCodigoPropriedade: $('.rbPossuiCodigoSim', UnidadeProducao.container).is(':checked'),
			CodigoPropriedade: Mascara.getIntMask($('.txtCodigoPropriedade', UnidadeProducao.container).val()),
			Empreendimento: {
				Id: +$('.hdnEmpreendimentoId', UnidadeProducao.container).val(),
				Codigo: +$('.txtCodigoEmpreendimento', UnidadeProducao.container).val()
			},
			UnidadesProducao: []
		};

		$('.gridUnidadeProducao tbody tr:not(.trTemplate) .hdnItemObjeto', UnidadeProducao.container).each(function () {
		    unidadeProducaoObj.UnidadesProducao.push(JsonParser.parse($(this).val()));
		});

		return unidadeProducaoObj;
	},

	salvar: function () {
		Mensagem.limpar(UnidadeProducao.container);
		var msgValidacao = [];
		var objeto = UnidadeProducao.obter();

		if (objeto.PossuiCodigoPropriedade && objeto.CodigoPropriedade < 1) {
			msgValidacao.push(UnidadeProducao.settings.mensagens.CodigoPropriedadeObrigatorio);
		}

		if (objeto.LocalLivroDisponivel == '') {
			msgValidacao.push(UnidadeProducao.settings.mensagens.LocalLivroDisponivelObrigatorio);
		}

		if (objeto.UnidadesProducao.length < 1) {
			msgValidacao.push(UnidadeProducao.settings.mensagens.UnidadeProducaoObrigatorio);
		}

		if (msgValidacao.length > 0) {
			Mensagem.gerar(UnidadeProducao.container, msgValidacao);
			return;
		}

		MasterPage.carregando(true);
		$.ajax({
			url: UnidadeProducao.settings.urls.salvar,
			data: JSON.stringify(objeto),
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
					Mensagem.gerar(UnidadeProducao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
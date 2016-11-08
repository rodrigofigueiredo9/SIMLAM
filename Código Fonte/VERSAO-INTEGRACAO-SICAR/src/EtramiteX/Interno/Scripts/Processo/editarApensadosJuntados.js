/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

EditarApensadosJuntados = {
	settings: {
		container: null,
		urls: {
			pdfRequerimento: '',
			associarRequerimento: '',
			buscarAtividadesDeRequerimento: '',
			salvar: ''
		},
		Mensagens: null
	},

	load: function (container, options) {
		if (options) {
			$.extend(EditarApensadosJuntados.settings, options);
		}

		EditarApensadosJuntados.settings.container = container;
		container.delegate('.btnAssociarRequerimento', 'click', EditarApensadosJuntados.onBtnAssociarRequerimentoClick);
		container.delegate('.btnGerarPdfRequerimento', 'click', EditarApensadosJuntados.onBtnGerarPdfRequerimentoClick);

		$('.btnSalvarGrupoRequerimento', container).click(EditarApensadosJuntados.salvar);

		container.associarMultiplo({
			'onExpandirEsconder': function () { MasterPage.redimensionar(); }
		});
	},

	onBtnAssociarRequerimentoClick: function () {
		$('.divHiddenItemContainer', EditarApensadosJuntados.settings.container).removeClass('associando');
		$(this).closest('.divHiddenItemContainer').addClass('associando');

		Modal.abrir(EditarApensadosJuntados.settings.urls.associarRequerimento, null, function (container) {
			Modal.defaultButtons(container);
			RequerimentoListar.load(container, { associarFuncao: EditarApensadosJuntados.onAssociarRequerimento });
		});
	},

	onAssociarRequerimento: function (Requerimento) {
		var divAssociando = $('.associando', EditarApensadosJuntados.settings.container);

		if ($('.txtReqNumero', divAssociando).val() == Requerimento.Id) {
			return true;
		}

		var arrayMensagem = new Array();
		var params = {
			requerimentoId: Requerimento.Id,
			isProcesso: divAssociando.closest('.divTipo').hasClass('divProcessos'),
			excetoId: parseInt($(divAssociando).find('.hdnItemId').val())
		};

		$.ajax({ url: EditarApensadosJuntados.settings.urls.buscarAtividadesDeRequerimento,
			data: params, type: "GET", cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, EditarApensadosJuntados.settings.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					arrayMensagem = response.Msg;
				} else {
					$('.txtReqNumero', divAssociando).val(Requerimento.Id);
					$('.txtReqDataCriacao', divAssociando).val(Requerimento.DataCriacao);

					var atividadesContainer = $('.divConteudoAtividadeSolicitada .asmItens', divAssociando);
					atividadesContainer.html(response);
				}
			}
		});

		if (arrayMensagem && arrayMensagem.length > 0) {
			return arrayMensagem;
		}

		MasterPage.botoes(divAssociando);
		return true;
	},

	onBtnGerarPdfRequerimentoClick: function () {
		var id = parseInt($('.txtReqNumero', $(this).closest('.divHiddenItemContainer')).val());
		MasterPage.redireciona(EditarApensadosJuntados.settings.urls.pdfRequerimento + '?id=' + id);
		MasterPage.carregando(false);
	},

	salvar: function () {

		function ObjetoSerializar() {
			this.Id = 0;
			this.Tipo = { Id: 0 };
			this.Requerimento = { Id: 0 };
			this.ChecagemRoteiro = { Id: 0 };
			this.Atividades = [];
		};

		var Processo = {
			Id: parseInt($('.hdnProcessoId').val()),
			Processos: [],
			Documentos: []
		};

		var divRequerimento = $(this).closest('.divHiddenItemContainer');

		var objeto = new ObjetoSerializar();
		objeto.Id = parseInt($(divRequerimento).find('.hdnItemId').val());
		objeto.Tipo.Id = parseInt($(divRequerimento).find('.hdnItemTipo').val());
		objeto.ChecagemRoteiro.Id = parseInt($(divRequerimento).find('.hdnItemCheckListId').val());
		objeto.Requerimento.Id = parseInt($(divRequerimento).find('.txtReqNumero').val());
		objeto.Atividades = AtividadeSolicitadaAssociar.gerarObjeto(divRequerimento);

		if (divRequerimento.closest('.divTipo').hasClass('divProcessos')) {
			Processo.Processos.push(objeto);
		} else {
			Processo.Documentos.push(objeto);
		}

		MasterPage.carregando(true);
		$.ajax({
			url: EditarApensadosJuntados.settings.urls.salvar,
			data: JSON.stringify(Processo),
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, EditarApensadosJuntados.settings.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(EditarApensadosJuntados.settings.container), response.Msg);
				}

				if (response.EhValido) {
					divRequerimento.find('.asmConteudoInternoExpander.asmExpansivel').click();
				}
			}
		});
		MasterPage.carregando(false);
	}
}
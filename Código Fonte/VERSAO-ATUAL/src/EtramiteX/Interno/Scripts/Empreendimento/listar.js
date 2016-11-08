/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

EmpreendimentoListar = {
	urlEditar: '',
	urlConfirmarExcluir: '',
	urlExcluir: '',
	urlCaracterizacao: '',
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(EmpreendimentoListar.settings, options); }

		container.listarAjax({
			onBeforeFiltrar: EmpreendimentoListar.onBeforeFiltrar
		});

		container.delegate('.btnExcluir', 'click', EmpreendimentoListar.excluir);
		container.delegate('.btnVisualizar', 'click', EmpreendimentoListar.visualizar);
		container.delegate('.btnAssociar', 'click', EmpreendimentoListar.associar);
		container.delegate('.btnEditar', 'click', EmpreendimentoListar.editar);
		container.delegate('.btnCaracterizacao', 'click', EmpreendimentoListar.caracterizacao);

		container.delegate('.radioResponsavelCpfCnpj', 'change', Aux.onChangeRadioCpfCnpjMask);
		Aux.onChangeRadioCpfCnpjMask($('.radioResponsavelCpfCnpj', container));

		Aux.setarFoco(container);
		EmpreendimentoListar.container = container;

		if (EmpreendimentoListar.settings.associarFuncao) {
			$('.hdnIsAssociar', container).val(true);
		}
	},
	
	onBeforeFiltrar: function (container, serializedData) {
		serializedData.Filtros.Codigo = Mascara.getIntMask($(".txtCodigo", EmpreendimentoListar.container).val()).toString();
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());

		if (EmpreendimentoListar.settings.associarFuncao) {
			Modal.abrir($('.urlVisualizar', EmpreendimentoListar.container).val() + "/" + itemId, null, function (container) {
				Modal.defaultButtons(container);
			});
		} else {
			MasterPage.redireciona($('.urlVisualizar', EmpreendimentoListar.container).val() + "/" + itemId);
		}
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		var retorno = MasterPage.validarAjax($('.urlValidarPosse', EmpreendimentoListar.container).val(), { id: itemId }, EmpreendimentoListar.container, false);

		if (!retorno.EhValido) {
			return;
		}

		MasterPage.redireciona(EmpreendimentoListar.urlEditar + "/" + itemId);
	},

	associar: function () {
		var linha = $(this).closest('tr');
		var itemId = parseInt(linha.find('.itemId:first').val());
		var itemDenominador = linha.find('.itemDenominador:first').val();
		var itemCnpj = linha.find('.itemCnpj:first').val();
		var itemCodigo = $('.itemCodigo', linha).text().trim();

		retorno = EmpreendimentoListar.settings.associarFuncao({ Id: itemId, Denominador: itemDenominador, CNPJ: itemCnpj, Codigo: itemCodigo });

		if (retorno === true) {
			Modal.fechar(EmpreendimentoListar.container);
		} else {
			Mensagem.gerar(EmpreendimentoListar.container, retorno);
		}
	},

	caracterizacao: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(EmpreendimentoListar.urlCaracterizacao + "/" + itemId);
	},

	excluir: function () {
		Modal.excluir({
			'urlConfirm': EmpreendimentoListar.urlConfirmarExcluir,
			'urlAcao': EmpreendimentoListar.urlExcluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'btnExcluir': this
		});
	}
}
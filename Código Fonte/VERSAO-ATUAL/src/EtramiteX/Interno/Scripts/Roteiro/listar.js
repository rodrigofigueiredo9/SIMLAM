/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

RoteiroListar = {
	urlEditar: '',
	urlDesativarComfirm: '',
	urlDesativar: '',
	urlPdfRoteiro: '',
	urlDesativarRoteiro: '',
	associarFuncao: null,
	container: null,

	load: function (container, fnAssociar) {
		container = MasterPage.getContent(container);
		container.listarAjax();

		container.delegate('.btnVisualizar', 'click', RoteiroListar.visualizar);
		container.delegate('.btnEditar', 'click', RoteiroListar.editar);
		container.delegate('.btnPdf', 'click', RoteiroListar.PdfGerar);
		container.delegate('.btnDesativar', 'click', RoteiroListar.desativar);

		Aux.setarFoco(container);
		RoteiroListar.associarFuncao = fnAssociar;
		RoteiroListar.container = container;

		if (fnAssociar) {
			$('.hdnIsAssociar', container).val(true);
			container.delegate('.btnAssociar', 'click', RoteiroListar.associar);
		}
	},

	editar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(RoteiroListar.urlEditar + '/' + itemId);
	},

	PdfGerar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		MasterPage.redireciona(RoteiroListar.urlPdfRoteiro + '/' + itemId);
	},

	visualizar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		var content = MasterPage.getContent($(this, RoteiroListar.container));

		if (RoteiroListar.associarFuncao) {
			Modal.abrir($('.urlVisualizar', content).val() + "/" + itemId, null, function (context) {
				Modal.defaultButtons(context);
			});
		} else {
			MasterPage.redireciona($('.urlVisualizar', content).val() + "/" + itemId);
		}
	},

	associar: function () {
		var modal = RoteiroListar.container;
		var linha = $(this).parents('tr');
		var id = linha.find('.itemId').val();
		var numero = linha.find('.roteiroNumero').text();
		var texto = $.trim(linha.find('.roteiroNome').text()).replace("\n", "");
		var versao = linha.find('.roteiroVersao').text();
		var tid = linha.find('.roteiroTid').val();

		var msgErro = RoteiroListar.associarFuncao(id, numero, texto, versao, tid);

		if (msgErro !== undefined && msgErro.length > 0) {
			Mensagem.gerar(MasterPage.getContent(modal), msgErro);
		} else {
			Modal.fechar(modal);
		}
	},

	desativar: function () {
		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		var retorno = MasterPage.validarAjax(RoteiroListar.urlDesativarRoteiro, { id: itemId }, RoteiroListar.container, false);

		if (!retorno.EhValido) {
			return;
		}

		Modal.executar({
			'urlConfirm': RoteiroListar.urlDesativarComfirm,
			'urlAcao': RoteiroListar.urlDesativar,
			'id': itemId,
			'btnAcao': this,
			'btnTexto': 'Desativar'
		});
	}
}
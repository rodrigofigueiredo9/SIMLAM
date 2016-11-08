HabilitacaoEmissaoPTVListar = {
	urlVisualizar: null,
	urlEditar: null,
	urlAlterarSituacao: null,
	urlGerarPDF: null,
	container: null,
	settings: {
		associarFuncao: null
	},

	load: function (container, options) {
		container = MasterPage.getContent(container);
		if (options) { $.extend(HabilitacaoEmissaoPTVListar.settings, options); }

		container.listarAjax();
		container.delegate('.btnEditar', 'click', HabilitacaoEmissaoPTVListar.editar);
		container.delegate('.btnVisualizar', 'click', HabilitacaoEmissaoPTVListar.visualizar);
		container.delegate('.btnDesativar', 'click', HabilitacaoEmissaoPTVListar.desativar);
		container.delegate('.btnAtivar', 'click', HabilitacaoEmissaoPTVListar.ativar);
		container.delegate('.btnGerarPDF', 'click', HabilitacaoEmissaoPTVListar.gerarPDF);

		Aux.setarFoco(container);
		HabilitacaoEmissaoPTVListar.container = container;
	},

	obter: function (container) {
		return JSON.parse($(container).closest('tr').find('.itemJson:first').val());
	},

	visualizar: function () {
		var objeto = HabilitacaoEmissaoPTVListar.obter(this);
		MasterPage.redireciona(HabilitacaoEmissaoPTVListar.urlVisualizar + '/' + objeto.Id);
	},

	editar: function () {
		var objeto = HabilitacaoEmissaoPTVListar.obter(this);
		MasterPage.redireciona(HabilitacaoEmissaoPTVListar.urlEditar + '/' + objeto.Id);
	},

	ativar: function () {
		var objeto = HabilitacaoEmissaoPTVListar.obter(this);
		var linha = $(this).closest('tr');

		$.ajax({
			url: HabilitacaoEmissaoPTVListar.urlAlterarSituacao,
			data: JSON.stringify({ id: objeto.Id, status: 1 }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$(linha).find('.btnAtivar').closest('div').addClass('hide');
					$(linha).find('.btnDesativar').closest('div').removeClass('hide');
					$(linha).find('.situacao').text('Ativo');
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(HabilitacaoEmissaoPTVListar.container, response.Msg);
				}
			}
		});
	},

	desativar: function () {
		var objeto = HabilitacaoEmissaoPTVListar.obter(this);
		var linha = $(this).closest('tr');
		$.ajax({
			url: HabilitacaoEmissaoPTVListar.urlAlterarSituacao,
			data: JSON.stringify({ id: objeto.Id, status: 0 }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$(linha).find('.btnAtivar').closest('div').removeClass('hide');
					$(linha).find('.btnDesativar').closest('div').addClass('hide');
					$(linha).find('.situacao').text('Inativo');
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(HabilitacaoEmissaoPTVListar.container, response.Msg);
				}
			}
		});
	},

	gerarPDF: function () {
		var objeto = HabilitacaoEmissaoPTVListar.obter(this);
		MasterPage.redireciona(HabilitacaoEmissaoPTVListar.urlGerarPDF + '/' + objeto.Id);
	}
}
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

TramitacaoConfigurar = {
	settings: {
		container: null,
		urls: {
			validarFuncContidoSetor: '',
			validarAssociarFunc: '',
			salvar: ''
		},
		Mensagens: null
	},

	load: function (container, options) {
		if (options) {
			$.extend(TramitacaoConfigurar.settings, options);
		}

		TramitacaoConfigurar.settings.container = container;

		container.delegate('.rdbTipoTramitacao', 'change', TramitacaoConfigurar.onTipoTramitacao);
		container.delegate('.btnAssociarFuncionario', 'click', TramitacaoConfigurar.onAssociarFuncionario);
		container.delegate('.btnExcluirFunc', 'click', TramitacaoConfigurar.onExcluirFuncionario);
		container.delegate('.btnTramitConfigSalvar', 'click', TramitacaoConfigurar.onSalvar);

		container.associarMultiplo({
			'onExpandirEsconder': function () { MasterPage.redimensionar(); }
		});
	},

	onTipoTramitacao: function () {
		if ($(this).closest('.asmConteudoInterno').find('.rdbSim').attr('checked')) {
			$(this).closest('.divHiddenItemContainer').find('.divTramitacaoRegistro').removeClass('hide');
		} else {
			$(this).closest('.divHiddenItemContainer').find('.divTramitacaoRegistro').addClass('hide');
		}
	},

	onAssociarFuncionario: function () {
		$('.divHiddenItemContainer', TramitacaoConfigurar.settings.container).removeClass('associando');
		$(this).closest('.divHiddenItemContainer').addClass('associando');

		Modal.abrir(TramitacaoConfigurar.settings.urls.associarFuncionario, null, function (container) {
			FuncionarioListar.load(container, { associarFuncao: TramitacaoConfigurar.callBackAssociarFuncionario });
			Modal.defaultButtons(container);
		});
	},

	callBackAssociarFuncionario: function (Funcionario, container) {
		var tabela = $('.tabFuncionarios', $('.associando', TramitacaoConfigurar.settings.container));
		var arrayMensagem = new Array();

		$(tabela).find('.hdnFuncId').each(function () {
			if ($(this).val() == Funcionario.Id) {
				arrayMensagem.push(TramitacaoConfigurar.settings.Mensagens.FuncionarioJaAdicionado);
				return;
			}
		});

		if (arrayMensagem && arrayMensagem.length > 0) {
			return arrayMensagem;
		}

		var params = {
			funcionario: Funcionario.Id,
			setor: tabela.closest('.divHiddenItemContainer').find('.hdnItemId').val(),
			funcionarioNome: Funcionario.Nome,
			setorSigla: tabela.closest('.divHiddenItemContainer').find('.hdnItemSigla').val()
		};

		var retorno = MasterPage.validarAjax(TramitacaoConfigurar.settings.urls.validarFuncContidoSetor, params, container, false);
		if (!retorno.EhValido) {
			return retorno.Msg;
		}
		var linha = $('.trFuncTemplate').clone().removeClass('trFuncTemplate');

		linha.find('.hdnFuncId').val(Funcionario.Id);
		linha.find('.hdnFuncNome').val(Funcionario.Nome);
		linha.find('.trFuncNome').text(Funcionario.Nome);
		linha.find('.trFuncNome').attr('title', Funcionario.Nome);
		tabela.append(linha);
		Listar.atualizarEstiloTable($('.tabFuncionarios', TramitacaoConfigurar.settings.container));
		return true;
	},

	onExcluirFuncionario: function () {
		var tabela = $(this).closest('tbody');

		if (tabela.find('tr').length == 1) {
			var arrayMensagem = new Array();
			arrayMensagem.push(TramitacaoConfigurar.settings.Mensagens.FuncionarioObrigatorio);
			Mensagem.gerar(TramitacaoConfigurar.settings.container, arrayMensagem);
			return;
		}

		$(this).closest('tr').remove();

		$(tabela).find('tr').each(function (i, linha) {
			$(linha).removeClass();
			$(linha).addClass((i % 2) === 0 ? 'par' : 'impar');
		});
	},

	onSalvar: function () {
		var Setores = new Array();

		function Setor() {
			this.Id = 0;
			this.Sigla = '';
			this.IdRelacao = 0;
			this.TramitacaoTipoId = 1;
			this.Funcionarios = [];
		};

		function Funcionario() {
			this.Id = 0;
			this.Texto = '';
		};

		$('.divHiddenItemContainer', TramitacaoConfigurar.settings.container).each(function (i, divSetor) {
			var setor = new Setor();
			setor.Id = parseInt($(divSetor).find('.hdnItemId').val());
			setor.Sigla = $(divSetor).find('.hdnItemSigla').val();
			setor.IdRelacao = parseInt($(divSetor).find('.hdnItemIdRelacao').val());
			setor.TramitacaoTipoId = $(divSetor).find('.rdbSim').attr('checked') ? 2 : 1;

			if (setor.TramitacaoTipoId === 2) {
				$('.tabFuncionarios tbody tr', divSetor).each(function (i, trFunc) {
					var funcionario = new Funcionario();
					funcionario.Id = parseInt($('.hdnFuncId', trFunc).val());
					funcionario.Texto = $('.hdnFuncNome', trFunc).val();
					setor.Funcionarios.push(funcionario);
				});
			}

			Setores.push(setor);
		});

		var params = { setores: Setores, urlRedirecionar: TramitacaoConfigurar.settings.urls.salvar };

		$.ajax({
			url: TramitacaoConfigurar.settings.urls.salvar,
			data: JSON.stringify(params),
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(TramitacaoConfigurar.settings.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Salvo) {
					if (typeof response.UrlRedireciona != "undefined" && response.UrlRedireciona !== null) {
						MasterPage.redireciona(response.UrlRedireciona);
					}
				} else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(TramitacaoConfigurar.settings.container), response.Msg);
				}
			}
		});
	}
}
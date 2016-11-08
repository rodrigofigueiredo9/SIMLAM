/// <reference path="../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../Lib/JQuery/jquery.json - 2.2.min.js" />
/// <reference path="../masterpage.js" />

TramitacaoArquivo = {
	container: null,
	settings: {
		urls: null,
		Mensagens: null,
		id: 0
	},

	load: function (context, options) {
		if (options) {
			$.extend(TramitacaoArquivo.settings, options);
		}
		TramitacaoArquivo.container = context;
		$('.btnTramitacaoArqSalvar', TramitacaoArquivo.container).click(TramitacaoArquivo.onSalvar);
		$('.txtNome', context).focus();

		Localizacao.settings.urls = TramitacaoArquivo.settings.urls;
		Localizacao.settings.mensagens = TramitacaoArquivo.settings.Mensagens;
		Localizacao.load(context);
	},

	obterSituacaoProcDoc: function (contexto) {
		var countSituacao = 0;
		$(contexto, TramitacaoArquivo.container).find('input[type="checkbox"]:checked').each(function () {
			countSituacao = countSituacao + parseInt($(this).val());
		});
		return countSituacao;
	},

	onSalvar: function () {
		var Objeto = {
			Id: TramitacaoArquivo.settings.id,
			Nome: $.trim($('.txtNome', TramitacaoArquivo.container).val()),
			SetorId: $('.ddlSetor', TramitacaoArquivo.container).val(),
			TipoId: $('.ddlTipo', TramitacaoArquivo.container).val(),
			Estantes: Localizacao.gerarObjeto(),
			ProtocoloSituacao: TramitacaoArquivo.obterSituacaoProcDoc('.divProcessoSituacaos')//,
			//DocumentoSituacao: TramitacaoArquivo.obterSituacaoProcDoc('.divDocumentoSituacaos')
		};

		var msg = new Array();
		var itemContexto = {};
		$('.divConteudoEstante .asmItens .asmItemContainer', Localizacao.container).each(function (i, itemEstante) {
			if ($('.txtEstanteNome', itemEstante).val() == '' && msg.length <= 0) {
				itemContexto = itemEstante;
				msg.push(Localizacao.settings.mensagens.CamposObrigatorio)
			}
		});

		if (msg.length > 0) {
			Localizacao.publicarMensagem(msg);
			$(itemContexto).find('.fsEstante, input[type=text], select').addClass('erroCampo');
			return;
		}

		MasterPage.carregando(true);
		$.ajax({ url: TramitacaoArquivo.settings.urls.salvar,
			data: JSON.stringify(Objeto),
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			cache: false,
			async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(TramitacaoArquivo.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.IsArquivoSalvo) {
					MasterPage.redireciona(response.UrlRedireciona);
					return;
				}
				else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(TramitacaoArquivo.container), response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}

Localizacao = {
	settings: {
		urls: null,
		mensagens: {}
	},
	container: null,

	load: function (container) {
		Localizacao.container = MasterPage.getContent(container);
		container.delegate('.btnAddPrateleira', 'click', Localizacao.onAddPrateleira);
		container.delegate('.btnExcluirLinha', 'click', Localizacao.onExcluirPrateleira);
		Localizacao.configurarAssociar();
		Listar.atualizarEstiloTable($('.tabPrateleira', Localizacao.container));
	},

	configurarAssociar: function () {
		$('.divConteudoEstante', Localizacao.container).associarMultiplo({
			'minItens': 1,
			'onExcluirClick': Localizacao.onExcluirEstante,
			'onAdicionarClick': Localizacao.onAddEstante,
			'onItemAdicionado': Localizacao.onAfterAddItem,
			'msgObrigatoriedade': Localizacao.settings.mensagens.CamposObrigatorio,
			'tituloExcluir': 'Excluir Estante',
			'msgExcluir': Localizacao.settings.mensagens.ExcluirEstante.Texto
		});
	},

	onAddEstante: function (item, extra) {
		var podeAdicionar = true;
		$('.asmItens .asmItemContainer', item).each(function (i, itemLista) {
			if ($(' .txtEstanteNome', itemLista).val() == '' || $('.tabPrateleira tbody tr', itemLista).length <= 0) {
				Localizacao.publicarMensagem(new Array(Localizacao.settings.mensagens.CamposObrigatorio));
				$(itemLista).find('.fsEstante, input[type=text], select').addClass('erroCampo');
				podeAdicionar = false;
			}
		});
		return podeAdicionar;
	},

	onAfterAddItem: function myfunction(novoItem) {
		$('.hdnEstanteId', novoItem).val(-1);
	},

	onExcluirEstante: function (item, extra) {

		var id = $('.hdnEstanteId', item).val();

		if (id > 0) {
			var retorno = MasterPage.validarAjax(Localizacao.settings.urls.validarExcluirEstante, { idEstante: id }, TramitacaoArquivo.container, false);
			if (!retorno.EhValido) return false;
		}
		return true;
	},

	onAddPrateleira: function () {
		Mensagem.limpar(Localizacao.container);
		$(Localizacao.container).removeClass('erroModo erroIdentificacao');
		var mensagens = new Array();
		var containerPrateleira = $(this).closest('.fsPrateleira');
		var containerEstante = $(this).closest('.divConteudoEstante');

		var modoId = $('.ddlModo', containerPrateleira).val();
		var identificacao = $('.txtIdentificacao', containerPrateleira).val();

		if (modoId == 0) {
			$('.ddlModo', containerPrateleira).addClass('erroModo');
			mensagens.push(Localizacao.settings.mensagens.ModoObrigratorio);
		}

		if (identificacao == '') {
			$('.txtIdentificacao', containerPrateleira).addClass('erroIdentificacao');
			mensagens.push(Localizacao.settings.mensagens.IdentificacaoObrigratorio);
		}

		if (Localizacao.publicarMensagem(mensagens)) {
			return false;
		}

		var tabelaPrateleiras = $('.tabPrateleira tbody tr', containerPrateleira);

		$(tabelaPrateleiras).each(function (i, item) {
			if ($('.hdnModoId', item).val() == modoId && $('.trIdentificacaoTexto', item).text() == identificacao) {
				mensagens.push(Localizacao.settings.mensagens.PrateleiraItemArquivoJaAdicionada);
			}
		});

		if (Localizacao.publicarMensagem(mensagens)) {
			return false;
		}

		var linha = $('.templatePrateleira', containerPrateleira).clone();

		linha.find('.hdnModoId').val(modoId);

		linha.find('.trModoTexto').text($('.ddlModo :selected', containerPrateleira).text());
		linha.find('.trModoTexto').attr('title', $('.ddlModo :selected', containerPrateleira).text());

		linha.find('.trIdentificacaoTexto').text(identificacao);
		linha.find('.trIdentificacaoTexto').attr('title', identificacao);

		linha.removeClass('templatePrateleira hide');

		$('.tabPrateleira > tbody:last', containerPrateleira).append(linha);

		Listar.atualizarEstiloTable($('.tabPrateleira', containerPrateleira));

		$('.ddlModo', containerPrateleira).find('option:first').attr('selected', 'selected');
		$('.txtIdentificacao', containerPrateleira).val('')

		$(containerEstante).removeClass('erroCampo');
	},

	onExcluirPrateleira: function () {

		var linha = $(this).closest('tr');

		var containerPrateleira = $(this).closest('.fsPrateleira');
		var containerEstante = $(this).closest('.divConteudoEstante');

		$(this).closest('tr').remove();

		Listar.atualizarEstiloTable($('.tabPrateleira', containerPrateleira));

		var tabelaPrateleiras = $('.tabPrateleira tbody tr', containerPrateleira);
	},

	gerarObjeto: function () {

		function EstanteObj() {
			this.Id = 0;
			this.Texto = '';
			this.Prateleiras = new Array();
		};

		var lista = new Array();

		$('.divConteudoEstante .asmItens .asmItemContainer', Localizacao.container).each(function (i, itemEstante) {

			if ($('.txtEstanteNome', itemEstante).val() == '') {
				return;
			}

			var estante = new EstanteObj();

			estante.Id = $('.hdnEstanteId', itemEstante).val();

			estante.Texto = $('.txtEstanteNome', itemEstante).val();

			$('.fsEstante, .txtEstanteNome', itemEstante).attr('id', 'Estante_' + i);

			var prateleiras = new Array();
			$('.tabPrateleira tbody tr', itemEstante).each(function (j, itemPrateleira) {

				var objeto = { IdRelacionamento: 0, Identificacao: '', ModoId: 0, ModoTexto: '' };
				objeto.Id = $('.hdnIdRelacionamento', itemPrateleira).val();
				objeto.ModoId = $('.hdnModoId', itemPrateleira).val();
				objeto.ModoTexto = $('.trModoTexto', itemPrateleira).text();
				objeto.Texto = $('.trIdentificacaoTexto', itemPrateleira).text();
				estante.Prateleiras.push(objeto);
			});

			lista.push(estante);
		});

		return lista;
	},

	publicarMensagem: function (mensagens) {
		if (mensagens.length > 0) {
			Mensagem.gerar(Localizacao.container, mensagens)
			return true;
		}
		return false;
	}
}

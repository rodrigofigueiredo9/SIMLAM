/// <reference path="declaracaoAdicional.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../masterpage.js" />

Cultura = {
	settings: {
		urls: {
			salvar: '',
			obter: '',
            outro_estado : '',
			urlDeclaracaoAdicional:null
		},
		Mensagens: null
	},
	container: null,

	load: function (container, options) {

		if (options) {
			$.extend(Cultura.settings, options);
		}

		Cultura.container = MasterPage.getContent(container);
		Cultura.container = container;
		Cultura.container.delegate('.btnSalvar', 'click', Cultura.salvar);
		Cultura.container.delegate('.btnAdicionar', 'click', Cultura.onAdicionarCultivar);
		Cultura.container.delegate('.btnEditar', 'click', Cultura.editar);
		Cultura.container.delegate('.btnItemEditar', 'click', Cultura.onItemEditar);
		Cultura.container.delegate('.rdbOutroEstado', 'click', Cultura.onOutroEstado);

		Cultura.container.delegate('.btnConfigurar', 'click', Cultura.configurarDeclaracaoAdicional);
		Aux.setarFoco(Cultura.container);
	},	


	onOutroEstado: function () {

	    $.ajax({
	        url: Cultura.settings.urls.outro_estado,
	        data: JSON.stringify(Cultura.obter()),
	        cache: false,
	        async: false,
	        type: 'POST',
	        dataType: 'json',
	        contentType: 'application/json; charset=utf-8',
	        error: function (XMLHttpRequest, textStatus, erroThrown) {
	            Aux.error(XMLHttpRequest, textStatus, erroThrown, Cultura.container);
	        },
	        success: function (response, textStatus, XMLHttpRequest) {
	            if (response.EhValido) {
	                if (response.Url) {
	                    MasterPage.redireciona(response.Url);
	                }
	            }

	            if (response.Msg && response.Msg.length > 0) {
	                Mensagem.gerar(Cultura.container, response.Msg);
	            }
	        }
	    });

	},

	onAdicionarCultivar: function () {
		Mensagem.limpar(Cultura.container);
		var validacao = true;

		if ($('.txtCultivar', Cultura.container).val() == '') {
			Mensagem.gerar(MasterPage.getContent(Cultura.container), [Cultura.settings.Mensagens.CultivarObrigatorio]);
			return;
		}

		$('label', $('.gridCultivar tbody tr')).each(function () {
			if ($(this).html().trim() == $('.txtCultivar', Cultura.container).val().trim()) {
				validacao = false;
			}
		});

		if (!validacao) {
			Mensagem.gerar(MasterPage.getContent(Cultura.container), [Cultura.settings.Mensagens.CultivarJaAdicionado]);
			return;
		}

		var linha = $('.tr_template', Cultura.container).clone();				
		$('.lblNome', linha).html($('.txtCultivar', Cultura.container).val());				
		
		if ($('.ddlTipoProducao option:selected', Cultura.container).val() == '0') {
			$('.lblProduto', linha).html('');
		} else {
			$('.lblProduto', linha).html($('.ddlTipoProducao option:selected', Cultura.container).text());			
		}
		
		if ($('.ddlDeclaracaoAdicional option:selected', Cultura.container).val() == '0') {
			$('.lblDeclaracaoAdicional', linha).html('');
		} else {
			$('.lblDeclaracaoAdicional', linha).html($('.ddlDeclaracaoAdicional option:selected', Cultura.container).text());			
		}
		
		$('.hdnItemIndexProducao', linha).val($('.ddlTipoProducao option:selected', Cultura.container).val());
		$('.hdnItemIndexDeclaracao', linha).val($('.ddlDeclaracaoAdicional option:selected', Cultura.container).val());
		$('.hdnItemId', linha).val('0');
		$('.hdnItemIndex', linha).val($('.gridCultivar tbody tr:not(.tr_template)').length);

		$(linha).removeClass('hide');
		$(linha).removeClass('tr_template');

		$('.gridCultivar tbody', Cultura.container).append($(linha));

		$('.txtCultivar', Cultura.container).val('');
		$('.ddlTipoProducao', Cultura.container).val('0');
		$('.ddlDeclaracaoAdicional', Cultura.container).val('0');
		Listar.atualizarEstiloTable($('.gridCultivar', Cultura.container));
	},

	configurarDeclaracaoAdicional: function () {
		$('.itemConfigurar', Cultura.container).removeClass('itemConfigurar');
		var linha = $(this).closest('tr');
		linha.addClass('itemConfigurar');

		var item = {
			IdRelacionamento: $('.hdnId', Cultura.container).val(),
			Id: $('.hdnItemId', linha).val(),
			CulturaTexto: $('.txtCultura', Cultura.container).val(),
			Nome: linha.find('.lblNome').text(),
			LsCultivarConfiguracao: (linha.find('.hdnItemJson').val() == '' ? {} : JSON.parse(linha.find('.hdnItemJson').val()))
		};

		Modal.abrir(Cultura.settings.urls.urlDeclaracaoAdicional, item, function (container) {
			DeclaracaoAdicional.load(container, {
				associarFuncao: Cultura.callBackDeclaracaoAdicional,
				Mensagens: Cultura.settings.Mensagens
			});
		}, Modal.tamanhoModalMedia, 'Configurar Declaração Adicional');		
	},

	callBackDeclaracaoAdicional: function (configuracaoObj) {		
		var linha = $('.itemConfigurar', Cultura.container);		
		$('.hdnItemJson', linha).val(JSON.stringify(configuracaoObj));
	},
	
	onItemEditar: function () {	
		Mensagem.limpar(Cultura.container);		
		var item = {
			Index: $(this).closest('tr').find('.hdnItemIndex').val(),
			Texto: $(this).closest('tr').find('.lblNome').text().trim(),
			IndexProducao: $(this).closest('tr').find('.hdnItemIndexProducao').val(),
			IndexDeclaracaoAdicional: $(this).closest('tr').find('.hdnItemIndexDeclaracao').val()			
		};

		$(this).closest('tr').addClass('itemEdicao');
		$('.ddlTipoProducao', Cultura.container).val(item.IndexProducao);		
		$('.ddlDeclaracaoAdicional', Cultura.container).val(item.IndexDeclaracaoAdicional);
		$('.txtCultivar', Cultura.container).val(item.Texto);		
		$('.btnAdicionar', Cultura.container).addClass('hide');
		$('.btnEditar', Cultura.container).removeClass('hide');
	},

	editar: function () {	

		if ($('.txtCultivar', Cultura.container).val() == '') {
			Mensagem.gerar(MasterPage.getContent(Cultura.container), [Cultura.settings.Mensagens.CultivarObrigatorio]);
			return;
		}

		var validacao = true;
		$('label', $('.gridCultivar tbody tr:not(.itemEdicao)')).each(function () {
			if ($(this).html().trim() == $('.txtCultivar', Cultura.container).val().trim()) {
				validacao = false;
			}
		});

		if (!validacao) {
			Mensagem.gerar(MasterPage.getContent(Cultura.container), [Cultura.settings.Mensagens.CultivarJaAdicionado]);
			return;
		}

		var linha = $('.itemEdicao', Cultura.container);		
		$('.lblNome', $(linha)).html($('.txtCultivar', Cultura.container).val());		

		if ($('.ddlTipoProducao option:selected', Cultura.container).val() == '0') {
			$('.hdnItemIndexProducao', linha).val('0');
			$('.lblProduto', linha).html('');
		} else {
			$('.hdnItemIndexProducao', linha).val($('.ddlTipoProducao option:selected', Cultura.container).val());
			$('.lblProduto', linha).html($('.ddlTipoProducao option:selected', Cultura.container).text());
		}

		if ($('.ddlDeclaracaoAdicional option:selected', Cultura.container).val() == '0') {
			$('.hdnItemIndexDeclaracao', linha).val('0');
			$('.lblDeclaracaoAdicional', linha).html('');
		} else {
			$('.hdnItemIndexDeclaracao', linha).val($('.ddlDeclaracaoAdicional option:selected', Cultura.container).val());
			$('.lblDeclaracaoAdicional', linha).html($('.ddlDeclaracaoAdicional option:selected', Cultura.container).text());
		}				

		//limpa componente cultivar
		$('.txtCultivar', Cultura.container).val('');

		$(linha).removeClass('itemEdicao');
		$('.btnAdicionar', Cultura.container).removeClass('hide');
		$('.btnEditar', Cultura.container).addClass('hide');
	},

	excluir: function (){
		var par = $(this).parent().parent(); //tr	
		par.remove();		
	},
		
	obter: function () {
		var gridContainer = $('.gridCultivar tbody', Cultura.container);

		var CulturaObj = {
			Id: $('.hdnId', Cultura.container).val(),
			Nome: $('.txtCultura', Cultura.container).val(),			
			Tid: '',
			LstCultivar: new Array()				
		};

		$('tr:not(.tr_template)', gridContainer).each(function () {
			var objListaConfiguracao = $('.hdnItemJson', this).val() ? JSON.parse($('.hdnItemJson', this).val()) : null;
			CulturaObj.LstCultivar.push({
				Id: $('.hdnItemId', this).val(),
				Nome: $('.lblNome', this).html(),
				Tid: '',				
				LsCultivarConfiguracao: objListaConfiguracao
			});
		});

		return CulturaObj;
	},

	salvar: function () {

		if ($('.txtCultura', Cultura.container).val() == '') {
			Mensagem.gerar(MasterPage.getContent(Cultura.container), [Cultura.settings.Mensagens.CulturaObrigatorio]);
			return;
		}

		Mensagem.limpar(Cultura.container);
		MasterPage.carregando(true);

		$.ajax({
			url: Cultura.settings.urls.salvar,
			data: JSON.stringify(Cultura.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Cultura.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					if (response.Url) {
						MasterPage.redireciona(response.Url);
					}
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Cultura.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	}
}
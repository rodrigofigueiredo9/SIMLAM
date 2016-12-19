/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="../../mensagem.js" />

DeclaracaoAdicional = {
	settings: {
		urls: {
		    validarDeclaracaoAdicional: null,
            urlOutroEstado : null
		},
		associarFuncao: null,
		Mensagens: null
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(DeclaracaoAdicional.settings, options);
		}

		Modal.defaultButtons(container, DeclaracaoAdicional.salvar, 'Salvar');

		DeclaracaoAdicional.container = container;
		DeclaracaoAdicional.container.delegate('.btnAdicionar', 'click', DeclaracaoAdicional.adicionarCultivarConfiguracao);
		DeclaracaoAdicional.container.delegate('.btnItemExcluir', 'click', DeclaracaoAdicional.excluir);
		DeclaracaoAdicional.container.delegate('.rdbOutroEstado', 'change', DeclaracaoAdicional.onOutroEstado);

		$('.rdbOutroEstado').change();
	},

	onOutroEstado: function () {
	   
	   
	    var val_outro_estado = $('.rdbOutroEstado:visible:checked', DeclaracaoAdicional.container).val();
	

	    $.ajax({
	        url: DeclaracaoAdicional.settings.urls.urlOutroEstado,
	        data: JSON.stringify({ valOutroEstado: val_outro_estado }),
	        cache: false,
	        async: false,
	        type: 'POST',
	        dataType: 'json',
	        contentType: 'application/json; charset=utf-8',
	        error: function (XMLHttpRequest, textStatus, erroThrown) {
	            Aux.error(XMLHttpRequest, textStatus, erroThrown, Cultura.container);
	        },
	        success: function (response, textStatus, XMLHttpRequest) {
	            if (response.Declaracoes) {
	                $('.ddlDeclaracaoAdicional', DeclaracaoAdicional.container).ddlLoad(response.Declaracoes, { disabled: false });
	            }
	        }
	    });

	},
	
	adicionarCultivarConfiguracao: function () {
		Mensagem.limpar(DeclaracaoAdicional.container);
		
		var lista = DeclaracaoAdicional.obter();
		var item = {
			Cultivar: $('.hdnItemId', DeclaracaoAdicional.container).val(),
			PragaId: $('.ddlPragas option:selected', DeclaracaoAdicional.container).val(),
			PragaTexto: $('.ddlPragas option:selected', DeclaracaoAdicional.container).text(),
			TipoProducaoId: $('.ddlTipoProducao option:selected', DeclaracaoAdicional.container).val(),
			TipoProducaoTexto: $('.ddlTipoProducao option:selected', DeclaracaoAdicional.container).text(),
			DeclaracaoAdicionalId: $('.ddlDeclaracaoAdicional option:selected', DeclaracaoAdicional.container).val(),
			DeclaracaoAdicionalTexto: $('.ddlDeclaracaoAdicional option:selected', DeclaracaoAdicional.container).text(),
			OutroEstado: $('.rdbOutroEstado:checked', DeclaracaoAdicional.container).val()
		};

		var retorno = MasterPage.validarAjax(
			DeclaracaoAdicional.settings.urls.validarDeclaracaoAdicional, {
				item: item,
				lista: lista
			}, null, false);

		if (!retorno.EhValido && retorno.Msg) {
			Mensagem.gerar(DeclaracaoAdicional.container, retorno.Msg);
			return;
		}

		var linha = $('.trTemplate', DeclaracaoAdicional.container).clone();//clona linha

		$('.hdnItemJSON', linha).val(JSON.stringify(item));
		$('.lblPragas', linha).html(item.PragaTexto);
		$('.lblTipoProducao', linha).html(item.TipoProducaoTexto);
		$('.lblDeclaracaoAdicional', linha).html(item.DeclaracaoAdicionalTexto);		

		$(linha).removeClass('hide').removeClass('trTemplate');
		$('.gridDeclaracaoAdicional tbody', DeclaracaoAdicional.container).append($(linha));

		$('.ddlPragas', DeclaracaoAdicional.container).ddlFirst();
		$('.ddlTipoProducao', DeclaracaoAdicional.container).ddlFirst();
		$('.ddlDeclaracaoAdicional', DeclaracaoAdicional.container).ddlFirst();

		Listar.atualizarEstiloTable($('.gridDeclaracaoAdicional', DeclaracaoAdicional.container));
	},

	excluir: function () {
		Mensagem.limpar(DeclaracaoAdicional.container);
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable(DeclaracaoAdicional.container.find('.gridDeclaracaoAdicional'));
	},
		
	obter: function () {
		var lista = new Array();

		$('.gridDeclaracaoAdicional tbody tr:not(.trTemplate) .hdnItemJSON', DeclaracaoAdicional.container).each(function () {
			lista.push(JSON.parse($(this).val()));
		});

		return lista;
	},

	salvar: function () {
		Mensagem.limpar(DeclaracaoAdicional.container);

		if ($('.gridDeclaracaoAdicional tbody tr:not(.trTemplate)').length > 0) {
			Modal.fechar(DeclaracaoAdicional.container);
			DeclaracaoAdicional.settings.associarFuncao(DeclaracaoAdicional.obter());
		} else {
			Mensagem.gerar(DeclaracaoAdicional.container, [{ Tipo: 3, Texto: 'Pelo menos uma configuração é necessário.' }]);
		}
	}
}
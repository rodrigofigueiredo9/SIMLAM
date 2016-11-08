/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../mensagem.js" />

TermoCPFARLR = {
	container: null,

	settings: {
		urls: { urlObterDadosTermoCPFARLR: '', urlObterTituloAnterior: '' },
		Mensagens: null
	},

	load: function (especificidadeRef) {
		TermoCPFARLR.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
		$('.btnAdicionarDestinatario', TermoCPFARLR.container).click(TermoCPFARLR.adicionarDestinatario);
		TermoCPFARLR.container.delegate('.btnExcluirDestinatario', 'click', TermoCPFARLR.excluirDestinatario);
		TituloCondicionante.load($('.condicionantesContainer', TermoCPFARLR.container));
		TermoCPFARLR.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });
	},

	obterDadosTermoCPFARLR: function (protocolo) {

		$('.divDestinatarios .dataGridTable tbody tr:not(.trTemplateRow, .trCondTemplate)', TermoCPFARLR.container).remove();
		$('.ddlDestinatarios', TermoCPFARLR.container).ddlClear();
		$('.txtNumeroAverbacao', TermoCPFARLR.container).val('');
		$('.txtDataEmissao', TermoCPFARLR.container).val('');

		if (protocolo == null) {
			return;
		}

		$.ajax({
			url: TermoCPFARLR.settings.urls.urlObterDadosTermoCPFARLR,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(Titulo.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios.length > 0) {
					$('.ddlDestinatarios', TermoCPFARLR.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	adicionarDestinatario: function () {

		var mensagens = new Array();
		Mensagem.limpar(Titulo.container);
		var container = $('.divDestinatarios');

		var item = { Id: $('.ddlDestinatarios :selected', container).val(), Nome: $('.ddlDestinatarios :selected', container).text() };

		if (jQuery.trim(item.Nome) == '*** Selecione ***') {
			mensagens.push(jQuery.extend(true, {}, TermoCPFARLR.settings.Mensagens.DestinatarioObrigatorio));
		}

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var itemAdd = (JSON.parse(obj));
				if (item.Id == itemAdd.Id) {
					mensagens.push(jQuery.extend(true, {}, TermoCPFARLR.settings.Mensagens.DestinatarioJaAdicionado));
				}
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(Titulo.container, mensagens);
			return;
		}

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(item));
		linha.find('.Destinatario').html(item.Nome).attr('title', item.Nome);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.ddlDestinatarios', container).ddlFirst();
	},

	excluirDestinatario: function () {
		var container = $('.divDestinatarios');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	},

	obterObjeto: function () {

		var containerGrid = $('.gridDestinatarios', TermoCPFARLR.container);
		var especificidade = {
			NumeroAverbacao: $('.txtNumeroAverbacao', TermoCPFARLR.container).val(),
			DataTituloAnterior: { DataTexto: $('.txtDataEmissao', TermoCPFARLR.container).val() },
			Destinatarios: []
		};

		//Destinatarios
		container = TermoCPFARLR.container.find('.divDestinatarios');
		$('.hdnItemJSon', container).each(function () {
			var objDestinatario = String($(this).val());
			if (objDestinatario != '') {
				especificidade.Destinatarios.push(JSON.parse(objDestinatario));
			}
		});

		return especificidade;
	},

	obterAnexosObjeto: function () {
		var anexos = new Array();
		anexos = TermoCPFARLR.container.find('.fsArquivos').arquivo('obterObjeto');
		return anexos;
	}
};

Titulo.settings.especificidadeLoadCallback = TermoCPFARLR.load;
Titulo.settings.obterEspecificidadeObjetoFunc = TermoCPFARLR.obterObjeto;
Titulo.addCallbackProtocolo(TermoCPFARLR.obterDadosTermoCPFARLR);
Titulo.settings.obterAnexosCallback = TermoCPFARLR.obterAnexosObjeto;
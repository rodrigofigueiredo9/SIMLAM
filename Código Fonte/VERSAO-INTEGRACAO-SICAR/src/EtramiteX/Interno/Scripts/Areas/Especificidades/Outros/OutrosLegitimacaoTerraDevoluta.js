/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../mensagem.js" />
/// <reference path="../../../jquery.ddl.js" />

OutrosLegitimacaoTerraDevoluta = {
	container: null,

	settings: {
		urls: { urlObterDadosOutrosLegitimacaoTerraDevoluta: '' },
		Mensagens: null
	},

	load: function (especificidadeRef) {
		OutrosLegitimacaoTerraDevoluta.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
		OutrosLegitimacaoTerraDevoluta.container.delegate('.btnAdicionarDestinatario', 'click', OutrosLegitimacaoTerraDevoluta.adicionarDestinatario);
		OutrosLegitimacaoTerraDevoluta.container.delegate('.btnExcluirDestinatario', 'click', OutrosLegitimacaoTerraDevoluta.excluirDestinatario);
	},

	obterDadosOutrosLegitimacaoTerraDevoluta: function (protocolo) {

		if (protocolo == null) {
			$('.ddlDestinatarios', OutrosLegitimacaoTerraDevoluta.container).ddlClear();
			$('.ddlDominios', OutrosLegitimacaoTerraDevoluta.container).ddlClear();
			$('.txtValorTerreno', OutrosLegitimacaoTerraDevoluta.container).val('');
			$('.rdbIsInalienabilidade', OutrosLegitimacaoTerraDevoluta.container).removeAttr('checked');

			$('.dataGridTable tbody tr', OutrosLegitimacaoTerraDevoluta.container).each(function () {
				if (!$(this).hasClass('hide')) {
					$(this).remove();
				}
			});

			return;
		}

		$.ajax({ url: OutrosLegitimacaoTerraDevoluta.settings.urls.urlObterDadosOutrosLegitimacaoTerraDevoluta,
			data: JSON.stringify({ id: protocolo.Id, Empreendimento: { Id: protocolo.EmpreendimentoId } }),
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
					$('.ddlDestinatarios', OutrosLegitimacaoTerraDevoluta.container).ddlLoad(response.Destinatarios);
				}
				if (response.Dominios.length > 0) {
					$('.ddlDominios', OutrosLegitimacaoTerraDevoluta.container).ddlLoad(response.Dominios);
				}
				$('.ddlMunicipioGleba', OutrosLegitimacaoTerraDevoluta.container).ddlSelect({ selecionado: response.MunicipioEmp });
			}
		});
	},

	adicionarDestinatario: function () {
		Mensagem.limpar(Titulo.container);
		var mensagens = new Array();
		var container = $('.divDestinatarios');

		var item = { Id: $('.ddlDestinatarios :selected', container).val(), Nome: $('.ddlDestinatarios :selected', container).text() };

		if (jQuery.trim(item.Nome) == '*** Selecione ***') {
			mensagens.push(jQuery.extend(true, {}, OutrosLegitimacaoTerraDevoluta.settings.Mensagens.DestinatarioObrigatorio));
		}

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var itemAdd = (JSON.parse(obj));
				if (item.Id == itemAdd.Id) {
					mensagens.push(jQuery.extend(true, {}, OutrosLegitimacaoTerraDevoluta.settings.Mensagens.DestinatarioJaAdicionado));
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
		var especificidade = {
			Dominio: $('.ddlDominios', OutrosLegitimacaoTerraDevoluta.container).val(),
			ValorTerreno: $('.txtValorTerreno', OutrosLegitimacaoTerraDevoluta.container).val(),
			MunicipioGlebaId: $('.ddlMunicipioGleba', OutrosLegitimacaoTerraDevoluta.container).val(),
			IsInalienabilidade: '',
			Destinatarios: []
		};

		if ($('.rdbSim:checked', OutrosLegitimacaoTerraDevoluta.container).val()) {
			especificidade.IsInalienabilidade = true;
		}

		if ($('.rdbNao:checked', OutrosLegitimacaoTerraDevoluta.container).val()) {
			especificidade.IsInalienabilidade = false;
		}

		//Destinatarios
		container = OutrosLegitimacaoTerraDevoluta.container.find('.divDestinatarios');
		$('.hdnItemJSon', container).each(function () {
			var objDestinatario = String($(this).val());
			if (objDestinatario != '') {
				especificidade.Destinatarios.push(JSON.parse(objDestinatario));
			}
		});

		return especificidade;
	}
};

Titulo.settings.especificidadeLoadCallback = OutrosLegitimacaoTerraDevoluta.load;
Titulo.settings.obterEspecificidadeObjetoFunc = OutrosLegitimacaoTerraDevoluta.obterObjeto;
Titulo.addCallbackProtocolo(OutrosLegitimacaoTerraDevoluta.obterDadosOutrosLegitimacaoTerraDevoluta);
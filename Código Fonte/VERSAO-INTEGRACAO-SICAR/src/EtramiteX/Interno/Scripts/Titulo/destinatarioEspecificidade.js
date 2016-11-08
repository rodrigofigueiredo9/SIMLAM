/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />
/// <reference path="../jquery.json-2.2.min.js" />

DestinatarioEspecificidade = {
	settings: {
		mensagens: null,
		urls: {
			obterDadosDestinatarioEspecificadade: ''
		}
	},
	container: null,

	load: function (especificidadeRef) {
		DestinatarioEspecificidade.container = especificidadeRef;

		especificidadeRef.delegate('.btnAdicionarDestinatariosEsp', 'click', DestinatarioEspecificidade.onAdicionar);
		especificidadeRef.delegate('.btnExcluirDestinatarioEsp', 'click', DestinatarioEspecificidade.onExcluir);
	},

	obterDestinatarioEspecificidade: function (protocolo) {
		if (protocolo == null) {
			DestinatarioEspecificidade.clear();
			return;
		}

		$.ajax({
			url: DestinatarioEspecificidade.settings.urls.obterDadosDestinatarioEspecificadade,
			data: JSON.stringify({ id: protocolo.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(DestinatarioEspecificidade.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarioEsp', DestinatarioEspecificidade.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	onAdicionar: function () {
		var id = parseInt($('.ddlDestinatarioEsp', DestinatarioEspecificidade.container).val()) || 0;
		var texto = $('.ddlDestinatarioEsp :selected', DestinatarioEspecificidade.container).text();
		var ehValido = true;

		if (id <= 0) {
			Mensagem.gerar(MasterPage.getContent(DestinatarioEspecificidade.container), new Array(DestinatarioEspecificidade.settings.mensagens.DestinatarioObrigatorio));
			return;
		}

		if (ehValido) {
			Mensagem.limpar(DestinatarioEspecificidade.container);
			var tabela = $('.dgDestinatariosEsp', DestinatarioEspecificidade.container);
			var linha = $('.trTemplateRow', DestinatarioEspecificidade.container).clone()
								.removeClass('trTemplateRow')
								.removeClass('hide');
			var adicionar = true;

			tabela.find('tbody tr').each(function (i, linha) {
				if ($(linha).find('.hdnDestinatarioId').val() == id) {
					adicionar = false;
					Mensagem.gerar(MasterPage.getContent(DestinatarioEspecificidade.container), new Array(DestinatarioEspecificidade.settings.mensagens.DestinatarioJaAdicionado));
					return;
				}
			});

			if (adicionar) {
				linha.find('.hdnDestinatarioId').val(id);
				linha.find('.hdnDestinatarioJSon').val(JSON.stringify({ Id: id, Nome: texto }));
				linha.find('.Destinatario').html(texto).attr('title', texto);
				tabela.find('tbody:last').append(linha);

				Listar.atualizarEstiloTable(tabela);
			}
		}
	},

	onExcluir: function () {
		var container = $('.dgDestinatariosEsp');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dgDestinatariosEsp'));
	},

	obter: function () {

		var lstDest = [];
		var destinatarioContainer = DestinatarioEspecificidade.container.find('.dgDestinatariosEsp');
		$('.hdnDestinatarioJSon', destinatarioContainer).each(function () {
			var objDestinatarios = String($(this).val());
			if (objDestinatarios != '') {
				lstDest.push(JSON.parse(objDestinatarios));
			}
		});

		return lstDest;
	},

	clear: function () {
		$('.ddlDestinatarioEsp', DestinatarioEspecificidade.container).ddlClear();
		$('.dgDestinatariosEsp tbody tr:not(.trTemplateRow)', DestinatarioEspecificidade.container).empty();
	}
};

Titulo.addCallbackProtocolo(DestinatarioEspecificidade.obterDestinatarioEspecificidade);
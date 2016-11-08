/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../mensagem.js" />
/// <reference path="../../../jquery.ddl.js" />

TermoCPFARLCR = {
	container: null,
	settings: {
		urls: {
			obterDadosTermoCPFARLCR: null,
			obterARL: null,
			obterDadosEmpreendimentoReceptor: null,
			validarAdicionarARL: null
		},
		dominialidadeID: null,
		Mensagens: null
	},

	load: function (especificidadeRef) {
		TermoCPFARLCR.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);

		$('.ddlCedenteDominio', TermoCPFARLCR.container).change(TermoCPFARLCR.changeDdlCedenteDominio);
		$('.btnAddCedenteARLCompensacao', TermoCPFARLCR.container).click(TermoCPFARLCR.addCedenteARLCompensacao);
		$('.btnAddResponsavelEmp', TermoCPFARLCR.container).click(TermoCPFARLCR.addResponsavelEmpreendimento);
		TermoCPFARLCR.container.delegate('.btnExcluir', 'click', TermoCPFARLCR.excluirLinha);
		TituloCondicionante.load($('.condicionantesContainer', TermoCPFARLCR.container));
	},

	obterDadosTermoCPFARLCR: function (especificidade) {
		var mensagens = new Array();
		$('.dataGridTable tbody tr:not(.trTemplateRow, .trCondTemplate)', TermoCPFARLCR.container).remove();
		$('.ddlCedenteDominio', TermoCPFARLCR.container).ddlClear();
		$('.ddlCedenteARLCompensacao', TermoCPFARLCR.container).ddlClear();
		$('.ddlCedenteResposaveisEmp', TermoCPFARLCR.container).ddlClear();

		$('.ddlReceptorEmpreendimentos', TermoCPFARLCR.container).ddlClear();
		$('.ddlReceptorDominio', TermoCPFARLCR.container).ddlClear();
		$('.ddlReceptorResposaveisEmp', TermoCPFARLCR.container).ddlClear();
		$('.txtNumeroAverbacao').val('');
		$('.txtDataEmissao').val('');

		if (especificidade.EmpreendimentoId == 0) {
			mensagens.push(TermoCPFARLC.settings.Mensagens.CedenteEmpreendimentoObrigatorio);
			Mensagem.gerar(Titulo.container, mensagens);
			return;
		}

		$.ajax({
			url: TermoCPFARLCR.settings.urls.obterDadosTermoCPFARLCR,
			data: JSON.stringify(especificidade),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				TermoCPFARLCR.settings.dominialidadeID = response.Dominialidade.Id;

				$('.ddlCedenteDominio', TermoCPFARLCR.container).ddlLoad(response.CedenteDominios);
				$('.ddlCedenteARLCompensacao', TermoCPFARLCR.container).ddlLoad(response.CedenteARLCompensacao);
				$('.ddlCedenteResposaveisEmp', TermoCPFARLCR.container).ddlLoad(response.CedenteResponsaveisEmpreendimento, { disabled: false });
			}
		});
	},

	changeDdlCedenteDominio: function () {
		Mensagem.limpar(Titulo.container);
		var item = $('.ddlCedenteDominio', TermoCPFARLCR.container).ddlSelecionado();
		$('.dataGridTable:not(.dgCedenteResponsaveis, .dgCondicionantes) tbody tr:not(.trTemplateRow, .trCondTemplate)', TermoCPFARLCR.container).remove();

		$.ajax({
			url: TermoCPFARLCR.settings.urls.obterARL,
			data: JSON.stringify({ dominio: item.Id }),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				$('.ddlCedenteARLCompensacao', TermoCPFARLCR.container).ddlLoad(response.ARLCedente, { disabled: false });
			}
		});
	},

	addCedenteARLCompensacao: function () {
		var mensagens = new Array();
		Mensagem.limpar(Titulo.container);

		var ddl = $('.ddlCedenteARLCompensacao', TermoCPFARLCR.container);
		var item = ddl.ddlSelecionado();

		if (item.Id == 0) {
			mensagens.push(TermoCPFARLCR.settings.Mensagens.CedenteARLCompensacaoObrigatoria);
		}

		$('.dgReservasLegal tbody tr:not(.trTemplateRow) .hdnItemJSon', TermoCPFARLCR.container).each(function () {
			var itemAdd = JSON.parse($(this).val());
			if (item.Id == itemAdd.Id) {
				mensagens.push(TermoCPFARLCR.settings.Mensagens.CedenteARLCompensacaoDuplicada);
				return;
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(Titulo.container, mensagens);
			return;
		}

		if ($('.dgReservasLegal tbody tr:not(.trTemplateRow)', TermoCPFARLCR.container).length > 0) { 
			var response = MasterPage.validarAjax(
				TermoCPFARLCR.settings.urls.validarAdicionarARL,
				{ reservaLegal: item.Id, empreendimento: $('.ddlReceptorEmpreendimentos', TermoCPFARLCR.container).val() },
				Titulo.container
			);

			if (response.EhValido) {
				return;
			}
		}

		AtualizarDataGrid($('.dgReservasLegal', TermoCPFARLCR.container), item);
		ddl.ddlFirst();

		TermoCPFARLCR.carregarEmpreendimentoReceptor(item);
	},

	carregarEmpreendimentoReceptor: function (item) {
		if ($('.dgReservasLegal tbody tr:not(.trTemplateRow)', TermoCPFARLCR.container).length == 0) {
			$('.ddlReceptorEmpreendimentos', TermoCPFARLCR.container).ddlClear();
			$('.ddlReceptorDominio', TermoCPFARLCR.container).ddlClear();
			$('.ddlReceptorResposaveisEmp', TermoCPFARLCR.container).ddlClear();
			$('.dgReceptorResponsaveis tbody tr:not(.trTemplateRow)', TermoCPFARLCR.container).remove();
			return;
		}

		if (item) {

			$.ajax({
				url: TermoCPFARLCR.settings.urls.obterDadosEmpreendimentoReceptor,
				data: '{ reservaLegal: ' + item.Id + '}',
				cache: false,
				async: true,
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				error: Aux.error,
				success: function (response, textStatus, XMLHttpRequest) {
					$('.ddlReceptorEmpreendimentos', TermoCPFARLCR.container).ddlLoad(response.ReceptorEmpreendimento);
					$('.ddlReceptorDominio', TermoCPFARLCR.container).ddlLoad(response.ReceptorDominios);
					$('.ddlReceptorResposaveisEmp', TermoCPFARLCR.container).ddlLoad(response.ReceptorResponsaveisEmpreendimento, { disabled: false });
				}
			});
		}
	},

	addResponsavelEmpreendimento: function () {
		var mensagens = new Array();
		Mensagem.limpar(Titulo.container);

		var container = $(this).closest('fieldset');
		var ehCendente = container.find('.dgCedenteResponsaveis').length > 0;
		var ddl = container.find('.ddlResposaveisEmp');
		var item = ddl.ddlSelecionado();

		if (item.Id == 0) {
			var mensagem = Mensagem.replace(TermoCPFARLCR.settings.Mensagens.ResponsavelEmpreendimentoObrigatorio, '#TEXTO#', (ehCendente ? 'cedente' : 'receptor'));
			mensagem.Campo = (ehCendente ? 'Cedente' : 'Receptor') + mensagem.Campo;
			mensagens.push(mensagem);
		}

		$('.dgResponsaveis tbody tr:not(.trTemplateRow) .hdnItemJSon', container).each(function () {
			var itemAdd = JSON.parse($(this).val());
			if (item.Id == itemAdd.Id) {
				var mensagem = Mensagem.replace(TermoCPFARLCR.settings.Mensagens.ResponsavelEmpreendimentoDuplicado, '#TEXTO#', (ehCendente ? 'cedente' : 'receptor'));
				mensagem.Campo = (ehCendente ? 'Cedente' : 'Receptor') + mensagem.Campo;
				mensagens.push(mensagem);
				return;
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(Titulo.container, mensagens);
			return;
		}

		AtualizarDataGrid($('.dgResponsaveis', container), item);
		ddl.ddlFirst();
	},

	excluirLinha: function () {
		Mensagem.limpar(Titulo.container);
		var container = $(this).closest('.dataGridTable');
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable(container);

		if (container.hasClass('dgReservasLegal')) {
			TermoCPFARLCR.carregarEmpreendimentoReceptor();
		}
	},

	obterObjeto: function () {
		var objeto = {
			CedenteDominioID: $('.ddlCedenteDominio', TermoCPFARLCR.container).val(),
			ReceptorEmpreendimentoID: $('.ddlReceptorEmpreendimentos', TermoCPFARLCR.container).val(),
			ReceptorDominioID: $('.ddlReceptorDominio', TermoCPFARLCR.container).val(),
			NumeroAverbacao: $('.txtNumeroAverbacao', TermoCPFARLCR.container).val(),
			DataEmissao: { DataTexto: $('.txtDataEmissao', TermoCPFARLCR.container).val() },
			DominialidadeID: TermoCPFARLCR.settings.dominialidadeID,
			CedenteARLCompensacao: [],
			CedenteResponsaveisEmpreendimento: [],
			ReceptorResponsaveisEmpreendimento: []
		};

		$($('.dgReservasLegal tbody tr:not(.trTemplateRow) .hdnItemJSon', TermoCPFARLCR.container)).each(function () {
			objeto.CedenteARLCompensacao.push(JSON.parse($(this).val()));
		});

		$($('.dgCedenteResponsaveis tbody tr:not(.trTemplateRow) .hdnItemJSon', TermoCPFARLCR.container)).each(function () {
			objeto.CedenteResponsaveisEmpreendimento.push(JSON.parse($(this).val()));
		});

		$($('.dgReceptorResponsaveis tbody tr:not(.trTemplateRow) .hdnItemJSon', TermoCPFARLCR.container)).each(function () {
			objeto.ReceptorResponsaveisEmpreendimento.push(JSON.parse($(this).val()));
		});

		return objeto;
	}
};

function AtualizarDataGrid(container, item) {
	var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');

	if (!container.hasClass('dgReservasLegal')) {
		linha.find('.hdnItemJSon').val(JSON.stringify({ Id: item.Id, NomeRazao: item.Texto }));
		linha.find('.Nome').html(item.Texto).attr('title', item.Texto);
	} else {
		linha.find('.hdnItemJSon').val(JSON.stringify({ Id: item.Id, Identificacao: item.Texto }));
		var texto = item.Texto.split('-');
		var area = Mascara.getStringMask(parseFloat(texto[1].replace(',', '.')));
		linha.find('.Nome').html(texto[0]).attr('title', texto[0]);
		linha.find('.Area').html(area).attr('title', area);
	}

	$('tbody:last', container).append(linha);
	Listar.atualizarEstiloTable(container);
};

Titulo.settings.especificidadeLoadCallback = TermoCPFARLCR.load;
Titulo.settings.obterEspecificidadeObjetoFunc = TermoCPFARLCR.obterObjeto;
Titulo.addCallbackProtocolo(TermoCPFARLCR.obterDadosTermoCPFARLCR);
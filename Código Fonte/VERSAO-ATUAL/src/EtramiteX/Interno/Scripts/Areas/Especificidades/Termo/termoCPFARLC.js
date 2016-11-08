/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../mensagem.js" />
/// <reference path="../../../jquery.ddl.js" />

TermoCPFARLC = {
	container: null,
	settings: {
		urls: {
			obterDadosTermoCPFARLC: null,
			obterARL: null,
			obterDadosEmpreendimentoReceptor: null,
			validarAdicionarARL: null
		},
		dominialidadeID: null,
		Mensagens: null
	},

	load: function (especificidadeRef) {
		TermoCPFARLC.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);

		$('.ddlCedenteDominio', TermoCPFARLC.container).change(TermoCPFARLC.changeDdlCedenteDominio);
		$('.btnAddCedenteARLCompensacao', TermoCPFARLC.container).click(TermoCPFARLC.addCedenteARLCompensacao);
		$('.btnAddResponsavelEmp', TermoCPFARLC.container).click(TermoCPFARLC.addResponsavelEmpreendimento);
		TermoCPFARLC.container.delegate('.btnExcluir', 'click', TermoCPFARLC.excluirLinha);
		TituloCondicionante.load($('.condicionantesContainer', TermoCPFARLC.container));;
	},

	obterDadosTermoCPFARLC: function (especificidade) {
		var mensagens = new Array();
		$('.dataGridTable tbody tr:not(.trTemplateRow, .trCondTemplate)', TermoCPFARLC.container).remove();
		$('.ddlCedenteDominio', TermoCPFARLC.container).ddlClear();
		$('.ddlCedenteARLCompensacao', TermoCPFARLC.container).ddlClear();
		$('.ddlCedenteResposaveisEmp', TermoCPFARLC.container).ddlClear();

		$('.ddlReceptorEmpreendimentos', TermoCPFARLC.container).ddlClear();
		$('.ddlReceptorDominio', TermoCPFARLC.container).ddlClear();
		$('.ddlReceptorResposaveisEmp', TermoCPFARLC.container).ddlClear();

		if (especificidade.EmpreendimentoId == 0) {
			mensagens.push(TermoCPFARLC.settings.Mensagens.CedenteEmpreendimentoObrigatorio);
			Mensagem.gerar(Titulo.container, mensagens);
			return;
		}

		$.ajax({
			url: TermoCPFARLC.settings.urls.obterDadosTermoCPFARLC,
			data: JSON.stringify(especificidade),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				TermoCPFARLC.settings.dominialidadeID = response.Dominialidade.Id;

				$('.ddlCedenteDominio', TermoCPFARLC.container).ddlLoad(response.CedenteDominios);
				$('.ddlCedenteARLCompensacao', TermoCPFARLC.container).ddlLoad(response.CedenteARLCompensacao);
				$('.ddlCedenteResposaveisEmp', TermoCPFARLC.container).ddlLoad(response.CedenteResponsaveisEmpreendimento, { disabled: false });
			}
		});
	},

	changeDdlCedenteDominio: function () {
		Mensagem.limpar(Titulo.container);
		var item = $('.ddlCedenteDominio', TermoCPFARLC.container).ddlSelecionado();
		$('.dataGridTable:not(.dgCedenteResponsaveis, .dgCondicionantes) tbody tr:not(.trTemplateRow, .trCondTemplate)', TermoCPFARLC.container).remove();

		$.ajax({
			url: TermoCPFARLC.settings.urls.obterARL,
			data: JSON.stringify({ dominio: item.Id }),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				$('.ddlCedenteARLCompensacao', TermoCPFARLC.container).ddlLoad(response.ARLCedente, { disabled: false });
			}
		});
	},

	addCedenteARLCompensacao: function () {
		var mensagens = new Array();
		Mensagem.limpar(Titulo.container);

		var ddl = $('.ddlCedenteARLCompensacao', TermoCPFARLC.container);
		var item = ddl.ddlSelecionado();

		if (item.Id == 0) {
			mensagens.push(TermoCPFARLC.settings.Mensagens.CedenteARLCompensacaoObrigatoria);
		}

		$('.dgReservasLegal tbody tr:not(.trTemplateRow) .hdnItemJSon', TermoCPFARLC.container).each(function () {
			var itemAdd = JSON.parse($(this).val());
			if (item.Id == itemAdd.Id) {
				mensagens.push(TermoCPFARLC.settings.Mensagens.CedenteARLCompensacaoDuplicada);
				return;
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(Titulo.container, mensagens);
			return;
		}

		if ($('.dgReservasLegal tbody tr:not(.trTemplateRow)', TermoCPFARLC.container).length > 0) {


			var response = MasterPage.validarAjax(
					TermoCPFARLC.settings.urls.validarAdicionarARL,
					{ reservaLegal: item.Id, empreendimento: $('.ddlReceptorEmpreendimentos', TermoCPFARLC.container).val() },
					Titulo.container
				);

			if (response.EhValido) {
				return;
			}
		}

		AtualizarDataGrid($('.dgReservasLegal', TermoCPFARLC.container), item);
		ddl.ddlFirst();

		TermoCPFARLC.carregarEmpreendimentoReceptor(item);
	},

	carregarEmpreendimentoReceptor: function (item) {
		if ($('.dgReservasLegal tbody tr:not(.trTemplateRow)', TermoCPFARLC.container).length == 0) {
			$('.ddlReceptorEmpreendimentos', TermoCPFARLC.container).ddlClear();
			$('.ddlReceptorDominio', TermoCPFARLC.container).ddlClear();
			$('.ddlReceptorResposaveisEmp', TermoCPFARLC.container).ddlClear();
			$('.dgReceptorResponsaveis tbody tr:not(.trTemplateRow)', TermoCPFARLC.container).remove();
			return;
		}

		if (item) {

			$.ajax({
				url: TermoCPFARLC.settings.urls.obterDadosEmpreendimentoReceptor,
				data: '{ reservaLegal: ' + item.Id + '}',
				cache: false,
				async: true,
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				error: Aux.error,
				success: function (response, textStatus, XMLHttpRequest) {
					$('.ddlReceptorEmpreendimentos', TermoCPFARLC.container).ddlLoad(response.ReceptorEmpreendimento);
					$('.ddlReceptorDominio', TermoCPFARLC.container).ddlLoad(response.ReceptorDominios);
					$('.ddlReceptorResposaveisEmp', TermoCPFARLC.container).ddlLoad(response.ReceptorResponsaveisEmpreendimento, { disabled: false });
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
			var mensagem = Mensagem.replace(TermoCPFARLC.settings.Mensagens.ResponsavelEmpreendimentoObrigatorio, '#TEXTO#', (ehCendente ? 'cedente' : 'receptor'));
			mensagem.Campo = (ehCendente ? 'Cedente' : 'Receptor') + mensagem.Campo;
			mensagens.push(mensagem);
		}

		$('.dgResponsaveis tbody tr:not(.trTemplateRow) .hdnItemJSon', container).each(function () {
			var itemAdd = JSON.parse($(this).val());
			if (item.Id == itemAdd.Id) {
				var mensagem = Mensagem.replace(TermoCPFARLC.settings.Mensagens.ResponsavelEmpreendimentoDuplicado, '#TEXTO#', (ehCendente ? 'cedente' : 'receptor'));
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
			TermoCPFARLC.carregarEmpreendimentoReceptor();
		}
	},

	obterObjeto: function () {
		var objeto = {
			CedenteDominioID: $('.ddlCedenteDominio', TermoCPFARLC.container).val(),
			ReceptorEmpreendimentoID: $('.ddlReceptorEmpreendimentos', TermoCPFARLC.container).val(),
			ReceptorDominioID: $('.ddlReceptorDominio', TermoCPFARLC.container).val(),
			NumeroAverbacao: $('.txtNumeroAverbacao', TermoCPFARLC.container).val(),
			DataEmissao: { DataTexto: $('.txtDataEmissao', TermoCPFARLC.container).val() },
			DominialidadeID: TermoCPFARLC.settings.dominialidadeID,
			CedenteARLCompensacao: [],
			CedenteResponsaveisEmpreendimento: [],
			ReceptorResponsaveisEmpreendimento: []
		};

		$($('.dgReservasLegal tbody tr:not(.trTemplateRow) .hdnItemJSon', TermoCPFARLC.container)).each(function () {
			objeto.CedenteARLCompensacao.push(JSON.parse($(this).val()));
		});

		$($('.dgCedenteResponsaveis tbody tr:not(.trTemplateRow) .hdnItemJSon', TermoCPFARLC.container)).each(function () {
			objeto.CedenteResponsaveisEmpreendimento.push(JSON.parse($(this).val()));
		});

		$($('.dgReceptorResponsaveis tbody tr:not(.trTemplateRow) .hdnItemJSon', TermoCPFARLC.container)).each(function () {
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

Titulo.settings.especificidadeLoadCallback = TermoCPFARLC.load;
Titulo.settings.obterEspecificidadeObjetoFunc = TermoCPFARLC.obterObjeto;
Titulo.addCallbackProtocolo(TermoCPFARLC.obterDadosTermoCPFARLC);
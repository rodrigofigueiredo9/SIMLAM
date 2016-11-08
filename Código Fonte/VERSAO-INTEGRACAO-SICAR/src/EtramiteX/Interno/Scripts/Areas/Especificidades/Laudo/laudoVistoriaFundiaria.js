/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />
/// <reference path="../../../mensagem.js" />

LaudoVistoriaFundiaria = {
	container: null,
	urlObterDadosLaudoVistoriaFundiaria: '',
	mensagens: null,

	load: function (especificidadeRef) {
		LaudoVistoriaFundiaria.container = especificidadeRef;
		AtividadeEspecificidade.load(LaudoVistoriaFundiaria.container);
		LaudoVistoriaFundiaria.container.delegate('.btnAddRegularizacaoDominio', 'click', LaudoVistoriaFundiaria.adicionarRegularizacaoDominio);
		LaudoVistoriaFundiaria.container.delegate('.btnExcluirRegularizacaoDominio', 'click', LaudoVistoriaFundiaria.excluirRegularizacaoDominio);
	},

	adicionarRegularizacaoDominio: function () {
		var container = LaudoVistoriaFundiaria.container;
		var mensagens = [];

		Mensagem.limpar(container);

		var regularizacaoDominio = {
			Id: 0,
			DominioId: $('.ddlRegularizacaoDominio :selected').val(),
			ComprovacaoAreaCroqui: $('.ddlRegularizacaoDominio :selected', container).text()
		};

		if (regularizacaoDominio.DominioId <= 0) {
			mensagens.push(LaudoVistoriaFundiaria.mensagens.RegularizacaoComprovacaoObrigatoria);
		} else {
			$('.dgRegularizacaoDominio tr:not(.trTemplateRow) .hdnItemJSON', container).each(function () {
				var json = $(this).val();
				var item = JSON.parse(json);

				if (item.DominioId == regularizacaoDominio.DominioId) {
					mensagens.push(LaudoVistoriaFundiaria.mensagens.RegularizacaoDominioJaAdicionado);
					return false;
				}
			});
		}

		if (mensagens.length > 0)
		{
			Mensagem.gerar(container, mensagens);
			return;
		}

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');

		linha.find('.hdnItemJSON').val(JSON.stringify(regularizacaoDominio));
		linha.find('.hdnRegularizacaoDominioId').val(regularizacaoDominio.Id);
		linha.find('.hdnDominioId').val(regularizacaoDominio.DominioId);
		linha.find('.comprovacao').html(regularizacaoDominio.ComprovacaoAreaCroqui).attr('title', regularizacaoDominio.ComprovacaoAreaCroqui);

		$('.dataGridTable.dgRegularizacaoDominio tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.ddlRegularizacaoDominio', container).val(0);
	},

	excluirRegularizacaoDominio: function () {
		$(this).closest('tr').remove();
	},

	obterDadosLaudoVistoriaFundiaria: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', LaudoVistoriaFundiaria.container).ddlClear();
			$('.ddlRegularizacaoDominio', LaudoVistoriaFundiaria.container).ddlClear();
			return;
		}

		$.ajax({
			url: LaudoVistoriaFundiaria.urlObterDadosLaudoVistoriaFundiaria,
			data: JSON.stringify({ ProtocoloId: protocolo.Id, EmpreendimentoId: protocolo.EmpreendimentoId,  }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LaudoVistoriaFundiaria.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios.length > 0) {
					$('.ddlDestinatarios', LaudoVistoriaFundiaria.container).ddlLoad(response.Destinatarios);
				}

				if (response.Posses.length > 0) {
					$('.ddlRegularizacaoDominio', LaudoVistoriaFundiaria.container).ddlLoad(response.Posses);
				}

				$('.hdnRegularizacaoFundiariaId', LaudoVistoriaFundiaria.container).val(response.RegularizacaoFundiariaId);
				$('.hdnRegularizacaoFundiariaTid', LaudoVistoriaFundiaria.container).val(response.RegularizacaoFundiariaTid);

			}
		});
	},

	obter: function () {
		var obj = {
			Destinatario: LaudoVistoriaFundiaria.container.find('.ddlDestinatarios').val(),
			DataVistoria: { DataTexto: LaudoVistoriaFundiaria.container.find('.txtDataVistoria').val() },
			RegularizacaoId: $('.hdnRegularizacaoFundiariaId', LaudoVistoriaFundiaria.container).val(),
			RegularizacaoDominios: []
		};

		$('.dgRegularizacaoDominio tr:not(.trTemplateRow) .hdnItemJSON').each(function () {
			var json = $(this).val();
			obj.RegularizacaoDominios.push(JSON.parse(json));
		});

		return obj;

	}
};

Titulo.settings.especificidadeLoadCallback = LaudoVistoriaFundiaria.load;
Titulo.addCallbackProtocolo(LaudoVistoriaFundiaria.obterDadosLaudoVistoriaFundiaria);
Titulo.settings.obterEspecificidadeObjetoFunc = LaudoVistoriaFundiaria.obter;
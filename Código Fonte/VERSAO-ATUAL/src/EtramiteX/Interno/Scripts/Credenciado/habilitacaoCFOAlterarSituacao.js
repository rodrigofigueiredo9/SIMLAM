/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />

HabilitacaoCFOAlterarSituacao = {
	settings: {
		urls: {
			alterarSituacao: null
		},
		situacaoMotivo: null,
		situacaoMotivoAtivo: null,
		motivoSuspenso: null,
        motivoDescredenciado: null
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(HabilitacaoCFOAlterarSituacao.settings, options);
		}
		HabilitacaoCFOAlterarSituacao.container = MasterPage.getContent(container);
		HabilitacaoCFOAlterarSituacao.container.delegate('.btnSalvar', 'click', HabilitacaoCFOAlterarSituacao.alterarSituacao);
		$(".ddlSituacao", HabilitacaoCFOAlterarSituacao.container).change(HabilitacaoCFOAlterarSituacao.situacaoChange);
		$(".ddlMotivo", HabilitacaoCFOAlterarSituacao.container).change(HabilitacaoCFOAlterarSituacao.motivoChange);
		$(".txtSituacaoData", HabilitacaoCFOAlterarSituacao.container).change(HabilitacaoCFOAlterarSituacao.dataInicialChange);

		HabilitacaoCFOAlterarSituacao.container = container;
	},

    //retorna a data no formato dd/MM/aaaa
	dataFormatoBR: function(data){
	    return data.getDate() + '/' + (data.getMonth() + 1) + '/' + data.getFullYear();
	},

	pegaDataTela: function(){
	    var dataString = $(".txtSituacaoData", HabilitacaoCFOAlterarSituacao.container).val();
	    var dia = (dataString.substring(0, 2));
	    var mes = dataString.substring(3, 5);
	    var ano = dataString.substring(6, 10);
	    var data = new Date(ano, mes - 1, dia); //mes-1, pq jan=0, fev=1, mar=2...

	    return data;
	},

	situacaoChange: function () {
        //SITUAÇÃO: INATIVO
	    if ($(".ddlSituacao", HabilitacaoCFOAlterarSituacao.container).val() == HabilitacaoCFOAlterarSituacao.settings.situacaoMotivo) {

	        //Motivo
	        $(".divMotivo", HabilitacaoCFOAlterarSituacao.container).removeClass("hide");
	        $('.ddlMotivo :selected', HabilitacaoCFOAlterarSituacao.container).ddlFirst();
	        $('.labelNovoMotivo', HabilitacaoCFOAlterarSituacao.container).text("Novo Motivo *");
	        //Remove o motivo Advertência
	        $(".ddlMotivo option[value='1']", HabilitacaoCFOAlterarSituacao.container).show();
	        $(".ddlMotivo option[value='2']", HabilitacaoCFOAlterarSituacao.container).show();
	        $(".ddlMotivo option[value='3']", HabilitacaoCFOAlterarSituacao.container).show();
	        $(".ddlMotivo option[value='4']", HabilitacaoCFOAlterarSituacao.container).show();
	        $(".ddlMotivo option[value='5']", HabilitacaoCFOAlterarSituacao.container).hide();

            //Número do DUA
	        $(".divNumeroDua", HabilitacaoCFOAlterarSituacao.container).addClass("hide");

	        //Data do pagamento
	        $(".divDataPagamento", HabilitacaoCFOAlterarSituacao.container).addClass("hide");
	    }
        //SITUAÇÃO: ATIVO
	    else if ($(".ddlSituacao", HabilitacaoCFOAlterarSituacao.container).val() == HabilitacaoCFOAlterarSituacao.settings.situacaoMotivoAtivo) {

	        //Motivo
		    $(".divMotivo", HabilitacaoCFOAlterarSituacao.container).removeClass("hide");
		    $('.ddlMotivo :selected', HabilitacaoCFOAlterarSituacao.container).ddlFirst();
		    $('.labelNovoMotivo', HabilitacaoCFOAlterarSituacao.container).text("Novo Motivo");
            //Remove todos os motivos que não sejam Advertência
		    $(".ddlMotivo option[value='1']", HabilitacaoCFOAlterarSituacao.container).hide();
		    $(".ddlMotivo option[value='2']", HabilitacaoCFOAlterarSituacao.container).hide();
		    $(".ddlMotivo option[value='3']", HabilitacaoCFOAlterarSituacao.container).hide();
		    $(".ddlMotivo option[value='4']", HabilitacaoCFOAlterarSituacao.container).hide();
		    $(".ddlMotivo option[value='5']", HabilitacaoCFOAlterarSituacao.container).show();

	        //Número do DUA
		    $(".divNumeroDua", HabilitacaoCFOAlterarSituacao.container).removeClass("hide");

	        //Data do Pagamento
		    $(".divDataPagamento", HabilitacaoCFOAlterarSituacao.container).removeClass("hide");
		}
        //OUTRAS (SELECIONE)
	    else {
            //Motivo
		    $(".divMotivo", HabilitacaoCFOAlterarSituacao.container).addClass("hide");
		    $('.ddlMotivo :selected', HabilitacaoCFOAlterarSituacao.container).ddlFirst();

	        //Número do DUA
		    $(".divNumeroDua", HabilitacaoCFOAlterarSituacao.container).addClass("hide");

	        //Data do pagamento
		    $(".divDataPagamento", HabilitacaoCFOAlterarSituacao.container).addClass("hide");
		}
	},

	motivoChange: function () {
	    //MOTIVO: SUSPENSAO
	    if ($(".ddlMotivo", HabilitacaoCFOAlterarSituacao.container).val() == HabilitacaoCFOAlterarSituacao.settings.motivoSuspenso) {

	        //Data da nova situação
	        $(".divDataNovaSituacao", HabilitacaoCFOAlterarSituacao.container).removeClass("hide");

	        //Data final da situação
	        $(".divDataFinalSituacao", HabilitacaoCFOAlterarSituacao.container).removeClass("hide");
	        var dataFinal = HabilitacaoCFOAlterarSituacao.pegaDataTela();
	        dataFinal.setMonth(dataFinal.getMonth() + 3);
	        $(".txtFinalSituacaoData", HabilitacaoCFOAlterarSituacao.container).val(HabilitacaoCFOAlterarSituacao.dataFormatoBR(dataFinal));

	        //Número do Processo
	        $(".divNumeroProcesso", HabilitacaoCFOAlterarSituacao.container).removeClass("hide");
	    }
	    //MOTIVO: DESCREDENCIAMENTO
	    else if ($(".ddlMotivo", HabilitacaoCFOAlterarSituacao.container).val() == HabilitacaoCFOAlterarSituacao.settings.motivoDescredenciado) {

	        //Data da nova situação
	        $(".divDataNovaSituacao", HabilitacaoCFOAlterarSituacao.container).removeClass("hide");

	        //Data final da situação
	        $(".divDataFinalSituacao", HabilitacaoCFOAlterarSituacao.container).removeClass("hide");
	        var dataFinal = HabilitacaoCFOAlterarSituacao.pegaDataTela();
	        dataFinal.setMonth(dataFinal.getMonth() + 18);
	        $(".txtFinalSituacaoData", HabilitacaoCFOAlterarSituacao.container).val(HabilitacaoCFOAlterarSituacao.dataFormatoBR(dataFinal));

	        //Número do Processo
	        $(".divNumeroProcesso", HabilitacaoCFOAlterarSituacao.container).removeClass("hide");
	    }
	    else {

	        //Data da nova situação
	        $(".divDataNovaSituacao", HabilitacaoCFOAlterarSituacao.container).addClass("hide");

	        //Data final da situação
	        $(".divDataFinalSituacao", HabilitacaoCFOAlterarSituacao.container).addClass("hide");

	        //Número do Processo
	        $(".divNumeroProcesso", HabilitacaoCFOAlterarSituacao.container).addClass("hide");
	    }
	},

	dataInicialChange: function () {
	    //MOTIVO: SUSPENSAO
	    if ($(".ddlMotivo", HabilitacaoCFOAlterarSituacao.container).val() == HabilitacaoCFOAlterarSituacao.settings.motivoSuspenso) {

	        //Data final da situação
	        var dataFinal = HabilitacaoCFOAlterarSituacao.pegaDataTela();
	        dataFinal.setMonth(dataFinal.getMonth() + 3);
	        $(".txtFinalSituacaoData", HabilitacaoCFOAlterarSituacao.container).val(HabilitacaoCFOAlterarSituacao.dataFormatoBR(dataFinal));

	    }
	    //MOTIVO: DESCREDENCIAMENTO
	    else if ($(".ddlMotivo", HabilitacaoCFOAlterarSituacao.container).val() == HabilitacaoCFOAlterarSituacao.settings.motivoDescredenciado) {

	        //Data final da situação
	        var dataFinal = HabilitacaoCFOAlterarSituacao.pegaDataTela();
	        dataFinal.setMonth(dataFinal.getMonth() + 18);
	        $(".txtFinalSituacaoData", HabilitacaoCFOAlterarSituacao.container).val(HabilitacaoCFOAlterarSituacao.dataFormatoBR(dataFinal));
	    }
	},

	obter: function () {
		var container = HabilitacaoCFOAlterarSituacao.container;
		var obj = {
			Id: $('.hdnHabilitacaoId', container).val(),
			Situacao: $('.ddlSituacao :selected', container).val(),
			SituacaoData: $('.txtSituacaoData', container).val(),
			Motivo: $('.ddlMotivo :selected', container).val(),
			Observacao: $('.txtObservacao', container).val(),
			NumeroDua: $('.txtNumeroDua', container).val(),
			DataPagamentoDUA: $('.txtDataPagamentoDua', container).val(),
			NumeroProcesso: $('.txtNumeroProcesso', container).val()
		}

		return obj;
	},

	alterarSituacao: function () {
		Mensagem.limpar(HabilitacaoCFOAlterarSituacao.container);

		MasterPage.carregando(true);
		$.ajax({
			url: HabilitacaoCFOAlterarSituacao.settings.urls.alterarSituacao,
			data: JSON.stringify(HabilitacaoCFOAlterarSituacao.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(HabilitacaoCFOAlterarSituacao.container, response.Msg);
				}

				if (response.EhValido) {
					HabilitarEmissaoCFOCFOCListar.container.listarAjax('ultimaBusca');
					Mensagem.gerar(HabilitarEmissaoCFOCFOCListar.container, response.Msg);
					Modal.fechar(HabilitacaoCFOAlterarSituacao.container);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
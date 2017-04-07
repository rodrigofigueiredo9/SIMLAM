/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

LocalVistoria = {
	settings: {
	    urls: {
	        salvar: null,
	        obter: null,
	        escolherSetor: null,
	        podeExcluir: null,
            podeIncluirBloqueio: null,
	    },
        edicao: false,
	    Mensagens: null
	},
	container: null,

	load: function (container, options) {

	    if (options) {
	        $.extend(LocalVistoria.settings, options);
	    }

	    LocalVistoria.container = MasterPage.getContent(container);
	    LocalVistoria.container = container;
	    LocalVistoria.container.delegate('.txtHoraInicio', 'keyup', LocalVistoria.onKeyEnterHoraInicioFim);
	    LocalVistoria.container.delegate('.txtHoraFim', 'keyup', LocalVistoria.onKeyEnterHoraInicioFim);
	    LocalVistoria.container.delegate('.btnSalvar', 'click', LocalVistoria.salvar);
	    LocalVistoria.container.delegate('.btnAdicionar', 'click', LocalVistoria.onAdicionarLocal);
	    LocalVistoria.container.delegate('.btnEditar', 'click', LocalVistoria.editar);
	    LocalVistoria.container.delegate('.btnItemEditar', 'click', LocalVistoria.onItemEditar);
	    LocalVistoria.container.delegate('.ddlSetores', 'change', LocalVistoria.onSelecionarSetor);
	    LocalVistoria.container.delegate('.btnBloquear', 'click', LocalVistoria.bloquear);
	    LocalVistoria.container.delegate('.btnExcluir', 'click', LocalVistoria.excluir);
	    LocalVistoria.container.delegate('.btnAdicionarBloqueio', 'click', LocalVistoria.onAdicionarBloqueio);
	    LocalVistoria.container.delegate('.btnExcluirBloqueio', 'click', LocalVistoria.excluirBloqueio);
	},


	ValidarHora: function (hora) {

	    if (hora.length != 5) {
	        return false;
	    }

	    hrs = hora.substring(0,2);
	    min = hora.substring(3,5); 

	    situacao = "";    
	    if ((hrs == "") || (min == ""))
	    {
	        return false;
	    }
	    if ((hrs < 00 ) || (hrs > 23) || ( min < 00) ||( min > 59))
	    { 
	        return false; 
	    } 
	    return true;
	},

	HoraInicioMenorHoraFim: function (horaInicio, HoraFim) {

	    hrsIni = horaInicio.substring(0, 2);
	    minIni = horaInicio.substring(3, 5);

	    hrsFim = HoraFim.substring(0, 2);
	    minFim = HoraFim.substring(3, 5);

	    if (!((hrsIni < hrsFim) || ((hrsIni == hrsFim) && (minIni < minFim)))) {
	        return false;
	    }
	    return true;

	},

	onAdicionarBloqueio: function () {


	    var podeIncuir = 0;
	    var item = {
	        datInicial: $("#DataInicialBloqueio").val() + ' ' + $("#HoraInicialBloqueio").val(),
	        datFinal: $("#DataFinalBloqueio").val() + ' ' + $("#HoraFinalBloqueio").val(),
	        setorId: $("#SetorTipo option:selected").val(),
	    };

	    MasterPage.carregando(true);
	    $.ajax({
	        url: LocalVistoria.settings.urls.podeIncluirBloqueio,
	        data: JSON.stringify(item),
	        cache: false,
	        async: false,
	        type: 'POST',
	        dataType: 'json',
	        contentType: 'application/json; charset=utf-8',
	        error: function (XMLHttpRequest, textStatus, erroThrown) {
	            Aux.error(XMLHttpRequest, textStatus, erroThrown, LocalVistoria.container);
	        },
	        success: function (response, textStatus, XMLHttpRequest) {
	            if (response.Msg && response.Msg.length > 0) {
	                Mensagem.gerar(LocalVistoria.container, response.Msg);
	                podeIncuir = 0;
	                return
	            }
	            else {
	                podeIncuir = 1;
	            }
	        }
	    });

	    MasterPage.carregando(false);

	    if ($('.txtHoraInicialBloqueio', LocalVistoria.container).val() == '') {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraFimObrigatorio]);
	        $('.txtHoraInicialBloqueio', LocalVistoria.container).focus();
	        return;
	    }

	    if ($('.txtHoraFinalBloqueio', LocalVistoria.container).val() == '') {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraFimObrigatorio]);
	        $('.txtHoraInicialBloqueio', LocalVistoria.container).focus();
	        return;
	    }

	    if (!LocalVistoria.ValidarHora($('.txtHoraInicialBloqueio', LocalVistoria.container).val())) {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraInicioInvalida]);
	        $('.txtHoraInicialBloqueio', LocalVistoria.container).focus();
	        return;
	    }
	    if (!LocalVistoria.ValidarHora($('.txtHoraFinalBloqueio', LocalVistoria.container).val())) {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraFimInvalida]);
	        $('.txtHoraFinalBloqueio', LocalVistoria.container).focus();
	        return;
	    }

	    var linha = $('.tr_template_bloqueio', LocalVistoria.container).clone();
	    var item = {
	        Id: null,
	        DataInicialBloqueio: $('.txtDataInicialBloqueio', LocalVistoria.container).val(),
	        HoraInicialBloqueio: $('.txtHoraInicialBloqueio', LocalVistoria.container).val(),
	        DataFinalBloqueio: $('.txtDataFinalBloqueio', LocalVistoria.container).val(),
	        HoraFinalBloqueio: $('.txtHoraFinalBloqueio', LocalVistoria.container).val(),
	        Tid: null,
	    };

	    LocalVistoria.alterandoItemListaBloqueio(linha, item, 0);

	    

	    $(linha).removeClass('hide');
	    $(linha).removeClass('tr_template_bloqueio');


	    $('.gridBloqueios tbody', LocalVistoria.container).append($(linha));

	    $('.txtDataInicialBloqueio', LocalVistoria.container).val('');
	    $('.txtHoraInicialBloqueio', LocalVistoria.container).val('');
	    $('.txtDataFinalBloqueio', LocalVistoria.container).val('');
	    $('.txtHoraFinalBloqueio', LocalVistoria.container).val('');
	  

	    Listar.atualizarEstiloTable($('.gridBloqueios', LocalVistoria.container));
	    Aux.setarFoco(LocalVistoria.container);


	},

	onAdicionarLocal: function () {
	    Mensagem.limpar(LocalVistoria.container);
	    var validacao = true;

	    if ($('.ddlSetores option:selected', LocalVistoria.container).val() == '0') {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.SetorObrigatorio]);
	        $('.ddlSetores', LocalVistoria.container).focus();
	        return;
	    }
	    if ($('.ddldiasemana option:selected', LocalVistoria.container).val() == '0') {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.DiaSemanaObrigatorio]);
	        $('.ddldiasemana', LocalVistoria.container).focus();
	        return;
	    }
	    if ($('.txtHoraInicio', LocalVistoria.container).val() == '') {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraInicioObrigatorio]);
	        $('.txtHoraInicio', LocalVistoria.container).focus();
	        return;
	    }
	    if ($('.txtHoraFim', LocalVistoria.container).val() == '') {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraFimObrigatorio]);
	        $('.txtHoraFim', LocalVistoria.container).focus();
	        return;
	    }
	    if (!LocalVistoria.ValidarHora($('.txtHoraInicio', LocalVistoria.container).val())){
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraInicioInvalida]);
	        $('.txtHoraInicio', LocalVistoria.container).focus();
	        return;
	    }
	    if (!LocalVistoria.ValidarHora($('.txtHoraFim', LocalVistoria.container).val())){
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraFimInvalida]);
	        $('.txtHoraFim', LocalVistoria.container).focus();
	        return;
	    }
	    if (!(LocalVistoria.HoraInicioMenorHoraFim($('.txtHoraInicio', LocalVistoria.container).val(), $('.txtHoraFim', LocalVistoria.container).val())) ) {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraInicialMenorHoraFinal]);
	        $('.txtHoraInicio', LocalVistoria.container).focus();
	        return;
	    }

	    var linha = $('.tr_template', LocalVistoria.container).clone();
	    var item = {
	        Id: null,
	        DiaSemanaId: $('.ddldiasemana option:selected', LocalVistoria.container).val(),
	        DiaSemanaTexto: $('.ddldiasemana option:selected', LocalVistoria.container).text(),
	        HoraInicio: $('.txtHoraInicio', LocalVistoria.container).val(),
	        HoraFim: $('.txtHoraFim', LocalVistoria.container).val(),
	        Situacao:1,
	        Tid: null,
	    };

	    LocalVistoria.alterandoItemLista(linha, item, 0);
	    $(linha).removeClass('hide');
	    $(linha).removeClass('tr_template');

	    $('.gridLocalVistoria tbody', LocalVistoria.container).append($(linha));

	    $('.txtHoraInicio', LocalVistoria.container).val('');
	    $('.txtHoraFim', LocalVistoria.container).val('');
	    $('.ddldiasemana', LocalVistoria.container).val('0');
	    $('.ddlSetores', LocalVistoria.container).attr('disabled', 'disabled');
	    $('.ddlSetores', LocalVistoria.container).addClass('disabled');

	    Listar.atualizarEstiloTable($('.gridLocalVistoria', LocalVistoria.container));
	    Aux.setarFoco(LocalVistoria.container);
	},

	onKeyEnterHoraInicioFim: function (e) {
	    var keyENTER = 13;
	    if (e.keyCode == keyENTER) {
	        if (LocalVistoria.settings.edicao == false)
	        {
	            $('.btnAdicionar').click();
	        }
	        else
	        {
	            $('.btnEditar').click();
	        }
	        $('.ddldiasemana', LocalVistoria.container).focus();
	    }
	    return false;
	},

	onItemEditar: function () {
	    Mensagem.limpar(LocalVistoria.container);
	    var item = {
	        Index: $(this).closest('tr').find('.hdnItemIndex').val(),
	        Id: $(this).closest('tr').find('.hdnItemId').val(),
	        DiaSemanaId: $(this).closest('tr').find('.hdnDiaSemanaId').val(),
	        HoraInicio: $(this).closest('tr').find('.lblHoraInicio').html(),
	        HoraFim: $(this).closest('tr').find('.lblHoraFim').html(),
	        Tid: $(this).closest('tr').find('.hdnItemTid').val(),
	    };

	    LocalVistoria.settings.edicao = true;

	    $(this).closest('tr').addClass('itemEdicao');
	    $('.ddldiasemana', LocalVistoria.container).val(item.DiaSemanaId);
	    $('.txtHoraInicio', LocalVistoria.container).val(item.HoraInicio);
	    $('.txtHoraFim', LocalVistoria.container).val(item.HoraFim);
	    $('.hdnLocalVistoriaId', LocalVistoria.container).val(item.Id);
	    $('.hdnLocalVistoriaTid', LocalVistoria.container).val(item.Tid);
	    $('.btnAdicionar', LocalVistoria.container).addClass('hide');
	    $('.btnEditar', LocalVistoria.container).removeClass('hide');
	    $('.ddldiasemana', LocalVistoria.container).focus();
	},
    
	editar: function () {

	    var validacao = true;

	    if ($('.ddldiasemana option:selected', LocalVistoria.container).val() == '0') {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.DiaSemanaObrigatorio]);
	        $('.ddldiasemana', LocalVistoria.container).focus();
	        return;
	    }
	    if ($('.txtHoraInicio', LocalVistoria.container).val() == '') {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraInicioObrigatorio]);
	        $('.txtHoraInicio', LocalVistoria.container).focus();
	        return;
	    }
	    if ($('.txtHoraFim', LocalVistoria.container).val() == '') {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraFimObrigatorio]);
	        $('.txtHoraFim', LocalVistoria.container).focus();
	        return;
	    }
	    if (!LocalVistoria.ValidarHora($('.txtHoraInicio', LocalVistoria.container).val())) {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraInicioInvalida]);
	        $('.txtHoraInicio', LocalVistoria.container).focus();
	        return;
	    }
	    if (!LocalVistoria.ValidarHora($('.txtHoraFim', LocalVistoria.container).val())) {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraFimInvalida]);
	        $('.txtHoraFim', LocalVistoria.container).focus();
	        return;
	    }
	    if (!(LocalVistoria.HoraInicioMenorHoraFim($('.txtHoraInicio', LocalVistoria.container).val(), $('.txtHoraFim', LocalVistoria.container).val())) ) {
	        Mensagem.gerar(MasterPage.getContent(LocalVistoria.container), [LocalVistoria.settings.Mensagens.HoraInicialMenorHoraFinal]);
	        $('.txtHoraInicio', LocalVistoria.container).focus();
	        return;
	    }


	    var linha = $('.itemEdicao', LocalVistoria.container);

	    var item = {
	        Id: $('.hdnLocalVistoriaId', LocalVistoria.container).val(),
	        DiaSemanaId: $('.ddldiasemana option:selected', LocalVistoria.container).val(),
	        DiaSemanaTexto: $('.ddldiasemana option:selected', LocalVistoria.container).text(),
	        HoraInicio: $('.txtHoraInicio', LocalVistoria.container).val(),
	        HoraFim: $('.txtHoraFim', LocalVistoria.container).val(),
	        Tid: "", //Se editou o TID fica vazio
	    };

	    LocalVistoria.alterandoItemLista(linha, item, 0);

	    //limpa componente de cadastro
	    $('.txtHoraInicio', LocalVistoria.container).val('');
	    $('.txtHoraFim', LocalVistoria.container).val('');
	    $('.ddldiasemana', LocalVistoria.container).val('0');
	    $('.hdnLocalVistoriaId', LocalVistoria.container).val('');
	    $('.hdnLocalVistoriaTid', LocalVistoria.container).val('');
	 
        $(linha).removeClass('itemEdicao');

	    $('.btnAdicionar', LocalVistoria.container).removeClass('hide');
	    $('.btnEditar', LocalVistoria.container).addClass('hide');

	    LocalVistoria.settings.edicao = false;
	},

	
	
	alterandoItemListaBloqueio: function (linha, item, itemIndex) {
	   
	    $('.lblDataInicialBloqueio', linha).html(item.DataInicialBloqueio);
	    $('.lblHoraInicialBloqueio', linha).html(item.HoraInicialBloqueio);
	    $('.lblDataFinalBloqueio', linha).html(item.DataFinalBloqueio);
	    $('.lblHoraFinalBloqueio', linha).html(item.HoraFinalBloqueio);
	    $('.hdnItemBloqueioId', linha).val(item.Id);
	    $('.hdnItemBloqueioTid', linha).val(item.Tid);
	   
	   
	    if ((itemIndex == 0) || (itemIndex == "0")) {
	        $('.hdnItemBloqueioIndex', linha).val($('.gridBloqueios tbody tr:not(.tr_template_bloqueio)').length);
	    }
	    else {
	        $('.hdnItemBloqueioIndex', linha).val(itemIndex);
	    }
	},


	alterandoItemLista: function(linha, item, itemIndex){
	    $('.lblDiaSemana', linha).html(item.DiaSemanaTexto);
	    $('.hdnDiaSemanaId', linha).val(item.DiaSemanaId);
	    $('.lblHoraInicio', linha).html(item.HoraInicio);
	    $('.lblHoraFim', linha).html(item.HoraFim);
	    $('.hdnItemId', linha).val(item.Id);
	    $('.hdnItemTid', linha).val(item.Tid);
	  

	    if ((itemIndex == 0)||(itemIndex == "0")) {
	        $('.hdnItemIndex', linha).val($('.gridLocalVistoria tbody tr:not(.tr_template)').length);
	    }
	    else {
	        $('.hdnItemIndex', linha).val(itemIndex);
	    }
	},
    

	atualizarLista: function (LocalVistoriaObj) {

	    $(LocalVistoriaObj.DiasHorasVistoria).each(function (index, item) {
	        var linha = $('.tr_template', LocalVistoria.container).clone();
	        LocalVistoria.alterandoItemLista(linha, item, 0);
	        $(linha).removeClass('hide');
	        $(linha).removeClass('tr_template');

	        $('.gridLocalVistoria tbody', LocalVistoria.container).append($(linha));
	    });

	    $('.txtHoraInicio', LocalVistoria.container).val('');
	    $('.txtHoraFim', LocalVistoria.container).val('');
	    $('.ddldiasemana', LocalVistoria.container).val('0');
	    $('.ddlSetores', LocalVistoria.container).attr('disabled', 'disabled');
	    $('.ddlSetores', LocalVistoria.container).addClass('disabled');
        
	    Listar.atualizarEstiloTable($('.gridLocalVistoria', LocalVistoria.container));
	    Aux.setarFoco(LocalVistoria.container);

	},

	entraEditando: function(){
	    $('.ddlSetores', LocalVistoria.container).attr('disabled', 'disabled');
	    $('.ddlSetores', LocalVistoria.container).addClass('disabled');
        
	    Listar.atualizarEstiloTable($('.gridLocalVistoria', LocalVistoria.container));
	    Aux.setarFoco(LocalVistoria.container);

	},


	LimparLista: function () {
	    var gridContainer = $('.gridLocalVistoria tbody', LocalVistoria.container);

	    $('tr:not(.tr_template)', gridContainer).each(function () {
	        $(this).closest('tr').remove();
	    });

	},

	obterApenasSetor: function () {
	    var LocalVistoriaObj = {
	        SetorId: $('.ddlSetores option:selected', LocalVistoria.container).val(),
	        SetorTexto: $('.ddlSetores option:selected', LocalVistoria.container).text(),
	        DiasHorasVistoria: new Array ()
	    };
	    return LocalVistoriaObj;
	},

	bloquear: function(){
	    Mensagem.limpar(LocalVistoria.container);
	    itemSituacao =  $(this).closest('tr').find('.hdnItemSituacao').val();

	    if ((itemSituacao == 1) || (itemSituacao == "1")) {
            //Colocar 0
	        $(this).closest('tr').find('.lblSituacao').html("Bloqueado");
	        $(this).closest('tr').find('.hdnItemSituacao').val("0");
	    }
	    else {
	        //Colocar 0
	        $(this).closest('tr').find('.lblSituacao').html("Ativo");
	        $(this).closest('tr').find('.hdnItemSituacao').val("1");
	    }
	    $(this).closest('tr').find('.hdnItemTid').val("");//Se editou o TID fica vazio
	},

	excluirBloqueio: function () {

	    $(this).closest('tr').remove();
	    Listar.atualizarEstiloTable($('.gridBloqueios', LocalVistoria.container));

	},

	excluir: function () {
	    Mensagem.limpar(LocalVistoria.container);
	    var item = {
	        Id: $(this).closest('tr').find('.hdnItemId').val(),
	        DiaSemanaId: $(this).closest('tr').find('.hdnDiaSemanaId').val(),
	        DiaSemanaTexto: $(this).closest('tr').find('.lblDiaSemana').html(),
	        HoraInicio: $(this).closest('tr').find('.lblHoraInicio').html(),
	        HoraFim: $(this).closest('tr').find('.lblHoraFim').html(),
	        Tid: $(this).closest('tr').find('.hdnItemTid').val(),
	    };

	    if (item.Id != "") {
	        var podeApagar = 0;
	        MasterPage.carregando(true);
	        $.ajax({
	            url: LocalVistoria.settings.urls.podeExcluir,
	            data: JSON.stringify(item),
	            cache: false,
	            async: false,
	            type: 'POST',
	            dataType: 'json',
	            contentType: 'application/json; charset=utf-8',
	            error: function (XMLHttpRequest, textStatus, erroThrown) {
	                Aux.error(XMLHttpRequest, textStatus, erroThrown, LocalVistoria.container);
	            },
	            success: function (response, textStatus, XMLHttpRequest) {
	                if (response.Msg && response.Msg.length > 0) {
	                    Mensagem.gerar(LocalVistoria.container, response.Msg);
	                    podeApagar = 0;
	                    return
	                }
	                else {
	                    podeApagar = 1;
	                }
	            }
	        });
	        MasterPage.carregando(false);
	        if (podeApagar == 1) {
	            $(this).closest('tr').remove();
	            Listar.atualizarEstiloTable($('.gridLocalVistoria', LocalVistoria.container));
	        }
	    }
	    else {
	        $(this).closest('tr').remove();
	        Listar.atualizarEstiloTable($('.gridLocalVistoria', LocalVistoria.container));
	    }
	},


	onSelecionarSetor: function (e) {
	    if (!($('.ddlSetores option:selected', LocalVistoria.container).val() == '0')) {
	        MasterPage.carregando(true);
	        $.ajax({
	            url: LocalVistoria.settings.urls.escolherSetor,
	            data: JSON.stringify(LocalVistoria.obterApenasSetor()),
	            cache: false,
	            async: false,
	            type: 'POST',
	            dataType: 'json',
	            contentType: 'application/json; charset=utf-8',
	            error: function (XMLHttpRequest, textStatus, erroThrown) {
	                Aux.error(XMLHttpRequest, textStatus, erroThrown, LocalVistoria.container);
	            },
	            success: function (response, textStatus, XMLHttpRequest) {
	                if (response.Msg && response.Msg.length > 0) {
	                    Mensagem.gerar(LocalVistoria.container, response.Msg);
	                }
	                if (response.EhValido) {
	                    LocalVistoriaObj = response.Local;
	                    LocalVistoria.LimparLista();
	                    LocalVistoria.atualizarLista(LocalVistoriaObj);
	                }
	            }
	        });
	        MasterPage.carregando(false);
	    }
	},

	obter: function () {
	    var gridContainer = $('.gridLocalVistoria tbody', LocalVistoria.container);

	    var LocalVistoriaObj = {
	        SetorId: $('.ddlSetores option:selected', LocalVistoria.container).val(),
	        SetorTexto: $('.ddlSetores option:selected', LocalVistoria.container).text(),
	        DiasHorasVistoria: new Array(),
	        Bloqueios: new Array()
        };

	    $('tr:not(.tr_template)', gridContainer).each(function () {
	        LocalVistoriaObj.DiasHorasVistoria.push({
	            Id: $('.hdnItemId', this).val(),
	            DiaSemanaId: $('.hdnDiaSemanaId', this).val(),
	            DiaSemanaTexto: $('.lblDiaSemana', this).html(),
	            HoraInicio: $('.lblHoraInicio', this).html(),
	            HoraFim: $('.lblHoraFim', this).html(),
	            Tid: $('.hdnItemTid', this).val()
	        });
	    });

	    gridContainer = $('.gridBloqueios tbody', LocalVistoria.container);

	    $('tr:not(.tr_template_bloqueio)', gridContainer).each(function () {
	        LocalVistoriaObj.Bloqueios.push({
	            Id: $('.hdnItemId', this).val(),
	            DiaInicio: $('.lblDataInicialBloqueio', this).html(),
	            HoraInicio: $('.lblHoraInicialBloqueio', this).html(),
	            DiaFim: $('.lblDataFinalBloqueio', this).html(),
	            HoraFim: $('.lblHoraFinalBloqueio', this).html(),
	            Tid: $('.hdnItemTid', this).val()
	        });
	    });

	  

	    return LocalVistoriaObj;
	},

	salvar: function () {
	    Mensagem.limpar(LocalVistoria.container);
		MasterPage.carregando(true);

		$.ajax({
		    url: LocalVistoria.settings.urls.salvar,
		    data: JSON.stringify(LocalVistoria.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
			    Aux.error(XMLHttpRequest, textStatus, erroThrown, LocalVistoria.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
			    if (response.Msg && response.Msg.length > 0) {
			        Mensagem.gerar(LocalVistoria.container, response.Msg);
			        MasterPage.carregando(false);
			    }
			    if (response.EhValido) {
			        MasterPage.redireciona(response.Url);
			    }
			}
		});
		
	},

	onEditar: function () {
	    Mensagem.limpar(LocalVistoria.container);

		$.ajax({
		    url: LocalVistoria.settings.urls.obter,
			data: JSON.stringify({ id: $(this).closest('tr').find('.hdnItemId').val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
			    Aux.error(XMLHttpRequest, textStatus, erroThrown, LocalVistoria.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('body').animate({ scrollTop: $('body').offset().top }, 300);
					$('.hdnId', LocalVistoria.container).val(response.ConfiguracaoVegetalItem.Id);
					$('.txtValor', LocalVistoria.container).val(response.ConfiguracaoVegetalItem.Texto);
					$('.txtValor', LocalVistoria.container).focus();
				}

				if (response.Msg && response.Msg.length > 0) {
				    Mensagem.gerar(LocalVistoria.container, response.Msg);
				}
			}
		});
	}
}
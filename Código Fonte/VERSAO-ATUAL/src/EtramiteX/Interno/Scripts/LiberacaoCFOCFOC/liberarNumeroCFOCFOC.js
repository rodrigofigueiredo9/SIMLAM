/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../masterpage.js" />

LiberarNumeroCFOCFOC = {
	settings: {
		urls: {
			salvar: null,
			visualizarPessoa: null,
			verificarCPF: null,
			urlVerificarConsultaDUA: null,
		    urlGravarVerificacaoDUA : null

		},
		Mensagens: null
	},
	container: null,

	load: function (container, options) {
		if (options) {
			$.extend(LiberarNumeroCFOCFOC.settings, options);
		}

		LiberarNumeroCFOCFOC.container = MasterPage.getContent(container);
		
		LiberarNumeroCFOCFOC.container.delegate('.btnVerificarCpf', 'click', LiberarNumeroCFOCFOC.verificarCPF);
		LiberarNumeroCFOCFOC.container.delegate('.cbLiberarBlocoCFO', 'click', LiberarNumeroCFOCFOC.cbLiberarBlocoCFOChange);
		LiberarNumeroCFOCFOC.container.delegate('.cbLiberarBlocoCFOC', 'click', LiberarNumeroCFOCFOC.cbLiberarBlocoCFOCChange);
		LiberarNumeroCFOCFOC.container.delegate('.cbLiberarNumeroDigitalCFO', 'click', LiberarNumeroCFOCFOC.cbLiberarNumeroDigitalCFOChange);
		LiberarNumeroCFOCFOC.container.delegate('.cbLiberarNumeroDigitalCFOC', 'click', LiberarNumeroCFOCFOC.cbLiberarNumeroDigitalCFOCChange);
		LiberarNumeroCFOCFOC.container.delegate('.btnVisualizarPessoa', 'click', LiberarNumeroCFOCFOC.abrirModalVisualizarPessoa);
		LiberarNumeroCFOCFOC.container.delegate('.btnSalvar', 'click', LiberarNumeroCFOCFOC.salvar);


		LiberarNumeroCFOCFOC.container.delegate('.txtCpf', 'keyup', function (e) {
			if (e.keyCode == 13) $('.btnVerificarCpf', LiberarNumeroCFOCFOC.container).click();
		});

		Aux.setarFoco(LiberarNumeroCFOCFOC.container);
	},

	onChecarRetornoDUA: function () {

	    
	    $.ajax({
	        url: LiberarNumeroCFOCFOC.settings.urls.urlVerificarConsultaDUA,
	        data: JSON.stringify(LiberarNumeroCFOCFOC.RequisicaoDUA),
	        cache: false,
	        async: true,
	        type: 'POST',
	        dataType: 'json',
	        contentType: 'application/json; charset=utf-8',
	        error: Aux.error,
	        success: function (response, textStatus, XMLHttpRequest) {
	            if (!response.Valido) {
	                MasterPage.carregando(false);
	                Mensagem.gerar(LiberarNumeroCFOCFOC.container, response.Msg);
	                return;
	            }

	            if (!response.Consultado) {
	                clearTimeout(LiberarNumeroCFOCFOC.settings.timeoutID);
	                LiberarNumeroCFOCFOC.settings.timeoutID =
						setTimeout(function () {
						    LiberarNumeroCFOCFOC.onChecarRetornoDUA();
						}, 5000);

	                return;
	            }


	            $.ajax({
	                url: LiberarNumeroCFOCFOC.settings.urls.verificarCPF,
	                data: JSON.stringify(LiberarNumeroCFOCFOC.RequisicaoDUA),
	                cache: false,
	                async: false,
	                type: 'POST',
	                dataType: 'json',
	                contentType: 'application/json; charset=utf-8',
	                error: Aux.error,
	                success: function (response, textStatus, XMLHttpRequest) {
	                    if (response.EhValido) {
	                        $('.mostrar', LiberarNumeroCFOCFOC.container).removeClass('hide');
	                        $('.hdnCredenciadoId', LiberarNumeroCFOCFOC.container).val(response.Credenciado.Id);
	                        $('.hdnPessoaId', LiberarNumeroCFOCFOC.container).val(response.Credenciado.Pessoa.InternoId);
	                        $('.txtNome', LiberarNumeroCFOCFOC.container).val(response.Credenciado.Nome);
	                    }
	                    else {
	                        Mensagem.gerar(LiberarNumeroCFOCFOC.container, response.Msg);
	                    }
	                }
	            });
	         

	            MasterPage.carregando(false);
	        }
	    });
	},

	verificarCPF: function () {
		Mensagem.limpar(LiberarNumeroCFOCFOC.container);
		MasterPage.carregando(true);

		LiberarNumeroCFOCFOC.RequisicaoDUA = { cpf: $('.txtCpf', LiberarNumeroCFOCFOC.container).val(), NumeroDua: $('.txtNumeroDua', LiberarNumeroCFOCFOC.container).val() };
		
		$.ajax({
		    url: LiberarNumeroCFOCFOC.settings.urls.urlGravarVerificacaoDUA,
		    data: JSON.stringify(LiberarNumeroCFOCFOC.RequisicaoDUA),
		    cache: false,
		    async: false,
		    type: 'POST',
		    dataType: 'json',
		    contentType: 'application/json; charset=utf-8',
		    error: Aux.error,
		    success: function (response, textStatus, XMLHttpRequest) {
		        if (!response.Valido) {
		            MasterPage.carregando(false);
		            Mensagem.gerar(LiberarNumeroCFOCFOC.container, response.Msg);
		            return;
		        }

		        LiberarNumeroCFOCFOC.RequisicaoDUA.filaID = response.FilaID;

		        clearTimeout(LiberarNumeroCFOCFOC.settings.timeoutID);
		        LiberarNumeroCFOCFOC.settings.timeoutID =
					setTimeout(function () {
					    LiberarNumeroCFOCFOC.onChecarRetornoDUA();
					}, 5000);
		    }
		});

		
		//MasterPage.carregando(false);
	},

	abrirModalVisualizarPessoa: function () {
		var url = LiberarNumeroCFOCFOC.settings.urls.visualizarPessoa + '/' + $('.hdnCredenciadoId', LiberarNumeroCFOCFOC.container).val();
		Modal.abrir(url, null, 
			function (context) { Modal.defaultButtons(context); },
			Modal.tamanhoModalGrande,
			'Visualizar Responsável Técnico');
	},

	cbLiberarBlocoCFOChange: function () {
		$('.txtNumeroInicialBlocoCFO', LiberarNumeroCFOCFOC.container).closest('.block').toggleClass('hide', !$('.cbLiberarBlocoCFO').is(':checked'));
	},

	cbLiberarBlocoCFOCChange: function () {
		$('.txtNumeroInicialBlocoCFOC', LiberarNumeroCFOCFOC.container).closest('.block').toggleClass('hide', !$('.cbLiberarBlocoCFOC').is(':checked'));
	},

	cbLiberarNumeroDigitalCFOChange: function () {
		$('.ddlQtdNumeroDigitalCFO', LiberarNumeroCFOCFOC.container).closest('.block').toggleClass('hide', !$('.cbLiberarNumeroDigitalCFO').is(':checked'));
	},

	cbLiberarNumeroDigitalCFOCChange: function () {
		$('.ddlQtdNumeroDigitalCFOC', LiberarNumeroCFOCFOC.container).closest('.block').toggleClass('hide', !$('.cbLiberarNumeroDigitalCFOC').is(':checked'));
	},

	obter:function(){
		var retorno = {
			Nome: $('.txtNome', LiberarNumeroCFOCFOC.container).val(),
			CPF: $('.txtCpf', LiberarNumeroCFOCFOC.container).val(),
			CredenciadoId: $('.hdnCredenciadoId', LiberarNumeroCFOCFOC.container).val(),
			LiberarBlocoCFO: $('.cbLiberarBlocoCFO', LiberarNumeroCFOCFOC.container).is(':checked'),
			NumeroInicialCFO: $('.txtNumeroInicialBlocoCFO:visible', LiberarNumeroCFOCFOC.container).val(),
			NumeroFinalCFO: $('.txtNumeroFinalBlocoCFO:visible', LiberarNumeroCFOCFOC.container).val(),
			LiberarBlocoCFOC: $('.cbLiberarBlocoCFOC', LiberarNumeroCFOCFOC.container).is(':checked'),
			NumeroInicialCFOC: $('.txtNumeroInicialBlocoCFOC:visible', LiberarNumeroCFOCFOC.container).val(),
			NumeroFinalCFOC: $('.txtNumeroFinalBlocoCFOC:visible', LiberarNumeroCFOCFOC.container).val(),
			LiberarDigitalCFO: $('.cbLiberarNumeroDigitalCFO', LiberarNumeroCFOCFOC.container).is(':checked'),
			QuantidadeDigitalCFO: $('.ddlQtdNumeroDigitalCFO:visible :selected', LiberarNumeroCFOCFOC.container).val(),
			LiberarDigitalCFOC: $('.cbLiberarNumeroDigitalCFOC', LiberarNumeroCFOCFOC.container).is(':checked'),
			QuantidadeDigitalCFOC: $('.ddlQtdNumeroDigitalCFOC:visible :selected', LiberarNumeroCFOCFOC.container).val(),
			NumeroDua: $('.txtNumeroDua', LiberarNumeroCFOCFOC.container).val(),
			FilaID: LiberarNumeroCFOCFOC.RequisicaoDUA.filaID
		};

		return retorno;
	},

	salvar: function () {
		MasterPage.carregando(true);
		var objeto = LiberarNumeroCFOCFOC.obter();
		$.ajax({
			url: LiberarNumeroCFOCFOC.settings.urls.salvar,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.Url);
				}
				else
				{
					Mensagem.gerar(LiberarNumeroCFOCFOC.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	}
}
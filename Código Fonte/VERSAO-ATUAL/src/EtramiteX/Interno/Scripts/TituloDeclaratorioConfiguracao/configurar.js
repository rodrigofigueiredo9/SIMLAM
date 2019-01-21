/// <reference path="../masterpage.js" />
/// <reference path="../jquery.ddl.js" />

TituloDeclaratorioConfiguracao = {
	settings: {
		urls: {
			salvar: '',
			enviarArquivo: ''
		}
	},
	TiposArquivo: [],
	container: null,

	load: function (container, options) {
		TituloDeclaratorioConfiguracao.container = container;
		if (options) {
			$.extend(TituloDeclaratorioConfiguracao.settings, options);
		}

		container.delegate('.btnSalvar', 'click', TituloDeclaratorioConfiguracao.salvar);
		container.delegate('.btnAddArqSemAPP', 'click', TituloDeclaratorioConfiguracao.onEnviarArquivoSemAPPClick);
		container.delegate('.btnLimparArqSemAPP', 'click', TituloDeclaratorioConfiguracao.onLimparArquivoSemAPPClick);
		container.delegate('.btnAddArqComAPP', 'click', TituloDeclaratorioConfiguracao.onEnviarArquivoComAPPClick);
		container.delegate('.btnLimparArqComAPP', 'click', TituloDeclaratorioConfiguracao.onLimparArquivoComAPPClick);
	},

	onEnviarArquivoSemAPPClick: function () {
		return TituloDeclaratorioConfiguracao.enviarArquivoClick($('.inputFileSemAPP', TituloDeclaratorioConfiguracao.container),
			TituloDeclaratorioConfiguracao.callBackArqSemAPPEnviado);
	},

	onEnviarArquivoComAPPClick: function () {
		return TituloDeclaratorioConfiguracao.enviarArquivoClick($('.inputFileComAPP', TituloDeclaratorioConfiguracao.container),
			TituloDeclaratorioConfiguracao.callBackArqComAPPEnviado);
	},

	enviarArquivoClick: function (inputFile, callBackArqEnviado) {
		var nomeArquivo = inputFile.val();

		erroMsg = new Array();

		if (nomeArquivo == '') {
			erroMsg.push(TituloDeclaratorioConfiguracao.Mensagens.ArquivoObrigatorio);
		}
		//else {
		//	var tam = nomeArquivo.length - 4;
		//	if (!TituloDeclaratorioConfiguracao.validarTipoArquivo(nomeArquivo.toLowerCase().substr(tam))) {
		//		erroMsg.push(TituloDeclaratorioConfiguracao.Mensagens.ArquivoNaoEhDoc);
		//	}
		//}

		if (erroMsg.length > 0) {
			Mensagem.gerar(TituloDeclaratorioConfiguracao.container, erroMsg);
			return;
		}

		MasterPage.carregando(true);
		FileUpload.upload(TituloDeclaratorioConfiguracao.settings.urls.enviarArquivo, inputFile, callBackArqEnviado);
	},

	validarTipoArquivo: function (tipo) {

		var tipoValido = false;
		$(TituloDeclaratorioConfiguracao.TiposArquivo).each(function (i, tipoItem) {
			if (tipoItem == tipo) {
				tipoValido = true;
			}
		});

		return tipoValido;
	},

	callBackArqSemAPPEnviado: function (controle, retorno, isHtml) {
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoSemAPPNome', TituloDeclaratorioConfiguracao.container).text(ret.Arquivo.Nome);
			$('.hdnArquivoSemAPPJson', TituloDeclaratorioConfiguracao.container).val(JSON.stringify(ret.Arquivo));
			$('.txtArquivoSemAPPNome', TituloDeclaratorioConfiguracao.container).attr('href', '/Arquivo/BaixarTemporario?nomeTemporario=' + ret.Arquivo.TemporarioNome + '&contentType=' + ret.Arquivo.ContentType);

			$('.spanInputFileSemAPP', TituloDeclaratorioConfiguracao.container).addClass('hide');
			$('.txtArquivoSemAPPNome', TituloDeclaratorioConfiguracao.container).removeClass('hide');

			$('.btnAddArqSemAPP', TituloDeclaratorioConfiguracao.container).addClass('hide');
			$('.btnLimparArqSemAPP', TituloDeclaratorioConfiguracao.container).removeClass('hide');
		} else {
			TituloDeclaratorioConfiguracao.onLimparArquivoSemAPPClick();
			Mensagem.gerar(TituloDeclaratorioConfiguracao.container, ret.Msg);
		}
		MasterPage.carregando(false);
	},

	onLimparArquivoSemAPPClick: function () {
		$('.hdnArquivoSemAPPJson', TituloDeclaratorioConfiguracao.container).val('');
		$('.inputFileSemAPP', TituloDeclaratorioConfiguracao.container).val('');

		$('.spanInputFileSemAPP', TituloDeclaratorioConfiguracao.container).removeClass('hide');
		$('.txtArquivoSemAPPNome', TituloDeclaratorioConfiguracao.container).addClass('hide');

		$('.btnAddArqSemAPP', TituloDeclaratorioConfiguracao.container).removeClass('hide');
		$('.btnLimparArqSemAPP', TituloDeclaratorioConfiguracao.container).addClass('hide');
	},

	callBackArqComAPPEnviado: function (controle, retorno, isHtml) {
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoComAPPNome', TituloDeclaratorioConfiguracao.container).text(ret.Arquivo.Nome);
			$('.hdnArquivoComAPPJson', TituloDeclaratorioConfiguracao.container).val(JSON.stringify(ret.Arquivo));
			$('.txtArquivoComAPPNome', TituloDeclaratorioConfiguracao.container).attr('href', '/Arquivo/BaixarTemporario?nomeTemporario=' + ret.Arquivo.TemporarioNome + '&contentType=' + ret.Arquivo.ContentType);

			$('.spanInputFileComAPP', TituloDeclaratorioConfiguracao.container).addClass('hide');
			$('.txtArquivoComAPPNome', TituloDeclaratorioConfiguracao.container).removeClass('hide');

			$('.btnAddArqComAPP', TituloDeclaratorioConfiguracao.container).addClass('hide');
			$('.btnLimparArqComAPP', TituloDeclaratorioConfiguracao.container).removeClass('hide');
		} else {
			TituloDeclaratorioConfiguracao.onLimparArquivoComAPPClick();
			Mensagem.gerar(TituloDeclaratorioConfiguracao.container, ret.Msg);
		}
		MasterPage.carregando(false);
	},

	onLimparArquivoComAPPClick: function () {
		$('.hdnArquivoComAPPJson', TituloDeclaratorioConfiguracao.container).val('');
		$('.inputFileComAPP', TituloDeclaratorioConfiguracao.container).val('');

		$('.spanInputFileComAPP', TituloDeclaratorioConfiguracao.container).removeClass('hide');
		$('.txtArquivoComAPPNome', TituloDeclaratorioConfiguracao.container).addClass('hide');

		$('.btnAddArqComAPP', TituloDeclaratorioConfiguracao.container).removeClass('hide');
		$('.btnLimparArqComAPP', TituloDeclaratorioConfiguracao.container).addClass('hide');
	},

	obter: function () {
		var configuracao = {
			Id: $('.hdnId', TituloDeclaratorioConfiguracao.container).val(),
			MaximoAreaAlagada: $('.txtValorMaximoAreaAlagada', TituloDeclaratorioConfiguracao.container).val(),
			MaximoVolumeArmazenado: $('.txtValorMaximoVolumeArmazenado', TituloDeclaratorioConfiguracao.container).val(),
			BarragemSemAPP: $.parseJSON($('.hdnArquivoSemAPPJson', TituloDeclaratorioConfiguracao.container).val()),
			BarragemComAPP: $.parseJSON($('.hdnArquivoComAPPJson', TituloDeclaratorioConfiguracao.container).val())			
		};

		return configuracao;
	},

	salvar: function () {
		MasterPage.carregando(true);
		var objeto = TituloDeclaratorioConfiguracao.obter();

		$.ajax({
			url: TituloDeclaratorioConfiguracao.settings.urls.salvar,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				MasterPage.carregando(false);
				Aux.error(XMLHttpRequest, textStatus, erroThrown, TituloDeclaratorioConfiguracao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				MasterPage.carregando(false);
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedireciona);
				} else {
					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(MasterPage.getContent(TituloDeclaratorioConfiguracao.container), response.Msg);
					}
				}
			}
		});
	}
};
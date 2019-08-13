/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />


CARSolicitacaoAlterarSituacao = {
	settings: {
		urls: {
			salvar: ''
		}
	},
	container: null,
	mensagem: null,

	load: function (container, options) {
		if (options) {
			$.extend(CARSolicitacaoAlterarSituacao.settings, options);
		}

		CARSolicitacaoAlterarSituacao.container = MasterPage.getContent(container);
		CARSolicitacaoAlterarSituacao.container.delegate('.btnSalvar', 'click', CARSolicitacaoAlterarSituacao.salvar);

		$(".ddlSituacaoNova", CARSolicitacaoAlterarSituacao.container).change(CARSolicitacaoAlterarSituacao.onSituacaoChange);
		$(".btnArqLimpar", CARSolicitacaoAlterarSituacao.container).click(CARSolicitacaoAlterarSituacao.limparArquivo);
	},

	onSituacaoChange: function () {
		situacao = $('.ddlSituacaoNova').val();
		if (situacao == "3") {// invalido / cancelado
			$('.divCancelado').removeClass('hide');
		} else {
			$('.divCancelado').addClass('hide');
			$('.ddlMotivo').val("0");
			$('.inputFile').val("");
		}

		CARSolicitacaoAlterarSituacao.limparArquivo();
		MasterPage.redimensionar();

	},

	obter: function () {
		debugger;
		var container = CARSolicitacaoAlterarSituacao.container;

		// ObterArquivoCancelamento
		situacao = $('.ddlSituacaoNova').val();
		if (situacao === "3") {// invalido / cancelado
			arquivo = $('.hdnArquivo').val();
			if (arquivo.isNullOrWhitespace()) {
				Mensagem.gerar(CARSolicitacaoAlterarSituacao.container, [CARSolicitacaoAlterarSituacao.mensagem.ArquivoObrigatorio]);
				return;
			} else
				arquivo = JSON.parse(arquivo);
		}
		var obj = {
			Id: $('.hdnSolicitacaoId', container).val(),
			Numero: $('.txtSituacaoNumeroControle', container).val(),
			DataEmissao: { DataTexto: $('.txtDataEmissao', container).val() },
			SituacaoAnteriorTexto: $('.txtSituacaoAtual', container).val(),
			DataSituacaoAnterior: { DataTexto: $('.txtSituacaoDataAnterior', container).val() },
			DataSituacao: { DataTexto: $('.txtDataSituacaoNova', container).val() },
			Motivo: $('.ddlMotivo :selected', container).val(),
			DescricaoMotivo: $('.txtSituacaoMotivo', container).val(),
			SituacaoId: situacao,
			ArquivoCancelamento: arquivo
		};

		return obj;
	},

	salvar: function () {
		Mensagem.limpar(CARSolicitacaoAlterarSituacao.container);
		var SituacaoAnteriorTexto = $('.txtSituacaoAtual').val();

		if (SituacaoAnteriorTexto === 'Válido') {
			Modal.confirma({
				btnOkCallback: function (container) {
					CARSolicitacaoAlterarSituacao.callBackAlterarSituacao(container);
				},
				titulo: "Alterar situação solicitação CAR",
				conteudo: CARSolicitacaoAlterarSituacao.mensagem.AlterarSituacaoMsgConfirmacao.Texto,
				tamanhoModal: Modal.tamanhoModalMedia
			});
		} else {
			MasterPage.carregando(true);
			$.ajax({
				url: CARSolicitacaoAlterarSituacao.settings.urls.salvar,
				data: JSON.stringify(CARSolicitacaoAlterarSituacao.obter()),
				cache: false,
				async: false,
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, SecagemMecanicaGraos.container);
				},
				success: function (response, textStatus, XMLHttpRequest) {
					if (response.EhValido) {
						MasterPage.redireciona(response.UrlRedirecionar);
					}
					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(CARSolicitacaoAlterarSituacao.container, response.Msg);
					}
				}
			});

		}

		MasterPage.carregando(false);
	},

	callBackAlterarSituacao: function (container) {

		MasterPage.carregando(true);

		$.ajax({
			url: CARSolicitacaoAlterarSituacao.settings.urls.salvar,
			data: JSON.stringify(CARSolicitacaoAlterarSituacao.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, CARSolicitacaoAlterarSituacao.container);
				MasterPage.carregando(false);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				Modal.fechar(container);

				if (response.EhValido) {
					//CARSolicitacaoAlterarSituacao.callBackPost(response, container);
					MasterPage.redireciona(response.UrlRedirecionar);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(CARSolicitacaoAlterarSituacao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	enviarArquivo: function (url) {
		var nome = "enviando ...";
		var nomeArquivo = $('.inputFile').val();
		if (nomeArquivo === '') {
			Mensagem.gerar(CARSolicitacaoAlterarSituacao.container, [CARSolicitacaoAlterarSituacao.mensagem.ArquivoObrigatorio]);
			return;
		}
		MasterPage.carregando(true);

		var inputFile = $('.inputFileDiv input[type="file"]');
		inputFile.attr("id", "ArquivoId");

		FileUpload.upload(url, inputFile, CARSolicitacaoAlterarSituacao.callBackEnviarArquivo);
		$('.inputFile').val('');
		MasterPage.carregando(false);
	},

	callBackEnviarArquivo: function (controle, retorno, isHtml) {
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoNome', CARSolicitacaoAlterarSituacao.container).val(ret.Arquivo.Nome);
			$('.hdnArquivo', CARSolicitacaoAlterarSituacao.container).val(JSON.stringify(ret.Arquivo));

			$('.spanInputFile', CARSolicitacaoAlterarSituacao.container).addClass('hide');
			$('.txtArquivoNome', CARSolicitacaoAlterarSituacao.container).removeClass('hide');

			$('.btnArq', CARSolicitacaoAlterarSituacao.container).addClass('hide');
			$('.btnArqLimpar', CARSolicitacaoAlterarSituacao.container).removeClass('hide');

		} else {
			CARSolicitacaoAlterarSituacao.limparArquivo();
		}

		Mensagem.gerar(MasterPage.getContent(CARSolicitacaoAlterarSituacao.container), ret.Msg);
	},

	limparArquivo: function () {
		$('.txtArquivoNome', CARSolicitacaoAlterarSituacao.container).data('arquivo', null);
		$('.txtArquivoNome', CARSolicitacaoAlterarSituacao.container).val('');
		$('.hdnArquivo', CARSolicitacaoAlterarSituacao.container).val('');

		$('.spanInputFile', CARSolicitacaoAlterarSituacao.container).removeClass('hide');
		$('.txtArquivoNome', CARSolicitacaoAlterarSituacao.container).addClass('hide');

		$('.btnArq', CARSolicitacaoAlterarSituacao.container).removeClass('hide');
		$('.btnArqLimpar', CARSolicitacaoAlterarSituacao.container).addClass('hide');

		$('.lnkArquivo', CARSolicitacaoAlterarSituacao.container).remove();
	},
}
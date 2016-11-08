/// <reference path="../JQuery/jquery-1.4.3.js"/>
/// <reference path="../masterpage.js"/>
/// <reference path="../mensagem.js"/>
/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />

Funcionario = {
	settings: {
		mensagens: null
	},

	urlVerificarResponsavelSetor: "Url carregada na tela",

	load: function () {
	    $('.CpfFuncionarioContainer').keyup(Funcionario.onVerficarCpfKeyUp);
	    $('.celulaSeletorLinha').click(Funcionario.onAdicionarPapel);
		$('.celSetorResp').click(function (e) { SetClickCheckBox(e, this); });
		$("#btnAddCargo").click(Funcionario.onAdicionarCargo);
		$("#btnAddSetor").click(Funcionario.onAdicionarSetor);
		$("#AlterarSenha").click(Funcionario.onAddClass);
	},


	onAdicionarPapel: function (e) {

		SetClickCheckBox(e, this);

		var valor = $("#tablePapeis input").serializeArray();
		var txtPermissoes = $("#TextoPermissoes");

		$.get(urlPermissao, valor, function (data, textStatus, XMLHttpRequest) {
			if (Aux.errorGetPost(data, textStatus, XMLHttpRequest, $("#central"))) {
				return;
			}
			Funcionario.totalLength++;
			if (data) {
				Funcionario.requestResults[Funcionario.totalLength] = data;
			} else {
				Funcionario.requestResults[Funcionario.totalLength] = -1;
			}
		}, "text");

		Funcionario.consumirTexto();
	},

	requestResults: {},
	lastProcessedIndex: 0,
	totalLength: -1,
	consumirAtivo: false,

	consumirTexto: function () {

		if (!Funcionario.consumirAtivo) {

			Funcionario.consumirAtivo = true;
			var txtPermissoes = $("#TextoPermissoes");

			var intervalId = setInterval(function () {
				if (Funcionario.requestResults[Funcionario.lastProcessedIndex]) {
					if (Funcionario.requestResults[Funcionario.lastProcessedIndex] != -1) {
						txtPermissoes.val(Funcionario.requestResults[Funcionario.lastProcessedIndex]);
					} else {
						txtPermissoes.val("");
					}
					Funcionario.lastProcessedIndex++;
				} else if (Funcionario.totalLength != -1 && Funcionario.lastProcessedIndex >= Funcionario.totalLength) {
					Funcionario.consumirAtivo = false;
					Funcionario.requestResults = {};
					Funcionario.lastProcessedIndex = 0;
					Funcionario.totalLength = -1;
					clearInterval(intervalId);
				}
			}, 800);
		}
	},

	onAdicionarCargo: function () {
		Mensagem.limpar($('#central'));
		var mensagens = [];

		var ddlSel = $("#ddlCargos option:selected");

		if (ddlSel.val() === "0") {
			mensagens.push(Funcionario.settings.mensagens.CargoObrigatorio);
			Mensagem.gerar($('#central'), mensagens);
			return;
		}

		var linha = $(".linhaCargo").clone();
		var tableCargo = $("#tableCargo");
		var hdnIdCargo = linha.find(".hdnCargoId");

		if (tableCargo.find(".hdnCargoId[value=" + ddlSel.val() + "]").val() !== undefined) {
			mensagens.push(Funcionario.settings.mensagens.CargoDuplicado);
			Mensagem.gerar($('#central'), mensagens);
			return;
		}

		hdnIdCargo.val(ddlSel.val());
		linha.find(".celCargoTexto").text(ddlSel.text());

		hdnIdCargo.removeAttr("disabled");
		linha.removeClass("linhaCargo");

		tableCargo.append(linha);

		$('#ddlCargos option:eq(0)').attr('selected', 'selected');

		Listar.atualizarEstiloTable(tableCargo);
	},

	onAdicionarSetor: function () {
		Mensagem.limpar($('#central'));
		var mensagens = [];

		var ddlSel = $("#ddlSetores option:selected");

		if (ddlSel.val() === "0") {
			mensagens.push(Funcionario.settings.mensagens.SetoresObrigatorio);
			Mensagem.gerar($('#central'), mensagens);
			return;
		}

		var linha = $(".linhaSetor").clone();
		var tableSetor = $("#tableSetor");
		var hdnSetorId = linha.find(".hdnSetorId");
		var hdnSetorIndex = linha.find(".hdnSetorIndex");
		var hdnSetorckb = linha.find(".ckbSetor");

		if (tableSetor.find(".hdnSetorId[value=" + ddlSel.val() + "]").val() !== undefined) {
			mensagens.push(Funcionario.settings.mensagens.SetorDuplicado);
			Mensagem.gerar($('#central'), mensagens);
			return;
		}

		linha.find(".celSetorTexto").text(ddlSel.text());
		linha.find('.celSetorResp').click(function (e) { SetClickCheckBox(e, this); });

		hdnSetorId.val(ddlSel.val());
		linha.find('.hdnSetorIndex').val(ddlSel.val());

		var nameIndex = hdnSetorckb.attr('name');
		var nameId = hdnSetorId.attr('name');

		hdnSetorckb.attr("name", nameIndex.replace("[]", '[' + ddlSel.val() + ']'));
		hdnSetorId.attr("name", nameId.replace("[]", '[' + ddlSel.val() + ']'));

		hdnSetorId.removeAttr("disabled");
		hdnSetorIndex.removeAttr("disabled");
		linha.removeClass("linhaSetor");

		tableSetor.append(linha);

		$('#ddlSetores option:eq(0)').attr('selected', 'selected');

		Listar.atualizarEstiloTable(tableSetor);

		$.ajax({ url: Funcionario.urlVerificarResponsavelSetor, data: "idSetor=" + ddlSel.val(), type: 'GET', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, $("#central"));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				var data = response.data;

				if (data.Msg != null && data.Msg.length > 0) {
					Mensagem.gerar($(".mensagemContent"), data.Msg);
					return;
				}
				else {
					if (!data.TemResponsavel) {
						linha.find('.celSetorResp div').removeClass("hide");
					}
				}
			}
		});
	},

	onExcluirLinha: function (ctr) {
		var tabela = $(ctr).parents(".dataGridTable");
		$(ctr).parents("tr").remove();
		Listar.atualizarEstiloTable(tabela);
	},


	onVerficarCpfKeyUp: function (e) {
	    var keyENTER = 13;
	    if (e.keyCode == keyENTER) {
	        $('#btnVerificarCpf').click();
	    }
	    return false;
	},


    //----------ANEXOS - ENVIAR ARQUIVO---------------
	onEnviarAnexoArquivoClick: function (url) {
	    var nome = "enviando ...";

	    var nomeArquivo = $('.inputFile').val();

	    Mensagem.limpar($('#central'));
	    var mensagens = [];


	    if (nomeArquivo === '') {
	        mensagens.push(Funcionario.settings.mensagens.ArquivoObrigatorio);
	        Mensagem.gerar($('#central'), mensagens);
	        return;
	    }

	    if (nomeArquivo !== '') {
	        var tam = nomeArquivo.length - 4;
	        if (nomeArquivo.toLowerCase().substr(tam) !== ".jpg" && nomeArquivo.toLowerCase().substr(tam) !== ".gif"
				&& nomeArquivo.toLowerCase().substr(tam) !== ".png") {
	            mensagens.push(Funcionario.settings.mensagens.ArquivoNaoImagem);
	            Mensagem.gerar($('#central'), mensagens);
	            return;
	        }
	    }

	    var inputFile = $('.inputFileDiv input[type="file"]');

	    inputFile.attr("id", "Arquivo");

	    FileUpload.upload(url, inputFile, Funcionario.msgArqEnviado);

	    $('.inputFile').val('');
	},


	msgArqEnviado: function (controle, retorno, isHtml) {
	    var ret = eval('(' + retorno + ')');
	    if (ret.Arquivo != null) {
	        $('.txtArquivoNome', Funcionario.container).val(ret.Arquivo.Nome);
	        $('.hdnAnexoArquivoJson', Funcionario.container).val(JSON.stringify(ret.Arquivo));

	        $('.spanInputFile', Funcionario.container).addClass('hide');
	        $('.txtArquivoNome', Funcionario.container).removeClass('hide');

	        $('.btnArq', Funcionario.container).addClass('hide');
	        $('.btnArqLimpar', Funcionario.container).removeClass('hide');

	        $('.hdnArquivoContentType', Funcionario.container).val(ret.Arquivo.ContentType);
	        $('.hdnArquivoExtensao', Funcionario.container).val(ret.Arquivo.Extensao);
	        $('.hdnArquivoId', Funcionario.container).val(ret.Arquivo.Id);
	        $('.hdnArquivoNome', Funcionario.container).val(ret.Arquivo.Nome);
	        $('.hdnArquivoTemporarioNome', Funcionario.container).val(ret.Arquivo.TemporarioNome);


	    } else {
	        Funcionario.onLimparArquivo();
	    }

	    Mensagem.gerar(MasterPage.getContent(Funcionario.container), ret.Msg);
	},

	onLimparArquivo: function () {

	    //implementar Limpar
	    $('.txtArquivoNome', Funcionario.container).data('arquivo', null);
	    $('.txtArquivoNome', Funcionario.container).val("");
	    $('.hdnAnexoArquivoJson', Funcionario.container).val("");

	    $('.spanInputFile', Funcionario.container).removeClass('hide');
	    $('.txtArquivoNome', Funcionario.container).addClass('hide');

	    $('.btnArq', Funcionario.container).removeClass('hide');
	    $('.btnArqLimpar', Funcionario.container).addClass('hide');

	    $('.lnkArquivo', Funcionario.container).remove();


	    $('.hdnArquivoContentType', Funcionario.container).val(null);
	    $('.hdnArquivoExtensao', Funcionario.container).val(null);
	    $('.hdnArquivoId', Funcionario.container).val(null);
	    $('.hdnArquivoNome', Funcionario.container).val(null);
	    $('.hdnArquivoTemporarioNome', Funcionario.container).val(null);


	},

	onAddClass: function () {
		var div = $("#divAlterarSenha");
		div.find('#Senha').attr('value', '');
		div.find('#ConfirmarSenha').attr('value', '');
		div.attr('class', (div.attr('class') == 'block hide') ? 'block' : 'block hide');
	},

    onSubmitEdit: function (){

        var editarVM =
		{
		    Funcionario:
			{
			    Arquivo: null,
			},

		};

        var arquivo = $('.hdnAnexoArquivoJson', Funcionario.container).val();
        editarVM.Funcionario.Arquivo = $.parseJSON(arquivo);
    }
}



Funcionario.load();
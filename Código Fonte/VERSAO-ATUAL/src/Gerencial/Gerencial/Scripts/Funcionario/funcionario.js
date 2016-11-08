/// <reference path="../JQuery/jquery-1.4.3.js"/>
/// <reference path="../masterpage.js"/>
/// <reference path="../mensagem.js"/>


Funcionario = {
	requestResults: {},
	lastProcessedIndex: 0,
	totalLength: -1,
	consumirAtivo: false,
	urlVerificarResponsavelSetor: "Url carregada na tela",

	load: function () {
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

		var ddlSel = $("#ddlCargos option:selected");

		if (ddlSel.val() === "0") {
			return;
		}

		var linha = $(".linhaCargo").clone();
		var tableCargo = $("#tableCargo");
		var hdnIdCargo = linha.find(".hdnCargoId");

		if (tableCargo.find(".hdnCargoId[value=" + ddlSel.val() + "]").val() !== undefined)
			return;

		hdnIdCargo.val(ddlSel.val());
		linha.find(".celCargoTexto").text(ddlSel.text());

		hdnIdCargo.removeAttr("disabled");
		linha.removeClass("linhaCargo");

		tableCargo.append(linha);

		Listar.atualizarEstiloTable(tableCargo);
	},

	onAdicionarSetor: function () {

		var ddlSel = $("#ddlSetores option:selected");

		if (ddlSel.val() === "0") {
			return;
		}

		var linha = $(".linhaSetor").clone();
		var tableSetor = $("#tableSetor");
		var hdnSetorId = linha.find(".hdnSetorId");
		var hdnSetorIndex = linha.find(".hdnSetorIndex");
		var hdnSetorckb = linha.find(".ckbSetor");

		if (tableSetor.find(".hdnSetorId[value=" + ddlSel.val() + "]").val() !== undefined)
			return;

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
		Listar.atualizarEstiloTable(tableSetor);

		$.ajax({ url: Funcionario.urlVerificarResponsavelSetor, data: "idSetor=" + ddlSel.val(), type: 'GET', typeData: 'json',
			contentType: 'application/json; charset=utf-8', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, $("#central"));
			},
			success: function (response, textStatus, XMLHttpRequest) {

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

	onAddClass: function () {
		var div = $("#divAlterarSenha");
		div.find('#Senha').attr('value', '');
		div.find('#ConfirmarSenha').attr('value', '');
		div.attr('class', (div.attr('class') == 'block hide')?'block':'block hide');
	}
}

Funcionario.load();
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />

Cobranca = {
	settings: {
		urls: {
			salvar: '',
			carregar: '',
			cancelar: ''
		},
		mensagens: null
	},

	container: null,

	load: function (container, options) {
		if (options) { $.extend(Cobranca.settings, options); }
		Cobranca.container = container;

		container.delegate('.btnSalvar', 'click', Cobranca.salvar);
		container.delegate('.btnEditar', 'click', Cobranca.editar);
		container.delegate('.btnRecalcular', 'click', Cobranca.recalcular);
		container.delegate('.btnAddSubparcela', 'click', Cobranca.addSubparcela);
		container.delegate('.ddlParcelas', 'change', Cobranca.alterarParcelas);
		container.delegate('.linkCancelar', 'click', Cobranca.cancelar);
		
		$('.txtProcessoNumero', container).focus();
	},

	obter: function () {
		var container = Cobranca.container;

		var obj = {
			Id: $('.hdnCobrancaId', container).val(),
			ProcessoNumero: $('.txtProcessoNumero', container).val(),
			NumeroAutos: $('.txtNumeroAutos', container).val(),
			NumeroFiscalizacao: $('.txtFiscalizacao', container).val(),
			NumeroIUF: $('.txtNumeroIUF', container).val(),
			SerieId: $('.hdnSerieId', container).val(),
			SerieTexto: $('.txtSerie', container).val(),
			DataLavratura: { DataTexto: $('.txtDataLavratura', container).val() },
			DataIUF: { DataTexto: $('.txtDataIUF', container).val() },
			DataJIAPI: { DataTexto: $('.txtDataJIAPI', container).val() },
			DataCORE: { DataTexto: $('.txtDataCORE', container).val() },
			CodigoReceitaId: $('.ddlCodigoReceita :selected', container).val(),
			AutuadoPessoaId: $('.hdnAutuadoPessoaId', container).val(),
			UltimoParcelamento: JSON.parse($('.hdnParcelamento', container).val())
		}
		obj.UltimoParcelamento.ValorMulta = $('.txtValorMulta', container).val();
		obj.UltimoParcelamento.QuantidadeParcelas = $('.ddlParcelas :selected', container).val();
		obj.UltimoParcelamento.Data1Vencimento = { DataTexto: $('.txtData1Vencimento', container).val() };
		obj.UltimoParcelamento.DataEmissao = { DataTexto: $('.txtDataEmissao', container).val() };
		obj.UltimoParcelamento.DUAS = Cobranca.obterListaParcelamento();

		return obj;
	},

	obterListaParcelamento: function () {
		var lista = [];

		$($('.tabParcelas tbody tr:not(.trTemplateRow) .hdnItemJSon', Cobranca.container)).each(function () {
			var item = JSON.parse($(this).val());
			var itensHtml = Array.from(this.parentElement.parentElement.children).filter(x => x.innerHTML.indexOf('input') > -1)
			item.NumeroDUA = itensHtml[0].children[0].value;
			if (itensHtml.length == 5) {
				item.ValorPago = parseFloat(itensHtml[1].children[0].value.replaceAll('.', '').replaceAll(',', '.'));
				item.DataPagamento = { DataTexto: itensHtml[2].children[0].value };
				item.InformacoesComplementares = itensHtml[3].children[0].value;
			} else {
				item.DataVencimento = { DataTexto: itensHtml[1].children[0].value };
				item.ValorPago = parseFloat(itensHtml[2].children[0].value.replaceAll('.', '').replaceAll(',', '.'));
				item.DataPagamento = { DataTexto: itensHtml[3].children[0].value };
				item.InformacoesComplementares = itensHtml[4].children[0].value;

				var parcela = Array.from(this.parentElement.parentElement.children).filter(x => x.innerHTML.indexOf('parcela') > -1);
				item.Parcela = parcela[0].children[0].innerText;
				var vrteHtml = Array.from(this.parentElement.parentElement.children).filter(x => x.innerHTML.indexOf('vrte') > -1);
				item.VRTE = parseFloat(vrteHtml[0].children[0].innerText.replaceAll('.', '').replaceAll(',', '.'));				
				var situacao = Array.from(this.parentElement.parentElement.children).filter(x => x.innerHTML.indexOf('situacao') > -1);
				item.Situacao = situacao[0].children[0].innerText;
				var valorDua = Array.from(this.parentElement.parentElement.children).filter(x => x.innerHTML.indexOf('valorDUA') > -1);
				item.ValorDUA = parseFloat(valorDua[0].children[0].innerText.replaceAll('.', '').replaceAll(',', '.'));		
				item.Tid = "";
				if (lista.filter(x => x.Id == item.Id).length > 0) {
					item.ParcelaPaiId = item.Id;
					item.Id = 0;
				}
			}
			lista.push(item);
		});

		return lista;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: Cobranca.settings.urls.salvar,
			data: JSON.stringify(Cobranca.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Cobranca.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Cobranca.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	alterarParcelas: function () {
		MasterPage.carregando(true);

		var container = Cobranca.container;
		var id = $('.hdnCobrancaId', container).val();
		var parcela = $('.ddlParcelas :selected', container).val();
		MasterPage.redireciona(Cobranca.settings.urls.carregar + "?parcela=" + parcela);

		MasterPage.carregando(false);
	},

	editar: function () {
		MasterPage.carregando(true);

		var container = Cobranca.container;
		var fiscalizacaoId = $('.txtFiscalizacao', container).val();
		MasterPage.redireciona(Cobranca.settings.urls.carregar + "/" + fiscalizacaoId);

		MasterPage.carregando(false);
	},

	cancelar: function () {
		MasterPage.carregando(true);

		var container = Cobranca.container;
		var fiscalizacaoId = $('.txtFiscalizacao', container).val();
		MasterPage.redireciona(Cobranca.settings.urls.cancelar + "/" + fiscalizacaoId);

		MasterPage.carregando(false);
	},

	recalcular: function () {
		if (confirm("Esta ação realizará o cálculo das parcelas que possuem o Valor (R$) zerado e as ações não salvas serão perdidas. Deseja continuar?")) {
			MasterPage.carregando(true);
			MasterPage.redireciona(Cobranca.settings.urls.carregar + "?recalcular=1");
			MasterPage.carregando(false);
		}
	},

	addSubparcela: function () {
		var newRow = $(this.parentElement.parentElement).clone();
		if (newRow[0].children[7].children[0].innerText == "Pago Parcial") {
			$(this.parentElement.parentElement)[0].children[7].children[0].innerText = "Pago";
			var acao = Array.from($(this.parentElement.parentElement)[0].children).filter(x => x.className == 'tdAcoes')[0].children[2];
			acao.disabled = true;
			$(acao).addClass('ui-button-disabled ui-state-disabled');

			var input = "<input class='text dataVencimento maskData' title='' style='width=100%' />";
			newRow[0].innerHTML = newRow[0].innerHTML.replaceAll(newRow[0].children[2].innerHTML, input);
			newRow[0].children[0].children[0].innerText = newRow[0].children[0].children[0].innerText + ' - Subparcela';
			newRow[0].children[1].children[0].value = "";

			var valorDua = parseFloat(newRow[0].children[3].children[0].innerText.replaceAll('.', '').replaceAll(',', '.'));
			var valorPago = parseFloat(newRow[0].children[4].children[0].value.replaceAll('.', '').replaceAll(',', '.'));
			var vrte = parseFloat(newRow[0].children[5].children[0].innerText.replaceAll('.', '').replaceAll(',', '.'));
			vrte = valorDua / vrte;
			var valorRestante = valorDua - valorPago;
			
			newRow[0].children[3].children[0].innerText = "";
			newRow[0].children[4].children[0].value = "";
			newRow[0].children[5].children[0].innerText = (valorRestante/vrte).formatMoney(4, ',', '.');;
			newRow[0].children[6].children[0].value = "";
			newRow[0].children[7].children[0].innerText = "Em Aberto";
			$('.tabParcelas tbody').append(newRow);
			Mascara.load(Cobranca.container);
		} else {
			ExibirMensagemErro('É permitido adicionar subparcela apenas para uma parcela com situação \"Pago Parcial\".');
		}
	}
}

String.prototype.trim = function () {
	return this.replace(/^\W+|\W+$/g, "");
}

Number.prototype.formatMoney = function (c, d, t) {
	var n = this,
		c = isNaN(c = Math.abs(c)) ? 2 : c,
		d = d == undefined ? "." : d,
		t = t == undefined ? "," : t,
		s = n < 0 ? "-" : "",
		i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
		j = (j = i.length) > 3 ? j % 3 : 0;
	return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

function ExibirMensagemErro(erro) {
	var mensagem = '\
			<div class=\"mensagemSistema erro ui-draggable\" style=\"position: relative;\">\
				<div class=\"textoMensagem \">\
					<a class=\"fecharMensagem\" title=\"Fechar Mensagem\">Fechar Mensagem</a>\
					<p> Mensagem do Sistema</p>\
					<ul>\
						<li>' + erro + '</li>\
					</ul>\
				</div>\
				<div class=\"redirecinamento block containerAcoes hide\">\
					<h5> O que deseja fazer agora ?</h5>\
					<p class=\"hide\">#DESCRICAO</p>\
					<div class=\"coluna100 margem0 divAcoesContainer\">\
						<p class=\"floatLeft margem0 append1\"><button title=\"[title]\" class=\"btnTemplateAcao hide ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only\" role=\"button\" aria-disabled=\"false\"><span class=\"ui-button-text\">[ACAO]</span></button></p>\
						<div class=\"containerBotoes\"></div>\
					</div>\
				</div>\
			</div>';
	$('.mensagemSistemaHolder')[0].innerHTML = mensagem;
}
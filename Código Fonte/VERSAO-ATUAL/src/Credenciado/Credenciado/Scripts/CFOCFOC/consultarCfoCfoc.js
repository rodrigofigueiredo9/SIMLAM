/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.ddl.js" />

ConsultarNumeroCFOCFOCLiberado = {
	settings: {
		urls: {
			buscar: null,
			verificarCPF: null,
			visualizarPessoa: null,
			invalidarModal: null,
			invalidar: null
		},
		Mensagens: null
	},
	container: null,

	Blocos: [],

	Digitais: [],

	load: function (container, options) {

		if (options) {
			$.extend(ConsultarNumeroCFOCFOCLiberado.settings, options);
		}

		ConsultarNumeroCFOCFOCLiberado.container = MasterPage.getContent(container);

		ConsultarNumeroCFOCFOCLiberado.container.delegate('.btnBuscar', 'click', ConsultarNumeroCFOCFOCLiberado.buscar);
		ConsultarNumeroCFOCFOCLiberado.container.delegate('.btnVisualizarMotivo', 'click', ConsultarNumeroCFOCFOCLiberado.abrirModalMotivo);

		//Eventos Paginação
		ConsultarNumeroCFOCFOCLiberado.container.delegate('.paginar.comeco', 'click', ConsultarNumeroCFOCFOCLiberado.paginarComeco);
		ConsultarNumeroCFOCFOCLiberado.container.delegate('.paginar.anterior', 'click', ConsultarNumeroCFOCFOCLiberado.paginarAnterior);
		ConsultarNumeroCFOCFOCLiberado.container.delegate('.paginar.proxima', 'click', ConsultarNumeroCFOCFOCLiberado.paginarProximo);
		ConsultarNumeroCFOCFOCLiberado.container.delegate('.paginar.final', 'click', ConsultarNumeroCFOCFOCLiberado.paginarUltima);

		ConsultarNumeroCFOCFOCLiberado.container.delegate('.paginar.pag', 'click', ConsultarNumeroCFOCFOCLiberado.paginar);

		Aux.setarFoco(ConsultarNumeroCFOCFOCLiberado.container);
		Mascara.load();
	},

	obterFiltros: function (container) {
		var retorno = {
			TipoDocumento: $('.ddlTipoDocumento :selected', container).val(),
			Numero: $('.txtNumero', container).val(),
			DataInicialEmissao: $('.txtDataInicialEmissao', container).val(),
			DataFinalEmissao: $('.txtDataFinalEmissao', container).val(),
			TipoNumero: 0
		};

		return retorno;
	},

	buscar: function () {
		MasterPage.carregando(true);
		var container = $(this).closest('fieldset');

		var objeto = ConsultarNumeroCFOCFOCLiberado.obterFiltros(container);

		if ($(this).hasClass('bloco')) {
			objeto.TipoNumero = 1;
		} else {
			objeto.TipoNumero = 2;
		}

		$.ajax({
			url: ConsultarNumeroCFOCFOCLiberado.settings.urls.buscar,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {

					if (response.lstRetorno && response.lstRetorno.length > 0) {
						if (objeto.TipoNumero == 1) {
							ConsultarNumeroCFOCFOCLiberado.Blocos = response.lstRetorno;
						} else {
							ConsultarNumeroCFOCFOCLiberado.Digitais = response.lstRetorno;
						}
						ConsultarNumeroCFOCFOCLiberado.carregarTabela(container, 1, response.lstRetorno);
						$('.divEsconder', container).removeClass('hide');
					} else {
						Mensagem.gerar(ConsultarNumeroCFOCFOCLiberado.container, response.Msg);
						$('tbody tr:not(.templateRow)', container).remove();
						$('.divEsconder', container).addClass('hide');
					}
					ConsultarNumeroCFOCFOCLiberado.quantidadeUtilizada(response, objeto);
				}
				else {
					Mensagem.gerar(ConsultarNumeroCFOCFOCLiberado.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},

	carregarTabela: function (container, numeroPagina, lista) {
		Mensagem.limpar(ConsultarNumeroCFOCFOCLiberado.container);
		var tabela = $(container).find('.dataGridTable');
		var qtdPaginas = Math.ceil(lista.length / 5);

		$(container).find('.numerosPag').empty();

		

		if (qtdPaginas <= 10) {						
		    for (var i = 0; i < qtdPaginas; i++) {
		        $(container).find('.numerosPag').append("<a class='" + (i + 1) + " paginar pag'>" + (i + 1) + "</a>");
		    }
		}else{
		    if (numeroPagina <= 6) {											
		        for (var i = 0; i < 10; i++) {
		            $(container).find('.numerosPag').append("<a class='" + (i + 1) + " paginar pag'>" + (i + 1) + "</a>");
		        }
		    } else if (numeroPagina + 4 >= qtdPaginas) {	
		        for (i = qtdPaginas - 9; i < qtdPaginas; i++) {
		            $(container).find('.numerosPag').append("<a class='" + (i + 1) + " paginar pag'>" + (i + 1) + "</a>");
		        }
		    } else {
		        for (i = numeroPagina - 5; i < numeroPagina + 4; i++) {
		            $(container).find('.numerosPag').append("<a class='" + (i + 1) + " paginar pag'>" + (i + 1) + "</a>");

		        }
		        console.log("ENTROU");
		    }																
		}

		$(container).find('.paginacaoCaixa').find('.hdnPaginaProxima').val((numeroPagina == qtdPaginas ? qtdPaginas : (numeroPagina + 1)));
		$(container).find('.paginacaoCaixa').find('.hdnPaginaUltima').val((qtdPaginas));
		$(container).find('.paginacaoCaixa').find('.hdnPaginaAnterior').val((numeroPagina == 1 ? 1 : (numeroPagina - 1)));

		$('tbody tr:not(.templateRow)', tabela).remove();

		var linha = null;
		var item = null;
		var index = (numeroPagina * 5) - 5;

		for (var i = index; i < (index + 5) ; i++) {
			linha = $('.templateRow', tabela).clone();
			item = lista[i];

			$('.tipoDocumento', linha).append(item.TipoDocumentoTexto);
			$('.Numero', linha).append(item.Numero);
			$('.Utilizado', linha).append(item.UtilizadoTexto);
			$('.Situacao', linha).append(item.SituacaoTexto);
			$('.hdnObjetoJson', linha).val(JSON.stringify(item));

			if (!item.Motivo || item.Motivo == '') {
				$('.btnVisualizarMotivo', linha).addClass('hide');
			} else {
				$('.btnVisualizarMotivo', linha).removeClass('hide');
			}


			$(linha).removeClass('hide').removeClass('templateRow');

			$('tbody', tabela).append(linha);

			if ((i + 1) == lista.length) {
				break;
			}
		}

		$(tabela).removeClass('hide');
		$(tabela).closest('fieldset').find('.paginacaoCaixa').removeClass('hide');
		$(tabela).closest('fieldset').find('.paginacaoCaixa').find('.numerosPag').find('.' + numeroPagina).addClass('ativo');
		Listar.atualizarEstiloTable(container);
	},

	quantidadeUtilizada: function (response, filtros) {
		if (filtros.TipoNumero == 1) {
			$('.lblQtdCFOBloco', ConsultarNumeroCFOCFOCLiberado.container).text(response.QtdCFO);
			$('.lblQtdCFOCBloco', ConsultarNumeroCFOCFOCLiberado.container).text(response.QtdCFOC);
			$('.lblQtdCFOBlocoUtilizado', ConsultarNumeroCFOCFOCLiberado.container).text(response.QtdCFOUtilizado);
			$('.lblQtdCFOCBlocoUtilizado', ConsultarNumeroCFOCFOCLiberado.container).text(response.QtdCFOCUtilizado);
		} else {
			$('.lblQtdCFODigital', ConsultarNumeroCFOCFOCLiberado.container).text(response.QtdCFO);
			$('.lblQtdCFOCDigital', ConsultarNumeroCFOCFOCLiberado.container).text(response.QtdCFOC);
			$('.lblQtdCFODigitalUtilizado', ConsultarNumeroCFOCFOCLiberado.container).text(response.QtdCFOUtilizado);
			$('.lblQtdCFOCDigitalUtilizado', ConsultarNumeroCFOCFOCLiberado.container).text(response.QtdCFOCUtilizado);
		}
	},

	abrirModalMotivo: function () {

		var objetoJson = JSON.parse($(this).closest('tr').find('.hdnObjetoJson').val());
		var obj = {
			Id: objetoJson.Id,
			Motivo: objetoJson.Motivo,
			TipoDocumento: objetoJson.TipoDocumentoTexto,
			Numero: objetoJson.Numero,
			IsVisualizar: true
		};

		Modal.abrir(ConsultarNumeroCFOCFOCLiberado.settings.urls.invalidarModal, { vm: obj }, function (content) {
			Modal.defaultButtons(content, null, null, null, null, 'Cancelar', null, false);
		}, Modal.tamanhoModalPequena);
	},

	//Paginação
	paginarComeco: function () {
		var container = $(this).closest('fieldset');
		var lista = $('.btnBuscar', container).hasClass('bloco') ? ConsultarNumeroCFOCFOCLiberado.Blocos : ConsultarNumeroCFOCFOCLiberado.Digitais;

		ConsultarNumeroCFOCFOCLiberado.carregarTabela(container, 1, lista);
	},

	paginarAnterior: function () {
		var container = $(this).closest('fieldset');
		var lista = $('.btnBuscar', container).hasClass('bloco') ? ConsultarNumeroCFOCFOCLiberado.Blocos : ConsultarNumeroCFOCFOCLiberado.Digitais;

		ConsultarNumeroCFOCFOCLiberado.carregarTabela(container, +$('.hdnPaginaAnterior', container).val(), lista);
	},

	paginarProximo: function () {
		var container = $(this).closest('fieldset');
		var lista = $('.btnBuscar', container).hasClass('bloco') ? ConsultarNumeroCFOCFOCLiberado.Blocos : ConsultarNumeroCFOCFOCLiberado.Digitais;

		ConsultarNumeroCFOCFOCLiberado.carregarTabela(container, +$('.hdnPaginaProxima', container).val(), lista);
	},

	paginarUltima: function () {
		var container = $(this).closest('fieldset');
		var lista = $('.btnBuscar', container).hasClass('bloco') ? ConsultarNumeroCFOCFOCLiberado.Blocos : ConsultarNumeroCFOCFOCLiberado.Digitais;

		ConsultarNumeroCFOCFOCLiberado.carregarTabela(container, +$('.hdnPaginaUltima', container).val(), lista);
	},

	paginar: function () {
		var container = $(this).closest('fieldset');
		var lista = $('.btnBuscar', container).hasClass('bloco') ? ConsultarNumeroCFOCFOCLiberado.Blocos : ConsultarNumeroCFOCFOCLiberado.Digitais;

		ConsultarNumeroCFOCFOCLiberado.carregarTabela(container, +$(this).text(), lista);
	}
}
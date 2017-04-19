/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../mensagem.js" />,
/// <reference path="../../../jquery.ddl.js" 
/// <reference path="../../masterpage.js" />

Barragem = {
	settings: {
		urls: {
			criar: '',
			editar: '',
			mergiar: '',
			visualizar: '',
			criarBarragemItem: '',
			editarBarragemItem: '',
			visualizarBarragemItem: '',
			excluirBarragemItem: '',
			confirmExcluirBarragemItem: '',
			editarModalFinalidade: '',
			salvarFinalidade: '',
			idEmpreendimento: '',
		},
		salvarCallBack: null,
		mensagens: {},
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null,
		temARL: false,
		temARLDesconhecida: false,
	},
	finalidadeOutrosId: null,
	finalidadeReservacaoId: null,
	identificador: 1,
	trEmEdicao: null,
	container: null,
	

	load: function (container, options) {
	    
		if (options) { $.extend(Barragem.settings, options); }
		Barragem.container = MasterPage.getContent(container);
		
		Barragem.container.delegate('#linkCancelar', 'click', Barragem.onClickLinkCancelar);
		Barragem.container.delegate('.btnAddBarragemItemDados', 'click', Barragem.onClickAddBarragemItemDados);
		Barragem.container.delegate('.btnExcluirLinhaBarragemItem', 'click', Barragem.onClickRemoverTR);
		Barragem.container.delegate('.btnAddBarragem', 'click', Barragem.onClickAddBarragem);
		Barragem.container.delegate('.btnSalvar', 'click', Barragem.onClickSalvar);
		
		Barragem.container.delegate('.btnVisualizar', 'click', Barragem.onClickVisualizar);
		Barragem.container.delegate('.btnEditarPrincipal', 'click', Barragem.onClickEditar);
		Barragem.container.delegate('.btnExcluirItemBarragem', 'click', Barragem.onClickExcluirItemBarragem);
		Barragem.container.delegate('.btnEditarFinalidade', 'click', Barragem.editarFinalidade);

		Barragem.container.delegate('.CheckReservacao', 'click', Barragem.onChangeFinalidade);
		

		var editar = $('.hdnIsEditar', container).val();

		if (!editar) {
			CoordenadaAtividade.obterDadosTipoGeometria();
			CoordenadaAtividade.obterDadosCoordenadaAtividade();
		}
		CoordenadaAtividade.load(container, { isSetEvento: true });

		if (Barragem.settings.textoMerge) {
			Barragem.abrirModalRedireciona(Barragem.settings.textoMerge, Barragem.settings.atualizarDependenciasModalTitulo);
		} else {
			Barragem.abrirModalARL();
		}
	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', Barragem.container).attr('href'));
			},
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {

				MasterPage.carregando(true);

				var barragem = {
					Id: parseInt($('.hdnCaracterizacaoId', Barragem.container).val()),
					EmpreendimentoId: parseInt($('.hdnEmpreendimentoId', Barragem.container).val()),
					Barragens: []
				};

				$('.gridBarragens', Barragem.container).find('.hdnItemBarragem').each(function (i) {
					var item = JSON.parse($(this).val());
					barragem.Barragens.push(item);
				});

				$.ajax({
					url: Barragem.settings.urls.criar,
					data: JSON.stringify({ barragem: barragem, barragemItemIdEdicao: null }),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, Barragem.container);
					},
					success: function (response, textStatus, XMLHttpRequest) {

						if (response.EhValido) {
							Barragem.toggleBotoes(false);
						} else {
							if (response.Msg && response.Msg.length > 0) {
								Mensagem.gerar(Barragem.container, response.Msg);
							}
						}
					}
				});

				MasterPage.carregando(false);

				Modal.fechar(conteudoModal);

				Barragem.abrirModalARL();
			},
			conteudo: textoModal,
			titulo: titulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	abrirModalMerge: function (textoModal) {
		Modal.confirma({
			removerFechar: true,
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				Modal.fechar(conteudoModal);
				Barragem.abrirModalARL();
			},
			conteudo: textoModal,
			titulo: Barragem.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	abrirModalARL: function () {

		var fnModal = function (texto) {
			Modal.confirma({
				removerFechar: true,
				btnOkLabel: 'Confirmar',
				btnOkCallback: function (conteudoModal) {
					Modal.fechar(conteudoModal);
				},
				btCancelLabel: 'Cancelar',
				btnCancelCallback: function (conteudoModal) {
					MasterPage.redireciona($('.linkCancelar', Barragem.container).attr('href'));
				},
				conteudo: texto,
				titulo: 'Área de Reserva Legal',
				tamanhoModal: Modal.tamanhoModalMedia
			});
		};

		if (!Barragem.settings.temARL) {
			fnModal(Barragem.settings.mensagens.SemARLConfirm.Texto);
		} else if (Barragem.settings.temARLDesconhecida) {
			fnModal(Barragem.settings.mensagens.ARLDesconhecidaConfirm.Texto);
		}
	},

	onClickRemoverTR: function () {

		$(this).closest('tr').remove();
		var totalLamina = 0;
		var totalArmazenado = 0;

		$('.gridBarragemItemDados tbody tr', Barragem.container).each(function (i) {
			var hdnItemBarragemItemDados = $(this).find('.hdnItemBarragemItemDados');
			var spanIdentificador = $(this).find('.spanIdentificador');
			var item = JSON.parse(hdnItemBarragemItemDados.val());
			item.Identificador = i + 1;
			hdnItemBarragemItemDados.val($.toJSON(item));
			spanIdentificador.text(i + 1);
			totalLamina += Mascara.getFloatMask(item.LaminaAgua);
			totalArmazenado += Mascara.getFloatMask(item.VolumeArmazenamento);
		});
		Barragem.identificador--;
		$('.txtIdentificador', Barragem.container).val(Barragem.identificador);

		$('.txtTotalLaminaItem', Barragem.container).val(Mascara.getStringMask(totalLamina, 'n4'));
		$('.txtTotalArmazenadoItem', Barragem.container).val(Mascara.getStringMask(totalArmazenado, 'n4'));

		Listar.atualizarEstiloTable(Barragem.container.find('.dataGridTable'));

		$('.hdnModificacoesNaoSalvas').val('1');
	},

	toggleBotoes: function (flag) {
		if (flag) {
			$('.btnSalvar,.btnModalOu,#linkCancelar', Barragem.container).removeClass('hide');
			$('#linkVoltar', Barragem.container).addClass('hide');
		} else {
			$('.btnSalvar,.btnModalOu,#linkCancelar', Barragem.container).addClass('hide');
			$('#linkVoltar', Barragem.container).removeClass('hide');
		}
	},

	obterTipo: function (container) {
	    var tipo = $(container).closest('tr').find('.FinalidadeTexto').text();

	    return tipo;
	},

	gerarObjeto: function () {

		var barragem = {
			Id: parseInt($('.hdnCaracterizacaoId', Barragem.container).val()),
			EmpreendimentoId: parseInt($('.hdnEmpreendimentoId', Barragem.container).val()),
			Barragens: [],
		};

		$('.gridBarragens', Barragem.container).find('.hdnItemBarragem').each(function (i) {
			var item = JSON.parse($(this).val());
			var itemEmEdicao = null;
			if (Barragem.trEmEdicao && $('.hdnItemBarragem', Barragem.trEmEdicao).length > 0) {
				itemEmEdicao = JSON.parse($('.hdnItemBarragem', Barragem.trEmEdicao).val());

				if (itemEmEdicao.Id != item.Id) {
					barragem.Barragens.push(item);
				}
			} else {
				barragem.Barragens.push(item);
			}
		});

		var barragemItem = {
			Id: parseInt($('.hdnBarragemItemId', Barragem.container).val()),
			IdRelacionamento: parseInt($('.hdnBarragemItemId', Barragem.container).val()),
			Quantidade: parseInt($('.txtQuantidade', Barragem.container).val()),
			FinalidadeTexto: $('.ddlFinalidade', Barragem.container).filter(':selected').text(),
			CoordenadaAtividade: CoordenadaAtividade.obter(),
			BarragensDados: []
		};

		$('.gridBarragemItemDados', Barragem.container).find('.hdnItemBarragemItemDados').each(function (i) {
			var item = JSON.parse($(this).val());
			barragemItem.BarragensDados.push(item);
		});

		barragem.Barragens.push(barragemItem);

		return barragem;
	},

	onClickLinkCancelar: function () {
		$('.divBarragemItem', Barragem.container).empty();
		Barragem.toggleBotoes(false);
		Mensagem.limpar(Barragem.container);
	},

	onChangeFinalidade: function () {
	    var reservacao = $('.CheckReservacao').attr('checked');

		$('.divOutorga', Barragem.container).addClass('hide');

		if (reservacao == true) {
		    $('.divOutorga', Barragem.container).removeClass('hide');
		}
		$('.ddlOutorga', Barragem.container).val(0);
		$('.txtNumero', Barragem.container).val('');
	},

	onClickVisualizar: function () {

		MasterPage.carregando(true);

		$.ajax({
			url: Barragem.settings.urls.visualizarBarragemItem,
			data: JSON.stringify({
				id: JSON.parse($(this).closest('tr').find('.hdnItemBarragem').val()).Id,
				empreendimentoId: $('.hdnEmpreendimentoId', Barragem.container).val(),
				barragemId: $('.hdnCaracterizacaoId', Barragem.container).val()
			}),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Barragem.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					$('.divBarragemItem', Barragem.container).empty().html(response.Html);
					$('fieldset:last', '.divBarragemItem').addClass('boxBranca');
					Barragem.identificador = response.Identificador;
					$('.txtIdentificador', Barragem.container).val(Barragem.identificador);
					Barragem.toggleBotoes(true);
					Mascara.load('.divBarragemItem');
					MasterPage.load();
					Mensagem.limpar(Barragem.container);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Barragem.container, response.Msg);
				}
			}
		});

		$('.btnSalvar,.btnModalOu', Barragem.container).addClass('hide');

		MasterPage.carregando(false);
	},

	onClickEditar: function () {

		Barragem.trEmEdicao = $(this).closest('tr');

		MasterPage.carregando(true);
		
		$.ajax({
			url: Barragem.settings.urls.editarBarragemItem,
			data: JSON.stringify({
				id: JSON.parse($(this).closest('tr').find('.hdnItemBarragem').val()).Id,
				empreendimentoId: $('.hdnEmpreendimentoId', Barragem.container).val(),
				barragemId: $('.hdnCaracterizacaoId', Barragem.container).val()
			}),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Barragem.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
			    
				if (response.EhValido) {
					$('.divBarragemItem', Barragem.container).empty().html(response.Html);
					$('fieldset:last', '.divBarragemItem').addClass('boxBranca');
					Barragem.identificador = response.Identificador;
					$('.txtIdentificador', Barragem.container).val(Barragem.identificador);
					Barragem.toggleBotoes(true);
					Mascara.load('.divBarragemItem');
					MasterPage.load();
					Mensagem.limpar(Barragem.container);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Barragem.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},

	onClickAddBarragem: function () {
		Barragem.trEmEdicao = null;

		MasterPage.carregando(true);

		$.ajax({
			url: Barragem.settings.urls.criarBarragemItem,
			data: JSON.stringify({ empreendimentoId: $('.hdnEmpreendimentoId', Barragem.container).val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Barragem.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					$('.divBarragemItem', Barragem.container).empty().html(response.Html);
					$('fieldset:last', '.divBarragemItem').addClass('boxBranca');
					Barragem.identificador = 1;
					$('.txtIdentificador', Barragem.container).val(Barragem.identificador);
					Barragem.toggleBotoes(true);
					Mascara.load('.divBarragemItem');
					MasterPage.load();
					Mensagem.limpar(Barragem.container);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Barragem.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	},

	onClickAddBarragemItemDados: function () {
	    var idsFinalidades = [];
	    var txtFinalidades = [];
	    var FinalidadeTextoString = "";
	    $('#checkboxes input:checked').each(function () {
	        idsFinalidades.push($(this).attr('value'));
	        txtFinalidades.push($(this).attr('name'));
	        FinalidadeTextoString += ($(this).attr('name'));
	        FinalidadeTextoString += ", ";
	    });

		Mensagem.limpar(Barragem.container);
		var arrayMsg = [];
		var trTemplate = null;

		var txtIdentificador = $('.txtIdentificador', Barragem.container);
		var txtLaminaAgua = $('.txtLaminaAgua', Barragem.container);
		var txtArmazenado = $('.txtArmazenado', Barragem.container);
		var ddlOutorga = $('.ddlOutorga', Barragem.container);
		var txtNumero = $('.txtNumero', Barragem.container);
		var txtTotalLaminaItem = $('.txtTotalLaminaItem', Barragem.container);
		var txtTotalArmazenadoItem = $('.txtTotalArmazenadoItem', Barragem.container);

		var numeroAux = 0;
		var itemJaAdd = false;
		var Quantidade = $('.txtQuantidade', Barragem.container).val();
		var Identificador = Number($('.txtIdentificador', Barragem.container).val()) || 0;

		var barragemDadosItem = {
		    Identificador: Barragem.identificador,
		    FinalidadeTextos: JSON.stringify(txtFinalidades),
		    ListaIdsFinalidades: idsFinalidades,
			LaminaAgua: txtLaminaAgua.val(),
			VolumeArmazenamento: txtArmazenado.val(),
			OutorgaId: parseInt(ddlOutorga.val()),
			OutorgaTexto: ddlOutorga.find('option').filter(':selected').text(),
			Numero: txtNumero.val()
		};
        
		if (Quantidade == '') {
			arrayMsg.push(Barragem.settings.mensagens.InformeQuantidade);
			Mensagem.gerar(Barragem.container, arrayMsg);
			return
		} else {
			if (Quantidade <= 0) {
				arrayMsg.push(Barragem.settings.mensagens.InformeQuantidadeZero);
				Mensagem.gerar(Barragem.container, arrayMsg);
				return;
			} else {
				if (Quantidade < Identificador) {
					arrayMsg.push(Barragem.settings.mensagens.QuantidadeInvalida);
					Mensagem.gerar(Barragem.container, arrayMsg);
					return;
				}
			}
		}

		if (txtFinalidades.length == 0) {
		    arrayMsg.push(Barragem.settings.mensagens.SelecioneFinalidade);
		    Mensagem.gerar(Barragem.container, arrayMsg);
		    return;
		}

		numeroAux = Mascara.getFloatMask(barragemDadosItem.LaminaAgua);
		if (isNaN(numeroAux) || numeroAux == 0) {
		    arrayMsg.push(Barragem.settings.mensagens.InformeLamina);
		    Mensagem.gerar(Barragem.container, arrayMsg);
		    return;
		}
		numeroAux = Mascara.getFloatMask(barragemDadosItem.VolumeArmazenamento);
		if (isNaN(numeroAux) || numeroAux == 0) {
		    arrayMsg.push(Barragem.settings.mensagens.InformeArmazenado);
		    Mensagem.gerar(Barragem.container, arrayMsg);
		    return;
		}

		if (txtFinalidades.indexOf('Reservação') >= 0) {

			if (barragemDadosItem.Numero != '' && barragemDadosItem.OutorgaId == 0) {
			    arrayMsg.push(Barragem.settings.mensagens.SelecioneOutorga);
			    Mensagem.gerar(Barragem.container, arrayMsg);
			    return;
			}

			if (barragemDadosItem.OutorgaId != 0 && barragemDadosItem.Numero == '') {
			    arrayMsg.push(Barragem.settings.mensagens.InformeNumero);
			    Mensagem.gerar(Barragem.container, arrayMsg);
			    return;
			}
		} else {
			barragemDadosItem.OutorgaId = null;
			barragemDadosItem.OutorgaTexto = '';
			barragemDadosItem.Numero = '';
		}

		if (arrayMsg.length > 0) {
			Mensagem.gerar(Barragem.container, arrayMsg);
			return;
		} else {
			if (barragemDadosItem.OutorgaId == 0) {
				barragemDadosItem.OutorgaId = null;
				barragemDadosItem.OutorgaTexto = '';
			}
		}

		trTemplate = $('.trBarragemItemDadosTemplate', Barragem.container).clone().removeAttr('class');

		$('.spanIdentificador', trTemplate).text(barragemDadosItem.Identificador).attr('title', barragemDadosItem.Identificador);
		$('.spanFinalidade', trTemplate).text(FinalidadeTextoString).attr('title', barragemDadosItem.FinalidadeTextos);
		$('.spanLaminaAgua', trTemplate).text(barragemDadosItem.LaminaAgua).attr('title', barragemDadosItem.LaminaAgua);
		$('.spanVolumeArmazenamento', trTemplate).text(barragemDadosItem.VolumeArmazenamento).attr('title', barragemDadosItem.VolumeArmazenamento);
		$('.spanOutorgaTexto', trTemplate).text(barragemDadosItem.OutorgaTexto).attr('title', barragemDadosItem.OutorgaTexto);
		$('.spanNumero', trTemplate).text(barragemDadosItem.Numero).attr('title', barragemDadosItem.Numero);

		$('.hdnItemBarragemItemDados', trTemplate).val($.toJSON(barragemDadosItem));
		$('.gridBarragemItemDados tbody', Barragem.container).append(trTemplate);

		Barragem.identificador++;

		txtIdentificador.val(Barragem.identificador);
		txtLaminaAgua.val('');
		txtArmazenado.val('');
		ddlOutorga.val(0);
		txtNumero.val('');

		$('#checkboxes input:checked').each(function () {
		    $(this).removeAttr('checked');
		});

		numeroAux = Mascara.getFloatMask(txtTotalLaminaItem.val());
		numeroAux += Mascara.getFloatMask(barragemDadosItem.LaminaAgua);
		txtTotalLaminaItem.val(Mascara.getStringMask(numeroAux, 'n4'));

		numeroAux = Mascara.getFloatMask(txtTotalArmazenadoItem.val());
		numeroAux += Mascara.getFloatMask(barragemDadosItem.VolumeArmazenamento);
		txtTotalArmazenadoItem.val(Mascara.getStringMask(numeroAux, 'n4'));

		Mensagem.limpar(Barragem.container);

		Listar.atualizarEstiloTable(Barragem.container.find('.dataGridTable'));

		$('.hdnModificacoesNaoSalvas').val('1');
	},

	onClickSalvar: function () {
		MasterPage.carregando(true);

		$.ajax({
			url: Barragem.settings.urls.criar,
			data: JSON.stringify({ barragem: Barragem.gerarObjeto(), barragemItemIdEdicao: Barragem.trEmEdicao ? JSON.parse(Barragem.trEmEdicao.find('.hdnItemBarragem').val()).Id : null }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Barragem.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				var trTemplate = null;
				var barragemItem = null;
				if (response.TextoMerge) {
					Barragem.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {

					barragemItem = response.BarragemItem;

					if (Barragem.trEmEdicao) {
						trTemplate = Barragem.trEmEdicao;
					} else {
						trTemplate = $('.trBarragemTemplate', Barragem.container).clone().removeAttr('class');
					}

					$('.spanQuantidade', trTemplate).text(barragemItem.Quantidade).attr('title', barragemItem.Quantidade);
					$('.spanFinalidade', trTemplate).text(barragemItem.FinalidadeTexto).attr('title', barragemItem.FinalidadeTexto);
					$('.spanTotalLamina', trTemplate).text(Mascara.getStringMask(barragemItem.TotalLamina, 'n4')).attr('title', Mascara.getStringMask(barragemItem.TotalLamina, 'n4'));
					$('.spanTotalArmazenamento', trTemplate).text(Mascara.getStringMask(barragemItem.TotalArmazenado, 'n4')).attr('title', Mascara.getStringMask(barragemItem.TotalArmazenado, 'n4'));

					$('.hdnItemBarragem', trTemplate).val($.toJSON(barragemItem));

					$('.txtTotalLamina', Barragem.container).val(Mascara.getStringMask(response.TotalLamina, 'n4'));
					$('.txtTotalArmazenado', Barragem.container).val(Mascara.getStringMask(response.TotalArmazenado, 'n4'));

					$('.hdnCaracterizacaoId', Barragem.container).val(response.BarragemId);

					if (!Barragem.trEmEdicao) {
						$('.gridBarragens tbody', Barragem.container).append(trTemplate);
					}

					Mensagem.limpar(Barragem.container);

					$('.divBarragemItem', Barragem.container).empty();

					Barragem.toggleBotoes(false);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Barragem.container, response.Msg);
				} else {
				    Barragem.trEmEdicao = null;
				}
			}
		});

		//Barragem.trEmEdicao = null;
		MasterPage.carregando(false);

		$('.hdnModificacoesNaoSalvas').val('0');
	},

	editarFinalidade: function () {
	    arrayMsg = [];

	    var modificado = $('.hdnModificacoesNaoSalvas').val();
	    if (modificado == '1') {
	        arrayMsg.push(Barragem.settings.mensagens.ModificacoesNaoSalvas);
	        Mensagem.gerar(Barragem.container, arrayMsg);
	        return;
	    }

	    var id = $(this).closest('tr').find('.hdnItemId').val();

	    if (id == undefined) {
	        arrayMsg.push(Barragem.settings.mensagens.BarragemNaoSalva);
	        Mensagem.gerar(Barragem.container, arrayMsg);
	        return;
	    }

	    var idGeral = $(this).closest('.divBarragemItem').find('.hdnBarragemItemId').val();

	    Mensagem.limpar(Barragem.container);
	    
	    var settings = function (content) { 
	        Modal.defaultButtons(content, function () { 
	            Barragem.modalOrigem = content; 
	            Barragem.salvarEdicaoFinalidade(content); 
	        }, 'Salvar'); 
	    };
	    
	    Modal.abrir(Barragem.settings.urls.editarModalFinalidade + '?id=' + id + "&idGeral=" + idGeral, null, settings, Modal.tamanhoModalMedia, "Editar Finalidade");
	},

	salvarEdicaoFinalidade: function(modalContent){
	    var selected = [];
	    $('#checkboxes input:checked').each(function () {
	        selected.push($(this).attr('value'));
	    });

	    var id = $('#checkboxes').find('.ItemIDBarragem').attr('value');
	    var idGeral = $('#checkboxes').find('.ItemIDGeral').attr('value');

	    var idEmp = Barragem.settings.urls.idEmpreendimento;

	    MasterPage.carregando(true);

	    $.ajax({
	        url: Barragem.settings.urls.salvarFinalidade,
	        data: JSON.stringify({
	            idBarragem: id,
	            idBarragemGeral: idGeral,
	            idsFinalidades: selected,
	            idEmpreendimento: idEmp
	        }),
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

	            if (response.Msg && response.Msg.length > 0) {
	                Mensagem.gerar(Barragem.container, response.Msg);
	            }
	        }
	    });

	    Barragem.trEmEdicao = null;
	    MasterPage.carregando(false);
	    
	},

	onClickExcluirItemBarragem: function () {

		Mensagem.limpar(Barragem.container);

		var trExcluir = this;
		var itemJson = JSON.parse($(this).closest('tr').find('.hdnItemBarragem').val());

		Modal.confirma({
			btnOkLabel: "Excluir",
			url: Barragem.settings.urls.confirmExcluirBarragemItem + '/' + itemJson.Id,
			tamanhoModal: Modal.tamanhoModalMedia,
			btnOkCallback: function (modalContent) {
				$.ajax(
					{
						url: Barragem.settings.urls.excluirBarragemItem,
						data: { barragemItemId: itemJson.Id, barragemId: $('.hdnCaracterizacaoId', Barragem.container).val() },
						type: "POST",
						cache: false,
						async: false,
						error: function (XMLHttpRequest, textStatus, errorThrown) {
							Aux.error(XMLHttpRequest, textStatus, errorThrown, MasterPage.getContent(modalContent));
						},
						success: function (response, textStatus, XMLHttpRequest) {
							if (response.EhValido) {
								$(trExcluir).closest('tr').remove();
								Modal.fechar(modalContent);
								$('.txtTotalLamina', Barragem.container).val(Mascara.getStringMask(response.TotalLamina, 'n4'));
								$('.txtTotalArmazenado', Barragem.container).val(Mascara.getStringMask(response.TotalArmazenado, 'n4'));
								$('.divBarragemItem', Barragem.container).empty();
								Barragem.toggleBotoes(false);
								if (response.Msg && response.Msg.length > 0) {
									Mensagem.gerar(Barragem.container, response.Msg);
								}
							}

							if (response.Msg && response.Msg.length > 0) {
								Mensagem.gerar(modalContent.find('.modalContent'), response.Msg);
							}
						}
					});
			}
		});
	}
};
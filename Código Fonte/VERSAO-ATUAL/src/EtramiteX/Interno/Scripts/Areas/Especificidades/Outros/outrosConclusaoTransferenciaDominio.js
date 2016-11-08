/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

OutrosConclusaoTransferenciaDominio = {
	settings: {
		urls: {
			carregarListas:''
		},
		mensagens: {
			Destinatarios: null,
			Interessados: null,
			Resposaveis: null
		},
	},
	container: null,

	load: function (especificidadeRef) {

	    OutrosConclusaoTransferenciaDominio.container = especificidadeRef;
	    AtividadeEspecificidade.load(especificidadeRef);

	    especificidadeRef.delegate('.btnAdd', 'click', OutrosConclusaoTransferenciaDominio.onAddDestinatariosTitulo);
	    especificidadeRef.delegate('.btnExcluirDestinatario', 'click', OutrosConclusaoTransferenciaDominio.onExcluirGrid);
	    especificidadeRef.delegate('.btnLimparNumero', 'click', function () { alert('oi') });
	},

	callBackProtocolo: function (protocolo) {
	    if (protocolo == null) {
	        $('.ddl', OutrosConclusaoTransferenciaDominio.container).ddlClear();
	        $('.dataGridTable tbody tr:not(.trTemplateRow)', OutrosConclusaoTransferenciaDominio.container).empty();
	        return;
	    }

	    $.ajax({
			url: OutrosConclusaoTransferenciaDominio.settings.urls.carregarListas,
			data: JSON.stringify({ protocoloId: protocolo.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(OutrosConclusaoTransferenciaDominio.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', OutrosConclusaoTransferenciaDominio.container).ddlLoad(response.Destinatarios);
					$('.ddlDestinatarios', OutrosConclusaoTransferenciaDominio.container).removeAttr('disabled');
					$('.ddlDestinatarios', OutrosConclusaoTransferenciaDominio.container).removeClass('disabled');

					$('.ddlResponsaveis', OutrosConclusaoTransferenciaDominio.container).ddlLoad(response.Responsaveis);
					$('.ddlResponsaveis', OutrosConclusaoTransferenciaDominio.container).removeAttr('disabled');
					$('.ddlResponsaveis', OutrosConclusaoTransferenciaDominio.container).removeClass('disabled');

					$('.ddlInteressados', OutrosConclusaoTransferenciaDominio.container).ddlLoad(response.Interessados);
					$('.ddlInteressados', OutrosConclusaoTransferenciaDominio.container).removeAttr('disabled');
					$('.ddlInteressados', OutrosConclusaoTransferenciaDominio.container).removeClass('disabled');
				}
			}
		});
	},

	onAddDestinatariosTitulo: function () {
		
		var ddl = $(this).closest('.divLista').find('.ddl');
		var table = $(this).closest('.divLista').find('.dataGridTable');
		var id = parseInt($(ddl, OutrosConclusaoTransferenciaDominio.container).val()) || 0;
		var texto = $('option:selected', ddl).text();
		var ehValido = true;
		var msg = $(this).closest('.divLista').find('.hdnMsg');
		if (id <= 0) {
			Mensagem.gerar(MasterPage.getContent(OutrosConclusaoTransferenciaDominio.container), new Array(JSON.parse($(msg).val()).msgs[0]));
			return;
		}

		if (ehValido) {
			Mensagem.limpar(OutrosConclusaoTransferenciaDominio.container);
			var tabela = $(table, OutrosConclusaoTransferenciaDominio.container);
			var linha = $('.trTemplateRow', tabela).clone()
								.removeClass('trTemplateRow')
								.removeClass('hide');
			var adicionar = true;

			tabela.find('tbody tr').each(function () {
				if ($(this).find('.hdnId').val() == id) {
					adicionar = false;
					Mensagem.gerar(MasterPage.getContent(OutrosConclusaoTransferenciaDominio.container), new Array(JSON.parse($(msg).val()).msgs[1]));
					return;
				}
			});

			if (table.hasClass('dgInteressados')) {
				var dgResponsaveis = $('.dgResponsaveis', OutrosConclusaoTransferenciaDominio.container);
                dgResponsaveis.find('tbody tr').each(function (i, linha) {
					if ($(linha).find('.hdnId').val() == id) {
						adicionar = false;
						Mensagem.gerar(MasterPage.getContent(OutrosConclusaoTransferenciaDominio.container), new Array(JSON.parse($(msg).val()).msgs[2]));
						return;
					}
				});
			}

			if (table.hasClass('dgResponsaveis')) {
			    
			    var dgInteressados = $('.dgInteressados', OutrosConclusaoTransferenciaDominio.container);
                dgInteressados.find('tbody tr').each(function (i, linha) {
			        if ($(linha).find('.hdnId').val() == id) {
			            adicionar = false;
			            Mensagem.gerar(MasterPage.getContent(OutrosConclusaoTransferenciaDominio.container), new Array(JSON.parse($(msg).val()).msgs[2]));
			            return;
			        }
			    });
			}

			if (adicionar) {
				linha.find('.hdnId').val(id);
				linha.find('.hdnJSon').val(JSON.stringify({ Id: id, Nome: texto }));
				linha.find('.Pessoa').html(texto).attr('title', texto);
				tabela.find('tbody:last').append(linha);
				$(ddl).val('0'); 
				
				Listar.atualizarEstiloTable(tabela);
                
			}
		}
	},

	onExcluirGrid: function () {
		
		var container = $(this).closest('.divLista').find('.dataGridTable');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	},

	obterObjeto: function () {
		var destinatarios = new Array();
		var interessados = new Array();
		var responsaveis = new Array();

        
		$('.dgDestinatarios', OutrosConclusaoTransferenciaDominio.container).find('tbody tr:not(.trTemplateRow)').each(function (i, linha) {
		    
		    if ($(linha).find('.hdnJSon').val()) {
		        destinatarios.push(JSON.parse($(linha).find('.hdnJSon').val()));
		    }
		});
		$('.dgInteressados', OutrosConclusaoTransferenciaDominio.container).find('tbody tr:not(.trTemplateRow)').each(function (i, linha) {
		    if ($(linha).find('.hdnJSon').val()) {
		        interessados.push(JSON.parse($(linha).find('.hdnJSon').val()));
		    }
		});
		$('.dgResponsaveis', OutrosConclusaoTransferenciaDominio.container).find('tbody tr:not(.trTemplateRow)').each(function (i, linha) {
		    if ($(linha).find('.hdnJSon').val()) {
		        responsaveis.push(JSON.parse($(linha).find('.hdnJSon').val()));
		    }
		});
		return {
			Destinatarios: destinatarios,
			Interessados: interessados,
			Responsaveis: responsaveis
		};
	}
};

Titulo.settings.especificidadeLoadCallback = OutrosConclusaoTransferenciaDominio.load;
Titulo.settings.obterEspecificidadeObjetoFunc = OutrosConclusaoTransferenciaDominio.obterObjeto;
Titulo.addCallbackProtocolo(OutrosConclusaoTransferenciaDominio.callBackProtocolo);
/// <reference path="../masterpage.js" />
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />

TituloModelo = {
	urlEditar: '',
	urlObterAssinantes: null,
	urlObterUltimoNumero: null,
	urlObterTextoPadraoEmail: null,
	urlEnviarArquivo: null,
	urlVisualizarPdf: '',
	urlArquivoModelo: '',
	urlVerificarPublicoExternoAtividade: null,
	TiposArquivo: [],
	container: null,
	Mensagens: null,

	load: function (container) {
		TituloModelo.container = MasterPage.getContent(container);

		container.delegate('.radPossuiPrazoS', 'change', TituloModelo.onMostrarDivPossuiPrazo);
		container.delegate('.radPossuiPrazoN', 'change', TituloModelo.onMostrarDivPossuiPrazo);

		container.delegate('.radPassivelRenovacaoS', 'change', TituloModelo.onMostrarDivRenovacao);
		container.delegate('.radPassivelRenovacaoN', 'change', TituloModelo.onMostrarDivRenovacao);

		container.delegate('.radFaseAnteriorS', 'change', TituloModelo.onMostrarDivFaseAnterior);
		container.delegate('.radFaseAnteriorN', 'change', TituloModelo.onMostrarDivFaseAnterior);

		container.delegate('.radEnviarEmailS', 'change', TituloModelo.onMostrarDivEmail);
		container.delegate('.radEnviarEmailN', 'change', TituloModelo.onMostrarDivEmail);

		container.delegate('.ddlPeriodoRenovacao', 'change', TituloModelo.onMostrarDivDias);

		container.delegate('.btnAddSetor', 'click', TituloModelo.onAddSetor);
		container.delegate('.btnExcluirSetor', 'click', TituloModelo.onExcluirSetor);

		container.delegate('.btnAddAssinante', 'click', TituloModelo.onAddAssinante);
		container.delegate('.btnExcluirAssinante', 'click', TituloModelo.onExcluirAssinante);

		container.delegate('.btnAddModelo', 'click', TituloModelo.onAddModelo);
		container.delegate('.btnExcluirModelo', 'click', TituloModelo.onExcluirModelo);

		container.delegate('.btnAddArq', 'click', TituloModelo.onEnviarArquivoClick);
		container.delegate('.btnLimparArq', 'click', TituloModelo.onLimparArquivoClick);

		container.delegate('.btnEmailOriginal', 'click', TituloModelo.onObterTextoPadraEmail);
		container.delegate('.btnModeloTituloSalvar', 'click', TituloModelo.onSalvarModelo);

		container.delegate('.radPublicoExternoN', 'click', TituloModelo.onVerificarPublicoExternoAtividade);

		container.delegate('.titFiltros', 'click', TituloModelo.expandirFiltro);

		container.delegate('.radCredenciadoEmite', 'change', TituloModelo.onMostrarHabilitarSinc);
		container.delegate('.radCredenciadoEmiteN', 'change', TituloModelo.onMostrarDesabilitarSinc);

		container.delegate('.ddlTipoDoc', 'change', TituloModelo.onChangeTipoDoc);

		$('.txtSubtipo').focus();

		TituloModelo.onConfigurarTela();
	},

	onChangeTipoDoc: function () {
	    $('.ddlTipoProtocolo', TituloModelo.container).val('');
	    TituloModelo.tratarTipoProtocolo($(this).val());
	},

	tratarTipoProtocolo: function (tipoDocumento) {
	    switch (tipoDocumento) {
	        case "1":
	            $('.ddlTipoProtocolo option[value="4"]', TituloModelo.container).attr('disabled', 'disabled');
	            break;
	        case "2":
	            $('.ddlTipoProtocolo option[value="4"]', TituloModelo.container).removeAttr('disabled');
	            break;
	    }
	},

	onMostrarHabilitarSinc: function () {
	    $('.radNumeroSincronizado, .radNumeroSincronizadoN', TituloModelo.container).removeAttr('disabled');
	},

	onMostrarDesabilitarSinc: function () {
	    $('.radNumeroSincronizado', TituloModelo.container).removeAttr('checked', 'checked');
	    $('.radNumeroSincronizadoN', TituloModelo.container).attr('checked', 'checked');
	    $('.radNumeroSincronizado, .radNumeroSincronizadoN', TituloModelo.container).attr('disabled', 'disabled');
	},

	onVerificarPublicoExternoAtividade: function () {
		$('.radPublicoExternoN', TituloModelo.container).removeAttr('checked', 'checked');

		$.ajax({ url: TituloModelo.urlVerificarPublicoExternoAtividade,
			data: JSON.stringify({ id: $('.hdnEditarModeloId', TituloModelo.container).val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, TituloModelo.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (!response.EhValido) {
					$('.radPublicoExterno').attr('checked', 'checked');
					Mensagem.gerar(MasterPage.getContent(TituloModelo.container), response.Msg);
				} else {
					$('.radPublicoExternoN').attr('checked', 'checked');
				}
			}
		});
	},

	expandirFiltro: function () {

		var container = $(this).closest('fieldset');

		$('.titFiltros', container).toggleClass('fAberto');

		if ($('.titFiltro', container).parent().find('.fixado').length == 0) {
			if ($('.filtroCorpo', container).is(':animated')) {
				$('.filtroCorpo', container).stop(true, true);
				$('.titFiltros', container).toggleClass('fAberto');
			} else {
				$('.filtroCorpo', container).slideToggle('normal');
			}
		} else {
			if ($('.filtroCorpo > div', container).children().not('.fixado').is(':animated')) {
				$('.filtroCorpo > div', container).children().not('.fixado').stop(true, true);
				$('.titFiltros', container).toggleClass('fAberto');
			} else {
				$('.titFiltros > div', container).children().not('.fixado').slideToggle('normal');
			}
		}
	},

	onConfigurarTela: function () {
		TituloModelo.onMostrarDivPossuiPrazo();
		TituloModelo.onMostrarDivRenovacao();
		TituloModelo.onMostrarDivEmail();
		TituloModelo.onMostrarDivDias();
		TituloModelo.onMostrarDivFaseAnterior();
		TituloModelo.tratarTipoProtocolo($('.ddlTipoDoc', TituloModelo.container).val());
	},

	onMostrarDivPossuiPrazo: function () {
		if ($('.radPossuiPrazoS', TituloModelo.container).attr('checked')) {
			$('.divPossuiPrazo', TituloModelo.container).show();
		} else {
			$('.divPossuiPrazo', TituloModelo.container).hide();
		}
	},

	onMostrarDivRenovacao: function () {
		if ($('.radPassivelRenovacaoS', TituloModelo.container).attr('checked')) {
			$('.divPeriodoRenovacao', TituloModelo.container).show();
		} else {
			$('.divPeriodoRenovacao', TituloModelo.container).hide();
		}
	},

	onMostrarDivEmail: function () {
		if ($('.radEnviarEmailS', TituloModelo.container).attr('checked')) {
			$('.divEnviarEmail', TituloModelo.container).show();

			if ($('.txtEmail', TituloModelo.container).val() == '') {
				TituloModelo.onObterTextoPadraEmail();
			}
		} else {
			$('.divEnviarEmail', TituloModelo.container).hide();
		}
	},

	onMostrarDivDias: function () {
		if ($('.ddlPeriodoRenovacao', TituloModelo.container).val() == '2') {
			$('.divDiasPeriodoRenovacao', TituloModelo.container).show();
		} else {
			$('.divDiasPeriodoRenovacao', TituloModelo.container).hide();
		}
	},

	onMostrarDivFaseAnterior: function () {
		if ($('.radFaseAnteriorS', TituloModelo.container).attr('checked')) {
			$('.divFaseAnteriror', TituloModelo.container).show();
		} else {
			$('.divFaseAnteriror', TituloModelo.container).hide();
		}
		MasterPage.redimensionar();
	},

	onAddSetor: function () {
		var valor = $('.ddlSetor :selected', TituloModelo.container).val();

		if (valor == '0') {
			Mensagem.gerar(MasterPage.getContent(TituloModelo.container), new Array(TituloModelo.Mensagens.SelecioneSetor));
			return;
		}

		var msg = new Array();

		$('.tabSetores tbody tr').each(function () {
			if ($('.hdnSetorId', this).val() == valor) {
				msg.push(TituloModelo.Mensagens.ExisteSetor);
			}
		});

		if (msg.length > 0) {
			Mensagem.gerar(MasterPage.getContent(TituloModelo.container), msg);
			return;
		}

		var tr = $('.trItemTemplateSetor').clone();
		var indice = $('.tabSetores tbody tr').length + 1;
		tr.removeClass('trItemTemplateSetor');

		tr.find('.hdnSetorId').val(valor);
		tr.find('.setorTexto').text($('.ddlSetor :selected').text());
		tr.find('.setorTexto').attr('title', $('.ddlSetor :selected').text());

		tr.find('.hierarquiaCab').text($('.txtHierarquiaCab').val());
		tr.find('.hierarquiaCab').attr('title', $('.txtHierarquiaCab').val());

		tr.addClass((indice % 2) === 0 ? 'par' : 'impar');

		$('.tabSetores > tbody:last').append(tr);

		$('.tabSetores', TituloModelo.container).show();
		$('.txtHierarquiaCab', TituloModelo.container).val('');
		
		Listar.atualizarEstiloTable($('.tabSetores'));
	},

	onExcluirSetor: function () {
		if ($(this).closest('tbody').find('tr').length == 1) {
			$('.tabSetores').hide();
		}

		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable($('.tabSetores'));
	},

	onAddAssinante: function () {
		var ddlSetor = $('.ddlSetorAssinante :selected', TituloModelo.container);
		var ddlAss = $('.ddlAssinante :selected', TituloModelo.container);

		if (ddlSetor.val() == '0' || ddlAss.val() == '0') {
			Mensagem.gerar(MasterPage.getContent(TituloModelo.container), new Array(TituloModelo.Mensagens.SelecioneSetorAssinante));
			return;
		}

		var setorId = ddlSetor.val();
		var assinanteId = ddlAss.val();

		var msg = new Array();

		$('.tabAssinantes tbody tr', TituloModelo.container).each(function () {
			if ($('.hdnSetorId', this).val() == setorId) {
				msg.push(TituloModelo.Mensagens.ExisteAssinante);
			}
		});

		if (msg.length > 0) {
			Mensagem.gerar(MasterPage.getContent(TituloModelo.container), msg);
			return;
		}

		var indice = $('.tabAssinantes tbody tr', TituloModelo.container).length + 1;

		var tr = $('.trItemTemplateAssinante', TituloModelo.container).clone();

		tr.removeClass('trItemTemplateAssinante');

		tr.find('.hdnSetorId').val(setorId);
		tr.find('.setorTexto').text(ddlSetor.text());
		tr.find('.setorTexto').attr('title', ddlSetor.text());

		tr.find('.hdnAssinanteTipoId').val(assinanteId);

		tr.find('.assinanteTexto').text(ddlAss.text());
		tr.find('.assinanteTexto').attr('title', ddlAss.text());

		tr.addClass((indice % 2) === 0 ? 'par' : 'impar');

		$('.tabAssinantes > tbody:last', TituloModelo.container).append(tr);
		Listar.atualizarEstiloTable($('.tabAssinantes'));
	},

	onExcluirAssinante: function () {
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable($('.tabAssinantes'));
	},

	onAddModelo: function () {
		var ddlModelo = $('.ddlModelo :selected', TituloModelo.container);
		var msg = new Array();

		if (ddlModelo.val() == '0') {
			Mensagem.gerar(MasterPage.getContent(TituloModelo.container), new Array(TituloModelo.Mensagens.SelecioneModelo));
			return;
		}

		var modeloId = ddlModelo.val();

		$('.tabModelos tbody tr').each(function () {
			if ($('.hdnModeloId', this).val() == modeloId) {
				msg.push(TituloModelo.Mensagens.ExisteModeloTitulo);
			}
		});

		if (msg.length > 0) {
			Mensagem.gerar(MasterPage.getContent(TituloModelo.container), msg);
			return;
		}

		var indice = $('.tabModelos tbody tr').length + 1;

		var tr = $('.trItemTemplateTitulo').clone();

		tr.removeClass('trItemTemplateTitulo');

		tr.find('.hdnModeloId').val(modeloId);
		tr.find('.modeloTexto').text(ddlModelo.text());

		tr.addClass((indice % 2) === 0 ? 'par' : 'impar');

		$('.tabModelos > tbody:last').append(tr);
		$('.tabModelos').show();
		$('.ddlModelo').val(0);
		Listar.atualizarEstiloTable($('.tabModelos'));
	},

	onExcluirModelo: function () {
		if ($(this).closest('tbody').find('tr').length == 1) {
			$('.tabModelos', TituloModelo.container).hide();
		}

		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable($('.tabModelos', TituloModelo.container));
	},

	onEnviarArquivoClick: function () {
		var nomeArquivo = $('.inputFile', TituloModelo.container).val();

		erroMsg = new Array();

		if (nomeArquivo == '') {
			erroMsg.push(TituloModelo.Mensagens.ArquivoObrigatorio);
		} else {
			var tam = nomeArquivo.length - 4;
			if (!TituloModelo.validarTipoArquivo(nomeArquivo.toLowerCase().substr(tam))) {
				erroMsg.push(TituloModelo.Mensagens.ArquivoNaoEhDoc);
			}
		}

		if (erroMsg.length > 0) {
			Mensagem.gerar(TituloModelo.container, erroMsg);
			return;
		}

		MasterPage.carregando(true);
		var inputFile = $('.inputFile', TituloModelo.container);
		FileUpload.upload(TituloModelo.urlEnviarArquivo, inputFile, TituloModelo.callBackArqEnviado);
	},

	validarTipoArquivo: function (tipo) {

		var tipoValido = false;
		$(TituloModelo.TiposArquivo).each(function (i, tipoItem) {
			if (tipoItem == tipo) {
				tipoValido = true;
			}
		});

		return tipoValido;
	},

	callBackArqEnviado: function (controle, retorno, isHtml) {
		var ret = eval('(' + retorno + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoNome', TituloModelo.container).text(ret.Arquivo.Nome);
			$('.hdnArquivoJson', TituloModelo.container).val(JSON.stringify(ret.Arquivo));
			$('.txtArquivoNome', TituloModelo.container).attr('href', '/Arquivo/BaixarTemporario?nomeTemporario=' + ret.Arquivo.TemporarioNome + '&contentType=' + ret.Arquivo.ContentType);

			$('.spanInputFile', TituloModelo.container).addClass('hide');
			$('.txtArquivoNome', TituloModelo.container).removeClass('hide');

			$('.btnAddArq', TituloModelo.container).addClass('hide');
			$('.btnLimparArq', TituloModelo.container).removeClass('hide');
		} else {
			TituloModelo.onLimparArquivoClick();
			Mensagem.gerar(TituloModelo.container, ret.Msg);
		}
		MasterPage.carregando(false);
	},

	onLimparArquivoClick: function () {
		$('.hdnArquivoJson', TituloModelo.container).val('');
		$('.inputFile', TituloModelo.container).val('');

		$('.spanInputFile', TituloModelo.container).removeClass('hide');
		$('.txtArquivoNome', TituloModelo.container).addClass('hide');

		$('.btnAddArq', TituloModelo.container).removeClass('hide');
		$('.btnLimparArq', TituloModelo.container).addClass('hide');
	},

	onObterTextoPadraEmail: function () {

		MasterPage.carregando(true);
		var modeloId = $('.hdnEditarModeloId', TituloModelo.container).val();

		$.ajax({ url: TituloModelo.urlObterTextoPadraoEmail,
			data: JSON.stringify({ id: modeloId }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, TituloModelo.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(TituloModelo.container), response.Msg);
					return;
				}
				$('.txtEmail', TituloModelo.container).val(response.TextoPadrao);
			}
		});
		MasterPage.carregando(false);
	},

	onCriarObjeto: function () {

		var modelo = {};

		function Regra() {
			this.Id = 0;
			this.Valor = true;
			this.Tipo = 0;
			this.Respostas = [];
		}

		modelo.Setores = [];
		modelo.Assinantes = [];
		modelo.Modelos = [];
		modelo.Regras = [];

		modelo.Id = $('.hdnEditarModeloId', TituloModelo.container).val();
		modelo.SubTipo = $('.txtSubtipo', TituloModelo.container).val();
		modelo.Nome = $('.txtNome', TituloModelo.container).val();
		modelo.Sigla = $('.txtSigla', TituloModelo.container).val();
		modelo.TipoProtocolo = $('.ddlTipoProtocolo', TituloModelo.container).val();
		modelo.Tipo = $('.ddlTipo', TituloModelo.container).val();
		modelo.TipoDocumento = $('.ddlTipoDoc', TituloModelo.container).val();

		var regra = new Regra();
		regra.Id = $('.hdnNumeracaoAutoId', TituloModelo.container).val();
		regra.Valor = $('.radNumeracaoAutomaticaS').attr('checked');
		regra.Tipo = 1;
		modelo.Regras.push(regra);

		regra = new Regra();
		regra.Id = $('.hdnPrazoId', TituloModelo.container).val();
		regra.Valor = $('.radPossuiPrazoS').attr('checked');
		regra.Tipo = 4;
		regra.Respostas.push({ Id: $('.hdnInicioPrazoId', TituloModelo.container).val(), Valor: $('.ddlInicioPrazo', TituloModelo.container).val(), Tipo: 4 });
		regra.Respostas.push({ Id: $('.hdnTipoPrazoId', TituloModelo.container).val(), Valor: $('.ddlTipoPrazo', TituloModelo.container).val(), Tipo: 5 });
		modelo.Regras.push(regra);

		regra = new Regra();
		regra.Id = $('.hdnRenovacaoId', TituloModelo.container).val();
		regra.Valor = $('.radPassivelRenovacaoS', TituloModelo.container).attr('checked');
		regra.Tipo = 6;
		modelo.Regras.push(regra);

		regra = new Regra();
		regra.Id = $('.hdnEnviarEmailId', TituloModelo.container).val();
		regra.Valor = $('.radEnviarEmailS', TituloModelo.container).attr('checked');
		regra.Tipo = 7;
		regra.Respostas.push({ Id: $('.hdnTextoEmailId', TituloModelo.container).val(), Valor: $('.txtEmail', TituloModelo.container).val(), Tipo: 6 });
		modelo.Regras.push(regra);


		modelo.Regras.push({ Id: $('.hdnNumeracaoReiniciadaId', TituloModelo.container).val(), Valor: $('.radNumeracaoReiniciadaAno').attr('checked'), Tipo: 2 });
		modelo.Regras.push({ Id: $('.hdnProtocoloObrId', TituloModelo.container).val(), Valor: $('.radProtocoloObrigatorio').attr('checked'), Tipo: 3 });
		modelo.Regras.push({ Id: $('.hdnCondicionanteId', TituloModelo.container).val(), Valor: $('.radPossuiCondicionantes').attr('checked'), Tipo: 5 });
		modelo.Regras.push({ Id: $('.hdnPublicoExternoId', TituloModelo.container).val(), Valor: $('.radPublicoExterno').attr('checked'), Tipo: 8 });
		modelo.Regras.push({ Id: $('.hdnPdfGeradoSistemaId', TituloModelo.container).val(), Valor: $('.radPdfGeradoSistema').attr('checked'), Tipo: 11 });

		modelo.Regras.push({ Id: $('.hdnCredenciadoEmiteId', TituloModelo.container).val(), Valor: $('.radCredenciadoEmite').attr('checked'), Tipo: 12 });
		modelo.Regras.push({ Id: $('.hdnNumeroSincronizadoId', TituloModelo.container).val(), Valor: $('.radNumeroSincronizado').attr('checked'), Tipo: 13 });

		if ($('.radEnviarEmailS', TituloModelo.container).attr('checked')) {
			modelo.Regras.push({ Id: $('.hdnAnexarPDFId', TituloModelo.container).val(), Valor: $('.radAnexarPDFS').attr('checked'), Tipo: 9 });
		}

		regra = new Regra();
		regra.Id = $('.hdnFaseAnteriorId', TituloModelo.container).val();
		regra.Valor = $('.radFaseAnteriorS', TituloModelo.container).attr('checked');
		regra.Tipo = 10;

		regra.Respostas.push({ Id: $('.hdnTituloAnteriroObrigatorioId').val(), Valor: $('.radTituloAnteriroObr').attr('checked'), Tipo: 8 });

		$('.tabModelos tbody tr', TituloModelo.container).each(function () {
			regra.Respostas.push({ Valor: $('.hdnModeloId', this).val(), Id: $('.hdnModeloIdRelacionamento', this).val(), Tipo: 7 });
		});

		modelo.Regras.push(regra);

		if ($('.radPdfGeradoSistema', TituloModelo.container).attr('checked')) {
			modelo.Arquivo = $.parseJSON($('.hdnArquivoJson', TituloModelo.container).val());
		}

		$('.tabSetores tbody tr', TituloModelo.container).each(function () {
			modelo.Setores.push({ Id: $('.hdnSetorId', this).val(), IdRelacao: $('.hdnSetorIdRelacionamento', this).val(), HierarquiaCabecalho: $('.hierarquiaCab', this).text() });
		});

		$('.tabAssinantes tbody tr', TituloModelo.container).each(function () {
			modelo.Assinantes.push({ Id: $('.hdnAssinanteId', this).val(), IdRelacionamento: $('.hdnAssinanteIdRelacionamento', this).val(), TipoId: $('.hdnAssinanteTipoId', this).val(), SetorId: $('.hdnSetorId', this).val() });
		});

		return { Modelo: modelo };
	},

	onSalvarModelo: function () {
		var objeto = TituloModelo.onCriarObjeto();

		$.ajax({ url: TituloModelo.urlEditar,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, TituloModelo.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedireciona);
				} else {
					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(MasterPage.getContent(TituloModelo.container), response.Msg);
					}
				}
			}
		});
	}
}
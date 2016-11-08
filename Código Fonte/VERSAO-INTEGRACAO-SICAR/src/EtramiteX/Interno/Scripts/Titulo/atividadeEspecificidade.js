/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />

AtividadeEspecificidade = {
	settings: {
		urls: {
			obterProcessosDocumentos: '',
			obterAtividades: '',
			pdfRequerimento: ''
		},
		afterChangeProcDoc: null,
		Mensagens: null
	},
	container: null,
	protocolo: null,

	load: function (especificidadeRef, options) {
		if (options) {
			$.extend(AtividadeEspecificidade.settings, options);
		}

		AtividadeEspecificidade.container = especificidadeRef;

		$('.btnAdicionarAtividade', especificidadeRef).click(AtividadeEspecificidade.onAdicionarAtividade);
		$('.btnPdfRequerimento', especificidadeRef).click(AtividadeEspecificidade.onAbrirPdfRequerimento);
		$('.ddlProcessosDocumentos', especificidadeRef).change(AtividadeEspecificidade.onDdlProcessosDocumentos);

		especificidadeRef.delegate('.btnExcluirAtividade', 'click', AtividadeEspecificidade.onExcluirLinha);

		if ($('.ddlProcessosDocumentos option', AtividadeEspecificidade.container).length <= 1) {
			$('.ddlProcessosDocumentos', AtividadeEspecificidade.container).attr('disabled', 'disabled').addClass('disabled');
		}

		if ($('.ddlAtividadeSolicitada option', AtividadeEspecificidade.container).length <= 1) {
			$('.ddlAtividadeSolicitada', AtividadeEspecificidade.container).attr('disabled', 'disabled').addClass('disabled');
		}

		AtividadeEspecificidade.container.delegate('.ddlAtividadeSolicitada', 'change', AtividadeEspecificidade.fnGerenciarAtividadeCaractericazao);

		Listar.atualizarEstiloTable(AtividadeEspecificidade.container.find('.dataGridTable'));
		
		AtividadeEspecificidade.fnGerenciarAtividadeCaractericazao();

		Titulo.container.delegate('.btnLimparNumero', 'click', AtividadeEspecificidade.fnLimparAtividadeCaracterizacao);
	},

	onChangeProtocolo: function (protocolo) {
		AtividadeEspecificidade.protocolo = protocolo;

		//quando é o change de empreendimento
		if (protocolo == null || protocolo.Id <= 0) {
			$('.ddlProcessosDocumentos', AtividadeEspecificidade.container).ddlLoad([]);
			$('.ddlAtividadeSolicitada', AtividadeEspecificidade.container).ddlLoad([]);
			$('.dgAtividadesSolicitadas tbody tr', AtividadeEspecificidade.container).remove();
			return;
		}

		if (AtividadeEspecificidade.settings.urls.obterProcessosDocumentos) {
			//Obter Processos/Documentos
			$.ajax({ url: AtividadeEspecificidade.settings.urls.obterProcessosDocumentos,
				data: JSON.stringify(protocolo),
				cache: false,
				async: false,
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(AtividadeEspecificidade.container));
				},
				success: function (data, textStatus, XMLHttpRequest) {

					if (data.Msg && data.Msg.length > 0) {
						Mensagem.gerar(MasterPage.getContent(AtividadeEspecificidade.container), data.Msg, true);
						return;
					}

					$(".ddlProcessosDocumentos", AtividadeEspecificidade.container).ddlLoad(data.Valores);
					AtividadeEspecificidade.onDdlProcessosDocumentos();
				}
			});
			return;
		}

		if (AtividadeEspecificidade.settings.afterChangeProcDoc) {
			AtividadeEspecificidade.settings.afterChangeProcDoc(Protocolo);
		}
	},

	onDdlProcessosDocumentos: function (dropDowA) {
		var ddlA = $(".ddlProcessosDocumentos", AtividadeEspecificidade.container);
		var ddlB = $('.ddlAtividadeSolicitada', AtividadeEspecificidade.container);

		var protocolo = AtividadeEspecificidade.obterProtocoloDdlSelecionado();
		ddlA.ddlCascate(ddlB, { url: AtividadeEspecificidade.settings.urls.obterAtividades, data: protocolo });
		$('.dgAtividadesSolicitadas tbody tr', AtividadeEspecificidade.container).remove();

		$(".divBtnAdicionarAtividade").removeClass("hide");

		AtividadeEspecificidade.fnGerenciarAtividadeCaractericazao();

		if (AtividadeEspecificidade.settings.changeAtividade) {
			AtividadeEspecificidade.settings.changeAtividade();
		}
	},

	onAbrirPdfRequerimento: function () {
		var id = AtividadeEspecificidade.obterRequerimento();

		if (!id || id <= 0) {
			return;
		}

		MasterPage.redireciona(AtividadeEspecificidade.settings.urls.pdfRequerimento + "?id=" + id);
		MasterPage.carregando(false);
	},

	onAdicionarAtividade: function () {
		var id = parseInt($('.ddlAtividadeSolicitada', AtividadeEspecificidade.container).val()) || 0;
		var texto = $('.ddlAtividadeSolicitada :selected', AtividadeEspecificidade.container).text();
		var ehValido = true;

		if (id <= 0) {
			Mensagem.gerar(MasterPage.getContent(AtividadeEspecificidade.container), new Array(AtividadeEspecificidade.settings.Mensagens.AtividadeObrigatoria));
			return;
		}

		if (ehValido) {
			Mensagem.limpar(AtividadeEspecificidade.container);
			var tabela = $('.dgAtividadesSolicitadas', AtividadeEspecificidade.container);
			var linha = $('.trAtividadeTemplate', AtividadeEspecificidade.container).clone().removeClass('trAtividadeTemplate');
			var adicionar = true;

			tabela.find('tbody tr').each(function (i, linha) {
				if ($(linha).find('.hdnAtividadeId').val() == id) {
					adicionar = false;
					Mensagem.gerar(MasterPage.getContent(AtividadeEspecificidade.container), new Array(AtividadeEspecificidade.settings.Mensagens.AtividadeJaAdicionada));
					return;
				}
			});

			if (adicionar) {
				linha.find('.hdnAtividadeId').val(id);
				linha.find('.tdNome').html(texto).attr('title', texto);
				tabela.find('tbody:last').append(linha);

				AtividadeEspecificidade.setarEstiloTabela(tabela);
			}
		}
	},

	setarEstiloTabela: function (tabela) {
		tabela.find('tbody tr').each(function (i, linha) {
			$(linha).attr('class', '').addClass((i % 2) == 0 ? 'par' : 'impar');
		});
	},

	onExcluirLinha: function () {
		var tabela = $(this).closest('table');
		$(this).closest('tr').remove();

		AtividadeEspecificidade.setarEstiloTabela(tabela);
	},

	obterAtividades: function () {
		if ($('.divAtividadeEspecificidade', AtividadeEspecificidade.container).length <= 0) {
			return null;
		}

		var atividades = [];
		var protocolo = AtividadeEspecificidade.obterProtocoloDdlSelecionado();

		if ($('.dgAtividadesSolicitadas', AtividadeEspecificidade.container).is(':visible')) {
			$('.dgAtividadesSolicitadas tbody tr:visible', AtividadeEspecificidade.container).each(function () {
				atividades.push({ Id: $(this).find('.hdnAtividadeId').val(), Protocolo: protocolo });
			});
		} else {
			atividades = new Array({ Id: $('.ddlAtividadeSolicitada').val(), Protocolo: protocolo });
		}

		if (atividades.length === 0) {
			atividades = new Array({ Id: 0, Protocolo: protocolo });
		}

		return atividades;
	},

	obterRequerimento: function () {
		if ($('.ddlProcessosDocumentos :selected', AtividadeEspecificidade.container).length > 0) {
			return parseInt($('.ddlProcessosDocumentos :selected', AtividadeEspecificidade.container).val().split('@')[2]);
		} else {
			return 0;
		}
	},

	obterAtividade: function () {
		return parseInt($('.ddlAtividadeSolicitada', AtividadeEspecificidade.container).val());
	},

	obterProtocoloDdlSelecionado: function () {
		if ($('.ddlProcessosDocumentos :selected', AtividadeEspecificidade.container).length > 0) {
			var selecionado = $('.ddlProcessosDocumentos :selected', AtividadeEspecificidade.container).val().split('@');
			return { Id: parseInt(selecionado[0]), IsProcesso: selecionado[1] == "1" };
		} else {
			return null;
		}
	},

	fnGerenciarAtividadeCaractericazao: function () {

		var atividadeId = parseInt($('.ddlAtividadeSolicitada', AtividadeEspecificidade.container).val());
		var hdnAtividadeEspecificidadeCaracterizacao = $('.hdnAtividadeEspecificidadeCaracterizacao', Titulo.container);
		var configTitAtividadeEspCaracterizacao = $.parseJSON(hdnAtividadeEspecificidadeCaracterizacao.val() ? hdnAtividadeEspecificidadeCaracterizacao.val() : '{}');
		var atividadesID = $.parseJSON($('.hdnAtividadesID', Titulo.container).val() ? $('.hdnAtividadesID', Titulo.container).val() : '{}');

		if (configTitAtividadeEspCaracterizacao.Atividades && $.inArray(atividadeId, configTitAtividadeEspCaracterizacao.Atividades) > -1) {

			switch (atividadeId) {
				case atividadesID.BarragemID:
					AtividadeEspecificidade.Barragem.load();
					break;
				default:
					AtividadeEspecificidade.fnLimparAtividadeCaracterizacao();
					break;
			}
		} else {
			AtividadeEspecificidade.fnLimparAtividadeCaracterizacao();
		}
	},

	fnLimparAtividadeCaracterizacao: function () { $('.contextAtividadeCaracterizacao', AtividadeEspecificidade.container).empty(); }
};

AtividadeEspecificidade.Barragem = {
	settings: {
		urls: {
			urlEspBarragem: ''
		},
		fnOnChangeAtividade: function () {
			MasterPage.carregando(true);

			var hdnLicencaJSON = $('.hdnLicencaJSON', AtividadeEspecificidade.container);
			var licenca = $.parseJSON(hdnLicencaJSON.val() ? hdnLicencaJSON.val() : '{}');
			var especificidadeDados = {
				EmpreendimentoId: parseInt($('.hdnEmpreendimentoId', Titulo.container).val()),
				AtividadeId: $('.ddlAtividadeSolicitada', AtividadeEspecificidade.container).val(),
				DadoAuxiliarJSON: $.toJSON({
					BarragemId: licenca.BarragemId,
					IsVisualizar: Titulo.settings.isVisualizar,
					CaracterizacaoTid: licenca.CaracterizacaoTid
				})
			};

			$.ajax({ url: AtividadeEspecificidade.Barragem.settings.urls.urlEspBarragem,
				data: JSON.stringify({ especificidadeDados: especificidadeDados }),
				cache: false,
				async: false,
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, Titulo.container);
				},
				success: function (response, textStatus, XMLHttpRequest) {

					if (response.EhValido) {
						AtividadeEspecificidade.Barragem.container.empty().html(response.Html);
						Mascara.load(AtividadeEspecificidade.Barragem.container);
						AtividadeEspecificidade.Barragem.load(AtividadeEspecificidade.Barragem.container);
						MasterPage.load();
						Mensagem.limpar(Titulo.container);
					}

					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(Titulo.container, response.Msg);
					}
				}
			});

			MasterPage.carregando(false);
		},
		Mensagens: null
	},
	container: null,
	load: function (especificidadeAtiv, options) {

		if (options) {
			$.extend(AtividadeEspecificidade.Barragem.settings, options);
		}

		AtividadeEspecificidade.Barragem.container = $('.contextAtividadeCaracterizacao', AtividadeEspecificidade.container);

		if (especificidadeAtiv) {
			AtividadeEspecificidade.Barragem.container = especificidadeAtiv;
		}

		if (AtividadeEspecificidade.Barragem.container.length == 0) {
			AtividadeEspecificidade.container.find('fieldset:first').append('<div class="contextAtividadeCaracterizacao"></div>');
			AtividadeEspecificidade.Barragem.container = $('.contextAtividadeCaracterizacao', AtividadeEspecificidade.container);
		}

		if (!especificidadeAtiv && !options) {
			AtividadeEspecificidade.Barragem.settings.fnOnChangeAtividade();
		}
	},
	fnOnChangeAtividade: null,
	gerarObjeto: function () {
		return { barragemId: parseInt($('.ddlBarragens', AtividadeEspecificidade.Barragem.container).val()) };
	}
};

Titulo.settings.obterAtividadesFunc = AtividadeEspecificidade.obterAtividades;
Titulo.settings.obterRequerimentoFunc = AtividadeEspecificidade.obterRequerimento;
Titulo.addCallbackProtocolo(AtividadeEspecificidade.onChangeProtocolo, true);
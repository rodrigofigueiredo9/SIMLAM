/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />

FiscalizacaoConfiguracao = {
	settings: {
		urls: {
			criar: ''
		},
		Mensagens: null
	},
	container: null,
	load: function (container, options) {
		if (options) { $.extend(FiscalizacaoConfiguracao.settings, options); }
		FiscalizacaoConfiguracao.container = MasterPage.getContent(container);

		FiscalizacaoConfiguracao.container.delegate('.excluir', 'click', FiscalizacaoConfiguracao.onClickRemoverTR)

		$('.btnAdicionarSubitem', FiscalizacaoConfiguracao.container).click(FiscalizacaoConfiguracao.onClickAddSubitem);
		$('.btnAdicionarPergunta', FiscalizacaoConfiguracao.container).click(FiscalizacaoConfiguracao.onClickAddPergunta);
		$('.btnAdicionarCampo', FiscalizacaoConfiguracao.container).click(FiscalizacaoConfiguracao.onClickAddCampo);
		$('.btnSalvar', FiscalizacaoConfiguracao.container).click(FiscalizacaoConfiguracao.onClickSalvar);
	},
	gerarObjeto: function () {

		var configFiscalizacao = {
			Id: $('.hdnConfigFiscalizacaoId', FiscalizacaoConfiguracao.container).val(),
			ClassificacaoId: $('.ddlClassificacoes', FiscalizacaoConfiguracao.container).val(),
			TipoId: $('.ddlTipos', FiscalizacaoConfiguracao.container).val(),
			TipoTexto: $('.ddlTipos option:selected', FiscalizacaoConfiguracao.container).text(),
			ItemId: $('.ddlItens', FiscalizacaoConfiguracao.container).val(),
			ItemTexto: $('.ddlItens option:selected', FiscalizacaoConfiguracao.container).text(),
			Subitens: [],
			Perguntas: [],
			Campos: []
		};

		$('#gridSubiten', FiscalizacaoConfiguracao.container).find('.hdnSubItemJSON').each(function (i, itemDom) {
			configFiscalizacao.Subitens.push(JSON.parse($(this).val()));
		});

		$('#gridPerguntas', FiscalizacaoConfiguracao.container).find('.hdnPerguntaItemJSON').each(function (i, itemDom) {
			configFiscalizacao.Perguntas.push(JSON.parse($(this).val()));
		});

		$('#gridCampos', FiscalizacaoConfiguracao.container).find('.hdnCampoItemJSON').each(function (i, itemDom) {
			configFiscalizacao.Campos.push(JSON.parse($(this).val()));
		});

		return configFiscalizacao;
	},
	onClickRemoverTR: function () {
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable(FiscalizacaoConfiguracao.container.find('.dataGridTable'));
	},
	onClickAddSubitem: function () {

		var arrayMsg = [];
		var trTemplate = null;
		var domTable = $('#gridSubiten', FiscalizacaoConfiguracao.container);
		var ddlSubitens = $('.ddlSubitens', FiscalizacaoConfiguracao.container);
		var itemJaAdd = false;
		var subItem = {
			SubItemId: parseInt(ddlSubitens.val()),
			SubItemTexto: ddlSubitens.find('option').filter(':selected').text()
		};

		if (!subItem.SubItemId || subItem.SubItemId == 0) {
			arrayMsg.push(FiscalizacaoConfiguracao.settings.Mensagens.SubitemObrigatorio);
		}

		$('tbody', domTable).find('.hdnSubItemJSON').each(function (i, itemDom) {
			if (itemJaAdd) return;
			var item = JSON.parse($(this).val());
			if (item.SubItemId == subItem.SubItemId) {
				itemJaAdd = true;
			}
		});

		if (itemJaAdd) {
			arrayMsg.push(FiscalizacaoConfiguracao.settings.Mensagens.SubitemJaAdd);
		}

		if (arrayMsg.length > 0) {
			Mensagem.gerar(FiscalizacaoConfiguracao.container, arrayMsg);
			return;
		}

		trTemplate = $('.trSubItemTemplateRow', FiscalizacaoConfiguracao.container).clone().removeAttr('class');

		$('.subitem', trTemplate).text(subItem.SubItemTexto).attr('title', subItem.SubItemTexto);
		$('.hdnSubItemJSON', trTemplate).val($.toJSON(subItem));

		$('tbody', domTable).append(trTemplate);

		ddlSubitens.val(0);

		Listar.atualizarEstiloTable(FiscalizacaoConfiguracao.container.find('.dataGridTable'));

		Mensagem.limpar(FiscalizacaoConfiguracao.container);
	},
	onClickAddPergunta: function () {

		var arrayMsg = [];
		var trTemplate = null;
		var domTable = $('#gridPerguntas', FiscalizacaoConfiguracao.container);
		var ddlPerguntas = $('.ddlPerguntas', FiscalizacaoConfiguracao.container);
		var itemJaAdd = false;
		var pergunta = {
			PerguntaId: parseInt(ddlPerguntas.val()),
			PerguntaTexto: ddlPerguntas.find('option').filter(':selected').text()
		};

		if (!pergunta.PerguntaId || pergunta.PerguntaId == 0) {
			arrayMsg.push(FiscalizacaoConfiguracao.settings.Mensagens.PerguntaObrigatorio);
		}

		$('tbody', domTable).find('.hdnPerguntaItemJSON').each(function (i, itemDom) {
			if (itemJaAdd) return;
			var item = JSON.parse($(this).val());
			if (item.PerguntaId == pergunta.PerguntaId) {
				itemJaAdd = true;
			}
		});

		if (itemJaAdd) {
			arrayMsg.push(FiscalizacaoConfiguracao.settings.Mensagens.PerguntaJaAdd);
		}

		if (arrayMsg.length > 0) {
			Mensagem.gerar(FiscalizacaoConfiguracao.container, arrayMsg);
			return;
		}

		trTemplate = $('.trPerguntaTemplateRow', FiscalizacaoConfiguracao.container).clone().removeAttr('class');

		$('.pergunta', trTemplate).text(pergunta.PerguntaTexto).attr('title', pergunta.PerguntaTexto);
		$('.hdnPerguntaItemJSON', trTemplate).val($.toJSON(pergunta));

		$('tbody', domTable).append(trTemplate);

		ddlPerguntas.val(0);

		Listar.atualizarEstiloTable(FiscalizacaoConfiguracao.container.find('.dataGridTable'));

		Mensagem.limpar(FiscalizacaoConfiguracao.container);

	},
	onClickAddCampo: function () {

		var arrayMsg = [];
		var trTemplate = null;
		var domTable = $('#gridCampos', FiscalizacaoConfiguracao.container);
		var ddlCampos = $('.ddlCampos', FiscalizacaoConfiguracao.container);
		var itemJaAdd = false;
		var campo = {
			CampoId: parseInt(ddlCampos.val()),
			CampoTexto: ddlCampos.find('option:selected').text()
		};

		if (!campo.CampoId || campo.CampoId == 0) {
			arrayMsg.push(FiscalizacaoConfiguracao.settings.Mensagens.CampoObrigatorio);
		}

		$('tbody', domTable).find('.hdnCampoItemJSON').each(function (i, itemDom) {
			if (itemJaAdd) return;
			var item = JSON.parse($(this).val());
			if (item.CampoId == campo.CampoId) {
				itemJaAdd = true;
			}
		});

		if (itemJaAdd) {
			arrayMsg.push(FiscalizacaoConfiguracao.settings.Mensagens.CampoJaAdd);
		}

		if (arrayMsg.length > 0) {
			Mensagem.gerar(FiscalizacaoConfiguracao.container, arrayMsg);
			return;
		}

		trTemplate = $('.trCampoTemplateRow', FiscalizacaoConfiguracao.container).clone().removeAttr('class');

		$('.campo', trTemplate).text(campo.CampoTexto).attr('title', campo.CampoTexto);
		$('.hdnCampoItemJSON', trTemplate).val($.toJSON(campo));

		$('tbody', domTable).append(trTemplate);

		ddlCampos.val(0);

		Listar.atualizarEstiloTable(FiscalizacaoConfiguracao.container.find('.dataGridTable'));

		Mensagem.limpar(FiscalizacaoConfiguracao.container);

	},
	onClickSalvar: function () {

		MasterPage.carregando(true);

		$.ajax({ url: FiscalizacaoConfiguracao.settings.urls.criar,
			data: $.toJSON({ configuracao: FiscalizacaoConfiguracao.gerarObjeto() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, FiscalizacaoConfiguracao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					Mensagem.limpar(FiscalizacaoConfiguracao.container);
					MasterPage.redireciona(response.Redirect);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(FiscalizacaoConfiguracao.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);

	}
}

ConfigurarTipoInfracao = {
	settings: {
		urls: {
			salvar: '',
			alterarSituacao: '',
			podeDesativar: '',
			podeEditar: '',
			ExcluirConfirm: '',
			Excluir: ''
		},

		mensagens: null
	},

	container: null,

	load: function (container, options) {
		if (options) { $.extend(ConfigurarTipoInfracao.settings, options); }
		ConfigurarTipoInfracao.container = container;

		Item.load(container);
		Item.settings.urls.podeDesativar = ConfigurarTipoInfracao.settings.urls.podeDesativar;
		Item.settings.urls.podeEditar = ConfigurarTipoInfracao.settings.urls.podeEditar;
		Item.settings.urls.salvar = ConfigurarTipoInfracao.settings.urls.salvar;
		Item.settings.urls.Excluir = ConfigurarTipoInfracao.settings.urls.Excluir;
		Item.settings.urls.ExcluirConfirm = ConfigurarTipoInfracao.settings.urls.ExcluirConfirm;
		Item.settings.urls.alterarSituacao = ConfigurarTipoInfracao.settings.urls.alterarSituacao;
		Item.settings.mensagens = ConfigurarTipoInfracao.settings.mensagens;
	}
}

ConfigurarItemInfracao = {
	settings: {
		urls: {
			salvar: '',
			alterarSituacao: '',
			podeDesativar: '',
			podeEditar: '',
			ExcluirConfirm: '',
			Excluir: ''
		},

		mensagens: null
	},

	container: null,

	load: function (container, options) {
		if (options) { $.extend(ConfigurarItemInfracao.settings, options); }
		ConfigurarItemInfracao.container = container;

		Item.load(container);
		Item.settings.urls.podeDesativar = ConfigurarItemInfracao.settings.urls.podeDesativar;
		Item.settings.urls.podeEditar = ConfigurarItemInfracao.settings.urls.podeEditar;
		Item.settings.urls.salvar = ConfigurarItemInfracao.settings.urls.salvar;
		Item.settings.urls.Excluir = ConfigurarItemInfracao.settings.urls.Excluir;
		Item.settings.urls.ExcluirConfirm = ConfigurarItemInfracao.settings.urls.ExcluirConfirm;
		Item.settings.urls.alterarSituacao = ConfigurarItemInfracao.settings.urls.alterarSituacao;
		Item.settings.mensagens = ConfigurarItemInfracao.settings.mensagens;
	}
}

ConfigurarSubItemInfracao = {
	settings: {
		urls: {
			salvar: '',
			alterarSituacao: '',
			podeDesativar: '',
			podeEditar: '',
			ExcluirConfirm: '',
			Excluir: ''
		},

		mensagens: null
	},

	container: null,

	load: function (container, options) {
		if (options) { $.extend(ConfigurarSubItemInfracao.settings, options); }
		ConfigurarSubItemInfracao.container = container;

		Item.load(container);
		Item.settings.urls.podeDesativar = ConfigurarSubItemInfracao.settings.urls.podeDesativar;
		Item.settings.urls.podeEditar = ConfigurarSubItemInfracao.settings.urls.podeEditar;
		Item.settings.urls.salvar = ConfigurarSubItemInfracao.settings.urls.salvar;
		Item.settings.urls.Excluir = ConfigurarSubItemInfracao.settings.urls.Excluir;
		Item.settings.urls.ExcluirConfirm = ConfigurarSubItemInfracao.settings.urls.ExcluirConfirm;
		Item.settings.urls.alterarSituacao = ConfigurarSubItemInfracao.settings.urls.alterarSituacao;
		Item.settings.mensagens = ConfigurarSubItemInfracao.settings.mensagens;
	}
}

ConfigurarCampoInfracao = {
	settings: {
		urls: {
			salvar: '',
			alterarSituacao: '',
			podeDesativar: '',
			podeEditar: '',
			ExcluirConfirm: '',
			Excluir: ''
		},
		idsTela: null,
		mensagens: null
	},

	container: null,

	load: function (container, options) {
		if (options) { $.extend(ConfigurarCampoInfracao.settings, options); }
		ConfigurarCampoInfracao.container = container;

		Item.load(container, { possuiTipo: true });

		Item.settings.urls.podeDesativar = ConfigurarCampoInfracao.settings.urls.podeDesativar;
		Item.settings.urls.podeEditar = ConfigurarCampoInfracao.settings.urls.podeEditar;
		Item.settings.urls.salvar = ConfigurarCampoInfracao.settings.urls.salvar;
		Item.settings.urls.Excluir = ConfigurarCampoInfracao.settings.urls.Excluir;
		Item.settings.urls.ExcluirConfirm = ConfigurarCampoInfracao.settings.urls.ExcluirConfirm;
		Item.settings.urls.alterarSituacao = ConfigurarCampoInfracao.settings.urls.alterarSituacao;
		Item.settings.mensagens = ConfigurarCampoInfracao.settings.mensagens;
		Item.settings.idsTela = ConfigurarCampoInfracao.settings.idsTela;
	}
}

ConfigurarRespostaInfracao = {
	settings: {
		urls: {
			salvar: '',
			alterarSituacao: '',
			podeDesativar: '',
			podeEditar: '',
			ExcluirConfirm: '',
			Excluir: ''
		},

		mensagens: null
	},

	container: null,

	load: function (container, options) {
		if (options) { $.extend(ConfigurarRespostaInfracao.settings, options); }
		ConfigurarRespostaInfracao.container = container;

		Item.load(container);
		Item.settings.urls.podeDesativar = ConfigurarRespostaInfracao.settings.urls.podeDesativar;
		Item.settings.urls.podeEditar = ConfigurarRespostaInfracao.settings.urls.podeEditar;
		Item.settings.urls.salvar = ConfigurarRespostaInfracao.settings.urls.salvar;
		Item.settings.urls.Excluir = ConfigurarRespostaInfracao.settings.urls.Excluir;
		Item.settings.urls.ExcluirConfirm = ConfigurarRespostaInfracao.settings.urls.ExcluirConfirm;
		Item.settings.urls.alterarSituacao = ConfigurarRespostaInfracao.settings.urls.alterarSituacao;
		Item.settings.mensagens = ConfigurarRespostaInfracao.settings.mensagens;
	}
}

ConfigurarPerguntaInfracao = {
	settings: {
		urls: {
			salvar: ''
		},

		mensagens: null
	},

	container: null,

	load: function (container, options) {
		if (options) { $.extend(ConfigurarPerguntaInfracao.settings, options); }
		ConfigurarPerguntaInfracao.container = container;

		container.delegate('.btnSalvar', 'click', ConfigurarPerguntaInfracao.salvar);
		container.delegate('.btnAdicionarResposta', 'click', ConfigurarPerguntaInfracao.adicionarResposta);
		container.delegate('.btnExcluirResposta', 'click', ConfigurarPerguntaInfracao.excluirResposta);

		$('.txtPergunta', container).focus();
	},

	adicionarResposta: function () {
		var container = ConfigurarPerguntaInfracao.container;
		Mensagem.limpar(container);
		var mensagens = new Array();

		var resposta = {
			Id: $('.ddlResposta :selected', container).val(),
			Texto: $('.ddlResposta :selected', container).text(),
			Especificar: $('.rdbEspecificarResposta:checked', container).val()
		}

		if (resposta.Id <= 0) {
			mensagens.push(jQuery.extend(true, {}, ConfigurarPerguntaInfracao.settings.mensagens.RespostaObrigatoria));
		} else {
			$('.hdnItemJSon', container).each(function () {
				var obj = String($(this).val());
				if (obj != '') {
					var resp = (JSON.parse(obj));
					if (resp.Id == resposta.Id) {
						mensagens.push(jQuery.extend(true, {}, ConfigurarPerguntaInfracao.settings.mensagens.RespostaDuplicada));
						Mensagem.gerar(ConfigurarPerguntaInfracao.container, mensagens);
						return;
					}
				}
			});
		}

		if (typeof resposta.Especificar == 'undefined') {
			mensagens.push(jQuery.extend(true, {}, ConfigurarPerguntaInfracao.settings.mensagens.RespostaEspecificarObrigatorio));
		} else {
			resposta.Especificar = resposta.Especificar == 1;
			resposta.EspecificarTexto = resposta.Especificar ? 'Sim' : 'Não';
		}

		if (mensagens.length > 0) {
			Mensagem.gerar(ConfigurarPerguntaInfracao.container, mensagens);
			return;
		}

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(resposta));

		linha.find('.resposta').html(resposta.Texto).attr('title', resposta.Texto);
		linha.find('.especificar').html(resposta.EspecificarTexto).attr('title', resposta.EspecificarTexto);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.ddlResposta', ConfigurarPerguntaInfracao.container).ddlFirst();
		$('.rdbEspecificarResposta', ConfigurarPerguntaInfracao.container).removeAttr('checked');
	},

	excluirResposta: function () {
		Mensagem.limpar(ConfigurarPerguntaInfracao.container);
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(ConfigurarPerguntaInfracao.container.find('.dataGridTable'));
	},

	obter: function () {
		var container = ConfigurarPerguntaInfracao.container;

		var obj = {
			Id: $('.hdnPerguntaId', container).val(),
			SituacaoId: $('.hdnSituacaoId', container).val(),
			Texto: $('.txtPergunta', container).val(),
			Respostas: []
		}

		$('.hdnItemJSon', container).each(function () {
			var objResposta = String($(this).val());
			if (objResposta != '') {
				obj.Respostas.push(JSON.parse(objResposta));
			}
		});

		return obj;

	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: ConfigurarPerguntaInfracao.settings.urls.salvar,
			data: JSON.stringify(ConfigurarPerguntaInfracao.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ConfigurarPerguntaInfracao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ConfigurarPerguntaInfracao.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}

Item = {
	settings: {
		urls: {
			podeDesativar: '',
			podeEditar: '',
			salvar: '',
			alterarSituacao: '',
			ExcluirConfirm: '',
			Excluir: ''

		},
		container: null,
		idsTela: null,
		mensagens: null,
		possuiTipo: false
	},

	load: function (container, options) {
		if (options) { $.extend(Item.settings, options); }
		Item.settings.container = container;

		Item.settings.container.delegate('.btnSalvar', 'click', Item.salvar);
		Item.settings.container.delegate('.btnEditarItem', 'click', Item.editar);
		Item.settings.container.delegate('.btnExcluirItem', 'click', Item.excluir);
		Item.settings.container.delegate('.btnAtivarItem', 'click', Item.ativar);
		Item.settings.container.delegate('.btnDesativarItem', 'click', Item.desativar);

		Item.gerenciarBotoes();

		$('.txtNomeCampo', container).focus();

		if (Item.settings.possuiTipo) {
			Item.settings.container.delegate('.ddlTipoCampo', 'change', Item.gerenciarTipo);
		}
	},

	gerenciarBotoes: function () {
		$('.hdnItemJSon', Item.settings.container).each(function () {
			var strJSON = $(this).val().toString();
			if (strJSON != '') {
				var item = JSON.parse(strJSON);
				var containerAux = $(this).closest('tr');

				if (item.IsAtivo) {
					$('.btnDesativarItem', containerAux).button({
						disabled: false
					});

					$('.btnAtivarItem', containerAux).button({
						disabled: true
					});
				} else {
					$('.btnDesativarItem', containerAux).button({
						disabled: true
					});

					$('.btnAtivarItem', containerAux).button({
						disabled: false
					});
				}

			}

		});

	},

	gerenciarTipo: function () {
		var tipo = Number($('.ddlTipoCampo :selected', Item.settings.container).val()) || 0;
		$('.divUnidadeMedida', Item.settings.container).addClass('hide');
		if (tipo == 2) {
			$('.divUnidadeMedida', Item.settings.container).removeClass('hide');
		}

	},

	desativar: function () {
		var container = $(this).closest('tr');
		Mensagem.limpar(Item.settings.container);
		var item = JSON.parse($('.hdnItemJSon', container).val());

		MasterPage.carregando(true);
		$.ajax({ url: Item.settings.urls.alterarSituacao,
			data: JSON.stringify({ tipoId: item.Id, situacaoNova: 0 }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ConfigurarItemInfracao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.DivItens', Item.settings.container).empty();
					$('.DivItens', Item.settings.container).append(response.Html);

					Listar.atualizarEstiloTable(Item.settings.container.find('.dataGridTable'));
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Item.settings.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);

		Item.limparCampos();
	},

	ativar: function () {
		var container = $(this).closest('tr');
		Mensagem.limpar(Item.settings.container);
		var item = JSON.parse($('.hdnItemJSon', container).val());

		MasterPage.carregando(true);
		$.ajax({ url: Item.settings.urls.alterarSituacao,
			data: JSON.stringify({ tipoId: item.Id, situacaoNova: 1 }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ConfigurarItemInfracao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.DivItens', Item.settings.container).empty();
					$('.DivItens', Item.settings.container).append(response.Html);

					Listar.atualizarEstiloTable(Item.settings.container.find('.dataGridTable'));
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Item.settings.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);

		Item.limparCampos();
	},

	salvar: function () {
		Mensagem.limpar(Item.settings.container);
		var mensagens = new Array();
		var container = Item.settings.container;
		var possuiTipo = Item.settings.possuiTipo;

		var item = {
			Id: $('.hdnItemId', container).val(),
			Texto: $('.txtNomeCampo', container).val(),
			IsAtivo: $('.hdnItemIsAtivo', container).val() == 1,
			SituacaoTexto: $('.hdnItemIsAtivo', container).val() == 1 ? 'Ativado' : 'Desativado',
			TipoCampo: $('.ddlTipoCampo :selected', container).val(),
			TipoCampoTexto: $('.ddlTipoCampo :selected', container).text(),
			UnidadeMedida: $('.ddlUnidadeMedida :selected', container).val(),
			UnidadeMedidaTexto: $('.ddlUnidadeMedida :selected', container).text()
		}

		if (item.Texto.trim() == '') {
			mensagens.push(jQuery.extend(true, {}, Item.settings.mensagens.NomeItemObrigatorio));
		} else {

			if (!$(Item.settings.container).hasClass('edit')) {
				$('.hdnItemJSon', container).each(function () {
					var obj = String($(this).val());
					if (obj != '') {
						var obj = (JSON.parse(obj));
						if (obj.Texto.toLowerCase().trim() == item.Texto.toLowerCase().trim()) {
							mensagens.push(jQuery.extend(true, {}, Item.settings.mensagens.ItemDuplicado));
						}
					}
				});
			}
		}

		if (possuiTipo) {
			if (item.TipoCampo > 0) {
				if (item.TipoCampo == Item.settings.idsTela.TipoCampoNumerico) {
					if (item.UnidadeMedida <= 0) {
						mensagens.push(jQuery.extend(true, {}, Item.settings.mensagens.UnidadeCampoObrigatorio));
					}
				}
			} else {
				mensagens.push(jQuery.extend(true, {}, Item.settings.mensagens.TipoCampoObrigatorio));
			}
		}

		if (mensagens.length > 0) {
			Mensagem.gerar(Item.settings.container, mensagens);
			return;
		}


		MasterPage.carregando(true);
		$.ajax({ url: Item.settings.urls.salvar,
			data: JSON.stringify(item),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ConfigurarItemInfracao.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.DivItens', Item.settings.container).empty();
					$('.DivItens', Item.settings.container).append(response.Html);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Item.settings.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);

		Listar.atualizarEstiloTable(Item.settings.container.find('.dataGridTable'));

		Item.limparCampos();

	},

	editar: function () {
		Mensagem.limpar(Item.settings.container);

		Item.cancelarEdicao();

		var container = $(this).closest('tr');
		var mensagens = new Array();
		var item = JSON.parse($('.hdnItemJSon', container).val());
		var possuiTipo = Item.settings.possuiTipo;

		MasterPage.carregando(true);
		$.ajax({ url: Item.settings.urls.podeEditar,
			data: JSON.stringify({ id: item.Id }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Item.settings.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {

					$(Item.settings.container).addClass('edit');
					$(Item.settings.container.find('.linhaSelecionada')).removeClass('linhaSelecionada');

					if (!item.IsAtivo) {
						mensagens.push(jQuery.extend(true, {}, Item.settings.mensagens.EditarItemDesativado));
						Mensagem.gerar(Item.settings.container, mensagens);
						return false;
					}

					$(container).addClass('linhaSelecionada');

					$('.txtNomeCampo', Item.settings.container).val(item.Texto);
					$('.hdnItemId', Item.settings.container).val(item.Id);
					$('.hdnItemIsAtivo', Item.settings.container).val(item.IsAtivo ? 1 : 0);

					$('.txtNomeCampo', Item.settings.container).focus();

					if (possuiTipo) {

						$('.ddlTipoCampo option', Item.settings.container).each(function () {
							if ($(this).val() == item.TipoCampo) $(this).attr('selected', 'selected');
						});
						$('.ddlUnidadeMedida option', Item.settings.container).each(function () {
							if ($(this).val() == item.UnidadeMedida) $(this).attr('selected', 'selected');
						});

						$('.ddlTipoCampo', Item.settings.container).attr('disabled', 'disabled');
						$('.ddlUnidadeMedida', Item.settings.container).attr('disabled', 'disabled');

						Item.gerenciarTipo();
					}
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Item.settings.container, response.Msg);
					return
				}
			}
		});
		MasterPage.carregando(false);
	},

	cancelarEdicao: function () {
		Item.limparCampos();
	},

	limparCampos: function () {
		var possuiTipo = Item.settings.possuiTipo;

		$('.txtNomeCampo', Item.settings.container).val('');
		$('.hdnItemId', Item.settings.container).val(0);

		if (possuiTipo) {
			$('.ddlTipoCampo', Item.settings.container).ddlFirst();
			$('.ddlUnidadeMedida', Item.settings.container).ddlFirst();

			Item.gerenciarTipo();
		}

		Item.gerenciarBotoes();
		$(Item.settings.container).removeClass('edit');
		$(Item.settings.container.find('.linhaSelecionada')).removeClass('linhaSelecionada');

	},

	excluir: function () {
		Mensagem.limpar(Item.settings.container);
		Modal.excluir({
			'urlConfirm': Item.settings.urls.ExcluirConfirm,
			'urlAcao': Item.settings.urls.Excluir,
			'id': $(this).closest('tr').find('.itemId:first').val(),
			'callBack': Item.callBackExcluir,
			'naoExecutarUltimaBusca': true
		});
	},

	callBackExcluir: function (data) {
		MasterPage.redireciona(data.urlRedireciona);
	}
}

String.prototype.trim = function () {
	return this.replace(/^\W+|\W+$/g, "");
}
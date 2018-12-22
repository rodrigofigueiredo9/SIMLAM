/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

RegularizacaoFundiaria = {
	settings: {
		zona: 0,
		empreendimentoID: 0,
		caracterizacaoID: 0,
		matriculas: null,
		urls: {
			criarPosse: null,
			editarPosse: null,
			visualizarPosse: null,
			visualizarDominio: null,
			validarDominioAvulso: null,
			obterAreaTotalPosse: null,
			obterPerimetroPosse: null,
			pessoaAssociar: null,
			pessoaVisualizar: null,
			validarTransmitente: null,
			validarUsoAtualSolo: null,
			validarEdificacao: null,
			validarPosse: null,
			mergiar: null,
			salvar: null
		},
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(RegularizacaoFundiaria.settings, options); }
		RegularizacaoFundiaria.container = MasterPage.getContent(container);

		RegularizacaoFundiaria.container.delegate('.btnEditarPosse', 'click', RegularizacaoFundiaria.editarPosse);
		RegularizacaoFundiaria.container.delegate('.btnConfirmarPosse', 'click', RegularizacaoFundiaria.salvarPosse);
		RegularizacaoFundiaria.container.delegate('.btnCancelarPosse', 'click', RegularizacaoFundiaria.fecharPosse);
		RegularizacaoFundiaria.container.delegate('.btnVisualizarPosse', 'click', RegularizacaoFundiaria.visualizarPosse);

		RegularizacaoFundiaria.container.delegate('.radioPossuiDominioAvulso', 'change', RegularizacaoFundiaria.changeDominioAvulso);
		RegularizacaoFundiaria.container.delegate('.btnAdicionarDominioAvulso', 'click', RegularizacaoFundiaria.adicionarDominioAvulso);
		RegularizacaoFundiaria.container.delegate('.btnLimparDominioAvulso', 'click', RegularizacaoFundiaria.limparCamposDominioAvulso);
		RegularizacaoFundiaria.container.delegate('.btnDominioEditar', 'click', RegularizacaoFundiaria.editarDominioAvulso);
		RegularizacaoFundiaria.container.delegate('.btnDominioVisualizar', 'click', RegularizacaoFundiaria.visualizarDominio);
		RegularizacaoFundiaria.container.delegate('.btnDominioExcluir', 'click', RegularizacaoFundiaria.removerDominioAvulso);
		RegularizacaoFundiaria.container.delegate('.divOpcaoOcupacao .radioOpcao', 'change', function () { RegularizacaoFundiaria.gerenciarOpcoes(); });

		RegularizacaoFundiaria.container.delegate('.btnCarregarCroqui', 'click', RegularizacaoFundiaria.obterAreaTotalPosse);
		RegularizacaoFundiaria.container.delegate('.btnCarregarPerimetro', 'click', RegularizacaoFundiaria.obterPerimetroPosse);

		RegularizacaoFundiaria.container.delegate('.btnAssociarPessoa', 'click', RegularizacaoFundiaria.associarTransmitente);
		RegularizacaoFundiaria.container.delegate('.btnAdicionarTransmitente', 'click', RegularizacaoFundiaria.adicionarTransmitente);
		RegularizacaoFundiaria.container.delegate('.btnVisualizarTransmitente', 'click', RegularizacaoFundiaria.visualizarTransmitente);
		RegularizacaoFundiaria.container.delegate('.btnRemoverTransmitente', 'click', RegularizacaoFundiaria.removerTransmitente);
		RegularizacaoFundiaria.container.delegate('.btnAdicionarUsoSolo', 'click', RegularizacaoFundiaria.adicionarUsoSolo);
		RegularizacaoFundiaria.container.delegate('.btnRemoverUsoSolo', 'click', RegularizacaoFundiaria.removerUsoSolo);
		RegularizacaoFundiaria.container.delegate('.btnAdicionarEdificacao', 'click', RegularizacaoFundiaria.adicionarEdificacao);
		RegularizacaoFundiaria.container.delegate('.btnRemoverLinhaGrid', 'click', RegularizacaoFundiaria.removerLinhaGrid);

		RegularizacaoFundiaria.container.delegate('.btnSalvar', 'click', RegularizacaoFundiaria.salvar);
		RegularizacaoFundiaria.container.delegate('.titFiltros', 'click', Aux.expadirFieldSet);

		RegularizacaoFundiaria.configurarObjeto();
		if (RegularizacaoFundiaria.settings.textoMerge) {
			RegularizacaoFundiaria.abrirModalRedireciona(RegularizacaoFundiaria.settings.textoMerge);
		}
	},

	configurarObjeto: function () {
		//Dominios Avulsos
		$('.tabRegularizacoesFundiarias tbody tr', RegularizacaoFundiaria.container).each(function () {
			var objeto = JSON.parse($(this).find('.hdnItemJSon').val());

			$(objeto.DominiosAvulsos).each(function (i, item) {
				if (item.NumeroCCIR) {
					item.NumeroCCIR = item.NumeroCCIR.toString();
				}
			});

			$(this).find('.hdnItemJSon').val(JSON.stringify(objeto));
		});

		//Matriculas
		$(RegularizacaoFundiaria.settings.matriculas).each(function (i, dominio) {
			if (dominio.NumeroCCIR) {
				dominio.NumeroCCIR = dominio.NumeroCCIR.toString();
			}

			dominio.DataUltimaAtualizacao = { DataTexto: dominio.DataUltimaAtualizacao.DataTexto };

			$(dominio.ReservasLegais).each(function () {
				var item = this;

				if (item.Coordenada.NorthingUtm == null) {
					return;
				}

				item.Coordenada.NorthingUtm = item.Coordenada.NorthingUtm.toString().replace('.', ',');
				item.Coordenada.EastingUtm = item.Coordenada.EastingUtm.toString().replace('.', ',');
			});
		});
	},

	abrirModalRedireciona: function (textoModal) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', RegularizacaoFundiaria.container).attr('href'));
			},
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: RegularizacaoFundiaria.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	abrirModalMerge: function (textoModal) {
		Modal.confirma({
			removerFechar: true,
			conteudo: textoModal,
			titulo: RegularizacaoFundiaria.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia,
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				MasterPage.carregando(true);
				$.ajax({
					url: RegularizacaoFundiaria.settings.urls.mergiar,
					data: JSON.stringify(RegularizacaoFundiaria.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: Aux.error,
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', RegularizacaoFundiaria.container);
						container.empty();
						container.append(response.Html);
						RegularizacaoFundiaria.settings.dependencias = response.Dependencias;
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
				return false;
			}
		});
	},

	visualizarDominio: function () {
		var dominio = JSON.parse($(this).closest('td').find('.hdnDominioJSON').val());

		if (!dominio.Identificacao) {
			RegularizacaoFundiaria.visualizarDominioAvulso(this);
			return;
		}

		if (dominio.NumeroCCIR) {
			dominio.NumeroCCIR = dominio.NumeroCCIR.toString();
		}

		dominio.DataUltimaAtualizacao = { DataTexto: dominio.DataUltimaAtualizacao.DataTexto };

		$(dominio.ReservasLegais).each(function () {
			var item = this;

			if (item.Coordenada.NorthingUtm == null) {
				return;
			}

			item.Coordenada.NorthingUtm = item.Coordenada.NorthingUtm.toString().replace('.', ',');
			item.Coordenada.EastingUtm = item.Coordenada.EastingUtm.toString().replace('.', ',');
		});

		RegularizacaoFundiaria.limparCamposDominioAvulso();
		Modal.abrir(
			RegularizacaoFundiaria.settings.urls.visualizarDominio,
			dominio,
			function (container) { Dominio.load(container); },
			Modal.tamanhoModalGrande);
	},

	changeDominioAvulso: function () {
		var esconder = $('.radioPossuiDominioAvulso:checked', RegularizacaoFundiaria.container).val() != 1;
		$('.dominioAvulso', RegularizacaoFundiaria.container).toggleClass('hide', esconder);
		$('.divNaoPossuiDominioAvulso', RegularizacaoFundiaria.container).addClass('hide');
		$('.tabAreasAnexadas', RegularizacaoFundiaria.container).closest('.dataGrid').removeClass('hide');

		if (esconder && $('.hdnQuantidadeMatriculaGeo', RegularizacaoFundiaria.container).val() == 0) {
			$('.divNaoPossuiDominioAvulso', RegularizacaoFundiaria.container).removeClass('hide');
			$('.tabAreasAnexadas', RegularizacaoFundiaria.container).closest('.dataGrid').addClass('hide');
		}
	},

	adicionarDominioAvulso: function () {
		Mensagem.limpar(RegularizacaoFundiaria.container);
		var isEditar = true;
		var tr = $('.tabAreasAnexadas .editando', RegularizacaoFundiaria.container);

		if (tr.length <= 0) {
			isEditar = false;
			tr = $('.tabAreasAnexadasTemplate tr', RegularizacaoFundiaria.container).clone();
		}

		var item = {
			Identificacao: '',
			Id: $('.hdnDominioID', RegularizacaoFundiaria.container).val(),
			Matricula: $('.txtMatricula', RegularizacaoFundiaria.container).val(),
			Folha: $('.txtFolha', RegularizacaoFundiaria.container).val(),
			Livro: $('.txtLivro', RegularizacaoFundiaria.container).val(),
			AreaDocumento: Mascara.getFloatMask($('.txtAreaDocumento', RegularizacaoFundiaria.container).val()),
			Cartorio: $('.txtCartorio', RegularizacaoFundiaria.container).val(),
			NumeroCCIR: $('.txtNumeroCCIR', RegularizacaoFundiaria.container).val(),
			AreaCCIR: Mascara.getFloatMask($('.txtAreaCCIR', RegularizacaoFundiaria.container).val()),
			DataUltimaAtualizacao: { DataTexto: $('.txtDataUltimaAtualizacao', RegularizacaoFundiaria.container).val() }
		};

		var lista = new Array();
		$('.tabAreasAnexadas tbody tr:not(.editando)', RegularizacaoFundiaria.container).each(function (i, linha) {
			lista.push(JSON.parse($('.hdnDominioJSON', linha).val()));
		});

		var ehValido = MasterPage.validarAjax(
			RegularizacaoFundiaria.settings.urls.validarDominioAvulso,
			{ dominioLista: lista, dominio: item },
			RegularizacaoFundiaria.container, false).EhValido;

		if (!ehValido) {
			return;
		}

		$('.hdnDominioJSON', tr).val(JSON.stringify(item));
		$('.matriculaFolhaLivro', tr).text(item.Matricula + ' - ' + item.Folha + ' - ' + item.Livro);
		$('.areaDocumental', tr).text(Mascara.getStringMask(item.AreaDocumento));

		if (!isEditar) {
			$('.tabAreasAnexadas tbody', RegularizacaoFundiaria.container).append(tr);
		}

		RegularizacaoFundiaria.limparCamposDominioAvulso();
		Listar.atualizarEstiloTable($('.tabAreasAnexadas', RegularizacaoFundiaria.container));
		$('.txtMatricula', RegularizacaoFundiaria.container).focus();
		$('.radioPossuiDominioAvulso', RegularizacaoFundiaria.container).addClass('disabled').attr('disabled', 'disabled');
	},

	montarCamposDominioAvulso: function (controle) {
		var linha = $(controle).closest('tr');
		var item = JSON.parse(linha.find('.hdnDominioJSON').val());

		if (item.AreaCCIR == 0) {
			item.AreaCCIR = '';
		}

		$('.hdnDominioID', RegularizacaoFundiaria.container).val(item.Id);
		$('.txtMatricula', RegularizacaoFundiaria.container).val(item.Matricula);
		$('.txtFolha', RegularizacaoFundiaria.container).val(item.Folha);
		$('.txtLivro', RegularizacaoFundiaria.container).val(item.Livro);
		$('.txtAreaDocumento', RegularizacaoFundiaria.container).val(Mascara.getStringMask(item.AreaDocumento));
		$('.txtCartorio', RegularizacaoFundiaria.container).val(item.Cartorio);
		$('.txtNumeroCCIR', RegularizacaoFundiaria.container).val(item.NumeroCCIR);
		$('.txtAreaCCIR', RegularizacaoFundiaria.container).val(Mascara.getStringMask(item.AreaCCIR));
		$('.txtDataUltimaAtualizacao', RegularizacaoFundiaria.container).val(item.DataUltimaAtualizacao.DataTexto);

		$('.btnLimparDominioAvulso', RegularizacaoFundiaria.container).removeClass('hide');

		return linha
	},

	limparCamposDominioAvulso: function () {
		$('.campoMatricula', RegularizacaoFundiaria.container).unmask().val('');
		Mascara.load($('.dominioAvulso', RegularizacaoFundiaria.container));
		$('.hdnDominioID', RegularizacaoFundiaria.container).val('0');

		$('.tabAreasAnexadas tbody tr', RegularizacaoFundiaria.container).removeClass('editando');
		$('.btnLimparDominioAvulso', RegularizacaoFundiaria.container).addClass('hide');

		if ($('.btnConfirmarPosse', RegularizacaoFundiaria.container).is(':visible')) { //cadastrar/editar
			$('.campoMatricula', RegularizacaoFundiaria.container).removeClass('disabled').removeAttr('disabled');
			$('.btnAdicionarDominioAvulso', RegularizacaoFundiaria.container).removeClass('hide');
		} else {
			$('.dominioAvulso', RegularizacaoFundiaria.container).addClass('hide');
		}
	},

	editarDominioAvulso: function () {
		$('.tabAreasAnexadas tbody tr', RegularizacaoFundiaria.container).removeClass('editando');
		var linha = RegularizacaoFundiaria.montarCamposDominioAvulso(this);
		linha.addClass('editando');
		$('.campoMatricula', RegularizacaoFundiaria.container).removeClass('disabled').removeAttr('disabled');
		$('.btnAdicionarDominioAvulso', RegularizacaoFundiaria.container).removeClass('hide');
	},

	visualizarDominioAvulso: function (controle) {
		RegularizacaoFundiaria.montarCamposDominioAvulso(controle);
		$('.campoMatricula', RegularizacaoFundiaria.container).addClass('disabled').attr('disabled', 'disabled');
		$('.btnAdicionarDominioAvulso', RegularizacaoFundiaria.container).addClass('hide');
		$('.dominioAvulso', RegularizacaoFundiaria.container).removeClass('hide');
	},

	verificarPossuiDominioAvulso: function () {
		if ($('.tabAreasAnexadas tbody tr .btnDominioEditar', RegularizacaoFundiaria.container).length <= 0) {
			$('.radioPossuiDominioAvulso', RegularizacaoFundiaria.container).removeAttr('disabled');
		}	
	},

	removerDominioAvulso: function () {
		RegularizacaoFundiaria.removerLinhaGrid(this);
		RegularizacaoFundiaria.limparCamposDominioAvulso();
		RegularizacaoFundiaria.verificarPossuiDominioAvulso();
	},

	gerenciarOpcoes: function () {
		$('.divOpcaoOcupacao', RegularizacaoFundiaria.container).each(function (i, div) {
			var opcao = $('.radioOpcao:checked', div).val();
			var opcoes = ($('.opcaoTextoSim', div).attr('class') || '').split(' ');

			if ($('.opcaoTextoSim', div).length > 0 && ($.inArray('opcaoTextoNao', opcoes) > -1 && $.inArray('opcaoTextoSim', opcoes) > -1)) {
				$('.opcaoTextoSim', div).toggleClass('hide', false);
				return;
			}

			$('.opcaoTextoSim', div).toggleClass('hide', opcao != 1);
			$('.opcaoTextoNao', div).toggleClass('hide', opcao != 0);
		});
	},

	associarTransmitente: function () {
		RegularizacaoFundiaria.pessoaModal = new PessoaAssociar();
		RegularizacaoFundiaria.pessoaModal.associarAbrir(RegularizacaoFundiaria.settings.urls.pessoaAssociar, {
			onAssociarCallback: RegularizacaoFundiaria.callBackAssociarTransmitente,
			tituloVerificar: 'Verificar CPF/CNPJ',
			tituloCriar: 'Cadastrar Transmitente',
			tituloEditar: 'Editar Transmitente',
			tituloVisualizar: 'Visualizar Transmitente'
		});
	},

	callBackAssociarTransmitente: function (pessoaObj) {
		$('.hdnTransmitentePessoaID', RegularizacaoFundiaria.container).val(pessoaObj.Id);
		$('.txtTransmitenteNome', RegularizacaoFundiaria.container).val(pessoaObj.NomeRazaoSocial);
		$('.txtTransmitenteCpfCpnj', RegularizacaoFundiaria.container).val(pessoaObj.CPFCNPJ);
	},

	adicionarTransmitente: function () {
		Mensagem.limpar(RegularizacaoFundiaria.container);
		var tr = $('.tabTransmitentesTemplate tr', RegularizacaoFundiaria.container).clone();

		var item = {
			Id: 0,
			TempoOcupacao: $('.txtTempoOcupacao', RegularizacaoFundiaria.container).val(),
			Transmitente: {
				Id: $('.hdnTransmitentePessoaID', RegularizacaoFundiaria.container).val(),
				Fisica: {
					Nome: $('.txtTransmitenteNome', RegularizacaoFundiaria.container).val(),
					CPF: $('.txtTransmitenteCpfCpnj', RegularizacaoFundiaria.container).val()
				}
			}
		};

		var lista = new Array();
		$('.tabTransmitentes tbody tr', RegularizacaoFundiaria.container).each(function (i, linha) {
			lista.push(JSON.parse($('.hdnTransmitenteJSON', linha).val()));
		});

		var ehValido = MasterPage.validarAjax(
			RegularizacaoFundiaria.settings.urls.validarTransmitente,
			{ transmitenteLista: lista, transmitente: item },
			RegularizacaoFundiaria.container, false).EhValido;

		if (!ehValido) {
			return;
		}

		$('.hdnTransmitenteJSON', tr).val(JSON.stringify(item));
		$('.nomeRazaoSocial', tr).text(item.Transmitente.Fisica.Nome);
		$('.cpfCnpj', tr).text(item.Transmitente.Fisica.CPF);
		$('.tempoOcupacao', tr).text(item.TempoOcupacao);
		$('.tabTransmitentes tbody', RegularizacaoFundiaria.container).append(tr);

		$('.campoTransmitente', RegularizacaoFundiaria.container).unmask().val('');
		Mascara.load($('.txtTempoOcupacao', RegularizacaoFundiaria.container).closest('div'));

		Listar.atualizarEstiloTable($('.tabTransmitentes', RegularizacaoFundiaria.container));
		RegularizacaoFundiaria.configurarTransmitentes();
		$('.btnAssociarPessoa', RegularizacaoFundiaria.container).focus();
	},

	configurarTransmitentes: function () {
		var transmitentes = 0;
		var tempoOcupacao = 0;

		$('.tabTransmitentes tbody tr', RegularizacaoFundiaria.container).each(function () {
			transmitentes++;
			if ($('.tempoOcupacao', this).text() != '') {
				tempoOcupacao += Number($('.tempoOcupacao', this).text());
			}
		});

		$('.labTotalTransmitente', RegularizacaoFundiaria.container).html(transmitentes);
		$('.labTotalTempoOcupacao', RegularizacaoFundiaria.container).html(tempoOcupacao);
	},

	visualizarTransmitente: function () {
		var item = JSON.parse($('.hdnTransmitenteJSON', $(this).closest('tr')).val());

		Modal.abrir(RegularizacaoFundiaria.settings.urls.pessoaVisualizar + '/' + item.Transmitente.Id, null, function (container) {
			new PessoaAssociar().load(container, {
				tituloVisualizar: 'Visualizar Transmitente',
				editarVisualizar: false
			});
		});
	},

	removerTransmitente: function () {
		RegularizacaoFundiaria.removerLinhaGrid(this);
		RegularizacaoFundiaria.configurarTransmitentes();
	},

	somarRelacaoTrabalho: function () {
		var total = 0;
		$('.checkRelacaoTrabalho :checked', RegularizacaoFundiaria.container).each(function (i, check) {
			total += +($(check).val());
		});

		return total;
	},

	somarAreasUsoSolo: function () {
		var total = 0;
		$('.divGridUsoSolo .areaUso', RegularizacaoFundiaria.container).each(function (i, num) {
			total += +($(num).text());
		});

		$('.labTotalAreaUso', RegularizacaoFundiaria.container).text(total);
		return total;
	},

	adicionarUsoSolo: function () {
		Mensagem.limpar(RegularizacaoFundiaria.container);
		var tr = $('.tabUsoSoloTemplate tr', RegularizacaoFundiaria.container).clone();

		var item = {
			Id: 0,
			TipoDeUso: $('.ddlTipoUso', RegularizacaoFundiaria.container).val(),
			TipoDeUsoTexto: $('.ddlTipoUso :selected', RegularizacaoFundiaria.container).text(),
			AreaPorcentagem: $('.txtAreaUsoPorcentagem', RegularizacaoFundiaria.container).val()
		};

		var lista = new Array();
		$('.tabUsoSolo tbody tr', RegularizacaoFundiaria.container).each(function (i, linha) {
			lista.push(JSON.parse($('.hdnUsoSoloJSON', linha).val()));
		});

		var ehValido = MasterPage.validarAjax(
			RegularizacaoFundiaria.settings.urls.validarUsoAtualSolo,
			{ usoAtualSoloLista: lista, usoAtualSolo: item },
			RegularizacaoFundiaria.container, false).EhValido;

		if (!ehValido) {
			return;
		}

		$('.hdnUsoSoloJSON', tr).val(JSON.stringify(item));
		$('.tipoUsoTexto', tr).text(item.TipoDeUsoTexto);
		$('.areaUso', tr).text(item.AreaPorcentagem);
		$('.tabUsoSolo tbody', RegularizacaoFundiaria.container).append(tr);

		$('.txtAreaUsoPorcentagem', RegularizacaoFundiaria.container).unmask().val('');
		$('.ddlTipoUso', RegularizacaoFundiaria.container).ddlFirst();
		Mascara.load($('.txtAreaUsoPorcentagem', RegularizacaoFundiaria.container).closest('div'));

		Listar.atualizarEstiloTable($('.tabUsoSolo', RegularizacaoFundiaria.container));
		RegularizacaoFundiaria.somarAreasUsoSolo();
		$('.ddlTipoUso', RegularizacaoFundiaria.container).focus();
	},

	removerUsoSolo: function () {
		RegularizacaoFundiaria.removerLinhaGrid(this);
		RegularizacaoFundiaria.somarAreasUsoSolo();
	},

	adicionarEdificacao: function () {
		Mensagem.limpar(RegularizacaoFundiaria.container);
		var tr = $('.tabEdificacaoTemplate tr', RegularizacaoFundiaria.container).clone();

		var item = {
			Id: 0,
			Tipo: $('.txtTipoEdificacao', RegularizacaoFundiaria.container).val(),
			Quantidade: $('.txtQuantidadeEdificacao', RegularizacaoFundiaria.container).val()
		};

		var ehValido = MasterPage.validarAjax(
			RegularizacaoFundiaria.settings.urls.validarEdificacao,
			{ edificacao: item },
			RegularizacaoFundiaria.container, false).EhValido;

		if (!ehValido) {
			return;
		}

		$('.hdnEdificacaoJSON', tr).val(JSON.stringify(item));
		$('.tipo', tr).text(item.Tipo);
		$('.quantidade', tr).text(item.Quantidade);
		$('.tabEdificacao tbody', RegularizacaoFundiaria.container).append(tr);

		$('.campoEdificacao', RegularizacaoFundiaria.container).unmask().val('');
		Mascara.load($('.txtQuantidadeEdificacao', RegularizacaoFundiaria.container).closest('div'));

		Listar.atualizarEstiloTable($('.tabEdificacao', RegularizacaoFundiaria.container));
		$('.txtTipoEdificacao', RegularizacaoFundiaria.container).focus();
	},

	removerLinhaGrid: function (controle) {
		if (controle.srcElement || controle.currentTarget) {
			controle = this;
		}

		var tabela = $(controle).closest('table');
		$(controle).closest('tr').remove();
		Listar.atualizarEstiloTable(tabela);
	},

	visualizarPosse: function () {
		var item = JSON.parse($(this).closest('tr').find('.hdnItemJSon').val());
		var matriculas = RegularizacaoFundiaria.settings.matriculas;

		params = {
			id: item.Id,
			empreendimento: RegularizacaoFundiaria.settings.empreendimentoID,
			posse: item,
			matriculas: matriculas
		};

		MasterPage.carregando(true);
		$.ajax({
			url: RegularizacaoFundiaria.settings.urls.visualizarPosse,
			data: JSON.stringify(params),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(RegularizacaoFundiaria.container, response.Msg);
					return;
				}

				var container = $('.divRegularizacaoFundiaria', RegularizacaoFundiaria.container);
				container.empty();
				container.append(response.Html);

				RegularizacaoFundiaria.configurarTransmitentes();
				RegularizacaoFundiaria.gerenciarOpcoes();
				MasterPage.redimensionar();
				MasterPage.botoes(container);
				Mascara.load(container);
			}
		});
		MasterPage.carregando(false);

		$('.cancelarCaixaPrincipal', RegularizacaoFundiaria.container).closest('div').addClass('hide');
		Listar.atualizarEstiloTable($('.dataGridTable', RegularizacaoFundiaria.container));
	},

	editarPosse: function () {
		$('.tabRegularizacoesFundiarias tbody tr', RegularizacaoFundiaria.container).removeClass('editando');
		var linha = $(this).closest('tr');
		linha.addClass('editando');

		var item = JSON.parse(linha.find('.hdnItemJSon').val());
		var matriculas = RegularizacaoFundiaria.settings.matriculas;

		params = {
			id: item.Id,
			empreendimento: RegularizacaoFundiaria.settings.empreendimentoID,
			posse: item,
			matriculas: matriculas
		};

		var url = (item.Id > 0) ? RegularizacaoFundiaria.settings.urls.editarPosse : RegularizacaoFundiaria.settings.urls.criarPosse;
		MasterPage.carregando(true);
		$.ajax({
			url: url,
			data: JSON.stringify(params),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(RegularizacaoFundiaria.container, response.Msg);
					return;
				}

				var container = $('.divRegularizacaoFundiaria', RegularizacaoFundiaria.container);
				container.empty();
				container.append(response.Html);

				RegularizacaoFundiaria.changeDominioAvulso();
				RegularizacaoFundiaria.verificarPossuiDominioAvulso();
				RegularizacaoFundiaria.configurarTransmitentes();
				RegularizacaoFundiaria.gerenciarOpcoes();
				MasterPage.redimensionar();
				MasterPage.botoes(container);
				Mascara.load(container);
				Aux.setarFoco(container);
			}
		});
		MasterPage.carregando(false);

		$('.txtAreaRequerida', RegularizacaoFundiaria.container).focus();
		$('.cancelarCaixaPrincipal', RegularizacaoFundiaria.container).closest('div').addClass('hide');
		Listar.atualizarEstiloTable($('.dataGridTable', RegularizacaoFundiaria.container));
	},

	fecharPosse: function () {
		Mensagem.limpar(RegularizacaoFundiaria.container);
		$('.divRegularizacaoFundiaria', RegularizacaoFundiaria.container).empty();
		$('.cancelarCaixaPrincipal', RegularizacaoFundiaria.container).closest('div').removeClass('hide');
	},

	obterPosse: function () {
		obj = {};
		obj.DominiosAvulsos = new Array();
		obj.Opcoes = new Array();
		obj.Transmitentes = new Array();
		obj.Edificacoes = new Array();
		obj.UsoAtualSolo = new Array();

		obj.Id = $('.hdnPosseId', RegularizacaoFundiaria.container).val();
		obj.Dominio = $('.hdnDominioId', RegularizacaoFundiaria.container).val();
		obj.Zona = $('.hdnZonaLocalizacaoId', RegularizacaoFundiaria.container).val();
		obj.Identificacao = $('.hdnIdentificacao', RegularizacaoFundiaria.container).val();
		obj.ComprovacaoTexto = $('.hdnComprovacaoTexto', RegularizacaoFundiaria.container).val();
		obj.AreaCroqui = Mascara.getFloatMask($('.txtAreaTotalPosse', RegularizacaoFundiaria.container).val());
		obj.Perimetro = Mascara.getFloatMask($('.txtPerimetro', RegularizacaoFundiaria.container).val());
		obj.AreaRequerida = Mascara.getFloatMask($('.txtAreaRequerida', RegularizacaoFundiaria.container).val());
		obj.RegularizacaoTipo = $('.ddlRegularizacaoTipo', RegularizacaoFundiaria.container).val();
		obj.Benfeitorias = $('.txtBenfeitorias', RegularizacaoFundiaria.container).val();
		obj.Observacoes = $('.txtObservacoes', RegularizacaoFundiaria.container).val();
		obj.RelacaoTrabalho = RegularizacaoFundiaria.somarRelacaoTrabalho();

		obj.AreaDocumento = $('.txtAreaPosse', RegularizacaoFundiaria.container).val();

		obj.PossuiDominioAvulso = $('.radioPossuiDominioAvulso:checked', RegularizacaoFundiaria.container).val();
		$('.tabAreasAnexadas tbody tr', RegularizacaoFundiaria.container).each(function (i, linha) {
			var dominio = JSON.parse($('.hdnDominioJSON', linha).val());
			if (!dominio.Identificacao) {
				obj.DominiosAvulsos.push(dominio);
			}
		});

		$('.divOpcaoOcupacao', RegularizacaoFundiaria.container).each(function () {
			obj.Opcoes.push({
				Id: $('.hdnRadioId', this).val(),
				Tipo: $('.hdnRadioTipo', this).val(),
				Valor: $('.radioOpcao:checked', this).val(),
				Outro: $('.campoOutro:visible', this).val()
			});
		});

		$('.tabTransmitentes tbody tr', RegularizacaoFundiaria.container).each(function (i, linha) {
			obj.Transmitentes.push(JSON.parse($('.hdnTransmitenteJSON', linha).val()));
		});

		$('.tabUsoSolo tbody tr', RegularizacaoFundiaria.container).each(function (i, linha) {
			obj.UsoAtualSolo.push(JSON.parse($('.hdnUsoSoloJSON', linha).val()));
		});

		$('.tabEdificacao tbody tr', RegularizacaoFundiaria.container).each(function (i, linha) {
			obj.Edificacoes.push(JSON.parse($('.hdnEdificacaoJSON', linha).val()));
		});

		return obj;
	},

	obterAreaTotalPosse: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: RegularizacaoFundiaria.settings.urls.obterAreaTotalPosse,
			data: JSON.stringify({ dominio: $('.hdnDominioId', RegularizacaoFundiaria.container).val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.txtAreaTotalPosse', RegularizacaoFundiaria.container).val(response.AreaTotalPosse);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(RegularizacaoFundiaria.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	obterPerimetroPosse: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: RegularizacaoFundiaria.settings.urls.obterPerimetroPosse,
			data: JSON.stringify({ dominio: $('.hdnDominioId', RegularizacaoFundiaria.container).val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.txtPerimetro', RegularizacaoFundiaria.container).val(response.PerimetroPosse);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(RegularizacaoFundiaria.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	validarPosse: function (obj) {
		return MasterPage.validarAjax(
		RegularizacaoFundiaria.settings.urls.validarPosse,
		{
			posse: obj,
			empreendimentoId: RegularizacaoFundiaria.settings.empreendimentoID
		}, RegularizacaoFundiaria.container, false).EhValido;
	},

	salvarPosse: function () {
		var linha = RegularizacaoFundiaria.container.find('.editando');
		var posse = RegularizacaoFundiaria.obterPosse();

		if (RegularizacaoFundiaria.validarPosse(posse)) {
			$('.areaRequerida', linha).html(Mascara.getStringMask(posse.AreaRequerida)).attr('title', Mascara.getStringMask(posse.AreaRequerida));
			$('.areaCroqui', linha).html(Mascara.getStringMask(posse.AreaCroqui, 'n2')).attr('title', Mascara.getStringMask(posse.AreaCroqui, 'n2'));
			$('.hdnItemJSon', linha).val(JSON.stringify(posse));
			RegularizacaoFundiaria.fecharPosse();
		}

		var mostrarSalvar = true;
		$('.tabRegularizacoesFundiarias tbody tr', RegularizacaoFundiaria.container).each(function () {
			var objeto = JSON.parse($(this).find('.hdnItemJSon').val());

			if (objeto.AreaRequerida == 0) {
				mostrarSalvar = false;
			}
		});

		if (mostrarSalvar) {
			$('.btnSalvar', RegularizacaoFundiaria.container).removeClass('hide');
			$('.btnModalOu', RegularizacaoFundiaria.container).removeClass('hide');
		}
	},

	obter: function () {
		var obj = {
			Id: RegularizacaoFundiaria.settings.caracterizacaoID,
			EmpreendimentoId: RegularizacaoFundiaria.settings.empreendimentoID,
			Dependencias: JSON.parse(RegularizacaoFundiaria.settings.dependencias),
			Posses: []
		};

		$('.tabRegularizacoesFundiarias tbody tr', RegularizacaoFundiaria.container).each(function (i, linha) {
			obj.Posses.push(JSON.parse($('.hdnItemJSon', linha).val()));
		});

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: RegularizacaoFundiaria.settings.urls.salvar,
			data: JSON.stringify(RegularizacaoFundiaria.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					RegularizacaoFundiaria.abrirModalMerge(response.TextoMerge);
					return;
				}

				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
					return;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(RegularizacaoFundiaria.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

Dominios = {
	settings: {
		urls: {
			editar: '',
			visualizar: ''
		},
		idsTela: null,
		salvarCallBack: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Dominios.settings, options); }
		Dominios.container = MasterPage.getContent(container);

		Dominios.container.delegate('.btnDominioVisualizar', 'click', Dominios.visualizar);
		Dominios.container.delegate('.btnDominioEditar', 'click', Dominios.editar);
	},

	obterMatriculasFolhaslivros: function (dominioRemover) {
		var array = new Array();

		$('.hdnItemJSon', Dominios.container).each(function () {
			dominio = JSON.parse($(this).val());

			if (dominio.Tipo == Dominios.settings.idsTela.TipoMatriculaId && dominio.Matricula != null && dominio.Matricula != '' && dominio.Identificacao != dominioRemover) {
				array.push({ Id: dominio.Identificacao, Texto: dominio.Matricula + ' - ' + dominio.Folha + ' - ' + dominio.Livro });
			}
		});

		return array;
	},

	visualizar: function () {
		var dominio = JSON.parse($(this).closest('tr').find('.hdnItemJSon').val());
		Modal.abrir(Dominios.settings.urls.visualizar, dominio, function (container) {
			Dominio.load(container);
		}, Modal.tamanhoModalGrande);
	},

	editar: function () {
	    var linha = $(this).closest('tr');
		linha.closest('tbody').find('tr').removeClass('editando');
		linha.addClass('editando');

		//var id = $('.hdnEmpreendimentoId', Dominio.container).val();
		var id = $('.hdnEmpreendimentoId').val();

		var dominio = JSON.parse(linha.find('.hdnItemJSon').val());

		var novoobj = {
		    dominio: dominio,
		    empreendimento: id
		};

		Modal.abrir(Dominios.settings.urls.editar, novoobj, function (container) {
			Dominio.load(container, { salvarCallBack: Dominios.salvarCallBack });
		}, Modal.tamanhoModalGrande);
	},

	salvarCallBack: function (dominio) {
		var matriculaComprovacao = dominio.Matricula || dominio.ComprovacaoTexto || '';
		$('.editando', Dominios.container).find('.matriculaComprovacao').html(matriculaComprovacao).attr('title', matriculaComprovacao);
		$('.editando', Dominios.container).find('.hdnItemJSon').val(JSON.stringify(dominio));

		if (typeof Dominios.settings.salvarCallBack == 'function') {
			Dominios.settings.salvarCallBack();
		}
	},

	obter: function () {
		var dominios = [];
		$('.hdnItemJSon', Dominios.container).each(function () {
			dominios.push(JSON.parse($(this).val()));
		});
		return dominios;
	}
}

Dominio = {
	settings: {
		urls: {
			reservaLegal: '',
			reservaLegalVisualizar: '',
			validarSalvar: ''
		},
		mensagens: null,
		salvarCallBack: null
	},
	container: null,
	isVisualizar: false,
	idsTela: null,

	load: function (container, options) {
		if (options) { $.extend(Dominio.settings, options); }
		Dominio.container = MasterPage.getContent(container);

		Dominio.container.delegate('.btnReservaAdicionar', 'click', Dominio.reservaAdicionar);
		Dominio.container.delegate('.btnReservaEditar', 'click', Dominio.reservaEditar);
		Dominio.container.delegate('.btnReservaVisualizar', 'click', Dominio.reservaVisualizar);
		Dominio.container.delegate('.btnReservaExcluir', 'click', Dominio.reservaExcluir);
		Dominio.container.delegate('.ddlComprovacao', 'change', Dominio.gerenciarComprovacao);
		Dominio.container.delegate('.titFiltros', 'click', Aux.expadirFieldSet);

		Dominio.isVisualizar = ($('.btnReservaEditar', Dominio.container).length <= 0);
		Dominio.gerenciarComprovacao();
		Dominio.configurarObjeto();

		if (Dominio.isVisualizar) {
			Modal.defaultButtons(Dominio.container);
		} else {
			Modal.defaultButtons(Dominio.container, Dominio.salvar, 'Salvar');
			Aux.setarFoco(Dominio.container);
		}
	},

	configurarObjeto: function () {
		$('.tabReservasLegais tr:not(.trTemplateRow) .hdnItemJSon', Dominio.container).each(function () {
			var reserva = JSON.parse($(this).val());

			if (reserva.Coordenada.NorthingUtm == null) {
				return;
			}

			reserva.Coordenada.NorthingUtm = reserva.Coordenada.NorthingUtm.toString().replace('.', ',');
			reserva.Coordenada.EastingUtm = reserva.Coordenada.EastingUtm.toString().replace('.', ',');

			$(this).val(JSON.stringify(reserva));
		});
	},

	gerenciarComprovacao: function () {
		var comprovacao = Number($('.ddlComprovacao', Dominio.container).val()) || 0;

		$('.asteriscoRegistro', Dominio.container).addClass('hide');
		$('.asteriscoAreaMatDoc', Dominio.container).addClass('hide');
		$('.divRegistro', Dominio.container).addClass('hide');

		if (comprovacao != Dominio.idsTela.DominioComprovacaoPosseiroPrimitivo) {
			$('.divRegistro', Dominio.container).removeClass('hide');
			$('.asteriscoAreaMatDoc', Dominio.container).removeClass('hide');
		}

		if (comprovacao != Dominio.idsTela.DominioComprovacaoRecibo &&
			comprovacao != Dominio.idsTela.DominioComprovacaoCertidaoPrefeitura &&
			comprovacao != Dominio.idsTela.DominioComprovacaoContratoCompraVenda &&
			comprovacao != Dominio.idsTela.DominioComprovacaoDeclaracao &&
			comprovacao != Dominio.idsTela.DominioComprovacaoOutros &&
			comprovacao != Dominio.idsTela.DominioCertificadoCadastroImovelRuralCCIR) {
			$('.asteriscoRegistro', Dominio.container).removeClass('hide');
		}
	},

	reservaAdicionar: function () {
		$(this).closest('fieldset').find('.dataGridTable tbody tr').removeClass('editando');
		var visualizar = $(this).hasClass('visualizar');

		Modal.abrir(Dominio.settings.urls.reservaLegal, null, function (container) {
			ReservaLegal.load(container, { salvarCallBack: Dominio.reservaSalvar, isVisualizar: visualizar, dominioIdentificacao: $('.txtIdentificacao', Dominio.container).val() });
		}, Modal.tamanhoModalGrande);
	},

	reservaEditar: function () {
		var visualizar = $(this).hasClass('visualizar');
		var linha = $(this).closest('tr');
		var reserva = JSON.parse($(this).closest('tr').find('.hdnItemJSon').val());
		reserva.EmpreendimentoId = +$('.hdnEmpreendimentoId', Dominios.container).val();
		var dominioTipo = $('input[type="radio"]:checked', Dominio.container).val();

		linha.closest('tbody').find('tr').removeClass('editando');
		linha.addClass('editando');

		Modal.abrir(Dominio.settings.urls.reservaLegal, { reservaLegal: reserva, dominioTipo: dominioTipo }, function (container) {
			ReservaLegal.load(container, { salvarCallBack: Dominio.reservaSalvar, isVisualizar: visualizar, dominioIdentificacao: $('.txtIdentificacao', Dominio.container).val() });
		}, Modal.tamanhoModalGrande);
	},

	reservaSalvar: function (reserva) {
		var termoMatricula = '';
		var areaARL = reserva.ARLCroqui || reserva.ARLCedida || reserva.ARLRecebida || '';
				
		if (areaARL) {
			areaARL = Mascara.getStringMask(areaARL);
		} else {
			areaARL = '---';
		}

		if (reserva.NumeroTermo) {
			termoMatricula = reserva.NumeroTermo;
		}

		if (reserva.MatriculaIdentificacao) {
			if (reserva.NumeroTermo) {
				termoMatricula = termoMatricula + '/' + reserva.MatriculaTexto;
			} else {
				termoMatricula = reserva.MatriculaTexto;
			}
		}
			
		var linha = null;

		if ($('.editando', Dominio.container).length > 0) {
			linha = $('.editando', Dominio.container);
			linha.find('.situacao').html(reserva.SituacaoTexto || '').attr('title', reserva.SituacaoTexto || '');
			linha.find('.compensacao').html(reserva.Compensacao || '').attr('title', reserva.Compensacao || '');
			linha.find('.situacaoVegetal').html(reserva.SituacaoVegetalTexto || '').attr('title', reserva.SituacaoVegetalTexto || '');
			linha.find('.termoMatriculaCompensada').html(termoMatricula).attr('title', termoMatricula);
			linha.find('.areaRL').html(areaARL).attr('title', areaARL);

			linha.find('.hdnItemJSon').val(JSON.stringify(reserva));
		} else {
			linha = $('.trTemplateRow', Dominio.container).clone().removeClass('trTemplateRow hide');
			linha.find('.hdnItemJSon').val(JSON.stringify(reserva));
			linha.find('.situacao').html(reserva.SituacaoTexto || '').attr('title', reserva.SituacaoTexto || '');
			linha.find('.termoMatriculaCompensada').html(termoMatricula).attr('title', termoMatricula);
			linha.find('.compensacao').html(reserva.Compensacao || '').attr('title', reserva.Compensacao || '');
			linha.find('.situacaoVegetal').html(reserva.SituacaoVegetalTexto || '').attr('title', reserva.SituacaoVegetalTexto || '');
			linha.find('.areaRL').html(areaARL).attr('title', areaARL);

			$('.dataGridTable tbody:last', Dominio.container).append(linha);
			Listar.atualizarEstiloTable(Dominio.container.find('.dataGridTable'));
		}
	},

	reservaVisualizar: function () {
		var reserva = JSON.parse($(this).closest('tr').find('.hdnItemJSon').val());
		reserva.EmpreendimentoId = +$('.hdnEmpreendimentoId', Dominios.container).val();

		var visualizar = $(this).hasClass('visualizar');

		Modal.abrir(Dominio.settings.urls.reservaLegalVisualizar, reserva, function (container) {
			ReservaLegal.load(container, { isVisualizar: visualizar, dominioIdentificacao: $('.txtIdentificacao', Dominio.container).val() });
		}, Modal.tamanhoModalGrande);
	},

	reservaExcluir: function () {
		var linha = $(this).closest('tr');

		Modal.confirma({
			btnOkCallback: function (conteudoModal) {
				linha.remove();
				Modal.fechar(conteudoModal); return false;
				Listar.atualizarEstiloTable(ReservaLegal.container.find('.dataGridTable'));
			},
			btnOkLabel: 'Excluir',
			conteudo: Dominio.settings.mensagens.ReservaLegalExcluir.Texto,
			titulo: 'Excluir Reserva Legal',
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	obterObjeto: function () {

		var objJson = JSON.parse($('.hdnDominioJson', Dominio.container).val());

		var objeto = {
			Id: objJson.DominioId,
			Tipo: $('.rdbTipo:checked', Dominio.container).val(),
			Identificacao: $('.txtIdentificacao', Dominio.container).val(),
			Matricula: $('.txtMatricula', Dominio.container).val(),
			Folha: $('.txtFolha', Dominio.container).val(),
			Livro: $('.txtLivro', Dominio.container).val(),
			Cartorio: $('.txtCartorio', Dominio.container).val(),
			ComprovacaoId: $('.ddlComprovacao', Dominio.container).val(),
			ComprovacaoTexto: $('.ddlComprovacao :selected', Dominio.container).text(),
			AreaCroqui: objJson.AreaCroqui,
			DataUltimaAtualizacao: { DataTexto: $('.txtDataUltimaAtualizacao', Dominio.container).val() },
			EmpreendimentoLocalizacao: objJson.EmpreendimentoLocalizacao,
			AreaDocumento: Mascara.getFloatMask($('.txtAreaDocumento', Dominio.container).val()) || 0.0,
			AreaDocumentoTexto: $('.txtAreaDocumento', Dominio.container).val(),
			DescricaoComprovacao: $('.txtRegistro', Dominio.container).val(),
			NumeroCCIR: $('.txtNumeroCCIR', Dominio.container).val(),
			AreaCCIR: Mascara.getFloatMask($('.txtAreaCCIR', Dominio.container).val()),
			AreaCCIRTexto: $('.txtAreaCCIR', Dominio.container).val(),
			APPCroqui: objJson.APPCroqui,
			ARLDocumento: Mascara.getFloatMask($('.txtARLDocumento', Dominio.container).val()) || 0.0,
			ARLDocumentoTexto: $('.txtARLDocumento', Dominio.container).val(),
			ConfrontacaoNorte: $('.txtConfrontacaoNorte', Dominio.container).val(),
			ConfrontacaoSul: $('.txtConfrontacaoSul', Dominio.container).val(),
			ConfrontacaoLeste: $('.txtConfrontacaoLeste', Dominio.container).val(),
			ConfrontacaoOeste: $('.txtConfrontacaoOeste', Dominio.container).val(),
			ReservasLegais: []
		}

		$('.hdnItemJSon', Dominio.container).each(function () {
			if (!$(this).closest('tr').hasClass('hide')) {
				objeto.ReservasLegais.push(JSON.parse($(this).val()));
			}
		});

		return objeto;
	},

	salvar: function () {
		var objeto = Dominio.obterObjeto();

		if (MasterPage.validarAjax(Dominio.settings.urls.validarSalvar, objeto, Dominio.container, false).EhValido) {
			Modal.fechar(Dominio.container);

			if (typeof Dominio.settings.salvarCallBack == 'function') {
				Dominio.settings.salvarCallBack(objeto);
			}
		}
	}
}

ReservaLegal = {
	settings: {
		urls: {
			validarSalvar: '',
			empreendimentoAssociar: '',
			empreendimentoAssociarCompensacao:'',
			obterDadosEmpreendimentoCompensacao: '',
			obterARLCompensacao: '',
			coordenadaGeo: '',
			obterARLCedente:''
		},
		idsTela: null,
		dominioIdentificacao: null,
		isVisualizar: false,
		salvarCallBack: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(ReservaLegal.settings, options); }
		ReservaLegal.container = MasterPage.getContent(container);

		ReservaLegal.container.delegate('.ddlSituacao', 'change', ReservaLegal.changeDdlSituacao);
		ReservaLegal.container.delegate('.ddlLocalizacao', 'change', ReservaLegal.gerenciarCampos);
		ReservaLegal.container.delegate('.btnBuscarEmpreendimento', 'click', ReservaLegal.abrirModalEmpreendimento);
		ReservaLegal.container.delegate('.ddlMatriculasCompensacao', 'change', ReservaLegal.carregarARLIdentificaoCompensacao);
		ReservaLegal.container.delegate('.ddlCedentePossuiCodigoEmpreendimento', 'change', ReservaLegal.cedentePossuiEmpreendimentoChange);
		ReservaLegal.container.delegate('.btnBuscarCoordenada', 'click', ReservaLegal.onBuscarCoordenada);
		ReservaLegal.container.delegate('.ddlARLIdentificacaoCompensacao', 'change', ReservaLegal.changeDdlARLIdentificacaoCompensacao);

		if (ReservaLegal.settings.isVisualizar) {
			Modal.defaultButtons(ReservaLegal.container);
		} else {
			Modal.defaultButtons(ReservaLegal.container, ReservaLegal.salvar, 'Salvar');
			Aux.setarFoco(ReservaLegal.container);
		}

		var objJson = JSON.parse($('.hdnReservaLegalJson', ReservaLegal.container).val());

		if (!ReservaLegal.settings.isVisualizar) {
			$('.ddlMatriculasFolhasLivros', ReservaLegal.container).ddlLoad(
			Dominios.obterMatriculasFolhaslivros(ReservaLegal.settings.dominioIdentificacao),
			{ selecionado: objJson.MatriculaIdentificacao });
		}
	},

	cedentePossuiEmpreendimentoChange: function () {
		$('.divCompensacao', ReservaLegal.container).addClass('hide');
		$('.coordenada', ReservaLegal.container).addClass('hide');

		if (+$(this).val() == 1) {
			$('.divCompensacao', ReservaLegal.container).removeClass('hide');
		}

		if (+$(this).val() == 0) {
			$('.coordenada', ReservaLegal.container).removeClass('hide');
		}

		ReservaLegal.gerenciarCampos();
	},

	changeDdlSituacao: function () {
		var situacao = $('.ddlSituacao', ReservaLegal.container).val();
		$('.divCamposSituacaoRegistrada', ReservaLegal.container).toggleClass('hide', situacao != ReservaLegal.settings.idsTela.SituacaoRegistradaId);
		$('.ddlLocalizacao', ReservaLegal.container).closest('div').toggleClass('hide', situacao == ReservaLegal.settings.idsTela.SituacaoNaoInformadaId);

		if (situacao == ReservaLegal.settings.idsTela.SituacaoNaoInformadaId) {
			$('.ddlCedentePossuiCodigoEmpreendimento', ReservaLegal.container).val(-1);
		}

		ReservaLegal.gerenciarCampos();
	},

	changeDdlARLIdentificacaoCompensacao: function () {
		if (+$(this).val() <=0) {
			return;
		}

		$.ajax({
			url: ReservaLegal.settings.urls.obterARLCedente,
			data: JSON.stringify({ reservaCedenteId: $(this).val() }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(ReservaLegal.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Valido) {
					$('.hdnDadosCedente', ReservaLegal.container).val(response.ReservaCedenteJson);
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ReservaLegal.container, response.Msg);
				}
			}
		});
	},

	gerenciarCampos: function () {
		var localizacao = $('.ddlLocalizacao', ReservaLegal.container).val();
		var situacao = $('.ddlSituacao', ReservaLegal.container).val();

		if ((localizacao == ReservaLegal.settings.idsTela.LocalizacaoCompensacaoMatriculaCedenteId || localizacao == ReservaLegal.settings.idsTela.LocalizacaoCompensacaoMatriculaReceptoraId) &&
		(situacao == ReservaLegal.settings.idsTela.SituacaoPropostaId || situacao == ReservaLegal.settings.idsTela.SituacaoRegistradaId)) {
			$('.ddlMatriculasFolhasLivros', ReservaLegal.container).closest('div').removeClass('hide');
		} else {
			$('.ddlMatriculasFolhasLivros', ReservaLegal.container).closest('div').addClass('hide');
		}

		$('.divCompensacao', ReservaLegal.container).addClass('hide');
		$('.receptora', ReservaLegal.container).addClass('hide');

		switch (+localizacao) {
			case ReservaLegal.settings.idsTela.LocalizacaoCompensacaoMatriculaCedenteId:
			case ReservaLegal.settings.idsTela.LocalizacaoCompensacaoEmpreendimentoCedenteId:
				$('.divCompensacao', ReservaLegal.container).removeClass('hide');
				break;
			case ReservaLegal.settings.idsTela.LocalizacaoCompensacaoMatriculaReceptoraId:
			case ReservaLegal.settings.idsTela.LocalizacaoCompensacaoEmpreendimentoReceptora:
				if (+$('.ddlCedentePossuiCodigoEmpreendimento', ReservaLegal.container).val() == 1) {
					$('.divCompensacao', ReservaLegal.container).removeClass('hide');
				}

				if (situacao == ReservaLegal.settings.idsTela.SituacaoRegistradaId && +$('.ddlCedentePossuiCodigoEmpreendimento', ReservaLegal.container).val() == 0) {
					$('.receptora', ReservaLegal.container).removeClass('hide');
				}

				break;
		}

		$('.btnBuscarCoordenada', ReservaLegal.container).closest('div').addClass('hide');
		if ((localizacao == ReservaLegal.settings.idsTela.LocalizacaoCompensacaoMatriculaReceptoraId || localizacao == ReservaLegal.settings.idsTela.LocalizacaoCompensacaoEmpreendimentoReceptora ||
            situacao == ReservaLegal.settings.idsTela.SituacaoNaoInformadaId) && +$('.ddlCedentePossuiCodigoEmpreendimento', ReservaLegal.container).val() == 0) {
			$('.btnBuscarCoordenada', ReservaLegal.container).closest('div').removeClass('hide');
		}

		$('.divIdentificacaoARL', ReservaLegal.container).toggleClass('hide',
            localizacao != ReservaLegal.settings.idsTela.LocalizacaoCompensacaoMatriculaReceptoraId &&
            localizacao != ReservaLegal.settings.idsTela.LocalizacaoCompensacaoEmpreendimentoReceptora);

		$('.cedente', ReservaLegal.container).toggleClass('hide',
            localizacao != ReservaLegal.settings.idsTela.LocalizacaoCompensacaoMatriculaCedenteId &&
            localizacao != ReservaLegal.settings.idsTela.LocalizacaoCompensacaoEmpreendimentoCedenteId);

		$('.ddlCedentePossuiCodigoEmpreendimento', ReservaLegal.container).closest('.receptora').toggleClass('hide',
            localizacao != ReservaLegal.settings.idsTela.LocalizacaoCompensacaoMatriculaReceptoraId &&
            localizacao != ReservaLegal.settings.idsTela.LocalizacaoCompensacaoEmpreendimentoReceptora ||
            situacao == ReservaLegal.settings.idsTela.SituacaoNaoInformadaId)

	},

	obterObjeto: function () {
		
		var objJson = JSON.parse($('.hdnReservaLegalJson', ReservaLegal.container).val());
		var objeto = {
			Id: objJson.ReservaLegalId
		};
		if ($('.ddlSituacao :selected', ReservaLegal.container).val() == ReservaLegal.settings.idsTela.SituacaoNaoInformadaId) {
			objeto.SituacaoId = $('.ddlSituacao :selected', ReservaLegal.container).val();
			objeto.SituacaoTexto = $('.ddlSituacao :selected', ReservaLegal.container).text();
			
			return objeto;
		}

		objeto = {
			Id: objJson.ReservaLegalId,
			SituacaoId: $('.ddlSituacao', ReservaLegal.container).val(),
			SituacaoTexto: $('.ddlSituacao :selected', ReservaLegal.container).text(),
			LocalizacaoId: $('.ddlLocalizacao:visible', ReservaLegal.container).val(),
			Identificacao: $('.txtIdentificacao', ReservaLegal.container).val(),
			SituacaoVegetalId: $('.ddlSituacaoVegetal:visible :selected', ReservaLegal.container).val() || objJson.ReservaLegalSituacaoVegetalId,
			SituacaoVegetalTexto: $('.txtSituacaoVegetal', ReservaLegal.container).val() || $('.ddlSituacaoVegetal:visible :selected', ReservaLegal.container).text(),
			ARLCroqui: objJson.ARLCroqui,
			NumeroTermo: $('.txtNumeroTermo:visible', ReservaLegal.container).val(),
			TipoCartorioId: $('.ddlTipoCartorio', ReservaLegal.container).val(),
			MatriculaIdentificacao: $('.ddlMatriculasFolhasLivros:visible', ReservaLegal.container).val(),
			MatriculaNumero: $('.txtMatriculaNumero:visible', ReservaLegal.container).val(),
			MatriculaTexto: $('.ddlMatriculasFolhasLivros :selected', ReservaLegal.container).text(),
			AverbacaoNumero: $('.txtAverbacaoNumero:visible', ReservaLegal.container).val(),
			ARLRecebida: Mascara.getFloatMask($('.txtARLRecebida:visible', ReservaLegal.container).val()),
			MatriculaId: $('.ddlMatriculasCompensacao:visible :selected', ReservaLegal.container).val(),
			IdentificacaoARLCedente: $('.ddlARLIdentificacaoCompensacao:visible :selected', ReservaLegal.container).val(),
			Compensada: objJson.ReservaLegalCompensada,
			Compensacao: '',
			Excluir : objJson.Excluir,
			NumeroCartorio: $('.txtNumeroCartorio:visible', ReservaLegal.container).val(),
			NomeCartorio: $('.txtNomeCartorio:visible', ReservaLegal.container).val(),
			NumeroFolha: $('.txtNumeroFolha:visible', ReservaLegal.container).val(),
			NumeroLivro: $('.txtNumeroLivro:visible', ReservaLegal.container).val(),
			ARLCedida: Mascara.getFloatMask($('.txtARLCedida:visible', ReservaLegal.container).val()),
			CedentePossuiEmpreendimento: +$('.ddlCedentePossuiCodigoEmpreendimento :selected', ReservaLegal.container).val(),
			EmpreendimentoId: +$('.hdnEmpreendimentoId', Dominios.container).val(),
			EmpreendimentoCompensacao: {
				Id: $('.hdnEmpreendimentoCompensacaoId', ReservaLegal.container).val(),
				Denominador: $('.txtEmpreendimentoCompDenominador', ReservaLegal.container).val(),
				Codigo: $('.txtEmpreendimentoCompCodigo', ReservaLegal.container).val(),
				CNPJ: $('.txtEmpreendimentoCompCNPJ', ReservaLegal.container).val()
			},
			Coordenada: {
				EastingUtm: $('.txtEasting:visible', ReservaLegal.container).val(),
				NorthingUtm: $('.txtNorthing:visible', ReservaLegal.container).val(),
				Tipo: { Id: $('.ddlCoordenadaTipo:visible :selected', ReservaLegal.container).val() },
				Datum: { Id: $('.ddlDatum:visible :selected', ReservaLegal.container).val() }
			}
		};

		if (!$('.txtEmpreendimentoCompDenominador', ReservaLegal.container).is(':visible')) {
			objeto.EmpreendimentoCompensacao = {};
		}

		if (objeto.Compensada) {
			objeto.Compensacao = "Cedente";
		}

		if (parseInt(objeto.LocalizacaoId) == ReservaLegal.settings.idsTela.LocalizacaoCompensacaoEmpreendimentoReceptora ||
            parseInt(objeto.LocalizacaoId) == ReservaLegal.settings.idsTela.LocalizacaoCompensacaoMatriculaReceptoraId) {

			objeto.Compensacao = "Receptora";
			objeto.Compensada = false;

			if (objeto.CedentePossuiEmpreendimento == 1) {
				
				var dadosCedente = $('.hdnDadosCedente', ReservaLegal.container).val();

				if (dadosCedente) {
					dadosCedente = JSON.parse(dadosCedente);
					objeto.SituacaoVegetalId = dadosCedente.SituacaoVegetalId;
					objeto.SituacaoVegetalTexto = dadosCedente.SituacaoVegetalTexto;
					objeto.ARLCedida = dadosCedente.Area;
				}
			}
		}
				
		return objeto;
	},

	salvar: function () {
		var reservas = [];
		var aux = $('.tabDominios tbody tr:not(.editando)').find('.hdnItemJSon').val();
		if (aux) {
			aux = JSON.parse(aux);
			$(aux.ReservasLegais).each(function () { reservas.push(this); });
		}

		var reserva = ReservaLegal.obterObjeto();

		aux = $('.tabReservasLegais tbody tr:not(.trTemplateRow, .editando)').find('.hdnItemJSon');
		$(aux).each(function () { reservas.push(JSON.parse($(this).val())); });

		var parametros = { reserva: reserva, reservasAdicionadas: reservas };
		if (MasterPage.validarAjax(ReservaLegal.settings.urls.validarSalvar, parametros, ReservaLegal.container, false).EhValido) {
			Modal.fechar(ReservaLegal.container);

			if (typeof ReservaLegal.settings.salvarCallBack == 'function') {
				ReservaLegal.settings.salvarCallBack(reserva);
			}
		}
	},

	abrirModalEmpreendimento: function () {
		var reserva = ReservaLegal.obterObjeto();
		var url = '';
		var empreendimentoId = $('.hdnEmpreendimentoId', Dominios.container).val();

		if (reserva.Compensacao == "Receptora") {
			url = ReservaLegal.settings.urls.empreendimentoAssociarCompensacao + "/?empreendimentoCompensacao=" + empreendimentoId;
		}else{
			url = ReservaLegal.settings.urls.empreendimentoAssociar;
		}

		Modal.abrir(url, null, function (content) {
			EmpreendimentoListar.load(content, { associarFuncao: ReservaLegal.associarEmpreendimento });
			Modal.defaultButtons(content);
		}, Modal.tamanhoModalMedia);
	},

	associarEmpreendimento: function (resposta) {

		if (resposta.Id == +$('.hdnEmpreendimentoId', Dominios.container).val()) {
			return [ReservaLegal.settings.mensagens.EmpreendimentoCedente];
		}

		var sucesso = false;
		$.ajax({
			url: ReservaLegal.settings.urls.obterDadosEmpreendimentoCompensacao,
			data: JSON.stringify({ empreendimentoId: resposta.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(ReservaLegal.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Valido) {

					$('.txtEmpreendimentoCompCodigo', ReservaLegal.container).val(resposta.Codigo);
					$('.txtEmpreendimentoCompDenominador', ReservaLegal.container).val(resposta.Denominador);
					$('.txtEmpreendimentoCompCNPJ', ReservaLegal.container).val(resposta.CNPJ);
					$('.hdnEmpreendimentoCompensacaoId', ReservaLegal.container).val(resposta.Id);

					if (response.Dominios) {
						$('.ddlMatriculasCompensacao', ReservaLegal.container).ddlLoad(response.Dominios);
					}

					sucesso = true;
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ReservaLegal.container, response.Msg);
					sucesso = false;
				}

			}
		});

		if (sucesso && (+$('.ddlMatriculasCompensacao :selected', ReservaLegal.container).val() > 0) && $('.ddlARLIdentificacaoCompensacao', ReservaLegal.constructor).is(':visible')) {
			$('.ddlMatriculasCompensacao', ReservaLegal.container).trigger('change');
		}

		return sucesso;
	},

	carregarARLIdentificaoCompensacao: function () {
		var matriculaId = +$(this).val();
		if (matriculaId < 1) {
			return;
		}

		$.ajax({
			url: ReservaLegal.settings.urls.obterARLCompensacao,
			data: JSON.stringify({ empreendimentoId: +$('.hdnEmpreendimentoId', Dominios.container).val(), dominio: matriculaId }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(ReservaLegal.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Valido) {

					if (response.Compensacoes) {
						$('.ddlARLIdentificacaoCompensacao', ReservaLegal.container).ddlLoad(response.Compensacoes);
						$('.ddlARLIdentificacaoCompensacao', ReservaLegal.container).removeClass('disabled');
						$('.ddlARLIdentificacaoCompensacao', ReservaLegal.container).removeAttr('disabled');
						$('.ddlARLIdentificacaoCompensacao', ReservaLegal.container).trigger('change');
					}
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ReservaLegal.container, response.Msg);
				}
			}
		});
	},

	onBuscarCoordenada: function () {
		Modal.abrir(ReservaLegal.settings.urls.coordenadaGeo, null, function (container) {
			Coordenada.load(container, {
				northing: $('.txtNorthing', ReservaLegal.container).val(),
				easting: $('.txtEasting', ReservaLegal.container).val(),
				pagemode: 'editMode',
				callBackSalvarCoordenada: ReservaLegal.setarCoordenada
			});
			Modal.defaultButtons(container);
		},
		Modal.tamanhoModalGrande);
	},

	setarCoordenada: function (retorno) {
		retorno = JSON.parse(retorno);

		$('.txtNorthing', ReservaLegal.container).val(retorno.northing);
		$('.txtEasting', ReservaLegal.container).val(retorno.easting);
		$('.btnBuscarCoordenada', ReservaLegal.container).focus();
	}
}
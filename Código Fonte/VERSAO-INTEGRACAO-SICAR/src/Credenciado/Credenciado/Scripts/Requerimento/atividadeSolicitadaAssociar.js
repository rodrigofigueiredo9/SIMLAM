/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../mensagem.js" />

AtividadeSolicitadaAssociar = {
	container: null,
	urlAbriModalAtividade: null,
	urlValidarExcluirAtividadeFinalidade: null,
	containerAtividade: null,
	Mensagens: {},
	onCallBackItemAssociado: null,

	load: function (container, onCallBackItemAssociado) {

		AtividadeSolicitadaAssociar.onCallBackItemAssociado = onCallBackItemAssociado;
		AtividadeSolicitadaAssociar.container = MasterPage.getContent(container);

		container.delegate('.btnExcluirAtividade', 'click', AtividadeSolicitadaAssociar.onExcluirAtividadeFinalidade);
		container.delegate('.btnAssociarFinalidade', 'click', AtividadeSolicitadaAssociar.onAbrirModalFinalidade);
		container.delegate('.divDetalhes', 'click', AtividadeSolicitadaAssociar.onExpandirMaisINformacao);
		Modal.defaultButtons(AtividadeSolicitadaAssociar.container);
	},

	onAbrirModalFinalidade: function () {

		AtividadeSolicitadaAssociar.containerAtividade = $(this).closest('.conteudoAtividadeSolicitada');
		var AtividadeId = AtividadeSolicitadaAssociar.containerAtividade.find('.hdnAtividadeId').val();

		if (AtividadeId == 0) {
			Mensagem.gerar(MasterPage.getContent(AtividadeSolicitadaAssociar.container), new Array(AtividadeSolicitadaAssociar.Mensagens.AssocieAtividade));
			return;
		}

		Modal.abrir(AtividadeSolicitadaAssociar.urlAbriModalAtividade, { id: AtividadeId }, function (container) { FinalidadeAssociar.load(container, AtividadeSolicitadaAssociar.onAssociarFinalidade); }, Modal.tamanhoModalGrande);
	},

	onExcluirAtividadeFinalidade: function () {

		var atividadeId = $(this).closest('.asmItemContainer').find('.hdnAtividadeId').val();
		var protocoloId = $(this).closest('.requerimentoPartial').find('.hdnProtocoloId').val();
		var protocoloTipo = $(this).closest('.requerimentoPartial').find('.hdnProtocoloTipo').val();
		var modeloId = $(this).parent().find('.hdnTituloModeloId').val();

		if (protocoloId > 0) {
			var retorno = MasterPage.validarAjax(AtividadeSolicitadaAssociar.urlValidarExcluirAtividadeFinalidade, { protocolo: protocoloId, isProcesso: (protocoloTipo == 1), atividade: atividadeId, modelo: modeloId }, AtividadeSolicitadaAssociar.container, false);
			if (!retorno.EhValido) return;
		}

		if ($(this).closest('.listaObjetos').find('li').length == 1) {

			var linha = $('.templateFinalidade:first', $(this).closest('.conteudoAtividadeSolicitada')).clone();

			linha.addClass('Nenhum');
			$('.finalidadeTexto', linha).text('Não existe finalidade adicionada.');
			$('.tituloModeloTexto', linha).text('Não existe título adicionado.');
			$('.divDetalhes', linha).remove();
			$('.btnExcluirAtividade', linha).remove();
			linha.removeClass('hide templateFinalidade');
			$('.listaObjetos', $(this).closest('.conteudoAtividadeSolicitada')).append(linha);
		}
		$(this).closest('li').remove();
	},

	onExpandirMaisINformacao: function () {

		var maisInfo = $('.maisInfo', $(this).closest('li'));

		if (maisInfo.is('.aberto')) {
			$(this).attr('title', 'Clique para mais Detalhes');
			$(this).text('(+ Detalhes)');
			maisInfo.removeClass('aberto');
		}
		else {
			$(this).attr('title', 'Clique para menos Detalhes');
			$(this).text('(- Detalhes)');
			maisInfo.addClass('aberto');
		}

		maisInfo.slideToggle('fast');
	},

	onAssociarFinalidade: function (objeto) {

		var msg = new Array();

		$('.listaObjetos li', AtividadeSolicitadaAssociar.containerAtividade).each(function (i, linha) {
			if ($('.hdnTituloModeloId', linha).val() == objeto.TituloModelo) {
				msg.push(AtividadeSolicitadaAssociar.Mensagens.FinalidadeModeloTituloExistente);
			}
		});

		if (msg.length > 0) {
			return msg;
		}

		$('.Nenhum', AtividadeSolicitadaAssociar.containerAtividade).remove();

		var linha = $('.templateFinalidade', AtividadeSolicitadaAssociar.containerAtividade).clone();

		$('.hdnfinalidadeId', linha).val(objeto.Finalidade);
		$('.finalidadeTexto', linha).text(objeto.FinalidadeTexto);
		$('.hdnTituloModeloId', linha).val(objeto.TituloModelo);
		$('.tituloModeloTexto', linha).text(objeto.TituloModeloTexto);

		$('.hdnModeloTituloAnterior', linha).val(objeto.TituloModeloAnterior);
		$('.tituloAnteriorTexto', linha).text(objeto.TituloModeloAnteriorTexto);

		$('.hdnTituloAnteriorOrigem', linha).val(objeto.TituloAnteriorOrigem);

		$('.numeroDocumentoAnterior', linha).text(objeto.NumeroDocumentoAnterior);
		$('.hdnTituloAnteriorId', linha).val(objeto.TituloAnteriorId);
		$('.hdnModeloTituloAnteriorAtivo', linha).val(objeto.IsAtivo);
		$('.hdnModeloTituloAnteriorAtivo', linha).val(objeto.IsAtivo);
		$('.hdnEmitidoPorInterno', linha).val(objeto.EmitidoPorInterno);
		$('.hdnModeloTituloAnteriorSigla', linha).val(objeto.Sigla);

		if (objeto.OrgaoExpedidor) {
			$('.divOrgaoExpedidor', linha).removeClass('hide');
			$('.orgaoExpedidor', linha).text(objeto.OrgaoExpedidor);
		}

		if (!objeto.temTituloAnterior) {
			$('.divDetalhes', linha).remove();
		}

		linha.removeClass('hide templateFinalidade');

		$('.listaObjetos', AtividadeSolicitadaAssociar.containerAtividade).append(linha);

		if (AtividadeSolicitadaAssociar.onCallBackItemAssociado)
			AtividadeSolicitadaAssociar.onCallBackItemAssociado();

		return msg;
	},

	gerarObjeto: function (containerAtividade) {

		function atividadeSolicitada() {
			this.Id = 0;
			this.NomeAtividade = '';
			this.IdRelacionamento = 0;
			this.Finalidades = new Array();
			this.Protocolo = { Id: 0, IsProcesso: true };
		};

		var atividades = new Array();

		$('.divConteudoAtividadeSolicitada .asmItens .asmItemContainer', containerAtividade).each(function (i, itemAtividade) {
			if (!$('.nomeAtividade', itemAtividade).val())
				return;

			var atividade = new atividadeSolicitada();

			atividade.Id = $('.hdnAtividadeId', itemAtividade).val();
			atividade.NomeAtividade = $('.nomeAtividade', itemAtividade).val();
			atividade.IdRelacionamento = $('.hdnAtividadeRelId', itemAtividade).val();

			var protocoloId = $('.hdnProtocoloId', itemAtividade).val() == 0 ?
				$(itemAtividade).closest('.requerimentoDocumentoPartial').find('.hdnProtocoloId').val() : $('.hdnProtocoloId', itemAtividade).val();

			var protocoloTipo = $('.hdnProtocoloId', itemAtividade).val() == 0 ?
					$(itemAtividade).closest('.requerimentoDocumentoPartial').find('.hdnProtocoloTipo').val() == 1 : $('.hdnProtocoloTipo', itemAtividade).val();

			atividade.Protocolo.Id = protocoloId;
			atividade.Protocolo.IsProcesso = protocoloTipo;

			atividade.SetorId = $('.hdnAtividadeSetorId', itemAtividade).val();

			$('.listaObjetos li', itemAtividade).each(function (i, item) {

				if ($('.hdnfinalidadeId', item).val() == 0)
					return;

				var finalidade = {};

				finalidade.Id = $('.hdnfinalidadeId', item).val();
				finalidade.IdRelacionamento = $('.hdnIdRelacionamento', item).val();
				finalidade.TituloModelo = $('.hdnTituloModeloId', item).val();
				finalidade.TituloModeloAnteriorId = $('.hdnModeloTituloAnterior', item).val();
				finalidade.TituloModeloTexto = $('.tituloModeloTexto', item).text();
				finalidade.TituloAnteriorTipo = $('.hdnTituloAnteriorOrigem', item).val();
				finalidade.TituloModeloAnteriorTexto = $('.tituloAnteriorTexto', item).text();
				finalidade.TituloAnteriorNumero = $('.numeroDocumentoAnterior', item).text();
				finalidade.TituloAnteriorId = $('.hdnTituloAnteriorId', item).val();
				finalidade.OrgaoExpedidor = $('.orgaoExpedidor', item).text();
				finalidade.IsAtivo = $('.hdnModeloTituloAnteriorAtivo', item).val();
				finalidade.EmitidoPorInterno = $('.hdnEmitidoPorInterno', item).val();
				finalidade.EhRenovacao = $('.hdnEhRenovacao', item).val();
				finalidade.TituloModeloAnteriorSigla = $('.hdnModeloTituloAnteriorSigla', item).val();

				atividade.Finalidades.push(finalidade);
			});

			atividades.push(atividade);
		});

		return atividades;
	}
}

FinalidadeAssociar = {

	container: null,
	onAssociar: null,
	urlObterTituloModeloAnterior: null,
	urlObterTituloModelo: null,
	urlValidarNumeroModeloAnterior: null,
	urlObterNumerosTitulos: null,
	EhRenovacao: false,
	Mensagens: {},
	listaTitulosNumeros: [],
	clicou: false,

	load: function (container, onAssociar) {
		FinalidadeAssociar.container = MasterPage.getContent(container);
		Modal.defaultButtons(container, FinalidadeAssociar.onAssociarFinalidade, 'Adicionar', null, FinalidadeAssociar.onFecharModal, 'Cancelar', null);
		FinalidadeAssociar.onAssociar = onAssociar;
		container.delegate('.ddlTitulo', 'change', FinalidadeAssociar.onChangeTituloModelo);
		container.delegate('.ddlFinalidade', 'change', FinalidadeAssociar.onChangeFinalidade);

		container.delegate('.orgaoEmissor', 'change', FinalidadeAssociar.onChangeOrgaoEmissor);
		container.delegate('.btnBuscarNumero', 'click', FinalidadeAssociar.onObterNumerosAnteriores);
		container.delegate('.btnLimpar', 'click', FinalidadeAssociar.onLimparFiltro);

		$('.ddlFinalidade').focus();
	},

	onChangeFinalidade: function () {

		$('.ddlTitulo option', FinalidadeAssociar.container).remove();
		FinalidadeAssociar.limparCamposTituloAnterior();
		FinalidadeAssociar.ocultarModeloTituloAnterior();

		if ($('.ddlFinalidade', FinalidadeAssociar.container).val() == 0) {

			$('.ddlTitulo', FinalidadeAssociar.container).ddlLoad(new Array());
			$('.ddlTitulo', FinalidadeAssociar.container).removeAttr('disabled').removeClass('disabled');
			return;
		}

		$.ajax({
			url: FinalidadeAssociar.urlObterTituloModelo,
			type: "POST",
			data: JSON.stringify({ atividadeId: $('.hdnAtividade', FinalidadeAssociar.container).val(), finalidade: $('.ddlFinalidade', FinalidadeAssociar.container).val() }),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(MasterPage.getContent(FinalidadeAssociar.container), response.Msg);
					return;
				}
				$('.ddltituloAnterior option', FinalidadeAssociar.container).remove();
				FinalidadeAssociar.carregarDropDown(response.Lista, $('.ddlTitulo', FinalidadeAssociar.container));
			}
		});
	},

	onChangeTituloModelo: function () {

		FinalidadeAssociar.limparCamposTituloAnterior();
		FinalidadeAssociar.ocultarModeloTituloAnterior();
		if ($('.ddlTitulo', FinalidadeAssociar.container).val() == 0) {
			return;
		}

		$.ajax({
			url: FinalidadeAssociar.urlObterTituloModeloAnterior,
			type: "POST",
			data: JSON.stringify({ titulo: $('.ddlTitulo', FinalidadeAssociar.container).val() }),
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(MasterPage.getContent(FinalidadeAssociar.container), response.Msg);
					return;
				}

				FinalidadeAssociar.EhRenovacao = false;

				if ($('.ddlFinalidade', FinalidadeAssociar.container).val() == 3) {
					FinalidadeAssociar.EhRenovacao = true;
					$('.modeloTituloConteudo', FinalidadeAssociar.container).removeClass('hide');
					FinalidadeAssociar.carregarDropDown(response.ListaRenovacao, $('.ddltituloAnterior', FinalidadeAssociar.container));

				} else {
					if (response.EhFaseAnterior) {
						$('.modeloTituloConteudo', FinalidadeAssociar.container).removeClass('hide');
						FinalidadeAssociar.carregarDropDown(response.Lista, $('.ddltituloAnterior', FinalidadeAssociar.container));
					}

					if (response.FaseAnteriorEhObrigatoria) {
						$('.hdnFaseAnteriorObrigatoria', FinalidadeAssociar.container).val(response.FaseAnteriorEhObrigatoria);

						if (response.FaseAnteriorEhObrigatoria === 'true') {
							$('.lbTitulo', FinalidadeAssociar.container).html('Título *');
							$('.lbNumero', FinalidadeAssociar.container).html('Número *');
							$('.lbNumerosDocumentoAnterior', FinalidadeAssociar.container).html('Número *');
							$('.lbOrgaoExpedidor', FinalidadeAssociar.container).html('Órgão Expedidor *');
						} else {
							$('.lbTitulo', FinalidadeAssociar.container).html('Título');
							$('.lbNumero', FinalidadeAssociar.container).html('Número');
							$('.lbNumerosDocumentoAnterior', FinalidadeAssociar.container).html('Número');
							$('.lbOrgaoExpedidor', FinalidadeAssociar.container).html('Órgão Expedidor');
						}
					}
				}
			}
		});
	},

	onObterNumerosAnteriores: function () {
		$.ajax({
			url: FinalidadeAssociar.urlObterNumerosTitulos,
			type: "POST",
			dataType: "json",
			contentType: "application/json; charset=utf-8",
			data: JSON.stringify({ numero: $('.txtNumeroDocAnterior', FinalidadeAssociar.container).val(), modeloId: $('.ddltituloAnterior', FinalidadeAssociar.container).val() }),
			cache: false,
			async: false,
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {

				Mensagem.gerar(MasterPage.getContent(FinalidadeAssociar.container), response.Msg);
				if (!response.EhValido) {
					return;
				}

				FinalidadeAssociar.listaTitulosNumeros = $.extend([], response.Lista);

				if (response.Lista.length > 1) {
					$('.divDdlNumero', FinalidadeAssociar.container).removeClass('hide');
					$('.divTxtNumero', FinalidadeAssociar.container).addClass('hide');
					FinalidadeAssociar.carregarDropDown(response.Lista, $('.ddlNumeroAnterior', FinalidadeAssociar.container));
				}

				FinalidadeAssociar.clicou = true;
				$('.txtNumeroDocAnterior, .ddltituloAnterior', FinalidadeAssociar.container).addClass('disabled').attr('disabled', 'disabled');
				$('.btnLimpar', FinalidadeAssociar.container).removeClass('hide');
				$('.btnBuscarNumero', FinalidadeAssociar.container).addClass('hide');
			}
		});
	},

	onLimparFiltro: function () {
		FinalidadeAssociar.clicou = false;
		$('.btnLimpar', FinalidadeAssociar.container).addClass('hide');
		$('.btnBuscarNumero', FinalidadeAssociar.container).removeClass('hide');
		$('.divDdlNumero', FinalidadeAssociar.container).addClass('hide');
		$('.divTxtNumero', FinalidadeAssociar.container).removeClass('hide');
		FinalidadeAssociar.limparCamposModeloTitulo();
	},

	obterNumeroSelecionado: function (value) {
		var itemSelecionado;

		$(FinalidadeAssociar.listaTitulosNumeros).each(function (i, item) {
			if (item.Id == value) {
				itemSelecionado = item;
			}
		});

		return itemSelecionado;
	},

	onChangeOrgaoEmissor: function () {
		$('.divOrgao, .divOrgaoExterno', FinalidadeAssociar.container).addClass('hide');

		Mensagem.limpar(FinalidadeAssociar.container);
		FinalidadeAssociar.limparCamposModeloTitulo();
		FinalidadeAssociar.onLimparFiltro();

		if ($(this).val() == 1) {
			$('.divOrgao', FinalidadeAssociar.container).removeClass('hide');
		} else {
			$('.divOrgaoExterno', FinalidadeAssociar.container).removeClass('hide');
		}
	},

	limparCamposModeloTitulo: function () {
		$('input[type=text]', $('.modeloTituloConteudo', FinalidadeAssociar.container)).val('');
		$('*', $('.modeloTituloConteudo', FinalidadeAssociar.container)).removeAttr('disabled').removeClass('disabled');

		$('select', $('.modeloTituloConteudo', FinalidadeAssociar.container)).each(function () {
			$(this).find('option:first').attr('selected', 'selected');
		});
	},

	onAssociarFinalidade: function () {

		var msg = new Array();

		if ($('.ddlFinalidade', FinalidadeAssociar.container).val() == 0) {
			msg.push(FinalidadeAssociar.Mensagens.FinalidadeObrigatorio);
		}

		if ($('.ddlTitulo', FinalidadeAssociar.container).val() == 0) {
			msg.push(FinalidadeAssociar.Mensagens.TituloModeloObrigatorio);
		}

		var faseAnteriorEhObrigatoria = $('.hdnFaseAnteriorObrigatoria', FinalidadeAssociar.container).val() === 'true';
		var adicionouTituloAnterior = false;

		if (!$('.modeloTituloConteudo', FinalidadeAssociar.container).hasClass('hide')) {

			if ($('.emitidoPorInterno', FinalidadeAssociar.container).is(':checked')) {

				var tituloAnterior = !($('.ddltituloAnterior', FinalidadeAssociar.container).val() == 0);
				var NumeroDocAnterior = !($('.txtNumeroDocAnterior', FinalidadeAssociar.container).val() == '');
				var NumeroAnterior = FinalidadeAssociar.listaTitulosNumeros.length > 1 && $('.ddlNumeroAnterior', FinalidadeAssociar.container).val() != 0;


				if (tituloAnterior || NumeroDocAnterior || NumeroAnterior || faseAnteriorEhObrigatoria) {

					if (!FinalidadeAssociar.clicou) {
						msg.push(FinalidadeAssociar.Mensagens.BuscarObrigatorio);
					}

					if (!tituloAnterior) {
						msg.push(FinalidadeAssociar.Mensagens.TituloAnteriorObrigatorio);
					}

					if (!NumeroDocAnterior) {
						msg.push(FinalidadeAssociar.Mensagens.NumeroAnteriorObrigatorio);
					}

					if (FinalidadeAssociar.listaTitulosNumeros.length > 1 &&
					$('.ddlNumeroAnterior', FinalidadeAssociar.container).val() == 0) {
						msg.push(FinalidadeAssociar.Mensagens.NumeroAnteriorObrigatorio);
					}

					adicionouTituloAnterior = msg.length == 0;
				}

			} else {

				var TituloOrgaoExterno = !($('.txtTituloOrgaoExterno', FinalidadeAssociar.container).val() == '');
				var NumeroOrgaoExterno = !($('.txtNumeroOrgaoExterno', FinalidadeAssociar.container).val() == '');
				var OrgaoExpedidor = !($('.txtOrgaoExpedidor', FinalidadeAssociar.container).val() == '');

				if (TituloOrgaoExterno || NumeroOrgaoExterno || OrgaoExpedidor || faseAnteriorEhObrigatoria) {

					if (!TituloOrgaoExterno) {
						msg.push(FinalidadeAssociar.Mensagens.TituloAnteriorObrigatorio);
					}

					if (!NumeroOrgaoExterno) {
						msg.push(FinalidadeAssociar.Mensagens.NumeroAnteriorObrigatorio);
					}

					if (!OrgaoExpedidor) {
						msg.push(FinalidadeAssociar.Mensagens.OrgaoExpedidorObrigatorio);
					}

					adicionouTituloAnterior = msg.length == 0;

				}

			}
		}

		if (msg.length > 0) {
			Mensagem.gerar(MasterPage.getContent(FinalidadeAssociar.container), msg);
			return;
		}

		var objeto = {};

		var validarProcessoSelecionado = false;
		objeto.Finalidade = $('.ddlFinalidade', FinalidadeAssociar.container).val();
		objeto.FinalidadeTexto = $('.ddlFinalidade :selected', FinalidadeAssociar.container).text();
		objeto.TituloModelo = $('.ddlTitulo', FinalidadeAssociar.container).val();
		objeto.TituloModeloTexto = $('.ddlTitulo :selected', FinalidadeAssociar.container).text();
		objeto.EhRenovacao = FinalidadeAssociar.EhRenovacao;
		objeto.temTituloAnterior = false;

		if (!$('.modeloTituloConteudo', FinalidadeAssociar.container).hasClass('hide') && adicionouTituloAnterior) {

			objeto.temTituloAnterior = true;

			if ($('.emitidoPorInterno', FinalidadeAssociar.container).is(':checked')) {

				objeto.EmitidoPorInterno = true;
				objeto.TituloModeloAnterior = $('.ddltituloAnterior', FinalidadeAssociar.container).val();
				objeto.TituloModeloAnteriorTexto = $('.ddltituloAnterior :selected', FinalidadeAssociar.container).text();
				objeto.NumeroDocumentoAnterior = $('.txtNumeroDocAnterior', FinalidadeAssociar.container).val();

				switch (FinalidadeAssociar.listaTitulosNumeros.length) {

					case 0:
						objeto.TituloAnteriorOrigem = 4;
						break;

					case 1:
						objeto.TituloAnteriorOrigem = FinalidadeAssociar.listaTitulosNumeros[0].IsAtivo ? 1 : 2;
						objeto.TituloAnteriorId = FinalidadeAssociar.listaTitulosNumeros[0].IdRelacionamento;
						break;

					default:
						var item = FinalidadeAssociar.obterNumeroSelecionado($('.ddlNumeroAnterior', FinalidadeAssociar.container).val());
						objeto.TituloAnteriorOrigem = item.IsAtivo ? 1 : 2;
						objeto.TituloAnteriorId = item.IdRelacionamento;
						validarProcessoSelecionado = true;
						break;
				}

			} else {
				objeto.EmitidoPorInterno = false;
				objeto.TituloAnteriorOrigem = 3;
				objeto.TituloModeloAnteriorTexto = $('.txtTituloOrgaoExterno', FinalidadeAssociar.container).val();
				objeto.NumeroDocumentoAnterior = $('.txtNumeroOrgaoExterno', FinalidadeAssociar.container).val();
				objeto.OrgaoExpedidor = $('.txtOrgaoExpedidor', FinalidadeAssociar.container).val();
			}

			if (validarProcessoSelecionado) {
				var retorno = MasterPage.validarAjax(FinalidadeAssociar.urlValidarNumeroModeloAnterior, { TituloAnteriorId: objeto.TituloAnteriorId, TituloAnteriorTipo: objeto.TituloAnteriorOrigem }, FinalidadeAssociar.container, false);
				if (!retorno.EhValido) {
					return;
				}
			}
		}

		var msg = FinalidadeAssociar.onAssociar(objeto);

		if (msg.length > 0) {
			Mensagem.gerar(FinalidadeAssociar.container, msg);
		} else {
			Modal.fechar(FinalidadeAssociar.container);
		}
	},

	onFecharModal: function () {
		Modal.fechar(FinalidadeAssociar.container);
	},

	ocultarModeloTituloAnterior: function () {
		$('.modeloTituloConteudo', FinalidadeAssociar.container).addClass('hide');
	},

	limparCamposTituloAnterior: function () {
		$('.emitidoPorInterno', FinalidadeAssociar.container).attr('checked', 'checked');
		$('.emitidoPorInterno', FinalidadeAssociar.container).change();

		FinalidadeAssociar.limparCamposModeloTitulo();
		FinalidadeAssociar.onLimparFiltro();
	},

	carregarDropDown: function (valores, dropDown, textoPadrao) {

		dropDown.addClass('disabled').attr('disabled', 'disabled');

		textoPadrao = (textoPadrao ? textoPadrao + '' : '*** Selecione ***');
		valores.splice(0, 0, { Id: 0, Texto: textoPadrao });

		$('option', dropDown).remove();

		$.each(valores, function (i, item) {
			dropDown.append('<option value="' + item.Id + '">' + item.Texto + '</option>');
		});

		dropDown.removeClass('disabled').removeAttr('disabled').find('option:first').attr('selected', 'selected');
	}
}
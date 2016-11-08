/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />

PecaTecnica = {

	settings: {
		urls: {
			obterPartialRequerimentos: '',
			obterPartialAtividades: '',
			obterPartialConteudo: '',
			obterElaboradores: '',
			pdfRequerimento: '',
			pdfPecaTecnica: '',
			obterSetores: '',
			salvarPecaTecnica: '',
			visualizarRequerimento: ''
		},
		msgs: {}
	},
	ElaboradorTipo: {
		// Os Valores são atualizados pela VM
		TecnicoIdaf: 1,
		TecnicoTerceirizado: 2
	},
	mensagens: null,
	container: null,

	load: function (content, options) {
		if (options) { $.extend(PecaTecnica.settings, options); }
		PecaTecnica.container = MasterPage.getContent(content);
		content.delegate('.btnVerificarRegistro', 'click', PecaTecnica.onObterRegistro);
		content.delegate('.btnLimparRegistro', 'click', PecaTecnica.onLimparRegistro);
		content.delegate('.radioRequerimento', 'change', PecaTecnica.onChangeRequerimento);
		content.delegate('.radioAtividade', 'change', PecaTecnica.onChangeAtividade);
		content.delegate('.radioResponsavelElaborador', 'change', PecaTecnica.onChangeResponsavelElaboracao);
		content.delegate('.ddlSetores', 'change', PecaTecnica.onChangeSetor);		
		content.delegate('.btnSalvarPecaTecnica', 'click', PecaTecnica.onSalvar);
		content.delegate('.btnRequerimentoPDF', 'click', PecaTecnica.onBaixarPdfRequerimento);
		content.delegate('.btnGerarPdf', 'click', PecaTecnica.onGerarPdf);
		content.delegate('.btnAdicionarResponsavel', 'click', PecaTecnica.adicionarResponsavel);
		content.delegate('.btnExcluirResponsavel', 'click', PecaTecnica.excluirResponsavel);
		content.delegate('.campo', 'change', PecaTecnica.onChangePecaTecnica);
		
		PecaTecnica.container.delegate('.txtNumeroProcDoc', 'keyup', function (e) {
			if (e.keyCode == 13) {
				$('.btnVerificarRegistro', PecaTecnica.container).click(); 
			}
		});
		$('.txtNumeroProcDoc', PecaTecnica.container).focus();
	},

	adicionarResponsavel: function () {
		Mensagem.limpar(PecaTecnica.container);
		var mensagens = new Array();

		var container = $(this).closest('fieldset');

		var responsavel = {
			Id: $('.ddlRespEmpreendimento :selected', container).val(),
			NomeRazao: $('.ddlRespEmpreendimento :selected', container).text()
		}

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var resp = (JSON.parse(obj));

				if (resp.Id == responsavel.Id) {
					mensagens.push(jQuery.extend(true, {}, PecaTecnica.mensagens.RespEmpreendimentoJaAdicionado));
					Mensagem.gerar(PecaTecnica.container, mensagens);
					return;
				}
			}
		});

		if (responsavel.Id == 0) {
			mensagens.push(jQuery.extend(true, {}, PecaTecnica.mensagens.RespEmpreendimentoObrigatorio));
		}

		if (mensagens.length > 0) {
			Mensagem.gerar(PecaTecnica.container, mensagens);
			return;
		}


		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(responsavel));
		linha.find('.ResponsavelNome').html(responsavel.NomeRazao).attr('title', responsavel.NomeRazao);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));


		$('.ddlRespEmpreendimento', container).ddlFirst();
	},

	excluirResponsavel: function () {
		var linha = $(this).closest('tr');
		linha.remove();

		Listar.atualizarEstiloTable(PecaTecnica.container.find('.dataGridTable'));
	},

	//Processo
	onLimparRegistro: function () {
		$('.txtNumeroProcDoc', PecaTecnica.container).val('');
		$('.txtNumeroProcDoc', PecaTecnica.container).removeAttr('disabled').removeClass('disabled');
		$('.divRequerimento', PecaTecnica.container).empty();
		$('.btnVerificarRegistro', PecaTecnica.container).toggleClass('hide', false);
		$('.btnLimparRegistro', PecaTecnica.container).toggleClass('hide', true);
		$('.divSalvar', PecaTecnica.container).toggleClass('hide', true);
		$('.divCancelar', PecaTecnica.container).toggleClass('hide', false);
		$('.txtNumeroProcDoc', PecaTecnica.container).focus();
	},

	onObterRegistro: function () {

		var Regex = RegExp("^[0-9]+/[0-9]{4}$");

		if ($('.txtNumeroProcDoc', PecaTecnica.container).val().match(Regex) <= 0) {
			var erros = new Array();
			erros.push(PecaTecnica.mensagens.ProtocoloInvalido);
			Mensagem.gerar(PecaTecnica.container, erros);
			return;
		}

		var data = { numero: $('.txtNumeroProcDoc', PecaTecnica.container).val() };
		var options = { callback: PecaTecnica.obterRegistroCallBack };

		PecaTecnica.requisicao(PecaTecnica.settings.urls.obterPartialRequerimentos, data, options);

	},

	obterRegistroCallBack: function (response) {
		if (response.EhValido) {
			$('.divRequerimento', PecaTecnica.container).append(response.Html);

			$('.txtNumeroProcDoc', PecaTecnica.container).attr('disabled', true).addClass('disabled');
			$('.btnVerificarRegistro', PecaTecnica.container).toggleClass('hide', true);
			$('.btnLimparRegistro', PecaTecnica.container).toggleClass('hide', false);
			$('.divSalvar', PecaTecnica.container).toggleClass('hide', true);

			PecaTecnica.onCancelar = PecaTecnica.onCancelarRequerimentos;
		}
	},

	//Requerimento
	onChangeRequerimento: function () {
		var protocoloNumero = $(".hdnProtocoloId", $(this).closest('tr')).val();
		var data = { id: protocoloNumero };
		var controle = this;
		$('.divAtividades', PecaTecnica.container).empty();
		PecaTecnica.requisicao(PecaTecnica.settings.urls.obterPartialAtividades, data, { callback: function (response) {
			response.protocoloId = protocoloNumero;
			return PecaTecnica.changeRequerimentoCallBack(response, controle);
		}
		});
	},

	changeRequerimentoCallBack: function (response, controle) {

		if (response.EhValido) {
			var divAtividades = $('.divAtividades', PecaTecnica.container);
			divAtividades.append(response.html);
			$('.protocoloSelecionadoId', divAtividades).val(response.protocoloId);
			$('.divCancelar', PecaTecnica.container).toggleClass('hide', false);
			$('.divSalvar', PecaTecnica.container).toggleClass('hide', true);

			PecaTecnica.onCancelar = PecaTecnica.onCancelarAtividades;
		} else {
			$(controle).removeAttr("checked");
		}
	},

	onBaixarPdfRequerimento: function () {
		var id = $(this).closest('tr').find('.hdnItemId').val();
		MasterPage.redireciona(PecaTecnica.settings.urls.pdfRequerimento + "?id=" + id);
	},


	//Atividade

	onChangeAtividade: function () {
		$('.divConteudo', PecaTecnica.container).empty();
		var data = {
			id: $(this).val(),
			protocoloId: $('.divAtividades .protocoloSelecionadoId', PecaTecnica.container).val()
		};
		PecaTecnica.requisicao(PecaTecnica.settings.urls.obterPartialConteudo, data, { callback: PecaTecnica.changeAtividadeCallBack });
	},

	changeAtividadeCallBack: function (response) {

		if (response.EhValido) {
			$('.divConteudo', PecaTecnica.container).append(response.html);
			$('.divCancelar', PecaTecnica.container).toggleClass('hide', true);
			$('.divSalvar', PecaTecnica.container).toggleClass('hide', false);

			PecaTecnica.resolverBotaoPdf();
			PecaTecnica.onCancelar = PecaTecnica.onCancelarConteudo;

			MasterPage.botoes(PecaTecnica.container);
			Listar.atualizarEstiloTable(PecaTecnica.container.find('.dataGridTable'));
		}
	},

	//Cancelar

	onCancelarRequerimentos: function () {
		PecaTecnica.onLimparRegistro()
	},

	onCancelarAtividades: function () {
		$(".divAtividades", PecaTecnica.container).empty();
		$('.divCancelar', PecaTecnica.container).toggleClass('hide', false);
		$('.divSalvar', PecaTecnica.container).toggleClass('hide', true);
		$('.radioRequerimento', PecaTecnica.container).removeAttr("checked");
		PecaTecnica.onCancelar = PecaTecnica.onCancelarRequerimentos;
	},

	onCancelarConteudo: function () {
		$(".divConteudo", PecaTecnica.container).empty();
		$('.divCancelar', PecaTecnica.container).toggleClass('hide', false);
		$('.divSalvar', PecaTecnica.container).toggleClass('hide', true);
		$('.radioAtividade', PecaTecnica.container).removeAttr("checked");
		$('.btnGerarPdf', PecaTecnica.container).toggleClass('hide', true);
		PecaTecnica.onCancelar = PecaTecnica.onCancelarAtividades;
	},

	onCancelar: function () {
		//Usado como delegate
	},


	//pdf Peça tecnica
	onGerarPdf: function () {

		var obj = PecaTecnica.obterObjeto();
		var empreendimentoId = $(".hdnEmpreendimentoId", PecaTecnica.container).val();
		MasterPage.redireciona(PecaTecnica.settings.urls.pdfPecaTecnica + "?id=" + obj.Id + "&empreendimentoId=" + empreendimentoId);
	},


	resolverBotaoPdf: function (id) {
		var id = $('.divConteudo .hdnPecaTecnicaId', PecaTecnica.container).val();
		$('.btnGerarPdf', PecaTecnica.container).toggleClass('hide', id <= 0)
												.attr('disabled', false)
												.toggleClass('disabled', false);

	},

	onChangePecaTecnica: function () {
		$('.btnGerarPdf', PecaTecnica.container).attr('disabled', true).toggleClass('disabled', true);
	},

	//Conteudo Peça Tecnica

	onChangeSetor: function(){
		var setor = Number($('.ddlSetores :selected').val()) || 0;
		var tecIdaf = $('.tecIDAF', PecaTecnica.container).attr('checked');

		var data = {
			registro: $('.divAtividades .protocoloSelecionadoId', PecaTecnica.container).val(),
			tecIdaf: tecIdaf,
			setor: setor
		};

		PecaTecnica.requisicao(PecaTecnica.settings.urls.obterElaboradores,
								data,
								{
									callback: function (response) {
										return PecaTecnica.onChangeSetorCallBack(response);
									}
								}
		);
	},

	onChangeSetorCallBack: function(response){
		
		if (response.lista) {
			$('.ddlElaborador', PecaTecnica.container).ddlLoad(response.lista);
		}
	},

	onChangeResponsavelElaboracao: function () {

		var tecIdaf = $('.tecIDAF', PecaTecnica.container).attr('checked');

		var data = {
			registro: $('.divAtividades .protocoloSelecionadoId', PecaTecnica.container).val(),
			tecIdaf: tecIdaf,
			setor: 0
		};

		PecaTecnica.requisicao(PecaTecnica.settings.urls.obterElaboradores, data,
			{
				callback: function (response) {
					return PecaTecnica.onChangeResponsavelElaboracaoCallBack(response, tecIdaf);
				}
			}
		);

		$('.ddlSetores', PecaTecnica.container).trigger('change');

	},

	onChangeResponsavelElaboracaoCallBack: function (response, tecIdaf) {
		if (response.EhValido) {
			if (tecIdaf) {
				$('.ddlSetores').ddlLoad(response.Setores, { disableQtd: 1 });
				$('.divSetores', PecaTecnica.container).removeClass('hide');
			} else {
				$('.divSetores', PecaTecnica.container).addClass('hide');
				$('.ddlSetores', PecaTecnica.container).ddlFirst();
				$('.ddlElaborador', PecaTecnica.container).ddlLoad(response.lista);
			}

		} else {
			var controle = $(".radioResponsavel :radio", PecaTecnica.container);
			$(controle).removeAttr('checked');
		}
	},

	limparConteudo: function () {
		$(".divConteudo", PecaTecnica.container).empty();
		$('.divCancelar', PecaTecnica.container).toggleClass('hide', false);
		$('.divSalvar', PecaTecnica.container).toggleClass('hide', true);
		$('.radioRequerimento', PecaTecnica.container).removeAttr("checked");
	},

	obterObjetoProtocolo: function () {
		var linha = $('.tabRequerimento :checked', PecaTecnica.container).closest('tr');

		var item = {};
		item.Id = $('.hdnProtocoloId', linha).val();
		item.Empreendimento = {};
		item.Empreendimento.Id = $(".hdnEmpreendimentoId", PecaTecnica.container).val();
		return item;
	},


	obterObjeto: function () {
		var obj = {
			Id: $('.hdnPecaTecnicaId', PecaTecnica.container).val(),
			Atividade: $('.tabAtividadeSolicitada :checked', PecaTecnica.container).val(),
			ResponsaveisEmpreendimento: [],
			ElaboradorTipo: $('.radioResponsavel :checked', PecaTecnica.container).val(),
			Elaborador: $('.ddlElaborador', PecaTecnica.container).val(),
			ProtocoloPai: $('.hdnProtocoloPai', PecaTecnica.container).val(),
			Protocolo: PecaTecnica.obterObjetoProtocolo()
		};

		obj.SetorCadastro = (obj.ElaboradorTipo != PecaTecnica.ElaboradorTipo.TecnicoIdaf) ? null : $('.ddlSetores', PecaTecnica.container).val();


		$('.hdnItemJSon', $('.fsResponsaveis', PecaTecnica.container)).each(function (i, item) {
			var responsavel = String($(item).val());
			if (responsavel != '') {
				obj.ResponsaveisEmpreendimento.push(JSON.parse(responsavel));
			}
		});

		return obj;
	},

	validarPecaTecnica: function (obj) {

		var erros = [];
		if ((obj.Atividade || 0) <= 0) {
			erros.push(PecaTecnica.mensagens.AtividadeObrigatorio);
		}

		if (obj.Destinatario <= 0) {
			erros.push(PecaTecnica.mensagens.RespEmpreendimentoObrigatorio);
		}

		if (obj.Elaborador <= 0 || obj.ElaboradorTipo <= 0) {
			erros.push(PecaTecnica.mensagens.ElaboradorObrigatorio);
		}

		if (obj.ElaboradorTipo == PecaTecnica.ElaboradorTipo.TecnicoIdaf && obj.SetorCadastro <= 0) {
			erros.push(PecaTecnica.mensagens.SetorObrigatorio);
		}

		Mensagem.limpar(PecaTecnica.container);
		if (erros.length > 0) {
			Mensagem.gerar(PecaTecnica.container, erros);
		}

		return (erros.length <= 0);
	},

	onSalvar: function () {

		var pecaTecnica = PecaTecnica.obterObjeto();

		if (!PecaTecnica.validarPecaTecnica(pecaTecnica)) { return; }

		var data = { pecaTecnica: pecaTecnica };

		var options = { callback: PecaTecnica.salvarCallback };

		PecaTecnica.requisicao(
								PecaTecnica.settings.urls.salvarPecaTecnica,
								data,
								options
								);

	},

	salvarCallback: function (retorno) {

		if (retorno.EhValido) {
			$('.hdnPecaTecnicaId', PecaTecnica.container).val(retorno.Id);
			PecaTecnica.resolverBotaoPdf();
		}

	},

	//Auxiliar
	requisicao: function (url, data, opcoes) {

		var config = {
			type: 'POST',
			async: true,
			callback: function () { },
			serializar: true,
			carregando: true
		};

		$.extend(config, opcoes);

		if (config.carregando) {
			MasterPage.carregando(true);
		}

		$.ajax({ url: url,
			data: (config.serializar ? JSON.stringify(data) : data),
			cache: false,
			async: config.async,
			type: config.type,
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(PecaTecnica.container));

				if (config.carregando) {
					MasterPage.carregando(false);
				}
			},
			success: function (response) {

				if (response.Msg) {
					Mensagem.gerar(PecaTecnica.container, response.Msg);
				}

				config.callback(response);

				if (config.carregando) {
					MasterPage.carregando(false);
				}
			}
		});
	}


}


/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />

Dominialidade = {
	settings: {
		urls: {
			salvar: '',
			mergiar: '',
			atualizarGrupoARL: ''
		},
		atualizarDependenciasModalTitulo: '',
		textoMerge: null,
		dependencias: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Dominialidade.settings, options); }
		Dominialidade.container = MasterPage.getContent(container);

		Dominialidade.container.delegate('.btnSalvar', 'click', Dominialidade.salvar);
		Dominialidade.container.delegate('.titFiltros', 'click', Aux.expadirFieldSet);

		Dominialidade.configurarObjeto();
		Aux.setarFoco(Dominialidade.container);
		Dominios.load(container, { salvarCallBack: Dominialidade.setarAreas });

		if (Dominialidade.settings.textoMerge) {
			Dominialidade.abrirModalRedireciona(Dominialidade.settings.textoMerge);
		}
	},

	configurarObjeto: function () {
		$('.tabDominios tbody tr', Dominialidade.container).each(function () {
			dominio = JSON.parse($(this).find('.hdnItemJSon').val());

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

			$(this).find('.hdnItemJSon').val(JSON.stringify(dominio));
		});
	},

	setarAreas: function () {
		var totalAreaDocumento = 0;
		var totalAreaCCIR = 0;
		var totalARLDocumento = 0;
		var compensacao = false;
		
		$('.tabDominios tbody tr', Dominialidade.container).each(function () {
			dominio = JSON.parse($(this).find('.hdnItemJSon').val());
			totalAreaDocumento += dominio.AreaDocumento || 0;
			totalAreaCCIR += dominio.AreaCCIR || 0;
			totalARLDocumento += dominio.ARLDocumento || 0;

			$(dominio.ReservasLegais).each(function () {
				var reserva = this;

				if (reserva.Compensacao) {
					compensacao = true;
				}
			});

		});

		$('.divARL', Dominialidade.container).toggleClass('hide', !compensacao)

		$('.txtAreaDocumento', Dominialidade.container).val(Mascara.getStringMask(totalAreaDocumento));
		$('.txtAreaCCRI', Dominialidade.container).val(Mascara.getStringMask(totalAreaCCIR, 'n5'));
		$('.txtARLDocumento', Dominialidade.container).val(Mascara.getStringMask(totalARLDocumento));

		MasterPage.carregando(true);
		$.ajax({
			url: Dominialidade.settings.urls.atualizarGrupoARL,
			data: JSON.stringify(Dominialidade.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					var container = $('.divARL', Dominialidade.container);
					container.empty();
					container.append(response.Html);
					if(response.Empty)
					{                        
					    Modal.confirma(
                            {
                                conteudo: response.Html,
                                btnOkLabel: "Confirmar",
                                tamanhoModal: Modal.tamanhoModalMedia
                            });
                        
					}
				}

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Dominialidade.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	abrirModalRedireciona: function (textoModal) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', Dominialidade.container).attr('href'));
			},
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: Dominialidade.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	abrirModalMerge: function (textoModal) {
		Modal.confirma({
			removerFechar: true,
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				MasterPage.carregando(true);
				$.ajax({ url: Dominialidade.settings.urls.mergiar,
					data: JSON.stringify(Dominialidade.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', Dominialidade.container);
						container.empty();
						container.append(response.Html);
						Dominialidade.settings.dependencias = response.Dependencias;
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
				return false;
			},
			conteudo: textoModal,
			titulo: Dominialidade.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	obter: function () {
		var objeto = {
			Id: $('.hdnCaracterizacaoId', Dominialidade.container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', Dominialidade.container).val(),
			ATPCroqui: $('.hdnATP', Dominialidade.container).val(),
			//ATPCroqui: Mascara.getFloatMask($('.hdnATP', Dominialidade.container).val()),
			PossuiAreaExcedenteMatricula: $('.ddlPossuiAreaExcedenteMatricula', Dominialidade.container).val(),
			ConfrontacaoNorte: $('.txtConfrontacaoNorte', Dominialidade.container).val(),
			ConfrontacaoSul: $('.txtConfrontacaoSul', Dominialidade.container).val(),
			ConfrontacaoLeste: $('.txtConfrontacaoLeste', Dominialidade.container).val(),
			ConfrontacaoOeste: $('.txtConfrontacaoOeste', Dominialidade.container).val(),
			Dependencias: JSON.parse(Dominialidade.settings.dependencias),
			Areas: Dominialidade.obterAreas(),
			Dominios: Dominios.obter()
		};

		return objeto;
	},

	obterAreas: function () {
		var areas = [];
		$('.divArea', Dominialidade.container).each(function () {
			areas.push(JSON.parse($('.hdnAreaJson', this).val()));
		});

		return areas;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: Dominialidade.settings.urls.salvar,
			data: JSON.stringify(Dominialidade.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Dominialidade.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					Dominialidade.abrirModalMerge(response.TextoMerge);
					return;
				}

				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
					return;
				} /*else
				    {
				    Modal.confirma({
				        titulo: response.titulo,
                        conteudo: response.Msg
				    })
				    }*/

				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(Dominialidade.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
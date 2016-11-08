/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="coordenadaAtividade.js" />

PulverizacaoProduto = {
	settings: {
		urls: {
			salvar: '',
			editar: '',
			mergiar: '',
			visualizar: ''
		},
		salvarCallBack: null,
		mensagens: {},
		idsTela: null,
		textoAbrirModal: null,
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null,
		temARL: false,
		temARLDesconhecida: false
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(PulverizacaoProduto.settings, options); }
		PulverizacaoProduto.container = MasterPage.getContent(container);

		PulverizacaoProduto.container.delegate('.btnSalvar', 'click', PulverizacaoProduto.salvar);
		PulverizacaoProduto.container.delegate('.ddlCulturaTipo', 'change', PulverizacaoProduto.gerenciarCulturaTipo);
		PulverizacaoProduto.container.delegate('.btnAdicionarCultura', 'click', PulverizacaoProduto.adicionarCultura);
		PulverizacaoProduto.container.delegate('.btnExcluirCultura', 'click', PulverizacaoProduto.excluirCultura);
		PulverizacaoProduto.container.delegate('.ddlCoordenadaTipoGeometria', 'change', CoordenadaAtividade.obterDadosCoordenadaAtividade);

		var editar = $('.hdnIsEditar', container).val();
		if (!editar) {
			CoordenadaAtividade.obterDadosTipoGeometria();
			CoordenadaAtividade.obterDadosCoordenadaAtividade();
		}

		CoordenadaAtividade.load(container);

		if (PulverizacaoProduto.settings.textoMerge) {
			PulverizacaoProduto.abrirModalRedireciona(PulverizacaoProduto.settings.textoMerge, PulverizacaoProduto.settings.atualizarDependenciasModalTitulo);
		} else {
			PulverizacaoProduto.abrirModalARL();
		}

		if (PulverizacaoProduto.settings.textoAbrirModal) {
			PulverizacaoProduto.abrirModalRedireciona(PulverizacaoProduto.settings.textoAbrirModal, 'Área de Vegetação Nativa em Estágio Desconhecido de Regeneração');
		}
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
					MasterPage.redireciona($('.linkCancelar', PulverizacaoProduto.container).attr('href'));
				},
				conteudo: texto,
				titulo: 'Área de Reserva Legal',
				tamanhoModal: Modal.tamanhoModalMedia
			});
		};

		if (!PulverizacaoProduto.settings.temARL) {
			fnModal(PulverizacaoProduto.settings.mensagens.SemARLConfirm.Texto);
		} else if (PulverizacaoProduto.settings.temARLDesconhecida) {
			fnModal(PulverizacaoProduto.settings.mensagens.ARLDesconhecidaConfirm.Texto);
		}
	},

	atualizarAreaTotal: function () {
		var container = $('.divCulturas', PulverizacaoProduto.container);
		var total = 0;
		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var objeto = (JSON.parse(obj));
				total += Number(Mascara.getFloatMask(objeto.Area));
			}
		});

		$('.txtAreaTotalCultura', PulverizacaoProduto.container).val(Mascara.getStringMask(total, 'n4'));

	},

	gerenciarCulturaTipo: function(){
		var tipo = $('.ddlCulturaTipo :selected', PulverizacaoProduto.container).val();

		$('.divEspecificar', PulverizacaoProduto.container).addClass('hide');
		$('.txtCulturaEspecificarTexto', PulverizacaoProduto.container).val('');

		if (tipo == PulverizacaoProduto.settings.idsTela.Outros) {
			$('.divEspecificar', PulverizacaoProduto.container).removeClass('hide');
		}
	
	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', PulverizacaoProduto.container).attr('href'));
			},
			btnOkLabel: 'Confirmar',
			btnOkCallback: function (conteudoModal) {
				Modal.fechar(conteudoModal);
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
				MasterPage.carregando(true);
				$.ajax({
					url: PulverizacaoProduto.settings.urls.mergiar,
					data: JSON.stringify(PulverizacaoProduto.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', PulverizacaoProduto.container);
						container.empty();
						container.append(response.Html);
						PulverizacaoProduto.settings.dependencias = response.Dependencias;
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: PulverizacaoProduto.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});
	},

	adicionarCultura: function () {
		var mensagens = new Array();
		Mensagem.limpar(PulverizacaoProduto.container);
		var container = $('.divCulturas');

		var Cultura = {
			Id: 0,
			Tid: '',
			TipoId: $('.ddlCulturaTipo :selected', container).val(),
			TipoTexto: $('.ddlCulturaTipo :selected', container).text(),
			EspecificarTexto: $('.txtCulturaEspecificarTexto', container).val(),
			Area:  Mascara.getFloatMask($('.txtCulturaArea', container).val())
		}

		if (Cultura.TipoId == 0) {
			mensagens.push(jQuery.extend(true, {}, PulverizacaoProduto.settings.mensagens.CulturaTipoObrigatorio));
		} else {

			if (Cultura.TipoId == PulverizacaoProduto.settings.idsTela.Outros)
			{
				Cultura.TipoTexto = Cultura.EspecificarTexto;

				if (Cultura.TipoTexto == '') {
					mensagens.push(jQuery.extend(true, {}, PulverizacaoProduto.settings.mensagens.CulturaEspecificarTextoObrigatorio));
				}
			}
		}

		if (Cultura.Area == '') {
			mensagens.push(jQuery.extend(true, {}, PulverizacaoProduto.settings.mensagens.CulturaAreaObrigatoria));
		} else {
			if (isNaN(Cultura.Area)) {
				mensagens.push(jQuery.extend(true, {}, PulverizacaoProduto.settings.mensagens.CulturaAreaInvalida));
			}
			if (Number(Cultura.Area) <= 0) {
				mensagens.push(jQuery.extend(true, {}, PulverizacaoProduto.settings.mensagens.CulturaAreaMaiorZero));
			}
		}



		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var cult = (JSON.parse(obj));
				if (cult.TipoTexto == Cultura.TipoTexto) {
					mensagens.push(jQuery.extend(true, {}, PulverizacaoProduto.settings.mensagens.CulturaTipoDuplicado));
				}
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(PulverizacaoProduto.container, mensagens);
			return;
		}

		Cultura.Area = Mascara.getStringMask(Cultura.Area, 'n4');

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(Cultura));
		linha.find('.tipo').html(Cultura.TipoTexto).attr('title', Cultura.TipoTexto);
		linha.find('.area').html(Cultura.Area).attr('title', Cultura.Area);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.txtCulturaArea', container).val('');
		$('.txtCulturaEspecificarTexto', container).val('');
		$('.ddlCulturaTipo', container).ddlFirst();

		PulverizacaoProduto.atualizarAreaTotal();
		PulverizacaoProduto.gerenciarCulturaTipo();

	},

	excluirCultura: function () {
		var container = $('.divCulturas');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
		PulverizacaoProduto.atualizarAreaTotal();
	},

	obter: function () {
		var container = PulverizacaoProduto.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
			Atividade: $('.ddlAtividade :selected', container).val(),
			EmpresaPrestadora: $('.txtEmpresaPrestadora', container).val(),
			CNPJ: $('.txtCNPJ', container).val(),
			Dependencias: JSON.parse(PulverizacaoProduto.settings.dependencias),
			CoordenadaAtividade: CoordenadaAtividade.obter(),
			Culturas: []
		};

		//Culturas
		container = PulverizacaoProduto.container.find('.divCulturas');
		$('.hdnItemJSon', container).each(function () {
			var objCultura = String($(this).val());
			if (objCultura != '') {
				obj.Culturas.push(JSON.parse(objCultura));
			}
		});

		return obj;
	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({
			url: PulverizacaoProduto.settings.urls.salvar,
			data: JSON.stringify(PulverizacaoProduto.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, PulverizacaoProduto.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					PulverizacaoProduto.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(PulverizacaoProduto.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
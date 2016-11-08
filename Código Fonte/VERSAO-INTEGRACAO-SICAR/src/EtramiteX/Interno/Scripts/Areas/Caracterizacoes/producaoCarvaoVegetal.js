/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="coordenadaAtividade.js" />
/// <reference path="materiaPrimaFlorestalConsumida.js" />

ProducaoCarvaoVegetal = {
	settings: {
		urls: {
			salvar: '',
			editar: '',
			visualizar: ''
		},
		salvarCallBack: null,
		mensagens: {},
		textoAbrirModal: null,
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(ProducaoCarvaoVegetal.settings, options); }
		ProducaoCarvaoVegetal.container = MasterPage.getContent(container);

		ProducaoCarvaoVegetal.container.delegate('.btnSalvar', 'click', ProducaoCarvaoVegetal.salvar);
		ProducaoCarvaoVegetal.container.delegate('.btnAdicionarForno', 'click', ProducaoCarvaoVegetal.adicionarForno);
		ProducaoCarvaoVegetal.container.delegate('.btnExcluirForno', 'click', ProducaoCarvaoVegetal.excluirForno);
		ProducaoCarvaoVegetal.container.delegate('.txtNumFornos', 'blur', ProducaoCarvaoVegetal.gerenciarNumeroFornos);
		ProducaoCarvaoVegetal.container.delegate('.ddlCoordenadaTipoGeometria', 'change', CoordenadaAtividade.obterDadosCoordenadaAtividade);

		var editar = $('.hdnIsEditar', container).val();
		if (!editar) {
			CoordenadaAtividade.obterDadosTipoGeometria();
			CoordenadaAtividade.obterDadosCoordenadaAtividade();
		}

		CoordenadaAtividade.load(container);
		MateriaPrimaFlorestalConsumida.load(container);

		if (ProducaoCarvaoVegetal.settings.textoMerge) {
			ProducaoCarvaoVegetal.abrirModalRedireciona(ProducaoCarvaoVegetal.settings.textoMerge, ProducaoCarvaoVegetal.settings.atualizarDependenciasModalTitulo);
		}
	},

	gerenciarNumeroFornos: function () {
		var qtdFornos = $('.txtNumFornos', ProducaoCarvaoVegetal.container).val();
		qtdFornos = !isNaN(qtdFornos) ? Number(qtdFornos) : 0;
		var container = ProducaoCarvaoVegetal.container.find('.divFornos');
		var qtdFornosAdicionados = 0;

		$('.hdnItemJSon', container).each(function () {
			if ($(this).val() != '') {
				qtdFornosAdicionados++;
			}
		});

		if (qtdFornosAdicionados <= 0) {
			if (qtdFornos > 0) {
				$('.txtIdentificador', container).val(1);
				$('.txtVolume', container).focus();
			} else {
				$('.txtIdentificador', container).val('');
			}
		}
	},

	atualizarVolumeTotal: function () {
		var container = $('.divFornos');
		var total = 0;
		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var objeto = (JSON.parse(obj));
				total += Number(objeto.Volume.replace(',', '.'));
			}
		});

		$('.txtVolumeTotalFornos', container).val(total.toString().replace('.', ','));

	},

	abrirModalRedireciona: function (textoModal, titulo) {
		Modal.confirma({
			removerFechar: true,
			btnCancelCallback: function (conteudoModal) {
				MasterPage.redireciona($('.linkCancelar', ProducaoCarvaoVegetal.container).attr('href'));
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
				$.ajax({ url: ProducaoCarvaoVegetal.settings.urls.mergiar,
					data: JSON.stringify(ProducaoCarvaoVegetal.obter()),
					cache: false,
					async: false,
					type: 'POST',
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, conteudoModal);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						var container = $('.divCaracterizacao', ProducaoCarvaoVegetal.container);
						container.empty();
						container.append(response.Html);
						ProducaoCarvaoVegetal.settings.dependencias = response.Dependencias;
					}
				});
				MasterPage.carregando(false);
				Modal.fechar(conteudoModal);
			},
			conteudo: textoModal,
			titulo: ProducaoCarvaoVegetal.settings.atualizarDependenciasModalTitulo,
			tamanhoModal: Modal.tamanhoModalMedia
		});

		Listar.atualizarEstiloTable(ProducaoCarvaoVegetal.container.find('.dataGridTable'));
	},

	adicionarForno: function () {
		var mensagens = new Array();
		Mensagem.limpar(ProducaoCarvaoVegetal.container);
		var container = $('.divFornos');
		var qtdFornos = $('.txtNumFornos', ProducaoCarvaoVegetal.container).val();

		if (qtdFornos == '') {
			mensagens.push(jQuery.extend(true, {}, ProducaoCarvaoVegetal.settings.mensagens.NumeroFornosObrigatorio));
			Mensagem.gerar(ProducaoCarvaoVegetal.container, mensagens);
			return;

		}

		qtdFornos = !isNaN(qtdFornos) ? Number(qtdFornos) : 0;

		if (qtdFornos <= 0) {
			mensagens.push(jQuery.extend(true, {}, ProducaoCarvaoVegetal.settings.mensagens.NumeroFornosMaiorZero));
			Mensagem.gerar(ProducaoCarvaoVegetal.container, mensagens);
			return;
		}

		var identificador = $('.txtIdentificador', container).val();
		identificador = !isNaN(identificador) ? Number(identificador) : 0;

		if (identificador == 0) identificador++;

		if (identificador > qtdFornos) {
			mensagens.push(jQuery.extend(true, {}, ProducaoCarvaoVegetal.settings.mensagens.FornosJaAdicionados));
			Mensagem.gerar(ProducaoCarvaoVegetal.container, mensagens);
			return;
		}

		var forno = {
			Id: 0,
			Tid: '',
			Identificador: identificador,
			Volume: $('.txtVolume', container).val().replace('.', '').replace(',', '.')
		}

		if (forno.Volume == '') {
			mensagens.push(jQuery.extend(true, {}, ProducaoCarvaoVegetal.settings.mensagens.VolumeFornoObrigatorio));
			Mensagem.gerar(ProducaoCarvaoVegetal.container, mensagens);
			return;
		} else {
			if (isNaN(forno.Volume)) {
				mensagens.push(jQuery.extend(true, {}, ProducaoCarvaoVegetal.settings.mensagens.VolumeFornoInvalido));
				Mensagem.gerar(ProducaoCarvaoVegetal.container, mensagens);
				return;
			}
			if (Number(forno.Volume) <= 0) {
				mensagens.push(jQuery.extend(true, {}, ProducaoCarvaoVegetal.settings.mensagens.VolumeFornoMaiorZero));
				Mensagem.gerar(ProducaoCarvaoVegetal.container, mensagens);
				return;
			}
		}

		identificador++;

		forno.Volume = forno.Volume.replace(".", ",");

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(forno));
		linha.find('.identificador').html(forno.Identificador).attr('title', forno.Identificador);
		linha.find('.volumeForno').html(forno.Volume).attr('title', forno.Volume);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.txtVolume', container).val('');
		$('.txtIdentificador', container).val(identificador);

		//colocando foco no campo capacidade enquanto pode adicionar Fornos
		if (identificador != qtdFornos + 1) {
			$('.txtVolume', container).focus();
		}

		ProducaoCarvaoVegetal.atualizarVolumeTotal();

	},

	excluirForno: function () {
		var container = $('.divFornos');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		var identificador = $('.txtIdentificador', container).val();
		identificador = !isNaN(identificador) ? Number(identificador) : 0;
		identificador--;
		$('.txtIdentificador', container).val(identificador);

		var cont = 1;
		$('.hdnItemJSon', container).each(function (i, item) {
			var objForno = String($(item).val());
			if (objForno != '') {
				var obj = JSON.parse(objForno);
				obj.Identificador = cont;
				linha = $(item).closest('tr');
				linha.find('.hdnItemJSon').val(JSON.stringify(obj));
				linha.find('.identificador').html(obj.Identificador).attr('title', obj.Identificador);
				cont++;
			}
		});

		ProducaoCarvaoVegetal.atualizarVolumeTotal();
	},

	obter: function () {
		var container = ProducaoCarvaoVegetal.container;
		var obj = {
			Id: $('.hdnCaracterizacaoId', container).val(),
			EmpreendimentoId: $('.hdnEmpreendimentoId', container).val(),
			Atividade: $('.ddlAtividade :selected', container).val(),
			Dependencias: JSON.parse(ProducaoCarvaoVegetal.settings.dependencias),
			NumeroFornos: $('.txtNumFornos', container).val(),
			CoordenadaAtividade: CoordenadaAtividade.obter(),
			Fornos: ProducaoCarvaoVegetal.obterFornos(),
			MateriasPrimasFlorestais: MateriaPrimaFlorestalConsumida.obter()
		};

		return obj;

	},

	obterFornos: function () {
		var obj = [];
		var container = ProducaoCarvaoVegetal.container.find('.divFornos');
		$('.hdnItemJSon', container).each(function () {
			var objForno = String($(this).val());
			if (objForno != '') {
				obj.push(JSON.parse(objForno));
			}
		});

		return obj;

	},

	salvar: function () {
		MasterPage.carregando(true);
		$.ajax({ url: ProducaoCarvaoVegetal.settings.urls.salvar,
			data: JSON.stringify(ProducaoCarvaoVegetal.obter()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, ProducaoCarvaoVegetal.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TextoMerge) {
					ProducaoCarvaoVegetal.abrirModalMerge(response.TextoMerge);
					return;
				}
				if (response.EhValido) {
					MasterPage.redireciona(response.UrlRedirecionar);
				}
				if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(ProducaoCarvaoVegetal.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />

QueimaControladaQueima = {
	settings: {
		urls: {
			urlObterDadosTipoQueimaAgricola: '',
			urlObterDadosTipoQueimaFloresta: ''
		},
		idsTela: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(QueimaControladaQueima.settings, options); }
		QueimaControladaQueima.container = MasterPage.getContent(container);

		QueimaControladaQueima.container.delegate('.btnAdicionarQueima', 'click', QueimaControladaQueima.adicionar);
		QueimaControladaQueima.container.delegate('.btnExcluirQueima', 'click', QueimaControladaQueima.excluir);
		QueimaControladaQueima.container.delegate('.asmConteudoInternoExpander.asmExpansivel', 'click', QueimaControladaQueima.gerenciarExpandir);
		QueimaControladaQueima.container.delegate('.ddlTipoCultivo', 'change', QueimaControladaQueima.gerenciarQueimaTipo)
		QueimaControladaQueima.gerenciarQueimaTipo(container, true);
	},

	gerenciarQueimaTipo: function (container, isOnLoad) {
		Mensagem.limpar(QueimaControladaQueima.container);

		if (!isOnLoad) {
			container = $(this).closest('fieldset');
		}
		var tipoCultivo = $('.ddlTipoCultivo :selected', container).val();

		if (tipoCultivo == QueimaControladaQueima.settings.idsTela.OutraFinalidade) {
			$('.divFinalidadeNome', container).removeClass('hide');
		} else {
			$('.divFinalidadeNome', container).addClass('hide');
		}

	},

	adicionar: function () {
		var mensagens = new Array();
		Mensagem.limpar(QueimaControladaQueima.container);
		var container = $(this).closest('fieldset');
		var identificacao = $('.txtIdentificacao', container).val();

		var queima = {
			Id: 0,
			FinalidadeNome: QueimaControladaQueima.obterCultivo(container),
			CultivoTipo: $('.ddlTipoCultivo :selected', container).val(),
			CultivoTipoTexto: $('.ddlTipoCultivo :selected', container).text(),
			AreaQueima: Mascara.getFloatMask($('.txtAreaAreaQueima', container).val()),
			AreaQueimaTexto: $('.txtAreaAreaQueima', container).val()
		}

		var outraFinalidade = QueimaControladaQueima.settings.idsTela.OutraFinalidade;
		var areaQueima = $('.txtAreaAreaQueima', container).val().toString().trim().replace(/\./g, '').replace(',', '.');


		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var quei = (JSON.parse(obj));

				if (queima.CultivoTipo != QueimaControladaQueima.settings.idsTela.OutraFinalidade) {
					if (quei.CultivoTipoTexto == queima.CultivoTipoTexto) {
						mensagens.push(jQuery.extend(true, {}, QueimaControladaQueima.settings.mensagens.TipoCultivoQueimaDuplicado));
						Mensagem.gerar(QueimaControladaQueima.container, mensagens);
						return;
					}
				} else {
					if (quei.CultivoTipoTexto.toLowerCase() == queima.FinalidadeNome.toLowerCase()) {
						mensagens.push(jQuery.extend(true, {}, QueimaControladaQueima.settings.mensagens.FinalidadeNomeQueimaDuplicada));
						Mensagem.gerar(QueimaControladaQueima.container, mensagens);
						return;
					}
				}
			}
		});

		if (queima.CultivoTipo == outraFinalidade) {
			if (queima.FinalidadeNome == '') {
				mensagens.push(jQuery.extend(true, {}, QueimaControladaQueima.settings.mensagens.FinalidadeNomeObrigatorio));
			} else {
				queima.CultivoTipoTexto = queima.FinalidadeNome;
			}
		} else {
			if (queima.CultivoTipo == 0) {
				mensagens.push(jQuery.extend(true, {}, QueimaControladaQueima.settings.mensagens.TipoCultivoObrigatorio));
			}
		}

		if (areaQueima == '') {
			mensagens.push(jQuery.extend(true, {}, QueimaControladaQueima.settings.mensagens.AreaQueimaObrigatoria));
		} else {
			if (isNaN(areaQueima)) {
				mensagens.push(jQuery.extend(true, {}, QueimaControladaQueima.settings.mensagens.AreaQueimaInvalida));
			} else if (Number(areaQueima) <= 0) {
				mensagens.push(jQuery.extend(true, {}, QueimaControladaQueima.settings.mensagens.AreaQueimaMaiorZero));
			}
		}

		if (mensagens.length > 0) {
			var sufixo = container.find('.txtIdentificacao').val();
			$(mensagens).each(function () {
				this.Campo += sufixo;
			});
			Mensagem.gerar(QueimaControladaQueima.container, mensagens);
			return;
		}

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(queima));
		linha.find('.tipoCultivo').html(queima.CultivoTipoTexto).attr('title', queima.CultivoTipoTexto);
		linha.find('.areaQueima').html(queima.AreaQueimaTexto).attr('title', queima.AreaQueimaTexto);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.txtFinalidadeNome', container).val('');
		$('.txtAreaAreaQueima', container).val('');
		$('.ddlTipoCultivo', container).ddlFirst();
		$('.divFinalidadeNome', container).addClass('hide');

	},

	obterCultivo: function (container) {
		var identificacao = $('.txtIdentificacao', container).val();
		var tipoCultivo = $('.ddlTipoCultivo :selected', container).val();

		if (tipoCultivo == QueimaControladaQueima.settings.idsTela.OutraFinalidade) {
			return $('.txtFinalidadeNome', container).val();
		}
		return '';

	},

	obter: function () {
		var queimasControladas = [];
		$('.divQueimaControladaQueima', QueimaControladaQueima.container).each(function () {
			var identificacao = $('.txtIdentificacao', this).val();
			var objeto = {
				Id: $('.hdnQueimaControladaQueimaId', this).val(),
				Identificacao: $('.txtIdentificacao', this).val(),
				AreaCroqui: Number($('.hdnAreaCroqui', this).val()),
				AreaRequerida: Mascara.getFloatMask($('.txtAreaRequerida', this).val()),
				QueimaTipo: $('.ddlTipoQueima' + identificacao, this).val(),
				Cultivos: []
			}

			$('.hdnItemJSon', this).each(function (i, item) {
				var obj = String($(item).val());
				if (obj != '') {
					objeto.Cultivos.push(JSON.parse(obj));
				}
			});

			queimasControladas.push(objeto);
		});

		return queimasControladas;
	},

	excluir: function () {
		var container = $(this).closest('fieldset');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	},

	gerenciarExpandir: function () {
		var container = $(this).closest('fieldset');

		$('.asmConteudoInterno', container).toggle('fast', function () {
			var visivel = $('.asmConteudoInterno', container).is(':visible');

			if (visivel) {
				$('.asmExpansivel', container).text('Clique aqui para ocultar detalhes');
			} else {
				$('.asmExpansivel', container).text('Clique aqui para ver mais detalhes');
			}
			$('.linkVejaMaisCampos', container).toggleClass('ativo', visivel);
		});

		return false;
	}
}
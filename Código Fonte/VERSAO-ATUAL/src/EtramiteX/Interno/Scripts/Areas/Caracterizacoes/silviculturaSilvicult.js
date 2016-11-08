/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="silvicultura.js" />

SilviculturaSilvicult = {
	settings: {
		urls: {},
		idsTela: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(SilviculturaSilvicult.settings, options); }
		SilviculturaSilvicult.container = MasterPage.getContent(container);

		SilviculturaSilvicult.container.delegate('.btnAdicionarCultura', 'click', SilviculturaSilvicult.adicionar);
		SilviculturaSilvicult.container.delegate('.btnExcluirCultura', 'click', SilviculturaSilvicult.excluir);
		SilviculturaSilvicult.container.delegate('.ddlTipoCultura', 'change', SilviculturaSilvicult.gerenciarCulturaTipo)

		SilviculturaSilvicult.gerenciarCulturaTipo(null, container);
	},

	gerenciarCulturaTipo: function (e, container) {
		Mensagem.limpar(SilviculturaSilvicult.container);

		if (!container) {
			container = $(this).closest('fieldset');
		}
		var tipoCultura = $('.ddlTipoCultura :selected', container).val();

		if (tipoCultura == SilviculturaSilvicult.settings.idsTela.Outros) {
			$('.divEspecificar', container).removeClass('hide');
		} else {
			$('.divEspecificar', container).addClass('hide');
		}

	},

	adicionar: function () {
		Mensagem.limpar(SilviculturaSilvicult.container);
		var mensagens = new Array();

		var container = $(this).closest('fieldset');
		var identificacao = $('.txtIdentificacao', container).val();

		var cultura = {
			Id: 0,
			CulturaTipo: $('.ddlTipoCultura :selected', container).val(),
			CulturaTipoTexto: $('.ddlTipoCultura :selected', container).text(),
			Especificar: $('.txtEspecificar', container).val(),
			AreaCulturaHa: Mascara.getFloatMask($('.txtAreaCultura', container).val()),
			AreaCulturaTexto: $('.txtAreaCultura', container).val()
		}
		var outraCultura = SilviculturaSilvicult.settings.idsTela.Outros;

		if (cultura.CulturaTipo == outraCultura) {
			cultura.CulturaTipoTexto = cultura.Especificar;
		}


		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var cult = (JSON.parse(obj));

				if (cultura.CulturaTipo != outraCultura) {
					if (cult.CulturaTipoTexto == cultura.CulturaTipoTexto) {
						mensagens.push(jQuery.extend(true, {}, SilviculturaSilvicult.settings.mensagens.TipoCulturaJaAdicionado));
						Mensagem.gerar(SilviculturaSilvicult.container, mensagens);
						return;
					}
				} else {
					if (cult.CulturaTipoTexto.toLowerCase() == cultura.Especificar.toLowerCase()) {
						mensagens.push(jQuery.extend(true, {}, SilviculturaSilvicult.settings.mensagens.EspecificarTipoCulturaJaAdicionado));
						Mensagem.gerar(SilviculturaSilvicult.container, mensagens);
						return;
					}
				}
			}
		});

		if (cultura.CulturaTipo == outraCultura) {
			if (cultura.Especificar == '') {
				mensagens.push(jQuery.extend(true, {}, SilviculturaSilvicult.settings.mensagens.EspecificarTipoCulturaObrigatorio));
			}
		} else {
			if (cultura.CulturaTipo == 0) {
				mensagens.push(jQuery.extend(true, {}, SilviculturaSilvicult.settings.mensagens.TipoCulturaObrigatorio));
			}
		}

		if (cultura.AreaCulturaHa == '') {
			mensagens.push(jQuery.extend(true, {}, SilviculturaSilvicult.settings.mensagens.AreaCulturaObrigatoria));
		} else {
			if (isNaN(cultura.AreaCulturaHa)) {
				mensagens.push(jQuery.extend(true, {}, SilviculturaSilvicult.settings.mensagens.AreaCulturaInvalida));
			} else if (Number(cultura.AreaCultura) <= 0) {
				mensagens.push(jQuery.extend(true, {}, SilviculturaSilvicult.settings.mensagens.AreaCulturaMaiorZero));
			}
		}

		if (mensagens.length > 0) {
			var sufixo = container.find('.txtIdentificacao').val();
			$(mensagens).each(function () {
				this.Campo += sufixo;
			});
			Mensagem.gerar(SilviculturaSilvicult.container, mensagens);
			return;
		}


		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(cultura));
		linha.find('.CulturaTipo').html(cultura.CulturaTipoTexto).attr('title', cultura.CulturaTipoTexto);
		linha.find('.AreaCultura').html(cultura.AreaCulturaTexto).attr('title', cultura.AreaCulturaTexto);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.txtEspecificar', container).val('');
		$('.txtAreaCultura', container).val('');
		$('.ddlTipoCultura', container).ddlFirst();
		SilviculturaSilvicult.gerenciarCulturaTipo(null, container);

		//Atualizando Grid de Agrupamento de culturas
		Silvicultura.agruparCulturas();

	},

	obter: function () {
		var silviculturas = [];

		$('.fsSilvicultura', SilviculturaSilvicult.container).each(function () {
			var identificacao = $('.txtIdentificacao', this).val();
			var objeto = {
				Id: $('.hdnSilviculturaSilvicultId', this).val(),
				Identificacao: $('.txtIdentificacao', this).val(),
				GeometriaTipo: $('.ddlGeometriaTipo :selected', this).val(),
				GeometriaTipoTexto: $('.ddlGeometriaTipo :selected', this).text(),
				AreaCroquiHa: Number($('.hdnSilviculturaSilvicultAreaCroqui', this).val()),
				Culturas: []
			}

			$('.hdnItemJSon', this).each(function (i, item) {
				var obj = String($(item).val());
				if (obj != '') {
					objeto.Culturas.push(JSON.parse(obj));
				}
			});

			silviculturas.push(objeto);
		});

		return silviculturas;
	},

	excluir: function () {
		var container = $(this).closest('fieldset');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		//Atualizando Grid de Agrupamento de culturas
		Silvicultura.agruparCulturas();
	}
}
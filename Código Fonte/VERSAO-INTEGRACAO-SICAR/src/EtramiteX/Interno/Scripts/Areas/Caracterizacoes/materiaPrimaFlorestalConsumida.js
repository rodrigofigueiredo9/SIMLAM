/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

MateriaPrimaFlorestalConsumida = {
	settings: {
		urls: {
			salvar: ''
		},
		mensagens: {}
	},
	container: null,

	load: function (container, options) {

		if (options) { $.extend(MateriaPrimaFlorestalConsumida.settings, options); }

		MateriaPrimaFlorestalConsumida.container = MasterPage.getContent(container);
		MateriaPrimaFlorestalConsumida.container.delegate('.btnAdicionarMateria', 'click', MateriaPrimaFlorestalConsumida.adicionarMateria);
		MateriaPrimaFlorestalConsumida.container.delegate('.btnExcluirMateria', 'click', MateriaPrimaFlorestalConsumida.excluirMateria);
		MateriaPrimaFlorestalConsumida.container.delegate('.ddlMateriaPrima', 'change', MateriaPrimaFlorestalConsumida.gerenciarMateriaPrimaOutros);
	},

	gerenciarMateriaPrimaOutros: function (e, container) {
		if (!container) container = $(this).closest('fieldset');
		var materia = $('.ddlMateriaPrima :selected', container).text().trim().toLowerCase();
		if (materia == 'outros' || materia == 'outras' || materia == 'outro' || materia == 'outra') {
			$('.divEspecificar', container).removeClass('hide');
		} else {
			$('.divEspecificar', container).addClass('hide');
		}

	},

	obter: function (container, associarMultiplo) {
		if (!associarMultiplo) {
			container = MateriaPrimaFlorestalConsumida.container.find('.divMateriasPrima');
		} else {
			if (!container) {
				container = $(this).closest('fieldset');
			}
		}

		var obj = [];
		$('.hdnItemJSon', container).each(function () {
			var objMateria = String($(this).val());
			if (objMateria != '') {
				obj.push(JSON.parse(objMateria));
			}
		});

		return obj;
	},

	adicionarMateria: function () {
		var mensagens = new Array();
		Mensagem.limpar(MateriaPrimaFlorestalConsumida.container);
		var container = $(this).closest('fieldset');

		var materia = {
			Id: 0,
			Tid: '',
			MateriaPrimaConsumida: $('.ddlMateriaPrima :selected', container).val(),
			MateriaPrimaConsumidaTexto: $('.ddlMateriaPrima :selected', container).text(),
			Unidade: $('.ddlUnidade :selected', container).val(),
			UnidadeTexto: $('.ddlUnidade :selected', container).text(),
			EspecificarTexto: $('.txtEspecificar', container).val(),
			Quantidade: $('.txtQuantidade', container).val().replace('.', '').replace(',', '.')
		}

		if (materia.MateriaPrimaConsumida <= 0) {
			mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.MateriaPrimaFlorestalConsumidaObrigatoria));
		}

		if (materia.Unidade <= 0) {
			mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.UnidadeMateriaPrimaObrigatoria));
		}

		if (materia.Quantidade == '') {
			mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.QuantidadeMateriaPrimaObrigatoria));
		} else {

			if (isNaN(materia.Quantidade)) {
				mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.QuantidadeMateriaPrimaInvalida));
			}

			if (materia.Quantidade <= 0) {
				mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.QuantidadeMateriaPrimaMaiorZero));
			}
		}

		var materiatext = $('.ddlMateriaPrima :selected', container).text().trim().toLowerCase();
		if (materiatext == 'outros' || materiatext == 'outras' || materiatext == 'outro' || materiatext == 'outra') {

			if (materia.EspecificarTexto == '') {
				mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.EspecificarMateriaPrimaObrigatorio));
			} else {
				materia.MateriaPrimaConsumidaTexto = materia.EspecificarTexto;
			}
		}

		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var mat = (JSON.parse(obj));
				if (mat.MateriaPrimaConsumidaTexto == materia.MateriaPrimaConsumidaTexto) {
					mensagens.push(jQuery.extend(true, {}, MateriaPrimaFlorestalConsumida.settings.mensagens.MateriaPrimaFlorestalConsumidaDuplicada));
					Mensagem.gerar(MateriaPrimaFlorestalConsumida.container, mensagens);
					return;
				}
			}
		});

		if (mensagens.length > 0) {
			Mensagem.gerar(MateriaPrimaFlorestalConsumida.container, mensagens);
			return;
		}

		materia.Quantidade = materia.Quantidade.replace(".", ",");

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(materia));
		linha.find('.materiaPrimaConsumida').html(materia.MateriaPrimaConsumidaTexto).attr('title', materia.MateriaPrimaConsumidaTexto);
		linha.find('.unidade').html(materia.UnidadeTexto).attr('title', materia.UnidadeTexto);
		linha.find('.quantidade').html(materia.Quantidade).attr('title', materia.Quantidade);

		$('.dataGridTable tbody:last', container).append(linha);
		$('.txtQuantidade', container).val('');
		$('.txtEspecificar', container).val('');
		$('.ddlMateriaPrima', container).ddlFirst();
		MateriaPrimaFlorestalConsumida.gerenciarMateriaPrimaOutros(null, container);

		Listar.atualizarEstiloTable(MateriaPrimaFlorestalConsumida.container.find('.dataGridTable'));
	},

	excluirMateria: function () {
		var container = $(this).closest('fieldset');

		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));
	}
}
/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />
/// <reference path="coordenadaAtividade.js" />

BeneficiamentoMadeiraBeneficiamento = {
	settings: {
		urls: {
			salvar: '',
			editar: '',
			mergiar: '',
			visualizar: ''
		},
		idsTela: null,
		salvarCallBack: null,
		mensagens: {},
		textoAbrirModal: null,
		atualizarDependenciasModalTitulo: null,
		textoMerge: null,
		dependencias: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(BeneficiamentoMadeiraBeneficiamento.settings, options); }
		BeneficiamentoMadeiraBeneficiamento.container = MasterPage.getContent(container);

		BeneficiamentoMadeiraBeneficiamento.container.delegate('.ddlAtividade', 'change', BeneficiamentoMadeiraBeneficiamento.gerenciarAtividade);
		BeneficiamentoMadeiraBeneficiamento.container.delegate('.ddlCoordenadaTipoGeometria', 'change', CoordenadaAtividade.obterDadosCoordenadaAtividade);

		var editar = $('.hdnIsEditar', container).val();
		if (!editar) {
			CoordenadaAtividade.obterDadosTipoGeometria();
			CoordenadaAtividade.obterDadosCoordenadaAtividade();
		}

		CoordenadaAtividade.load(container);
		MateriaPrimaFlorestalConsumida.load(container);

		$('.fsBeneficiamento').each(function () {
			BeneficiamentoMadeiraBeneficiamento.gerenciarAtividade(null, this);
		});
	},

	gerenciarAtividade: function (e, container) {

		if (!container) container = $(this).closest('fieldset');

		var atividade = $('.ddlAtividade :selected', container).val();
		if (atividade == 0) {
			$('.divVolumeMadeiraSerrar', container).addClass('hide');
			$('.divVolumeMadeiraProcessar', container).addClass('hide');
		} else {
			if (atividade == BeneficiamentoMadeiraBeneficiamento.settings.idsTela.SerrariasQuandoNaoAssociadasAFabricacaoDeEstruturas) {
				$('.divVolumeMadeiraProcessar', container).addClass('hide');
				$('.divVolumeMadeiraSerrar', container).removeClass('hide');
			}

			if (atividade == BeneficiamentoMadeiraBeneficiamento.settings.idsTela.FabricacaoDeEstruturasDeMadeiraComAplicacaoRural) {
				$('.divVolumeMadeiraSerrar', container).addClass('hide');
				$('.divVolumeMadeiraProcessar', container).removeClass('hide');
			}
		}

	},

	obter: function () {
		var container = BeneficiamentoMadeiraBeneficiamento.container;
		var beneficiamento = [];

		$('.fsBeneficiamento', container).each(function () {
			var obj = {
				Id: Number($('.hdnBeneficiamentoId', this).val()),
				Identificador: $('.hdnIdentificador', this).val(),
				EmpreendimentoId: $('.hdnEmpreendimentoId', this).val(),
				Atividade: $('.ddlAtividade :selected', this).val(),
				VolumeMadeiraSerrar: $('.txtVolumeMadeiraSerrar', this).val(),
				VolumeMadeiraProcessar: $('.txtVolumeMadeiraProcessar', this).val(),
				EquipControlePoluicaoSonora: $('.txtEquipControlePoluicaoSonora', this).val(),
				CoordenadaAtividade: CoordenadaAtividade.obter(this, true),
				MateriasPrimasFlorestais: MateriaPrimaFlorestalConsumida.obter(this, true)
			};

			if (obj.Atividade == BeneficiamentoMadeiraBeneficiamento.settings.idsTela.SerrariasQuandoNaoAssociadasAFabricacaoDeEstruturas) {
				obj.VolumeMadeiraProcessar = '';
			} else {
				obj.VolumeMadeiraSerrar = '';
			}

			beneficiamento.push(obj);

		});

		return beneficiamento;
	}
}

CoordenadaAtividade = {
	settings: {
		urls: {
			salvar: '',
			urlObterDadosCoordenadaAtividade: '',
			urlObterDadosTipoGeometria: ''
		},
		mensagens: null
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(CoordenadaAtividade.settings, options); }
		CoordenadaAtividade.container = MasterPage.getContent(container);
	},

	obter: function (container, associarMultiplo) {
		if (!associarMultiplo) {
			container = CoordenadaAtividade.container;
		} else {
			if (!container) {
				container = $(this).closest('fieldset');
			}
		}

		var coord = $('.ddlCoordenadaAtividade :selected', container).val().toString().split('|');

		var obj = {
			Id: Number(coord[0]),
			Tipo: Number($('.ddlCoordenadaTipoGeometria :selected', container).val()),
			CoordX: coord[1] || 0,
			CoordY: coord[2] || 0
		}
		return obj;
	},

	obterDadosCoordenadaAtividade: function (e, container) {
		if (!container) {
			container = $(this).closest('fieldset');
		}

		var empreendimento = $('.hdnEmpreendimentoId', CoordenadaAtividade.container).val(); //container geral
		var tipo = $('.ddlCoordenadaTipoGeometria :selected', container).val(); //container local

		if (empreendimento == null) {
			$('.ddlCoordenadaAtividade', container).ddlClear();
			return;
		}

		$.ajax({
			url: CoordenadaAtividade.settings.urls.urlObterDadosCoordenadaAtividade,
			data: JSON.stringify({ empreendimentoId: empreendimento, tipoGeometria: tipo }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.CoordenadaAtividade) {
					$('.ddlCoordenadaAtividade', container).ddlClear();
					$('.ddlCoordenadaAtividade', container).ddlLoad(response.CoordenadaAtividade);
				}
			}
		});
	},

	obterDadosTipoGeometria: function (e, container, associarMultiplo) {
		if (!associarMultiplo) {
			container = CoordenadaAtividade.container;
		} else {
			container = $(this).closest('fieldset');
		}

		var empreendimento = $('.hdnEmpreendimentoId', CoordenadaAtividade.container).val();
		var caracterizacao = $('.hdnCaracterizacaoTipo', CoordenadaAtividade.container).val();

		if (empreendimento == null) {
			$('.ddlCoordenadaTipoGeometria', container).ddlClear();
			return;
		}

		$.ajax({
			url: CoordenadaAtividade.settings.urls.urlObterDadosTipoGeometria,
			data: JSON.stringify({ empreendimentoId: empreendimento, caracterizacaoTipo: caracterizacao }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.TiposGeometricos) {
					$('.ddlCoordenadaTipoGeometria', container).ddlLoad(response.TiposGeometricos);
				}

			}
		});
	}
}
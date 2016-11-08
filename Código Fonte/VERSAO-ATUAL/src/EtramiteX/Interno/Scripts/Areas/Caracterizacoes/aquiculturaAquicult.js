/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../jquery.ddl.js" />

AquiculturaAquicult = {
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
		if (options) { $.extend(AquiculturaAquicult.settings, options); }
		AquiculturaAquicult.container = MasterPage.getContent(container);

		AquiculturaAquicult.container.delegate('.ddlAtividade', 'change', AquiculturaAquicult.gerenciarAtividade);

		AquiculturaAquicult.container.delegate('.btnAdicionarCultivo', 'click', AquiculturaAquicult.adicionarCultivo);
		AquiculturaAquicult.container.delegate('.btnExcluirCultivo', 'click', AquiculturaAquicult.excluirCultivo);
		AquiculturaAquicult.container.delegate('.txtNumUnidadeCultivos', 'blur', AquiculturaAquicult.gerenciarNumUnidadeCultivoss);

		AquiculturaAquicult.container.delegate('.ddlCoordenadaTipoGeometria', 'change', CoordenadaAtividade.obterDadosCoordenadaAtividade);

		var editar = $('.hdnIsEditar', container).val();
		if (!editar) {
			CoordenadaAtividade.obterDadosTipoGeometria();
			CoordenadaAtividade.obterDadosCoordenadaAtividade();
		}

		CoordenadaAtividade.load(container);

		$('.fsAquicultura').each(function () {
			AquiculturaAquicult.gerenciarAtividade(null, this);
			AquiculturaAquicult.atualizarVolumeTotal(null, this);
			Listar.atualizarEstiloTable($(this).find('.dataGridTable'));
		});
	},

	gerenciarNumUnidadeCultivoss: function () {
		var container = $(this).closest('.divGrupo2');
		var qtdCultivos = $('.txtNumUnidadeCultivos', container).val();
		qtdCultivos = !isNaN(qtdCultivos) ? Number(qtdCultivos) : 0;
		var qtdCultivosAdicionados = 0;

		$('.hdnItemJSon', container).each(function () {
			if ($(this).val() != '') {
				qtdCultivosAdicionados++;
			}
		});

		if (qtdCultivosAdicionados <= 0) {
			if (qtdCultivos > 0) {
				$('.txtIdentificador', container).val(1);
				$('.txtVolume', container).focus();
			} else {
				$('.txtIdentificador', container).val('');
			}
		}
	},

	atualizarVolumeTotal: function (e, container) {
		if (!container) container = $(this).closest('.divCultivos');
		var total = 0;
		$('.hdnItemJSon', container).each(function () {
			var obj = String($(this).val());
			if (obj != '') {
				var objeto = (JSON.parse(obj));
				total += Number(objeto.Volume.toString().replace(',', '.'));
			}
		});

		$('.txtVolumeTotalCultivos', container).val(total.toString().replace('.', ','));

	},

	adicionarCultivo: function () {
		var mensagens = new Array();
		Mensagem.limpar(AquiculturaAquicult.container);

		var container = $(this).closest('.divGrupo2');
		var sufixo = $('.hdnIdentificador', $(container).closest('.fsAquicultura')).val();
		var qtdCultivos = $('.txtNumUnidadeCultivos', container).val();

		if (qtdCultivos == '') {
			mensagens.push(jQuery.extend(true, {}, AquiculturaAquicult.settings.mensagens.NumUnidadeCultivosObrigatorio));
			$(mensagens).each(function () {
				this.Campo += sufixo;
			});
			Mensagem.gerar(AquiculturaAquicult.container, mensagens);
			return;

		}

		qtdCultivos = !isNaN(qtdCultivos) ? Number(qtdCultivos) : 0;

		if (qtdCultivos <= 0) {
			mensagens.push(jQuery.extend(true, {}, AquiculturaAquicult.settings.mensagens.NumUnidadeCultivosMaiorZero));
			$(mensagens).each(function () {
				this.Campo += sufixo;
			});
			Mensagem.gerar(AquiculturaAquicult.container, mensagens);
			return;
		}

		var identificador = $('.txtIdentificador', container).val();
		identificador = !isNaN(identificador) ? Number(identificador) : 0;

		if (identificador == 0) identificador++;

		if (identificador > qtdCultivos) {
			mensagens.push(jQuery.extend(true, {}, AquiculturaAquicult.settings.mensagens.CultivosJaAdicionados));
			$(mensagens).each(function () {
				this.Campo += sufixo;
			});
			Mensagem.gerar(AquiculturaAquicult.container, mensagens);
			return;
		}

		var Cultivo = {
			Id: 0,
			Tid: '',
			Identificador: identificador,
			Volume: $('.txtVolume', container).val().replace('.', '').replace(',', '.')
		}

		if (Cultivo.Volume == '') {
			mensagens.push(jQuery.extend(true, {}, AquiculturaAquicult.settings.mensagens.VolumeCultivoObrigatorio));
			$(mensagens).each(function () {
				this.Campo += sufixo;
			});
			Mensagem.gerar(AquiculturaAquicult.container, mensagens);
			return;
		} else {
			if (isNaN(Cultivo.Volume)) {
				mensagens.push(jQuery.extend(true, {}, AquiculturaAquicult.settings.mensagens.VolumeCultivoInvalido));
				$(mensagens).each(function () {
					this.Campo += sufixo;
				});
				Mensagem.gerar(AquiculturaAquicult.container, mensagens);
				return;
			}
			if (Number(Cultivo.Volume) <= 0) {
				mensagens.push(jQuery.extend(true, {}, AquiculturaAquicult.settings.mensagens.VolumeCultivoMaiorZero));
				$(mensagens).each(function () {
					this.Campo += sufixo;
				});
				Mensagem.gerar(AquiculturaAquicult.container, mensagens);
				return;
			}
		}

		identificador++;

		Cultivo.Volume = Cultivo.Volume.replace(".", ",");

		var linha = $('.trTemplateRow', container).clone().removeClass('trTemplateRow hide');
		linha.find('.hdnItemJSon').val(JSON.stringify(Cultivo));
		linha.find('.identificador').html(Cultivo.Identificador).attr('title', Cultivo.Identificador);
		linha.find('.volume').html(Cultivo.Volume).attr('title', Cultivo.Volume);

		$('.dataGridTable tbody:last', container).append(linha);
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		$('.txtVolume', container).val('');
		$('.txtIdentificador', container).val(identificador);

		//colocando foco no campo volume enquanto pode adicionar Cultivos
		if (identificador != qtdCultivos + 1) {
			$('.txtVolume', container).focus();
		}

		AquiculturaAquicult.atualizarVolumeTotal(null, container);

	},

	excluirCultivo: function () {
		var container = $(this).closest('.divGrupo2');
		var linha = $(this).closest('tr');
		linha.remove();
		Listar.atualizarEstiloTable(container.find('.dataGridTable'));

		var identificador = $('.txtIdentificador', container).val();
		identificador = !isNaN(identificador) ? Number(identificador) : 0;
		identificador--;
		$('.txtIdentificador', container).val(identificador);

		var cont = 1;
		$('.hdnItemJSon', container).each(function (i, item) {
			var objCultivo = String($(item).val());
			if (objCultivo != '') {
				var obj = JSON.parse(objCultivo);
				obj.Identificador = cont;
				linha = $(item).closest('tr');
				linha.find('.hdnItemJSon').val(JSON.stringify(obj));
				linha.find('.identificador').html(obj.Identificador).attr('title', obj.Identificador);
				cont++;
			}
		});

		AquiculturaAquicult.atualizarVolumeTotal(null, container);
	},

	gerenciarAtividade: function (e, container) {
		if (!container) container = $(this).closest('.fsAquicultura');

		var atividade = $('.ddlAtividade :selected', container).val();
		if (atividade == 0) {
			$('.divGrupo1', container).addClass('hide');
			$('.divGrupo2', container).addClass('hide');
			$('.divGrupo3', container).addClass('hide');
		} else {

			var mostrarGrupo1 = (atividade == AquiculturaAquicult.settings.idsTela.Atividade01 || atividade == AquiculturaAquicult.settings.idsTela.Atividade02 || atividade == AquiculturaAquicult.settings.idsTela.Atividade03);
			var mostrarGrupo3 = (atividade == AquiculturaAquicult.settings.idsTela.Atividade10);
			var mostrarGrupo2 = (!mostrarGrupo1 && !mostrarGrupo3);

			if (mostrarGrupo1) {
				$('.divGrupo1', container).removeClass('hide');
				$('.divGrupo2', container).addClass('hide');
				$('.divGrupo3', container).addClass('hide');
			}

			if (mostrarGrupo2) {
				$('.divGrupo1', container).addClass('hide');
				$('.divGrupo2', container).removeClass('hide');
				$('.divGrupo3', container).addClass('hide');
			}

			if (mostrarGrupo3) {
				$('.divGrupo1', container).addClass('hide');
				$('.divGrupo2', container).addClass('hide');
				$('.divGrupo3', container).removeClass('hide');
			}
		}

	},

	obter: function () {
		var container = AquiculturaAquicult.container;
		var Aquicultura = [];

		$('.fsAquicultura', container).each(function () {
			var container = $(this).find('.divGrupo2');
			var obj = {
				Id: Number($('.hdnAquiculturaId', this).val()),
				Atividade: $('.ddlAtividade :selected', this).val(),
				EmpreendimentoId: $('.hdnEmpreendimentoId', this).val(),
				Identificador: $('.hdnIdentificador', this).val(),
				AreaInundadaTotal: $('.txtAreaInundadaTotal', this).val(),
				AreaCultivo: $('.txtAreaCultivo', this).val(),
				NumViveiros: $('.txtNumViveiros', this).val(),
				NumUnidadeCultivos: $('.txtNumUnidadeCultivos', container).val(),
				Cultivos: [],
				CoordenadaAtividade: CoordenadaAtividade.obter(this, true)
			};

			//Cultivos
			$('.hdnItemJSon', $(this).find('.divCultivos')).each(function () {
				var objCultivo = String($(this).val());
				if (objCultivo != '') {
					obj.Cultivos.push(JSON.parse(objCultivo));
				}
			});

			//Limpando campos que nao serao salvos de acordo com o grupo de atividades
			var AtividadeGrupo1 = (obj.Atividade == AquiculturaAquicult.settings.idsTela.Atividade01 || obj.Atividade == AquiculturaAquicult.settings.idsTela.Atividade02 || obj.Atividade == AquiculturaAquicult.settings.idsTela.Atividade03);
			var AtividadeGrupo3 = (obj.Atividade == AquiculturaAquicult.settings.idsTela.Atividade10);

			if (!AtividadeGrupo1) {
				obj.AreaInundadaTotal = '';
				obj.NumViveiros = '';
			}

			if (!AtividadeGrupo3) {
				obj.AreaCultivo = '';
			}


			Aquicultura.push(obj);
		});

		return Aquicultura;
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
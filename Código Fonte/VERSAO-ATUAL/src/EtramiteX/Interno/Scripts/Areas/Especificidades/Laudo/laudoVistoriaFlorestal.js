/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />

LaudoVistoriaFlorestal = {
	container: null,
	urlEspecificidade: null,
	urlObterDadosLaudoVistoriaFlorestal: null,
	Mensagens: null,
	idsTela: null,

	load: function (especificidadeRef) {
		LaudoVistoriaFlorestal.container = especificidadeRef;
		AtividadeEspecificidade.load(LaudoVistoriaFlorestal.container);
		TituloLaudoExploracaoFlorestal.load(LaudoVistoriaFlorestal.container);
		TituloLaudoExploracaoFlorestal.Mensagens = LaudoVistoriaFlorestal.Mensagens;
		TituloCondicionante.load($('.condicionantesContainer', LaudoVistoriaFlorestal.container));
		LaudoVistoriaFlorestal.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });

		LaudoVistoriaFlorestal.container.delegate('.ddlEspecificidadeConclusoes', 'change', LaudoVistoriaFlorestal.changeDdlEspecificidadeConclusoes);
		LaudoVistoriaFlorestal.container.delegate('.btnAddCaracterizacao', 'click', LaudoVistoriaFlorestal.gerenciarVisibilidadeDescricaoTecnica);
		LaudoVistoriaFlorestal.container.delegate('.btnExcluirExploracao', 'click', LaudoVistoriaFlorestal.gerenciarVisibilidadeDescricaoTecnica);

		LaudoVistoriaFlorestal.gerenciarCampos();
	},

	changeDdlEspecificidadeConclusoes: function () {
		var conclusao = $('.ddlEspecificidadeConclusoes', LaudoVistoriaFlorestal.container).val();
		$('.divRestricao', LaudoVistoriaFlorestal.container).toggleClass('hide', conclusao != LaudoVistoriaFlorestal.idsTela.EspecificidadeConclusaoFavoravelId);
		LaudoVistoriaFlorestal.gerenciarCampos();
	},

	gerenciarCampos: function () {
		var conclusao = $('.ddlEspecificidadeConclusoes', LaudoVistoriaFlorestal.container).val();
		if (conclusao == LaudoVistoriaFlorestal.idsTela.EspecificidadeConclusaoFavoravelId) {
			$('.divRestricao', LaudoVistoriaFlorestal.container).removeClass('hide');
		} else {
			$('.divRestricao', LaudoVistoriaFlorestal.container).addClass('hide');
		}
	},

	obterObjeto: function () {
		return {
			Id: Number(LaudoVistoriaFlorestal.container.find('.hdnLaudoVistoriaFlorestal').val()) || 0,
			Destinatario: LaudoVistoriaFlorestal.container.find('.ddlDestinatarios').val(),
			DataVistoria: { DataTexto: LaudoVistoriaFlorestal.container.find('.txtDataVistoria').val() },
			Objetivo: LaudoVistoriaFlorestal.container.find('.txtObjetivo').val(),
			Responsavel: LaudoVistoriaFlorestal.container.find('.ddlResponsaveisTecnico').val(),
			Consideracao: LaudoVistoriaFlorestal.container.find('.txtConsideracao').val(),
			ParecerDescricao: LaudoVistoriaFlorestal.container.find('.txtDescricao').val(),
			ParecerDescricaoDesfavoravel: LaudoVistoriaFlorestal.container.find('.txtDescricaoDesfavoravel').val(),
			FavoravelObrigatorio: !$('.descricaoFavoravel', LaudoVistoriaFlorestal.container).attr('class').includes('hide'),
			DesfavoravelObrigatorio: !$('.descricaoDesfavoravel', LaudoVistoriaFlorestal.container).attr('class').includes('hide'),
			Conclusao: LaudoVistoriaFlorestal.container.find('.ddlEspecificidadeConclusoes').val(),
			Restricao: LaudoVistoriaFlorestal.container.find('.txtRestricao:visible').val(),
			Anexos: LaudoVistoriaFlorestal.container.find('.fsArquivos').arquivo('obterObjeto'),		
			Caracterizacao: 4 //ExploracaoFlorestal
			//Caracterizacao: $('.exploracaoId', LaudoVistoriaFlorestal.container).toArray().filter(x => x.value > 0).map(x => x.value).join(',')
		};
	},

	obterDadosLaudoVistoriaFlorestal: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', LaudoVistoriaFlorestal.container).ddlClear();
			$('.ddlCaracterizacoes', LaudoVistoriaFlorestal.container).ddlClear();
			$('.ddlResponsaveisTecnico', LaudoVistoriaFlorestal.container).ddlClear();
			return;
		}

		$.ajax({
			url: LaudoVistoriaFlorestal.urlObterDadosLaudoVistoriaFlorestal,
			data: JSON.stringify({ id: protocolo.Id, empreendimento: protocolo.EmpreendimentoId }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(LaudoVistoriaFlorestal.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', LaudoVistoriaFlorestal.container).ddlLoad(response.Destinatarios);
				}
				if (response.Caracterizacoes) {
					var dropDown = $('.ddlCaracterizacoes', LaudoVistoriaFlorestal.container);
					dropDown.find('option').remove();
					dropDown.append('<option value="">*** Selecione ***</option>');					
					$.each(response.Caracterizacoes, function () {
						dropDown.append('<option value="' + this.Id + '" parecerfavoravel="' + this.ParecerFavoravel + '" parecerdesfavoravel="' + this.ParecerDesfavoravel + '">' + this.Texto + '</option>');
					});
					dropDown.removeClass('disabled');
					dropDown.removeAttr('disabled');
					dropDown.val(0);
				}

				if (response.ResponsaveisTecnico) {
					$('.ddlResponsaveisTecnico', LaudoVistoriaFlorestal.container).ddlLoad(response.ResponsaveisTecnico);
				}
			}
		});
	},

	obterAnexosObjeto: function () {
		var anexos = new Array();
		anexos = LaudoVistoriaFlorestal.container.find('.fsArquivos').arquivo('obterObjeto');
		return anexos;
	},

	gerenciarVisibilidadeDescricaoTecnica: function () {
		var tabela = $('.tabCaracterizacao tbody tr', LaudoVistoriaFlorestal.container);
		var favoravel = [];
		var desfavoravel = [];
		$(tabela).each(function (i, cod) {
			if ($('.parecerDesfavoravel', cod).val() != "") {
				desfavoravel.push($('.descricao', cod).attr('title') + ' (' + $('.parecerDesfavoravel', cod).val() + ')');
			}
			if ($('.parecerFavoravel', cod).val() != "") {
				favoravel.push($('.descricao', cod).attr('title') + ' (' + $('.parecerFavoravel', cod).val() + ')');
			}
		});

		var label = $("label[for='Laudo_ParecerDescricaoDesfavoravel']");
		label[0].textContent = 'Descrição do Parecer Técnico Desfavorável a Exploração *';
		if (desfavoravel.length > 0) {
			label[0].textContent = label[0].textContent + ': ' + desfavoravel.join(', ');
			$('.descricaoDesfavoravel', LaudoVistoriaFlorestal.container).toggleClass('hide', false);
		} else {
			$('.descricaoDesfavoravel', LaudoVistoriaFlorestal.container).toggleClass('hide', true);
		}

		label = $("label[for='Laudo_ParecerDescricao']");
		label[0].textContent = 'Descrição do Parecer Técnico Favorável a Exploração *';
		if (favoravel.length > 0) {
			label[0].textContent = label[0].textContent + ': ' + favoravel.join(', ');
			$('.descricaoFavoravel', LaudoVistoriaFlorestal.container).toggleClass('hide', false);
		} else {
			$('.descricaoFavoravel', LaudoVistoriaFlorestal.container).toggleClass('hide', true);
		}
	}
};

Titulo.settings.especificidadeLoadCallback = LaudoVistoriaFlorestal.load;
Titulo.addCallbackProtocolo(LaudoVistoriaFlorestal.obterDadosLaudoVistoriaFlorestal);
Titulo.settings.obterEspecificidadeObjetoFunc = LaudoVistoriaFlorestal.obterObjeto;
Titulo.settings.obterAnexosCallback = LaudoVistoriaFlorestal.obterAnexosObjeto;
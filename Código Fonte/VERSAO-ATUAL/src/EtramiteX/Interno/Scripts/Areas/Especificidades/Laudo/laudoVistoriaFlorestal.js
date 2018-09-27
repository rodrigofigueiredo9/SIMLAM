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
		TituloCondicionante.load($('.condicionantesContainer', LaudoVistoriaFlorestal.container));
		LaudoVistoriaFlorestal.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });

		LaudoVistoriaFlorestal.container.delegate('.ddlEspecificidadeConclusoes', 'change', LaudoVistoriaFlorestal.changeDdlEspecificidadeConclusoes);
		LaudoVistoriaFlorestal.container.delegate('.btnAddCaracterizacao', 'click', LaudoVistoriaFlorestal.adicionarCaracterizacao);
		LaudoVistoriaFlorestal.container.delegate('.btnExcluirExploracao', 'click', LaudoVistoriaFlorestal.excluirCaracterizacao);

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
			Caracterizacao: LaudoVistoriaFlorestal.container.find('.ddlCaracterizacoes').val()
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

	publicarMensagem: function (mensagens) {
		if (mensagens.length > 0) {
			Mensagem.gerar(LaudoVistoriaFlorestal.container, mensagens)
			return true;
		}
		return false;
	},

	adicionarCaracterizacao: function () {
		Mensagem.limpar(LaudoVistoriaFlorestal.container);
		var mensagens = new Array(); 
		var tabela = $('.tabCaracterizacao tbody tr', LaudoVistoriaFlorestal.container);
		
		var id = $('.ddlCaracterizacoes', LaudoVistoriaFlorestal.container).val();
		if (id == 0 || id == "") return;
		var descricao = $('.ddlCaracterizacoes option:selected', LaudoVistoriaFlorestal.container).html(); 
		var parecerFavoravel = $('.ddlCaracterizacoes option:selected', LaudoVistoriaFlorestal.container).attr('parecerfavoravel');
		var parecerDesfavoravel = $('.ddlCaracterizacoes option:selected', LaudoVistoriaFlorestal.container).attr('parecerdesfavoravel');

		$(tabela).each(function (i, cod) {			
			if ($('.exploracaoId', cod).val() == id) {
				mensagens.push(LaudoVistoriaFlorestal.Mensagens.CaracterizacaoDuplicada);
			}
		});
		
		if (LaudoVistoriaFlorestal.publicarMensagem(mensagens)) {
			return false;
		}

		//monta o objeto 
		var objeto = {
			Id: id,
			CodigoExploracaoTexto: descricao,
			ParecerFavoravel: parecerFavoravel,
			ParecerDesfavoravel: parecerDesfavoravel
		};

		var linha = '';
		linha = $('.trTemplateRow', LaudoVistoriaFlorestal.container).clone();

		linha.find('.hdnItemJSon').val(JSON.stringify(objeto));
		linha.find('.descricao').text(descricao);
		linha.find('.descricao').attr('title', descricao);
		linha.find('.exploracaoId').val(id);
		linha.find('.parecerFavoravel').val(parecerFavoravel);
		linha.find('.parecerDesfavoravel').val(parecerDesfavoravel);

		linha.removeClass('trTemplateRow hide');
		$('.tabCaracterizacao > tbody:last', LaudoVistoriaFlorestal.container).append(linha);

		Listar.atualizarEstiloTable($('.tabCaracterizacao', LaudoVistoriaFlorestal.container));

		//limpa os campos de texto 
		$('.ddlCaracterizacoes', LaudoVistoriaFlorestal.container).val('');
		LaudoVistoriaFlorestal.gerenciarVisibilidadeDescricaoTecnica();
	},

	excluirCaracterizacao: function () {
		$(this).closest('tr').remove();
		LaudoVistoriaFlorestal.gerenciarVisibilidadeDescricaoTecnica();
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
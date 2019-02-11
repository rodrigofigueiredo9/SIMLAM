/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

AutorizacaoExploracaoFlorestal = {
	container: null,
	settings: {
		urls: {
			obterDadosAutorizacaoExploracaoFlorestal: '',
			obterLaudoVistoria: '',
			validarAssociarVistoria: ''
		},
		modelos: {}
	},

	load: function (especificidadeRef) {
		AutorizacaoExploracaoFlorestal.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
		TituloCondicionante.load($('.condicionantesContainer', AutorizacaoExploracaoFlorestal.container));
		TituloAutorizacaoExploracaoFlorestal.load(AutorizacaoExploracaoFlorestal.container);
		AutorizacaoExploracaoFlorestal.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp', 'pdf'] });
		AutorizacaoExploracaoFlorestal.container.delegate('.btnVistoriaAdicionar', 'click', AutorizacaoExploracaoFlorestal.btnLaudoVistoriaAdicionarClick);
		AutorizacaoExploracaoFlorestal.container.delegate('.btnVistoriaLimpar', 'click', AutorizacaoExploracaoFlorestal.btnLaudoVistoriaLimparClick);
	},

	obterDadosAutorizacaoExploracaoFlorestal: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', AutorizacaoExploracaoFlorestal.container).ddlClear();
			$('.ddlExploracoes', AutorizacaoExploracaoFlorestal.container).ddlClear();
			return;
		}

		$.ajax({ url: AutorizacaoExploracaoFlorestal.settings.urls.obterDadosAutorizacaoExploracaoFlorestal,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(AutorizacaoExploracaoFlorestal.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios.length > 0) {
					$('.ddlDestinatarios', AutorizacaoExploracaoFlorestal.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	btnLaudoVistoriaLimparClick: function () {
		AutorizacaoExploracaoFlorestal.container.find('.hdnLaudoVistoriaFlorestalId').val('');
		AutorizacaoExploracaoFlorestal.container.find('.txtLaudoVistoriaFlorestal').val('');
		AutorizacaoExploracaoFlorestal.container.find('.btnVistoriaAdicionar').toggleClass('hide', false);
		AutorizacaoExploracaoFlorestal.container.find('.btnVistoriaLimpar').toggleClass('hide', true);
		AutorizacaoExploracaoFlorestal.container.find('.ddlExploracoes').val('');
	},

	btnLaudoVistoriaAdicionarClick: function () {
		Modal.abrir(AutorizacaoExploracaoFlorestal.settings.urls.obterLaudoVistoria, { modelosCodigos: AutorizacaoExploracaoFlorestal.settings.modelos.LaudoVistoriaFlorestal }, function (container) {
			Modal.defaultButtons(container);
			TituloListar.load(container, { associarFuncao: AutorizacaoExploracaoFlorestal.onAssociarLaudoVistoria });
		}, Modal.tamanhoModalGrande);
	},

	onAssociarLaudoVistoria: function (titulo) {
		var params = Titulo.ObterObjeto();
		var retorno = MasterPage.validarAjax(AutorizacaoExploracaoFlorestal.settings.urls.validarAssociarVistoria, { tituloAssociadoId: titulo.Id }, null, false);

		if (retorno.EhValido) {
			AutorizacaoExploracaoFlorestal.container.find('.hdnLaudoVistoriaFlorestalId').val(titulo.Id);
			AutorizacaoExploracaoFlorestal.container.find('.hdnLaudoVistoriaFlorestalIdRelacionamento').val(0);
			AutorizacaoExploracaoFlorestal.container.find('.txtLaudoVistoriaFlorestal').val(titulo.ModeloSigla + ' - ' + titulo.Numero);
			AutorizacaoExploracaoFlorestal.container.find('.btnVistoriaAdicionar').toggleClass('hide', true);
			AutorizacaoExploracaoFlorestal.container.find('.btnVistoriaLimpar').toggleClass('hide', false);
		}

		AutorizacaoExploracaoFlorestal.carregarExploracoes(titulo.Id);

		return { Msg: retorno.Msg, FecharModal: retorno.Msg.length <= 0 };
	},

	carregarExploracoes: function (tituloAssociadoId) {
		if (!tituloAssociadoId) return;

		$.ajax({
			url: AutorizacaoExploracaoFlorestal.settings.urls.obterDadosExploracao,
			data: { tituloAssociadoId: tituloAssociadoId },
			cache: false,
			async: true,
			type: 'GET',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(AutorizacaoExploracaoFlorestal.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Exploracoes.length > 0) {
					var dropDown = $('.ddlExploracoes', AutorizacaoExploracaoFlorestal.container);
					dropDown.find('option').remove();
					dropDown.append('<option value="">*** Selecione ***</option>');
					$.each(response.Exploracoes, function () {
						dropDown.append('<option value="' + this.ExploracaoFlorestalId + '" detalhes="' + JSON.stringify(this.TituloExploracaoFlorestalExploracaoList).replaceAll('"', "'") + '">' + this.ExploracaoFlorestalTexto + '</option>');
					});
					dropDown.removeClass('disabled');
					dropDown.removeAttr('disabled');
					dropDown.val(0);
				}
			}
		});
	},

	obterTitulosAssociado: function () {
		if ($('.hdnLaudoVistoriaFlorestalId', AutorizacaoExploracaoFlorestal.container).val() == 0) {
			return new Array();
		}

		return new Array(
		{
			Id: $('.hdnLaudoVistoriaFlorestalId', AutorizacaoExploracaoFlorestal.container).val(),
			IdRelacionamento: $('.hdnLaudoVistoriaFlorestalIdRelacionamento', AutorizacaoExploracaoFlorestal.container).val()
		});
	},

	obterAnexosObjeto: function () {
		var anexos = new Array();
		anexos = AutorizacaoExploracaoFlorestal.container.find('.fsArquivos').arquivo('obterObjeto');
		return anexos;
	},
	
	obterObjeto: function () {
		return {
			Destinatario: AutorizacaoExploracaoFlorestal.container.find('.ddlDestinatarios').val(),
			Observacao: AutorizacaoExploracaoFlorestal.container.find('.txtObservacoes').val()
		};
	}
};

Titulo.settings.especificidadeLoadCallback = AutorizacaoExploracaoFlorestal.load;
Titulo.settings.obterEspecificidadeObjetoFunc = AutorizacaoExploracaoFlorestal.obterObjeto;
Titulo.addCallbackProtocolo(AutorizacaoExploracaoFlorestal.obterDadosAutorizacaoExploracaoFlorestal);
Titulo.settings.obterTitulosAssociadoCallback = AutorizacaoExploracaoFlorestal.obterTitulosAssociado;
Titulo.settings.obterAnexosCallback = AutorizacaoExploracaoFlorestal.obterAnexosObjeto;
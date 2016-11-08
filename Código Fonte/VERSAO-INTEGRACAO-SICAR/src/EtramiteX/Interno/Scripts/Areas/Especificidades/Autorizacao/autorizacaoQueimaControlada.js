/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

AutorizacaoQueimaControlada = {
	container: null,
	settings: {
		urls: {
			obterDadosAutorizacaoQueimaControlada: '',
			obterLaudoVistoria: '',
			validarAssociarVistoria: ''
		},
		modelos: {}
	},

	load: function (especificidadeRef) {
		AutorizacaoQueimaControlada.container = especificidadeRef;
		AtividadeEspecificidade.load(especificidadeRef);
		TituloCondicionante.load($('.condicionantesContainer', AutorizacaoQueimaControlada.container));
		AutorizacaoQueimaControlada.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp', 'pdf'] });
		AutorizacaoQueimaControlada.container.delegate('.btnVistoriaAdicionar', 'click', AutorizacaoQueimaControlada.btnLaudoVistoriaAdicionarClick);
		AutorizacaoQueimaControlada.container.delegate('.btnVistoriaLimpar', 'click', AutorizacaoQueimaControlada.btnLaudoVistoriaLimparClick);
	},

	obterDadosAutorizacaoQueimaControlada: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', AutorizacaoQueimaControlada.container).ddlClear();
			return;
		}

		$.ajax({ url: AutorizacaoQueimaControlada.settings.urls.obterDadosAutorizacaoQueimaControlada,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				alert('error');
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(AutorizacaoQueimaControlada.container));

			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios.length > 0) {
					$('.ddlDestinatarios', AutorizacaoQueimaControlada.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	btnLaudoVistoriaLimparClick: function () {
		AutorizacaoQueimaControlada.container.find('.hdnLaudoVistoriaFlorestalId').val('');
		AutorizacaoQueimaControlada.container.find('.txtLaudoVistoriaFlorestal').val('');
		AutorizacaoQueimaControlada.container.find('.btnVistoriaAdicionar').toggleClass('hide', false);
		AutorizacaoQueimaControlada.container.find('.btnVistoriaLimpar').toggleClass('hide', true);
	},

	btnLaudoVistoriaAdicionarClick: function () {
		Modal.abrir(AutorizacaoQueimaControlada.settings.urls.obterLaudoVistoria, { modelosCodigos: AutorizacaoQueimaControlada.settings.modelos.LaudoVistoriaFlorestal }, function (container) {
			Modal.defaultButtons(container);
			TituloListar.load(container, { associarFuncao: AutorizacaoQueimaControlada.onAssociarLaudoVistoria });
		}, Modal.tamanhoModalGrande);
	},

	onAssociarLaudoVistoria: function (titulo) {
		var params = Titulo.ObterObjeto();
		var retorno = MasterPage.validarAjax(AutorizacaoQueimaControlada.settings.urls.validarAssociarVistoria, { tituloAssociadoId: titulo.Id }, null, false);

		if (retorno.EhValido) {
			AutorizacaoQueimaControlada.container.find('.hdnLaudoVistoriaFlorestalId').val(titulo.Id);
			AutorizacaoQueimaControlada.container.find('.hdnLaudoVistoriaFlorestalIdRelacionamento').val(0);
			AutorizacaoQueimaControlada.container.find('.txtLaudoVistoriaFlorestal').val(titulo.ModeloSigla + ' - ' + titulo.Numero);
			AutorizacaoQueimaControlada.container.find('.btnVistoriaAdicionar').toggleClass('hide', true);
			AutorizacaoQueimaControlada.container.find('.btnVistoriaLimpar').toggleClass('hide', false);
		}

		return { Msg: retorno.Msg, FecharModal: retorno.Msg.length <= 0 };
	},

	obterTitulosAssociado: function () {
		if ($('.hdnLaudoVistoriaFlorestalId', AutorizacaoQueimaControlada.container).val() == 0) {
			return new Array();
		}

		return new Array(
		{
			Id: $('.hdnLaudoVistoriaFlorestalId', AutorizacaoQueimaControlada.container).val(),
			IdRelacionamento: $('.hdnLaudoVistoriaFlorestalIdRelacionamento', AutorizacaoQueimaControlada.container).val()
		});
	},

	obterObjeto: function () {
		return {
			Destinatario: AutorizacaoQueimaControlada.container.find('.ddlDestinatarios').val()
		};
	}
};

Titulo.settings.especificidadeLoadCallback = AutorizacaoQueimaControlada.load;
Titulo.settings.obterEspecificidadeObjetoFunc = AutorizacaoQueimaControlada.obterObjeto;
Titulo.addCallbackProtocolo(AutorizacaoQueimaControlada.obterDadosAutorizacaoQueimaControlada);
Titulo.settings.obterTitulosAssociadoCallback = AutorizacaoQueimaControlada.obterTitulosAssociado;
Titulo.settings.obterAnexosCallback = AutorizacaoQueimaControlada.obterAnexosObjeto;
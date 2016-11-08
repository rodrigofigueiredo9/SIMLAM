/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />
/// <reference path="../../../Titulo/titulo.js" />

CertidaoDebito = {
	container: null,
	urlObterDadosCertidaoDebito: '',
	urlObterFiscalizacoesPorAutuado: '',
	urlVisualizarFiscalizacao: '',

	load: function (especificidadeRef) {
		CertidaoDebito.container = especificidadeRef;
		AtividadeEspecificidade.load(CertidaoDebito.container);
		CertidaoDebito.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });

		CertidaoDebito.container.delegate('.btnVisualizarFiscalizacao', 'click', CertidaoDebito.visualizarFiscalizacao);
		CertidaoDebito.container.delegate('.ddlDestinatarios', 'change', CertidaoDebito.ObterFiscalizacoesPorAutuado);
	},

	obterDadosCertidaoDebito: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', CertidaoDebito.container).ddlClear();
			return;
		}

		$.ajax({
			url: CertidaoDebito.urlObterDadosCertidaoDebito,
			data: JSON.stringify({ id: protocolo.Id }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CertidaoDebito.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', CertidaoDebito.container).ddlLoad(response.Destinatarios);
				}
			}
		});
	},

	ObterFiscalizacoesPorAutuado: function () {

		autuado = Number($('.ddlDestinatarios :selected', CertidaoDebito.container).val()) || 0;

		$.ajax({
			url: CertidaoDebito.urlObterFiscalizacoesPorAutuado,
			data: JSON.stringify({ autuadoId: autuado }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CertidaoDebito.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Html) {
					$('.divFiscalizacoes', CertidaoDebito.container).empty();
					$('.divFiscalizacoes', CertidaoDebito.container).append(response.Html);

					Listar.atualizarEstiloTable();
				}

				$('.txtTipoCertidao', CertidaoDebito.container).val(response.StrResultado);
			}
		});
	},

	visualizarFiscalizacao: function () {

		var itemId = parseInt($(this).closest('tr').find('.itemId:first').val());
		var params = { id: itemId };

		Modal.abrir(CertidaoDebito.urlVisualizarFiscalizacao, params, function (container) {
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalGrande);

	},

	obterObjeto: function () {
		var obj = {
			Destinatario: CertidaoDebito.container.find('.ddlDestinatarios').val(),
			Fiscalizacoes: [],
			Anexos: CertidaoDebito.container.find('.fsArquivos').arquivo('obterObjeto')
		};

		$('.hdnItemJSon', CertidaoDebito.container).each(function () {
			obj.Fiscalizacoes.push(JSON.parse($(this).val()));
		});

		return obj;
	}
};

Titulo.settings.especificidadeLoadCallback = CertidaoDebito.load;
Titulo.addCallbackProtocolo(CertidaoDebito.obterDadosCertidaoDebito);
Titulo.addCallbackProtocolo(CertidaoDebito.ObterFiscalizacoesPorAutuado);
Titulo.settings.obterEspecificidadeObjetoFunc = CertidaoDebito.obterObjeto;
Titulo.settings.obterAnexosCallback = CertidaoDebito.obterAnexosObjeto;
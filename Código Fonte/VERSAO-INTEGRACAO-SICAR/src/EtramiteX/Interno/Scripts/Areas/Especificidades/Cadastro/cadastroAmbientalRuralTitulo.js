/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

CadastroAmbientalRuralTitulo = {
	container: null,
	urlObterDadosCadastroAmbientalRuralTitulo: '',

	load: function (especificidadeRef) {
		CadastroAmbientalRuralTitulo.container = especificidadeRef;
		AtividadeEspecificidade.load(CadastroAmbientalRuralTitulo.container);
		TituloCondicionante.load($('.condicionantesContainer', CadastroAmbientalRuralTitulo.container));
		CadastroAmbientalRuralTitulo.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });
	},

	obterDadosCadastroAmbientalRuralTitulo: function (protocolo) {
		if (protocolo == null) {
			$('.ddlDestinatarios', CadastroAmbientalRuralTitulo.container).ddlClear();
			return;
		}

		$.ajax({
			url: CadastroAmbientalRuralTitulo.urlObterDadosCadastroAmbientalRuralTitulo,
			data: JSON.stringify({ id: protocolo.Id, Empreendimento: { Id: protocolo.EmpreendimentoId } }),
			cache: false,
			async: false,
			type: 'POST',
			typeData: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(CadastroAmbientalRuralTitulo.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.Destinatarios) {
					$('.ddlDestinatarios', CadastroAmbientalRuralTitulo.container).ddlLoad(response.Destinatarios);
				}

				if (response.Matriculas) {
					$('.txtMatricula', CadastroAmbientalRuralTitulo.container).val(response.Matriculas);
				}
			}
		});
	},

	obterObjeto: function () {
		return {
			Destinatario: CadastroAmbientalRuralTitulo.container.find('.ddlDestinatarios').val(),
			Matricula: $('.txtMatricula', CadastroAmbientalRuralTitulo.container).val(),
			Anexos: CadastroAmbientalRuralTitulo.container.find('.fsArquivos').arquivo('obterObjeto')
		};
	}
};

Titulo.settings.especificidadeLoadCallback = CadastroAmbientalRuralTitulo.load;
Titulo.addCallbackProtocolo(CadastroAmbientalRuralTitulo.obterDadosCadastroAmbientalRuralTitulo);
Titulo.settings.obterEspecificidadeObjetoFunc = CadastroAmbientalRuralTitulo.obterObjeto;
Titulo.settings.obterAnexosCallback = CadastroAmbientalRuralTitulo.obterAnexosObjeto;
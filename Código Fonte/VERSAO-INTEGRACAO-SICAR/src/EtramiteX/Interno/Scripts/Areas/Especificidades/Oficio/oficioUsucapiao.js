/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />

OficioUsucapiao = {
	container: null,
	urlObterDadosOficioUsucapiao: '',

	load: function (especificidadeRef) {
		OficioUsucapiao.container = especificidadeRef;
		OficioUsucapiao.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp', 'pdf'] });
		AtividadeEspecificidade.load(OficioUsucapiao.container);
	},

	obterDadosOficioUsucapiao: function (protocolo) {
		if (protocolo == null) {
			$('.rdbEmpreendimentoTipo', OficioUsucapiao.container).removeAttr('checked');
			return;
		}

		$.ajax({ url: OficioUsucapiao.urlObterDadosOficioUsucapiao,
			data: JSON.stringify(protocolo),
			cache: false,
			async: true,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(OficioUsucapiao.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				$('.rdbEmpreendimentoTipo', OficioUsucapiao.container).removeAttr('checked');

				if (response.ZonaLocalizacao == 1) {
					$('.rdbEmpreendimentoTipoUrbano', OficioUsucapiao.container).attr('checked', 'checked');
				}

				if (response.ZonaLocalizacao == 2) {
					$('.rdbEmpreendimentoTipoRural', OficioUsucapiao.container).attr('checked', 'checked');
				}
			}
		});
	},

	obterObjeto: function () {
		return {
			Destinatario: $('.ddlDestinatarios', OficioUsucapiao.container).val(),
			Dimensao: $('.txtDimensao', OficioUsucapiao.container).val(),
			EmpreendimentoTipo: $('.rdbEmpreendimentoTipo:checked', OficioUsucapiao.container).val(),
			Destinatario: $('.txtDestinatarioPGE', OficioUsucapiao.container).val(),
			Descricao: $('.txtDescricaoOficio', OficioUsucapiao.container).val(),
			Anexos: OficioUsucapiao.container.find('.fsArquivos').arquivo('obterObjeto')
		};
	},

	obterAnexosObjeto: function () {
		var anexos = new Array();
		anexos = OficioUsucapiao.container.find('.fsArquivos').arquivo('obterObjeto');
		return anexos;
	}
};

Titulo.settings.especificidadeLoadCallback = OficioUsucapiao.load;
Titulo.settings.obterEspecificidadeObjetoFunc = OficioUsucapiao.obterObjeto;
Titulo.addCallbackProtocolo(OficioUsucapiao.obterDadosOficioUsucapiao);
Titulo.settings.obterAnexosCallback = OficioUsucapiao.obterAnexosObjeto;
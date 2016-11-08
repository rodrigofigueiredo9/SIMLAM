/// <reference path="../../../Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../../masterpage.js" />
/// <reference path="../../../jquery.ddl.js" />

CertidaoAnuencia = {
	container: null,
	urlObterDadosCertidaoAnuencia: '',

	load: function (especificidadeRef) {
		CertidaoAnuencia.container = especificidadeRef;
		AtividadeEspecificidade.load(CertidaoAnuencia.container);
		CertidaoAnuencia.container.find('.fsArquivos').arquivo({ extPermitidas: ['jpg', 'gif', 'png', 'bmp'] });
		DestinatarioEspecificidade.load(CertidaoAnuencia.container);
	},

	obterObjeto: function () {
		return {
			Destinatarios: DestinatarioEspecificidade.obter(),
			Certificacao: $('.txtCertificacao', CertidaoAnuencia.container).val(),
			Anexos: CertidaoAnuencia.container.find('.fsArquivos').arquivo('obterObjeto')
		};
	}
};

Titulo.settings.especificidadeLoadCallback = CertidaoAnuencia.load;
Titulo.settings.obterEspecificidadeObjetoFunc = CertidaoAnuencia.obterObjeto;
Titulo.settings.obterAnexosCallback = CertidaoAnuencia.obterAnexosObjeto;
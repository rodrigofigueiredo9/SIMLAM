/// <reference path="../../JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../../masterpage.js" />

Caracterizacao = {
	settings: {
		urls: {
			CriarProjetoGeo: '',
			EditarProjetoGeo: '',
			VisualizarProjetoGeo: '',
			CriarDscLicAtividade: '',
			EditarDscLicAtividade: '',
			VisualizarDscLicAtividade: ''
		}
	},
	container: null,

	load: function (container, options) {
		if (options) { $.extend(Caracterizacao.settings, options); }
		Caracterizacao.container = MasterPage.getContent(container);

		Caracterizacao.container.delegate('.btnVisualizar', 'click', Caracterizacao.onVisualizar);
		Caracterizacao.container.delegate('.btnProjetoGeografico', 'click', Caracterizacao.onProjetoGeografico);
		Caracterizacao.container.delegate('.btnEditar', 'click', Caracterizacao.onEditar);
		Caracterizacao.container.delegate('.btnExcluir', 'click', Caracterizacao.onExcluir);
		Caracterizacao.container.delegate('.btnAdicionar', 'click', Caracterizacao.onAdicionar);
		Caracterizacao.container.delegate('.btnDscLicAtividade', 'click', Caracterizacao.onDscLicAtividade);
	},

	onAdicionar: function () {
		var linha = $(this).closest('tr');
		var id = $('.hdnEmpreendimentoId', Caracterizacao.container).val();

		if ($('.hdnPossuiProjetoGeo', linha).val() == 'true') {
			var caracterizacaoTipo = $('.hdnCaracterizacaoTipo', linha).val();
			var isCadastrarCaracterizacao = !linha.closest('fieldset').hasClass('fsCadastradas');
			var projeto = $('.hdnProjetoGeograficoId', linha).val();
			var isCadastrar = !$(this).hasClass('projetoGeografico');
			var urlProjeto = (isCadastrar) ? Caracterizacao.settings.urls.CriarProjetoGeo : Caracterizacao.settings.urls.EditarProjetoGeo;

			if (isCadastrar) {
				urlProjeto = urlProjeto + '?empreendimento=' + id + '&tipo=' + caracterizacaoTipo + '&isCadastrarCaracterizacao=' + isCadastrarCaracterizacao;
			} else {
				urlProjeto = urlProjeto + '?id=' + projeto + '&empreendimento=' + id + '&tipo=' + caracterizacaoTipo + '&isCadastrarCaracterizacao=' + isCadastrarCaracterizacao;
			}
			MasterPage.redireciona(urlProjeto);
		} else {
			MasterPage.redireciona($('.hdnUrlCriar', linha).val() + '/' + id);
		}
	},

	onVisualizar: function () {
		var id = $('.hdnEmpreendimentoId', Caracterizacao.container).val();
		MasterPage.redireciona($('.hdnUrlVisualizar', $(this).closest('tr')).val() + '/' + id);
	},

	onProjetoGeografico: function () {
		var linha = $(this).closest('tr');
		var empreendimento = $('.hdnEmpreendimentoId', Caracterizacao.container).val();
		var projeto = $('.hdnProjetoGeograficoId', linha).val();
		var caracterizacaoTipo = $('.hdnCaracterizacaoTipo', linha).val();
		var isCadastrarCaracterizacao = !linha.closest('fieldset').hasClass('fsCadastradas');
		var isCadastrar = !$('.btnProjetoGeografico', Caracterizacao.container).hasClass('projetoGeografico');
		var urlProjeto = (isCadastrar) ? Caracterizacao.settings.urls.CriarProjetoGeo : Caracterizacao.settings.urls.EditarProjetoGeo;
		var isVisualizar = $('.hdnProjetoGeograficoVisualizar', linha).val() == 'True';

		urlProjeto = (isVisualizar) ? Caracterizacao.settings.urls.VisualizarProjetoGeo : urlProjeto;

		if (isCadastrar) {
			urlProjeto = urlProjeto + '?empreendimento=' + empreendimento + '&tipo=' + caracterizacaoTipo + '&isCadastrarCaracterizacao=' + isCadastrarCaracterizacao;
		} else {
			urlProjeto = urlProjeto + '?id=' + projeto + '&empreendimento=' + empreendimento + '&tipo=' + caracterizacaoTipo + '&isCadastrarCaracterizacao=' + isCadastrarCaracterizacao;
		}

		MasterPage.redireciona(urlProjeto);
	},

	onEditar: function () {
		var id = $('.hdnEmpreendimentoId', Caracterizacao.container).val();
		MasterPage.redireciona($('.hdnUrlEditar', $(this).closest('tr')).val() + '/' + id);
	},

	onExcluir: function () {
		var linha = $(this).closest('tr');

		Modal.excluir({
			'urlConfirm': $('.hdnUrlExcluirConfirm', linha).val(),
			'urlAcao': $('.hdnUrlExcluir', linha).val(),
			'id': $('.hdnEmpreendimentoId', Caracterizacao.container).val(),
			'callBack': Caracterizacao.callBackExcluirCaracterizacao,
			'naoExecutarUltimaBusca': true
		});
	},

	callBackExcluirCaracterizacao: function (data) {
		MasterPage.redireciona(data.urlRedireciona);
	},

	onDscLicAtividade: function () {

		var linha = $(this).closest('tr');
		var empreendimento = $('.hdnEmpreendimentoId', Caracterizacao.container).val();
		var dscLicAtividade = parseInt($('.hdnDscLicAtividade', linha).val());
		var caracterizacaoTipo = $('.hdnCaracterizacaoTipo', linha).val();
		var isCadastrarCaracterizacao = !linha.closest('fieldset').hasClass('fsCadastradas');
		var isCadastrar = dscLicAtividade < 1;
		var urlDscLicAtividade = (isCadastrar) ? Caracterizacao.settings.urls.CriarDscLicAtividade : Caracterizacao.settings.urls.EditarDscLicAtividade;
		var isVisualizar = $('.hdnDscLicAtividadeVisualizar', linha).val() == 'True';

		urlDscLicAtividade = (isVisualizar) ? Caracterizacao.settings.urls.VisualizarDscLicAtividade : urlDscLicAtividade;

		if (isCadastrar) {
			urlDscLicAtividade += '?empreendimento=' + empreendimento + '&tipo=' + caracterizacaoTipo + '&isCadastrarCaracterizacao=' + isCadastrarCaracterizacao;
		} else {
			urlDscLicAtividade += '?id=' + dscLicAtividade + '&empreendimento=' + empreendimento + '&tipo=' + caracterizacaoTipo + '&isCadastrarCaracterizacao=' + isCadastrarCaracterizacao;
		}
		MasterPage.redireciona(urlDscLicAtividade);
	}
}
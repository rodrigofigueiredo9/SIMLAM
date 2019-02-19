/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="partial.js" />

EmpreendimentoTela = {
	settings: {
		urls: {
			avancar: '',
			voltar: '',
			salvarCadastrar: '',
			editar: '',
			verificarCnpj: '',
			visualizar: '',
			associarResponsavelModal: '',
			associarResponsavelEditarModal: '',
			associarAtividadeModal: '',
			enderecoMunicipio: '',
			pessoaAssociar: '',
			verificarLocalizaoEmpreendimento: ''
		},
		onAssociarCallback: null,
		denominadoresSegmentos: null,
		msgs: {},
		idsTela: null
	},

	content: null,

	load: function (content, options) {
		EmpreendimentoTela.content = content;

		if (options) {
			$.extend(EmpreendimentoTela.settings, options);
		}

		Empreendimento.load($('.empreendimentoPartial', content), {
			onSalvar: EmpreendimentoTela.onSalvar,
			onEditarEnter: EmpreendimentoTela.onEditarEnter,
			onCriarEnter: EmpreendimentoTela.onCriarEnter,
			onVisualizarEnter: EmpreendimentoTela.onVisualizarEnter,
			onVerificarEnter: EmpreendimentoTela.onVerificarEnter,
			urls: EmpreendimentoTela.settings.urls,
			msgs: EmpreendimentoTela.settings.msgs,
			idsTela: EmpreendimentoTela.settings.idsTela,
			denominadoresSegmentos: EmpreendimentoTela.settings.denominadoresSegmentos
		});

		$('.btnEmpAvancar', content).click(EmpreendimentoTela.onAvancarClick);
		$('.btnEmpVoltar', content).click(EmpreendimentoTela.onVoltarClick);
		$('.btnEmpSalvar', content).click(EmpreendimentoTela.onBtnSalvarClick);
	},

	onAvancarClick: function () {
		Empreendimento.avancar();
		EmpreendimentoTela.setBotoes(false, false, true, true, true);
	},

	onVoltarClick: function () {
		Empreendimento.voltar();
		EmpreendimentoTela.setBotoes(false, true, false, false, true);
		EmpreendimentoTela.limparMensagens(EmpreendimentoTela.content);
	},

	onSalvar: function (partialContent, responseJson, isEditar) {
		MasterPage.redireciona(responseJson.UrlRedireciona);
	},

	// mostra botão cancelar e salvar
	onEditarEnter: function () {
		EmpreendimentoTela.setBotoes(true, false, false, true, true);
		EmpreendimentoTela.limparMensagens(EmpreendimentoTela.content);
	},

	// mostra botão cancelar e salvar
	onCriarEnter: function () {
		EmpreendimentoTela.setBotoes(false, true, false, false, true);
	},

	onBtnSalvarClick: function (_this, onCall) {
		Empreendimento.salvar(true);
	},

	setBotoes: function (btnEditarVisible, btnAvancarVisible, btnSalvarVisible, btnVoltarVisible, btnCancelarVisible) {
		var containerGeral = MasterPage.getContent(EmpreendimentoTela.content);
		$('.btnEmpContainer', containerGeral).toggle(btnAvancarVisible || btnSalvarVisible || btnVoltarVisible || btnCancelarVisible);
		$('.btnEmpAvancar', containerGeral).toggle(btnAvancarVisible);
		$('.btnEmpSalvar', containerGeral).toggle(btnSalvarVisible);
		$('.btnEmpVoltar', containerGeral).toggle(btnVoltarVisible);
		$('.cancelarCaixa', containerGeral).toggle(btnCancelarVisible);
		$('.btnModalOu', containerGeral).toggle(btnEditarVisible || btnAvancarVisible || btnSalvarVisible);
	},

	limparMensagens: function (content) {
		Mensagem.limpar(MasterPage.getContent(content));
	}
}
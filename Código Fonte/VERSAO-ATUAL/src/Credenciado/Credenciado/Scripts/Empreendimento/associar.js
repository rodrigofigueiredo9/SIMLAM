/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="partial.js" />

EmpreendimentoAssociar = {
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
			pessoaAssociar: ''
		},
		onAssociarCallback: null,
		visualizando: false,
		editarVisualizar: true,
		msgs: {}
	},

	content: null,

	load: function (content, options) {
		EmpreendimentoAssociar.content = content;

		if (options) {
			$.extend(EmpreendimentoAssociar.settings, options);
		}

		Empreendimento.load($('.empreendimentoPartial', content), {
			onEditarEnter: EmpreendimentoAssociar.onEditarEnter,
			onCriarEnter: EmpreendimentoAssociar.onCriarEnter,
			onVisualizarEnter: EmpreendimentoAssociar.onVisualizarEnter,
			onVerificarEnter: EmpreendimentoAssociar.onVerificarEnter,
			urls: EmpreendimentoAssociar.settings.urls,
			msgs: EmpreendimentoAssociar.settings.msgs,
			denominadoresSegmentos: EmpreendimentoAssociar.settings.denominadoresSegmentos
		});

		$('.btnEmpAssociar', content).click(EmpreendimentoAssociar.onBtnAssociarClick);
		$('.btnEmpSalvar', content).click(EmpreendimentoAssociar.onBtnSalvarClick);
		$('.btnEmpEditar', content).click(EmpreendimentoAssociar.onBtnEditarClick);
		$('.linkCancelar', content).click(function () { Modal.fechar($(this)); });

		if (EmpreendimentoAssociar.settings.visualizando) {
			EmpreendimentoAssociar.onVisualizarEnter(true);
		} else {
			EmpreendimentoAssociar.onIdentificacaoEnter();
		}
	},

	associarAbrir: function (url, empAssociarLoadOptions) {
		$.extend(empAssociarLoadOptions, { modoInline: true });
		Modal.abrir(url, null, function (content) {
			EmpreendimentoAssociar.load(content, empAssociarLoadOptions);
		}, Modal.tamanhoModalGrande);
	},

	onSalvar: function (responseJson) {
		var modal = Modal.getModalContent(EmpreendimentoAssociar.content);
		var msgErro = EmpreendimentoAssociar.settings.onAssociarCallback(responseJson);
		if (typeof msgErro == 'object' && msgErro.length) {
			Mensagem.gerar(modal, msgErro);
		} else {
			Modal.fechar(modal);
		}
	},

	onEditarEnter: function () {// mostra botão cancelar e salvar
		EmpreendimentoAssociar.setBotoes(false, false, true, true, false, false);
	},

	onCriarEnter: function () {// mostra botão cancelar e salvar
		EmpreendimentoAssociar.setBotoes(false, false, true, true, false, false);
	},

	onVisualizarEnter: function (esconderAssociar) {// mostra botão cancelar e salvar
		if (esconderAssociar === true) {
			EmpreendimentoAssociar.setBotoes(EmpreendimentoAssociar.settings.editarVisualizar, false, false, true, false, false);
		} else {
			EmpreendimentoAssociar.setBotoes(true, true, false, true, false, false);
		}
	},

	onIdentificacaoEnter: function () {// mostra botão cancelar
		EmpreendimentoAssociar.setBotoes(false, false, false, true, false, false);
	},

	onBtnSalvarClick: function () {
		var retorno = Empreendimento.salvar(false);
		EmpreendimentoAssociar.onSalvar(retorno);
	},

	onBtnEditarClick: function () {
		Empreendimento.abrirEditar($('[name="Empreendimento.Id"]', EmpreendimentoAssociar.content).val());
		EmpreendimentoAssociar.setBotoes(false, false, true, true, false, false);
	},

	onBtnAssociarClick: function () {
		var objEmpreendimento = Modal.json(EmpreendimentoAssociar.content);

		if (objEmpreendimento.Id == "0") {
			var retorno = Empreendimento.salvar(false);
			objEmpreendimento.Id = retorno.Empreendimento.Id;
			EmpreendimentoAssociar.onSalvar(objEmpreendimento);
			return;
		}

		var erroMsg = EmpreendimentoAssociar.settings.onAssociarCallback(objEmpreendimento);

		if (typeof erroMsg == 'object' && erroMsg.length) {
			Mensagem.gerar($('.modalContent', EmpreendimentoAssociar.content), erroMsg);
		} else {
			Modal.fechar(EmpreendimentoAssociar.content);
		}
	},

	setBotoes: function (btnEditarVisible, btnAssociarVisible, btnSalvarVisible, btnCancelarVisible, btnAvancarVisible, btnVoltarVisible) {
		// mostra/esconde
		$('.btnEmpContainer', EmpreendimentoAssociar.content).toggleClass('hide', (btnEditarVisible || btnAssociarVisible || btnSalvarVisible || btnCancelarVisible || btnAvancarVisible || btnVoltarVisible));
		$('.btnEmpEditar', EmpreendimentoAssociar.content).toggleClass('hide', (btnEditarVisible));
		$('.btnEmpAssociar', EmpreendimentoAssociar.content).toggleClass('hide', (btnAssociarVisible));
		$('.btnEmpSalvar', EmpreendimentoAssociar.content).toggleClass('hide', (btnSalvarVisible));
		$('.cancelarCaixa', EmpreendimentoAssociar.content).toggleClass('hide', (btnCancelarVisible));
		$('.btnEmpAvancar', EmpreendimentoAssociar.content).toggleClass('hide', (btnAvancarVisible));
		$('.btnEmpVoltar', EmpreendimentoAssociar.content).toggleClass('hide', (btnVoltarVisible));
		$('.btnModalOu', EmpreendimentoAssociar.content).toggleClass('hide', (btnEditarVisible || btnAssociarVisible || btnSalvarVisible));
	}
}
/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="partial.js" />

EmpreendimentoInline = {
	settings: {
		urls: {
			avancar: '',
			voltar: '',
			salvarCadastrar: '',
			editar: '',
			verificarCnpj: '',
			visualizarEmpreendimento: '',
			associarResponsavelModal: '',
			associarResponsavelEditarModal: '',
			associarAtividadeModal: '',
			enderecoMunicipio: '',
			pessoaAssociar: '',
			obterEnderecoResponsavel: '',
			obterListaPessoasAssociada: '',
			verificarLocalizaoEmpreendimento: ''
		},
		onIdentificacaoEnter: null,
		onVisualizarEnter: null,
		onEditarEnter: null,
		onNovoEnter: null,
		msgs: {},
		idsTela: null,
		obterEnderecoPessoaAssociada: null
	},

	content: null,
	modo: 1,
	isVoltarIdentificacao: false,

	load: function (content, options) {
		EmpreendimentoInline.content = $('.empreendimentoPartial', content);

		if (options) {
			$.extend(EmpreendimentoInline.settings, options);
		}

		Empreendimento.load(EmpreendimentoInline.content, {
			modoInline: true,
			callBackVisualizar: EmpreendimentoInline.onVisualizarEnter,
			urls: EmpreendimentoInline.settings.urls,
			msgs: EmpreendimentoInline.settings.msgs,
			idsTela: EmpreendimentoInline.settings.idsTela,
			denominadoresSegmentos: EmpreendimentoInline.settings.denominadoresSegmentos,
			obterEnderecoPessoaAssociada: EmpreendimentoInline.settings.obterEnderecoPessoaAssociada
		});

		EmpreendimentoInline.configurarTela();
	},

	configurarTela: function () {
		MasterPage.botoes(EmpreendimentoInline.content);
		Mascara.load(EmpreendimentoInline.content);
		$('.titTela', EmpreendimentoInline.content).remove();
	},

	onVoltarEnter: function () {
		EmpreendimentoInline.modo = 1;
		Empreendimento.voltar();
		EmpreendimentoInline.isVoltarIdentificacao = false;
		EmpreendimentoInline.settings.onIdentificacaoEnter();
		$('.titTela', EmpreendimentoInline.content).remove();
	},

	onAvancarEnter: function () {
		EmpreendimentoInline.modo = 1;
		Empreendimento.avancar();
		EmpreendimentoInline.isVoltarIdentificacao = true;
		EmpreendimentoInline.onEditarEnter();
		EmpreendimentoInline.settings.onNovoEnter();
		$('.titTela', EmpreendimentoInline.content).remove();
	},

	onEditarEnter: function () {
		EmpreendimentoInline.modo = 1;
		EmpreendimentoInline.settings.onEditarEnter();
		$('.titTela', EmpreendimentoInline.content).remove();
	},

	onVisualizarEnter: function () {
		EmpreendimentoInline.modo = 2;
		EmpreendimentoInline.settings.onVisualizarEnter();
		$('.titTela', EmpreendimentoInline.content).remove();
	},

	onSalvarClick: function () {

		var retorno = null;

		if (EmpreendimentoInline.modo === 1) {
			retorno = Empreendimento.salvar(false);
			if (retorno == null) {
				return '0';
			} else {
				return retorno.Id;
			}
		} else {
			return Empreendimento.obterIdEmpreendimento();
		}
	},

	onBtnEditarClick: function () {
		Empreendimento.abrirEditar();
		EmpreendimentoInline.onEditarEnter();
	},

	onBtnVisualizarClick: function (empreendimentoId) {
		Empreendimento.abrirVisualizar(empreendimentoId);
	},

	obterIdEmpreendimento: function () {
		return Empreendimento.obterIdEmpreendimento();
	},

	limparCamposFiltro: function () {
		EmpreendimentoLocalizar.limparCamposFiltro();
	}
}
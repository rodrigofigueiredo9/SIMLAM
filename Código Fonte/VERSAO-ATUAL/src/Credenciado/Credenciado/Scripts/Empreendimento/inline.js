/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
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
			copiarInterno:''
		},
		onIdentificacaoEnter: null,
		onVisualizarEnter: null,
		onEditarEnter: null,
		onNovoEnter: null,
		msgs: {}
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
			denominadoresSegmentos: EmpreendimentoInline.settings.denominadoresSegmentos
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

	onSalvarClick: function (requerimento = 0) {
		var objEmpreendimento = Empreendimento.obterEmpreendimentoIds();
		var retorno = null;

		requerimento = (typeof (requerimento) === "number" && requerimento > 0) ? requerimento : 0;

		switch (EmpreendimentoInline.modo) {
			case 1:
				retorno = Empreendimento.salvar(false, requerimento);
				if (retorno != null) {
					objEmpreendimento.id = retorno.Id;
				} else {
					return 0;
				}

				break;

			case 2:
				if (!objEmpreendimento.id) {
					retorno = Empreendimento.salvar(false, requerimento );
					if (retorno != null) {
						objEmpreendimento.id = retorno.Id;
					}
				}
				break;

			default:
				objEmpreendimento.id = -1;
				break;
		}

		return objEmpreendimento.id;
	},

	onBtnEditarClick: function () {
		Empreendimento.abrirEditar();
		EmpreendimentoInline.onEditarEnter();
	},

	onBtnVisualizarClick: function (objParam) {
	    Empreendimento.abrirVisualizar(objParam);
	},

	obterEmpreendimentoIds: function () {
		return Empreendimento.obterEmpreendimentoIds();
	},

	limparCamposFiltro: function () {
		EmpreendimentoLocalizar.limparCamposFiltro();
	}
}
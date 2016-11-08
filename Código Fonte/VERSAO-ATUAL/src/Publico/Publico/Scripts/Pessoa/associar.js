﻿/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="partial.js" />

PessoaAssociar = function () {

	var _objRef = null;

	return {
		settings: {
			urls: {
				modalAssociarProfissao: '',
				modalAssociarRepresentante: '',
				verificar: '',
				limpar: '',
				visualizar: '',
				criar: '',
				editar: '',
				salvarConjuge: '',
				pessoaModalVisualizarConjuge: ''
			},
			tituloVerificar: 'Verificar CPF/CNPJ',
			tituloCriar: 'Cadastrar Pessoa',
			tituloEditar: 'Editar Pessoa',
			tituloVisualizar: 'Visualizar Pessoa',
			onAssociarCallback: null,
			associarConjuge: false,
			visualizando: false,
			editarVisualizar: true,
			tipoCadastro: 0,
			isConjuge: false,
			msgs: {
				RepresentanteExistente: null
			}
		},

		content: null,
		pessoaObj: null,

		load: function (content, options) {
			_objRef = this;
			_objRef.content = content;

			if (PessoaAssociar.settings) {
				$.extend(_objRef.settings, PessoaAssociar.settings);
			}

			if (options) {
				var urls = $.extend(_objRef.settings.urls, options.urls);
				var msgs = $.extend(_objRef.settings.msgs, options.msgs);
				$.extend(_objRef.settings, options);
				_objRef.settings.urls = urls;
				_objRef.settings.msgs = msgs;
			}

			_objRef.pessoaObj = new Pessoa();

			_objRef.pessoaObj.load($('.pessoaPartial', content), {
				modoAssociar: true,
				onSalvar: _objRef.onSalvar,
				onEditarEnter: _objRef.onEditarEnter,
				onCriarEnter: _objRef.onCriarEnter,
				onVisualizarEnter: _objRef.onVisualizarEnter,
				onVerificarEnter: _objRef.onVerificarEnter,
				tipoCadastro: _objRef.settings.tipoCadastro,
				associarConjuge: _objRef.settings.associarConjuge,
				isConjuge: _objRef.settings.isConjuge,
				msgs: _objRef.settings.msgs,
				urls: $.extend({}, options.urls, _objRef.settings.urls)
			});

			$('.btnPessoaAssociar', content).click(_objRef.onBtnAssociarClick);
			$('.btnPessoaSalvar', content).click(_objRef.onBtnSalvarClick);
			$('.btnPessoaEditar', content).click(_objRef.onBtnEditarClick);
			$('.linkCancelar', content).click(function () { Modal.fechar($(this)); });
			$('.inputCpfPessoa', content).focus();

			if (_objRef.settings.visualizando) {
				_objRef.onVisualizarEnter({esconderAssociar: true}); //esconder o botao de associar
			} else {
				_objRef.onVerificarEnter();
			}
			Modal.customButtons(_objRef.content, $('.divPessoaContainer', _objRef.content).removeClass('.divPessoaContainer'));
		},

		associarAbrir: function (url, pessoaAssociarLoadOptions, data) {
			$.extend(pessoaAssociarLoadOptions, { modoAssociar: true });

			var pessoaAssociarObj = this;

			Modal.abrir(url, data, function (content) {
				pessoaAssociarObj.load(content, pessoaAssociarLoadOptions);
			}, Modal.tamanhoModalGrande);
		},

		// exemplo de responseJson: { IsPessoaSalva = Validacao.EhValido, UrlRedireciona = urlRedireciona, @Pessoa = vm.Pessoa, Msg = Validacao.Erros }
		// deve chamar o callback que foi passado para _objRef.load. Este se enconta em _objRef.settings
		onSalvar: function (partialContent, responseJson, isEditar) {
			var modal = Modal.getModalContent(_objRef.content);
			var msgErro = _objRef.settings.onAssociarCallback(responseJson.Pessoa);
			if (typeof msgErro == 'object' && msgErro.length) {
				Mensagem.gerar(modal, msgErro);
			} else {
				Modal.fechar(modal);
			}
		},

		onEditarEnter: function () {	// mostra botão cancelar e salvar
			_objRef.setBotoes(false, false, true, true);
			_objRef.setTitulo(_objRef.settings.tituloEditar);
		},

		onCriarEnter: function () {		// mostra botão cancelar e salvar
			_objRef.setBotoes(false, false, true, true);
			_objRef.setTitulo(_objRef.settings.tituloCriar);
		},

		onVisualizarEnter: function (params) {
			_objRef.setBotoes(_objRef.settings.editarVisualizar, !params.esconderAssociar, false, true);
			_objRef.setTitulo(_objRef.settings.tituloVisualizar);
		},

		onVerificarEnter: function () {		// mostra botão cancelar
			_objRef.setBotoes(false, false, false, true);
			_objRef.setTitulo(_objRef.settings.tituloVerificar);
		},

		onBtnSalvarClick: function () {
			_objRef.pessoaObj.salvar();
		},

		onBtnEditarClick: function () {
			_objRef.pessoaObj.editar($('[name="Pessoa.Id"]', _objRef.content).val());
		},

		onBtnAssociarClick: function () {
			var objPessoa = _objRef.pessoaObj.obter();
			var erroMsg = _objRef.settings.onAssociarCallback(objPessoa);
			if (typeof erroMsg == 'object' && erroMsg.length) {
				Mensagem.gerar($('.modalContent', _objRef.content), erroMsg);
			} else {
				Modal.fechar(_objRef.content);
			}
		},

		setTitulo: function (titulo) {
			$('.titTela', _objRef.content).text(titulo);
		},

		setBotoes: function (btnEditarVisible, btnAssociarVisible, btnSalvarVisible, btnCancelarVisible) {
			// mostra/esconde
			$('.divPessoaContainer', _objRef.content).toggle(btnEditarVisible || btnAssociarVisible || btnSalvarVisible || btnCancelarVisible);
			$('.btnPessoaEditar', _objRef.content).toggle(btnEditarVisible);
			$('.btnPessoaAssociar', _objRef.content).toggle(btnAssociarVisible);
			$('.btnPessoaSalvar', _objRef.content).toggle(btnSalvarVisible);
			$('.cancelarCaixa', _objRef.content).toggle(btnCancelarVisible);
			$('.btnModalOu', _objRef.content).toggle(btnEditarVisible || btnAssociarVisible || btnSalvarVisible);
		}
	};
};
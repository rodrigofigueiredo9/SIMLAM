/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
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
				editar: ''
			},
			tituloVerificar: 'Verificar CPF/CNPJ',
			tituloCriar: 'Cadastrar Pessoa',
			tituloEditar: 'Editar Pessoa',
			tituloVisualizar: 'Visualizar Pessoa',
			onAssociarCallback: null,
			visualizando: false,
			editarVisualizar: false,
			tipoCadastro: 0,
			isCopiado: false,
			isConjuge: false,
			possuiConjuge: false,
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
				isCopiado: _objRef.settings.isCopiado,
				isConjuge: _objRef.settings.isConjuge,
				possuiConjuge: _objRef.settings.possuiConjuge,
				editarVisualizar: _objRef.settings.editarVisualizar,
				msgs: _objRef.settings.msgs,
				urls: $.extend({}, options.urls, _objRef.settings.urls)
			});

			$('.btnPessoaAssociar', content).click(_objRef.onBtnAssociarClick);
			$('.btnPessoaSalvar', content).click(_objRef.onBtnSalvarClick);
			$('.btnPessoaEditar', content).click(_objRef.onBtnEditarClick);
			$('.linkCancelar', content).click(function () { Modal.fechar($(this)); });
			$('.inputCpfPessoa', content).focus();

			if (_objRef.settings.visualizando) {
				_objRef.onVisualizarEnter({ esconderAssociar: true });
				if (!(window.RequerimentoVis === undefined)) {
					$('.btnPessoaEditar, .btnModalOu', _objRef.content).remove();
				}

			} else {
				_objRef.onVerificarEnter();
			}

			Modal.customButtons(_objRef.content, $('.divPessoaContainer', _objRef.content).removeClass('.divPessoaContainer'));
		},

		associarAbrir: function (url, pessoaAssociarLoadOptions, data) {
			$.extend(pessoaAssociarLoadOptions, { modoAssociar: true });

			var pessoaAssociarObj = this;
			if (!data) {
				data = null;
			}

			Modal.abrir(url, data, function (content) {
				pessoaAssociarObj.load(content, pessoaAssociarLoadOptions);
			}, Modal.tamanhoModalGrande);
		},

		onSalvar: function (partialContent, responseJson, isEditar) {
			var modal = Modal.getModalContent(_objRef.content);
			var msgErro = _objRef.settings.onAssociarCallback(responseJson.Pessoa);

			if (typeof msgErro == 'object' && msgErro.length) {
				Mensagem.gerar(modal, msgErro);
			} else {

				Modal.fechar(modal);
			}
		},

		onEditarEnter: function () {
			_objRef.setBotoes(false, false, true, true);
			_objRef.setTitulo(_objRef.settings.tituloEditar);
		},

		onCriarEnter: function () {
			_objRef.setBotoes(false, false, true, true);
			_objRef.setTitulo(_objRef.settings.tituloCriar);
		},

		onVisualizarEnter: function (params) {
			if (params.mostrarEditar) {
				_objRef.settings.editarVisualizar = params.mostrarEditar;
			}

			if (typeof RequerimentoVis != 'undefined') {
				_objRef.settings.editarVisualizar = RequerimentoVis.mostrarBtnEditar;
			}

			_objRef.setBotoes(_objRef.settings.editarVisualizar, !params.esconderAssociar, false, true);
			_objRef.setTitulo(_objRef.settings.tituloVisualizar);
		},

		onVerificarEnter: function () {
			_objRef.setBotoes(false, false, false, true);
			_objRef.setTitulo(_objRef.settings.tituloVerificar);
		},

		onBtnSalvarClick: function () {
			_objRef.pessoaObj.salvar();
		},

		onBtnEditarClick: function () {
			_objRef.pessoaObj.editar(_objRef.pessoaObj.obterIdPessoa());
		},

		onBtnAssociarClick: function () {
			var objPessoa = Modal.json(_objRef.content);

			if (objPessoa.Id == "0") {
				var retorno = _objRef.pessoaObj.salvar();
				return;
			}

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
			$('.divPessoaContainer', _objRef.content).toggleClass('hide', !(btnEditarVisible || btnAssociarVisible || btnSalvarVisible || btnCancelarVisible));
			$('.btnPessoaEditar', _objRef.content).toggleClass('hide', !(btnEditarVisible));
			$('.btnPessoaAssociar', _objRef.content).toggleClass('hide', !(btnAssociarVisible));
			$('.btnPessoaSalvar', _objRef.content).toggleClass('hide', !(btnSalvarVisible));
			$('.cancelarCaixa', _objRef.content).toggleClass('hide', !(btnCancelarVisible));
			$('.btnModalOu', _objRef.content).toggleClass('hide', !(btnEditarVisible || btnAssociarVisible || btnSalvarVisible));
		}
	};
};
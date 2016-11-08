/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="partial.js" />

PessoaInline = function () {

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
				pessoaModal: '',
				pessoaModalVisualizar: '',
				obterEnderecoPessoa: ''
			},
			onSalvar: null,
			onVisualizarEnter: null,
			onVerificarEnter: null,
			onCriarEnter: null,

			msgs: {
				RepresentanteExistente: null
			}
		},

		content: null,
		modo: 3, //Modo verificar
		pessoaObj: new Pessoa(),

		load: function (content, options) {

			_objRef = this;

			_objRef.content = $('.pessoaPartial', content);

			if (PessoaInline.settings) {
				$.extend(_objRef.settings, PessoaInline.settings);
			}

			if (options) {
				$.extend(_objRef.settings, options);
			}

			_objRef.pessoaObj.load(_objRef.content, {
				modoAssociar: true,
				modoInline: true,

				onSalvar: _objRef.settings.onSalvar,
				onEditarEnter: _objRef.onEditarEnter,
				onCriarEnter: _objRef.onCriarEnter,
				onVisualizarEnter: _objRef.onVisualizarEnter,
				onVerificarEnter: _objRef.onVerificarEnter,

				urls: {
					modalAssociarProfissao: _objRef.settings.urls.modalAssociarProfissao,
					modalAssociarRepresentante: _objRef.settings.urls.modalAssociarRepresentante,
					verificar: _objRef.settings.urls.verificar,
					limpar: _objRef.settings.urls.limpar,
					criar: _objRef.settings.urls.criar,
					visualizar: _objRef.settings.urls.visualizar,
					editar: _objRef.settings.urls.editar,
					pessoaModal: _objRef.settings.urls.pessoaModal,
					pessoaModalVisualizar: _objRef.settings.urls.pessoaModalVisualizar,
					obterEnderecoPessoa: _objRef.settings.urls.obterEnderecoPessoa
				},
				msgs: {
					RepresentanteExistente: _objRef.settings.msgs.RepresentanteExistente
				}
			});
		},

		onEditarEnter: function () {
			_objRef.modo = 1;
			_objRef.settings.onEditarEnter();
		},

		onCriarEnter: function () {
			_objRef.modo = 1;
			_objRef.settings.onCriarEnter();
		},

		onVisualizarEnter: function () {
			_objRef.modo = 2;
			_objRef.settings.onVisualizarEnter();
		},

		onVerificarEnter: function () {
			_objRef.modo = 3;
			_objRef.settings.onVerificarEnter();
		},

		onSalvarClick: function () {

			var retorno = null;

			if (_objRef.modo === 1) {
				retorno = _objRef.pessoaObj.salvar();
				if (retorno == null) {
					return '0';
				} else {
					return retorno.Pessoa.Id;
				}
			}

			if (_objRef.modo === 2) {
				return _objRef.pessoaObj.obterIdPessoa();
			}

			if (_objRef.modo === 3) {
				return -1;
			}
		},

		onBtnEditarClick: function () {
			_objRef.pessoaObj.editar(_objRef.pessoaObj.obterIdPessoa());
		},

		onBtnLimparClick: function () {
			_objRef.pessoaObj.onLimparClick();
		},

		onVerificarClick: function () {
			_objRef.pessoaObj.onVerificarClick();
		}
	};
};
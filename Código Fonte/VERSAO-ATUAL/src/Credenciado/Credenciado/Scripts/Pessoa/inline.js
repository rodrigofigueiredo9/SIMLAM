/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
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
				copiarIdaf: ''
			},
			onSalvar: null,
			onVisualizarEnter: null,
			onVerificarEnter: null,
			onCriarEnter: null,
			msgs: {
				RepresentanteExistente: null
			},
			editarVisualizar: false
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
				editarVisualizar: _objRef.settings.editarVisualizar,
				onSalvar: _objRef.settings.onSalvar,
				onEditarEnter: _objRef.onEditarEnter,
				onCriarEnter: _objRef.onCriarEnter,
				onVisualizarEnter: _objRef.onVisualizarEnter,
				onVerificarEnter: _objRef.onVerificarEnter,
				msgs: _objRef.settings.msgs,
				urls: {
					modalAssociarProfissao: _objRef.settings.urls.modalAssociarProfissao,
					modalAssociarRepresentante: _objRef.settings.urls.modalAssociarRepresentante,
					verificar: _objRef.settings.urls.verificar,
					limpar: _objRef.settings.urls.limpar,
					criar: _objRef.settings.urls.criar,
					visualizar: _objRef.settings.urls.visualizar,
					editar: _objRef.settings.urls.editar,
					pessoaModal: _objRef.settings.urls.pessoaModal,
					pessoaModalVisualizar: _objRef.settings.urls.pessoaModalVisualizar
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
			var objPessoa = _objRef.pessoaObj.obterIdPessoa();
			var retorno = null;

			switch (_objRef.modo) {
				case 1:
					retorno = _objRef.pessoaObj.salvar();
					if (retorno != null) {
						objPessoa.id = retorno.Pessoa.Id;
					} else {
						return 0;
					}
					break;

				case 2:
					if (objPessoa.id == '0') {
						retorno = _objRef.pessoaObj.salvar();
						if (retorno != null) {
							objPessoa.id = retorno.Pessoa.Id;
						}
					}
					break;

				default:
					objPessoa.id = -1;
					break;
			}
			 
			return objPessoa.id;
		},

		onBtnEditarClick: function () {
			_objRef.pessoaObj.editar(_objRef.pessoaObj.obterIdPessoa());
		},

		onBtnLimparClick: function () {
			_objRef.pessoaObj.onLimparClick();
		},

		onVerificarClick: function (requerimento = 0) {
			
			_objRef.pessoaObj.onVerificarClick(requerimento);
		}
	};
};
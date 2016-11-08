/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="partial.js" />

var PessoaTela = function () {
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
			tituloVerificar: 'Verificar Pessoa',
			tituloCriar: 'Cadastrar Pessoa',
			tituloEditar: 'Editar Pessoa',
			tituloVisualizar: 'Visualizar Pessoa',
			msgs: {
				RepresentanteExistente: null
			},
			editarVisualizar: false
		},
		content: null,
		pessoaObj: null,

		load: function (content, options) {
			_objRef = this;
			_objRef.content = content;

			if (options) {
				$.extend(_objRef.settings, options);
			}

			pessoaObj = new Pessoa();

			pessoaObj.load($('.pessoaPartial', content), {
				modoAssociar: false,
				onSalvar: _objRef.onSalvar,
				onVisualizarEnter: _objRef.onVisualizarEnter,
				onEditarEnter: _objRef.onEditarEnter,
				onCriarEnter: _objRef.onCriarEnter,
				onVerificarEnter: _objRef.onVerificarEnter,
				urls: _objRef.settings.urls,
				msgs: _objRef.settings.msgs,
				editarVisualizar: _objRef.settings.editarVisualizar,
				conjugeEditarVisualizar: _objRef.settings.editarVisualizar
			});

			$('.btnPessoaSalvar', content).click(_objRef.onBtnSalvarClick);
			$('.btnPessoaEditar', content).click(_objRef.onBtnEditarClick);
			$('.inputCpfPessoa', content).focus();
		},

		onVerificarEnter: function () {
			_objRef.setBotoes(false, false, true);
			_objRef.setTitulo(_objRef.settings.tituloVerificar);
		},

		// exemplo de responseJson: { IsPessoaSalva = Validacao.EhValido, UrlRedireciona = urlRedireciona, @Pessoa = vm.Pessoa, Msg = Validacao.Erros }
		onSalvar: function (partialContent, responseJson, isEditar) {
			MasterPage.redireciona(responseJson.UrlRedireciona);
		},

		onEditarEnter: function () {
			_objRef.setBotoes(true, false, true);
			_objRef.setTitulo(_objRef.settings.tituloEditar);
		},

		onCriarEnter: function () {
			_objRef.setBotoes(true, false, true);
			_objRef.setTitulo(_objRef.settings.tituloCriar);
		},

		onVisualizarEnter: function () {
			_objRef.setBotoes(false, true, true);
			_objRef.setTitulo(_objRef.settings.tituloVisualizar);
		},

		onBtnSalvarClick: function (_this, onCall) {
			MasterPage.carregando(true);
			pessoaObj.salvar();
			MasterPage.carregando(false);
		},

		onBtnEditarClick: function (_this, onCall) {
			MasterPage.carregando(true);
			pessoaObj.editar(pessoaObj.obterIdPessoa());
			MasterPage.carregando(false);
		},

		setTitulo: function (titulo) {
			$('.titTela', _objRef.content).text(titulo);
		},

		setBotoes: function (btnSalvarVisible, btnEditarVisible, btnCancelarVisible) {
			var containerGeral = MasterPage.getContent(_objRef.content);
			
			$('.divPessoaContainer', containerGeral).toggleClass('hide', !(btnSalvarVisible || btnCancelarVisible));
			$('.btnPessoaSalvar', containerGeral).toggleClass('hide', !(btnSalvarVisible));
			$('.btnPessoaEditar', containerGeral).toggleClass('hide', !(btnEditarVisible));
			$('.cancelarCaixa', containerGeral).toggleClass('hide', !(btnCancelarVisible));
			$('.btnModalOu', containerGeral).toggleClass('hide', !((btnSalvarVisible || btnEditarVisible) && btnCancelarVisible));
		}
	};
};
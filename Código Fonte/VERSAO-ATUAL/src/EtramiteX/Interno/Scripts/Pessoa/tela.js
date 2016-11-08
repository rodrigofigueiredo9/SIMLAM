/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
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
			}
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
				msgs: {
					RepresentanteExistente: _objRef.settings.msgs.RepresentanteExistente
				}
			});

			$('.btnPessoaSalvar', content).click(_objRef.onBtnSalvarClick);
			$('.btnPessoaEditar', content).click(_objRef.onBtnEditarClick);
			$('.inputCpfPessoa', content).focus();
		},

		onVerificarEnter: function () {		// mostra botão cancelar
			_objRef.setBotoes(false, false, true);
			_objRef.setTitulo(_objRef.settings.tituloVerificar);
		},

		// exemplo de responseJson: { IsPessoaSalva = Validacao.EhValido, UrlRedireciona = urlRedireciona, @Pessoa = vm.Pessoa, Msg = Validacao.Erros }
		onSalvar: function (partialContent, responseJson, isEditar) {
			MasterPage.redireciona(responseJson.UrlRedireciona);
		},

		// mostra botão cancelar e salvar
		onEditarEnter: function () {
			_objRef.setBotoes(true, false, true);
			_objRef.setTitulo(_objRef.settings.tituloEditar);
		},

		// mostra botão cancelar e salvar
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
			$('.divPessoaContainer, .btnPessoaSalvar, .cancelarCaixa, btnModalOu.', containerGeral).removeClass('hide');
			$('.divPessoaContainer', containerGeral).toggle(btnSalvarVisible || btnCancelarVisible);
			$('.btnPessoaSalvar', containerGeral).toggle(btnSalvarVisible);
			$('.btnPessoaEditar', containerGeral).toggle(btnEditarVisible);
			$('.cancelarCaixa', containerGeral).toggle(btnCancelarVisible);
			$('.btnModalOu', containerGeral).toggle((btnSalvarVisible || btnEditarVisible) && btnCancelarVisible);
		}
	};
};
/// <reference path="Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../Lib/jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="profissao.js" />

var Pessoa = function () {
	var _objRef = null;

	return {
		settings: {
			modoAssociar: true,
			modoInline: false,
			representanteModalLoadFunction: 'RepresentanteAssociar.load',
			profissaoModalLoadFunction: 'ProfissaoAssociar.load',
			onVisualizarEnter: null, // quando entra na tela de visualizar
			onVerificarEnter: null,
			onEditarEnter: null, // quando entra na tela de editar
			onCriarEnter: null, // quando entra na tela de criar
			onSalvar: null, // quando termina de salvar uma pessoa
			onAssociarCallback: null, // usado por associar-multiplo. todos os modais que queiram funcionar com associar-multiplo devem declarar esta opção
			textoDefaultProfissao: '*** Associar uma profissão ***',
			tipoCadastro: 0,
			isCopiado: false,
			isConjuge: false,
			possuiConjuge: false,
			editarVisualizar: false,
			conjugeEditarVisualizar: false,
			urls: {
				modalAssociarProfissao: '',
				modalAssociarRepresentante: '',
				verificar: '',
				limpar: '',
				visualizar: '',
				criar: '',
				editar: '',
				pessoaModal: '',
				pessoaModalVisualizar: ''
			},
			msgs: {
				RepresentanteExistente: null,
				PessoaConjugeSaoIguais: null
			}
		},

		content: null,
		conjugeModal: null,
		representanteModal: null,

		load: function (content, options, setDefaults) {
			_objRef = this;

			_objRef.content = content;
			if (typeof setDefaults == 'undefined') setDefaults = true;

			if (setDefaults) {
				_objRef.settings.onVerificar = _objRef.onVerificar; // comportamento padrão onVerificar: ir para visualizar
			}

			if (options) {
				$.extend(_objRef.settings, options);
			}

			_objRef.settings.isCopiado = eval(_objRef.settings.isCopiado.toString().toLowerCase());
			_objRef.settings.isConjuge = eval(_objRef.settings.isConjuge.toString().toLowerCase());

			$('.rdbPessaoTipo', content).change(_objRef.onPessoaTipoChange);
			$('.CpfPessoaContainer,.CnpjPessoaContainer', content).keyup(_objRef.onVerficarCpfCnpjKeyUp);
			$('.btnAssociarReprensetante', content).click(_objRef.onAssociarRepresentanteClick);
			$('.btnAssociarProfissao', content).click(_objRef.onAssociarProfissaoClick);
			$('.btnLimparProfissao', content).click(_objRef.onBtnLimparProfissaoClick);
			$('.ddlEstado', content).change(Aux.onEnderecoEstadoChange);
			$('.btnLimparCpfCnpj', content).click(_objRef.onLimparClick);
			$('.btnVerificarCpfCnpj', content).click(_objRef.onVerificarClick);

			$('.ddlEstadoCivil', content).change(_objRef.onChangeEstadoCivil);
			$('.btnLimparConjuge', content).click(_objRef.onLimparConjuge);
			$('.btnAssociarConjuge', content).click(_objRef.onAssociarConjuge);
			$('.ckbAlterarSenha', content).click(_objRef.onAlterarSenhaClick);

			if (_objRef.settings.modoInline) {
				$('.btnLimparCpfCnpj', content).remove();
				_objRef.settings.isCopiado = $('.hdnIsCopiado', _objRef.content).val() == 'true';
			} else {
				$('.hdnIsCopiado:not(.tabRepresentantes .hdnIsCopiado)', _objRef.content).val(_objRef.settings.isCopiado);
			}

			$(content).delegate('.btnExcluirRepresentante', 'click', _objRef.onExcluirRepresentante);
			$(content).delegate('.btnVisualizarRepresentante', 'click', _objRef.onBtnVisualizarRepresentanteClick);
			$(content).delegate('.btnCopiarInterno', 'click', _objRef.onCopiarIdaf);
			$(content).delegate('.btnVisualizarConjuge', 'click', _objRef.onVisualizarConjuge);

			$(content).find(':text:enabled:first').focus();

			if ($('.hdnOcultarIsCopiado').length > 0) {
				$('.btnCopiarInterno', _objRef.content).remove();
			}

			if (_objRef.settings.possuiConjuge == true) {
				$('.divConjuge', _objRef.content).remove();
			}

			Listar.atualizarEstiloTable(content);
		},

		onVisualizarConjuge: function () {
			var params = { id: $('.hdnConjugeId', _objRef.content).val() || 0, internoId: $('.hdnConjugeInternoId', _objRef.content).val(), isConjuge: true };

			_objRef.conjugeModal = new PessoaAssociar();
			_objRef.conjugeModal.settings.isCopiado = (params.id == 0);
			_objRef.conjugeModal.settings.isConjuge = true;
			_objRef.conjugeModal.settings.editarVisualizar = _objRef.settings.conjugeEditarVisualizar;

			Modal.abrir(_objRef.settings.urls.pessoaModalVisualizar,
				params,
				function (container) {
					_objRef.conjugeModal.load(container, {
						tituloCriar: 'Criar Pessoa',
						tituloEditar: 'Editar Pessoa',
						tituloVisualizar: 'Visualizar Pessoa',
						onAssociarCallback: function (pessoa) {

							$('.hdnIsCopiado', _objRef.content).val(false);
							$('.hdnConjugeId', _objRef.content).val(pessoa.Id);
							$('.hdnConjugeInternoId', _objRef.content).val(pessoa.InternoId);
							$('.txtConjugeNome', _objRef.content).val(pessoa.Fisica.Nome);
							$('.txtConjugeCPF', _objRef.content).val(pessoa.Fisica.CPF);
							_objRef.settings.isCopiado = false;
						}
					});
				}, Modal.tamanhoModalGrande);
		},

		onCopiarIdaf: function (content) {
			Modal.confirma({
				btnOkLabel: 'Confirmar',
				titulo: 'Copiar?',
				conteudo: 'Ao confirmar a cópia de dados do IDAF, todos os campos da tela serão substituídos pelos dados que estão cadastrados na base de dados do IDAF. Tem certeza que deseja efetuar a cópia?',
				tamanhoModal: Modal.tamanhoModalMedia,
				btnOkCallback: function (content) {
					_objRef.copiarIdaf();
					Modal.fechar(content);
					Mascara.load(_objRef.content);

					var isConjuge = ((Number($('hdnConjugeId', _objRef.content).val()) || 0) > 0) || ((Number($('hdnConjugeInternoId', _objRef.content).val()) || 0) > 0);

					if (isConjuge) {
						$('.btnVisualizarConjuge', _objRef.content).removeClass('hide');
					}
					_objRef.settings.isCopiado = true;
					$('.hdnIsCopiado', _objRef.content).val(true);
				}
			});
		},

		copiarIdaf: function () {

			var content = $('.pessoaPartial', MasterPage.getContent(_objRef.content));
			var params = { id: $('.pessoaId', content).val(), internoId: $('.internoId', content).val() };

			MasterPage.carregando(true);

			$.ajax({
				url: PessoaInline.settings.urls.copiarIdaf, data: params, cache: false, async: false,
				error: function (XMLHttpRequest, textStatus, erroThrown) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
				},
				success: function (response, textStatus, XMLHttpRequest) {

					if (response.Msg && response.Msg.length > 0 && !response.EhValido) {
						Mensagem.gerar(MasterPage.getContent(objRef.content), response.Msg);
						return;
					}

					content.empty();
					content.append(response);

					var pai = _objRef.content.parent();
					_objRef.content.replaceWith(response);

					var novoContent = $('.pessoaPartial', pai);
					_objRef.load(novoContent, _objRef.settings, false);

					MasterPage.botoes(novoContent);
					Mascara.load(content);
					Listar.atualizarEstiloTable(content);
				}
			});
			MasterPage.carregando(false);
		},

		onChangeEstadoCivil: function () {

			$('.divConjuge', _objRef.content).removeClass('hide');

			if ($(this).val() != 2 && $(this).val() != 5) {
				_objRef.onLimparConjuge();
				$('.divConjuge', _objRef.content).addClass('hide');
			}
		},

		onAssociarConjuge: function () {
			_objRef.conjugeModal = new PessoaAssociar();
			_objRef.conjugeModal.settings.possuiConjuge = true;
			_objRef.conjugeModal.settings.isConjuge = true;
			_objRef.conjugeModal.settings.editarVisualizar = _objRef.settings.editarVisualizar;

			Modal.abrir(_objRef.settings.urls.pessoaModal, { tipoCadastro: 1 }, function (container) {
				Modal.defaultButtons(container);
				_objRef.conjugeModal.load(container, { urls: {}, msgs: {}, onAssociarCallback: _objRef.callBackAssociarConjuge });

			}, Modal.tamanhoModalGrande, 'Pessoas');
		},

		onLimparConjuge: function () {
			$('.txtConjugeCPF', _objRef.content).val('');
			$('.hdnConjugeCPF', _objRef.content).val('');

			$('.txtConjugeNome', _objRef.content).val('');
			$('.txtConjugeNome', _objRef.content).removeClass('erroCampo');

			$('.hdnConjugeId', _objRef.content).val(0);
			$('.hdnConjugeInternoId', _objRef.content).val(0);

			$('.btnLimparConjuge', _objRef.content).addClass('hide');
			$('.btnAssociarConjuge', _objRef.content).removeClass('hide');
			$('.btnVisualizarConjuge', _objRef.content).closest('span').addClass('hide');
		},

		callBackAssociarConjuge: function (pessoaAssociada) {

			if ($('.pessoaId', _objRef.content).val() == pessoaAssociada.Id) {
				return new Array(_objRef.settings.msgs.PessoaConjugeSaoIguais);
			}

			$('.hdnConjugeCPF', _objRef.content).val(pessoaAssociada.CPFCNPJ);
			$('.txtConjugeCPF', _objRef.content).val(pessoaAssociada.CPFCNPJ);
			$('.txtConjugeNome', _objRef.content).val(pessoaAssociada.NomeRazaoSocial);
			$('.hdnConjugeId', _objRef.content).val(pessoaAssociada.Id);
			$('.hdnConjugeInternoId', _objRef.content).val(pessoaAssociada.InternoId);

			$('.btnVisualizarConjuge', _objRef.content).closest('span').removeClass('hide');
			$('.btnLimparConjuge', _objRef.content).removeClass('hide');
			$('.btnAssociarConjuge', _objRef.content).addClass('hide');

			Mensagem.limpar();

			return true;
		},

		onPessoaTipoChange: function () {
			if ($('input.rdbPessaoTipo:checked', _objRef.content).val() == '1') { // física
				$('.CpfPessoaContainer', _objRef.content).removeClass('hide');
				$('.CnpjPessoaContainer', _objRef.content).addClass('hide');
				$('.inputCnpjPessoa', _objRef.content).val('');
				$('.inputCpfPessoa', _objRef.content).focus();
			}
			else // jurídica
			{
				$('.CnpjPessoaContainer', _objRef.content).removeClass('hide');
				$('.CpfPessoaContainer', _objRef.content).addClass('hide');
				$('.inputCpfPessoa', _objRef.content).val('');
				$('.inputCnpjPessoa', _objRef.content).focus();
			}
		},

		editar: function (params, thisRef) {
			Mensagem.limpar(MasterPage.getContent(_objRef.content));

			Aux.carregando(_objRef.content, true);
			params['tipoCadastro'] = $('#Pessoa_Tipo', _objRef.content).val();
			params['id'] = _objRef.settings.isCopiado ? 0 : params['id'];
			params['internoId'] = $('.internoId', _objRef.content).val();
			params['isCopiado'] = _objRef.settings.isCopiado;
			params['isConjuge'] = _objRef.settings.isConjuge;
			_objRef.settings.conjugeEditarVisualizar = true;

			$.ajax({
				url: _objRef.settings.urls.editar, data: params, cache: false, async: false,
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					Aux.error(XMLHttpRequest, textStatus, errorThrown, MasterPage.getContent(_objRef.content));
					Aux.carregando(_objRef.content, false);
				},
				success: function (responseHtml, textStatus, XMLHttpRequest) {

					if (typeof (responseHtml) === "object") {
						return;
					}

					var pai = _objRef.content.parent();
					_objRef.content.replaceWith(responseHtml);
					var novoContent = $('.pessoaPartial', pai);
					_objRef.load(novoContent, _objRef.settings, false);
					MasterPage.botoes(novoContent);
					Mascara.load(novoContent);
					MasterPage.redimensionar();
					Aux.carregando(_objRef.content, false);
					_objRef.settings.onEditarEnter(params);
				}
			});
		},

		onVerificarClick: function () {
			Aux.carregando(_objRef.content, true);
			Mensagem.limpar(MasterPage.getContent(_objRef.content));

			var tipoPessoaNum = 0;
			var cpfCnpj = '';

			if (+$('input.rdbPessaoTipo:checked', _objRef.content).val() == 1) { // física
				tipoPessoaNum = 1;
				cpfCnpj = $('input.inputCpfPessoa', _objRef.content).val();
			} else { // jurídica
				tipoPessoaNum = 2;
				cpfCnpj = $('input.inputCnpjPessoa', _objRef.content).val();
			}
			debugger;
			var jsonVerificar = MasterPage.json(_objRef.content);
			jsonVerificar.TipoCadastro = _objRef.settings.tipoCadastro;

			$.ajax({
				url: _objRef.settings.urls.verificar, data: jsonVerificar, cache: false, async: false, dataType: 'json',
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					Aux.error(XMLHttpRequest, textStatus, errorThrown, MasterPage.getContent(_objRef.content));
					Aux.carregando(_objRef.content, false);
				},
				success: function (response, textStatus, XMLHttpRequest) {

					if (response.IsCpfCnpjValido) {
						var pai = _objRef.content.parent();
						var urlAcao = '';
						var param = {};

						if (response.PessoaId > 0 || response.InternoId > 0) {
							param = { Id: response.PessoaId, TipoCadastro: _objRef.settings.tipoCadastro, InternoId: response.InternoId };
						} else {
							param = { cpfCnpj: cpfCnpj, tipoPessoa: tipoPessoaNum, TipoCadastro: _objRef.settings.tipoCadastro };
						}

						//caso a atividade seja Informação de Corte e não tenha encontrado a pessoa
						//não pode deixar que seja fieto o cadastro da pessoa
						if (response.isAtividadeCorteAssociada && (response.PessoaId === 0 && response.InternoId === 0)) {
							Mensagem.gerar(MasterPage.getContent(_objRef.content), response.Msg);
							Aux.carregando(_objRef.content, false);
						} else {
							$.ajax({
								url: response.UrlAcao, data: param, cache: false, async: false,
								error: function (XMLHttpRequest, textStatus, errorThrown) {
									Aux.error(XMLHttpRequest, textStatus, errorThrown, MasterPage.getContent(_objRef.content));
								},
								success: function (responseHtml, textStatus, XMLHttpRequest) {
									if (typeof (responseHtml) === "object") {
										return;
									}
									var pai = _objRef.content.parent();
									_objRef.settings.isCopiado = response.isCopiado;
									_objRef.content.replaceWith(responseHtml);

									var novoContent = $('.pessoaPartial', pai);

									if (response.Msg != null && response.Msg.length > 0) {
										Mensagem.gerar(MasterPage.getContent(pai), response.Msg);
									}

									_objRef.load(novoContent, _objRef.settings, false);
									MasterPage.botoes(novoContent);
									Mascara.load(novoContent);
									MasterPage.redimensionar();

									Modal.dimensionar(novoContent, Modal.tamanhoModalMedia.width, Modal.tamanhoModalMedia.minWidthPerc);
									$('.fundoModal', novoContent).data['modalTamanho'] = Modal.tamanhoModalMedia;

									if (response.PessoaId > 0 || response.InternoId > 0) {
										_objRef.settings.onVisualizarEnter({ mostrarImportar: response.PessoaId <= 0 && response.InternoId > 0, mostrarEditar: true });
									} else {
										_objRef.settings.onCriarEnter();
									}

									$('.hdnIsConjuge', _objRef.content).val(_objRef.settings.isConjuge);

									setTimeout(function () {
										$(':text:enabled:first', _objRef.content).focus();
									}, 300);
								}
							});

							Aux.carregando(_objRef.content, false);
						}
					}
					else {
						if (response.Msg != null && response.Msg.length > 0) {
							Mensagem.gerar(MasterPage.getContent(_objRef.content), response.Msg);
							Aux.carregando(_objRef.content, false);
						}
					}
				}
			});
		},

		onLimparClick: function () {
			Mensagem.limpar(MasterPage.getContent(_objRef.content));

			$.ajax({
				url: _objRef.settings.urls.limpar, cache: false, async: false, data: { tipoCadastro: _objRef.settings.tipoCadastro },
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					Aux.error(XMLHttpRequest, textStatus, errorThrown, MasterPage.getContent(_objRef.content));
				},

				success: function (responseHtml, textStatus, XMLHttpRequest) {

					if (typeof (responseHtml) === "object") {
						return;
					}

					var pai = _objRef.content.parent();

					_objRef.content.replaceWith(responseHtml);
					var novoContent = $('.pessoaPartial', pai);
					_objRef.load(novoContent, false);
					MasterPage.botoes(novoContent);
					Mascara.load(novoContent);
					if (typeof _objRef.settings.onVerificarEnter == 'function') {
						_objRef.settings.onVerificarEnter();
					}
				}
			});
		},

		obter: function () {
			$('.disabled', _objRef.content).removeAttr('disabled');

			$('.tabRepresentantes', _objRef.content).find('tbody tr').each(function (index, linha) {
				$(linha).find('.hdnRepresentanteIndex').attr('name', 'Pessoa.Juridica.Representantes[' + index + '].Index').val(index);
				$(linha).find('.hdnRepresentanteId').attr('name', 'Pessoa.Juridica.Representantes[' + index + '].Id');
				$(linha).find('.hdnRepresentanteInternoId').attr('name', 'Pessoa.Juridica.Representantes[' + index + '].InternoId');
				$(linha).find('.hdnRepresentanteNome').attr('name', 'Pessoa.Juridica.Representantes[' + index + '].Fisica.Nome');
				$(linha).find('.hdnRepresentanteCpf').attr('name', 'Pessoa.Juridica.Representantes[' + index + '].Fisica.CPF');
				$(linha).find('.hdnIsCopiado').attr('name', 'Pessoa.Juridica.Representantes[' + index + '].IsCopiado');

				$(linha).find('.hdnConjugeId').attr('name', 'Pessoa.Juridica.Representantes[' + index + '].Fisica.ConjugeId');
				$(linha).find('.hdnConjugeInternoId').attr('name', 'Pessoa.Juridica.Representantes[' + index + '].Fisica.ConjugeInternoId');
			});

			var jsonPessoa = MasterPage.json(_objRef.content);
			jsonPessoa['TipoCadastro'] = _objRef.settings.tipoCadastro;

			if ($('.ckbAlterarSenha', _objRef.content).length > 0) {
				jsonPessoa["Credenciado.AlterarSenha"] = $('.ckbAlterarSenha', _objRef.content).is(':checked');
			}

			$('.disabled', _objRef.content).attr('disabled', 'disabled');

			return jsonPessoa;
		},

		salvar: function () {
			Mensagem.limpar();
			var jsonPessoa = _objRef.obter();
			var urlAcao;

			if (_objRef.obterIdPessoa().id == 0) {
				urlAcao = _objRef.settings.urls.criar;
			} else {
				urlAcao = _objRef.settings.urls.editar;
			}

			var retorno = null;

			$.ajax({
				url: urlAcao, data: JSON.stringify(jsonPessoa), type: 'POST', typeData: 'json',
				contentType: 'application/json; charset=utf-8', cache: false, async: false,
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					Aux.error(XMLHttpRequest, textStatus, errorThrown, MasterPage.getContent(_objRef.content));
				},

				success: function (responseJson, textStatus, XMLHttpRequest) {

					if (responseJson.IsPessoaSalva) {
						if (_objRef.settings.onSalvar != null) {
							_objRef.settings.onSalvar(_objRef.content, responseJson, (urlAcao == _objRef.settings.urls.editar));
						}
						retorno = responseJson;
					} else {
						if (responseJson.Msg !== null && responseJson.Msg.length > 0) {
							Mensagem.gerar(MasterPage.getContent(_objRef.content), responseJson.Msg);
							MasterPage.carregando(false);
						}
					}

				}
			});

			if (retorno != null) {
				retorno['IsEditar'] = (urlAcao == _objRef.settings.urls.editar);
			}

			return retorno;
		},

		obterIdPessoa: function () {
			return { id: parseInt($('.pessoaId', _objRef.content).val()) || 0, internoId: $('.internoId', _objRef.content).val() || 0 };
		},

		onAssociarRepresentanteClick: function () {
			$('.tabRepresentantes tbody tr', _objRef.content).removeClass('editando');
			_objRef.representanteModal = new PessoaAssociar();
			_objRef.representanteModal.settings.editarVisualizar = _objRef.settings.editarVisualizar;

			Modal.abrir(_objRef.settings.urls.pessoaModal, { tipoCadastro: 1 }, function (container) {
				Modal.defaultButtons(container);
				_objRef.representanteModal.load(container, { urls: {}, msgs: {}, onAssociarCallback: _objRef.associarRepresentante });
			}, Modal.tamanhoModalGrande);
		},

		associarRepresentante: function (pessoaRepresentante) {
			if (_objRef.existeRepresentanteAssociado(pessoaRepresentante.Id)) {
				return [_objRef.settings.msgs.RepresentanteExistente];
			}

			$('.tabRepresentantes tbody tr', _objRef.content).each(function () {

				if (pessoaRepresentante.Fisica.Conjuge != null && ($('.hdnRepresentanteId', this).val() == pessoaRepresentante.Fisica.Conjuge.InternoId)) {
					$('.hdnConjugeId', this).val(pessoaRepresentante.Id)
					$('.hdnConjugeInternoId', this).val(pessoaRepresentante.InternoId)
				}
			});

			var tabRepresentantes = $('.tabRepresentantes', _objRef.content);
			var linha = $('.tabRepresentantes tbody .editando', _objRef.content);
			var adicionar = linha.length <= 0;

			if (adicionar) {
				var linha = $('.trRepresentanteTemplate', _objRef.content).clone().removeClass('trRepresentanteTemplate');
			}

			$('.hdnRepresentanteId', linha).val(pessoaRepresentante.Id);
			$('.hdnRepresentanteInternoId', linha).val(pessoaRepresentante.InternoId);
			$('.hdnRepresentanteNome', linha).val(pessoaRepresentante.NomeRazaoSocial);
			$('.RepresentanteNome', linha).text(pessoaRepresentante.NomeRazaoSocial).attr('title', pessoaRepresentante.NomeRazaoSocial);
			$('.hdnRepresentanteCpf', linha).val(pessoaRepresentante.CPFCNPJ);
			$('.RepresentanteCpf', linha).html(pessoaRepresentante.CPFCNPJ).attr('title', pessoaRepresentante.CPFCNPJ);
			$('.hdnIsCopiado', linha).val(false);

			if (adicionar) {
				$('tbody:last', tabRepresentantes).append(linha);
				Listar.atualizarEstiloTable(tabRepresentantes);
			}

			return true;
		},

		existeRepresentanteAssociado: function (id) {
			var existe = false;
			$('.tabRepresentantes .hdnRepresentanteId', _objRef.content).each(function () {
				if (!$(this).closest('tr').hasClass('editando') && $(this).val() == id) {
					existe = true;
					return;
				}
			});

			return existe;
		},

		onExcluirRepresentante: function () {
			tabRepresentantes = $(this).closest('table');
			$(this).closest('tr').remove();
			Listar.atualizarEstiloTable(tabRepresentantes);
		},

		onBtnVisualizarRepresentanteClick: function () {
			var container = $(this).closest('tr');

			$(this).closest('tbody').find('tr').removeClass('editando');
			container.addClass('editando');

			_objRef.representanteModal = new PessoaAssociar();

			var repId = container.find('.hdnRepresentanteId').val();
			var internoId = container.find('.hdnRepresentanteInternoId').val();
			var isCopiado = container.find('.hdnIsCopiado').val() == "true";

			_objRef.representanteModal.associarAbrir(
				_objRef.settings.urls.pessoaModalVisualizar,
				{
					onAssociarCallback: _objRef.associarRepresentante,
					tituloVerificar: 'Verificar CPF/CNPJ',
					tituloCriar: 'Cadastrar Representante',
					tituloEditar: 'Editar Representante',
					tituloVisualizar: 'Visualizar Representante',
					visualizando: true,
					tipoCadastro: 1,
					isCopiado: isCopiado,
					editarVisualizar: _objRef.settings.editarVisualizar
				},

				{ id: isCopiado ? 0 : repId, internoId: internoId });
		},

		onBtnLimparProfissaoClick: function () {
			_objRef.associarProfissao(0, _objRef.settings.textoDefaultProfissao);
			$('.btnLimparProfissao', _objRef.content).addClass('hide');
			$('.btnAssociarProfissao', _objRef.content).removeClass('hide');
			$('.hdnProfissaoIdRelacionamento', _objRef.content).val(0);
		},

		onAssociarProfissaoClick: function () {
			Modal.abrir(_objRef.settings.urls.modalAssociarProfissao, null, function (container) {
				Modal.defaultButtons(container);
				ProfissaoAssociar.load(container, { associarFunc: _objRef.associarProfissao });
			}, Modal.tamanhoModalGrande);
		},

		associarProfissao: function (id, texto) {
			$('.hdnProfissao', _objRef.content).val(id);
			$('.txtProfissao', _objRef.content).val(texto);
			$('.ddlOrgaoClasse', _objRef.content).focus();

			$('.btnLimparProfissao', _objRef.content).toggleClass('hide', id <= 0);
			$('.btnAssociarProfissao', _objRef.content).toggleClass('hide', id > 0);
			$('.hdnProfissaoIdRelacionamento', _objRef.content).val(0);
		},

		onVerficarCpfCnpjKeyUp: function (e) {
			var keyENTER = 13;
			if (e.keyCode == keyENTER) {
				$('.btnVerificarCpfCnpj', _objRef.content).click();
			}
			return false;
		},

		onAlterarSenhaClick: function () {
			var alterarSenha = $('.ckbAlterarSenha', _objRef.content).is(':checked');

			$('.divAlterarSenha', _objRef.content).toggleClass('hide', !alterarSenha);
			$('.txtSenha', _objRef.content).val('');
			$('.txtConfirmarSenha', _objRef.content).val('');
		}
	};
};
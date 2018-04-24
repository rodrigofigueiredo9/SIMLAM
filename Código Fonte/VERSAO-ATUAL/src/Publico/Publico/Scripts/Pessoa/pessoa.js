/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
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
			associarConjuge: false,
			isConjuge: false,

			urls: {
				modalAssociarProfissao: '',
				modalAssociarRepresentante: '',
				verificar: '',
				limpar: '',
				visualizar: '',
				visualizarModal: '',
				criar: '',
				editar: '',
				pessoaModal: '',
				pessoaModalVisualizar: '',
				salvarConjuge: '',
				pessoaModalVisualizarConjuge: ''
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

			$('.rdbPessaoTipo', content).change(_objRef.onPessoaTipoChange);
			$('.rdbCredenciadoTipo', content).change(_objRef.onCredenciadoTipoChange);
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

			if (_objRef.settings.modoInline) {
				$('.btnLimparCpfCnpj', content).remove();
			}

			$(content).delegate('.btnExcluirRepresentante', 'click', _objRef.onExcluirRepresentante);
			$(content).delegate('.btnVisualizarRepresentante', 'click', _objRef.onBtnVisualizarRepresentanteClick);
			$(content).delegate('.btnVisualizarConjugue', 'click', _objRef.onVisualizarConjugue);

			$(content).find(':text:enabled:first').focus();
		},

		onVisualizarConjugue: function () {
			_objRef.conjugeModal = new PessoaAssociar();
			_objRef.conjugeModal.settings.isCopiado = _objRef.settings.isCopiado;
			_objRef.conjugeModal.settings.isConjuge = true;

			var conjuge = JSON.parse($('.hdnConjugeJSON', _objRef.content).val());

			Modal.abrir(_objRef.settings.urls.pessoaModalVisualizarConjuge, {pessoa: conjuge},
				function (container) {
					_objRef.conjugeModal.load(container, {
						tituloCriar: 'Criar Pessoa',
						tituloEditar: 'Editar Pessoa',
						tituloVisualizar: 'Visualizar Pessoa',
						onAssociarCallback: _objRef.callBackAssociarConjuge,
						visualizando: true,
						isConjuge: true
					});
				}, Modal.tamanhoModalGrande);
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
			_objRef.conjugeModal.associarAbrir(
				_objRef.settings.urls.modalAssociarRepresentante,
				{ onAssociarCallback: _objRef.callBackAssociarConjuge,
					tituloVerificar: 'Verificar CPF/CNPJ',
					tituloCriar: 'Cadastrar Cônjuge',
					tituloEditar: 'Editar Cônjuge',
					tituloVisualizar: 'Visualizar Cônjuge'
				},
				{ tipoCadastro: 1, isAssociarConjuge: true });
		},

		onLimparConjuge: function () {
			$('.txtConjugeCPF', _objRef.content).val('');
			$('.hdnConjugeJSON', _objRef.content).val('');

			$('.txtConjugeNome', _objRef.content).val('');
			$('.txtConjugeNome', _objRef.content).removeClass('erroCampo');

			$('.btnLimparConjuge', _objRef.content).addClass('hide');
			$('.btnAssociarConjuge', _objRef.content).removeClass('hide');
			$('.spanVisualizarConjuge', _objRef.content).addClass('hide');
			
		},

		callBackAssociarConjuge: function (pessoaAssociada) {
			if ($('.inputCpfPessoa', _objRef.content).val() == pessoaAssociada.CPFCNPJ) {
				return new Array(_objRef.settings.msgs.PessoaConjugeSaoIguais);
			}

			$('.hdnConjugeJSON', _objRef.content).val(JSON.stringify(pessoaAssociada));
			$('.txtConjugeCPF', _objRef.content).val(pessoaAssociada.CPFCNPJ);
			$('.txtConjugeNome', _objRef.content).val(pessoaAssociada.NomeRazaoSocial);

			$('.btnLimparConjuge', _objRef.content).removeClass('hide');
			$('.spanVisualizarConjuge', _objRef.content).removeClass('hide');
			$('.btnAssociarConjuge', _objRef.content).addClass('hide');

			return true;
		},

		onPessoaTipoChange: function () {
			if ($('input.rdbPessaoTipo:checked', _objRef.content).val() == '1') { // física
				$('.CpfPessoaContainer', _objRef.content).show();
				$('.CnpjPessoaContainer', _objRef.content).hide();
				$('.inputCnpjPessoa', _objRef.content).val('');
				$('.inputCpfPessoa', _objRef.content).focus();
			}
			else // jurídica
			{
				$('.CnpjPessoaContainer', _objRef.content).show();
				$('.CpfPessoaContainer', _objRef.content).hide();
				$('.inputCpfPessoa', _objRef.content).val('');
				$('.inputCnpjPessoa', _objRef.content).focus();
			}
		},

		onCredenciadoTipoChange: function () {
			var tipo = $('.rdbCredenciadoTipo:checked', _objRef.content).val();
			var exibirAsterisco = (tipo == 2); //Responsavel tecnico

			$('.divOrgaoParceiroConveniado', _objRef.content).toggleClass('hide', tipo != 3);//Orgao parceiro/ conveniado

			_objRef.asterisco($('.lgProfissao', _objRef.content), exibirAsterisco);
		},

		asterisco: function (control, exibir) {
		    
			control.text(control.text().replace(' *', ''));

			if (exibir){
				control.text(control.text() + ' *');
			}
		},

		editar: function (PessoaId, thisRef) {
			Aux.carregando(_objRef.content, true);
			var jsonPessoa = _objRef.obter();

			$.ajax({ url: _objRef.settings.urls.editar, data: JSON.stringify(jsonPessoa), type: 'POST', typeData: 'json',
				contentType: 'application/json; charset=utf-8', cache: false, async: false,
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
					var param = { Id: PessoaId };
					_objRef.settings.onEditarEnter(param);
				}
			});
		},

		onVerificarClick: function () {
			Mensagem.limpar(MasterPage.getContent(_objRef.content));
			Aux.carregando(_objRef.content, true);

			var tipoPessoaNum = 0;
			var cpfCnpj = '';

			if (+$('input.rdbPessaoTipo:checked', _objRef.content).val() == 1) { // física
				tipoPessoaNum = 1;
				cpfCnpj = $('input.inputCpfPessoa', _objRef.content).val();
			} else { // jurídica
				tipoPessoaNum = 2;
				cpfCnpj = $('input.inputCnpjPessoa', _objRef.content).val();
			}

			var jsonVerificar = MasterPage.json(_objRef.content);
			jsonVerificar.TipoCadastro = _objRef.settings.tipoCadastro;

			$.ajax({ url: _objRef.settings.urls.verificar, data: jsonVerificar, cache: false, async: false, dataType: 'json',
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					Aux.error(XMLHttpRequest, textStatus, errorThrown, MasterPage.getContent(_objRef.content));
					Aux.carregando(_objRef.content, false);
				},
				success: function (response, textStatus, XMLHttpRequest) {

					if (response.IsRedirecionar) {
						MasterPage.redireciona(response.UrlAcao);
						return;
					}

					if (response.IsCpfCnpjValido) {
						var pai = _objRef.content.parent();

						$.ajax({ url: response.UrlAcao, data: response.Parametros, cache: false, async: false,
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

								if (_objRef.settings.associarConjuge) {
									$('.divConjuge', novoContent).remove();
								}

								if (response.Msg != null && response.Msg.length > 0) {
									Mensagem.gerar(MasterPage.getContent(pai), response.Msg);
								}

								_objRef.load(novoContent, _objRef.settings, false);
								MasterPage.botoes(novoContent);
								Mascara.load(novoContent);
								MasterPage.redimensionar();
								Listar.atualizarEstiloTable(_objRef.content);

								if (response.PessoaId > 0) {
									_objRef.settings.onVisualizarEnter();
								} else {
									_objRef.settings.onCriarEnter();
								}

								setTimeout(function () {
									$(':text:enabled:first', _objRef.content).focus();
								}, 300);
							}
						});

						Aux.carregando(_objRef.content, false);
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
			$.ajax({ url: _objRef.settings.urls.limpar, cache: false, async: false, data: { tipoCadastro: _objRef.settings.tipoCadastro },
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
				$(linha).find('.hdnRepresentanteJSON').attr('name', 'Pessoa.Juridica.Representantes[' + index + ']');
			});

			var jsonPessoa = MasterPage.json(_objRef.content);

			if ($('input.inputCnpjPessoa', _objRef.content).is(':enabled')) {
				$('.tabRepresentantes', _objRef.content).find('tbody tr').each(function (index, linha) {
					jsonPessoa['Pessoa.Juridica.Representantes[' + index + ']'] = JSON.parse($(linha).find('.hdnRepresentanteJSON').val());
				});
			} else if ($('.hdnConjugeJSON', _objRef.content).val()) {
				jsonPessoa['Pessoa.Fisica.Conjuge'] = JSON.parse($('.hdnConjugeJSON', _objRef.content).val());
			}

			jsonPessoa['isConjuge'] = _objRef.settings.isConjuge;

			$('.disabled', _objRef.content).attr('disabled', 'disabled');

			return jsonPessoa;
		},

		salvar: function () {
			//Salvar Conjuge
			if (_objRef.settings.isConjuge) {
				return _objRef.salvarConjuge();
			}

			var jsonPessoa = _objRef.obter();
			var pessoaId = _objRef.obterIdPessoa();
			var urlAcao;

			if (pessoaId == 0) {
				urlAcao = _objRef.settings.urls.criar;
			} else {
				urlAcao = _objRef.settings.urls.editar;
			}

			var retorno = null;

			$.ajax({ url: urlAcao, data: JSON.stringify(jsonPessoa), type: 'POST', typeData: 'json',
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

		salvarConjuge: function () {
			var jsonPessoa = _objRef.obter();
			var retorno = null;

			$.ajax({
				url: _objRef.settings.urls.salvarConjuge, data: JSON.stringify(jsonPessoa), type: 'POST', typeData: 'json',
				contentType: 'application/json; charset=utf-8', cache: false, async: false,
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					Aux.error(XMLHttpRequest, textStatus, errorThrown, MasterPage.getContent(_objRef.content));
				},
				success: function (response, textStatus, XMLHttpRequest) {

					if (response.IsValido) {

						if (_objRef.settings.onSalvar != null) {
							_objRef.settings.onSalvar(_objRef.content, response, true);
						}

						retorno = response;

						return;

					}

					if (response.Msg !== null && response.Msg.length > 0) {
						Mensagem.gerar(MasterPage.getContent(_objRef.content), response.Msg);
						MasterPage.carregando(false);
					}
				}
			});

			return retorno;

		},

		obterIdPessoa: function () {
			if (!$('.pessoaId', _objRef.content).length) return 0;
			return parseInt($('.pessoaId', _objRef.content).val());
		},

		onAssociarRepresentanteClick: function () {
			$('.tabRepresentantes tbody tr', _objRef.content).removeClass('editando');
			_objRef.representanteModal = new PessoaAssociar();

			_objRef.representanteModal.associarAbrir(
				_objRef.settings.urls.modalAssociarRepresentante,
				{ onAssociarCallback: _objRef.associarRepresentante,
					tituloVerificar: 'Verificar CPF/CNPJ',
					tituloCriar: 'Cadastrar Representante',
					tituloEditar: 'Editar Representante',
					tituloVisualizar: 'Visualizar Representante'
				},
				{ tipoCadastro: 1 });
		},

		associarRepresentante: function (pessoaRepresentante) {
			if (_objRef.existeRepresentanteAssociado(pessoaRepresentante.CPFCNPJ)) {
				return [_objRef.settings.msgs.RepresentanteExistente];
			}

			var tabRepresentantes = $('.tabRepresentantes', _objRef.content);
			var linha = $('.tabRepresentantes tbody .editando', _objRef.content);
			var adicionar = linha.length <= 0;

			if (adicionar) {
				var linha = $('.trRepresentanteTemplate', _objRef.content).clone().removeClass('trRepresentanteTemplate');
			}

			$('.hdnRepresentanteJSON', linha).val(JSON.stringify(pessoaRepresentante));
			$('.RepresentanteNome', linha).html(pessoaRepresentante.NomeRazaoSocial).attr('title', pessoaRepresentante.NomeRazaoSocial);
			$('.RepresentanteCpf', linha).html(pessoaRepresentante.CPFCNPJ).attr('title', pessoaRepresentante.CPFCNPJ);

			if (adicionar) {
				$('tbody:last', tabRepresentantes).append(linha);
				Listar.atualizarEstiloTable(tabRepresentantes);
			}

			return true;
		},

		existeRepresentanteAssociado: function (cpfCnpj) {
			var existe = false;
			$('.tabRepresentantes .hdnRepresentanteJSON', _objRef.content).each(function () {
				if (JSON.parse($(this).val()).Fisica.CPF == cpfCnpj && !$(this).closest('tr').hasClass('editando')) {
					existe = true;
					return;
				}
			});

			return existe;
		},

		onExcluirRepresentante: function () {
			$(this).closest('tr').remove();
		},

		onBtnVisualizarRepresentanteClick: function () {
			var linha = $(this).closest('tr');
			var rep = JSON.parse($(this).closest('tr').find('.hdnRepresentanteJSON').val());

			$(this).closest('tbody').find('tr').removeClass('editando');
			linha.addClass('editando');

			_objRef.representanteModal = new PessoaAssociar();

			_objRef.representanteModal.associarAbrir(
				_objRef.settings.urls.visualizarModal,
				{ onAssociarCallback: _objRef.associarRepresentante,
					tituloVerificar: 'Verificar CPF/CNPJ',
					tituloCriar: 'Cadastrar Representante',
					tituloEditar: 'Editar Representante',
					tituloVisualizar: 'Visualizar Representante',
					visualizando: true,
					tipoCadastro: 1
				},
				rep);
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
		}
	};
};
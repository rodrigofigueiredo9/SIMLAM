/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../masterpage.js" />
/// <reference path="profissao.js" />
/// <reference path="representante.js" />


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
			callBackCopiarEnderecoConjuge: null,
			conjugeId: 0,
			possuiConjuge: false,

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
				obterEnderecoPessoa: ''
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

			if (!_objRef.settings.callBackCopiarEnderecoConjuge) {
				_objRef.settings.callBackCopiarEnderecoConjuge = _objRef.copiarEnderecoConjugeCadastrado;
			}

			if (options) {
				$.extend(_objRef.settings, options);
			}

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

			if (_objRef.settings.modoInline) {
				$('.btnLimparCpfCnpj', content).remove();
			}

			$(content).delegate('.btnExcluirRepresentante', 'click', _objRef.onExcluirRepresentante);
			$(content).delegate('.btnVisualizarRepresentante', 'click', _objRef.onBtnVisualizarRepresentanteClick);
			$(content).delegate('.btnCopiarEnderecoConjuge', 'click', _objRef.onCopiarEnderecoConjuge);

			$(content).find(':text:enabled:first').focus();

			_objRef.gerenciarBotaoCopiarEnderecoConjuge();
		},

		onChangeEstadoCivil: function () {
            
            
			$('.divConjuge', _objRef.content).removeClass('hide');
			if ($(this).val() != 2 && $(this).val() != 5) {
				_objRef.onLimparConjuge();
				$('.divConjuge', _objRef.content).addClass('hide');
			}

			_objRef.gerenciarBotaoCopiarEnderecoConjuge();
		},

		onAssociarConjuge: function () {
			_objRef.conjugeModal = new PessoaAssociar();
			var conjuge = $('.pessoaId', _objRef.settings.container).val();
			var possuiConjuge = $('.ddlEstadoCivil :selected', _objRef.settings.container).val() == 2 || $('.ddlEstadoCivil :selected', _objRef.settings.container).val() == 5;

			Modal.abrir(_objRef.settings.urls.pessoaModal, { tipoCadastro: 1 }, function (container) {

				Modal.defaultButtons(container);
				_objRef.conjugeModal.load(container, { urls: {}, msgs: {},conjugeId: conjuge, possuiConjuge: possuiConjuge, onAssociarCallback: _objRef.callBackAssociarConjuge, callBackCopiarEnderecoConjuge: _objRef.copiarEnderecoConjugeNaoCadastrado });

			}, Modal.tamanhoModalGrande, 'Pessoas');
		},

		copiarEnderecoConjugeNaoCadastrado: function () {

			var container = _objRef.content;

			return {
				Cep: $('#Pessoa_Endereco_Cep', container).val(),
				Logradouro: $('#Pessoa_Endereco_Logradouro', container).val(),
				Bairro: $('#Pessoa_Endereco_Bairro', container).val(),
				Numero: $('#Pessoa_Endereco_Numero', container).val(),
				DistritoLocalizacao: $('#Pessoa_Endereco_DistritoLocalizacao', container).val(),
				Complemento: $('#Pessoa_Endereco_Complemento', container).val(),
				Corrego: $('#Pessoa_Endereco_Corrego', container).val(),
				EstadoId: $('.ddlEstado :selected', container).val(),
				MunicipioId: $('.ddlMunicipio :selected', container).val()
			};

		},

		copiarEnderecoConjugeCadastrado: function () {
			var endereco = null;
			var conjugeId = $('.hdnConjugeId', _objRef.content).val();

			$.ajax({
				url: _objRef.settings.urls.obterEnderecoPessoa,
				data: { pessoaId: conjugeId },
				async: false,
				cache: false,
				error: function (XMLHttpRequest, textStatus, erroThrown, container) {
					Aux.error(XMLHttpRequest, textStatus, erroThrown, container);
				},
				success: function (response, textStatus, XMLHttpRequest) {

					if (response.Endereco) {
						endereco = response.Endereco;
					}

					if (response.Msg && response.Msg.length > 0) {
						Mensagem.gerar(response.Msg);
					}
				}
			});

			return endereco;
		},

		onCopiarEnderecoConjuge: function () {
			var endereco = null;

			if (_objRef.settings.callBackCopiarEnderecoConjuge) {
				endereco = _objRef.settings.callBackCopiarEnderecoConjuge();
			}

			if (endereco) {
				_objRef.carregarEndereco(endereco, _objRef.content);
			}
		},

		gerenciarBotaoCopiarEnderecoConjuge: function () {
			var container = _objRef.content;
			var possuiConjuge = _objRef.settings.possuiConjuge;
			var conjugeId = Number(_objRef.settings.conjugeId) || $('.hdnConjugeId', container).val();

			$('.divCopiarEnderecoConjuge', container).addClass('hide');

			if (conjugeId > 0 || possuiConjuge) {
				$('.divCopiarEnderecoConjuge', container).removeClass('hide');
			}
		},

		onLimparConjuge: function () {

			$('.txtConjugeCPF', _objRef.content).val('');
			$('.hdnConjugeCPF', _objRef.content).val('');

			$('.txtConjugeNome', _objRef.content).val('');
			$('.txtConjugeNome', _objRef.content).removeClass('erroCampo');

			$('.hdnConjugeId', _objRef.content).val(0);

			$('.btnLimparConjuge', _objRef.content).addClass('hide');
			$('.btnAssociarConjuge', _objRef.content).removeClass('hide');
			$('.divCopiarEnderecoConjuge', _objRef.content).addClass('hide');
		},

		carregarEndereco: function (endereco, container) {
			$('#Pessoa_Endereco_Cep', container).val(endereco.Cep);
			$('#Pessoa_Endereco_Logradouro', container).val(endereco.Logradouro);
			$('#Pessoa_Endereco_Bairro', container).val(endereco.Bairro);
			$('#Pessoa_Endereco_Numero', container).val(endereco.Numero);
			$('#Pessoa_Endereco_DistritoLocalizacao', container).val(endereco.DistritoLocalizacao);
			$('#Pessoa_Endereco_Complemento', container).val(endereco.Complemento);
			$('#Pessoa_Endereco_Corrego', container).val(endereco.Corrego);

			$('.ddlEstado option', container).each(function () {
				if ($(this).val() == endereco.EstadoId) {
					$(this).attr('selected', 'selected');
				}
			});

			$('.ddlEstado', container).trigger('change');
			$('.ddlEstado', container).removeAttr('disabled');
			$('.ddlEstado', container).removeClass('disabled');

			$('.ddlMunicipio option', container).each(function () {
				if ($(this).val() == endereco.MunicipioId) {
					$(this).attr('selected', 'selected');
				}
			});
		},

		callBackAssociarConjuge: function (pessoaAssociada) {

			if ($('.Pessoa_Id', _objRef.content).val() == pessoaAssociada.Id) {
				return new Array(_objRef.settings.msgs.PessoaConjugeSaoIguais);
			}

			$('.hdnConjugeCPF', _objRef.content).val(pessoaAssociada.CPFCNPJ);
			$('.txtConjugeCPF', _objRef.content).val(pessoaAssociada.CPFCNPJ);
			$('.txtConjugeNome', _objRef.content).val(pessoaAssociada.NomeRazaoSocial);
			$('.hdnConjugeId', _objRef.content).val(pessoaAssociada.Id);

			$('.btnLimparConjuge', _objRef.content).removeClass('hide');
			$('.btnAssociarConjuge', _objRef.content).addClass('hide');

			_objRef.gerenciarBotaoCopiarEnderecoConjuge();

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

		editar: function (PessoaId, thisRef) {
			Aux.carregando(_objRef.content, true);

			var urlAcao = _objRef.settings.urls.editar + '/' + PessoaId + '?tipoCadastro=' + _objRef.settings.tipoCadastro;

			$.ajax({ url: urlAcao, cache: false, async: false,
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

					if (response.IsCpfCnpjValido) {
						var pai = _objRef.content.parent();
						var urlAcao = '';
						var param = {};

						if (response.PessoaId > 0) {
							param = { Id: response.PessoaId, TipoCadastro: _objRef.settings.tipoCadastro };
							//if (_objRef.settings.modoAssociar) { // se estiver em modoAssociar, mostrar visualizar
							urlAcao = _objRef.settings.urls.visualizar;
							//} else {
							//urlAcao = _objRef.settings.urls.editar;
							//}
						} else {
							urlAcao = _objRef.settings.urls.criar;
							param = { cpfCnpj: cpfCnpj, tipoPessoa: tipoPessoaNum, TipoCadastro: _objRef.settings.tipoCadastro };
						}

						$.ajax({ url: urlAcao, data: param, cache: false, async: false,
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

								if (response.Msg != null && response.Msg.length > 0) {
									Mensagem.gerar(MasterPage.getContent(pai), response.Msg);
								}

								_objRef.load(novoContent, _objRef.settings, false);
								MasterPage.botoes(novoContent);
								Mascara.load(novoContent);
								MasterPage.redimensionar();

								if (response.PessoaId > 0) {
									//if (_objRef.settings.modoAssociar) {
									_objRef.settings.onVisualizarEnter(param);
									//} else {
									//_objRef.settings.onEditarEnter(param);
									//}
								} else {
									_objRef.settings.onCriarEnter(param);
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

		salvar: function () {
			var cpfHabilitado = $('input.inputCpfPessoa', _objRef.content).is(':enabled');
			var cnpjHabilitado = $('input.inputCnpjPessoa', _objRef.content).is(':enabled');

			$('input.inputCnpjPessoa', _objRef.content).removeAttr('disabled');
			$('input.inputCpfPessoa', _objRef.content).removeAttr('disabled');

			$('.tabRepresentantes', _objRef.content).find('tbody tr').each(function (index, linha) {
				$(linha).find('.hdnRepresentanteIndex').attr('name', 'Pessoa.Juridica.Representantes[' + index + '].Index').val(index);
				$(linha).find('.hdnRepresentanteId').attr('name', 'Pessoa.Juridica.Representantes[' + index + '].Id');
				$(linha).find('.hdnRepresentanteNome').attr('name', 'Pessoa.Juridica.Representantes[' + index + '].Fisica.Nome');
				$(linha).find('.hdnRepresentanteCpf').attr('name', 'Pessoa.Juridica.Representantes[' + index + '].Fisica.CPF');
			});

			var jsonPessoa = MasterPage.json(_objRef.content);

			jsonPessoa['TipoCadastro'] = _objRef.settings.tipoCadastro;

			if (!cpfHabilitado) $('input.inputCpfPessoa', _objRef.content).attr('disabled', 'disabled');
			if (!cnpjHabilitado) $('input.inputCnpjPessoa', _objRef.content).attr('disabled', 'disabled');

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

		obterIdPessoa: function () {
			if (!$('.pessoaId', _objRef.content).length) return 0;
			return parseInt($('.pessoaId', _objRef.content).val());
		},

		onAssociarRepresentanteClick: function () {

			_objRef.representanteModal = new PessoaAssociar();

			Modal.abrir(_objRef.settings.urls.pessoaModal, { tipoCadastro: 1 }, function (container) {
				Modal.defaultButtons(container);
				//RepresentanteAssociar.load(container, { associarFunc: _objRef.associarRepresentante });
				_objRef.representanteModal.load(container, { urls: {}, msgs: {}, onAssociarCallback: _objRef.associarRepresentante });
			}, Modal.tamanhoModalGrande);
		},

		associarRepresentante: function (pessoaRepresentante) {
			if (_objRef.existeRepresentanteAssociado(pessoaRepresentante.Id)) {
				return [_objRef.settings.msgs.RepresentanteExistente];
			}

			var tabRepresentantes = $('.tabRepresentantes', _objRef.content);
			var linha = $('.trRepresentanteTemplate', _objRef.content).clone().removeClass('trRepresentanteTemplate');
			$('.hdnRepresentanteId', linha).val(pessoaRepresentante.Id);
			$('.hdnRepresentanteNome', linha).val(pessoaRepresentante.NomeRazaoSocial);
			$('.RepresentanteNome', linha).text(pessoaRepresentante.NomeRazaoSocial).attr('title', pessoaRepresentante.NomeRazaoSocial);
			$('.hdnRepresentanteCpf', linha).val(pessoaRepresentante.CPFCNPJ);
			$('.RepresentanteCpf', linha).html(pessoaRepresentante.CPFCNPJ);
			$('tbody:last', tabRepresentantes).append(linha);
			Listar.atualizarEstiloTable(tabRepresentantes);
			return true;
		},

		existeRepresentanteAssociado: function (id) {
			return $('.tabRepresentantes .hdnRepresentanteId[value="' + parseInt(id) + '"]', _objRef.content).length;
		},

		onExcluirRepresentante: function () {
			$(this).closest('tr').remove();
		},

		onBtnVisualizarRepresentanteClick: function () {
			var repId = $(this).closest('tr').find('.hdnRepresentanteId').val();
			Modal.abrir(_objRef.settings.urls.visualizarModal, { Id: repId }, function (container) {
				$('.titTela', container).text('Visualizar Representante');
				Modal.defaultButtons(container);
			});
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
			}, Modal.tamanhoModalMedia);
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


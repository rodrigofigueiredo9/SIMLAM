/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />
/// <reference path="../jquery.ddl.js" />
/// <reference path="analisarItemAnalise.js" />
/// <reference path="../mensagem.js" />

HabilitacaoEmissaoPTV = {
	settings: {
		urls: {
			verificarCPF: '',
			associarFuncionario: '',
			enviarArquivo: '',
			associarProfissao: '',
			buscarMunicipios: '',
			validarOperador: null,
			salvar: ''
		},
		estadoDefaultId: 0,
		tiposArquivo: null,
		mensagens: null,
		onExisteOperador: null
	},
	container: null,

	load: function (container, options) {

		if (options) { $.extend(HabilitacaoEmissaoPTV.settings, options); }
		HabilitacaoEmissaoPTV.container = MasterPage.getContent(container);

		HabilitacaoEmissaoPTV.container.delegate(".btnVerificarCpf", 'click', HabilitacaoEmissaoPTV.verificarCPF);
		HabilitacaoEmissaoPTV.container.delegate(".btnSalvar", 'click', HabilitacaoEmissaoPTV.salvar);
		HabilitacaoEmissaoPTV.container.delegate(".btnAddArq", 'click', HabilitacaoEmissaoPTV.enviarArquivo);
		HabilitacaoEmissaoPTV.container.delegate(".btnArqLimpar", 'click', HabilitacaoEmissaoPTV.onLimparArquivoClick);
		HabilitacaoEmissaoPTV.container.delegate(".btnExcluirOperador", 'click', HabilitacaoEmissaoPTV.excluirItemGrid);
		HabilitacaoEmissaoPTV.container.delegate(".btnBuscarProfissao", 'click', HabilitacaoEmissaoPTV.abrirModalProfissao);
		HabilitacaoEmissaoPTV.container.delegate(".ddlEstadoEndereco", 'change', HabilitacaoEmissaoPTV.estadoChange);
		HabilitacaoEmissaoPTV.container.delegate(".btnBuscarOperador", 'click', HabilitacaoEmissaoPTV.abrirModalFuncionarios);
		HabilitacaoEmissaoPTV.container.delegate(".ddlEstadosRegistro", 'change', HabilitacaoEmissaoPTV.onChangeUf);

		Aux.setarFoco(container);
		HabilitacaoEmissaoPTV.container.delegate('.txtCpf', 'keyup', function (e) {
			if (e.keyCode == 13) $('.btnVerificarCpf', HabilitacaoEmissaoPTV.container).click();
		});
	},

	verificarCPF: function () {
		Mensagem.limpar(HabilitacaoEmissaoPTV.container);
		MasterPage.carregando(true);

		$.ajax({
			url: HabilitacaoEmissaoPTV.settings.urls.verificarCPF,
			data: JSON.stringify({ cpf: $('.txtCpf', HabilitacaoEmissaoPTV.container).val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.hdnFuncionarioId', HabilitacaoEmissaoPTV.container).val(response.Funcionario.Id);
					$('.txtNome', HabilitacaoEmissaoPTV.container).val(response.Funcionario.Nome);
					$('.mostrar', HabilitacaoEmissaoPTV.container).removeClass('hide');
					$('.gridSetores tbody tr:not(.templateRow)', HabilitacaoEmissaoPTV.container).empty();

					$(response.Funcionario.Setores).each(function () {
						var item = this;
						var linha = $('.gridSetores', HabilitacaoEmissaoPTV.container).find('.templateRow').clone();

						$('.setor', linha).text(item.Nome);
						$('.gridSetores tbody', HabilitacaoEmissaoPTV.container).append(linha);
						$(linha).removeClass('templateRow');
						$(linha).removeClass('hide');
					});
					Listar.atualizarEstiloTable($('.gridSetores', HabilitacaoEmissaoPTV.container));
				}
				else {
					Mensagem.gerar(HabilitacaoEmissaoPTV.container, response.Msg);
				}
			}
		});
		MasterPage.carregando(false);
	},

	abrirModalFuncionarios: function () {
		Modal.abrir(HabilitacaoEmissaoPTV.settings.urls.associarFuncionario, null, function (container) {
			FuncionarioListar.load(container, { associarFuncao: HabilitacaoEmissaoPTV.callBackAssociarFuncionario });
			Modal.defaultButtons(container);
		}, Modal.tamanhoModalMedia, 'Buscar Funcionário');
	},

	callBackAssociarFuncionario: function (funcionario) {
		var itemAdicionado = false;
		var operadorNome = null;

		$('.gridOperadores tbody tr:not(.templateRow)', HabilitacaoEmissaoPTV.container).each(function () {
			var item = JSON.parse($('.hdnOperadorJson', this).val());
			if (item.FuncionarioId == funcionario.Id) {
				itemAdicionado = true;
			}
		});

		if (itemAdicionado) {
			return [HabilitacaoEmissaoPTV.settings.mensagens.OperadorAdicionado];
		}

		//valida se o funcionario já é um operador
		operadorNome = HabilitacaoEmissaoPTV.onExisteOperador(funcionario);
		if (operadorNome == null) {
			if (funcionario.Situacao != 'Ativo') {
				return [HabilitacaoEmissaoPTV.settings.mensagens.SituacaoFuncionarioDeveSerAtivo];
			}

			var operador = {
				FuncionarioId: funcionario.Id,
				FuncionarioNome: funcionario.Nome
			};

			var linha = $('.gridOperadores', HabilitacaoEmissaoPTV.container).find('.templateRow').clone();

			$('.nome', linha).text(funcionario.Nome);
			$('.hdnOperadorJson', linha).val(JSON.stringify(operador));
			$(linha).removeClass('templateRow');
			$(linha).removeClass('hide');
			$('.gridOperadores tbody', HabilitacaoEmissaoPTV.container).append(linha);
			Listar.atualizarEstiloTable($('.gridOperadores', HabilitacaoEmissaoPTV.container));
			return true;
		}		
		else {
			return [Mensagem.replace(HabilitacaoEmissaoPTV.settings.mensagens.FuncionarioJaOperador, '#TEXTO#', operadorNome)];
		}
	},

	enviarArquivo: function () {
		var nomeArquivo = $('#arquivo', HabilitacaoEmissaoPTV.container).val();

		erroMsg = new Array();

		var tam = nomeArquivo.length - 4;
		if (!HabilitacaoEmissaoPTV.validarTipoArquivo(nomeArquivo.toLowerCase().substr(tam))) {
			erroMsg.push(HabilitacaoEmissaoPTV.settings.mensagens.ArquivoInvalido);
		}

		if (erroMsg.length > 0) {
			Mensagem.gerar(HabilitacaoEmissaoPTV.container, erroMsg);
			return;
		}

		MasterPage.carregando(true);
		var inputFile = $('#arquivo', HabilitacaoEmissaoPTV.container);
		FileUpload.upload(HabilitacaoEmissaoPTV.settings.urls.enviarArquivo, inputFile, HabilitacaoEmissaoPTV.callBackArqEnviado);
	},

	callBackArqEnviado: function (controle, resposta, isHtml) {
		var ret = eval('(' + resposta + ')');
		if (ret.Arquivo != null) {
			$('.txtArquivoNome', HabilitacaoEmissaoPTV.container).val(ret.Arquivo.Nome);
			$('.hdnArquivoJson', HabilitacaoEmissaoPTV.container).val(JSON.stringify(ret.Arquivo));
			$('.txtArquivoNome', HabilitacaoEmissaoPTV.container).attr('href', '/Arquivo/BaixarTemporario?nomeTemporario=' + ret.Arquivo.TemporarioNome + '&contentType=' + ret.Arquivo.ContentType);

			$('.spanInputFile', HabilitacaoEmissaoPTV.container).addClass('hide');
			$('.txtArquivoNome', HabilitacaoEmissaoPTV.container).removeClass('hide');

			$('.btnAddArq', HabilitacaoEmissaoPTV.container).addClass('hide');
			$('.btnArqLimpar', HabilitacaoEmissaoPTV.container).removeClass('hide');

			Mensagem.limpar(HabilitacaoEmissaoPTV.container);
			Mensagem.gerar(HabilitacaoEmissaoPTV.container, ret.Msg);
		} else {
			HabilitacaoEmissaoPTV.onLimparArquivoClick();
			Mensagem.gerar(HabilitacaoEmissaoPTV.container, ret.Msg);
		}

		MasterPage.carregando(false);
	},

	validarTipoArquivo: function (tipo) {
		var tipoValido = false;
		$(HabilitacaoEmissaoPTV.settings.tiposArquivo).each(function (i, tipoItem) {
			if (tipoItem == tipo) {
				tipoValido = true;
			}
		});

		return tipoValido;
	},

	onLimparArquivoClick: function () {
		$('.hdnArquivoJson', HabilitacaoEmissaoPTV.container).val('');
		$('.inputFile', HabilitacaoEmissaoPTV.container).val('');

		$('.spanInputFile', HabilitacaoEmissaoPTV.container).removeClass('hide');
		$('.txtArquivoNome', HabilitacaoEmissaoPTV.container).addClass('hide');

		$('.btnAddArq', HabilitacaoEmissaoPTV.container).removeClass('hide');
		$('.btnLimparArq', HabilitacaoEmissaoPTV.container).addClass('hide');

		$('.lnkArquivo', HabilitacaoEmissaoPTV.container).closest('div').addClass('hide');

		Mensagem.limpar(HabilitacaoEmissaoPTV.container);
	},

	abrirModalProfissao: function () {
		Modal.abrir(HabilitacaoEmissaoPTV.settings.urls.associarProfissao, null, function (container) {
			Modal.defaultButtons(container);
			ProfissaoAssociar.load(container, { associarFunc: HabilitacaoEmissaoPTV.callBackAssociarProfissao });
		}, Modal.tamanhoModalMedia, 'Buscar Profissão');
	},

	callBackAssociarProfissao: function (id, texto) {
		$('.txtProfissao', HabilitacaoEmissaoPTV.container).val(texto);
		$('.hdnProfissaoId', HabilitacaoEmissaoPTV.container).val(id);
	},

	onChangeUf: function () {
		$('.txtNumeroVistoCrea', HabilitacaoEmissaoPTV.container).val('');
		$('.txtNumeroCREA', HabilitacaoEmissaoPTV.container).val('');

		if ($('.ddlEstadosRegistro', HabilitacaoEmissaoPTV.container).val() == HabilitacaoEmissaoPTV.settings.estadoDefaultId) {
			$('.divNumeroVistoCrea', HabilitacaoEmissaoPTV.container).addClass('hide');
			$('.divNumeroCREA', HabilitacaoEmissaoPTV.container).removeClass('hide');
		}
		else {
			$('.divNumeroCREA', HabilitacaoEmissaoPTV.container).addClass('hide');
			$('.divNumeroVistoCrea', HabilitacaoEmissaoPTV.container).removeClass('hide');
		}
	},

	estadoChange: function () {
		Mensagem.limpar(HabilitacaoEmissaoPTV.container);
		MasterPage.carregando(true);
		$.ajax({
			url: HabilitacaoEmissaoPTV.settings.urls.buscarMunicipios,
			data: JSON.stringify({ estado: +$(this).val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					$('.ddlMunicipio', HabilitacaoEmissaoPTV.container).ddlLoad(response.Municipios);
				}

				if (response.Erros && response.Erros.length > 0) {
					Mensagem.gerar(HabilitacaoEmissaoPTV.container, response.Erros);
				}
			}
		});
		MasterPage.carregando(false);
	},

	onExisteOperador: function (funcionario) {
		var retorno = null;
		$.ajax({
			url: HabilitacaoEmissaoPTV.settings.urls.validarOperador,
			data: JSON.stringify({ id: funcionario.Id }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				retorno = response.Operador
			}
		});
		return retorno;
	},

	excluirItemGrid: function () {
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable(HabilitacaoEmissaoPTV.container);
	},

	obter: function () {
		var retorno = {
			Id: $('.hdnHabilitacaoId', HabilitacaoEmissaoPTV.container).val(),
			Funcionario: {
				Id: $('.hdnFuncionarioId', HabilitacaoEmissaoPTV.container).val()
			},
			NumeroHabilitacao: $('.txtNumeroHabilitacao', HabilitacaoEmissaoPTV.container).val(),
			RG: $('.txtRG', HabilitacaoEmissaoPTV.container).val(),
			NumeroMatricula: $('.txtNumeroMatricula', HabilitacaoEmissaoPTV.container).val(),
			EstadoRegistro: $('.ddlEstadosRegistro :selected', HabilitacaoEmissaoPTV.container).val(),
			NumeroVistoCrea: $('.txtNumeroVistoCrea', HabilitacaoEmissaoPTV.container).val(),
			NumeroCREA: $('.txtNumeroCREA', HabilitacaoEmissaoPTV.container).val(),
			Profissao: {
				Id: $('.hdnProfissaoId', HabilitacaoEmissaoPTV.container).val(),
				OrgaoClasseId: $('.ddlOrgaoClasse :selected', HabilitacaoEmissaoPTV.container).val(),
				Registro: $('.txtRegistroOrgaoClasse', HabilitacaoEmissaoPTV.container).val()
			},
			Telefones: [],
			Operadores: [],
			Endereco: {
				Cep: $('.txtCep', HabilitacaoEmissaoPTV.container).val(),
				Logradouro: $('.txtLogradouro', HabilitacaoEmissaoPTV.container).val(),
				Bairro: $('.txtBairro', HabilitacaoEmissaoPTV.container).val(),
				EstadoId: $('.ddlEstadoEndereco :selected', HabilitacaoEmissaoPTV.container).val(),
				MunicipioId: $('.ddlMunicipio :selected', HabilitacaoEmissaoPTV.container).val(),
				Numero: $('.txtNumero', HabilitacaoEmissaoPTV.container).val(),
				DistritoLocalizacao: $('.txtDistrito', HabilitacaoEmissaoPTV.container).val(),
				Complemento: $('.txtComplemento', HabilitacaoEmissaoPTV.container).val()
			},
			Arquivo: $('.hdnArquivoJson').val() ? JSON.parse($('.hdnArquivoJson').val()) : ""
		};

		if ($('.txtTelefoneResidencial', HabilitacaoEmissaoPTV.container).val() != '') {
			retorno.Telefones.push({ TipoContatoInteiro: 1, Valor: $('.txtTelefoneResidencial', HabilitacaoEmissaoPTV.container).val() });
		}

		if ($('.txtTelefoneCelular', HabilitacaoEmissaoPTV.container).val() != '') {
			retorno.Telefones.push({ TipoContatoInteiro: 2, Valor: $('.txtTelefoneCelular', HabilitacaoEmissaoPTV.container).val() });
		}

		if ($('.txtTelefoneComercial', HabilitacaoEmissaoPTV.container).val() != '') {
			retorno.Telefones.push({ TipoContatoInteiro: 4, Valor: $('.txtTelefoneComercial', HabilitacaoEmissaoPTV.container).val() });
		}

		$('.gridOperadores tbody tr:not(.templateRow)', HabilitacaoEmissaoPTV.container).each(function () {
			retorno.Operadores.push(JSON.parse($('.hdnOperadorJson', this).val()));
		});

		return retorno;
	},

	salvar: function () {
		Mensagem.limpar(HabilitacaoEmissaoPTV.container);
		MasterPage.carregando(true);
		$.ajax({
			url: HabilitacaoEmissaoPTV.settings.urls.salvar,
			data: JSON.stringify({ habilitacao: HabilitacaoEmissaoPTV.obter() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: Aux.error,
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.Url);
				}

				if (response.Erros && response.Erros.length > 0) {
					Mensagem.gerar(HabilitacaoEmissaoPTV.container, response.Erros);
				}
			}
		});
		MasterPage.carregando(false);
	}
}
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

AtividadeConfiguracao = {
	urlFiltrar: '',
	urlConfiguracaoSalvar: '',
	urlExcluir: '',
	urlVisualizar: '',
	urlObterAtividade: '',
	urlValidarAtividadeConfigurada: '',
	Mensagens: null,
	container: null,

	load: function (container) {
		AtividadeConfiguracao.container = MasterPage.getContent(container);

		container.delegate('.btnBuscarAtividade', 'click', AtividadeConfiguracao.onBuscarAtividade);
		container.delegate('.btnExcluirAtividade', 'click', AtividadeConfiguracao.onExcluirAtividade);
		container.delegate('.btnAssociarItem', 'click', AtividadeConfiguracao.onAddModelo);

		container.delegate('.btnExcluirModelo', 'click', AtividadeConfiguracao.onExcluirModelo);
		container.delegate('.btnSalvar', 'click', AtividadeConfiguracao.onSalvarConfiguracao);
		$('.textNome', container).focus();
	},

	onBuscarAtividade: function () {
		Modal.abrir(AtividadeConfiguracao.urlObterAtividade, null, function (container) {
			AtividadeSolicitadaListar.load(container);
			AtividadeSolicitadaListar.settings.onAssociarCallback = AtividadeConfiguracao.associarAtividade;
		}, Modal.tamanhoModalGrande);
	},

	associarAtividade: function (objeto) {

		var mensagem = new Array();
		var setorDiferente = false;

		$('.tabAtividades tbody tr', AtividadeConfiguracao.container).each(function () {
			if ($('.hdnItemId', this).val() == objeto.Id) {
				mensagem.push(AtividadeConfiguracao.Mensagens.AtividadeJaAssociada);
			}

			if (!setorDiferente && ($('.hdnItemSetorId', this).val() != objeto.SetorId)) {
				setorDiferente = true;
			}
		});

		if (setorDiferente) {
			mensagem.push(AtividadeConfiguracao.Mensagens.AtividadeSetorDiferentes);
		}

		if (mensagem.length > 0) {
			return mensagem;
		}

		$.ajax({
			url: AtividadeConfiguracao.urlValidarAtividadeConfigurada,
			data: { id: objeto.Id },
			cache: false,
			async: false,
			type: 'POST',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					mensagem = response.Msg;
				}
			}
		});

		if (mensagem.length > 0) {
			return mensagem;
		}

		var linha = $('.templateAtividade', AtividadeConfiguracao.container).clone();

		linha.removeClass('templateAtividade');
		linha.find('.hdnItemId').val(objeto.Id);
		linha.find('.hdnItemSetorId').val(objeto.SetorId);
		linha.find('.ItemNome').text(objeto.Nome).attr('title', objeto.Nome);

		$('.tabAtividades > tbody', AtividadeConfiguracao.container).append(linha);
		Listar.atualizarEstiloTable($('.tabAtividades', AtividadeConfiguracao.container));
		MasterPage.redimensionar();

		mensagem[0] = {};
		$.extend(mensagem[0], AtividadeConfiguracao.Mensagens.AtividadeAssociada);
		mensagem[0].Texto = mensagem[0].Texto.replace('#ATIVIDADE#', objeto.Nome);

		return mensagem;
	},

	onExcluirAtividade: function () {
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable($('.tabAtividades'));
		MasterPage.redimensionar();
	},

	onAddModelo: function () {

		var id = $('.ddlModelos :selected').val();
		var mensagem = new Array();

		if (id == 0) {
			mensagem.push(AtividadeConfiguracao.Mensagens.ModeloObrigatorio);
		}

		$('.tabModelos tr', AtividadeConfiguracao.container).each(function () {
			if ($('.hdnItemModeloId', this).val() == id) {
				mensagem.push(AtividadeConfiguracao.Mensagens.ModeloJaAssociado);
			}
		});

		if (mensagem.length > 0) {
			Mensagem.gerar(AtividadeConfiguracao.container, mensagem);
			return;
		}

		var linha = $('.templateModelo', AtividadeConfiguracao.container).clone();

		linha.removeClass('templateModelo');
		linha.find('.hdnItemModeloId').val($('.ddlModelos :selected').val());
		linha.find('.ItemModeloNome').text($('.ddlModelos :selected').text()).attr('title', $('.ddlModelos :selected').text());
		$('.tabModelos > tbody', AtividadeConfiguracao.container).append(linha);
		Listar.atualizarEstiloTable($('.tabModelos', AtividadeConfiguracao.container));
		MasterPage.redimensionar();
	},

	onExcluirModelo: function () {
		$(this).closest('tr').remove();
		Listar.atualizarEstiloTable($('.tabModelos'));
		MasterPage.redimensionar();
	},

	onGerarObjeto: function () {

		var objeto = {
			Id: 0,
			NomeGrupo: '',
			Atividades: [],
			Modelos: []
		};

		objeto.Id = $('.hdnConfiguracaoId', AtividadeConfiguracao.container).val();
		objeto.NomeGrupo = $('.textNome', AtividadeConfiguracao.container).val();

		$('.tabAtividades tbody tr', AtividadeConfiguracao.container).each(function () {
		    objeto.Atividades.push({
		        Id: $('.hdnItemId', this).val(),
		        IdRelacionamento: $('.hdnAtividadeIdRelacionamento', this).val(),
		        Texto: $('.ItemNome', this).text()
		    });
		});

		$('.tabModelos tbody tr', AtividadeConfiguracao.container).each(function () {
			objeto.Modelos.push({ Id: $('.hdnItemModeloId', this).val(), IdRelacionamento: $('.hdnItemIdRelacionamento', this).val() });
		});

		return objeto;
	},

	onSalvarConfiguracao: function () {

		$.ajax({ url: AtividadeConfiguracao.urlConfiguracaoSalvar,
			data: JSON.stringify(AtividadeConfiguracao.onGerarObjeto()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, MasterPage.getContent(AtividadeConfiguracao.container));
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (response.EhValido) {
					MasterPage.redireciona(response.urlRedireciona);
				} else {
					Mensagem.gerar(MasterPage.getContent(AtividadeConfiguracao.container), response.Msg);
				}
			}
		});
	}
}
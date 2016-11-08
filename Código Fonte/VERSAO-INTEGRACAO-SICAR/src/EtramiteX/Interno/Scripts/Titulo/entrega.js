/// <reference path="Lib/JQuery/jquery-1.4.3-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../mensagem.js" />

Entrega = {
	urlIncial: null,
	urlSalvar: null,
	urlGerarPdfTitulo: null,
	urlObterTitulosProcesso: null,

	urlObterDocumentoTipos: null,
	urlObterProcessoTipos: null,
	urlObterTituloNumeroConcatenados: null,
	urlEntregaConfirm: null,

	urlObterPessoaEntrega: null,
	container: null,
	Mensagens: null,

	load: function (container) {

		Entrega.container = MasterPage.getContent(container);

		container.delegate('.btnVerificarProc', 'click', Entrega.onVerificarProcesso);
		container.delegate('.btnLimparProc', 'click', Entrega.onLimparCampoProcesso);

		container.delegate('.btnVerificarCpf', 'click', Entrega.onVerificarPessoa);
		container.delegate('.btnLimparCpf', 'click', Entrega.onLimparCamposPessoa);

		container.delegate('.chkMarcarTodos', 'change', Entrega.onMarcarDesMarcarTodos);
		container.delegate('.chkTitulo', 'change', Entrega.onCkbTituloChange);
		container.delegate('.trItem', 'click', Entrega.onTrClick);

		container.delegate('.btnPdfTitulo', 'click', Entrega.onAbrirPdfTitulo);
		container.delegate('.btnEntregaSalvar', 'click', Entrega.onObterTituloSituacaoAssinado);

		container.delegate('.txtProcessoNumero', 'keyup', function (e) {
			if (e.keyCode == (keyENTER = 13)) $('.btnVerificarProc', Entrega.container).click();
		});

		Aux.setarFoco(Entrega.container);
	},

	onMarcarDesMarcarTodos: function () {
		var valorSetar = $(this).attr('checked');
		$('.tabItens tbody tr', Entrega.container).each(function () {
			$('.chkTitulo', this).attr('checked', valorSetar);
		});
	},

	onTrClick: function (e) {
		if ($(e.target).is('input')) return;
		$('.chkTitulo', this).attr('checked', !$('.chkTitulo', this).is(':checked'));
		Entrega.marcarTodos();
	},

	onCkbTituloChange: function () {
		Entrega.marcarTodos();
	},

	marcarTodos: function () {
		var isAllChk = true;
		$('.tabItens tbody', Entrega.container).find("tr").each(function (idx, item) {
			isAllChk = isAllChk && $('.chkTitulo', item).is(':checked');
		});
		$('.tabItens .chkMarcarTodos', Entrega.container).attr('checked', isAllChk);
	},

	onLimparCamposPessoa: function () {
		Mensagem.limpar(Entrega.container);
		$('.txtNome', Entrega.container).val('');
		$('.txtCpf', Entrega.container).val('');

		$('.txtNome', Entrega.container).attr('disabled', 'disabled');
		$('.txtNome', Entrega.container).addClass('disabled');

		$('.txtCpf', Entrega.container).removeAttr('disabled', 'disabled');
		$('.txtCpf', Entrega.container).removeClass('disabled');

		$('.containerLimpar', Entrega.container).addClass('hide');
		$('.containerVerificar', Entrega.container).removeClass('hide');
	},

	onVerificarPessoa: function () {
		Mensagem.limpar(Entrega.container);

		if ($('.txtCpf', Entrega.container).val() == '') {
			$('.txtNome', Entrega.container).attr('disabled', 'disabled');
			$('.txtNome', Entrega.container).addClass('disabled');
			Mensagem.gerar(Entrega.container, new Array(Entrega.Mensagens.CPFObrigatorio));
			return;
		}

		$.ajax({ url: Entrega.urlObterPessoaEntrega,
			data: JSON.stringify({ cpf: $('.txtCpf', Entrega.container).val() }),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Entrega.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(Entrega.container, response.Msg);
					return;
				}

				$('.txtCpf', Entrega.container).attr('disabled', 'disabled');
				$('.txtCpf', Entrega.container).addClass('disabled');

				$('.containerLimpar', Entrega.container).removeClass('hide');
				$('.containerVerificar', Entrega.container).addClass('hide');

				if (response.Pessoa.Id != 0) {
					$('.hdnPessoaId', Entrega.container).val(response.Pessoa.Id);
					$('.txtNome', Entrega.container).val(response.Pessoa.NomeRazaoSocial);

					$('.txtNome', Entrega.container).attr('disabled', 'disabled');
					$('.txtNome', Entrega.container).addClass('disabled');

					return;
				}

				Mensagem.gerar(Entrega.container, response.Msg);
				$('.txtNome', Entrega.container).removeAttr('disabled', 'disabled');
				$('.txtNome', Entrega.container).removeClass('disabled');
			}
		});
	},

	onVerificarProcesso: function () {
		Mensagem.limpar(Entrega.container);
		MasterPage.carregando(true);
		var objeto = Entrega.gerarObjeto();

		$.ajax(
		{ url: Entrega.urlObterTitulosProcesso,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Entrega.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(Entrega.container, response.Msg);
					return;
				}

				$('.hdnProcDocId', Entrega.container).val(response.ProtocoloId);
				$('.hdnProcDocIsProcesso', Entrega.container).val(response.ProcDocIsProcesso);

				$('.txtProcessoNumero', Entrega.container).attr('disabled', 'disabled');
				$('.txtProcessoNumero', Entrega.container).addClass('disabled');

				$('.containerVerificarProc', Entrega.container).addClass('hide');
				$('.containerLimparProc', Entrega.container).removeClass('hide');
				$('.divConteudoEntrega', Entrega.container).removeClass('hide');

				$(response.Lista).each(function () {
					var titulo = this;

					var linha = $('.trItemTemplate', Entrega.container).clone();

					linha.removeClass('trItemTemplate');

					linha.find('.hdnTituloId').val(titulo.Id);
					linha.find('.tituloNumero').text(titulo.Numero.Texto);
					linha.find('.tituloModelo').text(titulo.Modelo.Nome);
					linha.find('.tituloSituacao').text(titulo.Situacao.Texto);
					linha.addClass(($('.tabItens tbody tr', Entrega.container).length % 2) === 0 ? 'par' : 'impar');
					$('.tabItens > tbody', Entrega.container).append(linha);
				});

				Listar.atualizarEstiloTable($('.tabItens', Entrega.container));
				MasterPage.redimensionar();
			}
		});

		MasterPage.carregando(false);
	},

	onLimparCampoProcesso: function () {
		Mensagem.limpar(Entrega.container);

		$.ajax({ url: Entrega.urlIncial,
			data: null,
			cache: false,
			async: false,
			type: 'GET',
			dataType: 'html',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Entrega.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				Entrega.container.find('.divConteudo').html(response);
				MasterPage.redimensionar();
				MasterPage.botoes(Entrega.container.find('.divConteudo'));
				Mascara.load(Entrega.container.find('.divConteudo'));
				$('.txtProcessoNumero', Entrega.container).focus();
			}
		});
	},

	onAbrirPdfTitulo: function () {
		MasterPage.redireciona(Entrega.urlGerarPdfTitulo + '/' + $(this).closest('tr').find('.hdnTituloId').val());
	},

	onObterTituloSituacaoAssinado: function () {

		$.ajax({ url: Entrega.urlObterTituloNumeroConcatenados,
			data: JSON.stringify(Entrega.gerarObjeto()),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Entrega.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {

				if (!response.EhValido) {
					Mensagem.gerar(Entrega.container, response.Msg);
					return;
				}

				if (response.NumerosConcatenados == '') {
					Entrega.onSalvarEntrega();
				}
				else {
					Modal.confirma({
						btnOkLabel: 'Confirmar',
						url: Entrega.urlEntregaConfirm,
						urlData: { titulos: response.NumerosConcatenados },
						tamanhoModal: Modal.tamanhoModalMedia,
						btnOkCallback: function (modalContent) {
							Modal.fechar(modalContent);
							Entrega.onSalvarEntrega();
						}
					});
				}
			}
		});
	},

	gerarObjeto: function () {
		var objeto = {
			DataEntrega: {},
			Protocolo: {},
			PessoaId: '',
			Nome: '',
			CPF: '',
			Titulos: new Array()
		};

		objeto.DataEntrega.DataTexto = $('.txtDataEntrega', Entrega.container).val();

		objeto.Protocolo.Id = $('.hdnProcDocId', Entrega.container).val();
		objeto.Protocolo.IsProcesso = $('.hdnProcDocIsProcesso', Entrega.container).val();
		objeto.Protocolo.NumeroAutuacao = $('.txtProcessoNumero', Entrega.container).val();
		var numero = $('.txtProcessoNumero', Entrega.container).val().toString().split('/');
		if (numero.length > 0) {
			objeto.Protocolo.NumeroProtocolo = numero[0];
		}
		if (numero.length > 1) {
			objeto.Protocolo.Ano = numero[1];
		}

		if ($('.hdnPessoaId', Entrega.container).val() != 0) {
			objeto.PessoaId = $('.hdnPessoaId', Entrega.container).val();
		}

		objeto.Nome = $('.txtNome', Entrega.container).val();
		objeto.CPF = $('.txtCpf', Entrega.container).val();

		$('.tabItens > tbody > tr', Entrega.container).each(function () {
			var check = $('.chkTitulo', this);
			if ($(check).attr('checked')) {
				objeto.Titulos.push(+$('.hdnTituloId', this).val());
			}
		});

		return { Entrega: objeto };
	},

	onSalvarEntrega: function () {
		Mensagem.limpar(Entrega.container);
		MasterPage.carregando(true);
		var objeto = Entrega.gerarObjeto();

		$.ajax({ url: Entrega.urlSalvar,
			data: JSON.stringify(objeto),
			cache: false,
			async: false,
			type: 'POST',
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, Entrega.container);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.EhValido) {
					MasterPage.redireciona(response.urlRedirecionar);
				} else {
					Mensagem.gerar(Entrega.container, response.Msg);
				}
			}
		});

		MasterPage.carregando(false);
	}
}
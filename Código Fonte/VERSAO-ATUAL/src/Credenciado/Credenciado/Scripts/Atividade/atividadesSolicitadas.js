/// <reference path="../Lib/JQuery/jquery-1.10.1-vsdoc.js" />
/// <reference path="../masterpage.js" />
/// <reference path="../jquery.json-2.2.min.js" />

AtividadesSolicitadas = {

	urlEncerrarAtividade: '',
	urlMotivoEncerrarAtividade: '',
	urlSalvarMotivoEncerrarAtividade: '',
	urlVisualizarEncerrarAtividade: '',
	urlVisualizarPdf: '',
	container: {},

	load: function (container) {
		AtividadesSolicitadas.container = container;
		container.delegate('.btnEncerrarAtividade', 'click', AtividadesSolicitadas.onEncerrarAtividadeClick);
		container.delegate('.btnVisualizarAtividade', 'click', AtividadesSolicitadas.onVisualizarAtividadeClick);
		container.delegate('.btnVisualizarPdf', 'click', function () { AtividadesSolicitadas.onAbrirPdfClick(this, container) });
		AtividadesSolicitadas.container = container;
		$('.divConteudoAtividade').associarMultiplo();
	},

	onVisualizarAtividadeClick: function () {
		var linha = $(this).closest('tr');
		Modal.abrir(
			AtividadesSolicitadas.urlVisualizarEncerrarAtividade,
			{ motivo: linha.find('.hdnAtividadeMotivo').val() },
			function (container) { Modal.defaultButtons(container); },
			Modal.tamanhoModalMedia);
	},

	onEncerrarAtividadeClick: function () {
		var linha = $(this).closest('tr');
		var pai = $('.modalAtividadesSolicitadas', AtividadesSolicitadas.container);

		var dado = {};
		dado["atividade.Id"] = $('.hdnAtividadeId', linha).val();
		dado["atividade.IdRelacionamento"] = $('.hdnAtividadeIdRelacionamento', linha).val();
		dado["atividade.Protocolo.Id"] = $('.hdnAtividadeIdProtocolo', linha).val();
		dado["atividade.Protocolo.IsProcesso"] = $('.hdnAtividadeIsProcesso', linha).val();

		$.ajax({ url: AtividadesSolicitadas.urlEncerrarAtividade, data: dado, type: 'POST', cache: false, async: false,
			error: function (XMLHttpRequest, textStatus, erroThrown) {
				Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
			},
			success: function (response, textStatus, XMLHttpRequest) {
				if (response.IsAtividadePodeEncerrar) {

					var Atividade = {
						Id: $('.hdnAtividadeId', linha).val(),
						IdRelacionamento: $('.hdnAtividadeIdRelacionamento', linha).val(),
						IdProtocolo: $('.hdnAtividadeIdProtocolo', linha).val(),
						IsProcesso: $('.hdnAtividadeIsProcesso', linha).val()
					};

					AtividadesSolicitadas.onConfirmarEncerrarAtividade(Atividade, pai, linha);

				} else if (response.Msg && response.Msg.length > 0) {
					Mensagem.gerar(MasterPage.getContent(pai), response.Msg);
				}
			}
		});
	},

	onConfirmarEncerrarAtividade: function (Atividade, content, linhaTr) {

		var dado = {};
		dado["atividade.Id"] = Atividade.Id;
		dado["atividade.IdRelacionamento"] = Atividade.IdRelacionamento;
		dado["atividade.Protocolo.Id"] = Atividade.IdProtocolo;
		dado["atividade.Protocolo.IsProcesso"] = Atividade.IsProcesso;
		dado["id"] = $('.hdnProtocoloId', content).val();
		dado["isProcesso"] = $('.hdnProtocoloTipo', content).val();

		Modal.confirma({
			btnOkLabel: 'Encerrar',
			url: AtividadesSolicitadas.urlMotivoEncerrarAtividade,
			urlData: dado,
			tamanhoModal: Modal.tamanhoModalMedia,
			btnOkCallback: function (content) {
				$('.hdnAtividadeId', content).val(Atividade.Id);
				$('.hdnAtividadeRelacionamentoId', content).val(Atividade.IdRelacionamento);
				$('.hdnAtividadeProtocoloId', content).val(Atividade.IdProtocolo);
				$('.hdnAtividadeIsProcesso', content).val(Atividade.IsProcesso);
				$('.hdnPaiProtocoloId', content).val(dado["id"]);
				$('.hdnPaiProtocoloIsProcesso', content).val(dado["isProcesso"]);

				var vm = Modal.json(content);

				vm.Motivo = $('.txtMotivo', content).val();

				$.ajax({ url: AtividadesSolicitadas.urlSalvarMotivoEncerrarAtividade, data: vm, type: 'POST', cache: false, async: false,
					error: function (XMLHttpRequest, textStatus, erroThrown) {
						Aux.error(XMLHttpRequest, textStatus, erroThrown, content);
					},
					success: function (response, textStatus, XMLHttpRequest) {
						if (response.IsAtividadeEncerrada) {
							Modal.fechar($('.divEncerrarAtividadeMotivo', content));

							$('.hdnSituacaoId', linhaTr).val(response.Situacao.Id);
							$('.hdnAtividadeMotivo', linhaTr).val(vm.Motivo);
							$('.spSituacaoTexto', linhaTr).html(response.Situacao.Texto);
							$('.btnEncerrarAtividade', linhaTr).addClass('hide');
							$('.spnVisualizarAtividade', linhaTr).removeClass('hide');

							if (response.Msg && response.Msg.length > 0) {
								Mensagem.gerar(MasterPage.getContent(AtividadesSolicitadas.container), response.Msg);
							}
						}
						else if (response.Msg && response.Msg.length > 0) {
							Mensagem.gerar(MasterPage.getContent(content), response.Msg);
						}
					}
				});
			}
		});
	},

	onAbrirPdfClick: function (btnPdf, container) {
		var id = parseInt($(btnPdf).closest('.divRequerimento', container).find('.hdnRequerimentoId').val());
		MasterPage.redireciona(AtividadesSolicitadas.urlVisualizarPdf + "?id=" + id);
		MasterPage.carregando(false);
	}
}
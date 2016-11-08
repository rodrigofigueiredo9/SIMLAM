/// <reference path="../../Lib/JQuery/jquery-1.4.3.min.js" />
/// <reference path="../../Lib/JQuery/jquery.json - 2.2.min.js" />
/// <reference path="../../masterpage.js" />
/// <reference path="../../mensagem.js" />

// Estrutura HTML em: /src/EtramiteX/Interno/Views/Arquivo/Arquivo.ascx


/* EXEMPLO DE USO:

<script type="text/javascript" src="<%= Url.Content("~/Scripts/arquivo.js") %>"></script>
<script type="text/javascript">
	$(function () {
		var container = $('#central');
		var mensagens = <%= Model.Mensagens %>;
		$('.arquivoContianer', container).arquivo({
			urlDownload: '<%: Url.Action("BaixarArquivo", "TesteUpload") %>',
			urlDownloadTemporario: '<%: Url.Action("BaixarArquivoTemporario", "TesteUpload") %>',
			urlEnviarArquivo: '<%: Url.Action("Arquivo", "Arquivo") %>',
			msgNenhumArquivoSelecionado: mensagens.NenhumArquivoSelecionado,
			msgDescricaoObrigatorio: mensagens.DescricaoObrigatorio,
			msgArquivoExistente: mensagens.ArquivoExistente,
			msgArquivoTipoInvalido: mensagens.ArquivoTipoInvalidoJs,
			extPermitidas: ['pdf', 'jpg']
		});
	});
</script>

<fieldset class="block box filtroExpansivoAberto fsArquivos">
	<% Html.RenderPartial("~/Views/Shared/Arquivo.ascx", Model.ArquivoVM); %>
</fieldset>

</div>

<script type="text/javascript">
	// pega arquivos da tabela
	var arquivos = $('.arquivoContianer').arquivo('obterObjeto');
</script>
*/

ComponenteArquivoDefaultSettings = {
	urlUpload: '',
	urlDownload: '',
	urlDownloadTemporario: '',
	msgNenhumArquivoSelecionado: '',
	msgDescricaoObrigatorio: '',
	msgArquivoExistente: '',
	msgArquivoTipoInvalido: '',
	extPermitidas: [] // array vazio = todas. especificar em letras minúsculas sem ponto. ex: ['pdf', 'doc', 'html']
};

(function ($) {
	$.fn.arquivo = function (method) {
		if (methods[method]) {
			return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
		} else if (typeof method === 'object' || !method) {
			return methods.init.apply(this, arguments);
		} else {
			$.error('Erro de sistema: Método "' + method + '" não existe em jQuery.arquivo');
		}
	};

	/******* MÉTODOS GLOBAIS *******/
	var methods = {
		init: function (options) {
			return this.each(function () {
				var settings = {};
				$.extend(settings, ComponenteArquivoDefaultSettings, options);

				var container = $(this);
				atualizarEstiloGrid(container.find('.tabAnexos'));

				container.delegate('.btnDescerLinha', 'click', function () {
					var tr = $(this).closest('tr');
					tr.next().after(tr);
					atualizarEstiloGrid($(this).closest('.tabAnexos'));
				});

				container.delegate('.btnSubirLinha', 'click', function () {
					var tr = $(this).closest('tr');
					tr.prev().before(tr);
					atualizarEstiloGrid($(this).closest('.tabAnexos'));
				});

				// usuário clica no nome do arquivo e inicia o seu download, seja ele temporário ou não.
				container.delegate('.anexoNome', 'click', function () {
					var anexo = eval('(' + $(this).closest('tr').find('.hdnAnexoJson').val() + ')');
					if (anexo.Arquivo.Id > 0) {
						MasterPage.redireciona(settings.urlDownload + '/' + anexo.Arquivo.Id);
					} else {
						MasterPage.redireciona(settings.urlDownloadTemporario + '/?nomeTemporario=' + anexo.Arquivo.TemporarioNome + '&contentType=' + anexo.Arquivo.ContentType);
					}
					return false;
				});

				// excluir
				container.delegate('.btnExcluirLinha', 'click', function () {
					var tabela = $(this).closest('.tabAnexos');
					$(this).closest('tr').remove();
					if (tabela.find('tbody tr:visible').size() <= 0) {
						tabela.find('.trSemItens').removeClass('hide');
					}
					atualizarEstiloGrid(tabela);
				});

				// enviar
				container.delegate('.btnEnviar', 'click', function () {
					var inputFile = container.find('.inputFile');
					var nomeArquivo = container.find('.inputFile').val();
					var descricao = container.find('.txtAnexoDescricao').val();
					var tabela = container.find('.tabAnexos');

					if (!nomeArquivo.trim()) {
						return Mensagem.gerar(MasterPage.getContent(container), [settings.msgNenhumArquivoSelecionado]);
					}

					if (!descricao.trim()) {
						return Mensagem.gerar(MasterPage.getContent(container), [settings.msgDescricaoObrigatorio]);
					}

					if (arquivoExisteNaLista(nomeArquivo, tabela)) {
						return Mensagem.gerar(MasterPage.getContent(container), [settings.msgArquivoExistente]);
					}

					if (typeof settings.extPermitidas == 'object' && settings.extPermitidas.length > 0 &&
						$.inArray(nomeArquivo.trim().toLowerCase().split('.').pop(), settings.extPermitidas) < 0) {
						var msg = Mensagem.replace(settings.msgArquivoTipoInvalido, '#arquivo#', nomeArquivo);
						msg.Texto += settings.extPermitidas.toString() + '.';
						return Mensagem.gerar(MasterPage.getContent(container), [msg]);
					}

					var botao = $(this).addClass('hide');
					FileUpload.upload(settings.urlUpload, inputFile, function (controle, retorno, isHtml) {
						// exemplo retorno: {"Msg":[{"Tipo":2,"Campo":null,"Texto":"Arquivo CorelDRAW Object Model Diagram.pdf enviado com sucesso."}],
						// "Arquivo":{"Id":0,"Raiz":0,"Nome":"CorelDRAW Object Model Diagram.pdf","Extensao":".pdf","Caminho":null,"Diretorio":null,"TemporarioNome":"ytsvddq2.td0","TemporarioPathNome":"","ContentType":"application/pdf","ContentLength":0,"Tid":null,"Buffer":null,"Apagar":0}}

						var ret = eval('(' + retorno + ')');

						if (ret && ret.Arquivo) {
							var anexo = {
								Id: 0,
								Tid: '',
								Ordem: 0,
								Descricao: descricao,
								Arquivo: ret.Arquivo
							};
							var linha = tabela.find('.trTemplate').clone().removeClass('trTemplate hide');
							linha.find('.anexoNome').attr('title', ret.Arquivo.Nome).text(ret.Arquivo.Nome);
							linha.find('.anexoDescricao').attr('title', descricao).text(descricao);
							linha.find('.hdnAnexoJson').val(JSON.stringify(anexo));
							tabela.find('tbody:last').append(linha);
							tabela.find('.trSemItens').addClass('hide');

							atualizarEstiloGrid(tabela);
						}

						botao.removeClass('hide');
						container.find('.inputFile').val('');
						container.find('.txtAnexoDescricao').val('');

						if (ret && ret.Msg) {
							Mensagem.gerar(MasterPage.getContent(container), ret.Msg);
						}
					});
				});
			});
		},

		obterObjeto: function () {
			var anexos = [];
			var ordem = 0;
			$(this).find('.tabAnexos tr:visible').each(function () {
				var anexo = eval('(' + $(this).find('.hdnAnexoJson').val() + ')');
				if (anexo) {
					anexo.Ordem = ordem;
					anexos.push(anexo);
					ordem++;
				}
			});
			return anexos;
		}
	};

	var arquivoExisteNaLista = function (nomeArquivo, tab) {
		var existe = false;
		var trs = $(tab).find('tbody tr:visible');
		$.each(trs, function (key, trElem) {
			var anexo = eval('(' + $(trElem).find('.hdnAnexoJson').val() + ')');
			if (anexo) existe = (anexo.Arquivo.Nome.toLowerCase().trim() === nomeArquivo.toLowerCase().trim());
			if (existe) return false;
		});
		return existe;
	};

	var atualizarEstiloGrid = function (table) {
		Listar.atualizarEstiloTable(table);

		var rows = $('tbody tr:visible', table).removeClass('selecionado');
		rows.each(function (index, elem) {
			var btnDescer = $(elem).find('.btnDescerLinhaTab,.btnDescerLinha');
			var btnSubir = $(elem).find('.btnSubirLinhaTab,.btnSubirLinha');

			if (index == 0) {
				btnSubir.addClass('desativado');
			} else {
				btnSubir.removeClass('desativado');
			}

			if (index >= rows.length - 1) {
				btnDescer.addClass('desativado');
			} else {
				btnDescer.removeClass('desativado');
			}
		});
	};
})(jQuery);
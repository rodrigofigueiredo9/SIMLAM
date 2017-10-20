<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Etx.ModuloArquivo" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ArquivoVM>" %>

<script>
	var mensagensArquivo = <%= Model.Mensagens %>;
	ComponenteArquivoDefaultSettings = {
		urlDownload: '<%: Url.Action("Baixar", "Arquivo", new {Area ="" }) %>',
		urlDownloadTemporario: '<%: Url.Action("BaixarTemporario", "Arquivo", new {Area ="" }) %>',
		urlUpload: '<%: Url.Action("Arquivo", "Arquivo", new {Area ="" }) %>',
		msgNenhumArquivoSelecionado: mensagensArquivo.ArquivoNenhumSelecionado,
		msgDescricaoObrigatorio: mensagensArquivo.ArquivoDescricaoObrigatorio,
		msgArquivoExistente: mensagensArquivo.ArquivoExistente,
		msgArquivoTipoInvalido: mensagensArquivo.ArquivoTipoInvalidoJs,
		extPermitidas: [] // array vazio = todas. especificar em letras minúsculas sem ponto. ex: ['pdf', 'doc', 'html']
	};
</script>

<% if (!Model.IsVisualizar) { %>
<div class="block">
	<div class="coluna60">
		<label>Arquivo *</label>
		<input type="hidden" class="hdnAnexoJson" name="hdnAnexoJson" />
		<input type="file" class="inputFile" style="display: block; width: 100%" name="file" />
	</div>
</div>

<div class="block">
	<div class="coluna60">
		<label for="Descricao">Descrição *</label>
		<%= Html.TextBox("__AnexoDescricao__", null, new { @maxlength = "100", @class = "text txtAnexoDescricao" })%>
	</div>
	<div class="coluna20 botoesAnexoDiv">
		<button type="button" class="inlineBotao btnEnviar botaoAdicionarIcone" title="Adicionar anexo">Adicionar</button>
	</div>
</div>
<% } %>

<div class="block dataGrid">
	<table class="dataGridTable tabAnexos" width="100%" border="0" cellspacing="0" cellpadding="0">
		<thead>
			<tr>
                <th>Foto Nº</th>
				<th>Arquivo</th>
				<th>Descrição</th>
				<% if (!Model.IsVisualizar) { %><th width="15%">Ações</th><% } %>
			</tr>
		</thead>
		<tbody>
			<%
			JavaScriptSerializer _jsSerializer = new JavaScriptSerializer();
            int nFoto = 0;
			foreach (Anexo anexo in Model.Anexos) { %>
			<tr>
                <td>
                    <span title="Foto <%=++nFoto%>" class="nFoto"><%=nFoto%></span>
                </td>

				<td>
					<span class="anexoNome" title="<%: anexo.Arquivo.Nome %>">
						<a href="#" class="downloadLink"><%: anexo.Arquivo.Nome %></a>
						<input type="hidden" class="hdnAnexoJson" value="<%: _jsSerializer.Serialize(anexo) %>" />
					</span>
				</td>
				<td>
					<span title="<%= Html.Encode(anexo.Descricao) %>" class="AnexoDescricao"><%= Html.Encode(anexo.Descricao) %></span>
				</td>
				<% if (!Model.IsVisualizar) { %>
				<td>
					<% if (Model.UsarOrdenacao) { %>
						<button title="Descer" class="icone abaixo btnDescerLinha" type="button"></button>
						<button title="Subir" class="icone acima btnSubirLinha" type="button"></button>
					<% } %>
					<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
				</td>
				<% } %>
			</tr>
			<% } %>

			<!-- LINHA MOSTRADA QUANDO NÃO HÁ ANEXOS -->
			<tr class="trSemItens <%= Model.Anexos.Count > 0 ? "hide" : "" %>">
				<td colspan="<%= (Model.IsVisualizar ? "3" : "4") %>">
					<label>Não existe arquivo adicionado.</label>
				</td>
			</tr>

			<!-- LINHA DE TEMPLATE -->
			<tr class="trTemplate hide">
                <td>
                    <span title="" class="nFoto"></span>
                </td>
				<td>
					<a class="anexoNome" href="" title=""></a>
				</td>
				<td>
					<span title="" class="anexoDescricao"></span>
				</td>
				<td>
					<% if (!Model.IsVisualizar) { %>
					<% if (Model.UsarOrdenacao) { %>
						<button title="Descer" class="icone abaixo btnDescerLinha <%= Model.UsarOrdenacao ? "" : "hide"%>" type="button"></button>
						<button title="Subir" class="icone acima btnSubirLinha <%= Model.UsarOrdenacao ? "" : "hide"%>" type="button"></button>
					<% } %>
					<button title="Excluir" class="icone excluir btnExcluirLinha" value="" type="button"></button>
					<% } %>
					<input type="hidden" class="hdnAnexoJson" value="" />
				</td>
			</tr>
		</tbody>
	</table>
</div>
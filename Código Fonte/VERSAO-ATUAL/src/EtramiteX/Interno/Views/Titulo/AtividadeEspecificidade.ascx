<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AtividadeEspecificidadeVM>" %>

<script>
	AtividadeEspecificidade.settings.urls.obterProcessosDocumentos = '<%= Url.Action("ObterProcessosDocumentos", "Titulo", new {area=""}) %>';
	AtividadeEspecificidade.settings.urls.obterAtividades = '<%= Url.Action("ObterAtividades", "Titulo", new {area=""}) %>';
	AtividadeEspecificidade.settings.urls.pdfRequerimento = '<%= Url.Action("GerarPdf", "Requerimento", new {area=""}) %>';
	AtividadeEspecificidade.settings.Mensagens = <%= Model.Mensagens %>;
	AtividadeEspecificidade.TextoDropDownDefault = '<%= ViewModelHelper.SelecionePadrao.Text %>';
</script>

<div class="divAtividadeEspecificidade">
	<div class="block">
		<div class="coluna75">
			<label for="ProcessosDocumentos">Requerimento padrão *</label>
			<% if (Model.IsVisualizar) { %>
				<%= Html.DropDownList("ProcessosDocumentos", Model.ProcessosDocumentosLst, new { @class = "disabled text ddlProcessosDocumentos", @disabled = "disabled" })%>
			<% } else { %>
				<%= Html.DropDownList("ProcessosDocumentos", Model.ProcessosDocumentosLst, new { @class = "text ddlProcessosDocumentos" })%>
			<% } %>
		</div>
		<div class="coluna10">
			<button class="icone pdf inlineBotao btnPdfRequerimento" title="PDF do requerimento">PDF do requerimento</button>
		</div>
	</div>

	<% if(Model.Especificidade == Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.eEspecificidade.AutorizacaoExploracaoFlorestal){ %>
	<div class="block">
		<div class="coluna75">
			<label>Destinatário *</label>
			<%: Html.DropDownList("Autorizacao.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count <= 1, new { @class = "text  ddlDestinatarios" }))%>
		</div>
	</div>
	<%} %>

	<% if (!Model.IsVisualizar) { %>
			<div class="block">
				<div class="coluna75">
					<label for="Atividades">Atividade solicitada *</label>
					<% if (Model.AtividadeLst.Count > 1) { %>
						<%= Html.DropDownList("AtividadeSolicitada", Model.AtividadeLst, new { @class = "text ddlAtividadeSolicitada" })%>
					<% } else { %>
						<%= Html.DropDownList("AtividadeSolicitada", Model.AtividadeLst, new { @class = "text ddlAtividadeSolicitada disabled", @disabled = "disabled" })%>
					<% } %>
				</div>

				<% if (Model.MostrarBotoes) { %>
					<div class="mostrarBotoes divBtnAdicionarAtividade col7">
						<button type="button" class="inlineBotao btnAdicionarAtividade">Adicionar</button>
					</div>
				<% } %>
			</div>
	<% } %>

	<% if (Model.IsVisualizar && !Model.MostrarBotoes) { %>
		<div class="block">
			<div class="coluna75">
				<label for="Atividades">Atividade solicitada *</label>
				<%= Html.DropDownList("AtividadeSolicitada", Model.AtividadeLst, new { @class = "text ddlAtividadeSolicitada disabled", @disabled = "disabled" })%>
			</div>
		</div>
	<% } %>

	<div class="block <%= Model.MostrarBotoes ? "" : "hide" %>">
		<div class="dataGrid">
			<table class="dataGridTable ordenavel dgAtividadesSolicitadas" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th class="semOrdenacao">Nome</th>
						<% if (!Model.IsVisualizar) { %><th class="semOrdenacao" width="10%">Ações</th><% } %>
					</tr>
				</thead>
				<tbody>
					<% foreach (var atividade in Model.Atividades) { %>
					<tr class="trAtividade">
						<td class="tdNome" title="<%: atividade.NomeAtividade %>"><%: atividade.NomeAtividade %></td>
						<% if (!Model.IsVisualizar) { %>
						<td class="tdAcoes">
							<input type="hidden" class="hdnAtividadeId" value="<%: atividade.Id %>" />
							<button type="button" title="Excluir" class="icone excluir mostrarBotoes btnExcluirAtividade"></button>
						</td>
						<% } %>
					</tr>
					<% } %>
				</tbody>
			</table>

			<table style="display: none; visibility: hidden;">
				<tr class="trAtividadeTemplate">
					<td class="tdNome" title="#NOME">#NOME</td>
					<td class="tdAcoes">
						<input type="hidden" class="hdnAtividadeId" value="#ITEMID" />
						<button type="button" title="Excluir" class="icone excluir mostrarBotoes btnExcluirAtividade"></button>
					</td>
				</tr>
			</table>
		</div>
	</div>
</div>
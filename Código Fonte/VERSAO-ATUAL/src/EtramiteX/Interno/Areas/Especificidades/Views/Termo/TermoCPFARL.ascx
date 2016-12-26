<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Termo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TermoCPFARLVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Termo/termoCPFARL.js") %>"></script>
<script>
	TermoCPFARL.settings.urls.urlObterDadosTermoCPFARL = '<%= Url.Action("ObterDadosTermoCPFARL", "Termo", new {area="Especificidades"}) %>';
	TermoCPFARL.settings.Mensagens = <%= Model.Mensagens %>;
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>

	<div class="divDestinatarios">
		<%if(!Model.IsVisualizar) {%>
		<div class="block">
			<div class="coluna70 append2">
				<label >Destinatários *</label>
				<%= Html.DropDownList("Termo.Destinatario", Model.Destinatarios, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.Destinatarios.Count == 1, new { @class = "text ddlDestinatarios" }))%>
			</div>

			<div class="coluna10">
				<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarDestinatario btnAddItem" title="Adicionar">+</button>
			</div>
		</div>
		<%} %>

		<div class="block dataGrid">
			<div class="coluna70">
				<table class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="91%">Destinatários</th>
							<%if (!Model.IsVisualizar){%><th width="9%">Ação</th><%} %>
						</tr>
					</thead>
					<% foreach (var item in Model.Termo.Destinatarios){ %>
					<tbody>
						<tr>
							<td>
								<span class="Destinatario" title="<%:item.Nome%>"><%:item.Nome%></span>
							</td>
							<%if (!Model.IsVisualizar){%>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
									<input title="Excluir" type="button" class="icone excluir btnExcluirDestinatario" value="" />
								</td>
							<%} %>
						</tr>
						<% } %>
						<% if(!Model.IsVisualizar) { %>
							<tr class="trTemplateRow hide">
								<td><span class="Destinatario"></span></td>
								<td class="tdAcoes">
									<input type="hidden" class="hdnItemJSon" value="" />
									<input title="Excluir" type="button" class="icone excluir btnExcluirDestinatario" value="" />
								</td>
							</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
	</div>

</fieldset>

<% if (Model.IsCondicionantes){ %>
<fieldset class="block box condicionantesContainer">
	<legend>Condicionantes * </legend>
	<% Html.RenderPartial("~/Views/Titulo/TituloCondicionante.ascx", Model.Condicionantes); %>
</fieldset>
<% } %>

<fieldset class="block box filtroExpansivoAberto fsArquivos">
	<legend>Arquivo</legend>
	<% Html.RenderPartial("~/Views/Arquivo/Arquivo.ascx", Model.ArquivoVM); %>
</fieldset>

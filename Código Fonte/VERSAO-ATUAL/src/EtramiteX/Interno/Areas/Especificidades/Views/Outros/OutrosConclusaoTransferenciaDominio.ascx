<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Outros" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OutrosConclusaoTransferenciaDominioVM>" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script src="<%= Url.Content("~/Scripts/Areas/Especificidades/Outros/outrosConclusaoTransferenciaDominio.js") %>"></script>
<script>
	OutrosConclusaoTransferenciaDominio.settings.urls.carregarListas = '<%= Url.Action("CarregarListasPessoas", "Outros") %>';
	OutrosConclusaoTransferenciaDominio.settings.mensagens.Destinatarios = <%= Model.MensagensDestinatarios %>;
	OutrosConclusaoTransferenciaDominio.settings.mensagens.Interessados = <%= Model.MensagensInteressados %>;
	OutrosConclusaoTransferenciaDominio.settings.mensagens.Responsaveis = <%= Model.MensagensResponsaveisEmpreendimento%>;
</script>

<fieldset class="block box">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>

	<div class="divLista block">
		<div class="coluna70 append2 ">
			<label>Destinatários do título *</label>
			<%= Html.DropDownList("ddlDestinatarios", Model.DestinatariosLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.DestinatariosLst.Count == 1, new { @class = "ddl text ddlDestinatarios" + (Model.IsVisualizar? " hide":"")  }))%>
		</div>
		<input type="hidden" class="hdnMsg" value='<%: Model.MensagensDestinatarios%>' />
		<div class="coluna10  <%=Model.IsVisualizar? "hide": "" %>">
			<button type="button" style="width: 35px" class="inlineBotao botaoAdicionarIcone btnAdd" title="Adicionar">+</button>
		</div>
		<div class="dataGrid coluna82">
			<table class="dataGridTable dgDestinatarios" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th>Nome</th>
						<%if (!Model.IsVisualizar){%>
							<th width="9%">Ação</th>
						<%} %>
					</tr>
				</thead>
				<% foreach (PessoaEspecificidade destinatario in Model.Outros.Destinatarios){ %>
				<tbody>
					<tr>
						<td>
							<span class="Pessoa" title="<%: destinatario.Nome %>"><%: destinatario.Nome %></span>
						</td>
						<%if (!Model.IsVisualizar){%>
						<td class="tdAcoes">
							<input type="hidden" class="hdnId" value='<%: destinatario.Id %>' />
							<input type="hidden" class="hdnJSon" value='<%: ViewModelHelper.Json(destinatario)%>' />
							<input title="Excluir" type="button" class="icone excluir btnExcluirDestinatario" value="" />
						</td>
						<%} %>
					</tr>
					<% } %>
					<% if (!Model.IsVisualizar){ %>
					<tr class="trTemplateRow hide">
						<td><span class="Pessoa"></span></td>
						<td class="tdAcoes">
							<input type="hidden" class="hdnId" value="" />
							<input type="hidden" class="hdnJSon" value="" />
							<input title="Excluir" type="button" class="icone excluir btnExcluirDestinatario" value="" />
						</td>
					</tr>
					<% } %>
				</tbody>
			</table>
		</div>
	</div>
    <br />
	<div class="divLista block">
		<div class="coluna70 append2">
			<label>Responsáveis do empreendimento *</label>
			<%= Html.DropDownList("ddlResponsaveis", Model.ResponsaveisLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.ResponsaveisLst.Count == 1, new { @class = "ddl text ddlResponsaveis" + (Model.IsVisualizar? " hide":"")}))%>
		</div>
		<input type="hidden" class="hdnMsg" value='<%: Model.MensagensResponsaveisEmpreendimento%>' />
		<div class="coluna10 <%=Model.IsVisualizar? "hide": "" %>">
			<button type="button" style="width: 35px" class="inlineBotao botaoAdicionarIcone btnAdd" title="Adicionar">+</button>
		</div>
		<div class="dataGrid coluna82">
			<table class="dgResponsaveis dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th>Nome</th>
						<%if (!Model.IsVisualizar){%>
						<th width="9%">Ação</th>
						<%} %>
					</tr>
				</thead>
				<% foreach (PessoaEspecificidade destinatario in Model.Outros.Responsaveis)  { %>
				<tbody>
					<tr>
						<td>
							<span class="Pessoa" title="<%: destinatario.Nome %>"><%: destinatario.Nome %></span>
						</td>
						<%if (!Model.IsVisualizar){%>
						<td class="tdAcoes">
							<input type="hidden" class="hdnId" value='<%: destinatario.Id %>' />
							<input type="hidden" class="hdnJSon" value='<%: ViewModelHelper.Json(destinatario)%>' />
							<input title="Excluir" type="button" class="icone excluir btnExcluirDestinatario" value="" />
						</td>
						<%} %>
					</tr>
					<% } %>
					<% if (!Model.IsVisualizar){ %>
					<tr class="trTemplateRow hide">
						<td><span class="Pessoa"></span></td>
						<td class="tdAcoes">
							<input type="hidden" class="hdnId" value="" />
							<input type="hidden" class="hdnJSon" value="" />
							<input title="Excluir" type="button" class="icone excluir btnExcluirDestinatario" value="" />
						</td>
					</tr>
					<% } %>
				</tbody>
			</table>
		</div>
	</div>
    <br />
	<div class="divLista block">
		<div class="coluna70 append2">
			<label>Interessados *</label>
            <label style="color:#999; font-style:italic;">(A quem será transferido o domínio do imóvel)</label>
			<%= Html.DropDownList("ddlInteressados", Model.InteressadosLst, ViewModelHelper.SetaDisabled(Model.IsVisualizar || Model.InteressadosLst.Count == 1, new { @class = "ddl text ddlInteressados" + (Model.IsVisualizar? " hide":"")}))%>
		</div>
		<input type="hidden" class="hdnMsg" value='<%: Model.MensagensInteressados%>' />
		<div class="coluna10  <%=Model.IsVisualizar? "hide": "" %>">
			<button type="button" style="width: 35px" class="inlineBotao botaoAdicionarIcone btnAdd" title="Adicionar">+</button>
		</div>
		<div class="dataGrid coluna82">
			<table class="dgInteressados dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
				<thead>
					<tr>
						<th>Nome</th>
						<%if (!Model.IsVisualizar){%>
						<th width="9%">Ação</th>
						<%} %>
					</tr>
				</thead>
				<% foreach (PessoaEspecificidade destinatario in Model.Outros.Interessados)	   { %>
				<tbody>
					<tr>
						<td>
							<span class="Pessoa" title="<%: destinatario.Nome %>"><%: destinatario.Nome %></span>
						</td>
						<%if (!Model.IsVisualizar){%>
						<td class="tdAcoes">
							<input type="hidden" class="hdnId" value='<%: destinatario.Id %>' />
							<input type="hidden" class="hdnJSon" value='<%: ViewModelHelper.Json(destinatario)%>' />
							<input title="Excluir" type="button" class="icone excluir btnExcluirDestinatario" value="" />
						</td>
						<%} %>
					</tr>
					<% } %>
					<% if (!Model.IsVisualizar){ %>
					<tr class="trTemplateRow hide">
						<td><span class="Pessoa"></span></td>
						<td class="tdAcoes">
							<input type="hidden" class="hdnId" value="" />
							<input type="hidden" class="hdnJSon" value="" />
							<input title="Excluir" type="button" class="icone excluir btnExcluirDestinatario" value="" />
						</td>
					</tr>
					<% } %>
				</tbody>
			</table>
		</div>
	</div>
</fieldset>

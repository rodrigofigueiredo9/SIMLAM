<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Termo" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AberturaLivroUnidadeConsolidacaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Especificidades/Termo/aberturaLivroUnidadeConsolidacao.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Titulo/listar.js") %>"></script>

<script type="text/javascript">
	AberturaLivroUnidadeConsolidacao.settings.urls.obterDadosAberturaLivroUnidadeConsolidacao = '<%= Url.Action("ObterDadosAberturaLivroUnidadeConsolidacao", "Termo", new {area="Especificidades"}) %>';
	AberturaLivroUnidadeConsolidacao.settings.Mensagens = <%= Model.Mensagens %>;
</script>

<fieldset class="block box fdsEspecificidade">
	<legend>Especificidade</legend>
	<% Html.RenderPartial("~/Views/Titulo/AtividadeEspecificidade.ascx", Model.Atividades); %>

	<div class="block">
		<div class="coluna75">
			<div class="coluna33 append2">
				<label for="Termo_TotalPaginasLivro">Total de páginas do livro *</label>
				<%= Html.TextBox("Termo.TotalPaginasLivro", Model.Termo.TotalPaginasLivro, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTotalPaginasLivro", @maxlength="5" }))%>
			</div>

			<div class="coluna30 append2">
				<label for="Termo_PaginaInicial">Nº de página inicial *</label>
				<%= Html.TextBox("Termo.PaginaInicial", Model.Termo.PaginaInicial, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtPaginaInicial", @maxlength="5" }))%>
			</div>

			<div class="coluna30">
				<label for="Termo_PaginaFinal">Nº de página final *</label>
				<%= Html.TextBox("Termo.PaginaFinal", Model.Termo.PaginaFinal, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtPaginaFinal", @maxlength="5" }))%>
			</div>
		</div>
	</div>

	<div class="block">
		<div class="coluna75">
			<label for="Termo_CulturaCultId">Cultura *</label>
			<%= Html.DropDownList("Termo.CulturaCultId", Model.Culturas, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlCultura" }))%>
		</div>
		<div class="coluna10">
			<button class="inlineBotao btnAdicionarCultura" type="button" value="Adicionar"><span>Adicionar</span></button>
		</div>
	</div>
	
	<div class="block">
		<div class="coluna75">
			<table class="dataGridTable gridCulturas" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Cultura</th>
						<%if(!Model.IsVisualizar){ %><th width="9%">Ações</th><%} %>
					</tr>
				</thead>
				<tbody>
					<%foreach (var item in Model.Termo.Culturas)
					{%>
					<tr>
						<td>
							<label class="lblNome" title="<%=item.Nome %>"><%=item.Nome %> </label>
						</td>
						<%if(!Model.IsVisualizar){ %>
						<td>
							<a class="icone excluir btnExcluirItem"></a>
							<input type="hidden" value="<%=item.Id %>" class="hdnItemId" />
							<input type="hidden" value="<%=item.IdRelacionamento%>" class="hdnRelacionamentoId" />
						</td>
						<%} %>
					</tr>
					<% } %>
					<tr class="trTemplate hide">
						<td>
							<label class="lblNome" title=""></label>
						</td>
						<td>
							<a class="icone excluir btnExcluirItem"></a>
							<input type="hidden" value="0" class="hdnItemId" />
							<input type="hidden" value="0" class="hdnRelacionamentoId" />
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</div>
</fieldset>
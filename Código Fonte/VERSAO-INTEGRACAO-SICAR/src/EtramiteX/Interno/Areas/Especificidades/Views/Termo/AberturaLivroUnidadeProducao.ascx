<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Termo" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AberturaLivroUnidadeProducaoVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<script type="text/javascript" src="<%= Url.Content("~/Scripts/Areas/Especificidades/Termo/aberturaLivroUnidadeProducao.js") %>"></script>
<script type="text/javascript" src="<%= Url.Content("~/Scripts/Titulo/listar.js") %>"></script>

<script type="text/javascript">
	AberturaLivroUnidadeProducao.settings.urls.obterDadosAberturaLivroUnidadeProducao = '<%= Url.Action("ObterDadosAberturaLivroUnidadeProducao", "Termo", new {area="Especificidades"}) %>';
	AberturaLivroUnidadeProducao.settings.urls.obterUnidadeProducaoItem = '<%= Url.Action("ObterUnidadeProducaoItem", "Termo", new {area="Especificidades"}) %>';
	AberturaLivroUnidadeProducao.settings.Mensagens = <%= Model.Mensagens %>;
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
		<% if (!Model.IsVisualizar) { %>
		<div class="block">
			<div class="coluna70 append2">
				<label for="Termo_UnidadeProducaoUnidadeId">Unidade de Produção *</label>
				<%= Html.DropDownList("Termo.UnidadeProducaoUnidadeId", Model.UnidadesProducoes, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text ddlUnidadeProducao" }))%>
			</div>

			<div class="coluna10">
				<button type="button" style="width: 35px" class="inlineBotao botaoAdicionarIcone btnAddItem btnAddUnidade" title="Adicionar">+</button>
			</div>
		</div>
		<% } %>

		<div class="block dataGrid">
			<div class="coluna70">
				<table class="dataGridTable tbUnidade" width="100%" border="0" cellspacing="0" cellpadding="0" rules="all">
					<thead>
						<tr>
							<th width="20%">Unidade de produção</th>
							<th>Cultura/Cultivar</th>
							<th width="20%">Quantidade/Ano</th>
							<% if (!Model.IsVisualizar) { %>
							<th width="9%">Ação</th>
							<% } %>
						</tr>
					</thead>
					<% foreach (var item in Model.Termo.Unidades) { %>
					<tbody>
						<tr>
							<td><span class="UnidadeProducao" title="<%: item.CodigoUP %>"><%: item.CodigoUP %></span></td>
							<td><span class="CulturaCultivar" title="<%: item.CulturaTexto + ' ' + item.CultivarTexto %>"><%: item.CulturaTexto + ' ' + item.CultivarTexto %></span></td>
							<td><span class="QuantidadeAno" title="<%: item.EstimativaProducaoQuantidadeAno.ToStringTrunc(4, false) %>"><%: item.EstimativaProducaoQuantidadeAno.ToStringTrunc(4, false) %></span></td>
							<%if (!Model.IsVisualizar){%>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value='<%: ViewModelHelper.Json(item)%>' />
								<input title="Excluir" type="button" class="icone excluir btnExcluir" value="" />
							</td>
							<%} %>
						</tr>
						<% } %>
						<% if (!Model.IsVisualizar) { %>
						<tr class="trTemplateRow hide">
							<td><span class="UnidadeProducao"></span></td>
							<td><span class="CulturaCultivar"></span></td>
							<td><span class="QuantidadeAno"></span></td>
							<td class="tdAcoes">
								<input type="hidden" class="hdnItemJSon" value="" />
								<input title="Excluir" type="button" class="icone excluir btnExcluir" value="" />
							</td>
						</tr>
						<% } %>
					</tbody>
				</table>
			</div>
		</div>
	</div>
</fieldset>
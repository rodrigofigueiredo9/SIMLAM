<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PersonalizadoVM>" %>

<div class="block margemDTop borderB padding0">
	<div class="coluna25">
		<p>
			<label for="Agrupador">Dimensão</label>
			<%= Html.DropDownList("Agrupador", Model.DimensoesLst, new { @class = "text setarFoco ddlAgrupador" })%>
		</p>
	</div>
</div>
<!-- ========================================================================= -->

<h5>Odenar Valores das Colunas</h5>

<div class="block margem0">
	<div class="coluna50 colunaA">
		<strong>Coluna A</strong>
		<ul class="sortable agrupador0">
			<% foreach (var campo in Model.ConfiguracaoRelatorio.FonteDados.CamposFatoExibicao.Where(x => x.CampoOrdenacao && !Model.ConfiguracaoRelatorio.Ordenacoes.Exists(y => y.Campo.Id == x.Id))) { %>
			<li>
				<input type="hidden" class="hdnCampoId" value="<%= campo.Id %>" />
				<span class="campoExibicao"><%: Model.ConfiguracaoRelatorio.FonteDados.Nome + " - " + campo.Alias %></span>
				<span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span>
			</li>
			<% } %>
		</ul>

		<% int agrupador = 0; %>
		<% foreach (var dimensao in Model.ConfiguracaoRelatorio.FonteDados.Dimensoes) { %>
		<% agrupador++; %>
			<ul class="sortable hide agrupador<%= agrupador.ToString() %>">
				<% foreach (var campo in dimensao.CamposExibicao.Where(x => x.CampoOrdenacao && !Model.ConfiguracaoRelatorio.Ordenacoes.Exists(y => y.Campo.Id == x.Id))) { %>
				<li>
					<input type="hidden" class="hdnCampoId" value="<%= campo.Id %>" />
					<span class="campoExibicao"><%: dimensao.Nome + " - " + campo.Alias %></span>
					<span class="alfaOrder" title="Altere a Classificação Alfabética">A > Z</span>
				</li>
				<% } %>
			</ul>
		<% } %>
	</div>

	<div class="coluna48 ultima colunaB">
		<strong>Coluna B</strong>
		<% bool semItens = (Model.ConfiguracaoRelatorio.Ordenacoes == null || Model.ConfiguracaoRelatorio.Ordenacoes.Count <= 0); %>
		<ul class="sortable <%= semItens ? "box dashed" : "" %>">
			<% foreach (var item in Model.ConfiguracaoRelatorio.Ordenacoes) { %>
			<li>
				<input type="hidden" class="hdnCampoId" value="<%= item.Campo.Id %>" />
				<span class="campoExibicao"><%: item.Campo.DimensaoNome + " - " + item.Campo.Alias %></span>
				<span class="alfaOrder <%= item.Crescente ? "" : "ativo" %>" title="Altere a Classificação Alfabética">A > Z</span>
			</li>
			<% } %>
			<% if(semItens) { %>
				<span class="quiet">Arraste os item aqui.</span>
			<% } %>
		</ul>
	</div>
</div>
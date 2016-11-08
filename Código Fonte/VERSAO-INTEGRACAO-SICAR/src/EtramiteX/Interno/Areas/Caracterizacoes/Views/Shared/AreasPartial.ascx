<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AreasVM>" %>

<% if(Model.IsVisualizar) { %>
	<fieldset class="block box filtroExpansivoAberto fsAreas">
		<legend class="titFiltros">Áreas</legend>
			<div class="block filtroCorpo">
				<div class="block">
					<% foreach (var item in Model.Areas) { %>
						<div class="coluna30 append2 divArea">
							<label for="<%= "Area" + item.Tipo %>"><%= item.TipoTexto + " *" %></label>
							<%= Html.TextBox("Area" + item.Tipo, item.Valor, new { @class = "text disabled", @disabled = "disabled" })%>
						</div>
					<% } %>
				</div>
			</div>
	</fieldset>
<% } else { %>
	<fieldset class="block box filtroExpansivoAberto fsAreas">
		<legend class="titFiltros">Áreas</legend>
			<div class="block filtroCorpo">
				<div class="block">
					<% foreach (var item in Model.Areas) { %>
						<div class="coluna30 append2 divArea">
							<input type="hidden" class="hdnAreaId" value="<%= item.Id %>" />
							<input type="hidden" class="hdnAreaTipo" value="<%= item.Tipo %>" />
							<label for="<%= "Area" + item.Tipo %>"><%= item.TipoTexto + " *" %></label>
							<%= Html.TextBox("Area" + item.Tipo, item.Valor, new { @maxlength = "12", @class = "text txtAreaValor" })%>
						</div>
					<% } %>
				</div>
			</div>
	</fieldset>
<% } %>
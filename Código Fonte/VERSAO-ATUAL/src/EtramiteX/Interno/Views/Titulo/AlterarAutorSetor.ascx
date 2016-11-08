<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AlterarAutorSetorVM>" %>


<h2 class="titTela">Alterar Título</h2>
<br />

<div class="divAlterarTitulo">
	<% if(Model.TrocarAutor) { %>
		<fieldset class="block box fsAutor">
			<legend>Autor</legend>
			<div class="block">
				<div class="coluna60">
					<label for="DeAutor">De *</label>
					<%= Html.TextBox("DeAutor", Model.AutorAtual, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna60">
					<label for="ParaAutor">Para *</label>
					<%= Html.TextBox("ParaAutor", Model.AutorNovo, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>
		</fieldset>
	<% } %>

	<% if(Model.TrocarSetor) { %>
		<fieldset class="block box fsSetor">
			<legend>Setor de Cadastro</legend>
			<div class="block">
				<div class="coluna60">
					<label for="DeSetor">De *</label>
					<%= Html.DropDownList("DeSetor", new List<SelectListItem>() { new SelectListItem() { Text = Model.SetorAtual } }, new { @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna60">
					<label for="SetorCadastro">Para *</label>
					<% if(Model.Setores.Count > 1) { %>
						<%= Html.DropDownList("SetorCadastro", Model.Setores, new { @class = "text ddlSetores" })%>
					<% } else { %>
						<%= Html.DropDownList("SetorCadastro", Model.Setores, new { @class = "text ddlSetores disabled", @disabled = "disabled" })%>
					<% } %>
				</div>
			</div>
		</fieldset>
	<% } %>
</div>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPapel" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PapelVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Visualizar
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Visualizar Papel</h1>
		<br />
		<% using (Html.BeginForm("Visualizar", "Papel"))
		{ %>
		<div class="block box">
			<div class="block">
				<div class="coluna95">
					<label for="Nome">Nome *</label>
					<%= Html.TextBox("Nome", Model.Nome, new { maxlength = "40", @class = "text disabled", @disabled = "disabled" })%>
				</div>
			</div>
		</div>
		<fieldset class="block box">
			<legend>Permissões</legend>
			<% for (int i = 0; i < Model.GrupoColecao.Count; i++)
			{ %>
			<fieldset class="block box">
				<legend>
					<%= Html.Encode(Model.GrupoColecao[i].Nome)%></legend>
				<div class="block">
					<div class="coluna100">
						<p class="dataColuna">
							<% for (int j = 0; j < Model.GrupoColecao[i].PermissaoColecao.Count; j++)
							{ %>
							<span class="coluna19">
								<%= Html.HiddenFor(x => x.GrupoColecao[i].PermissaoColecao[j].ID)%>
								<%= Html.CheckBoxFor(x => x.GrupoColecao[i].PermissaoColecao[j].IsPermitido, new { @type = "checkbox", @disabled = "disabled" })%>
								<label class="labelCheckBox">
									<%= Model.GrupoColecao[i].PermissaoColecao[j].Nome %></label>
							</span>
							<% } %>
						</p>
					</div>
				</div>
			</fieldset>
			<% } %>
		</fieldset>
		<div class="block box">
			<a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a>
		</div>
		<% } %>
	</div>
</asp:Content>

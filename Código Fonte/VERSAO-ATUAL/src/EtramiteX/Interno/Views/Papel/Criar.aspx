<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPapel" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<PapelVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Cadastrar Papel</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Papel/salvar.js") %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Cadastrar Papel</h1>
		<br />
		<% using (Html.BeginForm("Criar", "Papel"))
	 { %>
		<div class="block box">
			<div class="block">
				<div class="coluna95">
					<label for="Nome">Nome *</label>
					<%= Html.TextBox("Nome", Model.Nome, new { maxlength = "40", @class = "text txtNome" })%>
					<%= Html.ValidationMessage("Nome")%>
				</div>
			</div>
		</div>
		<fieldset class="block box">
			<legend>Permissões</legend>
			<% for (int i = 0; i < Model.GrupoColecao.Count; i++)
			{ %>
			<fieldset id="fragment-1" class="block box">
				<legend>
					<%= Html.Encode(Model.GrupoColecao[i].Nome)%></legend>
				<div class="block">
					<div class="coluna100">
						<p class="dataColuna">
							<% for (int j = 0; j < Model.GrupoColecao[i].PermissaoColecao.Count; j++)
							{ %>
							<span class="coluna19">
								<%= Html.HiddenFor(x => x.GrupoColecao[i].PermissaoColecao[j].ID)%>
								<%= Html.CheckBoxFor(x => x.GrupoColecao[i].PermissaoColecao[j].IsPermitido, new { @type = "checkbox" })%>
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
			<input id="salvar" type="submit" value="Salvar" class="floatLeft" tabindex="35" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("")  %>">Cancelar</a></span>
		</div>
		<% }  %>
	</div>
</asp:Content>

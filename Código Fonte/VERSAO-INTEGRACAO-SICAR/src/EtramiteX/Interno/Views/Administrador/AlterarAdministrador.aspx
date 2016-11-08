<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<EditarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Alterar Administrador
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="central">

	<h1 class="titTela">Alterar Dados de Administrador</h1>
	<br />

	<% if (Model.Administrador != null && Model.Administrador.Id > 0) { %>
		<% using (Html.BeginForm("AlterarAdministrador", "Administrador")){ %>

			<div class="block box">

					<div class="block">
						<div class="coluna32">
							<label for="CpfAdministrador">CPF</label>
							<%= Html.TextBox("Cpf", Model.Administrador.Cpf, new { @class = "text disabled", disabled = "disabled" })%>
						</div>					
					</div>

					<div class="block">
						<div class="coluna32">
							<label for="Nome">Nome</label>
							<%= Html.TextBox("Nome", Model.Administrador.Nome, new { maxlength = "80", @class = "text disabled", disabled = "disabled" })%>
							<%= Html.ValidationMessage("Nome")%>
						</div>
					</div>

					<div class="block">
						<div class="coluna32">
								<label for="login">E-mail</label>
								<%= Html.TextBox("Email", Model.Administrador.Email, new { maxlength = "250", @class = "text disabled", disabled = "disabled" })%>
								<%= Html.ValidationMessage("email")%>
							</div>
					</div>

			</div>

			<div class="block box">
				<div class="block">
					<div class="coluna20">
					<p>
						<label for="login">Login</label>
						<%= Html.TextBox("Login", Model.Administrador.Usuario.Login, new { maxlength = "30", @class = "text disabled", disabled = "disabled" })%>
						<%= Html.ValidationMessage("login")%>
					</p>
						<%= Html.Hidden("forcaSenha") %>
						<label for="senha">Senha*</label>
						<%= Html.Password("Senha", null, new { @class = "text", type = "password" })%>
						<%= Html.ValidationMessage("senha")%>
					<p>
						<label for="ConfirmarSenha">Confirmar senha*</label>
						<%= Html.Password("ConfirmarSenha", null, new { @class = "text", type = "password" })%>
						<%= Html.ValidationMessage("ConfirmarSenha")%>
					</p>

					</div>
					<div class="coluna50 prepend2">
					</div>  
				</div>

			</div>
	
			<div class="block box">
				<input id="Submit1" type="submit" value="Salvar" class="floatLeft" />
				<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("Index", "Home")  %>">Cancelar</a></span>
			</div>

		<% } %>
	<% } else { %>
		<div class="block box">
			<a class="linkCancelar" title="Cancelar" href="<%= Url.Action("Index", "Home")  %>">Cancelar</a>
		</div>
	<% } %>	

</div>

</asp:Content>

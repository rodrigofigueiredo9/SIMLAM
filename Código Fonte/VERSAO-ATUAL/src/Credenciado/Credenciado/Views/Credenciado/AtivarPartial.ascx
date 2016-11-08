<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CredenciadoVM>" %>

<div class="block box">
	<div class="block">
		<div class="coluna30">
			<label for="Login">Login *</label>
			<%= Html.TextBox("Login", Model.Login, ViewModelHelper.SetaDisabled(Model.Credenciado.Situacao != 1, new { @class = "text txtLogin",  @maxlength = "30" }))%>
		</div>
	</div>

	<div class="block">
		<div class="coluna30">
			<label for="Senha">Senha *</label>
			<%= Html.TextBox("Senha", Model.Senha, new { @class = "text txtSenha", type = "password", @maxlength = "20" })%>
		</div>
	</div>

	<div class="block">
		<div class="coluna30">
			<label for="ConfirmarSenha">Confirmar Senha *</label>
			<%= Html.TextBox("ConfirmarSenha", Model.ConfirmarSenha, new { @class = "text txtConfirmarSenha", type = "password", @maxlength = "20" })%>
		</div>
	</div>
</div>
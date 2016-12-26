<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<EditarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Editar
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="central">
	<h1 class="titTela">Editar Administrador</h1>
	<br />

	<% if (Model.Administrador != null){ %>

	<% using (Html.BeginForm("Editar", "Administrador")){ %>

		<div class="block box">
			<div class="block">
				<div class="coluna25">
					<label for="CpfAdministrador">CPF *</label>
					<%= Html.TextBox("Cpf", Model.Administrador.Cpf, new { @class = "text disabled", readOnly= true, tabindex="1" })%>
				</div>
				<div class="coluna15">
				</div>
			</div>

			<div class="block">
				<div class="coluna32">
					<label for="Nome">Nome *</label>
					<%= Html.TextBox("Nome", Model.Administrador.Nome, new { maxlength = "80", @class = "text", tabindex = "2" })%>
					<%= Html.ValidationMessage("Nome")%>
				</div>
			</div>

			<div class="block">
				<div class="coluna32">
					<label for="login">E-mail *</label>
					<%= Html.TextBox("Email", Model.Administrador.Email, new { maxlength = "250", @class = "text", tabindex = "3" })%>
					<%= Html.ValidationMessage("email")%>
				</div>
			</div>
		</div>

		<div class="block box">
			<div class="coluna48 border">
				<div class="caixaAbas">
				<div id="abasNav">
					<ul class="ui-tabs-nav">
						<li class="ui-tabs-nav-item ui-tabs-selected" id="nav-fragment-1">
							<a href="#fragment-1" class="primeira"> Papéis </a>
						</li>
					</ul>

					<div id="fragment-1" class="ui-tabs-panel" >
						<div class="block boxEscuro">
							<div id="divPapeis" class="caixaScroll">
								<div class="handclick">
									<table id="tablePapeis" class="dataGridTable" border="0" cellpadding="0" cellspacing="0" width="100%">
									<tbody>
										<% for (int i = 0; i < Model.Papeis.Count; i++) {%>
											<%= Html.HiddenFor(x => x.Papeis[i].Papel.Id)%>
											<tr class="<%= ((Model.Papeis[i].IsAtivo)?"linhaSelecionada":string.Empty) %>" >
												<td class="celulaSeletorLinha" width="12%"><%= Html.CheckBoxFor(x => x.Papeis[i].IsAtivo, new { @type = "checkbox", tabindex="12" })%></td>
												<td class="celulaSeletorLinha" width="92%"><span title="<%= Html.Encode(Model.Papeis[i].Papel.Nome)%>"><%= Html.Encode(Model.Papeis[i].Papel.Nome)%></span> </td>
											</tr>
										<% } %>
									</tbody>
									</table>
								</div>
							</div>
						</div>
					</div>

				</div>
				</div>

			</div>

			<div class="coluna48 ultima">
				<div class="paddingT10">
					<label for="PermissoesConcedidas">Permissões concedidas</label>
				</div>
				<div>
					<%= Html.TextArea("TextoPermissoes", Model.TextoPermissoes, new { readOnly="true", @class = "textarea caixaPapeis", tabindex = "800" })%>
				</div>
			</div>
		</div>

		<div class="block box">
			<div class="block">
				<div class="coluna20">
					<label for="login">Login *</label>
					<%= Html.TextBox("Login", Model.Administrador.Usuario.Login, new { maxlength = "30", @class = "text disabled", readOnly = true, tabindex = "13" })%>
					<%= Html.ValidationMessage("login")%>
				</div>
				<div class="coluna20" style="margin-top: 21px;">
					<%= Html.CheckBox("AlterarSenha")%>
					<label for="AlterarSenha">Alterar senha</label>
				</div>
			</div>
			<div class="coluna20 alterarSenhaContainer <%= Model.AlterarSenha ? "" : "hide" %>">
				<%= Html.Hidden("forcaSenha") %>
				<label for="senha">Senha *</label>
				<%= Html.Password("Senha", null, new { @class = "text", type = "password", tabindex = "14" })%>
				<%= Html.ValidationMessage("senha")%>

				<label for="ConfirmarSenha">Confirmar senha *</label>
				<%= Html.Password("ConfirmarSenha", null, new { @class = "text", type = "password", tabindex = "15" })%>
				<%= Html.ValidationMessage("ConfirmarSenha")%>
			</div>
		</div>

		<div class="block box">
			<input id="salvar" type="submit" value="Salvar" class="floatLeft" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" href="<%= Url.Action("")  %>">Cancelar</a></span>
		</div>

		<% } %>
		<% } %>

	<script>
		$("#login").focus();
		var urlPermissao = '<%= Url.Content("~/Administrador/TextoPermissoes") %>';
	</script>

</div>
<script src="<%= Url.Content("~/Scripts/Administrador/salvar.js") %>" ></script>
<script>
	Administrador.urlVerificarResponsavelSetor = '<%= Url.Action("VerificarResponsavelSetor","Administrador") %>';
</script>

</asp:Content>
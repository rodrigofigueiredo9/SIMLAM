<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CriarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Cadastrar Administrador
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script>
	$(function () {
		$('[name="Cpf"]').focus();
	});
</script>

<div id="central">

	<% if (Model.Administrador != null && Model.Administrador.Id > 0)
	{%>
	<h1 class="titTela">Editar Administrador</h1>
	<% }
	else
	{  %>
	<h1 class="titTela">Cadastrar Administrador</h1>
	<% }%>

	<br />

	<% if (Model.CpfValido != null && !Model.CpfValido)
	   {%>
	   <div class="block box">
		<% using (Html.BeginForm("VerificarCpf", "Administrador", FormMethod.Get)) { %>
			<div class="block">
				<div class="coluna20">
					<label for="CpfAdministrador">CPF *</label>
					<%= Html.TextBox("Cpf", null, new { @class = "text maskCpf", tabindex="1" }) %>
				</div>
				<div class="coluna15">
					<input type="submit" id="btnVerificarCpf" value="Verificar" class="inlineBotao" tabindex="2" />
				</div>
			</div>
		<% } %>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" tabindex = "36" href="<%= Url.Action("") %>" >Cancelar</a></span>
		</div>
	<% } else {%>
		<% using (Html.BeginForm("Criar", "Administrador")) { %>
		<div class="block box">
			<div class="block">
				<div class="coluna20">
					<label for="CpfAdministrador">CPF *</label>
					<%= Html.TextBox("Cpf", null, new { @class = "	text disabled", readOnly = true, tabindex = "3" })%>
				</div>
				<div class="coluna15">
					<button name="button" type="button" onclick="document.location.href=$('#limparCpf').attr('href')" class="inlineBotao" tabindex="4">Limpar</button>
					<a id="limparCpf" href="<%= Html.AttributeEncode(Url.Action("Criar", "Administrador")) %>" style="display:none;" tabindex="5"></a>
				</div>
			</div>

			<div class="block">
				<div class="coluna32">
					<label for="Nome">Nome *</label>
					<%= Html.TextBox("Nome", Model.Administrador.Nome, new { maxlength = "80", @class = "text", tabindex = "6" })%>
					<%= Html.ValidationMessage("Nome")%>
				</div>
			</div>

			<div class="block">
				<div class="coluna32">
					<label for="login">E-mail *</label>
					<%= Html.TextBox("Email", Model.Administrador.Email, new { maxlength = "40", @class = "text", tabindex = "7" })%>
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
									<% for (int i = 0; i < Model.Papeis.Count; i++)
									   {%>
										<%= Html.HiddenFor(x => x.Papeis[i].Papel.Id)%>

											<tr class="<%= ((Model.Papeis[i].IsAtivo)?"linhaSelecionada":string.Empty) %>" >
												<td class="celulaSeletorLinha" width="12%"><%= Html.CheckBoxFor(x => x.Papeis[i].IsAtivo, new { @type = "checkbox", tabindex="24"})%></td>
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
					<%= Html.TextArea("TextoPermissoes", Model.TextoPermissoes, new { readOnly = "true", @class = "textarea caixaPapeis", tabindex = "800" })%>
				</div>
			</div>

		</div>

		<div class="block box">

					<div class="block">
						<div class="coluna20">
						<p>
							<label for="login">Login *</label>
							<%= Html.TextBox("Login", Model.Administrador.Usuario.Login, new { maxlength = "30", @class = "text", tabindex = "32" })%>
							<%= Html.ValidationMessage("login")%>
						</p>
							<%= Html.Hidden("forcaSenha") %>
							<label for="senha">Senha *</label>
							<%= Html.Password("Senha", null, new { @class = "text", type = "password", tabindex = "33" })%>
							<%= Html.ValidationMessage("senha")%>
						<p>
							<label for="ConfirmarSenha">Confirmar senha *</label>
							<%= Html.Password("ConfirmarSenha", null, new { @class = "text", type = "password", tabindex = "34" })%>
							<%= Html.ValidationMessage("ConfirmarSenha")%>
						</p>

						</div>

						<div class="coluna50 prepend2">

						</div>
					</div>

		</div>

		<div class="block box">
			<input id="salvar" type="submit" value="Salvar" class="floatLeft" tabindex = "35" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" title="Cancelar" tabindex = "36" href="<%= Url.Action("") %>" >Cancelar</a></span>
		</div>

		<% }  %>

	<script>
		$("#login").focus();
		var urlPermissao = '<%= Url.Content("~/Administrador/TextoPermissoes") %>';
	</script>

	<% } %>

</div>

<script src="<%= Url.Content("~/Scripts/Administrador/salvar.js") %>" ></script>
<script>
	Administrador.urlVerificarResponsavelSetor = '<%= Url.Action("VerificarResponsavelSetor","Administrador") %>';
</script>

</asp:Content>
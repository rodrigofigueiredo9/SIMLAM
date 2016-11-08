<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<VisualizarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Visualizar
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="central">
	
	<h1 class="titTela">Visualizar Administrador</h1>
	<br />

	<% if (Model.Administrador != null)
	{ %>

		<div class="block box">

				<div class="block">
					<div class="coluna32">
						<label for="CpfAdministrador">CPF</label>
						<%= Html.TextBox("Cpf", Model.Administrador.Cpf, new { @class = "text disabled", readOnly = true })%>
					</div>
					<div class="coluna32">                        
					</div>
				</div>

				<div class="block">
					<div class="coluna32">
						<label for="Nome">Nome</label>
						<%= Html.TextBox("Nome", Model.Administrador.Nome, new { maxlength = "80", @class = "text disabled", readOnly = true })%>
						<%= Html.ValidationMessage("Nome")%>
					</div>
				</div>

				<div class="block">
					<div class="coluna32">
						<label for="login">E-mail</label>
						<%= Html.TextBox("Email", Model.Administrador.Email, new { maxlength = "250", @class = "text disabled", readOnly = true })%>
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
			{
				if (!Model.Papeis[i].IsAtivo)
				{
					continue;
				} 
										   %>  
											
											<tr>                                                
												<td width="92%"><%= Html.Encode(Model.Papeis[i].Papel.Nome)%> </td>
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
					<%= Html.TextArea("TextoPermissoes", Model.TextoPermissoes, new { readOnly = "true", @class = "textarea caixaPapeis" })%>
				</div>
			</div>

		</div>

		<div class="block box">

			<div class="block">
						<div class="coluna20">
						<p>
							<label for="login">Login de usuário</label>
							<%= Html.TextBox("Login", Model.Administrador.Usuario.Login, new { maxlength = "30", @class = "text disabled", @disabled = "disabled" })%>
							<%= Html.ValidationMessage("login")%>
						</p>                            
						</div>
						
					</div>

		</div>

		<% } %>
		<div class="block box">
			<a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a>
		</div>
</div>

</asp:Content>

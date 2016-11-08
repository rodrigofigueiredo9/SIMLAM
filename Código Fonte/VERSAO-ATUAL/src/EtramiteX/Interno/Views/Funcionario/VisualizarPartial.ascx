<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFuncionario" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<VisualizarVM>" %>

<div class="divVisualizarPartial">
	<h1 class="titTela">Visualizar Funcionário</h1>
	<br />

	<% if (Model.Funcionario != null) { %>
		<div class="block box">
			<div class="block">
				<div class="coluna32">
					<label for="CpfFuncionario">CPF</label>
					<%= Html.TextBox("Cpf", Model.Funcionario.Cpf, new { @class = "text disabled", readOnly = true })%>
				</div>
			</div>

			<div class="block">
				<div class="coluna32">
					<label for="Nome">Nome</label>
					<%= Html.TextBox("Nome", Model.Funcionario.Nome, new { maxlength = "80", @class = "text disabled", readOnly = true })%>
					<%= Html.ValidationMessage("Nome")%>
				</div>
			</div>

			<div class="block">
				<div class="coluna32">
					<label for="login">E-mail</label>
					<%= Html.TextBox("Email", Model.Funcionario.Email, new { maxlength = "100", @class = "text disabled", readOnly = true })%>
					<%= Html.ValidationMessage("email")%>
				</div>
			</div>

			<div class="block">				
				<label for="cargo">Função</label>
				<div class="caixaScroll coluna100">
					<table id="tableCargo" class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
						<% for (int i = 0; i < Model.Funcionario.Cargos.Count; i++) { %> 
								<tr>
									<td width="100%"><%= Html.Label(Model.Funcionario.Cargos[i].Nome)%></td>
								</tr>
						<% } %>
					</table>
				</div>
			</div>

			<div class="block">
				<label for="setor">Setor</label>
				<div class="caixaScroll coluna100">
					<table id="table2" class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
						<% for (int i = 0; i < Model.Funcionario.Setores.Count; i++) { %> 
								<tr>
									<td width="100%"><%= Html.Label(Model.Funcionario.Setores[i].SiglaComNome)%></td>
								</tr>
						<% } %>
					</table>
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
												<% for (int i = 0; i < Model.Papeis.Count; i++) {
													if (!Model.Papeis[i].IsAtivo)
													{
														continue;
													} %>
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
			        <div class="coluna80 inputFileDiv">
				        <label for="ArquivoTexto">Assinatura Digitalizada</label>
                        <% if(Model.Funcionario.Arquivo.Id.GetValueOrDefault() > 0) { %>
					        <%= Html.ActionLink(Tecnomapas.EtramiteX.Interno.ViewModels.ViewModelHelper.StringFit(Model.Funcionario.Arquivo.Nome, 45), "Baixar", "Arquivo", new { @id = Model.Funcionario.Arquivo.Id.GetValueOrDefault() }, new { @Style = "display: block", @class = "lnkArquivo", @title = Model.Funcionario.Arquivo.Nome })%>
				        <% } %>
			        </div>

		        </div>
        </div>


		<div class="block box">
			<div class="block">
				<div class="coluna20">
					<p>
						<label for="login">Login de usuário</label>
						<%= Html.TextBox("Login", Model.Funcionario.Usuario.Login, new { maxlength = "30", @class = "text disabled" })%>
						<%= Html.ValidationMessage("login")%>
					</p>
				</div>
			</div>
		</div>
	<% } %>
</div>
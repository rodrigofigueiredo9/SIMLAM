<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa.SalvarVM>" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloPessoa" %>

<fieldset class="block box">
<%= Html.Hidden("Pessoa.Tipo", 2) %>
	<div class="block">
		<div class="coluna20">
			<% if (Model.IsVisualizar) { %>
				<label for="">CNPJ</label>
			<% } else { %>
				<label for="">CNPJ *</label>
			<% } %>
			<%= Html.TextBox("Pessoa.Juridica.CNPJ", Model.Pessoa.Juridica.CNPJ, new { @maxlength = "20", @class = "inputCnpjPessoa text disabled", @disabled = "disabled" })%>
		</div>
		<% if (!Model.IsVisualizar && Model.ExibirLimparPessoa) { %>
		<div class="coluna10">
			<button type="button" title="Limpar CNPJ" class="inlineBotao btnLimparCpfCnpj">Limpar</button>
		</div>
		<% } %>
	</div>
	<div class="block">
		<div class="coluna90">
			<% if (Model.IsVisualizar) { %>
				<label for="Pessoa.Juridica.RazaoSocial">Razão social</label>
				<%= Html.TextBox("Pessoa.Juridica.RazaoSocial", Model.Pessoa.Juridica.RazaoSocial, new { @class = "text disabled", @maxlength = "80", @disabled = "disabled" })%> 
			<% } else { %>
				<label for="Pessoa.Juridica.RazaoSocial">Razão social *</label>
				<%= Html.TextBox("Pessoa.Juridica.RazaoSocial", Model.Pessoa.Juridica.RazaoSocial, new { @class = "text ", @maxlength = "80" })%> 
			<% } %>
		</div>
	</div>
	<div class="block">
		<div class="coluna90">
			<label for="Pessoa.Juridica.NomeFantasia">Nome fantasia</label>
			<% if (Model.IsVisualizar) { %>
			<%= Html.TextBox("Pessoa.Juridica.NomeFantasia", Model.Pessoa.Juridica.NomeFantasia, new { @maxlength = "80", @class = "text disabled txtPessoaNome", @disabled = "disabled" })%> 
			<% } else { %>
			<%= Html.TextBox("Pessoa.Juridica.NomeFantasia", Model.Pessoa.Juridica.NomeFantasia, new { @maxlength = "80", @class = "text txtPessoaNome" })%> 
			<% } %>
		</div>
	</div>	
	<div class="block">
		<div class="coluna20">
			<label for="Pessoa.Juridica.Ie">Inscrição estadual</label>
			<% if (Model.IsVisualizar) { %>
			<%= Html.TextBox("Pessoa.Juridica.Ie", Model.Pessoa.Juridica.IE, new { @maxlength = "20", @class = "text disabled", @disabled = "disabled" })%> 
			<% } else { %>
			<%= Html.TextBox("Pessoa.Juridica.Ie", Model.Pessoa.Juridica.IE, new { @maxlength = "20", @class = "text" })%> 
			<% } %>
		</div>
	</div>
</fieldset>

<fieldset class="block box" id="Representante_Container">
	<legend>Representantes</legend>
		<% if (!Model.IsVisualizar) { %>
			<div class="block">	
				<div class="floatRight">
					<button type="button" title="Buscar representante" class="btnAssociarReprensetante">Buscar</button>
					<%= Html.Hidden("Pessoa.Juridica.RepresentanteId") %>
				</div>
			</div>
		<% } %>
		<div class="block dataGrid">
			<table class="dataGridTable tabRepresentantes" width="100%" border="0" cellspacing="0" cellpadding="0">
				<thead>
					<tr>
						<th>Nome</th>
						<th>CPF</th>
						<% if (!Model.IsVisualizar) { %><th width="10%">Ações</th><% } %>
					</tr>
				</thead>
				<tbody>
				<%
					int idRep = 0;
					foreach (Pessoa rep in Model.Pessoa.Juridica.Representantes) {
					idRep++; %>
					<tr>
						<td><span title="<%: Html.Encode(rep.Fisica.Nome) %>"><%= Html.Encode(rep.Fisica.Nome) %></span>
							<input type="hidden" class="hdnRepresentanteIndex" name="Pessoa.Juridica.Representantes.Index" value="<%= idRep %>" />
							<input type="hidden" class="hdnRepresentanteId" name="Pessoa.Juridica.Representantes[<%= idRep %>].Id" value="<%= Html.Encode(rep.Id) %>" />
							<input type="hidden" class="hdnRepresentanteNome" name="Pessoa.Juridica.Representantes[<%= idRep %>].Fisica.Nome" value="<%= Html.Encode(rep.Fisica.Nome) %>" />
							<input type="hidden" class="hdnRepresentanteCpf" name="Pessoa.Juridica.Representantes[<%= idRep %>].Fisica.CPF" value="<%= Html.Encode(rep.Fisica.CPF) %>" />
						</td>
						<td>
							<span title="<%: rep.Fisica.CPF %>"><%: rep.Fisica.CPF %></span>
						</td>
						<% if (!Model.IsVisualizar) { %>
						<td>
							<input title="Visualizar" type="button" class="icone visualizar btnVisualizarRepresentante" value="" />
							<input title="Excluir" type="button" class="icone excluir btnExcluirRepresentante" value="" />
						</td>
						<% } %>
					</tr>
				<%	} %>
				</tbody>
			</table>
			<table style="display: none">
				<tbody>
					<tr class="trRepresentanteTemplate">
						<td><span class="RepresentanteNome" title="tooltip">NOME</span>
								<input type="hidden" class="hdnRepresentanteIndex" name="templatePessoa.Representantes.Index" value="#Index" />
								<input type="hidden" class="hdnRepresentanteId" name="templatePessoa.Representantes[#Index].Id" value="#Id" />
								<input type="hidden" class="hdnRepresentanteNome" name="templatePessoa.Representantes[#Index].Fisica.Nome" value="#NOME" />
								<input type="hidden" class="hdnRepresentanteCpf" name="templatePessoa.Representantes[#Index].Fisica.CPF" value="#CPF" />
							</td>
						<td>
							<span class="RepresentanteCpf">CPF</span>
						</td>
						<td>
							<% if (!Model.IsVisualizar) { %>
								<input title="Visualizar" type="button" class="icone visualizar btnVisualizarRepresentante" value="" />
								<button title="Excluir" alt="Excluir" class="icone excluir btnExcluirRepresentante" type="button"></button>								
							<% } %>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</fieldset>
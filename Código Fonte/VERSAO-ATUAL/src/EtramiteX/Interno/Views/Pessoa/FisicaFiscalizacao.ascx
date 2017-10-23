<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa.SalvarVM>" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>

<fieldset class="block box">
<%= Html.Hidden("Pessoa.Tipo", 1) %>
	<div class="block">
		<div class="coluna20">
			<% if (Model.IsVisualizar) { %>
				<label for="Pessoa.Fisica.CPF">CPF</label>
			<% } else { %>
				<label for="Pessoa.Fisica.CPF">CPF *</label>
			<% } %>
			<%= Html.TextBox("Pessoa.Fisica.CPF", Model.Pessoa.Fisica.CPF, new { @maxlength = "15", @class = "inputCpfPessoa text disabled", @disabled = "disabled" })%> 
		</div>
		<% if (!Model.IsVisualizar && Model.ExibirLimparPessoa) { %>
		<div class="coluna10">
			<button type="button" title="Limpar CPF" class="inlineBotao btnLimparCpfCnpj">Limpar</button>
		</div>
		<% } %>
	</div>
	<div class="block">
		<div class="coluna98">
			<% if (Model.IsVisualizar) { %>
				<label for="Pessoa.Fisica.Nome">Nome</label>
			<% } else { %>
				<label for="Pessoa.Fisica.Nome">Nome *</label>
			<% } %>
			<% if (Model.IsVisualizar) { %>
			<%= Html.TextBox("Pessoa.Fisica.Nome", Model.Pessoa.Fisica.Nome, new { @class = "text disabled txtPessoaNome", @disabled = "disabled" })%> 
			<% } else { %>
			<%= Html.TextBox("Pessoa.Fisica.Nome", Model.Pessoa.Fisica.Nome, new { @maxlength = "80	", @class = "text txtPessoaNome" })%> 
			<% } %>
		</div>
	</div>

</fieldset>

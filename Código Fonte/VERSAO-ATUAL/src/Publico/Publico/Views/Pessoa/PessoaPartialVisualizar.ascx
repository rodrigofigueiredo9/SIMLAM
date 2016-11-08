<%@ Import Namespace="Tecnomapas.EtramiteX.Publico.ViewModels.VMPessoa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PessoaVM>" %>

<div class="pessoaPartial">
	<input type="hidden" name="Id" value="<%= Model.Pessoa.Id %>" />
	<input type="hidden" name="CPFCNPJ" value="<%= Model.Pessoa.CPFCNPJ %>" />
	<input type="hidden" name="NomeRazaoSocial" value="<%= Model.Pessoa.NomeRazaoSocial %>" />


	<%= Html.Hidden("Pessoa.Id", Model.Pessoa.Id, new { @class = "pessoaId" })%>
	<%= Html.Hidden("Pessoa.InternoId", Model.Pessoa.InternoId, new { @class = "internoId" })%>
	<%= Html.Hidden("Pessoa.CredenciadoId", Model.Pessoa.CredenciadoId, new { @class = "credenciadoId" })%>

	<% if (Model.Pessoa.Tipo == 1) { %>
		<% Html.RenderPartial("Fisica"); %>
	<% } else { %>
		<% Html.RenderPartial("Juridica"); %>
	<% } %>

<fieldset class="block box">
	<legend>Meios de Contato</legend>
	<div class="block">
		<div class="coluna22">
			<label for="Contato.TelefoneResidencial">Telefone residencial</label>
			<%= Html.TextBox("Contato.TelefoneResidencial", Model.Contato.TelefoneResidencial, new { @maxlength = "40", @class = "text maskFone disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna22 prepend2">
			<label for="Contato.TelefoneCelular">Telefone celular</label>
			<%= Html.TextBox("Contato.TelefoneCelular", Model.Contato.TelefoneCelular, new { @class = "text maskFone disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna22  prepend2">
			<label for="Contato.TelefoneFax">Telefone fax</label>
			<%= Html.TextBox("Contato.TelefoneFax", Model.Contato.TelefoneFax, new { @maxlength = "40", @class = "text maskFone disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna22 prepend2">
			<label for="Contato.TelefoneComercial">Telefone comercial</label>
			<%= Html.TextBox("Contato.TelefoneComercial", Model.Contato.TelefoneComercial, new { @maxlength = "40", @class = "text maskFone disabled", @disabled = "disabled" })%>
		</div>
	</div>
	<div class="block">
		<div class="coluna60">
			<label for="Contato.Email">E-mail</label>
			<%= Html.TextBox("Contato.Email", Model.Contato.Email, new { @maxlength = "40", @class = "text maskEmail disabled", @disabled = "disabled" })%>
		</div>
	</div>
	<div class="block">
		<div class="coluna60">
			<label for="Contato.Nome">Nome para Contato</label>
			<%= Html.TextBox("Contato.Nome", Model.Contato.Nome, new { @maxlength = "80", @class = "text txtNomeContato disabled", @disabled = "disabled" })%>
		</div>
	</div>
</fieldset>
<fieldset class="block box">
<legend>Endereço</legend>
	<div class="block">
		<div class="coluna19">
			<label for="Pessoa.Endereco.Cep">CEP</label>
			<%= Html.TextBox("Pessoa.Endereco.Cep", Model.Pessoa.Endereco.Cep, new { @class = "text maskCep disabled", @disabled = "disabled", @maxlength = "10" })%>
		</div>		
		<div class="coluna75 prepend2">
			<label for="Pessoa.Endereco.Logradouro">Logradouro/Rua/Rodovia *</label>
			<%= Html.TextBox("Pessoa.Endereco.Logradouro", Model.Pessoa.Endereco.Logradouro, new { @maxlength = "40", @class = "text disabled", @disabled = "disabled" })%>
		</div>
	</div>
	<div class="block divEndereco">
		<div class="coluna45">
			<label for="Pessoa.Endereco.Bairro">Bairro/Gleba *</label>
			<%= Html.TextBox("Pessoa.Endereco.Bairro", Model.Pessoa.Endereco.Bairro, new { @class = "text disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna20 prepend2">
			<label for="Pessoa.Endereco.Estado">UF *</label>
			<%= Html.DropDownList("Pessoa.Endereco.EstadoId", Model.Estados, new { @class = "text disabled ddlEstado", @disabled = "disabled" })%>
		</div>
		<div class="coluna26 prepend2">
			<label for="Pessao.Cidade">Município *</label>
			<%= Html.DropDownList("Pessoa.Endereco.MunicipioId", Model.Municipios, new { @class = "text disabled ddlMunicipio", disabled = "disabled" })%>
		</div>
	</div>
	<div class="block">
		<div class="coluna10">
			<label for="Pessoa.Endereco.Numero">Número</label>
			<%= Html.TextBox("Pessoa.Endereco.Numero", Model.Pessoa.Endereco.Numero, new { @maxlength = "4", @class = "text maskNumEndereco disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna32 prepend2">
			<label for="Pessoa.Endereco.DistritoLocalizacao">Distrito/Localidade *</label>
			<%= Html.TextBox("Pessoa.Endereco.DistritoLocalizacao", Model.Pessoa.Endereco.DistritoLocalizacao, new { @maxlength = "100", @class = "text disabled", @disabled = "disabled" })%>
		</div>
		<div class="coluna49 prepend2">
			<label for="Pessoa.Endereco.Complemento">Complemento</label>
			<%= Html.TextBox("Pessoa.Endereco.Complemento", Model.Pessoa.Endereco.Complemento, new { @maxlength = "50", @class = "text disabled", @disabled = "disabled" })%>
		</div>
	</div>
</fieldset>
</div>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CredenciadoVM>" %>

<div class="pessoaPartial">
	<%= Html.Hidden("Pessoa.Id", Model.Pessoa.Id, new { @class = "pessoaId" })%>
	<%= Html.Hidden("Pessoa.InternoId", Model.Pessoa.InternoId, new { @class = "internoId" })%>
	<%= Html.Hidden("Pessoa.CredenciadoId", Model.Pessoa.CredenciadoId, new { @class = "credenciadoId" })%>
	<%= Html.Hidden("PessoaVM.OcultarIsCopiado", Model.PessoaVM.OcultarIsCopiado, new { @class = "hdnOcultarIsCopiado" })%>

	<div class="block box">
		<div class="coluna98">
			<p><label for="Pessoa.Tipo">Perfil do usuário credenciado *</label></p>
			<label><%= Html.RadioButton("Credenciado.Tipo", (int)eCredenciadoTipo.Interessado, Model.Credenciado.Tipo == (int)eCredenciadoTipo.Interessado, new { @class = "radio rdbCredenciadoTipo disabled", @disabled = "disabled" })%> Interessado</label>
			<label class="prepend2"><%= Html.RadioButton("Credenciado.Tipo", (int)eCredenciadoTipo.ResponsavelTecnico, Model.Credenciado.Tipo == (int)eCredenciadoTipo.ResponsavelTecnico, new { @class = "radio rdbCredenciadoTipo disabled", @disabled = "disabled" })%> Responsável técnico</label>
			<%if(Model.Pessoa.Tipo == 1){%><label class="prepend2"><%= Html.RadioButton("Credenciado.Tipo", (int)eCredenciadoTipo.OrgaoParceiroConveniado, Model.Credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado, new { @class = "radio rdbCredenciadoTipo disabled", @disabled = "disabled" })%> Órgão parceiro/ conveniado</label><%}%>
		</div>
	</div>
	
	<div class="divOrgaoParceiroConveniado <%=Model.Credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado ? "" : "hide" %>">
		<% Html.RenderPartial("OrgaoParceiroConveniado", Model); %>
	</div>
	
	<% if (Model.Pessoa.Tipo == 1) { %>
			<% Html.RenderPartial("~/Views/Pessoa/Fisica.ascx", Model.PessoaVM); %>
		<% } else { %>
			<% Html.RenderPartial("~/Views/Pessoa/Juridica.ascx", Model.PessoaVM); %>
		<% } %>

	<fieldset class="block box">
		<legend>Meios de Contato</legend>

		<div class="block">
			<div class="coluna22">
				<label for="Contato.TelefoneResidencial">Telefone residencial</label>
				<%= Html.TextBox("Contato.TelefoneResidencial", Model.Contato.TelefoneResidencial, new { @maxlength = "32", @class = "text maskFone" })%>
			</div>
			<div class="coluna22 prepend2">
				<label for="Contato.TelefoneCelular">Telefone celular</label>
				<%= Html.TextBox("Contato.TelefoneCelular", Model.Contato.TelefoneCelular, new { @maxlength = "32", @class = "text maskFone" })%>
			</div>
			<div class="coluna22  prepend2">
				<label for="Contato.TelefoneFax">Telefone fax</label>
				<%= Html.TextBox("Contato.TelefoneFax", Model.Contato.TelefoneFax, new { @maxlength = "32", @class = "text maskFone" })%>
			</div>
			<div class="coluna22 prepend2">
				<label for="Contato.TelefoneComercial">Telefone comercial</label>
				<%= Html.TextBox("Contato.TelefoneComercial", Model.Contato.TelefoneComercial, new { @maxlength = "32", @class = "text maskFone" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna60">
				<label for="Contato.Email">E-mail *</label>
				<%= Html.TextBox("Contato.Email", Model.Contato.Email, new { @maxlength = "40", @class = "text maskEmail" })%>
			</div>
		</div>

		<div class="block">
			<div class="coluna60">
				<label for="Contato.Nome">Nome para Contato</label>
				<%= Html.TextBox("Contato.Nome", Model.Contato.Nome, new { @maxlength = "80", @class = "text txtNomeContato" })%>
			</div>
		</div>
	</fieldset>

	<fieldset class="block box">
		<legend>Endereço</legend>
		<input type="hidden" name="Pessoa.Endereco.Id" value="<%= Model.Pessoa.Endereco.Id %>" />

		<div class="block">
			<div class="coluna19">
				<label for="Pessoa.Endereco.Cep">CEP *</label>
				<%= Html.TextBox("Pessoa.Endereco.Cep", Model.Pessoa.Endereco.Cep, new { @class = "text maskCep", @maxlength = "10" })%>
			</div>
			<div class="coluna75 prepend2">
				<label for="Pessoa.Endereco.Logradouro">Logradouro/Rua/Rodovia *</label>
				<%= Html.TextBox("Pessoa.Endereco.Logradouro", Model.Pessoa.Endereco.Logradouro, new { @maxlength = "500", @class = "text" })%>
			</div>
		</div>

		<div class="block divEndereco">
			<div class="coluna45">
				<label for="Pessoa.Endereco.Bairro">Bairro/Gleba *</label>
				<%= Html.TextBox("Pessoa.Endereco.Bairro", Model.Pessoa.Endereco.Bairro, new { @maxlength = "100", @class = "text" })%>
			</div>
			<div class="coluna20 prepend2">
				<label for="Pessoa.Endereco.Estado">UF *</label>
				<%= Html.DropDownList("Pessoa.Endereco.EstadoId", Model.PessoaVM.Estados, new { @class = "text ddlEstado" })%>
			</div>
			<div class="coluna26 prepend2">
				<label for="Pessao.Cidade">Município *</label>
				<% if (Model.Pessoa.Endereco.EstadoId == 0) { %>
					<%= Html.DropDownList("Pessoa.Endereco.MunicipioId", Model.PessoaVM.Municipios, new { disabled = "disabled", @class = "text ddlMunicipio disabled" })%> 
				<% } else { %>
					<%= Html.DropDownList("Pessoa.Endereco.MunicipioId", Model.PessoaVM.Municipios, new { @class = "text ddlMunicipio" })%> 
				<% } %>
			</div>
		</div>

		<div class="block">
			<div class="coluna10">
				<label for="Pessoa.Endereco.Numero">Número *</label>
				<%= Html.TextBox("Pessoa.Endereco.Numero", Model.Pessoa.Endereco.Numero, new { @maxlength = "4", @class = "text maskNumEndereco" })%>
			</div>
			<div class="coluna32 prepend2">
				<label for="Pessoa.Endereco.DistritoLocalizacao">Distrito/Localidade *</label>
				<%= Html.TextBox("Pessoa.Endereco.DistritoLocalizacao", Model.Pessoa.Endereco.DistritoLocalizacao, new { @maxlength = "100", @class = "text" })%>
			</div>
			<div class="coluna49 prepend2">
				<label for="Pessoa.Endereco.Complemento">Complemento</label>
				<%= Html.TextBox("Pessoa.Endereco.Complemento", Model.Pessoa.Endereco.Complemento, new { @maxlength = "50", @class = "text" })%>
			</div>
		</div>
	</fieldset>

	<div class="block box">
		<div class="block">
			<div class="coluna30">
				<label for="Login">Login *</label>
				<%= Html.TextBox("Login", Model.Login, new { @class = "text txtLogin disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna15">
				<%=Html.CheckBox("Credenciado.AlterarSenha", Model.Credenciado.AlterarSenha, new { style = "margin-top: 21px;", @class = "ckbAlterarSenha" })%>
				<label for="Credenciado_AlterarSenha">Alterar Senha</label>
			</div>
		</div>

		<div class="divAlterarSenha block <%= ((Model.Credenciado.AlterarSenha) ? string.Empty : "hide") %>">
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
	</div>
</div>
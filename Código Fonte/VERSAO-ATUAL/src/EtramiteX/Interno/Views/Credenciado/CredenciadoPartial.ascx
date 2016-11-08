<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CredenciadoVM>" %>

<div class="titTela"></div>

<div class="pessoaPartial">
	<%= Html.Hidden("Pessoa.Id", Model.Pessoa.Id, new { @class = "pessoaId" })%>
	<%= Html.Hidden("Pessoa.InternoId", Model.Pessoa.InternoId, new { @class = "internoId" })%>
	<%= Html.Hidden("Credenciado.Id", Model.Credenciado.Id, new { @class = "credenciadoId" })%>

	<div class="block box">
			<div class="coluna65">
				<p><label for="Pessoa.Tipo">Perfil do usuário credenciado *</label></p>
				<label><%= Html.RadioButton("Credenciado.Tipo", (int)eCredenciadoTipo.Interessado, Model.Credenciado.Tipo == (int)eCredenciadoTipo.Interessado, new { @class = "radio rdbCredenciadoTipo disabled", @disabled = "disabled" })%> Interessado</label>
				<label class="prepend1"><%= Html.RadioButton("Credenciado.Tipo", (int)eCredenciadoTipo.ResponsavelTecnico, Model.Credenciado.Tipo == (int)eCredenciadoTipo.ResponsavelTecnico, new { @class = "radio rdbCredenciadoTipo disabled", @disabled = "disabled" })%> Responsável técnico</label>
				<label class="prepend1"><%= Html.RadioButton("Credenciado.Tipo", (int)eCredenciadoTipo.ResponsavelTecnico, Model.Credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado, new { @class = "radio rdbCredenciadoTipo disabled", @disabled = "disabled" })%> Órgão parceiro/ conveniado</label>
			</div>
		</div>
		
		<%if(Model.Credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado){ %>
			<fieldset class="block box">
				<legend>Parceiro/ Convênio</legend>
				<div class="block">
					<div class="coluna98">
						<label for="Pessao.Cidade">Nome do órgão parceiro/ conveniado *</label>
						<%= Html.DropDownList("Credenciado.OrgaoParceiroId", Model.OrgaosParceiros, new { @class = "text ddlOrgaoParceiro disabled", @disabled="disabled" })%> 
					</div>
				</div>

				<div class="block">
					<div class="coluna98">
						<label for="Pessao.Cidade">Unidade *</label>
						<%= Html.DropDownList("Credenciado.OrgaoParceiroUnidadeId", Model.OrgaosParceirosUnidades, new { @class = "text ddlOrgaoParceiroUnidade disabled", @disabled="disabled"  })%> 
					</div>
				</div>
			</fieldset>		

		<%} %>
		<% if (Model.PessoaVM.CpfCnpjValido && Model.Pessoa.Tipo == PessoaTipo.FISICA) { %>
			<% Html.RenderPartial("~/Views/Pessoa/Fisica.ascx", Model.PessoaVM); %>
		<% } else { %>
			<% Html.RenderPartial("~/Views/Pessoa/Juridica.ascx", Model.PessoaVM); %>
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
	</fieldset>
	<fieldset class="block box">
	<legend>Endereço</legend>
		<div class="block">
			<div class="coluna19">
				<label for="Pessoa.Endereco.Cep">CEP</label>
				<%= Html.TextBox("Pessoa.Endereco.Cep", Model.Pessoa.Endereco.Cep, new { @class = "text maskCep disabled", @disabled = "disabled" })%>
			</div>		
			<div class="coluna75 prepend2">
				<label for="Pessoa.Endereco.Logradouro">Logradouro</label>
				<%= Html.TextBox("Pessoa.Endereco.Logradouro", Model.Pessoa.Endereco.Logradouro, new { @maxlength = "40", @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>
		<div class="block divEndereco">
			<div class="coluna45">
				<label for="Pessoa.Endereco.Bairro">Bairro</label>
				<%= Html.TextBox("Pessoa.Endereco.Bairro", Model.Pessoa.Endereco.Bairro, new { @class = "text disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna20 prepend2">
				<label for="Pessoa.Endereco.Estado">Estado</label>
				<%= Html.DropDownList("Pessoa.Endereco.EstadoId", Model.PessoaVM.Estados, new { @class = "text disabled ddlEstado", @disabled = "disabled" })%>
			</div>
			<div class="coluna26 prepend2">
				<label for="Pessao.Cidade">Município</label>
				<%= Html.DropDownList("Pessoa.Endereco.MunicipioId", Model.PessoaVM.Municipios, new { @class = "text disabled ddlMunicipio", disabled = "disabled", })%>
			</div>
		</div>
		<div class="block">
			<div class="coluna19">
				<label for="Pessoa.Endereco.Numero">Número</label>
				<%= Html.TextBox("Pessoa.Endereco.Numero", Model.Pessoa.Endereco.Numero, new { @maxlength = "4", @class = "text disabled", @disabled = "disabled" })%>
			</div>
			<div class="coluna75 prepend2">
				<label for="Pessoa.Endereco.Complemento">Complemento</label>
				<%= Html.TextBox("Pessoa.Endereco.Complemento", Model.Pessoa.Endereco.Complemento, new { @maxlength = "50", @class = "text disabled", @disabled = "disabled" })%>
			</div>
		</div>
	</fieldset>
</div>
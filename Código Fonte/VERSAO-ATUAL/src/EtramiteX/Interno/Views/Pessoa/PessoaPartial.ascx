<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SalvarVM>" %>

<div class="pessoaPartial">

<%= Html.Hidden("ExibirBotoes", Model.ExibirBotoes) %>
<%= Html.Hidden("ExibirMensagensPartial", Model.ExibirMensagensPartial)%>
<%= Html.Hidden("Pessoa.Id", Model.Pessoa.Id, new {@class = "pessoaId"})%>
		
		<% if (!Model.CpfCnpjValido) { %>
				<fieldset class="block box">
					<div class="block">
						<div class="coluna25">
							<p><label for="Pessoa.Tipo">Tipo *</label></p>							
							<label class="<%= (Model.TipoCadastro == 0 || Model.TipoCadastro == 1)  ? "" : "hide" %>"><%= Html.RadioButton("Pessoa.Tipo", PessoaTipo.FISICA, Model.Pessoa.Tipo != PessoaTipo.JURIDICA, new { @class = "radio pessoaf rdbPessaoTipo" })%> Física</label>							
							<label class="<%= (Model.TipoCadastro == 0 || Model.TipoCadastro == 2)  ? "" : "hide" %> append5"><%= Html.RadioButton("Pessoa.Tipo", PessoaTipo.JURIDICA, Model.Pessoa.Tipo == PessoaTipo.JURIDICA, new { @class = "radio pessoaj rdbPessaoTipo" })%> Jurídica</label>
						</div>
						<div class="coluna30 prepend7">
							<div class="CpfPessoaContainer <%= Model.Pessoa.Tipo != PessoaTipo.JURIDICA ? "" : "hide" %> ">
								<label for="Pessoa.Fisica.Cpf">CPF *</label>
								<%= Html.TextBox("Pessoa.Fisica.Cpf", Model.Pessoa.Fisica.CPF, new { @class = "text maskCpf inputCpfPessoa" })%>
							</div>
							<div class="CnpjPessoaContainer <%= Model.Pessoa.Tipo == PessoaTipo.JURIDICA ? "" : "hide" %> ">
								<label for="Pessoa.Juridica.Cnpj">CNPJ *</label>
								<%= Html.TextBox("Pessoa.Juridica.Cnpj", Model.Pessoa.Juridica.CNPJ, new { @class = "text maskCnpj inputCnpjPessoa" })%>
							</div>
						</div>
						<div class="coluna30">
							<button type="button" class="inlineBotao btnVerificarCpfCnpj">Verificar</button>
						</div>
					</div>
				</fieldset>
		<% } else { %>

			<% if (Model.CpfCnpjValido && Model.Pessoa.Tipo == 1) { %>

				<% Html.RenderPartial("Fisica"); %>
			
			<% } else { %>

				<% Html.RenderPartial("Juridica"); %>

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
					<label for="Contato.Email">E-mail</label>
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

			<% if (Model.CpfCnpjValido && Model.Pessoa.Tipo == 1) { %>
			<div class="block divCopiarEnderecoConjuge <%= Model.Pessoa.Fisica.ConjugeId > 0 ? "" : "hide"%>">
				<div class="direita append4">
					<input type="button" class="btnCopiarEnderecoConjuge" value="Copiar Endereço do Conjuge" />
				</div>
			</div>
			<%} %>

			<div class="block">
				<div class="coluna19">
					<label for="Pessoa.Endereco.Cep">CEP</label>
					<%= Html.TextBox("Pessoa.Endereco.Cep", Model.Pessoa.Endereco.Cep, new { @class = "text maskCep", @maxlength = "10" })%>
				</div>
				<div class="coluna75 prepend2">
					<label for="Pessoa.Endereco.Logradouro">Logradouro/Rua/Rodovia *</label>
					<%= Html.TextBox("Pessoa.Endereco.Logradouro", Model.Pessoa.Endereco.Logradouro, new { @maxlength = "500", @class = "text" })%>
				</div>
			</div>

			<div class="block divEndereco">
				<div class="coluna45">
					<label for="Pessoa.Endereco.Bairro">Bairro/Gleba</label>
					<%= Html.TextBox("Pessoa.Endereco.Bairro", Model.Pessoa.Endereco.Bairro, new { @maxlength = "100", @class = "text" })%>
				</div>
				<div class="coluna20 prepend2">
					<label for="Pessoa.Endereco.Estado">UF *</label>
					<%= Html.DropDownList("Pessoa.Endereco.EstadoId", Model.Estados, new { @class = "text ddlEstado" })%>
				</div>
				<div class="coluna26 prepend2">
					<label for="Pessao.Cidade">Município *</label>
					<% if (Model.Pessoa.Endereco.EstadoId == 0) { %>
						<%= Html.DropDownList("Pessoa.Endereco.MunicipioId", Model.Municipios, new { disabled = "disabled", @class = "text ddlMunicipio disabled" })%> 
					<% } else { %>
						<%= Html.DropDownList("Pessoa.Endereco.MunicipioId", Model.Municipios, new { @class = "text ddlMunicipio" })%> 
					<% } %>
				</div>
			</div>
			<div class="block">
				<div class="coluna10">
					<label for="Pessoa.Endereco.Numero">Número</label>
					<%= Html.TextBox("Pessoa.Endereco.Numero", Model.Pessoa.Endereco.Numero, new { @maxlength = "4", @class = "text maskNumEndereco" })%>
				</div>
				<div class="coluna32 prepend2">
					<label for="Pessoa.Endereco.Numero">Distrito/Localidade *</label>
					<%= Html.TextBox("Pessoa.Endereco.DistritoLocalizacao", Model.Pessoa.Endereco.DistritoLocalizacao, new { @maxlength = "100", @class = "text" })%>
				</div>
				<div class="coluna49 prepend2">
					<label for="Pessoa.Endereco.Complemento">Complemento</label>
					<%= Html.TextBox("Pessoa.Endereco.Complemento", Model.Pessoa.Endereco.Complemento, new { @maxlength = "50", @class = "text" })%>
				</div>
			</div>
		</fieldset>
	<% } %>
</div>
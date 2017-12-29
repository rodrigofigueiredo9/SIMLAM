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

			<% Html.RenderPartial("FisicaFiscalizacao"); %>
	<% } %>
</div>